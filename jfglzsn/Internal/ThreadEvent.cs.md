## 文件作用

全局工具模块（VB `[StandardModule]`）：**注册表读写总封装** + 一个 SHA256 摘要函数（带硬编码盐）+ 一堆未使用的 CryptoAPI P/Invoke。

它是全样本的「系统操作底座」——几乎所有持久化和机房管控动作最终都落到这里的六个注册表方法上。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ThreadEvent`（`[StandardModule]`） | 注册表工具 + 哈希工具 + 策略常量 |
| `计算SHA256摘要`（原 `SelectCallback`） | `SHA256(UTF8(入参 + "bfdshgs"))` → 64 位小写十六进制 |
| `读注册表值`（原 `SearchCallback`） | 打开子键取值的容错封装（出错返回 `null`） |
| `写注册表值`（原 `EnableCallback`） | 写值，键缺失先建；失败包成一般异常抛出 |
| `删除注册表值`（原 `SetCallback`） | `DeleteValue(name, false)`，值不存在不报错 |
| `注册表键存在`（原 `NewCallback`） | 判断子键存在，异常当不存在 |
| `确保注册表键存在`（原 `ViewCallback`） | 缺失就创建（幂等），返回 `true` |
| `删除注册表键`（原 `RateCallback`） | `DeleteSubKey`（**非递归**） |
| 6 个 `Crypt*` P/Invoke | CryptoAPI 声明，全工程**无调用**（壳做 MD5 时的遗留） |
| `m_AdapterEvent` / `_AdvisorEvent` | 当前进程名 / 可执行文件名（Form1 随机生成后写入） |
| `mapperEvent` | 锁屏窗体「禁止关闭」标志（本工程内从未置真） |
| `callbackSetter` / `_EventSetter` | 两个中文常量，写进 Form1 的关键词黑名单表 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `SelectCallback(string)` | `计算SHA256摘要` | 壳随机名；行为是 SHA256 + 固定盐 |
| `SearchCallback(...)` | `读注册表值` | 壳随机名；`Registry.OpenSubKey` + `GetValue` |
| `EnableCallback(...)` | `写注册表值` | 壳随机名；`OpenSubKey(..., true)` + `SetValue`，缺失 `CreateSubKey` |
| `SetCallback(...)` | `删除注册表值` | 壳随机名；`DeleteValue` |
| `NewCallback(...)` | `注册表键存在` | 壳随机名；`key != null` |
| `ViewCallback(...)` | `确保注册表键存在` | 壳随机名；缺失则 `CreateSubKey` |
| `RateCallback(...)` | `删除注册表键` | 壳随机名；`DeleteSubKey` |
| `m_AdapterEvent` | `当前进程名` | 壳随机名；被 Form1 赋值为随机字符串，用于进程名/服务名 |
| `_AdvisorEvent` | `当前可执行文件名` | 值 = 当前进程名 + `.exe` |
| `mapperEvent` | `保留原名`（语义：锁屏窗禁止关闭标志） | 只有 Form4 的 `FormClosing` 读它，从未被置真 |
| `callbackSetter` / `_EventSetter` | 保留原名 | 值分别是「教室终结者」「易语言程序」，被写进 Form1 的关键词黑名单第 76 / 72 项 |
| `string_0` / `string_1` / `byte_0` / `intptr_0` / `registryKey_0` / `object_0` | `原文` / `子键路径` / … / `根键` / `默认值` 等 | 全为占位名，按实际用途重命名 |
| `num` / `num2` / `num4` / `obj` / `obj2` / `flag` / `array` / `text` / `text2` | `结果` / `存在` / `摘要` / … | 局部占位名按用途重命名 |
| `m_FacadeSetter`（在 ListSetter 中） | 见 `ListSetter.cs.md` | 本类不涉及 |

## 关键方法与流程

### `计算SHA256摘要(原文)`

1. `SHA256.Create()`（`using` 包裹）。
2. `ComputeHash(UTF8(原文 + "bfdshgs"))`。
3. 逐字节 `ToString("x2")` 拼成 64 字符小写十六进制。

作用：退出小助手的口令校验（Form2 / Form4 用同一算法比对注册表里的摘要）。

### `读注册表值(根键, 子键路径, 值名, 默认值 = null)`

原版是 VB 的 `On Error Resume Next`，只有一条读语句：

1. `try { using (子键 = 根键.OpenSubKey(子键路径)) { 子键 != null ? GetValue(值名, 默认值) : 默认值 } }`
2. `catch { }` 吞掉所有异常。
3. 返回 `结果`。

⚠️ **关键语义**：出错时返回的是 `null`，**不是** `默认值`（赋值语句还没执行到）。上层大量用 `Conversions.ToString(...)` / `Conversions.ToInteger(...)` 接这个 `null`，分别得到 `""` 和 `0`——这条差异直接影响所有「未被篡改 / 已篡改」的判断，不能"修正"。

### `写注册表值(根键, 子键路径, 值名, 值, 值类型 = String)`

1. `OpenSubKey(子键路径, true)`：
   - `null` → `CreateSubKey(子键路径)` 后在新键上 `SetValue`；
   - 非 `null` → 直接在原键上 `SetValue`。
2. 任何异常 → `throw new Exception("写入注册表值时发生错误: " + ex.Message)`。

### `删除注册表值` / `注册表键存在` / `确保注册表键存在` / `删除注册表键`

| 方法 | 要点 |
| --- | --- |
| `删除注册表值` | `DeleteValue(值名, false)`：值不存在静默；子键不存在什么都不做；异常包成 `"删除注册表值时发生错误: ..."` |
| `注册表键存在` | `key != null`；**任何异常都当作不存在**（返回 `false`） |
| `确保注册表键存在` | 键缺失 → `using (根键.CreateSubKey(...)) { return true; }`（不接变量，创建即释放，与原版一致）；键存在 → `结果 = true` 后返回；异常包成 `"确保注册表键存在操作失败: ..."` |
| `删除注册表键` | 先 `注册表键存在`，存在才 `DeleteSubKey`（**不递归**，有子键会抛）；异常包成 `"删除注册表键时发生错误: ..."` |

### 未使用的 CryptoAPI 声明

`CryptAcquireContext` / `CryptCreateHash` / `CryptHashData` / `CryptGetHashParam` / `CryptDestroyHash` / `CryptReleaseContext` + 常量 `PROV_RSA_FULL=1`、`CRYPT_VERIFYCONTEXT=0xF0000000`、`CALG_MD5=0x8003`、`HP_HASHVAL=2`。**全工程无调用点**——壳原本用自己的 MD5 校验，解壳后调用点消失，只剩声明。

## 恶意行为 / 敏感操作

| 行为 | 具体落点 | 风险 |
| --- | --- | --- |
| **注册表持久化** | `写注册表值` 被用来写 `HKLM\...\CurrentVersion\Run\prozs` 与 `Run\jfglzsn` | 高（开机自启） |
| **系统策略篡改** | 写 `HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD`（禁 CMD） | 高 |
| **防火墙关闭** | 写 `SharedAccess\Parameters\FirewallPolicy\{Standard,Public}Profile\EnableFirewall = 0` | 高 |
| **USB 存储禁用** | 写 `SYSTEM\CurrentControlSet\Services\usbstor\Start = 4`（含 ControlSet001~003） | 高（阻止 U 盘） |
| **口令摘要比对** | `计算SHA256摘要` + 读 `HKCU\Software\n` | 中（退出密码机制） |
| **删 IFEO 劫持** | `删除注册表键` 删 `IFEO\route.exe` | 中（清掉学生的路由劫持） |
| 掩码保护 | `mapperEvent` 控制锁屏窗能否关闭 | 中 |

> 注：`读注册表值`/`写注册表值` 是"中性工具"，恶意性来自调用方（Form1），本文件提供的是能力。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 哈希算法 | `SHA256(UTF8(输入 + "bfdshgs"))`，小写 hex |
| **固定盐** | `bfdshgs` |
| 常量字符串 | `教室终结者`（`callbackSetter`） |
| 常量字符串 | `易语言程序`（`_EventSetter`） |
| 异常文案 | `写入注册表值时发生错误: `、`删除注册表值时发生错误: `、`确保注册表键存在操作失败: `、`删除注册表键时发生错误: ` |
| P/Invoke | `advapi32.dll`：`CryptAcquireContext`/`CryptCreateHash`/`CryptHashData`/`CryptGetHashParam`/`CryptDestroyHash`/`CryptReleaseContext` |
| 常量 | `PROV_RSA_FULL = 1`、`CRYPT_VERIFYCONTEXT = -268435456`(0xF0000000)、`CALG_MD5 = 32771`(0x8003)、`HP_HASHVAL = 2` |
