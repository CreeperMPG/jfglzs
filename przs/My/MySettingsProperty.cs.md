# My\MySettingsProperty.cs.md — `My.Settings` 入口

## 文件作用

VB 自动生成的 `My.Settings` 入口（`[StandardModule]`），只把 `MySettings.Default` 转发出来。
本工程没有设置项，所以这里除了转发什么都没有。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `MySettingsProperty` | `My.Settings` 的静态入口 |
| `Settings` 属性 | `return MySettings.Default;` |

## 反混淆命名映射

无混淆名可改：类型名与属性名都是 VB 生成器产出、且被工程内引用（`MySettings.Designer.cs` 里
`MySettingsProperty.Settings.Save()`）。

## 关键方法与流程

`Settings` getter 一行：`return MySettings.Default;`。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| 无 | 纯转发 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 特性 | `[HelpKeyword("My.Settings")]` |
