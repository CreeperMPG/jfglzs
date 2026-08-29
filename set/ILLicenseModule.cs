using System;

		/// <summary>
		/// 壳授权模块占位（Eziriz IntelliLock 特征）：仅含两个返回常量的占位方法，无实际逻辑。
		/// 与 HelperMock 的 IntelliLock 提示、TestMock 的 {11111-22222-...} 特征 GUID 同属加壳器残留。
		/// </summary>
// Token: 0x02000012 RID: 18
internal class ILLicenseModule
{
		/// <summary>
		/// 占位方法（壳残留）：恒返回 true，无实际逻辑。
		/// </summary>
	// Token: 0x060000F8 RID: 248 RVA: 0x000021D1 File Offset: 0x000003D1
	internal static bool EnableClass()
	{
		return true;
	}

		/// <summary>
		/// 占位方法（壳残留）：恒返回 false，无实际逻辑。
		/// </summary>
	// Token: 0x060000F9 RID: 249 RVA: 0x000021C6 File Offset: 0x000003C6
	internal static bool StopClass()
	{
		return false;
	}
}
