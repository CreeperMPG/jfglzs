# Status.cs — 混淆器生成的 Attribute 辅助类

## 文件作用
反混淆后真实身份：**混淆器（ConfuserEx/Reactor）生成的辅助 Attribute**，允许挂在
程序集/类/方法/属性/字段等几乎所有目标上，携带两个 bool 与一个 string 配置值
（`_Exporter`/`manager`/`_Composer`/`_Parser`，全部默认 true/""）。全程序无任何业务代码引用它，
纯为混淆框架预留；`CloneThread`/`InvokeThread` 为混淆器填充方法。无恶意行为。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Status | 混淆器辅助 Attribute（配置载体，未使用） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `SelectSchema/RestartSchema` | `_Exporter 读/写` | bool 配置 |
| `RemoveSchema/CheckSchema` | `manager 读/写` | bool 配置 |
| `QuerySchema/ChangeSchema` | `_Composer 读/写` | bool 配置 |
| `IncludeSchema/FlushSchema` | `_Parser 读/写` | string 配置 |
| `CloneThread`（恒 true）/`InvokeThread`（恒 false） | 混淆器填充方法 | 无意义 |

## 关键方法与流程
- 纯属性存取器，无流程。

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 无。
