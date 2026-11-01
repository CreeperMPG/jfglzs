# Internal/ExpressionMapper.cs

## 文件作用

VB.NET `My` 命名空间的投影层（`My.Computer` / `My.Application` / `My.User` / `My.Forms` / `My.WebServices`）。这是 Visual Studio 的 My 模板生成的样板，业务代码真正关心的只有两处：

- `My.Forms.Form1`（`VisitComposer.Form1`）——主窗体；
- `My.Application`（`CancelComposer`）——程序入口 `Run()` 与退出事件 `Shutdown`。

文件里同时定义了三个嵌套类：窗体缓存、Web 服务缓存、以及给五个入口共用的「线程级懒加载单例」。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ExpressionMapper`（`[StandardModule]`） | `My` 五个入口的属性投影，每个入口持有一个线程级单例 |
| `窗体集合缓存` | `My.Forms` 实现：按类型缓存 Form1/Form3，负责创建、递归创建检测、Dispose |
| `Web服务集合缓存` | `My.WebServices` 实现：本工程无 Web 服务引用，只剩模板方法 |
| `线程级单例<T>` | 每个线程一个实例的懒加载容器（无锁） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `SetComposer` | 保留原名（`My.Computer`） | 属性名被 `[HelpKeyword]` 绑定，业务不直接用，保持原样 |
| `CancelComposer` | 保留原名（`My.Application`） | 同上；被 `ComposerMapper` 与 `MySettings` 引用 |
| `RateComposer` | 保留原名（`My.User`） | 同上 |
| `VisitComposer` | 保留原名（`My.Forms`） | 同上；`ComposerMapper` 用 `VisitComposer.Form1` |
| `CheckComposer` | 保留原名（`My.WebServices`） | 同上 |
| `m_ProxyMapper` / `consumerMapper` / `methodMapper` / `_RecordMapper` / `_ParameterMapper` | `计算机访问器` / `应用程序访问器` / `用户访问器` / `窗体访问器` / `Web服务访问器` | 五个 `线程级单例<T>` 字段 |
| `EventMapper<T>` / `taskMapper` / `LoginComposer()` | `线程级单例<T>` / `实例` / `获取实例()` | 线程级单例容器 |
| `ClientMapper` | `窗体集合缓存` | `My.Forms` 实现类型 |
| `ExcludeComposer<T>(T Instance)` | `获取或创建窗体<T>(T 窗体实例)` | 缓存命中或新建窗体 |
| `ChangeComposer<T>(ref T gparam_0)` | `销毁窗体<T>(ref T 窗体)` | Dispose + 置 null |
| `UpdateComposer()` | `获取类型()` | 返回自身 Type（模板） |
| `_AuthenticationMapper` | `正在创建的窗体` | `[ThreadStatic]` Hashtable，检测递归创建 |
| `_StateMapper` / `m_PoolMapper` | `缓存的Form1` / `缓存的Form3` | 两个窗体实例字段 |
| `BridgeMapper` / `MoveComposer()` | `Web服务集合缓存` / `获取类型()` | `My.WebServices` 实现类型 |
| `InstantiateComposer<T>` / `ListComposer<T>` | `获取或创建服务<T>` / `清空服务实例<T>` | 服务缓存的新建与清空 |
| `PatchFacade()` / `UpdateFacade()` / `PopFacade()` / `ResolveFacade()` / `InsertFacade()` | `返回假占位()` / `返回真占位()` / `返回假占位()` / `返回真占位()` / `返回假占位()` | 五个恒值桩 |

## 关键方法与流程

1. **五个入口属性**（`get`）：全部转发到各自的 `线程级单例<T>.获取实例()`，无额外逻辑。
2. **`窗体集合缓存.获取或创建窗体<T>(实例)`**
   - 实例非 null 且未 `IsDisposed` → 直接复用；
   - 否则检查 `正在创建的窗体`：表已存在且含当前类型 → 抛 `InvalidOperationException("WinForms_RecursiveFormCreate")`（构造函数递归创建窗体）；
   - 表为空则先 new 一个 Hashtable，`Add(typeof(T), null)` 打标记；
   - `new T()`；若抛 `TargetInvocationException` 且 `InnerException` 非空，包成 `InvalidOperationException("WinForms_SeeInnerException")`；
   - `finally` 里 `Remove(typeof(T))`，保证标记清理。
3. **`窗体集合缓存.销毁窗体<T>(ref T)`**：`Dispose()` 后置 `default(T)`。
4. **`Form1` / `Form3` 属性**
   - `get`：`缓存 = 获取或创建窗体(缓存)`，返回缓存；
   - `set`：只接受 `null`（等值直接忽略，非 null 抛 `ArgumentException("Property can only be set to Nothing")`），非 null 走 `销毁窗体`。
5. **`线程级单例<T>.获取实例()`**：`[ThreadStatic]` 字段为空则 `new T()`，随后返回。**无锁**。
6. **模板方法**：`Equals` / `GetHashCode` / `ToString` / `获取类型` 全部直转基类或返回自身 Type；`获取或创建服务` / `清空服务实例` 是给未来的 Web 服务引用预留的。
7. **五个恒值桩**：`返回真/假占位`，无调用点。

## 恶意行为 / 敏感操作

无直接恶意行为。但它是**窗体生命周期的唯一入口**：`Form1` 一旦通过 `My.Forms.Form1` 被构造，其构造函数就会挂上 `Load` 处理器并生成随机助手进程名，整条恶意链路由此展开。

另外 `[ThreadStatic]` 单例意味着：若某处换了线程去取 `My.Forms.Form1`，会拿到**另一个** Form1 实例（新线程各自 new），这是 VB My 模板的既有行为，也解释了为什么本程序不靠单例做互斥。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 资源字符串 | `WinForms_RecursiveFormCreate`、`WinForms_SeeInnerException` |
| 异常文本 | `Property can only be set to Nothing` |
| 组集合特性 | `MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")` |
| 组集合特性（服务） | `MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", ...)` |
| IL 行号 | 类型 `0x02000004` / `0x02000005` / `0x02000006` / `0x02000007` |
