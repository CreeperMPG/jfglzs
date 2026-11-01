# Internal\AttrAttr.cs —— 文件级分析

## 文件作用
VB.NET `My.Computer` 的自动生成包装类（`[GeneratedCode("MyTemplate","11.0.0.0")]`）。它本身不写任何逻辑，只是通过继承 `Microsoft.VisualBasic.Devices.Computer` 把系统访问入口（键盘、鼠标、剪贴板、文件系统、注册表等）挂到 `My.Computer` 上。另有两个加壳工具插入的恒值死代码桩。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `AttrAttr : Computer` | `My.Computer` 的载体；构造函数为空，全部能力来自基类 |
| `恒真桩()` | 恒返回 `true` 的死代码桩（RVA `0x000021E5`），无调用点 |
| `恒假桩()` | 恒返回 `false` 的死代码桩（RVA `0x000021E8`），无调用点 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `AttrAttr` | 未改名 | `My.Computer` 包装类；类名被 Reactor 重写成属性名词池，但语义仍是 `Computer` |
| `StopRule()` | `恒真桩()` | `return true;`，无调用点 |
| `RegisterRule()` | `恒假桩()` | `return false;`，无调用点 |

## 关键方法与流程（按方法分段，写清步骤）
- `AttrAttr()`：空构造函数；只调用基类 `Computer` 的默认构造。
- `恒真桩()`：`return true;`
- `恒假桩()`：`return false;`

## 恶意行为 / 敏感操作
- 文件本身无恶意代码。
- 风险点在于**能力面**：`My.Computer` 一旦被使用，就等价于获得了 `Microsoft.VisualBasic` 的全套系统交互能力（含 `Computer.Registry`、`Computer.FileSystem`）。本工程里 `CandidateAttr.计算机`（旧名 `SetInfo`）就是它的访问入口，但全工程没有实际调用。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 无字符串常量、无路径、无注册表键。

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无 `IL_xxxx:` / `num2 = N` / `ClearProjectError()`
- 保留 4 条 `// Token:` 注释
