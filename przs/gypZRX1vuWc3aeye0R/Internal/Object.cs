using System;
using System.Windows.Forms;

// ============================================================================
// Object.cs — IntelliLock 保护壳的试用期弹窗残留
// 真实职责：反混淆后为 .NET Reactor / IntelliLock 加壳器注入的"未注册试用版"提示代码。
// 弹试用提示 在首次调用时弹窗 "This assembly is protected by an unregistered version of \"IntelliLock\"!"
// （标题 www.eziriz.com，eziriz 即 IntelliLock/.NET Reactor 厂商）；MessageBox 失败则降级执行
// JS alert（JS弹窗降级接收 空实现）。只弹一次标志 保证只弹一次。
// 说明：类名保持混淆原名，方法/字段已按 Object.cs.md 映射表反混淆重命名。
// ============================================================================
namespace gypZRX1vuWc3aeye0R.Internal
{
	// Token: 0x02000019 RID: 25
	internal class Object
	{
		// Token: 0x060000D7 RID: 215 RVA: 0x0000A100 File Offset: 0x00008300
/// <summary>
/// 弹试用提示：1. 只弹一次标志 为 false 则置 true；2. MessageBox.Show("This assembly is protected by an
/// unregistered version of \"IntelliLock\"!", "www.eziriz.com")；3. 抛异常时调用 JS弹窗降级接收("<script...alert(...)</script>")。
/// </summary>
		internal static void 弹试用提示()
		{
			if (!Object.只弹一次标志)
			{
				Object.只弹一次标志 = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					Object.JS弹窗降级接收("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00002321 File Offset: 0x00000521
/// <summary>JS alert 降级接收：空方法（壳的浏览器弹窗回调，无实现）。</summary>
		internal static void JS弹窗降级接收(object object_0)
		{
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>混淆器填充方法（恒返回 true，无意义）。</summary>
		internal static bool RegisterThread()
		{
			return true;
		}

		// Token: 0x04000068 RID: 104
/// <summary>只弹一次标志：静态 bool，保证试用提示只弹一次。</summary>
		private static bool 只弹一次标志;
	}
}
