# My/MySettings.Designer.cs — My.Settings 设计器分部类（自动生成）

## 文件作用
VB.NET `My.Settings` 的设计器分部：`Default` 属性（线程安全懒加载单例，`SettingsBase.Synchronized`）与 `AutoSaveSettings`（应用 Shutdown 时若 `SaveMySettingsOnExit` 则 `Save()`）。纯编译器生成代码，无恶意逻辑。

## 关键方法与流程
- `Default`：首次访问时把 `AutoSaveSettings` 挂到 `TokenizerConfig.PopTemplate.Shutdown`（即 My.Application 关闭事件），返回 `defaultInstance`。
- `AutoSaveSettings`：`TokenizerConfig.PopTemplate.SaveMySettingsOnExit` 为 true 时保存设置。

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 无。
