using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace zmserv
{
	// Token: 0x0200000A RID: 10
	/// <summary>
	/// 「机房管理助手」（jfglzs）的后台守护服务本体，也是整个 zmserv 工程唯一的业务逻辑。
	/// 真实职责（恶意行为）：
	/// 1) 以 Windows 服务身份常驻（ServiceName = "Service1"，StartupObject 指向本类）；
	/// 2) 启动后等待 30 秒，然后每 5 秒轮询一次进程列表；
	/// 3) 受监视的是两个进程：写死的 "jfglzsn"（主程序），以及一个由"月 × 日"推导出来的
	///    5 位小写进程名（每天自动变一次，用来对抗按名字找进程的分析/破解）；
	/// 4) 只要这两个进程都不存在，就累加计数；累计满 16 次（≈80 秒）就强制关机：
	///    先 RtlAdjustPrivilege(19) 拿 SeShutdownPrivilege，再 NtShutdownSystem(2)，
	///    随后再补一枪 shutdown.exe /s /f /t 0；
	/// 5) 任一进程回来，计数清零。
	/// 换句话说：学生只要把助手关掉超过一分多钟，机器就被这个服务关机。
	/// </summary>
	[DesignerGenerated]
	public class ProcessMonitorService : ServiceBase
	{
		// Token: 0x06000025 RID: 37 RVA: 0x00002494 File Offset: 0x00000694
		/// <summary>
		/// 构造函数：先写入一个从未被读取的默认安装路径常量，
		/// 再调用 InitializeComponent() 建立组件容器并设置服务名。
		/// </summary>
		public ProcessMonitorService()
		{
			this.默认安装目录 = "C:\\windows\\jf";
			this.InitializeComponent();
		}

		// Token: 0x06000026 RID: 38
		/// <summary>
		/// ntdll!RtlAdjustPrivilege 的原生声明：调整本进程（或当前线程）的特权。
		/// 参数：特权号、是否启用(1=启用)、作用对象(0=当前线程)、接收原状态的 out 值。
		/// </summary>
		[DllImport("ntdll.dll")]
		private static extern int RtlAdjustPrivilege(int 特权号, int 启用标志, int 当前线程标志, ref int 原状态);

		// Token: 0x06000027 RID: 39
		/// <summary>
		/// ntdll!NtShutdownSystem 的原生声明：不经过 Win32 关机流程，直接让内核关机。
		/// 参数：1 = 重启，2 = 关机。
		/// </summary>
		[DllImport("ntdll.dll")]
		private static extern int NtShutdownSystem(int 动作码);

		// Token: 0x06000028 RID: 40 RVA: 0x000024B8 File Offset: 0x000006B8
		/// <summary>
		/// 服务启动回调。做三件事：
		/// 1) 按"月 × 日"算出当天的受监视进程名：先拼一版 4 个字母的（随后立刻被覆盖，是废代码，
		///    原样保留）；再用 VBMath.Randomize(月×日) 播种，取一个 0..300001 的随机数，
		///    把它按十进制逐位映射成 'i'..'r' 的 5 个字符，作为真正的受监视进程名；
		/// 2) 把名字连同 ".exe" 一起记下来；
		/// 3) 建 30 秒的启动等待计时器（AutoReset），到期后转入监视阶段。
		/// 注意：整个方法体在 checked 上下文里（VB 默认溢出检查），不能改成 unchecked。
		/// </summary>
		protected override void OnStart(string[] args)
		{
			checked
			{
				int 月日乘积 = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
				int 余7 = 月日乘积 % 7;
				int 余9 = 月日乘积 % 9;
				int 余5 = 月日乘积 % 5;
				int 余3 = 月日乘积 % 3;
				string 废弃进程名;
				if (月日乘积 % 2 == 0)
				{
					废弃进程名 = Conversions.ToString(Strings.Chr(97 + 余7)) + Conversions.ToString(Strings.Chr(98 + 余9)) + Conversions.ToString(Strings.Chr(101 + 余5)) + Conversions.ToString(Strings.Chr(99 + 余3));
				}
				else
				{
					废弃进程名 = Conversions.ToString(Strings.Chr(97 + 余9)) + Conversions.ToString(Strings.Chr(98 + 余7)) + Conversions.ToString(Strings.Chr(101 + 余3)) + Conversions.ToString(Strings.Chr(99 + 余5));
				}
				// 下面两行立刻被第 2 段生成的随机名覆盖，等于没起作用（保留原样，不做"清理"）
				this.受监视进程名 = 废弃进程名;
				this.受监视进程文件名 = 废弃进程名 + ".exe";
				VBMath.Randomize((double)月日乘积);
				long 随机数 = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
				string 进程名 = "";
				int 位数 = 1;
				// 十进制逐位取余 → Chr(余数 + 105)，即 'i'..'r'，拼够 5 位为止
				do
				{
					进程名 = Conversions.ToString(Strings.Chr((int)(随机数 % 10L + 105L))) + 进程名;
					随机数 /= 10L;
					位数++;
				}
				while (位数 <= 5);
				this.受监视进程名 = 进程名;
				this.受监视进程文件名 = 进程名 + ".exe";
				this.连续缺席次数 = 0L;
				this.启动等待计时器 = new Timer(30000.0);
				this.启动等待计时器.AutoReset = true;
				this.启动等待计时器.Elapsed += this.启动等待到期;
				this.启动等待计时器.Start();
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002674 File Offset: 0x00000874
		/// <summary>
		/// 启动等待计时器到期（只会有意义地命中一次）：如果主程序 "jfglzsn" 已经在跑，
		/// 就换成 5 秒周期的监视计时器，并把 30 秒计时器销毁。
		/// 若主程序一直没起来，这里就什么都不做（不关机），等下一次 30 秒到期再看。
		/// </summary>
		private void 启动等待到期(object sender, ElapsedEventArgs e)
		{
			if (Process.GetProcessesByName("jfglzsn").Any<Process>())
			{
				this.监视计时器 = new Timer(5000.0);
				this.监视计时器.AutoReset = true;
				this.监视计时器.Elapsed += this.监视周期检查;
				this.监视计时器.Start();
				this.启动等待计时器.Dispose();
				this.启动等待计时器 = null;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000026E8 File Offset: 0x000008E8
		/// <summary>
		/// 核心判定（每 5 秒一次）：受监视进程（当日随机名）与主程序 "jfglzsn" 都不在时，
		/// 连续缺席次数 +1；满 16 次（80 秒）就强制关机并追加执行 shutdown /s /f /t 0。
		/// 只要任一进程在场，计数立刻归零。
		/// </summary>
		private void 监视周期检查(object sender, ElapsedEventArgs e)
		{
			if (!Process.GetProcessesByName(this.受监视进程名).Any<Process>() && !Process.GetProcessesByName("jfglzsn").Any<Process>())
			{
				// VB 默认 checked：溢出会抛异常，这里保持 checked 语义
				this.连续缺席次数 = checked(this.连续缺席次数 + 1L);
				if (this.连续缺席次数 >= 16L)
				{
					ProcessMonitorService.ForceShutdown();
					Process.Start("shutdown", "/s /f /t 0");
					return;
				}
			}
			else
			{
				this.连续缺席次数 = 0L;
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002768 File Offset: 0x00000968
		/// <summary>
		/// 服务停止回调：停掉并释放两个计时器（两者都可能为 null，判空顺序与原版一致）。
		/// </summary>
		protected override void OnStop()
		{
			if (this.监视计时器 != null)
			{
				this.监视计时器.Stop();
				this.监视计时器.Dispose();
			}
			if (this.启动等待计时器 != null)
			{
				this.启动等待计时器.Stop();
				this.启动等待计时器.Dispose();
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000027B4 File Offset: 0x000009B4
		/// <summary>
		/// 立即关机：先取 SeShutdownPrivilege（19），再调 NtShutdownSystem(2)。
		/// 不等待、不提示、不给程序保存的机会。
		/// </summary>
		public static void ForceShutdown()
		{
			int 原状态 = 0;
			ProcessMonitorService.RtlAdjustPrivilege(19, 1, 0, ref 原状态);
			ProcessMonitorService.NtShutdownSystem(2);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000027D8 File Offset: 0x000009D8
		/// <summary>
		/// 立即重启：同样是先取 SeShutdownPrivilege(19)，再 NtShutdownSystem(1)。
		/// 工程内没有调用点，但作者显然留了这条路。
		/// </summary>
		public static void ForceRestart()
		{
			int 原状态 = 0;
			ProcessMonitorService.RtlAdjustPrivilege(19, 1, 0, ref 原状态);
			ProcessMonitorService.NtShutdownSystem(1);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000027FC File Offset: 0x000009FC
		/// <summary>
		/// 标准 ServiceBase 释放：先释放组件容器，再交给基类（finally 保证基类一定被调到）。
		/// </summary>
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

		// Token: 0x0600002F RID: 47 RVA: 0x000022BF File Offset: 0x000004BF
		/// <summary>
		/// 进程入口（csproj 的 StartupObject 就是本类）：把本服务交给 SCM 运行时。
		/// </summary>
		[MTAThread]
		public static void Main()
		{
			ServiceBase.Run(new ServiceBase[]
			{
				new ProcessMonitorService()
			});
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000022D4 File Offset: 0x000004D4
		/// <summary>
		/// 设计器生成的初始化（原名 LogoutParams）：建组件容器，设置服务名 "Service1"。
		/// 服务名写死，卸载/管理服务时按这个名字找。
		/// </summary>
		private void InitializeComponent()
		{
			this.组件容器 = new Container();
			base.ServiceName = "Service1";
		}

		// Token: 0x04000009 RID: 9
		/// <summary>5 秒周期的监视计时器（主程序起来之后才创建）。原名 _VisitorStrategy。</summary>
		private Timer 监视计时器;

		// Token: 0x0400000A RID: 10
		/// <summary>30 秒周期的启动等待计时器。原名 _ProducerStrategy。</summary>
		private Timer 启动等待计时器;

		// Token: 0x0400000B RID: 11
		/// <summary>监视周期 5000ms。原名 attrStrategy。注意：是常量，代码里并没有用它，构造 Timer 时是硬编码 30000.0/5000.0。</summary>
		private const int 监视间隔毫秒 = 5000;

		// Token: 0x0400000C RID: 12
		/// <summary>启动等待 30000ms。原名 _SingletonStrategy，同样未被引用。</summary>
		private const int 启动等待毫秒 = 30000;

		// Token: 0x0400000D RID: 13
		/// <summary>连续缺席次数，满 16 触发关机。原名 _OrderStrategy。</summary>
		private long 连续缺席次数;

		// Token: 0x0400000E RID: 14
		/// <summary>当天受监视的进程名（不含扩展名）。原名 _AttributeStrategy。</summary>
		private string 受监视进程名;

		// Token: 0x0400000F RID: 15
		/// <summary>当天受监视的进程文件名（含 .exe）。原名 m_MessageStrategy。工程内未被读取。</summary>
		private string 受监视进程文件名;

		// Token: 0x04000010 RID: 16
		/// <summary>SeShutdownPrivilege 的特权号 19。原名 m_PredicateStrategy，同样未被引用（调用处写死 19）。</summary>
		private const int 关机特权号 = 19;

		// Token: 0x04000011 RID: 17
		/// <summary>RtlAdjustPrivilege 的作用对象：0 = 当前线程。原名 m_ManagerStrategy。</summary>
		private const int 当前线程标志 = 0;

		// Token: 0x04000012 RID: 18
		/// <summary>RtlAdjustPrivilege 的启用标志：1 = 启用。原名 prototypeStrategy。</summary>
		private const int 启用特权标志 = 1;

		// Token: 0x04000013 RID: 19
		/// <summary>NtShutdownSystem 的关机动作码：2 = 关机。原名 parameterStrategy。</summary>
		private const int 关机动作码 = 2;

		// Token: 0x04000014 RID: 20
		/// <summary>默认安装目录 "C:\windows\jf"：构造时赋值，之后从未读取（壳/模板残留）。原名 _StubStrategy。</summary>
		private string 默认安装目录;

		// Token: 0x04000015 RID: 21
		/// <summary>WinForms/服务设计器用的组件容器。原名 _EventStrategy。</summary>
		private IContainer 组件容器;
	}
}
