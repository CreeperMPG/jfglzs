using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using OZxTKXuvsl9D34LWhP.Internal;

/// <summary>
/// 桌面右上角的"机号"小窗：无边框、透明底色、不显示在任务栏，
/// 加载时移动到 (屏幕宽-350, 100)，用大号橙字显示计算机名最后 6 个字符。
/// 鼠标悬停时第二行显示"由机房管理助手自动生成"，移开即清空。
/// 用于让管理员远距离识别当前学生机，属于信息暴露界面。
/// </summary>
// Token: 0x0200000B RID: 11
[DesignerGenerated]
public class bianhao : Form
{
	/// <summary>构造窗体，挂接 Load 事件并初始化控件。</summary>
	// Token: 0x06000038 RID: 56 RVA: 0x000029B4 File Offset: 0x00000BB4
	public bianhao()
	{
		base.Load += this.窗体加载_定位并显示编号;
		this.初始化组件();
	}

	/// <summary>释放设计器容器中的组件。</summary>
	/// <param name="disposing">true 表示同时释放托管资源。</param>
	// Token: 0x06000039 RID: 57 RVA: 0x000029E0 File Offset: 0x00000BE0
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
	/// Designer 生成的 InitializeComponent（原名 GetVal）：
	/// 建立橙色的机号标签和提示标签，把窗体设成无边框、透明、手动定位。
	/// </summary>
	// Token: 0x0600003A RID: 58 RVA: 0x00002A20 File Offset: 0x00000C20
	private void 初始化组件()
	{
		this.Label1 = new Label();
		this.Label2 = new Label();
		base.SuspendLayout();
		this.Label1.AutoSize = true;
		this.Label1.Font = new Font("黑体", 65.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label1.ForeColor = Color.FromArgb(255, 128, 0);
		this.Label1.Location = new Point(2, 2);
		this.Label1.Name = "Label1";
		this.Label1.Size = new Size(257, 87);
		this.Label1.TabIndex = 1;
		this.Label1.Text = "stu30";
		this.Label2.AutoSize = true;
		this.Label2.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label2.Location = new Point(27, 9);
		this.Label2.Name = "Label2";
		this.Label2.Size = new Size(0, 16);
		this.Label2.TabIndex = 2;
		base.AutoScaleDimensions = new SizeF(6f, 12f);
		base.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = Color.FromArgb(192, 64, 0);
		base.ClientSize = new Size(321, 97);
		base.Controls.Add(this.Label2);
		base.Controls.Add(this.Label1);
		base.FormBorderStyle = FormBorderStyle.None;
		base.Name = "bianhao";
		base.ShowInTaskbar = false;
		base.StartPosition = FormStartPosition.Manual;
		this.Text = "bianhao";
		base.TransparencyKey = Color.FromArgb(192, 64, 0);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	/// <summary>机号标签：双击/悬停事件的挂接由 Designer 属性 setter 维护。</summary>
	// Token: 0x1700000F RID: 15
	// (get) Token: 0x0600003B RID: 59 RVA: 0x000023AA File Offset: 0x000005AA
	// (set) Token: 0x0600003C RID: 60 RVA: 0x00002C04 File Offset: 0x00000E04
	internal virtual Label Label1
	{
		[CompilerGenerated]
		get
		{
			return this.机号标签;
		}
		[CompilerGenerated]
		set
		{
			EventHandler eventHandler = new EventHandler(this.鼠标悬停_显示说明);
			EventHandler eventHandler2 = new EventHandler(this.鼠标离开_清空说明);
			Label label = this.机号标签;
			if (label != null)
			{
				label.MouseHover -= eventHandler;
				label.MouseLeave -= eventHandler2;
			}
			this.机号标签 = value;
			label = this.机号标签;
			if (label != null)
			{
				label.MouseHover += eventHandler;
				label.MouseLeave += eventHandler2;
			}
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x0600003D RID: 61 RVA: 0x000023B2 File Offset: 0x000005B2
	// (set) Token: 0x0600003E RID: 62 RVA: 0x000023BA File Offset: 0x000005BA
	internal virtual Label Label2 { get; set; }

	/// <summary>
	/// 窗体加载（原名 RunVal）：把窗口放到主屏右上角，
	/// 并显示 <c>My.Computer.Name</c> 的最后 6 个字符作为机号。
	/// <para>
	/// 屏幕宽度先被 <c>(short)</c> 截断，再参与 <c>- 350</c> 运算；
	/// 原版就是带 <c>checked</c> 的 VB 语义，宽屏/多屏下会抛 OverflowException，这里保留不改。
	/// </para>
	/// </summary>
	// Token: 0x0600003F RID: 63 RVA: 0x00002C64 File Offset: 0x00000E64
	private void 窗体加载_定位并显示编号(object sender, EventArgs e)
	{
		checked
		{
			short 屏宽 = (short)Screen.PrimaryScreen.Bounds.Width;
			base.Left = (int)(屏宽 - 350);
			base.Top = 100;
			string 计算机名 = Queue.MyComputer.Name;
			this.Label1.Text = Strings.Right(计算机名, 6);
		}
	}

	/// <summary>鼠标悬停时显示说明文案（原名 VisitVal）。</summary>
	// Token: 0x06000040 RID: 64 RVA: 0x000023C3
	private void 鼠标悬停_显示说明(object sender, EventArgs e)
	{
		this.Label2.Text = "由机房管理助手自动生成";
	}

	/// <summary>鼠标离开时清空说明文案（原名 SortVal）。</summary>
	// Token: 0x06000041 RID: 65 RVA: 0x000023D5
	private void 鼠标离开_清空说明(object sender, EventArgs e)
	{
		this.Label2.Text = "";
	}

	/// <summary>设计器组件容器（原名 attr）。</summary>
	// Token: 0x04000012 RID: 18
	private IContainer 组件容器;

	/// <summary>机号标签的后备字段（原名 reg）。</summary>
	// Token: 0x04000013 RID: 19
	[AccessedThroughProperty("Label1")]
	[CompilerGenerated]
	private Label 机号标签;

	/// <summary>Label2 的 Designer 后备字段（原名 m_Params，未直接使用）。</summary>
	// Token: 0x04000014 RID: 20
	[AccessedThroughProperty("Label2")]
	[CompilerGenerated]
	private Label Label2后备字段;
}
