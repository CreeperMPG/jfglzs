using System;
using System.Runtime.InteropServices;

namespace jfglzsn.Internal
{
	// Token: 0x0200001B RID: 27
	/// <summary>
	/// 壳注入的标记属性（Attribute），可标注到程序集、类型、成员、参数等几乎所有目标上，
	/// 且 <c>AllowMultiple = true, Inherited = false</c>，同时 <c>[ComVisible(true)]</c>。
	/// <para>
	/// 它携带四份元数据：三个布尔标志（默认全为 <c>true</c>）和一个字符串（默认空串）。
	/// 全工程搜索确认：**没有任何位置真正把它当特性用过**，也没有任何代码 new 它，
	/// 因此它只是加壳/混淆阶段留下来的元数据容器，对运行时行为无影响。
	/// </para>
	/// <para>
	/// 命名说明：四个成员的读写方法名（SearchEvent / EnableEvent / NewEvent / ViewEvent /
	/// ManageEvent / InvokeEvent / PushEvent / SortEvent）由壳随机指派，指向的字段
	/// roleSetter / m_DecoratorSetter / _DefinitionSetter / _StateSetter 同样没有可辨识语义。
	/// 因为无法从代码里推出真实含义，这里**刻意保留原名**（遵循 AGENTS.md「沿用反编译出来的名字」），
	/// 只把纯占位参数名 <c>bool_0</c> / <c>string_0</c> 换成 <c>值</c>。硬套一个"看起来有意义"的名字
	/// 反而会误导后续分析。
	/// </para>
	/// </summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	internal sealed class RuleSetter : Attribute
	{
		// Token: 0x0600010B RID: 267 RVA: 0x000026BF File Offset: 0x000008BF
		/// <summary>读取布尔标志 roleSetter（默认 <c>true</c>）。语义未知，无调用方。</summary>
		public bool SearchEvent()
		{
			return this.roleSetter;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000026C7 File Offset: 0x000008C7
		/// <summary>写入布尔标志 roleSetter。语义未知，无调用方。</summary>
		/// <param name="值">要写入标志的值。</param>
		public void EnableEvent(bool 值)
		{
			this.roleSetter = 值;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000026D0 File Offset: 0x000008D0
		/// <summary>读取布尔标志 m_DecoratorSetter（默认 <c>true</c>）。语义未知，无调用方。</summary>
		public bool NewEvent()
		{
			return this.m_DecoratorSetter;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000026D8 File Offset: 0x000008D8
		/// <summary>写入布尔标志 m_DecoratorSetter。语义未知，无调用方。</summary>
		/// <param name="值">要写入标志的值。</param>
		public void ViewEvent(bool 值)
		{
			this.m_DecoratorSetter = 值;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000026E1 File Offset: 0x000008E1
		/// <summary>读取字符串标志 _StateSetter（默认空串）。语义未知，无调用方。</summary>
		public string ManageEvent()
		{
			return this._StateSetter;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000026E9 File Offset: 0x000008E9
		/// <summary>写入字符串标志 _StateSetter。语义未知，无调用方。</summary>
		/// <param name="值">要写入的字符串，原版调用方均传空串。</param>
		public void InvokeEvent(string 值)
		{
			this._StateSetter = 值;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000026F2 File Offset: 0x000008F2
		/// <summary>读取布尔标志 _DefinitionSetter（默认 <c>true</c>）。语义未知，无调用方。</summary>
		public bool PushEvent()
		{
			return this._DefinitionSetter;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000026FA File Offset: 0x000008FA
		/// <summary>写入布尔标志 _DefinitionSetter。语义未知，无调用方。</summary>
		/// <param name="值">要写入标志的值。</param>
		public void SortEvent(bool 值)
		{
			this._DefinitionSetter = 值;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>混淆器插入的判定桩，恒返回 <c>true</c>，工程内无引用。</summary>
		internal static bool InterruptRule()
		{
			return true;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000021E0 File Offset: 0x000003E0
		/// <summary>混淆器插入的判定桩，恒返回 <c>false</c>，工程内无引用。</summary>
		internal static bool MoveRule()
		{
			return false;
		}

		// Token: 0x040000C5 RID: 197
		/// <summary>布尔标志，默认 <c>true</c>；由 SearchEvent/EnableEvent 读写。</summary>
		private bool roleSetter = true;

		// Token: 0x040000C6 RID: 198
		/// <summary>布尔标志，默认 <c>true</c>；由 NewEvent/ViewEvent 读写。</summary>
		private bool m_DecoratorSetter = true;

		// Token: 0x040000C7 RID: 199
		/// <summary>布尔标志，默认 <c>true</c>；由 PushEvent/SortEvent 读写。</summary>
		private bool _DefinitionSetter = true;

		// Token: 0x040000C8 RID: 200
		/// <summary>字符串标志，默认空串；由 ManageEvent/InvokeEvent 读写。</summary>
		private string _StateSetter = "";
	}
}
