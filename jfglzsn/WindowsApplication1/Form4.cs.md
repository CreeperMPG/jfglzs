## 文件作用

**全屏「锁屏」窗体**——惩罚链的第三级（最狠的一级）。与 Form2 同构（无边框、最大化、置顶、不进任务栏、同一套控件与 1 秒定时器），但语义是"锁住整台机器"：定时器每跳一次重新置顶并把鼠标关进左上角 800×100 的小矩形。

与 Form2 的三处关键差异：

1. 密码校验接受**两套**口令（主口令 + 备用口令）；
2. 退出流程会**保留** `DisableCMD` 的原有状态；
3. `FormClosing` 只在 `ThreadEvent.mapperEvent` 为真时才拒绝关闭。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `Form4 : Form` | 全屏锁屏窗体 |
| `初始化组件()`（原 `ManageCallback`） | VB 设计器 `InitializeComponent` |
| `InvokeCallback`（确定按钮） | **双密码**校验 → 通过则 `SortCallback` |
| `DefineCallback`（定时器 1000 ms） | 重新置顶 + 锁鼠标 + 5 次后蜂鸣 |
| `CompareEvent`（重启电脑按钮） | `shutdown /r /f /t 0` |
| `SortCallback` | 退出流程：杀进程、按需临时放开 `DisableCMD` 再禁回去、停服务、解除锁定、`EndApp` |
| `RevertCallback`（`FormClosing`） | `mapperEvent` 为真才锁死窗口，否则允许关闭并复位鼠标限制 |
| `GetEvent`（`Load`） | 打开锁定定时器 |
| `PushCallback`（`Label_pd` 双击） | **空实现** |
| `ClipCursor` / `Beep` | user32 / kernel32 P/Invoke |
| `RECT`（嵌套 struct） | Win32 矩形 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ManageCallback()` | `初始化组件()` | 设计器 `InitializeComponent` |
| `_SetterSetter` | `组件容器` | `IContainer` |
| `_FactorySetter` | 保留原名 | `Button2`（重启电脑）后备字段 |
| `helperSetter` | 保留原名 | `Label2`（密码提示）后备字段 |
| `m_MockSetter` | 保留原名 | `Label1`（"退出小助手密码："）后备字段 |
| `_ComparatorSetter` | 保留原名 | `TextBox1`（密码框）后备字段 |
| `m_ErrorSetter` | 保留原名 | `Button1`（确定）后备字段 |
| `m_ProductSetter` | 保留原名 | `shuoming` 后备字段 |
| `propertySetter` | 保留原名 | `Timer_frm4` 后备字段 |
| `_ExceptionSetter` | 保留原名 | `shuoming2` 后备字段 |
| `m_IdentifierSetter` | 保留原名 | `Label_pd` 后备字段 |
| `_AnnotationSetter` | 保留原名 | `shuoming3` 后备字段 |
| `_ProxySetter` | 保留原名（语义：锁定计数） | 定时器触发次数，≥5 开始蜂鸣 |
| `long_0` / `long_1`（`Beep`） | `频率` / `时长` | Win32 参数名 |
| `text` / `text2` / `text3` / `num3` / `num4` / `num7` 等 | `输入摘要` / `主口令摘要` / `备用口令摘要` / `字符下界` / `命令提示符状态` / … | 局部占位名按用途重命名 |

> 关于**方法名**：`InvokeCallback` / `DefineCallback` / `SortCallback` / `RevertCallback` / `GetEvent` / `CompareEvent` / `PushCallback` 全部**保留原名**（遵循 AGENTS.md「方法名、成员名等沿用反编译出来的名字」）。真实语义见各方法中文 `<summary>` 与下表：
>
> | 混淆名 | 真实语义 | 触发点 |
> | --- | --- | --- |
> | `InvokeCallback` | 「确定」按钮 → **双密码**校验 | `Button1.Click` |
> | `DefineCallback` | 锁定定时器回调：置顶 + 锁鼠标 + 蜂鸣 | `Timer_frm4.Tick`（1000 ms） |
> | `CompareEvent` | 「重启电脑」按钮 → `shutdown /r /f /t 0` | `Button2.Click` |
> | `SortCallback` | 密码正确后的恢复流程（保留 `DisableCMD` 状态）+ `EndApp` | `InvokeCallback` 内部 |
> | `RevertCallback` | `FormClosing` → 按 `mapperEvent` 决定放行/锁死 | `FormClosing` |
> | `GetEvent` | `Load` → 打开定时器 | `Load` |
> | `PushCallback` | 空实现（双击无效果） | `Label_pd.DoubleClick` |

## 关键方法与流程

### `初始化组件()`（原 `ManageCallback`）

控件集合与 Form2 **完全相同**，差异只在字号与坐标：

| 项 | Form2 | Form4 |
| --- | --- | --- |
| `shuoming` 字号 | 18pt | **26.25pt** |
| `shuoming3` 字号 | 14.25pt | **9pt** |
| `Label_pd` 位置 | (1, 2) | (1, 3) |
| `shuoming3` 位置 | (669, 125) | (922, 103) |
| `ClientSize` | 1518×1092 | **1254×729** |
| 定时器名 | `Timer_frm2` | `Timer_frm4` |

窗体属性仍是无边框 + 最大化 + 置顶 + 不进任务栏。**`this.Text = "Form2"`** —— 作者从 Form2 复制窗体后忘了改，可作同源指纹。

### `InvokeCallback(sender, e)` — 确定按钮（双密码）

1. `Strings.Len(TextBox1.Text) < 6` → 提示「密码应该是6位以上的字母或数字，请重新输入！」，清空，返回。
2. `输入摘要 = 计算SHA256摘要(Trim(TextBox1.Text))`。
3. `主口令摘要 = ToString(读注册表值(HKCU, "Software", "n", ""))`。
4. `备用口令摘要 = ToString(读注册表值(HKCU, "Software\jfglzs", "n2", ""))`。
5. `if (!((CompareString(输入, 主) == 0) | (CompareString(输入, 备用) == 0)))` → 提示「密码不正确，请重新输入！」并清空。
6. 否则 `Button1.Enabled = false; SortCallback();`

⚠️ 判断里用的是**按位或 `|`**（不是 `||`）：两只比较**都会求值**，这是 VB `Or` 的原样产物。

### `DefineCallback(sender, e)` — 定时器（锁定）

与 Form2 的 `StopCallback` 完全一致：`TopMost = true` → `ClipCursor{0,0,800,100}` → `_ProxySetter++` → `>= 5` 则 `Beep(2000, 500)`。按语句拆 try/catch。

### `SortCallback()` — 退出流程

1. 循环杀同名进程（`do { GetProcessesByName; [0].Kill(); } while (Length != 0)`，靠越界异常退出）。
2. 杀"名字全由 ASCII 102~111 组成"的进程（与 Form2 同一套逻辑、同一处死代码）。
3. **读 `DisableCMD`**：
   - 等于 `1`（原本禁用）→ 置 `0` → `net stop zmserv` → `Sleep(3000)` → **再置回 `1`**；
   - 不等于 `1` → 只做 `net stop zmserv` + `Sleep(3000)`。
4. `GetProcessesByName("zmserv")[0].Kill()`。
5. `ClipCursor(整屏)` 解除鼠标锁定。
6. `ProjectData.EndApp()`。

**与 Form2 的差别就在这里**：Form4 退出后会**恢复** CMD 禁用状态（维持管控），而 Form2 退出则把 CMD 永久放开。

### `RevertCallback(sender, e)` — `FormClosing`

```
if (!ThreadEvent.mapperEvent) { e.Cancel = false; ClipCursor(整屏); return; }
e.Cancel = true;
```

- `mapperEvent` 为假（默认）：**允许关闭**，并顺手把鼠标限制区恢复成整屏（避免窗口没了鼠标还被关在小矩形里）。
- 为真：取消关闭，锁死。

⚠️ 本工程内**没有任何代码把 `mapperEvent` 置真**，所以这个锁屏窗实际总是能关掉。这要么是破解/去壳时被抹掉的一环，要么原版靠反射/其他模块赋值。

### `GetEvent` / `CompareEvent` / `PushCallback`

| 方法 | 行为 |
| --- | --- |
| `GetEvent`（`Load`） | `Timer_frm4.Enabled = true` |
| `CompareEvent`（重启按钮） | `Interaction.Shell("shutdown /r /f /t 0", MinimizedFocus, false, -1)` |
| `PushCallback`（双击） | 空实现（原版方法体只有 `On Error` 脚手架） |

## 恶意行为 / 敏感操作

| 行为 | 说明 | 风险 |
| --- | --- | --- |
| **全屏锁屏 + 鼠标锁定** | 每秒置顶 + `ClipCursor{0,0,800,100}` | **高**（完全剥夺操作能力） |
| **强制重启** | 「重启电脑」按钮 `shutdown /r /f /t 0` | **高（破坏性）** |
| **杀进程** | 同 Form2 的字符区间判据 | 高 |
| **保持 CMD 禁用** | 退出前后把 `DisableCMD` 恢复成 1 | 高（重启后学生依旧没 CMD） |
| 停服务 + 自杀 | `net stop zmserv` + `ProjectData.EndApp()` | 中 |
| 蜂鸣骚扰 | `Beep(2000, 500)` × 每跳一次 | 低 |
| 双密码门 | 主口令 `HKCU\Software\n` + 备用口令 `HKCU\Software\jfglzs\n2` | 中 |

> 备用口令 `n2` 的存在说明作者为"老师忘密码"留了后门：`n2` 只在本窗体被校验（Form2 只认 `n`），因此 `n2` 是**锁屏专用的应急口令**。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 密码注册表键 1 | `HKCU\Software` 值名 `n` |
| 密码注册表键 2 | `HKCU\Software\jfglzs` 值名 `n2` |
| CMD 策略键 | `HKCU\Software\Policies\Microsoft\Windows\System` 值名 `DisableCMD`（DWord） |
| 脚本命令 | `net stop zmserv`、`shutdown /r /f /t 0` |
| 服务名 | `zmserv` |
| 鼠标锁定矩形 | `{Left=0, Top=0, Right=800, Bottom=100}` |
| 蜂鸣参数 | `2000` Hz / `500` ms |
| 密码最小长度 | `6` |
| 定时器间隔 | `1000` ms |
| 进程名判据 | 长度 ≥ 4 且所有字符 ∈ `[102, 111]` |
| 窗体标题 | `Form2`（复制残留，实际类是 Form4） |
| 文案 | `退出小助手密码：`、`确 定`、`重启电脑`、`密码应该是6位以上的字母或数字，请重新输入！`、`密码不正确，请重新输入！`、`电子教室程序被暂停！`、`你拔了网线或禁用了网络连接！`、`请马上恢复网络，否则会全屏锁定！`、`你修改了IP地址！原地址为:`、`你启用了防火墙，危害电子教室的运行！`、`现在助手强制关闭防火墙！`、`你修改了助手的密码！`、`现在强制恢复密码！`、`你修改了极域电子教室的参数！`、`请马上恢复，否则会锁屏！`、`极域电子教室的2个重要服务不存在！`、`极域电子教室的2个重要服务停止！`、`你运行的软件违规！如有误判，请老师拍照与作者QQ联系。`、`本电脑禁止使用解压软件，如果确有需要，请老师来退出小助手。` |
| 字体 | `宋体` 10.5pt / 26.25pt / 9pt，`GdiCharSet = 134` |
| P/Invoke | `user32!ClipCursor`、`kernel32!Beep` |
