using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace about.Internal
{
	// Token: 0x0200000B RID: 11
	/// <summary>
	/// 强类型资源访问器（原名 ItemProduct，WindowsApplication1.Resources）：懒加载 ResourceManager("WindowsApplication1.Resources", ...)，
	/// 供 My.Resources 使用。资源本体在 Resources.resources / Form1.resources / Form3.resources（背景图、图标等）。无手工逻辑。
	/// </summary>
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[StandardModule]
	[HideModuleName]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal sealed class ItemProduct
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600006D RID: 109 RVA: 0x000046D4 File Offset: 0x000028D4
		/// <summary>资源管理器单例（基名 WindowsApplication1.Resources），懒加载缓存到 resourceMan。</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(ItemProduct.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("WindowsApplication1.Resources", typeof(ItemProduct).Assembly);
					ItemProduct.resourceMan = resourceManager;
				}
				return ItemProduct.resourceMan;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00004714 File Offset: 0x00002914
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00002286 File Offset: 0x00000486
		/// <summary>当前区域性（资源本地化用）。</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return ItemProduct.resourceCulture;
			}
			set
			{
				ItemProduct.resourceCulture = value;
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>混淆填充桩（原名 QueryListener），恒 true，无实义。</summary>
		internal static bool QueryListener()
		{
			return true;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000021C9 File Offset: 0x000003C9
		/// <summary>混淆填充桩（原名 ViewListener），恒 false，无实义。</summary>
		internal static bool ViewListener()
		{
			return false;
		}

		// Token: 0x04000023 RID: 35
		/// <summary>ResourceManager 缓存字段（生成代码标准命名）。</summary>
		private static ResourceManager resourceMan;

		// Token: 0x04000024 RID: 36
		/// <summary>区域性缓存字段（生成代码标准命名）。</summary>
		private static object resourceCulture;
	}
}
