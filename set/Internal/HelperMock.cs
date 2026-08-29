using System;
using System.Windows.Forms;

namespace WindowsApplication1.Internal
{
		/// <summary>
		/// IntelliLock 未注册版提示存根（壳特征）：首次调用弹 MessageBox "This assembly is protected by an unregistered version of \"IntelliLock\"!"；
		/// 弹窗失败则回退执行一段 HTML script 字符串（FallbackEmptyHandler 实际为空实现）。样本正式代码未见对其调用。
		/// </summary>
	// Token: 0x02000017 RID: 23
	internal class HelperMock
	{
		/// <summary>
		/// 一次性弹 IntelliLock 未注册提示（_noticeShown 标志保护）；MessageBox.Show 抛异常时回退调用 FallbackEmptyHandler(HTML script 字符串)。
		/// </summary>
		// Token: 0x06000125 RID: 293 RVA: 0x0000EA6C File Offset: 0x0000CC6C
		internal static void ShowTrialNotice_Once()
		{
			if (!HelperMock._noticeShown)
			{
				HelperMock._noticeShown = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					HelperMock.FallbackEmptyHandler("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		/// <summary>
		/// 空回退实现：参数是 <script>alert(...)</script> 字符串，函数体为空。
		/// </summary>
		// Token: 0x06000126 RID: 294 RVA: 0x00002479 File Offset: 0x00000679
		internal static void FallbackEmptyHandler(object htmlFallback)
		{
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 true，无实际逻辑。
		/// </summary>
		// Token: 0x06000128 RID: 296 RVA: 0x000021D1 File Offset: 0x000003D1
		internal static bool Placeholder_ReturnTrue()
		{
			return true;
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 false，无实际逻辑。
		/// </summary>
		// Token: 0x06000129 RID: 297 RVA: 0x000021C6 File Offset: 0x000003C6
		internal static bool Placeholder_ReturnFalse()
		{
			return false;
		}

		/// <summary>
		/// 是否已弹过试用提示标志。
		/// </summary>
		// Token: 0x04000085 RID: 133
		private static bool _noticeShown;
	}
}
