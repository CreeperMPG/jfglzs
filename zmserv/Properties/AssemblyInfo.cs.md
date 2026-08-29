# Properties/AssemblyInfo.cs — 程序集元数据（IOC 来源）

## 文件作用
程序集属性声明（自动生成 + 攻击者填写）。元数据被伪装成微软官方程序（Title/Product=`service`、Company=`Microsoft`、Copyright `© Microsoft 2025`），Version 1.0.0.0，`AllowPartiallyTrustedCallers` + `SuppressIldasm` 抑制反编译。GUID 可作样本关联 IOC。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| （无类型） | 程序集级 Attribute 集合 |

## 关键属性（已反混淆）
| 属性 | 值 | 说明 |
|------|-----|------|
| AssemblyTitle / Product | `service` | 伪装名 |
| AssemblyCompany | `Microsoft` | 伪装公司 |
| AssemblyCopyright | `Copyright © Microsoft 2025` | 伪装版权 |
| AssemblyVersion / FileVersion | `1.0.0.0` | 版本 |
| AssemblyGuid | `73204acb-20a4-433c-93f1-cd274c4ed922` | 程序集 GUID（样本关联 IOC） |
| AllowPartiallyTrustedCallers | 有 | 允许部分信任调用 |
| SuppressIldasm | 有 | 抑制 ildasm 反编译 |
| ComVisible | false | 不暴露 COM |
| 程序集名（csproj） | `zmserv` | 工程名 |
| 目标框架（csproj） | .NET Framework 3.5 | 老框架，兼容性强 |

## 恶意行为 / 敏感操作
- **伪装元数据**：冒充 Microsoft 产品"service"，版权年份 2025 与样本制作时间吻合。
- **反分析**：`SuppressIldasm` 抑制反编译。

## IOC / 硬编码
- 程序集名：`zmserv`（服务可执行 `zmserv.exe`）
- 程序集 GUID：`73204acb-20a4-433c-93f1-cd274c4ed922`
- 伪装字符串：`service` / `Microsoft` / `Copyright © Microsoft 2025`
- 工程 GUID（csproj）：`{23B9F6C2-A449-4D2B-87EC-34A2645EFD94}`
