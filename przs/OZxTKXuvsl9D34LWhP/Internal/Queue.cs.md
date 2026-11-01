# Queue.cs.md — VB `My` 命名空间聚合器

## 文件作用

`OZxTKXuvsl9D34LWhP\Internal\Queue.cs` 是 VB 项目模板自动生成的那一层 "`My` 命名空间"
（原类型名应为 `MyProject`，`Queue` 是混淆后的名字）。它不含任何业务逻辑，只做两件事：

1. 把 `My.Computer` / `My.Application` / `My.User` / `My.Forms` / `My.WebServices` 暴露成静态属性；
2. 提供延迟单例容器（`LazySingleton<T>`）和 VB 的"窗体组"实现（`FormCollection`）。

本工程真正被用到的入口只有两个：
- `Queue.MyForms.Form1/Form2/Form3/Form4/bianhao` —— Form1 里 40 多处窗体互调全靠它；
- `Queue.MyApplication` —— `Val` 那个入口类要用、`MySettings` 退出时要挂 `Shutdown` 事件。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Queue`（`[StandardModule]`） | `My` 命名空间聚合器：5 个静态属性 + 5 个单例字段 |
| `Queue.FormCollection`（原名 `Adapter`） | `My.Forms`：按需创建/释放 5 个窗体，带递归创建检测 |
| `Queue.WebServiceCollection`（原名 `Order`） | `My.WebServices`：本项目没有 Web 服务，纯样板 |
| `Queue.LazySingleton<T>`（原名 `Parameter<T>`） | 延迟单例：第一次取时 `new T()` |
| 各 `XxxTag()` 方法 | .NET Reactor 生成的授权标记桩（恒 true/false，无调用者） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ConcatVal` | `MyComputer` | `My.Computer`，类型 `Watcher` |
| `ResetVal` | `MyApplication` | `My.Application`，类型 `Val` |
| `OrderVal` | `MyUser` | `My.User` |
| `CollectVal` | `MyForms` | `My.Forms`，类型 `FormCollection` |
| `IncludeVal` | `MyWebServices` | `My.WebServices` |
| `Adapter` | `FormCollection` | VB 的窗体组 |
| `Order` | `WebServiceCollection` | VB 的 Web 服务组 |
| `Parameter<T>` | `LazySingleton<T>` | 延迟单例容器 |
| `AwakeVal<T>` | `获取或创建窗体<T>` | 已有且未释放就复用 |
| `LoginVal<T>` | `释放窗体<T>` | Dispose + 置 default |
| `PrintVal` / `FillVal` | 保留原名 | 返回 `typeof(本类型)` 的样板 |
| `DefineVal<T>` | `取实例或新建<T>` | Web 服务组用 |
| `RemoveVal<T>` | 保留原名 | 只是把引用置 default |
| `ResolveVal` | 保留原名 | 取单例 |
| `m_Publisher` / `config` / `message` / `tag` / `m_Strategy` | `计算机单例` / `应用程序单例` / `用户单例` / `窗体集合单例` / `Web服务单例` | 五个 `LazySingleton` 字段 |
| `m_Producer` | `正在创建的窗体表` | `[ThreadStatic] Hashtable`，递归创建检测 |
| `_Interceptor` / `_Process` / `m_Schema` / `_Stub` / `m_Proxy` | `编号窗体缓存` / `主窗体缓存` / `警告窗体缓存` / `提醒窗体缓存` / `锁屏窗体缓存` | 窗体引用缓存 |
| `m_System` | `单例实例` | `[ThreadStatic] T` |
| `PushTag` / `RateTag` / `FillTag` / `CloneTag` / `SortTag` | 保留原名 | 授权标记桩 |

## 关键方法与流程

### 5 个静态属性

都是 `LazySingleton<T>.ResolveVal()` 的一行转发，等价于 VB 的
`If 字段 Is Nothing Then 字段 = New T()`。属性上保留 `[HelpKeyword("My.Computer")]` 这类特性
（VB 设计器用来识别的，无运行期作用）。

### FormCollection.获取或创建窗体&lt;T&gt;(T 现有实例)

1. `现有实例 != null && !IsDisposed` → 直接返回；
2. 否则检查 `[ThreadStatic]` 的"正在创建的窗体表"：
   - 表已存在且已含有 `typeof(T)` → 抛 `InvalidOperationException("WinForms_RecursiveFormCreate")`
     （即"窗体 A 的构造函数里又去访问 A"）；
   - 表不存在 → 新建 `Hashtable`；
3. 把 `typeof(T)` 塞进表 → `new T()`；
4. 构造函数抛 `TargetInvocationException` 且带内层异常 → 包装成
   `InvalidOperationException("WinForms_SeeInnerException" + 内层消息)`；
5. `finally` 里把类型从表中移除（无论成败，避免"创建失败就永久递归"）。

### 窗体属性 `bianhao` / `Form1` / `Form2` / `Form3` / `Form4`

- getter：`缓存 = 获取或创建窗体<T>(缓存)`；
- setter：只接受 `null`（`throw new ArgumentException("Property can only be set to Nothing")`），
  传 null 时调 `释放窗体<T>(ref 缓存)`。

### LazySingleton&lt;T&gt;.ResolveVal()

`单例实例 == null` 时 `new T()`，然后返回——**注意它带 `[ThreadStatic]`**，
所以每个线程各有一份实例。

## 恶意行为 / 敏感操作

本文件**没有恶意行为**：不含文件、注册表、进程、网络操作。它的意义是把恶意窗体串起来：

| 关联 | 说明 |
| --- | --- |
| `Queue.MyForms.Form2` | 全屏锁屏 / 要密码退出的窗体（见 Form2.md） |
| `Queue.MyForms.Form3` | 警告提示牌（见 Form3.md） |
| `Queue.MyForms.Form4` | 锁屏窗体（见 Form4.md） |
| `Queue.MyForms.bianhao` | 显示机号的小窗（见 bianhao.md） |
| `Queue.MyApplication` | 进程入口 `Val`，启动主窗体 `Form1` |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 资源串 | `WinForms_RecursiveFormCreate`、`WinForms_SeeInnerException`（VB 运行时资源键） |
| 异常文案 | `Property can only be set to Nothing` |
| 特性 | `MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")` |
| 窗体组顺序 | `bianhao`、`Form1`、`Form2`、`Form3`、`Form4` |
