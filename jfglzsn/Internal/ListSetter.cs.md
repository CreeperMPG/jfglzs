## 文件作用

**壳运行时核心**（.NET Reactor / IntelliLock 的保护代码），本工程里技术含量最高、也最值得分析的文件。可分成五块：

1. **自实现 MD5**（完整手写主循环 + 四个轮函数 + 循环左移 + 64 项 T 表）——因为 FIPS 策略下系统 MD5 被禁用，壳自带一份；
2. **对称加密封装**（按 FIPS 标志选 AES/Rijndael；硬编码 Key/IV 解密内嵌资源；固定 Key + 口令派生 IV 加密配置）；
3. **字符串解密器**（`ReadProcessMemory` 从**自身模块内存**按偏移读密文再 AES 解密，带缓存）；
4. **运行时自补丁**（从内嵌资源取补丁表，`VirtualProtect` + `WriteProcessMemory` 把原始数据写回自身代码段——反脱壳/反篡改）；
5. **一堆死桩**（6 个恒 `null`、6 个恒 `{1,2}`、2 个恒真/假判定、1 个空方法、1 个空调试方法）。

**关键结论：除 `计算MD5` 外，本类所有保护相关方法在本工程内都没有调用点**——脱壳阶段已把调用点替换成明文，这段壳代码只是残骸，但它是理解样本来源的直接证据。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ListSetter` | 壳运行时容器（MD5 / AES / 字符串解密 / 自补丁） |
| `自实现MD5`（原 `SetupEvent`） | 手写 MD5：补位 → 分组 → 64 步四轮 → 输出 16 字节摘要 |
| `自实现MD5_轮函数F/G/H/I`（原 `CheckEvent`/`ReadEvent`/`ValidateEvent`/`AwakeEvent`） | MD5 四个轮步函数 |
| `循环左移`（原 `CalcEvent`） | 32 位 ROTL |
| `是否仅FIPS算法`（原 `MoveEvent`） | FIPS 策略查询（懒加载） |
| `探测FIPS策略`（原 `CustomizeEvent`） | 反射读 `CryptoConfig.AllowOnlyFipsAlgorithms` |
| `创建对称算法`（原 `LogoutEvent`） | 按 FIPS 标志返回 `AesCryptoServiceProvider` 或 `RijndaelManaged` |
| `计算MD5`（原 `InterruptEvent`） | MD5 分发：非 FIPS 用系统实现，FIPS 用自实现 |
| `解密字符串`（原 `ResolveEvent`） | 字符串解密（自身内存读取 + AES + 自定义滚动 XOR） |
| `Base64转字符串`（原 `PublishEvent`） | Base64 → UTF-16 字符串 |
| `运行时还原方法体`（原 `PrepareEvent`） | 从内嵌资源取补丁，改回自身代码段（反篡改） |
| `取程序集路径`（原 `ConcatEvent`） | 三种手段取程序集磁盘路径 |
| `读整个文件`（原 `CancelEvent`） | 整文件读入内存（无调用点） |
| `AES解密`（原 `RunEvent`） | 硬编码 Key/IV 的 AES 解密 |
| `AES加密`（原 `UpdateEvent`） | 固定 Key + `MD5(口令)` 作 IV 的 AES 加密，输出 Base64 |
| `DeleteEvent`/`LoginEvent`/`FindEvent`/`StopEvent`/`InsertEvent`/`PostEvent` | 6 个恒返回 `null` 的 byte[] 桩 |
| `InitEvent`/`CountEvent`/`CloneEvent`/`ExcludeEvent`/`StartEvent`/`VisitEvent` | 6 个 IntelliLock 授权数据桩（恒返回 `{1,2}` + 一个 GUID 诱饵串） |
| `视图规则桩`/`登录规则桩`（原 `ViewRule`/`LoginRule`） | 恒 `true` / `false` |
| `空方法桩`（原 `method_0`） | 空方法体 |
| `BaseSetter : Attribute` | 壳标记属性（只被 `运行时还原方法体` 自己标了一次） |
| `ParamsSetter<T>` | 仅用于让上面的 `typeof(...[])` 有类型可写的空泛型壳 |
| `WriterSetter`（`[Flags]` enum） | 空枚举，无成员、无使用 |

## 反混淆命名映射

### 方法

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `method_0()` | `空方法桩()` | dnSpy 占位名（第 0 个方法），方法体为空 |
| `SetupEvent(object)` | `自实现MD5(byte[] 数据)` | 手写 MD5 主循环；参数原为 VB `Object`（后期绑定字节数组），按实际用法改为 `byte[]` |
| `CheckEvent` | `自实现MD5_轮函数F` | 轮 1：`(b&c)|(~b&d)`，位移 7/12/17/22 |
| `ReadEvent` | `自实现MD5_轮函数G` | 轮 2：`(b&d)|(c&~d)`，位移 5/9/14/20 |
| `ValidateEvent` | `自实现MD5_轮函数H` | 轮 3：`b^c^d`，位移 4/11/16/23 |
| `AwakeEvent` | `自实现MD5_轮函数I` | 轮 4：`c^(b|~d)`，位移 6/10/15/21 |
| `CalcEvent` | `循环左移` | ROTL（MD5 的 `<←<s`） |
| `MoveEvent` | `是否仅FIPS算法` | 返回 FIPS 策略标志 |
| `CustomizeEvent` | `探测FIPS策略` | 反射读 `CryptoConfig.AllowOnlyFipsAlgorithms` |
| `LogoutEvent` | `创建对称算法` | 返回未初始化的 AES/Rijndael 实例 |
| `InterruptEvent` | `计算MD5` | FIPS 感知的 MD5 分发（**唯一被外部调用的方法**） |
| `ResolveEvent(int)` | `解密字符串(int 字符串序号)` | 壳的字符串解密 + 缓存 |
| `PublishEvent` | `Base64转字符串` | Base64 → Unicode 字符串 |
| `PrepareEvent` | `运行时还原方法体` | 运行时自补丁（反篡改） |
| `ConcatEvent` | `取程序集路径` | Location → CodeBase → 反射 |
| `CancelEvent` | `读整个文件` | 整文件读入 |
| `RunEvent` | `AES解密` | 硬编码 Key/IV 解密 |
| `UpdateEvent` | `AES加密` | 固定 Key + MD5(口令) 作 IV |
| `ViewRule` / `LoginRule` | `视图规则桩` / `登录规则桩` | 恒真/恒假 |
| `WriteProcessMemory` 等 P/Invoke | 参数名 `intptr_0`/`byte_0`/`uint_0` → 标准 Win32 名（`lpBaseAddress`/`lpBuffer`/`nSize` 等） | 便于对照 MSDN |

### 字段

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `m_InterpreterSetter` | `FIPS仅允许标志` | FIPS 策略缓存值 |
| `_InfoSetter` | `解密数据表` | 字符串偏移表（解密后的数据） |
| `_ValueSetter` | `本进程句柄` | `PROCESS_VM_READ` 句柄（惰性打开、缓存） |
| `m_FilterSetter` | `模块基址32` | `HINSTANCE.ToInt32()` |
| `indexerSetter` | `模块基址64` | `HINSTANCE.ToInt64()` |
| `dicSetter` | `字符串缓存表` | 已解密字符串缓存 |
| `codeSetter` | `缓存槽索引` | 偏移表下标 → 缓存槽号（0 = 未缓存） |
| `_FieldSetter` | `已执行还原标志` | 自补丁幂等标志 |
| `_ItemSetter` | `已探测FIPS标志` | FIPS 探测幂等标志 |
| `_ObserverSetter` | `读写字数` | `Read/WriteProcessMemory` 的输出参数占位 |
| `m_ProcessSetter` | `下一个缓存槽` | 初始 1（0 号槽留空表示未缓存） |
| `m_FacadeSetter` | `MD5常量表` | MD5 的 64 项 T 表 |
| `m_InstanceSetter` / `_ClassSetter` / `_ClientSetter` | 保留原名 | 三个 `new byte[0]` 字段，**未被任何方法使用**，无语义可推断 |

### 大量「诱饵赋值」的处理（重要）

`解密字符串` 与 `运行时还原方法体` 里，原版对 `array2[0..31]` / `array4[0..15]` 的每个元素写了 **3~7 次**（例如 `array2[0] = 92; array2[0] = 217; array2[0] = 117;`），这是混淆器塞的**死存储**，只有最后一次生效。

处理方式：**每个下标只保留最后一次赋值的最终值**，写成规整的 `new byte[] { … }` 初始化器，并在方法注释里写明"原版每个下标被反复赋值，此处仅保留最后一次生效的值"。数组内容与运行时**完全一致**；其余语句（顺序、流控、内存读取、异常处理）逐条保留，未做任何精简。

## 关键方法与流程

### `自实现MD5(数据)` — 手写 MD5

1. **补位**：`补位比特 = 448 - 数据.Length*8 % 512`；`补位字节 = (补位比特+512) % 512`，为 0 则取 512；总长度 = 原文 + 补位字节 + 8。
2. 复制原文到 `缓冲区`，取**别名** `消息块视图 = 缓冲区`（VB 里就是同一个对象），`消息块视图[数据长度] |= 128`。
3. 末尾 8 字节写入原始比特长度（小端）。
4. `分组数 = 缓冲区.Length * 8 / 32`。
5. 初始状态 `A=1732584193(0x67452301)`、`B=4023233417(0xEFCDAB89)`、`C=2562383102(0x98BADCFE)`、`D=271733878(0x10325476)`。
6. 按 64 字节一组：把 16 个字按小端搬进 `消息块`（**内层循环上界写的是 `num13 < 61`、步长 4，正好 16 次**；改成 `< 64` 会变 17 次并崩），保存 A~D，跑 64 步（F×16 / G×16 / H×16 / I×16，`ref` 轮转顺序原样保留），最后把 A~D 累加回初值。
7. 输出 16 字节：A、B、C、D 各 4 字节小端拼接。

T 表（`MD5常量表`）就是标准 `floor(2^32 × |sin(i+1)|)`，原版以 64 个十进制字面量硬编码。

### `计算MD5(数据)` — MD5 分发

```
if (!是否仅FIPS算法()) return new MD5CryptoServiceProvider().ComputeHash(数据);
return 自实现MD5(数据);
```

**为什么要有自实现**：Windows 的 FIPS 策略（`CryptoConfig.AllowOnlyFipsAlgorithms = true`）会禁用 MD5，`MD5CryptoServiceProvider` 直接抛异常，壳因此自带一份。两条路径输出一致。

### `是否仅FIPS算法()` / `探测FIPS策略()`

- 首次调用：反射 `typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig").GetMethod("get_AllowOnlyFipsAlgorithms").Invoke(null, empty)`；任何异常（老框架无此属性）被吞掉，标志保持 `false`。
- 之后返回缓存标志。

### `创建对称算法()`

```
FIPS 模式：try { return new AesCryptoServiceProvider(); } catch { return new RijndaelManaged(); }
非 FIPS：  return new RijndaelManaged();
```

不改任何 Key/IV/Mode 属性（由调用方设）。

### `解密字符串(字符串序号)` — 字符串解密器

**首次调用（序号 = -1）**：

1. 读内嵌资源 `2163406f-f304-4b64-8350-1fe705a394ba` 全部字节。
2. 取硬编码 32 字节 `密钥` 与 16 字节 `初始化向量`（见 IOC 表）。
3. 若程序集有 `PublicKeyToken`（8 字节），用其覆盖 `初始化向量` 的奇数位 `[1][3][5][7][9][11][13][15]`——**本样本非强签名，此分支不生效**。
4. `密钥[i] ^= 初始化向量[i]`（前 16 字节）。
5. `AES-CBC` 解密资源 → `解密数据表`，并分配 `字符串缓存表` / `缓存槽索引`（长度 = `解密数据表.Length/4 + 1`）。
6. `密文数据 = 解密数据表`。
7. **自定义滚动 XOR 还原**：按 4 字节块，`密钥字` 从 `密钥` 循环取，`累计 += 密钥字`，再过一个固定常数链的非线性混合器（`^>>25`、`+1394801171`、`^<<5`、`+3457341307`、`^>>3`、`+4040842541`、`+2018246656`），`解出 = 累计 ^ 明文字`；末块不足 4 字节时从尾部倒着拼。
8. 重新分配缓存数组；取模块 `HINSTANCE` 存进 `模块基址32` / `模块基址64`。

**每次调用**：

1. `缓存槽 = 字符串序号 / 4`；`缓存槽索引[缓存槽] > 0` → 直接返回缓存串。
2. `记录偏移 = BitConverter.ToInt32(解密数据表, 字符串序号)`。
3. 首次需要时 `OpenProcess(0x10 = PROCESS_VM_READ, 1, 自身 PID)` 并缓存句柄。
4. `ReadProcessMemory(模块基址 + 记录偏移, 4 字节)` → `密文长度`。
5. `ReadProcessMemory(模块基址 + 记录偏移 + 4, 密文长度)` → 密文。
6. `AES解密(密文)` → `Encoding.Unicode.GetString(...)`。
7. 写入缓存并返回。

**本质**：字符串密文就藏在自己的 PE 映像里，用"读自己进程内存"的方式取出来——这是 .NET Reactor 的字符串加密方案。本工程内**无调用点**（字符串已被前置阶段解出并内联）。

### `运行时还原方法体()` — 反篡改自补丁

1. `已执行还原标志` 为真则返回；**先**置真。
2. 读内嵌资源 `c2a928a7-eb8e-4050-b410-49f5dde0edce`。
3. 取硬编码 32 字节 `密钥` / 16 字节 `初始化向量`，**先 `Array.Reverse(初始化向量)`**（与 `解密字符串` 的唯一区别，因为 `array5` 是 `array4` 的别名，反转即作用于 IV 本身）。
4. `PublicKeyToken` 覆盖奇数位后**立即 `Array.Clear` 清零**（`解密字符串` 里没有这一步）。
5. `密钥[i] ^= 初始化向量[i]`（前 16 字节）。
6. 同样的滚动 XOR 还原（**多一句原版自赋值 `num4 = num4;`，已保留并注释**）→ `补丁数据`。
7. `Array.Clear(初始化向量)`；关闭资源读取器，把 `补丁数据` 重新包成 `BinaryReader`。
8. `OpenProcess(0x38 = VM_OPERATION|VM_WRITE|VM_READ, 1, 自身 PID)`；取 `模块基址64`。
9. 读 `条目数`、跳过一个 int，然后**第一段**：逐条 `VirtualProtect(基址+偏移, 4, PAGE_READWRITE)` → `WriteProcessMemory` 写 4 字节 → 恢复原保护属性。
10. **第二段**（`try/catch { }`）：`while (Position < Length-1)` 读 `块偏移` + `块字数`，`VirtualProtect` 放开后逐字 `Marshal.WriteInt32` 写入，再恢复保护；循环结束 `CloseHandle`。

**含义**：磁盘上的方法体是被人改过的，靠这段代码在运行期改回来。静态反编译看到的差异正源于此。**本工程内无调用点**（原由模块初始化器触发）。

### `AES解密(密文)`

`创建对称算法()` → 设硬编码 32 字节 Key（AES-256）与 16 字节 IV → `CreateDecryptor()` → `CryptoStream` 写入 → `Close()`（**没有显式 `FlushFinalBlock`**，靠 Close 收尾，与原版一致）→ 返回明文字节。

### `AES加密(明文, 口令)`

`明文字节 = Encoding.Unicode.GetBytes(明文)`；Key 是硬编码 32 字节；**IV = `计算MD5(Encoding.Unicode.GetBytes(口令))`**（16 字节，正好是 AES 块长）→ `CreateEncryptor()` → Base64 输出。

即「固定密钥 + 口令派生 IV」：同一口令可解、不同口令解出乱码。本工程内无调用点（原本用于生成注册码 / 加密配置）。

### `取程序集路径(程序集)`

三种手段依次尝试，都失败返回 `""`：

1. `Assembly.Location` 存在 → 返回；
2. `Assembly.GetName().CodeBase` 去掉 `file:///` 前缀后存在 → 返回；
3. 反射读 `GetType().GetProperty("Location")` 的值（应对被替换的 Assembly 子类）。

每段各有一个空 `catch { }`（动态程序集没有 Location，必然抛）。

### 6 + 6 个桩

| 组 | 行为 |
| --- | --- |
| `DeleteEvent`/`LoginEvent`/`FindEvent`/`StopEvent`/`InsertEvent`/`PostEvent` | `return null` |
| `InitEvent`/`CountEvent`/`CloneEvent`/`ExcludeEvent`/`StartEvent`/`VisitEvent` | 先放一个 `{11111-22222-40001-00001}` 形状的许可标识字符串（6 个各不相同，见 IOC），再**恒返回 `{1,2}`**；`if (许可标识.Length > 0)` 恒真，两句 return 完全一样，是纯混淆结构 |

## 恶意行为 / 敏感操作

| 行为 | 说明 | 风险 |
| --- | --- | --- |
| **运行时改自身代码段** | `VirtualProtect` + `WriteProcessMemory` + `Marshal.WriteInt32` 往模块基址写补丁 | **高**（反脱壳/反篡改，典型加壳特征） |
| **读自身进程内存** | `OpenProcess(PROCESS_VM_READ)` + `ReadProcessMemory` 取字符串密文 | 中（隐蔽字符串） |
| `OpenProcess(0x38, ...)` | 拿 VM_OPERATION / VM_WRITE / VM_READ 权限 | 高（可写任意进程内存，此处只写自己） |
| 内嵌资源解密 | 两个 GUID 命名的资源（字符串表 / 补丁表） | 高（隐蔽载荷） |
| FIPS 绕过 | 系统禁 MD5 时改用自实现 | 中（规避安全策略） |
| 反分析 | 反射读 FIPS 属性、三种方式取程序集路径、`PublicKeyToken` 参与密钥派生 | 中 |

> 这些能力在本工程里**全部处于"装配好但未接线"状态**（无调用点），说明脱壳后保护层已被剥掉；但代码本身证明了样本的加壳方式与技术档次。

## IOC / 硬编码

### 内嵌资源名（GUID，稳定特征）

| 资源名 | 用途 |
| --- | --- |
| `2163406f-f304-4b64-8350-1fe705a394ba` | 字符串密文表（`解密字符串`） |
| `c2a928a7-eb8e-4050-b410-49f5dde0edce` | 自补丁数据表（`运行时还原方法体`） |

### 硬编码密钥

| 位置 | 值 |
| --- | --- |
| `解密字符串` 密钥（32 B） | `117,180,112,108,225,207,9,250,37,97,48,84,43,186,145,87,1,2,35,222,128,59,228,136,249,57,180,25,186,149,137,44` |
| `解密字符串` IV（16 B） | `63,156,138,34,156,109,15,17,21,132,121,206,47,105,113,129` |
| `运行时还原方法体` 密钥（32 B） | `198,13,105,42,35,239,245,150,82,115,174,92,154,123,74,25,46,154,51,77,106,129,235,106,198,101,109,126,181,74,74,138` |
| `运行时还原方法体` IV（16 B，先整体反转） | `68,84,96,184,123,242,197,131,0,78,33,213,211,12,105,111` |
| `AES解密` Key（32 B） | `43,141,39,235,62,227,120,163,228,147,184,192,30,19,73,36,5,188,199,56,130,178,139,87,75,159,37,245,127,175,121,34` |
| `AES解密` IV（16 B） | `84,233,235,135,93,125,119,114,87,33,60,75,99,97,0,134` |
| `AES加密` 固定 Key（32 B） | `82,102,104,110,32,77,24,34,118,181,51,17,18,51,12,109,10,32,77,24,34,158,161,41,97,28,118,181,5,25,1,88` |
| `AES加密` IV | `MD5(UTF16LE(口令))` |

### 其它常量

| 类型 | 值 |
| --- | --- |
| 滚动 XOR 混合常数 | `1394801171`、`3457341307`、`4040842541`、`2018246656`；`^>>25`、`^<<5`、`^>>3` |
| MD5 初值 | `1732584193`、`4023233417`、`2562383102`、`271733878` |
| MD5 字块循环上界 | `61`（步长 4，正好 16 次，**不可改**） |
| 授权桩 GUID 串 | `{11111-22222-40001-00001}`、`{11111-22222-40001-00002}`、`{11111-22222-50001-00001}`、`{11111-22222-50001-00002}`、`{11111-22222-60001-00001}`、`{11111-22222-60001-00002}` |
| P/Invoke DLL | `kernel32.dll`（`VirtualProtect`、`WriteProcessMemory`、`ReadProcessMemory`、`OpenProcess`、`CloseHandle`） |
| 进程权限 | `0x10`(VM_READ)、`0x38`(VM_OP|VM_WRITE|VM_READ) |
| 页属性 | `4` = PAGE_READWRITE |
| 反射目标 | `System.Security.Cryptography.CryptoConfig` → `get_AllowOnlyFipsAlgorithms` |
