## 文件作用

VB.NET 的 **`My` 命名空间根模块**（`My.MyProject`，原混淆名 `PolicyStrategy`）。
它把 `My.Computer` / `My.Application` / `My.User` / `My.WebServices` 四个入口转发给
四个线程安全的对象提供器（`ThreadSafeObjectProvider<T>`）。

这四个属性在本工程里**没有任何调用点**；本文件是「VB 工程骨架」的一部分，
不是恶意代码——但它能说明样本是用 VB.NET + Visual Studio 模板正经编译出来的（而非手写 IL 拼出来的壳）。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MyProject`（`[StandardModule]`） | `My` 命名空间根；四个转发属性 + 四个 `ThreadSafeObjectProvider` 字段 |
| `Computer` / `Application` / `User` / `WebServices`（属性） | 对应 `My.Computer`、`My.Application`、`My.User`、`My.WebServices` |
| `MyWebServices`（原 `ReaderStrategy`） | `[MyGroupCollection]` 生成的 Web 服务集合；本工程没有 Web 引用，内部为空 |
| `ThreadSafeObjectProvider<T>`（原 `TagStrategy<T>`） | 线程本地懒加载容器（`[ThreadStatic]` 字段 + 首次访问 `new T()`） |
| `VisitDefinition()` 等 `*Definition` 方法 | 壳注入空桩，`true`/`false`，无调用点 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `PolicyStrategy` | `MyProject` | `[StandardModule]` + `[HelpKeyword("My.Computer")]` 等四个键，可确定它就是 `My.MyProject` |
| `SetupParams` | `Computer` | `[HelpKeyword("My.Computer")]` |
| `MoveParams` | `Application` | `[HelpKeyword("My.Application")]` |
| `WriteParams` | `User` | `[HelpKeyword("My.User")]` |
| `AwakeParams` | `WebServices` | `[HelpKeyword("My.WebServices")]` |
| `specificationStrategy` | `computerProvider` | `ThreadSafeObjectProvider<MyComputer>` |
| `statusStrategy` | `applicationProvider` | `ThreadSafeObjectProvider<MyApplication>` |
| `ruleStrategy` | `userProvider` | `ThreadSafeObjectProvider<User>` |
| `descriptorStrategy` | `webServicesProvider` | `ThreadSafeObjectProvider<MyWebServices>` |
| `ReaderStrategy` | `MyWebServices` | `[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", …)]` |
| `TagStrategy<T>` | `ThreadSafeObjectProvider<T>` | 线程本地对象提供器 |
| `SearchParams()` | `GetInstance()` | 提供器取实例（空则 new） |
| `initializerStrategy` | `threadLocalInstance` | `[ThreadStatic]` 缓存字段 |
| `PushParams()` | `GetInstanceType()` | 模板里的 `GetType` 遮蔽（VB `Shadows`）；C# 里改名以免与 `Object.GetType` 混淆 |
| `ViewParams<T>` | `CreateInstance<T>(instance)` | null 则 `new T()`，否则原样返回 |
| `SortParams<T>(ref T)` | `DisposeInstance<T>(ref instance)` | 置 `default(T)` |
| `VisitDefinition` / `PrepareDefinition` / `CompareDefinition` / `InvokeDefinition` / `ManageDefinition` | 保持原名 | 壳注入空桩 |

## 关键方法与流程

### 四个属性
- `Computer` → `computerProvider.GetInstance()`
- `Application` → `applicationProvider.GetInstance()`
- `User` → `userProvider.GetInstance()`
- `WebServices` → `webServicesProvider.GetInstance()`

### `ThreadSafeObjectProvider<T>.GetInstance()`
1. `if (threadLocalInstance == null)` → `threadLocalInstance = new T();`
2. 返回 `threadLocalInstance`。

`threadLocalInstance` 是 `[ThreadStatic]`，所以每个线程各有一份实例；本工程其实没有多线程访问这些属性。

### `MyWebServices` 的模板成员
- `Equals(object obj)` → `base.Equals(RuntimeHelpers.GetObjectValue(obj))`
  （VB 写法；`RuntimeHelpers.GetObjectValue` 会剥掉 `VBByRefStr` 之类的特殊包装，与 C# 直接比较**不完全等价**，故保留）；
- `GetHashCode()` → `base.GetHashCode()`；
- `GetInstanceType()`（原 `PushParams`）→ `typeof(MyProject.MyWebServices)`；
- `ToString()` → `base.ToString()`；
- `CreateInstance<T>(T instance)` → 空则 new；
- `DisposeInstance<T>(ref T instance)` → `instance = default(T)`。

### 构造函数
`MyWebServices()` 与 `ThreadSafeObjectProvider()` 都是空的 `[EditorBrowsable(Never)]` 构造。

## 恶意行为 / 敏感操作

无。本文件唯一值得记的是：**它证明样本是正统 VB.NET 工程编译的**，
加上 `My.Settings`/`My.Resources`/`My.Computer` 全套骨架都在，作者是拿 Visual Studio 做的产品级工程，
而不是临时拼的玩具木马。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| `[HelpKeyword]` 键 | `My.Computer`、`My.Application`、`My.User`、`My.WebServices` |
| `[MyGroupCollection]` | `System.Web.Services.Protocols.SoapHttpClientProtocol`、`Create__Instance__`、`Dispose__Instance__` |
| 特性 | `[GeneratedCode("MyTemplate", "11.0.0.0")]`、`[StandardModule]`、`[HideModuleName]`、`[ComVisible(false)]`、`[ThreadStatic]`、`[CompilerGenerated]` |
| 路径 / 注册表 / URL / 进程名 / 字符串常量 | 无 |
