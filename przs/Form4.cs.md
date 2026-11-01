# Form4.Designer.cs.md — 全屏锁屏窗

## 文件作用

`Form4` 是第二个**锁屏窗**（与 Form2 分工不同）：Form2 是"警告 + 要密码"，Form4 是纯"锁死屏幕"。
它最大化、置顶、无边框、不显示在任务栏，每秒把鼠标用 `ClipCursor` 锁在左上角 (0,0)-(800,100)，
5 秒后开始 `Beep(2000, 500)` 长鸣。

能不能关掉由 `Proccesor.锁屏中禁止关闭`（原名 `_Class`）决定：
- `true`（锁屏进行中）→ `FormClosing` 里 `e.Cancel = true`，用户关不掉；
- `false` → 允许关闭，并把 `ClipCursor` 恢复成整屏，鼠标重新能动。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Form4` | 全屏锁屏窗体 |
| `初始化组件`（原名 `SetVal`） | Designer 的 InitializeComponent |
| `定时器_锁鼠标并报警`（原名 `CreateVal`） | Timer_frm4.Tick：置顶 + `ClipCursor` + 蜂鸣 |
| `窗体显示_启用定时器`（原名 `AddVal`） | Form.Load |
| `窗体关闭_判断是否允许`（原名 `MapVal`） | FormClosing：按开关决定是否放行 |
| `RECT` | Win32 `ClipCursor` 用的矩形结构 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `SetVal` | `初始化组件` | InitializeComponent |
| `CreateVal` | `定时器_锁鼠标并报警` | Timer_frm4.Tick |
| `AddVal` | `窗体显示_启用定时器` | Form.Load |
| `MapVal` | `窗体关闭_判断是否允许` | FormClosing |
| `_Role` | `组件容器` | IContainer |
| `writer` | `锁屏定时器` | Timer_frm4 后备字段 |
| `m_Event` / `m_Dic` | `shuoming后备字段` / `Label2后备字段` | 未使用的后备字段 |
| `m_Status` | `锁鼠标次数` | Tick 计数 |
| `Proccesor._Class` | `Proccesor.锁屏中禁止关闭` | 跨文件改名 |
| `long_0` / `long_1` | `频率` / `时长` | `Beep` 形参 |

## 关键方法与流程

### 定时器_锁鼠标并报警()

1. `TopMost = true`（防止被别的窗口盖住）；
2. `矩形 = (0,0,800,100)`；`Form4.ClipCursor(矩形)`；
3. `checked { 锁鼠标次数++; }`；
4. `锁鼠标次数 >= 5` → `Beep(2000L, 500L)`。

### 窗体关闭_判断是否允许(FormClosingEventArgs e)

1. `Proccesor.锁屏中禁止关闭 == false` → `e.Cancel = false`；
   `矩形 = 整屏`；`ClipCursor(矩形)`（解除锁定）；`return`；
2. 否则 `e.Cancel = true`。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| **锁屏** | `TopMost` + 最大化无边框 + 每秒 `ClipCursor` 限制鼠标活动区 |
| **蜂鸣骚扰** | `Beep(2000, 500)` 从第 5 秒起持续响 |
| **禁止关闭** | 依赖 `Proccesor.锁屏中禁止关闭`；为 true 时窗口无法关闭 |
| 解除方式 | 只有把 `Proccesor.锁屏中禁止关闭` 置 false（由 Form1 控制）才允许关闭 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 光标锁定矩形 | `(0,0)-(800,100)`；解除时用整屏 `Screen.PrimaryScreen.Bounds` |
| 蜂鸣 | `Beep(2000, 500)`，阈值 5 次 Tick（Tick=1 秒） |
| 窗口 | 标题 `Form4`，`ClientSize 1023×686`，`ShowInTaskbar=false`，`TopMost=true`，`WindowState=Maximized` |
| 控件 | `shuoming`（宋体 26.25pt 红字）、`Label2`（深红） |
| P/Invoke | `user32!ClipCursor`、`kernel32!Beep` |
