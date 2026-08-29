using System;
using System.Runtime.InteropServices;

namespace zmserv.Internal
{
	// Token: 0x02000013 RID: 19
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	/// <summary>
	/// DicReg —— 自定义标记 Attribute（保护器标记/混淆填充）：可应用于几乎所有目标（AllowMultiple），
	/// 含 4 个可读写属性（3 个 bool 默认 true + 1 个 string 默认空串）。无任何逻辑，是
	/// .NET Reactor/ConfuserEx 风格的保护器标记属性，也可能是脱壳后的残留。
	/// </summary>
	internal sealed class DicReg : Attribute
	{
		// Token: 0x06000069 RID: 105 RVA: 0x000023B5 File Offset: 0x000005B5
		/// <summary>bool 属性 1 的 getter：返回 flag1（默认 true）。</summary>
		public bool Flag1()
		{
			return this.flag1;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000023BD File Offset: 0x000005BD
		/// <summary>bool 属性 1 的 setter：写入 flag1。</summary>
		public void SetFlag1(bool bool_0)
		{
			this.flag1 = bool_0;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000023C6 File Offset: 0x000005C6
		/// <summary>bool 属性 2 的 getter：返回 flag2（默认 true）。</summary>
		public bool Flag2()
		{
			return this.flag2;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000023CE File Offset: 0x000005CE
		/// <summary>bool 属性 2 的 setter：写入 flag2。</summary>
		public void SetFlag2(bool bool_0)
		{
			this.flag2 = bool_0;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000023D7 File Offset: 0x000005D7
		/// <summary>string 属性的 getter：返回 tag（默认 ""）。</summary>
		public string Tag()
		{
			return this.tag;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000023DF File Offset: 0x000005DF
		/// <summary>string 属性的 setter：写入 tag。</summary>
		public void SetTag(string string_0)
		{
			this.tag = string_0;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000023E8 File Offset: 0x000005E8
		/// <summary>bool 属性 3 的 getter：返回 flag3（默认 true）。</summary>
		public bool Flag3()
		{
			return this.flag3;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000023F0 File Offset: 0x000005F0
		/// <summary>bool 属性 3 的 setter：写入 flag3。</summary>
		public void SetFlag3(bool bool_0)
		{
			this.flag3 = bool_0;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>混淆填充：恒真，无业务意义（无调用方）。</summary>
		internal static bool ConfuserFillerTrue()
		{
			return true;
		}

		// Token: 0x04000029 RID: 41
		/// <summary>bool 属性 1 后端字段，默认 true。</summary>
		private bool flag1 = true;

		// Token: 0x0400002A RID: 42
		/// <summary>bool 属性 2 后端字段，默认 true。</summary>
		private bool flag2 = true;

		// Token: 0x0400002B RID: 43
		/// <summary>bool 属性 3 后端字段，默认 true。</summary>
		private bool flag3 = true;

		// Token: 0x0400002C RID: 44
		/// <summary>string 属性后端字段，默认空串。</summary>
		private string tag = "";
	}
}
