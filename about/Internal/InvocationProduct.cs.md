# InvocationProduct.cs — .NET Reactor 加壳保护代码（加密字符串表解密 + 内存自修补反篡改 + FIPS 兼容 MD5 + AES 加解密工具）

## 文件作用
本文件是 **.NET Reactor（Eziriz）加壳运行时**的反编译残留，是"壳"而非业务逻辑，但极具 IOC 价值：
1. `ManageReponse`：读取嵌入资源 `002a5299-65c7-44dc-af9b-810aaba51bee`，解密后通过 `OpenProcess/WriteProcessMemory/VirtualProtect` **向自身进程已加载模块的内存写入补丁**（反篡改/运行时解包，按 RVA 表打补丁）；
2. `LoginReponse`：读取嵌入资源 `5182af00-bdc6-4082-952b-72890357d973`（AES-CBC 解密 + 自定义"搅乱"变换），还原被加密的**字符串常量表**（长度前缀 + UTF-16 字符串），供壳还原被混淆字符串；
3. `InitReponse` + `PushReponse/InsertReponse/WriteReponse/DisableReponse/ReadReponse`：**手工实现的 MD5**（含标准 K 常量表 `databaseProduct`），用于 FIPS 模式下替代 `MD5CryptoServiceProvider`；
4. 其余为加解密/文件读取/许可证桩等辅助。所有方法名被混淆为 `*Reponse`，并塞入大量 `*Listener` 恒值桩方法。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| InvocationProduct | Reactor 壳运行时：资源解密、内存自修补、MD5、AES 工具 |
| StrategyProduct : Attribute | 混淆器添加的自定义特性（无实义） |
| ProducerProduct\<T\> | 空泛型占位类 |
| ContainerProduct | 空 Flags 枚举（混淆残留） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `InitReponse(byte[])` | 手工 MD5 计算 | 返回 16 字节摘要 |
| `PushReponse/InsertReponse/WriteReponse/DisableReponse` | MD5 轮函数 F/G/H/I | 四轮压缩函数 |
| `ReadReponse` | MD5 循环左移 | `rotl` |
| `databaseProduct` | MD5 K 常量表（64 项） | 标准 MD5 常量 |
| `FillReponse/PrepareReponse` | 检测 FIPS 策略 | 反射读 `CryptoConfig.AllowOnlyFipsAlgorithms` / `Utils.FipsAlgorithmPolicy` |
| `LogoutReponse` | 创建 AES/Rijndael 算法实例 | FIPS 时用 AesCryptoServiceProvider，否则 RijndaelManaged |
| `CountReponse(byte[])` | MD5 哈希（FIPS 兼容） | FIPS 下走手工实现 |
| `LoginReponse(int)` | 解密加密字符串表并取第 i 条字符串 | 资源 `5182af00-...` + AES-CBC + 自研 XOR 变换 + 长度前缀读取 |
| `ConnectReponse(string)` | Base64→UTF-16 解码 | 解密后的常量字符串还原工具 |
| `ManageReponse()` | 内存反篡改/解包补丁 | 资源 `002a5299-...` 解密后 WriteProcessMemory 到自身模块 |
| `IncludeReponse(Assembly)` | 获取程序集文件路径 | 多重 fallback |
| `TestProduct(string)` | 读文件为字节数组 | — |
| `RunProduct(byte[])` | AES-CBC 解密（硬编码 Key/IV） | — |
| `DefineProduct(s1,s2)` | AES 加密→Base64（Key 硬编码，IV=MD5(s2)） | — |
| `VisitProduct/RegisterProduct/MoveProduct/InvokeProduct/EnableProduct/ConcatProduct` | IntelliLock 许可证桩 | 返回 {1,2}，含 `{11111-22222-*}` GUID 占位 |
| `PopProduct/ResetProduct/CollectProduct/PatchProduct/CustomizeProduct/SearchProduct` | 空桩 | return null |
| `_ResolverProduct/_IdentifierProduct` | FIPS 检测标志 | — |
| `infoProduct` | 解密后的字符串表缓存 | — |
| `_WatcherProduct` | 自身模块基址（HINSTANCE） | — |
| `schemaProduct` | ManageReponse 一次性标志 | — |
| `StrategyProduct/ProducerProduct/ContainerProduct` | 混淆残留 | 无实义 |
| `*Listener` 系列 | 混淆填充桩 | 恒 true/false |

## 关键方法与流程

### 内存自修补（ManageReponse）
- **流程**：
  1. `schemaProduct` 一次性保护；
  2. 读嵌入资源 `002a5299-65c7-44dc-af9b-810aaba51bee`；
  3. 用硬编码 32 字节密钥（与程序集公钥 Token 异或）做自定义逐 4 字节"搅乱"解密；
  4. `OpenProcess(PROCESS_ALL_ACCESS=56, 1, 当前PID)` 打开**自身进程**；
  5. `_WatcherProduct = Marshal.GetHINSTANCE(模块[0])` 得到模块基址；
  6. 读取补丁表：先按 `(RVA, value)` 对 4 字节位置 `VirtualProtect(可写) → WriteProcessMemory → VirtualProtect(还原)`；再按 `(RVA, 长度, 数据)` 批量写入；最后 `CloseHandle`。
- **语义**：运行时解包/防篡改——被加密的代码/常量在运行时原地还原到内存镜像。
- **关键调用/API**：`OpenProcess`、`WriteProcessMemory`、`VirtualProtect`、`CloseHandle`、`Marshal.GetHINSTANCE`。

### 加密字符串表解密（LoginReponse）
- **流程**：
  1. 读资源 `5182af00-bdc6-4082-952b-72890357d973`；
  2. 构造 32 字节密钥：硬编码字节经公钥 Token 异或（16 字节 IV = 公钥 Token）；
  3. AES-CBC（`LogoutReponse`）解密 → 再执行逐 4 字节"搅乱"变换（`num4` 累加 + 固定常量 3753412436/678874538/179542347/522623986 的位运算）；
  4. 结果缓存到 `infoProduct`；
  5. 按 `int_0` 偏移读取 `length` 前缀的 UTF-16 字符串返回。
- **语义**：Reactor 的字符串加密还原——程序内被加密的字符串（如注册表路径等）通过该表在运行时恢复。

### FIPS 兼容 MD5（InitReponse 系列）
- **流程**：标准 MD5：填充（0x80 + 长度 64 位）、16 字分块、4 轮 64 步压缩（K 表 `databaseProduct`）、结果小端输出 16 字节。`CountReponse` 在非 FIPS 环境直接走 `MD5CryptoServiceProvider`。

### 加密/解密工具
- `RunProduct`：硬编码 32 字节 Key `{14,38,36,62,87,26,83,240,...}`、16 字节 IV `{214,212,210,126,...}` 的 AES-CBC 解密。
- `DefineProduct`：硬编码 Key `{82,102,104,110,...}`，IV = `MD5(UTF16(s2))`，AES 加密后 Base64。
- `ConnectReponse`：Base64 → UTF-16 明文。

## 恶意行为 / 敏感操作
- **自进程内存写入（WriteProcessMemory + VirtualProtect）**：运行时修改自身已加载模块代码——反分析/反篡改特征。
- **内嵌加密资源 + 公钥 Token 派生密钥**：隐藏字符串常量与代码，对抗静态分析。
- 手工实现 MD5 绕过 FIPS 限制（保证在受限策略主机上仍可完成哈希）。

## IOC / 硬编码
- 嵌入资源：`5182af00-bdc6-4082-952b-72890357d973`（加密字符串表）、`002a5299-65c7-44dc-af9b-810aaba51bee`（反篡改补丁数据）
- API：`kernel32.dll`：`OpenProcess`、`WriteProcessMemory`、`ReadProcessMemory`、`VirtualProtect`、`CloseHandle`
- 许可证占位 GUID：`{11111-22222-40001-00001}`、`{11111-22222-40001-00002}`、`{11111-22222-50001-00001}`、`{11111-22222-50001-00002}`、`{11111-22222-60001-00001}`、`{11111-22222-60001-00002}`（IntelliLock API 占位）
- AES Key（DefineProduct）：`52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58`
- AES Key（RunProduct）：`0E 26 24 3E 57 1A 53 F0 22 47 77 56 90 98 E4 1F 27 2C CB E9 3E 9E 03 22 0E 21 0F 51 E5 C1 D5 7F`；IV：`D6 D4 D2 7E 88 A2 97 8E AA 24 9A 49 DA A8 29 4E`
- 搅乱常量：`3753412436`、`678874538`、`179542347`、`522623986`
