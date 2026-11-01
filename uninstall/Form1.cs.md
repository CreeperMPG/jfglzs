# Form1.Designer.cs 分析

## 文件作用

`uninstall.exe` 的**启动窗体**（主窗体）。一个 385×224、无边框、不显示在任务栏、启动即最小化的
「隐身窗口」——用户看不到它，它只在 `Load` 事件里决定：

- 本机**没有**设置卸载密码 → 直接执行卸载，等 9 秒，结束进程；
- 设置了密码 → 弹出 `Form2` 让用户输入密码，并开 1 秒定时器保证 `Form2` 一直可见。

dnSpy 把整个 `Form1` 类（含事件处理器）全导在这个 `.Designer.cs` 里，所以本文件同时是
「Designer 代码 + 业务代码」。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `Form1` | 隐藏的启动窗体；生成当日随机进程名；按有无密码分流 |
| `SearchReader`（Load） | 生成进程名 → 写 `卸载核心.进程名/进程文件名/候选安装目录` → 分流到「直接卸载」或「Form2 要密码」 |
| `PublishReader`（Timer2.Tick） | 每 1 秒检查 `Form2.Visible`，不可见就再 `Show()`（防止用户把密码窗藏掉） |
| `FillReader`（InitializeComponent） | 控件初始化：1 秒定时器、385×224、无边框、无控制框、不进任务栏、标题「卸载」、最小化 |
| `Dispose(bool)` | 释放 Designer 组件容器 |
| `Timer2` 属性 | 1 秒定时器的可视化属性，set 时自动挂/摘 Tick |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `Form1` | `Form1`（保留） | 与 `.resx`、`My.Forms` 绑定，不能改名 |
| `SearchReader` | `SearchReader`（保留方法名） | 命名已无语义，靠 XML 注释说明；语义＝Load 初始化 |
| `PublishReader` | 同上 | Timer2.Tick：保证 Form2 可见 |
| `FillReader` | 同上 | InitializeComponent |
| `m_Policy` | `组件容器` | `IContainer` |
| `_Advisor` | `_定时器实例` | `Timer2` 的后备字段 |
| 局部 `num3` | `日期乘积` | `Month(Now) * Day(Now)` |
| 局部 `num4/num5/num6/num7` | `模7 / 模9 / 模5 / 模3` | 日期乘积的四个取模 |
| 局部 `num8` | `随机数` | `Round(Rnd() * 100000 * 3 + 1)` |
| 局部 `text` | `进程名文本` | 5 个 `i..r` 字母 |
| 局部 `num9` | `位数计数` | 循环 5 次 |
| 局部 `num10` | `字符码` | `随机数 % 10 + 105` |
| 局部 `text2` | `密码哈希` | `HKCU\Software` 的 `n` |
| 局部 `num/num2/num11/num12` | 已删除 | On Error 状态机 |

## 关键方法与流程

### `SearchReader(object, EventArgs)` —— Load 事件

1. `日期乘积 = Month(Now) * Day(Now)`；算 `%7`、`%9`、`%5`、`%3`。
2. 按奇偶拼 4 个字母给窗体改名：
   - 奇数：`Chr(97+模9) Chr(98+模7) Chr(101+模3) Chr(99+模5)`
   - 偶数：`Chr(97+模7) Chr(98+模9) Chr(101+模5) Chr(99+模3)`
   然后写 `卸载核心.进程名` / `进程文件名` —— **这两行紧接着就被第 3 步覆盖，属于遗留代码**（原样保留）。
3. `VBMath.Randomize(日期乘积)` → `随机数 = Round(Rnd() * 100000 * 3 + 1)`；
   循环 5 次取十进制最低位 `% 10 + 105` 变成字符并前插，得到 5 个 `i`..`r` 的字母：
   ```
   do { 进程名文本 = Chr(随机数 % 10 + 105) + 进程名文本; 随机数 /= 10; } while (计数 <= 5)
   ```
   写入 `卸载核心.进程名 = 进程名文本`、`进程文件名 = 进程名文本 + ".exe"`、
   `候选安装目录 = "C:\Program Files (x86)\" + Mid(进程名文本, 2, 4)`。
4. `Show()` 把隐藏窗口显示出来（最小化 + 不进任务栏，用户看不到）。
5. 读 `HKCU\Software` 的 `n`：
   - 非空（有密码）→ `My.Forms.Form2.Show()`，`Timer2.Enabled = true`；
   - 为空（无密码）→ `卸载核心.执行卸载()` → `Thread.Sleep(9000)` → `ProjectData.EndApp()`（VB 的 `End`，直接结束进程）。

原版整段在 `On Error Resume Next` 之下：每条语句出错都被吞掉继续下一条，因此反混淆后逐条 `try/catch`；
纯算术赋值不会抛异常，保持裸语句。

### `PublishReader(object, EventArgs)` —— Timer2.Tick

`if (!Form2.Visible) Form2.Show();` —— 每 1 秒兜一次，防止用户把密码窗体最小化/隐藏。

### `FillReader()` —— InitializeComponent

`组件容器 = new Container()`；`Timer2 = new Timer(组件容器)`，`Interval = 1000`；
窗体 `ClientSize = 385×224`、`FormBorderStyle.None`、`ControlBox = false`、`ShowInTaskbar = false`、
`Text = "卸载"`、`WindowState = Minimized`。

## 恶意行为 / 敏感操作

| 类型 | 具体行为 |
| --- | --- |
| 隐蔽窗口 | 无边框 + 不进任务栏 + 最小化 + `ShowInTaskbar=false`：用户看不到卸载器正在运行 |
| 免密自动卸载 | 只要 `HKCU\Software\n` 为空，**不需要任何交互**就调用 `执行卸载()`，9 秒后 `EndApp()` |
| 强制保持前台 | 定时器不断把 `Form2` 重新 `Show()` 出来，绕过「最小化/隐藏密码窗」的规避手法 |
| 进程名随机化 | 用日期做种子的 `VBMath` 随机数生成 5 字母进程名，使静态进程名规则失效 |
| 结束进程 | `ProjectData.EndApp()`（VB `End`）强制终止当前进程 |

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 窗口标题 | `卸载` | 隐藏窗体的 `Text` |
| 窗口尺寸 | `385 × 224` | ClientSize |
| 定时器 | `Interval = 1000` | `Timer2` |
| 字符范围 | `97..114`（`a`..`r`）偏移、`+ 105` | 进程名由 `chr(105..114)` 组成，即 **`i`..`r` 五个字母段** |
| 随机数公式 | `Rnd() * 100000 * 3 + 1` | 进程名种子 |
| 注册表 | `HKCU\Software` → 值 `n` | 卸载密码哈希（空＝免密） |
| 路径 | `C:\Program Files (x86)\` + `Mid(进程名, 2, 4)` | `候选安装目录`（本工程未使用） |
| 等待 | `Thread.Sleep(9000)` | 卸载后停留 9 秒 |
| 调用 | `卸载核心.执行卸载()`、`My.Forms.Form2.Show()`、`ProjectData.EndApp()` | 分流动作 |
