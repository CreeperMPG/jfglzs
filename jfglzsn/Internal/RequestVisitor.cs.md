# Internal/RequestVisitor.cs — 壳自定义 Attribute（自动生成，无逻辑）

## 文件作用
一个自定义 `Attribute` 类（`[AttributeUsage(...)]`、`[ComVisible(true)]`），带 3 个 bool 属性与 1 个 string 属性及各自的 get/set。这是 .NET Reactor/混淆器生成的**标记属性**（用于给混淆项打标签，类似 ObfuscationAttribute 的替代品），方法名（DisableVisitor/InterruptVisitor/ResetVisitor/ResolveVisitor/RegisterVisitor/DeleteVisitor/AwakeVisitor/CompareVisitor）全是混淆假名。无恶意逻辑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `RequestVisitor` | 壳标记 Attribute |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `_SchemaVisitor` / `DisableVisitor` / `InterruptVisitor` | bool 属性对 | 默认 true |
| `m_BaseVisitor` / `ResetVisitor` / `ResolveVisitor` | bool 属性对 | 默认 true |
| `m_AuthenticationVisitor` / `AwakeVisitor` / `CompareVisitor` | bool 属性对 | 默认 true |
| `_FieldVisitor` / `RegisterVisitor` / `DeleteVisitor` | string 属性对 | 默认 "" |

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 无。
