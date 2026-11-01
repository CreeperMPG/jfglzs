# My\MySettings.Designer.cs.md — VB 应用程序设置（半体二）

## 文件作用

VB `My.Settings` 的实现主体：全局单例 `Default` + 程序退出时的自动保存。
本工程没有任何设置项（`My\MySettings.settings` 是空模板），所以这段代码只会"挂一个退出事件"。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `MySettings`（partial） | 设置类主体 |
| `Default` 属性 | 同步单例；首次访问时把退出处理器挂到 `Queue.MyApplication.Shutdown` |
| `AutoSaveSettings` | 程序退出时按 `SaveMySettingsOnExit` 决定是否 `Save()` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `obj`（`lock` 的对象） | `锁对象` | dnSpy 的局部名 |
| `defaultInstance` / `addedHandler` / `addedHandlerLockObject` | 保留原名 | 生成器命名的字段，语义已可读 |
| `Queue.ResetVal` | `Queue.MyApplication` | 跨文件改名 |

## 关键方法与流程

### `Default` getter

1. `addedHandler` 为 false → 取 `addedHandlerLockObject`；
2. `ObjectFlowControl.CheckForSyncLockOnValueType(锁对象)`（VB 的 `SyncLock` 前置检查，值类型会抛）；
3. `lock (锁对象)` 内再判一次 `addedHandler`，为 false 就：
   `Queue.MyApplication.Shutdown += MySettings.AutoSaveSettings;` → `addedHandler = true`；
4. 返回 `defaultInstance`（`SettingsBase.Synchronized(new MySettings())` 产生的同步包装）。

### `AutoSaveSettings(object, EventArgs)`

`Queue.MyApplication.SaveMySettingsOnExit` 为真 → `MySettingsProperty.Settings.Save()`。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| 无 | 模板代码，不涉及文件/注册表/进程；注意 `SaveMySettingsOnExit` 在 `Val` 构造函数里被设为 true |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 生成器 | `SettingsSingleFileGenerator`, `16.10.0.0` |
