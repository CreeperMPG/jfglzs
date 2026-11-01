using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace jfglzsn.Internal
{
	// Token: 0x02000008 RID: 8
	/// <summary>
	/// 强类型资源访问器，等价于 VB.NET 模板里的「My.Resources」。
	/// 资源容器是资源清单名 <c>WindowsApplication1.Resources</c>（见 g.resx / Resources.resx）。
	/// <para>
	/// 本工程没有定义任何具名资源属性（模板只留下 ResourceManager 与 Culture 两处基础访问），
	/// 另外多出两个恒返回常量的判定桩，工程内无引用。
	/// </para>
	/// </summary>
	[HideModuleName]
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[StandardModule]
	internal sealed class ProductEvent
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002304 File Offset: 0x00000504
		/// <summary>
		/// 懒加载的资源管理器，绑定到清单 <c>WindowsApplication1.Resources</c>。
		/// 用 <c>ReferenceEquals</c> 判空而非 <c>==</c>，与模板生成一致。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(ProductEvent.resourceMan, null))
				{
					ProductEvent.resourceMan = new ResourceManager("WindowsApplication1.Resources", typeof(ProductEvent).Assembly);
				}
				return ProductEvent.resourceMan;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002336 File Offset: 0x00000536
		// (set) Token: 0x0600002F RID: 47 RVA: 0x0000233D File Offset: 0x0000053D
		/// <summary>
		/// 资源查找使用的区域文化。<c>null</c> 表示跟随当前线程文化。
		/// 注意字段声明类型是 <c>object</c>（模板生成如此），不要"修正"成 CultureInfo；
		/// 取值时这一句 <c>(CultureInfo)</c> 是 VB 后期绑定转换的显式化，与原版行为一致（类型不对会抛 InvalidCastException）。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return (CultureInfo)ProductEvent.resourceCulture;
			}
			set
			{
				ProductEvent.resourceCulture = value;
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>混淆器/模板插入的判定桩，恒返回 <c>true</c>，工程内无引用。</summary>
		internal static bool FlushDefinition()
		{
			return true;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000021E0 File Offset: 0x000003E0
		/// <summary>混淆器/模板插入的判定桩，恒返回 <c>false</c>，工程内无引用。</summary>
		internal static bool AddDefinition()
		{
			return false;
		}

		// Token: 0x0400000C RID: 12
		/// <summary>缓存下来的资源管理器实例（懒加载）。</summary>
		private static ResourceManager resourceMan;

		// Token: 0x0400000D RID: 13
		/// <summary>资源查找的文化信息缓存；类型为 <c>object</c>，实际存放 <see cref="CultureInfo"/>。</summary>
		private static object resourceCulture;
	}
}
