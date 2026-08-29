# Param.cs — 壳/混淆器核心：字符串解密 + 反篡改内存补丁 + 加解密原语

## 文件作用
反混淆后真实身份：**ConfuserEx（含 .NET Reactor 混合）加壳/反篡改运行时模块**，全部方法名
（`RevertBroadcaster`、`ViewSchema`、`MoveSchema` 等）与字段（`_Identifier`、`m_Setter` 等）均为混淆假名。
它承担四类工作：
1. **MD5 自实现**（`RevertBroadcaster` + `View/Setup/Stop/Verify/CollectSchema` + `_Identifier` 常量表）——
   FIPS 模式（`ForgotSchema`）下替代 `MD5CryptoServiceProvider`；
2. **嵌入资源解密**（`CalcSchema`）：用 AES-CBC（密钥=程序集公钥标记派生）解密嵌入资源
   `f2c33f7a-a406-48ab-af63-034b0bca5201`（字符串常量池），再经自研 32 位流式 XOR/累加算法
   （常数 3753412436/678874538/179542347/522623986，即 ConfuserEx 常量加密的伪随机流）解出
   密文字符串——运行时按偏移取真实字符串；
3. **反篡改（anti-tamper）**（`MoveSchema`）：解密嵌入资源 `d08e40f8-0b4f-412d-8089-995ba478b812`，
   得到"偏移+长度+数据"补丁清单，用 `VirtualProtect` + `WriteProcessMemory`/`Marshal.WriteInt32`
   把解密后的节数据写回自身映像（`GetHINSTANCE(模块)` 基址 + 偏移），覆盖被加密的 IL/常量区——
   这是典型的 ConfuserEx AntiTamper 运行时自修改；
4. **AES 加解密原语**：`InterruptSchema`（固定密钥加密→Base64）、`ConnectSchema`（固定密钥/IV 解密）、
   `CreateSchema`/`DefineSchema`（算法选择与 MD5 派生 IV）、`UpdateSchema`（Base64→Unicode 解码）。
若干 `*Schema()` 桩方法（`DestroySchema` 等）仅返回 `{1,2}` 并引用 `{11111-22222-XXXXX-XXXXX}`
  伪 GUID 字符串——混淆器插入的诱饵/填充方法。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Param（静态核心） | 壳运行时：MD5、AES、字符串解密、反篡改内存补丁 |
| Param.Service | 混淆器辅助 Attribute（标记方法，无运行时逻辑） |
| Param.Service.Worker\<T\> | 填充泛型（恒 true/false 方法） |
| Param.AuditSettings | 空 Flags 枚举（填充物） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `RevertBroadcaster` | `自实现 MD5` | 标准 MD5 压缩（魔数 1732584193 等 + 64 轮常量表） |
| `View/Setup/Stop/VerifySchema` | `MD5 四轮 F 函数` | F/G/H/I 逻辑（与/或/异或/取反） |
| `CollectSchema` | `循环左移` | `ROL(ushort)` |
| `ForgotSchema` | `是否 FIPS 模式` | 反射查 `CryptoConfig.AllowOnlyFipsAlgorithms` / `Utils.FipsAlgorithmPolicy` |
| `GetSchema` | `检测 FIPS 策略` | 反射设置 `_Issuer` |
| `CreateSchema` | `创建对称算法` | FIPS 下用 AesCryptoServiceProvider，否则 RijndaelManaged |
| `DefineSchema` | `MD5 摘要` | 非 FIPS 用 `MD5CryptoServiceProvider`，FIPS 用自实现 |
| `CalcSchema` | `取解密字符串常量` | 解密嵌入资源 → 按偏移读长度+Unicode 文本 |
| `UpdateSchema` | `Base64→Unicode` | `Convert.FromBase64String` + `Encoding.Unicode.GetString` |
| `MoveSchema` | `反篡改内存补丁` | 解密补丁资源并写回自身进程内存 |
| `WriteSchema` | `取程序集路径` | 尝试 Location/CodeBase/反射属性 |
| `ConnectSchema` | `AES 解密` | 固定 Key(32B)/IV(16B)，CryptoStream 解密 |
| `InterruptSchema` | `AES 加密→Base64` | 固定 Key，IV=MD5(输入2)，加密输入1 |
| `InsertSchema` | `读文件全部字节` | FileStream 循环读 |
| `DestroySchema` 等 9 个桩 | `诱饵方法` | 返回 `{1,2}`，引用 `{11111-…}` 伪 GUID 常量 |
| `_Identifier` | `MD5 64 轮常数表` | 3614090360 起始的标准 MD5 常量 |
| `m_Setter` | `解密后的字符串池` | CalcSchema 缓存 |
| `_Database` | `反篡改已执行标志` | MoveSchema 一次性 |
| `m_Printer` | `模块基址` | `Marshal.GetHINSTANCE(模块0)` |
| `merchant/m_Connection/_Comparator/role/mock/task/_Strategy/m_Observer/_Field` | 未使用字段 | 混淆器填充 |

## 关键方法与流程
### 自实现 MD5（RevertBroadcaster）
- **流程**：1. 标准 MD5 填充（补 0x80、64 位长度）；2. 初始化 A=0x67452301、B=0xEFCDAB89、
  C=0x98BADCFE、D=0x10325476；3. 每 64 字节块执行 4 轮×16 步，经 `View/Setup/Stop/VerifySchema`
  四类轮函数与 `_Identifier` 常量表更新 A/B/C/D；4. 输出 16 字节摘要。
- **目的**：FIPS 合规环境下替代系统 MD5（`DefineSchema` 二选一）。

### 取解密字符串常量（CalcSchema(int offset)）
- **流程**：1. 首次执行：读取嵌入资源 `f2c33f7a-a406-48ab-af63-034b0bca5201` 全部字节；
  2. 构造 32 字节密钥：硬编码字节数组与**程序集公钥标记（PublicKeyToken）**异或（数组偶数下标
     写入 token 字节），16 字节 IV 同理（奇数下标）；3. 若 `offset==-1`：用 `CreateSchema()`
     AES-CBC 解密整段资源存入 `m_Setter`；4. 对解密结果再做自研流解密：每 4 字节与密钥轮转累加
     `num4 += keyDword`，并混入 `x ^= x<<5; x+=3753412436; x^=x<<6; x+=678874538; x^=x>>13;
     x+=179542347; x=522623986-x` 的伪随机流（ConfuserEx 常量加密的特征常数），得到明文；
  5. 按偏移读 `int32 长度` + Unicode 文本返回。
- **关键调用/API**：`GetManifestResourceStream`、`RijndaelManaged/AesCryptoServiceProvider`、
  `GetPublicKeyToken`、`CryptoStream`。

### 反篡改内存补丁（MoveSchema）
- **流程**：1. `_Database` 防重入；2. 读嵌入资源 `d08e40f8-0b4f-412d-8089-995ba478b812`；
  3. 构造 32 字节密钥并与 PublicKeyToken 异或（IV 先 `Array.Reverse`），自研流解密出补丁数据；
  4. `OpenProcess(PROCESS_ALL_ACCESS=56, 1, 当前PID)`；5. 取自身模块基址
     `m_Printer = Marshal.GetHINSTANCE(modules[0])`；6. 读 `count`，循环：对
     `基址+偏移` 处 `VirtualProtect(PAGE_EXECUTE_READWRITE=4)` → `WriteProcessMemory` 写 4 字节 →
     恢复原保护；7. 再处理"长度+数据块"（`VirtualProtect` → `Marshal.WriteInt32` 逐字写回）；
  8. `CloseHandle`。**效果：运行时把被加密的 IL/常量区解密写回自身内存**。
- **关键调用/API**：`VirtualProtect`、`WriteProcessMemory`、`OpenProcess`、`CloseHandle`、
  `Marshal.GetHINSTANCE`、`Marshal.WriteInt32`。

### AES 加密（InterruptSchema）
- **流程**：`Key`=32 字节固定数组；`IV` = `DefineSchema(MD5(UTF8(string_1)))`；`CreateEncryptor`
  加密 `UTF8(string_0)` 后 Base64 输出（字符串加密工具，供壳解密字符串用）。

### AES 解密（ConnectSchema）
- **流程**：固定 32B Key（174,3,233,…）/16B IV（72,84,7,…），`CreateDecryptor` 解密输入字节。

## 恶意行为 / 敏感操作
- **运行时自修改内存**：`VirtualProtect` + `WriteProcessMemory` 改写自身代码段（anti-tamper，
  对抗静态分析与内存 dump）。
- **公钥标记参与密钥派生**：密钥随程序集签名变化，导致字符串解密结果依赖原始签名（防篡改）。
- 隐式加载 `System.Core` 3.5 的 `AesCryptoServiceProvider`（FIPS 模式）。

## IOC / 硬编码
- 嵌入资源 GUID：`f2c33f7a-a406-48ab-af63-034b0bca5201`（字符串常量池）、
  `d08e40f8-0b4f-412d-8089-995ba478b812`（反篡改补丁数据）
- 固定 AES Key（32B）：`AE 03 E9 C6 6C 7C 82 B5 D1 8A 7C 36 01 3A 92 53 E3 90 23 EC D5 A9 4F 11 A0 8C C1 01 89 B5 E9 7D`
- 固定 AES IV（16B）：`48 54 07 8C 27 ED A8 4D 24 B0 BB B1 16 68 D9 CB`
- 流解密常数：`3753412436`、`678874538`、`179542347`、`522623986`（ConfuserEx 常量加密特征）
- 伪 GUID 诱饵串：`{11111-22222-40001-00001}`、`{11111-22222-40001-00002}`、`{11111-22222-50001-00001}`、
  `{11111-22222-50001-00002}`、`{11111-22222-60001-00001}`、`{11111-22222-60001-00002}`
- Win32 API：`kernel32!VirtualProtect/WriteProcessMemory/OpenProcess/CloseHandle`
