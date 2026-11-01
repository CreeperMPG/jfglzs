using System;
using System.Windows.Forms;

namespace zmserv.Internal
{
	// Token: 0x02000011 RID: 17
	/// <summary>
	/// IntelliLock（EZIRIZ）加壳工具的「未注册版本」提示代码。
	/// 真实职责：被调用时弹一次 MessageBox，内容是
	/// 「This assembly is protected by an unregistered version of "IntelliLock!"」、
	/// 标题 "www.eziriz.com"；弹窗本身抛异常（服务/无桌面环境）时退化成写一段 JS alert 脚本。
	/// 本工程内没有任何调用点——zmserv 是服务进程，没有交互桌面，这段代码实际不会执行。
	/// 保留它的价值只有一个：能确认样本确实套过 IntelliLock 的试用版。
	/// </summary>
	internal class IntelliLock提示
	{
		// Token: 0x0600005F RID: 95 RVA: 0x00005804 File Offset: 0x00003A04
		/// <summary>
		/// 弹一次未授权提示（靠 已提示过 标志保证只弹一次）。
		/// 弹窗抛异常时进入 catch，改走 ResolveParams 的脚本分支——catch 不能删。
		/// </summary>
		internal static void 显示未授权提示()
		{
			if (!IntelliLock提示.已提示过)
			{
				IntelliLock提示.已提示过 = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					IntelliLock提示.ResolveParams("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000022EC File Offset: 0x000004EC
		/// <summary>
		/// 空方法体：被当作"没有 GUI 时的退路"调用，但里面什么都没做（壳的占位实现）。
		/// 参数是上一步拼好的 JS alert 脚本内容，接收后即丢弃。
		/// </summary>
		internal static void ResolveParams(object 脚本内容)
		{
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
		internal static bool VerifyDefinition()
		{
			return false;
		}

		// Token: 0x04000026 RID: 38
		/// <summary>是否已经弹过提示（保证只弹一次）。原名 m_DatabaseStrategy。</summary>
		private static bool 已提示过;
	}
}
