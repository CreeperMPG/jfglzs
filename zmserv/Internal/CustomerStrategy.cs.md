## 文件作用

IntelliLock 的「**未注册试用版**」提示代码。它证明样本是用**未注册版 IntelliLock** 加壳的，
同时也说明作者没有为加壳工具付费——这是识别「同一作者同批样本」的一个旁证。

代码本身没有业务价值：`zmserv` 是服务进程，没有交互桌面，`MessageBox.Show` 基本不可能成功弹出来；
而且这整个类在工程内**没有任何调用点**。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `IntelliLock提示`（原 `CustomerStrategy`） | 只弹一次的「unregistered version」提示；弹不出来就退化成写一段 JS alert 脚本 |
| `显示未授权提示()`（原 `smethod_0`） | `MessageBox.Show` + `catch` 兜底 |
| `ResolveParams(object)` | 空方法体（JS 脚本的「退路」占位，接收后即丢弃） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `CustomerStrategy` | `IntelliLock提示` | 类名是随机词 |
| `smethod_0()` | `显示未授权提示()` | dnSpy 的静态方法占位名 |
| `ResolveParams(object)` | 保持原名 | 方法体为空，保留原名便于回溯 |
| `m_DatabaseStrategy`（bool） | `已提示过` | 保证只弹一次 |
| `object_0` | `脚本内容` | 传入的 JS alert 字符串 |
| `VerifyDefinition()` | 保持原名 | 壳注入空桩，`return false` |

## 关键方法与流程

### `显示未授权提示()`
1. `if (!已提示过)` → 先 `已提示过 = true`（**在弹窗之前**置位，所以只会尝试一次）；
2. `try`：`MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com")`；
3. `catch`：`ResolveParams("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>")`。

> `catch` 不能删：在服务/无桌面会话下 `MessageBox.Show` 会抛异常（或至少可能抛），
> 原版就是靠这个 `catch` 把流程导向「无 GUI 方案」。删掉它语义就变了。

### `ResolveParams(object 脚本内容)`
空方法体——壳里本该「把脚本写到某个页面/响应」，结果是空的，等于把异常吃掉后什么都不做。

## 恶意行为 / 敏感操作

- 自身不构成恶意行为：只有一个弹窗 + 一段 JS `alert` 字符串。
- 分析价值：
  1. 证明加壳工具是 **IntelliLock（EZIRIZ）的未注册版**；
  2. 字符串 `www.eziriz.com` 与 `IntelliLock` 是**强指纹**，可直接用于同族样本聚类；
  3. 与 `Internal/AdvisorStrategy.cs`、`ILLicenseModule.cs` 一起构成「壳三件套」。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 弹窗标题 | `www.eziriz.com` |
| 弹窗正文 | `This assembly is protected by an unregistered version of "IntelliLock"!` |
| JS 脚本常量 | `<script language='javascript'>alert( "This assembly is protected by an unregistered version of IntelliLock!" );</script>` |
| 类型名 | `ILLicenseModule` 同批；本类原名 `CustomerStrategy` |
| 注册表 / 文件 / 进程 / URL 请求 | 无（`www.eziriz.com` 只是标题文字，不发网络请求） |
