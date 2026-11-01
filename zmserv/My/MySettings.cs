using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace zmserv.My
{
	// Token: 0x02000008 RID: 8
	/// <summary>
	/// My.Settings 的宿主类型（VB 的「我的设置」模板生成，ApplicationSettingsBase 派生）。
	/// 与 MySettings.Designer.cs 是同一个 partial 类的另一半：这里放两个壳注入的空桩，
	/// Designer 那边放真正的 Default 单例。本工程的 MySettings.settings 里一条设置都没有，
	/// 所以这个类实际不承载任何配置项。
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
	internal sealed partial class MySettings : ApplicationSettingsBase
	{
		// Token: 0x06000022 RID: 34 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>
		/// 壳注入的空桩：固定返回 true，无副作用、无调用点（不是"删除设置"的实现）。
		/// </summary>
		internal static bool DeleteDefinition()
		{
			return true;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>
		/// 壳注入的空桩：固定返回 false，无副作用、无调用点。
		/// </summary>
		internal static bool SetDefinition()
		{
			return false;
		}
	}
}
