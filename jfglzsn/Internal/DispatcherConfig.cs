using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000003 RID: 3
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	// ============================================================
	// Internal/DispatcherConfig.cs — My.Computer 生成类（自动生成，无逻辑）
	// VB.NET My.Computer 的编译器生成包装类（继承 Microsoft.VisualBasic.Devices.Computer），被混淆器改名 DispatcherConfig。构造器空实现；CloneParser/InvokeParser 为混淆器加入的桩方法。
	// ============================================================
	internal class DispatcherConfig : Computer
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000021C9 File Offset: 0x000003C9
		[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 构造器（空实现，My.Computer 生成类）。
		/// </summary>
		public DispatcherConfig()
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>
		/// 桩方法：恒 true（混淆器加入）。
		/// </summary>
		internal static bool CloneParser()
		{
			return true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>
		/// 桩方法：恒 false（混淆器加入）。
		/// </summary>
		internal static bool InvokeParser()
		{
			return false;
		}
	}
}
