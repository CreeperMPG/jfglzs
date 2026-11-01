# Properties\AssemblyInfo.cs —— 文件级分析

## 文件作用
程序集级元数据特性清单。除常规版本号/GUID 外，包含一条反分析特性 `[assembly: SuppressIldasm]`：它让 `ildasm` 拒绝反汇编本程序集，是样本作者主动加入的对抗手段。

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| （程序集特性清单，无类型定义） | 声明版本、GUID、COM 可见性与反反编译开关 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| —— | —— | 本文件没有可反混淆的标识符；特性本身即原文 |

## 关键方法与流程（按方法分段，写清步骤）
- 无方法。全部是 `[assembly: ...]` 声明，顺序即原顺序。
- 逐条含义：
  - `AssemblyVersion/AssemblyFileVersion = 1.0.0.0`：版本号被刻意抹成最普通的默认值，不暴露真实版本（真实版本 13.4 只出现在 UI/注册表字符串里）。
  - `Guid("1484da4c-c185-46c9-9ac8-389a117b3888")`：程序集 GUID，可作为同一编译批次样本的关联标识。
  - `SuppressIldasm`：禁止 ildasm 反汇编，反分析。
  - `AssemblyKeyName("")`、`AssemblyKeyFile("")`、`AssemblyDelaySign(false)`：未签名（无强名称）。
  - `ComVisible(false)`：不对 COM 暴露。
  - `AssemblyTitle/Description/Copyright/Trademark/Company/Product` 全为空串：刻意不留下作者/公司信息。

## 恶意行为 / 敏感操作
- **反分析**：`[assembly: SuppressIldasm]` 阻止使用微软官方 `ildasm` 查看 IL（对本仓库无实际阻碍，因为用的是 dnSpy/de4dot 链路）。
- **信息抹除**：Title/Company/Product/Copyright 全部留空，规避溯源。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- GUID：`1484da4c-c185-46c9-9ac8-389a117b3888`
- 版本：`1.0.0.0`（AssemblyVersion 与 AssemblyFileVersion 相同）

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无
- 未改动任何特性取值与顺序，只加了一段说明性注释
