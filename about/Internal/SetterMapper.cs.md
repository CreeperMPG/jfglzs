# Internal/SetterMapper.cs

## 文件作用

注册表操作与密码哈希工具集。本程序集几乎所有的**持久化痕迹**都要经过这个类：密码凭据存在 `HKCU\Software\n`，浏览器/USB/CMD 策略键的写入与回滚也走这里。

它不自己决定策略内容，只是「读/写/删注册表值」和「算密码哈希」的通用底座，真正决定写什么的是 `Form1` / `Form3`。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `SetterMapper`（`[StandardModule]` 静态类） | 注册表读写删 + 密码哈希（SHA256 加固定盐） |
| `计算密码哈希(string)` | `SHA256(UTF8(密码 + "bfdshgs"))` → 小写 hex |
| `读取注册表值(...)` | 读一个值，**吞掉所有异常**返回 null |
| `写入注册表值(...)` | 写一个值（键不存在则先建），失败抛带中文前缀的 Exception |
| `删除注册表值(...)` / `删除注册表键(...)` | 删值 / 递归删子键 |
| `注册表键是否存在(...)` / `确保注册表键存在(...)` | 存在性探测 / 不存在则创建 |
| `小助手进程名` / `小助手可执行文件名` | 静态共享状态：由 Form1 的 Load 生成，Form3 的退出流程消费 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `CalcComposer(string)` | `计算密码哈希(string 密码)` | SHA256 + 固定盐 `bfdshgs` |
| `DisableComposer(RegistryKey, string, string, object)` | `读取注册表值(...)` | 读值，异常返回 null |
| `PrintComposer(RegistryKey, string, string, object, RegistryValueKind)` | `写入注册表值(...)` | 写值，键缺失时创建 |
| `ConcatComposer(RegistryKey, string, string)` | `删除注册表值(...)` | 删值 |
| `AddComposer(RegistryKey, string)` | `注册表键是否存在(...)` | 存在性探测 |
| `IncludeComposer(RegistryKey, string)` | `确保注册表键存在(...)` | 不存在则创建 |
| `FillComposer(RegistryKey, string)` | `删除注册表键(...)` | 递归删除子键 |
| `_ValMapper` | `小助手进程名` | 随机生成的助手进程名 |
| `m_WatcherMapper` | `小助手可执行文件名` | 进程名 + `.exe` |
| `_ClassMapper` / `annotationMapper` / `testMapper` / `exceptionMapper` | `PROV_RSA_FULL` / `CRYPT_VERIFYCONTEXT` / `CALG_SHA1` / `HP_HASHVAL` | CryptoAPI 常量，均未使用 |
| `string_0` / `string_1` / `object_0` | `密码`/`子键路径`/`值名`/`值`/`默认值` | 参数语义化 |
| `ListFacade()` / `InitFacade()` | `返回真占位()` / `返回假占位()` | 恒值桩 |

## 关键方法与流程

1. **`计算密码哈希(密码)`**
   - `SHA256.Create()` → `ComputeHash(UTF8("密码" + "bfdshgs"))`；
   - 每字节 `ToString("x2")` 拼成 64 字符小写 hex；
   - 盐硬编码，等价于「加盐哈希但盐公开」，密码可离线爆破。

2. **`读取注册表值(根, 子键, 值名, 默认值 = null)`**
   - 原版是 `On Error Resume Next`：整段只覆盖一条实质语句，出错就返回 null；
   - 还原为 `try { using(OpenSubKey) { 有键→GetValue，无键→默认值 } } catch { }`；
   - 返回 `object`（VB 弱类型），调用方一般再用 `Conversions.ToString` 转字符串。

3. **`写入注册表值(根, 子键, 值名, 值, 类型 = String)`**
   - `OpenSubKey(path, true)` 拿到可写键；
   - 键为 null → `CreateSubKey` 新建后写；不为 null → 直接写；
   - 原版用 `goto` 跳过「已存在键的重复写入」，还原成 if/else；
   - 任何异常 → `throw new Exception("写入注册表值时发生错误: " + ex.Message)`。

4. **`删除注册表值`**：`OpenSubKey(path, true)`，键存在才 `DeleteValue(值名, false)`（值为空不抛）。

5. **`注册表键是否存在`**：`OpenSubKey(path)` 后判断 `!= null`；异常 → false。

6. **`确保注册表键存在`**：`OpenSubKey` 为 null 时 `using(CreateSubKey(path)) return true;`，否则置结果 true 后返回。

7. **`删除注册表键`**：先用 `注册表键是否存在` 探测，存在才 `DeleteSubKey`。

8. **未使用的 CryptoAPI 残留**：6 个 `advapi32.dll` P/Invoke（`CryptAcquireContext`/`CryptCreateHash`/`CryptHashData`/`CryptGetHashParam`/`CryptDestroyHash`/`CryptReleaseContext`）+ 4 个常量，全文件无调用点。

## 恶意行为 / 敏感操作

- **密码凭据持久化**：`HKCU\Software\n` 存 64 字符 SHA256 hex，是「谁能退出管控」的唯一凭据；删除该值等于恢复出厂（无密码）。
- **注册表写入能力**：本类提供 `HKCU` 与 `HKLM` 双根的写入通道，`Form3` 正是用它改 `SYSTEM\CurrentControlSet\Services\usbstor`（USB 存储）、`Policies\...\Chrome|Edge|Firefox`（下载/开发者工具）、`Policies\Microsoft\Windows\System\DisableCMD`——都是**策略级、系统级**配置。
- 密码哈希可离线爆破：盐固定为 `bfdshgs`，`SHA256` 单轮。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 固定盐 | `bfdshgs` |
| 哈希算法 | `SHA256`，输出小写 hex（64 字符） |
| 密码存储位置 | `HKCU\Software` 下的值名 `n`（见 Form1/Form3 调用点） |
| 错误提示前缀 | `写入注册表值时发生错误: `、`删除注册表值时发生错误: `、`确保注册表键存在操作失败: `、`删除注册表键时发生错误: ` |
| 未使用 API | `advapi32.dll`：`CryptAcquireContext`、`CryptCreateHash`、`CryptHashData`、`CryptGetHashParam`、`CryptDestroyHash`、`CryptReleaseContext` |
| 未使用常量 | `1`(PROV_RSA_FULL)、`-268435456`(CRYPT_VERIFYCONTEXT)、`32771`(CALG_SHA1)、`2`(HP_HASHVAL) |
| IL 行号 | 类型 `0x0200000D`，方法 `0x06000069`–`0x06000077` |
