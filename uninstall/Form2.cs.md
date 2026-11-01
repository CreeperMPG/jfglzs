# Form2.Designer.cs 分析

## 文件作用

`uninstall.exe` 的**密码确认窗体**：蓝色全屏、置顶、无边框，要求输入「卸载小助手密码」。
密码正确才调用 `卸载核心.执行卸载()`；随后换壁纸、把鼠标锁在屏幕范围内、等 9 秒结束程序。

这个窗体同时是「强制/限制」模块：

- `FormClosing` 无条件 `e.Cancel = true` —— 点 × 、Alt+F4、任务管理器里「关闭」都关不掉；
- 1 秒定时器每 Tick 重新 `TopMost = true` 并把鼠标 `ClipCursor` 到 `(0,0)-(800,200)`；
- 计数 ≥ 5 后每 Tick 响一声 `Beep(2000, 500)`；
- 「重启电脑」按钮直接 `shutdown /r /f /t 0`。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `Form2` | 密码窗体本体（637×407、`BackColor = SystemColors.Highlight`、无边框、置顶、最大化） |
| `InsertReader`（Button1.Click） | 校验密码 → 执行卸载 → 换壁纸 → 锁鼠标 → Sleep 9s → 结束进程 |
| `StopReader`（Timer_frm2.Tick） | 强制置顶 + 锁鼠标 + 计数 + 蜂鸣 |
| `ReadReader`（FormClosing） | 禁止关闭窗体 |
| `WriteReader`（Load） | 启动 1 秒定时器 |
| `OrderReader`（Button2.Click） | `shutdown /r /f /t 0` 立即重启 |
| `PatchReader`（Label_pd.DoubleClick） | **空方法**（原方法体只有 On Error 框架，没有任何语句） |
| `VisitReader`（LinkLabel 点击） | `Process.Start("http://www.jfglzs.com")`；**Designer 里没有 LinkLabel，属接不上的死代码** |
| `ValidateReader` | 换壁纸：写 `Wallpaper`/`WallpaperStyle=2`，再 `SystemParametersInfo(20,0,路径,3)` |
| `RateReader`（InitializeComponent） | 控件初始化 |
| `WallpaperStyle` 枚举 | 壁纸样式：`Tiled=1`、`Centered`、`Stretched=6`（代码里从未使用） |
| `RECT` 结构 | 传给 `ClipCursor` 的 Win32 矩形 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `RateReader` | `RateReader`（保留） | InitializeComponent |
| `InsertReader` | 同上 | 确定按钮 = 校验密码并卸载 |
| `StopReader` | 同上 | 定时器 Tick（置顶/锁鼠标/蜂鸣） |
| `ReadReader` | 同上 | FormClosing → 禁止关闭 |
| `WriteReader` | 同上 | Load → 开定时器 |
| `OrderReader` | 同上 | 重启电脑 |
| `PatchReader` | 同上 | 空方法 |
| `VisitReader` | 同上 | 打开官网（死代码） |
| `ValidateReader(string, WallpaperStyle)` | `ValidateReader(string 壁纸路径, WallpaperStyle 壁纸样式)` | 换壁纸 |
| `account` | `组件容器` | `IContainer` |
| `_Broadcaster` | `_Button2实例` | Button2（重启电脑）后备字段 |
| `m_Bridge` | `_Button1实例` | Button1（确定）后备字段 |
| `m_Printer` | `_Timer2实例` | Timer_frm2 后备字段 |
| `m_ReaderConfig` | `_Label_pd实例` | Label_pd 后备字段 |
| `m_PrototypeConfig` | `m_PrototypeConfig`（保留） | 定时器计数（数值语义靠注释说明） |
| `m_Instance` / `client` / `m_Manager` / `m_Tokenizer` / `_Proc` | 保留原名 | 自动属性的废弃后备字段，全工程无引用 |
| `configConfig/serviceConfig/stubConfig` | 保留原名 | 未使用常量 |
| 局部 `text` | `输入哈希` | 用户输入算出的 SHA256 |
| 局部 `text2` | `已存哈希` | 注册表里的 `n` |
| 局部 `text3` / `text4` | `当前壁纸` / `默认壁纸` |  |
| 局部 `rect` | `鼠标限制矩形` |  |
| 局部 `num/num2/num3/num4/obj/obj2` | 已删除 | On Error 状态机 |

## 关键方法与流程

### `InsertReader(object, EventArgs)` —— 「确定(需10秒)」

1. `Strings.Len(TextBox1.Text) < 6` → `Label2.Text = "密码应该是6位以上的字母或数字，请重新输入！"`、
   清空输入框、结束。
2. `输入哈希 = 卸载核心.计算密码哈希(Trim(TextBox1.Text))`（= `SHA256(密码 + "bfdshgs")` 十六进制）；
   `已存哈希 = HKCU\Software` 的 `n`。
3. `Operators.CompareString(输入哈希, 已存哈希, false) != 0` → `Label2.Text = "密码不正确，请重新输入！"`、
   清空、结束。
4. 相等：`Button1.Enabled = false`、清提示、`Cursor = WaitCursor`、**`卸载核心.执行卸载()`**、`Cursor = Default`。
5. 读 `Control Panel\Desktop` 的 `Wallpaper`；若路径里含「壁纸」两个字（管理端换过的壁纸），
   调 `ValidateReader("C:\Windows\Web\Wallpaper\Windows\img0.jpg", Stretched)` 换回系统默认。
6. `ClipCursor` 把鼠标锁在整屏矩形（`0,0`–`屏幕宽,屏幕高`）。
7. `Thread.Sleep(9000)` → `ProjectData.EndApp()` 结束进程。

整段原版在 `On Error Resume Next` 之下，反混淆后逐条 `try/catch`，且保留「密码错就结束」的提前返回。

### `StopReader(object, EventArgs)` —— Timer_frm2.Tick（每 1000 ms）

`TopMost = true` → `RECT{0,0,800,200}` → `ClipCursor` → `checked { m_PrototypeConfig++ }` →
`m_PrototypeConfig >= 5` 时 `Beep(2000, 500)`。

### `ReadReader` / `WriteReader` / `OrderReader` / `PatchReader` / `VisitReader`

- `ReadReader`：`e.Cancel = true`（不可关闭）。
- `WriteReader`：`Timer_frm2.Enabled = true`。
- `OrderReader`：`Interaction.Shell("shutdown /r /f /t 0", MinimizedFocus, false, -1)`（不等待）。
- `PatchReader`：空方法（双击状态标签无反应）。
- `VisitReader`：`Process.Start("http://www.jfglzs.com")`，但界面上没有 LinkLabel，绑定不到。

### `ValidateReader(string, WallpaperStyle)`

图片不存在 → `throw new ArgumentException("指定的图片文件不存在")`；
写 `HKCU\Control Panel\Desktop` 的 `Wallpaper = 路径`、`WallpaperStyle = "2"`；
`SystemParametersInfo(20, 0, 路径, 3)` 失败 → `throw new Win32Exception()`。
形参 `壁纸样式` 实际未被使用（样式写死 `"2"`），保留签名以免改变调用点。

## 恶意行为 / 敏感操作

| 类型 | 具体行为 |
| --- | --- |
| 强制锁屏式限制 | 无边框 + 置顶 + 最大化 + `FormClosing` 取消关闭 + 定时器 `ClipCursor` 锁鼠标 + 蜂鸣 |
| 关机/重启 | `shutdown /r /f /t 0`（强制重启，不保存任何东西） |
| 修改系统设置 | 改桌面壁纸（注册表 + `SystemParametersInfo`） |
| 结束进程 | `ProjectData.EndApp()` |
| 密码校验 | 本地哈希比对（`SHA256` + 固定盐 `bfdshgs`），没有任何服务端校验，属于「防误用」而非真安全 |
| 卸载入口 | 通过后才执行 `卸载核心.执行卸载()` —— 本程序真正的破坏/回滚动作都在这之后 |

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| UI 文案 | `卸载小助手密码：`、`确定(需10秒)`、`重启电脑` | 控件文本 |
| UI 文案 | `密码应该是6位以上的字母或数字，请重新输入！`、`密码不正确，请重新输入！` | 错误提示 |
| UI 文案 | `提醒：1、软件有问题请联系QQ：1023411046，下载最新版本请访问：www.jfglzs.com` | 作者联系方式 |
| UI 文案 | `2、卸载后记得在硬盘保护系统中创建进度或保存系统。` | 使用提示 |
| 窗口参数 | `Text = "Form2"`、`ClientSize = 637×407`、`Font = 宋体 10.5`、`Label_pd 26.25 号` | Designer 配置 |
| 定时器 | `Interval = 1000`；鼠标矩形 `0,0,800,200`；`Beep(2000, 500)` | Tick 行为 |
| 注册表 | `HKCU\Control Panel\Desktop` → `Wallpaper`、`WallpaperStyle = "2"` | 换壁纸 |
| 路径 | `C:\Windows\Web\Wallpaper\Windows\img0.jpg` | 默认壁纸 |
| 命令 | `shutdown /r /f /t 0` | 重启 |
| URL | `http://www.jfglzs.com` | 官网（死代码里） |
| API | `ClipCursor`、`SystemParametersInfo(20,…)`、`Beep` | user32 / kernel32 |
| 常量 | `20`（SPI_SETDESKWALLPAPER）、`3`（SPIF_UPDATEINIFILE\|SPIF_SENDCHANGE）、`5`（蜂鸣阈值） | 行为阈值 |
| 等待 | `Thread.Sleep(9000)` | 卸载后停留 |
