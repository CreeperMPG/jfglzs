using System;
using System.Windows.Forms;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000016 RID: 22
	// <summary>
	/// IntelliLock（Eziriz）授权运行时注入的提示模块：首次调用时弹窗显示
	/// "This assembly is protected by an unregistered version of \"IntelliLock\"!"
	/// （标题 www.eziriz.com），属加壳/授权工具的试用版水印，与业务逻辑无关。
	/// _warningShown 为是否已弹窗标志。
	/// </summary>
	internal class ContextConfig
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00007C8C File Offset: 0x00005E8C
		/// <summary>
		/// 显示未注册提示（原混淆名 smethod_0）：只弹一次——
		/// 若未弹过则置 _warningShown=true 并 MessageBox.Show 提示；
		/// 弹窗失败则调用 LegacyWebAlertFallback（空实现）。
		/// </summary>
		internal static void ShowUnregisteredWarning()
		{
			if (!ContextConfig._warningShown)
			{
				ContextConfig._warningShown = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					ContextConfig.LegacyWebAlertFallback("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000229D File Offset: 0x0000049D
		/// <summary>
		/// 备用弹窗路径（原混淆名 ManageConfig）：空实现——
		/// 原为 Web 场景的 JS alert 提示，已废弃（反编译仅剩异常处理骨架）。
		/// </summary>
		internal static void LegacyWebAlertFallback(object object_0)
		{
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>恒 true 混淆桩，无实际逻辑。</summary>
		internal static bool ListRegistry()
		{
			return true;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>恒 false 混淆桩，无实际逻辑。</summary>
		internal static bool InitRegistry()
		{
			return false;
		}

		// Token: 0x04000041 RID: 65
		/// <summary>是否已弹出 IntelliLock 未注册提示的标志（只弹一次）。</summary>
		private static bool _warningShown;
	}
}
