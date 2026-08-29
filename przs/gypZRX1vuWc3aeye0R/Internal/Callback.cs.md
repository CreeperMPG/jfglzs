# Callback.cs — 强类型资源访问器（Resources 资源文件读取）

## 文件作用
反混淆后真实身份：**强类型资源类**（`StronglyTypedResourceBuilder` 生成，ConfuserEx 改名为 `Callback`）。
内部持有一个 `ResourceManager`，指向程序集内名为 `"Resources"` 的资源文件，供 `My.Resources` 体系
读取嵌入资源（图标等）。`Culture` 属性用于切换资源区域性。本项目业务代码未直接调用它（图标等由
`ComponentResourceManager` 直接读），属自动生成的支撑类；`FillThread`/`ConnectThread` 为混淆器
填充的无意义方法。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Callback | My.Resources 强类型资源管理器（读取 "Resources" 嵌入资源） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `Callback.ResourceManager` | `资源管理器` | `new ResourceManager("Resources", 程序集)` 惰性单例 |
| `Callback.Culture` | `资源区域性` | get/set `resourceCulture` |
| `FillThread`（恒 true）/`ConnectThread`（恒 false） | 混淆器填充方法 | 无意义 |

## 关键方法与流程
### 资源管理器（ResourceManager getter）
- **流程**：`resourceMan` 为 null 时创建 `new ResourceManager("Resources", typeof(Callback).Assembly)` 并缓存。

## 恶意行为 / 敏感操作
- 无。仅资源访问支撑类。

## IOC / 硬编码
- 资源名：`"Resources"`。
