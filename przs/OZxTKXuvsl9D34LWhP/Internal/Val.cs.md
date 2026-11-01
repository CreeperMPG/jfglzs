# Val.cs.md — 进程入口 / VB `My.Application`

## 文件作用

`OZxTKXuvsl9D34LWhP\Internal\Val.cs` 是 VB 的 `My.Application`（`Val` 是混淆名），
**同时是本进程的入口点**：`przs.csproj` 里 `<StartupObject>OZxTKXuvsl9D34LWhP.Internal.Val</StartupObject>`
直接写死了这个类型，所以类名和入口方法名都不能改（本任务禁止改 csproj）。

它决定了两件事：
1. **程序允许双开**（`IsSingleInstance = false`）；
2. **主窗体是 `Form1`**——也就是那个隐藏运行、装定时器、挂 FileSystemWatcher 的管控核心，
   所以双击 exe 后看不到窗口，程序直接在后台开始"管理"。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Val : WindowsFormsApplicationBase` | VB 应用对象 + 进程入口 |
| `CallVal(string[])` | `[STAThread]` 入口：设置渲染默认值 → `Queue.MyApplication.Run(args)` |
| 构造函数 | 多开、视觉样式、退出保存设置、主窗体关闭即退出 |
| `OnCreateMainForm()` | `base.MainForm = Queue.MyForms.Form1` |
| `EnableTag()` | .NET Reactor 授权标记桩，恒 true |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| 类 `Val` | **保留原名** | csproj 的 StartupObject 写死了它 |
| `CallVal` | **保留原名** | 入口方法，改名会影响入口点解析 |
| `Queue.ResetVal` | `Queue.MyApplication` | 跨文件改名 |
| `Queue.CollectVal.Form1` | `Queue.MyForms.Form1` | 跨文件改名 |
| `EnableTag` | 保留原名 | 授权标记桩 |

## 关键方法与流程

### CallVal(string[] args) —— 进程入口

1. `Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering)`；
2. `Queue.MyApplication.Run(args)` —— 进入 WinForms 消息循环，`OnCreateMainForm` 里把主窗体设成 `Form1`。

这就是"程序一启动、主窗体一创建就触发 `Form1.Load`"的链路起点：
`Form1.Load` → 自我迁居到 `C:\Program Files (x86)\…` 或开始装限制（详见 Form1.md）。

### 构造函数

```text
IsSingleInstance   = false
EnableVisualStyles = true
SaveMySettingsOnExit = true
ShutdownStyle      = AfterMainFormCloses
```

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| **隐藏启动** | 主窗体是隐藏的 `Form1`（`WindowState.Minimized` + `ShowInTaskbar=false` + 无边框），用户看不见 |
| **允许双开** | `IsSingleInstance = false`：被查杀后重新运行不会被"已在运行"拦住 |
| 无直接文件/注册表操作 | 都在 `Form1` 里 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 入口点 | `OZxTKXuvsl9D34LWhP.Internal.Val`（csproj `StartupObject`） |
| 主窗体 | `Form1` |
| 认证模式 | `AuthenticationMode.Windows` |
| 生成器 | `MyTemplate`, `11.0.0.0` |
