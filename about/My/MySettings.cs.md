# MySettings.cs

## 文件作用

VB.NET 「我的设置」（`My.Settings`）宿主类的两个分部文件之一。这个分部只包含两个常量返回桩；真正的设置容器、默认单例和「退出时自动保存」逻辑在同名的 `MySettings.Designer.cs` 分部里。

`ApplicationSettingsBase` 意味着设置项最终落到用户配置（`user.config`）里，但本工程没有任何自定义设置项（`MySettings.settings` 是空的），所以它实际上不持久化任何东西。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettings`（分部） | `My.Settings` 宿主。本分部只提供两个恒真/恒假的桩方法，不参与业务 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `CompareFacade()` | `返回真占位()` | 恒 `return true;` |
| `InvokeFacade()` | `返回假占位()` | 恒 `return false;` |

## 关键方法与流程

1. `返回真占位()`：`return true;`，无副作用、无调用点。
2. `返回假占位()`：`return false;`，无副作用、无调用点。

以上即本文件全部逻辑。设置项的存取不在这里，见 `MySettings.Designer.cs`。

## 恶意行为 / 敏感操作

无。既不写文件也不读写注册表；设置内容为空，不会留下持久化痕迹。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 生成器标识 | `Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator`，版本 `16.10.0.0`（说明作者用 VS2019 16.10 编译） |
| 基类 | `System.Configuration.ApplicationSettingsBase` |
| IL 行号 | `0x02000009`、方法 `0x06000030` / `0x06000031` |
