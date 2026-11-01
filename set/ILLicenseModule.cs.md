# ILLicenseModule.cs —— 文件级分析

## 文件作用
IntelliLock 加壳工具残留在程序集里的「许可模块」桩类。两个方法只返回常量，没有任何调用点，是壳工具写入的占位/水印代码。它和 `Internal\TokenAttr.cs` 里的「This assembly is protected by an unregistered version of IntelliLock!」提示配套，说明样本曾用**未注册版 IntelliLock** 处理过。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `ILLicenseModule` | 壳工具的许可模块占位类；两个静态方法恒返回常量，无业务逻辑 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `CalcFactory()` | `恒真桩()` | 方法体只有 `return true;`，无调用点，是死代码 |
| `ListFactory()` | `恒假桩()` | 方法体只有 `return false;`，无调用点，是死代码 |
| `ILLicenseModule` | 未改名 | 壳工具自带的类名，保留原样 |

## 关键方法与流程（按方法分段，写清步骤）
- `恒真桩()`：直接 `return true;`。没有参数、没有副作用。
- `恒假桩()`：直接 `return false;`。没有参数、没有副作用。

两个方法的 RVA 分别是 `0x000021E5` / `0x000021E8`，与本工程中另外十余个「恒真/恒假」桩完全相同 —— 这是 .NET Reactor 批量插入死代码的典型特征（同一份 IL 被复用）。

## 恶意行为 / 敏感操作
- 无恶意行为。它的意义在**取证**层面：证明样本被 IntelliLock/.NET Reactor 的非注册版本加壳。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 字符串常量：无。
- 关联字符串（在同工程的 `Internal\TokenAttr.cs`）：
  - `This assembly is protected by an unregistered version of "IntelliLock"!`
  - 窗口标题 `www.eziriz.com`（Eziriz 是 .NET Reactor / IntelliLock 的厂商站点）

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无 `IL_xxxx:` 标签，无 `num2 = N` 状态号，无 `ClearProjectError()`
- 保留 3 条 `// Token:` 注释
