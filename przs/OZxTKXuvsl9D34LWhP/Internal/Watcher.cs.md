# Watcher.cs.md — VB `My.Computer`

## 文件作用

`OZxTKXuvsl9D34LWhP\Internal\Watcher.cs` 是 VB 的 `My.Computer`（`Watcher` 是混淆名，原类型名应为 `MyComputer`）。
它继承 `Microsoft.VisualBasic.Devices.Computer`，自己不实现任何逻辑，
只是让 `Queue.MyComputer.Name` 这类写法可用。本项目唯一实际用到它的地方是
`bianhao` 窗体（取计算机名后 6 位作为机号）。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Watcher : Computer` | `My.Computer` 的外观类型 |
| 构造函数 | 空 |
| `MapTag()` / `PatchTag()` | .NET Reactor 授权标记桩，恒 true / false |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| 类 `Watcher` | 保留原名 | 与文件名绑定；`Queue.MyComputer` 属性返回它 |
| `MapTag` / `PatchTag` | 保留原名 | 授权标记桩 |

## 关键方法与流程

没有流程：构造函数为空，两个静态方法恒定返回 `true` / `false`，全工程无调用者。
真正的功能（`Name`、`Info`、`FileSystem`、`Network` 等）都来自基类 `Computer`。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| 无直接恶意行为 | 只是把基类的"读计算机名"能力暴露出来；`bianhao` 用它显示机号 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 基类 | `Microsoft.VisualBasic.Devices.Computer` |
| 生成器 | `MyTemplate`, `11.0.0.0` |
