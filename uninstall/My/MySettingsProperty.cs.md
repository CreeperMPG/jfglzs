# My/MySettingsProperty.cs 分析

## 文件作用

VB.NET 自动生成的 **My.Settings 访问模块**（`[StandardModule]` + `[HideModuleName]`）。
只有一个属性 `Settings`，转手返回 `MySettings.Default` 单例。

因为本工程没有声明任何自定义设置项，这个模块实际是空的——只提供「My.Settings 存在」这件事本身。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettingsProperty` | My.Settings 的入口模块（隐藏模块名，VB 里直接写 `My.Settings`） |
| `Settings` 属性 | 返回 `MySettings.Default`（线程安全单例） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `MySettingsProperty` | 保留 | 本来就是生成器的正确名字 |
| `Settings` | 保留 | `[HelpKeyword("My.Settings")]` 已明确身份 |

## 关键方法与流程

`Settings` getter：`return MySettings.Default;` —— 毫无分支。真正逻辑都在 `MySettings` 里
（单例 + 双检锁 + 退出保存），本文件只是别名层。

## 恶意行为 / 敏感操作

无。

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 特性 | `[StandardModule]`、`[HideModuleName]`、`[DebuggerNonUserCode]`、`[HelpKeyword("My.Settings")]` | 模板产物标记 |
| Token | 类型 `0x0200000A`；属性 `0x1700000B`；getter `0x06000030` | 元数据行号 |
