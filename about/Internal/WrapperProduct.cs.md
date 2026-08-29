# WrapperProduct.cs — VB.NET `My.*` 访问器生成代码（My.Forms 单例封装）

## 文件作用
VB.NET 编译器生成的 `My` 项目基础设施（`GeneratedCode("MyTemplate", "8.0.0.0")`）：对外暴露 `My.Computer`（`RunReponse`→ProductProduct）、`My.Application`（`ResetReponse`→ReponseProduct）、`My.User`（`PatchReponse`）、`My.Forms`（`SearchReponse`→FilterProduct）、`My.WebServices`（`RegisterReponse`→StatusProduct）。其中 `FilterProduct`（My.Forms）实现了 Form1/Form3 的**懒加载单例**（首次访问 `Activator.CreateInstance`，之后复用，可置空释放）。业务入口点 `WrapperProduct.SearchReponse.Form1` / `.Form3` 被 Form1/Form3 使用。无恶意逻辑，纯框架代码，但混淆器把类名改成 `*Product` 并把嵌套类命名为 `FilterProduct`/`StatusProduct`/`BroadcasterProduct<T>`。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| WrapperProduct | `My.*` 全局访问器静态模块 |
| FilterProduct | My.Forms：Form1/Form3 单例管理 |
| StatusProduct | My.WebServices 空集合 |
| BroadcasterProduct\<T\> | 线程静态单例持有器 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `RunReponse` | My.Computer | → ProductProduct（Computer 派生） |
| `ResetReponse` | My.Application | → ReponseProduct（WindowsFormsApplicationBase） |
| `PatchReponse` | My.User | → Microsoft.VisualBasic.ApplicationServices.User |
| `SearchReponse` | My.Forms | → FilterProduct（Form1/Form3 单例） |
| `RegisterReponse` | My.WebServices | → StatusProduct（空） |
| `StartListener/ReflectListener/RemoveListener/NewListener/ValidateListener/LoginListener/VisitListener` | 混淆填充桩 | 恒 true/false，无实义 |
| `m_ProxyProduct` | Form1 单例字段 | — |
| `tagProduct` | Form3 单例字段 | — |
| `_InstanceProduct` | 创建中窗体防递归表 | ThreadStatic Hashtable |
| `MoveReponse<T>` | 获取或创建窗体单例 | 防递归 + Activator |
| `InvokeReponse<T>` | 释放窗体单例 | Dispose 并置空 |

## 关键方法与流程
- `SearchReponse.Form1` getter：单例存活则返回，否则经 `MoveReponse<Form1>` 创建（用 Hashtable 标记防递归创建，异常时包装 InnerException）。
- `SearchReponse.Form3` getter：同 Form1 模式。

## 恶意行为 / 敏感操作
- 无。属于编译器生成框架代码。

## IOC / 硬编码
- 无。
