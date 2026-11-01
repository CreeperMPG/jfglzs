# Proccesor.cs.md — 注册表读写 + 授权摘要工具

## 文件作用

`OZxTKXuvsl9D34LWhP\Internal\Proccesor.cs` 是被混淆器改过名的工具类（注意类名本身就把 Processor 拼错了，原样保留）。
它只干两类事：

1. **注册表操作**：读值 / 写值 / 删值 / 判存在 / 确保存在 / 删键，六个方法都带同一套"中文异常包装"；
2. **授权摘要**：`SHA256(明文 + "bfdshgs")` 转小写十六进制。

外加几个**跨窗体共享的全局状态**：本程序目录、两个"360 浏览器下载"目录名、锁屏禁止关闭开关。

文件顶部 6 个 `advapi32.dll` 的 CryptoAPI 声明（`CryptAcquireContext`/`CryptCreateHash`/`CryptHashData`/
`CryptGetHashParam`/`CryptDestroyHash`/`CryptReleaseContext`）**没有被任何代码调用**，配套的四个私有常量
（`PROV_RSA_FULL`、`CRYPT_VERIFYCONTEXT`、`CALG_MD5`、`HP_HASHVAL`）同样没用到——典型的"早期版本遗留 + 加壳器改名"痕迹。

> 本文件不是"恶意行为"的发生地，但它是**机房管理助手的注册表写入/锁屏链路**的公共工具层：
> `写入注册表值`/`删除注册表值` 被 Form1 用来装开机自启、写策略键、清理痕迹；
> `计算SHA256加盐` 用来生成机器码。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Proccesor`（`[StandardModule]`，等同 VB 模块） | 注册表工具 + 摘要工具 + 全局状态容器 |
| `CryptAcquireContext` 等 6 个 P/Invoke | **死代码**：只声明不调用，配套常量也没用到 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `UpdateVal` | `计算SHA256加盐` | 盐固定 `bfdshgs`，输出小写 hex（64 字符） |
| `InsertVal` | `读取注册表值` | 读不到返回默认值 |
| `WriteVal` | `写入注册表值` | 子键不在就建 |
| `ListVal` | `删除注册表值` | `DeleteValue(name, false)`，缺值不报错 |
| `PatchVal` | `注册表键是否存在` | 异常一律当不存在 |
| `StopVal` | `确保注册表键存在` | 建完即释放 |
| `ChangeVal` | `删除注册表键` | 非递归 `DeleteSubKey` |
| `registryKey_0` | `根键` | 参数 |
| `string_0` | `子键路径` | 参数 |
| `string_1` | `值名` | 参数 |
| `object_0` | `默认值` | 参数（`InsertVal`） |
| `object_0` | `值` | 参数（`WriteVal`） |
| `registryValueKind_0` | `值类型` | 参数，默认 `RegistryValueKind.String` |
| `intptr_0…` | `加密服务提供者句柄` / `哈希句柄` … | P/Invoke 形参，按 Win32 语义命名 |
| `m_Filter` | `程序所在目录` | Form1 用它拼 `jfglzsn.exe` / `about.exe` / `yz.exe` |
| `_Object` | `安全浏览器目录名` | `"360安全浏览器下载"` |
| `m_Iterator` | `极速浏览器目录名` | `"360极速浏览器下载"` |
| `_Class` | `锁屏中禁止关闭` | Form4 的 `FormClosing` 用它决定能不能关 |
| `_Map` / `m_Setter` | 保留原名 | 全工程零读写的遗留字段 |
| `_Visitor` / `_Parser` / `property` / `configuration` | 保留原名 | 未使用的 CryptoAPI 常量 |

## 关键方法与流程

### 计算SHA256加盐(string 原文)

1. `SHA256.Create()`；
2. `ComputeHash(UTF8(原文 + "bfdshgs"))`；
3. 逐字节 `ToString("x2")` 拼成 64 字符小写 hex；
4. 返回。

盐是硬编码的 `bfdshgs`，所以要复现授权码只要知道原文即可。

### 读取注册表值(根键, 子键路径, 值名, 默认值=null)

1. `根键.OpenSubKey(子键路径)`（只读）；
2. 子键非空 → `GetValue(值名, RuntimeHelpers.GetObjectValue(默认值))`；子键为空 → 返回默认值；
3. 全过程包在 `try / catch(Exception){ throw; }` 里。

反混淆要点：反编译结果里的 `ProjectData.ClearProjectError()`、`num = 1`、`num2 = 2`、
`catch when (endfilter(...))`、`goto IL_0056`、`num4 = num2; switch(...)`、
`throw ProjectData.CreateProjectError(-2146828237)` 全都是 VB 编译器为
**`On Error Resume Next`** 生成的错误恢复机制（状态号 + 恢复分派表 + "无法恢复"错误）。
它的语义是"出错就丢掉、继续下一条语句"，所以还原成 <c>catch { }</c>（吞掉异常）：
`值` 先置 `null`（VB 里 `Object` 局部变量初值就是 `Nothing`），出错时直接返回它。
注意**不能**改成 `catch (Exception) { throw; }`——那会让原版能容忍的注册表访问失败变成崩溃。

### 写入注册表值(根键, 子键路径, 值名, 值, 值类型)

1. `OpenSubKey(子键路径, true)`（可写）；
2. 子键为空 → `CreateSubKey(子键路径)` 后在 `using` 内 `SetValue`，然后结束；
3. 子键存在 → 直接 `SetValue`；
4. 任何异常 → `new Exception("写入注册表值时发生错误: " + ex.Message)`。

反混淆要点：原版有一处 `goto IL_003F`（VB 为"using 块内提前跳出"生成），跳到的位置正好是
外层 using 的末尾，等价于"新建子键分支不再执行后面的 SetValue" → 还原成 `if/else`，语义一致。

### 删除注册表值(根键, 子键路径, 值名)

`OpenSubKey(路径, true)` → 非空则 `DeleteValue(值名, false)`；异常 → `删除注册表值时发生错误: …`。
`false` 表示值不存在时不抛异常。

### 注册表键是否存在 / 确保注册表键存在 / 删除注册表键

- 判存在：`OpenSubKey` 结果是否非 null；异常 → false。
- 确保存在：非空直接 `成功 = true`；为空则 `using (根键.CreateSubKey(路径)) { return true; }`——
  建完立刻释放，**不**给调用方留下句柄（保留原结构，别"顺手优化"）。
- 删键：先判存在，再 `DeleteSubKey(路径)`（非递归，子键非空会抛，被包装成
  `删除注册表键时发生错误: …`）。

## 恶意行为 / 敏感操作

| 行为 | 位置 | 说明 |
| --- | --- | --- |
| 注册表写入（可写打开 + 建子键） | `写入注册表值` | 被 Form1 用于开机自启、写策略/映像劫持类键值 |
| 注册表删除（值 / 子键） | `删除注册表值`、`删除注册表键` | 清理痕迹、卸载、改策略 |
| 机器码生成 | `计算SHA256加盐` | 盐 `bfdshgs` 硬编码，用于授权/绑定 |
| 锁屏开关 | 字段 `锁屏中禁止关闭` | 与 Form4 的 `ClipCursor` + `TopMost` 全屏窗口配合，构成"锁屏" |
| 伪装目录名 | 字段 `安全浏览器目录名` / `极速浏览器目录名` | 名字伪装成"360浏览器下载"，实际是程序建的目录 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 摘要盐 | `bfdshgs` |
| 目录名常量 | `360安全浏览器下载`、`360极速浏览器下载`（会建在 `C:\`、`D:\`、`E:\`、`F:\` 根下） |
| 异常文案 | `写入注册表值时发生错误: `、`删除注册表值时发生错误: `、`确保注册表键存在操作失败: `、`删除注册表键时发生错误: ` |
| 未使用的 DLL | `advapi32.dll`（6 个 CryptoAPI 函数） |
| 未使用的常量 | `PROV_RSA_FULL=1`、`CRYPT_VERIFYCONTEXT=0xF0000000`、`CALG_MD5=0x8003`、`HP_HASHVAL=2` |
