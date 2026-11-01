# Internal/DescriptorMapper.cs

## 文件作用

一个自定义 `Attribute` 类型，结构上等价于 .NET 自带的 `System.Reflection.ObfuscationAttribute`，是 .NET Reactor 随程序集一起塞进来的「混淆控制属性」。

在本工程里它**既没有被标注到任何成员上，也没有被任何代码读取**（全目录 grep 只有它自己的定义）。所以它对运行时行为零影响，只用于对齐原元数据。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `DescriptorMapper : Attribute` | 混淆控制属性；四个开关（排除 / 应用于成员 / 混淆后剥离 / 功能名），默认 `true,true,true,""` |
| 四个 `取/设` 方法对 | 对上述开关的读写访问器（属性风格的 get/set，但写成了方法） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `listMapper` | `排除标记` | 对应 `ObfuscationAttribute.Exclude` |
| `AwakeMapper()` / `ManageMapper(bool)` | `取排除标记()` / `设排除标记(bool)` | 上述字段的读写 |
| `m_ProducerMapper` | `应用于成员` | 对应 `ApplyToMembers` |
| `DeleteMapper()` / `InvokeMapper(bool)` | `取应用于成员()` / `设应用于成员(bool)` | 读写 |
| `m_CandidateMapper` | `功能名` | 对应 `Feature`，默认空串 |
| `SortMapper()` / `InsertMapper(string)` | `取功能名()` / `设功能名(string)` | 读写 |
| `collectionMapper` | `混淆后剥离` | 对应 `StripAfterObfuscation` |
| `DefineMapper()` / `ResetMapper(bool)` | `取混淆后剥离()` / `设混淆后剥离(bool)` | 读写 |
| `CheckFacade()` / `PublishFacade()` | `返回真占位()` / `返回假占位()` | 恒值桩 |
| `bool_0` / `string_0` | `值` | 各访问器的入参 |

## 关键方法与流程

1. **构造**：隐式无参构造，四个字段走字段初始化器（`true/true/true/""`）。
2. **`取/设` 八件套**：全部是 `return this.字段;` / `this.字段 = 值;`，无校验、无副作用。
3. **`返回值占位` 两件套**：常量返回，无调用点。

## 恶意行为 / 敏感操作

无。纯数据载体。唯一值得记的是 `[ComVisible(true)]` 与 `AllowMultiple = true, Inherited = false` 的 `AttributeUsage`——这是混淆器为了能往任意目标上反复贴标记而设计的。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 属性目标 | Assembly / Class / Struct / Enum / Method / Property / Field / Event / Interface / Parameter / Delegate |
| `AllowMultiple` | `true` |
| `Inherited` | `false` |
| COM 可见 | `[ComVisible(true)]` |
| IL 行号 | 类型 `0x02000016`，方法 `0x060000B0`–`0x060000BA` |
