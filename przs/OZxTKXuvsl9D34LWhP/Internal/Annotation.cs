using System;
using System.Windows.Forms;

namespace OZxTKXuvsl9D34LWhP.Internal
{
	/// <summary>
	/// IntelliLock 试用版水印（类名 Annotation 是混淆名）。
	/// 只在首次调用时弹一次"本程序集受未注册版 IntelliLock 保护"的提示框，
	/// 若 MessageBox 不可用（非交互环境）就退回把 alert 脚本塞给 <see cref="EnableWatcher"/>（那里是空实现）。
	/// 本工程里没有任何代码调用它，属于壳的残留。
	/// </summary>
	// Token: 0x02000019 RID: 25
	internal class Annotation
	{
		// Token: 0x060000D7 RID: 215 RVA: 0x00009DD8 File Offset: 0x00007FD8
		/// <summary>
		/// 弹一次"未注册版 IntelliLock"提示（原名 smethod_0）。
		/// <c>_Info</c> 保证只弹一次；MessageBox 失败时把一段 JS alert 交给 <see cref="EnableWatcher"/>。
		/// </summary>
		internal static void 显示未注册提示()
		{
			if (!Annotation._Info)
			{
				Annotation._Info = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					Annotation.EnableWatcher("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00002579 File Offset: 0x00000779
		/// <summary>空实现（原名 EnableWatcher）：壳里给"非 WinForms 环境"留的降级出口，什么都不做。</summary>
		internal static void EnableWatcher(object 提示内容)
		{
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>壳生成的授权标记桩函数，恒返回 false，无调用者（原名 NewTag）。</summary>
		internal static bool NewTag()
		{
			return false;
		}

		// Token: 0x04000068 RID: 104
		/// <summary>"是否已经弹过提示"标志（原名 _Info）。</summary>
		private static bool _Info;
	}
}
