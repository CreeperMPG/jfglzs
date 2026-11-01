using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace OZxTKXuvsl9D34LWhP.Internal
{
	/// <summary>
	/// VB 的 <c>My.Computer</c>（类名 Watcher 是混淆名，原类型名应为 <c>MyComputer</c>）。
	/// 继承 <see cref="Computer"/>，只额外提供 <c>Info</c>/<c>Name</c> 这些现成属性。
	/// 本项目里唯一实际用到的地方是 <c>bianhao</c> 窗体：<c>Queue.MyComputer.Name</c> 取计算机名。
	/// 两个 <c>XxxTag()</c> 是 .NET Reactor 生成的授权标记桩。
	/// </summary>
	// Token: 0x02000003 RID: 3
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class Watcher : Computer
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000021E0
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Watcher()
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021DD
		/// <summary>.NET Reactor 生成的授权标记桩，恒返回 true，无调用者（原名 MapTag）。</summary>
		internal static bool MapTag()
		{
			return true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021E8
		/// <summary>.NET Reactor 生成的授权标记桩，恒返回 false，无调用者（原名 PatchTag）。</summary>
		internal static bool PatchTag()
		{
			return false;
		}
	}
}
