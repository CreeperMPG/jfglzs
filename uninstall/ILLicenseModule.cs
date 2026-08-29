using System;

// Token: 0x02000011 RID: 17
// <summary>
/// IntelliLock（Eziriz）授权运行时注入的模块占位类，仅含两个恒值桩方法，
/// 佐证本程序集还经过 IntelliLock 授权保护。无恶意行为、无 IOC。
/// </summary>
internal class ILLicenseModule
{
	// Token: 0x06000067 RID: 103 RVA: 0x000021D1 File Offset: 0x000003D1
	/// <summary>授权运行时混淆桩：恒返回 true，无实际逻辑。</summary>
	internal static bool PostRegistry()
	{
		return true;
	}

	// Token: 0x06000068 RID: 104 RVA: 0x000021C6 File Offset: 0x000003C6
	/// <summary>授权运行时混淆桩：恒返回 false，无实际逻辑。</summary>
	internal static bool TestRegistry()
	{
		return false;
	}
}
