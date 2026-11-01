# MySettings.Designer.cs

## 文件作用

VB.NET 「我的设置」宿主类的实现分部。承担两件事：维护全局设置单例；在应用程序退出时按开关决定是否回写设置。工程里 `MySettings.settings` 是空的，因此回写动作实际不产生任何有效配置项。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettings`（分部，继承 `ApplicationSettingsBase`） | 设置单例持有者 + 退出时自动保存 |
| `Default` 属性 | 双检锁初始化：首次访问时挂接 `My.Application.Shutdown` 事件 |
| `AutoSaveSettings` | 退出事件处理器，按 `SaveMySettingsOnExit` 决定是否 `Save()` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `defaultInstance` | `默认实例` | 被 `SettingsBase.Synchronized` 包装的设置单例 |
| `addedHandler` | `已挂退出保存` | 是否已挂接 Shutdown 事件 |
| `addedHandlerLockObject` | `挂接锁` | 双检锁用的锁对象 |
| `obj`（局部） | `锁对象` | 从静态字段取出的锁引用 |
| `AutoSaveSettings` | 保留原名 | 已是语义名，属 VS 生成的标准名 |

## 关键方法与流程

1. **`Default` 取值器**
   - 先无锁读 `已挂退出保存`；
   - 为 false 时取出 `挂接锁`，调 `ObjectFlowControl.CheckForSyncLockOnValueType`（VB 对 lock 值类型的检查，原样保留）；
   - `lock` 内再判一次，未挂过则 `+= AutoSaveSettings` 并置标志；
   - 最后返回 `默认实例`。
2. **`AutoSaveSettings(sender, e)`**
   - 读 `ExpressionMapper.CancelComposer.SaveMySettingsOnExit`（该值在 `ComposerMapper` 构造函数里被设为 true）；
   - 为 true 时调用 `MySettingsProperty.Settings.Save()`。

## 恶意行为 / 敏感操作

无恶意行为。仅涉及 `user.config` 的常规读写路径；由于没有自定义设置项，落盘内容为空。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 事件源 | `My.Application.Shutdown`（`WindowsFormsApplicationBase`） |
| 开关 | `SaveMySettingsOnExit = true`（见 `Internal/ComposerMapper.cs`） |
| 生成器版本 | `16.10.0.0` |
| IL 行号 | 类型 `0x02000009`，方法 `0x0600002E` / `0x0600002F` |
