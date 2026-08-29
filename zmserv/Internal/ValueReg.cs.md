# Internal/ValueReg.cs — 强类型资源包装类（Resources.resources，自动生成）

## 文件作用
VB.NET 资源文件的强类型包装（`ResX` 代码生成器产物）：提供 `ResourceManager`（指向 `zmserv.Resources`，即嵌入资源 `Resources.resources`）与 `Culture` 属性。**无任何业务逻辑**，仅一个恒假的混淆填充方法。恶意载荷（如果有）可能以资源形式存在于 `Resources.resources`/`g.resources` 中，但本类本身无行为。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `ValueReg` | zmserv.Resources 强类型访问器 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `ResourceManager` | 资源管理器 | `new ResourceManager("zmserv.Resources", 程序集)` |
| `Culture` | 区域文化 | get/set |
| `resourceMan/resourceCulture` | 缓存字段 | 懒初始化 |
| `StartParser` | 混淆填充 | 恒假 |

## 关键方法与流程
- `ResourceManager` getter：懒创建 `ResourceManager("zmserv.Resources", typeof(ValueReg).Assembly)`。
- 提示：`g.resources` 与 `Resources.resources` 均为嵌入资源，前者包含的 GUID 资源（`c3a777aa-...`、`b215f184-...`、`b0494a1f-...` 系列）由 CustomerReg/DispatcherReg 在运行时读取。

## 恶意行为 / 敏感操作
- 无（资源容器本身需另行解析 g.resources/Resources.resources）。

## IOC / 硬编码
- 资源基名：`zmserv.Resources`（对应 `Resources.resources`）
