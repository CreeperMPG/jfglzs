using System;

// Token: 0x0200000F RID: 15
/// <summary>
/// IntelliLock 许可证桩模块（空）：仅含两个恒值方法（DisableListener 恒 true、PublishListener 恒 false），
/// 无实际逻辑，属于加壳器（Eziriz IntelliLock）注入的占位代码。
/// </summary>
internal class ILLicenseModule
{
	// Token: 0x0600007A RID: 122 RVA: 0x000021C6 File Offset: 0x000003C6
	/// <summary>恒 true 桩，无实义。</summary>
	internal static bool DisableListener()
	{
		return true;
	}

	// Token: 0x0600007B RID: 123 RVA: 0x000021C9 File Offset: 0x000003C9
	/// <summary>恒 false 桩，无实义。</summary>
	internal static bool PublishListener()
	{
		return false;
	}
}
