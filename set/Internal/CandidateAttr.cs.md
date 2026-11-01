# Internal\CandidateAttr.cs —— 文件级分析

## 文件作用
VB.NET `My` 命名空间的自动生成聚合入口（原模板类名即 MyProject）。把 `My.Computer`、`My.Application`、`My.User`、`My.Forms`、`My.WebServices` 五组成员转成静态属性，每组背后都是一个惰性单例持有器。全工程对它的主要使用是 `CandidateAttr.应用程序`（`My.Application`，Main 与 My.Settings 用）和 `CandidateAttr.窗体集合.load`（首个窗体）。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `CandidateAttr` | My 命名空间入口；5 个静态属性 + 5 个持有器字段 + 1 个死代码桩 |
| `CandidateAttr.ServerAttr` | `My.Forms` 实现体：三个窗体的惰性单例（`Form1` / `Form3` / `load`），带递归创建保护 |
| `CandidateAttr.MessageAttr` | `My.WebServices` 实现体；工程没引用 Web 服务，实际是空壳 |
| `CandidateAttr.ManagerAttr<T>` | 通用惰性单例持有器：`取单例()` 第一次 new，之后复用 |
| `创建窗体单例<T>` / `释放窗体单例<T>` | VB 模板的 `Create__Instance__` / `Dispose__Instance__` |
| `取非空实例<T>` / `清空引用<T>` | Web 服务版的 `Create__Instance__` / `Dispose__Instance__` |
| `恒真桩` / `恒假桩` | 恒值死代码桩（5 处），无调用点 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| 属性 `SetInfo` | `计算机` | `[HelpKeyword("My.Computer")]` |
| 属性 `RestartInfo` | `应用程序` | `[HelpKeyword("My.Application")]`，被 Main 与 My.Settings 引用 |
| 属性 `ExcludeInfo` | `当前用户` | `[HelpKeyword("My.User")]` |
| 属性 `ComputeInfo` | `窗体集合` | `[HelpKeyword("My.Forms")]` |
| 属性 `PublishInfo` | `Web服务` | `[HelpKeyword("My.WebServices")]` |
| 字段 `contextAttr` | `计算机持有器` | `ManagerAttr<AttrAttr>` |
| 字段 `m_QueueAttr` | `应用程序持有器` | `ManagerAttr<InfoAttr>` |
| 字段 `_ProducerAttr` | `用户持有器` | `ManagerAttr<User>` |
| 字段 `m_ConfigurationAttr` | `窗体集合持有器` | `ManagerAttr<ServerAttr>` |
| 字段 `m_ValueAttr` | `Web服务持有器` | `ManagerAttr<MessageAttr>` |
| `PrintInfo<T>(T)` | `创建窗体单例<T>(T)` | 原 `Create__Instance__` |
| `RunInfo<T>(ref T)` | `释放窗体单例<T>(ref T)` | 原 `Dispose__Instance__` |
| `ValidateInfo<T>(T)` | `取非空实例<T>(T)` | 原 `Create__Instance__`（Web 服务版） |
| `RemoveInfo<T>(ref T)` | `清空引用<T>(ref T)` | 原 `Dispose__Instance__`（Web 服务版） |
| `InvokeInfo()` | `取单例()` | `ManagerAttr<T>` 的懒创建 |
| `WriteInfo()` / `PatchInfo()` | `取类型()` | 都返回 `typeof(...)` |
| 字段 `recordAttr` | `创建中窗体表` | ThreadStatic 的 Hashtable，防递归 |
| 字段 `_TokenizerAttr` | `单例` | `ManagerAttr<T>` 的缓存字段 |
| 字段 `utilsAttr` / `orderAttr` / `m_ParamsAttr` | `form1实例` / `form3实例` / `load实例` | My.Forms 的单例字段 |
| 参数 `gparam_0` | `实例` | ref 参数 |
| `FlushRule()` | `恒假桩()` | `return false;`，无调用点 |
| `PatchFactory()` / `PushFactory()` | `恒真桩()` / `恒假桩()` | `MessageAttr` 内 |
| `PrepareFactory()` / `CompareFactory()` | `恒真桩()` / `恒假桩()` | `ManagerAttr<T>` 内 |
| 类名 `CandidateAttr` / `ServerAttr` / `MessageAttr` / `ManagerAttr` | 未改名 | 被 csproj/跨文件引用，且属标准模板结构，保留 |

## 关键方法与流程（按方法分段，写清步骤）
- `取单例()`（`ManagerAttr<T>`）：字段为 null 就 `new T()`，随后返回该字段。注意字段带 `[ThreadStatic]`，即**每个线程各持一份**单例。
- `创建窗体单例<T>(实例)`（`ServerAttr`）：
  1. 实例非 null 且未 `IsDisposed` → 直接复用。
  2. 否则检查 `创建中窗体表`：表为 null 就新建；若表里已有 `typeof(T)` → 抛 `InvalidOperationException(WinForms_RecursiveFormCreate)`（防构造函数递归 new 自己）。
  3. `表.Add(typeof(T), null)` → `try { 结果 = new T(); }`。
  4. 构造抛 `TargetInvocationException` 且内层异常非空 → 包成 `InvalidOperationException(WinForms_SeeInnerException)` 重抛。
  5. `finally` 里 `表.Remove(typeof(T))` —— 无论成功失败都清表。
- `释放窗体单例<T>(ref 实例)`：`实例.Dispose()` 后置 `default(T)`。
- `取非空实例<T>` / `清空引用<T>`：Web 服务版的同名逻辑（null 则 new / 直接置 default）。
- 三个窗体属性 `Form1` / `Form3` / `load`：get 走 `创建窗体单例<T>`；set 只接受 `null`（非 null 且与现有实例不同则抛 `ArgumentException("Property can only be set to Nothing")`），等价于「释放」。

## 恶意行为 / 敏感操作
- 无恶意行为，纯框架模板。
- 与恶意逻辑的关联：`窗体集合.load` 是启动窗体，`窗体集合.Form1` 是随后弹出的设置主界面（`load` 校验通过后打开）。要跟踪恶意行为进 `Form1`。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 字符串常量（均为框架资源键，不是 IOC）：
  - `WinForms_RecursiveFormCreate`、`WinForms_SeeInnerException`
  - `Property can only be set to Nothing`
  - `System.Windows.Forms.Form`、`System.Web.Services.Protocols.SoapHttpClientProtocol`
  - `My.MyProject.Forms`（`MyGroupCollection` 特性里的分类名）
- 类型引用：`WindowsApplication1.Form1`、`WindowsApplication1.Form3`、`WindowsApplication1.load`

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无 `IL_xxxx:` 标签、无 `num2 = N`、无 `ClearProjectError()`
- 保留全部 43 条 `// Token:` 注释
- 注意事项：`[MyGroupCollection(...)]` 里的 `"Create__Instance__"` / `"Dispose__Instance__"` 是字符串常量，**未改动**；只是对应的方法改名了，已在 XML 注释里标注原名
