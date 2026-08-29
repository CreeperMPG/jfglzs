# ConfigConfig.cs — My.Computer 包装类（自动生成）

## 文件作用
VB.NET MyTemplate 自动生成的 `My.Computer` 访问器：继承 `Microsoft.VisualBasic.Devices.Computer`，暴露计算机信息/剪贴板/网络等能力。业务代码通过 `VisitorConfig.StopUtils` 获取（本组件仅用其 `Registry` 属性间接访问注册表根键，见 RegConfig）。

## 说明
- 构造函数为空；`LogoutRegistry()` 恒 true、`CallRegistry()` 恒 false，均为混淆桩。
- 无恶意行为，无 IOC。
