# Form2.cs — 全屏警告/锁屏窗（退出密码验证 + 鼠标锁定 + 强制重启）

## 文件作用
"违规"告警与拦截主窗口：无边框全屏、TopMost、不可关闭（`FormClosing` 被取消）。正常情况用红色大字
显示违规说明；用户若想退出程序必须输入密码（SHA256 加盐比对注册表），双击隐藏的 `Label_pd` 可进入
教师维护后门（写入明文密码、启动验证程序 `yz.exe`）。提供"重启电脑"按钮并可用 `ClipCursor` 把鼠标
限制在屏幕顶部 100 像素区域（锁屏效果）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form2 | 全屏拦截窗：密码退出、锁鼠标、重启、后门维护入口 |
| Form2.RECT | Win32 RECT 结构（ClipCursor 参数） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `PatchBroadcaster` | `初始化组件` | 创建标签/按钮/文本框/1s 定时器 |
| `ChangeBroadcaster` | `确定按钮：验证退出密码` | SHA256 比对 `HKCU\Software\n` |
| `PopBroadcaster` | `执行退出流程` | 杀 zmserv/jfglzsn、net stop zmserv、解除鼠标锁定、退出程序 |
| `FillBroadcaster` | `锁鼠标定时器`（Timer_frm2 1s） | ClipCursor 到顶部 100px，5 次后 Beep |
| `FindBroadcaster` | `Label_pd 双击后门` | 显示维护密码串，写 dtmm/czmm，启动 yz.exe |
| `InitBroadcaster` | `重启电脑按钮` | `shutdown /r /f /t 0` |
| `OrderBroadcaster` | `阻止窗体关闭` | `e.Cancel = true` |
| `CustomizeBroadcaster` | `Load 启用定时器` | Timer_frm2.Enabled = true |
| `pc_num()` | `取本机编号` | 计算机名末字符 ASCII 码，空名返回 88 |
| `interceptor` | `锁屏计时计数` | 达到 5 次开始蜂鸣 |

## 关键方法与流程
### 确定按钮：验证退出密码（ChangeBroadcaster）
- **流程**：1. 密码长度 <6 → 提示"密码应该是6位以上的字母或数字"并清空；2. 否则
  `Initializer.MapBroadcaster(Trim(输入))`（SHA256(输入+"bfdshgs") 十六进制）与
  `HKCU\Software\n` 比对；3. 相等 → 禁用按钮、执行 `PopBroadcaster` 退出；不等 → "密码不正确，请重新输入！"。

### 执行退出流程（PopBroadcaster）
- **流程**：1. 循环 `Process.GetProcessesByName("jfglzsn")[0].Kill()` + Sleep(500) 直至杀光；
  2. `HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD = 0`（解除禁用 CMD）；
  3. `Shell("net stop zmserv", Hide)`（停止机房管理服务 zmserv）；4. Sleep(3000)；
  5. 杀 `zmserv` 进程；6. `ClipCursor` 恢复全屏区域；7. `EndApp()` 退出本程序。
- **关键调用/API**：`Process.Kill`、`Interaction.Shell("net stop zmserv")`、`ClipCursor`、`EndApp`。

### 锁鼠标定时器（FillBroadcaster，1s Tick）
- **流程**：`TopMost=true`，`ClipCursor({0,0,500,100})` 把鼠标限制在屏幕顶部 100px 高区域，
  `interceptor>=5` 后每次 `Beep(2000, 500)` 蜂鸣警告。

### Label_pd 双击后门（FindBroadcaster）
- **流程**：`Label2.Text = "ngfxd568bdh158" + pc_num() + "8bsdg45ybfdhg"`（显示教师维护密码，
  由固定前缀/后缀拼接本机编号得到）；把当前输入框内容明文写入 `HKCU\Software\dtmm`；
  写 `HKCU\Software\czmm = SHA256("111111"+盐)`；启动 `config\yz.exe`（验证工具）。

### 重启电脑按钮（InitBroadcaster）
- **流程**：`Interaction.Shell("shutdown /r /f /t 0")` 立即强制重启。

## 恶意行为 / 敏感操作
- 强制重启电脑（`shutdown /r /f /t 0`）。
- 停止并杀死服务/进程：`net stop zmserv`、Kill `zmserv`、Kill `jfglzsn`。
- `ClipCursor` 锁鼠标（顶部 100px），配合蜂鸣构成锁屏惩罚。
- 阻止窗体关闭（`FormClosing` 取消），不可任务栏关闭。
- 隐藏后门：双击 `Label_pd` 泄露维护密码串并把密码明文写入注册表（`dtmm`）。

## IOC / 硬编码
- 进程/服务：`jfglzsn`、`zmserv`（`net stop zmserv`）
- 注册表：`HKCU\Software\n`（退出密码哈希）、`HKCU\Software\dtmm`（明文密码）、
  `HKCU\Software\czmm`（重置密码哈希）、`HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD`
- 密码盐：`"bfdshgs"`（SHA256 加盐）；默认重置密码为 `"111111"`
- 后门密码串模板：`ngfxd568bdh158<编号>8bsdg45ybfdhg`（编号 = 计算机名末字符 ASCII）
- 命令：`shutdown /r /f /t 0`、`net stop zmserv`
- 路径：`<config>\yz.exe`
