# Internal/InstanceMapper.cs

## 文件作用

混淆器（.NET Reactor）的运行时支撑类，整个 `about` 工程里体量最大、也最"非业务"的文件。它把三件完全不同的事塞在同一个类里：

1. **密码学**：自实现 MD5（64 轮 + 常量表）、FIPS 策略探测、对称算法工厂、AES-CBC 加解密；
2. **字符串解密器**：从内嵌资源解出整池加密字符串，再按索引从本进程内存里读单条明文并缓存；
3. **内存补丁器**：解密另一份内嵌数据后，用 `VirtualProtect` + `WriteProcessMemory` 往自身模块的 RVA 处回写 int32。

其余二十来个方法恒返回 `null` / `true` / `false`，是混淆器插入的垃圾方法。

**重要结论**：本工程是解壳（NETReactorSlayer → de4dot → dnSpy）后的产物，两个解密例程依赖的内嵌资源 `dac028a9-…`、`c7c9a35c-…` 已被剥离（现存资源只有 `Form1.resources` 里的图标与背景图）。所以在本工程里它们是**死代码**；但在原样本运行时，它们由壳的 stub 调用，是"源码里为什么看不见字符串"的真正原因。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `InstanceMapper` | 混淆器运行时支撑：MD5、FIPS 探测、算法工厂、字符串池解密、内存补丁、AES 加解密 |
| `自实现MD5` + `MD5轮函数F/G/H/I` + `循环左移` + `MD5常量表` | 完整的手写 MD5（用于 FIPS 模式下替代 BCL 的 MD5） |
| `解密字符串(int)` | 字符串池解密：资源 → AES 解密 → 32 位混洗 → 按索引从内存读单条 → 缓存 |
| `解密并修补内存数据()` | 资源解密 → 按 RVA 回写 int32 到自身模块（抗篡改/代码还原） |
| `AES解密字符串数据` / `AES加密字符串` | 固定 key 的 AES-CBC 解密 / 加密（加密侧 IV 由 MD5 派生） |
| `GetterMapper` / `ObserverMapper<T>` / `FSCOMPRESULT` | 混淆器塞进来的占位 Attribute、占位内部类、空枚举 |
| 若干 `空实现占位N` / `占位方法N` | 垃圾方法，无调用点 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `method_0` | `空方法占位` | 空方法体 |
| `SelectComposer(object)` | `自实现MD5(object 输入数据)` | 手写 MD5 主循环（16 字消息调度 + 64 轮） |
| `CloneComposer` | `MD5轮函数F` | `(b & c) \| (~b & d)` |
| `DestroyComposer` | `MD5轮函数G` | `(b & d) \| (c & ~d)` |
| `PrepareComposer` | `MD5轮函数H` | `b ^ c ^ d` |
| `CreateComposer` | `MD5轮函数I` | `c ^ (b \| ~d)` |
| `InitComposer` | `循环左移` | `(v >> (32-n)) \| (v << n)` |
| `m_GlobalMapper` | `MD5常量表` | 64 个标准 T 值 |
| `TestComposer` | `是否仅允许FIPS算法()` | 读缓存的 FIPS 开关 |
| `InterruptComposer` | `读取FIPS策略()` | 反射读 `CryptoConfig.AllowOnlyFipsAlgorithms` |
| `ComputeComposer` | `创建对称算法()` | 按 FIPS 开关选 `RijndaelManaged` / `AesCryptoServiceProvider` |
| `FlushComposer` | `MD5摘要(byte[])` | FIPS 分流：BCL MD5 或自实现 |
| `PopComposer(int)` | `解密字符串(int 字符串索引)` | 字符串池解密器 |
| `GetComposer` | `Base64转Unicode` | Base64 → UTF-16 字符串 |
| `FindComposer` | `解密并修补内存数据()` | 用解出的指令表回写自身内存 |
| `CalculateComposer` | `获取程序集路径(Assembly)` | 三种方式取磁盘路径 |
| `StopMapper` | `读取整个文件(string)` | 整文件读入 byte[] |
| `SetMapper` | `AES解密字符串数据(byte[])` | 固定 key/IV 的 AES-CBC 解密 |
| `SearchMapper` | `AES加密字符串(string,string)` | 固定 key、IV=MD5(种子) 的 AES 加密 |
| `QueryMapper`…`VisitMapper` | `空实现占位1`…`空实现占位6` | 恒返回 null |
| `CallMapper`…`EnableMapper` | `占位方法1`…`占位方法6` | 带 `{11111-22222-…}` 标记串、两分支同值 |
| `SelectFacade` / `InstantiateFacade` | `返回真占位` / `返回假占位` | 恒值桩 |
| `_AccountMapper` | `字符串池数据` | 解密后的池（相对偏移表 + 密文） |
| `_WorkerMapper` / `m_WrapperMapper` / `m_SchemaMapper` | `字符串缓存` / `缓存槽索引` / `下一个缓存槽` | 三件套的字符串缓存 |
| `_RoleMapper` | `当前进程句柄` | `OpenProcess` 得到的自身句柄 |
| `paramMapper` / `m_IteratorMapper` | `模块基址32` / `模块基址64` | `Marshal.GetHINSTANCE` 结果 |
| `_ProductMapper` / `_MockMapper` | `已读取FIPS策略` / `仅允许FIPS算法` | FIPS 探测状态 |
| `systemMapper` | `已解密资源数据` | 内存补丁例程的一次性开关 |
| `infoMapper` | `读取字节数` | `ReadProcessMemory` 出参静态字段 |
| `comparatorMapper` / `m_HelperMapper` / `m_PredicateMapper` | `未使用缓冲1/2/3` | 无读取点的残留缓冲 |
| 局部 `num*` / `array*` | `补位基准`/`累加器`/`密钥字`/`明文缓冲` 等 | 逐处按语义命名 |

## 关键方法与流程

### 1. `自实现MD5(object)` — 手写 MD5

1. `补位基准 = 448 - 输入长度*8 % 512`，`补位字节数 = (补位基准 + 512) % 512`，若为 0 则取 512；
2. `总长度 = 输入长度 + 补位字节数/8 + 8`，`位长度 = 输入长度 * 8`；
3. 拷贝原文，置 `0x80`，末 8 字节写小端位长度；
4. `分组数 = 补位后长度*8/32`，状态初值 `a=0x67452301, b=0xEFCDAB89, c=0x98BADCFE, d=0x10325476`；
5. 每个 512 位分组：16 个字按小端拼装（循环上界写 `61`、步长 4，正好覆盖 0..60）；
6. 依次调用 F×16、G×16、H×16、I×16（共 64 次，参数与常量索引逐条对应标准 MD5）；
7. `a/b/c/d += 旧值`，最后把四个状态字小端拼成 16 字节摘要。

> 参数类型是元数据里的 `object`（IL 用 `ldlen` 直接取长度），属不可验证代码，这里保持原签名。

### 2. FIPS 分流

- `是否仅允许FIPS算法()`：首次调用触发 `读取FIPS策略()`，之后只读缓存；
- `读取FIPS策略()`：`typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig").GetMethod("get_AllowOnlyFipsAlgorithms").Invoke(null, new object[0])`，整段 `try/catch` 吞异常；
- `创建对称算法()`：FIPS=真 → `AesCryptoServiceProvider`（失败退 `RijndaelManaged`）；FIPS=假 → `RijndaelManaged`；
- `MD5摘要()`：FIPS=假 → `MD5CryptoServiceProvider.ComputeHash`；FIPS=真 → `自实现MD5`。

### 3. `解密字符串(int 字符串索引)` — 字符串池

**第一段（只跑一次，`字符串池数据` 为空时）**

1. 打开内嵌资源 `dac028a9-ca53-4586-8b57-452994e78968`，全读进 `数据缓冲`；
2. 展开 32 字节 `密钥材料` 与 16 字节 `初始向量`（原版对同一下标连续赋值多次，只有最后一次生效，已按最终值展开）；
3. `公钥标记 = Assembly.GetName().GetPublicKeyToken()`，若非空，把 `向量缓冲` 的**奇数号下标**（1,3,5…15）替换成标记的 8 个字节；
4. `密钥缓冲[i] ^= 向量缓冲[i]`（i < 16，只影响密钥前 16 字节）；
5. 若 `字符串索引 == -1`：`CBC` 解密资源 → `字符串池数据`；按池长度分配 `字符串缓存` 与 `缓存槽索引`；
6. 对整块做 32 位流式混洗：
   - 每 4 字节取一个「密钥字」，`累加器 += 密钥字`；
   - `混合值 = 累加器`，依次 `^>>25`、`+=1394801171`、`^<<5`、`+=3457341307`、`^>>3`、`+=4040842541`、`+2018246656`；
   - `累加器 = 种子 + 混合值`，输出字 = `累加器 ^ 数据字`；
   - 末组按 `长度 % 4` 做部分字节输出（掩码逐次左移 8）；
7. 记录模块基址：`IntPtr.Size == 4` 时取 `GetHINSTANCE().ToInt32()`，同时无条件取 `ToInt64()`。

**第二段（每次调用）**

1. `槽位 = 字符串索引 / 4`，`缓存槽索引[槽位] > 0` 时直接返回缓存；
2. `相对偏移 = BitConverter.ToInt32(字符串池数据, 字符串索引)`；
3. `OpenProcess(0x10, 1, 当前进程ID)`（懒打开，权限 `PROCESS_VM_READ`）；
4. 从 `模块基址 + 相对偏移` 读 4 字节得到密文长度，再从 `+4` 处读该长度的密文；
5. `AES解密字符串数据(密文)` → `Encoding.Unicode.GetString` → 写入缓存，返回。

### 4. `解密并修补内存数据()`

1. 一次性开关 `已解密资源数据`；
2. 读内嵌资源 `c7c9a35c-0300-44ea-976c-1797ff0710b0`；
3. 展开 32 字节密钥材料、16 字节向量材料 → `Array.Reverse(向量缓冲)` → 奇位替换成公钥标记 → `Array.Clear(公钥标记)` → `密钥材料[i] ^= 向量缓冲[i]`；
4. 同样的 32 位混洗解出 `结果数据`；
5. `OpenProcess(0x38, 1, 当前进程ID)`（`PROCESS_VM_OPERATION|VM_WRITE|VM_READ`），取模块基址；
6. 前段补丁：读 `补丁项数`（再丢弃一个 int32），逐项「RVA + 4 字节值」→ `VirtualProtect(地址, 4, PAGE_READWRITE, ref 旧保护)` → `WriteProcessMemory` → 恢复保护；
7. 后段补丁：`while (Position < Length - 1)`，每段「RVA + 字数 + 若干 int32」，`VirtualProtect` 后 `Marshal.WriteInt32` 逐字写入，再恢复保护；
8. `CloseHandle`；整段 `try/catch` 吞异常。

### 5. 其它真实实现

- `AES解密字符串数据`：硬编码 32 字节 key + 16 字节 IV，CBC 解密（无 `FlushFinalBlock`，直接取 `ToArray()`）；
- `AES加密字符串`：key 另一组 32 字节常量，IV = `MD5摘要(Unicode(密钥种子))`，输出转 Base64；
- `Base64转Unicode`：`Convert.FromBase64String` → `Encoding.Unicode.GetString`；
- `获取程序集路径`：`Location` → `CodeBase` 去 `file:///` → 反射读 `Location` 属性，三路各自 `try/catch`，全失败返回空串；
- `读取整个文件`：按 `Length` 分配后循环读满（若文件被截断会空转，原版隐患未修）。

### 6. 垃圾方法（无调用点）

- `空实现占位1..6`：`return null;`
- `占位方法1..6`：各带一个 `{11111-22222-…}` 标记串，`if (串.Length > 0)` 与 `else` 两支返回同一个 `new byte[] { 1, 2 }`；
- `返回真占位` / `返回假占位`；
- `GetterMapper : Attribute`（构造函数忽略入参、贴给 `解密并修补内存数据`）、内部类 `ObserverMapper<T>`、空枚举 `FSCOMPRESULT`。

## 恶意行为 / 敏感操作

| 行为 | 具体表现 |
| --- | --- |
| **自进程内存写** | `OpenProcess` 打开**自己** → `VirtualProtect` 改 `PAGE_READWRITE` → `WriteProcessMemory` / `Marshal.WriteInt32` 按 RVA 回写 int32，再恢复保护。这是壳的代码还原/抗篡改，也是典型「运行时改自己代码段」技术 |
| **自进程内存读** | 字符串池密文不在文件里直接可用，而是运行时用 `ReadProcessMemory` 从模块内存按偏移取，规避静态字符串扫描 |
| **硬编码密钥** | AES key/IV 直接写在代码里（`131,59,128,…` 与 `59,187,243,…` 两组；加密侧 `82,102,104,…`），等于把"加密"退化成混淆，可完全离线还原字符串 |
| **隐藏内嵌资源** | 两个 GUID 命名的 manifest 资源承载加密字符串池与补丁表；解壳后已被剥离 |
| **反分析**：FIPS 分支、`SuppressIldasm`（同程序集清单）、`Base64`/`Unicode` 冷门编码 | 让静态扫描与自动脱壳更容易失败 |
| 无网络、无注册表、无文件落地 | 本文件自身不落盘（落盘逻辑在 `IssuerMapper`） |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 内嵌资源（字符串池） | `dac028a9-ca53-4586-8b57-452994e78968` |
| 内嵌资源（补丁表） | `c7c9a35c-0300-44ea-976c-1797ff0710b0` |
| AES key（解密字符串池） | `98 9C 77 EE 39 D4 CC 13 93 5B BD 3B 5F 13 AA 20 E0 77 33 26 A6 5C 9D 5D C9 27 3D 9F C9 3A 73 4E` |
| AES IV（解密字符串池） | `67 71 14 EA 2E 5A 50 E5 7A 78 21 98 00 00 8B 69` |
| AES key（解密单条字符串） | `83 3B 80 DD BF 84 24 DC 7B E1 0E FD 19 B3 6C 8C 11 04 7D 11 C1 C7 35 22 F5 4D 0D 15 E9 F1 8F 79` |
| AES IV（解密单条字符串） | `3B BB F3 D9 09 BA 13 C2 52 3E A5 09 13 D8 ED 9D` |
| AES key（加密） | `52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58` |
| 混洗常量 | `1394801171`、`3457341307`、`4040842541`、`2018246656`（连续加/异或扰动） |
| 占位标记串 | `{11111-22222-40001-00001}`、`{11111-22222-40001-00002}`、`{11111-22222-50001-00001}`、`{11111-22222-50001-00002}`、`{11111-22222-60001-00001}`、`{11111-22222-60001-00002}` |
| 反射探测字符串 | `System.Security.Cryptography.CryptoConfig`、`get_AllowOnlyFipsAlgorithms` |
| P/Invoke | `kernel32.dll`：`VirtualProtect`、`WriteProcessMemory`、`ReadProcessMemory`、`OpenProcess`、`CloseHandle` |
| 进程权限常量 | `16`（PROCESS_VM_READ）、`56`（0x38：VM_OPERATION/VM_WRITE/VM_READ）、保护值 `4`（PAGE_READWRITE） |
| MD5 常量表示例 | 首项 `3614090360`，末项 `3951481745` |
| IL 行号 | 类型 `0x02000010`，方法 `0x0600007C`–`0x060000A5`，字段 `0x04000028`–`0x04000036` |
