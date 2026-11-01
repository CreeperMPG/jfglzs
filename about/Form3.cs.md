# Form3.Designer.cs

## 文件作用

「退出程序」窗体——本程序**唯一受密码保护的出口**，也是整套管控的"总开关"。名字叫退出，实际做的是"临时解除管控"：密码校验通过后，停监控进程与服务、把 USB 存储与浏览器下载策略改回允许、杀掉浏览器、重启 explorer。

这个文件同样是 `[DesignerGenerated]` 窗体（控件初始化 + 事件处理器同文件），并且是 `about` 工程里 `goto` 最集中的一处（原 15 个），全部已还原。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `Form3`（继承 `Form`） | 密码校验 + 解除管控 |
| `退出按钮_Click` | 「退出(需10秒)」：长度检查 → 密码哈希比对 → 依次执行解除动作 |
| `执行退出流程` | 停监控：循环杀助手/监控进程、按 ASCII 区间模糊杀进程、`net stop zmserv`、成功/失败提示 |
| `恢复浏览器下载策略` | 12 条浏览器策略键改回"允许"，并杀掉 chrome/msedge/firefox/iexplorer |
| `进程是否在运行` | 进程存在性判断 |
| `计算机器编号` | 取 `COMPUTERNAME` 末字符 ASCII；空则 88 |
| `初始化控件` | 密码框 + 按钮 + 两个标签 + 说明文字 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ForgotComposer` | `退出按钮_Click` | 按钮点击，密码校验总入口 |
| `InsertComposer` | `执行退出流程` | 停监控端的主流程（含两个 do-while 重试循环） |
| `ReadComposer` | `恢复浏览器下载策略` | 12 条策略 + 杀浏览器 |
| `SortComposer(string)` | `进程是否在运行(string 进程名)` | `GetProcessesByName().Length > 0` |
| `InvokeComposer` | `初始化控件` | VB 的 InitializeComponent |
| `pc_num()` | `计算机器编号()` | 保留原方法名含义（`pc_num` 原本就是它） |
| `m_TokenizerMapper` | `提示标签` | Label1 后备字段 |
| `readerMapper` | `密码标题标签` | Label3 后备字段 |
| `_ModelMapper` | `密码输入框` | TextBox1 后备字段 |
| `_InterceptorMapper` | `退出按钮` | Button1 后备字段 |
| `specificationMapper` | `说明标签` | Label2 后备字段 |
| `_DecoratorMapper` | `控件容器` | `IContainer` |
| 局部 `text` / `text2` | `输入哈希` / `已存哈希` | 密码比对两侧 |
| 局部 `text`/`text2`/`text3`（Insert） | `服务名` / `助手进程名` / `监控进程名` | `zmserv` / 日期随机名 / `jfglzsn` |
| 局部 `num3`/`num4`/`num5`/`num6` | `字符下界` / `进程名长度` / `扫描位置` / `字符编码` | 进程名 ASCII 扫描 |

## 关键方法与流程

### 1. `退出按钮_Click`（原 3 个 goto）

1. `Strings.Len(TextBox1.Text) < 6` → 提示「密码应该是6位以上的字母或数字，请重新输入！」+ 清空输入框 + `return`；
2. `输入哈希 = 计算密码哈希(Strings.Trim(输入))`（= `SHA256(UTF8(密码 + "bfdshgs"))` 小写 hex）；
3. `已存哈希 = Conversions.ToString(读取注册表值(HKCU, "Software", "n", ""))`；
4. `Operators.CompareString(输入哈希, 已存哈希, false) != 0` → 提示「密码不正确，请重新输入！」+ 清空 + `return`；
5. 校验通过：`Button1.Enabled = false`、清提示；
6. `执行退出流程()`；
7. `恢复浏览器下载策略()`；
8. 写 4 处 USB 策略：`HKLM\SYSTEM\{CurrentControlSet,ControlSet001,ControlSet002,ControlSet003}\Services\usbstor` 的 `Start = 3`（3 = 允许 USB 存储）；
9. 杀 `explorer` → `Process.Start("explorer.exe")`。

原版 goto 映射：`IL_0046` = 跳过长度提示进入校验；`IL_0182` = 跳到密码错误分支；`IL_01A8` = 跳到统一出口。全部还原为 if/return。
`catch when (endfilter(...))` 是 VB 错误分发；因为每条语句都单独忽略了错误，删除状态机（`num2 = 2..23`、`ProjectData.ClearProjectError`）不影响行为。

### 2. `执行退出流程`（原 6 个 goto + 2 个状态机）

1. 禁用按钮；`服务名 = "zmserv"`、`助手进程名 = 小助手进程名`（Form1 按日期算出的随机名）、`监控进程名 = "jfglzsn"`；
2. **循环 1**（do-while）：`GetProcessesByName(助手名)[0].Kill()`、`GetProcessesByName(监控名)[0].Kill()`，任一条抛异常（列表空 → 下标越界）被单独吞掉后继续，直到两个列表都为空；
3. `字符下界 = 10; 字符下界 = 字符下界 * 10 + 2;` → 102（`'f'`）；
4. 遍历 `Process.GetProcesses()`：进程名长度 ≥ 4，且**每个字符**的 ASCII 都落在 `102..111`（`'f'`..`'o'`）之间 → `Kill()`。
   这是"按名字形状"模糊查杀：`i`..`r` 区间的随机名（Form1 生成的那种）大多会命中；
5. `HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD = 0`（恢复命令提示符）；
6. `Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1)`（隐藏窗口、等待完成）；
7. `Thread.Sleep(3000)` → 杀 `zmserv` 进程；
8. **循环 2**：与循环 1 完全相同的重试逻辑；
9. 再 `net stop zmserv` → `Thread.Sleep(2000)`；
10. 判定：`!进程是否在运行("jfglzsn")` 与 `!进程是否在运行(小助手进程名)` 都为真（注意用的是 `&` 非短路）
    → 弹「退出程序成功！」并 `Close()`；
    否则弹「退出程序不成功！请重新执行退出程序命令或重启电脑」并恢复按钮。

原版 goto 映射：`IL_0122` = 跳到"该进程处理完毕"；`IL_0130` = 循环退出后的判定点；
`IL_023A` = 失败分支；`IL_0259` = 汇合出口；catch 里的 `IL_0346`/`IL_0378` = 错误分发与"未处理错误"。
`while` + `break` 取代了 `IL_0122`/`IL_0130` 那套跳转，`if/else` 取代了成功/失败分支。

### 3. `恢复浏览器下载策略`（原 `checked { }` 四个 Kill 循环）

按顺序写 12 条策略（全部 `RegistryValueKind.DWord`）：

| 根键 | 子键 | 值 | 数据 |
| --- | --- | --- | --- |
| HKCU | `SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3` | `1803` | 0 |
| HKCU | 同上 | `2200` | 0 |
| HKCU | `Software\Policies\Microsoft\Internet Explorer\Restrictions` | `NoBrowserSaveAs` | 0 |
| HKLM | `SOFTWARE\Policies\Microsoft\Edge` | `DownloadRestrictions` / `SaveAs` / `DeveloperToolsAvailability` | 0 / 1 / 0 |
| HKLM | `SOFTWARE\Policies\Google\Chrome` | 同上三项 | 0 / 1 / 0 |
| HKLM | `SOFTWARE\Policies\Mozilla\Firefox` | `DisableDownloads` / `BlockAboutDownloads` / `DeveloperToolsAvailability` | 0 / 0 / 0 |

随后依次 Kill `chrome`、`msedge`、`firefox`、`iexplorer`（原文拼写少一个 `e`，对 IE 无效）。
本方法没有 try/catch，写入失败会抛到调用方（`退出按钮_Click` 里被吞）。

### 4. `计算机器编号`

`Environment.GetEnvironmentVariable("COMPUTERNAME")` → 空则 `88`；否则 `Strings.Asc(末字符)`。
变量取不到（null）会 NRE —— 原版同样如此。本程序集内无调用点。

## 恶意行为 / 敏感操作

| 行为 | 具体表现 |
| --- | --- |
| **密码校验（可爆破）** | 单轮 `SHA256(密码 + 固定盐 "bfdshgs")`，盐写死在代码里，密码可离线爆破；比较结果存 `HKCU\Software\n` |
| **结束进程（模糊查杀）** | ① 按精确名杀 `jfglzsn` / 按日期算出的随机名 / `zmserv`；② 按"名字全是 `'f'`..`'o'`"的形状杀任意进程（**误杀风险**：任何 4 字符以上、全由 f–o 组成的进程名都会被杀） |
| **停服务** | `Interaction.Shell("net stop zmserv", Hide, wait, -1)`——隐藏窗口、同步等待，停掉 `zmserv` 服务 |
| **系统策略改写** | 4 处 `usbstor\Start = 3`（放开 USB 存储）、`DisableCMD = 0`（放开命令提示符）——都是 `HKLM` 系统级键 |
| **浏览器策略改写** | Edge / Chrome / Firefox / IE 的下载、另存为、开发者工具共 12 条策略，全部改为"允许" |
| **强制重启桌面** | 杀 `explorer` 后重启，让策略立即生效，用户感知为桌面闪断 |
| 自述文案 | 「注：用密码退出后可使用U盘、下载文件，停止监控。」——直接说明本窗体的真实用途 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 窗口标题 | `退出程序` |
| 界面 | `ClientSize 417×181`，按钮文本 `退出(需10秒)`，密码框 `PasswordChar = '*'`，标题 `输入小助手密码：` |
| 提示文案 | `密码应该是6位以上的字母或数字，请重新输入！`、`密码不正确，请重新输入！`、`退出程序成功！`、`退出程序不成功！请重新执行退出程序命令或重启电脑`、`注：用密码退出后可使用U盘、下载文件，停止监控。` |
| 服务名 | `zmserv`；命令 `net stop zmserv`（执行两次） |
| 进程名 | `jfglzsn`、`zmserv`、`explorer`、`chrome`、`msedge`、`firefox`、`iexplorer`（原文拼写） |
| 进程名启发式区间 | ASCII `102..111`（`'f'`..`'o'`），长度 ≥ 4，由 `10*10+2` 算出 |
| 注册表（USB） | `SYSTEM\CurrentControlSet\Services\usbstor`、`SYSTEM\ControlSet001/002/003\Services\usbstor`，值 `Start`=3 |
| 注册表（CMD） | `HKCU\Software\Policies\Microsoft\Windows\System`，`DisableCMD`=0 |
| 注册表（浏览器） | 见上表 12 条 |
| 注册表（密码） | `HKCU\Software` 值名 `n` |
| 延时 | `Thread.Sleep(3000)`、`Thread.Sleep(2000)` |
| 机器编号 | `COMPUTERNAME` 末字符 ASCII；空时 `88` |
| IL 行号 | 类型 `0x0200000C`，方法 `0x06000057`–`0x06000068`，属性 `0x17000017`–`0x1700001B` |
