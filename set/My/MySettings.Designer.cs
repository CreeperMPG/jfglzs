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
		/// VB 项目 My.Settings 设计器分部类（自动生成）：单例 Default + 应用退出时自动 Save()（AutoSaveSettings 订阅 My.Application.Shutdown）。
		/// </summary>
	// Token: 0x0200000F RID: 15
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[CompilerGenerated]
	internal sealed partial class MySettings : ApplicationSettingsBase
	{
		/// <summary>
		/// Shutdown 事件处理：Reader.MyApplication.SaveMySettingsOnExit 为真则 Settings.Save() 保存设置。
		/// </summary>
		// Token: 0x060000F2 RID: 242 RVA: 0x00002461 File Offset: 0x00000661
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		private static void AutoSaveSettings(object sender, EventArgs e)
		{
			if (Reader.MyApplication.SaveMySettingsOnExit)
			{
				MySettingsProperty.Settings.Save();
			}
		}

		/// <summary>
		/// 设置单例：线程安全惰性初始化（Synchronized），首次访问时订阅 Shutdown→AutoSaveSettings。
		/// </summary>
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x0000BD40 File Offset: 0x00009F40
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
							Reader.MyApplication.Shutdown += MySettings.AutoSaveSettings;
							MySettings.addedHandler = true;
						}
					}
				}
				return MySettings.defaultInstance;
			}
		}

		/// <summary>
		/// Default 单例后备字段。
		/// </summary>
		// Token: 0x04000072 RID: 114
		private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());

		/// <summary>
		/// 是否已订阅 Shutdown 事件标志。
		/// </summary>
		// Token: 0x04000073 RID: 115
		private static bool addedHandler;

		/// <summary>
		/// 订阅锁对象。
		/// </summary>
		// Token: 0x04000074 RID: 116
		private static object addedHandlerLockObject = RuntimeHelpers.GetObjectValue(new object());
	}
}
