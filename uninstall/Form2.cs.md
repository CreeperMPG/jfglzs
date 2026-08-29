# Form2.cs — 卸载确认窗体：密码验证、卸载执行、鼠标锁定与强制重启

## 文件作用
Form2 是"卸载小助手"的全屏置顶强制界面：要求输入 6 位以上密码（密码哈希存在 `HKCU\Software\n`），验证通过后执行完整卸载（`RegConfig.InvokeUtils`），恢复默认壁纸、锁定鼠标、7 秒后退出；同时提供"重启电脑"按钮（强制重启）与 QQ/官网联系方式。窗体无法关闭（FormClosing 被取消），Timer 每 1 秒把鼠标夹在屏幕顶部区域。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form2 | 密码门 + 卸载执行界面（全屏、置顶、不可关闭） |
| `WallpaperStyle` 枚举 | 壁纸拉伸样式（Tiled=1, Centered=2, Stretched=6） |
| `RECT` 结构体 | ClipCursor 的矩形区域 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `TestUtils` | 初始化窗体控件/布局 | 构建全部控件（密码框、按钮、提示 Label） |
| `ReadUtils` | Button1_Click：校验密码并执行卸载 | 扁平化控制流主逻辑 |
| `AssetUtils` | Timer_frm2_Tick：锁定鼠标+蜂鸣 | 每秒把鼠标限制在 (0,0,800,200) |
| `StartUtils` | Button2_Click：强制重启电脑 | `shutdown /r /f /t 0` |
| `OrderUtils` | 设置默认壁纸 | 写注册表 + SystemParametersInfo(SPI_SETDESKWALLPAPER) |
| `CompareUtils` | FormClosing 拦截 | `e.Cancel = true` 禁止关闭 |
| `InterruptUtils` | Form2_Load | 启动 Timer_frm2 |
| `ConnectUtils` | 打开官网链接 | `Process.Start("http://www.jfglzs.com")`（当前无控件绑定） |
| `SetupUtils` | Label_pd 双击处理 | 空壳（异常处理骨架，无实际逻辑） |
| `Label_pd`/`m_IdentifierConfig` | 隐藏双击触发 Label | 用于 SetupUtils 挂钩 |

## 关键方法与流程

### Button1_Click（原名 ReadUtils）
- **职责**：验证密码，通过则执行完整卸载并收尾。
- **流程**（还原扁平化控制流）：
  1. 密码框文本长度 < 6 → Label2 显示"密码应该是6位以上的字母或数字，请重新输入！"，清空输入，返回。
  2. `left = RegConfig.VisitUtils(Trim(密码))`（即 SHA256(密码+"bfdshgs") 十六进制小写）。
  3. `right = HKCU\Software\n` 的值；`left != right` → "密码不正确，请重新输入！"，清空，返回。
  4. 密码正确：禁用按钮、等待光标 → **`RegConfig.InvokeUtils()` 执行完整卸载** → 恢复默认光标。
  5. 读取 `HKCU\Control Panel\Desktop\Wallpaper`，若包含"壁纸"字样 → `OrderUtils("C:\Windows\Web\Wallpaper\Windows\img0.jpg", Stretched)` 恢复系统默认壁纸。
  6. `ClipCursor` 锁定鼠标到全屏矩形。
  7. `Thread.Sleep(7000)` → `ProjectData.EndApp()` 退出进程。
- **关键调用/API**：`RegConfig.VisitUtils`、`RegConfig.RemoveUtils`、`ClipCursor`、`SystemParametersInfo`。

### Timer_frm2_Tick（原名 AssetUtils）
- **流程**：`TopMost = true`；`ClipCursor(0,0,800,200)` 把鼠标限制在屏幕顶部区域；计数 `_ListenerConfig++`，每 5 次蜂鸣一次 `Beep(2000, 500)`。

### Button2_Click（原名 StartUtils）
- **流程**：`Interaction.Shell("shutdown /r /f /t 0", MinimizedFocus)` —— **强制立即重启电脑**。

### 设置壁纸（原名 OrderUtils）
- **流程**：文件不存在则抛异常；写 `HKCU\Control Panel\Desktop` 的 `Wallpaper` 与 `WallpaperStyle=2`；调用 `SystemParametersInfo(20, 0, path, 3)`（SPI_SETDESKWALLPAPER + SPIF_UPDATEINIFILE|SPIF_SENDWININICHANGE）。

## 恶意行为 / 敏感操作
- 强制全屏置顶、不可关闭、鼠标锁定 + 周期性蜂鸣 —— 典型的"勒索/强制交互"式 UI（阻止用户绕过卸载）。
- 校验密码后执行 `RegConfig.InvokeUtils` 全套卸载动作（杀进程/删服务/删注册表/删文件，详见 RegConfig.cs.md）。
- `shutdown /r /f /t 0` 无确认强制重启。
- 卸载完成后睡眠 7 秒自动退出。

## IOC / 硬编码
- 网站：`http://www.jfglzs.com`（界面提示"下载最新版本"）
- QQ：`1023411046`（界面提示联系客服）
- 提示文本："2、卸载后记得在硬盘保护系统中创建进度或保存系统。"
- 壁纸恢复路径：`C:\Windows\Web\Wallpaper\Windows\img0.jpg`
- 密码算法：`SHA256(输入密码 + "bfdshgs")` 十六进制小写，与 `HKCU\Software\n` 比对
- 命令：`shutdown /r /f /t 0`
