using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000003 RID: 3
	/// <summary>
	/// VB.NET 自动生成的 My.Computer（真实身份已确认：[HelpKeyword("My.Computer")] 指向它）。
	/// 混淆器只把类名改成了 Template，它本身没有任何逻辑，只是 Computer 的服务入口。
	/// 本程序没有用到 My.Computer 的任何成员。
	/// 原混淆名：Template
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	internal class MyComputer : Computer
	{
		// Token: 0x06000005 RID: 5
		/// <summary>空构造函数，等同于 Computer 的默认构造。</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MyComputer()
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>混淆器注入的占位方法：恒返回 true，无调用者。原混淆名：PatchParser</summary>
		internal static bool 混淆器占位方法_恒真()
		{
			return true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>混淆器注入的占位方法：恒返回 false，无调用者。原混淆名：PushParser</summary>
		internal static bool 混淆器占位方法_恒假()
		{
			return false;
		}
	}
}
