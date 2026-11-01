# Form1.Designer.cs

## 文件作用

主窗体「学生机房管理助手v13.4」的完整实现。因为原程序是 VB.NET 的 `[DesignerGenerated]` 窗体，dnSpy 把**控件初始化 + 全部事件处理器 + 属性访问器**都导出到这一个文件里，所以业务逻辑和设计器样板混在一起。

反混淆后可以确认，这个窗体是整个程序的**前台控制面板**，只做四件事：

1. 窗口 Load 时算出监控端的随机进程名（写进静态字段）；
2. 每 500ms 检查监控端进程 `jfglzsn` 是否在跑并显示状态；
3. 「启动程序」菜单拉起监控端并重启资源管理器；
4. 「退出程序」菜单是唯一受密码保护的入口（转发到 Form3 或 `set.exe`）。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `Form1`（继承 `Form`） | 主窗体；状态轮询 + 三个外链/配置入口 + 启动监控端 |
| 构造函数 | 挂 `Load` 处理器后初始化控件（顺序不可换） |
| `窗体加载_生成助手进程名` | Load 处理器：按「月×日」生成监控端进程名，写入 `SetterMapper.小助手进程名` |
| `定时检查小助手状态` | Timer(500ms) Tick：查 `jfglzsn` 进程，切换标签文本 |
| `进程是否在运行` | 进程存在性判断（按进程名，非文件名） |
| `启动小助手_Click` | 「启动程序」：按有无密码拉 `set.exe` 或 `przs.exe`+`jfglzsn.exe`，再重启 explorer |
| `退出程序_Click` | 「退出程序」：无密码 → 打开 `set.exe`；有密码 → `Form3.Show()` |
| `设置_Click` | 「设  置」：`set.exe 1` |
| `访问官网_Click` / `官网链接_Click` / `查看帮助_Click` | 打开三个 jfglzs 站点 URL |
| `主菜单项点击_空实现` | MenuStrip.ItemClicked 空处理器 |
| `启动更新器_未绑定` | 启动 `更新器.exe`；**未绑定任何事件**（死代码） |
| `初始化控件` | VB 的 InitializeComponent：菜单、标签、链接、定时器 |
| 十个控件属性 | 设计器生成的属性；setter 里手动维护事件订阅 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `PatchComposer` | `窗体加载_生成助手进程名` | Load 处理器，生成随机进程名 |
| `SearchComposer` | `定时检查小助手状态` | Timer.Tick |
| `EnableComposer(string)` | `进程是否在运行(string 进程名)` | `GetProcessesByName().Length > 0` |
| `CollectComposer` | `启动小助手_Click` | 启动程序菜单，含 On Error 还原 |
| `CustomizeComposer` | `退出程序_Click` | 退出程序菜单，转发 Form3 |
| `WriteComposer` | `设置_Click` | 启动 `set.exe 1` |
| `StartComposer` | `访问官网_Click` | 打开 `http://www.jfglzs.com/` |
| `SetupComposer` | `官网链接_Click` | 打开 `http://www.jfglzs.com` |
| `ManageComposer` | `查看帮助_Click` | 打开 `.../c1.html` |
| `RunComposer` | `主菜单项点击_空实现` | 空方法体 |
| `AwakeComposer` | `启动更新器_未绑定` | 启动 `更新器.exe`，无引用 |
| `DeleteComposer` | `初始化控件` | VB 的 InitializeComponent |
| `_PublisherMapper` | `菜单栏` | MenuStrip1 的后备字段 |
| `_RuleMapper` | `系统功能菜单项` | 「系统功能（F）」后备字段（自动属性，实际未读写） |
| `m_FilterMapper` / `m_RefMapper` / `m_ReponseMapper` / `factoryMapper` | `设置菜单项` / `退出程序菜单项` / `访问官网菜单项` / `启动小助手菜单项` | 四个菜单项后备字段 |
| `_BroadcasterMapper` | `定时器` | Timer 后备字段 |
| `_PrinterMapper` / `definitionMapper` | `状态标签` / `标题标签` | Label2 / Label1 后备字段 |
| `m_RequestMapper` / `m_ConnectionMapper` | `官网链接` / `帮助链接` | LinkLabel1 / LinkLabel2 后备字段 |
| `m_ServerMapper` | `控件容器` | `IContainer` |
| `_TokenMapper` | `未使用字段` | 无赋值点的 long 残留 |
| `componentResourceManager` | `资源管理器` | `ComponentResourceManager` |
| 局部 `text` / `text2` / `num*` | `密码哈希` / `日期名` / `随机数` / `月日乘积` 等 | 逐处语义化 |

## 关键方法与流程

### 1. 构造函数

`base.Load += 窗体加载_生成助手进程名;` → `初始化控件();`
顺序是原版顺序：先挂事件，再建控件。

### 2. `窗体加载_生成助手进程名(sender, e)`（Load）

整段在 `checked` 上下文（VB 默认 checked，溢出会抛异常）：

1. `月日乘积 = 当月 × 当日`；
2. 取 `%7 / %9 / %5 / %3` 四个余数；
3. 按 `月日乘积 % 2` 选两套排列，用 `Strings.Chr(97/98/101/99 + 余数)` 拼出 4 字母名
   （第一套 `a(97)+r7, b(98)+r9, e(101)+r5, c(99)+r3`，第二套把 r7/r9、r5/r3 互换）；
4. 写 `SetterMapper.小助手进程名 = 日期名`、`小助手可执行文件名 = 日期名 + ".exe"`；
5. `VBMath.Randomize(月日乘积)` 播种 → `Rnd()` → `Math.Round(Rnd()*100000*3 + 1)` 得到 `随机数`；
6. `do { 数字名 = Chr(随机数 % 10 + 105) + 数字名; 随机数 /= 10; 位数++; } while (位数 <= 5);`
   即取 5 个 `Chr(105..114)` 字符，得到 `i`..`r` 区间的 5 字符名；
7. **覆盖**第 4 步的结果：再写一次 `小助手进程名` / `小助手可执行文件名`。

> 第 4 步的名字立刻被第 7 步覆盖，属原版残留逻辑；但两次赋值都是真实副作用，故完整保留。
> 结果是：进程名每天固定、完全可预测（`Randomize` 用的种子就是「月×日」），并非真随机。

### 3. `定时检查小助手状态`

`进程是否在运行("jfglzsn")` → 假则 `Label2.Text = "关 闭"` 并 `return`；真则 `"正在运行中。。。"`。

### 4. `启动小助手_Click`（On Error Resume Next 还原）

1. 读 `HKCU\Software\n` → `密码哈希`（VB `On Error Resume Next`：读失败按空串处理）；
2. `Operators.CompareString(密码哈希, "", false) == 0`：
   - 相等（没设过密码）→ `Process.Start(启动目录 + "\set.exe")`；
   - 否则 → 依次 `Process.Start(..."\przs.exe")`、`Process.Start(..."\jfglzsn.exe")`；
3. 枚举 `explorer` 进程逐个 `Kill()`；
4. `Process.Start("explorer.exe")`。

原版是 `ClearProjectError()` + `num2 = 2,3,4,6,7,8,9,10,11` 的状态机 + 三个 `goto`
（`IL_0052` 跳过 set.exe 分支、`IL_0080` 汇合到 explorer 段、`IL_0142` 结束），
`catch when (endfilter(...))` 是 VB 的错误分发。这里按「if/else + 每条语句独立 try/catch」还原：
`Process.Start` / `Kill` / `GetProcessesByName` 各自吞异常，保证一条失败不影响后续步骤——与
`On Error Resume Next` 的逐语句语义一致。

### 5. `退出程序_Click`

`Operators.CompareString(Conversions.ToString(读取注册表值(HKCU, "Software", "n", "")), "", false) == 0`
→ 相等则 `Process.Start(...\set.exe)` 后 `return`；否则 `ExpressionMapper.VisitComposer.Form3.Show()`。
也就是说：**没设密码时点「退出程序」不会退出，而是打开配置程序**。

### 6. `初始化控件`

创建 `MenuStrip1`、四个菜单项（设置 / 退出程序 / 启动程序 / 访问官网）、两个 Label、两个 LinkLabel、
一个 500ms 的 Timer；窗口标题 `学生机房管理助手v13.4`、`ClientSize 682×486`、居中启动、
背景图与图标取自 `Form1.resources`。所有属性赋值逐条保留。

### 7. 控件属性

六个带事件的属性（MenuStrip1 / 设置 / 退出程序 / 访问官网 / Timer1 / LinkLabel1 / LinkLabel2 / 启动小助手）
在 setter 里「先退订旧实例再订阅新实例」，其余（系统功能 / Label1 / Label2）是自动属性。

## 恶意行为 / 敏感操作

| 行为 | 具体表现 |
| --- | --- |
| **查杀进程** | `Process.GetProcessesByName("explorer")` 后逐个 `Kill()`，再重启 explorer —— 强制刷新桌面/策略，用户会看到桌面闪断 |
| **拉起监控端** | `set.exe`（配置/设密码）、`przs.exe`、`jfglzsn.exe`（监控端，名字来自注册表判断），全部从程序自身目录启动 |
| **密码凭据读取** | 读 `HKCU\Software\n`（SHA256 值）决定走「无密码」还是「要密码」分支 |
| **受限的退出** | 退出流程不是关程序，而是要求密码；无密码时等于把控制权交给 `set.exe` |
| **进程名规避** | Load 时把监控端进程名算成 `i`..`r` 区间的可预测随机串（按日期播种），避免固定进程名被杀软/学生手工查杀 |
| **外部通信** | 打开 `http://www.jfglzs.com/`、`.../c1.html`（纯浏览器跳转，程序自身不联网） |
| 无注册表写入 | 本文件只**读**注册表；写入在 Form3 与 SetterMapper |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 窗体标题 | `学生机房管理助手v13.4` |
| URL | `http://www.jfglzs.com/`、`http://www.jfglzs.com`、`http://www.jfglzs.com/c1.html` |
| 同目录程序 | `set.exe`、`przs.exe`、`jfglzsn.exe`、`更新器.exe` |
| 进程名 | `explorer`、`jfglzsn` |
| 注册表 | `HKCU\Software` 值名 `n`（退出密码 SHA256 hex） |
| 定时器 | `Interval = 500`（毫秒），`Enabled = true` |
| 界面尺寸 | `ClientSize 682×486`、`AutoScaleDimensions 6×12`、字体 `宋体 9pt` |
| 标签文本 | `当前小助手工作状态：`、`关 闭`、`正在运行中。。。`、`>>>访问官网`、`查看帮助` |
| 菜单文本 | `系统功能（F）`、`设  置`、`退出程序`、`启动程序`、`访问官网` |
| 进程名生成参数 | `Chr` 基数 `97/98/101/99`、模数 `7/9/5/3`、`Rnd()*100000*3+1`、`Chr(%10+105)` |
| 图标/背景资源 | `$this.Icon`、`$this.BackgroundImage`（来自 `Form1.resources`） |
| IL 行号 | 类型 `0x0200000B`，方法 `0x06000033`–`0x06000040`，属性 `0x1700000C`–`0x17000016` |
