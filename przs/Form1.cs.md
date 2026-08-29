# Form1.cs — 机房管理助手主控窗体（隐藏驻留 + 文件监控 + 注册表强制策略）

## 文件作用
主窗体，程序的核心调度器。启动后立即隐藏（`Hide()`），以托盘图标"学生机房管理助手"驻留后台，
挂载 7 个 Timer 与多个 FileSystemWatcher，持续执行：进程/文件自愈、禁止下载安装、hosts 自愈、
禁用 USB 存储、禁用浏览器游戏、监视桌面/下载/全盘目录并删除"违规"文件。同时实现自我复制持久化
（写入 `C:\Program Files (x86)\<随机名>\`）与注册表自启动（`HKLM\...\Run` 键 `prozs`）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form1 | 隐藏主窗体；托盘图标；7 个定时任务；全盘目录监视；自我复制与注册表持久化 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `PushBroadcaster` | `主加载/持久化`（Form1.Load 事件） | 隐藏、读 Run 键、复制自身、禁用 USB、写下载策略、安装文件监视 |
| `CallBroadcaster` | `检查游戏/编号开关`（Timer_main 10s） | 读 `HKCU\Software\jfglzs\games_jianche`、`bianhao` 决定是否显示 bianhao 窗/启用游戏定时器 |
| `VisitBroadcaster` | `显示 Form2 警告窗`（Timer2 1s） | 若 Form2 不可见则弹出 |
| `InterruptBroadcaster` | `exe 自愈重生`（Timer1 3s） | 检查/复制/启动 `jfglzsn.exe`，超 40 次提示杀软干扰 |
| `ManageBroadcaster` | `hosts 文件自愈`（Timer_jianchahosts 15s） | hosts 被删则用 hosts2 备份恢复，超 5 次弹 Form2 |
| `SelectBroadcaster` | `禁止下载策略强制`（Timer_jzxz 5s） | 校验/写回浏览器禁止下载注册表项，杀浏览器进程并重启 explorer |
| `RateBroadcaster` | `禁止浏览器游戏`（Timer_youxi 30s） | 校验/清零 Chrome 恐龙、Edge Surf 游戏注册表，杀进程 |
| `RestartBroadcaster` | `安装目录监视器` | 对指定目录挂 FileSystemWatcher（Renamed→删文件；Created/Changed→删 exe/高危文件） |
| `RemoveBroadcaster` | `重命名事件：删除下载文件` | 删 .zip/.rar/.exe/.bat/.vbs 等 16 种扩展名文件，重试 3 次 |
| `CheckBroadcaster` | `新建事件：删除 exe/高危文件` | 删 exe/com/reg/bat/msi/MZ 头文件；先杀同名进程再删 |
| `IncludeBroadcaster` | `托盘图标点击` | 启动 `config\about.exe` |
| `FlushBroadcaster` | `进程存在检测` | `Process.GetProcessesByName(name).Length > 0` |
| `ValidateBroadcaster` | `初始化组件`（窗体布局） | 创建托盘、Splitter、7 个 Timer 并配置间隔 |
| `m_Adapter` | `备用程序目录` | `C:\windows\jf`，存放 jfglzsn.dat 备份 |
| `_Exception` | `hosts 路径` | `C:\Windows\System32\drivers\etc\hosts` |
| `m_Thread` | `重生计数器` | 阈值 40 |
| `m_Definition` | `hosts 恢复计数器` | 阈值 5 |
| `m_Error` | `违规修改注册表计数器` | 阈值 15 |
| `m_Value/writer` | `随机副本名` | 由日期派生 + Rnd() 生成 |
| `m_Product` | `高危文件策略模式` | 读 `HKCU\Software\jfglzs\gaowei`（0/1/2） |
| `_Message` | `下载限制开关` | 读 `HKCU\Software\jfglzs\xiazai`（on/off） |

## 关键方法与流程
### 主加载/持久化（PushBroadcaster，Load 事件）
- **职责**：首启自举：把自身复制到受保护目录并随机改名，配置注册表自启动与系统策略，随后退出。
- **流程**：
  1. `Hide()` 隐藏主窗。
  2. 读 `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN\prozs`（缺则读 WOW6432NODE 分支），
     取值为本程序路径，`Initializer.config = 去掉路径末尾 9 字符`（即 exe 所在目录，去 `\przs.exe`）。
  3. 用当前日期 `月×日` 做种子，生成 4 字符随机名（`Chr(97+…)` 系列），再以 `Rnd()` 生成 5 位数字串 `text2`。
  4. 目标目录 `C:\Program Files (x86)\<text2中4字符>`；若 `Application.StartupPath` 不在
     `C:\Program Files (x86)`（即首次运行）→ 复制 `przs.exe` 为 `<随机名>.exe`，
     目录与文件都加 `Hidden|System` 属性，若目标进程未在运行则 `Process.Start` 启动副本，然后 `EndApp()` 结束自身。
  5. 已在安装目录运行时（二次启动）：若 `HKCU\Software\jfglzs\usb_jianche=on` → 把
     `SYSTEM\CurrentControlSet*\Services\usbstor\Start` 置 4（禁用 USB 存储）。
  6. 若 `xiazai=off` → 写全量禁止下载注册表（见 IOC），启用 `Timer_jzxz`。
  7. 对以下目录逐一安装 FileSystemWatcher：`C:\Users\<用户名>\Downloads`、桌面、我的文档、
     **`Environment.GetLogicalDrives()` 遍历所有逻辑盘根目录**、`D:/E:/F:/C: + "360安全浏览器下载"`、
     `D:/E:/F:/C: + "360极速浏览器下载"`、`D:\ChromeCoreDownloads`、`C:\Windows\Microsoft.NET\Framework\v4.0.30319`。
- **关键调用/API**：`Environment.GetLogicalDrives`、`FileSystemWatcher`、`Registry.LocalMachine`、
  `Process.Start`、`File.Copy/SetAttributes`。

### 检查游戏/编号开关（CallBroadcaster，Timer_main 10s）
- **流程**：1. 启用 Timer1(3s)；2. 读 `HKCU\Software\jfglzs\games_jianche`，为 `"on"` 则启用
  `Timer_youxi`(30s)；3. 读 `HKCU\Software\jfglzs\bianhao`，为 `"on"` 则显示 `bianhao` 编号窗；
  4. 关闭 Timer_main（一次性）。

### exe 自愈重生（InterruptBroadcaster，Timer1 3s）
- **流程**：若 `jfglzsn` 进程已存在则跳过；否则 `m_Thread++`，若 `config\jfglzsn.exe` 不存在且
  `C:\windows\jf\jfglzsn.dat` 存在 → 复制为 exe，然后 `Process.Start(config\jfglzsn.exe)`。
  `m_Thread>=40` 时在 Form2 提示"进程j被多次终止或文件会被杀毒软件（如win10自带的defender）清除！"并显示 Form2。
- **关键调用/API**：`File.Copy`、`Process.Start`。

### hosts 文件自愈（ManageBroadcaster，Timer_jianchahosts 15s）
- **流程**：hosts 不存在 → `m_Definition++`、`m_Facade=true`；若存在 `hosts2` 备份则复制回 hosts，
  Form3 提示"hosts文件被删除！现在恢复。"；`m_Definition>5` → 关 Form3，Form2 提示
  "hosts文件多次被删除或备份文件不存在！"，启用 Timer2。hosts 正常存在时关掉 Form3 并复位标志。
- **关键调用/API**：`File.Exists/Copy`。

### 禁止下载策略强制（SelectBroadcaster，Timer_jzxz 5s）
- **流程**：校验 3 个值是否被改：IE Zone 3 `1803`、Chrome `DownloadRestrictions`、Edge `DownloadRestrictions`；
  任一 ≠ 预期 → `m_Error++`。`m_Error<=15` 时写回全部策略（IE Zone 1803=3、2200=1、
  IE Restrictions NoBrowserSaveAs=1、Edge/Chrome DownloadRestrictions=3、SaveAs=0、DeveloperToolsAvailability=2、
  Firefox DisableDownloads=1、BlockAboutDownloads=1、DeveloperToolsAvailability=2），
  然后**依次 Kill 所有 chrome、msedge、firefox、iexplorer、explorer 进程**，最后 `Process.Start("explorer")` 重启资源管理器，
  Form3 提示"你违规修改了注册表的禁止下载项！"。`m_Error>15` → Form2"你多次修改了注册表的禁止下载项！请重新启动电脑！"。
  全部符合预期时关闭 Form3、复位标志。

### 禁止浏览器游戏（RateBroadcaster，Timer_youxi 30s）
- **流程**：读 Chrome `AllowDinosaurEasterEgg`、Edge `AllowSurfGame`，任一非 0 → 写回 0，
  Kill 所有 chrome/msedge，Form3 提示"你违规修改了注册表的禁止游戏项！"。

### 安装目录监视器（RestartBroadcaster(path)）
- **流程**：目录存在才监视；`NotifyFilter=FileName|LastWrite`，Filter=`*.*`；读 `gaowei`（模式）与
  `xiazai`（下载开关）；`xiazai=off` → 订阅 `Renamed`（删改名后的文件）；`gaowei!=1` → 订阅 `Created/Changed`
  （删除新建的可执行/高危文件）。

### 重命名事件：删除下载文件（RemoveBroadcaster）
- **流程**：跨线程则 `BeginInvoke` 重入；扩展名 ∈ {zip,rar,7z,arj,exe,com,msi,reg,bat,cmd,vbs,vbe,scr,dll,ps1,pif}
  时循环 3 次 `File.Delete`（间隔 200ms），Form3 提示"本电脑禁止下载安装软件，再次操作会锁屏！"。

### 新建事件：删除 exe/高危文件（CheckBroadcaster）
- **流程**：
  1. `.exe/.com`：把扩展名替换成 `.cpp/.cp`，若对应"白名单编译产物"不存在且策略为 2 或 0 → 按
     MainModule.FileName 匹配杀掉同名进程，`File.SetAttributes(Normal)` 后循环删除直至消失，Form3 提示。
  2. `.reg/.bat/.cmd/.vbs/.vbe/.scr/.dll/.ps1/.pif`：重试 3 次删除，`m_Attr++`，Form3 提示。
  3. `.msi`：路径含 `config.msi` 跳过（防误删系统安装缓存），否则重试 3 次删除。
  4. 其他扩展名：读前 2 字节，若为 `0x4D 0x5A`（MZ，PE 头）→ 杀同名进程并循环删除（变相 exe 检测）。
- **关键调用/API**：`Process.GetProcessesByName`、`MainModule.FileName`、`FileStream` 读魔数、`File.Delete`。

## 恶意行为 / 敏感操作
- **自我复制与伪装**：复制自身到 `C:\Program Files (x86)\<随机名>\<随机名>.exe`，加 Hidden+System 属性，随机改名。
- **注册表持久化**：`HKLM\...\Run` 键 `prozs`（安装时写入）。
- **禁用 USB 存储**：`usbstor` 服务 Start=4（所有 ControlSet）。
- **强制浏览器策略**：IE/Chrome/Edge/Firefox 禁止下载、禁止另存为、禁用开发者工具（组策略注册表）。
- **进程终止**：批量 Kill chrome/msedge/firefox/iexplorer/explorer，并重启 explorer。
- **文件删除**：监控桌面/下载/文档/所有盘符根目录/360 下载目录，自动删除 zip/rar/exe/msi/脚本等 16 类文件，
  含 MZ 魔数启发式（防改名 exe）。
- **杀软对抗提示**：检测到进程被杀/文件被删后提示"会被杀毒软件（如win10自带的defender）清除！"并持续重生。

## IOC / 硬编码
- 路径：
  - `C:\windows\jf\jfglzsn.dat`（备用程序）、`<config>\jfglzsn.exe`（重生目标）
  - `C:\Windows\System32\drivers\etc\hosts`、`hosts2`（备份）
  - `C:\Program Files (x86)\<随机>`（安装副本）、`<config>\about.exe`、`<config>\yz.exe`
  - `D:/E:/F:/C: + "360安全浏览器下载"`、`D:/E:/F:/C: + "360极速浏览器下载"`、`D:\ChromeCoreDownloads`
  - `C:\Windows\Microsoft.NET\Framework\v4.0.30319`
- 注册表：
  - `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN\prozs`（及 WOW6432NODE 分支）
  - `HKCU\Software\jfglzs\`：`games_jianche`、`bianhao`、`usb_jianche`、`xiazai`、`gaowei`
  - `HKLM\SYSTEM\CurrentControlSet{1,2,3}\Services\usbstor\Start` = 4
  - `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3\1803`=3、`2200`=1
  - `HKCU\Software\Policies\Microsoft\Internet Explorer\Restrictions\NoBrowserSaveAs`=1
  - `HKLM\SOFTWARE\Policies\Microsoft\Edge\`：DownloadRestrictions=3、SaveAs=0、DeveloperToolsAvailability=2
  - `HKLM\SOFTWARE\Policies\Google\Chrome\`：DownloadRestrictions=3、SaveAs=0、DeveloperToolsAvailability=2
  - `HKLM\SOFTWARE\Policies\Mozilla\Firefox\`：DisableDownloads=1、BlockAboutDownloads=1、DeveloperToolsAvailability=2
  - Chrome `AllowDinosaurEasterEgg`、Edge `AllowSurfGame` = 0
- 进程：`jfglzsn`、`chrome`、`msedge`、`firefox`、`iexplorer`、`explorer`
- 扩展名黑名单：`.zip .rar .7z .arj .exe .com .msi .reg .bat .cmd .vbs .vbe .scr .dll .ps1 .pif`
