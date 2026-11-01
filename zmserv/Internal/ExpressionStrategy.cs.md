## 文件作用

一个**完全没人用的 Attribute 派生类**（加壳器/模板注入的残留）。

它的特性定义允许贴在程序集、类、结构、枚举、方法、属性、字段、事件、接口、参数、委托上，并且允许重复贴、
不参与继承——听起来像"授权/标记"用途，但全工程**没有任何一处 `[ExpressionStrategy]` 贴用点**，
四个字段也只是默认值（三个 `true` + 一个 `""`）。

结论：整文件可视为噪声，处理方式为「保留原标识符 + 用 XML 注释标明是死代码」，方便和 IL 对照。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ExpressionStrategy : Attribute` | 空壳特性类。8 个 get/set 方法对应 4 个私有字段，全部无调用点 |
| `ReflectParams` / `StopParams` | 读/写 `factoryStrategy`（bool，默认 `true`） |
| `NewParams` / `DestroyParams` | 读/写 `threadStrategy`（bool，默认 `true`） |
| `GetParams` / `ListParams` | 读/写 `m_RoleStrategy`（string，默认 `""`） |
| `ManageParams` / `ForgotParams` | 读/写 `m_ModelStrategy`（bool，默认 `true`） |
| `FillDefinition` / `CloneDefinition` | 壳注入空桩：`return true` / `return false` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ExpressionStrategy` | 保持原名 | 死类，保留原名便于回溯；注释里已标明「空壳特性」 |
| `ReflectParams()` | 保持原名 | 读 `factoryStrategy`（等同 getter） |
| `StopParams(bool)` | 保持原名 | 写 `factoryStrategy`（等同 setter） |
| `NewParams()` / `DestroyParams(bool)` | 保持原名 | 读/写 `threadStrategy` |
| `GetParams()` / `ListParams(string)` | 保持原名 | 读/写 `m_RoleStrategy` |
| `ManageParams()` / `ForgotParams(bool)` | 保持原名 | 读/写 `m_ModelStrategy` |
| `factoryStrategy` / `threadStrategy` / `m_ModelStrategy` / `m_RoleStrategy` | 保持原名 | 私有字段，注释里标为「布尔字段1/2/3、文本字段」 |
| `bool_0` / `string_0` | `新工厂标志` / `新线程标志` / `新角色文本` / `新模型标志` | dnSpy 的占位参数名，已按各自字段语义改名 |

> 命名策略说明：**没有行为的方法保留反编译原名**（回溯 IL 需要），**只有参数名**做语义化
> （`bool_0`、`string_0` 这类是 dnSpy 生成的序号名，留着没有任何信息量）。

## 关键方法与流程

- 8 个 get/set：直接读写对应私有字段，无校验、无副作用、无调用点。
- `FillDefinition()` → `return true;`；`CloneDefinition()` → `return false;`（壳注入空桩）。

没有流程可言，属于「看得懂就能跳过」的噪声文件。

## 恶意行为 / 敏感操作

无。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| AttributeUsage | 覆盖 `Assembly/Class/Struct/Enum/Method/Property/Field/Event/Interface/Parameter/Delegate`，`AllowMultiple = true`，`Inherited = false` |
| `[ComVisible(true)]` | 是（与 `AssemblyInfo` 里的 `ComVisible(false)` 不一致，属模板/壳残留） |
| 字段默认值 | 三个 `true`、一个 `""` |
| 路径 / 注册表 / URL / 进程名 | 无 |
