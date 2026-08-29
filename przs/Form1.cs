// ============================================================================
// Form1.cs — 机房管理助手主控窗体（隐藏驻留 + 文件监控 + 注册表强制策略）
// 真实职责：主窗体/核心调度器。启动后立即隐藏并驻留托盘图标，挂载 7 个 Timer 与多个
// FileSystemWatcher：进程/文件自愈、禁止下载安装、hosts 自愈、禁用 USB 存储、禁止浏览器
// 游戏、监视桌面/下载/全盘目录并删除"违规"文件；自我复制持久化到
// C:\Program Files (x86)\<随机名>\ 并写入注册表自启动（HKLM\...\Run 键 prozs）。
// 说明：ConfuserEx 混淆反编译产物，标识符已按 Form1.cs.md 映射表反混淆重命名。
// ============================================================================
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using gypZRX1vuWc3aeye0R.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

// Token: 0x02000009 RID: 9
[DesignerGenerated]
public partial class Form1 : Form
{
	// Token: 0x06000037 RID: 55 RVA: 0x00002D24 File Offset: 0x00000F24
/// <summary>
/// 构造主窗体：订阅 Load 事件（首启自举），初始化计数器与路径常量，并创建托盘/定时器控件。
/// </summary>
	public Form1()
	{
		base.Load += this.主加载持久化;
		this.m_Proccesor = 0L;
		this._Reponse = 0;
		this.m_Attr = 0;
		this.备用程序目录 = "C:\\windows\\jf";
		this.hosts路径 = "C:\\Windows\\System32\\drivers\\etc\\hosts";
		this.m_Base = 4194304L;
		this.初始化组件();
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002DD4 File Offset: 0x00000FD4
/// <summary>
/// 初始化组件（窗体布局）：创建托盘图标 NotifyIcon1、Splitter 与 7 个定时器
/// （Timer_main 10s / Timer2 1s / Timer1 3s / Timer_jianchahosts 15s / Timer_jzxz 5s / Timer_youxi 30s / Timer4 1s）。
/// 事件挂接在属性 setter 中完成（NotifyIcon1.Click、各 Timer.Tick）。
/// </summary>
	private void 初始化组件()
	{
		this.queue = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Form1));
		this.NotifyIcon1 = new NotifyIcon(this.queue);
		this.Splitter1 = new Splitter();
		this.Timer_main = new System.Windows.Forms.Timer(this.queue);
		this.Timer2 = new System.Windows.Forms.Timer(this.queue);
		this.Timer1 = new System.Windows.Forms.Timer(this.queue);
		this.Timer4 = new System.Windows.Forms.Timer(this.queue);
		this.Timer_jianchahosts = new System.Windows.Forms.Timer(this.queue);
		this.Timer_jzxz = new System.Windows.Forms.Timer(this.queue);
		this.Timer_youxi = new System.Windows.Forms.Timer(this.queue);
		this.SuspendLayout();
		this.NotifyIcon1.Icon = (Icon)componentResourceManager.GetObject("NotifyIcon1.Icon");
		this.NotifyIcon1.Text = "学生机房管理助手";
		this.NotifyIcon1.Visible = true;
		Control splitter = this.Splitter1;
		Point location = new Point(0, 0);
		splitter.Location = location;
		this.Splitter1.Name = "Splitter1";
		Control splitter2 = this.Splitter1;
		Size size = new Size(3, 471);
		splitter2.Size = size;
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
		SizeF autoScaleDimensions = new SizeF(6f, 12f);
		this.AutoScaleDimensions = autoScaleDimensions;
		this.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = SystemColors.ButtonHighlight;
		size = new Size(676, 471);
		this.ClientSize = size;
		this.Controls.Add(this.Splitter1);
		this.FormBorderStyle = FormBorderStyle.None;
		this.HelpButton = true;
		this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
		this.Name = "Form1";
		this.ShowInTaskbar = false;
		this.StartPosition = FormStartPosition.CenterScreen;
		this.Text = "word";
		this.WindowState = FormWindowState.Minimized;
		this.ResumeLayout(false);
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600003A RID: 58 RVA: 0x00003054 File Offset: 0x00001254
	// (set) Token: 0x0600003B RID: 59 RVA: 0x00003068 File Offset: 0x00001268
/// <summary>托盘图标（"学生机房管理助手"）；Click → 托盘图标点击（启动 about.exe）。</summary>
	internal virtual NotifyIcon NotifyIcon1
	{
		get
		{
			return this.@event;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.托盘图标点击);
			if (this.@event != null)
			{
				this.@event.Click -= value2;
			}
			this.@event = value;
			if (this.@event != null)
			{
				this.@event.Click += value2;
			}
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600003C RID: 60 RVA: 0x000030B4 File Offset: 0x000012B4
	// (set) Token: 0x0600003D RID: 61 RVA: 0x00002209 File Offset: 0x00000409
/// <summary>分隔条控件（无事件挂接）。</summary>
	internal virtual Splitter Splitter1
	{
		get
		{
			return this.decorator;
		}
		set
		{
			this.decorator = value;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x0600003E RID: 62 RVA: 0x000030C8 File Offset: 0x000012C8
	// (set) Token: 0x0600003F RID: 63 RVA: 0x000030DC File Offset: 0x000012DC
/// <summary>Timer_main（10s，一次性）；Tick → 检查游戏编号开关。</summary>
	internal virtual System.Windows.Forms.Timer Timer_main
	{
		get
		{
			return this.request;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.检查游戏编号开关);
			if (this.request != null)
			{
				this.request.Tick -= value2;
			}
			this.request = value;
			if (this.request != null)
			{
				this.request.Tick += value2;
			}
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000040 RID: 64 RVA: 0x00003128 File Offset: 0x00001328
	// (set) Token: 0x06000041 RID: 65 RVA: 0x0000313C File Offset: 0x0000133C
/// <summary>Timer2（1s）；Tick → 显示Form2警告窗。</summary>
	internal virtual System.Windows.Forms.Timer Timer2
	{
		get
		{
			return this._Tokenizer;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.显示Form2警告窗);
			if (this._Tokenizer != null)
			{
				this._Tokenizer.Tick -= value2;
			}
			this._Tokenizer = value;
			if (this._Tokenizer != null)
			{
				this._Tokenizer.Tick += value2;
			}
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000042 RID: 66 RVA: 0x00003188 File Offset: 0x00001388
	// (set) Token: 0x06000043 RID: 67 RVA: 0x0000319C File Offset: 0x0000139C
/// <summary>Timer1（3s）；Tick → exe自愈重生。</summary>
	internal virtual System.Windows.Forms.Timer Timer1
	{
		get
		{
			return this.rule;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.exe自愈重生);
			if (this.rule != null)
			{
				this.rule.Tick -= value2;
			}
			this.rule = value;
			if (this.rule != null)
			{
				this.rule.Tick += value2;
			}
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x06000044 RID: 68 RVA: 0x000031E8 File Offset: 0x000013E8
	// (set) Token: 0x06000045 RID: 69 RVA: 0x00002212 File Offset: 0x00000412
/// <summary>Timer4（1s，未启用/未挂事件）。</summary>
	internal virtual System.Windows.Forms.Timer Timer4
	{
		get
		{
			return this.m_State;
		}
		set
		{
			this.m_State = value;
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000046 RID: 70 RVA: 0x000031FC File Offset: 0x000013FC
	// (set) Token: 0x06000047 RID: 71 RVA: 0x00003210 File Offset: 0x00001410
/// <summary>Timer_jianchahosts（15s）；Tick → hosts文件自愈。</summary>
	internal virtual System.Windows.Forms.Timer Timer_jianchahosts
	{
		get
		{
			return this.m_Record;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.hosts文件自愈);
			if (this.m_Record != null)
			{
				this.m_Record.Tick -= value2;
			}
			this.m_Record = value;
			if (this.m_Record != null)
			{
				this.m_Record.Tick += value2;
			}
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x06000048 RID: 72 RVA: 0x0000325C File Offset: 0x0000145C
	// (set) Token: 0x06000049 RID: 73 RVA: 0x00003270 File Offset: 0x00001470
/// <summary>Timer_jzxz（5s）；Tick → 禁止下载策略强制。</summary>
	internal virtual System.Windows.Forms.Timer Timer_jzxz
	{
		get
		{
			return this._Test;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.禁止下载策略强制);
			if (this._Test != null)
			{
				this._Test.Tick -= value2;
			}
			this._Test = value;
			if (this._Test != null)
			{
				this._Test.Tick += value2;
			}
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600004A RID: 74 RVA: 0x000032BC File Offset: 0x000014BC
	// (set) Token: 0x0600004B RID: 75 RVA: 0x000032D0 File Offset: 0x000014D0
/// <summary>Timer_youxi（30s）；Tick → 禁止浏览器游戏。</summary>
	internal virtual System.Windows.Forms.Timer Timer_youxi
	{
		get
		{
			return this.val;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.禁止浏览器游戏);
			if (this.val != null)
			{
				this.val.Tick -= value2;
			}
			this.val = value;
			if (this.val != null)
			{
				this.val.Tick += value2;
			}
		}
	}

	// Token: 0x0600004C RID: 76 RVA: 0x0000331C File Offset: 0x0000151C
/// <summary>
/// 检查游戏/编号开关（Timer_main 10s，一次性）：
/// 1. 启用 Timer1(3s exe 自愈)；2. 读 HKCU\Software\jfglzs\games_jianche，为 "on" 则启用 Timer_youxi(30s)；
/// 3. 读 bianhao 键，为 "on" 则显示 bianhao 编号窗；4. 关闭 Timer_main。
/// </summary>
	private void 检查游戏编号开关(object sender, EventArgs e)
	{
		try
		{
			// 1. 启用 Timer1（3s，exe 自愈重生）
			this.Timer1.Enabled = true;
			// 2. games_jianche=on → 启用 Timer_youxi（30s，禁止浏览器游戏）
			if (Operators.CompareString(Conversions.ToString(Initializer.读注册表值(Registry.CurrentUser, "Software\\jfglzs", "games_jianche", "")), "on", false) == 0)
			{
				this.Timer_youxi.Enabled = true;
			}
			// 3. bianhao=on → 显示编号窗
			if (Operators.CompareString(Conversions.ToString(Initializer.读注册表值(Registry.CurrentUser, "Software\\jfglzs", "bianhao", "")), "on", false) == 0)
			{
				Account.MyForms.bianhao.Show();
			}
			// 4. 关闭 Timer_main（一次性调度）
			this.Timer_main.Enabled = false;
		}
		catch (Exception ex) { /* [反混淆] 原状态机72行 → 还原为正常控制流；VB On Error 样板已忽略 */ }
	}

	// Token: 0x0600004D RID: 77 RVA: 0x0000221B File Offset: 0x0000041B
/// <summary>
/// 显示 Form2 警告窗（Timer2 1s）：若 Form2 不可见则 Show()。用于违规次数超限后的持续弹窗。
/// </summary>
	private void 显示Form2警告窗(object sender, EventArgs e)
	{
		if (!Account.MyForms.Form2.Visible)
		{
			Account.MyForms.Form2.Show();
		}
	}

	// Token: 0x0600004E RID: 78 RVA: 0x0000348C File Offset: 0x0000168C
/// <summary>
/// exe 自愈重生（Timer1 3s）：若 jfglzsn 进程不存在 → 重生计数器++；
/// config\jfglzsn.exe 不存在且 C:\windows\jf\jfglzsn.dat 存在则复制为 exe；随后 Process.Start 启动。
/// 计数器>=40 时在 Form2 提示"进程j被多次终止或文件会被杀毒软件（如win10自带的defender）清除！"并显示 Form2、启用 Timer2。
/// </summary>
	private void exe自愈重生(object sender, EventArgs e)
	{
		try
		{
			// 若 jfglzsn 进程已在运行 → 无需重生
			if (!this.进程存在检测("jfglzsn"))
			{
				checked
				{
					// 重生计数 +1
					this.重生计数器 += 1L;
					// jfglzsn.exe 缺失且备用 jfglzsn.dat 存在 → 复制还原
					if (!File.Exists(Conversions.ToString(Operators.ConcatenateObject(Initializer.程序安装目录, "\\jfglzsn.exe"))) && File.Exists(this.备用程序目录 + "\\jfglzsn.dat"))
					{
						File.Copy(this.备用程序目录 + "\\jfglzsn.dat", Conversions.ToString(Operators.ConcatenateObject(Initializer.程序安装目录, "\\jfglzsn.exe")), true);
					}
					// 启动 jfglzsn.exe
					NewLateBinding.LateCall(null, typeof(Process), "Start", new object[]
					{
						Operators.ConcatenateObject(Initializer.程序安装目录, "\\jfglzsn.exe")
					}, null, null, null, true);
					// 重生计数 >= 40 → 提示杀软干扰并弹 Form2
					if (this.重生计数器 >= 40L)
					{
						Account.MyForms.Form2.shuoming.Text = "进程j被多次终止或文件会被杀毒软件（如win10自带的defender）清除！";
						Account.MyForms.Form2.Show();
						this.Timer2.Enabled = true;
					}
				}
			}
		}
		catch (Exception ex) { /* [反混淆] 原状态机91行 → 还原为正常控制流；VB On Error 样板已忽略 */ }
	}

	// Token: 0x0600004F RID: 79 RVA: 0x0000367C File Offset: 0x0000187C
/// <summary>
/// hosts 文件自愈（Timer_jianchahosts 15s）：hosts 不存在 → hosts恢复计数器++、置 hosts被删标志；
/// 存在 hosts2 备份则复制恢复并提示"hosts文件被删除！现在恢复。"；恢复失败次数>5 → 关 Form3、
/// Form2 提示"hosts文件多次被删除或备份文件不存在！"并启用 Timer2。hosts 正常存在时关 Form3 并复位标志。
/// </summary>
	private void hosts文件自愈(object sender, EventArgs e)
	{
		try
		{
			checked
			{
				// hosts 正常存在：复位标志，必要时关闭 Form3
				if (File.Exists(this.hosts路径))
				{
					if (this.m_Facade)
					{
						Account.MyForms.Form3.Close();
					}
					this.m_Facade = false;
				}
				else
				{
					// hosts 被删除：计数 + 置标志
					this.hosts恢复计数器 += 1L;
					this.m_Facade = true;
					// 存在 hosts2 备份 → 复制恢复并提示
					if (File.Exists(this.hosts路径 + "2"))
					{
						File.Copy(this.hosts路径 + "2", this.hosts路径, true);
						Account.MyForms.Form3.shuoming.Text = "hosts文件被删除！现在恢复。";
						Account.MyForms.Form3.Show();
					}
					// 恢复失败计数 > 5 → 放弃恢复，改弹 Form2 警告
					if (this.hosts恢复计数器 > 5L)
					{
						Account.MyForms.Form3.Close();
						Account.MyForms.Form2.shuoming.Text = "hosts文件多次被删除或备份文件不存在！";
						Account.MyForms.Form2.Show();
						this.Timer2.Enabled = true;
					}
				}
			}
		}
		catch (Exception ex) { /* [反混淆] 原状态机105行 → 还原为正常控制流；VB On Error 样板已忽略 */ }
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00003894 File Offset: 0x00001A94
/// <summary>
/// 禁止下载策略强制（Timer_jzxz 5s）：校验 IE Zone3 1803、Chrome/Edge DownloadRestrictions 是否被改；
/// 任一被改 → 违规修改注册表计数器++；<=15 次则写回全部禁止下载注册表策略（IE/Chrome/Edge/Firefox），
/// 依次 Kill chrome/msedge/firefox/iexplorer/explorer 并重启 explorer，Form3 提示"你违规修改了注册表的禁止下载项！"；
/// >15 次 → Form2 提示"请重新启动电脑！"。全部符合预期时关闭 Form3、复位标志。
/// </summary>
	private void 禁止下载策略强制(object sender, EventArgs e)
	{
		try
		{
			// 校验三项下载策略值是否仍为预期值 "3"
			object ieZone1803 = RuntimeHelpers.GetObjectValue(Initializer.读注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", ""));
			object chromeDownloadRestrictions = RuntimeHelpers.GetObjectValue(Initializer.读注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", ""));
			object edgeDownloadRestrictions = RuntimeHelpers.GetObjectValue(Initializer.读注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", ""));
			checked
			{
				// 任一 ≠ "3" → 视为违规
				if (Conversions.ToBoolean(Operators.OrObject(Operators.OrObject(Operators.CompareObjectNotEqual(ieZone1803, "3", false), Operators.CompareObjectNotEqual(chromeDownloadRestrictions, "3", false)), Operators.CompareObjectNotEqual(edgeDownloadRestrictions, "3", false))))
				{
					this.违规修改注册表计数器 += 1L;
					if (this.违规修改注册表计数器 > 15L)
					{
						// 违规次数超限：放弃恢复，弹 Form2 提示重启电脑
						Account.MyForms.Form3.Close();
						Account.MyForms.Form2.shuoming.Text = "你多次修改了注册表的禁止下载项！";
						Account.MyForms.Form2.shuoming2.Text = "请重新启动电脑！";
						Account.MyForms.Form2.Show();
						this.Timer2.Enabled = true;
					}
					else
					{
						// 写回全部禁止下载注册表策略（IE/Chrome/Edge/Firefox）
						Initializer.写注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 3, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 1, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 1, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 3, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 0, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 3, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 0, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 1, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 1, RegistryValueKind.DWord);
						Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
						// 依次 Kill 所有 chrome/msedge/firefox/iexplorer/explorer 进程，最后重启 explorer
						foreach (Process chromeProcess in Process.GetProcessesByName("chrome"))
						{
							chromeProcess.Kill();
						}
						foreach (Process edgeProcess in Process.GetProcessesByName("msedge"))
						{
							edgeProcess.Kill();
						}
						foreach (Process firefoxProcess in Process.GetProcessesByName("firefox"))
						{
							firefoxProcess.Kill();
						}
						foreach (Process ieProcess in Process.GetProcessesByName("iexplorer"))
						{
							ieProcess.Kill();
						}
						foreach (Process explorerProcess in Process.GetProcessesByName("explorer"))
						{
							explorerProcess.Kill();
						}
						Process.Start("explorer");
						this.m_Context = true;
						Account.MyForms.Form3.shuoming.Text = "你违规修改了注册表的禁止下载项！";
						Account.MyForms.Form3.shuoming2.Text = "现在助手强制恢复！";
						Account.MyForms.Form3.Show();
					}
				}
				else
				{
					// 全部符合预期：关闭 Form3 并复位违规标志
					if (this.m_Context)
					{
						Account.MyForms.Form3.Close();
					}
					this.m_Context = false;
				}
			}
		}
		catch (Exception ex) { /* [反混淆] 原状态机239行 → 还原为正常控制流；VB On Error 样板已忽略 */ }
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00003EC0 File Offset: 0x000020C0
/// <summary>
/// 安装目录监视器：对指定目录挂 FileSystemWatcher（Filter=*.*，桌面目录含子目录，NotifyFilter=FileName|LastWrite）。
/// 读 gaowei（高危文件策略模式）与 xiazai（下载限制开关）：xiazai=off → 订阅 Renamed（删除下载文件）；
/// gaowei!=1 → 订阅 Created/Changed（删除新建 exe/高危文件）。
/// </summary>
	private void 安装目录监视器(string string_0)
	{
		if (Directory.Exists(string_0))
		{
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
			fileSystemWatcher.Path = string_0;
			fileSystemWatcher.Filter = "*.*";
			if (Operators.CompareString(string_0, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), false) == 0)
			{
				fileSystemWatcher.IncludeSubdirectories = true;
			}
			else
			{
				fileSystemWatcher.IncludeSubdirectories = false;
			}
			fileSystemWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite);
			this.高危文件策略模式 = Conversions.ToInteger(Initializer.读注册表值(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 2));
			this.下载限制开关 = Conversions.ToString(Initializer.读注册表值(Registry.CurrentUser, "Software\\jfglzs", "xiazai", "off"));
			if (Operators.CompareString(this.下载限制开关, "off", false) == 0)
			{
				fileSystemWatcher.Renamed += this.删除下载文件;
			}
			if (this.高危文件策略模式 != 1)
			{
				fileSystemWatcher.Created += this.删除高危文件;
				fileSystemWatcher.Changed += this.删除高危文件;
			}
			fileSystemWatcher.EnableRaisingEvents = true;
		}
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00003FC0 File Offset: 0x000021C0
/// <summary>
/// 禁止浏览器游戏（Timer_youxi 30s）：读 Chrome AllowDinosaurEasterEgg / Edge AllowSurfGame，
/// 任一非 0 → 写回 0、Kill 所有 chrome/msedge、Form3 提示"你违规修改了注册表的禁止游戏项！"；
/// 全部为 0 时关闭 Form3 并复位标志。
/// </summary>
	private void 禁止浏览器游戏(object sender, EventArgs e)
	{
		try
		{
			// 读 Chrome 恐龙游戏 / Edge Surf 游戏开关
			int chromeDinoEgg = Conversions.ToInteger(Initializer.读注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "AllowDinosaurEasterEgg", ""));
			int edgeSurfGame = Conversions.ToInteger(Initializer.读注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "AllowSurfGame", ""));
			checked
			{
				// 任一非 0 → 违规：清零游戏开关并杀浏览器进程
				if (chromeDinoEgg != 0 | edgeSurfGame != 0)
				{
					Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "AllowDinosaurEasterEgg", 0, RegistryValueKind.DWord);
					Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "AllowSurfGame", 0, RegistryValueKind.DWord);
					foreach (Process chromeProcess in Process.GetProcessesByName("chrome"))
					{
						chromeProcess.Kill();
					}
					foreach (Process edgeProcess in Process.GetProcessesByName("msedge"))
					{
						edgeProcess.Kill();
					}
					this.indexer = true;
					Account.MyForms.Form3.shuoming.Text = "你违规修改了注册表的禁止游戏项！";
					Account.MyForms.Form3.shuoming2.Text = "现在助手强制恢复！";
					Account.MyForms.Form3.Show();
				}
				else
				{
					// 全部合规：关闭 Form3 并复位标志
					if (this.indexer)
					{
						Account.MyForms.Form3.Close();
					}
					this.indexer = false;
				}
			}
		}
		catch (Exception ex) { /* [反混淆] 原状态机124行 → 还原为正常控制流；VB On Error 样板已忽略 */ }
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00004264 File Offset: 0x00002464
/// <summary>
/// 重命名事件：删除下载文件。跨线程时 BeginInvoke 重入。
/// 扩展名 ∈ {zip,rar,7z,arj,exe,com,msi,reg,bat,cmd,vbs,vbe,scr,dll,ps1,pif} → 循环 3 次 File.Delete（间隔 200ms），
/// Form3 提示"本电脑禁止下载安装软件，再次操作会锁屏！"。
/// </summary>
	private void 删除下载文件(object sender, RenamedEventArgs e)
	{
		try
		{
			// 跨线程 → BeginInvoke 回 UI 线程处理
			if (this.InvokeRequired)
			{
				Action<object, RenamedEventArgs> method = new Action<object, RenamedEventArgs>(this.删除下载文件);
				this.BeginInvoke(method, new object[]
				{
					RuntimeHelpers.GetObjectValue(sender),
					e
				});
				return;
			}
			string extension = Path.GetExtension(e.Name).ToLower();
			// 16 种下载文件扩展名黑名单 → 循环 3 次删除（间隔 200ms）
			if (Operators.CompareString(extension, ".zip", false) == 0 | Operators.CompareString(extension, ".rar", false) == 0 | Operators.CompareString(extension, ".7z", false) == 0 | Operators.CompareString(extension, ".arj", false) == 0 | Operators.CompareString(extension, ".exe", false) == 0 | Operators.CompareString(extension, ".com", false) == 0 | Operators.CompareString(extension, ".msi", false) == 0 | Operators.CompareString(extension, ".reg", false) == 0 | Operators.CompareString(extension, ".bat", false) == 0 | Operators.CompareString(extension, ".cmd", false) == 0 | Operators.CompareString(extension, ".vbs", false) == 0 | Operators.CompareString(extension, ".vbe", false) == 0 | Operators.CompareString(extension, ".scr", false) == 0 | Operators.CompareString(extension, ".dll", false) == 0 | Operators.CompareString(extension, ".ps1", false) == 0 | Operators.CompareString(extension, ".pif", false) == 0)
			{
				checked
				{
					for (int i = 1; i <= 3; i++)
					{
						File.Delete(e.FullPath);
						Thread.Sleep(200);
					}
				}
				Account.MyForms.Form3.shuoming.Text = "本电脑禁止下载安装软件，再次操作会锁屏！";
				Account.MyForms.Form3.shuoming2.Text = "如果确有需要，请老师来退出小助手。";
				Account.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
				Account.MyForms.Form3.Show();
			}
		}
		catch (Exception ex) { /* [反混淆] 原状态机102行 → 还原为正常控制流；VB On Error 样板已忽略 */ }
	}

	// Token: 0x06000054 RID: 84 RVA: 0x0000455C File Offset: 0x0000275C
/// <summary>
/// 新建事件：删除 exe/高危文件。跨线程 BeginInvoke 重入。按扩展名分四类处理：
/// 1) .exe/.com：对应 .cpp/.cp 白名单编译产物不存在且策略为 2 或 0 → 按 MainModule.FileName 杀同名进程、循环删除；
/// 2) .reg/.bat/.cmd/.vbs/.vbe/.scr/.dll/.ps1/.pif：重试 3 次删除；
/// 3) .msi：路径含 config.msi 跳过（防误删系统安装缓存），否则重试 3 次删除；
/// 4) 其他扩展名：读前 2 字节，若为 0x4D 0x5A（MZ/PE 头）→ 杀同名进程并循环删除（变相 exe 检测）。
/// 均通过 Form3 红色提示。
/// </summary>
	private void 删除高危文件(object sender, FileSystemEventArgs e)
	{
		try
		{
			// 跨线程 → BeginInvoke 回 UI 线程处理
			if (this.InvokeRequired)
			{
				Action<object, FileSystemEventArgs> method = new Action<object, FileSystemEventArgs>(this.删除高危文件);
				this.BeginInvoke(method, new object[]
				{
					RuntimeHelpers.GetObjectValue(sender),
					e
				});
				return;
			}
			string extension = Path.GetExtension(e.Name).ToLower();
			checked
			{
				// 1) .exe/.com：白名单编译产物不存在且策略为 2 或 0 → 杀同名进程并循环删除
				if (Operators.CompareString(extension, ".exe", false) == 0 | Operators.CompareString(extension, ".com", false) == 0)
				{
					object compiledProductPath;
					if (Operators.CompareString(extension, ".exe", false) == 0)
					{
						compiledProductPath = Strings.Replace(e.FullPath, ".exe", ".cpp", 1, -1, CompareMethod.Binary);
					}
					else
					{
						compiledProductPath = Strings.Replace(e.FullPath, ".com", ".cp", 1, -1, CompareMethod.Binary);
					}
					string processName = Path.GetFileNameWithoutExtension(e.FullPath);
					if ((!File.Exists(Conversions.ToString(compiledProductPath)) & this.高危文件策略模式 == 2) | this.高危文件策略模式 == 0)
					{
						do
						{
							// 杀同名进程（MainModule 路径匹配）
							foreach (Process process in Process.GetProcessesByName(processName))
							{
								if (Operators.CompareString(process.MainModule.FileName.ToLower(), e.FullPath.ToLower(), false) == 0)
								{
									process.Kill();
									break;
								}
							}
							Thread.Sleep(500);
							File.SetAttributes(e.FullPath, FileAttributes.Normal);
							File.Delete(e.FullPath);
						}
						while (File.Exists(e.FullPath));
						Account.MyForms.Form3.shuoming.Text = "本电脑禁止创建新的exe文件，再次操作会锁屏！";
						Account.MyForms.Form3.shuoming2.Text = "老师请先退出小助手，重新安装软件。你也可重新设置：允许（不建议）";
						Account.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
						Account.MyForms.Form3.Show();
					}
				}
				// 2) .reg/.bat/.cmd/.vbs/.vbe/.scr/.dll/.ps1/.pif：重试 3 次删除
				else if (Operators.CompareString(extension, ".reg", false) == 0 | Operators.CompareString(extension, ".bat", false) == 0 | Operators.CompareString(extension, ".cmd", false) == 0 | Operators.CompareString(extension, ".vbs", false) == 0 | Operators.CompareString(extension, ".vbe", false) == 0 | Operators.CompareString(extension, ".scr", false) == 0 | Operators.CompareString(extension, ".dll", false) == 0 | Operators.CompareString(extension, ".ps1", false) == 0 | Operators.CompareString(extension, ".pif", false) == 0)
				{
					for (int i = 1; i <= 3; i++)
					{
						File.SetAttributes(e.FullPath, FileAttributes.Normal);
						File.Delete(e.FullPath);
						Thread.Sleep(200);
					}
					this.m_Attr++;
					Account.MyForms.Form3.shuoming.Text = "本电脑禁止创建新的reg、bat等高危文件，再次操作会锁屏！";
					Account.MyForms.Form3.shuoming2.Text = "老师请先退出小助手，重新安装软件。你也可重新设置：允许（不建议）";
					Account.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
					Account.MyForms.Form3.Show();
				}
				// 3) .msi：路径含 config.msi 跳过（系统安装缓存），否则重试 3 次删除
				else if (Operators.CompareString(extension, ".msi", false) == 0)
				{
					if (Strings.InStr(Strings.LCase(e.FullPath), "config.msi", CompareMethod.Binary) == 0)
					{
						for (int i = 1; i <= 3; i++)
						{
							File.SetAttributes(e.FullPath, FileAttributes.Normal);
							File.Delete(e.FullPath);
							Thread.Sleep(200);
						}
						Account.MyForms.Form3.shuoming.Text = "本电脑禁止创建新的msi等高危文件！";
						Account.MyForms.Form3.shuoming2.Text = "老师请先退出小助手，重新安装软件。你也可重新设置：允许（不建议）";
						Account.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
						Account.MyForms.Form3.Show();
					}
				}
				// 4) 其他扩展名：读前 2 字节，若为 0x4D 0x5A（MZ/PE 头）→ 杀同名进程并循环删除（变相 exe 检测）
				else
				{
					Thread.Sleep(500);
					bool isMZHeader = false;
					using (FileStream fileStream = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						byte[] header = new byte[2];
						fileStream.Read(header, 0, 2);
						isMZHeader = (header[0] == 77 & header[1] == 90);
					}
					if (isMZHeader)
					{
						string fileName = Path.GetFileName(e.FullPath);
						do
						{
							foreach (Process process in Process.GetProcessesByName(fileName))
							{
								if (Operators.CompareString(process.MainModule.FileName.ToLower(), e.FullPath.ToLower(), false) == 0)
								{
									process.Kill();
									break;
								}
							}
							File.SetAttributes(e.FullPath, FileAttributes.Normal);
							File.Delete(e.FullPath);
							Thread.Sleep(200);
						}
						while (File.Exists(e.FullPath));
						Account.MyForms.Form3.shuoming.Text = "本电脑禁止创建新的exe、dll变相文件，再次操作会锁屏！";
						Account.MyForms.Form3.shuoming2.Text = "老师请先退出小助手，重新安装软件。你也可重新设置：允许（不建议）";
						Account.MyForms.Form3.shuoming3.Text = "刚刚删除文件：" + e.FullPath;
						Account.MyForms.Form3.Show();
					}
				}
			}
		}
		catch (Exception ex) { /* [反混淆] 原状态机307行 → 还原为正常控制流；VB On Error 样板已忽略 */ }
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00004DD0 File Offset: 0x00002FD0
/// <summary>
/// 主加载/持久化（Form1.Load 事件，首启自举）：
/// 1. Hide() 隐藏主窗；2. 读 HKLM\...\RUN\prozs（缺则读 WOW6432NODE 分支）解析安装目录到 Initializer.程序安装目录；
/// 3. 由日期（月×日）派生随机 4 字符名 + Rnd() 生成 5 位数字名，目标目录 C:\Program Files (x86)\<随机4字符>；
///    若 StartupPath 不在安装目录（首次运行）→ 复制 przs.exe 为随机名.exe（Hidden|System），启动副本后 EndApp 结束自身；
/// 4. 二次启动：usb_jianche=on → 各 ControlSet 的 usbstor Start=4（禁用 USB）；xiazai=off → 写全量禁止下载策略并启用 Timer_jzxz；
/// 5. 对 Downloads/桌面/我的文档/所有逻辑盘根目录/360 下载目录/ChromeCoreDownloads/.NET Framework 目录安装文件监视。
/// </summary>
	private void 主加载持久化(object sender, EventArgs e)
	{
		try
		{
			this.Hide();
			// 读 Run 键自启动路径（64 位分支取空则读 WOW6432NODE 32 位分支）
			object selfPath = RuntimeHelpers.GetObjectValue(Initializer.读注册表值(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
			if (Operators.ConditionalCompareObjectEqual(selfPath, "", false))
			{
				selfPath = RuntimeHelpers.GetObjectValue(Initializer.读注册表值(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
			}
			checked
			{
				// 程序安装目录 = 自启动路径去掉末尾 9 字符（即去 "\przs.exe"）
				Initializer.程序安装目录 = Strings.Mid(Conversions.ToString(selfPath), 1, Strings.Len(RuntimeHelpers.GetObjectValue(selfPath)) - 9);
				// 由日期（月×日）派生随机 4 字符名
				int seed = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
				int m4 = seed % 7;
				int m5 = seed % 9;
				int m6 = seed % 5;
				int m7 = seed % 3;
				string randomName;
				if (seed % 2 == 0)
				{
					randomName = Conversions.ToString(Strings.Chr(97 + m4)) + Conversions.ToString(Strings.Chr(98 + m5)) + Conversions.ToString(Strings.Chr(101 + m6)) + Conversions.ToString(Strings.Chr(99 + m7));
				}
				else
				{
					randomName = Conversions.ToString(Strings.Chr(97 + m5)) + Conversions.ToString(Strings.Chr(98 + m4)) + Conversions.ToString(Strings.Chr(101 + m7)) + Conversions.ToString(Strings.Chr(99 + m6));
				}
				this.随机副本名 = randomName;
				this.随机副本exe名 = randomName + ".exe";
				// 注：以下用 Rnd() 生成的 5 位数字名会覆盖上面的 4 字符名（原状态机即如此，前者为死赋值）
				VBMath.Randomize((double)seed);
				long randomValue = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
				string digitsName = "";
				for (int i = 1; i <= 5; i++)
				{
					long charCode = randomValue % 10L + 105L;
					digitsName = Conversions.ToString(Strings.Chr((int)charCode)) + digitsName;
					randomValue /= 10L;
				}
				this.随机副本名 = digitsName;
				this.随机副本exe名 = digitsName + ".exe";
				string targetDir = "C:\\Program Files (x86)\\" + Strings.Mid(digitsName, 2, 4);
				this.Text = Strings.Mid(digitsName, 1, 3);
				// 首次运行（自身不在 C:\Program Files (x86)）→ 复制自身到受保护目录、启动副本后退出
				if (Strings.InStr(Application.StartupPath, "C:\\Program Files (x86)", CompareMethod.Binary) == 0)
				{
					this.NotifyIcon1.Visible = false;
					if (!Directory.Exists(targetDir))
					{
						Directory.CreateDirectory(targetDir);
					}
					File.SetAttributes(targetDir + "\\" + this.随机副本exe名, FileAttributes.Normal);
					File.Delete(targetDir + "\\" + this.随机副本exe名);
					File.Copy(Application.StartupPath + "\\przs.exe", targetDir + "\\" + this.随机副本exe名, true);
					File.SetAttributes(targetDir, File.GetAttributes(targetDir) | FileAttributes.Hidden | FileAttributes.System);
					File.SetAttributes(targetDir + "\\" + this.随机副本exe名, File.GetAttributes(targetDir + "\\" + this.随机副本exe名) | FileAttributes.Hidden | FileAttributes.System);
					if (!this.进程存在检测(this.随机副本名))
					{
						Process.Start(targetDir + "\\" + this.随机副本exe名);
					}
					ProjectData.EndApp();
				}
				else
				{
					// 已在安装目录运行（二次启动）
					// usb_jianche=on 且 usbstor Start ≠ 4 → 禁用 USB 存储（写 4 个 ControlSet）
					if (Operators.CompareString(Conversions.ToString(Initializer.读注册表值(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", "")), "on", false) == 0)
					{
						object usbStart = RuntimeHelpers.GetObjectValue(Initializer.读注册表值(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", ""));
						if (Operators.ConditionalCompareObjectNotEqual(usbStart, 4, false))
						{
							Initializer.写注册表值(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
						}
					}
					// xiazai=off → 校验 Chrome DownloadRestrictions ≠ 3 则写回全量禁止下载策略，并启用 Timer_jzxz
					this.下载限制开关 = Conversions.ToString(Initializer.读注册表值(Registry.CurrentUser, "Software\\jfglzs", "xiazai", "off"));
					if (Operators.CompareString(this.下载限制开关, "off", false) == 0)
					{
						object chromeDownload = RuntimeHelpers.GetObjectValue(Initializer.读注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", ""));
						if (Operators.ConditionalCompareObjectNotEqual(chromeDownload, 3, false))
						{
							Initializer.写注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 3, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 1, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 1, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 3, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 0, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 3, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 0, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 1, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 1, RegistryValueKind.DWord);
							Initializer.写注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DeveloperToolsAvailability", 2, RegistryValueKind.DWord);
						}
						this.Timer_jzxz.Enabled = true;
					}
					// 安装文件监视器：Downloads/桌面/我的文档/所有逻辑盘根目录
					string path360Safe = "";
					string path360Fast = "";
					this.安装目录监视器("C:\\Users\\" + Environment.UserName + "\\Downloads");
					this.安装目录监视器(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
					this.安装目录监视器(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
					foreach (string drive in Environment.GetLogicalDrives())
					{
						this.安装目录监视器(drive);
					}
					// 360安全浏览器下载目录：d→e→f→c 取第一个存在的
					if (Directory.Exists("d:\\" + Initializer.360安全浏览器下载目录名))
					{
						path360Safe = "d:\\" + Initializer.360安全浏览器下载目录名;
					}
					else if (Directory.Exists("e:\\" + Initializer.360安全浏览器下载目录名))
					{
						path360Safe = "e:\\" + Initializer.360安全浏览器下载目录名;
					}
					else if (Directory.Exists("f:\\" + Initializer.360安全浏览器下载目录名))
					{
						path360Safe = "f:\\" + Initializer.360安全浏览器下载目录名;
					}
					else if (Directory.Exists("c:\\" + Initializer.360安全浏览器下载目录名))
					{
						path360Safe = "c:\\" + Initializer.360安全浏览器下载目录名;
					}
					if (Operators.CompareString(path360Safe, "", false) != 0)
					{
						this.安装目录监视器(path360Safe);
					}
					// 360极速浏览器下载目录：d→e→f→c 取第一个存在的
					if (Directory.Exists("d:\\" + Initializer.360极速浏览器下载目录名))
					{
						path360Fast = "d:\\" + Initializer.360极速浏览器下载目录名;
					}
					else if (Directory.Exists("e:\\" + Initializer.360极速浏览器下载目录名))
					{
						path360Fast = "e:\\" + Initializer.360极速浏览器下载目录名;
					}
					else if (Directory.Exists("f:\\" + Initializer.360极速浏览器下载目录名))
					{
						path360Fast = "f:\\" + Initializer.360极速浏览器下载目录名;
					}
					else if (Directory.Exists("c:\\" + Initializer.360极速浏览器下载目录名))
					{
						path360Fast = "c:\\" + Initializer.360极速浏览器下载目录名;
					}
					if (Operators.CompareString(path360Fast, "", false) != 0)
					{
						this.安装目录监视器(path360Fast);
					}
					if (Directory.Exists("D:\\ChromeCoreDownloads"))
					{
						this.安装目录监视器("D:\\ChromeCoreDownloads");
					}
					if (Directory.Exists("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319"))
					{
						this.安装目录监视器("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319");
					}
				}
			}
		}
		catch (Exception ex) { /* [反混淆] 原状态机422行 → 还原为正常控制流；VB On Error 样板已忽略 */ }
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00005994 File Offset: 0x00003B94
/// <summary>
/// 托盘图标点击：Process.Start(程序安装目录\about.exe) 启动说明程序。
/// </summary>
	private void 托盘图标点击(object sender, EventArgs e)
	{
		try
		{
			// 启动说明程序 about.exe
			NewLateBinding.LateCall(null, typeof(Process), "Start", new object[]
			{
				Operators.ConcatenateObject(Initializer.程序安装目录, "\\about.exe")
			}, null, null, null, true);
		}
		catch (Exception ex) { /* [反混淆] 原状态机48行 → 还原为正常控制流；VB On Error 样板已忽略 */ }
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00005A5C File Offset: 0x00003C5C
/// <summary>
/// 进程存在检测：Process.GetProcessesByName(name).Length > 0。
/// </summary>
	private bool 进程存在检测(string string_0)
	{
		return Process.GetProcessesByName(string_0).Length > 0;
	}

	// Token: 0x04000011 RID: 17
	[AccessedThroughProperty("NotifyIcon1")]
/// <summary>托盘图标字段（NotifyIcon1 后备字段）。</summary>
	private NotifyIcon @event;

	// Token: 0x04000012 RID: 18
	[AccessedThroughProperty("Splitter1")]
/// <summary>分隔条字段（Splitter1 后备字段）。</summary>
	private Splitter decorator;

	// Token: 0x04000013 RID: 19
	[AccessedThroughProperty("Timer_main")]
/// <summary>Timer_main 后备字段。</summary>
	private System.Windows.Forms.Timer request;

	// Token: 0x04000014 RID: 20
	[AccessedThroughProperty("Timer2")]
/// <summary>Timer2 后备字段。</summary>
	private System.Windows.Forms.Timer _Tokenizer;

	// Token: 0x04000015 RID: 21
	[AccessedThroughProperty("Timer1")]
/// <summary>Timer1 后备字段。</summary>
	private System.Windows.Forms.Timer rule;

	// Token: 0x04000016 RID: 22
	[AccessedThroughProperty("Timer4")]
/// <summary>Timer4 后备字段。</summary>
	private System.Windows.Forms.Timer m_State;

	// Token: 0x04000017 RID: 23
	[AccessedThroughProperty("Timer_jianchahosts")]
/// <summary>Timer_jianchahosts 后备字段。</summary>
	private System.Windows.Forms.Timer m_Record;

	// Token: 0x04000018 RID: 24
	[AccessedThroughProperty("Timer_jzxz")]
/// <summary>Timer_jzxz 后备字段。</summary>
	private System.Windows.Forms.Timer _Test;

	// Token: 0x04000019 RID: 25
	[AccessedThroughProperty("Timer_youxi")]
/// <summary>Timer_youxi 后备字段。</summary>
	private System.Windows.Forms.Timer val;

	// Token: 0x0400001A RID: 26
/// <summary>未使用计数器（初始 0，混淆残留）。</summary>
	private long m_Proccesor;

	// Token: 0x0400001B RID: 27
/// <summary>随机副本名：由日期派生 4 字符（Chr(97+…)系列）后再覆盖为 Rnd() 生成的 5 位数字串。</summary>
	private string 随机副本名;

	// Token: 0x0400001C RID: 28
/// <summary>随机副本 exe 名：随机副本名 + ".exe"（写入 C:\Program Files (x86)\ 的副本文件名）。</summary>
	private string 随机副本exe名;

	// Token: 0x0400001D RID: 29
/// <summary>未使用字符串字段（混淆残留）。</summary>
	private string @ref;

	// Token: 0x0400001E RID: 30
/// <summary>下载限制开关：读 HKCU\Software\jfglzs\xiazai（"off"=启用禁止下载策略并监视）。</summary>
	private string 下载限制开关;

	// Token: 0x0400001F RID: 31
/// <summary>高危文件策略模式：读 HKCU\Software\jfglzs\gaowei（0=严格/1=放行/2=条件删除，见删除高危文件）。</summary>
	private int 高危文件策略模式;

	// Token: 0x04000020 RID: 32
/// <summary>未使用计数器（初始 0，混淆残留）。</summary>
	private int _Reponse;

	// Token: 0x04000021 RID: 33
/// <summary>高危文件删除计数（自增但未使用，混淆残留）。</summary>
	private int m_Attr;

	// Token: 0x04000022 RID: 34
/// <summary>重生计数器：exe 自愈重生次数，阈值 40（达到后提示杀软干扰并弹 Form2）。</summary>
	private long 重生计数器;

	// Token: 0x04000023 RID: 35
/// <summary>违规修改注册表计数器：禁止下载策略被改次数，阈值 15（超过后 Form2 提示重启电脑）。</summary>
	private long 违规修改注册表计数器;

	// Token: 0x04000024 RID: 36
/// <summary>下载策略违规标志：违规时置 true，恢复后复位（控制 Form3 关闭）。</summary>
	private bool m_Context;

	// Token: 0x04000025 RID: 37
/// <summary>hosts 被删标志：hosts 缺失时置 true，恢复后复位（控制 Form3 关闭）。</summary>
	private bool m_Facade;

	// Token: 0x04000026 RID: 38
/// <summary>游戏违规标志：禁止浏览器游戏被触发时置 true，恢复后复位。</summary>
	private bool indexer;

	// Token: 0x04000027 RID: 39
/// <summary>hosts 恢复计数器：hosts 被删次数，阈值 5（超过后 Form2 提示）。</summary>
	private long hosts恢复计数器;

	// Token: 0x04000028 RID: 40
/// <summary>备用程序目录："C:\windows\jf"，存放 jfglzsn.dat 备份（exe 自愈重生用）。</summary>
	private string 备用程序目录;

	// Token: 0x04000029 RID: 41
/// <summary>hosts 路径："C:\Windows\System32\drivers\etc\hosts"（hosts 自愈监视对象）。</summary>
	private string hosts路径;

	// Token: 0x0400002A RID: 42
/// <summary>未使用字符串字段（混淆残留）。</summary>
	private string m_Predicate;

	// Token: 0x0400002B RID: 43
/// <summary>未使用长整型字段（初始 4194304，混淆残留）。</summary>
	private long m_Base;
}
