## 文件作用

VB.NET 模板生成的 `My.Settings` 访问入口（`[StandardModule]` 语义的静态类）。业务代码统一通过 `MySettingsProperty.Settings` 取单例，不直接 `new`。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettingsProperty`（`[StandardModule]`） | 提供 `My.Settings` 这一个静态属性 |
| `Settings` 属性 | 直接转发 `MySettings.Default` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `MySettingsProperty` | 未改名 | 模板生成 + 文件名绑定 |
| `Settings` | 未改名 | 模板生成，且带 `[HelpKeyword("My.Settings")]` 元数据 |

本文件没有任何混淆痕迹，也没有可改的占位名。

## 关键方法与流程

只有一个 getter：

```
internal static MySettings Settings { get { return MySettings.Default; } }
```

调用它会触发 `MySettings.Default` 的首次访问逻辑——即挂接退出时保存钩子（见 `MySettings.Designer.cs.md`）。工程内引用点：`MySettings.AutoSaveSettings` 里通过 `MySettingsProperty.Settings.Save()` 拿到实例。

## 恶意行为 / 敏感操作

无。纯转发层，不访问注册表、不落盘、不联网。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 特性 | `[HelpKeyword("My.Settings")]`、`[StandardModule]`、`[HideModuleName]`、`[CompilerGenerated]`、`[DebuggerNonUserCode]` |
| 常量 | 无 |
