## 文件作用

**全屏「违规警告 + 退出密码」惩罚窗体**（学生端惩罚界面第二级）。无边框、最大化、置顶、不进任务栏、`FormClosing` 直接取消（**关不掉**），背景是系统高亮色，红字写违规原因。

界面元素：密码提示 + 掩码输入框 + 「确 定」+「重启电脑」+ 三个说明标签 + 一个 1 秒定时器。定时器每跳一次就把窗口重新置顶，并**把鼠标光标锁死在屏幕左上角 800×100 的窄带里**。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `Form2 : Form` | 全屏惩罚界面（遮罩 + 密码门 + 鼠标锁定 + 重启按钮） |
| `初始化组件()`（原 `DeleteCallback`） | VB 设计器 `InitializeComponent` |
| `FindCallback`（确定按钮） | 退出密码校验 → 通过则 `PostCallback` |
| `StopCallback`（定时器 1000 ms） | 重新置顶 + `ClipCursor` 锁鼠标 + 5 次后蜂鸣 |
| `CountCallback`（重启电脑按钮） | `shutdown /r /f /t 0` **立即强制重启** |
| `PostCallback` | 密码正确后的恢复流程：杀进程、恢复 CMD、停服务、解除鼠标锁定、`EndApp` |
| `TestCallback`（`FormClosing`） | 无条件 `e.Cancel = true` |
| `InitCallback`（`Load`） | 打开锁定定时器 |
| `InsertCallback`（`Label_pd` 双击） | **空实现**（原版方法体只有 `On Error` 脚手架） |
| `ClipCursor` / `Beep` | user32 / kernel32 P/Invoke |
| `RECT`（嵌套 struct） | Win32 矩形 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `DeleteCallback()` | `初始化组件()` | 设计器 `InitializeComponent`，被壳洗名 |
| `m_CreatorEvent` | `组件容器` | `IContainer`，承载定时器 |
| `m_StubEvent` | 保留原名 | `Button2`（重启电脑）后备字段 |
| `m_StatusEvent` | 保留原名 | `Label2`（密码提示）后备字段 |
| `_ProducerEvent` | 保留原名 | `Label1`（"退出小助手密码："）后备字段 |
| `_SystemEvent` | 保留原名 | `TextBox1`（密码框）后备字段 |
| `m_QueueEvent` | 保留原名 | `Button1`（确定）后备字段 |
| `containerEvent` | 保留原名 | `shuoming` 后备字段 |
| `readerEvent` | 保留原名 | `Timer_frm2` 后备字段 |
| `m_DescriptorEvent` | 保留原名 | `shuoming2` 后备字段 |
| `listenerEvent` | 保留原名 | `Label_pd` 后备字段 |
| `_PublisherEvent` | 保留原名 | `shuoming3` 后备字段 |
| `m_MerchantEvent` | 保留原名（语义：锁定计数） | 定时器触发次数，≥5 开始蜂鸣 |
| `long_0` / `long_1`（`Beep` 参数） | `频率` / `时长` | Win32 参数名 |
| `text` / `text2` / `num` / `num2` / `num3` / `num4` / `obj` / `obj2` | `输入摘要` / `期望摘要` / `字符下界` / … | 局部占位名按用途重命名 |

> 说明：设计器控件的后备字段（`m_StatusEvent` 等）由 `[AccessedThroughProperty("Label2")]` 绑定到属性；属性 getter/setter 已经把它们包装好了，改字段名对阅读帮助有限、却容易看错对应关系，因此保留原名，对应表列在上方。

> 关于**方法名**：`FindCallback` / `StopCallback` / `CountCallback` / `PostCallback` / `TestCallback` / `InitCallback` / `InsertCallback` 全部**保留原名**（遵循 AGENTS.md「方法名、成员名等沿用反编译出来的名字」）。它们的真实语义已写在每个方法的中文 `<summary>` 里，也在下面「关键方法与流程」逐条说明，对照表如下：
>
> | 混淆名 | 真实语义 | 触发点 |
> | --- | --- | --- |
> | `FindCallback` | 「确定」按钮 → 退出密码校验 | `Button1.Click` |
> | `StopCallback` | 锁定定时器回调：置顶 + 锁鼠标 + 蜂鸣 | `Timer_frm2.Tick`（1000 ms） |
> | `CountCallback` | 「重启电脑」按钮 → `shutdown /r /f /t 0` | `Button2.Click` |
> | `PostCallback` | 密码正确后的恢复流程 + `EndApp` | `FindCallback` 内部 |
> | `TestCallback` | `FormClosing` → 无条件取消关闭 | `FormClosing` |
> | `InitCallback` | `Load` → 打开定时器 | `Load` |
> | `InsertCallback` | 空实现（双击无效果） | `Label_pd.DoubleClick` |

## 关键方法与流程

### `初始化组件()`（原 `DeleteCallback`）

建 `Container` + 8 个控件 + `Timer_frm2`，逐项设属性，最后：

| 窗体属性 | 值 |
| --- | --- |
| `FormBorderStyle` | `None`（无边框） |
| `WindowState` | `Maximized`（最大化） |
| `TopMost` | `true` |
| `ShowInTaskbar` | `false` |
| `BackColor` | `SystemColors.Highlight` |
| `ClientSize` | `1518 × 1092` |
| `Text` | `Form2` |

`Timer_frm2.Interval = 1000`。`shuoming`（18pt 红字）默认文案：「你运行的软件危害电子教室运行！如有误判，请老师拍照与作者QQ联系。」

### `FindCallback(sender, e)` — 确定按钮（密码校验）

1. `Strings.Len(TextBox1.Text) < 6` → `Label2.Text = "密码应该是6位以上的字母或数字，请重新输入！"`，清空输入框，**返回**。
2. `输入摘要 = ThreadEvent.计算SHA256摘要(Strings.Trim(TextBox1.Text))`（即 `SHA256(输入 + "bfdshgs")`）。
3. `期望摘要 = Conversions.ToString(读注册表值(HKCU, "Software", "n", ""))`。
4. `Operators.CompareString(输入摘要, 期望摘要, false) != 0` → `Label2.Text = "密码不正确，请重新输入！"`，清空输入框。
5. 相等 → `Button1.Enabled = false` 并调 `PostCallback()`。

原版整段在 `On Error Resume Next` 下；这几句互为因果（取值 → 比较 → 分支），合并成一个 `catch { }`，注释已说明。

### `StopCallback(sender, e)` — 定时器（锁定）

1. `TopMost = true`（防止被其他窗口盖住）。
2. `RECT{Left=0, Top=0, Right=800, Bottom=100}` 并 `Form2.ClipCursor(rect)` —— **把光标关进屏幕左上角 800×100**。
3. `m_MerchantEvent++`（`checked` 上下文）。
4. `m_MerchantEvent >= 5` → `Beep(2000, 500)`（2000 Hz、500 ms）。

原版逐句 `On Error Resume Next`，这里**按语句拆 try/catch**：任一步失败（如置顶被拒）都不能阻断后面的锁鼠标动作。

### `PostCallback()` — 退出流程（老师后门）

按顺序：

1. `服务名 = "zmserv"`；`当前进程名 = ThreadEvent.m_AdapterEvent`。
2. `do { 进程列表 = GetProcessesByName(当前进程名); 进程列表[0].Kill(); } while (进程列表.Length != 0);`
   —— **靠"下标越界"异常退出循环**（进程被自己杀完后列表为空 → `[0]` 抛 → 原版 `On Error` 吞掉 → 循环条件判假退出）。这里两条语句各自 try/catch，保留这一机制。
3. `字符下界 = 10; 字符下界 = 字符下界*10 + 2`（= 102，即 `'f'`）。枚举全部进程：名字长度 ≥ 4 且**每个字符都在 ASCII 102~111（'f'~'o'）**的进程一律 `Kill()`。
   —— 作者的意图应是识别某类程序，但判定条件写成了字符区间比较，实际效果是"杀掉名字全由 f~o 字母组成的进程"。原版循环体内还有一条恒假分支（`j == 长度+1`），已注释标注为死代码。
4. `写注册表值(HKCU\Software\Policies\Microsoft\Windows\System, "DisableCMD", 0, DWord)` —— 恢复命令提示符。
5. `Interaction.Shell("net stop zmserv", Hide, true, -1)` —— 停自己的服务（隐藏窗口 + 等待）。
6. `Thread.Sleep(3000)`。
7. `GetProcessesByName("zmserv")[0].Kill()`。
8. `ClipCursor(整屏)` —— 解除鼠标锁定。
9. `ProjectData.EndApp()` —— **直接结束进程**（不返回调用方）。

### `CountCallback(sender, e)` — 重启电脑按钮

```
Interaction.Shell("shutdown /r /f /t 0", AppWinStyle.MinimizedFocus, false, -1)
```

`/r` 重启、`/f` 强制关闭应用、`/t 0` 零延迟；第三个参数 `false` = **不等待**，命令发出即返回。

### `TestCallback(sender, e)` — `FormClosing`

`e.Cancel = true;` 无条件取消关闭。学生无法用 Alt+F4 / 关闭按钮退出。

### `InitCallback(sender, e)` — `Load`

`Timer_frm2.Enabled = true;`（之后每秒执行 `StopCallback`）。

### `InsertCallback` — 空

`Label_pd` 的双击事件处理器，原版方法体**除 `On Error` 脚手架外没有任何语句**（第一条指令就跳出块），双击无任何效果。保留空实现以维持事件绑定。

## 恶意行为 / 敏感操作

| 行为 | 说明 | 风险 |
| --- | --- | --- |
| **鼠标锁定** | 每秒 `ClipCursor` 把光标关进 800×100 | 高（剥夺操作能力） |
| **窗口无法关闭** | `FormClosing` 无条件 `e.Cancel = true` | 高 |
| **强制重启** | 「重启电脑」按钮 `shutdown /r /f /t 0`，不提示、不给保存机会 | **高（破坏性）** |
| **杀进程** | 按"字符全在 f~o"的怪条件批量 `Kill()` | 高 |
| **停服务 / 结束自身** | `net stop zmserv` + `ProjectData.EndApp()` | 中 |
| 恢复 CMD | 写 `DisableCMD = 0`（**这是"退出小助手"的副作用**，不是善意） | 中 |
| 蜂鸣骚扰 | `Beep(2000, 500)`，累计 5 次后每跳一次 | 低 |
| 密码门 | `SHA256(输入 + "bfdshgs")` 与 `HKCU\Software\n` 比对 | 中（管控手段） |

> 惩罚分级：Form2 是"第二级"（有密码门、有重启按钮、锁鼠标）；第一级是 Form3（只弹文字警告）；第三级是 Form4（锁屏）。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 密码注册表键 | `HKCU\Software` 值名 `n`（存 `SHA256(口令 + "bfdshgs")` 十六进制） |
| 脚本命令 | `net stop zmserv` |
| 脚本命令 | `shutdown /r /f /t 0` |
| 服务名 | `zmserv` |
| 鼠标锁定矩形 | `{Left=0, Top=0, Right=800, Bottom=100}` |
| 蜂鸣参数 | `2000` Hz / `500` ms |
| 密码最小长度 | `6` |
| 定时器间隔 | `1000` ms |
| 进程名判据 | 名字长度 ≥ 4 且所有字符 ∈ `[102, 111]`（ASCII 'f'~'o'） |
| 文案 | `密码应该是6位以上的字母或数字，请重新输入！`、`密码不正确，请重新输入！`、`电子教室进程未找到或本地连接IP地址错误！请重新启动电脑。`、`你拔了网线或禁用了网络连接！`、`请重新启动电脑！`、`你修改了IP地址！原地址为:`、`你启用了防火墙，危害电子教室的运行！`、`你修改了助手的密码！`、`你修改了极域电子教室的参数！`、`极域电子教室的2个重要服务停止！`、`你运行的软件危害电子教室运行！如有误判，请老师拍照与作者QQ联系。`、`你运行了杀毒类的软件，危害电子教室的运行！如果你必须要用火绒杀毒软件，要设置：'允许火绒杀毒'。如有误判，请老师拍照与作者QQ联系。`、`退出小助手密码：`、`确 定`、`重启电脑` |
| 联系方式串模板 | `gyhrw4yw4y3tq3yszz` + `<进程名>` + `mmfnfxhghrts`（写进 `shuoming3`） |
| 字体 | `宋体` 10.5pt / 18pt / 26.25pt / 14.25pt，`GdiCharSet = 134` |
| P/Invoke | `user32!ClipCursor`、`kernel32!Beep`（均 `CharSet.Ansi, ExactSpelling, SetLastError`） |
