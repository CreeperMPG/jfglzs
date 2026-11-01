# WindowsApplication1\My\MySettings.Designer.cs —— 文件级分析

## 文件作用
VB.NET 应用程序设置（`My.Settings`）的自动生成单例类，基类 `ApplicationSettingsBase`。逻辑全是模板：懒加载单例 + 首次取用时在 `My.Application.Shutdown` 上挂一个「退出时保存设置」的回调。`MySettings.settings` 里没有定义任何自定义设置项，所以它只承担「退出时触发 Save()」这个固定动作。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettings : ApplicationSettingsBase` | 设置单例（partial 类） |
| `Default` 属性 | 返回单例；首次访问时挂 Shutdown 回调 |
| `AutoSaveSettings(object, EventArgs)` | 退出时保存设置的静态回调 |
| `defaultInstance` | 线程安全的设置实例（`SettingsBase.Synchronized`） |
| `addedHandler` / `addedHandlerLockObject` | 「回调已挂」标志与加锁对象 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `CandidateAttr.RestartInfo` | `CandidateAttr.应用程序` | 跨文件同步改名（`My.Application`） |
| `MySettingsProperty.Settings` | `MySettingsProperty.设置` | 跨文件同步改名 |
| 其余成员 | 未改名 | `Default` / `AutoSaveSettings` / `defaultInstance` / `addedHandler` 等均为模板标准名，语义已明确 |

## 关键方法与流程（按方法分段，写清步骤）
`Default` getter（双重检查锁）
1. `if (!addedHandler)` —— 快路径，已挂过就直接返回实例。
2. 取 `addedHandlerLockObject`；先调用 `ObjectFlowControl.CheckForSyncLockOnValueType(obj)` —— 这是 VB 在 `SyncLock` 前的值类型检查，**必须保留**（不能删成普通 `lock`）。
3. `lock (obj)` 内再 `if (!addedHandler)`：`CandidateAttr.应用程序.Shutdown += AutoSaveSettings; addedHandler = true;`
4. 返回 `defaultInstance`。

`AutoSaveSettings(sender, e)`
1. `if (CandidateAttr.应用程序.SaveMySettingsOnExit)` —— 由 `InfoAttr` 构造里的 `SaveMySettingsOnExit = true` 决定，实际恒为真。
2. `MySettingsProperty.设置.Save();`

## 恶意行为 / 敏感操作
- 无恶意行为。仅涉及用户级配置文件读写（`ApplicationSettingsBase.Save()`）。
- 由于 `settings` 文件里没有任何自定义项，该保存动作不会写入业务数据。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 无字符串常量、无路径。配置文件由 .NET 默认落在 `%LOCALAPPDATA%\<程序集名>\...user.config`，属框架行为。

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无 `IL_xxxx:` / `num2 = N` / `ClearProjectError()`
- 保留全部 6 条 `// Token:` 注释
- `ObjectFlowControl.CheckForSyncLockOnValueType` 与 `RuntimeHelpers.GetObjectValue` 原样保留（VB 语义，不许改成 `new object()`）
- 跨文件改名联动：`CandidateAttr.RestartInfo` → `CandidateAttr.应用程序`、`MySettingsProperty.Settings` → `MySettingsProperty.设置`
