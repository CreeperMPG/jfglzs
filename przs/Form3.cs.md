# Form3.Designer.cs.md — "提醒"窗口（警告牌）

## 文件作用

`Form3` 是一个**只会显示三行红字的全屏提示窗**（标题"提醒"、蓝底、无边框、最大化、`TopMost`）。
它自己没有任何逻辑，完全被 Form1 当"警告牌"复用：Form1 先改三个标签的文本再 `Show()`，
不同的文案覆盖"hosts 被删了""你改了禁止下载项""刚删除了哪个文件"等场景。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Form3` | 三行文案的提示窗 |
| `初始化组件`（原名 `EnableVal`） | Designer 的 InitializeComponent |
| `shuoming` / `shuoming2` / `shuoming3` | 三个 Label（文本由 Form1 写） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `EnableVal` | `初始化组件` | InitializeComponent |
| `list` | `组件容器` | IContainer |
| `factory` / `_Registry` / `m_Definition` | `shuoming2后备字段` / `shuoming后备字段` / `shuoming3后备字段` | 未使用的 Designer 后备字段 |

## 关键方法与流程

### 初始化组件()

1. 三个 Label：`shuoming2`、`shuoming`、`shuoming3`（宋体 15.75pt，前两个红色，`Text = " "`）；
2. 窗体：`BackColor = SystemColors.Highlight`（蓝）、`ClientSize = 1248×559`、
   `FormBorderStyle = None`、`StartPosition = CenterScreen`、`TopMost = true`、`Text = "提醒"`；
3. `PerformLayout()`。

没有事件、没有定时器、没有 `goto`。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| **强制置顶提示** | `TopMost` + 无边框 + 最大化，用来打断学生操作 |
| 文案恐吓 | 具体文案在 Form1，例如"本电脑禁止下载安装软件，再次操作会锁屏！"——"会锁屏"指的是 Form2/Form4 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 窗口标题 | `提醒` |
| 控件名 | `shuoming`、`shuoming2`、`shuoming3` |
| 初始文本 | `" "`（单个空格） |
| 布局 | `ClientSize 1248×559`；字体 `宋体 15.75pt`；前景 `Color.Red`；背景 `SystemColors.Highlight` |
