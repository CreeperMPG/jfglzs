# ContextConfig.cs — IntelliLock 未注册版本提示（授权运行时）

## 文件作用
IntelliLock（Eziriz）授权运行时注入的提示模块：首次调用时弹窗显示 "This assembly is protected by an unregistered version of \"IntelliLock\"!"（标题 www.eziriz.com）。属于加壳/授权工具的试用版水印，与业务逻辑无关。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| ContextConfig | IntelliLock 试用版弹窗 |
| `m_StructConfig` | 是否已弹窗标志 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `smethod_0` | 显示未注册提示 | 只弹一次 |
| `ManageConfig` | 备用弹窗路径 | 空实现（Web 场景的 JS alert 已废弃） |
| `ListRegistry/InitRegistry` | 恒值桩 | true/false |

## IOC / 硬编码
- 弹窗文本：`This assembly is protected by an unregistered version of "IntelliLock"!`
- 弹窗标题/URL：`www.eziriz.com`
- 佐证样本使用 IntelliLock 授权保护（可能叠加于 .NET Reactor 之上）
