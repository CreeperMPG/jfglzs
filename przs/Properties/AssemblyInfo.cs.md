# Properties\AssemblyInfo.cs.md — 程序集元数据

## 文件作用

程序集属性清单。原样本被 .NET Reactor 处理过，作者信息全被清空（Title/Company/Description/Product/Copyright
都是空字符串），只剩版本号、GUID 和一个反分析特性 `SuppressIldasm`。

## 类与职责

| 类 / 模块 | 真实职责 |
| --- | --- |
| 程序集特性 | 标识版本、GUID、可见性、是否禁止 ildasm |
| （无类/方法） | 本文件只有 `[assembly: …]` 特性 |

## 反混淆命名映射

不涉及命名：本文件没有类型或成员，只有程序集级特性。

## 关键方法与流程

| 特性 | 值 | 含义 |
| --- | --- | --- |
| `AssemblyVersion` / `AssemblyFileVersion` | `1.0.0.0` | 版本（默认值，没随"13.4"改） |
| `Guid` | `ca2000e9-458b-4ab6-989c-c47028ea8888` | 程序集 GUID |
| `SuppressIldasm` | — | **禁止 ildasm 反编译**，反分析手段 |
| `AssemblyTitle` / `Description` / `Copyright` / `Trademark` / `Company` / `Product` | `""` | 作者刻意留空，不泄露身份 |
| `AssemblyKeyFile` / `AssemblyKeyName` | `""` | 未签名 |
| `AssemblyDelaySign` | `false` | 延迟签名关闭 |
| `ComVisible` | `false` | 不暴露给 COM |

## 恶意行为 / 敏感操作

| 行为 | 说明 |
| --- | --- |
| **反分析** | `[assembly: SuppressIldasm]` 直接禁用 .NET 自带的 `ildasm` |
| **身份隐藏** | 所有描述性字段为空 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| GUID | `ca2000e9-458b-4ab6-989c-c47028ea8888` |
| 版本 | `1.0.0.0` |
| 反分析特性 | `SuppressIldasm` |
