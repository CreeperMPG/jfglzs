# Properties/AssemblyInfo.cs 分析

## 文件作用

程序集级特性（`[assembly: …]`）。这里的信息被加壳器**刻意清空**：标题、描述、公司、产品、
版权、商标全是空字符串，只留一个随机 GUID 与固定的 `1.0.0.0` 版本号。

同时带 `[assembly: SuppressIldasm]` —— 阻止 `ildasm` 直接反汇编该程序集，是「防静态分析」标记。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `AssemblyInfo.cs`（无类型） | 只提供程序集元数据：版本、GUID、可见性、IL 反汇编抑制 |

## 反混淆命名映射

无方法/字段可改名。属性值本身全部保留（它们就是证据）：

| 原值 | 含义 |
| --- | --- |
| `AssemblyVersion("1.0.0.0")` / `AssemblyFileVersion("1.0.0.0")` | 固定版本，不随 13.4 这种产品版本变化 |
| `Guid("26148b78-26a7-4e21-aa4f-2f2b74167088")` | 程序集 GUID（`uninstall` 独占，可与其它 5 个样本对比） |
| `AssemblyTitle/Description/Company/Product/Copyright/Trademark = ""` | 全部清空，抹掉作者/产品线索 |
| `ComVisible(false)` | 不向 COM 暴露 |
| `AssemblyDelaySign(false)` / `AssemblyKeyName("")` / `AssemblyKeyFile("")` | 不签名（因此 `GetPublicKeyToken()` 为空，见 `IndexerConfig` 里的强名称分支） |
| `SuppressIldasm` | 抑制 ildasm |

## 关键方法与流程

无方法。编译期这些特性被写入程序集清单；运行时 `Assembly.GetName()` 能读到 GUID 与版本。

与本工程其它文件的关联点：

- `IndexerConfig.解密字符串` / `还原方法体` 都会读 `Assembly.GetName().GetPublicKeyToken()`：
  本程序集**未签名**，所以那段「用公钥标记覆盖 IV」的分支不会执行（IV 保持硬编码值）。
- `AttributeConfig` 用 `Assembly.Load` 加载内嵌 DLL，也是靠程序集全名匹配。

## 恶意行为 / 敏感操作

| 类型 | 具体行为 |
| --- | --- |
| 反分析 | `SuppressIldasm` 阻止 ildasm 打开程序集 |
| 溯源抹除 | 版本固定 `1.0.0.0`，所有描述性字段清空 |
| 说明 | 这段代码本身不执行任何动作，但它是「样本被专业加壳处理过」的直接证据 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 程序集 GUID | `26148b78-26a7-4e21-aa4f-2f2b74167088` |
| 版本 | `1.0.0.0`（AssemblyVersion 与 AssemblyFileVersion 相同） |
| 特性 | `SuppressIldasm`、`ComVisible(false)`、`AssemblyDelaySign(false)` |
| 签名 | 无强名称（`AssemblyKeyFile` / `AssemblyKeyName` 为空） |
