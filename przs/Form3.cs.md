# Form3.cs — 违规提醒小窗（三行红色说明标签）

## 文件作用
无边框提示窗，TopMost 置顶，标题"提醒"。不拦截鼠标、不锁屏，仅用三个 `shuoming/shuoming2/shuoming3`
红色大字号 Label 展示违规说明（如"本电脑禁止下载安装软件""现在助手强制恢复""刚刚删除文件：…"）。
由 Form1 各定时器/监视器事件驱动显示与关闭，无独立恶意逻辑，纯 UI 提示载体。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form3 | 违规行为红色提醒窗（3 个说明 Label） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `RunBroadcaster` | `初始化组件` | 创建 3 个 Label 并排版 |
| `shuoming` | `主说明 Label` | 违规行为描述（26 号红色字） |
| `shuoming2` | `次级说明 Label` | 处理结果/建议 |
| `shuoming3` | `详情 Label` | 被删除文件路径等 |

## 关键方法与流程
### 初始化组件（RunBroadcaster）
- **流程**：创建 `shuoming2`（15.75pt 红色）、`shuoming`（15.75pt 红色）、`shuoming3`（默认字号），
  依次加入控件；窗体 `TopMost=true`、`StartPosition=CenterScreen`、`Text="提醒"`、蓝色背景。

## 恶意行为 / 敏感操作
- 无直接恶意操作；仅作为其他模块（Form1/Form2）的告警 UI。其余为窗体属性设置。

## IOC / 硬编码
- 无 IOC。纯 UI 窗体。
