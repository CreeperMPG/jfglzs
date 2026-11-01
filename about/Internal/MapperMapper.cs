using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace WindowsApplication1.Internal
{
	/// <summary>
	/// VB.NET 「我的电脑」（`My.Computer`）的实现类，继承自 <see cref="Computer"/>。
	/// 本工程没有往 `My.Computer` 上加任何自定义成员，所以这里只有空构造函数和两个桩方法；
	/// 业务代码里对 `My.Computer` 的访问都走 <see cref="ExpressionMapper.SetComposer"/>。
	/// </summary>
	// Token: 0x02000003 RID: 3
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	internal class MapperMapper : Computer
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000021E0 File Offset: 0x000003E0
		/// <summary>空构造函数，仅用于让 My.Computer 的单例能 new 出来。</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MapperMapper()
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>恒返回 true 的桩方法，无调用点。</summary>
		internal static bool 返回真占位()
		{
			return true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>恒返回 false 的桩方法，无调用点。</summary>
		internal static bool 返回假占位()
		{
			return false;
		}
	}
}
