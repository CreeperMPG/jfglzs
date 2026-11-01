using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.My
{
	/// <summary>
	/// VB.NET 「我的设置」的门面：把内部的 <see cref="MySettings.Default"/> 以
	/// `My.Settings` 的形式暴露给业务代码（唯一的消费点是 MySettings.Designer.cs 里的退出自动保存）。
	/// 没有任何自定义设置项，因此它只是把 VB 模板代码原样带过来了。
	/// </summary>
	// Token: 0x0200000A RID: 10
	[StandardModule]
	[HideModuleName]
	[CompilerGenerated]
	[DebuggerNonUserCode]
	internal sealed class MySettingsProperty
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002358 File Offset: 0x00000558
		/// <summary>
		/// 取设置单例。直接转发到 <see cref="MySettings.Default"/>，
		/// 不做任何检查（单例由 ApplicationSettingsBase.Synchronized 保证线程安全）。
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
