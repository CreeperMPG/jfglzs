# AssemblyInfo.cs — 程序集元数据（自动生成，含加壳特征）

## 文件作用
程序集属性声明文件，全部为自动生成的空元数据：`AssemblyVersion 1.0.0.0`、程序集 GUID
`ca2000e9-458b-4ab6-989c-c47028ea8888`，公司/标题/版权/产品/描述全部为空。值得注意的加壳/混淆特征：
- `[assembly: SuppressIldasm]`：禁止 ildasm 反汇编（反分析手段，通常由 .NET Reactor 等加壳器添加）
- `[assembly: AllowPartiallyTrustedCallers]`：允许部分信任调用（壳兼容性设置）
- `[assembly: AssemblyDelaySign(false)]` / `AssemblyKeyName("")` / `AssemblyKeyFile("")`：未签名

## IOC / 硬编码
- 程序集 GUID：`ca2000e9-458b-4ab6-989c-c47028ea8888`（编译期生成，非 IOC）
- `SuppressIldasm` 属性为加壳器反分析特征
