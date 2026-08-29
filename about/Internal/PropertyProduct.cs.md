# PropertyProduct.cs — 注册表读写/密码哈希工具模块（静态全局状态）

## 文件作用
全局工具模块（VB.NET StandardModule）：封装注册表值的读（`PostReponse`）、写（`ChangeReponse`）、删（`AssetReponse`/`PrintReponse`）、键存在性判断（`UpdateReponse`/`NewReponse`），并提供密码哈希函数 `ExcludeReponse`（SHA256 + 盐 `"bfdshgs"`）。同时持有两个**全局静态字段** `_SystemProduct` / `indexerProduct`，由 Form1 在 Load 时写入"随机监控进程名"，供 Form3 退流程按名杀进程使用——是跨窗体共享"监控进程身份"的枢纽。文件头部还残留一组 `advapi32.dll` 加密 API 的 DllImport（`CryptAcquireContext` 等，未在本文档中被调用，疑为加壳/混淆器残留）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| PropertyProduct | 注册表操作 + SHA256 密码哈希 + 全局随机进程名静态字段 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `ExcludeReponse(string)` | SHA256 密码哈希 | SHA256(输入 + "bfdshgs") 十六进制小写 |
| `PostReponse(key,sub,name,default)` | 读注册表值 | 打开子键读值，子键不存在返回默认值（带 try/catch 状态机混淆） |
| `ChangeReponse(key,sub,name,val,kind)` | 写/创建注册表值 | 子键不存在则创建；失败抛「写入注册表值时发生错误」 |
| `AssetReponse(key,sub,name)` | 删除注册表值 | 失败抛「删除注册表值时发生错误」 |
| `UpdateReponse(key,sub)` | 判断子键是否存在 | 异常视为 false |
| `NewReponse(key,sub)` | 确保子键存在 | 不存在则创建并返回 true |
| `PrintReponse(key,sub)` | 删除子键 | 存在才删；失败抛「删除注册表键时发生错误」 |
| `_SystemProduct` | 随机监控进程名（无 .exe） | Form1.RateReponse 写入，Form3.CalcReponse 读取 |
| `indexerProduct` | 随机监控进程名 + ".exe" | 同上 |
| `m_StateProduct/methodProduct/workerProduct/prototypeProduct` | 常量 | 混淆器添加的无用常量 |

## 关键方法与流程

### 密码哈希（ExcludeReponse）
- **流程**：`SHA256.Create()` → `ComputeHash(UTF8(输入 + "bfdshgs"))` → 每字节 `ToString("x2")` 拼接小写十六进制。
- **说明**：盐 `bfdshgs` 硬编码，与 Form3.InterruptReponse 配合校验密码。

### 读注册表值（PostReponse）
- **流程**：`registryKey.OpenSubKey(sub)`；非空则 `GetValue(name, defaultValue)`，为空返回默认值。原代码带 `IL_xx/num2` 异常处理状态机，语义等价于 try-catch-return。

### 写注册表值（ChangeReponse）
- **流程**：`OpenSubKey(sub, writable:true)` → 存在则 `SetValue`；不存在则 `CreateSubKey` 后 `SetValue`。异常统一包装抛「写入注册表值时发生错误: …」。

## 恶意行为 / 敏感操作
- 提供对注册表任意键值的读写删能力，是 Form1/Form3 全部策略修改（usbstor、DisableCMD、浏览器策略、密码哈希存储）的底层执行者。
- `_SystemProduct` 全局动态进程名机制帮助监控进程规避静态查杀。

## IOC / 硬编码
- 密码盐：`"bfdshgs"`
- 注册表：所有调用方传入的键路径（见 Form1.cs.md / Form3.cs.md）
- API：`advapi32.dll`：`CryptAcquireContext`、`CryptCreateHash`、`CryptHashData`、`CryptGetHashParam`、`CryptDestroyHash`、`CryptReleaseContext`（残留，未在本文件调用）
