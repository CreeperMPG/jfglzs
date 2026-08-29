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
	[CompilerGenerated]
	[HideModuleName]
	[StandardModule]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[DebuggerNonUserCode]
	/// <summary>
	/// ValueReg —— 强类型资源包装类（ResX 代码生成器自动生成）：提供 ResourceManager（指向嵌入资源
	/// zmserv.Resources / Resources.resources）与 Culture 属性。无任何业务逻辑；恶意载荷（如有）以资源形式存在
	/// Resources.resources / g.resources 中（GUID 资源 c3a777aa-...、b215f184-...、b0494a1f-... 由 CustomerReg/DispatcherReg 运行时读取）。
	/// </summary>
	internal sealed class ValueReg
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002550 File Offset: 0x00000750
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		/// <summary>资源管理器：懒创建 new ResourceManager("zmserv.Resources", 程序集)。</summary>
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(ValueReg.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("zmserv.Resources", typeof(ValueReg).Assembly);
					ValueReg.resourceMan = resourceManager;
				}
				return ValueReg.resourceMan;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002590 File Offset: 0x00000790
		// (set) Token: 0x0600001D RID: 29 RVA: 0x000021D2 File Offset: 0x000003D2
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		/// <summary>区域文化：get/set（resourceCulture 缓存字段）。</summary>
		internal static CultureInfo Culture
		{
			get
			{
				return ValueReg.resourceCulture;
			}
			set
			{
				ValueReg.resourceCulture = value;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>混淆填充：恒假，无业务意义（无调用方）。</summary>
		internal static bool StartParser()
		{
			return false;
		}

		// Token: 0x04000006 RID: 6
		/// <summary>ResourceManager 缓存字段（懒初始化）。</summary>
		private static ResourceManager resourceMan;

		// Token: 0x04000007 RID: 7
		/// <summary>Culture 缓存字段。</summary>
		private static object resourceCulture;
	}
}
