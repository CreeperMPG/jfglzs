# Internal\ProducerCandidate.cs —— 文件级分析

## 文件作用
一个可多次应用（`AllowMultiple = true`、`Inherited = false`）的自定义特性：三个布尔开关（默认全 `true`）+ 一个字符串（默认 `""`）。类名与成员名都被 .NET Reactor 写成了随机词。**程序集内没有任何 `[ProducerCandidate]` 应用点，也没有任何代码构造它**，因此原语义无法静态确定，属于死代码/元数据残留。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `ProducerCandidate : Attribute` | 自定义特性容器；可作用于程序集/类/结构/枚举/方法/属性/字段/事件/接口/参数/委托 |
| `取开关一/二/三()` | 三个布尔字段的 getter，默认值 `true` |
| `设开关一/二/三(bool)` | 三个布尔字段的 setter |
| `取文本项()` / `设文本项(string)` | 字符串字段的读写，默认 `""` |
| `恒真桩()` | 恒返回 `true` 的死代码桩，无调用点 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `PopAttr()` | `取开关一()` | getter → `_ConfigurationCandidate`（默认 true） |
| `ReflectAttr(bool)` | `设开关一(bool)` | setter → `_ConfigurationCandidate` |
| `LogoutAttr()` | `取开关二()` | getter → `_ValueCandidate`（默认 true） |
| `AssetAttr(bool)` | `设开关二(bool)` | setter → `_ValueCandidate` |
| `ConnectAttr()` | `取文本项()` | getter → `poolCandidate`（默认空串） |
| `CustomizeAttr(string)` | `设文本项(string)` | setter → `poolCandidate` |
| `NewAttr()` | `取开关三()` | getter → `_AdapterCandidate`（默认 true） |
| `InstantiateAttr(bool)` | `设开关三(bool)` | setter → `_AdapterCandidate` |
| `SortFactory()` | `恒真桩()` | `return true;`，无调用点 |
| 字段 `_ConfigurationCandidate` / `_ValueCandidate` / `_AdapterCandidate` / `poolCandidate` | `_开关一` / `_开关二` / `_开关三` / `_文本项` | 原用途不可判定，按序号命名避免撞车 |
| `ProducerCandidate` | 未改名 | 类名保留原样，避免破坏跨程序集引用 |

## 关键方法与流程（按方法分段，写清步骤）
- 六个访问器都是最直白的字段读写，没有任何校验、惰性初始化或副作用。
- 字段初始化即默认值：`_开关一 = _开关二 = _开关三 = true`，`_文本项 = ""`。
- 注意这不是自动属性：它是**成对的方法**（`取xxx()` / `设xxx()`），不是 `property`，调用方需显式调用方法。

## 恶意行为 / 敏感操作
- 无。文件不含文件/注册表/进程/网络操作。
- 仍未排除的可能：这类 `AllowMultiple` 三元布尔特性在某些壳里用于标记「已混淆/已加密」的元数据。静态上没有证据，记录为**用途未知的死代码**。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 空字符串 `""`（文本项默认值）
- 无路径、无 URL、无注册表键。

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无 `IL_xxxx:` / `num2 = N` / `ClearProjectError()`
- 保留全部 14 条 `// Token:` 注释
- `AttributeUsage` 的目标列表与 `AllowMultiple/Inherited` 取值原样保留
