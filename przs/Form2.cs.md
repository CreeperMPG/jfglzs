# Form2.Designer.cs.md — 全屏锁屏 / 密码退出 / 隐藏后门窗

## 文件作用

`Form2` 是"机房管理助手"的**全屏警告+锁屏窗**，也是整个样本里**用户唯一能碰到"退出"的地方**。
它无边框、最大化、`TopMost`，用 `ClipCursor` 把鼠标锁在左上角 500×100 的矩形里，
关窗事件被无条件取消；只有输入正确密码才会放开鼠标并结束程序。

同一个窗体里还藏着一个**后门**：右上角那个看起来空白的标签 `Label_pd`，双击会显示"暗号"、
把输入框里的明文密码写进注册表、把密码**重置为 111111**，并启动 `yz.exe`。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Form2` | 全屏锁屏 + 密码校验 + 后门入口 |
| `初始化组件`（原名 `ViewVal`） | Designer 的 InitializeComponent |
| `定时器_锁鼠标并报警`（原名 `CustomizeVal`） | 每秒 `ClipCursor(0,0,500,100)`；≥5 次后 `Beep(2000,500)` |
| `校验退出密码`（原名 `ReflectVal`） | 长度检查 + `SHA256(输入+盐)` 与注册表 `n` 比对 |
| `退出小助手_停服务杀进程`（原名 `RestartVal`） | 杀 `jfglzsn`、放开 DisableCMD、`net stop zmserv`、杀 `zmserv`、解除鼠标锁、`EndApp` |
| `隐藏标签_双击后门`（原名 `InterruptVal`） | 显示暗号、写 `dtmm`、把密码重置为 `111111`、启动 `yz.exe` |
| `禁止关闭窗体`（原名 `ReadVal`） | `e.Cancel = true` |
| `显示时启用定时器`（原名 `CancelVal`） | 窗体显示就启动锁鼠标定时器 |
| `重启电脑`（原名 `ComputeVal`） | `shutdown /r /f /t 0` |
| `pc_num` | 计算机名末字符的 ASCII（机器码后缀） |
| `RECT` | Win32 `ClipCursor` 用的矩形结构 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ViewVal` | `初始化组件` | InitializeComponent |
| `CustomizeVal` | `定时器_锁鼠标并报警` | Timer_frm2.Tick |
| `ReflectVal` | `校验退出密码` | Button1.Click |
| `InterruptVal` | `隐藏标签_双击后门` | Label_pd.DoubleClick |
| `RestartVal` | `退出小助手_停服务杀进程` | 密码正确后的收尾 |
| `ReadVal` | `禁止关闭窗体` | FormClosing |
| `CancelVal` | `显示时启用定时器` | Form.Load |
| `ComputeVal` | `重启电脑` | Button2.Click |
| `pc_num` | 保留原名 | 语义已可读（PC 编号） |
| `m_Authentication` | `组件容器` | IContainer |
| `@ref` / `_Server` / `_State` / `decorator` / `m_Interpreter` / `_Prototype` | `…后备字段` | 未使用的 Designer 后备字段 |
| `_Invocation` | `重启按钮` | Button2 后备字段 |
| `m_Mapping` | `确定按钮` | Button1 后备字段 |
| `m_Merchant` | `锁鼠标定时器` | Timer_frm2 后备字段 |
| `tests` | `隐藏标签` | Label_pd 后备字段 |
| `_Mock` | `锁鼠标次数` | Tick 计数 |
| `text` / `text2` / `obj` / `num2` / `num3` / `num4` | 按语义命名 | dnSpy 局部名 |
| `Proccesor.UpdateVal` | `Proccesor.计算SHA256加盐` | 跨文件改名 |
| `Proccesor.InsertVal` / `WriteVal` | `Proccesor.读取注册表值` / `写入注册表值` | 跨文件改名 |

## 关键方法与流程

### 校验退出密码(object, EventArgs)

1. `Strings.Len(TextBox1.Text) < 6` → `Label2 = "密码应该是6位以上的字母或数字，请重新输入！"`、清空输入框、结束；
2. 否则 `输入摘要 = SHA256(Trim(输入) + "bfdshgs")`（`Proccesor.计算SHA256加盐`）；
3. `注册表摘要 = HKCU\Software` 的 `n` 值；
4. 相等 → 清提示、`Button1.Enabled = false`、调 `退出小助手_停服务杀进程()`；
5. 不等 → `Label2 = "密码不正确，请重新输入！"`、清空输入框。

### 退出小助手_停服务杀进程()

1. 死循环杀 `jfglzsn`：`GetProcessesByName("jfglzsn")` → `[0].Kill()` → 数组长度为 0 就 `break` → 否则 `Sleep(500)`；
   **`[0]` 在进程不存在时下标越界，原版就是靠 `On Error Resume Next` 吞掉它再走 `Length == 0` 退出**；
2. 写 `HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD = 0`（放开 cmd）；
3. `Interaction.Shell("net stop zmserv", Hide, true, -1)`（等它跑完）；
4. `Thread.Sleep(3000)`；
5. 杀 `zmserv` 进程（同样吞掉下标越界）；
6. `ClipCursor` 恢复成整屏矩形，`ProjectData.EndApp()` 结束进程。

### 隐藏标签_双击后门()

1. `Label2.Text = "ngfxd568bdh158" + pc_num() + "8bsdg45ybfdhg"`
   —— 末尾那段数字是"计算机名最后一个字符的 ASCII"，等于每台机器的暗号；
2. `当前输入 = TextBox1.Text`（**明文密码**）；写 `HKCU\Software\dtmm`；
3. `默认密码摘要 = SHA256("111111" + "bfdshgs")`；写 `HKCU\Software\czmm`
   —— 把退出密码**重置成 111111**；
4. `Process.Start(<程序所在目录>\yz.exe)`。

### 定时器_锁鼠标并报警 / 重启电脑

- 定时器：`TopMost = true` → `ClipCursor(0,0,500,100)` → `锁鼠标次数++` → ≥5 时 `Beep(2000, 500)`。
- 重启：`Interaction.Shell("shutdown /r /f /t 0", AppWinStyle.MinimizedFocus, false, -1)`。
  保留 `Interaction.Shell`（窗口样式与等待语义与 `Process.Start` 不同）。

## 恶意行为 / 敏感操作

| 行为 | 位置 | 说明 |
| --- | --- | --- |
| **锁屏** | `定时器_锁鼠标并报警` | `TopMost` + 最大化无边框 + `ClipCursor` 限制鼠标 + 蜂鸣 |
| **禁止关闭** | `禁止关闭窗体` | `e.Cancel = true`，Alt+F4 / 关闭按钮全无效 |
| **强制重启** | `重启电脑` | `shutdown /r /f /t 0`（`/f` 强制结束程序，不提示保存） |
| **停服务** | `退出小助手_停服务杀进程` | `net stop zmserv` + 杀 `zmserv` 进程 |
| **进程查杀** | 同上 | `jfglzsn`、`zmserv` |
| **注册表写入（明文密码）** | `隐藏标签_双击后门` | `HKCU\Software\dtmm` = 用户输入的明文密码 |
| **密码重置后门** | 同上 | `HKCU\Software\czmm` = `SHA256("111111"+盐)` |
| **暗号泄露机器信息** | 同上 | 暗号里带计算机名末字符 ASCII |
| **启动伴生程序** | 同上 | `yz.exe` |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 密码摘要盐 | `bfdshgs`（在 `Proccesor.计算SHA256加盐` 里） |
| 密码注册表 | `HKCU\Software`：`n`（正确密码摘要）、`dtmm`（后门写入的明文）、`czmm`（重置后的默认密码摘要） |
| 后门暗号 | `ngfxd568bdh158` + `pc_num()` + `8bsdg45ybfdhg` |
| 默认密码 | `111111` |
| 服务名 | `zmserv`（`net stop zmserv`） |
| 进程名 | `jfglzsn`、`zmserv` |
| 外部程序 | `yz.exe`（路径 = `Proccesor.程序所在目录 + "\yz.exe"`） |
| 命令行 | `shutdown /r /f /t 0`、`net stop zmserv` |
| 提示文案 | `退出小助手密码：`、`确 定`、`重启电脑`、`密码应该是6位以上的字母或数字，请重新输入！`、`密码不正确，请重新输入！` |
| 数字常量 | 密码最小长度 `6`；`ClipCursor` 矩形 `(0,0)-(500,100)`；响铃阈值 `5` 次、`Beep(2000,500)`；`Sleep(500)` / `Sleep(3000)`；计算机名缺失时返回 `88` |
| P/Invoke | `user32!ClipCursor`、`kernel32!Beep` |
| 注册表键 | `HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD = 0` |
