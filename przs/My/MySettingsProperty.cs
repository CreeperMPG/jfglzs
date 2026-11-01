using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace My
{
	/// <summary>
	/// <c>My.Settings</c> 的入口（Designer 生成的样板），实际只是把
	/// <see cref="MySettings.Default"/> 转出来。
	/// </summary>
	// Token: 0x0200000A RID: 10
	[StandardModule]
	[CompilerGenerated]
	[HideModuleName]
	[DebuggerNonUserCode]
	internal sealed class MySettingsProperty
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000023A3 File Offset: 0x000005A3
		/// <summary>返回设置单例（原名 Settings）。</summary>
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
