using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace zmserv.Internal
{
	// Token: 0x02000003 RID: 3
	/// <summary>
	/// VB 的 My.Computer（Microsoft.VisualBasic.Devices.Computer 的转发壳）。
	/// 真实职责：提供 My.Computer 对象，本身不实现任何逻辑——没有成员、没有初始化代码。
	/// 本工程里没有任何地方用到它的成员（文件/网络/时钟这些能力都没被调用）。
	/// 下面两个 Definition 方法是加壳器注入的空桩，不是业务代码。
	/// </summary>
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class MyComputer : Computer
	{
		// Token: 0x06000004 RID: 4 RVA: 0x000021C2 File Offset: 0x000003C2
		/// <summary>空构造（模板生成）。</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MyComputer()
		{
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>壳注入的空桩：固定返回 true，无调用点。</summary>
		internal static bool StartDefinition()
		{
			return true;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
		internal static bool WriteDefinition()
		{
			return false;
		}
	}
}
