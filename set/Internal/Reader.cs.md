# Internal/Reader.cs — VB.NET "My" 命名空间模拟（My.Computer/My.Application/My.Forms 等）

## 文件作用
VB.NET 项目框架的 `My` 对象实现（`[GeneratedCode("MyTemplate","8.0.0.0")]`），把 `My.Computer`、`My.Application`、`My.User`、`My.Forms`、`My.WebServices` 映射到本项目的实际类。程序代码（Form1 等）通过 `Reader.SetProperty.Registry`、`Reader.ChangeProperty`、`Reader.ReflectProperty.Form1/Form3/load` 访问系统对象与窗体——**入口链关键节点**：`Reader.ReflectProperty.load` 即入口窗体（`ExceptionMock.InvokeProperty` 与 `Descriptor.OnCreateMainForm` 都用它）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Reader（StandardModule） | My 命名空间门面 + 各单例容器 |
| Reader.Model | My.Forms 集合（Form1/Form3/load 窗体单例缓存） |
| Reader.Map | My.WebServices 占位（SoapHttpClientProtocol 集合） |
| Reader.Configuration<T> | 线程静态单例容器（惰性 Activator.CreateInstance） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| SetProperty | My.Computer | → Watcher（Computer 子类），供 `Registry/FileSystem` 使用 |
| ChangeProperty | My.Application | → Descriptor（WindowsFormsApplicationBase） |
| ViewProperty | My.User | → User |
| ReflectProperty | My.Forms | → Model（Form1/Form3/load 属性） |
| InterruptProperty | My.WebServices | → Map（空集合） |
| ReflectClass() | 占位 | return false |
| m_Order/_Client/factory/m_Producer/_Struct | 各单例 Configuration<T> | Watcher/Descriptor/User/Model/Map |
| Model.CallProperty<T>(inst) | GetOrCreateForm_窗体惰性创建 | 防递归创建检查 + Activator.CreateInstance<T>() |
| Model.ComputeProperty<T>(ref f) | DisposeForm_释放窗体 | Dispose 后置 null |
| Model.PatchProperty() | 返回自身类型 | 框架占位 |
| Map.PublishProperty<T>(inst) | 惰性创建 WebService | |
| Map.LoginClass() / VisitClass() | 占位 | return true / false |
| Configuration<T>.VisitProperty() | GetSingleton_取单例 | 线程静态 _Listener |
| Configuration<T>.RemoveClass()/NewClass() | 占位 | true/false |
| Model._Mapper | 递归创建检测表 | ThreadStatic Hashtable |

## 关键方法与流程
### My.Forms 窗体获取（Model 的属性）
- `Form1`/`Form3`/`load` 属性 getter：`CallProperty<T>(字段)`——若实例已存在且未 Dispose 直接返回；否则先检查 _Mapper 防递归（抛 "WinForms_RecursiveFormCreate"），`Activator.CreateInstance<T>()` 创建
- setter：只允许赋 null（Dispose 语义）

### 程序入口链（跨文件）
`Descriptor.InstantiateProperty(Main)` → `Reader.ChangeProperty.Run(args)` → `OnCreateMainForm` → `MainForm = Reader.ReflectProperty.load`；另有 `ExceptionMock.InvokeProperty()` → `Reader.ReflectProperty.load.Show()` + `Application.Run()`。两条路径最终都显示 load 密码窗体。

## 恶意行为 / 敏感操作
- 无独立恶意逻辑，纯 VB 框架胶水；但它是**入口链中枢**（ReflectProperty.load / .Form1 / .Form3）

## IOC / 硬编码
- 无恶意 IOC；框架字符串：`WinForms_RecursiveFormCreate`、`WinForms_SeeInnerException`、`My.MyProject.Forms`、`MyTemplate 8.0.0.0`
