# Internal/Importer.cs 分析

## 文件作用

VB.NET 自动生成的 **My.Application**（`WindowsFormsApplicationBase` 派生）。
它同时是**程序入口**：csproj 里 `<StartupObject>WindowsApplication1.Internal.Importer</StartupObject>`
指向本类，`Main(string[])`（原 `VerifyReader`）是真正的 `Sub Main`。

职责只有三件：固定文本渲染开关、设置单实例/视觉样式/退出保存/关闭策略、把主窗体定为 `Form1`。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `Importer` | My.Application 实现 + 程序入口（类名被 csproj 引用，**不能改**） |
| `Main(string[])`（原 `VerifyReader`） | 程序入口：`SetCompatibleTextRenderingDefault` → `MyProject.Application.Run(参数)` |
| `Importer()` | 构造函数：四个应用级开关全部硬编码 |
| `OnCreateMainForm()` | `MainForm = MyProject.Forms.Form1`（隐藏的卸载窗） |
| `混淆器占位方法_恒真()`（原 `CallParser`） | `return true`，无调用者 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `Importer` | 保留 | csproj 的 `StartupObject` 写死了这个类名，改名会导致工程入口丢失 |
| `VerifyReader` | `Main` | 它就是 VB 生成的 `Sub Main`；改为 `Main` 既符语义又让工程可正常编译 |
| `CallParser` | `混淆器占位方法_恒真` | 占位 |
| 参数 `args` | `命令行参数` |  |

## 关键方法与流程

### `Main(string[])`

```
Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
MyProject.Application.Run(命令行参数);
```

`UseCompatibleTextRendering` 在这里是**静态属性**（值来自编译期设置），所以这一行等价于把
`SetCompatibleTextRenderingDefault(false)` 固定下来，再交给 `WindowsFormsApplicationBase.Run` 启动消息循环。

### 构造函数

| 设置 | 值 | 影响 |
| --- | --- | --- |
| `IsSingleInstance` | `false` | 允许多开（卸载器可以同时跑多份） |
| `EnableVisualStyles` | `true` | 启用 XP 视觉样式 |
| `SaveMySettingsOnExit` | `true` | 退出时保存 My.Settings（`MySettings.AutoSaveSettings` 的开关） |
| `ShutdownStyle` | `AfterMainFormCloses` | 主窗体关闭即退出 |
| `AuthenticationMode` | `Windows` | 传给基类 |

### `OnCreateMainForm`

`base.MainForm = MyProject.Forms.Form1;` —— 走 `My.Forms` 单例，保证全程序只有一个 `Form1` 实例。
所以启动后显示的是那个 385×224、最小化、不进任务栏的隐藏窗体（见 `Form1.Designer.cs.md`）。

## 恶意行为 / 敏感操作

无直接恶意行为。它在链路里的位置是「一切从这里开始」：

- 入口 → `Form1.Load` → 要么直接 `执行卸载()`，要么 `Form2` 要密码；
- `SaveMySettingsOnExit = true` 让 `MySettings` 的退出保存链路生效（无实际设置项，但会建立事件挂接）。

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 入口 | `WindowsApplication1.Internal.Importer`（csproj `StartupObject`） | 启动类 |
| 特性 | `[STAThread]`、`[EditorBrowsable(Advanced)]`、`[GeneratedCode("MyTemplate","11.0.0.0")]` | WinForms 入口 / 模板标记 |
| 开关 | `IsSingleInstance=false`、`EnableVisualStyles=true`、`SaveMySettingsOnExit=true`、`ShutdownStyle=AfterMainFormCloses` | 应用级配置 |
| Token | 类型 `0x02000002`；方法 `0x06000001..04` | 元数据行号 |
