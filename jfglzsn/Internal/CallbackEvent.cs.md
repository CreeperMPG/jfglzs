## 文件作用

程序启动入口（等价于 VB.NET 模板的 `My.Application`）。`[STAThread]` 的 `GetCallback` 就是进程的 `Main`：设置 WinForms 文本渲染兼容模式，然后把控制权交给 `SetterEvent.CollectCallback.Run`，由应用框架创建主窗体 Form1。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `CallbackEvent : WindowsFormsApplicationBase` | 应用框架宿主：定义单实例策略、视觉样式、退出时保存设置、关闭策略、主窗体来源 |
| `GetCallback(string[])` | 进程入口点（STA 线程） |
| `OnCreateMainForm` | 指定主窗体为 `Form1`（从 `My.Forms` 懒加载容器取） |
| `CountDefinition` / `SelectDefinition` | 壳注入的判定桩（恒 `true` / 恒 `false`），无引用 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `CallbackEvent` | 未改名 | 类名与文件名绑定，且它是模板生成的 `My.Application` 替身 |
| `GetCallback(string[] args)` | 未改名（语义：进程入口 Main） | 由 IL 入口点直接引用，改名不影响运行，但会遮蔽「这就是 Main」这一事实，故保留并在注释说明 |
| `SetterEvent.CollectCallback` | 未改名（语义：My.Application 单例入口） | 跨文件公共 API，见 `SetterEvent.cs.md` |
| `SetterEvent.ComputeCallback` | 未改名（语义：My.Forms 容器入口） | 同上 |
| `CountDefinition` / `SelectDefinition` | 未改名 | 恒真/恒假桩，无语义可推断 |

## 关键方法与流程

### `GetCallback(string[] args)` — 进程入口

1. `Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering)`：必须在任何窗体创建之前调用，决定 GDI+ / GDI 文本渲染（本样本取 `false`）。
2. `SetterEvent.CollectCallback.Run(args)`：把命令行参数交给 `WindowsFormsApplicationBase.Run`，进入 WinForms 消息循环。

### `CallbackEvent()` — 框架配置

| 属性 | 取值 | 含义 |
| --- | --- | --- |
| `AuthenticationMode` | `Windows` | 使用当前 Windows 身份 |
| `IsSingleInstance` | `false` | **允许同时运行多份**（配合「进程名随机 + 服务守护」的存活策略） |
| `EnableVisualStyles` | `true` | 启用视觉样式 |
| `SaveMySettingsOnExit` | `true` | 退出时保存 My.Settings（另见 `MySettings.Designer.cs` 里额外的保存钩子） |
| `ShutdownStyle` | `AfterMainFormCloses` | 主窗体关闭即退出 |

### `OnCreateMainForm()`

`base.MainForm = SetterEvent.ComputeCallback.Form1;` —— 通过 `My.Forms` 容器按需构造 Form1。启动链完整路径：

`GetCallback` → `SetterEvent.CollectCallback.Run` → `OnCreateMainForm` → `My.Forms.Form1` → `Form1()` 构造 → `Form1_Load`（`窗体加载初始化`）。

## 恶意行为 / 敏感操作

本文件自身只做启动配置，但有两条值得注意的「为恶意行为服务」的设定：

1. **`IsSingleInstance = false`**：不限制单实例，配合 `Form1` 里「服务 zmserv + 随机进程名副本」的守护链，学生杀掉进程后还能被重新拉起、甚至多副本共存。
2. **`SaveMySettingsOnExit = true` + `MySettings` 里的 Shutdown 钩子**：退出时会把设置落盘，属于持久化痕迹（user.config）。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 主窗体类型 | `WindowsApplication1.Form1` |
| 无字符串常量 | 本文件不含任何业务字符串 |
