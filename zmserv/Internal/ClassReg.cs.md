# Internal/ClassReg.cs — VB.NET "My.*" 模板生成类（My.Computer/My.Application/My.User/My.WebServices 门面）

## 文件作用
这是 VB.NET 项目自动生成的 `My` 命名空间门面类（`MyTemplate` 生成），混淆器只改了名字。它通过 `ErrorReg<T>`（线程静态惰性单例容器）向业务代码暴露 `My.Computer`（`RegReg`）、`My.Application`（`ListReg`）、`My.User`（`User`）、`My.WebServices`（`TemplateReg`）四个访问器。**无任何恶意逻辑**，属于"每个 VB 程序都有"的脚手架代码，连同若干恒真/恒假的 `*Parser` 混淆填充方法。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `ClassReg` | `My` 门面（隐藏模块） |
| `ClassReg.TemplateReg` | `My.WebServices` 集合类（SoapHttpClientProtocol 客户端集合，生成器产物） |
| `ClassReg.ErrorReg<T>` | 线程静态惰性单例容器（get-or-create） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `MapList` | My.Computer | `RegReg`（继承 `Microsoft.VisualBasic.Devices.Computer`） |
| `InitList` | My.Application | `ListReg`（继承 `ConsoleApplicationBase`） |
| `InterruptList` | My.User | `Microsoft.VisualBasic.ApplicationServices.User` |
| `ResetList` | My.WebServices | `TemplateReg` |
| `accountReg/attrReg/ruleReg/configurationReg` | 各单例持有器 | `ErrorReg<T>` 实例 |
| `ReflectList` | 取单例 | 懒创建 + 缓存 |
| `TemplateReg.SelectList` | 返回自身类型 | 生成器代码 |
| `TemplateReg.RemoveList<T>` | 空则新建 | `Activator.CreateInstance<T>()` |
| `TemplateReg.DisableList<T>` | 置默认值 | 生成器代码 |
| `InterruptParser/CreateParser/SetupParser/ResolveParser/LogoutParser/CallParser` | 混淆填充 | 恒真/恒假，无调用方业务意义 |

## 关键方法与流程
- `MapList/InitList/InterruptList/ResetList`：均为 `xxxReg.ReflectList()` 惰性取单例的 getter。
- `ErrorReg<T>.ReflectList()`：`itemReg`（ThreadStatic）为 null 时 `Activator.CreateInstance<T>()` 并缓存，返回。
- `TemplateReg`：标准 MyWebServices 生成类（Equals/GetHashCode/ToString/类型返回/泛型工具），无手工逻辑。
- 说明：业务代码若引用 `My.Computer`（如 `My.Computer.Clipboard`、`My.Computer.Registry`）会经由此类访问，但本项目反编译代码中未见此类引用，属死门面。

## 恶意行为 / 敏感操作
- 无。纯 VB 运行时脚手架。

## IOC / 硬编码
- 无恶意 IOC。仅 `GeneratedCode("MyTemplate", "8.0.0.0")` 生成器指纹。
