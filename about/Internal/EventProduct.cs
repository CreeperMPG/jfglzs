using System;
using System.Windows.Forms;

namespace about.Internal
{
	// Token: 0x02000014 RID: 20
	/// <summary>
	/// IntelliLock（Eziriz）未注册版提示模块（原名 EventProduct）：程序加载时弹窗警告「此程序集受未注册版 IntelliLock 保护」。
	/// 属于壳的广告/授权水印，无业务逻辑；提示文本确认了加壳工具为 Eziriz IntelliLock / .NET Reactor 未注册版。
	/// </summary>
	internal class EventProduct
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x00007350 File Offset: 0x00005550
		/// <summary>
		/// 显示未注册提示（原名 smethod_0，一次性）：watermarkShown 防重复；
		/// 优先 MessageBox.Show，catch 后走 ShowScriptFallbackWatermark（Web 宿主场景的 script alert）。
		/// </summary>
		internal static void ShowUnregisteredWatermark()
		{
			if (!EventProduct.watermarkShown)
			{
				EventProduct.watermarkShown = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					EventProduct.ShowScriptFallbackWatermark("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000222E File Offset: 0x0000042E
		/// <summary>备用脚本提示通道（原名 PublishProduct）：空实现（Web 场景用）。</summary>
		internal static void ShowScriptFallbackWatermark(object object_0)
		{
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000021C9 File Offset: 0x000003C9
		/// <summary>混淆填充桩（原名 StopListener），恒 false，无实义。</summary>
		internal static bool StopListener()
		{
			return false;
		}

		// Token: 0x04000038 RID: 56
		/// <summary>是否已提示过未注册水印（原名 pageProduct）。</summary>
		private static bool watermarkShown;
	}
}
