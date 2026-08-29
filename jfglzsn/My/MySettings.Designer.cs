using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;
using WindowsApplication1.Internal;

namespace WindowsApplication1.My
{
	// Token: 0x02000011 RID: 17
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
	// ============================================================
	// My/MySettings.Designer.cs — My.Settings 设计器分部类（自动生成）
	// VB.NET My.Settings 设计器分部：Default 属性（线程安全懒加载单例）与 AutoSaveSettings（应用 Shutdown 时若 SaveMySettingsOnExit 则 Save()）。纯编译器生成代码。
	// ============================================================
	internal sealed partial class MySettings : ApplicationSettingsBase
	{
		// Token: 0x060000CD RID: 205 RVA: 0x00002306 File Offset: 0x00000506
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		/// <summary>
		/// My.Settings 自动保存：应用 Shutdown 时若 MyApplication.SaveMySettingsOnExit 为 true 则 Settings.Save()。
		/// </summary>
		private static void AutoSaveSettings(object sender, EventArgs e)
		{
			if (TokenizerConfig.MyApplication.SaveMySettingsOnExit)
			{
				MySettingsProperty.Settings.Save();
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00009C74 File Offset: 0x00007E74
		/// <summary>
		/// My.Settings.Default：线程安全懒加载单例（SettingsBase.Synchronized），首次访问把 AutoSaveSettings 挂到 MyApplication.Shutdown。
		/// </summary>
		public static MySettings Default
		{
			get
			{
				if (!MySettings.addedHandler)
				{
					object obj = MySettings.addedHandlerLockObject;
					ObjectFlowControl.CheckForSyncLockOnValueType(obj);
					lock (obj)
					{
						if (!MySettings.addedHandler)
						{
							TokenizerConfig.MyApplication.Shutdown += MySettings.AutoSaveSettings;
							MySettings.addedHandler = true;
						}
					}
				}
				return MySettings.defaultInstance;
			}
		}

		// Token: 0x040000AF RID: 175
		/// <summary>
		/// MySettings 单例实例。
		/// </summary>
		private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());

		// Token: 0x040000B0 RID: 176
		/// <summary>
		/// Shutdown 事件是否已挂接标志。
		/// </summary>
		private static bool addedHandler;

		// Token: 0x040000B1 RID: 177
		/// <summary>
		/// 挂接锁对象。
		/// </summary>
		private static object addedHandlerLockObject = RuntimeHelpers.GetObjectValue(new object());
	}
}
