## 文件作用

程序集级元数据（`AssemblyInfo`）。没有一行可执行代码，但**是判断样本伪装手法的重要一页**：

- 公司/产品/标题全部伪装成微软的系统服务（`Company="Microsoft"`、`AssemblyTitle="service"`、`AssemblyProduct="service"`、
  `Copyright © Microsoft 2025`）；
- `[assembly: SuppressIldasm]` 禁止 ILDasm 反汇编，和 IntelliLock 壳一起构成反分析屏障；
- `Guid` 是本次编译产物的唯一标识，可用于在同一批样本之间比对「是否同一次构建」。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| 程序集特性块（无类型、无方法） | 声明版本/标题/公司/GUID/COM 可见性/ILDasm 抑制等元数据 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| （无） | （无） | 本文件只有 `[assembly: …]` 特性，没有被混淆器随机化的成员名 |

## 关键方法与流程

无方法。逐条特性含义：

| 特性 | 值 | 含义 |
| --- | --- | --- |
| `AssemblyVersion` | `1.0.0.0` | 程序集版本 |
| `AssemblyFileVersion` | `1.0.0.0` | 文件版本 |
| `AssemblyConfiguration` | `""` | 空 |
| `Guid` | `73204acb-20a4-433c-93f1-cd274c4ed922` | 程序集 GUID（每次新建工程生成一次） |
| `ComVisible` | `false` | 不对 COM 暴露 |
| `SuppressIldasm` | — | **禁止 ILDasm** |
| `AssemblyKeyName` / `AssemblyKeyFile` / `AssemblyDelaySign` | `""` / `""` / `false` | 未签名（强名称未使用） |
| `AssemblyTitle` / `AssemblyProduct` | `service` | 伪装成系统服务 |
| `AssemblyCompany` | `Microsoft` | 伪装 |
| `AssemblyCopyright` | `Copyright © Microsoft 2025` | 伪装（说明构建年份 2025） |
| `AssemblyDescription` / `AssemblyTrademark` | `""` / `""` | 空 |

## 恶意行为 / 敏感操作

1. **身份伪装**：`Company=Microsoft`、`Product/Title=service`、`Copyright © Microsoft 2025`
   —— 让服务管理器/杀软进程列表里看起来像「微软的系统服务」。
2. **反分析**：`[SuppressIldasm]` 让 `ildasm` 直接拒绝反汇编（对现代工具效果有限，但仍是刻意为之）。
3. 与 `app.manifest`（`requestedExecutionLevel level="asInvoker"`）配合：
   程序**不主动要管理员权限**，靠作为服务运行时自带的 SYSTEM/服务账户权限去关机，避免安装时触发 UAC 提示。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 程序集名 | `zmserv`（见 csproj `AssemblyName`） |
| GUID | `73204acb-20a4-433c-93f1-cd274c4ed922` |
| 版本 | `1.0.0.0`（程序集 / 文件） |
| 伪装字段 | `Company`=`Microsoft`、`Product`=`service`、`Title`=`service`、`Copyright`=`Copyright © Microsoft 2025` |
| 目标框架 | `v4.5`（见 csproj `TargetFrameworkVersion`） |
| 反分析标记 | `SuppressIldasm` |
| 清单权限 | `asInvoker`（`app.manifest`），不请求提升 |
| 路径 / 注册表 / URL / 进程名 | 无 |
