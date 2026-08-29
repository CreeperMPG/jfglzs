# bianhao.cs — "编号"悬浮标（显示计算机名后 6 位，透明悬浮窗）

## 文件作用
小号透明悬浮窗（窗体名"bianhao"=编号）。Load 时自动定位到屏幕右上角（`Left = 屏宽-350, Top = 100`），
用 65 号黑体大字显示**计算机名后 6 位**（`Strings.Right(My.Computer.Name, 6)`），即机房机器编号标识。
鼠标悬停时显示小字"由机房管理助手自动生成"，移开清除。无恶意行为，属提示类 UI；由 Form1 的
`Timer_main` 在 `HKCU\Software\jfglzs\bianhao=on` 时显示。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| bianhao | 机器编号悬浮标签窗 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `CloneBroadcaster` | `初始化组件` | 创建两个 Label |
| `DestroyBroadcaster` | `Load 定位与取编号` | 右上角定位 + 取计算机名后 6 位 |
| `ConcatBroadcaster` | `悬停显示提示` | "由机房管理助手自动生成" |
| `ListBroadcaster` | `移开清除提示` | Label2 置空 |
| `token` | `编号 Label`（Label1） | 默认文本 "stu30" |
| `m_Rules` | `提示 Label`（Label2） | 悬停说明 |

## 关键方法与流程
### Load 定位与取编号（DestroyBroadcaster）
- **流程**：`Left = Screen.PrimaryScreen.Bounds.Width - 350`，`Top = 100`；
  `Label1.Text = Strings.Right(Account.StopBroadcaster.Name, 6)`（取 `My.Computer.Name` 即计算机名末 6 字符）。

## 恶意行为 / 敏感操作
- 无恶意行为；仅读取本机计算机名（信息收集性质极弱）并悬浮显示。

## IOC / 硬编码
- 无 IOC。默认显示文本 `"stu30"`（学生 30 号，作者调试遗留）。
