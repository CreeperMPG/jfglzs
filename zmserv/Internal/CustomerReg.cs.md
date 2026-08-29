# Internal/CustomerReg.cs — .NET Reactor 加壳运行时组件（字符串解密 / 反篡改内存补丁 / 许可证 API 桩）

## 文件作用
本文件是被 .NET Reactor（eziriz）加壳后残留的**运行时保护代码**，混入大量混淆填充方法。真实功能四块：① 手写 MD5 实现（`DefineList`/`PatchList`，含 FIPS 策略自适应）；② 解密嵌入资源字符串表（`InstantiateList`，资源 GUID `c3a777aa-...`）；③ **反篡改/解壳内存补丁**（`SetupList`，读取资源 `b215f184-...` 解密后，用 `OpenProcess`+`WriteProcessMemory` 直接改写自身模块内存）；④ IntelliLock 许可证 API 桩（`CreateList`/`ViewList` 等，含演示许可 GUID）。它不是业务逻辑，但**残留的密钥、资源名、内存补丁模式是重要的加壳 IOC**。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `CustomerReg` | Reactor 运行时：MD5、AES、字符串解密、内存补丁、许可桩 |
| `CustomerReg.AnnotationReg` | 保护器注入的标记 Attribute |
| `CustomerReg.AnnotationReg.ComposerReg<T>` | 空泛型嵌套类（混淆填充） |
| `CustomerReg.WebBrowserPermissionLevel` | 空枚举（混淆装饰，无成员） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `DefineList(object)` | MD5 哈希（手写） | 完整标准 MD5 实现，返回 16 字节 |
| `LogoutList/CountList/RateList/CancelList` | MD5 四轮函数 F/G/H/I | 对应 MD5 的 4 种非线性函数 + 轮常量 |
| `VisitList` | MD5 循环左移 | `rotate_left` |
| `m_ProxyReg` | MD5 K 常量表 | 64 个标准 MD5 常量（首项 3614090360=0xD76AA478），证实是 MD5 |
| `CallList/PushList` | FIPS 策略检测 | 反射 `CryptoConfig.AllowOnlyFipsAlgorithms` 或 `Utils.FipsAlgorithmPolicy` |
| `observerReg` | FIPS 标志 | 系统是否强制 FIPS 算法 |
| `VerifyList` | 获取对称算法 | FIPS 下用 `AesCryptoServiceProvider`，否则 `RijndaelManaged` |
| `PatchList(byte[])` | MD5 摘要 | FIPS 下走手写 `DefineList`，否则 `MD5CryptoServiceProvider` |
| `InstantiateList(int)` | 字符串表解密 | 解密内嵌资源字符串表并按下标取 UTF-16 字符串 |
| `NewList(string)` | Base64→UTF-16 解码 | 简单字符串解密助手 |
| `SetupList` | 反篡改内存补丁 | 解密资源→OpenProcess 自身→按 RVA 列表改写模块内存 |
| `StartList(Assembly)` | 取程序集文件路径 | Location / CodeBase 多级兜底 |
| `CalcList(byte[])` | AES 解密（硬编码密钥） | 用于解密（如许可证/配置载荷） |
| `IncludeList(string)` | 读文件全部字节 | FileStream 循环读取 |
| `ExcludeList(s1,s2)` | AES 加密→Base64 | IV=MD5(s2 的 UTF-16)，疑似许可激活辅助 |
| `PopList/CheckList/ListList/CustomizeList/FlushList/WriteList` | 空桩 | 全部返回 null（壳剥离后的残留） |
| `CreateList/ViewList/ManageList/AwakeList/RegisterList/StopList` | 许可证 API 桩 | 返回 `{1,2}`，内含 IntelliLock 演示许可 GUID |
| `_ManagerReg` | 自身模块基址 | `GetHINSTANCE(GetModules()[0])` |
| `mapReg` | 解密后字符串表 | 一次性解密缓存 |
| `m_PredicateReg` | SetupList 执行标志 | 防重复执行 |
| `m_UtilsReg/_ParamReg/containerReg/mapperReg/registryReg/_SystemReg/_FieldReg/_MockReg/identifierReg` | 未使用字段 | 混淆填充字段 |

## 关键方法与流程

### MD5 手写实现（原名 `DefineList` + 轮函数）
- **职责**：对任意字节串计算标准 MD5。
- **流程**：1. 按 MD5 规范填充（0x80 补位 + 64 位长度）；2. 初始化 A=0x67452301、B=0xEFCDAB89、C=0x98BADCFE、D=0x10325476；3. 每 64 字节一块，4 轮 × 16 步调用 `LogoutList`(F)、`CountList`(G)、`RateList`(H)、`CancelList`(I)；4. 累加输出 16 字节（小端）。
- **关键调用/API**：`BitConverter`、`m_ProxyReg` 常量表。
- **意义**：加壳器用自实现 MD5 摆脱对系统加密 API 的依赖（也用于生成嵌入资源名哈希）。

### FIPS 策略检测（原名 `CallList`/`PushList`）
- **流程**：首次调用反射 `System.Security.Cryptography.CryptoConfig.get_AllowOnlyFipsAlgorithms`；失败则反射 `Utils.get_FipsAlgorithmPolicy == 1`；结果存 `observerReg`。

### 字符串表解密（原名 `InstantiateList(int)`）
- **职责**：解密嵌入资源 `c3a777aa-b0d9-45b5-9f38-a4546dce2557` 得到"长度前缀 + UTF-16 字符串"表，按下标返回字符串。
- **流程**：1. 读入资源全部字节；2. 构造 32 字节密钥（硬编码数组**逐字节 XOR 程序集公钥令牌**）；3. 构造 16 字节 IV（硬编码数组，奇数位填入公钥令牌 8 字节）；4. 若首次：`VerifyList()` 取 AES/Rijndael，CBC 模式解密整个资源 → `mapReg`；5. 再对解密结果跑一段**自定义流解密**（`num14 ^= num14<<5; +=3753412436; ^=<<6; +=678874538; ^=>>13; +=179542347; =522623986-num14` 的 xorshift 式密钥流），得到明文表；6. 读取偏移 `int_0` 处的 int 长度 → 复制 → `Encoding.Unicode.GetString`。
- **关键调用/API**：`GetManifestResourceStream`、`CreateDecryptor`、`CryptoStream`、`BitConverter`。

### 反篡改内存补丁（原名 `SetupList`）— 加壳残留高危逻辑
- **职责**：解密资源 `b215f184-b268-4760-a367-fb3a9e128901`，对**自身进程内存**做补丁（.NET Reactor 反篡改/解壳的托管侧）。
- **流程**：1. 读资源→构造 32 字节密钥（硬编码数组 XOR 公钥令牌，IV 先 `Array.Reverse` 再填令牌）；2. 同样的自定义流解密得补丁数据；3. `OpenProcess(56 /*PROCESS_ALL_ACCESS*/, 1, GetCurrentProcess().Id)` 打开自身；4. `_ManagerReg = GetHINSTANCE(模块0)` 取自身基址；5. 读 RVA 列表：每个 RVA → `VirtualProtect(基址+RVA, 4, 4 /*PAGE_EXECUTE_READWRITE*/, ref old)` → `WriteProcessMemory` 写 4 字节 → 恢复保护属性；6. 随后再读 `(RVA, 个数)` 对，`VirtualProtect` 后 `Marshal.WriteInt32` 批量写回；7. `CloseHandle`。
- **关键调用/API**：`kernel32!OpenProcess/WriteProcessMemory/VirtualProtect/CloseHandle`、`Marshal.GetHINSTANCE`、`Marshal.WriteInt32`。
- **危害性**：这是**进程自我修改内存**的反调试/反篡改机制残留，运行期会改写自身 IL 或校验和，干扰内存取证与补丁分析。

### 硬编码密钥解密/加密（`CalcList` / `ExcludeList`）
- `CalcList`：Rijndael/AES 解密，密钥 32 字节 `05 50 DC 8D 55 71 78 92 B0 96 DC 9C D9 43 3C D2 8F C3 28 D0 0D B5 20 E0 5B 77 20 39 CD F0 C2 CE`，IV 16 字节 `07 13 8F A1 51 09 0A 1B F0 49 4F 2B 8B 54 1A 2D`。
- `ExcludeList(s1, s2)`：对 s1 的 UTF-16 字节做 AES 加密（密钥 `52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58`），IV = `PatchList(UTF16(s2))`（即 s2 的 MD5），输出 Base64。

### 许可证桩（`CreateList` 等 6 个方法）
- 每个先取一个 IntelliLock 演示 GUID 的长度（`"{11111-22222-40001-00001}"` 等），再返回 `{1,2}`。为 .NET Reactor/IntelliLock 许可证校验 API 被脱壳后的空壳。

### 其他
- `StartList`：多级兜底返回程序集文件路径；`IncludeList`：读文件字节；`NewList`：Base64→UTF-16。
- 若干 `*Parser()` 恒真/恒假方法：ConfuserEx 混淆填充（如 `DestroyParser→true`、`CalcParser→false`）。

## 恶意行为 / 敏感操作
- **进程自修改内存**（`SetupList`）：OpenProcess 自身 + VirtualProtect + WriteProcessMemory，属于加壳器反篡改机制，会干扰安全软件与取证。
- **隐藏字符串**：全部字符串经加密资源表（GUID 资源名）运行时解密，静态字符串少。
- **算法自实现**：手写 MD5 规避 API 钩子/检测。
- **FIPS 规避**：按系统策略自适应选算法，保证解密链在任何环境可运行。

## IOC / 硬编码
- 嵌入资源名（GUID）：`c3a777aa-b0d9-45b5-9f38-a4546dce2557`（字符串表）、`b215f184-b268-4760-a367-fb3a9e128901`（补丁数据）
- 解密密钥结构：32 字节硬编码数组 ⊕ 程序集公钥令牌；IV 16 字节内嵌令牌
- 硬编码 AES 密钥（CalcList）：`05 50 DC 8D 55 71 78 92 B0 96 DC 9C D9 43 3C D2 8F C3 28 D0 0D B5 20 E0 5B 77 20 39 CD F0 C2 CE`；IV：`07 13 8F A1 51 09 0A 1B F0 49 4F 2B 8B 54 1A 2D`
- 硬编码 AES 密钥（ExcludeList）：`52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58`
- 自定义流解密魔数：3753412436、678874538、179542347、522623986
- IntelliLock 演示许可 GUID：`{11111-22222-40001-00001}`、`{11111-22222-40001-00002}`、`{11111-22222-50001-00001}`、`{11111-22222-50001-00002}`、`{11111-22222-60001-00001}`、`{11111-22222-60001-00002}`
- 原生 API：`kernel32!OpenProcess(PROCESS_ALL_ACCESS=0x38)`、`WriteProcessMemory`、`ReadProcessMemory`、`VirtualProtect(PAGE_EXECUTE_READWRITE=0x04)`、`CloseHandle`；`Marshal.GetHINSTANCE`
- 保护器指纹：.NET Reactor / IntelliLock（eziriz）运行时 + ConfuserEx 风格改名
