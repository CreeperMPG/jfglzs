# Internal/RegReg.cs — My.Computer 门面（自动生成）

## 文件作用
VB.NET `MyTemplate` 自动生成的 `My.Computer` 门面类，继承 `Microsoft.VisualBasic.Devices.Computer`（提供文件系统、剪贴板、注册表、网络等 My.Computer.* 能力入口）。**无任何业务逻辑**，仅两个混淆填充方法。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `RegReg` | My.Computer 门面（自动生成） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `ForgotParser` | 混淆填充 | 恒真 |
| `AddParser` | 混淆填充 | 恒假 |

## 关键方法与流程
- 空类；`[EditorBrowsable(Never)]` 表示设计器隐藏。
- 说明：若业务代码通过 `My.Computer.Registry/FileSystem` 操作，会经由继承的 Computer 成员，但本反编译代码未见调用。

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 无。
