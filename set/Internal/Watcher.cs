using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace WindowsApplication1.Internal
{
		/// <summary>
		/// My.Computer 门面（Microsoft.VisualBasic.Devices.Computer 子类）：供程序访问 Registry/FileSystem/Network 等系统对象，无自定义逻辑。
		/// Form1 中大量 Reader.MyComputer.Registry.LocalMachine、Reader.MyComputer.FileSystem.GetFiles(...) 经它转发。
		/// </summary>
	// Token: 0x02000003 RID: 3
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	internal class Watcher : Computer
	{
		/// <summary>
		/// 构造（空）。
		/// </summary>
		// Token: 0x06000005 RID: 5 RVA: 0x000021C9 File Offset: 0x000003C9
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Watcher()
		{
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 true，无实际逻辑。
		/// </summary>
		// Token: 0x06000006 RID: 6 RVA: 0x000021D1 File Offset: 0x000003D1
		internal static bool Placeholder_ReturnTrue()
		{
			return true;
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 false，无实际逻辑。
		/// </summary>
		// Token: 0x06000007 RID: 7 RVA: 0x000021C6 File Offset: 0x000003C6
		internal static bool Placeholder_ReturnFalse()
		{
			return false;
		}
	}
}
