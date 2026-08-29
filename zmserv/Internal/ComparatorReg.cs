using System;
using System.Windows.Forms;

namespace zmserv.Internal
{
	// Token: 0x02000011 RID: 17
	/// <summary>
	/// ComparatorReg —— IntelliLock 未注册许可提示（加壳器残留）：首次调用弹 MessageBox 声明程序受
	/// "IntelliLock" 保护并指向 www.eziriz.com，证明该样本曾用未注册的 .NET Reactor/IntelliLock 加壳。无业务恶意逻辑。
	/// </summary>
	internal class ComparatorReg
	{
		// Token: 0x0600005F RID: 95 RVA: 0x000054D4 File Offset: 0x000036D4
		/// <summary>
		/// 显示未注册提示（一次性）：noticeShown 为 false 时置 true 并 MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com")；
		/// MessageBox 抛异常时走 FallbackNotice（空实现）兜底。
		/// </summary>
		internal static void ShowUnregisteredNotice()
		{
			if (!ComparatorReg.noticeShown)
			{
				ComparatorReg.noticeShown = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					ComparatorReg.FallbackNotice("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002211 File Offset: 0x00000411
		/// <summary>兜底提示方法：空实现（异常分支残留）。</summary>
		internal static void FallbackNotice(object object_0)
		{
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>混淆填充：恒真，无业务意义（无调用方）。</summary>
		internal static bool ConfuserFillerTrue()
		{
			return true;
		}

		// Token: 0x04000026 RID: 38
		/// <summary>提示已显示标志：保证只弹一次。</summary>
		private static bool noticeShown;
	}
}
