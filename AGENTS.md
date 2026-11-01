# AGENTS.md — 机房管理助手 反编译工作区

## 这是什么

- 样本：6 个 .NET 程序（VB.NET + WinForms），**全部被 .NET Reactor 加壳**；
- **样本只做静态分析，禁止运行**（包括 `*_Slayed.exe` 和 `unpacked\` 里的一切）。

## 目录结构

`samples\`、`unpacked\` 与本目录同级；`samples\` 只是本文对「原始样本所在目录」的代称，实际目录名按你的放置替换。

| 路径 | 内容 | 谁写 |
| --- | --- | --- |
| `samples\<名>.exe` | 原始样本，只读 | 分析者提供 |
| `samples\<名>_Slayed.exe` | NETReactorSlayer 解壳产物 | 流程第 2 步 |
| `unpacked\<名>.exe` | de4dot 收尾后的 assembly | 流程第 3 步 |
| `<名>\` | 单个工程的扁平日录：`<名>.csproj` + 源码 + 资源 | 流程第 4、5 步 |
| `jfglzs.slnx` | 一个 XML 解决方案装全部工程 | 手写维护（流程第 6 步） |

## 工具

| 工具 | 版本 | 用途 |
| --- | --- | --- |
| NETReactorSlayer（CLI 版） | 6.4.0 | 解 .NET Reactor 壳 |
| de4dot | 3.1.41592.3405（dnlib 3.3.2） | 收尾清理 |
| dnSpy（Console 版） | 6.6.0 及以上 | 导出 C# 工程 |
| ilspycmd（可选） | 10.1.0 | 单类型速看、列类型 |

## 流程

### 第 1 步 · 判定壳类型

命令：`de4dot -d samples\<名>.exe`

读输出里的 `Detected ...`：必须是 `.NET Reactor` 才走第 2 步；其它壳按对应 de4dot 类型另行处理。

### 第 2 步 · 解壳

命令：`NETReactorSlayer.CLI.exe samples\<名>.exe`

产出：样本同目录下的 `samples\<名>_Slayed.exe`。`--dec-methods`、`--dec-strings` 默认就是开的，不用额外带。

**这一步不能跳过**：de4dot 对这批样本解不开代码加密，直接用它跑，方法体会全空（见判定表）。

### 第 3 步 · de4dot 收尾

命令：`de4dot --dont-rename -f samples\<名>_Slayed.exe -o unpacked\<名>.exe`

- `--dont-rename` **必须带**。不带的话它会把 `ComparatorVisitor`、`advisorConfig` 这类名字洗成 `Class0`、`Method0`，可读性反而下降。
- 输出与输入同名不同目录，方便对照。

### 第 4 步 · 导出 C# 工程

命令：`dnSpy.Console.exe -l C# -o <名> unpacked\<名>.exe`

- `-l C#` 必须显式写（目标源码统一为 C#）。
- 不要加 `--sdk-project`：样本是老 .NET Framework 程序，老式 csproj 更贴合，且不需要还原 NuGet。
- 不加 `-r`（递归）；一次只导一个 assembly。
- dnSpy 会多套一层同名目录（`<名>\<名>\<名>.csproj`），照第 5 步抹平。

### 第 5 步 · 抹平工程目录

1. 把 `<名>\<名>\` 下的全部内容上提到 `<名>\`，删掉空的 `<名>\<名>\`。
2. 若上提后还剩 `<名>\<名>\g.resx` 这种同名子目录：把 `g.resx` 提到工程根，同时把它在 `<名>.csproj` 里
   `<EmbeddedResource Include="<名>\g.resx" />` 的路径改成 `g.resx`。
3. 目标形态：`<名>\<名>.csproj` 直接位于本目录下一层，工程根下只有 `Internal\`、`My\`、`Properties\`
   这类真实命名空间目录。
4. 删掉导出/试编译残留的 `obj\` 目录。

### 第 6 步 · 登记到解决方案

在 `jfglzs.slnx` 里加一行：`<Project Path="<名>/<名>.csproj" />`

验证：`dotnet sln jfglzs.slnx list`，列出的工程数要等于本目录下的工程目录数。

## 判定表

| 现象 | 含义 | 处理 |
| --- | --- | --- |
| de4dot 日志出现 `Could not deobfuscate method ... Hello, E.T.`、`A method was removed that is still referenced by this module` | 代码加密没解开，de4dot 清掉了方法 | 回第 2 步用 NETReactorSlayer，**不要**用纯 de4dot 的产物 |
| 反编译出来所有方法体都是空 `{ }`，多个方法 RVA 相同 | 同上，拿到的是空壳 | 同上 |
| 第 3 步输出与 `_Slayed.exe` 哈希相同 | de4dot 没真正重写 | 检查 `-f` / `-o` 路径是否指到了同一个文件 |
| 工程里只有 `Form1.Designer.cs`、没有 `Form1.cs` | 正常：`[DesignerGenerated]` 的窗体类整体写在 Designer 文件里 | 无需处理 |
| `dotnet build` 报 `MSB3644 找不到 .NETFramework,Version=v3.5 的引用程序集` | 该机器没装对应 .NET Framework 目标包 | 只看代码就不必编译；真要编译要么装目标包、要么改 TFM（本仓库刻意保留原始 TFM） |
| `dotnet sln jfglzs.slnx list` 少工程或报错 | slnx 里的相对路径写错 | 路径从本目录算起，用正斜杠，指向 `<名>/<名>.csproj` |
| 工程根下出现 `<名>\` 子目录，里面是 `g.resx` | dnSpy 按资源清单名建的目录，抹平时漏了 | 按第 5 步第 2 条上提并改 csproj |

## 检查清单（每加一个样本勾一遍）

- [ ] 第 1 步：确认是 .NET 且壳为 `.NET Reactor`
- [ ] 第 2 步：`<名>_Slayed.exe` 已生成，体积小于原样本
- [ ] 第 2 步：`dnSpy.Console.exe -t <某类型全名> <名>_Slayed.exe` 能看到带真实赋值/调用的构造函数
- [ ] 第 3 步：`unpacked\<名>.exe` 与 `_Slayed.exe` 哈希不同
- [ ] 第 4 步：`<名>.csproj` 存在，`.cs` 文件数 ≥ 类型数（嵌套类型会并进父类型文件）
- [ ] 第 4 步：抽查一个类型，方法体非空、有可读字符串
- [ ] 第 5 步：`<名>\<名>.csproj` 只隔一层；无同名子目录残留、无 `obj\`
- [ ] 第 6 步：`dotnet sln jfglzs.slnx list` 里能看到该工程

## 约定

- 原始样本目录内的文件一律不改、不删、不运行。
- 工程目录保持**扁平**：`<名>\<名>.csproj`，不允许 `<名>\<名>\...` 这种再套一层。
- 中间产物只写 `unpacked\`；重跑前先确认目标文件是不是自己上一次的产物。
- 新增工程后必须更新本文档「当前状态」表，并确认 `jfglzs.slnx` 能 `list` 通过。

## 函数反混淆注意事项

### 一、先分清哪些行是噪声

| 行 | 来源 | 处理 |
| --- | --- | --- |
| `IL_000A:` | 反编译器的跳转标签 | 删（删前全局搜有没有 `goto` 指向它） |
| `num2 = 3;`、`num = -2;` | VB 编译器给 `On Error` 编的状态号 | 删 |
| `ProjectData.ClearProjectError();` | `On Error` 区域开始标记 | 删 |
| `// Token: 0x0600004F RID: 79` | dnSpy 注释 | 删 |
| `@switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num);` | dnSpy 放弃结构化时输出的非法 C# | 不照抄，按语义改写 |
| `catch when (endfilter(...))` | 同上，VB 错误分发 | 按"出错跳回"改写 |
| `"字符串"` | 程序真实行为 | 原样保留（含转义、大小写） |
| 数字常量 | 端口 / 超时 / 偏移 | 原样保留 |
| 方法调用 | 程序真实行为 | 保留，顺序不能乱 |

### 二、`On Error Resume Next` 的还原

语义：每执行一条语句，出错就丢掉错误、继续下一条。

| 写法 | 对不对 | 后果 |
| --- | --- | --- |
| 每条语句各自 `try { } catch { }` | 对 | 与 VB 语义一致 |
| 整段套一个大 `try/catch` | 错 | 第一条出错后全部跳过 |
| 删掉 try/catch | 错 | 错误不再被吞，可能直接崩 |

识别方法：`ClearProjectError()` 开场 + `num2 = 2,3,4,6…` 递增 + 末尾按状态号分发的 `catch`。

连着的同类清理动作（如连续 `File.Delete`）合一个 try/catch 一般可接受，但要注释说明"原版逐条忽略错误"。

### 三、`goto` 的处理

| 形态 | 对应结构 | 处理 |
| --- | --- | --- |
| 向前跳过一段 | `if (条件) { … }` | 还原成 if |
| 循环末尾回跳 | `while` / `do-while` | dnSpy 多数已还原 |
| 跳到 `IL_10A2` 这类恢复点 | `On Error` 机制 | 随第二条一起删 |

还原不了就保留 `goto 标签;`（C# 合法）。硬改成错的结构比留 goto 更糟。

### 四、命名

- 方法名、成员名等沿用反编译出来的名字

- dnSpy 的局部名（`obj`、`obj2`、`text`、`text2`）按"第几个同类变量"编号，源码多加一个变量就全体位移

按字符串常量和调用的 API 起语义名，例：`Registry.LocalMachine` + `"SOFTWARE\...\RUN"` → `ReadRunKey`。

### 五、不许"美化"掉的语义

| 原写法 | 常见改写 | 为什么不行 |
| --- | --- | --- |
| `catch { }` | 删掉 try | 错误不再被吞 |
| `Operators.ConditionalCompareObjectEqual(a, "", false)` | `a == ""` | VB 弱类型比较，规则与 C# 不同 |
| `Conversions.ToString(x)` | `x.ToString()` | `null` 时 VB 返回空串、C# 抛异常 |
| `Strings.Mid(s, 1, n)` | `s.Substring(0, n)` | VB 1 基；`n` 为负时原版抛异常被吞，别顺手修正 |
| `Interaction.Shell(cmd, Hide, wait, -1)` | `Process.Start` | 等待语义、窗口样式不同 |
| `object` 变量比较 | 强转后比较 | 装箱比较 ≠ 值比较 |
| VB 默认 checked | 当作 C# 默认 unchecked | 溢出行为不同 |
| `using (OpenSubKey)` | 手动 Close | 释放时机变了 |
| 两条独立语句 | 调换顺序 | 有副作用时结果不同 |

### 六、其它

- `Form1.Designer.cs` 里包含整个 Form 类（含事件处理器），里面同样有 `goto`。
- 方法体外包 `checked { }` 时，整段都在 checked 上下文。
- 不要为"类型明确"把 `var` 改成具体类型。
- 改完不要求能编译，但要求语义逐条对得上：语句顺序、字符串、数字、忽略错误的范围。
