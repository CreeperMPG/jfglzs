using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using OZxTKXuvsl9D34LWhP.Internal;

/// <summary>
/// 主窗体，也是全部管控行为的总调度器。窗体本身隐藏运行：
/// 无边框、最小化、不显示在任务栏，只在托盘留"学生机房管理助手"图标，
/// 标题先伪装成 <c>word</c>，自我迁居后改成随机 3 个字符。
/// <para>它同时负责：</para>
/// <list type="bullet">
/// <item>把自身复制到 <c>C:\Program Files (x86)\&lt;随机4字&gt;\&lt;随机5字&gt;.exe</c>，
///   加隐藏+系统属性，启动副本后结束当前进程；</item>
/// <item>7 个 Timer 轮询：守护 <c>jfglzsn</c> 进程、恢复 hosts、强制写回
///   "禁止下载/禁止游戏"注册表项、弹警告窗；</item>
/// <item>FileSystemWatcher 监控下载/桌面/各盘目录，按扩展名删文件、按路径杀进程，
///   并检测 <c>MZ</c> 文件头防止改名绕过。</item>
/// </list>
/// 所有事件处理器原本都是 VB 的 <c>On Error Resume Next</c>：
/// 反编译里的 <c>ProjectData.ClearProjectError()</c>、<c>num2 = 2/3/4…</c>、
/// <c>catch when (endfilter(...))</c>、状态号分派表和
/// <c>throw ProjectData.CreateProjectError(-2146828237)</c> 都是编译器噪声。
/// 下面把每条可能出错的语句单独包进空 <c>catch</c>，保留"出错就丢掉、继续下一条"的语义。
/// </summary>
// Token: 0x0200000C RID: 12
[DesignerGenerated]
public class Form1 : Form
{
	/// <summary>构造主窗体，挂接 Load 事件并初始化隐藏窗口与全部定时器。</summary>
	// Token: 0x06000042 RID: 66 RVA: 0x00002CB8 File Offset: 0x00000EB8
	public Form1()
	{
		base.Load += this.窗体加载_初始化并自启动;
		this._Reponse = 0L;
		this.worker = 0;
		this.高危文件删除计数 = 0;
		this.数据目录 = "C:\\windows\\jf";
		this.hosts路径 = "C:\\Windows\\System32\\drivers\\etc\\hosts";
		this._Attribute = 4194304L;
		this.初始化组件();
	}

	/// <summary>
	/// 主定时器（原名 FlushVal，Timer_main.Tick，10 秒）：
	/// 先启动看护定时器；读 <c>games_jianche</c>，为 <c>on</c> 就启用游戏限制定时器；
	/// 读 <c>bianhao</c>，为 <c>on</c> 就显示右上角机号窗；最后把自己停掉。
	/// 原版逐条忽略错误。
	/// </summary>
	// Token: 0x06000043 RID: 67 RVA: 0x00002D28 File Offset: 0x00000F28
	private void 定时器_应用开关设置(object sender, EventArgs e)
	{
		try
		{
			this.Timer1.Enabled = true;
		}
		catch
		{
		}
		string 游戏检查 = "";
		try
		{
			游戏检查 = Conversions.ToString(Proccesor.读取注册表值(Registry.CurrentUser, "Software\\jfglzs", "games_jianche", ""));
		}
		catch
		{
		}
		if (Operators.CompareString(游戏检查, "on", false) == 0)
		{
			try
			{
				this.Timer_youxi.Enabled = true;
			}
			catch
			{
			}
		}
		string 编号开关 = "";
		try
		{
			编号开关 = Conversions.ToString(Proccesor.读取注册表值(Registry.CurrentUser, "Software\\jfglzs", "bianhao", ""));
		}
		catch
		{
		}
		if (Operators.CompareString(编号开关, "on", false) == 0)
		{
			try
			{
				Queue.MyForms.bianhao.Show();
			}
			catch
			{
			}
		}
		try
		{
			this.Timer_main.Enabled = false;
		}
		catch
		{
		}
	}

	/// <summary>
	/// 提示定时器（原名 ConnectVal，Timer2.Tick，1 秒）：
	/// Form2（警告/锁屏窗）没显示就强制显示，让它常驻最前。
	/// </summary>
	// Token: 0x06000044 RID: 68 RVA: 0x000023E7 File Offset: 0x000005E7
	private void 定时器_显示提示窗体(object sender, EventArgs e)
	{
		if (!Queue.MyForms.Form2.Visible)
		{
			Queue.MyForms.Form2.Show();
		}
	}

	/// <summary>
	/// 看护定时器（原名 ValidateVal，Timer1.Tick，3 秒）：
	/// <c>jfglzsn</c> 没在跑就累计次数；程序目录缺 <c>jfglzsn.exe</c> 时
	/// 从 <c>C:\windows\jf\jfglzsn.dat</c> 释放一份，再启动；
	/// 累计 40 次就改 Form2 文案"进程j被多次终止……"并启动提示定时器。
	/// 原版逐条忽略错误。
	/// </summary>
	// Token: 0x06000045 RID: 69 RVA: 0x00002E70 File Offset: 0x00001070
	private void 定时器_看护守护进程(object sender, EventArgs e)
	{
		bool 守护进程在运行 = false;
		try
		{
			守护进程在运行 = this.进程是否在运行("jfglzsn");
		}
		catch
		{
		}
		if (守护进程在运行)
		{
			return;
		}
		try
		{
			checked
			{
				this.子进程被杀次数 += 1L;
			}
		}
		catch
		{
		}
		bool 程序文件存在 = true;
		try
		{
			程序文件存在 = File.Exists(Conversions.ToString(Operators.ConcatenateObject(Proccesor.程序所在目录, "\\jfglzsn.exe")));
		}
		catch
		{
		}
		if (!程序文件存在)
		{
			bool 备份文件存在 = false;
			try
			{
				备份文件存在 = File.Exists(this.数据目录 + "\\jfglzsn.dat");
			}
			catch
			{
			}
			if (备份文件存在)
			{
				try
				{
					File.Copy(this.数据目录 + "\\jfglzsn.dat", Conversions.ToString(Operators.ConcatenateObject(Proccesor.程序所在目录, "\\jfglzsn.exe")), true);
				}
				catch
				{
				}
			}
		}
		try
		{
			NewLateBinding.LateCall(null, typeof(Process), "Start", new object[] { Operators.ConcatenateObject(Proccesor.程序所在目录, "\\jfglzsn.exe") }, null, null, null, true);
		}
		catch
		{
		}
		if (this.子进程被杀次数 < 40L)
		{
			return;
		}
		try
		{
			Queue.MyForms.Form2.shuoming.Text = "进程j被多次终止或文件会被杀毒软件（如win10自带的defender）清除！";
		}
		catch
		{
		}
		try
		{
			Queue.MyForms.Form2.Show();
		}
		catch
		{
		}
		try
		{
			this.Timer2.Enabled = true;
		}
		catch
		{
		}
	}

	/// <summary>
	/// hosts 定时器（原名 PublishVal，Timer_jianchahosts.Tick，15 秒）：
	/// <list type="bullet">
	/// <item>hosts 存在：若正在提示就关掉 Form3，清提示标志，结束；</item>
	/// <item>hosts 不存在：被删次数 +1、置提示标志；有 <c>hosts2</c> 备份就复制回来，
	///   把 Form3 改成"hosts文件被删除！现在恢复。"并显示；</item>
	/// <item>被删超过 5 次：关 Form3，改 Form2 两行文案"hosts文件多次被删除或备份文件不存在！"，
	///   显示 Form2 并启用提示定时器。</item>
	/// </list>
	/// 原版逐条忽略错误。
	/// </summary>
	// Token: 0x06000046 RID: 70 RVA: 0x00003044 File Offset: 0x00001244
	private void 定时器_恢复hosts(object sender, EventArgs e)
	{
		bool hosts存在 = false;
		try
		{
			hosts存在 = File.Exists(this.hosts路径);
		}
		catch
		{
		}
		if (hosts存在)
		{
			if (this.正在提示hosts被删)
			{
				try
				{
					Queue.MyForms.Form3.Close();
				}
				catch
				{
				}
			}
			try
			{
				this.正在提示hosts被删 = false;
			}
			catch
			{
			}
			return;
		}
		try
		{
			checked
			{
				this.hosts被删次数 += 1L;
			}
		}
		catch
		{
		}
		try
		{
			this.正在提示hosts被删 = true;
		}
		catch
		{
		}
		bool 备份存在 = false;
		try
		{
			备份存在 = File.Exists(this.hosts路径 + "2");
		}
		catch
		{
		}
		if (备份存在)
		{
			try
			{
				File.Copy(this.hosts路径 + "2", this.hosts路径, true);
			}
			catch
			{
			}
			try
			{
				Queue.MyForms.Form3.shuoming.Text = "hosts文件被删除！现在恢复。";
			}
			catch
			{
			}
			try
			{
				Queue.MyForms.Form3.Show();
			}
			catch
			{
			}
		}
		if (this.hosts被删次数 <= 5L)
		{
			return;
		}
		try
		{
			Queue.MyForms.Form3.Close();
		}
		catch
		{
		}
		try
		{
			Queue.MyForms.Form2.shuoming.Text = "hosts文件多次被删除或备份文件不存在！";
		}
		catch
		{
		}
		try
		{
			Queue.MyForms.Form2.Show();
		}
		catch
		{
		}
		try
		{
			this.Timer2.Enabled = true;
		}
		catch
		{
		}
	}

	/// <summary>
	/// 下载限制定时器（原名 TestVal，Timer_jzxz.Tick，5 秒）：
	/// 检查 IE 区域 3 的 <c>1803</c>、Chrome 与 Edge 的 <c>DownloadRestrictions</c>，
	/// 三者都为 <c>"3"</c> 才算正常；否则累计次数：
	/// <list type="bullet">
	/// <item>超过 15 次：关 Form3，Form2 报"你多次修改了注册表的禁止下载项！/请重新启动电脑！"；</item>
	/// <item>否则强制写回 12 个策略键，杀掉 chrome/msedge/firefox/iexplorer/explorer，
	///   重新启动 explorer.exe，再弹 Form3"你违规修改了注册表的禁止下载项！/现在助手强制恢复！"。</item>
	/// </list>
	/// 原版逐条忽略错误。
	/// </summary>
	// Token: 0x06000047 RID: 71 RVA: 0x0000323C File Offset: 0x0000143C
	private void 定时器_强制恢复下载限制(object sender, EventArgs e)
	{
		object 下载区域值 = null;
		try
		{
			下载区域值 = RuntimeHelpers.GetObjectValue(Proccesor.读取注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", ""));
		}
		catch
		{
		}
		object Chrome下载限制 = null;
		try
		{
			Chrome下载限制 = RuntimeHelpers.GetObjectValue(Proccesor.读取注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", ""));
		}
		catch
		{
		}
		object Edge下载限制 = null;
		try
		{
			Edge下载限制 = RuntimeHelpers.GetObjectValue(Proccesor.读取注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", ""));
		}
		catch
		{
		}
		bool 策略被改 = false;
		try
		{
			策略被改 = Conversions.ToBoolean(Operators.OrObject(Operators.OrObject(Operators.CompareObjectNotEqual(下载区域值, "3", false), Operators.CompareObjectNotEqual(Chrome下载限制, "3", false)), Operators.CompareObjectNotEqual(Edge下载限制, "3", false)));
		}
		catch
		{
		}
		if (!策略被改)
		{
			if (this.正在提示注册表被改)
			{
				try
				{
					Queue.MyForms.Form3.Close();
				}
				catch
				{
				}
			}
			try
			{
				this.正在提示注册表被改 = false;
			}
			catch
			{
			}
			return;
		}
		try
		{
			checked
			{
				this.注册表被改次数 += 1L;
			}
		}
		catch
		{
		}
		if (this.注册表被改次数 > 15L)
		{
			try
			{
				Queue.MyForms.Form3.Close();
			}
			catch
			{
			}
			try
			{
				Queue.MyForms.Form2.shuoming.Text = "你多次修改了注册表的禁止下载项！";
			}
			catch
			{
			}
			try
			{
				Queue.MyForms.Form2.shuoming2.Text = "请重新启动电脑！";
			}
			catch
			{
			}
			try
			{
				Queue.MyForms.Form2.Show();
			}
			catch
			{
			}
			try
			{
				this.Timer2.Enabled = true;
			}
			catch
			{
			}
			return;
		}
		try
		{
			Proccesor.写入注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 3, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 1, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 1, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 3, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 0, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 3, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 0, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 1, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 1, RegistryValueKind.DWord);
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
		}
		catch
		{
		}
		Process[] 进程数组 = null;
		try
		{
			进程数组 = Process.GetProcessesByName("chrome");
		}
		catch
		{
		}
		if (进程数组 != null)
		{
			foreach (Process 进程 in 进程数组)
			{
				try
				{
					进程.Kill();
				}
				catch
				{
				}
			}
		}
		Process[] 进程数组2 = null;
		try
		{
			进程数组2 = Process.GetProcessesByName("msedge");
		}
		catch
		{
		}
		if (进程数组2 != null)
		{
			foreach (Process 进程2 in 进程数组2)
			{
				try
				{
					进程2.Kill();
				}
				catch
				{
				}
			}
		}
		Process[] 进程数组3 = null;
		try
		{
			进程数组3 = Process.GetProcessesByName("firefox");
		}
		catch
		{
		}
		if (进程数组3 != null)
		{
			foreach (Process 进程3 in 进程数组3)
			{
				try
				{
					进程3.Kill();
				}
				catch
				{
				}
			}
		}
		Process[] 进程数组4 = null;
		try
		{
			进程数组4 = Process.GetProcessesByName("iexplorer");
		}
		catch
		{
		}
		if (进程数组4 != null)
		{
			foreach (Process 进程4 in 进程数组4)
			{
				try
				{
					进程4.Kill();
				}
				catch
				{
				}
			}
		}
		Process[] 进程数组5 = null;
		try
		{
			进程数组5 = Process.GetProcessesByName("explorer");
		}
		catch
		{
		}
		if (进程数组5 != null)
		{
			foreach (Process 进程5 in 进程数组5)
			{
				try
				{
					进程5.Kill();
				}
				catch
				{
				}
			}
		}
		try
		{
			Process.Start("explorer");
		}
		catch
		{
		}
		try
		{
			this.正在提示注册表被改 = true;
		}
		catch
		{
		}
		try
		{
			Queue.MyForms.Form3.shuoming.Text = "你违规修改了注册表的禁止下载项！";
		}
		catch
		{
		}
		try
		{
			Queue.MyForms.Form3.shuoming2.Text = "现在助手强制恢复！";
		}
		catch
		{
		}
		try
		{
			Queue.MyForms.Form3.Show();
		}
		catch
		{
		}
	}

	/// <summary>
	/// 给指定目录挂 <see cref="FileSystemWatcher"/>（原名 ManageVal）：
	/// 过滤 <c>*.*</c>，桌面递归、其它目录不递归；读 <c>gaowei</c>（拦截模式）和
	/// <c>xiazai</c>（下载拦截设置）后，按条件挂 Renamed / Created / Changed。
	/// 注意：watcher 是局部变量，方法返回后没有任何字段持有它（原版行为如此）。
	/// </summary>
	/// <param name="目录路径">要监控的目录。</param>
	// Token: 0x06000048 RID: 72 RVA: 0x00003818 File Offset: 0x00001A18
	private void 监听目录(string 目录路径)
	{
		if (Directory.Exists(目录路径))
		{
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
			fileSystemWatcher.Path = 目录路径;
			fileSystemWatcher.Filter = "*.*";
			if (Operators.CompareString(目录路径, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), false) == 0)
			{
				fileSystemWatcher.IncludeSubdirectories = true;
			}
			else
			{
				fileSystemWatcher.IncludeSubdirectories = false;
			}
			fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
			this.拦截模式 = Conversions.ToInteger(Proccesor.读取注册表值(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 2));
			this.下载拦截设置 = Conversions.ToString(Proccesor.读取注册表值(Registry.CurrentUser, "Software\\jfglzs", "xiazai", "off"));
			if (Operators.CompareString(this.下载拦截设置, "off", false) == 0)
			{
				fileSystemWatcher.Renamed += this.文件重命名_拦截高危类型;
			}
			if (this.拦截模式 != 1)
			{
				fileSystemWatcher.Created += this.文件创建_拦截高危类型;
				fileSystemWatcher.Changed += this.文件创建_拦截高危类型;
			}
			fileSystemWatcher.EnableRaisingEvents = true;
		}
	}

	/// <summary>
	/// 游戏限制定时器（原名 InvokeVal，Timer_youxi.Tick，30 秒）：
	/// Chrome 的 <c>AllowDinosaurEasterEgg</c>、Edge 的 <c>AllowSurfGame</c>
	/// 任一不为 0，就都写回 0，杀掉 chrome/msedge，弹 Form3
	/// "你违规修改了注册表的禁止游戏项！/现在助手强制恢复！"；
	/// 两项都正常时关 Form3 并清标志。原版逐条忽略错误。
	/// </summary>
	// Token: 0x06000049 RID: 73 RVA: 0x00003914 File Offset: 0x00001B14
	private void 定时器_强制恢复游戏限制(object sender, EventArgs e)
	{
		int Chrome恐龙彩蛋 = 0;
		try
		{
			Chrome恐龙彩蛋 = Conversions.ToInteger(Proccesor.读取注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "AllowDinosaurEasterEgg", ""));
		}
		catch
		{
		}
		int Edge冲浪游戏 = 0;
		try
		{
			Edge冲浪游戏 = Conversions.ToInteger(Proccesor.读取注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "AllowSurfGame", ""));
		}
		catch
		{
		}
		if ((Chrome恐龙彩蛋 != 0) | (Edge冲浪游戏 != 0))
		{
			try
			{
				Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "AllowDinosaurEasterEgg", 0, RegistryValueKind.DWord);
			}
			catch
			{
			}
			try
			{
				Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "AllowSurfGame", 0, RegistryValueKind.DWord);
			}
			catch
			{
			}
			Process[] 进程数组 = null;
			try
			{
				进程数组 = Process.GetProcessesByName("chrome");
			}
			catch
			{
			}
			if (进程数组 != null)
			{
				foreach (Process 进程 in 进程数组)
				{
					try
					{
						进程.Kill();
					}
					catch
					{
					}
				}
			}
			Process[] 进程数组2 = null;
			try
			{
				进程数组2 = Process.GetProcessesByName("msedge");
			}
			catch
			{
			}
			if (进程数组2 != null)
			{
				foreach (Process 进程2 in 进程数组2)
				{
					try
					{
						进程2.Kill();
					}
					catch
					{
					}
				}
			}
			try
			{
				this.正在提示游戏项被改 = true;
			}
			catch
			{
			}
			try
			{
				Queue.MyForms.Form3.shuoming.Text = "你违规修改了注册表的禁止游戏项！";
			}
			catch
			{
			}
			try
			{
				Queue.MyForms.Form3.shuoming2.Text = "现在助手强制恢复！";
			}
			catch
			{
			}
			try
			{
				Queue.MyForms.Form3.Show();
			}
			catch
			{
			}
			return;
		}
		if (this.正在提示游戏项被改)
		{
			try
			{
				Queue.MyForms.Form3.Close();
			}
			catch
			{
			}
		}
		try
		{
			this.正在提示游戏项被改 = false;
		}
		catch
		{
		}
	}

	/// <summary>
	/// 文件重命名拦截（原名 PrepareVal，Watcher.Renamed）：
	/// 扩展名命中 zip/rar/7z/arj/exe/com/msi/reg/bat/cmd/vbs/vbe/scr/dll/ps1/pif
	/// 就删除、Sleep 200，连做 3 次，然后弹 Form3 三行警告并显示。
	/// 跨线程时先 BeginInvoke 回 UI 线程。原版逐条忽略错误。
	/// </summary>
	/// <param name="sender">事件源。</param>
	/// <param name="e">重命名事件参数。</param>
	// Token: 0x0600004A RID: 74 RVA: 0x00003B8C File Offset: 0x00001D8C
	private void 文件重命名_拦截高危类型(object sender, RenamedEventArgs e)
	{
		if (base.InvokeRequired)
		{
			try
			{
				Action<object, RenamedEventArgs> action = new Action<object, RenamedEventArgs>(this.文件重命名_拦截高危类型);
				base.BeginInvoke(action, new object[] { sender, e });
			}
			catch
			{
			}
			return;
		}
		string 扩展名 = "";
		try
		{
			扩展名 = Path.GetExtension(e.Name).ToLower();
		}
		catch
		{
		}
		if (!((Operators.CompareString(扩展名, ".zip", false) == 0) | (Operators.CompareString(扩展名, ".rar", false) == 0) | (Operators.CompareString(扩展名, ".7z", false) == 0) | (Operators.CompareString(扩展名, ".arj", false) == 0) | (Operators.CompareString(扩展名, ".exe", false) == 0) | (Operators.CompareString(扩展名, ".com", false) == 0) | (Operators.CompareString(扩展名, ".msi", false) == 0) | (Operators.CompareString(扩展名, ".reg", false) == 0) | (Operators.CompareString(扩展名, ".bat", false) == 0) | (Operators.CompareString(扩展名, ".cmd", false) == 0) | (Operators.CompareString(扩展名, ".vbs", false) == 0) | (Operators.CompareString(扩展名, ".vbe", false) == 0) | (Operators.CompareString(扩展名, ".scr", false) == 0) | (Operators.CompareString(扩展名, ".dll", false) == 0) | (Operators.CompareString(扩展名, ".ps1", false) == 0) | (Operators.CompareString(扩展名, ".pif", false) == 0)))
		{
			return;
		}
		int 删除次数 = 1;
		checked
		{
			do
			{
				try
				{
					File.Delete(e.FullPath);
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
				删除次数++;
			}
			while (删除次数 <= 3);
		}
		try
		{
			Queue.MyForms.Form3.shuoming.Text = "本电脑禁止下载安装软件，再次操作会锁屏！";
		}
		catch
		{
		}
		try
		{
			Queue.MyForms.Form3.shuoming2.Text = "如果确有需要，请老师来退出小助手。";
		}
		catch
		{
		}
		try
		{
			Queue.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
		}
		catch
		{
		}
		try
		{
			Queue.MyForms.Form3.Show();
		}
		catch
		{
		}
	}

	/// <summary>
	/// 文件创建/修改拦截（原名 CloneVal，Watcher.Created / Changed），最狠的一支：
	/// <list type="bullet">
	/// <item><c>.exe</c> / <c>.com</c>：同名 <c>.cpp</c> / <c>.cp</c> 不存在且拦截模式为 2 或 0 时，
	///   按无扩展名文件名找进程，主模块路径与事件路径一致就 Kill；随后 Sleep 500、
	///   去掉属性、删除；文件还在就整轮重试（死循环）；成功后弹 Form3。</item>
	/// <item><c>.reg .bat .cmd .vbs .vbe .scr .dll .ps1 .pif</c>：去属性、删除、Sleep 200，连做 3 次；计数 +1 并弹 Form3。</item>
	/// <item><c>.msi</c>：路径含 <c>config.msi</c> 就放过（Windows Installer 临时目录），否则同样连删 3 次。</item>
	/// <item>其它扩展名：Sleep 500 等写完，读前两字节，<c>MZ</c>（77/90）就按伪装 PE 处理：
	///   杀同名进程、去属性、删除、Sleep 200，还在就重试；成功后弹 Form3。</item>
	/// </list>
	/// 开头 <c>new FileInfo(e.FullPath)</c> 构造后丢弃，原版如此，保留。
	/// 原版逐条忽略错误，且整个方法在 <c>checked</c> 上下文里。
	/// </summary>
	/// <param name="sender">事件源。</param>
	/// <param name="e">文件系统事件参数。</param>
	// Token: 0x0600004B RID: 75 RVA: 0x00003E64 File Offset: 0x00002064
	private void 文件创建_拦截高危类型(object sender, FileSystemEventArgs e)
	{
		checked
		{
			try
			{
				new FileInfo(e.FullPath);
			}
			catch
			{
			}
			if (base.InvokeRequired)
			{
				try
				{
					Action<object, FileSystemEventArgs> action = new Action<object, FileSystemEventArgs>(this.文件创建_拦截高危类型);
					base.BeginInvoke(action, new object[] { sender, e });
				}
				catch
				{
				}
				return;
			}
			string 扩展名 = "";
			try
			{
				扩展名 = Path.GetExtension(e.Name).ToLower();
			}
			catch
			{
			}
			if ((Operators.CompareString(扩展名, ".exe", false) == 0) | (Operators.CompareString(扩展名, ".com", false) == 0))
			{
				object 同名源码路径 = null;
				try
				{
					if (Operators.CompareString(扩展名, ".exe", false) == 0)
					{
						同名源码路径 = Strings.Replace(e.FullPath, ".exe", ".cpp", 1, -1, CompareMethod.Binary);
					}
					else
					{
						同名源码路径 = Strings.Replace(e.FullPath, ".com", ".cp", 1, -1, CompareMethod.Binary);
					}
				}
				catch
				{
				}
				string 无扩展名文件名 = "";
				try
				{
					无扩展名文件名 = Path.GetFileNameWithoutExtension(e.FullPath);
				}
				catch
				{
				}
				bool 应删除 = false;
				try
				{
					应删除 = ((!File.Exists(Conversions.ToString(同名源码路径)) & (this.拦截模式 == 2)) | (this.拦截模式 == 0));
				}
				catch
				{
				}
				if (应删除)
				{
					for (;;)
					{
						Process[] 进程数组 = null;
						try
						{
							进程数组 = Process.GetProcessesByName(无扩展名文件名);
						}
						catch
						{
						}
						if (进程数组 != null)
						{
							for (int 索引 = 0; 索引 < 进程数组.Length; 索引++)
							{
								try
								{
									if (Operators.CompareString(进程数组[索引].MainModule.FileName.ToLower(), e.FullPath.ToLower(), false) == 0)
									{
										try
										{
											进程数组[索引].Kill();
										}
										catch
										{
										}
										break;
									}
								}
								catch
								{
								}
							}
						}
						try
						{
							Thread.Sleep(500);
						}
						catch
						{
						}
						try
						{
							File.SetAttributes(e.FullPath, FileAttributes.Normal);
						}
						catch
						{
						}
						try
						{
							File.Delete(e.FullPath);
						}
						catch
						{
						}
						bool 文件还在 = true;
						try
						{
							文件还在 = File.Exists(e.FullPath);
						}
						catch
						{
						}
						if (!文件还在)
						{
							try
							{
								Queue.MyForms.Form3.shuoming.Text = "本电脑禁止创建新的exe文件，再次操作会锁屏！";
							}
							catch
							{
							}
							try
							{
								Queue.MyForms.Form3.shuoming2.Text = "老师请先退出小助手，重新安装软件。你也可重新设置：允许（不建议）";
							}
							catch
							{
							}
							try
							{
								Queue.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
							}
							catch
							{
							}
							try
							{
								Queue.MyForms.Form3.Show();
							}
							catch
							{
							}
							break;
						}
					}
				}
				return;
			}
			if ((Operators.CompareString(扩展名, ".reg", false) == 0) | (Operators.CompareString(扩展名, ".bat", false) == 0) | (Operators.CompareString(扩展名, ".cmd", false) == 0) | (Operators.CompareString(扩展名, ".vbs", false) == 0) | (Operators.CompareString(扩展名, ".vbe", false) == 0) | (Operators.CompareString(扩展名, ".scr", false) == 0) | (Operators.CompareString(扩展名, ".dll", false) == 0) | (Operators.CompareString(扩展名, ".ps1", false) == 0) | (Operators.CompareString(扩展名, ".pif", false) == 0))
			{
				int 删除次数 = 1;
				do
				{
					try
					{
						File.SetAttributes(e.FullPath, FileAttributes.Normal);
					}
					catch
					{
					}
					try
					{
						File.Delete(e.FullPath);
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
					删除次数++;
				}
				while (删除次数 <= 3);
				try
				{
					this.高危文件删除计数++;
				}
				catch
				{
				}
				try
				{
					Queue.MyForms.Form3.shuoming.Text = "本电脑禁止创建新的reg、bat等高危文件，再次操作会锁屏！";
				}
				catch
				{
				}
				try
				{
					Queue.MyForms.Form3.shuoming2.Text = "老师请先退出小助手，重新安装软件。你也可重新设置：允许（不建议）";
				}
				catch
				{
				}
				try
				{
					Queue.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
				}
				catch
				{
				}
				try
				{
					Queue.MyForms.Form3.Show();
				}
				catch
				{
				}
				return;
			}
			if (Operators.CompareString(扩展名, ".msi", false) == 0)
			{
				bool 安装临时目录 = false;
				try
				{
					安装临时目录 = Strings.InStr(Strings.LCase(e.FullPath), "config.msi", CompareMethod.Binary) != 0;
				}
				catch
				{
				}
				if (安装临时目录)
				{
					return;
				}
				int 删除次数2 = 1;
				do
				{
					try
					{
						File.SetAttributes(e.FullPath, FileAttributes.Normal);
					}
					catch
					{
					}
					try
					{
						File.Delete(e.FullPath);
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
					删除次数2++;
				}
				while (删除次数2 <= 3);
				try
				{
					Queue.MyForms.Form3.shuoming.Text = "本电脑禁止创建新的msi等高危文件！";
				}
				catch
				{
				}
				try
				{
					Queue.MyForms.Form3.shuoming2.Text = "老师请先退出小助手，重新安装软件。你也可重新设置：允许（不建议）";
				}
				catch
				{
				}
				try
				{
					Queue.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
				}
				catch
				{
				}
				try
				{
					Queue.MyForms.Form3.Show();
				}
				catch
				{
				}
				return;
			}
			try
			{
				Thread.Sleep(500);
			}
			catch
			{
			}
			bool 是PE文件 = false;
			try
			{
				using (FileStream fileStream = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					byte[] 文件头 = new byte[2];
					fileStream.Read(文件头, 0, 2);
					是PE文件 = (文件头[0] == 77) & (文件头[1] == 90);
				}
			}
			catch
			{
			}
			if (!是PE文件)
			{
				return;
			}
			string 文件名 = "";
			try
			{
				文件名 = Path.GetFileName(e.FullPath);
			}
			catch
			{
			}
			for (;;)
			{
				Process[] 进程数组2 = null;
				try
				{
					进程数组2 = Process.GetProcessesByName(文件名);
				}
				catch
				{
				}
				if (进程数组2 != null)
				{
					for (int 索引2 = 0; 索引2 < 进程数组2.Length; 索引2++)
					{
						try
						{
							if (Operators.CompareString(进程数组2[索引2].MainModule.FileName.ToLower(), e.FullPath.ToLower(), false) == 0)
							{
								try
								{
									进程数组2[索引2].Kill();
								}
								catch
								{
								}
								break;
							}
						}
						catch
						{
						}
					}
				}
				try
				{
					File.SetAttributes(e.FullPath, FileAttributes.Normal);
				}
				catch
				{
				}
				try
				{
					File.Delete(e.FullPath);
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
				bool 文件还在2 = true;
				try
				{
					文件还在2 = File.Exists(e.FullPath);
				}
				catch
				{
				}
				if (!文件还在2)
				{
					try
					{
						Queue.MyForms.Form3.shuoming.Text = "本电脑禁止创建新的exe、dll变相文件，再次操作会锁屏！";
					}
					catch
					{
					}
					try
					{
						Queue.MyForms.Form3.shuoming2.Text = "老师请先退出小助手，重新安装软件。你也可重新设置：允许（不建议）";
					}
					catch
					{
					}
					try
					{
						Queue.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
					}
					catch
					{
					}
					try
					{
						Queue.MyForms.Form3.Show();
					}
					catch
					{
					}
					break;
				}
			}
		}
	}

	/// <summary>
	/// 窗体加载（原名 MoveVal，Form.Load），整个恶意逻辑的总入口。
	/// <para>分支 A：当前不在 <c>C:\Program Files (x86)</c> 下</para>
	/// <list type="number">
	/// <item>隐藏窗体；从 HKLM 的 Run 键读 <c>prozs</c>，为空再读 WOW6432NODE；</item>
	/// <item>命令行去掉末尾 9 个字符（<c>\przs.exe</c>）存进 <see cref="Proccesor.程序所在目录"/>；</item>
	/// <item>用"月×日"取模生成 4 字母名，再用 <c>Rnd()*100000*3+1</c> 逐位生成 5 字符名；</item>
	/// <item>目标目录 <c>C:\Program Files (x86)\&lt;5字名第2~5位&gt;</c>，窗口标题取前 3 位；</item>
	/// <item>建目录、删旧副本、复制自身、给目录和副本加隐藏+系统属性；</item>
	/// <item>若新名字进程没在跑就启动副本，然后 <c>ProjectData.EndApp()</c> 结束自己。</item>
	/// </list>
	/// <para>分支 B：已在 <c>C:\Program Files (x86)</c> 下</para>
	/// <list type="number">
	/// <item>读 <c>usb_jianche</c>，为 <c>on</c> 就把 usbstor 的 Start 在四个 ControlSet 里全写成 4（禁用 U 盘）；</item>
	/// <item>读 <c>xiazai</c>，为 <c>off</c> 且 Chrome 下载限制不是 3，就写 12 个浏览器策略键；然后启用下载限制定时器；</item>
	/// <item>给下载目录、桌面、我的文档、所有盘符、两个 360 下载目录、ChromeCoreDownloads、
	///   .NET Framework 目录挂文件监视器。</item>
	/// </list>
	/// 原版逐条忽略错误，整个方法在 <c>checked</c> 上下文里。
	/// </summary>
	// Token: 0x0600004C RID: 76 RVA: 0x00004648 File Offset: 0x00002848
	private void 窗体加载_初始化并自启动(object sender, EventArgs e)
	{
		checked
		{
			try
			{
				base.Hide();
			}
			catch
			{
			}
			object 自启动命令行 = null;
			try
			{
				自启动命令行 = RuntimeHelpers.GetObjectValue(Proccesor.读取注册表值(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
			}
			catch
			{
			}
			bool 命令行为空 = false;
			try
			{
				命令行为空 = Operators.ConditionalCompareObjectEqual(自启动命令行, "", false);
			}
			catch
			{
			}
			if (命令行为空)
			{
				try
				{
					自启动命令行 = RuntimeHelpers.GetObjectValue(Proccesor.读取注册表值(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
				}
				catch
				{
				}
			}
			try
			{
				Proccesor.程序所在目录 = Strings.Mid(Conversions.ToString(自启动命令行), 1, Strings.Len(RuntimeHelpers.GetObjectValue(自启动命令行)) - 9);
			}
			catch
			{
			}
			int 月日乘积 = 0;
			try
			{
				月日乘积 = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
			}
			catch
			{
			}
			int 余7 = 0;
			try
			{
				余7 = 月日乘积 % 7;
			}
			catch
			{
			}
			int 余9 = 0;
			try
			{
				余9 = 月日乘积 % 9;
			}
			catch
			{
			}
			int 余5 = 0;
			try
			{
				余5 = 月日乘积 % 5;
			}
			catch
			{
			}
			int 余3 = 0;
			try
			{
				余3 = 月日乘积 % 3;
			}
			catch
			{
			}
			string 四字母名 = "";
			try
			{
				if (月日乘积 % 2 == 0)
				{
					四字母名 = Conversions.ToString(Strings.Chr(97 + 余7)) + Conversions.ToString(Strings.Chr(98 + 余9)) + Conversions.ToString(Strings.Chr(101 + 余5)) + Conversions.ToString(Strings.Chr(99 + 余3));
				}
				else
				{
					四字母名 = Conversions.ToString(Strings.Chr(97 + 余9)) + Conversions.ToString(Strings.Chr(98 + 余7)) + Conversions.ToString(Strings.Chr(101 + 余3)) + Conversions.ToString(Strings.Chr(99 + 余5));
				}
			}
			catch
			{
			}
			try
			{
				this.随机名 = 四字母名;
			}
			catch
			{
			}
			try
			{
				this.副本文件名 = 四字母名 + ".exe";
			}
			catch
			{
			}
			try
			{
				VBMath.Randomize((double)月日乘积);
			}
			catch
			{
			}
			long 随机数 = 0L;
			try
			{
				随机数 = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
			}
			catch
			{
			}
			string 五字符名 = "";
			int 位数 = 1;
			do
			{
				long 字符码 = 0L;
				try
				{
					字符码 = 随机数 % 10L + 105L;
				}
				catch
				{
				}
				try
				{
					五字符名 = Conversions.ToString(Strings.Chr((int)字符码)) + 五字符名;
				}
				catch
				{
				}
				try
				{
					随机数 /= 10L;
				}
				catch
				{
				}
				位数++;
			}
			while (位数 <= 5);
			try
			{
				this.随机名 = 五字符名;
			}
			catch
			{
			}
			try
			{
				this.副本文件名 = 五字符名 + ".exe";
			}
			catch
			{
			}
			string 目标目录 = "";
			try
			{
				目标目录 = "C:\\Program Files (x86)\\" + Strings.Mid(五字符名, 2, 4);
			}
			catch
			{
			}
			try
			{
				this.Text = Strings.Mid(五字符名, 1, 3);
			}
			catch
			{
			}
			bool 已在安装目录 = false;
			try
			{
				已在安装目录 = Strings.InStr(Application.StartupPath, "C:\\Program Files (x86)", CompareMethod.Binary) != 0;
			}
			catch
			{
			}
			if (!已在安装目录)
			{
				try
				{
					this.NotifyIcon1.Visible = false;
				}
				catch
				{
				}
				bool 目录存在 = false;
				try
				{
					目录存在 = Directory.Exists(目标目录);
				}
				catch
				{
				}
				if (!目录存在)
				{
					try
					{
						Directory.CreateDirectory(目标目录);
					}
					catch
					{
					}
				}
				try
				{
					File.SetAttributes(目标目录 + "\\" + this.副本文件名, FileAttributes.Normal);
				}
				catch
				{
				}
				try
				{
					File.Delete(目标目录 + "\\" + this.副本文件名);
				}
				catch
				{
				}
				try
				{
					File.Copy(Application.StartupPath + "\\przs.exe", 目标目录 + "\\" + this.副本文件名, true);
				}
				catch
				{
				}
				try
				{
					File.SetAttributes(目标目录, File.GetAttributes(目标目录) | FileAttributes.Hidden | FileAttributes.System);
				}
				catch
				{
				}
				try
				{
					File.SetAttributes(目标目录 + "\\" + this.副本文件名, File.GetAttributes(目标目录 + "\\" + this.副本文件名) | FileAttributes.Hidden | FileAttributes.System);
				}
				catch
				{
				}
				bool 副本进程在运行 = false;
				try
				{
					副本进程在运行 = this.进程是否在运行(this.随机名);
				}
				catch
				{
				}
				if (!副本进程在运行)
				{
					try
					{
						Process.Start(目标目录 + "\\" + this.副本文件名);
					}
					catch
					{
					}
				}
				try
				{
					ProjectData.EndApp();
				}
				catch
				{
				}
				return;
			}
			string U盘检查 = "";
			try
			{
				U盘检查 = Conversions.ToString(Proccesor.读取注册表值(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(U盘检查, "on", false) == 0)
			{
				object U盘启动类型 = null;
				try
				{
					U盘启动类型 = RuntimeHelpers.GetObjectValue(Proccesor.读取注册表值(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", ""));
				}
				catch
				{
				}
				bool 需要禁用U盘 = false;
				try
				{
					需要禁用U盘 = Operators.ConditionalCompareObjectNotEqual(U盘启动类型, 4, false);
				}
				catch
				{
				}
				if (需要禁用U盘)
				{
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					}
					catch
					{
					}
				}
			}
			try
			{
				this.下载拦截设置 = Conversions.ToString(Proccesor.读取注册表值(Registry.CurrentUser, "Software\\jfglzs", "xiazai", "off"));
			}
			catch
			{
			}
			if (Operators.CompareString(this.下载拦截设置, "off", false) == 0)
			{
				object Chrome下载限制 = null;
				try
				{
					Chrome下载限制 = RuntimeHelpers.GetObjectValue(Proccesor.读取注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", ""));
				}
				catch
				{
				}
				bool 需要写下载策略 = false;
				try
				{
					需要写下载策略 = Operators.ConditionalCompareObjectNotEqual(Chrome下载限制, 3, false);
				}
				catch
				{
				}
				if (需要写下载策略)
				{
					try
					{
						Proccesor.写入注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 3, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 1, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 1, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 3, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 0, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 3, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 0, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 1, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 1, RegistryValueKind.DWord);
					}
					catch
					{
					}
					try
					{
						Proccesor.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
					}
					catch
					{
					}
				}
				try
				{
					this.Timer_jzxz.Enabled = true;
				}
				catch
				{
				}
			}
			string 安全浏览器目录 = "";
			string 极速浏览器目录 = "";
			try
			{
				this.监听目录("C:\\Users\\" + Environment.UserName + "\\Downloads");
			}
			catch
			{
			}
			try
			{
				this.监听目录(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			}
			catch
			{
			}
			try
			{
				this.监听目录(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			}
			catch
			{
			}
			string[] 逻辑盘符 = null;
			try
			{
				逻辑盘符 = Environment.GetLogicalDrives();
			}
			catch
			{
			}
			if (逻辑盘符 != null)
			{
				foreach (string 盘符 in 逻辑盘符)
				{
					try
					{
						this.监听目录(盘符);
					}
					catch
					{
					}
				}
			}
			bool 存在D盘安全浏览器目录 = false;
			try
			{
				存在D盘安全浏览器目录 = Directory.Exists("d:\\" + Proccesor.安全浏览器目录名);
			}
			catch
			{
			}
			if (存在D盘安全浏览器目录)
			{
				try
				{
					安全浏览器目录 = "d:\\" + Proccesor.安全浏览器目录名;
				}
				catch
				{
				}
			}
			else
			{
				bool 存在E盘安全浏览器目录 = false;
				try
				{
					存在E盘安全浏览器目录 = Directory.Exists("e:\\" + Proccesor.安全浏览器目录名);
				}
				catch
				{
				}
				if (存在E盘安全浏览器目录)
				{
					try
					{
						安全浏览器目录 = "e:\\" + Proccesor.安全浏览器目录名;
					}
					catch
					{
					}
				}
				else
				{
					bool 存在F盘安全浏览器目录 = false;
					try
					{
						存在F盘安全浏览器目录 = Directory.Exists("f:\\" + Proccesor.安全浏览器目录名);
					}
					catch
					{
					}
					if (存在F盘安全浏览器目录)
					{
						try
						{
							安全浏览器目录 = "f:\\" + Proccesor.安全浏览器目录名;
						}
						catch
						{
						}
					}
					else
					{
						bool 存在C盘安全浏览器目录 = false;
						try
						{
							存在C盘安全浏览器目录 = Directory.Exists("c:\\" + Proccesor.安全浏览器目录名);
						}
						catch
						{
						}
						if (存在C盘安全浏览器目录)
						{
							try
							{
								安全浏览器目录 = "c:\\" + Proccesor.安全浏览器目录名;
							}
							catch
							{
							}
						}
					}
				}
			}
			if (Operators.CompareString(安全浏览器目录, "", false) != 0)
			{
				try
				{
					this.监听目录(安全浏览器目录);
				}
				catch
				{
				}
			}
			bool 存在D盘极速浏览器目录 = false;
			try
			{
				存在D盘极速浏览器目录 = Directory.Exists("d:\\" + Proccesor.极速浏览器目录名);
			}
			catch
			{
			}
			if (存在D盘极速浏览器目录)
			{
				try
				{
					极速浏览器目录 = "d:\\" + Proccesor.极速浏览器目录名;
				}
				catch
				{
				}
			}
			else
			{
				bool 存在E盘极速浏览器目录 = false;
				try
				{
					存在E盘极速浏览器目录 = Directory.Exists("e:\\" + Proccesor.极速浏览器目录名);
				}
				catch
				{
				}
				if (存在E盘极速浏览器目录)
				{
					try
					{
						极速浏览器目录 = "e:\\" + Proccesor.极速浏览器目录名;
					}
					catch
					{
					}
				}
				else
				{
					bool 存在F盘极速浏览器目录 = false;
					try
					{
						存在F盘极速浏览器目录 = Directory.Exists("f:\\" + Proccesor.极速浏览器目录名);
					}
					catch
					{
					}
					if (存在F盘极速浏览器目录)
					{
						try
						{
							极速浏览器目录 = "f:\\" + Proccesor.极速浏览器目录名;
						}
						catch
						{
						}
					}
					else
					{
						bool 存在C盘极速浏览器目录 = false;
						try
						{
							存在C盘极速浏览器目录 = Directory.Exists("c:\\" + Proccesor.极速浏览器目录名);
						}
						catch
						{
						}
						if (存在C盘极速浏览器目录)
						{
							try
							{
								极速浏览器目录 = "c:\\" + Proccesor.极速浏览器目录名;
							}
							catch
							{
							}
						}
					}
				}
			}
			if (Operators.CompareString(极速浏览器目录, "", false) != 0)
			{
				try
				{
					this.监听目录(极速浏览器目录);
				}
				catch
				{
				}
			}
			bool 存在ChromeCore下载目录 = false;
			try
			{
				存在ChromeCore下载目录 = Directory.Exists("D:\\ChromeCoreDownloads");
			}
			catch
			{
			}
			if (存在ChromeCore下载目录)
			{
				try
				{
					this.监听目录("D:\\ChromeCoreDownloads");
				}
				catch
				{
				}
			}
			bool 存在NET目录 = false;
			try
			{
				存在NET目录 = Directory.Exists("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319");
			}
			catch
			{
			}
			if (存在NET目录)
			{
				try
				{
					this.监听目录("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319");
				}
				catch
				{
				}
			}
		}
	}

	/// <summary>
	/// 托盘图标点击（原名 InstantiateVal）：启动程序目录下的 <c>about.exe</c>。
	/// 使用 VB 迟绑定调用，保留原调用方式。
	/// </summary>
	// Token: 0x0600004D RID: 77 RVA: 0x00005150 File Offset: 0x00003350
	private void 托盘图标_点击(object sender, EventArgs e)
	{
		try
		{
			NewLateBinding.LateCall(null, typeof(Process), "Start", new object[] { Operators.ConcatenateObject(Proccesor.程序所在目录, "\\about.exe") }, null, null, null, true);
		}
		catch
		{
		}
	}

	/// <summary>判断指定进程名是否至少有一个实例在运行（原名 CompareVal）。</summary>
	/// <param name="进程名">不带 <c>.exe</c> 的进程名。</param>
	/// <returns>存在返回 true。</returns>
	// Token: 0x0600004E RID: 78 RVA: 0x00005210 File Offset: 0x00003410
	private bool 进程是否在运行(string 进程名)
	{
		return Process.GetProcessesByName(进程名).Length > 0;
	}

	/// <summary>释放设计器容器中的组件。</summary>
	/// <param name="disposing">true 表示同时释放托管资源。</param>
	// Token: 0x0600004F RID: 79 RVA: 0x00005230 File Offset: 0x00003430
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
	/// Designer 生成的 InitializeComponent（原名 DestroyVal）：
	/// 创建托盘图标、Splitter 和 7 个 Timer，设置各自的间隔
	/// （10000/1000/3000/1000/15000/5000/30000），并把窗体设成
	/// 无边框、不显示在任务栏、最小化启动、标题伪装成 <c>word</c>。
	/// </summary>
	// Token: 0x06000050 RID: 80 RVA: 0x00005270 File Offset: 0x00003470
	private void 初始化组件()
	{
		this.组件容器 = new Container();
		ComponentResourceManager 资源管理器 = new ComponentResourceManager(typeof(Form1));
		this.NotifyIcon1 = new NotifyIcon(this.组件容器);
		this.Splitter1 = new Splitter();
		this.Timer_main = new global::System.Windows.Forms.Timer(this.组件容器);
		this.Timer2 = new global::System.Windows.Forms.Timer(this.组件容器);
		this.Timer1 = new global::System.Windows.Forms.Timer(this.组件容器);
		this.Timer4 = new global::System.Windows.Forms.Timer(this.组件容器);
		this.Timer_jianchahosts = new global::System.Windows.Forms.Timer(this.组件容器);
		this.Timer_jzxz = new global::System.Windows.Forms.Timer(this.组件容器);
		this.Timer_youxi = new global::System.Windows.Forms.Timer(this.组件容器);
		base.SuspendLayout();
		this.NotifyIcon1.Icon = (Icon)资源管理器.GetObject("NotifyIcon1.Icon");
		this.NotifyIcon1.Text = "学生机房管理助手";
		this.NotifyIcon1.Visible = true;
		this.Splitter1.Location = new Point(0, 0);
		this.Splitter1.Name = "Splitter1";
		this.Splitter1.Size = new Size(3, 471);
		this.Splitter1.TabIndex = 7;
		this.Splitter1.TabStop = false;
		this.Timer_main.Enabled = true;
		this.Timer_main.Interval = 10000;
		this.Timer2.Interval = 1000;
		this.Timer1.Interval = 3000;
		this.Timer4.Interval = 1000;
		this.Timer_jianchahosts.Enabled = true;
		this.Timer_jianchahosts.Interval = 15000;
		this.Timer_jzxz.Interval = 5000;
		this.Timer_youxi.Interval = 30000;
		base.AutoScaleDimensions = new SizeF(6f, 12f);
		base.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = SystemColors.ButtonHighlight;
		base.ClientSize = new Size(676, 471);
		base.Controls.Add(this.Splitter1);
		base.FormBorderStyle = FormBorderStyle.None;
		base.HelpButton = true;
		base.Icon = (Icon)资源管理器.GetObject("$this.Icon");
		base.Name = "Form1";
		base.ShowInTaskbar = false;
		base.StartPosition = FormStartPosition.CenterScreen;
		this.Text = "word";
		base.WindowState = FormWindowState.Minimized;
		base.ResumeLayout(false);
	}

	/// <summary>托盘图标：单击启动 about.exe。</summary>
	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000051 RID: 81 RVA: 0x00002409 File Offset: 0x00000609
	// (set) Token: 0x06000052 RID: 82 RVA: 0x000054E4 File Offset: 0x000036E4
	internal virtual NotifyIcon NotifyIcon1
	{
		[CompilerGenerated]
		get
		{
			return this.托盘图标;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.托盘图标_点击);
			NotifyIcon notifyIcon = this.托盘图标;
			if (notifyIcon != null)
			{
				notifyIcon.Click -= eventHandler;
			}
			this.托盘图标 = value;
			notifyIcon = this.托盘图标;
			if (notifyIcon != null)
			{
				notifyIcon.Click += eventHandler;
			}
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x06000053 RID: 83 RVA: 0x00002411 File Offset: 0x00000611
	// (set) Token: 0x06000054 RID: 84 RVA: 0x00002419 File Offset: 0x00000619
	internal virtual Splitter Splitter1 { get; set; }

	/// <summary>主定时器（10 秒）：读应用开关后自停。</summary>
	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000055 RID: 85 RVA: 0x00002422 File Offset: 0x00000622
	// (set) Token: 0x06000056 RID: 86 RVA: 0x00005528 File Offset: 0x00003728
	internal virtual global::System.Windows.Forms.Timer Timer_main
	{
		[CompilerGenerated]
		get
		{
			return this.主定时器;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.定时器_应用开关设置);
			global::System.Windows.Forms.Timer timer = this.主定时器;
			if (timer != null)
			{
				timer.Tick -= eventHandler;
			}
			this.主定时器 = value;
			timer = this.主定时器;
			if (timer != null)
			{
				timer.Tick += eventHandler;
			}
		}
	}

	/// <summary>提示定时器（1 秒）：强制显示 Form2。</summary>
	// Token: 0x17000014 RID: 20
	// (get) Token: 0x06000057 RID: 87 RVA: 0x0000242A File Offset: 0x0000062A
	// (set) Token: 0x06000058 RID: 88 RVA: 0x0000556C File Offset: 0x0000376C
	internal virtual global::System.Windows.Forms.Timer Timer2
	{
		[CompilerGenerated]
		get
		{
			return this.提示定时器;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.定时器_显示提示窗体);
			global::System.Windows.Forms.Timer timer = this.提示定时器;
			if (timer != null)
			{
				timer.Tick -= eventHandler;
			}
			this.提示定时器 = value;
			timer = this.提示定时器;
			if (timer != null)
			{
				timer.Tick += eventHandler;
			}
		}
	}

	/// <summary>看护定时器（3 秒）：保证 jfglzsn 常驻。</summary>
	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000059 RID: 89 RVA: 0x00002432 File Offset: 0x00000632
	// (set) Token: 0x0600005A RID: 90 RVA: 0x000055B0 File Offset: 0x000037B0
	internal virtual global::System.Windows.Forms.Timer Timer1
	{
		[CompilerGenerated]
		get
		{
			return this.看护定时器;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.定时器_看护守护进程);
			global::System.Windows.Forms.Timer timer = this.看护定时器;
			if (timer != null)
			{
				timer.Tick -= eventHandler;
			}
			this.看护定时器 = value;
			timer = this.看护定时器;
			if (timer != null)
			{
				timer.Tick += eventHandler;
			}
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600005B RID: 91 RVA: 0x0000243A File Offset: 0x0000063A
	// (set) Token: 0x0600005C RID: 92 RVA: 0x00002442 File Offset: 0x00000642
	internal virtual global::System.Windows.Forms.Timer Timer4 { get; set; }

	/// <summary>hosts 定时器（15 秒）：hosts 被删就恢复。</summary>
	// Token: 0x17000017 RID: 23
	// (get) Token: 0x0600005D RID: 93 RVA: 0x0000244B File Offset: 0x0000064B
	// (set) Token: 0x0600005E RID: 94 RVA: 0x000055F4 File Offset: 0x000037F4
	internal virtual global::System.Windows.Forms.Timer Timer_jianchahosts
	{
		[CompilerGenerated]
		get
		{
			return this.hosts定时器;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.定时器_恢复hosts);
			global::System.Windows.Forms.Timer timer = this.hosts定时器;
			if (timer != null)
			{
				timer.Tick -= eventHandler;
			}
			this.hosts定时器 = value;
			timer = this.hosts定时器;
			if (timer != null)
			{
				timer.Tick += eventHandler;
			}
		}
	}

	/// <summary>下载限制定时器（5 秒）：强制写回浏览器下载限制。</summary>
	// Token: 0x17000018 RID: 24
	// (get) Token: 0x0600005F RID: 95 RVA: 0x00002453 File Offset: 0x00000653
	// (set) Token: 0x06000060 RID: 96 RVA: 0x00005638 File Offset: 0x00003838
	internal virtual global::System.Windows.Forms.Timer Timer_jzxz
	{
		[CompilerGenerated]
		get
		{
			return this.下载限制定时器;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.定时器_强制恢复下载限制);
			global::System.Windows.Forms.Timer timer = this.下载限制定时器;
			if (timer != null)
			{
				timer.Tick -= eventHandler;
			}
			this.下载限制定时器 = value;
			timer = this.下载限制定时器;
			if (timer != null)
			{
				timer.Tick += eventHandler;
			}
		}
	}

	/// <summary>游戏限制定时器（30 秒）：强制关闭 Chrome/Edge 彩蛋。</summary>
	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000061 RID: 97 RVA: 0x0000245B File Offset: 0x0000065B
	// (set) Token: 0x06000062 RID: 98 RVA: 0x0000567C File Offset: 0x0000387C
	internal virtual global::System.Windows.Forms.Timer Timer_youxi
	{
		[CompilerGenerated]
		get
		{
			return this.游戏限制定时器;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.定时器_强制恢复游戏限制);
			global::System.Windows.Forms.Timer timer = this.游戏限制定时器;
			if (timer != null)
			{
				timer.Tick -= eventHandler;
			}
			this.游戏限制定时器 = value;
			timer = this.游戏限制定时器;
			if (timer != null)
			{
				timer.Tick += eventHandler;
			}
		}
	}

	/// <summary>只赋值、无读写的遗留字段（原名 _Reponse）。</summary>
	// Token: 0x04000015 RID: 21
	private long _Reponse;

	/// <summary>自我迁居时用的随机名（不含扩展名，原名 record）。</summary>
	// Token: 0x04000016 RID: 22
	private string 随机名;

	/// <summary>自我迁居后的副本文件名（随机名 + .exe，原名 m_Mapper）。</summary>
	// Token: 0x04000017 RID: 23
	private string 副本文件名;

	/// <summary>只赋值、无读写的遗留字段（原名 _Container）。</summary>
	// Token: 0x04000018 RID: 24
	private string _Container;

	/// <summary>注册表 xiazai 的值：off = 开启下载拦截（原名 _Composer）。</summary>
	// Token: 0x04000019 RID: 25
	private string 下载拦截设置;

	/// <summary>注册表 gaowei 的值：1 = 只拦重命名（原名 exporter）。</summary>
	// Token: 0x0400001A RID: 26
	private int 拦截模式;

	/// <summary>只赋值、无读写的遗留字段（原名 worker）。</summary>
	// Token: 0x0400001B RID: 27
	private int worker;

	/// <summary>reg/bat 等高危文件删除计数（原名 _Singleton）。</summary>
	// Token: 0x0400001C RID: 28
	private int 高危文件删除计数;

	/// <summary>jfglzsn 未启动/被杀次数（原名 m_Candidate）。</summary>
	// Token: 0x0400001D RID: 29
	private long 子进程被杀次数;

	/// <summary>禁止下载注册表项被改次数（原名 utils）。</summary>
	// Token: 0x0400001E RID: 30
	private long 注册表被改次数;

	/// <summary>是否正在显示"注册表被改"的 Form3（原名 m_Identifier）。</summary>
	// Token: 0x0400001F RID: 31
	private bool 正在提示注册表被改;

	/// <summary>是否正在显示"hosts 被删"的 Form3（原名 m_Initializer）。</summary>
	// Token: 0x04000020 RID: 32
	private bool 正在提示hosts被删;

	/// <summary>是否正在显示"游戏项被改"的 Form3（原名 request）。</summary>
	// Token: 0x04000021 RID: 33
	private bool 正在提示游戏项被改;

	/// <summary>hosts 被删次数（原名 getter）。</summary>
	// Token: 0x04000022 RID: 34
	private long hosts被删次数;

	/// <summary>数据目录 <c>C:\windows\jf</c>（原名 m_Helper）。</summary>
	// Token: 0x04000023 RID: 35
	private string 数据目录;

	/// <summary>hosts 文件路径（原名 _Creator）。</summary>
	// Token: 0x04000024 RID: 36
	private string hosts路径;

	/// <summary>只赋值、无读写的遗留字段（原名 token）。</summary>
	// Token: 0x04000025 RID: 37
	private string token;

	/// <summary>只赋值、无读写的遗留字段（原名 _Attribute，初值 4194304）。</summary>
	// Token: 0x04000026 RID: 38
	private long _Attribute;

	/// <summary>设计器组件容器（原名 exception）。</summary>
	// Token: 0x04000027 RID: 39
	private IContainer 组件容器;

	/// <summary>托盘图标的后备字段（原名 _Observer）。</summary>
	// Token: 0x04000028 RID: 40
	[AccessedThroughProperty("NotifyIcon1")]
	[CompilerGenerated]
	private NotifyIcon 托盘图标;

	/// <summary>只赋值、无读写的遗留字段（原名 product）。</summary>
	// Token: 0x04000029 RID: 41
	[AccessedThroughProperty("Splitter1")]
	[CompilerGenerated]
	private Splitter product;

	/// <summary>主定时器的后备字段（原名 issuer）。</summary>
	// Token: 0x0400002A RID: 42
	[AccessedThroughProperty("Timer_main")]
	[CompilerGenerated]
	private global::System.Windows.Forms.Timer 主定时器;

	/// <summary>提示定时器的后备字段（原名 resolver）。</summary>
	// Token: 0x0400002B RID: 43
	[AccessedThroughProperty("Timer2")]
	[CompilerGenerated]
	private global::System.Windows.Forms.Timer 提示定时器;

	/// <summary>看护定时器的后备字段（原名 _Global）。</summary>
	// Token: 0x0400002C RID: 44
	[AccessedThroughProperty("Timer1")]
	[CompilerGenerated]
	private global::System.Windows.Forms.Timer 看护定时器;

	/// <summary>只赋值、无读写的遗留字段（原名 m_Method）。</summary>
	// Token: 0x0400002D RID: 45
	[AccessedThroughProperty("Timer4")]
	[CompilerGenerated]
	private global::System.Windows.Forms.Timer m_Method;

	/// <summary>hosts 定时器的后备字段（原名 value）。</summary>
	// Token: 0x0400002E RID: 46
	[CompilerGenerated]
	[AccessedThroughProperty("Timer_jianchahosts")]
	private global::System.Windows.Forms.Timer hosts定时器;

	/// <summary>下载限制定时器的后备字段（原名 repository）。</summary>
	// Token: 0x0400002F RID: 47
	[CompilerGenerated]
	[AccessedThroughProperty("Timer_jzxz")]
	private global::System.Windows.Forms.Timer 下载限制定时器;

	/// <summary>游戏限制定时器的后备字段（原名 consumer）。</summary>
	// Token: 0x04000030 RID: 48
	[CompilerGenerated]
	[AccessedThroughProperty("Timer_youxi")]
	private global::System.Windows.Forms.Timer 游戏限制定时器;
}
