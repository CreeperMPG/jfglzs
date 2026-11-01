using System;
using System.Runtime.InteropServices;

namespace WindowsApplication1.Internal
{
	/// <summary>
	/// 混淆器用的自定义 Attribute（本工程内没有任何应用点，也没有任何读取点）。
	/// 它的四个成员与 <c>System.Reflection.ObfuscationAttribute</c> 的四个开关一一对应
	/// （排除、应用于成员、混淆后剥离、功能名），默认值全是 true / 空串，
	/// 因此它就是 .NET Reactor 自带的那份「混淆控制属性」的克隆，用来在混淆期标注成员。
	/// 保留它只为对齐原元数据，对运行时没有影响。
	/// </summary>
	// Token: 0x02000016 RID: 22
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	internal sealed class DescriptorMapper : Attribute
	{
		// Token: 0x060000B0 RID: 176 RVA: 0x00002590 File Offset: 0x00000790
		/// <summary>读取「排除」开关（对应 ObfuscationAttribute.Exclude）。</summary>
		public bool 取排除标记()
		{
			return this.排除标记;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00002598 File Offset: 0x00000798
		/// <summary>设置「排除」开关。</summary>
		public void 设排除标记(bool 值)
		{
			this.排除标记 = 值;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000025A1 File Offset: 0x000007A1
		/// <summary>读取「应用于成员」开关（对应 ObfuscationAttribute.ApplyToMembers）。</summary>
		public bool 取应用于成员()
		{
			return this.应用于成员;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000025A9 File Offset: 0x000007A9
		/// <summary>设置「应用于成员」开关。</summary>
		public void 设应用于成员(bool 值)
		{
			this.应用于成员 = 值;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000025B2 File Offset: 0x000007B2
		/// <summary>读取「功能名」（对应 ObfuscationAttribute.Feature），默认空串。</summary>
		public string 取功能名()
		{
			return this.功能名;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000025BA File Offset: 0x000007BA
		/// <summary>设置「功能名」。</summary>
		public void 设功能名(string 值)
		{
			this.功能名 = 值;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000025C3 File Offset: 0x000007C3
		/// <summary>读取「混淆后剥离」开关（对应 ObfuscationAttribute.StripAfterObfuscation）。</summary>
		public bool 取混淆后剥离()
		{
			return this.混淆后剥离;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000025CB File Offset: 0x000007CB
		/// <summary>设置「混淆后剥离」开关。</summary>
		public void 设混淆后剥离(bool 值)
		{
			this.混淆后剥离 = 值;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>恒返回 true 的桩方法，无调用点。</summary>
		internal static bool 返回真占位()
		{
			return true;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>恒返回 false 的桩方法，无调用点。</summary>
		internal static bool 返回假占位()
		{
			return false;
		}

		// Token: 0x0400003B RID: 59
		/// <summary>「排除」开关，默认 true。</summary>
		private bool 排除标记 = true;

		// Token: 0x0400003C RID: 60
		/// <summary>「应用于成员」开关，默认 true。</summary>
		private bool 应用于成员 = true;

		// Token: 0x0400003D RID: 61
		/// <summary>「混淆后剥离」开关，默认 true。</summary>
		private bool 混淆后剥离 = true;

		// Token: 0x0400003E RID: 62
		/// <summary>「功能名」，默认空串。</summary>
		private string 功能名 = "";
	}
}
