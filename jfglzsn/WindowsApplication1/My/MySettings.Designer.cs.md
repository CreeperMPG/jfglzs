## 文件作用

VB.NET「我的设置」（`My.Settings`）单例，模板自动生成（`ApplicationSettingsBase` 子类）。

与标准生成版的差别在于：单例首次被访问时会往 `SetterEvent.CollectCallback.Shutdown` 挂一个保存钩子，于是程序退出前按 `SaveMySettingsOnExit` 决定是否落盘用户设置。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettings : ApplicationSettingsBase` | My.Settings 单例容器 |
| `Default` 属性 | 双重检查加锁的单例取值，首次访问挂接保存钩子 |
| `AutoSaveSettings(object, EventArgs)` | 关机事件处理器：按开关决定是否 `Save()` |
| `defaultInstance` | 单例本体（构造后交给 `SettingsBase.Synchronized` 包一层线程安全代理） |
| `addedHandler` / `addedHandlerLockObject` | 幂等标志 / 加锁对象 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `MySettings` / `MySettingsProperty` | 未改名 | 模板生成，且与文件名绑定 |
| `addedHandler` / `addedHandlerLockObject` | 未改名 | 模板生成字段，名字本身可读 |
| `obj`（`Default` getter 内） | `obj`（保留） | 局部变量持有锁对象，模板生成如此；改名收益低且易与锁语义混淆，保留 |
| `AutoSaveSettings(object sender, EventArgs e)` | 未改名 | 事件处理器签名，未被改名 |

## 关键方法与流程

### `Default` getter（双重检查加锁）

1. `addedHandler` 已为真 → 直接返回 `defaultInstance`（快路径无锁）。
2. 否则取锁对象 `addedHandlerLockObject`，先做 `ObjectFlowControl.CheckForSyncLockOnValueType(obj)`（VB `SyncLock` 的运行时检查；对象是引用类型，不会抛）。
3. `lock (obj)` 内二次检查 `addedHandler`：
   - 仍为假 → `SetterEvent.CollectCallback.Shutdown += AutoSaveSettings;` 然后 `addedHandler = true;`
4. 返回 `defaultInstance`。

### `AutoSaveSettings(sender, e)`

```
if (SetterEvent.CollectCallback.SaveMySettingsOnExit) { MySettingsProperty.Settings.Save(); }
```

无 try/catch —— 若 `Save()` 抛异常（user.config 无写权限），会走默认的未处理异常路径。**这一点与原版一致**，不要"顺手"加 try。

### 静态字段

| 字段 | 初值 |
| --- | --- |
| `defaultInstance` | `(MySettings)SettingsBase.Synchronized(new MySettings())` |
| `addedHandler` | `false` |
| `addedHandlerLockObject` | `RuntimeHelpers.GetObjectValue(new object())` |

## 恶意行为 / 敏感操作

本文件自身无恶意行为，但有一个与分析相关的细节：

**`Save()` 会在 `%LOCALAPPDATA%\...\user.config` 留痕**。本工程的 `MySettings.settings` 里没有定义任何设置项（只有模板空壳），因此实际写出的配置文件内容极少，情报价值低；但文件存在本身可作为「程序运行过」的取证证据。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 生成器标记 | `[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]` |
| 保存开关来源 | `SetterEvent.CollectCallback.SaveMySettingsOnExit`（在 `CallbackEvent` 构造里被设为 `true`） |
| 副作用产物 | 用户级 `user.config`（由框架决定路径） |
