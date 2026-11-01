using System;

// Token: 0x0200000C RID: 12
/// <summary>
/// IntelliLock 授权模块的残留壳代码。
/// 真实职责：什么都没有——两个方法都只返回写死的常量，全工程没有任何调用点。
/// 它们原本是加壳工具注入的"授权校验"接口（方法名里的 Definition 是加壳器的
/// 随机词表，不含语义），真正的校验逻辑在解壳/脱壳过程中已被剥离。
/// </summary>
internal class ILLicenseModule
{
	// Token: 0x06000032 RID: 50 RVA: 0x000021BC File Offset: 0x000003BC
	/// <summary>
	/// 壳注入的空桩：固定返回 true，不读任何输入、无副作用、无调用点。
	/// </summary>
	internal static bool CollectDefinition()
	{
		return true;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x000021BF File Offset: 0x000003BF
	/// <summary>
	/// 壳注入的空桩：固定返回 false，无副作用、无调用点。
	/// </summary>
	internal static bool PrintDefinition()
	{
		return false;
	}
}
