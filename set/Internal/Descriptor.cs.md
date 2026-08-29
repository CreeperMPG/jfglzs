# Internal/Descriptor.cs — My.Application：VB 应用框架主入口（Main/STAThread）

## 文件作用
`WindowsFormsApplicationBase` 子类，即 VB.NET 应用框架的 `My.Application`。**它是程序正式入口**：`InstantiateProperty(string[] args)` 带 `[STAThread]` 特性，等价于 Main 方法，调用 `Reader.ChangeProperty.Run(args)` 启动应用框架；`OnCreateMainForm` 把主窗体设为 `Reader.ReflectProperty.load`（密码门窗体）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Descriptor | VB 应用框架（单实例/视觉样式/关闭策略） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| InstantiateProperty(args) | Main_程序入口 | [STAThread]，SetCompatibleTextRenderingDefault 后 Run(args) |
| Descriptor() 构造 | 初始化框架 | IsSingleInstance=false、EnableVisualStyles=true、SaveMySettingsOnExit=true、ShutdownStyle=AfterMainFormCloses |
| OnCreateMainForm() | 指定主窗体 | MainForm = Reader.ReflectProperty.load |
| LogoutClass() | 占位 | return false |

## 关键方法与流程
### Main_程序入口（InstantiateProperty）
1. `Application.SetCompatibleTextRenderingDefault(UseCompatibleTextRendering)`（finally 空）
2. `Reader.ChangeProperty.Run(args)` 进入 VB 应用框架消息循环
3. 框架回调 `OnCreateMainForm` → 主窗体 = load（密码验证 → Form1 设置界面）
- 与 `ExceptionMock.InvokeProperty()`（Application.Run() 无参 + load.Show()）互为补充入口，壳运行时（TestMock.InterruptMock）可能在这两者之间做切换/补丁

## 恶意行为 / 敏感操作
- 无直接恶意逻辑；程序启动链入口（结合壳内存补丁）

## IOC / 硬编码
- 无恶意 IOC
