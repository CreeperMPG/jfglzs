## 文件作用

VB.NET 的 `My.Settings` **入口模块**：把 `MySettings.Default` 暴露成静态属性，
让源码能写 `My.Settings.XXX`。只有一个 getter，没有 setter，也没有任何真正的配置读写逻辑。

本工程里没有任何地方使用它（业务代码没有读写配置）。属于「VB 工程骨架」的一部分。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettingsProperty`（`[StandardModule]`、`[HideModuleName]`） | `My.Settings` 的转发入口 |
| `Settings`（静态属性） | `return MySettings.Default;` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `MySettingsProperty` | 保持原名 | 类名本身就是 VB 模板生成的（`[HelpKeyword("My.Settings")]` 佐证） |
| `Settings` | 保持原名 | 模板标准属性名 |

## 关键方法与流程

- `Settings` 的 getter：`return MySettings.Default;`（转发到 `My/MySettings.Designer.cs` 里的单例）。

没有其它成员、没有构造、没有副作用。

## 恶意行为 / 敏感操作

无。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 特性 | `[HelpKeyword("My.Settings")]`、`[CompilerGenerated]`、`[HideModuleName]`、`[StandardModule]`、`[DebuggerNonUserCode]` |
| 路径 / 注册表 / URL / 进程名 / 字符串常量 | 无 |
