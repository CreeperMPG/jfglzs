# ILLicenseModule.cs.md — .NET Reactor 授权桩

## 文件作用

`.NET Reactor` 的许可证模块桩（类名是壳留下的）。两个方法都没有参数、不看任何状态，
一个恒 `true`、一个恒 `false`，本工程里**没有任何调用者**。
它存在只说明：原样本是用 .NET Reactor 加壳的（壳在未注册版本里插入这类判断占位），
解壳之后它们被原样保留下来。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| `ILLicenseModule` | 授权判断占位类 |
| `RunTag()` | 恒 `true` |
| `CancelTag()` | 恒 `false` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `RunTag` / `CancelTag` | 保留原名 | 桩函数，改名没有收益；已在 XML 注释里写明"恒 true / false" |

## 关键方法与流程

```text
RunTag()    → return true;
CancelTag() → return false;
```

没有控制流、没有 `goto`、没有 `try/catch`。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| 无 | 既不读文件也不写注册表；是壳的痕迹而不是恶意功能 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 返回值 | `true` / `false` |
