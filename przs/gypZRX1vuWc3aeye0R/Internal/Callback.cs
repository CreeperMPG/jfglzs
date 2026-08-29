using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

// ============================================================================
// Callback.cs — 强类型资源访问器（Resources 资源文件读取）
// 真实职责：反混淆后为强类型资源类（StronglyTypedResourceBuilder 生成，ConfuserEx 改名 Callback），
// 持有指向程序集内 "Resources" 资源文件的 ResourceManager，供 My.Resources 体系读取嵌入资源（图标等）。
// 本项目业务代码未直接调用（图标由 ComponentResourceManager 直接读），属自动生成支撑类。
// 说明：类名保持混淆原名；ResourceManager/Culture 本身即语义名，仅加注释。
// ============================================================================
namespace gypZRX1vuWc3aeye0R.Internal
{
	// Token: 0x02000010 RID: 16
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[CompilerGenerated]
	[StandardModule]
	[HideModuleName]
	[DebuggerNonUserCode]
	internal sealed class Callback
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00007474 File Offset: 0x00005674
		[EditorBrowsable(EditorBrowsableState.Advanced)]
/// <summary>
/// 资源管理器：惰性创建 new ResourceManager("Resources", typeof(Callback).Assembly) 并缓存。
/// </summary>
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Callback.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("Resources", typeof(Callback).Assembly);
					Callback.resourceMan = resourceManager;
				}
				return Callback.resourceMan;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600009F RID: 159 RVA: 0x000074B4 File Offset: 0x000056B4
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x000022D3 File Offset: 0x000004D3
		[EditorBrowsable(EditorBrowsableState.Advanced)]
/// <summary>资源区域性：get/set resourceCulture（切换资源语言）。</summary>
		internal static CultureInfo Culture
		{
			get
			{
				return Callback.resourceCulture;
			}
			set
			{
				Callback.resourceCulture = value;
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>混淆器填充方法（恒返回 true，无意义）。</summary>
		internal static bool FillThread()
		{
			return true;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000021C6 File Offset: 0x000003C6
/// <summary>混淆器填充方法（恒返回 false，无意义）。</summary>
		internal static bool ConnectThread()
		{
			return false;
		}

		// Token: 0x04000053 RID: 83
/// <summary>资源管理器缓存字段（惰性单例）。</summary>
		private static ResourceManager resourceMan;

		// Token: 0x04000054 RID: 84
/// <summary>资源区域性缓存字段。</summary>
		private static object resourceCulture;
	}
}
