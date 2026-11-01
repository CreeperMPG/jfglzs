using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace set.Internal
{
	/// <summary>
	/// 「WindowsApplication1.Resources」的强类型资源访问类（VB 资源设计器模板生成，逻辑未变）。
	/// 只负责提供 <see cref="ResourceManager"/> 单例与当前区域性 <see cref="Culture"/>，
	/// 本样本没有定义任何资源字符串，所以这两个成员实际上从不被取用。
	/// </summary>
	// Token: 0x02000008 RID: 8
	[HideModuleName]
	[CompilerGenerated]
	[StandardModule]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	internal sealed class AdapterAttr
	{
		/// <summary>资源管理器单例（资源基名 <c>WindowsApplication1.Resources</c>），首次访问时惰性创建。</summary>
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000022EB File Offset: 0x000004EB
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(AdapterAttr.resourceMan, null))
				{
					AdapterAttr.resourceMan = new ResourceManager("WindowsApplication1.Resources", typeof(AdapterAttr).Assembly);
				}
				return AdapterAttr.resourceMan;
			}
		}

		/// <summary>资源查找使用的区域性（模板留的开关，本样本设为 null 即使用当前区域性）。</summary>
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000029 RID: 41 RVA: 0x0000231D File Offset: 0x0000051D
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002324 File Offset: 0x00000524
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return AdapterAttr.resourceCulture;
			}
			set
			{
				AdapterAttr.resourceCulture = value;
			}
		}

		/// <summary>恒返回 true 的死代码桩，无调用点。</summary>
		// Token: 0x0600002B RID: 43 RVA: 0x000021E5 File Offset: 0x000003E5
		internal static bool 恒真桩()
		{
			return true;
		}

		/// <summary>恒返回 false 的死代码桩，无调用点。</summary>
		// Token: 0x0600002C RID: 44 RVA: 0x000021E8 File Offset: 0x000003E8
		internal static bool 恒假桩()
		{
			return false;
		}

		// Token: 0x0400000B RID: 11
		private static ResourceManager resourceMan;

		// Token: 0x0400000C RID: 12
		private static object resourceCulture;
	}
}
