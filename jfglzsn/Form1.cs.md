# Form1.cs — 恶意软件主控窗体（隐藏主窗体，14 定时器调度全部恶意功能）

## 文件作用
这是整个恶意软件家族的**核心主程序窗体**。窗体本身无边框、最小化、不显示在任务栏（`FormBorderStyle=None`、`ShowInTaskbar=false`、`WindowState=Minimized`），构造时创建 **14 个 `System.Windows.Forms.Timer`**，Load 事件中完成配置读取、黑名单初始化、WMI 进程监视器启动、极域电子教室服务自检；随后各定时器分阶段接管：安装 `zmserv` 服务、禁用 U 盘、守护"电子教室"进程、监控网络/IP、强制关闭防火墙、恢复被篡改的注册表、下载黑名单改写 hosts、封堵 IP 路由、检测并击杀杀毒软件/进程工具/对抗工具进程。窗体关闭事件里释放 WMI 监视器。程序退出/解锁密码逻辑位于 Form2/Form4。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `Form1`（partial，逻辑部分） | 恶意主控：14 定时器 + WMI 进程监控 + hosts/路由/注册表/服务操作 |
| `Form1.ProcessStatus` 枚举 | 目标进程状态：Running / Suspended / NotResponding / NotFound |

## 14 个定时器反混淆映射（核心）
| 混淆名(定时器) | 间隔 ms | 事件处理器(混淆名) | 真实功能（反混淆名） |
|---------|------|-------------------|----------------------|
| `Timer_main` | 3000 | `FlushTemplate` | **主调度/启动序列**：每 3 秒一次，按计数分阶段启用其它定时器 |
| `Timer1` | 6000 | `InterruptTemplate` | **安装/重启 zmserv 服务、启动同伴进程 przs** |
| `Timer2` | 1000 | `DisableTemplate` | **弹全屏锁定窗 Form2**（强制显示） |
| `Timer_usb` | 5000 | `NewTemplate` | **U 盘禁用检测**（usbstor Start=4） |
| `Timer_dzjs` | 16000 | `ResetTemplate` | **电子教室进程守护**（重启被暂停/被杀进程，5 次后强制关机） |
| `Timer_wangluo` | 17000 | `ResolveTemplate` | **网络/IP 监控**（拔网线/改 IP 警告，超 5 次锁定） |
| `Timer_jcm` | 5000 | （无 Tick 处理器） | **死定时器**：属性 setter 不订阅事件，混淆/开发残留 |
| `Timer_fhq` | 13000 | `RegisterTemplate` | **防火墙监控**（检测到开启就强制关闭） |
| `Timer_qdx` | 3000 | `DeleteTemplate` | **注册表保护**（密码/自启动项被改则强制恢复） |
| `Timer4` | 1000 | `StartTemplate` | **弹出二级密码锁定窗 Form4** |
| `Timer_jiyu` | 15000 | `InvokeTemplate` | **极域电子教室参数与服务监控**（tdfilefilter/tdnetfilter） |
| `Timer3` | 1000 | （无 Tick 处理器） | **死定时器**：同上，无事件 |
| `Timer_updatehosts` | 10000 | `AwakeTemplate` | **hosts 更新**：重新下载 hmd_net.txt 并备份 hosts→hosts2 |
| `Timer_route` | 11000 | `CompareTemplate` | **IFEO route.exe 键保护**（Debugger 值被改则警告恢复） |

## 关键方法与流程

### 初始化定时器与窗体（原名 `ExcludeTemplate`，构造器调用）
- **职责**：创建全部 14 个定时器并挂接事件，设置窗体隐藏属性。
- **流程**：1. new 全部 Timer 挂到同一 Container；2. 逐个设置 Interval（3s/6s/1s/5s/16s/17s/5s/13s/3s/1s/15s/1s/10s/11s）；3. 仅 `Timer_main` 初始 `Enabled=true`；4. 窗体无边框、最小化、不在任务栏。
- **关键调用/API**：`System.Windows.Forms.Timer`、属性 setter 内部 `Tick += 处理器`。

### Form1_Load（原名 `LogoutTemplate`）
- **职责**：隐藏窗体；生成每日随机进程名；读取全部配置；初始化进程/产品黑名单表；击杀 taskmgr；启动 WMI 进程监视器；自检极域服务；读取密码与自启动注册表项。
- **流程**：
  1. `Hide()` 隐藏主窗体。
  2. 用 `当月×当日` 做种子生成随机 5 位进程名（字符范围 105~114，即 `i`~`r`，例如"ipqjf"），存入 `ModelVisitor._TestsVisitor`、`m_ConsumerVisitor`（=名字+.exe）；并算 `_PolicyVisitor = "C:\Program Files (x86)\" + 名字第2~5位`（该路径后续未使用，疑似残留）。
  3. 读取 `HKCU\Software\jfglzs` 下：`dzjs`（电子教室进程列表，逗号分隔）、`usb_jianche`（U盘检测开关）、`shadu_jianche`（杀毒检测开关）、`cmd_jianche`（cmd检测开关）、`zhuye`（服务器主机名）。
  4. 解析 `dzjs` 列表：`other/空`→模式0（无守护）；含 `\` 绝对路径→模式2（路径型，用 CheckProcessStatus 守护）；否则→模式1（进程名型）。保存待守护进程路径 `_ValVisitor` 与进程名 `m_RulesVisitor`。
  5. 填充进程名黑名单 `schemaConfig[]`：1~3"备用"；4~10 杀软进程：`360safe.exe`、`HipsTray.exe`（火绒）、`QQPCTray.exe`（腾讯电脑管家）、`360taskmgr`、`QQPCNetFlow.exe`、`360netman.exe`、`iu杀毒`；11~13 关键字"进程/任务管理/虚拟桌面"；14~21 游戏/压缩软件：`Minesweeper`、`winmine.exe`、`solitaire.exe`、`winrarrr.exe`、`wzmainnn.exe`、`bandizippp.exe`、`7zFMMM.exe`、`360zippp.exe`；22~28 对抗工具关键字："掌控课堂"、"夕颜若雪工具箱"、"极域"、"课堂狂欢器"、"电子教室"、"吾爱破解"、`NoTopDomain`。
  6. 填充产品描述黑名单 `authenticationConfig[]`：1~3 cmd/PowerShell（"Windows 命令处理程序"、"Windows Command Processor"、"Windows PowerShell"）；4 regedit；5 mmc；6 任务管理器；7~10 进程工具（PC Hunter、IceSword、Wsyscheck、SnipeSword）；11~20（Process Explorer、狡兔二窟、Process-X、Process Master、PrcView、Process Viewer、PowerTool、Windows Kernel Explorer、FTCleaner、Process Lasso）；21~50（process manager、Process Hacker、ProcessOVER、Dexpot、Deskman、iDesktop、YtMDesk、Desktops、Wise Desktop、VDesktop、MagicDesktop、MultiDesk、小宝虚拟桌面、系统配置实用程序、资源和性能监视器、Special Setting Program、瑞星防火墙、火绒剑、超级进程王、OpenArk、Procmon、GameBar、Registry Workshop、屏幕键盘、备用×5）；51~78 极域对抗工具（极域杀手、极域墓地制造器、极域杀死、翘课、夕颜若雪工具箱、学生机房Hack助手、再见极域、KillControl、去除控制、MsgFlood、极域工具包、Astrum For Online Classroom、JiYu Trainer、掌控课堂、FuckMythware、Hello Teacher、JIYUPLUS、极域Tool、微机课救星、极域X、UnMythware、易语言程序、BITS管理实用程序、DzjsTools、SkiesKiller、教室终结者、MultiDeskTop应用程序、极域电子教室克星）；`procConfig=78`（后续服务端下发产品名从 79 起）。
  7. `Process.GetProcessesByName("taskmgr")[0].Kill()` 击杀任务管理器。
  8. 启动 WMI 监视：`WqlEventQuery("__InstanceCreationEvent", 1s, "TargetInstance ISA 'Win32_Process'")` + `ManagementEventWatcher.Start()`，事件处理器 `FillTemplate`。
  9. 读极域注册表 `HKLM\SOFTWARE\Wow6432Node\TopDomain\e-Learning Class\Student` 的 `ChannelId/AutoLogin`；非零则 `expressionVisitor=1`（64 位注册表）并启用 `Timer_jiyu`；否则读 32 位路径 `HKLM\SOFTWARE\TopDomain\...`，非零则 `expressionVisitor=2` 并启用 `Timer_jiyu`。
  10. 自检服务 `tdfilefilter`/`tdnetfilter`（merchantVisitor/m_WorkerVisitor）：不存在→`m_PropertyVisitor=false`；Stopped→`Start()` 拉起；Running→`m_PropertyVisitor=true`。
  11. 读取主密码 `HKCU\Software\n`、自启动 `HKLM\...\RUN\prozs`、`jfglzsn`（含 WOW6432Node 回退）、`HKCU\Software\jfglzs\gaowei`（高危开关，默认 2）。
- **关键调用/API**：`Registry`、`ManagementEventWatcher`、`ServiceController`、`Process.Kill`、`Strings.Chr/Rnd`（VB 随机）。
- **说明**：控制流被还原为上述顺序；IL 状态机（`num2 = N` + `@switch`）仅用于 VB 异常处理，实际逻辑如上。

### Timer_main_Tick·主调度（原名 `FlushTemplate`）
- **职责**：按 tick 次数分阶段：更新 hosts 黑名单 → 备份 hosts2 → 按 `advisorConfig` 计数（2/7/10/12）启用对应功能定时器。
- **流程**：1. 计数+1；2. 读 `HKCU\Software\jfglzs\zhuye` 得到服务器地址；3. `CreateTemplate("http://<zhuye>/hmd_net.txt", "on")` 下载黑名单并写 hosts；4. hosts 复制为 `hosts2` 备份（先去掉只读属性）；5. 若 hosts 尚未被接管（`_ContextVisitor=false`）→启用 `Timer_updatehosts`；6. 计数==2：若 dzjs/usb 相关注册表项非空→启用 `Timer_qdx`；若 `usb_jianche=="on"`→启用 `Timer_usb`；计数==7：启用 `Timer_fhq`+`Timer1`；计数==10：启用 `Timer_wangluo`，且若电子教室进程 NotFound→从 `xxx.exe2` 副本恢复并启动；计数==12：若进程存在→启用 `Timer_dzjs`，随后 `Timer_main.Enabled=false`（启动序列结束）。
- **关键调用/API**：`File.Copy/SetAttributes`、`Process.Start`、`ModelVisitor.DeleteConfig`。

### Timer1_Tick·安装 zmserv 服务与启动同伴（原名 `InterruptTemplate`）
- **职责**：把 `C:\windows\jf\zmserv.dat` 释放为启动目录 `zmserv.exe`，用 `sc create` 注册自启服务并启动；再启动随机名同伴 `przs.exe`（从 `C:\windows\jf\przs.dat` 释放）；5 次后重建服务、自重启、退出。
- **流程**：1. 若 `zmserv` 进程已在跑→跳过；2. 否则若无 `zmserv.exe` 且存在 `C:\windows\jf\zmserv.dat`→复制为 `zmserv.exe`；3. `DisableCMD=0`（放开 cmd 限制）；4. `sc create zmserv binPath= <启动目录>\zmserv.exe start= auto`；5. `net start zmserv`；6. 检查随机名进程（`ModelVisitor._TestsVisitor`）是否在跑；不在→计数+1，释放 `przs.dat`→`przs.exe` 并 `Process.Start`；7. 计数≥5：`net stop zmserv`→Sleep 3s→重新 `sc create`+`net start`→`Process.Start(启动目录\jfglzsn.exe)`（自重启）→`EndApp()` 结束当前进程。
- **关键调用/API**：`Interaction.Shell("sc create ...")`、`Interaction.Shell("net start/stop ...")`、`File.Copy`、`Process.Start`、`ProjectData.EndApp`。

### Timer_usb_Tick·U 盘禁用（原名 `NewTemplate`）
- **职责**：检查 `usbstor` 驱动启动类型，若被改为手动(3)则改回禁用(4)，并弹全屏锁定窗。
- **流程**：1. 读 `HKLM\SYSTEM\CurrentControlSet\Services\usbstor\Start`；2. ==3 → 把 `Start` 写为 4（禁用），同时写 `ControlSet001/002/003` 三个 ControlSet；3. Form2.shuoming 显示"你修改注册表企图使用U盘！请重新启动电脑。"并 `Show()`；4. 启用 `Timer2`（持续强制显示 Form2）。
- **关键调用/API**：`Registry.LocalMachine`、`TokenizerConfig.FindTemplate.Form2`。

### Timer_dzjs_Tick·电子教室进程守护（原名 `ResetTemplate`）
- **职责**：按模式守护电子教室进程：模式0→关闭本定时器；模式1→逐个检查进程名列表（`_EventConfig`），进程不在则报"电子教室进程未找到或本地连接IP地址错误！"并锁屏；模式2→用 `CheckProcessStatus` 监控指定 exe：Running→复位计数并关警告窗；Suspended/NotResponding→暂停计数+1（≥8 次警告"电子教室程序被暂停！"，≥10 次锁屏）；NotFound→恢复 `xxx.exe2` 副本并启动，计数+1，**≥5 次直接 `ForceShutdown()`（提权强制关机）+ `shutdown /s /f /t 0`**。
- **关键调用/API**：`Process.Start`、`File.Copy`、`Form1.ForceShutdown()`、`RtlAdjustPrivilege(19,1,0,...)`（启用关机特权）+ `NtShutdownSystem(2)`。

### Timer_wangluo_Tick·网络/IP 监控（原名 `ResolveTemplate`）
- **职责**：检测有线网卡 IP：无 IP（拔网线/禁网）→警告"你拔了网线或禁用了网络连接！请马上恢复网络，否则会全屏锁定！"；IP 改变→警告"你修改了IP地址！原地址为:xxx"；超过 5 次→关 Form3、Form2 显示"请重新启动电脑！"+ 启用 Timer2 锁屏。
- **关键调用/API**：`GetWiredNetworkIP()`（遍历非虚拟、Up 的网卡取 IPv4 非回环地址）、`IPAddress`、`OperationalStatus`。

### WMI 进程创建事件·违规进程查杀（原名 `FillTemplate`，`accountVisitor.EventArrived`）
- **职责**：每个新进程创建时：取进程名 + 主模块文件的产品名/文件描述，与两个黑名单比对，按分级策略杀进程/删文件/弹警告。
- **流程**：
  1. 从 `e.NewEvent["TargetInstance"]` 取 `ProcessId`/`Name`，`Process.GetProcessById` 取主模块路径与版本信息（`ProductName+FileDescription` 拼接为 `@string`）。
  2. `shadu_jianche=="off"` 时进程黑名单从索引 11 开始扫，否则从 1 开始；`cmd_jianche=="off"` 时产品描述黑名单从 4 开始，否则从 1。
  3. 进程名匹配 `schemaConfig`（`InStr("|"+大写进程名, 大写规则)`，如 `|360SAFE.EXE` 含 `360SAFE.EXE`）→ `flag=true`；产品描述匹配 `authenticationConfig`（`InStr(产品名+描述, 中文规则)`）→ `flag2=true`。
  4. 按 `m_ProccesorVisitor`（进程黑名单命中位置）分级：≤10（杀软/备用）→仅 `BeginInvoke(VisitTemplate)` 弹 Form2 警告"你运行了杀毒类、虚拟机类的软件！"；11~16（进程/任务管理/虚拟桌面/扫雷/纸牌）→`Kill()` + 弹 Form3 警告；17~21（解压软件 winrar/wzmain/bandizip/7zFMMM/360zip）→`gaowei!=1` 时 Kill + 警告（Form3 提示"本电脑禁止使用解压软件"）；>21（对抗工具 + 服务端下发产品名）→Kill + 警告 + `WaitForExit(1000)` + **删除 exe 文件**（`FileSystem.Kill`）。
  5. 按 `_BridgeVisitor`（产品描述黑名单命中位置）分级：≤3（cmd/powershell）→Sleep 2s + Kill；==4（regedit）→Sleep 2s + Kill；5/6（mmc/任务管理器）→先弹警告再 Sleep 2s + Kill；>50（含服务端下发的产品名）→Kill + 警告 + 删文件；==43（GameBar）→只 Kill 不警告；其余→Kill + 警告。
- **关键调用/API**：`ManagementEventWatcher`、`FileVersionInfo.GetVersionInfo`、`Process.Kill`、`FileSystem.Kill`（删文件）、`BeginInvoke`。

### Timer_fhq_Tick·防火墙关闭（原名 `RegisterTemplate`）
- **职责**：读标准配置文件/公共配置文件防火墙开关，检测到开启(值为"1")→`netsh advfirewall set allprofiles state off` 强制关闭，并把两处 `EnableFirewall` 写 0，同时 `DisableCMD=0`；超过 5 次→锁屏"请重新启动电脑！"。
- **关键调用/API**：`HKLM\SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\{StandardProfile,PublicProfile}\EnableFirewall`、`netsh advfirewall`。

### Timer_qdx_Tick·注册表保护（原名 `DeleteTemplate`）
- **职责**：监控主密码 `HKCU\Software\n` 与自启动项 `HKLM\...\RUN\{prozs,jfglzsn}`，被篡改立即恢复并警告；超 25 次锁屏。
- **流程**：1. 读 `HKCU\Software\n`，与内存中密码 `dicVisitor` 不同→计数+1、写回原密码、Form3 警告"你修改了助手的密码！现在强制恢复密码！"；2. 读 RUN 键 `prozs`/`jfglzsn`（含 WOW6432Node 回退），与内存值不符→重写为 `启动目录\przs.exe` 与 `启动目录\jfglzsn.exe`。
- **关键调用/API**：`Registry`、`ModelVisitor.AwakeConfig`。

### Timer_jiyu_Tick·极域配置与服务监控（原名 `InvokeTemplate`）
- **职责**：监控极域电子教室客户端注册表参数（`ChannelId`/`AutoLogin`，分 64/32 位两套路径）与两个核心服务。
- **流程**：1. 按 `expressionVisitor` 选 64 位（`SOFTWARE\Wow6432Node\TopDomain\e-Learning Class\Student`）或 32 位路径读参数；2. 与内存值不同→计数+1，Form3 警告"你修改了极域电子教室的参数！请马上恢复，否则会锁屏！"，超 5 次锁屏；3. 若极域服务监控开启（`m_PropertyVisitor`）：`merchantVisitor.Refresh()`/`m_WorkerVisitor.Refresh()`（tdfilefilter/tdnetfilter）——服务不存在→警告"极域电子教室的2个重要服务不存在！"；非 Running→`Start()` 拉起，警告"极域电子教室的2个重要服务停止！"，超 5 次锁屏。
- **关键调用/API**：`ServiceController`（`tdfilefilter`、`tdnetfilter`）、`Registry.LocalMachine`。

### Timer_updatehosts_Tick·hosts 更新（原名 `AwakeTemplate`）
- **职责**：周期性重新下载 `http://<zhuye>/hmd_net.txt` 并写 hosts；若 hosts 已接管则备份 hosts→hosts2 并停止本定时器；第 3 次后把间隔改为 60s（降频）。
- **关键调用/API**：`CreateTemplate`、`File.Copy`。

### Timer_route_Tick·IFEO 保护（原名 `CompareTemplate`）
- **职责**：检查 `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\route.exe\Debugger` 是否被改成非 `"null"`；被改→重新下载黑名单恢复（`CreateTemplate(...,"on")`），警告"你修改了小助手r设置！现在助手强制恢复！"，超 5 次锁屏。
- **关键调用/API**：`Registry`、IFEO `route.exe`。

### 更新 hosts 黑名单与路由封堵（原名 `CreateTemplate(string url, string mode)`）
- **职责**：从服务器下载 `hmd_net.txt` 黑名单，解析"视频网站：/产品名：/IP地址："三节，向 hosts 追加 `127.0.0.1 <域名>` 屏蔽视频/产品网站，把产品名追加进产品黑名单，用 `route add` 封堵指定 IP，最后用 IFEO 劫持 `route.exe` 防止学生删路由。
- **流程**：
  1. `IsPortOpen("www.baidu.com", 443, 2000)` 探测外网（TCP 443），失败→`_ContextVisitor=false` 返回（不接管 hosts）。
  2. `WebClient.DownloadFile(url, "c:\1.txt")` 下载；首行不含 `"www."`→视为无效，返回。
  3. 逐行扫描标记行：`"视频网站："`、`"产品名："`、`"IP地址："`，记录各自前后行号区间。
  4. 读现有 hosts，把已屏蔽域名（以 `127.0.0.1` 开头的行第 2 列）存入 HashSet 去重。
  5. 视频网站区间的每行 → 去 `http(s)://`、取主机名 → 若未屏蔽则追加 `127.0.0.1 <域名>`；`mode=="off"` 时产品名区间同样处理（本程序内只传 "on"，"off" 逻辑未实际使用）。
  6. 产品名区间内容追加进 `authenticationConfig[]`（从索引 79 起，即 procConfig+1），成为动态产品黑名单。
  7. 删除 IFEO `route.exe` 旧键；对 IP 地址区间的每个 IP：`route add <ip> mask 255.255.255.255 0.0.0.0 metric 1`（黑洞路由，屏蔽目标服务器）。
  8. 写回 IFEO：`route.exe\Debugger = "null"`（route.exe 被劫持到不存在的程序，学生无法 `route delete`）。
  9. `File.WriteAllText(hosts, ...)`；`_ContextVisitor=true`；hosts 设为 `ReadOnly|Hidden|System` 属性；`DisableCMD=0`；`ipconfig /flushdns` 刷新 DNS。
- **关键调用/API**：`WebClient.DownloadFile`、`TcpClient.BeginConnect`、`route add`、IFEO、`File.SetAttributes`、`ipconfig /flushdns`。
- **IOC**：临时文件 `c:\1.txt`；黑名单 URL `http://<zhuye>/hmd_net.txt`。

### 其它方法
- `MoveTemplate(string)` → **检查进程是否存在**：`Process.GetProcessesByName(name).Length > 0`。
- `GetWiredNetworkIP()` → **获取有线网卡 IPv4**：遍历非 virtual 描述、`OperationalStatus.Up` 的网卡，取第一个非回环 IPv4。
- `CheckProcessStatus(processName)` → **检查目标进程状态**：无进程→NotFound；按 `_ValVisitor` 全路径匹配；`Responding` 且线程被 `Suspended`→Suspended；`Responding`→Running；否则 NotResponding。
- `IncludeTemplate(process)` → **检测进程是否被挂起**：遍历线程，`ThreadState==Wait && WaitReason==Suspended` 即判定被暂停。
- `PrintTemplate()` → **显示违规警告(Form3)**：文案"你运行的软件违规！如有误判，请老师拍照与作者QQ联系。"（死机时提示用 ctrl+alt+del 注销；解压软件专用文案；`shuoming3` 显示 `w4yw4y3tq3yszz<进程名>mmfnfxhghrts`，疑似作者水印/防冒充标记）。
- `VisitTemplate()` → **显示杀软/虚拟机警告(Form2)**：文案"你运行了杀毒类、虚拟机类的软件！"，`shuoming3` 显示 `gyhrw4yw4y3tq3yszz<进程名>mmfnfxhghrts`，并启用 Timer2 锁屏。
- `ForceShutdown()` → **强制关机**：`RtlAdjustPrivilege(19,1,0,ref)`（SE_SHUTDOWN_PRIVILEGE）+ `NtShutdownSystem(2)`（PowerOff）。
- `IsPortOpen(host, port, timeout)` → **TCP 端口探测**：`TcpClient.BeginConnect` + `WaitOne(timeout)`。
- `ConnectTemplate` → Ping 探测（**未使用**）；`SetTemplate` → 统计子串出现次数（**未使用**）。
- `CancelTemplate`（FormClosing）→ 停止并释放 `ManagementEventWatcher`。
- `StartTemplate`（Timer4）→ `Form4.Show()`；`DisableTemplate`（Timer2）→ `Form2.Show()`。
- **声明但未调用的 API（混淆残留）**：`ShowWindow`、`GetDriveTypeA`（U 盘类型检测，已声明未使用）、`NtSuspendProcess/NtResumeProcess`、`SetForegroundWindow/GetForegroundWindow/GetWindowTextA/GetWindowThreadProcessId`、`keybd_event`、`BlockInput`。

## 恶意行为 / 敏感操作
- **持久化**：`sc create zmserv` 自启服务；`HKLM\...\RUN` 写入 `prozs`/`jfglzsn` 自启动；`C:\windows\jf\*.dat` 释放可执行文件（zmserv.exe、przs.exe、随机名.exe）；随机日更进程名规避检测。
- **对抗安全软件**：进程级黑名单（360safe.exe、360taskmgr、360netman.exe、HipsTray.exe、QQPCTray.exe、QQPCNetFlow.exe、iu杀毒）+ 产品描述黑名单（PC Hunter、IceSword、火绒剑、Process Explorer、Procmon、OpenArk、PowerTool 等）；命中即杀进程/删文件。
- **系统破坏/锁定**：全屏无边框锁屏窗（Form2/Form4，FormClosing 被 Cancel）、`ClipCursor` 锁鼠标、强制关机（NtShutdownSystem）、`BlockInput` 声明。
- **网络阻断/劫持**：改写 hosts（127.0.0.1 屏蔽视频/产品网站，设为隐藏只读系统属性）、`route add ... mask 255.255.255.255` 黑洞路由、IFEO 劫持 route.exe、`ipconfig /flushdns`。
- **功能限制**：禁用 USB 存储（usbstor Start=4）、关闭 Windows 防火墙（netsh + 注册表）、限制 cmd（DisableCMD）。
- **进程管控**：击杀 taskmgr；击杀"违规"进程并删除文件；守护/重启电子教室进程，5 次失败即关机。
- **自我恢复**：监控密码/自启动/IFEO/hosts 等注册表与文件，被改即恢复；服务被停即重建。

## IOC / 硬编码
- **路径**：`C:\Windows\System32\drivers\etc\hosts`（+备份 `hosts2`）；`C:\windows\jf\`（存放 zmserv.dat / przs.dat）；`c:\1.txt`（黑名单临时文件）；`C:\Program Files (x86)\<随机>`（未使用残留）。
- **注册表**：`HKCU\Software\jfglzs`（zhuye/dzjs/usb_jianche/shadu_jianche/cmd_jianche/gaowei/n2）；`HKCU\Software\n`（主密码，SHA256 哈希）；`HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN\prozs`、`\jfglzsn`；`HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\route.exe\Debugger="null"`；`HKLM\SYSTEM\CurrentControlSet\Services\usbstor\Start=4`；`HKLM\...\FirewallPolicy\{StandardProfile,PublicProfile}\EnableFirewall=0`；`HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD=0`；`HKLM\SOFTWARE\[Wow6432Node\]TopDomain\e-Learning Class\Student\ChannelId/AutoLogin`（极域）。
- **服务/进程**：服务 `zmserv`；进程 `zmserv.exe`、`przs.exe`、`jfglzsn.exe`、随机名 5 位小写字母进程（字符 i~r）；服务 `tdfilefilter`、`tdnetfilter`（腾讯电脑管家过滤驱动，被强制拉起）。
- **网络**：`http://<zhuye>/hmd_net.txt`（zhuye 来自注册表，C2 配置源）；`www.baidu.com:443`（连通性探测）；`route add <ip> mask 255.255.255.255 0.0.0.0 metric 1`（封堵 IP 列表来自服务器）。
- **水印串**：`w4yw4y3tq3yszz` / `gyhrw4yw4y3tq3yszz` / `mmfnfxhghrts`（警告窗防冒充标记）。
- **黑名单进程**：360safe.exe、HipsTray.exe、QQPCTray.exe、360taskmgr、QQPCNetFlow.exe、360netman.exe、iu杀毒、Minesweeper、winmine.exe、solitaire.exe、winrarrr.exe、wzmainnn.exe、bandizippp.exe、7zFMMM.exe、360zippp.exe。
- **黑名单产品名/描述**：Windows 命令处理程序、Windows PowerShell、注册表编辑器、Microsoft 管理控制台、任务管理器、PC Hunter、IceSword、Wsyscheck、SnipeSword、Process Explorer、PowerTool、Process Hacker、Procmon、火绒剑、OpenArk、瑞星防火墙、Dexpot 等虚拟桌面、极域杀手、翘课、FuckMythware、Hello Teacher、掌控课堂、吾爱破解、教室终结者、易语言程序 等（详见上方数组，另服务端可动态追加）。
