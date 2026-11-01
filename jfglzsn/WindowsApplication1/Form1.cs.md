## 文件作用

**主控窗体 + 整个「机房管理助手」的策略执行引擎**。窗体本身不画界面（无边框、最小化、不进任务栏），只在 `Load` 里建好 14 个定时器 + 一套 WMI 进程监听，然后靠定时器分阶段、分主题地轮询并处置违规行为。

一句话概括：**这是学生端的一体化管控/惩罚程序**——自启、守护服务、猎杀进程、封网、改系统策略、监控极域电子教室、锁屏、关机。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `Form1 : Form` | 主控窗体（隐藏的后台宿主）+ 全部策略逻辑 |
| `窗体加载初始化`（原 `ListCallback`） | **初始化总入口**：随机进程名、读策略开关、填 4 张判据表、装 WMI 监听、探测极域、记录快照 |
| `主循环_分阶段启动`（原 `CalcCallback`） | `Timer_main`（6 s）心跳，按计数 1/4/5/6 分阶段打开其余监视器 |
| `服务守护与自更新`（原 `CustomizeCallback`） | `Timer1`（6 s）：注册/守护 `zmserv` 服务、补 `przs.exe`、失败 5 次则重启自身并自杀 |
| `检查USB存储禁用`（原 `LogoutCallback`） | `Timer_usb`（5 s）：`usbstor\Start` 被改回 3 就强制改 4 并警告 |
| `电子教室进程监控`（原 `InterruptCallback`） | `Timer_dzjs`（16 s）：按 `监控模式` 点检/状态机处置；缺失 5 次**强制关机** |
| `网络连接监控`（原 `PopCallback`） | `Timer_wangluo`（17 s）：断网/IP 变更检测 + 分级警告 |
| `新进程拦截`（原 `ForgotCallback`） | **WMI 进程创建回调**：四张黑名单匹配 + Kill + 删文件 + 弹窗 |
| `防火墙监控`（原 `PrintCallback`） | `Timer_fhq`（13 s）：发现防火墙开着就 `netsh` 关掉并改注册表 |
| `口令与自启动项监控`（原 `ResolveCallback`） | `Timer_qdx`（3 s）：口令摘要与两个 Run 项被改就强制恢复 |
| `极域监控`（原 `PublishCallback`） | `Timer_jiyu`（15 s）：极域 ChannelId/AutoLogin 参数 + 两个服务 |
| `定时刷新hosts`（原 `WriteCallback`） | `Timer_updatehosts`（30 s）：再拉一次云端黑名单、备份 hosts |
| `定时显示警告窗`（原 `MoveCallback`） | `Timer2`（1 s）：反复确认 Form2 在场 |
| `定时显示锁屏`（原 `ConcatCallback`） | `Timer4`（1 s）：反复确认 Form4 在场 |
| `显示违规软件警告`（原 `InstantiateCallback`） | 填 Form3 文案并 `Show()`（由 WMI 线程 `BeginInvoke` 投递） |
| `显示杀毒软件警告`（原 `ResetCallback`） | 填 Form2 文案 + `Show()` + 开 `Timer2` |
| `下载并写入hosts黑名单`（原 `ReflectCallback`） | **封网核心**：下载名单 → 写 hosts → 加黑洞路由 → flushdns |
| `进程是否在运行`（原 `CreateCallback`） | 进程存在性判定（全类几十处调用） |
| `GetWiredNetworkIP` | 取有线网卡 IPv4（排除 virtual 网卡与回环） |
| `CheckProcessStatus` | 电子教室进程状态机（Running/Suspended/NotResponding/NotFound） |
| `是否有线程被挂起`（原 `OrderCallback`） | 判断进程是否有线程处于挂起等待 |
| `子串出现次数`（原 `PrepareCallback`） | 统计子串出现次数（本工程无调用点） |
| `Ping主机可达`（原 `CancelCallback`） | Ping 探活（无调用点） |
| `IsPortOpen` | TCP 端口探活（实际在用的探网手段） |
| `强制关机`（原 `ForceShutdown`） | `RtlAdjustPrivilege(19)` + `NtShutdownSystem(2)` |
| `窗体关闭清理`（原 `PatchCallback`） | `FormClosing`：停/释放 WMI 监视器 |
| `初始化组件`（原 `RunCallback`） | 建 14 个定时器并设间隔 |
| 12 个 P/Invoke | user32 / kernel32 / ntdll 的系统级能力 |
| `ProcessStatus`（嵌套 enum） | 进程状态四值 |

## 反混淆命名映射

### 方法

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ListCallback` | `窗体加载初始化` | `Form1_Load`，全系统初始化总入口 |
| `CalcCallback` | `主循环_分阶段启动` | 6 s 心跳的分阶段启动器 |
| `CustomizeCallback` | `服务守护与自更新` | 服务注册 + 自重启 |
| `LogoutCallback` | `检查USB存储禁用` | U 盘管控 |
| `InterruptCallback` | `电子教室进程监控` | 进程存活/挂起处置 |
| `PopCallback` | `网络连接监控` | 断网/IP 变更告警 |
| `ForgotCallback` | `新进程拦截` | WMI `__InstanceCreationEvent` 回调 |
| `PrintCallback` | `防火墙监控` | 强关防火墙 |
| `ResolveCallback` | `口令与自启动项监控` | 恢复口令与 Run 项 |
| `PublishCallback` | `极域监控` | 极域参数/服务 |
| `WriteCallback` | `定时刷新hosts` | hosts 黑名单刷新 |
| `MoveCallback` | `定时显示警告窗` | 反复弹 Form2 |
| `ConcatCallback` | `定时显示锁屏` | 反复弹 Form4 |
| `InstantiateCallback` | `显示违规软件警告` | 填 Form3 |
| `ResetCallback` | `显示杀毒软件警告` | 填 Form2 |
| `ReflectCallback` | `下载并写入hosts黑名单` | 封网核心 |
| `CreateCallback` | `进程是否在运行` | `GetProcessesByName(name).Length > 0` |
| `PrepareCallback` | `子串出现次数` | 无调用点 |
| `OrderCallback` | `是否有线程被挂起` | 遍历 `ProcessThread` 状态 |
| `CancelCallback` | `Ping主机可达` | 无调用点 |
| `ForceShutdown` | `强制关机` | ntdll 强关 |
| `PatchCallback` | `窗体关闭清理` | 停 WMI 监视器 |
| `RunCallback` | `初始化组件` | 设计器 `InitializeComponent` |
| `GetWiredNetworkIP` / `IsPortOpen` / `CheckProcessStatus` | 未改名 | 名字本身已可读 |
| P/Invoke 参数 `intptr_0`/`int_0`/`string_0`/`byte_0`/`bool_0`/`long_0`/`long_1` | `窗口句柄`/`命令`/`根路径`/…（按 Win32 语义） | 便于对照 MSDN |

### 字段

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `_IdentifierEvent` | `主循环计数` | 1/4/5/6 触发不同启动阶段 |
| `m_PolicyEvent` | `网线告警中` | 断网已弹 Form3 |
| `m_OrderEvent` | `进程缺失标志` | 电子教室缺失已弹 Form3 |
| `iteratorEvent` | `防火墙告警中` | 防火墙告警已弹 Form3 |
| `_ParameterEvent` | `极域参数告警中` | |
| `specificationEvent` | `极域服务告警中` | |
| `customerEvent` | `口令告警中` | |
| `m_ResolverEvent` | `IP告警中` | |
| `facadeEvent` | `杀毒检查开关` | 注册表 `shadu_jianche`；`off` 放行火绒 |
| `m_ItemEvent` | `U盘检查开关` | 注册表 `usb_jianche` |
| `_InterpreterEvent` | `命令提示符检查开关` | 注册表 `cmd_jianche` |
| `_ClientEvent` | `电子教室配置串` | 注册表 `dzjs` |
| `_InstanceEvent` | `电子教室进程表` | 配置串按逗号拆分 |
| `_ClassEvent` | `进程名黑名单` | 定长 101，1~28 有效 |
| `infoEvent` | `关键词黑名单` | 定长 501，1~78 有效（云端可扩） |
| `valueEvent` | `关键词表上界` | 初始 78 |
| `dicEvent` | `备份程序路径` | 电子教室路径 + `"2"` |
| `_ProcessEvent` | `监控模式` | 0 / 1 / 2 |
| `m_FilterEvent` | `防火墙告警计数` | |
| `m_FieldEvent` | `网络告警计数` | 断网/IP 变更累计 |
| `_ParamsEvent` | `进程缺失计数` | ≥5 触发强制关机 |
| `writerEvent` | `进程挂起计数` | >8 弹 Form3，>10 弹 Form2 |
| `methodEvent` | `口令篡改计数` | >25 升级 Form2 |
| `_ParserEvent` | `极域参数篡改计数` | >5 升级 Form2 |
| `_BridgeEvent` | `极域服务告警计数` | >5 升级 Form2 |
| `m_RuleEvent` | `极域通道号快照` | `ChannelId` 基线 |
| `m_RoleEvent` | `极域自登录快照` | `AutoLogin` 基线 |
| `m_DecoratorEvent` | `极域注册表分支` | 0/1(Wow6432Node)/2(32 位) |
| `m_DefinitionEvent` | `极域服务受管` | 两服务都在才为真 |
| `_StateEvent` | `口令摘要快照` | `HKCU\Software\n` 基线 |
| `_TemplateEvent` | `prozs自启动快照` | `Run\prozs` 基线 |
| `_ImporterEvent` | `jfglzsn自启动快照` | `Run\jfglzsn` 基线 |
| `m_AlgoEvent` | `伪装安装目录` | `C:\Program Files (x86)\<4 字符>` |
| `m_AttributeEvent` | `tdfilefilter服务` | `ServiceController` |
| `m_GetterEvent` | `tdnetfilter服务` | `ServiceController` |
| `expressionEvent` | `原始IP` | 首次合法 IP |
| `m_MappingEvent` | `关键词匹配下标` | 第二轮匹配结果 |
| `m_InterceptorEvent` | `解压拦截开关` | 注册表 `gaowei`，默认 2 |
| `initializerEvent` | `载荷源目录` | `C:\windows\jf` |
| `wrapperEvent` | `hosts已写入` | |
| `pageEvent` | `hosts路径` | `C:\Windows\System32\drivers\etc\hosts` |
| `_TestEvent` | `进程名匹配下标` | 第一轮匹配结果 |
| `_WatcherEvent` | `新进程监视器` | `ManagementEventWatcher` |
| `composerEvent` | `触发进程名` | WMI 事件里的进程名 |
| `m_StrategyEvent` | `电子教室进程名` | 不含扩展名 |
| `_UtilsEvent` | `电子教室程序路径` | 配置串第一项 |
| `databaseEvent` | `云端域名` | 注册表 `zhuye` |
| `m_PredicateEvent` | `拉起计数` | ≥5 触发自重启 |
| `_MapEvent` | `组件容器` | `IContainer` |
| `_AnnotationEvent`/`_ProxyEvent`/`_ReponseEvent`/`connectionEvent`/`m_ListEvent`/`_ObserverEvent`/`codeEvent`/`m_IndexerEvent`/`_BaseEvent`/`candidateEvent`/`prototypeEvent`/`m_AccountEvent`/`collectionEvent`/`_ParamEvent`/`regEvent`/`m_InvocationEvent`/`_RequestEvent`/`_SerializerEvent`/`m_GlobalEvent`/`m_ExporterEvent`/`m_SchemaEvent` | **保留原名** | 代码里**从未被读写**，无法推断语义；硬造名字反而误导，按 AGENTS.md 沿用原名并加注"未在代码中引用" |
| `recordEvent` | 保留原名（=19） | 关机特权号 `SeShutdownPrivilege` |
| `_PropertyEvent`/`m_ExceptionEvent`/`m_MessageEvent`/`m_RefEvent`/`m_AttrEvent`/`authenticationEvent`/`_PoolEvent`/`dispatcherEvent`/`m_IssuerEvent`/`m_ProccesorEvent`/`m_BroadcasterEvent`/`configurationEvent`/`m_TagEvent` | 保留原名 | 全部未被引用；`authenticationEvent = 2` 正好是 `NtShutdownSystem` 的关机动作码 |

## 关键方法与流程

### `窗体加载初始化`（原 `ListCallback`）— 初始化九步

1. `Hide()` —— 隐藏窗体（常驻后台）。
2. **随机进程名**：`日期种子 = 月 × 日`；`余7/余9/余5/余3 = 种子 % 7/9/5/3`；按种子奇偶走两种字母排列拼 4 字母名 → 写 `ThreadEvent.m_AdapterEvent` / `_AdvisorEvent`。
3. **再随机一次**：`VBMath.Randomize(种子)`；`随机数 = Round(Rnd()*100000*3+1)`；循环 5 次取 `随机数 % 10 + 105` 拼成 5 字符名（字符集 `'i'~'r'`）→ 覆盖 `m_AdapterEvent`/`_AdvisorEvent`；`伪装安装目录 = "C:\Program Files (x86)\" + Mid(名, 2, 4)`。
4. 读四项策略开关：`dzjs`、`usb_jianche`、`shadu_jianche`、`cmd_jianche`。
5. `电子教室进程表 = Split(dzjs, ",")`；`监控模式`：首项为 `other` 或空 → 0；含 `\` → 2；否则 → 1；同时得 `电子教室程序路径` 与 `电子教室进程名`。
6. 填 `进程名黑名单[1..28]`（28 项）与 `关键词黑名单[1..78]`（78 项），`关键词表上界 = 78`。
7. **立刻杀一次 `taskmgr`**（`GetProcessesByName("taskmgr")[0].Kill()`）。
8. 注册 WMI：`WqlEventQuery("__InstanceCreationEvent", TimeSpan(0,0,1), "TargetInstance ISA 'Win32_Process'")` → `ManagementEventWatcher` → `EventArrived += 新进程拦截` → `Start()`。
9. 极域探测：读 `Wow6432Node\...\Student\ChannelId/AutoLogin`；`ChannelId == 0` 则改读 32 位路径，非 0 则 `极域注册表分支 = 2` + 开 `Timer_jiyu`；否则分支 = 1 + 开定时器。
10. 服务初始态：状态为 0（不存在）→ `极域服务受管 = false`；否则若 `Stopped` 就 `Start()`，`Sleep(1000)` 后都在 `Running` 才置 `true`。
11. 记录三份基线：`口令摘要快照`、`prozs自启动快照`、`jfglzsn自启动快照`（都先查 64 位视图，空则查 `Wow6432Node`）。
12. 读 `gaowei`（默认 2）与 `zhuye`（云端域名）。
13. 杀软检测：`QQPCTray | 360sd | 360Tray` 任一在跑 → 弹「加载一个控件不能成功！…」；否则 `zhuye` 非空 → `Process.Start(<程序目录>\zy.exe)`。

> 原版整段 `On Error Resume Next`，这里**逐条** try/catch：任何一步失败（尤其是"杀 taskmgr 失败"）都不能阻断后面的 WMI 监听安装。

### `主循环_分阶段启动`（原 `CalcCallback`）— 启动序列

每次心跳 `主循环计数++`，并**每次**先尝试 `下载并写入hosts黑名单("http://" + 云端域名 + "/hmd_net.txt", "on")`（仅当尚未接管且有域名）。

| 计数 | 动作 |
| --- | --- |
| 1 | 三个快照都非空才开 `Timer_qdx`；`usb_jianche == "on"` 才开 `Timer_usb` |
| 4 | 开 `Timer_fhq` + `Timer1` |
| 5 | 开 `Timer_wangluo`；电子教室 `NotFound` → 缺文件就从 `<路径>2` 复制回来，然后 `Process.Start(路径)` |
| 6 | 再查一次进程（缺失则开 `Timer_dzjs`）；`hosts` 备份为 `hosts2`；需要接管则开 `Timer_updatehosts`；**关掉 `Timer_main`**（初始化结束） |

### `服务守护与自更新`（原 `CustomizeCallback`）— `Timer1`

**第一段（服务）**：`进程是否在运行("zmserv")` 为假 →
1. 缺 `zmserv.exe` 且存在 `C:\windows\jf\zmserv.dat` → `File.Copy` 释放；
2. `写注册表值(DisableCMD, 0)`；
3. `sc create zmserv binPath= <程序目录>\zmserv.exe start= auto`（隐藏 + 等待）；
4. `net start zmserv`。

**第二段（自身副本）**：`进程是否在运行(m_AdapterEvent)` 为假 →
1. `拉起计数++`；
2. 缺 `przs.exe` 且存在 `C:\windows\jf\przs.dat` → `File.Copy` 释放；
3. `Process.Start(<程序目录>\przs.exe)`；
4. 若 `拉起计数 >= 5`：`DisableCMD=0` → `net stop zmserv` → `Sleep(3000)` → `sc create` → `net start` → `Process.Start(<程序目录>\jfglzsn.exe)` → **`ProjectData.EndApp()`（旧进程自杀）**。

### `检查USB存储禁用`（原 `LogoutCallback`）— `Timer_usb`

`Start` 值 = 3（允许 U 盘）→ 把 `CurrentControlSet` + `ControlSet001~003` 四处 `usbstor\Start` 全写 4 → Form2 文案「你修改注册表企图使用U盘！请重新启动电脑。」→ `Show()` → `Timer2.Enabled = true`。

### `电子教室进程监控`（原 `InterruptCallback`）— `Timer_dzjs`

- 模式 0：关掉自己（不监控）。
- 模式 1：遍历 `电子教室进程表`，去掉 `.exe` 后逐个 `进程是否在运行`，缺一个就弹 Form2「电子教室进程未找到或本地连接IP地址错误！请重新启动电脑。」+ `Timer2`。
- 模式 2：`CheckProcessStatus` 状态机
  - `Running` → `进程挂起计数 = 0`；若 `进程缺失标志` 则 `Form3.Close()`；标志复位。
  - `Suspended` / `NotResponding` → `进程挂起计数++`。
  - `NotFound` → `进程缺失标志 = true`；`进程缺失计数++`；缺文件则从 `<路径>2` 复制；`Process.Start(路径)`。
  - 收尾：`进程缺失计数 >= 5` → `强制关机()` + `shutdown /s /f /t 0`；`进程挂起计数 > 10` → Form2「电子教室程序被暂停！」+ `Timer2`；`> 8` → Form3 同文案。

### `网络连接监控`（原 `PopCallback`）— `Timer_wangluo`

1. `当前IP = GetWiredNetworkIP()`；`原始IP` 为空且当前 IP 合法（非 169.254.*）→ 记录。
2. IP 为空（断网）：`网络告警计数++`；>5 → `Form3.Close()` + Form2「你拔了网线或禁用了网络连接！」「请重新启动电脑！」+ `Timer2`；否则 → `网线告警中 = true` + Form3「你拔了网线或禁用了网络连接！」「请马上恢复网络，否则会全屏锁定！」。
3. IP 变了：`网络告警计数++`；>5 → Form2「你修改了IP地址！原地址为:<原始IP>」；否则 → `IP告警中 = true` + Form3 同文案 + 「请马上恢复网络，否则会全屏锁定！」。
4. IP 正常：`IP告警中 | 网线告警中` 时关 Form3，两个标志复位。

### `新进程拦截`（原 `ForgotCallback`）— WMI 回调（**核心**）

1. 从 `e.NewEvent["TargetInstance"]` 取 `ProcessId` / `Name`；`Process.GetProcessById` 拿 `MainModule.FileName`；空则返回。
2. `描述文本 = 版本信息.ProductName + FileDescription`。
3. **第一轮（进程名）**：起点由 `杀毒检查开关 == "off"` 决定（off → 从 2 起，放行 `HipsTray.exe`；否则从 1 起）；
   循环直到遇到空项：把 `"|" + UCase(进程名)` 与 `UCase(表项)` 做 `InStr` 子串匹配，命中即停。
4. **第二轮（文件属性）**：起点由 `命令提示符检查开关 == "off"` 决定（off → 从 4 起）；
   把 `描述文本` 与表项做 `InStr`（区分大小写），命中即停。
5. 第一轮命中 → 按 `进程名匹配下标` 分段：
   | 下标 | 分类 | 处置 |
   | --- | --- | --- |
   | 1~10 | 杀毒软件（360/QQ管家/火绒托盘等） | 只 `BeginInvoke(显示杀毒软件警告)` |
   | 11~16 | 进程工具 / 任务管理器 / 虚拟桌面 / 扫雷 / 纸牌 | `Kill()` + `BeginInvoke(显示违规软件警告)` |
   | 17~21 | 解压软件（WinRAR/7z/Bandizip/360压缩） | `解压拦截开关 != 1` 才 `Kill()` + 弹窗 |
   | ≥22 | 极域/掌控课堂/课堂狂欢器/吾爱破解等破解工具 | `Kill()` + 弹窗 + `WaitForExit(1000)` + **`FileSystem.Kill(程序路径)` 删文件** |
6. 第二轮命中 → 按 `关键词匹配下标` 分段：
   | 下标 | 处置 |
   | --- | --- |
   | ≤4 | `Sleep(2000)` → `Kill()` |
   | 5、6 | 弹 Form3 → `Sleep(2000)` → `Kill()` |
   | 7~50 | `Kill()`；≠43 时弹 Form3 |
   | >50 | `Kill()` + 弹窗 + `WaitForExit(1000)` + **删文件** |

### `防火墙监控`（原 `PrintCallback`）— `Timer_fhq`

读 `StandardProfile` / `PublicProfile` 的 `EnableFirewall`，用 `OrObject(CompareObjectEqual(v, "1"))` 判断（**拿字符串 "1" 比**，靠 VB 弱类型把 DWORD 1 认成相等）。开着 →
1. `防火墙告警计数++`；
2. ≤5 次：`DisableCMD=0` → `netsh advfirewall set allprofiles state off` → 两处 `EnableFirewall=0` → `防火墙告警中 = true` → Form3「你启用了防火墙，危害电子教室的运行！」「现在助手强制关闭防火墙！」；
3. >5 次：Form3 收掉，改 Form2「请重新启动电脑！」+ `Timer2`；
4. 防火墙已关 → 收 Form3、复位标志。

### `口令与自启动项监控`（原 `ResolveCallback`）— `Timer_qdx`

1. 读 `HKCU\Software\n`，与 `口令摘要快照` 不等 → `口令篡改计数++` → **写回快照值** → Form3「你修改了助手的密码！」「现在强制恢复密码！」→ `口令告警中 = true`；>25 次 → Form2 + `Timer2`。
2. 相等 → 收 Form3、复位标志。
3. 读 `Run\prozs` / `Run\jfglzsn`（空则查 `Wow6432Node`），与快照不一致 → 无条件写回 `<程序目录>\przs.exe` / `<程序目录>\jfglzsn.exe`。

### `极域监控`（原 `PublishCallback`）— `Timer_jiyu`

1. 按 `极域注册表分支`（1 = `Wow6432Node`，2 = 32 位）读 `ChannelId` / `AutoLogin`；与快照不等 → `极域参数篡改计数++` → Form3「你修改了极域电子教室的参数！」「请马上恢复，否则会锁屏！」→ 标志置真；相等则收窗复位。
2. `极域参数篡改计数 > 5` → Form2 + `Timer2`。
3. 若 `极域服务受管`：`Refresh()` 两个服务 →
   - 任一状态为 0（不存在）→ `极域服务告警计数++` → Form3「极域电子教室的2个重要服务不存在！」；
   - 都在 `Running` → 收窗复位；
   - 否则 → `Start()` 两个服务 → `极域服务告警计数++` → Form3「…2个重要服务停止！」。
4. `极域服务告警计数 > 5` → Form2 + `Timer2`。

### `定时刷新hosts`（原 `WriteCallback`）— `Timer_updatehosts`

无条件再拉一次名单；`hosts已写入` 为真时把 hosts 备份成 `hosts2` 并**关掉本定时器**。此方法**没有** `On Error` 脚手架，异常会直接冒泡（原版如此）。

### `下载并写入hosts黑名单`（原 `ReflectCallback`）— **封网核心**

1. `IsPortOpen("www.baidu.com", 443, 2000)` 不通 → `hosts已写入 = false`，返回。
2. `WebClient.DownloadFile(名单地址, "c:\1.txt")`。
3. `File.ReadAllLines`；第一行不含 `www.` → 视为无效名单，返回。
4. 扫描三个中文标记 `视频网站：` / `产品名：` / `IP地址：`，各记录"标记前一行/后一行"（未初始化时按 0 处理）。
5. `SetAttributes(hosts, Normal)` **连写两次**；读现有 hosts，收集已被写成 `127.0.0.1 <域名>` 的域名进 `HashSet`。
6. `StringBuilder(原hosts).AppendLine()`；**段 1**：`0 .. 视频标记前行` 逐行归一化成域名（去 `http://`/`https://`/路径、转小写），不在集合里就 `AppendLine("127.0.0.1 " + 域名)`。
7. **段 2**（仅当 `模式 == "off"`）：`视频标记后行 .. 产品标记前行` 同样处理。
8. **段 3**：`产品标记后行 .. IP标记前行` 的行**追加进关键词黑名单**（服务端远程扩表）。
9. `删除注册表键(HKLM, "SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\route.exe")` —— 清掉学生的 `route.exe` 劫持。
10. **段 4**：`IP标记后行 .. 末行` 逐条 `route add <IP> mask 255.255.255.255 0.0.0.0 metric 1`（**黑洞路由**）。
11. `File.WriteAllText(hosts, 构造器)`；`hosts已写入 = true`；hosts 设 `ReadOnly | Hidden | System`。
12. `DisableCMD = 0`；`ipconfig /flushdns`。

### 其余小方法

| 方法 | 要点 |
| --- | --- |
| `进程是否在运行(名)` | `Process.GetProcessesByName(名).Length > 0` |
| `GetWiredNetworkIP()` | 遍历网卡：描述不含 `virtual` 且 `Up`；在其单播地址里找 `InterNetwork` 且非回环的第一个，直接返回（跳过枚举器 Dispose，与原版一致）；异常返回 `""` |
| `CheckProcessStatus(名)` | 无进程 → `NotFound`；有则找映像路径与 `电子教室程序路径` 匹配的（`UCase` 比较）；找不到 → `NotFound`；`Responding` 假 → `NotResponding`；有线程挂起 → `Suspended`；否则 `Running`；异常 → `NotResponding` |
| `是否有线程被挂起(进程)` | 任一 `ProcessThread` 处于 `Wait` + `WaitReason.Suspended` → `true`（`finally` 里释放手工枚举器） |
| `子串出现次数(子串, 源串)` | `InStr` 循环计数；**计数器原版未初始化（=0）**，此处显式写 0 |
| `Ping主机可达(主机, 超时)` | 无网络 → `false`；`Ping.Send().Status == Success` → `true`；异常 → `false` |
| `IsPortOpen(主机, 端口, 超时=2000)` | `TcpClient.BeginConnect` + `WaitOne` + `EndConnect`，返回 `Connected`；异常 → `false` |
| `强制关机()` | `RtlAdjustPrivilege(19, 1, 0, ref 0)` → `NtShutdownSystem(2)` |
| `窗体关闭清理(sender, e)` | `新进程监视器?.Stop(); ?.Dispose();` —— **没有** `e.Cancel = true` |
| `显示违规软件警告()` | Form3 文案（3 种分支）+ 联系方式串 + `Show()` |
| `显示杀毒软件警告()` | Form2 文案 + 联系方式串 + `Show()` + `Timer2 = true` |
| `初始化组件()` | 建 14 个定时器并设间隔；只有 `Timer_main.Enabled = true` |

## 恶意行为 / 敏感操作

### 持久化

| 手段 | 落地 |
| --- | --- |
| Windows 服务 | `sc create zmserv binPath= <程序目录>\zmserv.exe start= auto` + `net start zmserv` |
| 载荷释放 | 从 `C:\windows\jf\zmserv.dat` → `zmserv.exe`，`C:\windows\jf\przs.dat` → `przs.exe` |
| 自启动项 | `HKLM\...\CurrentVersion\Run\prozs` = `<程序目录>\przs.exe`；`Run\jfglzsn` = `<程序目录>\jfglzsn.exe`（且每 3 秒校验、被改就写回） |
| 自重启守护 | 副本消失 5 次 → 重启服务 + `Process.Start(jfglzsn.exe)` + 旧进程 `EndApp()` |
| 随机进程名 | 每天按「月×日」派生 5 字符名（字符集 `i~r`），规避按名字查杀 |

### 进程猎杀 / 删除文件

1. **WMI 实时拦截**：`__InstanceCreationEvent`（1 秒轮询）监听每一个新进程；
2. **四张判据表**：进程名 28 项 + 文件属性关键词 78 项（云端还可扩容）；
3. 命中后：`Kill()` / `Kill()` + `WaitForExit(1000)` + `FileSystem.Kill(路径)`（**直接删程序文件**）；
4. 加载时**立刻杀一次 `taskmgr`**；对"名字全由 `f~o` 组成"的进程也杀（Form2/Form4 退出流程）。
5. 覆盖对象：任务管理器、注册表编辑器、MMC、PowerShell/cmd、各类进程工具（PC Hunter/IceSword/Procmon/Process Hacker…）、虚拟桌面、扫雷/纸牌、解压软件、极域破解工具（极域杀手/KillControl/FuckMythware…）、杀毒软件（360/QQ管家/火绒托盘，`shadu_jianche=off` 时放行火绒）。

### 系统策略篡改

| 目标 | 写什么 | 效果 |
| --- | --- | --- |
| `HKCU\Software\Policies\Microsoft\Windows\System` | `DisableCMD = 0` / `1` | 放开/禁用命令提示符 |
| `HKLM\SYSTEM\CurrentControlSet\Services\usbstor`（+ ControlSet001~003） | `Start = 4` | **禁用 USB 存储** |
| `HKLM\...\FirewallPolicy\{Standard,Public}Profile` | `EnableFirewall = 0` + `netsh advfirewall set allprofiles state off` | **强制关闭防火墙** |
| `HKLM\...\IFEO\route.exe` | `DeleteSubKey` | 清掉路由劫持 |

### 封网（最"有杀伤力"的一项）

1. 从云端 `http://<zhuye>/hmd_net.txt` 下载名单；
2. 写 hosts：`127.0.0.1 <域名>`（视频网站等），并把 hosts 设 `只读+隐藏+系统`；
3. `route add <IP> mask 255.255.255.255 0.0.0.0 metric 1` 加**黑洞路由**；
4. `ipconfig /flushdns` 立即生效；
5. 名单里的「产品名」段会追加进进程黑名单——**服务端可远程扩充猎杀范围**。

### 惩罚链（三级）

| 级别 | 窗体 | 手段 |
| --- | --- | --- |
| 1 | Form3 | 全屏文字警告 |
| 2 | Form2 | 全屏遮罩 + 密码门 + 鼠标锁 `{0,0,800,100}` + 5 次后蜂鸣 + 「重启电脑」按钮 |
| 3 | Form4 | 锁屏 + 同上 + 退出时保持 `DisableCMD` 禁用 |

`Timer2` / `Timer4` 每秒复弹，保证窗口关不掉。

### 破坏性操作

| 操作 | 触发条件 |
| --- | --- |
| `NtShutdownSystem(2)`（内核关机）+ `shutdown /s /f /t 0` | 电子教室进程连续 5 次救不回来 |
| `shutdown /r /f /t 0`（强制重启） | 用户在 Form2/Form4 点「重启电脑」 |
| `FileSystem.Kill(程序路径)`（删文件） | 命中黑名单 ≥22 或关键词 >50 |

### 反分析

`SuppressIldasm`（AssemblyInfo）+ 随机进程名 + 隐藏窗体 + `My.Settings` 落盘痕迹。

## IOC / 硬编码

### 注册表键

| 键 | 值名 | 含义 |
| --- | --- | --- |
| `HKCU\Software` | `n` | 退出小助手口令摘要（`SHA256(口令+"bfdshgs")`） |
| `HKCU\Software\jfglzs` | `n2` | 锁屏窗备用口令摘要 |
| `HKCU\Software\jfglzs` | `dzjs` | 电子教室进程配置串（逗号分隔，可带路径） |
| `HKCU\Software\jfglzs` | `usb_jianche` | U 盘检查开关（`on` 开） |
| `HKCU\Software\jfglzs` | `shadu_jianche` | 杀软检查开关（`off` 放行火绒） |
| `HKCU\Software\jfglzs` | `cmd_jianche` | CMD 检查开关 |
| `HKCU\Software\jfglzs` | `gaowei` | 解压软件拦截开关（默认 2；=1 不杀） |
| `HKCU\Software\jfglzs` | `zhuye` | 云端域名（拼 `http://<域名>/hmd_net.txt`） |
| `HKCU\Software\Policies\Microsoft\Windows\System` | `DisableCMD` | CMD 策略 |
| `HKLM\SYSTEM\CurrentControlSet\Services\usbstor`（+`ControlSet001~003`） | `Start` | USB 存储启动类型（3 ↔ 4） |
| `HKLM\SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\StandardProfile` | `EnableFirewall` | 防火墙（标准配置） |
| `HKLM\SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\PublicProfile` | `EnableFirewall` | 防火墙（公用配置） |
| `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN`（+`WOW6432NODE`） | `prozs` | 自启动 |
| `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN`（+`WOW6432NODE`） | `jfglzsn` | 自启动 |
| `HKLM\SOFTWARE\Wow6432Node\TopDomain\e-Learning Class\Student` | `ChannelId` / `AutoLogin` | 极域参数（64 位视图） |
| `HKLM\SOFTWARE\TopDomain\e-Learning Class\Student` | `ChannelId` / `AutoLogin` | 极域参数（32 位视图） |
| `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\route.exe` | — | 被删除的劫持键 |

### 文件路径

| 路径 | 含义 |
| --- | --- |
| `C:\windows\jf` | 载荷源目录（`zmserv.dat` / `przs.dat`） |
| `C:\windows\jf\zmserv.dat` | 服务程序载荷 |
| `C:\windows\jf\przs.dat` | 守护副本载荷 |
| `<程序目录>\zmserv.exe` | 释放后的服务程序 |
| `<程序目录>\przs.exe` | 释放后的守护副本 |
| `<程序目录>\jfglzsn.exe` | 主程序（自重启用） |
| `<程序目录>\zy.exe` | 配置了云端域名时启动的附加程序 |
| `C:\Windows\System32\drivers\etc\hosts` | 被封禁/接管的目标 |
| `hosts2`（同目录） | hosts 备份 |
| `<电子教室路径>2` | 电子教室程序备份（缺失时用来恢复） |
| `C:\Program Files (x86)\<4 字符>` | 伪装安装目录 |
| `C:\1.txt` | 云端黑名单临时下载文件 |

### 网络

| 项 | 值 |
| --- | --- |
| 云端黑名单 URL | `http://<zhuye>/hmd_net.txt` |
| 探活目标 | `www.baidu.com:443`（2000 ms 超时） |
| WMI 查询 | `__InstanceCreationEvent`，`TargetInstance ISA 'Win32_Process'`，间隔 1 秒 |

### 服务 / 进程名

| 名称 | 含义 |
| --- | --- |
| `zmserv` | 自建 Windows 服务名 |
| `tdfilefilter` / `tdnetfilter` | 极域两个服务（被监控、会强启） |
| `taskmgr` | 加载时立刻杀一次 |
| `QQPCTray` / `360sd` / `360Tray` | 杀软检测（命中则弹提示不启动 `zy.exe`） |
| `HipsTray.exe` | 火绒托盘（`shadu_jianche=off` 时放行） |

### 命令行

| 命令 | 用途 |
| --- | --- |
| `sc create zmserv binPath= <路径> start= auto` | 注册服务 |
| `net start zmserv` / `net stop zmserv` | 启/停服务 |
| `netsh advfirewall set allprofiles state off` | 关防火墙 |
| `route add  <IP> mask 255.255.255.255 0.0.0.0 metric 1` | 黑洞路由（注意 `add` 后两个空格） |
| `ipconfig /flushdns` | 刷 DNS |
| `shutdown /s /f /t 0` / `shutdown /r /f /t 0` | 关机 / 重启 |

### 判据表（黑名单）

**进程名黑名单（`进程名黑名单[1..28]`，28 项）**

`|HipsTray.exe`、`|备用`、`|备用`、`|360tray.exe`、`|NetFlow.exe`、`|QQPCTray.exe`、`|360taskmgr`、`|QQPCNetFlow.exe`、`|360netman.exe`、`|iu杀毒`、`进程`、`任务管理`、`虚拟桌面`、`|Minesweeper`、`|winmine.exe`、`|solitaire.exe`、`|winrarrr.exe`、`|wzmainnn.exe`、`|bandizippp.exe`、`|7zFMMM.exe`、`|360zippp.exe`、`掌控课堂`、`夕颜若雪工具箱`、`极域`、`课堂狂欢器`、`电子教室`、`吾爱破解`、`|NoTopDomain`

**关键词黑名单（`关键词黑名单[1..78]`，78 项）**

`Windows 命令处理程序`、`Windows Command Processor`、`Windows PowerShell`、`注册表编辑器`、`Microsoft 管理控制台`、`任务管理器`、`PC Hunter`、`IceSword`、`Wsyscheck`、`SnipeSword`、`Process Explorer`、`狡兔二窟`、`Process-X`、`Process Master`、`PrcView`、`Process Viewer`、`PowerTool`、`Windows Kernel Explorer`、`FTCleaner`、`Process Lasso`、`process manager`、`Process Hacker`、`ProcessOVER`、`Dexpot`、`Deskman`、`iDesktop`、`YtMDesk`、`DexpotProPortable`、`Desktops`、`Wise Desktop`、`VDesktop`、`MagicDesktop`、`MultiDesk`、`小宝虚拟桌面`、`系统配置实用程序`、`资源和性能监视器`、`Special Setting Program`、`瑞星防火墙`、`火绒剑`、`超级进程王`、`OpenArk`、`Procmon`、`GameBar`、`Registry Workshop`、`屏幕键盘`、`暂 时 备 用`×5、`极域杀手`、`极域墓地制造器`、`极域杀死`、`翘课`、`夕颜若雪工具箱`、`学生机房Hack助手`、`再见极域`、`KillControl`、`去除控制`、`MsgFlood`、`极域工具包`、`Astrum For Online Classroom`、`JiYu Trainer`、`掌控课堂`、`FuckMythware`、`Hello Teacher`、`JIYUPLUS`、`极域Tool`、`微机课救星`、`极域X`、`UnMythware`、`易语言程序`（= `ThreadEvent._EventSetter`）、`BITS管理实用程序`、`DzjsTools`、`SkiesKiller`、`教室终结者`（= `ThreadEvent.callbackSetter`）、`MultiDeskTop应用程序`、`极域电子教室克星`

### 界面文案（惩罚窗口）

`你运行的软件违规！如有误判，请老师拍照与作者QQ联系。`、`注：电脑死机时，不能用任务管理器，可同时按ctrl+alt+del三个键，点 注销 ，电脑不会自动还原。`、`本电脑禁止使用解压软件，如果确有需要，请老师来退出小助手。`、`你运行了杀毒类的软件，危害电子教室的运行！如果你必须要用火绒杀毒软件，要设置：'允许火绒杀毒'。如有误判，请老师拍照与作者QQ联系。`、`电子教室程序被暂停！`、`你拔了网线或禁用了网络连接！`、`请马上恢复网络，否则会全屏锁定！`、`请重新启动电脑！`、`你修改了IP地址！原地址为:`、`你启用了防火墙，危害电子教室的运行！`、`现在助手强制关闭防火墙！`、`你修改了助手的密码！`、`现在强制恢复密码！`、`你修改了极域电子教室的参数！`、`请马上恢复，否则会锁屏！`、`极域电子教室的2个重要服务不存在！`、`极域电子教室的2个重要服务停止！`、`电子教室进程未找到或本地连接IP地址错误！请重新启动电脑。`、`你修改注册表企图使用U盘！请重新启动电脑。`、`加载一个控件不能成功！检查是否安装了安全卫士、电脑管家、360等杀毒软件，它们会阻止运行！（如果必须要安装，只安装火绒杀毒软件)。技巧：如果你的安装软件列表中并没有杀毒软件，可能是360安全卫士卸载不干净，你可以再次安装360安全卫士然后再卸载它。`

**联系方式串模板**：`w4yw4y3tq3yszz<进程名>mmfnfxhghrts`（Form3）、`gyhrw4yw4y3tq3yszz<进程名>mmfnfxhghrts`（Form2）

### P/Invoke 与常数值

| DLL | 函数 | 用途 |
| --- | --- | --- |
| `user32.dll` | `ShowWindow` | 显示/隐藏窗口（未使用） |
| `kernel32` | `GetDriveTypeA` | 驱动器类型（未使用） |
| `ntdll.dll` | `NtSuspendProcess` / `NtResumeProcess` | 挂起/恢复进程（未使用） |
| `ntdll.dll` | `RtlAdjustPrivilege` | 提权（19 = SeShutdownPrivilege） |
| `ntdll.dll` | `NtShutdownSystem(2)` | **内核关机** |
| `user32` | `SetForegroundWindow` / `GetForegroundWindow` / `GetWindowTextA` / `GetWindowThreadProcessId` | 前台窗口操作（均未使用） |
| `user32` | `keybd_event` / `BlockInput` | 键鼠屏蔽能力（未使用） |

定时器间隔：`Timer1` 6000、`Timer2` 1000、`Timer_main` 6000、`Timer_usb` 5000、`Timer_dzjs` 16000、`Timer_wangluo` 17000、`Timer_jcm` 5000（空转）、`Timer_fhq` 13000、`Timer_qdx` 3000、`Timer4` 1000、`Timer_jiyu` 15000、`Timer3` 1000（空转）、`Timer_updatehosts` 30000、`Timer_route` 11000（空转）。

其它常量：`关键词表上界` 初始 78、`进程名黑名单` 长度 101、`关键词黑名单` 长度 501、`电子教室进程表` 长度 6、`recordEvent` 19、`authenticationEvent` 2、`WM_DEVICECHANGE` 537、`DBT_DEVICEARRIVAL` 32768、`DBT_USERDEFINED` 65535。
