## 文件作用

VB.NET 模板的「My」命名空间聚合类（`MyProject`）：对外暴露 `My.Computer` / `My.Application` / `My.User` / `My.Forms` / `My.WebServices` 五个入口。全部是模板样板，没有业务逻辑。

本文件同时是全工程的**窗体重心**：所有窗体单例都通过 `SetterEvent.ComputeCallback.Form1..Form4` 取（因此 Form1/2/3/4 里的跨窗体调用都长这样）。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `SetterEvent`（`[StandardModule]`） | My 命名空间聚合，5 个静态入口 + 5 个懒加载容器 |
| `CalculateCallback` | My.Computer → `EventEvent` 单例 |
| `CollectCallback` | My.Application → `CallbackEvent` 单例（**启动框架本体**） |
| `ConnectCallback` | My.User → `User` 单例 |
| `ComputeCallback` | My.Forms → `PolicySetter` 容器（Form1~Form4 都从这里拿） |
| `AssetCallback` | My.WebServices → `CustomerSetter` 空容器 |
| `PolicySetter`（嵌套类） | My.Forms 容器：窗体按需创建 + 递归创建检测 + 支持置空释放 |
| `CustomerSetter`（嵌套类） | My.WebServices 空容器（本工程没声明任何 Web 服务） |
| `ResolverSetter<T>`（嵌套泛型类） | 通用懒加载单例容器（`[ThreadStatic]` 静态字段） |
| `RegisterDefinition` / `ExcludeDefinition` | 桩：恒 `true` / `false` |
| `SetRule` / `IncludeRule` / `MapRule` / `PatchRule` | 嵌套类里的桩，恒 `true` / `false` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `SetterEvent` | 未改名 | 类名与文件绑定；实为 `MyProject` |
| `_FactoryEvent` / `_HelperEvent` / `mockEvent` / `m_ComparatorEvent` / `m_ErrorEvent` | 未改名 | 分别是 My.Computer / My.Application / My.User / My.Forms / My.WebServices 的懒加载容器；改名收益低（调用点全在本文件内），且注释已标明对应关系 |
| `PolicySetter` | 未改名 | 实为 `MyForms` |
| `CustomerSetter` | 未改名 | 实为 `MyWebServices` |
| `ResolverSetter<T>` | 未改名 | 实为 `MyGroupCollection` 的懒加载器 |
| `CalculateEvent<T>(T Instance)` | 未改名（语义：取或创建窗体实例） | 壳随机名；参数 `Instance` → `实例` |
| `RegisterEvent<T>(ref T gparam_0)` | 未改名（语义：释放并置空窗体字段） | 参数 `gparam_0` → `引用` |
| `FillEvent<T>(T instance)` | 未改名（语义：无则新建） | 参数 `instance` → `实例` |
| `AddEvent<T>(ref T gparam_0)` | 未改名（语义：置空引用） | 参数 `gparam_0` → `引用` |
| `CallEvent()` | 未改名（语义：取单例） | |
| `CollectEvent()` / `QueryEvent()` | 未改名（语义：返回容器类型） | |
| `reponseSetter` | 未改名 | 「正在构造中」的类型登记表（`[ThreadStatic]`），用于检测递归创建 |
| `m_ConnectionSetter` | 未改名 | 泛型单例缓存字段（`[ThreadStatic]`） |
| `m_OrderSetter` / `_IteratorSetter` / `parameterSetter` / `m_SpecificationSetter` | 未改名 | 分别是 Form1/2/3/4 的缓存字段（模板要求 `public` 字段） |
| `t` / `obj` / `gparam_0` / `Instance` / `instance` | 结果实例 / 待比较对象 / 引用 / 实例 / 实例 | 纯占位局部名与参数名 |

## 关键方法与流程

### `PolicySetter.CalculateEvent<T>(实例)` — 窗体按需创建

1. `实例 != null && !实例.IsDisposed` → 直接复用返回。
2. 否则：
   1. 若 `reponseSetter`（`[ThreadStatic]` Hashtable）非空且已含 `typeof(T)` → **抛 `InvalidOperationException("WinForms_RecursiveFormCreate")`**（识别"窗体构造过程中又请求自己"的死循环）。
   2. `reponseSetter` 为空则新建。
   3. `reponseSetter.Add(typeof(T), null)` 登记"正在造"。
   4. `try { 结果实例 = new T(); }`
      `catch (TargetInvocationException ex) when (ex.InnerException != null)` → 拆包重抛 `InvalidOperationException("WinForms_SeeInnerException" + 内层消息)`，把内层异常带出来。
      `finally { reponseSetter.Remove(typeof(T)); }` 无论如何销号。
3. 返回实例。

### 窗体属性（Form1 / Form2 / Form3 / Form4）

每个都是同一个模子：

- `get`：`字段 = CalculateEvent<T>(字段); return 字段;`
- `set`：若 `value != 字段`：非 `null` 就 `throw new ArgumentException("Property can only be set to Nothing")`（**只允许置空**）；是 `null` 就 `RegisterEvent<T>(ref 字段)`（`Dispose()` + 置 `default`）。

这是 VB `My.Forms` 的固定语义：窗体只能"用"或"释放"，不能换成另一个实例。

> `OnCreateMainForm` 里 `base.MainForm = SetterEvent.ComputeCallback.Form1;` 就是走这条 getter，第一次访问即创建主窗体。

### `ResolverSetter<T>.CallEvent()`

`m_ConnectionSetter == null` 时 `new T()` 并缓存；`[ThreadStatic]` 意味着**每个线程各一份**单例。

### `CustomerSetter`

- `FillEvent<T>`：无则 `new T()`，有则原样返回。
- `AddEvent<T>`：把引用置 `default(T)`（不 Dispose——Web 代理没这需求）。

## 恶意行为 / 敏感操作

本文件无直接恶意行为，但有一处与恶意逻辑强耦合：

| 点 | 说明 |
| --- | --- |
| `ComputeCallback.Form2/Form3/Form4` | 全工程所有"弹惩罚窗"的调用都经过这里。`Form1` 里 `SetterEvent.ComputeCallback.Form2.Show()` 这类写法就是通过本类拿到窗体单例 |
| `My.Forms` 的 `Dispose` 语义 | `form = null` 会真正 `Dispose` 窗体，因此"关掉警告窗"必须用 `Close()` 而不是置空（置空后下次取到的会是新实例，`Visible` 判断会失真） |
| `ResolverSetter<T>` 的 `[ThreadStatic]` | 每个线程独立单例，意味着 WMI 回调线程 / UI 线程拿到的是不同实例——分析 `Form1` 的 `BeginInvoke` 时要注意这一点 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 异常文案 | `WinForms_RecursiveFormCreate`、`WinForms_SeeInnerException`、`Property can only be set to Nothing` |
| 模板标记 | `[GeneratedCode("MyTemplate", "11.0.0.0")]`、`[MyGroupCollection(...)]` |
| HelpKeyword | `My.Computer`、`My.Application`、`My.User`、`My.Forms`、`My.WebServices` |
| 涉及的窗体类型 | `WindowsApplication1.Form1` ~ `Form4` |
