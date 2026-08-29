# Form1.cs — 机房管理助手"设置"主窗体：一键应用全套学生机系统限制

## 文件作用
这是恶意软件家族样本（VB.NET，ConfuserEx 重命名 + .NET Reactor/IntelliLock 加壳）的**主设置界面**（窗体标题"设 置"）。程序自称"机房管理小助手"（官网 www.jfglzs.com），运行后允许管理员对学生机批量施加限制：禁用 USB/杀毒软件/CMD/注册表编辑器/小游戏、篡改 hosts 屏蔽网站、替换桌面浏览器快捷方式为假浏览器、创建防删除服务 zmserv、隐藏文件、关闭防火墙与安全模式、禁用 IPv6 等。保存设置时以管理员身份写入 HKLM 并**杀掉 explorer 重启外壳**。本窗体由 load 密码窗体验证通过后显示（`Reader.ReflectProperty.Form1.Show()`）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form1（partial，主类在本文件） | 设置界面 + 全部系统限制逻辑（事件处理 + 工具方法） |
| Form1.WallpaperStyle 枚举 | 壁纸拉伸样式（Tiled=1, Centered=2, Stretched=6） |
| Form1.RECT 结构 | ClipCursor 用矩形 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| CalcProperty() | BuildUI_构建界面 | 在代码里手工创建全部控件（Designer 被合并） |
| CancelProperty(sender,e) | Form_Load_读取配置填充界面 | Load 事件：读 HKCU\Software\jfglzs 各开关并自动探测学生端进程路径 |
| DestroyProperty(sender,e) | Form_FormClosing_直接退出 | `ProjectData.EndApp()` 结束进程 |
| VerifyProperty(sender,e) | Button2_Click_手动选择学生端 | 打开文件对话框选 exe，校验对应进程是否在运行 |
| ValidateProperty(sender,e) | Button1_Click_保存设置入口 | 校验网络/学生端/密码后依次 ResetProperty→PushProperty→ConcatProperty |
| ConcatProperty() | SaveSettings_应用全部限制 | 核心：写注册表策略、hosts、服务、隐藏文件、杀 explorer |
| GetProperty() | CleanQuickLaunch_清理快速启动栏 | 清空 Quick Launch/任务栏/收藏夹 links，放入百度网址导航 |
| RemoveProperty() | ReplaceBrowserShortcuts_替换桌面浏览器 | 桌面/公共桌面 exe、lnk 全部替换为 \\zy\\ 假浏览器 |
| ReadShortCut(strFile) | ReadLnkTarget_读取快捷方式目标 | 用 WScript.Shell 读 .lnk 的 TargetPath |
| CloneProperty(sender,e) | Button3_Click_打开帮助页 | 打开 http://www.jfglzs.com/software/bangzhu.htm |
| PushProperty() | PreSaveCleanup_杀旧进程锁鼠标 | 杀 zmserv/jfglzsn/数字名进程，net stop zmserv，ClipCursor |
| RunProperty(sender,e) | Button_selectpic_Click_选择壁纸 | OpenFileDialog 选图 → FillProperty |
| InitProperty(sender,e) | Button_restorepic_Click_恢复默认壁纸 | 用 img0.jpg 恢复 |
| FillProperty(path,style) | SetWallpaper_设置壁纸 | 写 Control Panel\Desktop 的 Wallpaper/WallpaperStyle + SystemParametersInfo(20) |
| AssetProperty(domain) | AddHostsEntry_追加 hosts 屏蔽 | hosts 追加 "127.0.0.1 <域名>"（去重） |
| GetProcessPathsByName(name) | GetRunningProcessPath_取进程路径 | 返回进程 MainModule.FileName |
| FlushProperty(sender,e) | LinkLabel_Click_打开下载页 | 打开 https://www.jfglzs.com/c16.html |
| ResetProperty(url,flag) | UpdateHostsBlacklist_下载黑名单写 hosts | 下载 hmd 黑名单，解析域名并写入 hosts，flushdns |
| PostProperty(host,timeout) | PingHost_ICMP 探测 | Ping 连通性（IsPortOpen 之外的另一探测） |
| IsPortOpen(host,port,timeout) | TestTcpPort_TCP 连通探测 | BeginConnect + WaitOne 超时 |
| CountProperty(procName) | IsProcessRunning_进程是否在运行 | GetProcessesByName().Length > 0 |

### 字段
| 混淆字段 | 反混淆名 | 值/用途 |
|---------|---------|--------|
| _DispatcherMock | hosts 文件路径 | `%SystemRoot%\drivers\etc\hosts` |
| _InfoMock | hosts 备份路径 | `%SystemRoot%\drivers\etc\hosts-bak` |
| m_ServerMock | 隐藏备份目录 | `c:\windows\jf`（jfglzsn/przs/zmserv 的 .dat 副本） |
| _RulesMock | 黑名单服务器域名 | `www.zmyxl2008.com`（既是屏蔽对象又是 hmd_net.txt 源） |
| m_IssuerMock | hosts 绝对路径 | `C:\Windows\System32\drivers\etc\hosts` |
| m_AdvisorMock | 学生端程序路径 | 极域 StudentMain.exe / 锐捷 ClassMangerApp.exe / 红蜘蛛 REDAgent.exe |
| m_ObjectMock | USB 检查开关值 | "off"/"on" |
| exporterMock | 黑名单是否应用成功 | bool |
| _Global 等 *Mock | 各控件后备字段 | Button1/RadioButton*/GroupBox*/Label*（见属性映射） |

## 关键方法与流程

### Form1 构造函数
- 挂 FormClosing→DestroyProperty、Load→CancelProperty
- 初始化 hosts 路径、`c:\windows\jf`、`www.zmyxl2008.com`，然后 CalcProperty() 构建全部控件
- 界面控件文本即 IOC：极域 `c:\.\StudentMain.exe`、锐捷 `E:\..\ClassMangerApp.exe`、红蜘蛛 `C:\Program Files (x86)\3000soft\Red Spider\REDAgent.exe`

### Form_Load（CancelProperty）
1. 读 `HKCU\Software\jfglzs\dzjs`，非空则填入 TextBox1
2. 为空则自动探测运行中的 `studentMain` → `ClassMangerApp` → `REDAgent` 进程路径填入
3. 读 usb_jianche / shadu_jianche / cmd_jianche / win_jian / games_jianche / bianhao / xiazai / gaowei / zcb，把对应 RadioButton 置为保存时的状态（off/on 语义与保存时相反，代码中按分支映射）

### Button1_Click_保存设置（ValidateProperty）
1. `IsPortOpen("www.baidu.com",443,2000)` 不通则提示"网络不通!请先恢复网络，需要下载黑名单等。"并中止
2. 未选学生端文件 → 提示"选择学生端程序文件!"
3. 密码 < 6 位 → 提示；密码 == "123456" → 提示"密码太简单"
4. 备份 hosts → hosts-bak（不存在才拷）
5. `ResetProperty(StartupPath\hmd_local.txt,"on")` 应用本地黑名单
6. `ResetProperty("http://www.zmyxl2008.com/hmd_net.txt","on")` 下载网络黑名单
7. `PushProperty()` 清理旧环境 → `ConcatProperty()` 正式施加限制

### SaveSettings_应用全部限制（ConcatProperty）
流程（扁平化状态机还原后的顺序）：
1. 确保 `HKCU\Software\jfglzs` 存在，写入全部开关：dzjs、usb_jianche、shadu_jianche、cmd_jianche、win_jian、bianhao、games_jianche、xiazai、gaowei(0/1/2)、zcb(0/2)、zhuye=www.zmyxl2008.com
2. 复制学生端 exe → `exe+"2"` 备份；隐藏其同目录 `SpecialSet.exe`
3. 预置 DisableCMD=0、DisableRegistryTools=0、NoRun=0；`sc delete zmserv`
4. 密码：`ExceptionMock.InsertProperty(密码)`（SHA256(密码+"bfdshgs")）→ 写 `HKCU\Software\n`
5. `regini.exe regqx.ini`（regini 提权改注册表权限）→ 复制 `yl.wav`→`yl.reg` → `regedit.exe /s yl.reg`（yl.wav 实际是伪装成 wav 的 .reg 内容）
6. USB 控制：`SYSTEM\CurrentControlSet\Services\usbstor` 及 ControlSet001~003 的 Start=3（允许）/4（禁止）
7. 开机自启：`HKLM\...\RUN` 写 `prozs`→`przs.exe`、`jfglzsn`→`jfglzsn.exe`（写两遍）
8. 关闭防火墙 EnableFirewall=0（Standard/Public/DomainProfile）；DisableLockWorkstation=1；**删除 `SYSTEM\CurrentControlSet\Control\SafeBoot\Network`（禁用带网络的安全模式）**
9. `bcdedit /set {bootmgr} displaybootmenu no`；`bcdedit /delete {ee888888-8888-8888-8888-8888888888ee}`（删系统恢复条目）
10. GetProperty() 清理快速启动栏；若 `www.zmyxl2008.com:80` 可达 → RemoveProperty() 替换桌面浏览器
11. `regini.exe regqxoff.ini`；重写 `HKCU\Software\n` 密码哈希
12. 创建防删除服务：`sc create zmserv binPath= <StartupPath>\zmserv.exe start= auto` + `sc failure zmserv actions= "reboot/60000/reboot/60000/reboot/60000" reset= 300`（**服务崩溃/被杀 60 秒后自动重启机器**）
13. 小游戏控制（games_jianche）：禁止 → Chrome `AllowDinosaurEasterEgg=0`、Edge `AllowSurfGame=0`；允许 → =1 并删除全部 IFEO 劫持键（sidebar/Chess/FreeCell/Hearts/Minesweeper/PurblePlace/Mahjong/SpiderSolitaire/bckgzm/chkrzm/shvlzm/Solitaire/winmine）
14. Edge `WebWidgetAllowed=0`、`SearchboxTaskbarMode=0`、`RemoveWindowsStore=1`、`ShowTaskViewButton=0`、删 `DesktopProcess`、`AutoEndTasks=1`、`HungAppTimeout=500`、`WaitToKillAppTimeout=500`、`StartupDelayInMSec=0`
15. 允许任务栏右键（win_jian off）→ 导入 huifu.reg（huifu.wav 伪装）+ NoTrayContextMenu=0
16. 下载控制（xiazai）：禁止 → IE Zones\3 `1803=3/2200=1`、`NoBrowserSaveAs=1`、Edge/Chrome `DownloadRestrictions=3`、`SaveAs=0`、Firefox `DisableDownloads=1`、`BlockAboutDownloads=1`；允许 → 全部还原
17. hosts 与 drivers\etc 目录设 **ReadOnly+Hidden+System**；`Hidden=0`、`ShowSuperHidden=0`、`CheckedValue=0`（隐藏文件不可见）；`HideFileExt=1`；NoFolderOptions 按 xiazai 设置
18. 禁止注册表编辑器：`DisableRegistryTools=1`（zcb 完全禁止时）；隐藏 `C:\Program Files\Microsoft Games`；`DisabledComponents=255`（**禁用 IPv6**）；`TipbandDesiredVisibility=0`
19. 复制 jfglzsn.exe/przs.exe/zmserv.exe → `c:\windows\jf\*.dat`；把 c:\windows\jf、各 exe、yl.wav、huifu.wav、hmd_local.txt、regini.exe、启动目录、`C:\Windows\Microsoft.NET` 全部设 Hidden+System
20. 删除 yl.reg、huifu.reg；Sleep(2000)；**遍历 explorer 进程 Kill → 重启 explorer.exe** → 显示 Form3 完成提示

### ReplaceBrowserShortcuts_替换桌面浏览器（RemoveProperty）
- 遍历 用户桌面 + `C:\Users\Public\Desktop` 的 *.exe：文件名含"360安全浏览器/Google Chrome/2345加速浏览器/360极速浏览器/360 极速浏览器X/QQ浏览器/极速浏览器/双核浏览器/Firefox/Microsoft Edge" → 用 `StartupPath\zy\同名.exe` **覆盖**（假浏览器，用完即删）；含"谷歌浏览器"（假 chrome）→ 弹窗提示并删除
- 遍历 *.lnk：`ReadShortCut` 取 TargetPath → 写入 `HKCU\Software\{360,chrome,2345,firefox,360js,360jsx,qq,jisu,shuanghe,edge}` 对应值 → 删除原 lnk → 复制假浏览器 exe
- 一个都没替换过 → 桌面放"百度搜索&网址导航.url"；删除桌面含"网址导航/上网导航/百度一下"的 lnk；最后**删除 \\zy\\ 目录全部文件**（销毁证据）

### UpdateHostsBlacklist_下载黑名单写 hosts（ResetProperty）
1. 先探测 baidu:443，不通则 exporterMock=false 返回
2. WebClient 下载黑名单（本地 hmd_local.txt 或 `http://www.zmyxl2008.com/hmd_net.txt`）到临时文件
3. 首行必须含 "www."，否则失败返回
4. 定位"视频网站："与"产品名："两个分节标记
5. 读取现有 hosts 中已屏蔽的 127.0.0.1 域名入 HashSet 去重
6. 黑名单[0..产品名) 行全部追加 `127.0.0.1 <域名>`；参数为 "off" 时"视频网站："到"产品名："之间的行也追加（即视频网站域名仅在特定模式下屏蔽）
7. 写回 hosts → 设 ReadOnly+Hidden+System → `ipconfig /flushdns`

### PreSaveCleanup_杀旧进程锁鼠标（PushProperty）
- 杀 `ExceptionMock._DecoratorMock`（load 窗体按日期生成的随机名进程）、`jfglzsn`、`zmserv` 进程
- 遍历全部进程，进程名每个字符 ASCII 都在 [102,111]（'f'~'o'，混淆后常量）→ Kill
- `DisableCMD=0`；`net stop zmserv`；ClipCursor 锁定到全屏矩形

## 恶意行为 / 敏感操作
- **持久化**：HKLM RUN 自启动 przs.exe / jfglzsn.exe；创建自启动服务 zmserv（失败自动重启机器）；c:\windows\jf 隐藏副本
- **系统破坏**：删除 SafeBoot\Network 键（禁用安全模式网络）、bcdedit 删恢复条目、关闭防火墙、禁用 IPv6、杀 explorer 强刷外壳
- **对抗防护**：隐藏文件/目录（含 C:\Windows\Microsoft.NET）、禁杀毒软件、禁注册表编辑器、禁 CMD、隐藏扩展名、禁用文件夹选项
- **网络管控**：篡改 hosts（黑名单屏蔽 + 域名 www.zmyxl2008.com）、禁浏览器下载（IE/Chrome/Edge/Firefox 策略）、替换桌面浏览器为假浏览器并记录原目标到注册表
- **数据记录**：快捷方式目标路径写 HKCU\Software\{360,chrome,2345,firefox,...}
- **隐蔽性**：yl.reg/huifu.reg 伪装为 .wav；全部可执行文件设 Hidden+System；用完删除 \\zy\\ 与临时 reg 文件
- P/Invoke：SystemParametersInfo(20) 改壁纸、BlockInput 声明（未实际调用）、ClipCursor、Sleep

## IOC / 硬编码
- 网络：`www.zmyxl2008.com`（黑名单服务器/被屏蔽域名）、`http://www.zmyxl2008.com/hmd_net.txt`、`http://www.jfglzs.com/software/bangzhu.htm`、`https://www.jfglzs.com/c16.html`、`www.baidu.com:443/80` 探测
- 文件：`C:\Windows\System32\drivers\etc\hosts`(+hosts-bak)、`c:\windows\jf\*.dat`、`%AppData%\Microsoft\Internet Explorer\Quick Launch`、`C:\Users\Public\Desktop`、`C:\Program Files\Microsoft Games`、`C:\Windows\Web\Wallpaper\Windows\img0.jpg`、`C:\Windows\Microsoft.NET`、StartupPath 下 set/uninstall/about/jfglzsn/przs/zmserv/regini/yl/huifu/hmd_local.txt、`\\zy\\` 假浏览器包、`SpecialSet.exe`
- 服务/命令：`zmserv`（`sc create zmserv binPath= ... start= auto`、`sc failure zmserv actions= "reboot/60000/reboot/60000/reboot/60000" reset= 300`、`net stop zmserv`）、`sc delete zmserv`、`regedit.exe /s`、`regini.exe regqx.ini/regqxoff.ini`、`ipconfig /flushdns`、`cmd /c bcdedit /set {bootmgr} displaybootmenu no`、`cmd /c bcdedit /delete {ee888888-8888-8888-8888-8888888888ee}`
- 注册表：`HKCU\Software\jfglzs`（dzjs/usb_jianche/shadu_jianche/cmd_jianche/win_jian/bianhao/games_jianche/xiazai/gaowei/zcb/zhuye）、`HKCU\Software\n`（SHA256 密码哈希）、`HKLM\...\RUN`(prozs/jfglzsn)、`SYSTEM\CurrentControlSet\Services\usbstor`(Start 3/4)、`SYSTEM\CurrentControlSet\Control\SafeBoot\Network`(删除)、`SOFTWARE\Policies\Google\Chrome`、`SOFTWARE\Policies\Microsoft\Edge`、`SOFTWARE\Policies\Mozilla\Firefox`、`...\Image File Execution Options\*`(小游戏/winmine/sidebar 等)、`Control Panel\Desktop`、`HKCU\Software\{360,chrome,2345,firefox,360js,360jsx,qq,jisu,shuanghe,edge}`、`Internet Settings\Zones\3`、`Explorer\Advanced`、`SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\`
- 进程：studentMain / ClassMangerApp / REDAgent / explorer / zmserv / jfglzsn / przs / 随机名进程
- 密码默认值：`123456`（禁止使用）；密码规则 ≥6 位字母数字
