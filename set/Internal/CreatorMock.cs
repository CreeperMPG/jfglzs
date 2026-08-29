using System;
using System.Runtime.InteropServices;

namespace WindowsApplication1.Internal
{
		/// <summary>
		/// 混淆器标记 Attribute（元数据特性）：AttributeUsage 覆盖几乎所有目标且 AllowMultiple=true，
		/// 是加壳/混淆工具（IntelliLock/.NET Reactor/ConfuserEx 后处理）打标记用，本身无执行逻辑。
		/// </summary>
	// Token: 0x02000019 RID: 25
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	internal sealed class CreatorMock : Attribute
	{
		/// <summary>
		/// 读 FlagA（_m_flagA，默认 true）。
		/// </summary>
		// Token: 0x0600012F RID: 303 RVA: 0x0000261D File Offset: 0x0000081D
		public bool GetFlagA()
		{
			return this.m_flagA;
		}

		/// <summary>
		/// 写 FlagA。
		/// </summary>
		// Token: 0x06000130 RID: 304 RVA: 0x00002625 File Offset: 0x00000825
		public void SetFlagA(bool bool_0)
		{
			this.m_flagA = bool_0;
		}

		/// <summary>
		/// 读 FlagB（_m_flagB，默认 true）。
		/// </summary>
		// Token: 0x06000131 RID: 305 RVA: 0x0000262E File Offset: 0x0000082E
		public bool GetFlagB()
		{
			return this.m_flagB;
		}

		/// <summary>
		/// 写 FlagB。
		/// </summary>
		// Token: 0x06000132 RID: 306 RVA: 0x00002636 File Offset: 0x00000836
		public void SetFlagB(bool bool_0)
		{
			this.m_flagB = bool_0;
		}

		/// <summary>
		/// 读字符串属性（_m_flagText，默认 ""）。
		/// </summary>
		// Token: 0x06000133 RID: 307 RVA: 0x0000263F File Offset: 0x0000083F
		public string GetFlagText()
		{
			return this.m_flagText;
		}

		/// <summary>
		/// 写字符串属性。
		/// </summary>
		// Token: 0x06000134 RID: 308 RVA: 0x00002647 File Offset: 0x00000847
		public void SetFlagText(string string_0)
		{
			this.m_flagText = string_0;
		}

		/// <summary>
		/// 读 FlagC（_m_flagC，默认 true）。
		/// </summary>
		// Token: 0x06000135 RID: 309 RVA: 0x00002650 File Offset: 0x00000850
		public bool GetFlagC()
		{
			return this.m_flagC;
		}

		/// <summary>
		/// 写 FlagC。
		/// </summary>
		// Token: 0x06000136 RID: 310 RVA: 0x00002658 File Offset: 0x00000858
		public void SetFlagC(bool bool_0)
		{
			this.m_flagC = bool_0;
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 false，无实际逻辑。
		/// </summary>
		// Token: 0x06000138 RID: 312 RVA: 0x000021C6 File Offset: 0x000003C6
		internal static bool Placeholder_ReturnFalse()
		{
			return false;
		}

		/// <summary>
		/// FlagA 后备字段（默认 true）。
		/// </summary>
		// Token: 0x04000088 RID: 136
		private bool m_flagA = true;

		/// <summary>
		/// FlagB 后备字段（默认 true）。
		/// </summary>
		// Token: 0x04000089 RID: 137
		private bool m_flagB = true;

		/// <summary>
		/// FlagC 后备字段（默认 true）。
		/// </summary>
		// Token: 0x0400008A RID: 138
		private bool m_flagC = true;

		/// <summary>
		/// 字符串属性后备字段（默认空串）。
		/// </summary>
		// Token: 0x0400008B RID: 139
		private string m_flagText = "";
	}
}
