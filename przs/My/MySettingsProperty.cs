using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

// ============================================================================
// MySettingsProperty.cs — My.Settings 属性访问器（自动生成）
// 真实职责：VB.NET My.Settings 的标准访问模块（StandardModule + HideModuleName，自动生成）：
// Settings 属性直接返回 MySettings.Default 单例。全程序未发现业务代码使用设置项，属框架支撑。
// 说明：自动生成文件，按规则仅加注释不重命名。
// ============================================================================
namespace My
{
	// Token: 0x02000012 RID: 18
	[HideModuleName]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[StandardModule]
	internal sealed class MySettingsProperty
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00007530 File Offset: 0x00005730
		[HelpKeyword("My.Settings")]
/// <summary>返回 My.Settings 单例（MySettings.Default）。</summary>
		internal static MySettings Settings
		{
			get
			{
				return MySettings.Default;
			}
		}
	}
}
