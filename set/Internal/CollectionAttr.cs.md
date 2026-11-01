# Internal\CollectionAttr.cs —— 文件级分析

## 文件作用
.NET Reactor 运行期组件集合，也是全工程最长的一个文件（1367 行）。两块核心：
1. **密码学**：一份自实现的 MD5（`自实现MD5` + 4 个轮函数 + 循环左移 + 64 项常量表），以及按 FIPS 策略在「系统 MD5 / 自实现 MD5」和「AesCryptoServiceProvider / RijndaelManaged」之间切换的开关逻辑；
2. **Reactor 资源解密链**：`ExcludeAttr` 从清单资源里读加密数据并 AES 解密，`PublishAttr`/`ResolveAttr`/`DefineAttr`/`UpdateAttr` 等外壳代码，配合 P/Invoke（`VirtualProtect`/`WriteProcessMemory`/`ReadProcessMemory`/`OpenProcess`/`CloseHandle`）。

`计算摘要()` 的输出就是 `CandidateCandidate` 拼内嵌程序集资源名用的那段 16 字节 MD5。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `CollectionAttr` | Reactor 运行期工具类（全部成员都是 static 或私有实例方法） |
| `自实现MD5`（旧名 `ManageInfo`） | **自实现 MD5 主循环**：补齐、16 字拆分、64 步、链值累加、16 字节摘要 |
| `MD5轮函数F/G/H/I`（旧名 `IncludeInfo/AddInfo/CalcInfo/VerifyInfo`） | MD5 四轮轮函数 |
| `循环左移`（旧名 `FillInfo`） | 32 位循环左移 |
| `MD5常量表`（旧名 `_ThreadAttr`） | MD5 的 64 项 T 常量（sin 表） |
| `是否FIPS模式` / `读取FIPS开关` / `仅FIPS算法` / `已读取FIPS开关` | 反射读 `CryptoConfig.AllowOnlyFipsAlgorithms` 并缓存 |
| `新建对称算法`（旧名 `SetupAttr`） | FIPS→AesCryptoServiceProvider / 否则 RijndaelManaged |
| `计算摘要`（旧名 `SetAttr`） | FIPS→自实现 MD5 / 否则系统 MD5 |
| `ExcludeAttr` | **AES 解密内嵌资源**（资源名 `c4a6c669-082b-46ec-be30-7d414450bf0b`） |
| `PublishAttr` / `ResolveAttr` / `DefineAttr` / `UpdateAttr` / `QueryAttr` | Reactor 的内存补丁与资源解码辅助 |
| 12 个 `byte[] XxxAttr()`（`CollectAttr`…`SearchAttr`） | 常量/资源分片读取器，互不调用、无引用 |
| `PublishFactory` / `ViewFactory` | 恒值死代码桩 |
| P/Invoke 组 | `VirtualProtect` / `WriteProcessMemory` / `ReadProcessMemory` / `OpenProcess` / `CloseHandle` |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `method_0()` | `空方法()` | 空方法体 |
| `ManageInfo(object)` | `自实现MD5(object)` | 参数实际是 `byte[]` |
| 局部 `array` / `array2` / `array3` / `array4` | `X` / `缓冲` / `缓冲别名` / `摘要` | MD5 的 16 字块、补齐缓冲、输出 |
| 局部 `num,num2,…,num17` | `补齐目标位`/`补齐字节数`/`总字节数`/`原始长度位`/`原长度`/`块数`/`A`,`B`,`C`,`D`/`块`/`块偏移`/`偏移`/`AA`,`BB`,`CC`,`DD` | |
| `IncludeInfo(ref uint,…)` | `MD5轮函数F(ref uint,…)` | `(b & c) \| (~b & d)` |
| `AddInfo` | `MD5轮函数G` | `(b & d) \| (c & ~d)` |
| `CalcInfo` | `MD5轮函数H` | `b ^ c ^ d` |
| `VerifyInfo` | `MD5轮函数I` | `c ^ (b \| ~d)` |
| `FillInfo(uint,ushort)` | `循环左移(uint,ushort)` | |
| 参数 `uint_0…uint_5`,`ushort_0`,`object_0` | `a,b,c,d,k,s,i,X` | 链值、消息字下标、移位、常量表下标、消息块 |
| `RevertInfo()` | `是否FIPS模式()` | |
| `InterruptAttr()` | `读取FIPS开关()` | |
| `SetupAttr()` | `新建对称算法()` | |
| `SetAttr(byte[])` | `计算摘要(byte[])` | 被 `CandidateCandidate` 调用，已同步改名 |
| 字段 `m_RefAttr` | `已读取FIPS开关` | |
| 字段 `fieldAttr` | `仅FIPS算法` | |
| 字段 `_ThreadAttr` | `MD5常量表` | 64 项 uint 常量 |
| `ExcludeAttr` / `PublishAttr` / `ResolveAttr` / `DefineAttr` / `UpdateAttr` / `QueryAttr` | 未改名 | 名字不透明但语义不可静态确定，靠 XML 注释说明 |
| 12 个 `XxxAttr()` | 未改名 | 无引用的常量读取器 |

## 关键方法与流程（按方法分段，写清步骤）

### `自实现MD5(输入)`（旧 `ManageInfo`）
1. 计算补齐：`补齐目标位 = 448 - len*8 % 512`；`补齐字节数 = (补齐目标位+512) % 512`，为 0 时取 512；总长度 = 原长 + 补齐字节 + 8。
2. 复制原数据到缓冲，`缓冲[len] |= 0x80`，末尾 8 字节写入**原始长度（位，小端）**。
3. 初始化 `A=0x67452301, B=0xEFCDAB89, C=0x98BADCFE, D=0x10325476`。
4. 对每个 512 位块：把 16 个字节组按小端拼成 16 个 `uint`（X），保存 `AA/BB/CC/DD`。
5. 依次调用 64 步：F 轮 16 步（移位 7/12/17/22）→ G 轮 16 步（5/9/14/20）→ H 轮 16 步（4/11/16/23）→ I 轮 16 步（6/10/15/21），每步都带 `MD5常量表[i-1]`。
6. 64 步后 `A+=AA; B+=BB; C+=CC; D+=DD`。
7. 输出：`BitConverter.GetBytes(A/B/C/D)` 依次拷进 16 字节数组返回。

### `MD5轮函数F/G/H/I`
统一形式：`a = b + 循环左移(a + f(b,c,d) + X[k] + T[i-1], s)`，四轮只差 `f` 的定义。

### `循环左移`
`(v >> (32-s)) | (v << s)`。

### `是否FIPS模式` / `读取FIPS开关`
第一次调用时反射读 `CryptoConfig.AllowOnlyFipsAlgorithms`（`typeof(RijndaelManaged).Assembly.GetType(...).GetMethod("get_AllowOnlyFipsAlgorithms")`），失败保持 false，之后只读缓存。

### `新建对称算法` / `计算摘要`
- 对称算法：FIPS 模式优先 `AesCryptoServiceProvider`（异常退回 `RijndaelManaged`），非 FIPS 直接用 `RijndaelManaged`；
- 摘要：FIPS 模式走 `自实现MD5`，否则 `MD5CryptoServiceProvider().ComputeHash`。两条路径输出都是 16 字节 MD5。

### `ExcludeAttr(int)`
从 `typeof(CollectionAttr).Assembly.GetManifestResourceStream("c4a6c669-082b-46ec-be30-7d414450bf0b")` 读整段资源，按固定密钥/IV 用 `新建对称算法()` 建出的算法解密，返回字符串。工程内无调用点。

### `PublishAttr()` / `ResolveAttr()` / `DefineAttr()` / `UpdateAttr()` / `QueryAttr()`
Reactor 的运行时外壳：内存补丁（`VirtualProtect`+`WriteProcessMemory`）、从已加载程序集取宿主对象、字符串↔字节转换、资源键值读取。工程内均无调用点，方法体保持原样。

## 恶意行为 / 敏感操作
- 本文件**不直接**做破坏行为，但它是「让加壳样本能跑起来」的关键：资源解密 + 内嵌程序集摘要算法。
- 敏感点：
  - **内存补丁 API**：`VirtualProtect` / `WriteProcessMemory` / `ReadProcessMemory` / `OpenProcess` —— 典型的内存改写/注入能力面（本样本内无调用点）。
  - **加密常量与算法**：盐 `bfdshgs` 不在这里（在 `ObserverAttr`），但 MD5 实现与资源名算法在这里，复现它就能枚举内嵌的加密资源。
  - **FIPS 规避**：显式检测系统是否只允许 FIPS 算法，并准备了一份自实现 MD5 绕过它 —— 反检测意识。
- 无文件写入、无注册表、无网络调用。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 资源名：`c4a6c669-082b-46ec-be30-7d414450bf0b`
- 反射目标：`System.Security.Cryptography.CryptoConfig`、`get_AllowOnlyFipsAlgorithms`
- MD5 初值：`1732584193`(0x67452301) / `4023233417`(0xEFCDAB89) / `2562383102`(0x98BADCFE) / `271733878`(0x10325476)
- 补齐魔数：`128`(0x80)、`448`、`512`、`255`、`61`
- P/Invoke：`kernel32`（`VirtualProtect`/`WriteProcessMemory`/`ReadProcessMemory`/`OpenProcess`/`CloseHandle`）

## 反混淆处理记录
- goto：0 → 0（本文件原本就没有 goto / `IL_xxxx:` / `@switch` / `endfilter`）
- 已完整重写：`自实现MD5`（含 64 步调用，逐步保留参数）、4 个轮函数、循环左移、FIPS 开关三件套、`新建对称算法`、`计算摘要`
- 已加 XML 注释但**方法体未改**：`ExcludeAttr`（约 390 行 AES 解密，逐字节常量数组）、`PublishAttr`（约 400 行）、`ResolveAttr`、`DefineAttr`、`UpdateAttr`、`QueryAttr`、12 个 `byte[] XxxAttr()`、2 个恒值桩
  —— 这些方法内部全是密集的数字常量与 `IL` 级位运算，且在本工程内**没有调用点**；按「能读懂职责 + 不改语义」的原则只做命名说明，没有重排其实现，避免引入语义偏差
- 跨文件改名联动：`CandidateCandidate.解析内嵌程序集` 里的 `CollectionAttr.SetAttr(...)` → `CollectionAttr.计算摘要(...)`
- 语义保留点：`(UIntPtr)` 强转、`checked/unchecked`、`BitConverter.GetBytes` 顺序、`typeof(RijndaelManaged).Assembly` 反射写法、字符串常量，全部原样
