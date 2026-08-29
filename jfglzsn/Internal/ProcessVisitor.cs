using System;
using System.Windows.Forms;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000019 RID: 25
	// ============================================================
	// Internal/ProcessVisitor.cs — 壳未注册提示（IntelliLock 试用版弹窗）
	// .NET Reactor 附属产品 IntelliLock 的"未注册版本"提示逻辑：MessageBox 提示"This assembly is protected by an unregistered version of IntelliLock!"，网址 www.eziriz.com（Reactor 厂商）。smethod_0 用静态标志保证只提示一次。无恶意行为。
	// ============================================================
	internal class ProcessVisitor
	{
		// Token: 0x060000FF RID: 255 RVA: 0x0000C83C File Offset: 0x0000AA3C
		/// <summary>
		/// 显示未注册提示（原名 smethod_0）：一次性（noticeShownFlag 标志）；MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com")；失败走 LoginVisitor（空实现）。
		/// 说明：IntelliLock 试用版弹窗，非恶意 C2。
		/// </summary>
		internal static void ShowUnregisteredProtectionNotice()
		{
			if (!ProcessVisitor.noticeShownFlag)
			{
				ProcessVisitor.noticeShownFlag = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					ProcessVisitor.LoginVisitor("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000231E File Offset: 0x0000051E
		/// <summary>
		/// 提示备用路径（空实现）：接收 HTML script 字符串但函数体为空。
		/// </summary>
		internal static void LoginVisitor(object object_0)
		{
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>
		/// 壳桩：恒 true。
		/// </summary>
		internal static bool DisableMethod()
		{
			return true;
		}

		// Token: 0x040000C2 RID: 194
		/// <summary>
		/// 是否已提示标志（原名 broadcasterVisitor）：静态 bool。
		/// </summary>
		private static bool noticeShownFlag;
	}
}
