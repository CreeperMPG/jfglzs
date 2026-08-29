# Internal/TestMock.cs — 加壳运行时：字符串解密、模块初始化内存补丁、MD5/AES 密码学原语

## 文件作用
这是 **.NET Reactor / IntelliLock（Eziriz）加壳运行时**，混杂 ConfuserEx 风格重命名。核心功能：
1. **字符串解密表** `QueryMock(offset)`：读取嵌入资源 `dfa6a901-446d-4781-bab5-dec8b23ded66`，用 AES/Rijndael(CBC) + 自定义流密钥变换解密后按偏移读出明文字符串（壳替换了程序里所有字符串字面量为 `QueryMock(n)` 调用）；
2. **模块初始化器** `InterruptMock`（标 `[MethodMock]` 特性，即 .cctor 替代）：解密嵌入资源 `e6ab89dd-cbca-49e1-92f5-a278f1600946` 后，用 `OpenProcess + VirtualProtect + WriteProcessMemory` **直接改写自身模块内存**（打补丁/脱壳桩，属典型的 Reactor 内存补丁与反篡改）；
3. 密码学原语：手写 **MD5**（PrintProperty + 四个轮函数 + 常量表 m_AttributeMock）、FIPS 策略检测（RevertProperty/InstantiateMock）、AES/Rijndael 选择（MoveMock）、`SetMock`（MD5 摘要，FIPS 下用手写版）、`VerifyMock`（固定 Key + MD5(参数) 作 IV 的加密→Base64，疑似注册码生成）、`DeleteMock`（固定 Key/IV 解密）；
4. `WriteMock`：Base64→Unicode 解码（另一条字符串解密路径）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| TestMock | 壳运行时（解密/补丁/哈希/加解密） |
| TestMock.MethodMock : Attribute | 标记模块初始化器的特性（壳生成） |
| TestMock.MethodMock.InitializerMock<T> | 空泛型占位 |
| TestMock.AuthorMessageKind | 空枚举占位 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| PrintProperty(data) | MD5_手动实现 | 标准 MD5 主循环（初值 1732584193…，64 轮） |
| CompareProperty/OrderProperty/FindProperty/DefineProperty | MD5 轮函数 F/G/H/I | 与 m_AttributeMock 常量表配合 |
| RateProperty(v,n) | 循环左移 | v << n \| v >> (32-n) |
| RevertProperty() | IsFipsEnabled_是否强制 FIPS | 反射 CryptoConfig.AllowOnlyFipsAlgorithms / Utils.FipsAlgorithmPolicy |
| InstantiateMock() | 探测 FIPS 策略 | 反射 System.Security.Cryptography 内部类 |
| MoveMock() | GetSymmetricAlgo_取对称算法 | FIPS→AesCryptoServiceProvider，否则 RijndaelManaged |
| SetMock(bytes) | MD5 摘要 | FIPS 时用手写 MD5 |
| QueryMock(int) | DecryptString_字符串解密 | 资源+解密+按偏移读字符串 |
| WriteMock(b64) | Base64ToUnicode | Convert.FromBase64String → UTF-16 |
| InterruptMock() | ModuleInit_内存补丁 | 解密资源并 WriteProcessMemory 改写自身代码 |
| CallMock(asm) | GetAssemblyPath | 依次试 Location/CodeBase |
| SelectMock(path) | ReadFileBytes | FileStream 全读 |
| DeleteMock(bytes) | Rijndael 解密（固定 Key/IV） | 见 IOC |
| VerifyMock(s1,s2) | EncryptWithFixedKey | 固定 32 字节 Key，IV=SetMock(UTF16(s2))，输出 Base64 |
| CalcMock/CheckMock/ForgotMock/MapMock/CancelMock/DestroyMock | 占位方法 | 内含 IntelliLock 特征 GUID "{11111-22222-40001-00001}" 等，返回 {1,2} |
| method_0/NewMock/RestartMock/PublishMock/CreateMock/VisitMock/ConnectMock | 空占位 | return null |
| m_AttributeMock | MD5 常量表 | 64 个标准 MD5 常量（3614090360=0x67452301 系） |
| _StrategyMock | 解密后的字符串表缓存 | QueryMock 惰性解密一次 |
| _RuleMock/_PrinterMock | FIPS 探测标志/结果 | |
| _RoleMock | 模块初始化器执行标志 | 保证 InterruptMock 只跑一次 |
| m_TaskMock | 模块基址（HINSTANCE） | Marshal.GetHINSTANCE(模块) |
| serviceMock/m_MapperMock/m_AdapterMock 等 | 空字节数组占位 | |
| baseMock/policyMock | IntPtr 占位 | |
| proccesorMock/annotationMock/observerMock/m_InstanceMock | 标量占位 | |

## 关键方法与流程

### ModuleInit_内存补丁（InterruptMock）—— 壳核心
1. 读嵌入资源 `e6ab89dd-cbca-49e1-92f5-a278f1600946`（二进制）
2. 构造 32 字节密钥数组（逐字节覆盖赋值）与 16 字节 IV，`Array.Reverse(IV)`，再与程序集 PublicKeyToken 逐字节 XOR 混合
3. 数据段流变换解密（32 位字累加 + `x^=x<<5; x+=3753412436; x^=x<<6; x+=678874538; x^=x>>13; x+=179542347; x=522623986-x` 的密钥流）
4. `OpenProcess(0x38=PROCESS_ALL_ACCESS, ..., 当前进程ID)`；`m_TaskMock = GetHINSTANCE(模块0)`
5. 按补丁清单：`VirtualProtect(目标,4,PAGE_READWRITE)` → `WriteProcessMemory` 写 4 字节 int → 恢复保护（针对模块内 RVA 的 32 位常量补丁）
6. 再读剩余区段：`VirtualProtect(addr, len*4, RW)` → `Marshal.WriteInt32` 逐字写入 → 恢复；`CloseHandle`
- 即：**运行时把加密的补丁数据写回自身 .text/.rdata，实现脱壳/反调试补丁**（恶意样本与合法加壳程序都会这样做）

### DecryptString_字符串解密（QueryMock）
1. 惰性加载资源 `dfa6a901-446d-4781-bab5-dec8b23ded66`
2. 密钥/IV 构造同上（与 PublicKeyToken 混合），int_0==-1 时先走 Rijndael CBC 解密
3. 32 位字流变换解密整个资源 → 缓存为 _StrategyMock
4. `BitConverter.ToInt32(表, 偏移)` 取长度 → `Encoding.Unicode.GetString(表, 偏移+4, 长度)` 返回明文
- 特征：所有 `QueryMock(n)` 调用点即被加密的字符串，脱壳时可用此函数还原全部 IOC

### FIPS 检测与算法选择
- InstantiateMock 反射 `System.Security.Cryptography.CryptoConfig.get_AllowOnlyFipsAlgorithms`，失败再试 `Utils.get_FipsAlgorithmPolicy==1`
- MoveMock：FIPS 下反射 `System.Core AesCryptoServiceProvider`（.NET 3.5 程序集名硬编码），否则 RijndaelManaged

### VerifyMock_固定密钥加密
- Key = {82,102,104,110,32,77,24,34,118,181,51,17,18,51,12,109,10,32,77,24,34,158,161,41,97,28,118,181,5,25,1,88}
- IV = MD5(UTF16(string_1))；对 UTF16(string_0) 加密 → Base64（壳的注册码/激活校验路径）

## 恶意行为 / 敏感操作
- **自修改代码**：WriteProcessMemory/VirtualProtect 改写自身模块（反分析、反篡改、脱壳）
- 进程句柄操作：OpenProcess(ALL_ACCESS) 打开自身进程
- 反射访问 .NET 内部类（FIPS 探测，反沙箱特征）
- 固定密钥加密函数（VerifyMock）——若被外部调用可作数据外带/激活码生成
- 多个 P/Invoke：WriteProcessMemory/ReadProcessMemory/OpenProcess/CloseHandle/VirtualProtect

## IOC / 硬编码
- 嵌入资源：`dfa6a901-446d-4781-bab5-dec8b23ded66`（字符串表）、`e6ab89dd-cbca-49e1-92f5-a278f1600946`（内存补丁数据）
- 密钥流常量：3753412436、678874538、179542347、522623986
- 固定 Key（VerifyMock）：`52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58`
- DeleteMock 固定 Key：`FB 9A 69 4B FE C2 CD C4 EB C8 5A 6A B6 85 98 BB 8C E1 4C 6F D1 17 51 43 36 1D 46 57 76 2B CF E5`，IV：`43 BD 3C 69 16 EA F3 BA DB 42 C0 71 12 E2 22 8F`
- 特征 GUID（IntelliLock）：`{11111-22222-40001-00001}`、`{11111-22222-40001-00002}`、`{11111-22222-50001-00001}`、`{11111-22222-50001-00002}`、`{11111-22222-60001-00001}`、`{11111-22222-60001-00002}`
- 程序集名（FIPS 反射）：`System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089`
