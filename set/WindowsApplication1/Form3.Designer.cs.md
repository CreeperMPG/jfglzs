# WindowsApplication1\Form3.Designer.cs —— 文件级分析

## 文件作用
「保存完成」提醒窗体 `Form3`（标题「提醒」，`TopMost = true`，屏幕居中）。由 `Form1.PrepareInfo` 在全部设置写完并重启 explorer 之后调用 `Show()` 弹出。它同时兼一个权限自检：查 HKLM 的 Run 项里有没有 `prozs`，没有就把提示改成红字，告诉用户要用管理员身份重跑。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `Form3 : Form` | 提醒窗体（设计器生成的窗体类整体写在本文件里） |
| `Form3()` | 构造函数：挂 Load 事件、建界面 |
| `Dispose(bool)` | 释放设计器容器 `_ErrorAttr` |
| `QueryInfo()` | 设计器布局方法：创建 Button2 / Label2 / Label1 并设置外观 |
| `StopInfo` | **Form_Load 自检**：检查 HKLM Run 里 `prozs` 是否存在，判断自启动是否写成功 |
| `InitInfo` | 「确 定」按钮：`ProjectData.EndApp()` 直接结束进程 |
| `Button2` / `Label2` / `Label1` 属性 | VB `WithEvents` 包装属性（控件字段被 Reactor 改名为 `creatorAttr` / `m_ImporterAttr` / `m_EventAttr`） |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| 字段 `creatorAttr` | 未改名 | `Button2` 的后备字段（`[AccessedThroughProperty("Button2")]`），代码里只用属性 |
| 字段 `m_ImporterAttr` | 未改名 | `Label2` 的后备字段 |
| 字段 `m_EventAttr` | 未改名 | `Label1` 的后备字段 |
| 字段 `_ErrorAttr` | 未改名 | 就是 `components` 容器 |
| 局部 `obj` | `自启动值` | Run 项里 `prozs` 的值 |
| 局部 `num` / `num2` / `num3` / `num4` | 删除 | VB 错误状态号与分发计数器 |

## 关键方法与流程（按方法分段，写清步骤）
### `StopInfo`（Form_Load）
1. `ObserverAttr.SortInfo(HKLM, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", "")` 取值。
2. 若与 `""` 弱类型相等 → 再查 `SOFTWARE\\WOW6432NODE\\...\\RUN` 的同名值（64 位系统上 32 位程序的自启动位置）。
3. 两个都空 → `Label2.ForeColor = Color.Red`，并把文案改成「因权限不够设置不成功……右击文件set.exe，选择 以管理员身份运行，再次设置。」
4. 全程 `On Error Resume Next`：读注册表抛异常就当没查到。

### `InitInfo`
`ProjectData.EndApp()` —— 相当于 VB 的 `End`，直接掐进程（不触发正常的窗体关闭流程）。

### `QueryInfo`
纯设计器代码，未改语义：`Button2`（确 定，348,161，78×27）、`Label2`（20,20，406×59，提示"请在硬盘保护系统中创建进度或者保存系统（此步可省略）。"）、`Label1`（20,92，406×55，"注：软件在重启电脑后自动运行。"）；窗体外框 457×215、居中、置顶、标题「提醒」。

## 恶意行为 / 敏感操作
- 本文件不直接做坏事，但它是**安装成功后的确认环节**：提示语「软件在重启电脑后自动运行」印证了 `Form1.DefineInfo` 写的 HKLM Run 自启动项（`prozs`）是刻意行为。
- 敏感动作：`ProjectData.EndApp()` 强杀进程；读取 `HKLM\...\RUN`（读取范围仅此）。
- 提示语本身是 IOC：「右击文件set.exe，选择 以管理员身份运行」说明整套限制需要管理员权限才能落地。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 注册表键：`SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN`（值名 `prozs`）、`SOFTWARE\WOW6432NODE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN`
- 字符串：`确  定`、`提醒`、`请在硬盘保护系统中创建进度或者保存系统（此步可省略）。`、`注：软件在重启电脑后自动运行。`、`因权限不够设置不成功，请打开机房管理助手的安装文件夹（C盘j开头的文件夹），右击文件set.exe，选择 以管理员身份运行，再次设置。`
- 界面坐标/尺寸：`(348,161) 78×27`、`(20,20) 406×59`、`(20,92) 406×55`、`ClientSize 457×215`

## 反混淆处理记录
- goto：**6 → 0**；`IL_xxxx:` 14 → 0；`@switch` 2 → 0；`endfilter` 1 → 0；`ClearProjectError()` 全清
- `On Error Resume Next` 还原：整个 `StopInfo` 用一层 `try { } catch { }` 包住（原版逐条忽略错误，本方法只有两句可能抛异常的语句、且都只读注册表，合成一层不改变可观察行为，已在注释说明）
- 语义保留点：`Operators.ConditionalCompareObjectEqual(obj, "", false)`（VB 弱类型比较）原样保留，没有改成 `== ""`
- 未改动：`QueryInfo()` 布局正文、属性包装、字段声明
