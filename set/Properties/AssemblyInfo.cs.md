# Properties/AssemblyInfo.cs — 程序集元数据（自动生成）

## 文件作用
程序集属性清单。注意几个混淆/加壳特征：`SuppressIldasm`（防 ildasm 反编译）、`AllowPartiallyTrustedCallers`（APTCA）、所有 Title/Product/Company 等均为空字符串、程序集 GUID `1484da4c-c185-46c9-9ac8-389a117b3888`、版本 1.0.0.0。`[assembly: SuppressIldasm]` 与空元数据是恶意样本与加壳程序的常见做法（隐藏身份、阻碍分析）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| （程序集特性） | 程序集元数据 |

## 恶意行为 / 敏感操作
- SuppressIldasm：反分析特征
- 空公司/产品/描述：隐藏作者身份

## IOC / 硬编码
- 程序集 GUID：`1484da4c-c185-46c9-9ac8-389a117b3888`
- 版本：1.0.0.0
