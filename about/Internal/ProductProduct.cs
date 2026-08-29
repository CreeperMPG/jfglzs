using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace about.Internal
{
	// Token: 0x02000003 RID: 3
	/// <summary>
	/// VB.NET 编译器生成的 My.Computer 访问器（原名 ProductProduct）：继承 Microsoft.VisualBasic.Devices.Computer，
	/// 无任何自定义逻辑。ResolveListener/ComputeListener 为混淆器添加的恒值桩。
	/// </summary>
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class ProductProduct : Computer
	{
		// Token: 0x06000006 RID: 6 RVA: 0x000021CC File Offset: 0x000003CC
		/// <summary>构造函数（生成代码）。</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ProductProduct()
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>混淆填充桩（原名 ResolveListener），恒 true，无实义。</summary>
		internal static bool ResolveListener()
		{
			return true;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021C9 File Offset: 0x000003C9
		/// <summary>混淆填充桩（原名 ComputeListener），恒 false，无实义。</summary>
		internal static bool ComputeListener()
		{
			return false;
		}
	}
}
