using System;
using System.Runtime.InteropServices;

namespace OZxTKXuvsl9D34LWhP.Internal
{
	/// <summary>
	/// 通用标记特性（类名 Reader 是混淆名）。可标在程序集/类型/方法/属性/字段/参数/委托上，
	/// 只承载四个 bool / string 值，本工程里没有任何地方用到它的实例——是 IntelliLock/.NET Reactor
	/// 壳生成的属性类型残留，留着只是为了让元数据里的特性引用仍能解析。
	/// </summary>
	// Token: 0x0200001A RID: 26
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	internal sealed class Reader : Attribute
	{
		// Token: 0x060000DB RID: 219 RVA: 0x00002670
		/// <summary>读 <c>_Serializer</c> 标志（原名 SetWatcher），默认 true。</summary>
		public bool SetWatcher()
		{
			return this._Serializer;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00002678
		/// <summary>写 <c>_Serializer</c> 标志（原名 DeleteWatcher）。</summary>
		public void DeleteWatcher(bool 值)
		{
			this._Serializer = 值;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00002681
		/// <summary>读 <c>_Callback</c> 标志（原名 AddWatcher），默认 true。</summary>
		public bool AddWatcher()
		{
			return this._Callback;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00002689
		/// <summary>写 <c>_Callback</c> 标志（原名 MapWatcher）。</summary>
		public void MapWatcher(bool 值)
		{
			this._Callback = 值;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00002692
		/// <summary>读字符串成员 <c>code</c>（原名 QueryWatcher）。</summary>
		public string QueryWatcher()
		{
			return this.code;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000269A
		/// <summary>写字符串成员 <c>code</c>（原名 SearchWatcher）。</summary>
		public void SearchWatcher(string 值)
		{
			this.code = 值;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000026A3
		/// <summary>读 <c>collection</c> 标志（原名 VerifyWatcher），默认 true。</summary>
		public bool VerifyWatcher()
		{
			return this.collection;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000026AB
		/// <summary>写 <c>collection</c> 标志（原名 CalculateWatcher）。</summary>
		public void CalculateWatcher(bool 值)
		{
			this.collection = 值;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000021DD
		/// <summary>壳生成的授权标记桩函数，恒返回 true，无调用者（原名 QueryTag）。</summary>
		internal static bool QueryTag()
		{
			return true;
		}

		// Token: 0x04000069 RID: 105
		private bool _Serializer = true;

		// Token: 0x0400006A RID: 106
		private bool _Callback = true;

		// Token: 0x0400006B RID: 107
		private bool collection = true;

		// Token: 0x0400006C RID: 108
		private string code = "";
	}
}
