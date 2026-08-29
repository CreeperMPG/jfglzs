# Broadcaster.cs — VB.NET 应用程序入口（My.Application / WindowsFormsApplicationBase）

## 文件作用
反混淆后真实身份：**VB.NET 的 `My.Application` 对象**（继承 `Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase`），
由 `MyTemplate` 生成器产出，ConfuserEx 改名为 `Broadcaster`。它定义了程序的启动方式：`Main` 入口
（`ViewBroadcaster`）→ 设置兼容渲染 → 运行 `My.Application`；`OnCreateMainForm` 把主窗体指定为
**Form1**。`Account.CollectBroadcaster`（My.Application）即返回本类实例，MySettings 通过它订阅
`Shutdown` 事件以自动保存设置。`IsSingleInstance=false`、`EnableVisualStyles=true`、
`SaveMySettingsOnExit=true`、`ShutdownMode=AfterMainFormCloses`。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Broadcaster | My.Application：程序启动对象与生命周期管理 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `ViewBroadcaster` | `程序入口 Main` | `[STAThread]`，设置兼容文本渲染后 `Run(args)` 启动应用 |
| `OnCreateMainForm` | `指定主窗体` | `MainForm = My.Forms.Form1`（恶意主控窗体） |
| `SetThread`（恒 false） | 混淆器填充方法 | ConfuserEx 无意义方法 |
| `Account.CollectBroadcaster` | My.Application 单例 | 见 Account.cs |

## 关键方法与流程
### 程序入口 Main（ViewBroadcaster）
- **流程**：1. `Application.SetCompatibleTextRenderingDefault(...)`（try/finally 空壳）；
  2. `Account.CollectBroadcaster.Run(args)` 启动 WindowsFormsApplicationBase 消息循环。
- **关键调用/API**：`WindowsFormsApplicationBase.Run`。

### 指定主窗体（OnCreateMainForm）
- **流程**：`this.MainForm = Account.DefineBroadcaster.Form1`——程序一启动即实例化 Form1
  （触发 Form1 构造与 Load：隐藏窗口、自我复制、注册表持久化、全盘文件监视）。

## 恶意行为 / 敏感操作
- 启动即拉起恶意主窗体 Form1（隐藏驻留 + 持久化 + 文件监控）。

## IOC / 硬编码
- 无独立 IOC。行为由 Form1 承担。
