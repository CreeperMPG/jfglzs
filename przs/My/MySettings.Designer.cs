using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using gypZRX1vuWc3aeye0R.Internal;
using Microsoft.VisualBasic.CompilerServices;

// ============================================================================
// MySettings.Designer.cs — 应用设置自动生成类（My.Settings 单例）
// 真实职责：VS SettingsSingleFileGenerator（9.0.0.0）自动生成的 ApplicationSettingsBase 分部类：
// 提供线程安全单例 Default（SettingsBase.Synchronized），首次访问时订阅 My.Application.Shutdown 事件 →
// 退出时保存设置（SaveMySettingsOnExit 为真时调用 MySettingsProperty.Settings.Save()）。
// gypZRX1vuWc3aeye0R.Internal 的 Account.MyApplication 即 My.Application（混淆名）。
// 说明：自动生成文件，按规则仅加注释不重命名（仅同步了 Account.CollectBroadcaster → MyApplication 引用）。
// ============================================================================
namespace My
{
	// Token: 0x02000011 RID: 17
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal sealed partial class MySettings : ApplicationSettingsBase
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x00002309 File Offset: 0x00000509
		[EditorBrowsable(EditorBrowsableState.Advanced)]
/// <summary>退出时保存设置：My.Application.Shutdown 事件回调，SaveMySettingsOnExit 为真时保存。</summary>
		private static void AutoSaveSettings(object sender, EventArgs e)
		{
			if (Account.MyApplication.SaveMySettingsOnExit)
			{
				MySettingsProperty.Settings.Save();
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000074C8 File Offset: 0x000056C8
/// <summary>My.Settings 线程安全单例（首次访问订阅 Shutdown → 退出保存设置）。</summary>
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
							Account.MyApplication.Shutdown += MySettings.AutoSaveSettings;
							MySettings.addedHandler = true;
						}
					}
				}
				return MySettings.defaultInstance;
			}
		}

		// Token: 0x04000055 RID: 85
/// <summary>单例实例字段（SettingsBase.Synchronized 线程安全包装）。</summary>
		private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());

		// Token: 0x04000056 RID: 86
/// <summary>Shutdown 事件是否已订阅标志。</summary>
		private static bool addedHandler;

		// Token: 0x04000057 RID: 87
/// <summary>订阅锁对象（保证单次订阅）。</summary>
		private static object addedHandlerLockObject = RuntimeHelpers.GetObjectValue(new object());
	}
}
