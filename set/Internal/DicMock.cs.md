# Internal/DicMock.cs — 强类型资源包装（WindowsApplication1.Resources）

## 文件作用
标准强类型资源类（`GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder","2.0.0.0")`），包装程序集资源 `WindowsApplication1.Resources`，提供 ResourceManager 与 Culture 访问。样本（如 yl.wav/huifu.wav/zy\\ 假浏览器/壁纸等）通过资源方式携带载荷文件。代码为模板生成，无手工逻辑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| DicMock（StandardModule） | 资源访问门面 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| ResourceManager | 资源管理器单例 | new ResourceManager("WindowsApplication1.Resources", 程序集) |
| Culture | 资源文化 | 读/写 resourceCulture |
| DisableClass() | 占位 | return true |

## 恶意行为 / 敏感操作
- 无直接逻辑；指示程序集内嵌资源（含可能的载荷文件）

## IOC / 硬编码
- 资源基名：`WindowsApplication1.Resources`
