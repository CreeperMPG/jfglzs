# Internal/ComposerMapper.cs

## 文件作用

VB.NET 应用程序模型（`My.Application`）的实现类，同时是本程序集的**启动对象**：`about.csproj` 的 `<StartupObject>` 指向 `WindowsApplication1.Internal.ComposerMapper`。它决定了整个程序的启动链、退出条件和窗口生命周期。

启动链：`程序入口`（STAThread）→ `My.Application.Run(args)` → `OnCreateMainForm` → 取 `My.Forms.Form1` 单例 → 显示 `学生机房管理助手v13.4` 主窗体。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ComposerMapper`（继承 `WindowsFormsApplicationBase`） | 应用程序模型；设定单实例/视觉样式/退出策略；指定主窗体为 Form1 |
| `程序入口(string[])` | 真正的 Main：设置文本渲染兼容模式后进入消息循环 |
| `OnCreateMainForm()` | 覆写主窗体创建：`MainForm = My.Forms.Form1` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ReflectComposer(string[])` | `程序入口(string[] args)` | 带 `[STAThread]`，是启动对象里的 Main |
| `ComposerMapper()` | 保留（构造函数） | 配置 `WindowsFormsApplicationBase` 各项开关 |
| `OnCreateMainForm()` | 保留原名 | 基类约定的覆写点，不可改名 |
| `ConnectFacade()` | `返回真占位()` | 恒 `return true;` |
| `ComposerMapper`（类名） | 保留 | 类名=文件名 |

## 关键方法与流程

1. **`程序入口(string[] args)`**
   - `Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering)`——按 VB 约定关闭 GDI+ 文本渲染；
   - `ExpressionMapper.CancelComposer.Run(args)`——`My.Application` 进入 `WindowsFormsApplicationBase` 的消息循环。
2. **构造函数**
   - `base(AuthenticationMode.Windows)`；
   - `IsSingleInstance = false`——不做单实例检查，可同时运行多份；
   - `EnableVisualStyles = true`——启用 XP 以后的视觉样式；
   - `SaveMySettingsOnExit = true`——退出时触达 `MySettings.AutoSaveSettings`；
   - `ShutdownStyle = AfterMainFormCloses`——主窗体关闭即结束进程。
3. **`OnCreateMainForm()`**
   - `base.MainForm = ExpressionMapper.VisitComposer.Form1;`——第一次访问 `My.Forms.Form1` 时才构造窗体。

## 恶意行为 / 敏感操作

无直接的恶意动作，但它是**持久化的入口点**：只要 `Form1` 被构造，其构造函数就会挂上 `Load` 处理器 `PatchComposer`（生成随机助手进程名），并初始化整窗 UI。也就是说，程序一启动就会完成「助手进程名生成」这一准备动作。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 启动对象 | `WindowsApplication1.Internal.ComposerMapper`（见 `about.csproj` `<StartupObject>`） |
| 单实例 | 关闭（`IsSingleInstance = false`） |
| 退出策略 | `ShutdownMode.AfterMainFormCloses` |
| 模板标识 | `[GeneratedCode("MyTemplate", "11.0.0.0")]` |
| IL 行号 | 类型 `0x02000002`，方法 `0x06000001`–`0x06000004` |
