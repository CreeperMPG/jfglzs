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
	/// VB.NET 模板生成的「My.Settings」访问入口（<c>[StandardModule]</c> 语义的静态类）。
	/// 业务代码统一通过 <see cref="Settings"/> 取单例，不直接 new。
	/// </summary>
	[CompilerGenerated]
	[StandardModule]
	[HideModuleName]
	[DebuggerNonUserCode]
	internal sealed class MySettingsProperty
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000036 RID: 54 RVA: 0x0000238A File Offset: 0x0000058A
		/// <summary>
		/// 返回 <see cref="MySettings.Default"/>（首次访问会顺带挂上退出时保存的钩子）。
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
