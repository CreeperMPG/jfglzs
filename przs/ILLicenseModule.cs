using System;

/// <summary>
/// .NET Reactor 的授权模块桩（类名 ILLicenseModule 是壳留下的名字）。
/// 两个方法恒返回 true / false、没有参数、也不看任何状态，本工程里没有调用者——
/// 它们是壳在"未注册版"里插入的许可证判断占位，解壳后原样残留。
/// </summary>
// Token: 0x02000014 RID: 20
internal class ILLicenseModule
{
	/// <summary>授权检查桩：恒返回 true（原名 RunTag）。</summary>
	// Token: 0x060000AA RID: 170 RVA: 0x000021DD File Offset: 0x000003DD
	internal static bool RunTag()
	{
		return true;
	}

	/// <summary>授权检查桩：恒返回 false（原名 CancelTag）。</summary>
	// Token: 0x060000AB RID: 171 RVA: 0x000021E8 File Offset: 0x000003E8
	internal static bool CancelTag()
	{
		return false;
	}
}
