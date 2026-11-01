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
	// Token: 0x02000008 RID: 8
	/// <summary>
	/// VB.NET 自动生成的 My.Resources 访问模块（[StandardModule] + [HideModuleName]）。
	/// 资源容器名就是 "WindowsApplication1.Resources"，与本工程唯一的 Resources.resx 对应。
	/// 工程里没有强类型资源属性，所以这个类实际只提供 ResourceManager/Culture 两个开关，
	/// 全工程没有任何地方取资源——它属于模板残留。
	/// 原混淆名：Rules
	/// </summary>
	[HideModuleName]
	[StandardModule]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal sealed class MyResources
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000022D2 File Offset: 0x000004D2
		/// <summary>懒加载的 ResourceManager，绑定内嵌资源 "WindowsApplication1.Resources"。</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(MyResources.资源管理器缓存, null))
				{
					MyResources.资源管理器缓存 = new ResourceManager("WindowsApplication1.Resources", typeof(MyResources).Assembly);
				}
				return MyResources.资源管理器缓存;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002304 File Offset: 0x00000504
		// (set) Token: 0x06000029 RID: 41 RVA: 0x0000230B File Offset: 0x0000050B
		/// <summary>取/设资源查找用的区域文化（My.Resources.Culture 的等价物）。</summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return (CultureInfo)MyResources.当前区域文化;
			}
			set
			{
				MyResources.当前区域文化 = value;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>混淆器注入的占位方法：恒返回 true，无调用者。原混淆名：InvokeParser</summary>
		internal static bool 混淆器占位方法_恒真()
		{
			return true;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>混淆器注入的占位方法：恒返回 false，无调用者。原混淆名：ManageParser</summary>
		internal static bool 混淆器占位方法_恒假()
		{
			return false;
		}

		// Token: 0x0400000A RID: 10
		/// <summary>ResourceManager 单例缓存。</summary>
		private static ResourceManager 资源管理器缓存;

		// Token: 0x0400000B RID: 11
		/// <summary>当前区域文化。注意原字段类型是 object，取值时按原码强转回 CultureInfo。</summary>
		private static object 当前区域文化;
	}
}
