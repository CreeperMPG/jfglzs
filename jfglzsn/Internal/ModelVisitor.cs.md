# Internal/ModelVisitor.cs — 注册表操作封装 + 密码哈希 + 全局配置变量

## 文件作用
静态工具类：封装注册表读写/删除/键存在性检查（对 VB `My.Computer.Registry` 的混淆化包装），提供密码哈希函数 `FillConfig`（SHA256 加盐），并持有多个**全局配置变量**：随机进程名（`_TestsVisitor`/`m_ConsumerVisitor`，由 Form1 每天生成）、UI 水印名称（"教室终结者"/"易语言程序"）。文件头部还声明了一组 `advapi32.dll` 的 CryptoAPI P/Invoke（CryptAcquireContext/CryptCreateHash/CryptHashData/CryptGetHashParam/CryptDestroyHash/CryptReleaseContext），在反编译代码中未见调用（壳/混淆残留或动态调用）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `ModelVisitor`（StandardModule） | 注册表工具 + SHA256 密码哈希 + 全局变量 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `FillConfig` | SHA256 密码哈希 | SHA256(明文 + "bfdshgs") 十六进制小写 |
| `DeleteConfig` | 读注册表值 | 键不存在返回默认值；带 VB 错误状态机（IL 混淆） |
| `AwakeConfig` | 写注册表值 | 键不存在先 CreateSubKey；异常抛"写入注册表值时发生错误" |
| `CancelConfig` | 删除注册表值 | DeleteValue(throwOnMissing=false) |
| `PrintConfig` | 检查注册表键是否存在 | OpenSubKey != null |
| `QueryConfig` | 确保注册表键存在 | 不存在则 CreateSubKey，返回 true |
| `ForgotConfig` | 删除注册表键 | 存在才 DeleteSubKey |
| `_TestsVisitor` | 随机进程名（无扩展名） | 由 Form1.LogoutTemplate 每日生成（5 位，i~r） |
| `m_ConsumerVisitor` | 随机进程名（带 .exe） | = _TestsVisitor + ".exe" |
| `_WatcherVisitor` | 水印字符串"教室终结者" | 用于产品黑名单 |
| `_RepositoryVisitor` | 水印字符串"易语言程序" | 用于产品黑名单 |
| `_RegVisitor` | 锁屏窗体可否关闭标志 | Form4.FormClosing 读取（false 可关，true 禁止） |
| `roleVisitor/_FacadeVisitor/m_RefVisitor/annotationVisitor` | 常量 | 未使用（Crypt API 相关常量残留） |

## 关键方法与流程

### SHA256 密码哈希（原名 `FillConfig(明文)`）
- **流程**：`SHA256.ComputeHash(UTF8(明文 + "bfdshgs"))` → 逐字节 `x2` 十六进制小写拼接。
- **关键调用/API**：`System.Security.Cryptography.SHA256`。
- **用途**：Form2/Form4 的密码校验（与 `HKCU\Software\n`、`HKCU\Software\jfglzs\n2` 比对）。

### 读注册表值（原名 `DeleteConfig(根键, 子键路径, 值名, 默认值)`）
- **流程**：`OpenSubKey(路径)`；非空→`GetValue(值名, 默认)`；为空→返回默认值。函数体含 VB 编译器生成的错误处理状态机（`IL_xx`/`@switch`），语义即上述。
- **关键调用/API**：`RegistryKey.OpenSubKey/GetValue`。

### 写注册表值（原名 `AwakeConfig(...)`）
- **流程**：`OpenSubKey(路径, true)`；为空→`CreateSubKey` 后 `SetValue`；否则直接 `SetValue(值名, 值, 类型)`。异常统一抛"写入注册表值时发生错误: ..."。
- **关键调用/API**：`RegistryKey.SetValue`、`RegistryValueKind`。

### 删除值 / 键（`CancelConfig`/`ForgotConfig`）
- **流程**：`DeleteValue(值名, false)`；`PrintConfig` 确认存在后 `DeleteSubKey(路径)`。异常抛中文错误。

## 恶意行为 / 敏感操作
- 密码哈希（带固定盐 "bfdshgs"，可离线破解字典）；全局随机进程名变量供主控释放/击杀同伴进程。

## IOC / 硬编码
- **哈希盐**：`"bfdshgs"`（SHA256 加盐后缀）。
- **注册表路径**：由调用方（Form1/Form2/Form4）传入：`Software\jfglzs`、`Software\n`、`Software\Policies\Microsoft\Windows\System\DisableCMD`、`SYSTEM\CurrentControlSet\Services\usbstor\Start`、`SOFTWARE\...\RUN` 等。
- **水印/黑名单串**：`教室终结者`、`易语言程序`。
- **P/Invoke**：`advapi32.dll` CryptAcquireContext/CryptCreateHash/CryptHashData/CryptGetHashParam/CryptDestroyHash/CryptReleaseContext（声明未调用）。
