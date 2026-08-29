# ILLicenseModule.cs — .NET Reactor 加壳器伪方法类

## 文件作用
.NET Reactor 加壳器注入的占位类（命名 `ILLicenseModule`，Reactor 命名特征），含两个恒值方法：
`SelectThread()` 恒返回 `true`、`SearchThread()` 恒返回 `false`。这类方法通常被壳用来做许可证/
完整性检查的"假分支"或作为反静态分析诱饵（调用点被壳替换为动态解密）。无业务逻辑、无恶意行为，
但证实程序经 .NET Reactor 加壳（与 gypZRX1vuWc3aeye0R/Internal/Object.cs 的 IntelliLock 弹窗呼应）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| ILLicenseModule | Reactor 壳伪方法（恒 true/false 填充） |

## IOC / 硬编码
- 无。
