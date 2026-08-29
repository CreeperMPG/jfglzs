# MySettings.Designer.cs — 应用设置自动生成类（My.Settings 单例）

## 文件作用
VS `SettingsSingleFileGenerator`（9.0.0.0）自动生成的 `ApplicationSettingsBase` 分部类：
提供线程安全单例 `Default`（`SettingsBase.Synchronized`），并在首次访问时订阅
`My.Application.Shutdown` 事件 → `AutoSaveSettings` 在 `SaveMySettingsOnExit` 为真时调用
`MySettingsProperty.Settings.Save()` 保存设置。程序未定义自定义设置项，纯框架支撑；
`gypZRX1vuWc3aeye0R.Internal` 的 `Account.CollectBroadcaster` 即 My.Application（混淆名）。
无恶意行为。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| MySettings | My.Settings 单例（自动保存设置） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `AutoSaveSettings` | `退出时保存设置` | Shutdown 事件回调 |

## IOC / 硬编码
- 无。
