# Internal/ErrorMapper.cs

## 文件作用

VB.NET 的强类型资源访问器，即 `My.Resources` 的实现。它用 `ResourceManager` 按基名 `WindowsApplication1.Resources` 去本程序集里找资源。

工程里的 `Resources.resx` 是空容器（只有标准 resx 头），所以这个类实际取不到任何条目；窗体资源走的是 `Form1.resx` / `Form3.resx` 的 `ComponentResourceManager`，不经过这里。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ErrorMapper`（`[StandardModule]`） | 资源管理器懒加载 + 区域设置；另含两个恒值桩 |
| `ResourceManager` 属性 | 首次访问时创建并缓存 `ResourceManager("WindowsApplication1.Resources", 本程序集)` |
| `Culture` 属性 | 读写资源查找区域，后备字段原类型为 `object` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `resourceMan` | `资源管理器缓存` | 懒加载的 `ResourceManager` 实例 |
| `resourceCulture` | `当前区域` | 资源查找区域，元数据类型 `object` |
| `PushFacade()` | `返回真占位()` | 恒 `return true;` |
| `PrepareFacade()` | `返回假占位()` | 恒 `return false;` |

## 关键方法与流程

1. **`ResourceManager` 取值器**
   - `object.ReferenceEquals(缓存, null)` 判断（VB `Is Nothing` 的还原形式，保留写法）；
   - 为空则 `new ResourceManager("WindowsApplication1.Resources", typeof(ErrorMapper).Assembly)`；
   - 返回缓存。**无锁**，并发首次访问可能重复创建。
2. **`Culture` 取值器 / 赋值器**
   - 取值器直接返回 `object` 字段；原版靠 VB 的隐式收窄转成 `CultureInfo`，此处用显式强转还原（字段永远是 `CultureInfo`，行为一致）。
   - 赋值器只是把参数塞进字段，无校验。
3. **`返回真占位()` / `返回假占位()`**：常量返回，无调用点。

## 恶意行为 / 敏感操作

无。纯资源读取封装，不碰注册表/文件/进程/网络。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 资源基名 | `WindowsApplication1.Resources` |
| 生成器 | `System.Resources.Tools.StronglyTypedResourceBuilder`，版本 `16.0.0.0` |
| 模板标识 | `[StandardModule]`、`[HideModuleName]`、`[GeneratedCode]` |
| IL 行号 | 类型 `0x02000008`，属性 `0x17000008` / `0x17000009`，方法 `0x0600002A` / `0x0600002B` |
