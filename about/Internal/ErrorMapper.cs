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
	/// <summary>
	/// 强类型资源访问器（VB 的 `My.Resources` 实现）：用 <see cref="ResourceManager"/>
	/// 读取内嵌资源 `WindowsApplication1.Resources`。
	/// 注意工程里的 Resources.resx 是空容器，因此本类实际取不到任何资源；
	/// 它只是模板产物 + 两个恒值桩。
	/// </summary>
	// Token: 0x02000008 RID: 8
	[StandardModule]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[HideModuleName]
	[CompilerGenerated]
	internal sealed class ErrorMapper
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000022D2 File Offset: 0x000004D2
		/// <summary>
		/// 取资源管理器（懒加载）：首次访问时用基名 `WindowsApplication1.Resources`
		/// 和本程序集创建并缓存实例。缓存字段是普通静态字段，没有加锁。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(ErrorMapper.资源管理器缓存, null))
				{
					ErrorMapper.资源管理器缓存 = new ResourceManager("WindowsApplication1.Resources", typeof(ErrorMapper).Assembly);
				}
				return ErrorMapper.资源管理器缓存;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002304 File Offset: 0x00000504
		// (set) Token: 0x06000029 RID: 41 RVA: 0x0000230B File Offset: 0x0000050B
		/// <summary>
		/// 资源查找使用的区域。原版把后备字段声明成 `object`，取值器却按 `CultureInfo` 返回
		/// （VB 在 Option Strict Off 下自动做 CType 收窄），所以这里补一个显式强转还原同一行为；
		/// 字段类型仍保持 `object` 不变。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return (CultureInfo)ErrorMapper.当前区域;
			}
			set
			{
				ErrorMapper.当前区域 = value;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>恒返回 true 的桩方法，无调用点。</summary>
		internal static bool 返回真占位()
		{
			return true;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>恒返回 false 的桩方法，无调用点。</summary>
		internal static bool 返回假占位()
		{
			return false;
		}

		// Token: 0x0400000A RID: 10
		/// <summary>资源管理器实例缓存（懒加载，无锁）。</summary>
		private static ResourceManager 资源管理器缓存;

		// Token: 0x0400000B RID: 11
		/// <summary>资源查找区域，原元数据类型为 object。</summary>
		private static object 当前区域;
	}
}
