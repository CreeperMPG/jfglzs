# Annotation.cs.md — IntelliLock 未注册版水印

## 文件作用

`OZxTKXuvsl9D34LWhP\Internal\Annotation.cs` 是 IntelliLock 壳残留的"试用版提示"类（`Annotation` 是混淆名）。
它只在首次调用时弹一次 MessageBox，告诉用户"本程序集受未注册版 IntelliLock 保护"；
MessageBox 不可用时把一段 JS `alert` 交给空实现 `EnableWatcher`。

**本工程里没有任何调用点**（`显示未注册提示` 从未被引用），所以这段代码不会执行；
它的存在只说明：原样本除了 .NET Reactor，还叠过 IntelliLock 的壳。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Annotation` | IntelliLock 试用版提示（一次性） |
| `显示未注册提示()`（原名 `smethod_0`） | 用 `_Info` 保证只弹一次 |
| `EnableWatcher(object)`（原名同名） | 空实现，"非交互环境"的降级出口 |
| `NewTag()` | 壳生成的授权标记桩，恒 false |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `smethod_0` | `显示未注册提示` | 唯一的实体方法 |
| `EnableWatcher` | 保留原名 | 空方法，无调用者 |
| `_Info` | `_Info` | "已弹过"标志 |
| `NewTag` | 保留原名 | 授权标记桩 |

## 关键方法与流程

### 显示未注册提示()

1. `_Info` 为 false → 置 true；
2. `MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com")`；
3. 弹窗失败（`catch`）→ `EnableWatcher("<script language='javascript'>alert( … );</script>")`——空实现，什么也不做。

没有循环、没有 `goto`、没有 `On Error Resume Next` 噪声，原文已是干净结构，只做了命名与注释。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| 无 | 只是壳的提示框；不碰文件/注册表/进程/网络 |

> 但"IntelliLock 试用版 + .NET Reactor"这条线索本身对溯源有用：说明作者用多套商业壳叠加保护。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 弹窗标题 | `www.eziriz.com` |
| 弹窗正文 | `This assembly is protected by an unregistered version of "IntelliLock"!` |
| 降级脚本 | `<script language='javascript'>alert( "This assembly is protected by an unregistered version of IntelliLock!" );</script>` |
