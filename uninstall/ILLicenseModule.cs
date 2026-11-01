using System;

// Token: 0x02000011 RID: 17
/// <summary>
/// 保护器（IntelliLock / .NET Reactor 一类）注入的「许可模块」占位类。
/// 两个方法都恒返回常量、不接受参数，全工程没有任何调用点，属于混淆器塞进来的噪声。
/// 保留它们只是为了保持类型表与原始样本一致。
/// </summary>
internal class ILLicenseModule
{
	// Token: 0x06000068 RID: 104 RVA: 0x000021DD File Offset: 0x000003DD
	/// <summary>
	/// 混淆器注入的占位方法：恒返回 true，无调用者，无副作用。
	/// 原混淆名：MoveParser
	/// </summary>
	internal static bool 混淆器占位方法_恒真()
	{
		return true;
	}

	// Token: 0x06000069 RID: 105 RVA: 0x000021E8 File Offset: 0x000003E8
	/// <summary>
	/// 混淆器注入的占位方法：恒返回 false，无调用者，无副作用。
	/// 原混淆名：ReadParser
	/// </summary>
	internal static bool 混淆器占位方法_恒假()
	{
		return false;
	}
}
