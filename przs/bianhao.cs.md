# bianhao.Designer.cs.md — 右上角"机号"小窗

## 文件作用

`bianhao`（拼音"编号"）是桌面右上角的**机号显示窗**：无边框、透明底色、不显示在任务栏，
加载时把自己挪到 `(屏幕宽 - 350, 100)`，用大字显示**计算机名最后 6 个字符**。
鼠标悬停时第二行显示"由机房管理助手自动生成"，移开就清空。

由 Form1 在注册表 `bianhao=on` 时 `Queue.MyForms.bianhao.Show()` 显示，
作用是让老师一眼看到当前是哪台学生机。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `bianhao : Form` | 机号小窗 |
| `初始化组件`（原名 `GetVal`） | Designer 的 InitializeComponent |
| `窗体加载_定位并显示编号`（原名 `RunVal`） | 定位 + 取计算机名后 6 位 |
| `鼠标悬停_显示说明`（原名 `VisitVal`） | 显示"由机房管理助手自动生成" |
| `鼠标离开_清空说明`（原名 `SortVal`） | 清空说明文字 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `GetVal` | `初始化组件` | InitializeComponent |
| `RunVal` | `窗体加载_定位并显示编号` | Form.Load |
| `VisitVal` | `鼠标悬停_显示说明` | Label1.MouseHover |
| `SortVal` | `鼠标离开_清空说明` | Label1.MouseLeave |
| `attr` | `组件容器` | IContainer |
| `reg` | `机号标签` | Label1 后备字段 |
| `m_Params` | `Label2后备字段` | 未使用 |
| `Queue.ConcatVal` | `Queue.MyComputer` | 跨文件改名（`My.Computer.Name` = 计算机名） |
| `num`（`short`） | `屏宽` | `(short)Screen.PrimaryScreen.Bounds.Width` |

## 关键方法与流程

### 窗体加载_定位并显示编号(object, EventArgs)

1. `checked` 上下文里把屏幕宽截成 `short`（**原版就是 `(short)` 强转，超过 32767 的虚拟屏宽会溢出，保留不改**）；
2. `Left = 屏宽 - 350`；`Top = 100`；
3. `计算机名 = Queue.MyComputer.Name`；
4. `Label1.Text = Strings.Right(计算机名, 6)` —— 取最后 6 个字符当机号。

### 鼠标悬停 / 离开

- 悬停：`Label2.Text = "由机房管理助手自动生成"`；
- 离开：`Label2.Text = ""`。

### 初始化组件()

`Label1`（黑体 65.25pt，`Color.FromArgb(255,128,0)` 橙，初值 `stu30`）、`Label2`（宋体 12pt）；
窗体 `BackColor = Color.FromArgb(192,64,0)` 且同色设成 `TransparencyKey`（抠图透明）、
`FormBorderStyle = None`、`StartPosition = Manual`、`ShowInTaskbar = false`、`ClientSize = 321×97`。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| **暴露机器标识** | 直接显示计算机名后 6 位（机房机号命名通常是 `stu30` 之类），便于管理员定位机器 |
| **桌面常驻置顶** | 无边框透明窗，永远浮在右上角 |
| 无文件/注册表/进程操作 | 读取计算机名走 `My.Computer`，本身不落盘 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 控件初值 | `stu30`（Label1 占位文本） |
| 提示文案 | `由机房管理助手自动生成` |
| 定位常量 | `Left = 屏宽 - 350`、`Top = 100`；`ClientSize = 321×97` |
| 颜色 | 前景 `(255,128,0)`；背景/透明键 `(192,64,0)` |
| 显示规则 | 计算机名右侧 6 字符（`Strings.Right(name, 6)`） |
