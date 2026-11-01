using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

/// <summary>
/// 大尺寸"提醒"窗口：蓝底、居中、置顶，只显示三行红字。
/// 本身没有业务逻辑，由 Form1 改写 <see cref="shuoming"/>、<see cref="shuoming2"/>、
/// <see cref="shuoming3"/> 的文本后 <c>Show()</c>，用于"hosts 被删""注册表被改"
/// "刚删除了哪个文件"等警告场景。
/// </summary>
// Token: 0x0200000F RID: 15
[DesignerGenerated]
public class Form3 : Form
{
	/// <summary>构造窗体并初始化全部控件。</summary>
	// Token: 0x06000084 RID: 132 RVA: 0x00002500 File Offset: 0x00000700
	public Form3()
	{
		this.初始化组件();
	}

	/// <summary>释放设计器容器中的组件。</summary>
	/// <param name="disposing">true 表示同时释放托管资源。</param>
	// Token: 0x06000085 RID: 133 RVA: 0x00006470 File Offset: 0x00004670
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
	/// Designer 生成的 InitializeComponent（原名 EnableVal）：
	/// 创建三个宋体 15.75pt 的红色标签，把窗体设成蓝底、居中、置顶，标题"提醒"。
	/// 这个窗口会被 Form1 当作恐吓式警告牌反复弹出。
	/// </summary>
	// Token: 0x06000086 RID: 134 RVA: 0x000064B0 File Offset: 0x000046B0
	private void 初始化组件()
	{
		this.shuoming2 = new Label();
		this.shuoming = new Label();
		this.shuoming3 = new Label();
		base.SuspendLayout();
		this.shuoming2.AutoSize = true;
		this.shuoming2.Font = new Font("宋体", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming2.ForeColor = Color.Red;
		this.shuoming2.Location = new Point(54, 193);
		this.shuoming2.Name = "shuoming2";
		this.shuoming2.Size = new Size(21, 21);
		this.shuoming2.TabIndex = 20;
		this.shuoming2.Text = " ";
		this.shuoming.AutoSize = true;
		this.shuoming.Font = new Font("宋体", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming.ForeColor = Color.Red;
		this.shuoming.Location = new Point(54, 129);
		this.shuoming.Name = "shuoming";
		this.shuoming.Size = new Size(21, 21);
		this.shuoming.TabIndex = 19;
		this.shuoming.Text = " ";
		this.shuoming3.AutoSize = true;
		this.shuoming3.Location = new Point(56, 279);
		this.shuoming3.Name = "shuoming3";
		this.shuoming3.Size = new Size(0, 12);
		this.shuoming3.TabIndex = 21;
		base.AutoScaleDimensions = new SizeF(6f, 12f);
		base.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = SystemColors.Highlight;
		base.ClientSize = new Size(1248, 559);
		base.Controls.Add(this.shuoming3);
		base.Controls.Add(this.shuoming2);
		base.Controls.Add(this.shuoming);
		base.Name = "Form3";
		base.StartPosition = FormStartPosition.CenterScreen;
		this.Text = "提醒";
		base.TopMost = true;
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000087 RID: 135 RVA: 0x0000250E File Offset: 0x0000070E
	// (set) Token: 0x06000088 RID: 136 RVA: 0x00002516 File Offset: 0x00000716
	internal virtual Label shuoming2 { get; set; }

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000089 RID: 137 RVA: 0x0000251F File Offset: 0x0000071F
	// (set) Token: 0x0600008A RID: 138 RVA: 0x00002527 File Offset: 0x00000727
	internal virtual Label shuoming { get; set; }

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x0600008B RID: 139 RVA: 0x00002530 File Offset: 0x00000730
	// (set) Token: 0x0600008C RID: 140 RVA: 0x00002538 File Offset: 0x00000738
	internal virtual Label shuoming3 { get; set; }

	/// <summary>设计器组件容器（原名 list）。</summary>
	// Token: 0x04000041 RID: 65
	private IContainer 组件容器;

	/// <summary>shuoming2 的 Designer 后备字段（原名 factory，未直接使用）。</summary>
	// Token: 0x04000042 RID: 66
	[CompilerGenerated]
	[AccessedThroughProperty("shuoming2")]
	private Label shuoming2后备字段;

	/// <summary>shuoming 的 Designer 后备字段（原名 _Registry，未直接使用）。</summary>
	// Token: 0x04000043 RID: 67
	[AccessedThroughProperty("shuoming")]
	[CompilerGenerated]
	private Label shuoming后备字段;

	/// <summary>shuoming3 的 Designer 后备字段（原名 m_Definition，未直接使用）。</summary>
	// Token: 0x04000044 RID: 68
	[CompilerGenerated]
	[AccessedThroughProperty("shuoming3")]
	private Label shuoming3后备字段;
}
