# OrderConfig.cs — .NET Reactor 加壳运行时：反篡改补丁、字符串解密、AES/MD5 加密工具

## 文件作用
本文件是 .NET Reactor 保护壳注入的运行时支撑代码（加壳器残留），不参与卸载业务逻辑，但承载：
1. **反篡改（Anti-Tamper）**：`AddUtils` 用 `OpenProcess + VirtualProtect + WriteProcessMemory` 在内存中修补/改写当前进程的方法体；
2. **加密字符串解密**：`ResetUtils` 解密嵌入资源中的混淆字符串（本程序集内"随机进程名"等字符串即由此还原）；
3. **MD5/AES 工具**：FIPS 模式自适应（`SelectUtils` 自带 MD5 实现）、`PublishConfig` 字符串加密、`EnableConfig` 解密。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| OrderConfig | Reactor 运行时（加密/反篡改/字符串解密） |
| PageConfig 特性 | Reactor 标记特性（标识被保护成员） |
| RepositoryConfig\<T\> | 空壳桩类 |
| ImporterConfig 枚举 | 空壳桩枚举 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `SelectUtils` | 自实现 MD5 | 标准 MD5 压缩函数（含 64 项正弦常数表 `resolverConfig`） |
| `GetUtils/DisableUtils/CalculateUtils/ConcatUtils` | MD5 四轮 F/G/H/I 变换 | |
| `ForgotUtils` | MD5 循环左移 | |
| `FindUtils` | 探测 FIPS 策略 | 反射读 CryptoConfig.AllowOnlyFipsAlgorithms / Utils.FipsAlgorithmPolicy |
| `CancelUtils` | FIPS 是否启用缓存 | |
| `AwakeUtils` | 获取对称算法实例 | FIPS 时用 AesCryptoServiceProvider，否则 RijndaelManaged |
| `ReflectUtils` | 计算 MD5 摘要 | 兼容 FIPS 的自选实现 |
| `ResetUtils` | 解密嵌入字符串资源 | 读取资源 `c0682d03-...`，AES-CBC 解密 + 自定义流解密，按长度读取 Unicode 字符串 |
| `PostUtils` | Base64 → Unicode 字符串 | |
| `AddUtils` | 反篡改补丁 | 解密资源 `0d7bf7c5-...` 后改写进程内存 |
| `EnableConfig` | AES 解密（硬编码密钥） | |
| `PublishConfig` | AES 加密字符串 | Key 硬编码，IV=MD5(盐)，输出 Base64 |
| `FillUtils` | 获取程序集路径 | Location / CodeBase 回退 |
| `RevertUtils` | 读文件全部字节 | |
| `PopConfig/ListConfig/VerifyConfig/MapConfig/CallConfig/IncludeConfig` | 假方法桩 | 返回 `{1,2}`，混淆噪声 |
| `DestroyConfig/StopConfig/SortConfig/CollectConfig/FlushConfig/LogoutConfig` | 假方法桩 | 返回 null |
| `_CreatorConfig` | 模块基址（HINSTANCE） | 反篡改用 |
| `resolverConfig` | MD5 正弦常数表 | |

## 关键方法与流程

### 反篡改补丁（原名 AddUtils）
- **流程**：
  1. 读嵌入资源 `0d7bf7c5-ac22-45a5-a386-00782fb0dad7`。
  2. 用 32 字节密钥流与 16 字节 IV（均与程序集公钥令牌异或、IV 反转）解密（AES/Rijndael CBC），再做一轮自定义流解密得到补丁数据。
  3. `OpenProcess(56=PROCESS_ALL_ACCESS, 1, 当前PID)` 打开自身进程。
  4. 按补丁数据中的 RVA 偏移：`VirtualProtect` 改页权限 → `WriteProcessMemory`/`Marshal.WriteInt32` 写入 → 恢复权限（典型 Reactor 内存补丁：IAT 修正/校验和自校验）。
  5. `CloseHandle`。异常被吞掉。
- **关键调用/API**：`OpenProcess`、`WriteProcessMemory`、`ReadProcessMemory`、`VirtualProtect`、`CloseHandle`、`Marshal.GetHINSTANCE`。

### 解密嵌入字符串（原名 ResetUtils）
- **流程**：读资源 `c0682d03-f5b2-467a-9e64-46ffd27adaa2` → 用内嵌密钥（异或公钥令牌）AES-CBC 解密 → 自定义 32 位滚动 XOR 流解密 → 缓存；按 `int_0` 偏移读 4 字节长度 + Unicode 字符串返回。这是 Reactor "字符串加密" 的还原器，供被混淆字符串动态解码。

### MD5 / 加密工具
- `SelectUtils`：标准 MD5（消息填充、64 轮、小端输出）。
- `ReflectUtils`：非 FIPS 走 `MD5CryptoServiceProvider`，FIPS 走自实现。
- `AwakeUtils`：FIPS 环境用 AES（System.Core 3.5 AesCryptoServiceProvider），否则 RijndaelManaged。
- `EnableConfig`：硬编码 Key（146,223,56,9,...）与 IV（202,43,33,137,...）AES-CBC 解密。
- `PublishConfig(str, salt)`：Key `{82,102,104,110,32,77,24,34,...}`，IV = MD5(UTF16(salt))，AES 加密后 Base64 —— 用于运行时生成加密串。
- `FillUtils`：程序集 Location 多级回退解析。

## 恶意行为 / 敏感操作
- 反篡改：运行时改写自身进程内存（WriteProcessMemory），干扰内存补丁/脱壳/调试。
- 全套 CryptoAPI/kernel32 P/Invoke 用于加壳自保护。
- 本身无害，属于加壳器（.NET Reactor）保护机制，非业务恶意行为。

## IOC / 硬编码
- 嵌入资源名：`c0682d03-f5b2-467a-9e64-46ffd27adaa2`（加密字符串）、`0d7bf7c5-ac22-45a5-a386-00782fb0dad7`（反篡改补丁）
- AES 密钥（EnableConfig）：`92 DF 38 09 B9 BC 16 01 85 47 3D 17 E3 20 B0 25 61 56 C5 9E C4 69 19 30 7D 84 30 57 AC E9 C4 76`
- AES IV（EnableConfig）：`CA 2B 21 89 D6 BC 83 37 69 A8 72 E5 54 03 93 27`
- AES 密钥（PublishConfig）：`52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58`
- MD5 常数表首值：3614090360（0xD76AA478，确认标准 MD5）
- 假 GUID 串：`{11111-22222-40001-00001}` 等（桩方法，无意义）
