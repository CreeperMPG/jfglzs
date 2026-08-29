using System;
using System.Runtime.InteropServices;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000018 RID: 24
	// <summary>
	/// 自定义特性类（加壳/混淆工具 .NET Reactor/IntelliLock 用于给程序集成员打标记，
	/// 例如标记保护范围/排除项），运行时无任何行为。含 4 个属性：
	/// 属性 A/B/C 为 bool（默认 true），属性 D 为 string（默认 ""）。
	/// 反编译产物中未见消费方（属运行时反射标记），无恶意行为、无 IOC。
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	internal sealed class ClassConfig : Attribute
	{
		// Token: 0x0600009E RID: 158 RVA: 0x00002441 File Offset: 0x00000641
		/// <summary>属性 A getter（原 PushConfig）：bool，默认 true。</summary>
		public bool GetPropertyA()
		{
			return this._propertyA;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00002449 File Offset: 0x00000649
		/// <summary>属性 A setter（原 InitConfig）。</summary>
		public void SetPropertyA(bool bool_0)
		{
			this._propertyA = bool_0;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00002452 File Offset: 0x00000652
		/// <summary>属性 B getter（原 ResolveConfig）：bool，默认 true。</summary>
		public bool GetPropertyB()
		{
			return this._propertyB;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000245A File Offset: 0x0000065A
		/// <summary>属性 B setter（原 WriteConfig）。</summary>
		public void SetPropertyB(bool bool_0)
		{
			this._propertyB = bool_0;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00002463 File Offset: 0x00000663
		/// <summary>属性 D getter（原 NewConfig）：string，默认 ""。</summary>
		public string GetPropertyD()
		{
			return this._propertyD;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000246B File Offset: 0x0000066B
		/// <summary>属性 D setter（原 CustomizeConfig）。</summary>
		public void SetPropertyD(string string_0)
		{
			this._propertyD = string_0;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00002474 File Offset: 0x00000674
		/// <summary>属性 C getter（原 RunConfig）：bool，默认 true。</summary>
		public bool GetPropertyC()
		{
			return this._propertyC;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000247C File Offset: 0x0000067C
		/// <summary>属性 C setter（原 DefineConfig）。</summary>
		public void SetPropertyC(bool bool_0)
		{
			this._propertyC = bool_0;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>恒 true 桩方法（混淆噪声），无实际逻辑。</summary>
		internal static bool RegisterRegistry()
		{
			return true;
		}

		// Token: 0x04000044 RID: 68
		/// <summary>属性 A 后端字段（bool，默认 true）。</summary>
		private bool _propertyA = true;

		// Token: 0x04000045 RID: 69
		/// <summary>属性 B 后端字段（bool，默认 true）。</summary>
		private bool _propertyB = true;

		// Token: 0x04000046 RID: 70
		/// <summary>属性 C 后端字段（bool，默认 true）。</summary>
		private bool _propertyC = true;

		// Token: 0x04000047 RID: 71
		/// <summary>属性 D 后端字段（string，默认 ""）。</summary>
		private string _propertyD = "";
	}
}
