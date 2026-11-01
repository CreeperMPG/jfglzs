using System;
using System.Windows.Forms;

namespace set.Internal
{
	/// <summary>
	/// IntelliLock 试用版「未注册」提醒类。加壳工具把这段提示代码留在了程序集里。
	/// 真实路径只有一条：首次调用 <see cref="提示未注册"/> 时弹一个 MessageBox，告诉用户
	/// 本程序集是「未注册版 IntelliLock 保护」的；如果弹窗本身抛异常（例如没有消息循环或
	/// 被拦截），就退回到 <see cref="输出脚本弹窗"/> —— 那是个空方法，等于什么都不做。
	/// 静态标志 <see cref="已提示"/> 保证只弹一次。
	/// </summary>
	// Token: 0x02000017 RID: 23
	internal class TokenAttr
	{
		/// <summary>
		/// 未注册提醒（只执行一次）。窗口标题是作者网站 www.eziriz.com。
		/// </summary>
		// Token: 0x06000124 RID: 292 RVA: 0x0000E404 File Offset: 0x0000C604
		internal static void 提示未注册()
		{
			if (!TokenAttr.已提示)
			{
				TokenAttr.已提示 = true;
				try
				{
					// 原文：This assembly is protected by an unregistered version of "IntelliLock"!
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					// 弹窗失败时的降级路径：交给一个空方法，等于静默忽略
					TokenAttr.输出脚本弹窗("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		/// <summary>
		/// 原版是给 WebBrowser 控件注入 JS 弹窗的辅助方法，但方法体已被抹成空的 —— 调用它不会产生任何效果。
		/// </summary>
		/// <param name="脚本">要注入的脚本内容（当前被忽略）。</param>
		// Token: 0x06000125 RID: 293 RVA: 0x000027FF File Offset: 0x000009FF
		internal static void 输出脚本弹窗(object 脚本)
		{
		}

		/// <summary>恒返回 true 的死代码桩，无调用点。</summary>
		// Token: 0x06000127 RID: 295 RVA: 0x000021E5 File Offset: 0x000003E5
		internal static bool 恒真桩()
		{
			return true;
		}

		/// <summary>恒返回 false 的死代码桩，无调用点。</summary>
		// Token: 0x06000128 RID: 296 RVA: 0x000021E8 File Offset: 0x000003E8
		internal static bool 恒假桩()
		{
			return false;
		}

		/// <summary>「已经提示过」标志，防止重复弹窗。</summary>
		// Token: 0x04000085 RID: 133
		private static bool 已提示;
	}
}
