## 文件作用

VB.NET 的 **`My.Resources`**（`Resources.resx` 的强类型访问器，编译产物里叫 `MyResources`，原混淆名 `TemplateStrategy`）。

职责只有一个：按需惰性创建 `ResourceManager("zmserv.Resources", 本程序集)`，并提供 `Culture` 开关。
工程里 `Resources.resx` / `g.resx` 都是**空资源表**（只有 schema，没有任何 `<data>` 项），
所以本类取不到任何值——真正的秘密在加壳器塞进去的两份匿名 GUID 嵌入资源里（见 `AdvisorStrategy.cs.md`）。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MyResources`（`[StandardModule]`） | `My.Resources` 入口；惰性 `ResourceManager` + `Culture` |
| `ResourceManager`（属性） | 首次访问时 `new ResourceManager("zmserv.Resources", typeof(MyResources).Assembly)`，之后复用 |
| `Culture`（属性） | 资源查询的语言区域，默认 `null`（跟随当前线程） |
| `FindDefinition()` / `TestDefinition()` | 壳注入空桩：`return true` / `return false`，无调用点 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `TemplateStrategy` | `MyResources` | 由 `[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder","16.0.0.0")]` + `ResourceManager("zmserv.Resources", …)` 确认身份 |
| `resourceMan` | 保持原名 | 模板生成的缓存字段，名字本身就是语义化的 |
| `resourceCulture` | 保持原名 | 同上；类型是 `object`（要能同时接 `null` 与 `CultureInfo`） |
| `FindDefinition()` / `TestDefinition()` | 保持原名 | 壳注入空桩 |

## 关键方法与流程

### `ResourceManager` 的 getter
1. `if (object.ReferenceEquals(resourceMan, null))` → `resourceMan = new ResourceManager("zmserv.Resources", typeof(MyResources).Assembly)`；
2. 返回 `resourceMan`。

注意用的是 `object.ReferenceEquals(x, null)` 而不是 `x == null`——VB 模板固定输出这种形式，语义等价，不必改。

### `Culture` 的 getter / setter
- get → `return resourceCulture;`
- set → `resourceCulture = value;`

## 恶意行为 / 敏感操作

无。仅说明「业务资源表是空的」，从而把注意力引向壳的匿名资源（`9f3465d1-…`、`08d03c1e-…`）。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 资源清单名 | `zmserv.Resources` |
| 特性 | `[StandardModule]`、`[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]`、`[DebuggerNonUserCode]`、`[CompilerGenerated]`、`[HideModuleName]` |
| 路径 / 注册表 / URL / 进程名 | 无 |
