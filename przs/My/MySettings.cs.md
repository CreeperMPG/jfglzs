# My\MySettings.cs.md — VB 应用程序设置（半体一）

## 文件作用

`MySettings` 的 `partial` 另一半（Designer 生成），与 `MySettings.Designer.cs` 一起构成
VB 的 `My.Settings`。本工程**没有定义任何设置项**，所以这里只有一个 .NET Reactor 塞进来的
`ManageTag()` 桩（恒 true，无调用者）。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `MySettings : ApplicationSettingsBase`（partial） | 应用设置类 |
| `ManageTag()` | 授权标记桩，恒 true |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ManageTag` | 保留原名 | 桩函数；XML 注释已标注用途 |

## 关键方法与流程

无流程：`ManageTag()` 直接 `return true;`。真正的内容（`Default` 单例、退出保存）在
`MySettings.Designer.cs` 里。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| 无 | 模板代码 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 生成器 | `SettingsSingleFileGenerator`, `16.10.0.0` |
