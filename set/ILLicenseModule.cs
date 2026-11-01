using System;

/// <summary>
/// IntelliLock（.NET Reactor 的前身/同系加密器）留在程序集里的「许可模块」桩类。
/// 两个方法都只是返回常量，没有调用点，属于加壳工具写入的占位代码，不参与任何真实逻辑。
/// </summary>
// Token: 0x02000012 RID: 18
internal class ILLicenseModule
{
	/// <summary>恒返回 true 的死代码桩，无调用点。</summary>
	// Token: 0x060000F7 RID: 247 RVA: 0x000021E5 File Offset: 0x000003E5
	internal static bool 恒真桩()
	{
		return true;
	}

	/// <summary>恒返回 false 的死代码桩，无调用点。</summary>
	// Token: 0x060000F8 RID: 248 RVA: 0x000021E8 File Offset: 0x000003E8
	internal static bool 恒假桩()
	{
		return false;
	}
}
