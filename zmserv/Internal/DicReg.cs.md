# Internal/DicReg.cs — 自定义 Attribute（保护器标记 / 混淆填充）

## 文件作用
本文件定义了一个可应用于几乎所有目标（程序集/类/方法/字段/参数等，AllowMultiple）的密封 Attribute，含 4 个可读写属性：3 个 bool（默认 true）和 1 个 string（默认空串）。此类无任何逻辑，是 .NET Reactor/ConfuserEx 风格的保护器**标记属性**（用于标注混淆/保护开关），也可能是脱壳后的残留。其余为混淆填充方法。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `DicReg` | 自定义标记 Attribute |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `ChangeList/MoveList` | bool 属性 1（m_MappingReg） | get/set，默认 true |
| `OrderList/ForgotList` | bool 属性 2（_ObjectReg） | get/set，默认 true |
| `ComputeList/EnableList` | bool 属性 3（m_TokenizerReg） | get/set，默认 true |
| `CalculateList/ValidateList` | string 属性（resolverReg） | get/set，默认 "" |
| `ExcludeParser` | 混淆填充 | 恒真 |

## 关键方法与流程
- 仅属性访问器，无行为逻辑。方法均为一行 get/set。
- `[AttributeUsage(..., AllowMultiple=true, Inherited=false)]`、`[ComVisible(true)]`：供保护器在 IL 层面引用。

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 无恶意 IOC。可作为"该样本经混淆器处理"的元数据指纹。
