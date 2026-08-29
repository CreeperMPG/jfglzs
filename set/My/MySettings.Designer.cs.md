# My/MySettings.Designer.cs — 应用设置自动保存逻辑（自动生成）

## 文件作用
VB 项目 `My.Settings` 设计器文件：单例 `Default`（SettingsBase.Synchronized）并在应用 Shutdown 时自动 `Save()`（AutoSaveSettings 订阅 `Reader.ChangeProperty.Shutdown`，即 My.Application 关闭事件）。无恶意逻辑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| MySettings（partial : ApplicationSettingsBase） | 设置持久化（退出自动保存） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| AutoSaveSettings(s,e) | OnShutdown_Save | Shutdown 时 SaveMySettingsOnExit 为真则 Settings.Save() |
| Default | 设置单例 | 线程安全惰性初始化 |

## 恶意行为 / 敏感操作
- 无

## IOC / 硬编码
- 无
