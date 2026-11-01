# Internal/ListenerMapper.cs

## 文件作用

IntelliLock 未注册对话框的静态入口。设计与 `ILLicenseModule` 配套，属于「授权失败时提醒用户」那一路的壳代码。在本工程（`about`）里没有任何调用点，也不会自行触发。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ListenerMapper` | 一次性弹出 IntelliLock 未注册提示；弹窗失败时退化为「写入警告脚本」，而该脚本写出方法是空实现 |
| `已显示过提示`（静态字段） | 幂等标志，保证只提示一次 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ListenerMapper.smethod_0()` | `显示IntelliLock未注册提示()` | 内部逻辑即弹未授权提示框 |
| `CollectMapper(object)` | `写入警告脚本占位(object 脚本内容)` | 空方法体，参数是那段 js alert 字符串 |
| `RestartFacade()` | `返回假占位()` | 恒 `return false;` |
| `_RepositoryMapper` | `已显示过提示` | 布尔幂等标志 |
| `object_0` | `脚本内容` | 传入的 javascript 字符串 |

## 关键方法与流程

1. **`显示IntelliLock未注册提示()`**
   - 读 `已显示过提示`，为 true 直接返回；
   - 先置 `已显示过提示 = true`（顺序很关键：失败不会重试，也不会反复弹窗）；
   - `try` 里 `MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com")`；
   - `catch { }` 时调用 `写入警告脚本占位("<script language='javascript'>alert( \"...\" );</script>")`——注意这个调用点也是**吞掉异常**的兜底，写法保留原样。
2. **`写入警告脚本占位(object)`**：空方法体，无任何副作用。
3. **`返回假占位()`**：`return false;`，无调用点。

## 恶意行为 / 敏感操作

- 不是恶意行为，但属**授权提示弹窗**逻辑，消息里带第三方厂商域名 `www.eziriz.com`（Eziriz 是 IntelliLock/.NET Reactor 的作者公司）。
- 无注册表、无文件、无网络、无进程操作。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 弹窗标题 | `www.eziriz.com` |
| 弹窗正文 | `This assembly is protected by an unregistered version of "IntelliLock"!` |
| 兜底脚本 | `<script language='javascript'>alert( "This assembly is protected by an unregistered version of IntelliLock!" );</script>` |
| 类型/方法 | `ListenerMapper.smethod_0`（`0x060000A6`）、`CollectMapper`（`0x060000A7`） |
