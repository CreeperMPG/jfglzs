# WindowsApplication1\Form1.Designer.cs —— 文件级分析

## 文件作用
「机房管理助手」的设置主窗体 `Form1`。这是整个样本里**恶意行为最集中**的一个文件：界面上 12 项限制（学生端路径、USB、火绒、命令提示符、Windows 键、机器编号、高危文件、注册表、小游戏、密码、桌面壁纸、浏览器下载）最终都会由这里的代码落到注册表、hosts、文件属性、服务和引导项上。

文件里同时有两部分内容：
- 第 1~2040 行：业务逻辑（12 个事件处理器 + 若干辅助方法），原本是 VB 的 `On Error Resume Next` 状态机，已全部反混淆；
- 第 2042 行之后：设计器生成的 `SearchInfo()`（控件创建/布局）+ 43 个 `[AccessedThroughProperty]` 属性包装 + 字段声明，未做语义改动。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `Form1 : Form` | 设置主窗体（`[DesignerGenerated]`）；构造函数里初始化 hosts 路径/主页域名并建界面 |
| `Form1.RECT` | `ClipCursor` 用的矩形结构（Left/Top/Right/Bottom） |
| `Form1.WallpaperStyle` | 壁纸样式枚举（Tiled=1 / Centered / Stretched=6），**实际未被 ResetInfo 使用** |
| `AwakeInfo` | Form_Load：从注册表读回 10 项配置并还原单选按钮 |
| `ChangeInfo` | FormClosing：`ProjectData.EndApp()` 直接掐进程 |
| `ConcatInfo` | 「手动选择学生端文件」按钮 |
| `PrepareInfo` | 「保存设置」主流程（校验 → 落盘 → 重启 explorer → 显示 Form3） |
| `DefineInfo` | **核心恶意载荷**：写几十项策略注册表、装 zmserv 服务、关防火墙、删安全模式、改 bcdedit、藏文件 |
| `UpdateInfo` | 劫持快速启动栏/任务栏/收藏夹快捷方式 |
| `CollectInfo` | **替换桌面所有浏览器入口为马甲 exe**，并记录真实安装路径 |
| `ReadShortCut` | 用 WScript.Shell 读 .lnk 的目标路径 |
| `SelectInfo` / `InsertInfo` | 打开作者网站（bangzhu.htm / c16.html） |
| `CallInfo` | 批量杀进程 + `net stop zmserv` + `ClipCursor` 锁鼠标 |
| `VisitInfo` / `ForgotInfo` / `ResetInfo` | 设置/恢复桌面壁纸 |
| `FlushInfo` | 往 hosts 追加一条 127.0.0.1 记录 |
| `GetProcessPathsByName` | 按进程名取 exe 全路径（MainModule.FileName） |
| `DeleteInfo` | **下载黑名单并合并进 hosts**，最后 flushdns |
| `RegisterInfo` | Ping 探活 |
| `IsPortOpen` | TCP 端口探活 |
| `CheckInfo` | 进程是否在跑 |
| `Dispose` | 释设计器容器 |
| `SearchInfo` | 设计器：建控件、设布局（未改语义） |
| 43 个属性包装 | VB `WithEvents` 模板产物：`Button1` 属性 ↔ 字段 `m_ConnectionAttr` 等 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

### 字段

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `_PoolAttr` | `hosts文件路径` | `<System>\drivers\etc\hosts` |
| `m_ParamAttr` | `hosts备份路径` | 同目录 `hosts-bak` |
| `schemaAttr` | `程序释放目录` | `c:\windows\jf`，藏 `.dat` 的地方 |
| `m_ModelAttr` | `主页域名` | `www.zmyxl2008.com` |
| `m_InterceptorAttr` | `hosts硬编码路径` | `C:\Windows\System32\drivers\etc\hosts` |
| `authenticationAttr` | `hosts已改动` | 本次是否真的改过 hosts |
| `_ResolverAttr` | `学生端路径` | 电子教室学生端 exe 全路径 |
| `_SpecificationAttr` | `USB检查开关值` | `usb_jianche` 原始值 |
| `_StrategyAttr` | 未改名 | 就是 `components` 容器 |
| `m_DatabaseAttr` / `m_IdentifierAttr` / `m_ClientAttr` / `productAttr` / `_PropertyAttr` | 未改名 | 声明了但全工程无引用 |

### 控件字段（`[AccessedThroughProperty]`，代码里一律走属性名，故不必改）

| 混淆名 | 真实控件 | 混淆名 | 真实控件 |
| --- | --- | --- | --- |
| `m_ConnectionAttr` | Button1（保存设置） | `m_MapAttr` | RadioButton4 |
| `bridgeAttr` | RadioButton3 | `composerAttr` | RadioButton5 |
| `repositoryAttr` | OpenFileDialog1 | `issuerAttr` | Button2（手动选择） |
| `objectAttr` | GroupBox4 | `descriptorAttr` | TextBox1（学生端路径） |
| `_ClassAttr` | Button3（设置方法） | `m_IndexerAttr` | GroupBox5 |
| `m_StateAttr` | pd_new（密码框） | `m_RegistryAttr` | Label8 |
| `baseAttr` | RadioButton10 | `interpreterAttr` | Label10 |
| `algoAttr` | GroupBox10 | `predicateAttr` | Button_restorepic |
| `_ConfigAttr` | Button_selectpic | `m_StructAttr` | GroupBox9 |
| `serializerAttr` | RadioButton13 | `m_ComparatorAttr` | Label11 |
| `m_RulesAttr` | Label12 | `printerAttr` | Label13 |
| `m_StatusAttr` | GroupBox1 | `getterAttr` | GroupBox2 |
| `m_SetterAttr` | GroupBox3 | `m_AdvisorAttr` | GroupBox7 |
| `exporterAttr` | Label7 | `facadeAttr` | Label5 |
| `reponseAttr` | Label9 | `m_RegAttr` | LinkLabel2 |
| `containerAttr` | Label14 | `mapperAttr` | Label17 |
| `parserAttr` | GroupBox6 | `writerAttr` | RadioButton8 |
| `m_ReaderAttr` | RadioButton7 | `mappingAttr` | Label18 |
| `m_ServiceAttr` | RadioButton15 | `m_CodeAttr` | RadioButton16 |
| `attributeAttr` | Label21 | `parameterAttr` | GroupBox11 |
| `testAttr` | RadioButton19 | `m_MockAttr` | Label22 |
| `iteratorAttr` | Label23 | | |

### 方法与局部变量

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| 局部 `num` / `num2` / `num4` / `num5` / `num7` / `num11` | 删除 | VB 错误状态号与分发计数器 |
| 局部 `text` / `text2…` / `objectValue` / `fileName…` | `学生端路径` / `杀毒检查设置` / `游戏检查设置` / `目标` / `快速启动目录` 等 | 按用途取名 |
| 局部 `numerator` / `process` / `hashSet` / `stringBuilder` | `同名进程组` / `进程` / `已屏蔽域名` / `构造` | |

## 关键方法与流程（按方法分段，写清步骤）

### AwakeInfo（Form_Load）
1. 读 `HKCU\Software\jfglzs` 的 `dzjs` 得到学生端路径；为空则依次试 `studentMain`、`ClassMangerApp`、`REDAgent` 三个进程的路径。
2. 逐项读 `usb_jianche` / `shadu_jianche` / `cmd_jianche` / `win_jian` / `games_jianche` / `bianhao` / `xiazai`，按 `"off"`/`"on"` 决定选中「允许」还是「禁止」。
3. 读 DWORD 型 `gaowei`（1 全允许 / 2 仅 C++ / 0 全禁止，默认 2）与 `zcb`（2 自动关闭 / 0 完全禁止，默认 2）。

### ConcatInfo
ShowDialog → 文件名去 `.exe` 查进程 → 在跑就把路径写进 TextBox1 和学生端路径，否则 MsgBox 提示重选。

### PrepareInfo（保存设置）
1. 安全软件自检：`QQPCTray`/`360sd`/`360Tray`/`HipsTray` 任一在跑就拒绝保存。
2. 公共桌面 exe + 桌面 lnk 里出现 360 浏览器 → 提示「本软件禁不了它的下载」。
3. `www.baidu.com:443` 不通 → 提示并退出；学生端路径为空 → 提示；密码长度 <6 或等于 `123456` → 提示。
4. 光标等待、清提示、禁用按钮 → `CallInfo()` → `DefineInfo()`。
5. 首次运行复制 `hosts` 模板和 `hosts-bak`；`DeleteInfo(程序目录\hmd_local.txt, "on")`。
6. 主页 443 通的话：写 `zhuye`、`CollectInfo()`、两次 `DeleteInfo(http://<主页>/hmd_net.txt, "on")`。
7. 停 1 秒 → 杀掉所有 explorer → 重启 explorer → 显示 Form3。

### DefineInfo（核心恶意行为，32 个步骤，详见下方「恶意行为」）
写 `HKCU\Software\jfglzs` 全部开关 → 备份学生端 exe → 放开 cmd/regedit → `sc delete zmserv` → regini 提权 + 导入 `yl.wav→yl.reg` → 改 usbstor 启动方式 → 写 HKLM Run 自启 → 关防火墙/锁屏/删 SafeBoot\Network → bcdedit → `UpdateInfo()` → 收回权限 + 写密码摘要 + 重建 zmserv 服务（失败即重启）→ 浏览器策略 → 隐藏 hosts、etc、程序目录和所有伴随文件 → 删除临时 .reg。

### UpdateInfo（快捷方式劫持）
清空 `%APPDATA%\...\Quick Launch\*.*` 与 `...\User Pinned\TaskBar\*.*`，塞入自带的 `zy\Internet   Explore.lnk`；清空收藏夹 `links` 并塞入 `zy\百度搜索&网址导航.url`；最后写 `HKCU\Software\Microsoft\Internet Explorer\LinksBar\Enabled = 1`。

### CollectInfo（浏览器入口替换）
对「桌面 exe / 公共桌面 exe / 桌面 lnk / 公共桌面 lnk」四轮扫描，按 11 个浏览器关键字匹配：
- exe：`File.SetAttributes(Normal)` 后用程序目录 `zy\<同名>.exe` 覆盖；
- lnk：`ReadShortCut` 读出真实目标 → 写 `HKCU\Software\<品牌键>` → `Kill` 快捷方式 → 放马甲 exe（Edge 例外：目标含 `.exe` 才记路径+替换）；
- 一个都没找到 → 桌面塞 `百度搜索&网址导航.url`；再删掉桌面上「网址导航/上网导航/百度一下」的快捷方式；
- 最后把 `zy\` 下（含子目录）所有文件**永久删除**（`DeleteFile(..., RecycleOption.DeletePermanently, ...)`）。

### DeleteInfo（hosts 合并）
网络探活 → 下载名单到 `c:\1.txt` → 首行须含 `www.` → 定位「视频网站：」「产品名：」两行 → 读 hosts 收集已有 127.0.0.1 域名 → 追加主名单（跳过重复）→ 模式 `"off"` 时追加视频网站段 → 写回 hosts、置只读+隐藏+系统、`DisableCMD=0`、`ipconfig /flushdns`。

### CallInfo（清场 + 锁鼠标）
1. 反复杀 `ObserverAttr.m_AccountAttr`（外部程序集给的账号进程名）与 `jfglzsn`，直到两者都查不到（没有进程时 `[0]` 抛越界，原版靠 `On Error Resume Next` 吞掉）。
2. 遍历所有进程，杀掉「名字长度 ≥ 4 且每个字符 ASCII 在 102~111（f~o）」的进程。
3. `DisableCMD=0`、`net stop zmserv`、停 1 秒、再杀 zmserv。
4. `ClipCursor` 把鼠标限制在主屏矩形内。

### 其余小方法
- `FlushInfo(域名)`：hosts 里没有 `127.0.0.1 <域名>` 就整份重写追加。
- `GetProcessPathsByName`：遍历同名进程取 `MainModule.FileName`（取最后一个），异常返回 null。
- `ReadShortCut`：`WScript.Shell.CreateShortcut(...).TargetPath`，用完 `ReleaseComObject`。
- `ResetInfo`：校验文件 → 写 `HKCU\Control Panel\Desktop\Wallpaper` + `WallpaperStyle="2"` → `SystemParametersInfo(20,0,路径,3)`。
- `VisitInfo`/`ForgotInfo`：挑图 / 恢复 `C:\Windows\Web\Wallpaper\Windows\img0.jpg`（两者 catch 里的字符串都没有 `$`，是原版 bug，已原样保留）。
- `RegisterInfo`：`Ping.Send` 判 `IPStatus.Success`；`IsPortOpen`：`BeginConnect` + `WaitOne(timeout)`；`CheckInfo`：`GetProcessesByName().Length > 0`。

## 恶意行为 / 敏感操作

**1. 注册表批量改写（DefineInfo）**
- `HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD = 0`
- `HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\{System,Explorer}`：`DisableRegistryTools`、`NoRun`、`DisableLockWorkstation=1`、`NoFolderOptions`、`NoTrayContextMenu`
- `HKLM\SYSTEM\CurrentControlSet\Services\usbstor\Start`（+ ControlSet001/002/003）= 3 允许 / 4 禁用
- `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN`：`prozs`、`jfglzsn`（**开机自启**，写两遍）
- 防火墙：`EnableFirewall = 0`（StandardProfile / PublicProfile / DomainProfile\PublicProfile）
- 浏览器策略：Chrome/Edge `AllowDinosaurEasterEgg`、`AllowSurfGame`、`DownloadRestrictions`、`SaveAs`、`WebWidgetAllowed`；Firefox `DisableDownloads`、`BlockAboutDownloads`；IE `Zones\3` 的 `1803`/`2200`、`NoBrowserSaveAs`
- 隐藏项：`Explorer\Advanced` 的 `Hidden=0`、`ShowSuperHidden=0`、`HideFileExt=1`、`SHOWALL\CheckedValue=0`
- 其它：`RemoveWindowsStore=1`、`ShowTaskViewButton=0`、`SearchboxTaskbarMode=0`、`Control Panel\Desktop` 超时改 500、`Tcpip6\DisabledComponents=255`、`TabletTip\TipbandDesiredVisibility=0`
- `HKCU\Software\n` = `SHA256(密码+"bfdshgs")`（**密码摘要**，供其他组件校验）
- `HKCU\Software\<品牌>` = 浏览器真实安装路径（`360/chrome/2345/firefox/360js/360jsx/qq/jisu/shuanghe/edge`）

**2. 服务操作**：`sc delete zmserv` → `sc create zmserv binPath= <程序目录>\zmserv.exe start= auto` → `sc failure zmserv actions= "reboot/60000/reboot/60000/reboot/60000" reset= 300`（**服务挂了就重启机器**）；`net stop zmserv`。

**3. 引导 / 恢复手段破坏**：`DeleteSubKeyTree("SYSTEM\CurrentControlSet\Control\SafeBoot\Network")`（进不了带网络的安全模式）、`bcdedit /set {bootmgr} displaybootmenu no`、`bcdedit /delete {ee888888-8888-8888-8888-8888888888ee}`（删指定 GUID 启动项）。

**4. 浏览器入口替换 / 劫持**：`CollectInfo` 用自带马甲 exe 覆盖桌面浏览器；`UpdateInfo` 清空快速启动栏、任务栏固定项、收藏夹 links 并塞入作者链接；`UpdateInfo` 还会打开 IE LinksBar。

**5. hosts 劫持**：`DeleteInfo` 从作者站点拉黑名单写 `127.0.0.1 <域名>`，然后把 hosts 与 `etc` 目录设成只读+隐藏+系统，最后 `ipconfig /flushdns`。

**6. 进程查杀**：`CallInfo` 反复杀账号进程与 `jfglzsn`；杀「名字长度≥4 且字符全在 f~o」的进程；`PrepareInfo` 杀 explorer 后重启。

**7. 锁屏 / 锁输入**：`ClipCursor` 把鼠标锁在屏幕矩形内；P/Invoke 里还导入了 `BlockInput`、`Sleep`（本文件未调用）。

**8. 文件隐藏 / 反取证**：`c:\windows\jf` 藏 `jfglzsn.dat`/`przs.dat`/`zmserv.dat`；程序目录、所有 exe/wav/txt/regini.exe、`C:\Windows\Microsoft.NET`、`C:\Program Files\Microsoft Games` 全设 `Hidden|System`；`zy\` 马甲目录最后被永久删除。

**9. 反安全软件**：`PrepareInfo`/`DefineInfo` 检测并劝退 QQPCTray/360sd/360Tray/HipsTray；`DefineInfo` 用 `regini.exe` 两次改注册表 ACL（`regqx.ini` 提权、`regqxoff.ini` 收回）。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

- **路径**：`<System>\drivers\etc\hosts`、`hosts-bak`、`C:\Windows\System32\drivers\etc\hosts`、`c:\windows\jf`、`c:\1.txt`、`C:\Users\Public\Desktop`、`C:\Windows\System32\drivers\etc`、`C:\Windows\Microsoft.NET`、`C:\Program Files\Microsoft Games`、`C:\Windows\Web\Wallpaper\Windows\img0.jpg`、`%APPDATA%\Microsoft\Internet Explorer\Quick Launch`、`...\User Pinned\TaskBar`、收藏夹 `links`
- **程序目录内文件名**：`jfglzsn.exe`、`przs.exe`、`zmserv.exe`、`set.exe`、`uninstall.exe`、`about.exe`、`regini.exe`、`regqx.ini`、`regqxoff.ini`、`yl.wav`、`huifu.wav`、`hmd_local.txt`、`hosts`、`zy\Internet   Explore.lnk`、`zy\百度搜索&网址导航.url`、`zy\<11 个浏览器名>.exe`
- **URL**：`www.zmyxl2008.com`、`http://www.zmyxl2008.com/hmd_net.txt`、`http://www.jfglzs.com/software/bangzhu.htm`、`https://www.jfglzs.com/c16.html`、`www.baidu.com:443`、`127.0.0.1`
- **进程名**：`studentMain`、`ClassMangerApp`、`REDAgent`、`explorer`、`explorer.exe`、`zmserv`、`jfglzsn`、`QQPCTray`、`360sd`、`360Tray`、`HipsTray`
- **注册表值名**：`dzjs`、`usb_jianche`、`shadu_jianche`、`cmd_jianche`、`win_jian`、`games_jianche`、`bianhao`、`xiazai`、`gaowei`、`zcb`、`zhuye`、`n`、`360`、`chrome`、`2345`、`firefox`、`360js`、`360jsx`、`qq`、`jisu`、`shuanghe`、`edge`
- **服务名**：`zmserv`（binPath = `<程序目录>\zmserv.exe`，失败动作 `reboot/60000`）
- **命令**：`sc delete/create/failure`、`net stop`、`regedit.exe /s`、`regini.exe`、`ipconfig /flushdns`、`bcdedit`
- **GUID**：`{ee888888-8888-8888-8888-8888888888ee}`
- **字符串**：`视频网站：`、`产品名：`、`www.`、`123456`、`off`/`on`

## 反混淆处理记录
- goto：**124 → 0**；`IL_xxxx:` 标签 764 → 0；`@switch(...)` 20 → 0；`catch when (endfilter(...))` 10 → 0；`ClearProjectError()` 全清
- `On Error Resume Next` 还原粒度：按「一项设置 / 一个动作」分块 try/catch，块内语句顺序、字符串、数字不变；`CallInfo` 的「杀到没有为止」循环用逐条 try/catch 复现，保证越界异常不会中断后续步骤
- 语义保留点：`Projects.CompareString`、`Conversions.ToString/ToInteger`、`Strings.Mid/Len/Instr/Asc/Replace`、`Interaction.Shell(...)`、`Operator` 的位或比较、`checked { }` 上下文、`"设置失败: {ex.Message}"` 这类原版 bug 字符串，全部未改
- 未改动区域：`SearchInfo()` 设计器正文（约 1400 行布局代码）、43 个属性包装、字段声明顺序
