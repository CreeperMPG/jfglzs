# WindowsApplication1\My\MySettingsProperty.cs —— 文件级分析

## 文件作用
VB.NET `My.Settings` 的访问入口模板类（`[StandardModule]` + `[HideModuleName]`）。唯一作用是把 `My.Settings` 映射到 `MySettings.Default` 单例。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettingsProperty` | VB 模板生成的静态类，只暴露一个 `设置` 属性 |
| `设置`（旧名 `Settings`） | 返回 `MySettings.Default`（全局设置单例） |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `Settings` 属性 | `设置` | `[HelpKeyword("My.Settings")]` 对应的成员；引用点只有 `MySettings.Designer.cs` 的 `AutoSaveSettings` |
| `MySettingsProperty` | 未改名 | 类名保留 |

## 关键方法与流程（按方法分段，写清步骤）
- `设置` getter：`return MySettings.Default;` —— 一行转发，无异常处理、无副作用。

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- `My.Settings`（`HelpKeyword` 字符串常量）

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无
- 保留 2 条 `// Token:` 注释
