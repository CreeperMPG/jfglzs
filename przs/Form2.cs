// ============================================================================
// Form2.cs — 全屏警告/锁屏窗（退出密码验证 + 鼠标锁定 + 强制重启）
// 真实职责：违规告警与拦截主窗口。无边框全屏 TopMost，FormClosing 被取消不可关闭；
// 红色大字显示违规说明；输入 SHA256 加盐密码（比对 HKCU\Software\n）方可退出；
// 双击隐藏的 Label_pd 进入教师维护后门（泄露明文密码串、写 dtmm/czmm、启动 yz.exe）；
// 提供"重启电脑"按钮（shutdown /r /f /t 0）与 ClipCursor 顶部 100px 锁鼠标（1s 定时器）。
// 说明：ConfuserEx 混淆反编译产物，标识符已按 Form2.cs.md 映射表反混淆重命名。
// ============================================================================
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using gypZRX1vuWc3aeye0R.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

// Token: 0x0200000A RID: 10
[DesignerGenerated]
public partial class Form2 : Form
{
	// Token: 0x06000058 RID: 88 RVA: 0x00005A7C File Offset: 0x00003C7C
/// <summary>
/// 构造：订阅 FormClosing（阻止关闭）与 Load（启用定时器），随后初始化组件。
/// </summary>
	public Form2()
	{
		base.FormClosing += this.阻止窗体关闭;
		base.Load += this.启用锁鼠标定时器;
		this.初始化组件();
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00005AFC File Offset: 0x00003CFC
/// <summary>
/// 初始化组件：创建标签/按钮/文本框/隐藏后门标签 Label_pd 与 1s 定时器 Timer_frm2 并排版。
/// </summary>
	private void 初始化组件()
	{
		this.m_Page = new Container();
		this.shuoming = new Label();
		this.Button2 = new Button();
		this.Label2 = new Label();
		this.Label1 = new Label();
		this.TextBox1 = new TextBox();
		this.Button1 = new Button();
		this.Timer_frm2 = new System.Windows.Forms.Timer(this.m_Page);
		this.Label_pd = new Label();
		this.shuoming2 = new Label();
		this.shuoming3 = new Label();
		this.SuspendLayout();
		this.shuoming.AutoSize = true;
		this.shuoming.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming.ForeColor = Color.Red;
		Control shuoming = this.shuoming;
		Point location = new Point(160, 221);
		shuoming.Location = location;
		this.shuoming.Name = "shuoming";
		Control shuoming2 = this.shuoming;
		Size size = new Size(33, 35);
		shuoming2.Size = size;
		this.shuoming.TabIndex = 11;
		this.shuoming.Text = " ";
		Control button = this.Button2;
		location = new Point(382, 20);
		button.Location = location;
		this.Button2.Name = "Button2";
		Control button2 = this.Button2;
		size = new Size(82, 28);
		button2.Size = size;
		this.Button2.TabIndex = 10;
		this.Button2.Text = "重启电脑";
		this.Button2.UseVisualStyleBackColor = true;
		this.Label2.AutoSize = true;
		this.Label2.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label2.ForeColor = Color.DarkRed;
		Control label = this.Label2;
		location = new Point(27, 68);
		label.Location = location;
		this.Label2.Name = "Label2";
		Control label2 = this.Label2;
		size = new Size(0, 14);
		label2.Size = size;
		this.Label2.TabIndex = 9;
		this.Label1.AutoSize = true;
		this.Label1.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label1.ForeColor = Color.White;
		Control label3 = this.Label1;
		location = new Point(54, 26);
		label3.Location = location;
		this.Label1.Name = "Label1";
		Control label4 = this.Label1;
		size = new Size(119, 14);
		label4.Size = size;
		this.Label1.TabIndex = 8;
		this.Label1.Text = "退出小助手密码：";
		Control textBox = this.TextBox1;
		location = new Point(189, 22);
		textBox.Location = location;
		this.TextBox1.Name = "TextBox1";
		this.TextBox1.PasswordChar = '*';
		Control textBox2 = this.TextBox1;
		size = new Size(104, 21);
		textBox2.Size = size;
		this.TextBox1.TabIndex = 7;
		Control button3 = this.Button1;
		location = new Point(304, 19);
		button3.Location = location;
		this.Button1.Name = "Button1";
		Control button4 = this.Button1;
		size = new Size(54, 27);
		button4.Size = size;
		this.Button1.TabIndex = 6;
		this.Button1.Text = "确 定";
		this.Button1.UseVisualStyleBackColor = true;
		this.Timer_frm2.Interval = 1000;
		this.Label_pd.AutoSize = true;
		this.Label_pd.BackColor = SystemColors.Highlight;
		this.Label_pd.Font = new Font("宋体", 24f, FontStyle.Regular, GraphicsUnit.Point, 134);
		Control label_pd = this.Label_pd;
		location = new Point(470, 19);
		label_pd.Location = location;
		this.Label_pd.Name = "Label_pd";
		Control label_pd2 = this.Label_pd;
		size = new Size(95, 33);
		label_pd2.Size = size;
		this.Label_pd.TabIndex = 12;
		this.Label_pd.Text = "     ";
		this.shuoming2.AutoSize = true;
		this.shuoming2.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming2.ForeColor = Color.Red;
		Control shuoming3 = this.shuoming2;
		location = new Point(160, 301);
		shuoming3.Location = location;
		this.shuoming2.Name = "shuoming2";
		Control shuoming4 = this.shuoming2;
		size = new Size(33, 35);
		shuoming4.Size = size;
		this.shuoming2.TabIndex = 18;
		this.shuoming2.Text = " ";
		this.shuoming3.AutoSize = true;
		Control shuoming5 = this.shuoming3;
		location = new Point(173, 431);
		shuoming5.Location = location;
		this.shuoming3.Name = "shuoming3";
		Control shuoming6 = this.shuoming3;
		size = new Size(0, 12);
		shuoming6.Size = size;
		this.shuoming3.TabIndex = 22;
		SizeF autoScaleDimensions = new SizeF(6f, 12f);
		this.AutoScaleDimensions = autoScaleDimensions;
		this.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = SystemColors.Highlight;
		size = new Size(918, 727);
		this.ClientSize = size;
		this.Controls.Add(this.shuoming3);
		this.Controls.Add(this.shuoming2);
		this.Controls.Add(this.Label_pd);
		this.Controls.Add(this.shuoming);
		this.Controls.Add(this.Button2);
		this.Controls.Add(this.Label2);
		this.Controls.Add(this.Label1);
		this.Controls.Add(this.TextBox1);
		this.Controls.Add(this.Button1);
		this.FormBorderStyle = FormBorderStyle.None;
		this.Name = "Form2";
		this.ShowInTaskbar = false;
		this.Text = "Form2";
		this.TopMost = true;
		this.WindowState = FormWindowState.Maximized;
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600005B RID: 91 RVA: 0x00006138 File Offset: 0x00004338
	// (set) Token: 0x0600005C RID: 92 RVA: 0x0000223D File Offset: 0x0000043D
/// <summary>主说明 Label（红色大字，违规说明第一行）。</summary>
	internal virtual Label shuoming
	{
		get
		{
			return this.m_Consumer;
		}
		set
		{
			this.m_Consumer = value;
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x0600005D RID: 93 RVA: 0x0000614C File Offset: 0x0000434C
	// (set) Token: 0x0600005E RID: 94 RVA: 0x00006160 File Offset: 0x00004360
/// <summary>"重启电脑"按钮；Click → 重启电脑按钮（shutdown /r /f /t 0）。</summary>
	internal virtual Button Button2
	{
		get
		{
			return this._Candidate;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.重启电脑按钮);
			if (this._Candidate != null)
			{
				this._Candidate.Click -= value2;
			}
			this._Candidate = value;
			if (this._Candidate != null)
			{
				this._Candidate.Click += value2;
			}
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x0600005F RID: 95 RVA: 0x000061AC File Offset: 0x000043AC
	// (set) Token: 0x06000060 RID: 96 RVA: 0x00002246 File Offset: 0x00000446
/// <summary>提示 Label（深红小字，密码校验/后门密码结果显示）。</summary>
	internal virtual Label Label2
	{
		get
		{
			return this.advisor;
		}
		set
		{
			this.advisor = value;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000061 RID: 97 RVA: 0x000061C0 File Offset: 0x000043C0
	// (set) Token: 0x06000062 RID: 98 RVA: 0x0000224F File Offset: 0x0000044F
/// <summary>"退出小助手密码：" 标签。</summary>
	internal virtual Label Label1
	{
		get
		{
			return this.m_Code;
		}
		set
		{
			this.m_Code = value;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000063 RID: 99 RVA: 0x000061D4 File Offset: 0x000043D4
	// (set) Token: 0x06000064 RID: 100 RVA: 0x00002258 File Offset: 0x00000458
/// <summary>密码输入框（PasswordChar='*'）。</summary>
	internal virtual TextBox TextBox1
	{
		get
		{
			return this._Method;
		}
		set
		{
			this._Method = value;
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000065 RID: 101 RVA: 0x000061E8 File Offset: 0x000043E8
	// (set) Token: 0x06000066 RID: 102 RVA: 0x000061FC File Offset: 0x000043FC
/// <summary>"确 定"按钮；Click → 验证退出密码。</summary>
	internal virtual Button Button1
	{
		get
		{
			return this.m_Mapping;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.验证退出密码);
			if (this.m_Mapping != null)
			{
				this.m_Mapping.Click -= value2;
			}
			this.m_Mapping = value;
			if (this.m_Mapping != null)
			{
				this.m_Mapping.Click += value2;
			}
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000067 RID: 103 RVA: 0x00006248 File Offset: 0x00004448
	// (set) Token: 0x06000068 RID: 104 RVA: 0x0000625C File Offset: 0x0000445C
/// <summary>Timer_frm2（1s）；Tick → 锁鼠标定时器（ClipCursor 顶部 100px + 蜂鸣）。</summary>
	internal virtual System.Windows.Forms.Timer Timer_frm2
	{
		get
		{
			return this.dic;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.锁鼠标定时器);
			if (this.dic != null)
			{
				this.dic.Tick -= value2;
			}
			this.dic = value;
			if (this.dic != null)
			{
				this.dic.Tick += value2;
			}
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000069 RID: 105 RVA: 0x000062A8 File Offset: 0x000044A8
	// (set) Token: 0x0600006A RID: 106 RVA: 0x000062BC File Offset: 0x000044BC
/// <summary>隐藏后门标签；DoubleClick → 后门维护入口（泄露维护密码并启动 yz.exe）。</summary>
	internal virtual Label Label_pd
	{
		get
		{
			return this._Params;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.后门维护入口);
			if (this._Params != null)
			{
				this._Params.DoubleClick -= value2;
			}
			this._Params = value;
			if (this._Params != null)
			{
				this._Params.DoubleClick += value2;
			}
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600006B RID: 107 RVA: 0x00006308 File Offset: 0x00004508
	// (set) Token: 0x0600006C RID: 108 RVA: 0x00002261 File Offset: 0x00000461
/// <summary>次级说明 Label（红色大字，第二行）。</summary>
	internal virtual Label shuoming2
	{
		get
		{
			return this._Customer;
		}
		set
		{
			this._Customer = value;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x0600006D RID: 109 RVA: 0x0000631C File Offset: 0x0000451C
	// (set) Token: 0x0600006E RID: 110 RVA: 0x0000226A File Offset: 0x0000046A
/// <summary>详情 Label（默认字号，第三行）。</summary>
	internal virtual Label shuoming3
	{
		get
		{
			return this.prototype;
		}
		set
		{
			this.prototype = value;
		}
	}

	// Token: 0x0600006F RID: 111
	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
/// <summary>Win32 user32!ClipCursor：限制鼠标活动区域（锁屏用）。</summary>
	public static extern int ClipCursor(Form2.RECT lpRect);

	// Token: 0x06000070 RID: 112
	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
/// <summary>Win32 kernel32!Beep：蜂鸣警告（锁屏 5 次后每次触发）。</summary>
	private static extern long Beep(long long_0, long long_1);

	// Token: 0x06000071 RID: 113 RVA: 0x00006330 File Offset: 0x00004530
/// <summary>
/// 确定按钮：验证退出密码。1. 长度<6 → 提示"密码应该是6位以上的字母或数字，请重新输入！"并清空；
/// 2. 否则 Initializer.口令加盐哈希(Trim(输入))（SHA256(输入+"bfdshgs") 十六进制）与 HKCU\Software\n 比对；
/// 3. 相等 → 清空提示、禁用按钮、执行退出流程；不等 → "密码不正确，请重新输入！"。
/// </summary>
	private void 验证退出密码(object sender, EventArgs e)
	{
		// [反混淆] 原状态机85行 → 还原为正常控制流
		try
		{
			// 密码长度不足 6 位 → 提示并清空输入框
			if (Strings.Len(this.TextBox1.Text) < 6)
			{
				this.Label2.Text = "密码应该是6位以上的字母或数字，请重新输入！";
				this.TextBox1.Text = "";
				return;
			}
			// 长度合法 → SHA256(Trim(输入)+盐) 与 HKCU\Software\n 比对
			string passwordHash = Initializer.口令加盐哈希(Strings.Trim(this.TextBox1.Text));
			string storedHash = Conversions.ToString(Initializer.读注册表值(Registry.CurrentUser, "Software", "n", ""));
			if (Operators.CompareString(passwordHash, storedHash, false) == 0)
			{
				// 密码正确 → 清空提示、禁用按钮、执行退出流程
				this.Label2.Text = "";
				this.Button1.Enabled = false;
				this.执行退出流程();
			}
			else
			{
				// 密码错误 → 提示并清空输入框
				this.Label2.Text = "密码不正确，请重新输入！";
				this.TextBox1.Text = "";
			}
		}
		catch (Exception ex)
		{
			// [反混淆] 原 VB On Error 状态机样板（endfilter / ProjectData.CreateProjectError(-2146828237)），错误不再传播，等价忽略
		}
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00006500 File Offset: 0x00004700
/// <summary>
/// 锁鼠标定时器（Timer_frm2 1s Tick）：TopMost=true；ClipCursor 到屏幕顶部 100px 高区域；
/// 锁屏计时计数>=5 后每次 Beep(2000, 500) 蜂鸣警告。
/// </summary>
	private void 锁鼠标定时器(object sender, EventArgs e)
	{
		this.TopMost = true;
		Form2.RECT lpRect;
		lpRect.Left = 0;
		lpRect.Top = 0;
		lpRect.Right = 500;
		lpRect.Bottom = 100;
		Form2.ClipCursor(lpRect);
		checked
		{
			this.锁屏计时计数++;
			if (this.锁屏计时计数 >= 5)
			{
				Form2.Beep(2000L, 500L);
			}
		}
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00006570 File Offset: 0x00004770
/// <summary>
/// Label_pd 双击后门：Label2 显示维护密码串 "ngfxd568bdh158" + 本机编号 + "8bsdg45ybfdhg"；
/// 把当前输入框内容明文写入 HKCU\Software\dtmm；写 HKCU\Software\czmm = 口令加盐哈希("111111")；
/// 启动 程序安装目录\yz.exe（教师验证工具）。
/// </summary>
	private void 后门维护入口(object sender, EventArgs e)
	{
		// [反混淆] 原状态机61行 → 还原为正常控制流
		try
		{
			// 泄露教师维护密码串：固定前缀 + 本机编号 + 固定后缀（编号 = 计算机名末字符 ASCII）
			this.Label2.Text = "ngfxd568bdh158" + Conversions.ToString(this.取本机编号()) + "8bsdg45ybfdhg";
			// 当前输入框内容明文写入 HKCU\Software\dtmm
			object passwordInput = this.TextBox1.Text;
			Initializer.写注册表值(Registry.CurrentUser, "Software", "dtmm", RuntimeHelpers.GetObjectValue(passwordInput), RegistryValueKind.String);
			// 写 HKCU\Software\czmm = 口令加盐哈希("111111")（重置密码哈希）
			object resetPasswordHash = Initializer.口令加盐哈希("111111");
			Initializer.写注册表值(Registry.CurrentUser, "Software", "czmm", RuntimeHelpers.GetObjectValue(resetPasswordHash), RegistryValueKind.String);
			// 启动 程序安装目录\yz.exe（教师验证工具）；[反混淆] 原为 NewLateBinding.LateCall(typeof(Process), "Start", {安装目录 + "\\yz.exe"})
			Process.Start(Initializer.程序安装目录 + "\\yz.exe");
		}
		catch (Exception ex)
		{
			// [反混淆] 原 VB On Error 状态机样板（endfilter / ProjectData.CreateProjectError(-2146828237)），错误不再传播，等价忽略
		}
	}

	// Token: 0x06000074 RID: 116 RVA: 0x000066E0 File Offset: 0x000048E0
/// <summary>
/// 执行退出流程：1. 循环 Kill 所有 jfglzsn 进程（间隔 500ms）直至杀光；
/// 2. 写 HKCU\...\System\DisableCMD=0（解除禁用 CMD）；3. Shell("net stop zmserv") 停止机房管理服务；
/// 4. Sleep(3000)；5. Kill zmserv 进程；6. ClipCursor 恢复全屏区域；7. EndApp() 退出本程序。
/// </summary>
	private void 执行退出流程()
	{
		// [反混淆] 原状态机101行 → 还原为正常控制流
		try
		{
			string serviceName = "zmserv";
			string processName = "jfglzsn";
			// 循环 Kill 所有 jfglzsn 进程（间隔 500ms）直至杀光（保持原状态机 for(;;) 与 [0].Kill() 顺序）
			for (;;)
			{
				Process[] jfglzsnProcesses = Process.GetProcessesByName(processName);
				jfglzsnProcesses[0].Kill();
				if (jfglzsnProcesses.Length == 0)
				{
					break;
				}
				Thread.Sleep(500);
			}
			// 解除禁用 CMD：HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD = 0
			Initializer.写注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
			// 停止机房管理服务 zmserv
			Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
			Thread.Sleep(3000);
			// 杀 zmserv 进程
			Process[] zmservProcesses = Process.GetProcessesByName(serviceName);
			zmservProcesses[0].Kill();
			// ClipCursor 恢复全屏区域（解除鼠标锁定）
			Form2.RECT lpRect;
			lpRect.Left = 0;
			lpRect.Top = 0;
			lpRect.Right = Screen.PrimaryScreen.Bounds.Width;
			lpRect.Bottom = Screen.PrimaryScreen.Bounds.Height;
			Form2.ClipCursor(lpRect);
			// 退出本程序
			ProjectData.EndApp();
		}
		catch (Exception ex)
		{
			// [反混淆] 原 VB On Error 状态机样板（endfilter / ProjectData.CreateProjectError(-2146828237)），错误不再传播，等价忽略
		}
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00002273 File Offset: 0x00000473
/// <summary>
/// 阻止窗体关闭：e.Cancel = true（Form2 不可通过关闭按钮退出）。
/// </summary>
	private void 阻止窗体关闭(object sender, FormClosingEventArgs e)
	{
		e.Cancel = true;
	}

	// Token: 0x06000076 RID: 118 RVA: 0x0000227C File Offset: 0x0000047C
/// <summary>
/// Load 事件：启用 Timer_frm2（开始锁鼠标循环）。
/// </summary>
	private void 启用锁鼠标定时器(object sender, EventArgs e)
	{
		this.Timer_frm2.Enabled = true;
	}

	// Token: 0x06000077 RID: 119 RVA: 0x000068F4 File Offset: 0x00004AF4
/// <summary>
/// 重启电脑按钮：Interaction.Shell("shutdown /r /f /t 0") 立即强制重启。
/// </summary>
	private void 重启电脑按钮(object sender, EventArgs e)
	{
		// [反混淆] 原状态机44行 → 还原为正常控制流
		try
		{
			// 立即强制重启（shutdown /r /f /t 0：重启、强制关闭程序、无倒计时）
			Interaction.Shell("shutdown /r /f /t 0", AppWinStyle.MinimizedFocus, false, -1);
		}
		catch (Exception ex)
		{
			// [反混淆] 原 VB On Error 状态机样板（endfilter / ProjectData.CreateProjectError(-2146828237)），错误不再传播，等价忽略
		}
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00006998 File Offset: 0x00004B98
/// <summary>
/// 取本机编号：计算机名（COMPUTERNAME）末字符的 ASCII 码；名称为空时返回 88。
/// </summary>
	public int 取本机编号()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("COMPUTERNAME");
		int result;
		if (environmentVariable.Length <= 0)
		{
			result = 88;
		}
		else
		{
			char @string = environmentVariable[checked(environmentVariable.Length - 1)];
			result = Strings.Asc(@string);
		}
		return result;
	}

	// Token: 0x0400002D RID: 45
	[AccessedThroughProperty("shuoming")]
/// <summary>控件字段：shuoming 后备字段。</summary>
	private Label m_Consumer;

	// Token: 0x0400002E RID: 46
	[AccessedThroughProperty("Button2")]
/// <summary>控件字段：Button2 后备字段。</summary>
	private Button _Candidate;

	// Token: 0x0400002F RID: 47
	[AccessedThroughProperty("Label2")]
/// <summary>控件字段：Label2 后备字段。</summary>
	private Label advisor;

	// Token: 0x04000030 RID: 48
	[AccessedThroughProperty("Label1")]
/// <summary>控件字段：Label1 后备字段。</summary>
	private Label m_Code;

	// Token: 0x04000031 RID: 49
	[AccessedThroughProperty("TextBox1")]
/// <summary>控件字段：TextBox1 后备字段。</summary>
	private TextBox _Method;

	// Token: 0x04000032 RID: 50
	[AccessedThroughProperty("Button1")]
/// <summary>控件字段：Button1 后备字段。</summary>
	private Button m_Mapping;

	// Token: 0x04000033 RID: 51
	[AccessedThroughProperty("Timer_frm2")]
/// <summary>控件字段：Timer_frm2 后备字段。</summary>
	private System.Windows.Forms.Timer dic;

	// Token: 0x04000034 RID: 52
	[AccessedThroughProperty("Label_pd")]
/// <summary>控件字段：Label_pd 后备字段。</summary>
	private Label _Params;

	// Token: 0x04000035 RID: 53
	[AccessedThroughProperty("shuoming2")]
/// <summary>控件字段：shuoming2 后备字段。</summary>
	private Label _Customer;

	// Token: 0x04000036 RID: 54
	[AccessedThroughProperty("shuoming3")]
/// <summary>控件字段：shuoming3 后备字段。</summary>
	private Label prototype;

	// Token: 0x04000037 RID: 55
/// <summary>锁屏计时计数：达到 5 次开始每次蜂鸣（Beep 2000/500ms）。</summary>
	private int 锁屏计时计数;

	// Token: 0x0200000B RID: 11
/// <summary>Win32 RECT 结构（ClipCursor 参数）。</summary>
	public struct RECT
	{
		// Token: 0x04000038 RID: 56
/// <summary>矩形左边界。</summary>
		public int Left;

		// Token: 0x04000039 RID: 57
/// <summary>矩形上边界。</summary>
		public int Top;

		// Token: 0x0400003A RID: 58
/// <summary>矩形右边界。</summary>
		public int Right;

		// Token: 0x0400003B RID: 59
/// <summary>矩形下边界。</summary>
		public int Bottom;
	}
}
