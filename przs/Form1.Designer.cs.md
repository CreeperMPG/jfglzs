# Form1.Designer.cs.md — 主窗体（隐藏运行的管控核心）

## 文件作用

`Form1` 是整个"学生机房管理助手"的主窗体，也是**全部恶意行为的调度中心**。窗体自身被隐藏
（`Hide()` + `WindowState.Minimized` + `ShowInTaskbar=false` + 无边框），只在托盘留一个"学生机房管理助手"图标，
标题先伪装成 `word`，随后改成随机 3 个字符。

这个文件里同时装了三类东西：

1. **自我迁居持久化**：把自己复制到 `C:\Program Files (x86)\<随机4字>\<随机5字>.exe`，加隐藏+系统属性，
   启动副本后 `ProjectData.EndApp()` 结束当前进程；
2. **7 个 Timer 的轮询式管控**：守护伴生进程、恢复 hosts、强制恢复"禁止下载""禁止游戏"注册表项、读开关、弹提示窗；
3. **FileSystemWatcher 的下载/安装拦截**：见可执行文件/脚本/安装包就杀进程 + 删文件，见改名伪装的 PE 也删。

反混淆要点一：**On Error Resume Next**。每个事件处理器外层都有
`ProjectData.ClearProjectError()` + `num2 = 2/3/4…` 状态号 + `catch when (endfilter(...))`，
末尾还有按状态号分派的 `num3 = num2; switch(num3 + 1) { … }` 恢复表与
`throw ProjectData.CreateProjectError(-2146828237)`（= VB 的"无法恢复"错误 19）。
状态号的唯一用途就是让"某条语句抛异常 → 跳到下一条语句继续执行"，所以这里**逐条**还原成独立的
`try { … } catch { }`（catch 故意留空 = 原版忽略错误）。既不能合并成一个大 try
（第一条出错后其余全被跳过），也不能删掉（错误会往上冒）。要读的值先给默认值再在 try 里赋值，
对应 VB 局部变量出错时保持 Nothing/0 的行为。

反混淆要点二：`IL_xxxx` 标签与 `goto` 全部消除（103 个 → 0），还原成
`if / else / while / do-while`；`goto` 跳到方法末尾的写法统一改成 `return`（都在方法体最外层），
`goto` 跳进循环中段的写法（`CloneVal` 的"没找到同名进程就直接删"）用 `break` + 外层 `while` 还原，
删除顺序、Sleep 间隔与原版逐条对应。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Form1` | 主窗体；隐藏运行，承载全部管控逻辑与定时器 |
| `窗体加载_初始化并自启动` | 真正入口：自我迁居 + 策略落地 + 挂监视器 |
| `定时器_应用开关设置`（Timer_main 10s） | 读 `games_jianche` / `bianhao` 两个开关后自停 |
| `定时器_看护守护进程`（Timer1 3s） | 保证 `jfglzsn` 进程常驻，被杀就重启，累计 40 次报警 |
| `定时器_显示提示窗体`（Timer2 1s） | 让 Form2（警告窗）保持显示 |
| `定时器_恢复hosts`（Timer_jianchahosts 15s） | hosts 被删就恢复并报警 |
| `定时器_强制恢复下载限制`（Timer_jzxz 5s） | 浏览器"禁止下载/禁用开发者工具"策略被改就写回 + 杀浏览器 |
| `定时器_强制恢复游戏限制`（Timer_youxi 30s） | Chrome 恐龙彩蛋 / Edge 冲浪游戏被开启就写回 + 杀浏览器 |
| `监听目录` | 给目录挂 `FileSystemWatcher` |
| `文件创建_拦截高危类型` | 按扩展名删文件 / 杀进程；PE 文件头检测防"改名绕过" |
| `文件重命名_拦截高危类型` | 改名成危险扩展名就删 |
| `托盘图标_点击` | 启动 `about.exe` |
| `进程是否在运行` | `GetProcessesByName(...).Length > 0` |
| `初始化组件` | Designer 生成的 InitializeComponent（原名 `DestroyVal`） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `FlushVal` | `定时器_应用开关设置` | Timer_main.Tick |
| `ConnectVal` | `定时器_显示提示窗体` | Timer2.Tick |
| `ValidateVal` | `定时器_看护守护进程` | Timer1.Tick |
| `PublishVal` | `定时器_恢复hosts` | Timer_jianchahosts.Tick |
| `TestVal` | `定时器_强制恢复下载限制` | Timer_jzxz.Tick |
| `InvokeVal` | `定时器_强制恢复游戏限制` | Timer_youxi.Tick |
| `ManageVal` | `监听目录` | 挂 FileSystemWatcher |
| `PrepareVal` | `文件重命名_拦截高危类型` | Watcher.Renamed |
| `CloneVal` | `文件创建_拦截高危类型` | Watcher.Created / Changed |
| `MoveVal` | `窗体加载_初始化并自启动` | Form.Load |
| `InstantiateVal` | `托盘图标_点击` | NotifyIcon.Click |
| `CompareVal` | `进程是否在运行` | 参数 `string_0` → `进程名` |
| `DestroyVal` | `初始化组件` | 实为 InitializeComponent |
| `record` | `随机名` | 随机生成的名字（无扩展名） |
| `m_Mapper` | `副本文件名` | 随机名 + `.exe` |
| `_Composer` | `下载拦截设置` | 注册表 `xiazai`，`"off"` = 开启拦截 |
| `exporter` | `拦截模式` | 注册表 `gaowei`，`1` = 只拦重命名 |
| `_Singleton` | `高危文件删除计数` | reg/bat 类文件删除数 |
| `m_Candidate` | `子进程被杀次数` | jfglzsn 未启动次数 |
| `utils` | `注册表被改次数` | 禁止下载项被改次数 |
| `m_Identifier` | `正在提示注册表被改` | Form3 状态标志 |
| `m_Initializer` | `正在提示hosts被删` | Form3 状态标志 |
| `request` | `正在提示游戏项被改` | Form3 状态标志 |
| `getter` | `hosts被删次数` | |
| `m_Helper` | `数据目录` | `C:\windows\jf` |
| `_Creator` | `hosts路径` | `C:\Windows\System32\drivers\etc\hosts` |
| `exception` | `组件容器` | IContainer |
| `_Observer` | `托盘图标` | NotifyIcon1 后备字段 |
| `issuer` | `主定时器` | Timer_main 后备字段 |
| `resolver` | `提示定时器` | Timer2 后备字段 |
| `_Global` | `看护定时器` | Timer1 后备字段 |
| `value` | `hosts定时器` | Timer_jianchahosts 后备字段 |
| `repository` | `下载限制定时器` | Timer_jzxz 后备字段 |
| `consumer` | `游戏限制定时器` | Timer_youxi 后备字段 |
| `_Reponse` / `worker` / `token` / `_Container` / `_Attribute` / `product` / `m_Method` | 保留原名 | 只赋值或完全无读写的遗留字段 |
| `Proccesor.InsertVal` | `Proccesor.读取注册表值` | 跨文件方法改名 |
| `Proccesor.WriteVal` | `Proccesor.写入注册表值` | 同上 |
| `Queue.CollectVal` | `Queue.MyForms` | 原 VB `My.Forms` |
| `text` / `text2` / `obj` / `num3`… | 按语义命名（见下） | dnSpy 的局部名全体位移，必须逐个看用途 |

## 关键方法与流程

### 窗体加载_初始化并自启动(object, EventArgs)

**分支 A：当前不在 `C:\Program Files (x86)` 下**

1. `Hide()`；
2. 读 `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN` → `prozs`，为空再读 `…WOW6432NODE…RUN`；
3. 把读到的命令行去掉末尾 9 个字符（`\przs.exe`）存进 `Proccesor.程序所在目录`；
4. `月日乘积 = 月 × 日`，取 `%7 / %9 / %5 / %3` 四个余数；偶数按 `a+余7, b+余9, e+余5, c+余3` 拼 4 字母，奇数换序；
5. `VBMath.Randomize(月日乘积)` → `Rnd()*100000*3+1` 四舍五入 → 逐位 `%10+105` 取字符拼 5 位名（后写的覆盖前面的 4 字母名）；
6. 目标目录 `C:\Program Files (x86)\<5 位名第 2~5 位>`，窗口标题 = 5 位名前 3 位；
7. 隐藏托盘图标 → 建目录 → `SetAttributes(Normal)` + `Delete` 旧副本 → `File.Copy(启动路径\przs.exe, 目标)`；
8. 目录和副本都加上 `Hidden | System`；
9. 若新名字进程未在跑 → `Process.Start(副本)`；
10. `ProjectData.EndApp()` 结束自己，`return`。

**分支 B：已在 `C:\Program Files (x86)` 下**

1. 读 `HKCU\Software\jfglzs` 的 `usb_jianche`；为 `on` 且 `usbstor` 的 `Start ≠ 4` →
   `CurrentControlSet` / `ControlSet001` / `ControlSet002` / `ControlSet003` 四处全写 `Start = 4`（禁用 U 盘存储驱动）；
2. 读 `xiazai`；为 `off` 且 Chrome `DownloadRestrictions ≠ 3` → 写 12 个策略键（见"IOC"），
   然后 `Timer_jzxz.Enabled = true`；
3. 依次 `监听目录`：`C:\Users\<用户>\Downloads`、桌面、我的文档、`Environment.GetLogicalDrives()` 的**每个盘符**、
   两个"360 浏览器下载"目录（d/e/f/c 盘依次探测）、`D:\ChromeCoreDownloads`、
   `C:\Windows\Microsoft.NET\Framework\v4.0.30319`。

### 定时器_看护守护进程(object, EventArgs)

1. `GetProcessesByName("jfglzsn")` 有结果 → 什么都不做；
2. 否则计数 +1（checked）；若"程序目录\jfglzsn.exe"不存在且 `C:\windows\jf\jfglzsn.dat` 存在 → 复制过去；
3. `Process.Start(<程序目录>\jfglzsn.exe)`（经 `NewLateBinding.LateCall`，保留 VB 迟绑定语义）；
4. 计数 ≥ 40 → Form2 文案"进程j被多次终止或文件会被杀毒软件（如win10自带的defender）清除！" + 显示 + 启动 Timer2。

### 定时器_恢复hosts / 定时器_强制恢复下载限制 / 定时器_强制恢复游戏限制

- **hosts**：不存在就计数 +1、置提示标志，备份 `hosts2` 存在则 `Copy(…, true)` 恢复并弹 Form3；
  超过 5 次 → 关 Form3、Form2 报"多次被删除或备份文件不存在"、启动 Timer2。
- **下载限制**：三个值（Zones\3 的 `1803`、Chrome 与 Edge 的 `DownloadRestrictions`）都为 `3` 时视为正常；
  任一不是 → 计数 +1；>15 → Form2"请重新启动电脑"；否则写回 12 个策略 → 杀
  `chrome`/`msedge`/`firefox`/`iexplorer`/`explorer` → `Process.Start("explorer")` →
  置标志 + Form3 两行文案 + 显示。
- **游戏限制**：Chrome `AllowDinosaurEasterEgg`、Edge `AllowSurfGame` 任一 ≠ 0 → 写 0 → 杀 chrome/msedge →
  置标志 + Form3 文案 + 显示；都正常则收尾（关 Form3、清标志）。

### 文件创建_拦截高危类型(object, FileSystemEventArgs)

最狠的一支，按扩展名分四路：

| 扩展名 | 处理 |
| --- | --- |
| `.exe` / `.com` | 同名 `.cpp` / `.cp` 不存在且拦截模式 2 或 0 → 循环：按"进程名 = 无扩展名文件名"取进程，主模块路径与事件路径一致就 `Kill()`；然后 `Sleep(500)` → `SetAttributes(Normal)` → `Delete`；文件还在就整轮重试（死循环）。删成功后弹 Form3"本电脑禁止创建新的exe文件，再次操作会锁屏！" |
| `.reg` `.bat` `.cmd` `.vbs` `.vbe` `.scr` `.dll` `.ps1` `.pif` | `SetAttributes(Normal)` → `Delete` → `Sleep(200)`，连做 3 次；计数 +1；弹 Form3"禁止创建新的reg、bat等高危文件" |
| `.msi` | 路径含 `config.msi` 就放过（Windows Installer 临时目录）；否则连删 3 次 + 弹 Form3 |
| 其它 | `Sleep(500)` 等写完 → 读前 2 字节 → 是 `MZ`（77 0x4D、90 0x5A）就当伪装可执行文件：杀同名进程 → `SetAttributes` → `Delete` → `Sleep(200)` → 文件还在就重试；成功后弹 Form3"禁止创建新的exe、dll变相文件" |

开始时还有一句 `new FileInfo(e.FullPath)`（构造后丢弃），保留不动。
跨线程时先 `BeginInvoke` 回 UI 线程再处理。

### 文件重命名_拦截高危类型(object, RenamedEventArgs)

扩展名命中 `zip/rar/7z/arj/exe/com/msi/reg/bat/cmd/vbs/vbe/scr/dll/ps1/pif` 任一 → `Delete` + `Sleep(200)` 连做 3 次
→ Form3 三行文案（"禁止下载安装软件，再次操作会锁屏" + "请老师来退出小助手" + "刚刚删除文件：…"）。

### 其它

- `监听目录(string)`：`FileSystemWatcher{Path, Filter="*.*"}`，桌面开子目录、其它不开；
  `NotifyFilter = FileName | LastWrite`；读 `gaowei`、`xiazai` 两项后按条件挂 `Renamed` / `Created` / `Changed`。
- `初始化组件()`：托盘图标"学生机房管理助手"、7 个 Timer 的间隔（10000/1000/3000/1000/15000/5000/30000）、
  窗体无边框 + 不显示在任务栏 + 最小化启动 + 标题 `word`。
- 属性 setter 都是 Designer 的"先摘旧事件再加新事件"模式，已按语义重命名后备字段。

## 恶意行为 / 敏感操作

| 行为 | 位置 | 说明 |
| --- | --- | --- |
| **自我复制 + 自我重启** | `窗体加载` 分支 A | 复制到 `C:\Program Files (x86)\…`，隐藏+系统属性，启动副本后结束自己 |
| **反分析：随机文件名** | 同上 | 目录名/文件名由"月×日"和 `Rnd()` 生成，每次装到不同机器都不同 |
| **进程查杀** | 下载/游戏限制、文件拦截 | `chrome`、`msedge`、`firefox`、`iexplorer`、`explorer`、以及"同名文件路径一致"的任意进程 |
| **强制重启资源管理器** | `定时器_强制恢复下载限制` | 杀掉 `explorer` 后 `Process.Start("explorer")` |
| **禁用 U 盘** | `窗体加载` 分支 B | `usbstor` 服务 `Start = 4`，四个 ControlSet 全写 |
| **禁止浏览器下载/另存为/F12** | 同上 + Timer_jzxz | Chrome/Edge/Firefox/IE 策略键共 12 个 |
| **禁止游戏** | Timer_youxi | Chrome 恐龙彩蛋、Edge 冲浪游戏 |
| **删除用户文件** | 文件拦截 | exe/com/reg/bat/cmd/vbs/vbe/scr/dll/ps1/pif/msi/zip/rar/7z/arj，以及 PE 头伪装的任意扩展名 |
| **hosts 保护/恢复** | Timer_jianchahosts | 备份文件是 `hosts` 后加 `"2"` |
| **常驻守护** | Timer1 | 反复拉起 `jfglzsn.exe`，从 `C:\windows\jf\jfglzsn.dat` 释放 |
| **托盘隐身** | `初始化组件` | `ShowInTaskbar=false`、无边框、最小化、标题 `word` |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 数据目录 | `C:\windows\jf`（内含 `jfglzsn.dat`） |
| hosts | `C:\Windows\System32\drivers\etc\hosts`，备份 `…\hosts2` |
| 自我复制目标 | `C:\Program Files (x86)\<随机4字>\<随机5字>.exe`，源 `Application.StartupPath + "\przs.exe"` |
| 自启动键 | `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN` → `prozs`；32 位回退 `…WOW6432NODE…` |
| 配置键 | `HKCU\Software\jfglzs`：`games_jianche`、`bianhao`、`usb_jianche`、`xiazai`、`gaowei` |
| 下载限制键 | `HKCU\…\Internet Settings\Zones\3`：`1803=3`、`2200=1`；`HKCU\Software\Policies\Microsoft\Internet Explorer\Restrictions`：`NoBrowserSaveAs=1` |
| Chrome 策略 | `HKLM\SOFTWARE\Policies\Google\Chrome`：`DownloadRestrictions=3`、`SaveAs=0`、`DeveloperToolsAvailability=2`、`AllowDinosaurEasterEgg=0` |
| Edge 策略 | `HKLM\SOFTWARE\Policies\Microsoft\Edge`：`DownloadRestrictions=3`、`SaveAs=0`、`DeveloperToolsAvailability=2`、`AllowSurfGame=0` |
| Firefox 策略 | `HKLM\SOFTWARE\Policies\Mozilla\Firefox`：`DisableDownloads=1`、`BlockAboutDownloads=1`、`DeveloperToolsAvailability=2` |
| U 盘策略 | `HKLM\SYSTEM\CurrentControlSet\Services\usbstor`（及 ControlSet001/002/003）`Start=4` |
| 被守护进程 | `jfglzsn`（`jfglzsn.exe`）；托盘点击启动 `about.exe` |
| 监视目录 | `%USERPROFILE%\Downloads`、桌面、我的文档、所有逻辑盘符、`d:/e:/f:/c:` + `360安全浏览器下载` / `360极速浏览器下载`、`D:\ChromeCoreDownloads`、`C:\Windows\Microsoft.NET\Framework\v4.0.30319` |
| 危险扩展名（重命名） | `.zip .rar .7z .arj .exe .com .msi .reg .bat .cmd .vbs .vbe .scr .dll .ps1 .pif` |
| PE 文件头 | `MZ`（0x4D 0x5A） |
| 提示文案 | `进程j被多次终止或文件会被杀毒软件（如win10自带的defender）清除！`、`hosts文件被删除！现在恢复。`、`hosts文件多次被删除或备份文件不存在！`、`你违规修改了注册表的禁止下载项！`、`现在助手强制恢复！`、`你多次修改了注册表的禁止下载项！`、`请重新启动电脑！`、`你违规修改了注册表的禁止游戏项！`、`本电脑禁止下载安装软件，再次操作会锁屏！`、`如果确有需要，请老师来退出小助手。`、`本电脑禁止创建新的exe文件，再次操作会锁屏！`、`老师请先退出小助手，重新安装软件。你也可重新设置：允许（不建议）`、`本电脑禁止创建新的reg、bat等高危文件，再次操作会锁屏！`、`本电脑禁止创建新的msi等高危文件！`、`本电脑禁止创建新的exe、dll变相文件，再次操作会锁屏！`、`刚刚删除文件：`、`学生机房管理助手`、`word` |
| 数字常量 | Timer 间隔 `10000/1000/3000/1000/15000/5000/30000`；阈值 `40`（进程重启）、`5`（hosts）、`15`（注册表）；删除重试 `3` 次、间隔 `200ms`；PE 检测前 `Sleep(500)`；`Chr` 偏移 `97/98/99/101`、`105`；`Rnd()*100000*3+1` |
