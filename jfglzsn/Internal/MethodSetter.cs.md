## 文件作用

IntelliLock 授权提示的**残渣模块**：只负责弹一个「本程序集被未注册版 IntelliLock 保护」的对话框，弹不出就退回到往网页里注入 JS alert。不做任何真实校验。

这个文件是判定「样本用过哪些保护工具」的直接证据——`AGENTS.md` 记录的壳是 .NET Reactor，而这里的文案与 `www.eziriz.com`（IntelliLock 作者站点）说明样本链条上还叠过 IntelliLock，或保护工具被混用。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MethodSetter` | IntelliLock 提示弹窗（一次性，进程内只弹一次）+ 两个授权判定桩 |
| `显示未注册提示()`（原 `smethod_0`） | 弹出未注册提示；失败则调 `FlushEvent` 降级 |
| `FlushEvent(object)` | 降级桩，本样本**空实现**，因此 catch 分支里的脚本串永远不会被展示 |
| `ForgotRule()` / `DefineRule()` | 授权判定桩，恒 `true` / `false`，工程内无引用 |
| `m_提示已弹出`（原 `m_ParserSetter`） | 一次性标志 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `smethod_0()` | `显示未注册提示()` | dnSpy 的占位名（`smethod_0` = 第 0 个静态方法），不是真实方法名；工程内无调用点，改名安全 |
| `m_ParserSetter` | `m_提示已弹出` | 壳随机生成的字段名，实际语义是「提示是否已弹过」 |
| `object_0`（`FlushEvent` 参数） | `提示源` | 原版参数名，实际是待展示的 HTML/脚本片段；本样本未使用 |
| `ForgotRule` / `DefineRule` | 未改名 | 无 API/字符串可推断语义，保留原名并注明是授权桩 |

## 关键方法与流程

### `显示未注册提示()`

1. 若 `m_提示已弹出` 已是 `true` —— 直接返回（进程生命周期内只提示一次）。
2. **先**把标志置 `true`，再进 try（这一点很关键：弹窗失败也不会重试）。
3. `MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com")`。
4. 若上一步抛异常（无交互桌面 / 被安全软件拦截），进入 `catch` 调 `FlushEvent("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>")`。
5. `FlushEvent` 是空方法，所以降级路径实际什么都没发生。

### 授权桩

| 方法 | 返回 | 说明 |
| --- | --- | --- |
| `ForgotRule` | `true` | 视为「已注册」 |
| `DefineRule` | `false` | 视为「非受限」 |

与 `ILLicenseModule` 的两个方法同构，说明真实授权已被旁路。

## 恶意行为 / 敏感操作

无恶意行为。相关但无害的观察：

1. **保护工具指纹**：`IntelliLock` + `www.eziriz.com` 字符串；本工程未引用 `.NET Reactor` 的同类文案，可交叉印证保护链条。
2. **反分析意图**：`AssemblyInfo.cs` 里的 `[SuppressIldasm]` + 这里的提示弹窗，共同构成「阻止逆向」的两层安排。
3. `MessageBox.Show` 与 `catch` 后的静默，意味着该方法在无桌面会话（服务/计划任务）下等于空操作，不会造成崩溃。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 字符串 | `This assembly is protected by an unregistered version of "IntelliLock"!` |
| 字符串 | `www.eziriz.com` |
| 字符串 | `<script language='javascript'>alert( "This assembly is protected by an unregistered version of IntelliLock!" );</script>` |
| 窗口标题 | `www.eziriz.com` |
| 返回值常量 | `true`、`false` |
| 异常码 | 无 |
