# Form3.cs — 「退出程序」密码验证窗体（解除机房锁定：恢复USB/浏览器/CMD并结束监控进程）

## 文件作用
带密码保护的退出对话框。用户输入 6 位以上密码，与注册表 `HKCU\Software\n` 中保存的 `SHA256(密码+"bfdshgs")` 哈希比对；正确后执行一整套「解锁」流程：结束监控进程（`zmserv` 服务、`jfglzsn`、随机名进程）、恢复 USB 存储（`usbstor` Start=3）、恢复浏览器下载/开发者工具策略、重新启用 CMD（DisableCMD=0）、重启 explorer。该窗体是整套「机房管理助手」系统中唯一合法的"逃生门"。核心逻辑集中在 `InterruptReponse`（点击按钮）与 `CalcReponse`（杀进程）、`SortReponse`（恢复策略），均带控制流扁平化混淆（`IL_xx + num2/num3` 状态机）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form3 | 密码验证 + 解锁流程执行窗体 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `DeleteReponse` | InitializeComponent_窗体控件初始化 | Designer 产物 |
| `InterruptReponse` | 「退出(需10秒)」按钮点击处理 | 密码校验 + 触发解锁流程（状态机还原见下） |
| `CalcReponse` | 结束监控进程/服务 | 杀 zmserv、jfglzsn、随机名进程、数字/字母名进程 |
| `SortReponse` | 恢复浏览器下载与开发者工具策略 | 改 IE/Edge/Chrome/Firefox 策略并杀浏览器进程 |
| `MapReponse` | 检测进程是否运行 | 与 Form1.ViewReponse 相同实现 |
| `pc_num` | 返回主机名末字符 ASCII | 按 COMPUTERNAME 计算机器码（授权用） |
| `fieldProduct` | Label1（提示信息） | — |
| `m_ParamsProduct` | Label3（「输入小助手密码：」） | — |
| `connectionProduct` | TextBox1（密码输入框，PasswordChar='*'） | — |
| `_ParamProduct` | Button1（「退出(需10秒)」） | — |
| `ruleProduct` | Label2（使用说明） | — |
| `_ParserProduct` | IContainer 容器 | Designer 用 |

## 关键方法与流程

### 按钮点击处理（原名 InterruptReponse）
- **职责**：校验密码并执行解锁。
- **流程（还原自 IL 状态机：num2 = 2→3→4 / 7→8→9→10→12→13→14…→21，或错误分支 23→24→25）**：
  1. 密码长度 < 6 → Label1 提示「密码应该是6位以上的字母或数字，请重新输入！」并清空输入框，结束；
  2. `left = SHA256(Trim(密码) + "bfdshgs")`（十六进制），与 `HKCU\Software\n` 读出的值比较；
  3. 不一致 → 提示「密码不正确，请重新输入！」、清空输入框；
  4. 一致 → 禁用按钮、清空提示；
  5. 调用 `CalcReponse()`（杀监控进程/服务）、`SortReponse()`（恢复浏览器策略）；
  6. 依次将 `HKLM\SYSTEM\CurrentControlSet\Services\usbstor`、`ControlSet001/002/003\Services\usbstor` 的 `Start` 值设为 `3`（即恢复 USB 大容量存储设备服务为手动/可用）；
  7. Kill 所有 `explorer` 并重新 `Start("explorer.exe")`（重建桌面）；
  8. 结束（不再关闭本窗体——真正的关闭在 CalcReponse 成功分支）。
- **关键调用/API**：`PropertyProduct.ExcludeReponse/PostReponse/ChangeReponse`、`Registry`、`Process.Kill`。

### 结束监控（原名 CalcReponse）
- **职责**：强力结束监控进程与服务，并恢复被禁用的 CMD。
- **流程（还原自 IL 状态机）**：
  1. 禁用按钮；定义目标：`zmserv`（服务名）、`PropertyProduct._SystemProduct`（随机名进程）、`jfglzsn`；
  2. 循环 `GetProcessesByName(随机名)[0].Kill()` + `jfglzsn[0].Kill()`，直到两者都不存在；
  3. `num3 = 10; num3 = 10*10+2 = 102`——遍历**所有进程**，进程名长度 ≥4 且**每个字符的 ASCII 都在 [102,111]（'f'~'o'）** 的进程一律 `Kill()`（疑似为"全数字/特定命名"的隐藏进程设计的模糊判定，反编译后阈值失真）；
  4. 设置 `HKCU\Software\Policies\Microsoft\Windows\System` 的 `DisableCMD = 0`（重新启用命令提示符）；
  5. `Interaction.Shell("net stop zmserv", Hide, true)` 停服务 → `Thread.Sleep(3000)` → `GetProcessesByName("zmserv")[0].Kill()`；
  6. 再循环 Kill 随机名进程与 `jfglzsn` 直到消失；再次 `net stop zmserv`、Sleep 2000；
  7. 若 `jfglzsn` 与随机名进程都不在运行 → MsgBox「退出程序成功！」→ `Close()`；否则 MsgBox「退出程序不成功！请重新执行退出程序命令或重启电脑」并重新启用按钮。
- **关键调用/API**：`Process.GetProcesses`、`Process.Kill`、`Interaction.Shell("net stop zmserv")`、`Thread.Sleep`、`Registry`。

### 恢复浏览器策略（原名 SortReponse）
- **职责**：撤销监控期对浏览器下载/保存/开发者工具的封锁。
- **流程**：
  1. `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3`：`1803=0`、`2200=0`（解除 IE 安全区下载限制）；
  2. `HKCU\Software\Policies\Microsoft\Internet Explorer\Restrictions`：`NoBrowserSaveAs=0`；
  3. `HKLM\SOFTWARE\Policies\Microsoft\Edge`：`DownloadRestrictions=0`、`SaveAs=1`、`DeveloperToolsAvailability=0`；
  4. `HKLM\SOFTWARE\Policies\Google\Chrome`：同 Edge 三项；
  5. `HKLM\SOFTWARE\Policies\Mozilla\Firefox`：`DisableDownloads=0`、`BlockAboutDownloads=0`、`DeveloperToolsAvailability=0`；
  6. 依次 Kill 全部 `chrome`、`msedge`、`firefox`、`iexplorer` 进程。
- **关键调用/API**：`PropertyProduct.ChangeReponse`（写注册表）、`Process.GetProcessesByName().Kill()`。

### 机器码计算（原名 pc_num）
- **流程**：读环境变量 `COMPUTERNAME`；为空返回 88；否则返回**末字符的 ASCII 码**。用于授权/密码派生。

## 恶意行为 / 敏感操作
- 以 `OpenProcess` 之外的常规手段（枚举进程 + Kill）**定向清除监控进程与同族进程**（`zmserv`、`jfglzsn`、随机名进程）。
- **修改 `usbstor` 服务注册表 Start 值**（CurrentControlSet/ControlSet001~003 四处）恢复 USB 设备可用——反向印证监控期禁用了 USB。
- **重写浏览器组策略**（IE/Edge/Chrome/Firefox）并杀掉所有浏览器进程。
- **恢复 DisableCMD=0**（监控期禁用了 cmd）。
- Kill 并重启 explorer.exe。
- 按 ASCII 区间（102~111）批量 Kill 进程，误杀面广。

## IOC / 硬编码
- 注册表（写）：
  - `HKLM\SYSTEM\CurrentControlSet\Services\usbstor` / `ControlSet001` / `ControlSet002` / `ControlSet003`：`Start=3`
  - `HKCU\Software\Policies\Microsoft\Windows\System`：`DisableCMD=0`
  - `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3`：`1803=0`、`2200=0`
  - `HKCU\Software\Policies\Microsoft\Internet Explorer\Restrictions`：`NoBrowserSaveAs=0`
  - `HKLM\SOFTWARE\Policies\Microsoft\Edge`：`DownloadRestrictions=0`、`SaveAs=1`、`DeveloperToolsAvailability=0`
  - `HKLM\SOFTWARE\Policies\Google\Chrome`：同上
  - `HKLM\SOFTWARE\Policies\Mozilla\Firefox`：`DisableDownloads=0`、`BlockAboutDownloads=0`、`DeveloperToolsAvailability=0`
- 注册表（读）：`HKCU\Software\n`（密码哈希）
- 服务/命令：`net stop zmserv`
- 进程（Kill）：`zmserv`、`jfglzsn`、`PropertyProduct._SystemProduct`、ASCII∈[102,111] 且长度≥4 的进程、`explorer`（Kill 后重启）、`chrome`、`msedge`、`firefox`、`iexplorer`
- 密码盐：`"bfdshgs"`（SHA256）
- 界面文本：「输入小助手密码：」「退出(需10秒)」「注：用密码退出后可使用U盘、下载文件，停止监控。」「退出程序成功！」「退出程序不成功！请重新执行退出程序命令或重启电脑」
