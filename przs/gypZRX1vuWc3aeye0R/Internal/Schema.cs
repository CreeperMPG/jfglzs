using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.VisualBasic.Devices;

// ============================================================================
// Schema.cs — My.Computer 对象（VB.NET 计算机信息访问）
// 真实职责：反混淆后为 My.Computer——继承 Microsoft.VisualBasic.Devices.Computer 的空子类
// （MyTemplate 生成，ConfuserEx 改名 Schema）。程序通过 Account.MyComputer 访问，实际使用点：
// bianhao.cs 取计算机名末 6 位。本身无任何逻辑；LogoutThread/CallThread 为混淆器填充方法。
// 说明：类名保持混淆原名，本文件仅加注释不重命名（自动生成 + 填充方法）。
// ============================================================================
namespace gypZRX1vuWc3aeye0R.Internal
{
	// Token: 0x02000003 RID: 3
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class Schema : Computer
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000021C9 File Offset: 0x000003C9
		[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>Schema 构造函数（空实现，My.Computer 子类）。</summary>
		public Schema()
		{
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>混淆器填充方法（恒返回 true，无意义）。</summary>
		internal static bool LogoutThread()
		{
			return true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021C6 File Offset: 0x000003C6
/// <summary>混淆器填充方法（恒返回 false，无意义）。</summary>
		internal static bool CallThread()
		{
			return false;
		}
	}
}
