# Internal/ReaderVisitor.cs — 强类型资源访问器（Resources 生成类，自动生成）

## 文件作用
VB.NET 编译器生成的**强类型资源访问器**（`[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder","2.0.0.0")]`），用于访问程序集嵌入资源 `WindowsApplication1.Resources`。`ResourceManager` 属性懒加载 ResourceManager；`Culture` 读写资源区域性。`VisitMethod` 恒 false 为混淆桩。无手工逻辑、无恶意行为。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `ReaderVisitor` | 资源管理器访问器 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `ResourceManager` | 资源管理器 | 懒加载 `WindowsApplication1.Resources` |
| `Culture` | 资源区域性 | get/set |
| `VisitMethod` | 桩 | 恒 false |
| `resourceMan` / `resourceCulture` | 缓存字段 | ResourceManager / CultureInfo |

## 恶意行为 / 敏感操作
- 无（资源内容本身可能是壳加密载荷，但本类仅是访问通道）。

## IOC / 硬编码
- 资源基名：`WindowsApplication1.Resources`。
