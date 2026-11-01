using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;
using WindowsApplication1.Internal;

namespace WindowsApplication1.My
{
	/// <summary>
	/// VB.NET 的「我的设置」（My.Settings）宿主类，实际存储由 ApplicationSettingsBase 负责，
	/// 真正的实现（默认实例、退出自动保存）在同名的 MySettings.Designer.cs 里。
	/// 本文件只剩两个常量返回的桩方法，业务代码不会调用它们。
	/// </summary>
	// Token: 0x02000009 RID: 9
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal sealed partial class MySettings : ApplicationSettingsBase
	{
		// Token: 0x06000030 RID: 48 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>恒返回 true 的桩方法，无参数无副作用。</summary>
		internal static bool 返回真占位()
		{
			return true;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>恒返回 false 的桩方法，无参数无副作用。</summary>
		internal static bool 返回假占位()
		{
			return false;
		}
	}
}
