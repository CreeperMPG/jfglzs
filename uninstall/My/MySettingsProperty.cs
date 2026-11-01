using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.My
{
	// Token: 0x0200000A RID: 10
	/// <summary>
	/// VB.NET 自动生成的 My.Settings 访问模块（[StandardModule] + [HideModuleName]）。
	/// 它只暴露一个属性 Settings，转手把 MySettings.Default 单例返回出去。
	/// 本程序里没有任何地方读写设置项，所以它实际是空的：类存在，但值是默认值。
	/// </summary>
	[StandardModule]
	[CompilerGenerated]
	[DebuggerNonUserCode]
	[HideModuleName]
	internal sealed class MySettingsProperty
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002358 File Offset: 0x00000558
		/// <summary>
		/// 取 My.Settings 单例（等价于 VB 里的 My.Settings）。
		/// </summary>
		[HelpKeyword("My.Settings")]
		internal static MySettings Settings
		{
			get
			{
				return MySettings.Default;
			}
		}
	}
}
