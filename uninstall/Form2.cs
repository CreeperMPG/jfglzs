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
	/// <summary>
	/// 密码确认窗体 Form2：蓝色全屏、置顶、无边框，要求输入「卸载小助手密码」。
	/// 密码正确才调用 卸载核心.执行卸载()；随后换壁纸、把鼠标锁在屏幕内、等 9 秒结束进程。
	/// 本窗体同时是「强制/限制」模块：点 ×、Alt+F4、任务管理器关窗口都关不掉；
	/// 1 秒定时器每 Tick 重新置顶并把鼠标 ClipCursor 到 (0,0)-(800,200)；计数 ≥ 5 后每 Tick 响一声；
	/// 「重启电脑」按钮直接 shutdown /r /f /t 0。
	/// </summary>
	/// <remarks>
	/// 敏感行为：
	/// 1) 强制锁屏式限制：无边框 + 置顶 + 最大化 + FormClosing 取消关闭 + ClipCursor 锁鼠标 + Beep 蜂鸣；
	/// 2) 关机/重启：Interaction.Shell("shutdown /r /f /t 0") —— 强制重启，不保存任何东西；
	/// 3) 改系统设置：写 HKCU\Control Panel\Desktop 的 Wallpaper / WallpaperStyle 并调 SystemParametersInfo(20,…) 换壁纸；
	/// 4) ProjectData.EndApp() 结束进程；密码为本地哈希比对（SHA256 + 固定盐 bfdshgs），属于「防误用」而非真安全。
	/// </remarks>
	// Token: 0x0200000C RID: 12
	[DesignerGenerated]
	public class Form2 : Form
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00002C8C File Offset: 0x00000E8C
		/// <summary>
		/// 构造函数：先挂 FormClosing 与 Load 两个处理器，再调用初始化控件（顺序为原版顺序）。
		/// </summary>
		public Form2()
		{
			base.FormClosing += this.ReadReader;
			base.Load += this.WriteReader;
			this.RateReader();
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002CCC File Offset: 0x00000ECC
		/// <summary>
		/// 设计器生成的释放逻辑：disposing 为真时先释放组件容器，再调用基类 Dispose。
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

		// Token: 0x0600003A RID: 58 RVA: 0x00002D0C File Offset: 0x00000F0C
		/// <summary>
		/// VB 的 InitializeComponent：密码标题、密码框、「确定(需10秒)」「重启电脑」两个按钮、
		/// 两条提示文字、显示用的空白标签、1 秒定时器；窗体 637×407、无边框、置顶、最大化、不进任务栏。
		/// </summary>
		private void RateReader()
		{
			this.组件容器 = new Container();
			this.Button2 = new Button();
			this.Label1 = new Label();
			this.TextBox1 = new TextBox();
			this.Button1 = new Button();
			this.Timer_frm2 = new global::System.Windows.Forms.Timer(this.组件容器);
			this.Label2 = new Label();
			this.Label4 = new Label();
			this.Label3 = new Label();
			this.Label_pd = new Label();
			base.SuspendLayout();
			this.Button2.Location = new Point(397, 27);
			this.Button2.Name = "Button2";
			this.Button2.Size = new Size(84, 26);
			this.Button2.TabIndex = 19;
			this.Button2.Text = "重启电脑";
			this.Button2.UseVisualStyleBackColor = true;
			this.Label1.AutoSize = true;
			this.Label1.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label1.ForeColor = Color.White;
			this.Label1.Location = new Point(39, 31);
			this.Label1.Name = "Label1";
			this.Label1.Size = new Size(119, 14);
			this.Label1.TabIndex = 18;
			this.Label1.Text = "卸载小助手密码：";
			this.TextBox1.Location = new Point(164, 29);
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.PasswordChar = '*';
			this.TextBox1.Size = new Size(104, 21);
			this.TextBox1.TabIndex = 17;
			this.Button1.Location = new Point(284, 26);
			this.Button1.Name = "Button1";
			this.Button1.Size = new Size(92, 27);
			this.Button1.TabIndex = 16;
			this.Button1.Text = "确定(需10秒)";
			this.Button1.UseVisualStyleBackColor = true;
			this.Timer_frm2.Interval = 1000;
			this.Label2.AutoSize = true;
			this.Label2.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label2.ForeColor = Color.DarkRed;
			this.Label2.Location = new Point(32, 63);
			this.Label2.Name = "Label2";
			this.Label2.Size = new Size(0, 14);
			this.Label2.TabIndex = 20;
			this.Label4.AutoSize = true;
			this.Label4.Location = new Point(77, 135);
			this.Label4.Name = "Label4";
			this.Label4.Size = new Size(299, 12);
			this.Label4.TabIndex = 23;
			this.Label4.Text = "2、卸载后记得在硬盘保护系统中创建进度或保存系统。";
			this.Label3.AutoSize = true;
			this.Label3.Location = new Point(40, 104);
			this.Label3.Name = "Label3";
			this.Label3.Size = new Size(455, 12);
			this.Label3.TabIndex = 24;
			this.Label3.Text = "提醒：1、软件有问题请联系QQ：1023411046，下载最新版本请访问：www.jfglzs.com";
			this.Label_pd.AutoSize = true;
			this.Label_pd.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label_pd.Location = new Point(487, 26);
			this.Label_pd.Name = "Label_pd";
			this.Label_pd.Size = new Size(87, 35);
			this.Label_pd.TabIndex = 25;
			this.Label_pd.Text = "    ";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Highlight;
			base.ClientSize = new Size(637, 407);
			base.Controls.Add(this.Label_pd);
			base.Controls.Add(this.Label3);
			base.Controls.Add(this.Label4);
			base.Controls.Add(this.Label2);
			base.Controls.Add(this.Button2);
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

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002389 File Offset: 0x00000589
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00003228 File Offset: 0x00001428
		/// <summary>
		/// 「重启电脑」按钮（Button2）。setter 在换实例时同步退订/订阅 Click（指向 OrderReader）。
		/// </summary>
		internal virtual Button Button2
		{
			[CompilerGenerated]
			get
			{
				return this._Button2实例;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.OrderReader);
				Button button = this._Button2实例;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this._Button2实例 = value;
				button = this._Button2实例;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002391 File Offset: 0x00000591
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002399 File Offset: 0x00000599
		/// <summary>
		/// 密码标题标签（Label1）：固定文本「卸载小助手密码：」，白色宋体 10.5pt。
		/// </summary>
		internal virtual Label Label1 { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600003F RID: 63 RVA: 0x000023A2 File Offset: 0x000005A2
		// (set) Token: 0x06000040 RID: 64 RVA: 0x000023AA File Offset: 0x000005AA
		/// <summary>
		/// 密码输入框（TextBox1）：PasswordChar 为 '*'，校验失败或过短时被清空。
		/// </summary>
		internal virtual TextBox TextBox1 { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000023B3 File Offset: 0x000005B3
		// (set) Token: 0x06000042 RID: 66 RVA: 0x0000326C File Offset: 0x0000146C
		/// <summary>
		/// 「确定(需10秒)」按钮（Button1）。setter 在换实例时同步退订/订阅 Click（指向 InsertReader）。
		/// </summary>
		internal virtual Button Button1
		{
			[CompilerGenerated]
			get
			{
				return this._Button1实例;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.InsertReader);
				Button button = this._Button1实例;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this._Button1实例 = value;
				button = this._Button1实例;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000023BB File Offset: 0x000005BB
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000032B0 File Offset: 0x000014B0
		/// <summary>
		/// 限制定时器（Timer_frm2，1000ms）。setter 在换实例时同步退订/订阅 Tick（指向 StopReader）。
		/// </summary>
		internal virtual global::System.Windows.Forms.Timer Timer_frm2
		{
			[CompilerGenerated]
			get
			{
				return this._Timer2实例;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.StopReader);
				global::System.Windows.Forms.Timer timer = this._Timer2实例;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this._Timer2实例 = value;
				timer = this._Timer2实例;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000023C3 File Offset: 0x000005C3
		// (set) Token: 0x06000046 RID: 70 RVA: 0x000023CB File Offset: 0x000005CB
		/// <summary>
		/// 错误提示标签（Label2）：显示密码过短 / 密码不正确两种提示，深红色。
		/// </summary>
		internal virtual Label Label2 { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000023D4 File Offset: 0x000005D4
		// (set) Token: 0x06000048 RID: 72 RVA: 0x000023DC File Offset: 0x000005DC
		/// <summary>
		/// 提示标签（Label4）：「2、卸载后记得在硬盘保护系统中创建进度或保存系统。」
		/// </summary>
		internal virtual Label Label4 { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000049 RID: 73 RVA: 0x000023E5 File Offset: 0x000005E5
		// (set) Token: 0x0600004A RID: 74 RVA: 0x000023ED File Offset: 0x000005ED
		/// <summary>
		/// 提示标签（Label3）：作者联系方式与官网地址。
		/// </summary>
		internal virtual Label Label3 { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000023F6 File Offset: 0x000005F6
		// (set) Token: 0x0600004C RID: 76 RVA: 0x000032F4 File Offset: 0x000014F4
		/// <summary>
		/// 大号空白标签（Label_pd）：显示四个空格；setter 在换实例时同步退订/订阅 DoubleClick（指向 PatchReader，空方法）。
		/// </summary>
		internal virtual Label Label_pd
		{
			[CompilerGenerated]
			get
			{
				return this._Label_pd实例;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.PatchReader);
				Label label = this._Label_pd实例;
				if (label != null)
				{
					label.DoubleClick -= eventHandler;
				}
				this._Label_pd实例 = value;
				label = this._Label_pd实例;
				if (label != null)
				{
					label.DoubleClick += eventHandler;
				}
			}
		}

		// Token: 0x0600004D RID: 77
		/// <summary>
		/// user32!SystemParametersInfo：换壁纸用。调用点传 (20, 0, 路径, 3) ——
		/// 20 = SPI_SETDESKWALLPAPER，3 = SPIF_UPDATEINIFILE | SPIF_SENDCHANGE。
		/// </summary>
		/// <param name="uAction">要查询/设置的系统参数编号（这里 20 = SPI_SETDESKWALLPAPER）</param>
		/// <param name="uParam">依 uAction 而定，换壁纸时不用（传 0）</param>
		/// <param name="lpvParam">依 uAction 而定，换壁纸时是图片路径</param>
		/// <param name="fuWinIni">是否写用户配置与广播设置变更（这里 3）</param>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		// Token: 0x0600004E RID: 78
		/// <summary>
		/// user32!ClipCursor：把鼠标光标限制在给定矩形内（传整屏矩形等于限制在屏幕范围）。
		/// </summary>
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern int ClipCursor(Form2.RECT lpRect);

		// Token: 0x0600004F RID: 79
		/// <summary>
		/// kernel32!Beep：按给定频率与时长响一声（调用点用 2000Hz / 500ms）。
		/// </summary>
		/// <param name="dwFreq">频率（赫兹）</param>
		/// <param name="dwDuration">时长（毫秒）</param>
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern long Beep(long dwFreq, long dwDuration);

		// Token: 0x06000050 RID: 80 RVA: 0x00003338
		/// <summary>
		/// 「确定(需10秒)」按钮：校验密码 → 禁用按钮并进入等待光标 → 执行卸载 → 恢复光标 →
		/// 若当前壁纸是管理端换过的那张（路径含「壁纸」二字）就恢复系统默认壁纸 →
		/// 把鼠标锁在整屏矩形 → 等 9 秒 → 结束进程。
		/// </summary>
		/// <remarks>
		/// 原版整段是 VB 的 <c>On Error Resume Next</c>：<c>ClearProjectError()</c> 开场、<c>num2 = 2..26</c> 是状态号、
		/// 末尾 <c>catch when (endfilter(...))</c> 按状态号分发——每条语句失败都只是跳过该条。
		/// 四处跳转的含义：IL_0046 = 输入够长、跳过提示；IL_00B8 = 密码正确分支；
		/// IL_015F = 壁纸路径不含「壁纸」、跳过换壁纸；IL_01CC = 统一出口。
		/// 这里还原成「提前 return + if/else」，每个有副作用的语句各自包 try/catch。
		/// <c>using (OpenSubKey)</c> 是反编译出来的原样写法（释放时机与原版一致），不要改成手动 Close。
		/// </remarks>
		private void InsertReader(object sender, EventArgs e)
		{
			if (Strings.Len(this.TextBox1.Text) < 6)
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
			string 输入哈希 = "";
			try
			{
				输入哈希 = MappingConfig.CustomizeReader(Strings.Trim(this.TextBox1.Text));
			}
			catch
			{
			}
			string 已存哈希 = "";
			try
			{
				已存哈希 = Conversions.ToString(MappingConfig.SortReader(Registry.CurrentUser, "Software", "n", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(输入哈希, 已存哈希, false) != 0)
			{
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
				return;
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
				this.Label2.Text = "";
			}
			catch
			{
			}
			try
			{
				this.Cursor = Cursors.WaitCursor;
			}
			catch
			{
			}
			try
			{
				MappingConfig.PopReader();
			}
			catch
			{
			}
			try
			{
				this.Cursor = Cursors.Default;
			}
			catch
			{
			}
			string 当前壁纸 = "";
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true))
				{
					当前壁纸 = Conversions.ToString(registryKey.GetValue("Wallpaper"));
				}
			}
			catch
			{
			}
			if (Strings.InStr(当前壁纸, "壁纸", CompareMethod.Binary) != 0)
			{
				string 默认壁纸 = "C:\\Windows\\Web\\Wallpaper\\Windows\\img0.jpg";
				try
				{
					this.ValidateReader(默认壁纸, Form2.WallpaperStyle.Stretched);
				}
				catch
				{
				}
			}
			Form2.RECT 鼠标限制矩形;
			鼠标限制矩形.Left = 0;
			鼠标限制矩形.Top = 0;
			鼠标限制矩形.Right = Screen.PrimaryScreen.Bounds.Width;
			鼠标限制矩形.Bottom = Screen.PrimaryScreen.Bounds.Height;
			try
			{
				Form2.ClipCursor(鼠标限制矩形);
			}
			catch
			{
			}
			try
			{
				Thread.Sleep(9000);
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

		// Token: 0x06000051 RID: 81 RVA: 0x0000360C File Offset: 0x0000180C
		/// <summary>
		/// Label_pd 的双击事件：空方法体（原版只有 On Error Resume Next 骨架，没有任何语句），双击没有任何效果。
		/// </summary>
		private void PatchReader(object sender, EventArgs e)
		{
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003690 File Offset: 0x00001890
		/// <summary>
		/// Timer_frm2 的 Tick（每 1000ms）：强制置顶、把鼠标锁在 (0,0)-(800,200) 这条窄带里，
		/// 计数到 5 次之后每次再响一声 Beep(2000Hz, 500ms)——让用户既关不掉也点不到别处。
		/// </summary>
		private void StopReader(object sender, EventArgs e)
		{
			base.TopMost = true;
			Form2.RECT 鼠标限制矩形;
			鼠标限制矩形.Left = 0;
			鼠标限制矩形.Top = 0;
			鼠标限制矩形.Right = 800;
			鼠标限制矩形.Bottom = 200;
			Form2.ClipCursor(鼠标限制矩形);
			checked
			{
				this.m_PrototypeConfig++;
				if (this.m_PrototypeConfig >= 5)
				{
					Form2.Beep(2000L, 500L);
				}
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000023FE File Offset: 0x000005FE
		/// <summary>
		/// FormClosing：无条件 e.Cancel = true —— 点 ×、Alt+F4、任务管理器里的「关闭」都关不掉本窗体。
		/// </summary>
		private void ReadReader(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002407 File Offset: 0x00000607
		/// <summary>
		/// Form_Load：启动 1 秒定时器（开始置顶 + 锁鼠标 + 蜂鸣的循环）。
		/// </summary>
		private void WriteReader(object sender, EventArgs e)
		{
			this.Timer_frm2.Enabled = true;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003704 File Offset: 0x00001904
		/// <summary>
		/// 「重启电脑」按钮：Interaction.Shell("shutdown /r /f /t 0", MinimizedFocus, 等待=false, -1) ——
		/// 强制立即重启，不等待、不保存任何东西。
		/// </summary>
		/// <remarks>
		/// 原版是 VB 的 <c>On Error Resume Next</c> 骨架包着这一条语句（状态号 num2 = 2 + 按状态号分发的 catch），
		/// 这里保留 try/catch：执行命令失败被吞掉，与原版一致。
		/// </remarks>
		private void OrderReader(object sender, EventArgs e)
		{
			try
			{
				Interaction.Shell("shutdown /r /f /t 0", AppWinStyle.MinimizedFocus, false, -1);
			}
			catch
			{
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002415 File Offset: 0x00000615
		/// <summary>
		/// LinkLabel 的点击事件：打开官网。Designer 里并没有 LinkLabel，没有任何控件绑定到这里——
		/// 属于接不上的死代码。
		/// </summary>
		private void VisitReader(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.jfglzs.com");
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000379C File Offset: 0x00001904
		/// <summary>
		/// 换壁纸：图片不存在就抛 ArgumentException；否则写 HKCU\Control Panel\Desktop 的
		/// Wallpaper = 路径、WallpaperStyle = "2"，再调 SystemParametersInfo(20, 0, 路径, 3) 让设置立即生效，
		/// 该 API 返回失败则抛 Win32Exception。
		/// </summary>
		/// <param name="壁纸路径">壁纸图片的完整路径</param>
		/// <param name="壁纸样式">原版形参，实际未被使用（WallpaperStyle 在注册表里写死为 "2"），保留签名以免改变调用点</param>
		private void ValidateReader(string 壁纸路径, Form2.WallpaperStyle 壁纸样式)
		{
			if (!File.Exists(壁纸路径))
			{
				throw new ArgumentException("指定的图片文件不存在");
			}
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true))
			{
				registryKey.SetValue("Wallpaper", 壁纸路径);
				registryKey.SetValue("WallpaperStyle", "2");
			}
			if (!Form2.SystemParametersInfo(20, 0, 壁纸路径, 3))
			{
				throw new Win32Exception();
			}
		}

		// Token: 0x04000011 RID: 17
		/// <summary>组件容器（Designer 的 components），Dispose 时一并释放。</summary>
		private IContainer 组件容器;

		// Token: 0x04000012 RID: 18
		/// <summary>「重启电脑」按钮（Button2）的后备字段。</summary>
		[AccessedThroughProperty("Button2")]
		[CompilerGenerated]
		private Button _Button2实例;

		// Token: 0x04000013 RID: 19
		/// <summary>密码标题标签（Label1）的后备字段。</summary>
		[AccessedThroughProperty("Label1")]
		[CompilerGenerated]
		private Label m_Instance;

		// Token: 0x04000014 RID: 20
		/// <summary>密码输入框（TextBox1）的后备字段。</summary>
		[AccessedThroughProperty("TextBox1")]
		[CompilerGenerated]
		private TextBox client;

		// Token: 0x04000015 RID: 21
		/// <summary>「确定(需10秒)」按钮（Button1）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Button1")]
		private Button _Button1实例;

		// Token: 0x04000016 RID: 22
		/// <summary>限制定时器（Timer_frm2）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Timer_frm2")]
		private global::System.Windows.Forms.Timer _Timer2实例;

		// Token: 0x04000017 RID: 23
		/// <summary>错误提示标签（Label2）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label2")]
		private Label m_Manager;

		// Token: 0x04000018 RID: 24
		/// <summary>提示标签（Label4）的后备字段。</summary>
		[AccessedThroughProperty("Label4")]
		[CompilerGenerated]
		private Label m_Tokenizer;

		// Token: 0x04000019 RID: 25
		/// <summary>提示标签（Label3）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label3")]
		private Label _Proc;

		// Token: 0x0400001A RID: 26
		/// <summary>大号空白标签（Label_pd）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label_pd")]
		private Label _Label_pd实例;

		// Token: 0x0400001B RID: 27
		/// <summary>未使用常量，值 20（与 SPI_SETDESKWALLPAPER 相同）；代码里用的是字面量 20，没有引用本常量。</summary>
		private const int configConfig = 20;

		// Token: 0x0400001C RID: 28
		/// <summary>未使用常量，值 1；全工程无引用。</summary>
		private const int serviceConfig = 1;

		// Token: 0x0400001D RID: 29
		/// <summary>未使用常量，值 2；全工程无引用。</summary>
		private const int stubConfig = 2;

		// Token: 0x0400001E RID: 30
		/// <summary>定时器 Tick 计数：每 Tick 自增，累计到 5 之后每次 Tick 都响一声。</summary>
		private int m_PrototypeConfig;

		// Token: 0x0200000D RID: 13
		/// <summary>
		/// 壁纸样式（对应注册表 WallpaperStyle 的数值）：Tiled=1 平铺、Centered=2 居中、Stretched=6 拉伸。
		/// 代码里从未使用——换壁纸时样式被写死成字符串 "2"。
		/// </summary>
		public enum WallpaperStyle
		{
			// Token: 0x04000020 RID: 32
			Tiled = 1,
			// Token: 0x04000021 RID: 33
			Centered,
			// Token: 0x04000022 RID: 34
			Stretched = 6
		}

		// Token: 0x0200000E RID: 14
		/// <summary>
		/// Win32 RECT：传给 ClipCursor 的矩形（左、上、右、下，单位像素）。
		/// </summary>
		public struct RECT
		{
			// Token: 0x04000023 RID: 35
			/// <summary>左边界</summary>
			public int Left;

			// Token: 0x04000024 RID: 36
			/// <summary>上边界</summary>
			public int Top;

			// Token: 0x04000025 RID: 37
			/// <summary>右边界</summary>
			public int Right;

			// Token: 0x04000026 RID: 38
			/// <summary>下边界</summary>
			public int Bottom;
		}
	}
}
