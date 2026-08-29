using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using about.Internal;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.My
{
	// Token: 0x0200000C RID: 12
	/// <summary>
	/// My.Settings 设计器生成代码：SettingsBase.Synchronized 创建线程安全单例 defaultInstance；
	/// Default 属性首次访问时把 AutoSaveSettings 挂到 My.Application.Shutdown 事件（即 WrapperProduct.MyApplication），
	/// 退出时若 SaveMySettingsOnExit 为真则保存设置。本程序未定义任何自定义设置项。纯框架代码。
	/// </summary>
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal sealed partial class MySettings : ApplicationSettingsBase
	{
		// Token: 0x06000074 RID: 116 RVA: 0x000022BC File Offset: 0x000004BC
		/// <summary>应用关闭时保存设置（挂 My.Application.Shutdown 事件）：SaveMySettingsOnExit 为真时调用 Settings.Save()。</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		private static void AutoSaveSettings(object sender, EventArgs e)
		{
			if (WrapperProduct.MyApplication.SaveMySettingsOnExit)
			{
				MySettingsProperty.Settings.Save();
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00004728 File Offset: 0x00002928
		/// <summary>设置单例访问器：线程安全惰性初始化（double-checked lock），首次访问注册 AutoSaveSettings。</summary>
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
							WrapperProduct.MyApplication.Shutdown += MySettings.AutoSaveSettings;
							MySettings.addedHandler = true;
						}
					}
				}
				return MySettings.defaultInstance;
			}
		}

		// Token: 0x04000025 RID: 37
		/// <summary>单例缓存实例（SettingsBase.Synchronized 线程安全包装）。</summary>
		private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());

		// Token: 0x04000026 RID: 38
		/// <summary>Shutdown 事件是否已注册标志。</summary>
		private static bool addedHandler;

		// Token: 0x04000027 RID: 39
		/// <summary>注册锁对象。</summary>
		private static object addedHandlerLockObject = RuntimeHelpers.GetObjectValue(new object());
	}
}
