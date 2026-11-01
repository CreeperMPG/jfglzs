## 文件作用

VB.NET 的 **`My.Application`** 对象（编译产物里叫 `MyApplication`，原混淆名 `ParamsStrategy`）。
它派生自 `Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase`，
只是模板生成的转发壳：让代码能写 `My.Application.XXX`。

本工程里没有任何地方使用它的成员（没有命令行参数处理、没有启动/关闭事件），
所以这个类对行为分析没有贡献——但它属于「VB 工程标准骨架」，不应该被误当作恶意代码。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MyApplication : ConsoleApplicationBase` | VB 的 `My.Application` 入口；没有自有逻辑、没有成员、没有初始化 |
| `RevertFacade()` / `SearchDefinition()` | 壳注入空桩：`return true` / `return false`，无调用点 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ParamsStrategy` | `MyApplication` | 由 `[GeneratedCode("MyTemplate", "11.0.0.0")]` + 基类 `ConsoleApplicationBase` 可确认它就是 `My.Application` 的宿主类型 |
| `RevertFacade()` | 保持原名 | 壳注入空桩（`true`） |
| `SearchDefinition()` | 保持原名 | 壳注入空桩（`false`） |

## 关键方法与流程

只有两个静态空桩：

- `RevertFacade()` → `return true;`
- `SearchDefinition()` → `return false;`

都不读参数、不改状态、无调用点。类本身没有构造函数（用默认无参构造）。

## 恶意行为 / 敏感操作

无。

补充一点分析价值：`ConsoleApplicationBase` 说明本项目虽然是 `WinExe`，但 VB 模板按「控制台应用」生成了 `My.Application`，
这与 `StartupObject=zmserv.ProcessMonitorService`（服务类作为入口）是自洽的——服务型 VB 工程的典型形态。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 特性 | `[GeneratedCode("MyTemplate", "11.0.0.0")]`、`[EditorBrowsable(EditorBrowsableState.Never)]` |
| 基类 | `Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase` |
| 路径 / 注册表 / URL / 进程名 / 字符串常量 | 无 |
