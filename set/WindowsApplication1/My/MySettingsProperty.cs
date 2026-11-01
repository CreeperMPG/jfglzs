using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.My
{
	/// <summary>
	/// VB.NET「My.Settings」的自动生成访问入口（标准模板类，逻辑为编译器产物）。
	/// 唯一作用：把 <c>My.Settings</c> 映射到 <see cref="MySettings.Default"/> 单例。
	/// </summary>
	// Token: 0x0200000A RID: 10
	[HideModuleName]
	[DebuggerNonUserCode]
	[StandardModule]
	[CompilerGenerated]
	internal sealed class MySettingsProperty
	{
		/// <summary>返回应用程序设置的全局单例（My.Settings）。</summary>
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002371 File Offset: 0x00000571
		[HelpKeyword("My.Settings")]
		internal static MySettings 设置
		{
			get
			{
				return MySettings.Default;
			}
		}
	}
}
