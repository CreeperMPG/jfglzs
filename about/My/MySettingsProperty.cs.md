# MySettingsProperty.cs

## 文件作用

VB.NET 「我的设置」门面类。作用是把 `MySettings.Default` 包成 `My.Settings` 这个对业务代码更顺手的入口。属于 Visual Studio 模板自动生成的样板代码，没有业务加工。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettingsProperty`（`[StandardModule]` 模块） | 设置门面，唯一成员 `Settings` 直接转发到 `MySettings.Default` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `Settings`（get） | 保留 `Settings` | 原名即 VB 语义名 `My.Settings`，无需改名 |

## 关键方法与流程

1. `Settings` 取值器：`return MySettings.Default;`。
   - `MySettings.Default` 内部是第一层加锁检查单例（见 `MySettings.Designer.cs`），并会挂一次 `Shutdown` 自动保存事件。
   - 本方法自身不做判空、不做懒加载，语义就是纯转发。

## 恶意行为 / 敏感操作

无。既不写盘也不读注册表；工程里没有自定义设置项，因此连 `user.config` 都不会产生实质内容。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 特性标记 | `[StandardModule]`、`[HideModuleName]`、`[HelpKeyword("My.Settings")]` |
| IL 行号 | `0x0200000A`、属性 `0x1700000B`、方法 `0x06000032` |
