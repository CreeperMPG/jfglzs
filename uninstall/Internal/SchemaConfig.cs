using System;
using System.Runtime.InteropServices;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000018 RID: 24
	/// <summary>
	/// 一个「万能」的特性类：AttributeUsage 覆盖程序集/类/结构/枚举/方法/属性/字段/事件/接口/参数/委托，
	/// 并且 AllowMultiple = true。它带四个普通属性（三个 bool 默认 true、一个 string 默认 ""），
	/// 只是简单的取值/设值壳子，没有任何业务逻辑。
	///
	/// 这类东西是混淆器/保护器塞进来的：把随机名字的特性挂到目标上，让源码看起来更乱，
	/// 同时增加反射扫描时的噪声。全工程没有任何地方使用本特性，也没有任何地方读它的四个属性。
	///
	/// 原始属性名已被随机化且语义不可恢复，这里按「第几个属性」编号命名。
	/// 原混淆名：SchemaConfig
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	internal sealed class 属性容器特性 : Attribute
	{
		// Token: 0x0600009E RID: 158 RVA: 0x0000252B File Offset: 0x0000072B
		/// <summary>取属性 1（bool，默认 true）。原混淆名：InsertConfig</summary>
		public bool 属性1_取值()
		{
			return this.属性1;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00002533 File Offset: 0x00000733
		/// <summary>设属性 1。原混淆名：PatchConfig</summary>
		public void 属性1_设值(bool 值)
		{
			this.属性1 = 值;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0000253C File Offset: 0x0000073C
		/// <summary>取属性 2（bool，默认 true）。原混淆名：ReadConfig</summary>
		public bool 属性2_取值()
		{
			return this.属性2;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00002544 File Offset: 0x00000744
		/// <summary>设属性 2。原混淆名：WriteConfig</summary>
		public void 属性2_设值(bool 值)
		{
			this.属性2 = 值;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000254D File Offset: 0x0000074D
		/// <summary>取属性 3（string，默认空串）。原混淆名：VisitConfig</summary>
		public string 属性3_取值()
		{
			return this.属性3;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00002555 File Offset: 0x00000755
		/// <summary>设属性 3。原混淆名：ValidateConfig</summary>
		public void 属性3_设值(string 值)
		{
			this.属性3 = 值;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000255E File Offset: 0x0000075E
		/// <summary>取属性 4（bool，默认 true）。原混淆名：ConcatConfig</summary>
		public bool 属性4_取值()
		{
			return this.属性4;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00002566 File Offset: 0x00000766
		/// <summary>设属性 4。原混淆名：InitConfig</summary>
		public void 属性4_设值(bool 值)
		{
			this.属性4 = 值;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>混淆器注入的占位方法：恒返回 true，无调用者。原混淆名：NewParser</summary>
		internal static bool 混淆器占位方法_恒真()
		{
			return true;
		}

		// Token: 0x04000044 RID: 68
		/// <summary>属性 1 的存储。</summary>
		private bool 属性1 = true;

		// Token: 0x04000045 RID: 69
		/// <summary>属性 2 的存储。</summary>
		private bool 属性2 = true;

		// Token: 0x04000046 RID: 70
		/// <summary>属性 4 的存储（字段顺序照原样，属性 3 是字符串所以排在后面）。</summary>
		private bool 属性4 = true;

		// Token: 0x04000047 RID: 71
		/// <summary>属性 3 的存储。</summary>
		private string 属性3 = "";
	}
}
