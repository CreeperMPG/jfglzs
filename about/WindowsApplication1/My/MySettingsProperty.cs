using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.My
{
	// Token: 0x0200000D RID: 13
	/// <summary>
	/// VB.NET 生成的 My.Settings 全局访问器：暴露 Settings 属性返回 MySettings.Default。
	/// [HideModuleName] + [StandardModule] 标准生成形态，无任何手工逻辑。
	/// </summary>
	[DebuggerNonUserCode]
	[StandardModule]
	[CompilerGenerated]
	[HideModuleName]
	internal sealed class MySettingsProperty
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00004790 File Offset: 0x00002990
		/// <summary>My.Settings 访问器 → MySettings.Default 单例。</summary>
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
