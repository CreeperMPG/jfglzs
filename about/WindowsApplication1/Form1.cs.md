# Form1.cs — 主窗体「学生机房管理助手v13.1」（监控状态显示 + 启动/退出监控入口）

## 文件作用
程序主窗体。标题为「学生机房管理助手v13.1」，本质是**机房电脑锁定/监控软件的配套控制台**：通过菜单启动监控进程（`przs.exe` / `jfglzsn.exe`）、检测监控进程运行状态、根据注册表密码决定是否放行「退出程序」入口，并在窗体加载时生成**随日期变化的随机监控进程名**（用于对抗按进程名查杀/退出）。大量方法名被混淆为 `*Reponse`（如 `SelectReponse`/`FlushReponse`），字段被混淆为 `*Product`，其中 `InstantiateReponse` 保留了完整的 `IL_xx + num2 = N` 控制流扁平化状态机。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form1 | 主控制窗体：菜单（系统功能F）、状态标签、定时器、超链接 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `StartReponse` | InitializeComponent_窗体控件初始化 | 手工版 Designer 初始化（VB.NET DesignerGenerated 产物） |
| `RateReponse` | 窗体加载时生成随机监控进程名 | 挂在 `base.Load` 上 |
| `SelectReponse` | 菜单「退出程序」点击处理 | 读注册表密码判断走 set.exe 还是打开 Form3 |
| `FlushReponse` | 菜单「设置」点击处理 | 启动 `set.exe 1` |
| `FindReponse` | 菜单「访问官网」点击处理 | 打开官网 |
| `CheckReponse` | LinkLabel1「>>>访问官网」点击 | 打开官网 |
| `DestroyReponse` | LinkLabel2「查看帮助」点击 | 打开帮助页 c1.html |
| `ListReponse` | MenuStrip ItemClicked 事件 | 空实现（占位） |
| `InstantiateReponse` | 菜单「启动程序」点击处理 | 启动监控进程并重启资源管理器（控制流混淆最重的方法） |
| `RemoveReponse` | Timer1 500ms 定时器 Tick | 检测 `jfglzsn` 进程是否在运行，更新 Label2 状态 |
| `ViewReponse` | 检测进程是否在运行 | 封装 `Process.GetProcessesByName().Length > 0` |
| `CompareReponse` | 启动「更新器.exe」 | 未被 Designer 挂接的遗留方法 |
| `mockProduct` | 组件容器 IContainer | Designer 用 |
| `_ExpressionProduct` | MenuStrip1 后备字段 | — |
| `orderProduct` | 系统功能FToolStripMenuItem 后备字段 | — |
| `_CodeProduct` | 设置ToolStripMenuItem 后备字段 | — |
| `_ServerProduct` | 退出程序ToolStripMenuItem 后备字段 | — |
| `m_PoolProduct` | 访问官网ToolStripMenuItem 后备字段 | — |
| `parameterProduct` | 启动小助手ToolStripMenuItem 后备字段 | — |
| `m_BridgeProduct` | Timer1 后备字段 | — |
| `mapProduct` | Label2（状态标签）后备字段 | — |
| `m_DecoratorProduct` | Label1 后备字段 | — |
| `_FacadeProduct` | LinkLabel1 后备字段 | — |
| `tokenizerProduct` | LinkLabel2 后备字段 | — |
| `comparatorProduct` | long 型字段 | 未使用 |

## 关键方法与流程

### 随机监控进程名生成（原名 RateReponse，Load 事件）
- **职责**：根据当前日期与随机数生成监控程序的进程名（`PropertyProduct._SystemProduct` / `indexerProduct`），使监控进程名每天/每次启动都不同。
- **流程**：
  1. `num = 月 × 日`；分别取 `num % 7 / % 9 / % 5 / % 3` 得到 4 个偏移量；
  2. 若 `num` 为偶数：进程名 = chr(97+off1) + chr(98+off2) + chr(101+off3) + chr(99+off4)（小写字母拼出的 4 字符名）；
  3. 奇数则换一种顺序；
  4. 先写入 `_SystemProduct`，`indexerProduct = 名字 + ".exe"`；
  5. 再用 `num` 作随机种子 `Randomize`，生成 1~300000 的随机数，逐位取出数字并转成 `chr(105+digit)`（即字符 'i'~'n' 段），拼成 5 字符名，**再次覆盖** `_SystemProduct` / `indexerProduct`（最终生效的是 5 位数字映射的名字）。
- **关键调用/API**：`DateAndTime`、`VBMath.Randomize/Rnd`、`Strings.Chr`、写 `PropertyProduct` 静态字段。

### 「启动程序」点击处理（原名 InstantiateReponse）
- **职责**：启动机房监控/限制进程，并强制重启 explorer 以应用限制策略。
- **流程（还原自 IL 状态机：num2 依次 = 2→3→4/6→7→8→10→11→13）**：
  1. 读注册表 `HKCU\Software\n`（即密码哈希）：
  2. 若为空 → 启动 `Application.StartupPath\set.exe`（首次部署）；
  3. 否则 → 依次启动 `przs.exe` 与 `jfglzsn.exe`（监控主程序）；
  4. 遍历所有 `explorer` 进程并 `Kill()`；
  5. 重新启动 `explorer.exe`（让桌面在监控策略下重建）。
- **关键调用/API**：`Registry.CurrentUser`、`PropertyProduct.PostReponse`、`Process.Start`、`Process.GetProcessesByName("explorer").Kill`。

### 菜单「退出程序」点击处理（原名 SelectReponse）
- **流程**：读 `HKCU\Software\n` 密码哈希 → 为空则启动 `set.exe`（说明未完成配置），否则显示 `WrapperProduct.SearchReponse.Form3`（需密码的退出窗体）。

### 定时状态检测（原名 RemoveReponse，Timer1，间隔 500ms）
- **流程**：`ViewReponse("jfglzsn")` 为真 → Label2 显示「正在运行中。。。」，否则「关 闭」。即主界面实时显示监控进程是否存活。

### 官网/帮助链接
- `FindReponse` / `CheckReponse` → `Process.Start("http://www.jfglzs.com/")`
- `DestroyReponse` → `Process.Start("http://www.jfglzs.com/c1.html")`

## 恶意行为 / 敏感操作
- **主动 Kill 并重启 explorer.exe**（在「启动程序」流程中，疑似强刷桌面以应用/恢复系统限制）。
- **启动多个监控/管理进程**（`set.exe`、`przs.exe`、`jfglzsn.exe`），形成进程链。
- **按日期+随机数动态生成监控进程名**，规避按固定进程名的排查/退出。
- **从注册表读取密码哈希判断是否放行退出功能**，把控制权与服务端/安装期配置绑定。
- 大量 `Process.Start` 运行外部可执行文件（含 `更新器.exe`，暗示可自我更新）。

## IOC / 硬编码
- 路径：`Application.StartupPath + "\set.exe"`、`+ "\przs.exe"`、`+ "\jfglzsn.exe"`、`+ "\更新器.exe"`
- 注册表：`HKCU\Software\n`（读）
- 进程：`jfglzsn`、`explorer`（被 Kill 并重启）
- 网络：`http://www.jfglzs.com/`、`http://www.jfglzs.com/c1.html`
- 界面文本：「学生机房管理助手v13.1」「当前小助手工作状态：」「正在运行中。。。」「关 闭」「启动程序」「退出程序」「访问官网」
