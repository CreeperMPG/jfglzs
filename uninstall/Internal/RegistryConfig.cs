using System;
using System.Windows.Forms;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000016 RID: 22
	/// <summary>
	/// 保护器（IntelliLock）留下的「未注册版本」提醒类。
	/// 全工程没有任何地方调用它，逻辑本身也只是弹一个 MessageBox：
	/// 这是加壳工具试用版的广告/水印代码，不是「机房管理助手」的业务逻辑。
	/// 保留它是为了保持类型表一致（也提醒分析者：样本是用未注册的 IntelliLock 加壳的）。
	/// 原混淆名：RegistryConfig
	/// </summary>
	internal class 未注册许可证提示
	{
		// Token: 0x06000095 RID: 149 RVA: 0x00007BA8 File Offset: 0x00005DA8
		/// <summary>
		/// 弹一次 IntelliLock 未注册提示（用静态标志保证只弹一次）。
		/// 优先用 WinForms 的 MessageBox；若 MessageBox 本身不可用（无 GUI 环境会抛异常），
		/// 退化为把一段 JavaScript alert 交给 显示网页提示（而后者是空实现，等于什么都不做）。
		/// 原混淆名：smethod_0
		/// </summary>
		internal static void 显示未注册提示()
		{
			if (!未注册许可证提示.已提示过)
			{
				未注册许可证提示.已提示过 = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					未注册许可证提示.显示网页提示("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00002422 File Offset: 0x00000622
		/// <summary>
		/// 空实现：原本大概是往浏览器/WebBrowser 控件里塞提示脚本，编译产物里方法体是空的。
		/// 原混淆名：RateConfig
		/// </summary>
		internal static void 显示网页提示(object 提示脚本)
		{
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>混淆器注入的占位方法：恒返回 false，无调用者。原混淆名：ViewParser</summary>
		internal static bool 混淆器占位方法_恒假()
		{
			return false;
		}

		// Token: 0x04000041 RID: 65
		/// <summary>是否已经弹过提示。</summary>
		private static bool 已提示过;
	}
}
