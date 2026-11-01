# Internal/IndexerConfig.cs 分析

## 文件作用

**.NET Reactor 的运行时辅助类**。样本原被 .NET Reactor 加壳，解壳后保护器的运行时仍留在程序里，
就是这个 `IndexerConfig`。它跟「机房管理助手」的业务**没有任何关系**，功能全在「保护/自解密」侧：

1. 自实现 MD5（标准 RFC 1321），用于在系统只允许 FIPS 算法时顶替 `MD5CryptoServiceProvider`；
2. 字符串解密：从内嵌资源解出加密字符串表，再按偏移逐条 AES 解密（加密字符串常量最终都变成对它的调用）；
3. 方法体还原（anti-tamper）：把加密的方法体解密后直接写回本进程内存（自修改代码）；
4. AES 加解密小工具与一批空实现/混淆桩。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `IndexerConfig`（未改名） | Reactor 运行时容器：MD5 + 字符串解密 + 内存补丁 + AES 工具 |
| `自实现MD5(byte[])`（原 `DefineReader`） | 标准 MD5 主循环，返回 16 字节摘要 |
| `MD5轮函数F/G/H/I`（原 `RegisterReader`/`AwakeReader`/`MoveReader`/`RunReader`） | MD5 四轮轮函数 |
| `循环左移(uint, ushort)`（原 `GetReader`） | `(x << n) \| (x >> (32-n))` |
| `是否允许FIPS算法()`（原 `SetupReader`） | 懒探测「系统只允许 FIPS 算法」并缓存 |
| `探测FIPS开关()`（原 `DestroyReader`） | 反射读 `CryptoConfig.AllowOnlyFipsAlgorithms` |
| `创建AES实现()`（原 `QueryReader`） | FIPS 模式返回 `AesCryptoServiceProvider`，否则 `RijndaelManaged` |
| `计算哈希(byte[])`（原 `AssetReader`） | 非 FIPS 用系统 MD5，FIPS 用自实现 MD5 |
| `解密字符串(int)`（原 `RestartReader`） | **字符串解密主入口**（资源 → AES → 偏移表 → 读自身内存 → AES） |
| `Base64解码字符串(string)`（原 `CancelReader`） | Base64 → UTF-16 字符串 |
| `还原方法体()`（原 `ManageReader`） | **方法体内存补丁**（资源 → 双层解密 → VirtualProtect + WriteProcessMemory） |
| `获取程序集路径(Assembly)`（原 `ForgotReader`） | 三种 Fallback 取程序集磁盘路径 |
| `读取整个文件(string)`（原 `RevertReader`） | 循环读到长度读满的整文件读取（无调用点） |
| `AES解密(byte[])`（原 `VerifyConfig`） | 固定 Key/IV 的 AES-CBC 解密（字符串密文用它） |
| `AES加密(string,string)`（原 `FillConfig`） | 固定 Key 的 AES-CBC 加密 → Base64（无调用点） |
| `空实现_返回null_1..6`（原 `IncludeConfig`…`FindConfig`） | 空方法，恒返回 `null` |
| `混淆桩_返回1和2_1..6`（原 `ComputeConfig`…`PublishConfig`） | 带 GUID 样式许可串，但两个分支都返回 `{1,2}`，纯噪声 |
| `混淆器占位方法_恒真/恒假` | 恒返回 true/false，无调用者 |
| `空方法()`（原 `method_0`） | 空方法 |
| `WatcherConfig` 特性 + `CreatorConfig<T>` | 挂在还原方法体上的空特性，只为把 `typeof(CreatorConfig<object>[])` 塞进元数据 |
| `VirtualTreeDisplayDataChanges` 枚举 | 空枚举（`[Flags]`，无成员） |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `DefineReader(object)` | `自实现MD5(byte[] 待哈希数据)` | 参数 IL 类型是 `object`，只接受 `byte[]`，已还原为 `byte[]` |
| `RegisterReader(ref u,u,u,u,u,ushort,u,object)` | `MD5轮函数F(ref 目标字, b, c, d, 消息字索引, 循环位数, 常量索引, uint[] 消息字表)` |  |
| `AwakeReader` / `MoveReader` / `RunReader` | `MD5轮函数G` / `MD5轮函数H` / `MD5轮函数I` |  |
| `GetReader` | `循环左移` |  |
| `SetupReader` / `DestroyReader` | `是否允许FIPS算法` / `探测FIPS开关` |  |
| `QueryReader` | `创建AES实现` |  |
| `AssetReader` | `计算哈希` |  |
| `RestartReader(int)` | `解密字符串(int 字符串偏移)` |  |
| `CancelReader` | `Base64解码字符串` |  |
| `ManageReader` | `还原方法体` |  |
| `ForgotReader` | `获取程序集路径` |  |
| `RevertReader` | `读取整个文件` |  |
| `VerifyConfig` | `AES解密` |  |
| `FillConfig` | `AES加密` |  |
| `IncludeConfig`/`SetConfig`/`EnableConfig`/`InterruptConfig`/`PushConfig`/`FindConfig` | `空实现_返回null_1..6` | 全是 `return null` |
| `ComputeConfig`…`PublishConfig` | `混淆桩_返回1和2_1..6` | 两分支同值 |
| `PostParser`/`SetupParser` 及各处 `*Parser` | `混淆器占位方法_恒真/恒假` |  |
| `eventConfig` | `MD5常量表` | RFC 1321 的 64 个 K 值，未改动 |
| `_WrapperConfig` | `解密后数据` | 字符串表/补丁表解密结果 |
| `m_WriterConfig` / `mapperConfig` / `tagConfig` | `字符串缓存索引表` / `字符串缓存` / `字符串缓存游标` | 字符串解密缓存 |
| `_FilterConfig` / `expressionConfig` | `进程句柄` / `读取字节数` | 读自身内存用 |
| `_ReponseConfig` / `modelConfig` | `模块基址32` / `模块基址64` | `Marshal.GetHINSTANCE` |
| `queueConfig` / `_TestConfig` | `已探测FIPS` / `允许FIPS算法` |  |
| `itemConfig` | `已还原方法体` | 补丁只打一次 |
| `m_PageConfig` / `m_ListConfig` / `publisherConfig` | `未使用字段_1..3` | 无引用 |
| `method_0` | `空方法` |  |
| 局部 `num/num2/…/num19` | 语义名（`补位基准`/`总长度`/`A`~`D`/`块号`/`消息字`/`余数字节`/`块数`/`状态`/`混合值`/`补丁条数`/`补丁RVA`/`写入字数` …） | 逐处还原 |

## 关键方法与流程

### `计算哈希` + `自实现MD5` + 四个轮函数

- `计算哈希`：`if (!是否允许FIPS算法()) return MD5CryptoServiceProvider.ComputeHash(数据); return 自实现MD5(数据);`
  —— 两条路必须给出同一个 16 字节结果（同一个算法，只是实现不同）。
- `自实现MD5`：按 512 位补位（补 `0x80`、补零、末尾 8 字节写比特长度小端）→ 4 个状态字
  `A=0x67452301`、`B=0xefcdab89`、`C=0x98badcfe`、`D=0x10325476` → 每块 4 轮 × 16 步
  （F 用 7/12/17/22，G 用 5/9/14/20，H 用 4/11/16/23，I 用 6/10/15/21，常量依次 1..64）→
  `A/B/C/D` 各加回旧值 → 以小端拼 16 字节摘要。64 组（消息字索引, 位, 常量）与原 IL 逐条比对一致。

### `解密字符串(int)` —— 字符串解密

首次调用（`解密后数据` 为空）时：

1. 读内嵌资源 `21413984-2025-4873-8c6f-e4b197ae8963`（AES 加密的「偏移表」）。
2. 用硬编码的 32 字节 `array2`（只有每个下标的**最后一次**赋值生效，混淆器写了大量垃圾赋值）作为密钥、
   16 字节 `array4` 作为 IV；若程序集带强名称，再把公钥标记的 8 个字节隔位写进 IV；最后 `密钥[i] ^= IV[i]`。
3. AES-CBC 解密资源 → `解密后数据`。
4. 再做第二层解密：按 4 字节一组，`状态 += 密钥字`，然后
   `m ^= m>>25; m += 1394801171; m ^= m<<5; m += 3457341307; m ^= m>>3; m += 4040842541; m += 2018246656`，
   与明文块 XOR 得到真正的偏移表；同时用 `Marshal.GetHINSTANCE` 记下模块基址（32/64 位两份）。
5. 每次取字符串：按偏移查缓存 → 未命中则从偏移表取 4 字节 RVA →
   `OpenProcess(0x10)` 打开**自己** → `ReadProcessMemory` 读 4 字节长度 + 密文 →
   `AES解密` → UTF-16 解码 → 写入缓存。

要点：密文是**从本进程内存镜像**里读的，不是从文件；长度前缀在内存里，静态 dump 拿不到明文。

### `还原方法体()` —— 方法体内存补丁

1. 读内嵌资源 `47bd6f50-e78c-4bfb-9a1d-e06490ced281`。
2. 同样用硬编码 32 字节密钥 + 16 字节 IV（**先 `Array.Reverse`**）+ 公钥标记隔位写入 + XOR 出密钥流表；
   再做同款「XOR + 变种 ARX」流密码解密。
3. `OpenProcess(0x38)`（VM_OPERATION|VM_READ|VM_WRITE，作用于自己）。
4. 补丁表格式：`{4 字节条数, 4 字节忽略}`，随后 N 组 `{RVA, 值}`；
   对每组 `VirtualProtect(地址, 4, PAGE_READWRITE)` → `WriteProcessMemory` → `VirtualProtect(恢复)`。
5. 之后按块：`{RVA, 字数}` + 若干 4 字节值，用 `Marshal.WriteInt32` 逐字写入并恢复保护属性。
6. 结束 `CloseHandle`。整个写入循环包在 `try/catch { }` 里（出错就放弃，不抛）。

### `AES解密` / `AES加密`

- `AES解密`：固定 Key 32 字节、IV 16 字节（见 IOC 表），`CryptoStream(Write)` 解出明文直接返回。
- `AES加密`：固定 Key 32 字节（另一组），IV = `计算哈希(UTF16(第二个参数))`，输出 Base64。无调用点。

### 其余

- `获取程序集路径`：`Location` → `CodeBase`（去 `file:///`）→ 反射 `Location` 属性，三次独立 `try/catch`，都失败返回 `""`。
- `混淆桩_返回1和2_*`：局部变量是 `"{11111-22222-40001-00001}"` 这类许可串，但两个分支都 `return new byte[]{1,2}`。
- `空实现_返回null_*` / `混淆器占位方法_恒真/恒假`：无副作用、无调用者。

## 恶意行为 / 敏感操作

| 类型 | 具体行为 | 说明 |
| --- | --- | --- |
| 自修改代码 | `VirtualProtect` + `WriteProcessMemory` / `Marshal.WriteInt32` 改写本进程可执行内存 | anti-tamper：方法体磁盘上加密，运行时原地解密覆盖 |
| 读进程内存 | `OpenProcess(PROCESS_VM_READ)` + `ReadProcessMemory`（目标是自己） | 从模块镜像读加密字符串 |
| 写进程内存 | `OpenProcess(0x38)` + `WriteProcessMemory` | 打补丁 |
| 字符串加密 | 资源 + 双层解密（AES-CBC + 自定义 ARX 流密码）+ 偏移表 | 使静态字符串扫描失效 |
| 算法降级规避 | 自己实现 MD5，绕开 FIPS 限制 | 保证资源名/IV 一定能算出来 |
| 隐私/安全相关 | `RSACryptoServiceProvider` 不在本文件（在 `AttributeConfig`） | — |
| 性质 | 全部属于**保护器运行时**，不是机房管理助手的业务逻辑；但「运行时改写自身代码」这一行为本身就是杀软重点检测对象 |

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 内嵌资源名 | `21413984-2025-4873-8c6f-e4b197ae8963` | 加密字符串表（偏移表） |
| 内嵌资源名 | `47bd6f50-e78c-4bfb-9a1d-e06490ced281` | 加密方法体补丁表 |
| AES-256 密钥（字符串表，生效值） | `54 42 138 214 197 175 206 196 33 15 101 223 131 133 27 254 225 30 240 80 187 135 196 121 17 101 187 181 94 182 108 65` | 与 IV XOR 前 |
| AES IV（字符串表，生效值） | `185 172 25 163 0 15 90 109 199 31 118 120 140 167 161 14` | 强名称程序集会再被公钥标记覆盖 8 个位置 |
| AES-256 密钥（补丁表，生效值） | `21 235 68 119 61 41 47 14 211 150 154 31 231 112 30 78 90 80 24 105 200 39 82 96 74 120 4 43 53 186 20 148` |  |
| AES IV（补丁表，生效值，写入前 `Array.Reverse`） | `238 55 125 6 25 23 5 143 28 164 147 193 24 110 247 220` |  |
| AES 解密固定 Key | `248 124 241 203 138 171 153 86 79 123 202 239 236 16 191 67 80 235 38 120 232 218 19 17 82 207 204 82 116 20 145 96` | `AES解密`（字符串密文） |
| AES 解密固定 IV | `97 22 156 34 144 187 12 232 197 73 101 49 195 148 20 151` | 同上 |
| AES 加密固定 Key | `82 102 104 110 32 77 24 34 118 181 51 17 18 51 12 109 10 32 77 24 34 158 161 41 97 28 118 181 5 25 1 88` | `AES加密`（无调用点） |
| 流密码常量 | `25`、`1394801171`、`5`、`3457341307`、`3`、`4040842541`、`2018246656` | 自定义 ARX 层 |
| MD5 常量表 | 标准 RFC 1321 的 64 个 K 值（`3614090360` 起） | 自实现 MD5 |
| MD5 初值 | `1732584193`、`4023233417`、`2562383102`、`271733878` | 标准初值 |
| API | `OpenProcess(0x10 / 0x38)`、`ReadProcessMemory`、`WriteProcessMemory`、`VirtualProtect`、`CloseHandle` | kernel32 |
| 混淆串 | `{11111-22222-40001-00001}`、`{11111-22222-40001-00002}`、`{11111-22222-50001-00001}`、`{11111-22222-50001-00002}`、`{11111-22222-60001-00001}`、`{11111-22222-60001-00002}` | 混淆桩局部变量 |
| 反射 | `System.Security.Cryptography.CryptoConfig` / `get_AllowOnlyFipsAlgorithms` | FIPS 探测 |
