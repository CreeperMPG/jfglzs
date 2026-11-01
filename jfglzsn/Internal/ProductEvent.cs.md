## 文件作用

强类型资源访问器，等价于 VB.NET 模板的 `My.Resources`。资源清单名写死为 `WindowsApplication1.Resources`（对应工程里的 `Resources.resx` / `g.resx`）。

模板只留下 `ResourceManager` 与 `Culture` 两个基础访问器，**没有定义任何具名资源属性**；另外被塞进两个恒返回常量的判定桩。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ProductEvent`（`[StandardModule]`） | My.Resources 访问入口 |
| `ResourceManager` 属性 | 懒加载 `new ResourceManager("WindowsApplication1.Resources", typeof(ProductEvent).Assembly)` |
| `Culture` 属性 | 资源查找的区域文化（`null` = 跟随当前线程） |
| `FlushDefinition()` / `AddDefinition()` | 桩：恒 `true` / 恒 `false`，工程内无引用 |
| `resourceMan` / `resourceCulture` | 缓存字段 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ProductEvent` | 未改名 | 类名与文件绑定；实为 `My.Resources` 替身 |
| `FlushDefinition` / `AddDefinition` | 未改名 | 桩方法，无语义可推断 |
| `resourceMan` / `resourceCulture` | 未改名 | 模板生成字段，名字本身已可读 |
| `resourceCulture`（类型 `object`） | 未改名、**不可改类型** | 声明成 `object` 但实际存 `CultureInfo`，是模板生成的怪点，改成 `CultureInfo` 会改动类型签名；getter 里补了显式 `(CultureInfo)` 转换（= VB 后期绑定转换的显式化，类型不对同样抛 `InvalidCastException`），否则 dnSpy 产物在 C# 里根本编译不过 |

## 关键方法与流程

### `ResourceManager` getter

1. 用 `object.ReferenceEquals(resourceMan, null)` 判空（不是 `==`，模板生成如此）。
2. 为空则构造 `ResourceManager("WindowsApplication1.Resources", typeof(ProductEvent).Assembly)` 并缓存。
3. 返回缓存实例。

注意这是**非线程安全**的懒加载（没有锁），与原版一致。

### `Culture` getter / setter

直接读写 `resourceCulture` 字段；`null` 表示跟随线程文化。

### 桩方法

`FlushDefinition()` → `true`；`AddDefinition()` → `false`。工程内无调用。

## 恶意行为 / 敏感操作

无。本文件只做资源访问。

一个**值得注意的空白**：本工程资源里没有实际被引用的业务字符串——所有恶意文案都是硬编码在 `Form1`/`Form2`/`Form3`/`Form4` 里的，不在 `g.resx`。这意味着改文案不需要动资源，也意味着资源文件对分析没有情报价值。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 资源清单名 | `WindowsApplication1.Resources` |
| 生成器标记 | `[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]` |
| 编译器常量 | `true` / `false` |
