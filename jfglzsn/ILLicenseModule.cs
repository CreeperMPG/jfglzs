using System;

// Token: 0x02000014 RID: 20
/// <summary>
/// 授权/试用判定的桩类型。两个方法都只有一句 <c>return 常量</c>，
/// 明显是被剥掉的授权检查外壳：调用方拿到的永远是同一种结果。
/// <para>
/// 注意这里的返回值方向：<see cref="ListRule"/> 恒为 <c>true</c>（判定通过），
/// <see cref="InitRule"/> 恒为 <c>false</c>（未初始化/非受限），
/// 与「破解版把授权门打开」的常见改法一致——说明样本已被处理成免授权运行。
/// </para>
/// </summary>
internal class ILLicenseModule
{
	// Token: 0x060000D4 RID: 212 RVA: 0x000021DD File Offset: 0x000003DD
	/// <summary>
	/// 授权校验桩，恒返回 <c>true</c>。原版应在此处读取注册码/试用状态，样本中已被抹成常量。
	/// </summary>
	/// <returns>恒为 <c>true</c>。</returns>
	internal static bool ListRule()
	{
		return true;
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x000021E0 File Offset: 0x000003E0
	/// <summary>
	/// 初始化/受限状态桩，恒返回 <c>false</c>。
	/// </summary>
	/// <returns>恒为 <c>false</c>。</returns>
	internal static bool InitRule()
	{
		return false;
	}
}
