# Predicate.cs.md — .NET Reactor 运行时（自实现 MD5 / 字符串解密 / 方法体解密）

## 文件作用

`OZxTKXuvsl9D34LWhP\Internal\Predicate.cs` 是 **.NET Reactor 加壳器注入的运行时类**，不是"机房管理助手"的业务代码。
它负责三件事：

1. **自实现 MD5**：系统被配置成"只允许 FIPS 认证算法"时，`MD5CryptoServiceProvider` 会抛异常，
   加壳器改用手写的 MD5 顶上（`自实现MD5` + 四个轮函数 + 循环左移 + 64 项常量表）。
2. **字符串解密**：`解密字符串(int)` 从内嵌资源 `940fde4f-6937-4325-8f76-5a332189a59c` 恢复字符串表，
   再按传入下标从**本模块内存映像**里读出密文并 AES 解密成 Unicode 字符串。
   原始 IL 里每条加密字符串都是 `Predicate.RunWatcher(0x0000XXXX)` 这样一次调用。
3. **方法体解密**：`解密方法体并回写内存()` 从内嵌资源 `5c5b1961-5059-4bdd-b555-d9e6275d4842`
   解出 IL 补丁脚本，用 `VirtualProtect` + `WriteProcessMemory` **直接改写自己进程的代码段**。

> 本类**没有任何外部调用者**：全工程搜不到 `Predicate.` 的跨文件引用。
> NETReactorSlayer 已经把字符串内联成明文、把方法体还原，这一整套运行时因此退化成死代码。
> 分析业务行为时可以跳过它，但**不能忽略**它做过"运行期自我打补丁"这件事——这正是原样本能对抗静态分析的原因。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Predicate`（主类，混淆名） | 加壳器运行时聚合体：MD5 兜底、字符串解密、方法体解密、AES 小工具 |
| `Predicate.Indexer` | 加壳器自用标记特性，无逻辑，只让 IL 里带上类型引用 |
| `Predicate.Indexer.Wrapper<T>` | 泛型标记类型，仅用于构造 `typeof(...[])` 参数 |
| `Predicate.AuthFlags` | 空枚举，加壳器残留 |
| 内部 P/Invoke 组 | `VirtualProtect` / `ReadProcessMemory` / `WriteProcessMemory` / `OpenProcess` / `CloseHandle` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `method_0` | `占位空方法` | 空实现，无调用者 |
| `RevertVal(object)` | `自实现MD5(byte[] 数据)` | 参数原导出为 `object`（IL 里当 `byte[]` 用），按真实类型标注 |
| `CallWatcher` | `MD5轮函数F` | `(B & C) \| (~B & D)` |
| `NewWatcher` | `MD5轮函数G` | `(B & D) \| (C & ~D)` |
| `ConcatWatcher` | `MD5轮函数H` | `B ^ C ^ D` |
| `PostWatcher` | `MD5轮函数I` | `C ^ (B \| ~D)` |
| `ResetWatcher` | `循环左移` | `(x >> (32-n)) \| (x << n)` |
| `_Algo` | `MD5常量表` | 64 项 sin 表常量 |
| `PushWatcher` | `是否只允许FIPS算法` | 首次调用时反射读配置并缓存 |
| `OrderWatcher` | `读取FIPS配置` | 反射读 `CryptoConfig.AllowOnlyFipsAlgorithms` |
| `m_Listener` | `已读取FIPS配置` | 布尔标志 |
| `context` | `仅允许FIPS算法` | 读到的配置值 |
| `PopWatcher` | `创建对称加密算法` | FIPS→AesCryptoServiceProvider，否则 RijndaelManaged |
| `CollectWatcher` | `计算MD5` | 非 FIPS 走系统实现，FIPS 走自实现 |
| `RunWatcher(int)` | `解密字符串(int 字符串ID)` | 字符串表解密 + 按 ID 取值 + 缓存 |
| `VisitWatcher` | `Base64解码为Unicode字符串` | Base64 → Unicode |
| `ConnectWatcher` | `解密方法体并回写内存` | 读资源 → 流解密 → 写进程内存 |
| `ValidateWatcher` | `获取程序集磁盘路径` | 三种取 `Location` 的办法，逐条吞异常 |
| `PrepareWatcher` | `读取整个文件` | 无调用者 |
| `CloneWatcher` | `AES解密` | 固定 Key/IV |
| `ComputeWatcher` | `AES加密为Base64` | Key 写死，IV = MD5(种子) |
| `MoveWatcher` … `ExcludeWatcher`（6 个） | 保留原名 | 一律 `return null` 的占位方法 |
| `ReflectWatcher` … `CancelWatcher`（6 个） | 保留原名 | 带 `{11111-22222-…}` 假标记的假方法，两分支返回同值 |
| `RevertTag` / `SearchParser` | 保留原名 | 恒 true / false 的授权标记桩 |
| `m_Database` | `已解密的字符串缓冲` | 解密后的字节缓冲 |
| `m_Item` | `缓存索引` | `字符串ID/4` → 槽位 |
| `specification` | `字符串缓存` | 已解出的字符串 |
| `connection` | `缓存计数` | 从 1 开始 |
| `m_Thread` | `本进程句柄` | `OpenProcess(16,1,PID)` 拿到 |
| `m_Comparator` / `_Service` | `模块基址64` / `模块基址32` | `Marshal.GetHINSTANCE` |
| `_Field` | `方法体已解密` | 一次性标志 |
| `m_Dispatcher` | `读写字节数` | P/Invoke 的 out 参数 |
| `_Task` / `m_Page` / `facade` | 保留原名 | 未使用的空白缓冲 |

## 关键方法与流程

### 自实现MD5(byte[] 数据)

标准 MD5，**结果与 `MD5CryptoServiceProvider` 一致**：

1. 补位：`尾部需补位数 = 448 - (len*8 % 512)`，再补 64 位小端原始位长度；
2. 按 64 字节分块，每块把 16 个 4 字节拼成小端 `uint`（内层循环上界写 61、步长 4，实际 16 次）；
3. 64 轮 = F(16) → G(16) → H(16) → I(16)，每 4 轮轮换 `ref` 的 A/B/C/D 角色；
4. 每块结束把 A/B/C/D 加回各自初值（`0x67452301/0xEFCDAB89/0x98BADCFE/0x10325476`）；
5. 摘要 = A/B/C/D 小端拼接 16 字节。

轮函数内部统一写法：`A = B + 循环左移(A + f(B,C,D) + M[k] + T[i-1], s)`，
`T` 表下标在调用侧从 1 开始编号，函数里再 `-1`。

### 解密字符串(int 字符串ID)

1. 首次进入（`已解密的字符串缓冲.Length == 0`）：
   - 读资源 `940fde4f-6937-4325-8f76-5a332189a59c`；
   - 构造 32 字节密钥 + 16 字节 IV；若程序集有强名称，用**公钥标记低 8 字节覆盖 IV 的奇数位**（防篡改）；
   - 密钥前 16 字节与 IV 异或；
   - `字符串ID == -1` 时才做 AES-CBC 整表解密；
   - 再做一层自制 **xorshift 流解密**（4 字节一块、密钥循环复用、每块累加后混淆）；
   - 记录本模块基址（分 32/64 位两个字段）。
2. 按 `字符串ID/4` 查缓存，命中直接返回。
3. 未命中：从缓冲取"相对模块基址的偏移"，`ReadProcessMemory` 读 `长度 + 密文`，
   再用固定 Key/IV 的 AES 解密，`Encoding.Unicode` 解码，写入缓存后返回。

### 解密方法体并回写内存()

1. 读资源 `5c5b1961-5059-4bdd-b555-d9e6275d4842`；
2. 32 字节密钥（原版每个下标被重复赋值 4~7 次，只有最后一次有效）+ 16 字节 IV；
   IV 先整体 `Array.Reverse`，再用公钥标记替换奇数位，然后密钥前 16 字节与 IV 异或；
3. 同样的 xorshift 流解密得到"IL 补丁脚本"；
4. `OpenProcess(56,1,PID)` 打开**自己**，权限 = `PROCESS_VM_OPERATION|VM_READ|VM_WRITE|QUERY_INFORMATION`；
5. 第一批：逐条 `(模块基址+RVA, 4 字节值)`，`VirtualProtect(…,4,PAGE_READWRITE)` 后 `WriteProcessMemory`，再改回原属性；
6. 第二批：`while (剩余 > 1 字节)` 读 `(节 RVA, 32 位字数)`，成批 `Marshal.WriteInt32` 覆盖方法体；
7. `CloseHandle`（在 `try/catch` 里，异常被吞）。

### 其它

- `创建对称加密算法`：FIPS 模式下试 `AesCryptoServiceProvider`，失败退回 `RijndaelManaged`。
- `AES解密` / `AES加密为Base64`：固定 32 字节 Key 的工具函数，后者 IV = `计算MD5(种子)`，本类内无调用者。
- `获取程序集磁盘路径`：`Location` → `CodeBase` → 反射 `Location`，三段独立 `try/catch`（原 VB 逐条 `On Error Resume Next`）。

## 恶意行为 / 敏感操作

| 行为 | 位置 | 说明 |
| --- | --- | --- |
| **运行期自我打补丁（自修改代码）** | `解密方法体并回写内存` | `VirtualProtect` 把代码页改成可写，`WriteProcessMemory` / `Marshal.WriteInt32` 覆盖方法体 |
| 打开本进程句柄 | 同上 | `OpenProcess(56, 1, GetCurrentProcess().Id)` |
| 从自身内存映像读密文 | `解密字符串` | `OpenProcess(16,1,PID)` + `ReadProcessMemory`，基址来自 `Marshal.GetHINSTANCE` |
| 反篡改：绑公钥标记 | 两处密钥/IV 构造 | 换了强名称签名，密钥/IV 就错，数据解不出来 |
| 隐藏字符串 | 内嵌资源 | 明文全靠资源 + 多层解密，静态搜索关键字搜不到 |

> 这些是**加壳器**的行为，不是"机房管理助手"业务功能；但对安全产品而言，
> "自修改代码 + 读自己内存 + 内嵌加密资源"是可疑度很高的组合，值得作为样本特征记录。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 内嵌资源名 | `940fde4f-6937-4325-8f76-5a332189a59c`（字符串表密文） |
| 内嵌资源名 | `5c5b1961-5059-4bdd-b555-d9e6275d4842`（方法体补丁密文） |
| 反射读取的类型/方法 | `System.Security.Cryptography.CryptoConfig.get_AllowOnlyFipsAlgorithms` |
| 字符串表 32 字节密钥（去垃圾后） | `FB 00 34 E0 67 A7 71 40 E4 50 1D 49 8C 8E D0 F8 AC 23 53 5D E5 91 38 E9 B4 3B DE B9 7E 2E AE 8B` |
| 字符串表 16 字节 IV（去垃圾后） | `5C 12 31 E9 EA 57 CC 48 33 07 1D 4C 27 4D 45 55` |
| 方法体 32 字节密钥（去垃圾后） | `28 7B 69 A3 FB 60 EF 58 70 A7 22 C3 0E 68 0A 83 C3 E8 58 1A E4 A1 71 26 6E DD 99 50 3E 7D 5B 4A` |
| 方法体 16 字节 IV（反转前，去垃圾后） | `62 4A 0C 17 A3 F6 9F 07 B1 16 03 DE 50 70 5F 1C` |
| AES 解密 Key（`AES解密`） | `53 8E 29 86 8B 4A 80 1F FA 88 E5 34 E1 2C EE 12 53 ED FC 1B DD 6C C3 70 99 0D 95 A8 23 5D 57 DD` |
| AES 解密 IV（`AES解密`） | `9C 80 D1 58 88 3B 07 97 AB EB 12 67 89 7A 3E 51` |
| AES 加密 Key（`AES加密为Base64`） | `52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58` |
| 流解密常量 | `1394801171 / 3457341307 / 4040842541 / 2018246656`，右移 25 / 左移 5 / 右移 3 |
| 假标记字符串 | `{11111-22222-40001-00001}`、`…-40001-00002`、`…-50001-00001`、`…-50001-00002`、`…-60001-00001`、`…-60001-00002` |
| P/Invoke | `kernel32.dll`：`VirtualProtect`、`ReadProcessMemory`、`WriteProcessMemory`、`OpenProcess`、`CloseHandle` |
| 进程权限常量 | `16`（`PROCESS_VM_READ`）、`56`（`0x38` = VM_OPERATION+VM_READ+VM_WRITE+QUERY_INFORMATION） |
| 保护属性常量 | `4`（`PAGE_READWRITE`） |
