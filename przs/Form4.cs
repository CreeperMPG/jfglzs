// ============================================================================
// Form4.cs — 最高级别锁屏窗（鼠标锁定 + 蜂鸣，可配置是否允许关闭）
// 真实职责：比 Form2 更严厉的惩罚窗体。全屏、无边框、TopMost、隐藏任务栏，1s 定时器
// 用 ClipCursor 把鼠标限制在屏幕顶部 100 像素并反复 Beep（锁屏+警告）。关闭行为由全局标志
// Initializer.锁屏放行标志 控制：false 允许关闭并恢复全屏鼠标；true 禁止关闭（强行锁死）。
// 说明：ConfuserEx 混淆反编译产物，标识符已按 Form4.cs.md 映射表反混淆重命名。
// ============================================================================
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using gypZRX1vuWc3aeye0R.Internal;
using Microsoft.VisualBasic.CompilerServices;

// Token: 0x0200000D RID: 13
[DesignerGenerated]
public partial class Form4 : Form
{
	// Token: 0x06000082 RID: 130 RVA: 0x00006CC0 File Offset: 0x00004EC0
/// <summary>构造：订阅 Load（启用定时器）与 FormClosing（控制窗体关闭），随后初始化组件。</summary>
	public Form4()
	{
		base.Load += this.启用锁鼠标定时器;
		base.FormClosing += this.控制窗体关闭;
		this.初始化组件();
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00006D40 File Offset: 0x00004F40
/// <summary>
/// 初始化组件：创建 shuoming 红色大字 Label、Label2 与 1s 定时器 Timer_frm4；窗体全屏无边框 TopMost。
/// </summary>
	private void 初始化组件()
	{
		this.reg = new Container();
		this.shuoming = new Label();
		this.Timer_frm4 = new Timer(this.reg);
		this.Label2 = new Label();
		this.SuspendLayout();
		this.shuoming.AutoSize = true;
		this.shuoming.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming.ForeColor = Color.Red;
		Control shuoming = this.shuoming;
		Point location = new Point(95, 186);
		shuoming.Location = location;
		this.shuoming.Name = "shuoming";
		Control shuoming2 = this.shuoming;
		Size size = new Size(33, 35);
		shuoming2.Size = size;
		this.shuoming.TabIndex = 12;
		this.shuoming.Text = " ";
		this.Timer_frm4.Interval = 1000;
		this.Label2.AutoSize = true;
		this.Label2.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label2.ForeColor = Color.DarkRed;
		Control label = this.Label2;
		location = new Point(26, 83);
		label.Location = location;
		this.Label2.Name = "Label2";
		Control label2 = this.Label2;
		size = new Size(0, 14);
		label2.Size = size;
		this.Label2.TabIndex = 16;
		SizeF autoScaleDimensions = new SizeF(6f, 12f);
		this.AutoScaleDimensions = autoScaleDimensions;
		this.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = SystemColors.Highlight;
		size = new Size(1023, 686);
		this.ClientSize = size;
		this.Controls.Add(this.Label2);
		this.Controls.Add(this.shuoming);
		this.FormBorderStyle = FormBorderStyle.None;
		this.Name = "Form4";
		this.ShowInTaskbar = false;
		this.Text = "Form4";
		this.TopMost = true;
		this.WindowState = FormWindowState.Maximized;
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000085 RID: 133 RVA: 0x00006F5C File Offset: 0x0000515C
	// (set) Token: 0x06000086 RID: 134 RVA: 0x000022B3 File Offset: 0x000004B3
/// <summary>主说明 Label（红色大字）。</summary>
	internal virtual Label shuoming
	{
		get
		{
			return this._Process;
		}
		set
		{
			this._Process = value;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000087 RID: 135 RVA: 0x00006F70 File Offset: 0x00005170
	// (set) Token: 0x06000088 RID: 136 RVA: 0x00006F84 File Offset: 0x00005184
/// <summary>Timer_frm4（1s）；Tick → 锁鼠标定时器（ClipCursor 顶部 100px + 蜂鸣）。</summary>
	internal virtual Timer Timer_frm4
	{
		get
		{
			return this.singleton;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.锁鼠标定时器);
			if (this.singleton != null)
			{
				this.singleton.Tick -= value2;
			}
			this.singleton = value;
			if (this.singleton != null)
			{
				this.singleton.Tick += value2;
			}
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000089 RID: 137 RVA: 0x00006FD0 File Offset: 0x000051D0
	// (set) Token: 0x0600008A RID: 138 RVA: 0x000022BC File Offset: 0x000004BC
/// <summary>提示 Label（深红小字）。</summary>
	internal virtual Label Label2
	{
		get
		{
			return this.expression;
		}
		set
		{
			this.expression = value;
		}
	}

	// Token: 0x0600008B RID: 139
	[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
/// <summary>Win32 user32!ClipCursor：限制鼠标活动区域（锁屏用）。</summary>
	public static extern int ClipCursor(Form4.RECT lpRect);

	// Token: 0x0600008C RID: 140
	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
/// <summary>Win32 kernel32!Beep：蜂鸣警告（锁屏 5 次后每次触发）。</summary>
	private static extern long Beep(long long_0, long long_1);

	// Token: 0x0600008D RID: 141 RVA: 0x00006FE4 File Offset: 0x000051E4
/// <summary>
/// 锁鼠标定时器（Timer_frm4 1s Tick）：TopMost=true；ClipCursor 到屏幕顶部 100px 高区域；
/// 锁屏计时计数++，>=5 后每次 Beep(2000, 500) 蜂鸣。
/// </summary>
	private void 锁鼠标定时器(object sender, EventArgs e)
	{
		this.TopMost = true;
		Form4.RECT lpRect;
		lpRect.Left = 0;
		lpRect.Top = 0;
		lpRect.Right = 800;
		lpRect.Bottom = 100;
		Form4.ClipCursor(lpRect);
		checked
		{
			this.锁屏计时计数++;
			if (this.锁屏计时计数 >= 5)
			{
				Form4.Beep(2000L, 500L);
			}
		}
	}

	// Token: 0x0600008E RID: 142 RVA: 0x000022C5 File Offset: 0x000004C5
/// <summary>Load 事件：启用 Timer_frm4（开始锁鼠标循环）。</summary>
	private void 启用锁鼠标定时器(object sender, EventArgs e)
	{
		this.Timer_frm4.Enabled = true;
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00007054 File Offset: 0x00005254
/// <summary>
/// 控制窗体关闭（FormClosing）：Initializer.锁屏放行标志 == false → 允许关闭（e.Cancel=false）
/// 并 ClipCursor 恢复全屏区域（解除锁定）；== true → e.Cancel=true 拒绝关闭（锁死）。
/// </summary>
	private void 控制窗体关闭(object sender, FormClosingEventArgs e)
	{
		if (!Initializer.锁屏放行标志)
		{
			e.Cancel = false;
			Form4.RECT lpRect;
			lpRect.Left = 0;
			lpRect.Top = 0;
			lpRect.Right = Screen.PrimaryScreen.Bounds.Width;
			lpRect.Bottom = Screen.PrimaryScreen.Bounds.Height;
			Form4.ClipCursor(lpRect);
		}
		else
		{
			e.Cancel = true;
		}
	}

	// Token: 0x04000041 RID: 65
	[AccessedThroughProperty("shuoming")]
/// <summary>控件字段：shuoming 后备字段。</summary>
	private Label _Process;

	// Token: 0x04000042 RID: 66
	[AccessedThroughProperty("Timer_frm4")]
/// <summary>控件字段：Timer_frm4 后备字段。</summary>
	private Timer singleton;

	// Token: 0x04000043 RID: 67
	[AccessedThroughProperty("Label2")]
/// <summary>控件字段：Label2 后备字段。</summary>
	private Label expression;

	// Token: 0x04000044 RID: 68
/// <summary>锁屏计时计数：达到 5 次开始每次蜂鸣（Beep 2000/500ms）。</summary>
	private int 锁屏计时计数;

	// Token: 0x0200000E RID: 14
/// <summary>Win32 RECT 结构（ClipCursor 参数）。</summary>
	public struct RECT
	{
		// Token: 0x04000045 RID: 69
/// <summary>矩形左边界。</summary>
		public int Left;

		// Token: 0x04000046 RID: 70
/// <summary>矩形上边界。</summary>
		public int Top;

		// Token: 0x04000047 RID: 71
/// <summary>矩形右边界。</summary>
		public int Right;

		// Token: 0x04000048 RID: 72
/// <summary>矩形下边界。</summary>
		public int Bottom;
	}
}
