# Internal/Test.cs 分析

## 文件作用

VB.NET 自动生成的 **My 命名空间根模块**（`MyProject`，`[StandardModule]` + `[HideModuleName]`）。
它本身不做事，只把各个 `My.*` 单例按需创建出来：

| 属性 | 真实身份 | 返回类型 |
| --- | --- | --- |
| `Computer`（原 `SetReader`） | `My.Computer` | `MyComputer`（原 `Template`） |
| `Application`（原 `InterruptReader`） | `My.Application` | `Importer` |
| `User`（原 `FindReader`） | `My.User` | `Microsoft.VisualBasic.ApplicationServices.User` |
| `Forms`（原 `CallReader`） | `My.Forms` | `MyForms`（原 `Test.GetterConfig`） |
| `WebServices`（原 `PostReader`） | `My.WebServices` | `MyWebServices`（原 `Test.ParserConfig`） |

身份不是猜的：五个属性上分别挂着 `[HelpKeyword("My.Computer")]` 等标记，与 VB 生成器输出逐字对应。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MyProject`（原 `Test`） | My.* 根模块：五个单例容器 + 五个属性 |
| `MyProject.MyForms`（原 `Test.GetterConfig`） | My.Forms 实现：`Form1`/`Form2` 的单例创建与释放、构造递归检测 |
| `获取或创建窗体实例<T>`（原 `PrintReader`） | 已有且未释放就复用，否则 `new T()`；`[ThreadStatic]` 的 Hashtable 检测递归构造 |
| `释放窗体<T>`（原 `FlushReader`） | `Dispose()` + 把字段置 `default`（`My.Forms.X = Nothing` 时走这里） |
| `MyProject.MyWebServices`（原 `Test.ParserConfig`） | My.WebServices 实现（本工程没有 Web 服务，只剩样板方法） |
| `取或创建实例<T>` / `清空实例<T>`（原 `CalcReader` / `ReflectReader`） | Web 服务代理的懒创建 / 置空 |
| `MyProject.MyGroupCollection<T>`（原 `Test.ParamsConfig<T>`） | 每个线程一份实例的懒汉容器（`[ThreadStatic] _实例缓存`） |
| `混淆器占位方法_恒真/恒假` | 各原 `*Parser` 方法，全部恒定返回，无调用者 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `Test` | `MyProject` | `[HelpKeyword("My.*")]` 全挂在这个类上，它就是 My 根模块 |
| `SetReader` / `InterruptReader` / `FindReader` / `CallReader` / `PostReader` | `Computer` / `Application` / `User` / `Forms` / `WebServices` | 属性上的 `[HelpKeyword]` 直接给出答案 |
| `Test.GetterConfig` | `MyProject.MyForms` | `[MyGroupCollection("System.Windows.Forms.Form", …)]` |
| `Test.ParserConfig` | `MyProject.MyWebServices` | `[MyGroupCollection("System.Net…SoapHttpClientProtocol", …)]` |
| `Test.ParamsConfig<T>` | `MyProject.MyGroupCollection<T>` | 泛型单例容器 |
| `PrintReader<T>` | `获取或创建窗体实例<T>` |  |
| `FlushReader<T>` | `释放窗体<T>` |  |
| `CreateReader()` | `取本线程实例()` | `MyGroupCollection<T>` 的懒创建 |
| `CalcReader<T>` / `ReflectReader<T>` | `取或创建实例<T>` / `清空实例<T>` | Web 服务组 |
| `CountReader` / `InstantiateReader` | `取类型` | 生成器样板（返回 `typeof(本类)`） |
| `consumerConfig` | `正在创建的窗体类型` | 递归构造检测表 |
| `m_CodeConfig` / `m_ParameterConfig` | `m_Form1实例` / `m_Form2实例` | 窗体缓存字段 |
| `_Expression` / `m_Customer` / `m_Descriptor` / `_Struct` / `m_Param` | `_计算机容器` / `_应用程序容器` / `_用户容器` / `_窗体容器` / `_Web服务容器` | 五个 `MyGroupCollection` |
| `_RepositoryConfig` | `_实例缓存` | `MyGroupCollection<T>` 的 ThreadStatic 缓存 |
| `gparam_0`、`Instance`、`instance`、`t` | `窗体实例`、`现有实例`、`实例`、`结果` |  |
| `CompareParser` 等 | `混淆器占位方法_恒假/恒真` | 占位 |

## 关键方法与流程

### `MyForms.Form1` / `Form2` 属性

```
get  → this.m_Form1实例 = 获取或创建窗体实例<Form1>(this.m_Form1实例); return this.m_Form1实例;
set  → if (value != 当前) { if (value != null) throw new ArgumentException("Property can only be set to Nothing");
                           释放窗体<Form1>(ref this.m_Form1实例); }
```

`获取或创建窗体实例<T>` 的完整逻辑：

1. `现有实例 != null && !IsDisposed` → 直接返回；
2. 否则用 `[ThreadStatic] Hashtable 正在创建的窗体类型` 判断 `typeof(T)` 是否已在构造中 —— 是则抛
   `InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate"))`（防「构造函数里又去取自己」死递归）；
3. `Add(typeof(T), null)` → `try { 结果 = new T(); }`
   `catch (TargetInvocationException ex) when (ex.InnerException != null)` → 包装成
   `InvalidOperationException(Utils.GetResourceString("WinForms_SeeInnerException", …), 内层异常)`；
   `finally { Remove(typeof(T)); }`。

本程序里 `Form1` 由 `Importer.OnCreateMainForm` 取一次，`Form2` 由 `Form1` 与自身定时器反复取，
所以这条「同一个实例」的保证是卸载器行为的一部分（`Form2.Visible` 检测才有意义）。

### `MyGroupCollection<T>.取本线程实例()`

`if (_实例缓存 == null) _实例缓存 = new T(); return _实例缓存;` —— 无锁懒汉式，
但字段带 `[ThreadStatic]`，所以「每线程一个」（这是 VB `My.*` 的原始语义）。

## 恶意行为 / 敏感操作

无直接恶意行为。它是**基础设施**，决定了：

- `My.Forms.Form2` 永远是同一个窗体实例 —— 所以 `Form1` 的 1 秒定时器能可靠地 `Show()` 它；
- `My.Computer` 每线程单例 —— `MappingConfig` 每次拿到的都是同一个 `Computer`；
- 宿主是 `My.Application`（`Importer`），因此 `Shutdown` 事件的挂接点唯一。

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 特性字符串 | `[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]` | My.Forms 生成器标记 |
| 特性字符串 | `[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", …, "")]` | My.WebServices 生成器标记 |
| 资源键 | `WinForms_RecursiveFormCreate`、`WinForms_SeeInnerException` | 递归/内层异常文案 |
| 异常文案 | `Property can only be set to Nothing` | 生成器固定文案 |
| 特性 | `[HelpKeyword("My.Computer"/"My.Application"/"My.User"/"My.Forms"/"My.WebServices")]`、`[ThreadStatic]` | 身份标记 |
| Token | 类型 `0x02000004..07`；方法 `0x06000009..26`；字段 `0x04000001..09` | 元数据行号 |
