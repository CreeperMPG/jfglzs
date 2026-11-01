# Internal\ObserverAttr.cs —— 文件级分析

## 文件作用
加壳工具留下的「万能工具类」：注册表读写封装、SHA256 密码摘要、WinForms 消息循环、几个没用到的 CryptoAPI 声明。全样本对注册表的所有写操作都经过这里，密码摘要算法也在这里。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `ObserverAttr`（`[StandardModule]`，等价 C# 静态类） | 注册表/摘要工具 + 静态字段容器 |
| `InstantiateInfo` | **密码摘要**：`hex(SHA256(明文 + "bfdshgs"))` |
| `RateInfo` | **写注册表值的唯一入口**（子键不存在就建） |
| `SortInfo` | 读注册表值（带默认值，出错返回 Nothing） |
| `CountInfo` | 删注册表值 |
| `DisableInfo` | 判断子键是否存在 |
| `ListInfo` | 确保子键存在 |
| `CalculateInfo` | 删整个子键 |
| `CloneInfo` | 启用视觉样式 + 起 `load` 窗体的消息循环（无调用点） |
| `账号进程名` / `访客名` | 静态字符串字段，由 `load.LogoutInfo` 写入，被 `Form1.CallInfo` 当要杀的进程名 |
| 6 个 `Crypt*` P/Invoke | advapi32 CryptoAPI 声明，全工程无调用点 |
| 4 个 `int` 常量 | `m_TagAttr`/`_DispatcherAttr`/`stubAttr`/`_ItemAttr`，无引用 |
| `恒假桩` | 恒返回 false 的死代码桩 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `InstantiateInfo(string)` | 同名，参数 `string_0`→`原文` | `SHA256(原文+"bfdshgs")` 的十六进制串 |
| `RateInfo(...)` | 同名，参数 → `根键/子键/值名/值/类型` | 写注册表封装 |
| `SortInfo(...)` | 同名，参数 → `根键/子键/值名/默认值` | 读注册表封装 |
| `CountInfo` / `DisableInfo` / `ListInfo` / `CalculateInfo` | 同名，参数 → `根键/子键/值名` | 删值 / 判存在 / 确保存在 / 删键 |
| `SetFactory()` | `恒假桩()` | `return false;`，无调用点 |
| 字段 `m_AccountAttr` | `账号进程名` | 由 `load.LogoutInfo` 写入 |
| 字段 `m_VisitorAttr` | `访客名` | 同上（多一个 ".exe"） |
| 局部 `text` / `array` / `stringBuilder` / `b` | `摘要` / `哈希字节` / `构造` / `单字节` | |
| 局部 `num`/`num2`/`num4`/`obj`/`obj2` | 删除 | VB 错误状态号与分发计数器 |

## 关键方法与流程（按方法分段，写清步骤）

### `InstantiateInfo(原文)` —— 密码摘要
1. `using (SHA256.Create())`
2. `ComputeHash(UTF8(原文 + "bfdshgs"))` —— **硬编码盐 `bfdshgs`**
3. 每个字节 `ToString("x2")` 拼成小写十六进制字符串返回

调用点：`Form1.DefineInfo`（写 `HKCU\Software\n`）、`load.PopInfo`（登录校验）。

### `RateInfo(根键, 子键, 值名, 值, 类型=String)`
1. `using (键 = 根键.OpenSubKey(子键, true))`
2. 键为 null → `using (新建键 = 根键.CreateSubKey(子键)) { 新建键.SetValue(...) }`，**然后跳过下面的写入**（原版是 goto 到 using 末尾）
3. 否则 `键.SetValue(值名, RuntimeHelpers.GetObjectValue(值), 类型)`
4. 任何异常 → `throw new Exception("写入注册表值时发生错误: " + ex.Message)`（**是抛出不是吞掉**）

### `SortInfo(根键, 子键, 值名, 默认值=null)`
1. `using (键 = 根键.OpenSubKey(子键))`
2. 键非 null → `键.GetValue(值名, 默认值)`；否则用默认值
3. `On Error Resume Next` 语义：出错就吞掉，返回值保持 `Nothing`（**不是**传入的默认值）——`Form1.AwakeInfo` 依赖这个行为

### 其它
- `CountInfo`：`DeleteValue(值名, false)`，删不存在不报错；异常包成「删除注册表值时发生错误」抛出
- `DisableInfo`：`OpenSubKey` 非 null 即存在，异常当不存在
- `ListInfo`：不存在就 `CreateSubKey`（用无变量的 `using`）后返回 true；异常包成「确保注册表键存在操作失败」抛出
- `CalculateInfo`：`DisableInfo` 为真才 `DeleteSubKey`；异常包成「删除注册表键时发生错误」抛出
- `CloneInfo`：`EnableVisualStyles` + `SetCompatibleTextRenderingDefault(false)` + `窗体集合.load.Show()` + `Application.Run()`

## 恶意行为 / 敏感操作
- **注册表写入中枢**：`Form1.DefineInfo` 里几十条策略注册表都是经 `RateInfo` 写下去的（HKCU 策略、HKLM Run 自启、防火墙、usbStor、浏览器策略等）。
- **密码摘要**：`SHA256(明文+"bfdshgs")` → `HKCU\Software\n`，是样本的「管理员密码」存储方式；盐 `bfdshgs` 可直接用于离线爆破。
- **账号进程名字段**：`m_AccountAttr` 是 `Form1.CallInfo` 杀进程的目标名来源，可视为组件间约定的 IOC。
- 无文件操作、无网络操作。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 盐：`bfdshgs`
- 注册表（本文件不含具体键路径，键都由调用方传入）：`HKCU\Software\n`（密码摘要）、`HKCU\Software\jfglzs\*`、`HKLM\...\RUN\prozs`
- 异常文案：`写入注册表值时发生错误: `、`删除注册表值时发生错误: `、`确保注册表键存在操作失败: `、`删除注册表键时发生错误: `
- 反射目标：`System.Security.Cryptography.CryptoConfig` / `get_AllowOnlyFipsAlgorithms`（无调用点）
- P/Invoke：`advapi32.dll` 的 `CryptAcquireContext`/`CryptCreateHash`/`CryptHashData`/`CryptGetHashParam`/`CryptDestroyHash`/`CryptReleaseContext`

## 反混淆处理记录
- goto：**4 → 0**；`IL_xxxx:` 10 → 0；`@switch` 2 → 0；`endfilter` 1 → 0；`ClearProjectError()` 全清
- `SortInfo` 的 VB 错误分发按「读失败即返回 Nothing」还原成一层 `try/catch`（依据 ILSpy 的 resume 表：错误状态全部 resume 到 using 之后的返回点）
- `RateInfo` 里 `goto IL_003F`（跳到 using 末尾）还原成 `if/else`
- 语义保留点：`RuntimeHelpers.GetObjectValue`、`throw new Exception("...")`、`using (没有变量的 using)`、`ListInfo` 里的提前 `return true`、字段/常量取值，全部原样
- 跨文件改名联动：`Form1.CallInfo` 里的 `ObserverAttr.m_AccountAttr` → `ObserverAttr.账号进程名`
