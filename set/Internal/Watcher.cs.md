# Internal/Watcher.cs — My.Computer 模拟（Microsoft.VisualBasic.Devices.Computer 子类）

## 文件作用
`Reader.SetProperty`（即 My.Computer）的实际类型：继承 `Microsoft.VisualBasic.Devices.Computer`，供程序访问 `Registry`、`FileSystem`、`Network` 等系统对象。Form1 中大量 `Reader.SetProperty.Registry.LocalMachine`、`Reader.SetProperty.FileSystem.GetFiles(...)` 都经它转发。无任何自定义逻辑，纯框架类。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Watcher | My.Computer 门面（继承 Computer） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| Watcher() | 构造 | 空 |
| CallClass() / FlushClass() | 占位 | return true / false |

## 恶意行为 / 敏感操作
- 无；仅作为恶意代码访问注册表/文件系统的代理层

## IOC / 硬编码
- 无
