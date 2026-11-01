using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace zmserv.My
{
	// Token: 0x02000009 RID: 9
	/// <summary>
	/// VB 的「我的设置」入口模块，唯一职责是把 My.Settings 暴露成
	/// 可以写成 My.Settings.XXX 的静态属性（这里只有 getter，无 setter）。
	/// 注意：这里没有任何真正的配置读写逻辑，配置项为空。
	/// </summary>
	[CompilerGenerated]
	[HideModuleName]
	[StandardModule]
	[DebuggerNonUserCode]
	internal sealed class MySettingsProperty
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000022B8 File Offset: 0x000004B8
		/// <summary>
		/// 对应 VB 里的 My.Settings，直接转发到 MySettings.Default 单例。
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
