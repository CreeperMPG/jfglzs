using System;
using System.Runtime.InteropServices;

namespace WindowsApplication1.Internal
{
	// Token: 0x0200001B RID: 27
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	// ============================================================
	// Internal/RequestVisitor.cs — 壳自定义 Attribute（自动生成，无逻辑）
	// 自定义 Attribute（[AttributeUsage(...)]、[ComVisible(true)]），带 3 个 bool 属性与 1 个 string 属性及各自 get/set。.NET Reactor/混淆器生成的标记属性，方法名全是混淆假名。
	// ============================================================
	internal sealed class RequestVisitor : Attribute
	{
		// Token: 0x06000108 RID: 264 RVA: 0x000024C2 File Offset: 0x000006C2
		/// <summary>
		/// 属性对 1 的 getter（bool，默认 true）。
		/// </summary>
		public bool DisableVisitor()
		{
			return this._SchemaVisitor;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x000024CA File Offset: 0x000006CA
		/// <summary>
		/// 属性对 1 的 setter。
		/// </summary>
		public void InterruptVisitor(bool bool_0)
		{
			this._SchemaVisitor = bool_0;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x000024D3 File Offset: 0x000006D3
		/// <summary>
		/// 属性对 2 的 getter（bool，默认 true）。
		/// </summary>
		public bool ResetVisitor()
		{
			return this.m_BaseVisitor;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x000024DB File Offset: 0x000006DB
		/// <summary>
		/// 属性对 2 的 setter。
		/// </summary>
		public void ResolveVisitor(bool bool_0)
		{
			this.m_BaseVisitor = bool_0;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000024E4 File Offset: 0x000006E4
		/// <summary>
		/// 属性对 3 的 getter（string，默认 ""）。
		/// </summary>
		public string RegisterVisitor()
		{
			return this._FieldVisitor;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000024EC File Offset: 0x000006EC
		/// <summary>
		/// 属性对 3 的 setter。
		/// </summary>
		public void DeleteVisitor(string string_0)
		{
			this._FieldVisitor = string_0;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000024F5 File Offset: 0x000006F5
		/// <summary>
		/// 属性对 4 的 getter（bool，默认 true）。
		/// </summary>
		public bool AwakeVisitor()
		{
			return this.m_AuthenticationVisitor;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000024FD File Offset: 0x000006FD
		/// <summary>
		/// 属性对 4 的 setter。
		/// </summary>
		public void CompareVisitor(bool bool_0)
		{
			this.m_AuthenticationVisitor = bool_0;
		}

		// Token: 0x040000C5 RID: 197
		/// <summary>
		/// bool 属性对 1 后备字段（默认 true）。
		/// </summary>
		private bool _SchemaVisitor = true;

		// Token: 0x040000C6 RID: 198
		/// <summary>
		/// bool 属性对 2 后备字段（默认 true）。
		/// </summary>
		private bool m_BaseVisitor = true;

		// Token: 0x040000C7 RID: 199
		/// <summary>
		/// bool 属性对 4 后备字段（默认 true）。
		/// </summary>
		private bool m_AuthenticationVisitor = true;

		// Token: 0x040000C8 RID: 200
		/// <summary>
		/// string 属性对 3 后备字段（默认 ""）。
		/// </summary>
		private string _FieldVisitor = "";
	}
}
