// ============================================================================
// ILLicenseModule.cs — .NET Reactor 加壳器伪方法类（自动生成）
// 真实职责：.NET Reactor 加壳器注入的占位类（命名 ILLicenseModule，Reactor 命名特征），含两个恒值方法：
// SelectThread() 恒返回 true、SearchThread() 恒返回 false。这类方法通常被壳用来做许可证/完整性检查的
// "假分支"或反静态分析诱饵（调用点被壳替换为动态解密）。无业务逻辑、无恶意行为，但证实程序经
// .NET Reactor 加壳（与 gypZRX1vuWc3aeye0R/Internal/Object.cs 的 IntelliLock 弹窗呼应）。
// 说明：自动生成文件，按规则仅加注释不重命名。
// ============================================================================
﻿using System;

// Token: 0x02000014 RID: 20
internal class ILLicenseModule
{
	// Token: 0x060000AA RID: 170 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>Reactor 壳伪方法：恒返回 true（许可证/完整性检查假分支填充）。</summary>
	internal static bool SelectThread()
	{
		return true;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x000021C6 File Offset: 0x000003C6
/// <summary>Reactor 壳伪方法：恒返回 false（许可证/完整性检查假分支填充）。</summary>
	internal static bool SearchThread()
	{
		return false;
	}
}
