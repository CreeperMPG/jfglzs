using System;
using System.Runtime.InteropServices;

namespace zmserv.Internal
{
	// Token: 0x02000013 RID: 19
	/// <summary>
	/// 一个"什么都不是"的 Attribute 派生类（加壳器注入的残留类型）。
	/// 真实职责：没有。属性标志允许贴在程序集/类/方法等任何地方且可重复贴，
	/// 但全工程找不到任何一处 [ExpressionStrategy] 贴用点，四个字段也只是默认值。
	/// 下面 4 组 get/set 与 4 个私有字段一一对应（工厂/线程/模型/角色），
	/// 名字都是加壳器随机词，本文件保持原名以便和 IL 对照，读者可以整类跳过。
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	internal sealed class ExpressionStrategy : Attribute
	{
		// Token: 0x06000069 RID: 105 RVA: 0x000023F5 File Offset: 0x000005F5
		/// <summary>读 factoryStrategy（默认 true）。本工程无调用点。</summary>
		public bool ReflectParams()
		{
			return this.factoryStrategy;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000023FD File Offset: 0x000005FD
		/// <summary>写 factoryStrategy。本工程无调用点。</summary>
		public void StopParams(bool 新工厂标志)
		{
			this.factoryStrategy = 新工厂标志;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00002406 File Offset: 0x00000606
		/// <summary>读 threadStrategy（默认 true）。本工程无调用点。</summary>
		public bool NewParams()
		{
			return this.threadStrategy;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000240E
		/// <summary>写 threadStrategy。本工程无调用点。</summary>
		public void DestroyParams(bool 新线程标志)
		{
			this.threadStrategy = 新线程标志;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002417
		/// <summary>读 m_RoleStrategy（默认空串）。本工程无调用点。</summary>
		public string GetParams()
		{
			return this.m_RoleStrategy;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000241F
		/// <summary>写 m_RoleStrategy。本工程无调用点。</summary>
		public void ListParams(string 新角色文本)
		{
			this.m_RoleStrategy = 新角色文本;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00002428
		/// <summary>读 m_ModelStrategy（默认 true）。本工程无调用点。</summary>
		public bool ManageParams()
		{
			return this.m_ModelStrategy;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00002430
		/// <summary>写 m_ModelStrategy。本工程无调用点。</summary>
		public void ForgotParams(bool 新模型标志)
		{
			this.m_ModelStrategy = 新模型标志;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>壳注入的空桩：固定返回 true，无调用点。</summary>
		internal static bool FillDefinition()
		{
			return true;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
		internal static bool CloneDefinition()
		{
			return false;
		}

		// Token: 0x04000029 RID: 41
		/// <summary>布尔字段 1，默认 true。</summary>
		private bool factoryStrategy = true;

		// Token: 0x0400002A RID: 42
		/// <summary>布尔字段 2，默认 true。</summary>
		private bool threadStrategy = true;

		// Token: 0x0400002B RID: 43
		/// <summary>布尔字段 3，默认 true。</summary>
		private bool m_ModelStrategy = true;

		// Token: 0x0400002C RID: 44
		/// <summary>文本字段，默认空串。</summary>
		private string m_RoleStrategy = "";
	}
}
