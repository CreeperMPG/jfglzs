# ItemProduct.cs — 强类型资源访问器（WindowsApplication1.Resources）

## 文件作用
VS 生成的强类型资源访问器（`StronglyTypedResourceBuilder`）：懒加载 `ResourceManager("WindowsApplication1.Resources", ...)`，供 `My.Resources` 使用。资源本体在 `Resources.resources` / `Form1.resources` / `Form3.resources`（背景图、图标等）。无手工逻辑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| ItemProduct | My.Resources 强类型访问器 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `ResourceManager` | 资源管理器单例 | 基名 `WindowsApplication1.Resources` |
| `Culture` | 当前区域性 | — |
| `QueryListener/ViewListener` | 混淆桩 | 恒 true / false |
| `resourceMan` / `resourceCulture` | 缓存字段 | — |

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 资源基名：`WindowsApplication1.Resources`
