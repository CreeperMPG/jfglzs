using System;

/// <summary>
/// IntelliLock 授权模块的静态桩。整个类型只有两个恒返回常量的方法，
/// 既不读注册表、不校验许可证，也不会被本程序集里任何代码调用
/// （全工程 grep 只有它自己的定义）。
/// 它和 <see cref="WindowsApplication1.Internal.ListenerMapper"/> 的未注册提示一起，
/// 是「样本曾用 IntelliLock 加壳/授权」留下的残骸，不参与真实授权判断。
/// </summary>
// Token: 0x0200000F RID: 15
internal class ILLicenseModule
{
	// Token: 0x06000079 RID: 121 RVA: 0x000021DD File Offset: 0x000003DD
	/// <summary>
	/// 恒返回 true 的授权桩。原版应该是 IntelliLock 生成的「许可证校验通过」占位实现，
	/// 反编译后只剩常量返回，无任何校验逻辑。
	/// </summary>
	internal static bool 返回真占位()
	{
		return true;
	}

	// Token: 0x0600007A RID: 122 RVA: 0x000021E8 File Offset: 0x000003E8
	/// <summary>
	/// 恒返回 false 的授权桩，与上面的方法成对出现，同样不含任何校验逻辑。
	/// </summary>
	internal static bool 返回假占位()
	{
		return false;
	}
}
