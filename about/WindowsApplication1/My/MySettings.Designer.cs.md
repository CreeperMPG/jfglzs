# MySettings.Designer.cs — My.Settings 设计器生成代码（单例 + 退出自动保存）

## 文件作用
VS 生成的设置类：`SettingsBase.Synchronized` 创建线程安全单例 `defaultInstance`；`Default` 属性在首次访问时把 `AutoSaveSettings` 挂到 `My.Application.Shutdown` 事件，退出时若 `SaveMySettingsOnExit` 为真则保存设置。本程序未定义任何自定义设置项（`MySettings.settings` 为空模板）。纯框架代码。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| MySettings | 应用设置单例 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `AutoSaveSettings` | 应用关闭时保存设置 | 挂 Shutdown 事件 |
| `Default` | 设置单例访问器 | 线程安全惰性初始化 |
| `defaultInstance` / `addedHandler` / `addedHandlerLockObject` | 单例缓存/注册标志/锁 | — |

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 无。
