# MySettings.Designer.cs — VB 应用设置自动生成（My.Settings）

## 文件作用
VB 设置设计器生成的 `MySettings` 类（继承 `ApplicationSettingsBase`），提供 `Default` 单例，并在应用关闭（Shutdown）时若 `SaveMySettingsOnExit` 为真则自动保存设置。

## 说明
- 自动生成文件，无手工恶意逻辑。
- 关联 `MySettingsProperty.Settings` 与 `VisitorConfig.CollectUtils`（My.Application）。
- 本组件未定义任何自定义设置项，无 IOC。
