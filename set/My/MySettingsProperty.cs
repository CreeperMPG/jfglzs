using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.My
{
		/// <summary>
		/// My.Settings 访问门面（自动生成）：Settings 属性返回 MySettings.Default。
		/// </summary>
	// Token: 0x02000010 RID: 16
	[DebuggerNonUserCode]
	[HideModuleName]
	[StandardModule]
	[CompilerGenerated]
	internal sealed class MySettingsProperty
	{
		/// <summary>
		/// My.Settings 实例：返回 MySettings.Default。
		/// </summary>
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x0000BDA8 File Offset: 0x00009FA8
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
