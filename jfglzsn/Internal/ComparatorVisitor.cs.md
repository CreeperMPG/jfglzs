# Internal/ComparatorVisitor.cs — .NET Reactor 壳运行时（字符串解密、资源解密、进程内存补丁、MD5/AES 工具）

## 文件作用
这是 **.NET Reactor 加壳运行时支撑代码**（反编译保留的壳层）。职责包括：1) 解密嵌入资源中的**字符串表**（`AssetVisitor`，按长度前缀读出全部明文字符串，供被混淆代码使用）；2) **内存补丁自身映像**（`ChangeVisitor`：解密补丁资源后用 `OpenProcess`+`WriteProcessMemory` 改写当前进程模块内指定偏移的 int32，恢复被 Reactor 改写的常量/方法体）；3) 提供 MD5（含 FIPS 模式自实现）与 AES/Rijndael 加解密工具（`RevertConfig`/`CloneVisitor`/`FindVisitor`/`ViewVisitor`/`DefineVisitor`）；4) `AssemblyResolve` 支持下的依赖程序集嵌入资源解密（配合 GetterVisitor）；5) 大量壳授权桩方法（返回固定值/空）。类内大量字段（`m_CallbackVisitor`、`singletonVisitor`、`serviceVisitor`、`m_ThreadVisitor`、`m_InfoVisitor` 等）在静态构造器里初始化后未见使用，属壳/混淆器保留的伪状态。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `ComparatorVisitor` | 壳运行时：解密/解包/内存补丁/哈希 |
| `ComparatorVisitor.ProducerVisitor`（Attribute） | 壳标记属性（Obfuscation 残留） |
| `ComparatorVisitor.ProducerVisitor.ComposerVisitor<T>` | 壳桩类 |
| `ComparatorVisitor.LineBreakBoundaryConditions`（枚举） | 空枚举（混淆残留） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `RevertConfig` | 自实现 MD5（FIPS 模式用） | 标准 MD5 算法手写版 |
| `UpdateVisitor/PostVisitor/PublishVisitor/SortVisitor/PopVisitor` | MD5 轮函数 | F/G/H/I 轮 + 循环左移 |
| `CallVisitor` | 是否处于 FIPS 模式 | 惰性检测，缓存到 `m_MockVisitor` |
| `ManageVisitor` | 检测系统 FIPS 策略 | 反射读 `CryptoConfig.AllowOnlyFipsAlgorithms` / `Utils.FipsAlgorithmPolicy` |
| `FindVisitor` | 获取对称算法实例 | FIPS 下用 AesCryptoServiceProvider，否则 RijndaelManaged |
| `CloneVisitor` | MD5 哈希（FIPS 自适应） | 非 FIPS 用 MD5CryptoServiceProvider，FIPS 用自实现 |
| `AssetVisitor` | 解密字符串表 | 解密资源 `6693dd3c-...` 并按长度前缀取字符串 |
| `AddVisitor` | Base64 → Unicode 字符串 | 简单解码 |
| `ChangeVisitor` | 壳内存补丁（自改写映像） | 解密资源 `febbdcee-...` 后 WriteProcessMemory 打补丁 |
| `CountVisitor` | 获取程序集路径 | Location/CodeBase 多路尝试 |
| `RateVisitor` | 读取文件全部字节 | FileStream 循环读 |
| `ViewVisitor` | AES 解密（固定 Key/IV） | 数据解密工具 |
| `DefineVisitor` | AES 加密（IV=MD5(口令)） | 加密工具，疑似网络/配置加密 |
| `DestroyVisitor/PatchVisitor/ExcludeVisitor/SetupVisitor/InstantiateVisitor/RestartVisitor` | 空桩 | 返回 null（壳/混淆残留） |
| `PushVisitor/VerifyVisitor/ValidateVisitor/EnableVisitor/CheckVisitor/CalculateVisitor` | 授权桩 | 引用 `{11111-22222-40001-00001}` 等假 GUID，返回 `{1,2}` |
| `factoryVisitor` | MD5 K 常量表 | 64 个 uint（标准 MD5 正弦常量） |
| `ProducerVisitor`/`ComposerVisitor` 方法 | 桩 | 恒 true/false |

## 关键方法与流程

### 自实现 MD5（原名 `RevertConfig(byte[])`）
- **职责**：在 FIPS 模式（系统禁止 MD5CryptoServiceProvider）下手工计算 MD5。
- **流程**：1. 计算填充长度（448−len×8 mod 512），末尾补 `0x80` 与 64 位长度；2. 初始化 A=0x67452301、B=0xEFCDAB89、C=0x98BADCFE、D=0x10325476；3. 每 64 字节一块：加载 16 个 uint（小端），保存 A'B'C'D'，依次执行 64 轮 `UpdateVisitor`（F 轮，左移 7/12/17/22）、`PostVisitor`（G 轮，5/9/14/20）、`PublishVisitor`（H 轮，4/11/16/23）、`SortVisitor`（I 轮，6/10/15/21），每轮加 `factoryVisitor`（MD5 K 常量）；4. 累加回 A/B/C/D；5. 小端输出 16 字节。
- **关键调用/API**：`factoryVisitor`（MD5 K 表，64 项，即 0xD76AA478...0xA4BEEA44 序列）、`PopVisitor`（`x >> (32-s) | x << s` 循环左移）。

### 检测 FIPS 模式（原名 `CallVisitor`/`ManageVisitor`）
- **流程**：首次调用时反射 `System.Security.Cryptography.CryptoConfig.AllowOnlyFipsAlgorithms`；失败则反射非公开 `Utils.FipsAlgorithmPolicy == 1`；结果缓存于 `m_MockVisitor`。
- **关键调用/API**：`typeof(RijndaelManaged).Assembly` 反射。

### 获取对称算法实例（原名 `FindVisitor`）
- **流程**：FIPS → `System.Core 3.5` 的 `AesCryptoServiceProvider`（失败回退 RijndaelManaged）；非 FIPS → `new RijndaelManaged()`。
- **关键调用/API**：`Assembly.Load("System.Core, Version=3.5.0.0, ...")`、`Activator.CreateInstance`。

### MD5 哈希（原名 `CloneVisitor(byte[])`）
- **流程**：非 FIPS → `new MD5CryptoServiceProvider().ComputeHash()`；FIPS → `RevertConfig()`。
- **使用方**：GetterVisitor 用它把程序集名哈希成嵌入资源名；DefineVisitor 用它把口令哈希成 IV。

### 解密字符串表（原名 `AssetVisitor(int 索引)`）
- **职责**：懒加载解密嵌入资源 `6693dd3c-5579-48c1-9ace-94458d4f769b`，得到整块解密后字节流；调用方传 int 偏移，函数读 4 字节长度 + Unicode 字符串返回。
- **流程**：1. 首次：读入资源字节；2. 构造 32 字节密钥（初始字节数组经**与程序集 PublicKeyToken 逐字节异或**修正，注意源码里大量 `array2[i]=X` 重复赋值是反编译器伪影，最终值以最后一次赋值+异或为准）与 16 字节 IV（同样混入 PublicKeyToken 奇偶位）；3. `FindVisitor()` 取算法，CBC 模式 `CreateDecryptor(Key, IV)` 解密资源 → 得到解密数据；4. 对解密数据做自定义 32 位分组异或流变换（`num4` 累加 4 字节块 + 线性同余式 `x^=x<<5; x+=3753412436; x^=x<<6; x+=678874538; x^=x>>13; x+=179542347; x=522623986-x`），还原最终字符串表；5. 之后每次调用：`BitConverter.ToInt32(表, 偏移)` 取长度 → 复制 UTF-16 字符串。
- **关键调用/API**：`GetManifestResourceStream("6693dd3c-5579-48c1-9ace-94458d4f769b")`、`GetPublicKeyToken()`、`CryptoStream`。
- **说明**：这是被混淆代码中大量"明文常量"的真正来源（dnSpy 已解析），也是字符串隐藏机制。

### 壳内存补丁（原名 `ChangeVisitor()`，`[ProducerVisitor]` 标记，静态构造触发）
- **职责**：解密补丁资源 `febbdcee-0330-4e15-8e08-e279bd6f0d1e`，把补丁数据写进**当前进程自己的模块映像**，还原 Reactor 加壳时改写的 4 字节常量与方法体数据。
- **流程**：1. 一次性标志 `m_PublisherVisitor` 防重入；2. 读资源 → 构造密钥/IV（与 PublicKeyToken 异或修正，IV 先 `Array.Reverse`）→ 同样的 32 位分组异或流还原出补丁字节流；3. `OpenProcess(0x38 /*PROCESS_ALL_ACCESS*/, false, 当前PID)` 打开自己；4. `_QueueVisitor = Marshal.GetHINSTANCE(模块0)` 即模块基址；5. 读 int32 补丁条目数，逐条：目标偏移 = 基址 + 文件内偏移 → `VirtualProtect(..., PAGE_READWRITE)` → `WriteProcessMemory` 写 4 字节 → 还原保护属性；6. 后续流式条目：偏移 + 数量 → 同样改保护后 `Marshal.WriteInt32` 批量写；7. `CloseHandle`。
- **关键调用/API**：`OpenProcess`、`WriteProcessMemory`、`VirtualProtect`、`CloseHandle`、`GetHINSTANCE`（kernel32/ntdll P/Invoke）。
- **IOC**：嵌入资源名 `febbdcee-0330-4e15-8e08-e279bd6f0d1e`。

### 获取程序集路径（原名 `CountVisitor`）
- **流程**：依次尝试 `Assembly.Location`、`CodeBase`（去 `file:///` 前缀）、反射 `Location` 属性；都失败返回空串。
- **关键调用/API**：`File.Exists` 校验。

### 读取文件（原名 `RateVisitor`）
- **流程**：`FileStream(Open, Read, FileShare.Read)` 循环读满整个文件返回字节数组。

### AES 解密（原名 `ViewVisitor(byte[])`）
- **流程**：`FindVisitor()` 实例，固定 Key（32 字节：`14 EF 46 3A 70 2C 3B 90 43 29 08 23 78 A6 D9 27 AD F6 7C 70 86 DF C3 69 DE 8F 4E 4B D3 00 58 E2`）、固定 IV（16 字节：`9B BD 27 94 F0 5F 3E 48 17 6D B2 29 EB C3 3D B0`），`CreateDecryptor` 解密。
- **用途推测**：解密壳内嵌数据/网络载荷（本文件内未见调用者，可能被壳动态调用）。

### AES 加密（原名 `DefineVisitor(明文, 口令)`）
- **流程**：`Encoding.Unicode.GetBytes(明文)`；固定 32 字节 Key（`52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58`）；IV = `CloneVisitor(UnicodeBytes(口令))`（即口令的 MD5）；`CreateEncryptor` → 输出 Base64。
- **用途推测**：口令派生加密（网络通信/配置加密工具）。

### 授权桩方法（`PushVisitor` 等 7 个）
- **流程**：取假 GUID 字符串（`{11111-22222-40001-00001}` ~ `{11111-22222-60001-00002}`）长度后丢弃，返回 `new byte[]{1,2}`。纯壳授权检测桩，无实际逻辑。

## 恶意行为 / 敏感操作
- **进程内存自改写**：OpenProcess + VirtualProtect + WriteProcessMemory 对自身模块写补丁（绕过静态检测、还原被加壳篡改的代码/常量——反分析对抗）。
- **字符串/资源加密隐藏**：字符串表整体加密嵌入，运行时解密（对抗字符串特征检测）。
- **FIPS 规避**：系统禁止 MD5 时用自实现 MD5 继续工作。
- 反射调用非公开 API（`Utils.FipsAlgorithmPolicy`）。

## IOC / 硬编码
- **嵌入资源（解密对象）**：`6693dd3c-5579-48c1-9ace-94458d4f769b`（字符串表）、`febbdcee-0330-4e15-8e08-e279bd6f0d1e`（补丁数据）。
- **固定密钥/IV**：见 `ViewVisitor`/`DefineVisitor` 的 Key/IV 字节数组（十六进制已列于上文）。
- **假授权 GUID**：`{11111-22222-40001-00001}` 等 7 个。
- **MD5 K 常量表**：`factoryVisitor` 64 项标准 MD5 常量（0xD76AA478 开头）。
- **模块名**：`System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089`。
