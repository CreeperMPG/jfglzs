using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.ApplicationServices;

namespace zmserv.Internal
{
	// Token: 0x02000002 RID: 2
	/// <summary>
	/// VB 的 My.Application（ConsoleApplicationBase 的转发壳）。
	/// 真实职责：仅提供 My.Application 对象；因为本工程是 WinExe 但启动对象是服务类，
	/// 模板退化成了控制台基类。工程里没有任何地方使用它的成员。
	/// 两个 Definition 方法是加壳器注入的空桩。
	/// </summary>
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class MyApplication : ConsoleApplicationBase
	{
		// Token: 0x06000002 RID: 2 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>壳注入的空桩：固定返回 true，无调用点。</summary>
		internal static bool RevertFacade()
		{
			return true;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
		internal static bool SearchDefinition()
		{
			return false;
		}
	}
}
