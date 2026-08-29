using System;

// Token: 0x0200000C RID: 12
// ILLicenseModule —— IntelliLock 许可模块空壳：仅含两个混淆填充方法（恒真/恒假）。
// 类名暗示其原为 IntelliLock 许可证模块（IL = IntelliLock），经脱壳后仅剩空壳，无任何逻辑。
internal class ILLicenseModule
{
	// Token: 0x06000032 RID: 50 RVA: 0x000021BC File Offset: 0x000003BC
	/// <summary>混淆填充：恒真，无业务意义（无调用方）。</summary>
	internal static bool ResetParser()
	{
		return true;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x000021BF File Offset: 0x000003BF
	/// <summary>混淆填充：恒假，无业务意义（无调用方）。</summary>
	internal static bool RestartParser()
	{
		return false;
	}
}
