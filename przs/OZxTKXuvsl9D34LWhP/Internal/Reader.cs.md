# Reader.cs.md — 壳生成的通用标记特性

## 文件作用

`OZxTKXuvsl9D34LWhP\Internal\Reader.cs` 定义了一个可标在"程序集/类/结构/枚举/方法/属性/字段/事件/接口/参数/委托"
上的特性类（`Reader` 是混淆名）。它只有 3 个 bool 字段和 1 个 string 字段，
在本工程里**没有任何 `[Reader(...)]` 使用点**——保留它只是为了让元数据里对它的引用仍能解析。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `Reader : Attribute` | 通用标记特性（无逻辑） |
| `SetWatcher` / `DeleteWatcher` | 读写 `_Serializer`（默认 true） |
| `AddWatcher` / `MapWatcher` | 读写 `_Callback`（默认 true） |
| `VerifyWatcher` / `CalculateWatcher` | 读写 `collection`（默认 true） |
| `QueryWatcher` / `SearchWatcher` | 读写 `code`（默认空串） |
| `QueryTag()` | 授权标记桩，恒 true |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| 类 `Reader` | 保留原名 | 与文件名绑定 |
| `bool_0` / `string_0` | `值` | 参数名，原为 dnSpy 的按类型编号 |
| `_Serializer` / `_Callback` / `collection` / `code` | 保留原名 | 特性承载的数据 |
| `QueryTag` | 保留原名 | 授权标记桩 |

## 关键方法与流程

全是"一行 getter / 一行 setter"式的方法，没有控制流、没有 `try/catch`、没有 `goto`：

- `SetWatcher()` → `return this._Serializer;`
- `DeleteWatcher(bool 值)` → `this._Serializer = 值;`
- `AddWatcher()` → `return this._Callback;`
- `MapWatcher(bool 值)` → `this._Callback = 值;`
- `QueryWatcher()` → `return this.code;`
- `SearchWatcher(string 值)` → `this.code = 值;`
- `VerifyWatcher()` → `return this.collection;`
- `CalculateWatcher(bool 值)` → `this.collection = 值;`

`[AttributeUsage(..., AllowMultiple = true, Inherited = false)]` + `[ComVisible(true)]` 原样保留。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| 无 | 纯特性定义；它本身不执行任何东西 |

> 这类"带 3 个 bool 的特性"是 .NET Reactor 的元数据填充物，用来干扰反编译器的类型恢复。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 默认值 | `_Serializer = true`、`_Callback = true`、`collection = true`、`code = ""` |
| 特性范围 | Assembly / Class / Struct / Enum / Method / Property / Field / Event / Interface / Parameter / Delegate |
