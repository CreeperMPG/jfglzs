using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace set.Internal
{
	/// <summary>
	/// VB.NET「My.Computer」的自动生成包装类（原模板类名，混淆器未改动其语义）。
	/// 只继承 <see cref="Computer"/> 暴露键盘/鼠标/剪贴板/注册表等入口，自身没有任何自定义逻辑。
	/// 两个静态方法是加壳工具插入的恒值死代码桩，无调用点。
	/// </summary>
	// Token: 0x02000003 RID: 3
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class AttrAttr : Computer
	{
		/// <summary>My.Computer 的单例构造，无额外初始化。</summary>
		// Token: 0x06000004 RID: 4 RVA: 0x000021DD File Offset: 0x000003DD
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AttrAttr()
		{
		}

		/// <summary>恒返回 true 的死代码桩，无调用点。</summary>
		// Token: 0x06000005 RID: 5 RVA: 0x000021E5 File Offset: 0x000003E5
		internal static bool 恒真桩()
		{
			return true;
		}

		/// <summary>恒返回 false 的死代码桩，无调用点。</summary>
		// Token: 0x06000006 RID: 6 RVA: 0x000021E8 File Offset: 0x000003E8
		internal static bool 恒假桩()
		{
			return false;
		}
	}
}
