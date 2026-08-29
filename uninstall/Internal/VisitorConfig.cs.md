# VisitorConfig.cs — VB "My" 对象访问器（My.Computer / My.Application / My.Forms / My.User / My.WebServices）

## 文件作用
VB.NET 自动生成的 `My` 命名空间访问器（MyTemplate 生成），把 `My.Computer`（ConfigConfig）、`My.Application`（UtilsConfig）、`My.User`、`My.Forms`（MapperConfig）、`My.WebServices`（ProcessConfig）以单例形式暴露。`MapperConfig` 是 Form1/Form2 的懒加载单例容器，业务代码通过 `VisitorConfig.ListUtils.Form2` 等访问窗体实例。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| VisitorConfig | My 对象总入口 |
| MapperConfig（ListUtils） | My.Forms：Form1/Form2 单例容器 |
| ProcessConfig（MapUtils） | My.WebServices：Web 服务桩 |
| SingletonConfig\<T\> | 线程级懒加载单例持有器 |
| StartRegistry/VisitRegistry/RemoveRegistry 等 | 恒值桩方法（混淆噪声） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `StopUtils` | My.Computer | 返回 ConfigConfig（Computer 子类） |
| `CollectUtils` | My.Application | 返回 UtilsConfig |
| `LogoutUtils` | My.User | 返回 User |
| `ListUtils` | My.Forms | 返回 MapperConfig |
| `MapUtils` | My.WebServices | 返回 ProcessConfig |
| `WriteUtils` | 取单例实例 | 懒创建 |
| `CallUtils<T>` | 窗体单例获取 | 已存在且未释放则复用，否则 Activator 创建 |
| `IncludeUtils<T>` | 释放并置空窗体 | |
| `m_ExceptionConfig` 等 | 各单例持有字段 | |

## 关键方法与流程
### My.Forms 窗体单例（原名 MapperConfig）
- `Form1`/`Form2` 属性：若现有实例为空或已 Dispose 则 `Activator.CreateInstance<T>()` 新建；用 `m_InterpreterConfig`（ThreadStatic Hashtable）防递归创建。
- 业务调用点：Form1.cs `VisitorConfig.ListUtils.Form2.Show()`、UtilsConfig.cs `VisitorConfig.ListUtils.Form1`。

## 说明
- 全部为 VS 模板自动生成代码，无恶意行为、无 IOC；但它是业务代码访问 Form1/Form2 的通道，Form2 被反复 Show 正是经由此容器。
