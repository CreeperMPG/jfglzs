# Initializer.cs — 注册表工具库 + SHA256 口令哈希 + 全局配置字段

## 文件作用
反混淆后真实身份：**注册表读写/工具静态类**（ConfuserEx 改名为 `Initializer`），外加一组**全局配置
字段**（程序安装目录、监视目录名、锁屏放行标志）。Form1/Form2 大量调用：
`NewBroadcaster`（读注册表）、`LogoutBroadcaster`（写注册表）、`MapBroadcaster`（SHA256 加盐口令哈希）。
`utils="360安全浏览器下载"`、`m_Creator="360极速浏览器下载"` 是被监视的 360 下载目录名；
`_Collection` 是 Form4 锁屏是否可关闭的全局开关；`config` 是本程序所在目录（由 Form1 从 Run 键解析）。
类内还残留一组**未使用**的 CryptoAPI P/Invoke（`CryptAcquireContext` 等）与常量，为壳/混淆器残留。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Initializer | 注册表读写工具 + SHA256 口令哈希 + 全局配置（目录名/标志/安装目录） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `MapBroadcaster` | `口令加盐哈希` | `SHA256(输入 + "bfdshgs")` 十六进制小写；Form2 退出密码比对用 |
| `NewBroadcaster` | `读注册表值` | `OpenSubKey` 读值，键不存在返回默认值 |
| `LogoutBroadcaster` | `写注册表值` | 可写打开/自动创建子键后 `SetValue`（指定 ValueKind） |
| `ResetBroadcaster` | `删除注册表值` | `DeleteValue` |
| `SearchBroadcaster` | `检查注册表键存在` | `OpenSubKey != null` |
| `DisableBroadcaster` | `确保键存在` | 不存在则 `CreateSubKey`，返回 true |
| `SortBroadcaster` | `删除注册表键` | `DeleteSubKey` |
| `config` | `程序安装目录` | Form1 从 `HKLM\...\Run\prozs` 解析得到 |
| `_Collection` | `锁屏放行标志` | `false` 时 Form4 可关闭并解除鼠标锁定（默认 false） |
| `utils` | `360 浏览器下载目录名` | `"360安全浏览器下载"`（Form1 全盘查找并监视） |
| `m_Creator` | `360 极速浏览器下载目录名` | `"360极速浏览器下载"` |
| `_Filter/system` | 未使用字段 | 混淆器/壳残留 |
| `proxy/_Algo/m_Helper/m_Bridge` 常量 | 未使用 CryptoAPI 常量 | 残留（PROV/ALG/KEY 常量值） |

## 关键方法与流程
### 口令加盐哈希（MapBroadcaster）
- **流程**：`SHA256.ComputeHash(UTF8(输入 + "bfdshgs"))` → 每字节 `ToString("x2")` 拼接成 64 位十六进制串。
- **关键调用/API**：`SHA256.Create`、`Encoding.UTF8`。

### 读注册表值（NewBroadcaster）
- **流程**：`OpenSubKey(子键)`；键存在 → `GetValue(值名, 默认值)`；不存在 → 返回默认值（VB 错误状态机包裹）。

### 写注册表值（LogoutBroadcaster）
- **流程**：`OpenSubKey(子键, writable:true)`；为 null 则 `CreateSubKey` 后 `SetValue`；否则直接
  `SetValue(值名, 值, ValueKind)`。失败抛"写入注册表值时发生错误: …"。
- **关键调用/API**：`RegistryKey.SetValue`——被 Form1 用于写禁止下载策略、usbstor 禁用等。

## 恶意行为 / 敏感操作
- 本身是工具类；被恶意逻辑大量使用：写浏览器组策略、禁用 USB、读写自启动/口令。
- 全局目录名常量用于锁定 360 浏览器下载目录。
- `_Collection` 全局开关控制锁屏是否可关闭（锁死机制的一部分）。

## IOC / 硬编码
- 目录名：`"360安全浏览器下载"`（utils）、`"360极速浏览器下载"`（m_Creator）
- 哈希盐：`"bfdshgs"`
- 残留 P/Invoke：`advapi32!CryptAcquireContext/CryptCreateHash/CryptHashData/CryptGetHashParam/CryptDestroyHash/CryptReleaseContext`（未调用）
- 常量：`_Algo = -268435456`（0xF0000000，PROV 标志）、`m_Helper = 32771`（0x8001，CALG_MD5）、`m_Bridge = 2`
