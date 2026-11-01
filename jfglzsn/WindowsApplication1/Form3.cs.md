## 文件作用

窗体 Form3 = **纯「警告」窗体**：蓝底（`SystemColors.Highlight`）红字、无按钮、无输入框、置顶、居中、标题「警告」、尺寸 1260×558。

主窗体 Form1 检测到违规时创建它、往三个 Label 上写提示文字、`Show()` 一下，随后又 `Close()`。它是「吓唬学生」的一次性展示面板，不做任何实际拦截（拦截都在 Form1 里做）。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `Form3 : Form` | 违规警告展示窗（无输入、无按钮、不可操作） |
| `初始化组件()`（原 `CloneCallback`） | VB 设计器 `InitializeComponent`：建 3 个 Label 并设属性 |
| `Dispose(bool)` | 释放组件容器 + 基类释放 |
| `shuoming` / `shuoming2` / `shuoming3` | 三个提示标签：违规原因 / 处置要求 / 作者联系方式 |
| `GetRule()` | 壳注入的判定桩，恒 `true`，无引用 |

三个标签的显示规格：

| 控件 | 字号 | 颜色 | 用途 |
| --- | --- | --- | --- |
| `shuoming` | 15.75pt 宋体 | 红 | 违规原因 |
| `shuoming2` | 15.75pt 宋体 | 红 | 处置要求 |
| `shuoming3` | 14.25pt 宋体 | 默认 | 作者联系方式（混淆串） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `CloneCallback()` | `初始化组件()` | VB 设计器的 `InitializeComponent`，壳把名字洗了；仅在构造函数调用 |
| `m_TokenizerEvent`（`IContainer`） | `组件容器` | 设计器组件容器（本窗体无非可视组件，实际为 `null`） |
| `m_ValEvent`（Label 后备字段） | 未改名 | `[AccessedThroughProperty("shuoming")]` 绑定，属性 setter/getter 走它 |
| `singletonEvent`（Label 后备字段） | 未改名 | 同上，对应 `shuoming2` |
| `m_StructEvent`（Label 后备字段） | 未改名 | 同上，对应 `shuoming3` |
| `GetRule()` | 未改名 | 恒真桩 |
| `shuoming` / `shuoming2` / `shuoming3` | 未改名 | 设计器控件名（拼音"说明"），Form1 里按名字跨窗体引用，改名会连锁 |

## 关键方法与流程

### `初始化组件()`（原 `CloneCallback`）

1. `new Label()` × 3。
2. `SuspendLayout()`。
3. 逐个设置：`AutoSize = true`、`Font = new Font("宋体", 15.75f/14.25f, Regular, Point, 134)`、`ForeColor = Color.Red`（`shuoming3` 不设颜色）、`Location`（72,202 / 72,266 / 76,337）、`Name`、`Size`、`TabIndex`（17/18/19）；`shuoming2.Text = " "`。
4. 窗体属性：`AutoScaleDimensions(6,12)`、`AutoScaleMode.Font`、`BackColor = SystemColors.Highlight`、`ClientSize(1260,558)`、`Name = "Form3"`、`StartPosition = CenterScreen`、`Text = "警告"`、`TopMost = true`。
5. `Controls.Add` × 3（**注意顺序**：shuoming3 → shuoming2 → shuoming）、`ResumeLayout(false)`、`PerformLayout()`。

`Font` 里最后一个参数 `134` 是 `GdiCharSet`，134 = 简体中文代码页（936 对应的 GB2312 字符集），全工程统一。

### `Dispose(bool)`

`try { if (disposing && 组件容器 != null) 组件容器.Dispose(); } finally { base.Dispose(disposing); }` —— try/finally 保证基类释放一定执行。

### 三个标签的写入方（都在 Form1）

| 写入点 | 文案 |
| --- | --- |
| 电子教室程序被暂停 | `shuoming = "电子教室程序被暂停！"` |
| 拔网线 / 禁用网络 | `shuoming = "你拔了网线或禁用了网络连接！"`，`shuoming2 = "请马上恢复网络，否则会全屏锁定！"` |
| 修改 IP | `shuoming = "你修改了IP地址！原地址为:" + 原始IP`，`shuoming2` 同上 |
| 启用防火墙 | `shuoming = "你启用了防火墙，危害电子教室的运行！"`，`shuoming2 = "现在助手强制关闭防火墙！"` |
| 修改口令 | `shuoming = "你修改了助手的密码！"`，`shuoming2 = "现在强制恢复密码！"` |
| 修改极域参数 | `shuoming = "你修改了极域电子教室的参数！"`，`shuoming2 = "请马上恢复，否则会锁屏！"` |
| 极域服务不存在 | `shuoming = "极域电子教室的2个重要服务不存在！"` |
| 极域服务停止 | `shuoming = "极域电子教室的2个重要服务停止！"` |
| 违规软件 | `shuoming = "你运行的软件违规！如有误判，请老师拍照与作者QQ联系。"`；若下标=6 改写成"用 Ctrl+Alt+Del 注销"的说明；若 17~21（解压软件）改写成"本电脑禁止使用解压软件" |
| 违规软件（联系方式） | `shuoming3 = "w4yw4y3tq3yszz" + 进程名(去.exe) + "mmfnfxhghrts"` |

## 恶意行为 / 敏感操作

本窗体自身只是展示，但它是整套「惩罚链」的**第一级**（相对 Form2/Form4 温和）：

| 行为 | 说明 |
| --- | --- |
| 全屏置顶警告 | `TopMost = true` + 1260×558，覆盖学生操作区 |
| 文案恐吓 | 「否则会全屏锁定！」「现在强制恢复密码！」——下一级就是锁屏（Form4）和惩罚（Form2） |
| 作者联系方式混淆串 | `"w4yw4y3tq3yszz" + 进程名 + "mmfnfxhghrts"`，实际是把触发进程名夹在两段固定串之间，用于"举报/申诉"时定位是哪个进程被判违规（对分析者而言是很好的行为指纹） |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 窗体标题 | `警告` |
| 字体 | `宋体`，15.75pt / 14.25pt，`GdiCharSet = 134` |
| 背景色 | `SystemColors.Highlight`（系统高亮色，通常是蓝色） |
| 窗口尺寸 | 1260 × 558，`CenterScreen`，`TopMost = true` |
| 控件名 | `shuoming` / `shuoming2` / `shuoming3` |
| 联系方式串模板 | `w4yw4y3tq3yszz` + `<进程名>` + `mmfnfxhghrts` |
| 全部文案 | 见上一节表格（9 条，均为简体中文） |
