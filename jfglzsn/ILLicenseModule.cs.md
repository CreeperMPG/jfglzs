## 文件作用

授权校验的**空壳类**。两个方法都只有一句 `return 常量`，原本应该读取注册码 / 试用状态的位置已经被抹成固定值，典型的「破解版把授权门焊死」形态。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `ILLicenseModule` | 授权判定桩容器，两个方法分别恒返回 `true` / `false`，工程内**无任何调用点** |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ListRule()` | 未改名（语义：授权校验桩，恒真） | 方法体只有 `return true`，无字符串/API 可供推断真实语义，故按 AGENTS.md「沿用反编译出来的名字」保留，仅在注释里标明用途 |
| `InitRule()` | 未改名（语义：初始化/受限状态桩，恒假） | 同上，`return false` |

## 关键方法与流程

| 方法 | 步骤 |
| --- | --- |
| `ListRule` | 直接 `return true`（判定通过） |
| `InitRule` | 直接 `return false`（未初始化 / 非受限） |

两个方法构成「一真一假」的判定对，与本工程其余 7 组同类桩（`EventEvent.InstantiateDefinition/StopDefinition`、`RuleSetter.InterruptRule/MoveRule`、`SetterEvent` 内 4 组、`ListSetter.视图规则桩/登录规则桩`、`ThreadEvent` 无、`MethodSetter.ForgotRule/DefineRule`、`CallbackEvent.CountDefinition/SelectDefinition`、`ProductEvent.FlushDefinition/AddDefinition`、`Form3.GetRule`）结构完全一致——这是 .NET Reactor / IntelliLock 加壳时批量注入的「假分支生成器」，用于干扰反编译阅读。

## 恶意行为 / 敏感操作

本身无恶意行为。但它是一条**关键判读证据**：真实授权校验已被移除，程序可无授权全功能运行，说明拿到的样本是处理过的版本（壳被解、授权被旁路）。

## IOC / 硬编码

无字符串、无路径、无注册表键。仅两个编译期常量返回值 `true` / `false`。
