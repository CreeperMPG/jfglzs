# Internal/Template.cs 分析

## 文件作用

VB.NET 自动生成的 **My.Computer**（`Microsoft.VisualBasic.Devices.Computer` 派生）。
混淆器只把类名改成了 `Template`，类本身没有任何逻辑——它只是 `My.Computer` 的入口对象
（提供 `Registry` / `FileSystem` / `Info` 等成员，都是基类给的）。

本程序真正用到的是 `MyProject.Computer.Registry`（在 `MappingConfig` 里读写注册表），
所以这条链路是「活的」，只是本文件自己没有额外代码。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MyComputer`（原 `Template`） | My.Computer 的服务入口，继承 `Computer` |
| `MyComputer()` | 空构造 |
| `混淆器占位方法_恒真()`（原 `PatchParser`） | `return true`，无调用者 |
| `混淆器占位方法_恒假()`（原 `PushParser`） | `return false`，无调用者 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `Template` | `MyComputer` | `[HelpKeyword("My.Computer")]` 指的就是它 |
| `PatchParser` | `混淆器占位方法_恒真` | 占位 |
| `PushParser` | `混淆器占位方法_恒假` | 占位 |
| 调用点 `Test.SetReader`（原） | `MyProject.Computer` | 见 `Internal/Test.cs.md` |

## 关键方法与流程

无流程。构造函数为空；两个静态方法恒返回常量。实例由 `MyProject.MyGroupCollection<MyComputer>`
按线程懒创建（见 `Internal/Test.cs.md`）。

## 恶意行为 / 敏感操作

无直接行为。需要注意的只是「它是一个能力入口」：

- `Computer.Registry` 能读写注册表（`MappingConfig` 用它读写 HKLM/HKCU）；
- 真正的敏感操作发生在调用方（`MappingConfig.执行卸载`），不在本文件。

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 特性 | `[EditorBrowsable(Never)]`、`[GeneratedCode("MyTemplate", "11.0.0.0")]` | 模板产物标记 |
| 基类 | `Microsoft.VisualBasic.Devices.Computer` | My.Computer 的真实实现 |
| Token | 类型 `0x02000003`；方法 `0x06000005/06/07` | 元数据行号 |
