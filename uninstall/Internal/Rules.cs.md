# Internal/Rules.cs 分析

## 文件作用

VB.NET 自动生成的 **My.Resources** 访问模块（`[StandardModule]` + `[HideModuleName]`）。
资源容器名写死为 `WindowsApplication1.Resources`，对应工程里唯一的 `Resources.resx`。

工程没有声明任何强类型资源属性，所以这个类只提供 `ResourceManager` / `Culture` 两个开关，
而且**全工程没有任何地方取资源**——属于模板残留。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `MyResources`（原 `Rules`） | My.Resources 入口：`ResourceManager` 懒加载 + `Culture` 读写 |
| `ResourceManager` 属性 | 懒加载 `new ResourceManager("WindowsApplication1.Resources", typeof(MyResources).Assembly)` |
| `Culture` 属性 | 取/设 `My.Resources.Culture` |
| `资源管理器缓存` / `当前区域文化` | 上面两个属性的存储 |
| `混淆器占位方法_恒真/恒假` | 原 `InvokeParser` / `ManageParser`，占位 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `Rules` | `MyResources` | `[GeneratedCode("StronglyTypedResourceBuilder")]` + 资源名 `WindowsApplication1.Resources` 证明它是 My.Resources |
| `resourceMan` | `资源管理器缓存` | `ResourceManager` 单例 |
| `resourceCulture` | `当前区域文化` | 原字段类型 `object`，`Culture` 取值时强转回 `CultureInfo` |
| `InvokeParser` / `ManageParser` | `混淆器占位方法_恒真/恒假` | 占位 |

## 关键方法与流程

### `ResourceManager` getter

```
if (ReferenceEquals(资源管理器缓存, null))
    资源管理器缓存 = new ResourceManager("WindowsApplication1.Resources", typeof(MyResources).Assembly);
return 资源管理器缓存;
```

### `Culture` getter/setter

`get`：`return (CultureInfo)当前区域文化;` —— 原字段声明为 `object`，VB `Option Strict Off` 下
取值时隐式转回 `CultureInfo`；反混淆时用显式强转表达同一语义（`null` 仍是 `null`）。
`set`：直接赋给 `object` 字段。

## 恶意行为 / 敏感操作

无。清点价值：

- 证明本工程只有一个资源集 `WindowsApplication1.Resources`（`Resources.resx`、`g.resx` 都在 csproj 里）；
- 而 .NET Reactor 的加密字符串表 / 补丁表资源（GUID 名）**不在** csproj 里，是加壳时写进 exe 的
  —— 这也解释了为什么重建工程后 `IndexerConfig` 的两个资源读不到。

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 资源容器名 | `WindowsApplication1.Resources` | `ResourceManager` 的 baseName |
| 特性 | `[StandardModule]`、`[HideModuleName]`、`[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder","16.0.0.0")]` | 模板产物标记 |
| Token | 类型 `0x02000008`；属性 `0x17000008/09`；字段 `0x0400000A/0B` | 元数据行号 |
