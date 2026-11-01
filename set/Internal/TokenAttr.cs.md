# Internal\TokenAttr.cs —— 文件级分析

## 文件作用
IntelliLock 试用版的「未注册」提示类。首次调用 `提示未注册()` 时弹一个 MessageBox 告知「本程序集由未注册版 IntelliLock 保护」，并用静态标志保证只弹一次；弹窗失败则降级到一个**空方法**（等于静默忽略）。这是加壳工具留下的水印代码，与业务逻辑无关。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `TokenAttr` | IntelliLock 未注册提示容器 |
| `提示未注册()`（旧名 `smethod_0`） | 一次性弹窗提示；异常时走空方法降级 |
| `输出脚本弹窗(object)`（旧名 `InitAttr`） | 原为注入 JS 弹窗的辅助方法，**方法体已被抹空**，调用无效果 |
| `恒真桩()` / `恒假桩()` | 恒值死代码桩，无调用点 |
| `已提示`（旧名 `_ProcAttr`） | 静态布尔，控制「只提示一次」 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `smethod_0()` | `提示未注册()` | dnSpy 的无名方法占位名，按行为重命名 |
| `InitAttr(object object_0)` | `输出脚本弹窗(object 脚本)` | 参数是无名字符串，实际传入一段 `<script>` 文本 |
| `_ProcAttr` | `已提示` | 语义是「已经弹过窗了」 |
| `DisableFactory()` | `恒真桩()` | `return true;`，无调用点 |
| `ForgotFactory()` | `恒假桩()` | `return false;`，无调用点 |

## 关键方法与流程（按方法分段，写清步骤）
`提示未注册()`
1. `if (!已提示)` —— 已提示过就直接返回（幂等）。
2. `已提示 = true` —— 先置标志，因此**弹窗抛异常后也不会重试**。
3. `try { MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com"); }`
4. `catch { 输出脚本弹窗("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>"); }` —— 弹窗失败时的降级路径；而该方法是空方法，所以实际效果是**静默吞掉异常**。

`输出脚本弹窗(object 脚本)`：空方法体（原版会把它灌进 WebBrowser 控件）。

## 恶意行为 / 敏感操作
- 无恶意行为。
- 取证价值：`www.eziriz.com` + "unregistered version of IntelliLock" 说明样本使用**未授权**的加壳工具，属于盗版工具链的痕迹。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- `This assembly is protected by an unregistered version of "IntelliLock"!`
- `www.eziriz.com`（MessageBox 标题 / 厂商域名）
- `<script language='javascript'>alert( "This assembly is protected by an unregistered version of IntelliLock!" );</script>`

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无 `IL_xxxx:` / `num2 = N` / `ClearProjectError()`
- 保留 6 条 `// Token:` 注释
- **语义保护**：`catch` 分支原样保留（不许删），字符串常量（含 `\"` 转义与 `<script>` 里的空格）逐字符保留，两处 `MessageBox.Show` 参数顺序未变
