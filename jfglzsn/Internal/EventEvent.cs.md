## 文件作用

VB.NET 模板生成的 `My.Computer` 替身类型（继承 `Microsoft.VisualBasic.Devices.Computer`）。除模板构造外，**额外被塞进两个恒返回常量的静态方法**，属于壳/混淆器注入的假判定桩。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `EventEvent : Computer` | 提供 My.Computer 实例（被 `SetterEvent.CalculateCallback` 懒加载），本工程内没有业务代码使用它 |
| `InstantiateDefinition()` | 桩：恒 `true`，工程内无引用 |
| `StopDefinition()` | 桩：恒 `false`，工程内无引用 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `EventEvent` | 未改名 | 类名与文件名绑定；它是 `My.Computer` 的替身 |
| `InstantiateDefinition` | 未改名 | 方法体仅 `return true`，无 API / 字符串可推断语义 |
| `StopDefinition` | 未改名 | 方法体仅 `return false` |

## 关键方法与流程

无业务流程。`EventEvent()` 构造为空实现；两个静态方法各一条 `return`。

这两个桩与 `ILLicenseModule.ListRule/InitRule`、`CallbackEvent.CountDefinition/SelectDefinition`、`ProductEvent.FlushDefinition/AddDefinition`、`RuleSetter.InterruptRule/MoveRule`、`SetterEvent` 内 4 组、`ListSetter` 内 1 组、`Form3.GetRule`、`MethodSetter.ForgotRule/DefineRule` 完全同构——共 **9 组、17 个**「一真一假」空方法，是 .NET Reactor 批量注入的噪声。

## 恶意行为 / 敏感操作

无。本文件不含可执行逻辑，也不访问任何系统资源。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 生成器标记 | `[GeneratedCode("MyTemplate", "11.0.0.0")]` |
| 编译器常量 | `true` / `false` |
