using System;
using System.Windows.Forms;

namespace WindowsApplication1.Internal
{
	/// <summary>
	/// IntelliLock 未注册提示的静态入口。首次调用时弹一个标题为 "www.eziriz.com" 的
	/// 未授权对话框；若弹窗本身抛异常（例如无桌面会话），退化为把一段
	/// javascript alert 交给 <see cref="CollectMapper"/>（而后者是空实现，什么也不做）。
	/// 本工程内没有任何调用点，属壳/授权工具残留。
	/// </summary>
	// Token: 0x02000014 RID: 20
	internal class ListenerMapper
	{
		// Token: 0x060000A6 RID: 166 RVA: 0x000073CC File Offset: 0x000055CC
		/// <summary>
		/// 显示 IntelliLock 未注册提示（只显示一次）。用静态布尔做幂等控制，
		/// 先置标志再弹窗，所以弹窗失败也不会重复尝试。
		/// </summary>
		internal static void 显示IntelliLock未注册提示()
		{
			if (!ListenerMapper.已显示过提示)
			{
				ListenerMapper.已显示过提示 = true;
				try
				{
					MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com");
				}
				catch
				{
					ListenerMapper.写入警告脚本占位("<script language='javascript'>alert( \"This assembly is protected by an unregistered version of IntelliLock!\" );</script>");
				}
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00002396 File Offset: 0x00000596
		/// <summary>
		/// 空实现：方法体为空。设计用途是在无桌面环境下把警告脚本注入宿主页面，
		/// 但原版没有实现，所以调用它等于什么都没发生。
		/// </summary>
		internal static void 写入警告脚本占位(object 脚本内容)
		{
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>恒返回 false 的桩方法，无调用点。</summary>
		internal static bool 返回假占位()
		{
			return false;
		}

		// Token: 0x04000038 RID: 56
		/// <summary>未注册提示是否已经显示过（幂等标志）。</summary>
		private static bool 已显示过提示;
	}
}
