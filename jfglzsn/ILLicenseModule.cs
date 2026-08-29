using System;

// Token: 0x02000014 RID: 20
	// ============================================================
	// ILLicenseModule.cs — 壳授权桩类（自动生成/混淆残留）
	// .NET Reactor / IntelliLock 的授权模块桩：InstantiateMethod 恒 true、CountMethod 恒 false，无实际逻辑。
	// ============================================================
internal class ILLicenseModule
{
	// Token: 0x060000D2 RID: 210 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>
		/// 壳授权桩：恒返回 true。
		/// </summary>
	internal static bool InstantiateMethod()
	{
		return true;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>
		/// 壳授权桩：恒返回 false。
		/// </summary>
	internal static bool CountMethod()
	{
		return false;
	}
}
