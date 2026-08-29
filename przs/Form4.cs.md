# Form4.cs — 最高级别锁屏窗（鼠标锁定 + 蜂鸣，可配置是否允许关闭）

## 文件作用
比 Form2 更严厉的惩罚窗体：全屏、无边框、TopMost、隐藏任务栏，挂 1s 定时器把鼠标用 `ClipCursor`
限制在屏幕顶部 100 像素并反复 `Beep`（锁屏+警告）。关闭行为由全局标志 `Initializer._Collection` 控制：
为 `false` 时允许关闭并恢复全屏鼠标；为 `true` 时禁止关闭（强行锁死）。Form4 自身无业务逻辑，由其他
模块决定何时展示/何时放行。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form4 | 锁屏惩罚窗（ClipCursor + Beep） |
| Form4.RECT | Win32 RECT 结构（ClipCursor 参数） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `StartBroadcaster` | `初始化组件` | 创建 Label、1s 定时器 |
| `CountBroadcaster` | `锁鼠标定时器`（Timer_frm4 1s） | ClipCursor 顶部 100px，5 次后 Beep |
| `CancelBroadcaster` | `Load 启用定时器` | Timer_frm4.Enabled = true |
| `PrepareBroadcaster` | `控制窗体关闭`（FormClosing） | 依据 `Initializer._Collection` 决定是否取消 |
| `_Item` | `锁屏计时计数` | 达到 5 次开始蜂鸣 |

## 关键方法与流程
### 锁鼠标定时器（CountBroadcaster，1s Tick）
- **流程**：`TopMost=true`；`ClipCursor({0,0,800,100})` 锁定鼠标到顶部 100px 区域；`_Item++`，
  `>=5` 后每次 `Beep(2000, 500)` 蜂鸣。

### 控制窗体关闭（PrepareBroadcaster，FormClosing）
- **流程**：`Initializer._Collection == false` → 允许关闭（`e.Cancel=false`）并 `ClipCursor` 恢复
  全屏区域（解除锁定）；`== true` → `e.Cancel=true` 拒绝关闭（锁死）。

## 恶意行为 / 敏感操作
- `ClipCursor` 锁鼠标（顶部 100px 区域）。
- 高频 `Beep` 骚扰（每秒 2kHz/500ms，5 次后）。
- 条件性禁止关闭窗口（与 `Initializer._Collection` 联动，形成不可退出锁屏）。

## IOC / 硬编码
- 无路径/网络 IOC；仅 Win32 API：`user32!ClipCursor`、`kernel32!Beep`。
