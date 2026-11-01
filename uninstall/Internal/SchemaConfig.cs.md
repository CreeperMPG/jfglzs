# Internal/SchemaConfig.cs 分析

## 文件作用

一个「万能」特性类：`AttributeUsage` 覆盖程序集/类/结构/枚举/方法/属性/字段/事件/接口/参数/委托，
并且 `AllowMultiple = true`。带四个普通属性（三个 bool 默认 `true`、一个 string 默认 `""`），
只有取值/设值壳子，**没有任何业务逻辑，也没有任何地方使用它、读取它的属性**。

这类东西是混淆器/保护器塞进来的：把随机名字的特性挂到目标上，让源码看起来更乱，同时增加反射扫描噪声。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `属性容器特性`（原 `SchemaConfig`） | 通用属性袋（bag），四个属性全是 getter/setter |
| `属性1_取值/设值`（原 `InsertConfig`/`PatchConfig`） | bool 属性 1（默认 true） |
| `属性2_取值/设值`（原 `ReadConfig`/`WriteConfig`） | bool 属性 2（默认 true） |
| `属性3_取值/设值`（原 `VisitConfig`/`ValidateConfig`） | string 属性 3（默认 `""`） |
| `属性4_取值/设值`（原 `ConcatConfig`/`InitConfig`） | bool 属性 4（默认 true） |
| `混淆器占位方法_恒真()`（原 `NewParser`） | `return true` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `SchemaConfig` | `属性容器特性` | 只是属性袋，没有「schema」含义 |
| `InsertConfig` / `PatchConfig` | `属性1_取值` / `属性1_设值` | bool#1 |
| `ReadConfig` / `WriteConfig` | `属性2_取值` / `属性2_设值` | bool#2 |
| `VisitConfig` / `ValidateConfig` | `属性3_取值` / `属性3_设值` | string |
| `ConcatConfig` / `InitConfig` | `属性4_取值` / `属性4_设值` | bool#3 |
| `_UtilsConfig` | `属性1` |  |
| `_CollectionConfig` | `属性2` |  |
| `m_ComparatorConfig` | `属性4` | 字段顺序照原样（在 string 字段之前） |
| `_DescriptorConfig` | `属性3` |  |
| `NewParser` | `混淆器占位方法_恒真` |  |
| 参数 `bool_0` / `string_0` | `值` |  |

原始属性名已被随机化且语义不可恢复，因此按「第几个属性」编号，并在 XML 注释里保留原混淆名。

## 关键方法与流程

八个方法一一对应四个字段的读写：

- `属性1..4_取值()` → `return this.属性N;`
- `属性1/2/4_设值(bool 值)` → `this.属性N = 值;`
- `属性3_设值(string 值)` → `this.属性3 = 值;`

没有校验、没有副作用、没有事件。默认值：三个 bool 为 `true`，string 为 `""`。

## 恶意行为 / 敏感操作

无。价值在「反分析」：

- 特性可挂到任何目标且 `AllowMultiple = true`，能让反射枚举看到大量无意义特性；
- 类名/属性名全随机，增加阅读成本；
- 是**判断该样本经过专业混淆**的又一个证据（与 `IndexerConfig` 的混淆桩同类手法）。

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 特性用法 | `AttributeTargets` 覆盖 11 种目标、`AllowMultiple = true`、`Inherited = false`、`[ComVisible(true)]` | 万能特性声明 |
| 默认值 | `true`、`true`、`true`、`""` | 四个属性初值 |
| Token | 类型 `0x02000018`；方法 `0x0600009E..A7`；字段 `0x04000044..47` | 元数据行号 |
