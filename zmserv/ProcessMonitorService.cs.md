## 文件作用

zmserv 工程唯一的业务代码，也是整个样本的「惩罚执行端」。

它编译成一个 Windows 服务（`OutputType=WinExe`、`StartupObject=zmserv.ProcessMonitorService`、`ServiceName="Service1"`），
常驻后台监视「机房管理助手」的两个进程，一旦发现它们都不在了并且持续 80 秒，就**强制关机**。

对逆向的意义：这是「机房管理助手」用来防止学生结束程序/卸载助手的最后一道保险。
主程序（`jfglzsn`）负责界面与策略，本服务负责**执法**——即使学生把主程序结束掉，机器也会被关。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ProcessMonitorService`（`ServiceBase` 派生） | 服务宿主。构造函数写一个废路径常量并初始化组件；`Main` 把服务交给 SCM；两个计时器构成「先等 30 秒，再每 5 秒查一次」的监视循环；累计 16 次缺席即关机 |
| 原生导入 `RtlAdjustPrivilege`（ntdll） | 取 `SeShutdownPrivilege`（特权号 19），让进程有权限关机 |
| 原生导入 `NtShutdownSystem`（ntdll） | 绕过 Win32 关机流程直接内核级关机（1=重启，2=关机） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `_VisitorStrategy` | `监视计时器` | 5 秒周期的 `System.Timers.Timer`，主程序起来后才创建 |
| `_ProducerStrategy` | `启动等待计时器` | 30 秒周期的 Timer，启动阶段的等待器 |
| `attrStrategy`（const 5000） | `监视间隔毫秒` | 未被引用（构造 Timer 时写死 5000.0） |
| `_SingletonStrategy`（const 30000） | `启动等待毫秒` | 未被引用（写死 30000.0） |
| `_OrderStrategy`（long） | `连续缺席次数` | 满 16 触发关机 |
| `_AttributeStrategy`（string） | `受监视进程名` | 当日随机名，`Process.GetProcessesByName` 用它查进程 |
| `m_MessageStrategy`（string） | `受监视进程文件名` | 进程名 + ".exe"，赋值后从未读取 |
| `m_PredicateStrategy`（const 19） | `关机特权号` | `SeShutdownPrivilege`；调用处写死 19，常量没用上 |
| `m_ManagerStrategy`（const 0） | `当前线程标志` | `RtlAdjustPrivilege` 第 3 参数 0 = 当前线程 |
| `prototypeStrategy`（const 1） | `启用特权标志` | `RtlAdjustPrivilege` 第 2 参数 1 = 启用 |
| `parameterStrategy`（const 2） | `关机动作码` | `NtShutdownSystem(2)` = 关机 |
| `_StubStrategy` | `默认安装目录` | `"C:\windows\jf"`，构造时赋值，之后从未读取（模板/壳残留） |
| `_EventStrategy` | `组件容器` | 设计器用的 `IContainer` |
| `LogoutParams()` | `InitializeComponent()` | 设计器初始化：建容器 + 设 `ServiceName` |
| `CalcParams()` | `启动等待到期()` | 30 秒计时器回调 |
| `DefineParams()` | `监视周期检查()` | 5 秒计时器回调，核心判定 |
| `ForceShutdown()` / `ForceRestart()` | 同名保留 | 名字本来就是语义化的，未改名 |
| `text` / `text2` | `废弃进程名` / `进程名` | 第一版 4 字母名随即被覆盖；第二版才是真正生效的随机名 |
| `num` / `num2`…`num5` | `月日乘积` / `余7` `余9` `余5` `余3` | 月×日 及其模 7/9/5/3 |
| `num6` / `num7` | `随机数` / `位数` | 随机数逐位取模，位数控制循环 5 次 |

## 关键方法与流程

**`ProcessMonitorService()` 构造函数**
1. `默认安装目录 = "C:\windows\jf"`（死值，之后没人读）；
2. 调 `InitializeComponent()`：`new Container()` + `ServiceName = "Service1"`。

**`OnStart(string[])`（整段在 `checked` 上下文，VB 默认溢出检查）**
1. `月日乘积 = Month(Now) * Day(Now)`；
2. 取 `余7 = 月日乘积 % 7`、`余9 = %9`、`余5 = %5`、`余3 = %3`；
3. 按 `月日乘积 % 2` 分两支，用 `Chr(97+n)`/`Chr(98+n)`/`Chr(101+n)`/`Chr(99+n)` 拼出一版 4 字母进程名 → 立即被下一步覆盖（原版就这么写，属废代码）；
4. `VBMath.Randomize(月日乘积)` 播种，取 `Round(Rnd() * 100000 * 3 + 1)` 作为随机数；
5. `do…while` 循环 5 次：`Chr(随机数 % 10 + 105)` 拼到字符串**前面**，随机数 `/= 10`；
   → 结果是一个 5 个字母、每个字母都在 `'i'..'r'` 区间的进程名；
6. `受监视进程名 = 该名字`，`受监视进程文件名 = 名字 + ".exe"`；
7. `连续缺席次数 = 0`；
8. `new Timer(30000.0)`、`AutoReset = true`、`Elapsed += 启动等待到期`、`Start()`。

**`启动等待到期(sender, e)`（30 秒一次）**
- 若 `GetProcessesByName("jfglzsn")` 有结果（主程序在跑）→ 建 5 秒计时器 `监视计时器`（挂 `监视周期检查`）并 `Start()`，然后 `Dispose()` 并置空 30 秒计时器；
- 否则什么都不做（不关机），等 30 秒后下一跳。

**`监视周期检查(sender, e)`（5 秒一次，核心）**
1. 条件：**当日随机名进程不存在** 且 **`jfglzsn` 不存在**；
2. 成立 → `连续缺席次数 = checked(连续缺席次数 + 1)`；
   - 达到 `>= 16` → `ForceShutdown()`（NtShutdownSystem），随后 `Process.Start("shutdown", "/s /f /t 0")` 再补一枪，`return`；
3. 不成立（任一进程在）→ `连续缺席次数 = 0`。

**`ForceShutdown()` / `ForceRestart()`**
- `RtlAdjustPrivilege(19, 1, 0, ref 原状态)` → 取关机特权；
- `NtShutdownSystem(2)`（关机）/ `NtShutdownSystem(1)`（重启）。
- `ForceRestart` 在本工程内**没有调用点**，但作者留了这条路。

**`OnStop()`**：两个计时器判空 → `Stop()` + `Dispose()`。
**`Dispose(bool)`**：`try { 释放组件容器 } finally { base.Dispose(disposing) }`。
**`Main()`**：`[MTAThread]`，`ServiceBase.Run(new ServiceBase[]{ new ProcessMonitorService() })`。

## 恶意行为 / 敏感操作

1. **强制关机（核心恶意行为）**：`ntdll!RtlAdjustPrivilege` + `ntdll!NtShutdownSystem` 直接走内核关机，
   绕开「有程序阻止关机」的常规保护；紧接着 `shutdown /s /f /t 0` 兜底（`/f` 强关应用、`/t 0` 立即执行）。
2. **反规避监视**：受监视进程名**按日期变化**（`月 × 日` 决定），静态规则/任务管理器里按名字找进程会失效；
   同时硬编码 `jfglzsn`，两个都消失才算「助手已被结束」。
3. **以系统服务身份常驻**：`ServiceName="Service1"`，伪装成普通服务；
   程序集元数据伪装成微软产品（`Company=Microsoft`、`AssemblyTitle=service`、`Copyright © Microsoft 2025`）。
4. **反分析**：`[assembly: SuppressIldasm]` 禁 ILDasm；配合 IntelliLock 壳（见 `Internal/AdvisorStrategy.cs.md`）。
5. **无用户交互**：全程无提示、无倒计时、不给保存机会；`app.manifest` 是 `asInvoker`，靠服务本身已有的 SYSTEM 权限关机。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 服务名 | `Service1` |
| 进程名（写死） | `jfglzsn` |
| 进程名（当日生成） | 5 个字符，字符集 `'i'..'r'`；由 `Randomize(Month*Day)` + `Rnd()*100000*3+1` 决定 |
| 关机命令 | `shutdown` + 参数 `/s /f /t 0` |
| 原生函数 | `ntdll.dll!RtlAdjustPrivilege`、`ntdll.dll!NtShutdownSystem` |
| 数字常量 | 特权号 `19`；`RtlAdjustPrivilege` 参数 `1`、`0`；`NtShutdownSystem` 参数 `2`（关机）/ `1`（重启）；计时器 `30000.0`、`5000.0`；阈值 `16`；随机数系数 `100000.0`、`3.0`、`1.0`；`Chr` 基数 `97/98/99/101`、`105` |
| 残留路径 | `C:\windows\jf`（构造时赋值，未使用） |
| 程序集标识 | `AssemblyVersion/FileVersion 1.0.0.0`、`Guid 73204acb-20a4-433c-93f1-cd274c4ed922`、`AssemblyTitle/Product "service"`、`Company "Microsoft"` |
| 无注册表 / 无 URL | 本文件不碰注册表、不发网络请求 |

### 注意（容易被误判的点）

- `启动等待到期` 里「主程序没起来就什么都不做」是有意的：服务刚开机启动时主程序还没登录完成，
  不能一上来就关机，所以设计成「等 30 秒 → 看到 `jfglzsn` 才开始计时」。分析时不要把它当成 bug 改掉。
- `连续缺席次数` 的累加在原版是 `ref long ptr = ref _OrderStrategy; _OrderStrategy = checked(ptr + 1)`，
  那是 dnSpy 对 `checked` 上下文里 `+=` 的展开，已还原成 `checked(连续缺席次数 + 1)`，语义等价。
