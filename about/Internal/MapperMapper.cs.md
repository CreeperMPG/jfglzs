# Internal/MapperMapper.cs

## 文件作用

VB.NET 「我的电脑」（`My.Computer`）实现类。VS 的 My 模板固定生成它，用于承载 `My.Computer` 上的自定义成员；本工程一个都没加，于是只剩一个空构造函数和两个恒值桩。

真正被业务使用的 `My.Computer` 入口在 `ExpressionMapper.SetComposer`，它通过 `线程级单例<MapperMapper>` 惰性创建本类型的实例。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MapperMapper`（继承 `Microsoft.VisualBasic.Devices.Computer`） | `My.Computer` 的实现载体；无自定义成员，等价于直接使用基类 |
| 空构造函数 | 供 `ExpressionMapper` 内的单例创建器 `new` 出来 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `SetFacade()` | `返回真占位()` | 恒 `return true;` |
| `IncludeFacade()` | `返回假占位()` | 恒 `return false;` |
| `MapperMapper`（类名） | 保留 | 类名=文件名，按规范不改 |

## 关键方法与流程

1. **`MapperMapper()`**：空构造函数，无语句。
2. **`返回真占位()` / `返回假占位()`**：各自一条 `return` 常量，无副作用、无调用点。

## 恶意行为 / 敏感操作

无。基类 `Computer` 只提供本机信息读取（时钟、键盘、鼠标、剪贴板、文件系统等）的标准封装，本类没有覆写、也没有增加任何敏感操作。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 模板标识 | `[GeneratedCode("MyTemplate", "11.0.0.0")]` |
| 基类 | `Microsoft.VisualBasic.Devices.Computer` |
| IL 行号 | 类型 `0x02000003`，方法 `0x06000005` / `0x06000006` / `0x06000007` |
