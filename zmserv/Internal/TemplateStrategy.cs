using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace zmserv.Internal
{
	// Token: 0x02000007 RID: 7
	/// <summary>
	/// VB 的 My.Resources（Resources.resx 的强类型资源访问器）。
	/// 真实职责：按需惰性创建 "zmserv.Resources" 的 ResourceManager，并暴露 Culture 开关。
	/// 注意：Resources.resx 里没有任何资源项（只有一份空模板），所以本类取不到任何值；
	/// 真正的敏感数据在加壳器塞进程序集的两份匿名嵌入资源里（见 IntelliLock运行时）。
	/// </summary>
	[StandardModule]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[HideModuleName]
	internal sealed class MyResources
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002252 File Offset: 0x00000452
		/// <summary>
		/// 惰性创建的 ResourceManager，资源清单名为 "zmserv.Resources"。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(MyResources.resourceMan, null))
				{
					MyResources.resourceMan = new ResourceManager("zmserv.Resources", typeof(MyResources).Assembly);
				}
				return MyResources.resourceMan;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002284
		// (set) Token: 0x0600001C RID: 28 RVA: 0x0000228B
		/// <summary>
		/// 资源查询用的语言/区域（默认 null = 跟随当前线程）。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return MyResources.resourceCulture;
			}
			set
			{
				MyResources.resourceCulture = value;
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>壳注入的空桩：固定返回 true，无调用点。</summary>
		internal static bool FindDefinition()
		{
			return true;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
		internal static bool TestDefinition()
		{
			return false;
		}

		// Token: 0x04000006 RID: 6
		/// <summary>缓存的 ResourceManager 实例。</summary>
		private static ResourceManager resourceMan;

		// Token: 0x04000007 RID: 7
		/// <summary>当前区域文化（object 类型是为了同时接受 null 与 CultureInfo）。</summary>
		private static object resourceCulture;
	}
}
