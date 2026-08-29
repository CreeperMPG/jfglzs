using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;
using WindowsApplication1.Internal;

namespace WindowsApplication1.My
{
	// Token: 0x0200000E RID: 14
	// <summary>
	/// VB 设置设计器生成的 MySettings 类（继承 ApplicationSettingsBase）：提供 Default 单例，
	/// 并在应用关闭（Shutdown）时若 SaveMySettingsOnExit 为真则自动保存设置。
	/// 自动生成文件，无手工恶意逻辑；本组件未定义任何自定义设置项，无 IOC。
	/// </summary>
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[CompilerGenerated]
	internal sealed partial class MySettings : ApplicationSettingsBase
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00002285 File Offset: 0x00000485
		/// <summary>
		/// 应用关闭时的自动保存处理器：My.Application.SaveMySettingsOnExit 为真时
		/// 调用 MySettingsProperty.Settings.Save() 保存设置。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		private static void AutoSaveSettings(object sender, EventArgs e)
		{
			if (VisitorConfig.Application.SaveMySettingsOnExit)
			{
				MySettingsProperty.Settings.Save();
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00004F98 File Offset: 0x00003198
		/// <summary>
		/// MySettings 默认单例：首次访问时线程安全地挂接 My.Application.Shutdown 事件
		/// （AutoSaveSettings）后返回 defaultInstance。
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
							VisitorConfig.Application.Shutdown += MySettings.AutoSaveSettings;
							MySettings.addedHandler = true;
						}
					}
				}
				return MySettings.defaultInstance;
			}
		}

		// Token: 0x0400002E RID: 46
		/// <summary>默认单例实例（SettingsBase.Synchronized 线程安全包装）。</summary>
		private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());

		// Token: 0x0400002F RID: 47
		/// <summary>Shutdown 事件是否已挂接标志。</summary>
		private static bool addedHandler;

		// Token: 0x04000030 RID: 48
		/// <summary>挂接 Shutdown 事件用的锁对象。</summary>
		private static object addedHandlerLockObject = RuntimeHelpers.GetObjectValue(new object());
	}
}
