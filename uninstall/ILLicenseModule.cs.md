# ILLicenseModule.cs — IntelliLock 授权运行时占位模块

## 文件作用
IntelliLock（Eziriz）许可保护工具注入的模块占位类，仅含两个恒值桩方法。

## 说明
- `PostRegistry()` 恒返回 `true`、`TestRegistry()` 恒返回 `false`，是授权运行时生成的混淆桩，无实际逻辑。
- 佐证本程序集还经过 IntelliLock 授权保护（另见 `Internal/ContextConfig.cs` 的未注册弹窗）。
- 无恶意行为，无 IOC。
