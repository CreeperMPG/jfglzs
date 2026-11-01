## 文件作用

壳注入的**标记属性（Attribute）**，可标注到程序集、类型、成员、参数等几乎所有目标上（`AllowMultiple = true, Inherited = false`），并且 `[ComVisible(true)]`。

它携带 3 个布尔标志（默认全 `true`）+ 1 个字符串标志（默认空串）。全工程搜索确认：**没有任何位置把它当特性用过，也没有任何代码 new 它**，因此它只是加壳阶段的元数据容器，对运行时行为无影响。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `RuleSetter : Attribute` | 空属性类，4 个私有标志 + 4 组 get/set 方法 |
| `SearchEvent` / `EnableEvent` | 读写 `roleSetter`（bool，默认 true） |
| `NewEvent` / `ViewEvent` | 读写 `m_DecoratorSetter`（bool，默认 true） |
| `PushEvent` / `SortEvent` | 读写 `_DefinitionSetter`（bool，默认 true） |
| `ManageEvent` / `InvokeEvent` | 读写 `_StateSetter`（string，默认 `""`） |
| `InterruptRule` / `MoveRule` | 桩：恒 `true` / 恒 `false`，无引用 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `RuleSetter` | 未改名 | 类名与文件绑定 |
| `SearchEvent` / `EnableEvent` | 未改名 | 分别是 `roleSetter` 的 getter / setter。四个成员名（`Search/Enable`、`New/View`、`Push/Sort`、`Manage/Invoke`）由壳随机指派，互相之间毫无语义关系；**因为无法从代码推出真实含义，刻意保留原名**（遵循 AGENTS.md「沿用反编译出来的名字」），硬套"看起来有意义"的名字反而会误导后续分析 |
| `roleSetter` | 未改名 | 同上，语义不可推断 |
| `m_DecoratorSetter` | 未改名 | 同上 |
| `_DefinitionSetter` | 未改名 | 同上 |
| `_StateSetter` | 未改名 | 同上 |
| `bool_0`（两处参数） | `值` | 纯占位参数名 |
| `string_0`（一处参数） | `值` | 纯占位参数名 |
| `InterruptRule` / `MoveRule` | 未改名 | 恒真/恒假桩 |

**为什么不把 get/set 方法改成属性**：改成 `public bool X { get; set; }` 会改变反射面（方法名从 `get_X` 变成 `SearchEvent`），属于语义改动，按 AGENTS.md「五、不许美化的语义」不动。

## 关键方法与流程

无流程，全是单行读写。逐个：

| 方法 | 行为 |
| --- | --- |
| `SearchEvent()` | `return this.roleSetter;` |
| `EnableEvent(bool 值)` | `this.roleSetter = 值;` |
| `NewEvent()` | `return this.m_DecoratorSetter;` |
| `ViewEvent(bool 值)` | `this.m_DecoratorSetter = 值;` |
| `ManageEvent()` | `return this._StateSetter;` |
| `InvokeEvent(string 值)` | `this._StateSetter = 值;` |
| `PushEvent()` | `return this._DefinitionSetter;` |
| `SortEvent(bool 值)` | `this._DefinitionSetter = 值;` |
| `InterruptRule()` | `return true;` |
| `MoveRule()` | `return false;` |

## 恶意行为 / 敏感操作

无。仅一个观察：`[ComVisible(true)]` 让该类型对 COM 可见，但类型本身是 `internal sealed`、也没有注册任何 COM 接口，实际上不可从外部创建——属于壳的随手标记。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 默认值 | `true` / `true` / `true` / `""` |
| AttributeUsage 目标 | Assembly、Class、Struct、Enum、Method、Property、Field、Event、Interface、Parameter、Delegate |
