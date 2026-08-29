# My/MySettingsProperty.cs — My.Settings 访问器（自动生成）

## 文件作用
VB.NET 标准 `MySettingsProperty` 隐藏模块：暴露 `Settings` 属性（返回 `MySettings.Default`），供 `My.Settings` 语法使用。**无任何业务逻辑**。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `MySettingsProperty` | My.Settings 门面 |

## 关键方法与流程
- `Settings` getter：`return MySettings.Default;`（带 `HelpKeyword("My.Settings")`）。

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 无。
