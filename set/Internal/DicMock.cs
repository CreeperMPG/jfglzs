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
		/// 强类型资源包装（WindowsApplication1.Resources）：模板生成，提供 ResourceManager 与 Culture 访问。
		/// 样本通过资源方式携带载荷文件（yl.wav/huifu.wav/zy\\ 假浏览器/壁纸等）。
		/// </summary>
	// Token: 0x0200000E RID: 14
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[HideModuleName]
	[StandardModule]
	[CompilerGenerated]
	internal sealed class DicMock
	{
		/// <summary>
		/// 资源管理器单例：new ResourceManager("WindowsApplication1.Resources", 程序集)。
		/// </summary>
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000EC RID: 236 RVA: 0x0000BCEC File Offset: 0x00009EEC
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(DicMock.m_resourceManager, null))
				{
					ResourceManager resourceManager = new ResourceManager("WindowsApplication1.Resources", typeof(DicMock).Assembly);
					DicMock.m_resourceManager = resourceManager;
				}
				return DicMock.m_resourceManager;
			}
		}

		/// <summary>
		/// 资源文化（读/写 m_resourceCulture 字段）。
		/// </summary>
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000ED RID: 237 RVA: 0x0000BD2C File Offset: 0x00009F2C
		// (set) Token: 0x060000EE RID: 238 RVA: 0x0000242B File Offset: 0x0000062B
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return DicMock.m_resourceCulture;
			}
			set
			{
				DicMock.m_resourceCulture = value;
			}
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 true，无实际逻辑。
		/// </summary>
		// Token: 0x060000EF RID: 239 RVA: 0x000021D1 File Offset: 0x000003D1
		internal static bool Placeholder_ReturnTrue()
		{
			return true;
		}

		/// <summary>
		/// ResourceManager 单例后备字段。
		/// </summary>
		// Token: 0x04000070 RID: 112
		private static ResourceManager m_resourceManager;

		/// <summary>
		/// Culture 后备字段。
		/// </summary>
		// Token: 0x04000071 RID: 113
		private static object m_resourceCulture;
	}
}
