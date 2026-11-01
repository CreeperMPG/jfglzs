using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace OZxTKXuvsl9D34LWhP.Internal
{
	/// <summary>
	/// VB 强类型资源访问器（类名 Error 是混淆名，原类型名应为 <c>Resources</c>）。
	/// 由 <c>Resources.resx</c> 的 StronglyTypedResourceBuilder 自动生成，只提供
	/// <see cref="ResourceManager"/> 和 <see cref="Culture"/> 两个成员，本工程没有具体资源属性。
	/// 两个 <c>XxxTag()</c> 是 .NET Reactor 生成的授权标记桩。
	/// </summary>
	// Token: 0x02000008 RID: 8
	[HideModuleName]
	[CompilerGenerated]
	[StandardModule]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	internal sealed class Error
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002D RID: 45 RVA: 0x0000231D File Offset: 0x0000051D
		/// <summary>资源管理器单例，绑定的是程序集里的 <c>Resources</c> 资源（原名 ResourceManager）。</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Error.resourceMan, null))
				{
					Error.resourceMan = new ResourceManager("Resources", typeof(Error).Assembly);
				}
				return Error.resourceMan;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000234F File Offset: 0x0000054F
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002356 File Offset: 0x00000556
		/// <summary>资源查找用的区域性（原名 Culture），默认 null 表示当前区域性。</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Error.resourceCulture;
			}
			set
			{
				Error.resourceCulture = value;
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>.NET Reactor 生成的授权标记桩，恒返回 true，无调用者（原名 CompareTag）。</summary>
		internal static bool CompareTag()
		{
			return true;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>.NET Reactor 生成的授权标记桩，恒返回 false，无调用者（原名 InvokeTag）。</summary>
		internal static bool InvokeTag()
		{
			return false;
		}

		// Token: 0x0400000D RID: 13
		/// <summary>ResourceManager 缓存（原名 resourceMan）。</summary>
		private static ResourceManager resourceMan;

		// Token: 0x0400000E RID: 14
		/// <summary>区域性缓存（原名 resourceCulture）。</summary>
		private static object resourceCulture;
	}
}
