using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.My
{
	// Token: 0x0200000F RID: 15
	// <summary>
	/// VB.NET 自动生成的 My.Settings 静态属性包装（MyTemplate）：
	/// Settings 直接返回 MySettings.Default。纯模板代码，无任何逻辑与 IOC。
	/// </summary>
	[CompilerGenerated]
	[DebuggerNonUserCode]
	[HideModuleName]
	[StandardModule]
	internal sealed class MySettingsProperty
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00005000 File Offset: 0x00003200
		/// <summary>My.Settings 访问器：返回 MySettings.Default 单例。</summary>
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
