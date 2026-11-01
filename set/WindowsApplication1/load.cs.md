# WindowsApplication1\load.Designer.cs —— 文件级分析

## 文件作用
启动窗体 `load`（程序真正的主窗体，由 `InfoAttr.OnCreateMainForm` 指定）：**密码闸门** + **随机账号进程名生成器**。密码正确（或从未设过密码）时才隐藏自己并打开 `Form1`。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `load : Form` | 密码输入窗体（534×262，居中，标题 `load`） |
| `load()` | 构造：挂 `Shown`/`Load`，建界面 |
| `ReadInfo()` | 设计器布局：Label_pd（大号空白标签，可双击）、Label1（红字提示）、Label3（"输入小助手密码："）、TextBox1（`PasswordChar='*'`）、Button1（"确 定"） |
| `PopInfo` | 「确 定」：校验密码摘要，通过则显示 `Form1` |
| `DestroyInfo` | Form_Shown：若 `HKCU\Software\n` 为空（还没设过密码）直接放行进 `Form1` |
| `LogoutInfo` | Form_Load：按日期生成随机「账号进程名」写入 `ObserverAttr.账号进程名/访客名` |
| `ReflectInfo` | Label_pd 双击事件 —— **空实现** |
| `Dispose(bool)` | 释放设计器容器 `_PrototypeAttr` |
| 5 个属性包装 | VB `WithEvents` 模板：`Label_pd→m_PageAttr`、`Label1→valAttr`、`Label3→_ProcessAttr`、`TextBox1→requestAttr`、`Button1→watcherAttr` |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| 局部 `text`（PopInfo） | `输入摘要` | `SHA256(输入+"bfdshgs")` |
| 局部 `text2`（PopInfo） | `已存摘要` | `HKCU\Software\n` |
| 局部 `num`（LogoutInfo） | `日期乘积` | 月 × 日 |
| 局部 `num2/num3/num4/num5` | `余7/余9/余5/余3` | 日期乘积对各数的余数 |
| 局部 `text`（LogoutInfo） | `四字母名` | 第一版名字（会被覆盖） |
| 局部 `num6` | `种子` | `Rnd()*100000*3+1` 取整后的种子 |
| 局部 `text2`（LogoutInfo） | `五位名字` | 第二版名字（最终生效） |
| 局部 `num7` | `计数` | do/while 计数器 |
| 局部 `num` / `num2` / `num3` / `num4`（错误分发） | 删除 | VB 错误状态号 |
| 字段 `m_PageAttr` / `valAttr` / `_ProcessAttr` / `requestAttr` / `watcherAttr` | 未改名 | 控件后备字段，代码一律走属性名 |
| `_PrototypeAttr` | 未改名 | 就是 `components` |

## 关键方法与流程（按方法分段，写清步骤）

### `PopInfo`（密码校验）
1. `Strings.Len(TextBox1.Text) < 6` → `Label1.Text = "密码应该是6位以上的字母或数字，请重新输入！"`，清空输入，结束。
2. 否则：`输入摘要 = ObserverAttr.InstantiateInfo(Strings.Trim(输入))`，`已存摘要 = HKCU\Software\n`。
3. `Operators.CompareString(输入摘要, 已存摘要, false) == 0` → 清提示、禁用按钮、`Hide()`、`CandidateAttr.窗体集合.Form1.Show()`。
4. 否则 `Label1.Text = "密码不正确，请重新输入！"`，清空输入。

### `DestroyInfo`（Form_Shown）
读 `HKCU\Software\n`；等于空串（首次运行、没设过密码）→ `Hide()` + 显示 `Form1`；否则停在密码框等人输入。

### `LogoutInfo`（Form_Load，`checked { }` 上下文）
1. `日期乘积 = 月 × 日`；取 `%7 %9 %5 %3` 四个余数。
2. 按 `日期乘积 % 2` 的奇偶，用 `Chr(97+余数)/Chr(98+…)/Chr(101+…)/Chr(99+…)` 拼出 4 个小写字母，写入 `ObserverAttr.账号进程名`，并把 `+".exe"` 写入 `ObserverAttr.访客名`。
3. 紧接着**覆盖**：`VBMath.Randomize(日期乘积)` → `种子 = Round(Rnd()*100000*3+1)` → do/while 循环 5 次：`五位名字 = Chr(种子%10+105) + 五位名字`、`种子 /= 10`。
4. 把 5 位名字（字符范围 `'i'`~`'r'`）重新写入 `账号进程名` 与 `访客名`（后者带 `.exe`）。

### `ReflectInfo`
空方法体（原版只有 `On Error Resume Next` 骨架，没有任何语句）—— 双击 Label_pd 没有任何效果。

## 恶意行为 / 敏感操作
- **访问控制**：整个设置界面被一个 6 位以上密码挡住；密码摘要算法 = `SHA256(明文+"bfdshgs")`，存 `HKCU\Software\n`。
- **首次运行放行**：`DestroyInfo` 发现没设过密码就直接进 `Form1`，说明「密码」是管理员自己设的，不是授权校验。
- **随机进程名**：`LogoutInfo` 生成 `'i'`~`'r'` 范围的 5 字符随机名并写进全局静态字段，供 `Form1.CallInfo` 当作要杀掉的「账号进程名」。这也解释了 `CallInfo` 里那段「名字长度≥4 且字符 ASCII 在 f~o」的过滤逻辑（目标就是这类随机名进程）。
- 无文件/注册表写入（本文件只**读** `HKCU\Software\n`）。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 注册表：`HKCU\Software`，值名 `n`（密码摘要）
- 字符串：`输入小助手密码：`、`确 定`、`密码应该是6位以上的字母或数字，请重新输入！`、`密码不正确，请重新输入！`、`   `（Label_pd 的三个空格）、`load`
- 字符表：`Chr(97..)`（a 起）、`Chr(105..)`（i 起，最终名字范围 `i`~`r`）
- 界面坐标：Label_pd(356,70, 69×35, 宋体 26.25)、Label1(52,136)、Label3(46,82)、TextBox1(163,78, 122×21)、Button1(296,75, 54×27)

## 反混淆处理记录
- goto：**15 → 0**；`IL_xxxx:` 40 → 0；`@switch` 6 → 0；`endfilter` 3 → 0；`ClearProjectError()` 全清
- `On Error Resume Next` 还原：`PopInfo` / `DestroyInfo` 各用一层 `try { } catch { }` 包住（方法内只有字符串比较与控件赋值，合一层不改变可观察行为）
- 语义保留点：`Operators.CompareString`、`Strings.Len/Trim/Chr`、`Conversions.ToString`、`VBMath.Randomize/Rnd`、`Math.Round(unchecked(...))`、`checked { }` 上下文、`Strings.Chr(种子 % 10L + 105L)` 的 long 运算，全部原样
- 未改动：`ReadInfo()` 布局正文、5 个属性包装、字段声明
