# UtilsConfig.cs — VB 应用程序入口（My.Application，启动 Form1）

## 文件作用
VB.NET `WindowsFormsApplicationBase` 子类，即 `My.Application`。`EnableUtils(args)` 是程序集入口引导：设置兼容文本渲染后调用 `Run(args)` 启动 WinForms 消息循环；`OnCreateMainForm` 把主窗体设为 **Form1**（卸载入口窗体）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| UtilsConfig | 应用启动器 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `EnableUtils` | 程序入口 Main | SetCompatibleTextRenderingDefault + Run |
| `OnCreateMainForm` | 指定主窗体 | MainForm = Form1 |

## 关键方法与流程
- 构造参数：`IsSingleInstance=false`、`EnableVisualStyles=true`、`SaveMySettingsOnExit=true`、`ShutdownStyle=AfterMainFormCloses`。
- 启动顺序：`EnableUtils → VisitorConfig.CollectUtils.Run → OnCreateMainForm → Form1 创建 → Form1.Load（CustomizeUtils）→ 静默卸载或弹 Form2`。

## 说明
- 模板生成骨架 + 一行关键业务绑定（主窗体=Form1），本身无恶意动作。`SetRegistry()` 为恒 false 桩。
