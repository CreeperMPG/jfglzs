## 文件作用

`My.Settings` 的**设计器生成部分**（partial 的另一半）：只提供一个线程安全的静态单例 `Default`。
因为设置文件是空的，这个单例没有任何配置项，实际价值为零——但它解释了
`MySettingsProperty.Settings` 的取值来源。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettings : ApplicationSettingsBase`（partial） | 与 `MySettings.cs` 同一个类；本文件负责单例 |
| `Default`（静态属性） | 返回 `defaultInstance` |
| `defaultInstance`（静态字段） | `(MySettings)SettingsBase.Synchronized(new MySettings())` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `MySettings` | 保持原名 | VB 模板标准类型名 |
| `defaultInstance` | 保持原名 | 模板标准字段名 |
| `Default` | 保持原名 | 模板标准属性名 |

## 关键方法与流程

### `Default` 的 getter
- `return MySettings.defaultInstance;`

### 静态字段初始化（类型初始化时执行一次）
- `defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());`
  用 `SettingsBase.Synchronized` 包一层，让多线程访问安全；这是 VB 模板的固定写法，不要改成手写 `lock`。

## 恶意行为 / 敏感操作

无。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 特性 | 与 `MySettings.cs` 相同（`[EditorBrowsable(Advanced)]`、`[CompilerGenerated]`、`[GeneratedCode(…SettingsDesigner…, "16.10.0.0")]`） |
| 依赖的 settings 文件 | `My/MySettings.settings`（`<Profiles />`、`<Settings />` 均为空） |
| 路径 / 注册表 / URL / 进程名 | 无 |
