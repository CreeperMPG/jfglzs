# ProcessMonitorService.cs — Windows 防卸载监控服务（看门狗：杀进程即强制关机）

## 文件作用
本文件是恶意服务的**核心逻辑**，即 Windows 服务本体（ServiceBase 子类）。服务名为 `Service1`，安装目录为 `C:\windows\jf`。它运行一个"防卸载看门狗"：平时每 30 秒扫描一次 `jfglzsn` 进程是否存在；一旦发现该进程（恶意软件本体）在运行，就切换为每 5 秒一次的紧轮询，同时监控 `jfglzsn` 与一个**按日期算法生成的随机进程名**（影子进程）。若两者同时消失累计 16 次（约 80 秒），判定"本体被杀/被卸载"，立即调用 `NtShutdownSystem(2)` 强制关机，并用 `shutdown /s /f /t 0` 兜底，使受害者无法轻易清除恶意软件（杀毒/杀进程 → 整机断电）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `ProcessMonitorService` | 防卸载监控服务本体（服务入口 + 双定时器看门狗 + 强制关机） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `m_BridgeReg` | 安装目录 | 常量 `"C:\\windows\\jf"`（本体/影子进程存放目录） |
| `m_FilterReg` (IContainer) | 组件容器 | 设计器生成的组件容器，仅用于 Dispose 链 |
| `m_ReaderReg` | 影子进程名 | 按日期算法生成的 5 字符进程名（无 .exe），被监控对象之一 |
| `_PolicyReg` | 影子进程文件全名 | `m_ReaderReg + ".exe"` |
| `_SetterReg` (Timer) | 30 秒扫描定时器 | 轮询 jfglzsn 是否存在（AutoReset=true，周期 30000ms） |
| `authenticationReg` (Timer) | 5 秒紧轮询定时器 | 发现 jfglzsn 后启动，监控双进程存活（周期 5000ms） |
| `descriptorReg` (long) | 连续消失计数 | 双进程同时不存在的连续轮询次数，≥16 触发关机 |
| `m_AdvisorReg` | 紧轮询周期常量 | 5000ms |
| `m_FacadeReg` | 扫描周期常量 | 30000ms |
| `pageReg` | SeShutdownPrivilege 编号 | 19（关机特权） |
| `m_ThreadReg` | 启用特权标志 | 1（enable=TRUE） |
| `_AdapterReg` | 当前线程标志 | 0（只调整当前线程特权） |
| `_InterpreterReg` | NtShutdownSystem 动作码 | 2（关机 / 1=重启） |

## 关键方法与流程

### 构造函数（`ProcessMonitorService`）
- **职责**：初始化安装目录字段并调用 `FindList`。
- **流程**：`m_BridgeReg = "C:\\windows\\jf"` → `FindList()`。
- 注意：`m_BridgeReg` 在其余代码中未被引用，属于安装目录痕迹/待用常量。

### 初始化服务（原名 `FindList`）
- **职责**：服务初始化。
- **流程**：1. `m_FilterReg = new Container()`；2. `ServiceName = "Service1"`。
- **关键调用/API**：无（纯属性设置）。

### 服务入口（`Main`，MTAThread）
- **职责**：服务进程入口点。
- **流程**：创建 `ProcessMonitorService` 实例数组 → `ServiceBase.Run(services)` 交给 SCM 运行。
- 与 csproj 的 `StartupObject=zmserv.ProcessMonitorService` 一致。

### 服务启动（`OnStart`）— 生成随机进程名 + 启动 30 秒扫描
- **职责**：服务启动时生成"每日随机"影子进程名，并启动 30 秒扫描定时器。
- **流程**：
  1. `num = 当前月 × 当前日`（如 8 月 29 日 → 232）；
  2. 计算 `num2=num%7, num3=num%9, num4=num%5, num5=num%3`；
  3. 若 `num%2==0`：`text = chr(97+num2)+chr(98+num3)+chr(101+num4)+chr(99+num5)`；否则换序 `chr(97+num3)+chr(98+num2)+chr(101+num5)+chr(99+num4)`（得到 4 字符名字，**随后被覆盖，属于混淆残留/死代码**）；
  4. `m_ReaderReg = text`、`_PolicyReg = text+".exe"`（立即被下一步覆盖）；
  5. `VBMath.Randomize(num)` 播种 → `num6 = round(Rnd()*100000*3 + 1)`（1~300001，**按日期完全确定**）；
  6. 循环 5 次取 `num6` 末位数字 `d`，映射为字符 `chr(d+105)`（数字 0~9 → 字母 `i`~`r`），**逆序**拼接成 5 字符名字（若位数不足，高位补 `'i'`）；
  7. 最终 `m_ReaderReg = text2`（5 字符名）、`_PolicyReg = text2 + ".exe"`；
  8. `descriptorReg = 0` 清零计数；
  9. `_SetterReg = new Timer(30000)`，AutoReset=true，`Elapsed += PrintList`，启动 —— 进入 30 秒扫描循环。
- **关键调用/API**：`DateAndTime.Now/Month/Day`、`VBMath.Randomize/Rnd`、`Strings.Chr`、`System.Timers.Timer`。
- **说明**：名字虽"随机"但**每日期确定**（播种值=月×日），攻击者可预先计算当天影子进程名用于查杀；字符集仅 `i`~`r` 共 10 个字母，全枚举仅 10⁵ 组合。

### 30 秒扫描回调（原名 `PrintList`）— 探测 jfglzsn 并武装看门狗
- **职责**：每 30 秒检查主进程 `jfglzsn` 是否存活，存活则切换紧轮询。
- **流程**：1. `Process.GetProcessesByName("jfglzsn")` 非空？→ 2. 创建 `authenticationReg = new Timer(5000)`（AutoReset=true，`Elapsed += TestList`）并启动；3. `_SetterReg.Dispose()`、置 null（停止 30 秒扫描）。
- **关键调用/API**：`Process.GetProcessesByName`、`Timer.Dispose`。
- 若 `jfglzsn` 一直不出现，则服务只空转扫描，看门狗不武装。

### 5 秒紧轮询回调（原名 `TestList`）— 计数消失并触发强制关机
- **职责**：每 5 秒检测影子进程与 `jfglzsn` 是否同时消失，累计 16 次即强制关机。
- **流程**：
  1. 若 `GetProcessesByName(m_ReaderReg)` 为空 **且** `GetProcessesByName("jfglzsn")` 为空（两进程都不在）→ `descriptorReg += 1`；
  2. 若 `descriptorReg >= 16` → 调用 `ForceShutdown()`（提权 + NtShutdownSystem(2)），随后 `Process.Start("shutdown", "/s /f /t 0")` 作为兜底；
  3. 否则（任一进程存活）→ `descriptorReg = 0` 重置计数。
- **关键调用/API**：`Process.GetProcessesByName(...).Any()`（Linq）、`ForceShutdown`、`Process.Start("shutdown", "/s /f /t 0")`。
- **行为解读**：16 次 × 5s ≈ 80 秒内两个进程全部消失即认为"被清除"→ 直接断电，阻止受害者继续清理。若只是短暂消失会因计数重置而免触发（容错）。

### 强制关机（`ForceShutdown`，static）
- **职责**：提权并立即强制关机。
- **流程**：1. `RtlAdjustPrivilege(19, 1, 0, ref num)` 开启当前线程 SeShutdownPrivilege；2. `NtShutdownSystem(2)` 强制关机（不保存、不弹窗）。
- **关键调用/API**：`ntdll.dll!RtlAdjustPrivilege`、`ntdll.dll!NtShutdownSystem`。

### 强制重启（`ForceRestart`，static）
- **职责**：提权并强制重启（**当前流程未被调用**，属预留/死代码）。
- **流程**：`RtlAdjustPrivilege(19,1,0)` + `NtShutdownSystem(1)`（1=重启）。

### 服务停止（`OnStop`）
- **职责**：清理定时器。
- **流程**：分别 Stop+Dispose `authenticationReg` 与 `_SetterReg`（非空时）。
- 注意：仅停定时器，**不卸载**服务注册，SCM 停止后进程退出。

### 释放（`Dispose`）
- **职责**：释放组件容器 `m_FilterReg` 后调用基类 Dispose。

## 恶意行为 / 敏感操作
- **防卸载看门狗**：监控 `jfglzsn` 与按日期生成的影子进程，两者被清除 → 约 80 秒后强制整机关机。
- **绕过权限直接关机**：通过 ntdll `RtlAdjustPrivilege(SeShutdownPrivilege)` 自提权，`NtShutdownSystem(2)` 无 UI 强制断电。
- **双保险兜底**：再调用 `shutdown /s /f /t 0` 确保关机执行。
- **隐蔽性设计**：监控对象之一为"每日随机"进程名（伪随机、日期确定性），增加清除/取证难度；服务名伪装为泛化的 `Service1`。
- 预留 `ForceRestart`（NtShutdownSystem(1)）可在后续版本/配置中启用重启破坏。

## IOC / 硬编码
- 服务名：`Service1`
- 安装/运行目录：`C:\windows\jf`
- 被守护进程名：`jfglzsn`（本体）
- 影子进程名：5 字符，字符集 `i`–`r`（数字 0~9 → chr(d+105)），按月×日播种的 VB Rnd 生成，**每日期确定**；文件全名 `<名字>.exe`（预期位于 `C:\windows\jf\`）
- 定时参数：扫描周期 30000ms；紧轮询周期 5000ms；消失阈值 16 次（≈80s）
- 原生 API：`ntdll.dll!RtlAdjustPrivilege`、`ntdll.dll!NtShutdownSystem`
- 特权：`SeShutdownPrivilege`（编号 19）
- 关机命令：`shutdown /s /f /t 0`
- NtShutdownSystem 动作码：2=关机、1=重启
- 程序集入口：`zmserv.ProcessMonitorService`（StartupObject）
