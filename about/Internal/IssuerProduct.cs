using System;
using System.Runtime.InteropServices;

namespace about.Internal
{
	// Token: 0x02000016 RID: 22
	/// <summary>
	/// 混淆器注入的自定义 Attribute（原名 IssuerProduct）：AttributeUsage 覆盖几乎所有目标、AllowMultiple=true，
	/// 带 3 个布尔访问器（GetFlag1..3/SetFlag1..3，默认 true）与 1 个字符串访问器（GetTextValue/SetTextValue，默认 ""）。
	/// 未被业务代码使用，用于干扰反混淆工具与元数据分析，无业务语义。
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	internal sealed class IssuerProduct : Attribute
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00002476 File Offset: 0x00000676
		/// <summary>bool 属性 1 getter（原名 StopProduct，默认 true）。</summary>
		public bool GetFlag1()
		{
			return this._flag1;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000247E File Offset: 0x0000067E
		/// <summary>bool 属性 1 setter（原名 AddProduct）。</summary>
		public void SetFlag1(bool bool_0)
		{
			this._flag1 = bool_0;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00002487 File Offset: 0x00000687
		/// <summary>bool 属性 2 getter（原名 StartProduct，默认 true）。</summary>
		public bool GetFlag2()
		{
			return this._flag2;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000248F File Offset: 0x0000068F
		/// <summary>bool 属性 2 setter（原名 SelectProduct）。</summary>
		public void SetFlag2(bool bool_0)
		{
			this._flag2 = bool_0;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00002498 File Offset: 0x00000698
		/// <summary>string 属性 getter（原名 FindProduct，默认 ""）。</summary>
		public string GetTextValue()
		{
			return this._textValue;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000024A0 File Offset: 0x000006A0
		/// <summary>string 属性 setter（原名 ViewProduct）。</summary>
		public void SetTextValue(string string_0)
		{
			this._textValue = string_0;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000024A9 File Offset: 0x000006A9
		/// <summary>bool 属性 3 getter（原名 InstantiateProduct，默认 true）。</summary>
		public bool GetFlag3()
		{
			return this._flag3;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000024B1 File Offset: 0x000006B1
		/// <summary>bool 属性 3 setter（原名 CheckProduct）。</summary>
		public void SetFlag3(bool bool_0)
		{
			this._flag3 = bool_0;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>混淆填充桩（原名 CalcListener），恒 true，无实义。</summary>
		internal static bool CalcListener()
		{
			return true;
		}

		// Token: 0x0400003B RID: 59
		/// <summary>bool 属性 1 后备字段（原名 _SingletonProduct，默认 true）。</summary>
		private bool _flag1 = true;

		// Token: 0x0400003C RID: 60
		/// <summary>bool 属性 2 后备字段（原名 _MessageProduct，默认 true）。</summary>
		private bool _flag2 = true;

		// Token: 0x0400003D RID: 61
		/// <summary>bool 属性 3 后备字段（原名 m_ComposerProduct，默认 true）。</summary>
		private bool _flag3 = true;

		// Token: 0x0400003E RID: 62
		/// <summary>string 属性后备字段（原名 _IteratorProduct，默认 ""）。</summary>
		private string _textValue = "";
	}
}
