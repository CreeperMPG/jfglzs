# Internal/TemplateConfig.cs — VB 应用程序启动模板（程序入口，Main 函数所在）

## 文件作用
这是 VB.NET WinForms 项目的**程序入口**（`WindowsFormsApplicationBase` 子类，标准 My 模板）：`UpdateTemplate`（= Main）设置兼容文本渲染后调用 `TokenizerConfig.PopTemplate.Run(args)` 启动应用；`OnCreateMainForm` 把主窗体指定为 **Form1**（恶意主控窗体）。`OrderParser` 是壳/混淆器加入的恒 true 桩。类本身无恶意逻辑，但它决定了程序一启动就进入 Form1 的恶意调度。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `TemplateConfig` | 应用启动器（VB WindowsFormsApplicationBase） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `UpdateTemplate` | Main 入口 | `[STAThread]`，`Application.SetCompatibleTextRenderingDefault` 后 `TokenizerConfig.PopTemplate.Run(args)` |
| `.ctor` | 应用配置 | IsSingleInstance=false、EnableVisualStyles=true、SaveMySettingsOnExit=true、ShutdownStyle=AfterMainFormCloses |
| `OnCreateMainForm` | 指定主窗体 | `MainForm = TokenizerConfig.FindTemplate.Form1` |
| `OrderParser` | 壳桩 | 恒返回 true |

## 关键方法与流程

### Main 入口（原名 `UpdateTemplate(string[] args)`）
- **流程**：1. `Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering)`（try/finally 空壳）；2. `TokenizerConfig.PopTemplate.Run(args)` 进入 VB 消息循环。
- **关键调用/API**：`Application`、`WindowsFormsApplicationBase.Run`。

### 指定主窗体（原名 `OnCreateMainForm`）
- **流程**：`this.MainForm = TokenizerConfig.FindTemplate.Form1;`——主窗体即 Form1（隐藏式恶意主控窗体）。
- **关键调用/API**：`TokenizerConfig.FindTemplate.Form1`（经 DicConfig 懒加载创建 Form1 实例）。

## 恶意行为 / 敏感操作
- 程序入口直接导向恶意主控窗体 Form1（启动即加载全部定时器与监控）。

## IOC / 硬编码
- 无独立 IOC；入口：`TemplateConfig.UpdateTemplate` → `Form1`。
