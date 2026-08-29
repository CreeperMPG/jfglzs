# Form1.cs — 卸载器入口窗体：静默卸载 / 密码卸载的决策与随机进程名生成

## 文件作用
Form1 是 VB.NET 应用的主窗体（由 `UtilsConfig.OnCreateMainForm` 设为 MainForm）。它本身不可见（无边框、最小化、不在任务栏显示），构造时注册 Load 事件，在加载时完成两件事：生成"随机 5 字符进程名/安装目录名"（供卸载逻辑匹配可执行文件），并判断是否已设置卸载密码：无密码 → 直接静默执行完整卸载；有密码 → 弹出 Form2 要求输入密码。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form1 | 卸载入口窗体（隐藏），负责随机命名与卸载触发决策 |
| Timer2（`m_RequestConfig`） | 1 秒间隔心跳定时器，密码模式下反复确保 Form2 可见 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `NewUtils` | 初始化窗体控件/布局 | 构造 Timer2(1000ms)、隐藏窗体外观 |
| `CustomizeUtils` | Form1_Load：生成随机名并决策卸载路径 | 控制流被扁平化的主逻辑 |
| `ExcludeUtils` | Timer2_Tick：保持 Form2 可见 | Form2 不可见时 Show 它 |
| `m_RequestConfig` | Timer2 字段 | 定时器后端字段 |

## 关键方法与流程

### Form1_Load（原名 CustomizeUtils）
- **职责**：生成随机进程名/目录名，读取注册表密码标记，决定静默卸载还是走密码验证。
- **流程**（还原扁平化控制流）：
  1. 计算 `num3 = 当前月份 × 当前日期`（如 8 月 29 日 → 232），再取对 7/9/5/3 的余数 num4~num7。
  2. 若 num3 为偶数：`Name = Chr(97+num4) + Chr(98+num5) + Chr(101+num6) + Chr(99+num7)`；否则换顺序拼接。此 Name 随即被覆盖，属混淆噪声。
  3. `RegConfig.m_ConnectionConfig = Name`（随即被覆盖）。
  4. `Randomize(num3)` 后用 `Rnd()` 生成 1~300000 的随机数 num8，循环 5 次取每位数字 `Chr(数字+105)`（数字 0~9 → 字符 'i'~'r'）拼接成 **5 字符随机名 text**（如 "jkopq"）。
  5. 写入全局配置：`m_ConnectionConfig = text`；`propertyConfig = text + ".exe"`；`m_PrototypeConfig = "C:\Program Files (x86)\" + Mid(text,2,4)`（即安装目录为 "C:\Program Files (x86)\" + 4 个随机字符）。
  6. `this.Show()`。
  7. 读注册表 `HKCU\Software\n`（默认 ""）→ 判断是否设置了卸载密码。
  8. **若为空（未设密码）**：直接 `RegConfig.InvokeUtils()` 执行完整卸载 → `Thread.Sleep(7000)` → `ProjectData.EndApp()`（结束进程）。
  9. **若非空（已设密码）**：`VisitorConfig.ListUtils.Form2.Show()` 显示密码窗体，`Timer2.Enabled = true` 启动心跳。
- **关键调用/API**：`Registry.CurrentUser`、`RegConfig.InvokeUtils`、`VBMath.Randomize/Rnd`、`ProjectData.EndApp`。

### Timer2_Tick（原名 ExcludeUtils）
- **职责**：Form2 被关闭/隐藏时强制重新显示（配合 Form2 的 FormClosing 拦截，构成"无法关闭"的强制卸载界面）。
- **流程**：`if (!Form2.Visible) Form2.Show();`

## 恶意行为 / 敏感操作
- 程序自隐藏运行（无边框、最小化、不在任务栏），属于隐蔽执行。
- 动态生成随机 5 字符进程名与安装目录名，用于规避按固定文件名查杀（卸载器据此匹配并 Kill 同名进程）。
- 无密码时静默执行全套卸载（杀进程、删服务、删注册表、删文件、重启资源管理器）。

## IOC / 硬编码
- 注册表：`HKCU\Software\n`（卸载密码哈希标记，非空=需密码）
- 路径模板：`C:\Program Files (x86)\<4随机字符>\<5随机字符>.exe`（随机字符集 'i'~'r'）
- 关联组件：`jfglzsn.exe`、`zmserv`（服务）、`przs.exe`、`C:\windows\jf\`（见 RegConfig）
- 随机名与当天日期（月×日）相关，但最终名由 `Rnd()` 决定，不可预测
