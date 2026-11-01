using System;
using System.Runtime.InteropServices;

namespace set.Internal
{
	/// <summary>
	/// 一个可多次应用（AllowMultiple）的自定义特性：三个布尔开关（默认全为 true）+ 一个字符串（默认空）。
	///
	/// 说明：类名与成员名都被 .NET Reactor 重写成了随机词（Attr / Candidate / Factory 是它的名字池），
	/// 程序集内<b>没有任何</b> <c>[ProducerCandidate]</c> 应用点，也没有任何代码构造它，
	/// 因此原语义无法从代码静态确定 —— 大概率是加壳工具/原作者的元数据标记，属于死代码。
	/// 下面按「第几个开关」命名，只是为了读起来不撞车，不代表真实含义。
	/// </summary>
	// Token: 0x02000019 RID: 25
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	internal sealed class ProducerCandidate : Attribute
	{
		/// <summary>读开关一（默认 true）。</summary>
		// Token: 0x0600012E RID: 302 RVA: 0x00002908 File Offset: 0x00000B08
		public bool 取开关一()
		{
			return this._开关一;
		}

		/// <summary>写开关一。</summary>
		// Token: 0x0600012F RID: 303 RVA: 0x00002910 File Offset: 0x00000B10
		public void 设开关一(bool 值)
		{
			this._开关一 = 值;
		}

		/// <summary>读开关二（默认 true）。</summary>
		// Token: 0x06000130 RID: 304 RVA: 0x00002919 File Offset: 0x00000B19
		public bool 取开关二()
		{
			return this._开关二;
		}

		/// <summary>写开关二。</summary>
		// Token: 0x06000131 RID: 305 RVA: 0x00002921 File Offset: 0x00000B21
		public void 设开关二(bool 值)
		{
			this._开关二 = 值;
		}

		/// <summary>读文本项（默认空串）。</summary>
		// Token: 0x06000132 RID: 306 RVA: 0x0000292A File Offset: 0x00000B2A
		public string 取文本项()
		{
			return this._文本项;
		}

		/// <summary>写文本项。</summary>
		// Token: 0x06000133 RID: 307 RVA: 0x00002932 File Offset: 0x00000B32
		public void 设文本项(string 值)
		{
			this._文本项 = 值;
		}

		/// <summary>读开关三（默认 true）。</summary>
		// Token: 0x06000134 RID: 308 RVA: 0x0000293B File Offset: 0x00000B3B
		public bool 取开关三()
		{
			return this._开关三;
		}

		/// <summary>写开关三。</summary>
		// Token: 0x06000135 RID: 309 RVA: 0x00002943 File Offset: 0x00000B43
		public void 设开关三(bool 值)
		{
			this._开关三 = 值;
		}

		/// <summary>恒返回 true 的死代码桩，无调用点。</summary>
		// Token: 0x06000137 RID: 311 RVA: 0x000021E5 File Offset: 0x000003E5
		internal static bool 恒真桩()
		{
			return true;
		}

		// Token: 0x04000088 RID: 136
		private bool _开关一 = true;

		// Token: 0x04000089 RID: 137
		private bool _开关二 = true;

		// Token: 0x0400008A RID: 138
		private bool _开关三 = true;

		// Token: 0x0400008B RID: 139
		private string _文本项 = "";
	}
}
