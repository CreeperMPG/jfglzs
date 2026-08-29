using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace zmserv.My
{
	// Token: 0x02000008 RID: 8
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[CompilerGenerated]
	/// <summary>
	/// MySettings（Designer 部分）—— 由 MySettings.settings 自动生成：定义静态线程安全单例 Default。无任何业务逻辑。
	/// </summary>
	internal sealed partial class MySettings : ApplicationSettingsBase
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000025A4 File Offset: 0x000007A4
		/// <summary>设置单例：返回 defaultInstance（SettingsBase.Synchronized 线程安全单例）。</summary>
		public static MySettings Default
		{
			get
			{
				return MySettings.defaultInstance;
			}
		}

		// Token: 0x04000008 RID: 8
		/// <summary>单例字段：Synchronized 包装的 MySettings 实例。</summary>
		private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());
	}
}
