using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// 程序集级特性（dnSpy 从 Windows Metadata 还原，内容原样保留）。
// 关键信息：
//   AssemblyTitle / Product = "service"，Company = "Microsoft"、Copyright = "Microsoft 2025"
//     —— 这是伪装成微软系统服务的常见手法，实际作者与此无关；
//   SuppressIldasm —— 禁止 ILDasm 反汇编，配合加壳一起阻碍分析；
//   Guid 73204acb-20a4-433c-93f1-cd274c4ed922 是本次编译产物独有的标识。
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyConfiguration("")]
[assembly: Guid("73204acb-20a4-433c-93f1-cd274c4ed922")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: SuppressIldasm]
[assembly: AssemblyKeyName("")]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: ComVisible(false)]
[assembly: AssemblyTitle("service")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2025")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("service")]
