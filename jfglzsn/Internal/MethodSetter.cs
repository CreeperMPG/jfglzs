using System;
using System.Windows.Forms;

namespace jfglzsn.Internal
{
	// Token: 0x02000019 RID: 25
	/// <summary>
	/// IntelliLock 授权提示的后门模块。它本身不做任何校验，只负责「弹窗骂人」式的未注册提示。
	/// <para>
	/// 关键点：这里的提示文案说明样本**曾经**被 IntelliLock 保护（AGENTS.md 记录的壳是 .NET Reactor，
	/// 说明这份 exe 是「IntelliLock 保护 + 后续再处理」或保护工具混用的产物）。
	/// 真正的授权判定已被抹成常量（见 ILLicenseModule / ForgotRule / DefineRule），
	/// 因此这段提示在当前样本里不会阻挡任何功能——它只是壳的残渣。
	/// </para>
	/// <para>
	/// <see cref="显示未注册提示"/> 是公开给壳运行时调用的入口（工程内无显式调用点），
	/// 由 <c>m_提示已弹出</c> 保证一次进程只弹一次。
	/// </para>
	/// </summary>
	internal class MethodSetter
	{
		// Token: 0x06000101 RID: 257 RVA: 0x0000C354 File Offset: 0x0000A554
		/// <summary>
		/// 弹出「本程序集被未注册版 IntelliLock 保护」的提示框；失败时退回到往文档里注入一段
		/// JavaScript alert（原版用于 Web 宿主场景）。
		/// <para>
		/// 语义要点：整个弹出动作被一个 try/catch 包住，MessageBox 抛异常时走 catch 分支的
		/// <see cref="FlushEvent"/>；标志位在**进入 try 之前**就已置真，所以即使弹窗失败也不会重试。
		/// 这与原版逐句忽略错误的写法一致，勿把 try 提到标志位之前。
		/// </para>
		/// </summary>
		internal static void 显示未注册提示()
		{
			if (!MethodSetter.m_提示已弹出)
			{
				MethodSetter.m_提示已弹出 = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					// 弹窗失败（无桌面/被拦截）时的降级路径：交给 FlushEvent（本样本里是空实现）
					MethodSetter.FlushEvent("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000025B6 File Offset: 0x000007B6
		/// <summary>
		/// 提示降级桩：原版把传入的 HTML/脚本片段显示给 Web 宿主，本样本中被剥成空方法体，
		/// 因此上面 catch 分支里的脚本字符串永远不会被展示。方法名沿用壳自带命名。
		/// </summary>
		/// <param name="提示源">要展示的脚本/HTML 片段，本样本未使用。</param>
		internal static void FlushEvent(object 提示源)
		{
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>IntelliLock 授权判定桩，恒返回 <c>true</c>（视为已注册），工程内无引用。</summary>
		internal static bool ForgotRule()
		{
			return true;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000021E0 File Offset: 0x000003E0
		/// <summary>IntelliLock 授权判定桩，恒返回 <c>false</c>（视为无限制），工程内无引用。</summary>
		internal static bool DefineRule()
		{
			return false;
		}

		// Token: 0x040000C2 RID: 194
		/// <summary>未注册提示是否已弹出过（保证一进程只弹一次）。</summary>
		private static bool m_提示已弹出;
	}
}
