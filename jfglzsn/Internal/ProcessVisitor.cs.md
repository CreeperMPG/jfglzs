# Internal/ProcessVisitor.cs — 壳未注册提示（IntelliLock 试用版弹窗）

## 文件作用
.NET Reactor 附属产品 **IntelliLock（授权保护）** 的"未注册版本"提示逻辑：程序被识别为未注册授权时弹 MessageBox 提示"该程序集受未注册版 IntelliLock 保护"，网址指向 `www.eziriz.com`（Reactor 厂商）。`smethod_0` 用静态标志 `broadcasterVisitor` 保证只提示一次；MessageBox 失败则走 `LoginVisitor`（空实现）。无恶意行为，属壳授权提醒残留。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `ProcessVisitor` | 壳授权提示器 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `smethod_0` | 显示未注册提示 | 一次性（broadcasterVisitor 标志） |
| `LoginVisitor` | 提示备用路径 | 空实现 |
| `DisableMethod` | 壳桩 | 恒 true |
| `broadcasterVisitor` | 是否已提示标志 | 静态 bool |

## 关键方法与流程

### 显示未注册提示（原名 `smethod_0`）
- **流程**：1. `broadcasterVisitor` 为 false 则置 true；2. `MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com")`；3. 异常时调用空方法 `LoginVisitor("<script language='javascript'>alert(...)</script>")`（传 HTML script 字符串但函数体为空）。
- **关键调用/API**：`MessageBox.Show`。

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- **域名**：`www.eziriz.com`（.NET Reactor / IntelliLock 厂商，非恶意 C2）。
- **提示文案**："This assembly is protected by an unregistered version of IntelliLock!"。
