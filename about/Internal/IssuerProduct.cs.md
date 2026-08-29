# IssuerProduct.cs — 混淆器自定义 Attribute（无实义）

## 文件作用
. NET 自定义特性类（`AttributeUsage` 覆盖几乎所有目标，`AllowMultiple=true`），带 3 个布尔属性与 1 个字符串属性，均为 getter/setter 对。**未被业务代码使用**，是混淆器（Reactor/ConfuserEx 类工具）注入的占位特性，用于干扰反混淆工具与元数据分析。属性名/方法名被混淆为 `*Product`。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| IssuerProduct : Attribute | 混淆残留特性（无业务语义） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `StopProduct/AddProduct` | bool 属性 get/set | 默认 true |
| `StartProduct/SelectProduct` | bool 属性 get/set | 默认 true |
| `InstantiateProduct/CheckProduct` | bool 属性 get/set | 默认 true |
| `FindProduct/ViewProduct` | string 属性 get/set | 默认 "" |
| `_SingletonProduct/_MessageProduct/m_ComposerProduct/_IteratorProduct` | 后备字段 | — |
| `CalcListener` | 混淆桩 | 恒 true |

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 无。
