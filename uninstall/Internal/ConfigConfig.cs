using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000003 RID: 3
	// <summary>
	/// VB.NET MyTemplate 自动生成的 My.Computer 访问器：继承 Microsoft.VisualBasic.Devices.Computer，
	/// 暴露计算机信息/剪贴板/网络等能力。业务代码通过 VisitorConfig.Computer 获取
	/// （本组件仅用其 Registry 属性间接访问注册表根键，见 RegConfig）。无恶意行为、无 IOC。
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	internal class ConfigConfig : Computer
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000021C9 File Offset: 0x000003C9
		/// <summary>空构造函数。</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ConfigConfig()
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>恒 true 混淆桩，无实际逻辑。</summary>
		internal static bool LogoutRegistry()
		{
			return true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>恒 false 混淆桩，无实际逻辑。</summary>
		internal static bool CallRegistry()
		{
			return false;
		}
	}
}
