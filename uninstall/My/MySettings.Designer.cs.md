# My/MySettings.Designer.cs 分析

## 文件作用

VB.NET 的 **My.Settings** 载体（`ApplicationSettingsBase` 派生），由 Settings 设计器自动生成。
职责很小：包一个线程安全单例，并在第一次取 `Default` 时把「退出即保存」挂到
`My.Application.Shutdown` 事件上。

本程序没有任何自定义设置项（`MySettings.settings` 里只有默认的 `SaveMySettingsOnExit` 等），
所以它在运行时基本什么都不做，只是模板产物。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MySettings` | 设置单例（`SettingsBase.Synchronized` 包装），支持退出时自动保存 |
| `AutoSaveSettings(object, EventArgs)` | `My.Application.Shutdown` 回调：若 `SaveMySettingsOnExit` 为真则 `Save()` |
| `Default` 属性 | 单例入口：首次访问时用 `lock` + `addedHandler` 标志挂一次事件 |
| `defaultInstance` / `addedHandler` / `addedHandlerLockObject` | 单例实例、挂事件标志、SyncLock 用的锁对象 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `MySettings` | 保留 | 本来就是 VB 的正确名字，混淆器没改 |
| `AutoSaveSettings` | 保留 | 同上 |
| `Test.InterruptReader`（原调用点） | `MyProject.Application` | 类型/成员名已按真实身份还原（见 `Internal/Test.cs.md`） |
| 局部 `obj`（锁对象） | `锁对象` | `lock` 的目标 |

## 关键方法与流程

### `Default` getter

```
if (!addedHandler)
{
    锁对象 = addedHandlerLockObject;
    ObjectFlowControl.CheckForSyncLockOnValueType(锁对象);   // VB SyncLock 的类型检查
    lock (锁对象)
    {
        if (!addedHandler) { MyProject.Application.Shutdown += AutoSaveSettings; addedHandler = true; }
    }
}
return defaultInstance;
```

双检锁：外层无锁快速路径，内层加锁再判一次，保证事件只挂一次。
`ObjectFlowControl.CheckForSyncLockOnValueType` 是 VB `SyncLock` 的编译产物（值类型会抛异常），保留。

### `AutoSaveSettings`

`if (MyProject.Application.SaveMySettingsOnExit) MySettingsProperty.Settings.Save();`
—— `Importer` 的构造函数把 `SaveMySettingsOnExit` 设为 `true`，所以程序正常退出时会走一次保存。

## 恶意行为 / 敏感操作

无恶意行为。唯一与安全相关的点：

- `MyProject.Application.Shutdown` 是全局事件，本类在**首次访问设置**时挂接；
  卸载器代码里确实访问了设置（`SettingsBase.Synchronized(new MySettings())` 在静态初始化时就跑），
  所以这条链路在启动时就会建立。

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 生成器 | `Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator` `16.10.0.0` | 说明样本用 VS2019/2022（16.x）编译 |
| 特性 | `[CompilerGenerated]`、`[EditorBrowsable(Advanced)]` | 模板产物标记 |
| Token | 类型 `0x02000009`；方法 `0x0600002E/2F`；字段 `0x0400000C..0E` | 元数据行号 |
