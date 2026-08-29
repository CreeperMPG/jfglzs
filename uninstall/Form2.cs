using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using WindowsApplication1.Internal;

namespace WindowsApplication1
{
	// Token: 0x02000009 RID: 9
	// <summary>
	/// 卸载确认窗体（"卸载小助手"）：全屏、置顶、不可关闭的强制交互界面。
	/// 要求输入 6 位以上密码（哈希存于 HKCU\Software\n），验证通过后执行完整卸载
	/// （RegConfig.RunFullUninstall），恢复默认壁纸、锁定鼠标、7 秒后退出；
	/// 另提供"重启电脑"按钮（shutdown /r /f /t 0）与 QQ/官网联系方式。
	/// Timer_frm2 每秒把鼠标夹在屏幕顶部区域并周期性蜂鸣。
	/// </summary>
	[DesignerGenerated]
	public partial class Form2 : Form
	{
		// Token: 0x0600002E RID: 46 RVA: 0x00002D74 File Offset: 0x00000F74
		/// <summary>
		/// 构造函数：拦截 FormClosing（禁止关闭）、挂接 Load 事件并初始化全部控件布局。
		/// </summary>
		public Form2()
		{
			base.FormClosing += this.Form2_FormClosing;
			base.Load += this.Form2_Load;
			this.InitializeFormLayout();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002DF4 File Offset: 0x00000FF4
		/// <summary>
		/// 初始化窗体控件/布局（原混淆名 TestUtils）：构建全部控件（密码框 TextBox1、
		/// 按钮 Button1/Button2、提示 Label1~Label4、隐藏双击触发 Label Label_pd）与
		/// 1 秒定时器 Timer_frm2；窗体设为全屏置顶（Maximized、TopMost、无边框、
		/// 不在任务栏显示）。控件 set 时自动挂接相应事件处理器。
		/// </summary>
		private void InitializeFormLayout()
		{
			this._InfoConfig = new Container();
			this.Button2 = new Button();
			this.Label1 = new Label();
			this.TextBox1 = new TextBox();
			this.Button1 = new Button();
			this.Timer_frm2 = new System.Windows.Forms.Timer(this._InfoConfig);
			this.Label2 = new Label();
			this.Label4 = new Label();
			this.Label3 = new Label();
			this.Label_pd = new Label();
			this.SuspendLayout();
			Control button = this.Button2;
			Point location = new Point(397, 27);
			button.Location = location;
			this.Button2.Name = "Button2";
			Control button2 = this.Button2;
			Size size = new Size(84, 26);
			button2.Size = size;
			this.Button2.TabIndex = 19;
			this.Button2.Text = "重启电脑";
			this.Button2.UseVisualStyleBackColor = true;
			this.Label1.AutoSize = true;
			this.Label1.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label1.ForeColor = Color.White;
			Control label = this.Label1;
			location = new Point(39, 31);
			label.Location = location;
			this.Label1.Name = "Label1";
			Control label2 = this.Label1;
			size = new Size(119, 14);
			label2.Size = size;
			this.Label1.TabIndex = 18;
			this.Label1.Text = "卸载小助手密码：";
			Control textBox = this.TextBox1;
			location = new Point(164, 29);
			textBox.Location = location;
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.PasswordChar = '*';
			Control textBox2 = this.TextBox1;
			size = new Size(104, 21);
			textBox2.Size = size;
			this.TextBox1.TabIndex = 17;
			Control button3 = this.Button1;
			location = new Point(284, 26);
			button3.Location = location;
			this.Button1.Name = "Button1";
			Control button4 = this.Button1;
			size = new Size(92, 27);
			button4.Size = size;
			this.Button1.TabIndex = 16;
			this.Button1.Text = "确定(需10秒)";
			this.Button1.UseVisualStyleBackColor = true;
			this.Timer_frm2.Interval = 1000;
			this.Label2.AutoSize = true;
			this.Label2.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label2.ForeColor = Color.DarkRed;
			Control label3 = this.Label2;
			location = new Point(32, 63);
			label3.Location = location;
			this.Label2.Name = "Label2";
			Control label4 = this.Label2;
			size = new Size(0, 14);
			label4.Size = size;
			this.Label2.TabIndex = 20;
			this.Label4.AutoSize = true;
			Control label5 = this.Label4;
			location = new Point(77, 135);
			label5.Location = location;
			this.Label4.Name = "Label4";
			Control label6 = this.Label4;
			size = new Size(299, 12);
			label6.Size = size;
			this.Label4.TabIndex = 23;
			this.Label4.Text = "2、卸载后记得在硬盘保护系统中创建进度或保存系统。";
			this.Label3.AutoSize = true;
			Control label7 = this.Label3;
			location = new Point(40, 104);
			label7.Location = location;
			this.Label3.Name = "Label3";
			Control label8 = this.Label3;
			size = new Size(455, 12);
			label8.Size = size;
			this.Label3.TabIndex = 24;
			this.Label3.Text = "提醒：1、软件有问题请联系QQ：1023411046，下载最新版本请访问：www.jfglzs.com";
			this.Label_pd.AutoSize = true;
			this.Label_pd.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			Control label_pd = this.Label_pd;
			location = new Point(487, 26);
			label_pd.Location = location;
			this.Label_pd.Name = "Label_pd";
			Control label_pd2 = this.Label_pd;
			size = new Size(87, 35);
			label_pd2.Size = size;
			this.Label_pd.TabIndex = 25;
			this.Label_pd.Text = "    ";
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Highlight;
			size = new Size(637, 407);
			this.ClientSize = size;
			this.Controls.Add(this.Label_pd);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.Label4);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Button2);
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

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00003344 File Offset: 0x00001544
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00003358 File Offset: 0x00001558
		/// <summary>"重启电脑"按钮：set 时挂接/解除 Button2_Click（强制重启）。</summary>
		internal virtual Button Button2
		{
			get
			{
				return this._Button2Field;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button2_Click);
				if (this._Button2Field != null)
				{
					this._Button2Field.Click -= value2;
				}
				this._Button2Field = value;
				if (this._Button2Field != null)
				{
					this._Button2Field.Click += value2;
				}
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000033 RID: 51 RVA: 0x000033A4 File Offset: 0x000015A4
		// (set) Token: 0x06000034 RID: 52 RVA: 0x000021FE File Offset: 0x000003FE
		internal virtual Label Label1
		{
			get
			{
				return this._Label1Field;
			}
			set
			{
				this._Label1Field = value;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000035 RID: 53 RVA: 0x000033B8 File Offset: 0x000015B8
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002207 File Offset: 0x00000407
		internal virtual TextBox TextBox1
		{
			get
			{
				return this._TextBox1Field;
			}
			set
			{
				this._TextBox1Field = value;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000033CC File Offset: 0x000015CC
		// (set) Token: 0x06000038 RID: 56 RVA: 0x000033E0 File Offset: 0x000015E0
		/// <summary>"确定"按钮：set 时挂接/解除 Button1_Click（密码校验 + 执行卸载）。</summary>
		internal virtual Button Button1
		{
			get
			{
				return this._Button1Field;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button1_Click);
				if (this._Button1Field != null)
				{
					this._Button1Field.Click -= value2;
				}
				this._Button1Field = value;
				if (this._Button1Field != null)
				{
					this._Button1Field.Click += value2;
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000039 RID: 57 RVA: 0x0000342C File Offset: 0x0000162C
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00003440 File Offset: 0x00001640
		/// <summary>鼠标锁定/蜂鸣定时器：set 时挂接/解除 Timer_frm2_Tick（每秒钳制鼠标）。</summary>
		internal virtual System.Windows.Forms.Timer Timer_frm2
		{
			get
			{
				return this._TimerFrm2Field;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Timer_frm2_Tick);
				if (this._TimerFrm2Field != null)
				{
					this._TimerFrm2Field.Tick -= value2;
				}
				this._TimerFrm2Field = value;
				if (this._TimerFrm2Field != null)
				{
					this._TimerFrm2Field.Tick += value2;
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003B RID: 59 RVA: 0x0000348C File Offset: 0x0000168C
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002210 File Offset: 0x00000410
		internal virtual Label Label2
		{
			get
			{
				return this._Label2Field;
			}
			set
			{
				this._Label2Field = value;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600003D RID: 61 RVA: 0x000034A0 File Offset: 0x000016A0
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002219 File Offset: 0x00000419
		internal virtual Label Label4
		{
			get
			{
				return this._Label4Field;
			}
			set
			{
				this._Label4Field = value;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003F RID: 63 RVA: 0x000034B4 File Offset: 0x000016B4
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002222 File Offset: 0x00000422
		internal virtual Label Label3
		{
			get
			{
				return this._Label3Field;
			}
			set
			{
				this._Label3Field = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000034C8 File Offset: 0x000016C8
		// (set) Token: 0x06000042 RID: 66 RVA: 0x000034DC File Offset: 0x000016DC
		/// <summary>隐藏双击触发 Label（Label_pd）：set 时挂接/解除 Label_pd_DoubleClick。</summary>
		internal virtual Label Label_pd
		{
			get
			{
				return this._LabelPdField;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Label_pd_DoubleClick);
				if (this._LabelPdField != null)
				{
					this._LabelPdField.DoubleClick -= value2;
				}
				this._LabelPdField = value;
				if (this._LabelPdField != null)
				{
					this._LabelPdField.DoubleClick += value2;
				}
			}
		}

		// Token: 0x06000043 RID: 67
		/// <summary>
		/// user32 SystemParametersInfo P/Invoke：SPI_SETDESKWALLPAPER(20) 设置桌面壁纸。
		/// </summary>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool SystemParametersInfo(int int_0, int int_1, string string_0, int int_2);

		// Token: 0x06000044 RID: 68
		/// <summary>
		/// user32 ClipCursor P/Invoke：把鼠标限制在指定矩形内（本窗体用于强制锁定鼠标）。
		/// </summary>
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern int ClipCursor(Form2.RECT lpRect);

		// Token: 0x06000045 RID: 69
		/// <summary>
		/// kernel32 Beep P/Invoke：发出蜂鸣（Timer_frm2_Tick 中周期性骚扰提示）。
		/// </summary>
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern long Beep(long long_0, long long_1);

		// Token: 0x06000046 RID: 70 RVA: 0x00003528 File Offset: 0x00001728
		/// <summary>
		/// Button1（确定）点击事件（原混淆名 ReadUtils，控制流被扁平化为 IL 标签+switch
		/// 状态机，以下为还原后的真实逻辑顺序）：
		/// 1) 密码框长度 < 6 → Label2 提示"密码应该是6位以上的字母或数字，请重新输入！"，清空输入，返回；
		/// 2) left = RegConfig.ComputePasswordHash(Trim(密码))（SHA256(密码+"bfdshgs") 十六进制小写）；
		/// 3) right = HKCU\Software\n 的值；不一致 → "密码不正确，请重新输入！"，清空，返回；
		/// 4) 密码正确：禁用按钮、等待光标 → RegConfig.RunFullUninstall() 执行完整卸载 → 恢复光标；
		/// 5) 读 HKCU\Control Panel\Desktop\Wallpaper，含"壁纸"字样则 SetDefaultWallpaper 恢复系统默认壁纸；
		/// 6) ClipCursor 锁定鼠标到全屏矩形 → Thread.Sleep(7000) → ProjectData.EndApp() 退出进程。
		/// </summary>
		private void Button1_Click(object sender, EventArgs e)
		{
			// [反混淆] 原状态机（IL_xx 标签 + num/num2 调度 + endfilter 错误重派）→ 还原为顺序 if 控制流
			try
			{
				// 密码长度 < 6 直接拒绝
				if (Strings.Len(this.TextBox1.Text) < 6)
				{
					this.Label2.Text = "密码应该是6位以上的字母或数字，请重新输入！";
					this.TextBox1.Text = "";
					return;
				}
				// 比对 SHA256 哈希（ComputePasswordHash）与 HKCU\Software\n 存值
				string left = RegConfig.ComputePasswordHash(Strings.Trim(this.TextBox1.Text));
				string right = Conversions.ToString(RegConfig.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
				if (Operators.CompareString(left, right, false) != 0)
				{
					this.Label2.Text = "密码不正确，请重新输入！";
					this.TextBox1.Text = "";
					return;
				}
				// 密码正确：执行完整卸载
				this.Button1.Enabled = false;
				this.Label2.Text = "";
				this.Cursor = Cursors.WaitCursor;
				RegConfig.RunFullUninstall();
				this.Cursor = Cursors.Default;
				// 读当前壁纸路径，含"壁纸"字样则恢复系统默认壁纸
				string wallpaperPath;
				using (RegistryKey desktopKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true))
				{
					wallpaperPath = Conversions.ToString(desktopKey.GetValue("Wallpaper"));
				}
				if (Strings.InStr(wallpaperPath, "壁纸", CompareMethod.Binary) != 0)
				{
					this.SetDefaultWallpaper("C:\\Windows\\Web\\Wallpaper\\Windows\\img0.jpg", Form2.WallpaperStyle.Stretched);
				}
				// 锁定鼠标到全屏矩形，7 秒后结束进程
				Form2.RECT lpRect;
				lpRect.Left = 0;
				lpRect.Top = 0;
				lpRect.Right = Screen.PrimaryScreen.Bounds.Width;
				lpRect.Bottom = Screen.PrimaryScreen.Bounds.Height;
				Form2.ClipCursor(lpRect);
				Thread.Sleep(7000);
				ProjectData.EndApp();
			}
			catch (Exception ex)
			{
				// 原混淆样板（VB On Error 残留）：忽略错误
			}
		}

// Token: 0x06000047 RID: 71 RVA: 0x00003838 File Offset: 0x00001A38
		/// <summary>
		/// Label_pd 双击事件（原混淆名 SetupUtils）：空壳处理器——反编译仅剩异常处理骨架
		/// （IL_xx 标签 + switch 状态机），无实际业务逻辑。
		/// </summary>
		private void Label_pd_DoubleClick(object sender, EventArgs e)
		{
			// [反混淆] 空壳处理器：原混淆仅剩异常处理骨架（IL_xx 标签 + switch 状态机），无实际业务逻辑
		}

// Token: 0x06000048 RID: 72 RVA: 0x000038C8 File Offset: 0x00001AC8
		/// <summary>
		/// Timer_frm2 心跳（原混淆名 AssetUtils）：
		/// TopMost = true；ClipCursor(0,0,800,200) 把鼠标限制在屏幕顶部区域；
		/// 蜂鸣计数 _beepCounter 每 5 次 Beep(2000,500) 一次（周期性骚扰）。
		/// </summary>
		private void Timer_frm2_Tick(object sender, EventArgs e)
		{
			this.TopMost = true;
			Form2.RECT lpRect;
			lpRect.Left = 0;
			lpRect.Top = 0;
			lpRect.Right = 800;
			lpRect.Bottom = 200;
			Form2.ClipCursor(lpRect);
			checked
			{
				this._beepCounter++;
				if (this._beepCounter >= 5)
				{
					Form2.Beep(2000L, 500L);
				}
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000222B File Offset: 0x0000042B
		/// <summary>
		/// FormClosing 拦截（原混淆名 CompareUtils）：e.Cancel = true 禁止关闭窗体——
		/// 与 Form1 的 Timer2 心跳配合构成"无法关闭"的强制卸载界面。
		/// </summary>
		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002234 File Offset: 0x00000434
		/// <summary>
		/// Form2 加载事件（原混淆名 InterruptUtils）：启动 Timer_frm2（鼠标锁定+蜂鸣）。
		/// </summary>
		private void Form2_Load(object sender, EventArgs e)
		{
			this.Timer_frm2.Enabled = true;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000393C File Offset: 0x00001B3C
		/// <summary>
		/// Button2（重启电脑）点击事件（原混淆名 StartUtils）：
		/// Interaction.Shell("shutdown /r /f /t 0", MinimizedFocus) —— 无确认强制立即重启电脑。
		/// </summary>
		private void Button2_Click(object sender, EventArgs e)
		{
			// [反混淆] 原状态机（IL_xx 标签 + num/num2 调度 + endfilter 错误重派）→ 还原为单行调用
			try
			{
				Interaction.Shell("shutdown /r /f /t 0", AppWinStyle.MinimizedFocus, false, -1);
			}
			catch (Exception ex)
			{
				// 原混淆样板（VB On Error 残留）：忽略错误
			}
		}

// Token: 0x0600004C RID: 76 RVA: 0x00002242 File Offset: 0x00000442
		/// <summary>
		/// 打开官网链接（原混淆名 ConnectUtils）：Process.Start("http://www.jfglzs.com")，
		/// 当前无控件绑定（事件未被挂接）。
		/// </summary>
		private void OpenWebsiteLink(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.jfglzs.com");
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000039DC File Offset: 0x00001BDC
		/// <summary>
		/// 设置默认壁纸（原混淆名 OrderUtils）：
		/// 1) 图片文件不存在则抛 ArgumentException("指定的图片文件不存在")；
		/// 2) 写 HKCU\Control Panel\Desktop 的 Wallpaper 与 WallpaperStyle="2"；
		/// 3) SystemParametersInfo(20=SPI_SETDESKWALLPAPER, 0, 路径, 3=SPIF_UPDATEINIFILE|SPIF_SENDWININICHANGE)，
		///    失败抛 Win32Exception。
		/// </summary>
		private void SetDefaultWallpaper(string string_0, Form2.WallpaperStyle wallpaperStyle_0)
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
			if (!Form2.SystemParametersInfo(20, 0, string_0, 3))
			{
				throw new Win32Exception();
			}
		}

		// Token: 0x0400000D RID: 13
		/// <summary>Button2（重启电脑）控件后端字段。</summary>
		[AccessedThroughProperty("Button2")]
		private Button _Button2Field;

		// Token: 0x0400000E RID: 14
		/// <summary>Label1（"卸载小助手密码："）控件后端字段。</summary>
		[AccessedThroughProperty("Label1")]
		private Label _Label1Field;

		// Token: 0x0400000F RID: 15
		/// <summary>TextBox1（密码输入框，PasswordChar='*'）控件后端字段。</summary>
		[AccessedThroughProperty("TextBox1")]
		private TextBox _TextBox1Field;

		// Token: 0x04000010 RID: 16
		/// <summary>Button1（确定）控件后端字段。</summary>
		[AccessedThroughProperty("Button1")]
		private Button _Button1Field;

		// Token: 0x04000011 RID: 17
		/// <summary>Timer_frm2（1 秒鼠标锁定/蜂鸣定时器）控件后端字段。</summary>
		[AccessedThroughProperty("Timer_frm2")]
		private System.Windows.Forms.Timer _TimerFrm2Field;

		// Token: 0x04000012 RID: 18
		/// <summary>Label2（密码校验提示，DarkRed）控件后端字段。</summary>
		[AccessedThroughProperty("Label2")]
		private Label _Label2Field;

		// Token: 0x04000013 RID: 19
		/// <summary>Label4（卸载后保存系统提示）控件后端字段。</summary>
		[AccessedThroughProperty("Label4")]
		private Label _Label4Field;

		// Token: 0x04000014 RID: 20
		/// <summary>Label3（QQ/官网联系提示）控件后端字段。</summary>
		[AccessedThroughProperty("Label3")]
		private Label _Label3Field;

		// Token: 0x04000015 RID: 21
		/// <summary>Label_pd（隐藏双击触发 Label）控件后端字段。</summary>
		[AccessedThroughProperty("Label_pd")]
		private Label _LabelPdField;

		// Token: 0x04000016 RID: 22
		/// <summary>SystemParametersInfo 常量：SPI_SETDESKWALLPAPER=20（设置壁纸）。</summary>
		private const int SPI_SETDESKWALLPAPER = 20;

		// Token: 0x04000017 RID: 23
		/// <summary>SystemParametersInfo 标志：SPIF_UPDATEINIFILE=1（写回 ini）。</summary>
		private const int SPIF_UPDATEINIFILE = 1;

		// Token: 0x04000018 RID: 24
		/// <summary>SystemParametersInfo 标志：SPIF_SENDWININICHANGE=2（广播刷新）。</summary>
		private const int SPIF_SENDWININICHANGE = 2;

		// Token: 0x04000019 RID: 25
		/// <summary>Timer_frm2_Tick 蜂鸣计数：每 5 次 Tick 触发一次 Beep(2000,500)。</summary>
		private int _beepCounter;

		// Token: 0x0200000A RID: 10
		/// <summary>
		/// 壁纸拉伸样式（SystemParametersInfo SPI_SETDESKWALLPAPER 使用的样式值）：
		/// Tiled=1（平铺）、Centered=2（居中）、Stretched=6（拉伸）。
		/// </summary>
		public enum WallpaperStyle
		{
			// Token: 0x0400001B RID: 27
			Tiled = 1,
			// Token: 0x0400001C RID: 28
			Centered,
			// Token: 0x0400001D RID: 29
			Stretched = 6
		}

		// Token: 0x0200000B RID: 11
		/// <summary>
		/// ClipCursor 使用的矩形区域结构体（Left/Top/Right/Bottom）。
		/// </summary>
		public struct RECT
		{
			// Token: 0x0400001E RID: 30
			public int Left;

			// Token: 0x0400001F RID: 31
			public int Top;

			// Token: 0x04000020 RID: 32
			public int Right;

			// Token: 0x04000021 RID: 33
			public int Bottom;
		}
	}
}
