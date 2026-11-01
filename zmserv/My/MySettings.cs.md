## 文件作用

`My.Settings` 的宿主类型（`ApplicationSettingsBase` 派生）的**两个 partial 文件之一**：
本文件只放两个壳注入的空桩，真正的单例在 `MySettings.Designer.cs`。

`My/MySettings.settings` 里 `<Profiles />` 与 `<Settings />` 都是空的，
所以这个类**不承载任何配置项**——样本没有用 `My.Settings` 存过任何东西（配置都在别处）。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettings : ApplicationSettingsBase`（partial） | `My.Settings` 的宿主；本文件提供两个固定返回常量的空桩 |
| `DeleteDefinition()` / `SetDefinition()` | 壳注入空桩：`return true` / `return false`，无调用点 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `MySettings` | 保持原名 | 类名本身就是 VB 模板生成的 `My.Settings` 类型名（不是混淆产物） |
| `DeleteDefinition()` | 保持原名 | 空桩（`true`）；**注意不要误读成「删除设置」的实现** |
| `SetDefinition()` | 保持原名 | 空桩（`false`） |

## 关键方法与流程

- `DeleteDefinition()` → `return true;`
- `SetDefinition()` → `return false;`

没有流程。两个方法名里的 `Definition` 是加壳器随机词表的后缀，
和「定义/设置」的中文含义无关——按动态分析经验，这类 `*Definition()` 方法在解壳后的样本里
往往成对出现（一个 `true`、一个 `false`），全部无调用点。

## 恶意行为 / 敏感操作

无。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 特性 | `[EditorBrowsable(EditorBrowsableState.Advanced)]`、`[CompilerGenerated]`、`[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]` |
| 基类 | `System.Configuration.ApplicationSettingsBase` |
| 对应配置文件 | `My/MySettings.settings`（生成类命名空间 `zmserv.My`、类名 `MySettings`，内容为空） |
| 路径 / 注册表 / URL / 进程名 | 无 |
