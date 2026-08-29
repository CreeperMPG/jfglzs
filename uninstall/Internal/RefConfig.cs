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
	// Token: 0x0200000D RID: 13
	// <summary>
	/// VS 强类型资源生成器（StronglyTypedResourceBuilder）产物：为
	/// WindowsApplication1.Resources 资源提供 ResourceManager 单例与 Culture 属性。
	/// 本组件当前无资源属性被使用（无真实资源成员），仅框架桩。无恶意行为、无 IOC。
	/// </summary>
	[CompilerGenerated]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[StandardModule]
	[HideModuleName]
	internal sealed class RefConfig
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00004F44 File Offset: 0x00003144
		/// <summary>ResourceManager 单例：指向内嵌资源 WindowsApplication1.Resources（懒加载）。</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(RefConfig.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("WindowsApplication1.Resources", typeof(RefConfig).Assembly);
					RefConfig.resourceMan = resourceManager;
				}
				return RefConfig.resourceMan;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00004F84 File Offset: 0x00003184
		// (set) Token: 0x0600005F RID: 95 RVA: 0x0000224F File Offset: 0x0000044F
		/// <summary>资源区域性（CultureInfo）属性，模板生成。</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return RefConfig.resourceCulture;
			}
			set
			{
				RefConfig.resourceCulture = value;
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>恒 true 桩方法（混淆噪声），无实际逻辑。</summary>
		internal static bool RunRegistry()
		{
			return true;
		}

		// Token: 0x0400002C RID: 44
		/// <summary>ResourceManager 单例后端字段。</summary>
		private static ResourceManager resourceMan;

		// Token: 0x0400002D RID: 45
		/// <summary>资源区域性后端字段。</summary>
		private static object resourceCulture;
	}
}
