using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// 组装信息全部被加壳器清空（标题/公司/描述/版权都是空串），只剩一个随机 GUID。
// 文件名 uninstall、程序集名 uninstall，但对外不暴露任何标识，是刻意抹掉溯源的写法。
// 注意 SuppressIldasm：加壳器用它阻止 ildasm 直接反汇编，是「防静态分析」标记。

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyConfiguration("")]
[assembly: Guid("26148b78-26a7-4e21-aa4f-2f2b74167088")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyName("")]
[assembly: AssemblyKeyFile("")]
[assembly: SuppressIldasm]
[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
