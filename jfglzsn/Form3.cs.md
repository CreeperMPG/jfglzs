# Form3.cs — 警告提示窗（非锁定，仅显示违规说明）

## 文件作用
Form3 是**居中的置顶警告窗**（无边框），只有三个红色说明标签（shuoming / shuoming2 / shuoming3），没有按钮、没有密码输入。主控在检测到"较轻"违规（如防火墙开启、改密码、改 IP、极域参数被改、运行违规软件等）时通过 `BeginInvoke` 或定时器弹出提示；超过阈值后会被 Form2/Form4 锁屏取代并 `Close()`。该窗体本身没有任何恶意逻辑，只是 UI 组件。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `Form3` | 警告提示窗体（纯 UI） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `RemoveTemplate` | 初始化控件与布局 | 创建 3 个 Label，居中置顶显示 |
| `shuoming` / `shuoming2` / `shuoming3` | 警告主文案 / 副文案 / 水印行 | 由 Form1 各监控逻辑动态赋值 |

## 关键方法与流程
- 构造器调用 `RemoveTemplate()`：创建 `shuoming`（15.75pt 红字）、`shuoming2`（15.75pt 红字）、`shuoming3`（14.25pt 说明/水印）；`BackColor=SystemColors.Highlight`（蓝底）；`StartPosition=CenterScreen`、`TopMost=true`、无边框。
- 无任何事件处理器、无计时器、无关闭拦截——纯展示窗口，`Close()` 可正常关闭。

## 恶意行为 / 敏感操作
- 无直接恶意操作；仅用于恐吓/警告 UI（配合锁屏流程）。

## IOC / 硬编码
- 无网络/注册表/进程 IOC；文案由外部（Form1）注入。
