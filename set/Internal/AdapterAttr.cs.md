# Internal\AdapterAttr.cs —— 文件级分析

## 文件作用
「WindowsApplication1.Resources」的强类型资源访问类，由 VB 资源设计器模板生成（`[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder","16.0.0.0")]`）。工程里没有定义任何资源字符串，所以它只提供两个从不被取用的成员。另有两个加壳工具插入的恒值死代码桩。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `AdapterAttr` | 资源访问入口（`[StandardModule]`，等价于 C# 静态类） |
| `ResourceManager` 属性 | 惰性创建资源管理器单例，资源基名 `WindowsApplication1.Resources` |
| `Culture` 属性 | 资源查找的区域性（默认 null = 使用当前区域性） |
| `恒真桩()` / `恒假桩()` | 恒值死代码桩，无调用点 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `AddRule()` | `恒真桩()` | `return true;`，无调用点 |
| `PostRule()` | `恒假桩()` | `return false;`，无调用点 |
| `AdapterAttr` | 未改名 | 类名被模板/引用固定 |
| `resourceMan` / `resourceCulture` | 未改名 | VB 资源模板的标准字段名，本身已有明确含义 |

## 关键方法与流程（按方法分段，写清步骤）
- `ResourceManager` getter：
  1. `if (object.ReferenceEquals(resourceMan, null))` —— 用引用比较判断（VB 的 `Is Nothing` 语义，不要改成 `== null` 的写法以外的东西）。
  2. `resourceMan = new ResourceManager("WindowsApplication1.Resources", typeof(AdapterAttr).Assembly)`。
  3. 返回 `resourceMan`。**没有加锁**（模板原样），首次并发访问理论上可能重复创建。
- `Culture` get/set：直接读写 `resourceCulture`，无逻辑。
- `恒真桩()` / `恒假桩()`：各返回一个布尔常量。

## 恶意行为 / 敏感操作
- 无。纯模板代码。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 资源基名：`WindowsApplication1.Resources`
- 其余无。

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无 `IL_xxxx:` / `num2 = N` / `ClearProjectError()`
- 保留全部 7 条 `// Token:` 注释
- `object.ReferenceEquals` 的写法按「五、不许美化的语义」原样保留（这是 VB `Is` 的编译产物）
