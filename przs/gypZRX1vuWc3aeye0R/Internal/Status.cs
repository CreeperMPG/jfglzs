using System;
using System.Runtime.InteropServices;

// ============================================================================
// Status.cs — 混淆器生成的 Attribute 辅助类
// 真实职责：反混淆后为混淆器（ConfuserEx/Reactor）生成的辅助 Attribute，允许挂在程序集/类/方法/
// 属性/字段等几乎所有目标上，携带两个 bool 与一个 string 配置值（_Exporter/manager/_Composer/_Parser，
// 全部默认 true/""）。全程序无任何业务代码引用，纯为混淆框架预留；CloneThread/InvokeThread 为填充方法。
// 说明：类名保持混淆原名，方法已按 Status.cs.md 映射表反混淆重命名（字段名保留并注释）。
// ============================================================================
namespace gypZRX1vuWc3aeye0R.Internal
{
	// Token: 0x0200001A RID: 26
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	internal sealed class Status : Attribute
	{
		// Token: 0x060000DB RID: 219 RVA: 0x000024B2 File Offset: 0x000006B2
/// <summary>读 _Exporter 配置（bool，默认 true）。</summary>
		public bool 取Exporter配置()
		{
			return this._Exporter;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000024BA File Offset: 0x000006BA
/// <summary>写 _Exporter 配置。</summary>
		public void 写Exporter配置(bool bool_0)
		{
			this._Exporter = bool_0;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000024C3 File Offset: 0x000006C3
/// <summary>读 manager 配置（bool，默认 true）。</summary>
		public bool 取manager配置()
		{
			return this.manager;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000024CB File Offset: 0x000006CB
/// <summary>写 manager 配置。</summary>
		public void 写manager配置(bool bool_0)
		{
			this.manager = bool_0;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000024D4 File Offset: 0x000006D4
/// <summary>读 _Parser 配置（string，默认 ""）。</summary>
		public string 取Parser配置()
		{
			return this._Parser;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000024DC File Offset: 0x000006DC
/// <summary>写 _Parser 配置。</summary>
		public void 写Parser配置(string string_0)
		{
			this._Parser = string_0;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000024E5 File Offset: 0x000006E5
/// <summary>读 _Composer 配置（bool，默认 true）。</summary>
		public bool 取Composer配置()
		{
			return this._Composer;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000024ED File Offset: 0x000006ED
/// <summary>写 _Composer 配置。</summary>
		public void 写Composer配置(bool bool_0)
		{
			this._Composer = bool_0;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>混淆器填充方法（恒返回 true，无意义）。</summary>
		internal static bool CloneThread()
		{
			return true;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000021C6 File Offset: 0x000003C6
/// <summary>混淆器填充方法（恒返回 false，无意义）。</summary>
		internal static bool InvokeThread()
		{
			return false;
		}

		// Token: 0x04000069 RID: 105
/// <summary>bool 配置字段（默认 true，未使用）。</summary>
		private bool _Exporter = true;

		// Token: 0x0400006A RID: 106
/// <summary>bool 配置字段（默认 true，未使用）。</summary>
		private bool manager = true;

		// Token: 0x0400006B RID: 107
/// <summary>bool 配置字段（默认 true，未使用）。</summary>
		private bool _Composer = true;

		// Token: 0x0400006C RID: 108
/// <summary>string 配置字段（默认 ""，未使用）。</summary>
		private string _Parser = "";
	}
}
