using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.My
{
	// Token: 0x02000012 RID: 18
	[DebuggerNonUserCode]
	[HideModuleName]
	[StandardModule]
	[CompilerGenerated]
	// ============================================================
	// My/MySettingsProperty.cs — My.Settings 访问属性（自动生成）
	// VB.NET My.Settings 的标准模块访问器：Settings 属性直接返回 MySettings.Default。
	// ============================================================
	internal sealed class MySettingsProperty
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00009CDC File Offset: 0x00007EDC
		[HelpKeyword("My.Settings")]
		/// <summary>
		/// My.Settings 访问属性：直接返回 MySettings.Default。
		/// </summary>
		internal static MySettings Settings
		{
			get
			{
				return MySettings.Default;
			}
		}
	}
}
