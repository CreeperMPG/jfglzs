# Form2.cs — 全屏锁定密码窗（一级锁屏，解锁即卸载清理）

## 文件作用
Form2 是**全屏无边框、置顶（TopMost）的锁定窗体**：蓝色背景、大字红色警告说明、密码输入框与"确定"/"重启电脑"按钮。由主控在检测到"违规"行为时通过 `Timer2` 持续强制弹出（`DisableTemplate`）。输入正确密码（与 `HKCU\Software\n` 的 SHA256 哈希比对）后执行 `PrepareTemplate` 卸载流程：击杀 zmserv 与随机名同伴进程、枚举并击杀"同类命名"进程、停服务、释放鼠标、结束程序。`FormClosing` 永远 `e.Cancel=true` 禁止关闭；`Timer_frm2` 每秒把鼠标 `ClipCursor` 限制在屏幕顶部 100px 区域并周期性 `Beep` 报警。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `Form2` | 全屏锁屏 + 密码校验 + 卸载清理 |
| `Form2.RECT` | ClipCursor 用的矩形结构体 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `QueryTemplate` | 初始化控件与布局 | Designer 生成的控件初始化（被搬进逻辑文件） |
| `CalcTemplate`（Button1.Click） | 确定按钮_校验密码 | 密码比对 |
| `MapTemplate`（Timer_frm2.Tick） | 锁鼠标+蜂鸣 | 每秒 ClipCursor 到 (0,0)-(800,100)，第 5 次起 Beep(2000,500) |
| `SelectTemplate`（Label_pd.DoubleClick） | 双击标签空操作 | 空壳（作者预留后门/彩蛋位） |
| `PrepareTemplate` | 卸载清理并退出 | 杀进程、停服务、释放鼠标、EndApp |
| `CustomizeTemplate`（FormClosing） | 禁止关闭 | e.Cancel = true |
| `ForgotTemplate`（Load） | 启用锁鼠标定时器 | Timer_frm2.Enabled = true |
| `OrderTemplate`（Button2.Click） | 重启电脑 | `shutdown /r /f /t 0` |

## 关键方法与流程

### 确定按钮_校验密码（原名 `CalcTemplate`）
- **职责**：校验输入的退出密码。
- **流程**：1. 密码长度 <6 → Label2 提示"密码应该是6位以上的字母或数字"并清空输入框；2. `ModelVisitor.FillConfig(Trim(输入))`（= SHA256(输入+"bfdshgs")）与 `HKCU\Software\n` 比较；3. 相等→禁用按钮并调用 `PrepareTemplate()`（卸载退出）；不等→提示"密码不正确"。
- **关键调用/API**：`ModelVisitor.FillConfig`、`ModelVisitor.DeleteConfig`（读注册表）。

### 卸载清理并退出（原名 `PrepareTemplate`）
- **职责**：输入正确密码后的完整卸载流程。
- **流程**：1. `do { GetProcessesByName(随机名进程).Kill() } while (长度!=0)` 反复击杀随机名同伴进程；2. `num3=102`（10*10+2），枚举全部进程：进程名长度≥4 且**每个字符的 Asc 码都在 102~111（'f'~'o'）之间**→`Kill()`（清扫同类随机命名进程）；3. `DisableCMD=0`；4. `net stop zmserv`；5. Sleep 3000；6. `GetProcessesByName("zmserv")[0].Kill()`；7. `ClipCursor` 全屏（释放鼠标限制前先锁住）；8. `ProjectData.EndApp()` 结束本进程。
- **关键调用/API**：`Process.Kill`、`Interaction.Shell("net stop zmserv")`、`ClipCursor`、`EndApp`。
- **说明**：真正的"退出"= 卸载自身服务与同伴进程；本程序退出后 `przs.exe`/`zmserv` 仍可能由同伴保持。

### 锁鼠标+蜂鸣（原名 `MapTemplate`，Timer_frm2 每秒）
- **流程**：`TopMost=true`；`ClipCursor` 到 (0,0)-(800,100)（鼠标被限制在屏幕顶部窄条，无法点按按钮下方/其它窗口）；计数 ≥5 后 `Beep(2000, 500)` 持续报警。
- **关键调用/API**：`user32.ClipCursor`、`kernel32.Beep`。

### 禁止关闭（原名 `CustomizeTemplate`）
- **流程**：`e.Cancel = true`——用户无法用任何方式关闭锁屏窗（Alt+F4 等无效）。
- **关键调用/API**：`FormClosingEventArgs.Cancel`。

### 重启电脑（原名 `OrderTemplate`）
- **流程**：`Interaction.Shell("shutdown /r /f /t 0", MinimizedFocus)` 立即强制重启。
- **关键调用/API**：`shutdown /r /f /t 0`。

## 恶意行为 / 敏感操作
- **全屏锁屏勒索式阻断**：无边框置顶全屏 + 禁止关闭 + ClipCursor 锁鼠标 + Beep 报警。
- **未授权卸载清理**：击杀随机名进程与"f~o 字符集"命名进程（可能误杀同规律用户程序）、停止 zmserv 服务。
- **强制重启**：`shutdown /r /f /t 0`。

## IOC / 硬编码
- **注册表**：`HKCU\Software\n`（密码哈希）；`HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD`。
- **进程/服务**：`zmserv`（服务与进程）；随机 5 位小写名进程（`ModelVisitor._TestsVisitor`）。
- **命令**：`net stop zmserv`、`shutdown /r /f /t 0`。
- **UI 文案**："退出小助手密码："、"你运行的软件危害电子教室运行！如有误判，请老师拍照与作者QQ联系。"、"重启电脑"。
