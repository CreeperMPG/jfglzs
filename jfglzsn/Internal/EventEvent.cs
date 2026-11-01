using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

namespace jfglzsn.Internal
{
	// Token: 0x02000003 RID: 3
	/// <summary>
	/// VB.NET 模板生成的「My.Computer」替身类型（继承 <see cref="Computer"/>）。
	/// <para>
	/// 除了模板带来的默认构造之外，这个类里还多出两个恒返回常量的静态方法
	/// <see cref="InstantiateDefinition"/>（返回 <c>true</c>）和 <see cref="StopDefinition"/>（返回 <c>false</c>）。
	/// 它们不是业务逻辑，而是壳/混淆器插入的「判定桩」：与 ILLicenseModule 里的
	/// ListRule/InitRule 完全同构（一个恒真、一个恒假），作用是让反编译结果里出现假的授权/开关分支。
	/// 本工程没有任何地方引用它们，可以安全地视为噪声。
	/// </para>
	/// </summary>
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class EventEvent : Computer
	{
		// Token: 0x06000006 RID: 6 RVA: 0x000021E3 File Offset: 0x000003E3
		/// <summary>模板默认构造，无初始化逻辑。</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventEvent()
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>混淆器插入的判定桩，恒返回 <c>true</c>，工程内无引用。</summary>
		internal static bool InstantiateDefinition()
		{
			return true;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021E0 File Offset: 0x000003E0
		/// <summary>混淆器插入的判定桩，恒返回 <c>false</c>，工程内无引用。</summary>
		internal static bool StopDefinition()
		{
			return false;
		}
	}
}
