# AssemblyInfo.cs — 程序集元数据（全部为空，启用 SuppressIldasm）

## 文件作用
标准程序集属性文件。所有元数据（标题、产品、公司、版权、描述、密钥）均为空字符串，版本固定 1.0.0.0，无强名称（公钥 Token 为空——注意 InvocationProduct 用公钥 Token 派生密钥，此处为空意味着异或密钥等于硬编码原值）。特征点：`[assembly: SuppressIldasm]`（防 IL 反汇编）与 `[assembly: AllowPartiallyTrustedCallers]`（允许部分信任调用）。

## 关键属性
- `SuppressIldasm`：禁止 ildasm 反汇编（混淆/加壳常见配套手段）
- `AllowPartiallyTrustedCallers`：APTC 标记
- `ComVisible(false)`、GUID `ca03f0bd-3033-4420-8b48-e574750c6d44`
- 无强名称、无版本描述信息（隐蔽作者身份）

## 恶意行为 / 敏感操作
- 无直接行为；空元数据 + SuppressIldasm 为隐蔽/抗分析特征。

## IOC / 硬编码
- Assembly GUID：`ca03f0bd-3033-4420-8b48-e574750c6d44`
