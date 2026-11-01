## 文件作用

本工程里体积最大、也最「不像业务」的文件：它是 **IntelliLock（EZIRIZ）加壳器的运行时保护库**，
被注入到 `zmserv` 程序集里，负责「字符串加密 → 运行时解密」「方法体加密 → 运行时写回内存」
「内嵌 DLL 的散列式资源名」这几件事。

结论先说：
- 这里面**没有一条**对「机房管理助手」业务有用的代码；
- 但它是解释「为什么直接反编译看到的是乱码字符串 / 为什么有些方法体在静态视图里是加密的」的钥匙；
- 也正因为壳是 IntelliLock 的**未注册试用版**，程序里还残留了 `CustomerStrategy` 那条「unregistered version」提示（见 `CustomerStrategy.cs.md`）。

注意：原文 1335 行里有约 900 行是「同一个数组下标被连续赋值 4~7 次」的诱饵写入，
反混淆时按「最终生效值」重写成了标准数组初始化表达式，数组内容与原版逐字节等价（详见下文「关键方法与流程」）。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `IntelliLock运行时`（原 `AdvisorStrategy`） | 壳的运行时总入口：MD5/AES 工具箱、字符串解密、IL 补丁写回、一堆授权空桩 |
| ├ `自实现MD5` | 完整 MD5（补位 + 4 轮 64 步 + 摘要），只在系统开启 FIPS 限制时启用 |
| ├ `MD5轮函数F/G/H/I` | MD5 的四个轮函数 |
| ├ `循环左移` | 32 位 ROL，轮函数里用 |
| ├ `是否仅允许FIPS算法` / `读取FIPS限制开关` / `创建对称加密算法` / `计算MD5` | 按系统加密策略选择 BCL 实现或自实现 |
| ├ `解密字符串` | **字符串解密核心**：AES-CBC + 自制流密码，再按偏移从本进程内存读字符串 |
| ├ `运行时还原方法体` | **代码还原核心**：把加密 IL 补丁用 VirtualProtect + WriteProcessMemory 写回自己 |
| ├ `AES解密` / `AES加密为Base64` / `Base64转Unicode字符串` | 三条独立的加解密路径（前两条各带一套写死的 Key/IV） |
| ├ `获取程序集路径` / `读取文件字节` | 工具方法，壳里用，业务无调用点 |
| ├ `VirtualProtect` / `ReadProcessMemory` / `WriteProcessMemory` / `OpenProcess` / `CloseHandle` | kernel32 原生导入，全部是为了「读写自己进程的内存」 |
| ├ `CreateParams`…`PopParams`、`CloneParams`…`PrintParams` | 授权校验空桩（返回 `{1,2}` 或 `null`，无调用点） |
| └ `模块初始化标记`（原 `ErrorStrategy`）+ `标记占位<T>` | 自定义 Attribute 与泛型占位类型，用来标记「模块初始化点」 |
| `LabelPositions` | 空的 `[Flags]` 枚举，纯残留 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `AdvisorStrategy` | `IntelliLock运行时` | 整个保护库 |
| `method_0()` | `空方法体()` | 空的私有实例方法，从未被调用 |
| `CancelParams(byte[])` | `自实现MD5(消息)` | MD5 主流程 |
| `ConcatParams` | `MD5轮函数F` | `(b & c) | (~b & d)` |
| `UpdateParams` | `MD5轮函数G` | `(b & d) | (c & ~d)` |
| `ReadParams` | `MD5轮函数H` | `b ^ c ^ d` |
| `RunParams` | `MD5轮函数I` | `c ^ (b | ~d)` |
| `EnableParams` | `循环左移` | `(x >> (32-n)) | (x << n)` |
| `ChangeParams()` | `是否仅允许FIPS算法()` | 返回 `CryptoConfig.AllowOnlyFipsAlgorithms`，惰性缓存 |
| `DisableParams()` | `读取FIPS限制开关()` | 反射读那个属性，失败吞异常 |
| `InstantiateParams()` | `创建对称加密算法()` | FIPS → `AesCryptoServiceProvider`，否则 `RijndaelManaged` |
| `RateParams(byte[])` | `计算MD5(数据)` | 统一 MD5 入口（BCL 或自实现） |
| `ResetParams(int)` | `解密字符串(字符串表偏移)` | 字符串解密 + 缓存 |
| `CompareParams(string)` | `Base64转Unicode字符串(内容)` | Base64 → Unicode 文本 |
| `LoginParams()` | `运行时还原方法体()` | 模块初始化器，写 IL 回内存 |
| `SetParams(Assembly)` | `获取程序集路径(目标程序集)` | 三段式找物理路径 |
| `InitParams(string)` | `读取文件字节(文件路径)` | 循环读满整个文件 |
| `OrderParams(byte[])` | `AES解密(密文)` | 固定 Key/IV 的 AES 解密 |
| `VisitParams(string,string)` | `AES加密为Base64(明文, 密钥源)` | Key 写死，IV = MD5(密钥源) |
| `ErrorStrategy` | `模块初始化标记` | 自定义 Attribute |
| `ItemStrategy<T>` | `标记占位<T>` | 泛型占位类型 |
| `proxyStrategy` | `MD5常量表` | 64 个 `T[i] = floor(2^32·|sin(i)|)` |
| `m_IdentifierStrategy` | `已检查FIPS` | 惰性求值标志 |
| `serializerStrategy` / `_DispatcherStrategy` / `_ValStrategy` | `未使用字段1/2/3` | 只在声明处 `new byte[0]`，全类不读写 |
| `publisherStrategy` | `已读字节数` | `ReadProcessMemory` 的 out 参数接收器 |
| `systemStrategy` | `偏移到缓存槽` | 偏移 → 缓存槽号的 int[] |
| `m_CallbackStrategy` | `方法体已还原` | 只还原一次的标志 |
| `m_InfoStrategy` | `字符串缓存` | 已解出的字符串，槽位从 1 开始 |
| `_ConfigStrategy` | `FIPS开关值` | `AllowOnlyFipsAlgorithms` 的值 |
| `stateStrategy` | `模块基址` | 本模块内存基址（64 位） |
| `m_CandidateStrategy` | `模块基址32位` | 同上，32 位进程用 |
| `_IndexerStrategy` | `下一个缓存槽位` | 从 1 开始递增 |
| `objectStrategy` | `本进程句柄` | `OpenProcess` 得来 |
| `_MerchantStrategy` | `字符串表` | 解密后的字符串表字节 |
| 局部 `num*` / `array*` / `text*` | 见代码 | 逐个按用途命名（`累加器`、`混沌值`、`解密输出`、`密钥字`…） |

## 关键方法与流程

### 1. `自实现MD5(byte[] 消息)` — 自实现 MD5 主流程

1. 补位：`补位差 = 448 - 消息长度*8 % 512`，`补位字节数 = (补位差+512)%512`（为 0 时取 512），
   总长 = 消息长 + 补位/8 + 8；复制原文后在第 `消息长` 字节 `|= 0x80`；
2. 末尾 8 字节写入 64 位**小端**原始比特长度（`for j=8..1`，j=8 写最低字节）；
3. `字数 = 总长*8/32`，初始 IV `A/B/C/D = 1732584193/4023233417/2562383102/271733878`；
4. 每个 64 字节块：
   - 内层 `for (字偏移 = 0; 字偏移 < 61; 字偏移 += 4)` → 16 次，把 4 字节小端拼成 `uint`；
   - 保存四个初值，跑 4 轮共 64 步（`轮函数F×16` → `G×16` → `H×16` → `I×16`）；
   - `状态A..D += 初值A..D`；
5. 4 个 uint 用 `BitConverter.GetBytes` 拼成 16 字节摘要返回。

> 注意 `"< 61U; += 4U"` 是原版写法（等价 `< 64; += 4`），常量按要求原样保留，不要"优化"。

### 2. `MD5轮函数F/G/H/I` + `循环左移`

四个函数的签名完全一致（原版如此）：

```
函数(ref 当前值, b, c, d, 消息下标, 循环位数, 常量下标, 消息字[])
当前值 = b + 循环左移(当前值 + 轮函数(b,c,d) + 消息字[消息下标] + MD5常量表[常量下标-1], 循环位数)
```

常量下标是 **1 基**（所以有 `- 1U`）。64 次调用的「谁 ref、b/c/d 怎么轮换、消息下标、位移、常量下标」
与原版逐条一致，只是换了名字，调用顺序没有动。

### 3. FIPS 三件套 → `是否仅允许FIPS算法` / `读取FIPS限制开关` / `创建对称加密算法` / `计算MD5`

- `读取FIPS限制开关`：`typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", Static|Public).Invoke(null, [])`，
  结果存 `FIPS开关值`，异常被吞（值保持 false）；
- `创建对称加密算法`：FIPS=true → 试 `new AesCryptoServiceProvider()`，失败退回 `RijndaelManaged`；FIPS=false → 直接 `RijndaelManaged`；
- `计算MD5`：FIPS=false → `new MD5CryptoServiceProvider().ComputeHash(数据)`；FIPS=true → `自实现MD5`。

### 4. `解密字符串(int 字符串表偏移)` — 字符串解密 + 内存读取

首次调用（`字符串表` 为空）时的建表流程：

1. `new BinaryReader(Assembly.GetManifestResourceStream("9f3465d1-7736-405a-9581-d615ccfda07a"))` 读全部字节；
2. 32 字节**密钥素材**、16 字节**IV 素材**（下面 IOC 表给了完整字节），
   `密钥 = 密钥素材`、`初始化向量 = IV素材`（原版是别名）；
3. 取本程序集 `GetPublicKeyToken()` 的 8 个字节，写进 `初始化向量` 的**奇数下标** `[1][3]…[15]`；
4. `for i: 密钥[i] ^= 初始化向量[i]`（把密钥和 IV 混在一起）；
5. **仅当 `字符串表偏移 == -1`**：`算法.Mode = CBC` → `CreateDecryptor(密钥, 初始化向量)` →
   把资源字节写进 `CryptoStream` → `FlushFinalBlock()` → `字符串表 = 内存流.ToArray()`；
   分配 `字符串缓存` / `偏移到缓存槽`（长度 = `字符串表长/4 + 1`）→ 关闭两层流与读取器 →
   `表字节 = 字符串表`。
   （偏移 ≠ -1 时**不解密**，直接用资源原字节进下一步——原版如此，不能想当然改写。）
6. **第二层自制流密码**（每次都跑）：按 4 字节一组
   - `密钥字` = 密钥里第 `序号 % 8` 个 4 字节小端字；
   - 最后一组不足 4 字节时：从末尾往前 `数据字 = (数据字 << 8) | 表字节[末尾-1-序号]`，再 `累加器 += 密钥字`；
     否则 `数据字` 取该组 4 字节小端，再 `累加器 += 密钥字`；
   - 掩码 `累加器` 的推进：`混沌值 = 累加器`，然后
     `^= >>25; += 1394801171; ^= <<5; += 3457341307; ^= >>3; += 4040842541; + 2018246656`，
     `累加器 = 累加前值 + 混沌值`；
   - `解密输出[组] = 累加器 ^ 数据字`（尾组逐字节写）；
7. `字符串表 = 解密输出`，再次分配两个缓存数组；
8. 记模块基址：`Marshal.GetHINSTANCE(Assembly.GetModules()[0])`，32 位进程额外存 `模块基址32位`。

之后每次调用：

9. `槽位 = 字符串表偏移 / 4`；若 `偏移到缓存槽[槽位] > 0` → 直接返回 `字符串缓存[该槽]`；
10. `表内偏移 = BitConverter.ToInt32(字符串表, 字符串表偏移)`；
11. 若 `本进程句柄 == Zero` → `OpenProcess(16 /*VM_READ*/, 1, 本进程 PID)`；
12. **用 `ReadProcessMemory` 从本进程模块内存读**：先读 4 字节得到长度 `n`，再读 `n` 字节密文
    （32/64 位各一条分支，偏移分别用 `模块基址32位 + 表内偏移` / `模块基址 + 表内偏移`）；
13. `AES解密(密文)` → `Encoding.Unicode.GetString` → 存进 `字符串缓存[下一个缓存槽位]`，
    `偏移到缓存槽[槽位] = 下一个缓存槽位`，`下一个缓存槽位++` → 返回。

> 用 `ReadProcessMemory` 读**自己**是刻意的：内存转储工具按 PE 读字符串表会读到密文。

### 5. `运行时还原方法体()` — 代码还原（模块初始化器）

1. 只做一次（`方法体已还原` 标志）；
2. 读嵌入资源 `"08d03c1e-656e-4b82-b230-c3549f0c2ec9"`；
3. 32 字节密钥素材、16 字节 IV 素材 → **`Array.Reverse(初始化向量)`**（和 `解密字符串` 相反的做法）
   → 公钥标记覆盖奇数下标 → `Array.Clear(公钥标记)` → `密钥[i] ^= 初始化向量[i]`；
4. 用与 `解密字符串` 相同的「累加 + 混沌 + 异或」流密码解出补丁字节流
   （尾组分支里是**先** `累加器 += 密钥字` **再**拼数据字，跟 `解密字符串` 顺序相反，原版如此）；
5. `Array.Clear(初始化向量)`，关掉资源读取器，改用 `new BinaryReader(new MemoryStream(补丁字节))`；
6. `OpenProcess(56 /*VM_READ|VM_WRITE|VM_OPERATION*/, 1, 本进程 PID)`，`模块基址 = GetHINSTANCE(...)`；
7. `条目数 = ReadInt32()`，`ReadInt32()` 跳过一个保留字段；每条：`目标 = 模块基址 + ReadInt32()`，
   `VirtualProtect(目标, 4, 4 /*PAGE_READWRITE*/, ref 旧)` → `WriteProcessMemory(句柄, 目标, ReadInt32(), 4)` → `VirtualProtect(目标, 4, 旧, ref 旧)`；
   （原版 `if (IntPtr.Size == 4)` 两支代码**完全相同**，照原样保留）
8. `try`：循环 `while (位置 < 长度-1)`：`段地址 = 模块基址 + ReadInt32()`、`段字数 = ReadInt32()`，
   `VirtualProtect(段地址, 段字数*4, 4)` → 逐 4 字节 `Marshal.WriteInt32` → 恢复保护；
   结束后 `CloseHandle(句柄)`；`catch {}` 吞掉全部失败。

### 6. 其他方法

- `AES解密`：`RijndaelManaged/AesCryptoServiceProvider` + **写死的 32 字节 Key / 16 字节 IV**，
  把密文写进 `CryptoStream` 后取 `内存流.ToArray()`；
- `AES加密为Base64`：明文 `Encoding.Unicode` 取字节，Key 写死 32 字节，IV = `计算MD5(Unicode(密钥源))`，输出 Base64；
- `Base64转Unicode字符串`：`Convert.FromBase64String` → `Encoding.Unicode.GetString`；
- `获取程序集路径`：三段独立 `try/catch`（`Location` → `CodeBase` 去掉 `file:///` → 反射 `Location`），都失败返回 `""`；
- `读取文件字节`：`FileStream` + `while (剩余 > 0)` 循环读满；
- 6 个返回 `null` 的私有空桩、6 个返回 `{1,2}` 的授权空桩、6 个返回 true/false 的 `*Definition` 空桩——全部无调用点。

## 恶意行为 / 敏感操作

1. **运行时代码解密与内存自改**：`VirtualProtect` 改自身模块页属性 → `WriteProcessMemory` 写 IL → 恢复属性。
   这是「方法体加密」的标准落地方式，静态反编译看到的那些空方法体/乱码正是被它还原的。
2. **自读取进程内存**：`OpenProcess(自己的 PID)` + `ReadProcessMemory` 读自己的模块，用于字符串表，
   专门对抗内存 dump（dump 下来的是密文）。
3. **反调试/反分析配套**：公钥标记参与密钥派生（换签名就会解不出字符串）；FIPS 检测走反射；
   大量诱饵数组写入、空方法体、随机方法名。
4. **内嵌资源承载秘密**：两份匿名 GUID 资源分别是「字符串表」与「IL 补丁」——静态分析时应把它们 dump 出来单独分析。
5. **未注册试用版壳**：`CustomerStrategy` 里那条 `unregistered version of "IntelliLock"` 提示说明作者用的是**盗版/试用版** IntelliLock。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 嵌入资源名（字符串表） | `9f3465d1-7736-405a-9581-d615ccfda07a` |
| 嵌入资源名（IL 补丁） | `08d03c1e-656e-4b82-b230-c3549f0c2ec9` |
| 字符串表密钥素材（32 字节，异或 IV 后使用） | `187,155,14,206,64,218,4,187,188,111,114,135,161,59,18,64,86,109,242,248,201,19,240,40,243,162,96,115,144,60,174,71` |
| 字符串表 IV 素材（16 字节） | `1,159,234,155,181,140,150,141,247,35,193,170,77,110,196,73` |
| IL 补丁密钥素材（32 字节） | `94,92,40,45,5,179,190,225,55,128,208,17,133,84,149,116,9,134,24,78,151,228,126,21,63,208,205,135,28,91,23,110` |
| IL 补丁 IV 素材（16 字节，先 Reverse 再覆盖） | `14,136,12,170,30,40,188,231,204,247,32,247,48,51,97,148` |
| `AES解密` 固定 Key（32 字节） | `48 CB 0B C5 5E 4A 1A D4 F0 F1 15 3C 89 B0 B8 31 74 61 FB 15 0B D8 DC 27 F7 BF 8D 22 71 48 75 35` |
| `AES解密` 固定 IV（16 字节） | `3B F6 A6 4B 29 0D 77 0D 2C 2E 62 15 1C FB E0 6C` |
| `AES加密为Base64` 固定 Key（32 字节） | `52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58` |
| 流密码常量 | `1394801171`(0x5322FA13)、`3457341307`(0xCE12D77B)、`4040842541`(0xF0DA5D2D)、`2018246656`(0x784C0000)；位移 25 / 5 / 3 |
| 授权编号模板 | `{11111-22222-40001-00001}`、`{11111-22222-40001-00002}`、`{11111-22222-50001-00001}`、`{11111-22222-50001-00002}`、`{11111-22222-60001-00001}`、`{11111-22222-60001-00002}` |
| 反射目标 | `System.Security.Cryptography.CryptoConfig.get_AllowOnlyFipsAlgorithms` |
| 原生函数 | `kernel32!VirtualProtect`、`kernel32!ReadProcessMemory`、`kernel32!WriteProcessMemory`、`kernel32!OpenProcess`、`kernel32!CloseHandle` |
| 内存访问掩码 | `16`（VM_READ）、`56`（VM_READ|VM_WRITE|VM_OPERATION）、`4`（PAGE_READWRITE） |
| MD5 常量表 | 标准 64 项 `T[i]`（见代码 `MD5常量表`），可用于识别「同一套壳」的其他样本 |
| 注册表 / URL | 本文件不访问注册表、不发网络请求 |

### 反混淆时被「消除」的噪声（务必知道它们不是代码）

1. **同一数组下标连续赋值**：`array2[0] = 155; array2[0] = 138; … array2[0] = 187;` 这类 4~7 次重复写入，
   只有**最后一次**生效（中间没有读取），已按最终值重写成标准数组初始化。原版行为 = 只保留最后值，逐字节等价。
2. **`(int)((UIntPtr)x)` 索引强转**：dnSpy 为 `conv.u` 生成的噪声，已还原为普通下标。
3. **`num4 = num4;` 自赋值**：反编译噪声，已删。
4. **整段 `byte[] a = new byte[N];` + 逐下标赋值的展开式**：已合并为数组初始化器（值不变）。
5. **`object` 类型参数**：原版 `ConcatParams(..., object object_0)` 却在体内按下标取值，
   实际调用方一律传 `uint[]`，已还原为 `uint[] 消息字`（类型明确了，但没有改变任何调用或装箱行为）。
