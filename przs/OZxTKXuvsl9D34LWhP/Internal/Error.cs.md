# Error.cs.md — VB 强类型资源访问器（Resources）

## 文件作用

`OZxTKXuvsl9D34LWhP\Internal\Error.cs` 是 `Resources.resx` 的强类型访问器（`Error` 是混淆名，
原类型名应为 `Resources`，由 `StronglyTypedResourceBuilder` 生成）。它只暴露一个
`ResourceManager`（绑定程序集里的 `Resources` 资源）和一个 `Culture` 属性。
本工程没有定义任何具体资源项，所以这个类是空壳样板；两个 `XxxTag()` 是 .NET Reactor 的授权标记桩。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Error`（`[StandardModule]`） | 资源管理器入口 |
| `ResourceManager` 属性 | 懒创建 `new ResourceManager("Resources", 本程序集)` |
| `Culture` 属性 | 设置资源查找的区域性 |
| `CompareTag()` / `InvokeTag()` | 授权标记桩，恒 true / false |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `Error`（类名） | 保留原名 | 与文件名绑定，且全工程无外部引用 |
| `resourceMan` | 保留原名 | ResourceManager 缓存 |
| `resourceCulture` | 保留原名 | 区域性缓存 |
| `CompareTag` / `InvokeTag` | 保留原名 | 授权标记桩 |

## 关键方法与流程

### `ResourceManager` getter

`resourceMan` 为 null → `new ResourceManager("Resources", typeof(Error).Assembly)`；返回缓存。
资源基名是 `"Resources"`，对应工程根目录的 `Resources.resx`（该 resx 只有默认空条目）。

### `Culture` get / set

直接读写 `resourceCulture`（类型是 `object`，不是 `CultureInfo`——VB 生成器就是这么写的，保留原样）。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| 无 | 纯资源访问样板；不碰文件/注册表/进程 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 资源基名 | `Resources` |
| 生成器版本 | `System.Resources.Tools.StronglyTypedResourceBuilder`, `16.0.0.0` |
