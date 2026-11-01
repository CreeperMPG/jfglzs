## 文件作用

程序集清单（assembly manifest），只有特性声明、没有任何类型或方法。它决定了程序集身份（版本号、GUID、COM 可见性、是否强名称），并带一条**反分析标记** `[SuppressIldasm]`。

## 类与职责

本文件不含类。以下是各特性的真实含义与判读价值：

| 特性 | 值 | 真实职责 / 判读价值 |
| --- | --- | --- |
| `AssemblyVersion` | `10.2.0.0` | 程序集版本 |
| `AssemblyFileVersion` | `10.2.0.0` | 文件版本 |
| `Guid` | `867bd42f-1e95-4fbd-9aa5-f814e85d5036` | 程序集标识（`ComVisible(false)`，未注册 COM） |
| `SuppressIldasm` | — | **阻止 ILDasm 反汇编**：反分析标记 |
| `ComVisible` | `false` | 不向 COM 暴露 |
| `AssemblyKeyName` / `AssemblyKeyFile` | `""` | 未强名称签名 |
| `AssemblyDelaySign` | `false` | 非延迟签名 |
| `AssemblyTitle` / `Description` / `Copyright` / `Trademark` / `Company` / `Product` / `Configuration` | 全为空串 | **刻意抹掉厂商与产品标识**（去指纹） |

## 反混淆命名映射

本文件无可改名的标识符（只有框架特性）。命名说明：

| 项 | 说明 |
| --- | --- |
| 版本号 `10.2.0.0` | **注意与对外宣称的「机房管理助手 13.4」不一致**——程序集版本是开发期遗留值，不能作为版本判定依据；判定版本应看文件名/发布包名或主窗体的业务字符串 |
| `SuppressIldasm` | 说明原作者有意阻止静态分析，是"恶意意图"的间接证据之一 |

## 关键方法与流程

无。仅程序集级特性声明，顺序即原版顺序（`AssemblyVersion` → `Configuration` → `Guid` → `FileVersion` → `SuppressIldasm` → `KeyName` → `DelaySign` → `KeyFile` → `ComVisible` → `Title`/`Description`/`Copyright`/`Trademark`/`Company`/`Product`）。

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| `[SuppressIldasm]` | 反分析：让 `ildasm.exe` 拒绝打开该程序集 |
| 元数据全空 | 去指纹：`AssemblyTitle`/`Company`/`Product` 等全为空串，正常商业软件不会这么干 |

这两条是「规避分析 + 隐藏来源」的组合，与 `MethodSetter` 里那段 IntelliLock 未注册提示、以及 `PrototypeSetter` 的内嵌加密程序集加载共同构成样本的自我保护层。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 程序集 GUID | `867bd42f-1e95-4fbd-9aa5-f814e85d5036` |
| 程序集版本 / 文件版本 | `10.2.0.0` |
| 强名称 | 无（`KeyFile`/`KeyName` 为空、`DelaySign=false`） |
| 目标框架（来自 csproj） | .NET Framework 3.5（参见工程文件） |
