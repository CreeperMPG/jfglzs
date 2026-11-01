# AssemblyInfo.cs

## 文件作用

程序集清单属性文件。原 VB.NET 工程由 Visual Studio 生成，.NET Reactor 重新打包后这里的信息被整体抹平：版本统一 `1.0.0.0`，标题、公司、描述、产品、版权、商标全部是空串。所以它本身不提供任何产品身份线索，只能作为「被壳重打包」的证据。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| 程序集级 attribute（无类型） | 声明版本、GUID、COM 可见性、强名称设置、ILDasm 抑制 |

## 反混淆命名映射

无成员可命名（全部是程序集级 attribute）。仅整理注释。

## 关键方法与流程

无方法。逐条含义：

1. `AssemblyVersion` / `AssemblyFileVersion` = `1.0.0.0`：壳重打包后的统一版本，不代表真实产品版本。
2. `Guid("ca03f0bd-3033-4420-8b48-e574750c6d44")`：重打包时生成的新 GUID。
3. `AssemblyDelaySign(false)` + `AssemblyKeyName("")` + `AssemblyKeyFile("")`：未做延迟签名、无强名称密钥——即本程序集**没有强名称签名**，被篡改不会导致加载失败。
4. `SuppressIldasm`：给 ILDasm 打标记，阻止其反汇编本程序集，属反分析加固。
5. `ComVisible(false)`：不向 COM 暴露。
6. 空厂商字段：故意清空以隐藏生产者。

## 恶意行为 / 敏感操作

- `SuppressIldasm` 属**反分析**手段（阻止 ILDasm 直接反汇编）。
- 清空公司/产品/版权 + 1.0.0.0 版本 + 无签名，是典型的「去归属 + 便于二次打包」组合，常见于国产机房管理/学生机管控类软件的分发版本。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 程序集 GUID | `ca03f0bd-3033-4420-8b48-e574750c6d44` |
| 版本号 | `1.0.0.0`（AssemblyVersion 与 AssemblyFileVersion 相同） |
| 目标框架 | 工程文件声明 `v4.5`（本文件不含此信息，见 `about.csproj`） |
| 反分析标记 | `System.Runtime.CompilerServices.SuppressIldasmAttribute` |
