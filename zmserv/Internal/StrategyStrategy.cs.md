## 文件作用

VB.NET 的 **`My.Computer`** 对象（编译产物里叫 `MyComputer`，原混淆名 `StrategyStrategy`）。
派生自 `Microsoft.VisualBasic.Devices.Computer`，模板生成的转发壳。

本工程的业务代码**一次都没用过** `My.Computer`（没有用它的文件/注册表/时钟/网络能力——
关机是直接 P/Invoke `ntdll` 干的，见 `ProcessMonitorService.cs`）。所以本文件对行为分析无贡献。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MyComputer : Computer` | VB 的 `My.Computer` 入口；空类，无自有逻辑 |
| `StartDefinition()` / `WriteDefinition()` | 壳注入空桩：`return true` / `return false`，无调用点 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `StrategyStrategy` | `MyComputer` | 由 `[GeneratedCode("MyTemplate","11.0.0.0")]` + 基类 `Computer` 确认身份 |
| `StartDefinition()` | 保持原名 | 壳注入空桩（`true`） |
| `WriteDefinition()` | 保持原名 | 壳注入空桩（`false`） |

## 关键方法与流程

- 构造函数：空（`[EditorBrowsable(Never)]`，模板生成）。
- `StartDefinition()` → `return true;`
- `WriteDefinition()` → `return false;`

无任何流程。

## 恶意行为 / 敏感操作

无。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 特性 | `[GeneratedCode("MyTemplate", "11.0.0.0")]`、`[EditorBrowsable(EditorBrowsableState.Never)]` |
| 基类 | `Microsoft.VisualBasic.Devices.Computer` |
| 路径 / 注册表 / URL / 进程名 / 字符串常量 | 无 |
