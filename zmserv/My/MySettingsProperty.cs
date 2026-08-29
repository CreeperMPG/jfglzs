using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace zmserv.My
{
	// Token: 0x02000009 RID: 9
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[HideModuleName]
	[StandardModule]
	/// <summary>
	/// MySettingsProperty —— VB.NET 标准隐藏模块：暴露 Settings 属性（返回 MySettings.Default），
	/// 供 My.Settings 语法使用。无任何业务逻辑（自动生成）。
	/// </summary>
	internal sealed class MySettingsProperty
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000025B8 File Offset: 0x000007B8
		[HelpKeyword("My.Settings")]
		/// <summary>My.Settings 门面：返回 MySettings.Default。</summary>
		internal static MySettings Settings
		{
			get
			{
				return MySettings.Default;
			}
		}
	}
}
