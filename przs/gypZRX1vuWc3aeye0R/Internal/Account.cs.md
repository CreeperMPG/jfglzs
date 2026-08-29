# Account.cs — VB.NET "My" 根对象宿主（My.Computer/My.Application/My.Forms 等单例容器）

## 文件作用
`gypZRX1vuWc3aeye0R.Internal` 命名空间的核心容器类。**反混淆后真实身份：VB.NET 编译器的 `My` 项目
根对象**（`My.Computer`、`My.Application`、`My.User`、`My.Forms`、`My.WebServices`），由
`MyTemplate` 代码生成器产出，ConfuserEx 把类名改成了 `Account`、属性改成了 `StopBroadcaster` 等假名。
它不是通信/回调框架——`Broadcaster`/`Callback`/`Param` 等名字全是混淆假名；真实作用是给全程序提供
窗体单例（`My.Forms`，即 `Account.DefineBroadcaster`）与系统对象访问点。Form1/Form2/Form3/Form4/
bianhao 全部通过 `Account.DefineBroadcaster.XXX` 访问窗体实例。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Account（根类） | My 根对象：`My.Computer`/`My.Application`/`My.User`/`My.Forms`/`My.WebServices` 惰性单例 |
| Account.Importer | **My.Forms**：Form1/Form2/Form3/Form4/bianhao 的懒加载单例集合（全程序窗体访问入口） |
| Account.Container | My.WebServices：WebService 占位容器（本项目为空壳） |
| Account.Template\<T\> | 泛型单例模板（线程局部静态 `_Repository`，`Activator.CreateInstance` 惰性创建） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `Account.StopBroadcaster` | `My.Computer` | 返回 `Schema`（继承 `Microsoft.VisualBasic.Devices.Computer`）；bianhao 用它取计算机名 |
| `Account.CollectBroadcaster` | `My.Application` | 返回 `Broadcaster`（`WindowsFormsApplicationBase`）；MySettings 用它订阅 Shutdown |
| `Account.GetBroadcaster` | `My.User` | 返回 `User`（VB 用户对象） |
| `Account.DefineBroadcaster` | `My.Forms` | 返回 `Importer`，全程序窗体工厂（Form1-4、bianhao） |
| `Account.ResolveBroadcaster` | `My.WebServices` | 返回 `Container`（WebService 占位） |
| `Account.Template<T>.CalculateBroadcaster` | `获取单例` | 线程静态单例惰性创建 |
| `Account.Importer.CalcBroadcaster<T>` | `窗体懒加载` | 已存在且未释放则复用，否则 `Activator.CreateInstance<T>()` 新建（带递归创建保护） |
| `Account.Importer.SetBroadcaster<T>` | `释放窗体` | Dispose 并置空（仅允许置空） |
| `serializer/_Pool/m_Interpreter/annotation/wrapper` | My 单例字段 | 分别对应 Computer/Application/User/Forms/WebServices |
| `ReflectThread/NewThread/VisitThread`（恒 true） | 混淆器填充方法 | ConfuserEx 插入的无意义方法（返回常量） |
| `WriteThread/RemoveThread`（恒 false） | 混淆器填充方法 | 同上 |

## 关键方法与流程
### My.Forms 窗体单例（Importer.CalcBroadcaster\<T\>）
- **流程**：1. 若实例存在且未 `IsDisposed` → 直接返回；2. 用线程静态 `Hashtable policy` 防止
  递归创建（同一类型重复进入则抛 `WinForms_RecursiveFormCreate`）；3. `Activator.CreateInstance<T>()`
  创建新窗体；4. 捕获 `TargetInvocationException` 时包装为 `InvalidOperationException`；
  5. finally 移除递归锁。各窗体属性（bianhao/Form1/Form2/Form3/Form4）均走此流程，因此
  `Account.DefineBroadcaster.Form2` 等全局可拿到唯一窗体实例。

### 泛型单例模板（Template\<T\>.CalculateBroadcaster）
- **流程**：`_Repository` 为 `[ThreadStatic]` 静态字段，为 null 时 `Activator.CreateInstance<T>()`
  创建并缓存，之后恒返回同一实例。

## 恶意行为 / 敏感操作
- 本身无恶意操作；但它是全程序窗体的统一工厂：`Account.DefineBroadcaster.Form1~Form4/bianhao`
  是恶意窗体（锁屏/删除文件/持久化）被全局调用的枢纽。
- 暴露 `My.Computer`（`Account.StopBroadcaster`）供 bianhao 读取计算机名。

## IOC / 硬编码
- 无网络/路径 IOC。无加密常量。
