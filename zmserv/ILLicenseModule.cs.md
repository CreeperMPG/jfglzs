## 文件作用

IntelliLock 加壳器的**授权模块残留**。原版 IntelliLock 会在这里实现「许可证校验」，
脱壳/解混淆后只剩下两个固定返回常量的空方法——壳的校验逻辑已经被剥离。

对分析的意义：**证明样本用 IntelliLock 加过壳**（与 `Internal/AdvisorStrategy.cs`、`Internal/CustomerStrategy.cs` 互相印证），
除此之外不含任何业务或恶意逻辑。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ILLicenseModule` | 授权模块外壳。两个静态方法都没有调用点，只返回 `true` / `false` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `CollectDefinition()` | 保持原名 | 壳注入空桩，固定 `return true`。`Definition` 是加壳器随机词表的后缀，无语义 |
| `PrintDefinition()` | 保持原名 | 壳注入空桩，固定 `return false` |

> 本文件的两个方法属于「死代码」类噪声：**保留原名 + 在 XML 注释里注明是空桩**，
> 比另起名字更容易回溯 IL（方法名带 `Definition` 就是加壳器注入的标志之一）。

## 关键方法与流程

**`CollectDefinition()`**：`return true;`，不读参数、不写状态、无副作用。
**`PrintDefinition()`**：`return false;`，同上。

两者在全工程内都没有引用点（`grep -F "ILLicenseModule"` 只命中定义处）。

## 恶意行为 / 敏感操作

无。仅作为「样本被 IntelliLock 加壳」的物证。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 类型名 | `ILLicenseModule`（IntelliLock 生成，可用于识别同类壳） |
| Token | 类 `0x0200000C RID: 12`；方法 `0x06000032/0x06000033` |
| 其它 | 无路径、无注册表、无 URL、无进程名、无字符串常量 |
