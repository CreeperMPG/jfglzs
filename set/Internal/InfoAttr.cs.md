# Internal\InfoAttr.cs —— 文件级分析

## 文件作用
**程序入口**。本类是 VB.NET「Windows 应用程序框架」宿主（`WindowsFormsApplicationBase` 子类），`主入口(string[])` 就是 `Main`（csproj 的 `<StartupObject>set.Internal.InfoAttr</StartupObject>` 指向它）。启动流程极短：固定 WinForms 文本渲染方式 → 交给 VB 应用框架 `Run()` → 框架回调 `OnCreateMainForm` 指定首个窗体 `load`。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `InfoAttr : WindowsFormsApplicationBase` | 应用宿主；负责应用框架初始化与主窗体指定 |
| `主入口(string[] args)`（旧名 `InterruptInfo`） | `Main`：设置文本渲染默认值后启动应用框架 |
| `InfoAttr()` | 构造：单实例=否、启用视觉样式、退出保存 My.Settings、主窗体关闭即退出 |
| `OnCreateMainForm()` | 覆盖：把 `base.MainForm` 设为 `CandidateAttr.窗体集合.load`（载入/卡密验证窗体） |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `InterruptInfo(string[])` | `主入口(string[])` | 带 `[STAThread]` 的 `Main`，改名后仍是该类的入口方法 |
| `CandidateAttr.RestartInfo` | `CandidateAttr.应用程序` | VB 的 `My.Application`（跨文件同步改名） |
| `CandidateAttr.ComputeInfo` | `CandidateAttr.窗体集合` | VB 的 `My.Forms`（跨文件同步改名） |
| `InfoAttr` | 未改名 | 类名被 csproj 的 StartupObject 引用，不能改 |

## 关键方法与流程（按方法分段，写清步骤）
1. `主入口(args)`
   1. `Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering)` —— 按 VB 的 `UseCompatibleTextRendering`（false）固定控件文本渲染方式。**顺序不能动**：必须在任何控件创建之前。
   2. `CandidateAttr.应用程序.Run(args)` —— 启动 VB 应用框架；框架内部会 new 本类并调用 `OnCreateMainForm`。
2. `InfoAttr()` 构造（先 `base(AuthenticationMode.Windows)`）
   1. `IsSingleInstance = false` —— 允许同一台机器上多开。
   2. `EnableVisualStyles = true` —— 启用 XP 视觉样式。
   3. `SaveMySettingsOnExit = true` —— 退出时保存 My.Settings（见 `MySettings.Designer.cs` 的 `AutoSaveSettings`）。
   4. `ShutdownStyle = AfterMainFormCloses` —— 主窗体关闭后整个进程退出。
3. `OnCreateMainForm()`
   1. `base.MainForm = CandidateAttr.窗体集合.load` —— 注意：启动窗体是 `load`（卡密/载入窗口），**不是** `Form1`。也就是说真正的「设置主界面」要等 `load` 里的验证通过后才显示。

## 恶意行为 / 敏感操作
- 本文件本身没有恶意代码，只是启动器。
- 值得注意的**控制流事实**：程序一启动就进 `load` 窗体（卡密校验），校验通过才进 `Form1`（真正干坏事的地方：改 hosts、杀进程、锁屏/关机）。分析恶意行为时应从 `load` 入手。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 无字符串常量、无路径。
- 关键符号：`set.Internal.InfoAttr`（StartupObject）、`load`（首个窗体类名）。

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无
- 保留 4 条 `// Token:` 注释
- 跨文件改名联动：本文件的 `CandidateAttr.RestartInfo` / `CandidateAttr.ComputeInfo` 已同步改成 `CandidateAttr.应用程序` / `CandidateAttr.窗体集合`（与 `Internal\CandidateAttr.cs`、`WindowsApplication1\My\MySettings.Designer.cs` 保持一致）
