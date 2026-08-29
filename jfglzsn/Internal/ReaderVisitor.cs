using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000010 RID: 16
	[HideModuleName]
	[CompilerGenerated]
	[DebuggerNonUserCode]
	[StandardModule]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	// ============================================================
	// Internal/ReaderVisitor.cs — 强类型资源访问器（Resources 生成类，自动生成）
	// VB.NET 编译器生成的强类型资源访问器，访问嵌入资源 WindowsApplication1.Resources。ResourceManager 懒加载；Culture 读写区域性。VisitMethod 恒 false 为混淆桩。
	// ============================================================
	internal sealed class ReaderVisitor
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00009C20 File Offset: 0x00007E20
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		/// <summary>
		/// 资源管理器（懒加载）：new ResourceManager("WindowsApplication1.Resources", ...)。
		/// </summary>
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(ReaderVisitor.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("WindowsApplication1.Resources", typeof(ReaderVisitor).Assembly);
					ReaderVisitor.resourceMan = resourceManager;
				}
				return ReaderVisitor.resourceMan;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00009C60 File Offset: 0x00007E60
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x000022D0 File Offset: 0x000004D0
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		/// <summary>
		/// 资源区域性 get/set。
		/// </summary>
		internal static CultureInfo Culture
		{
			get
			{
				return ReaderVisitor.resourceCulture;
			}
			set
			{
				ReaderVisitor.resourceCulture = value;
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>
		/// 桩：恒 false（混淆残留）。
		/// </summary>
		internal static bool VisitMethod()
		{
			return false;
		}

		// Token: 0x040000AD RID: 173
		/// <summary>
		/// ResourceManager 缓存字段。
		/// </summary>
		private static ResourceManager resourceMan;

		// Token: 0x040000AE RID: 174
		/// <summary>
		/// CultureInfo 缓存字段。
		/// </summary>
		private static object resourceCulture;
	}
}
