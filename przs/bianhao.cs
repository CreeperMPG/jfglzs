// ============================================================================
// bianhao.cs — "编号"悬浮标（显示计算机名后 6 位，透明悬浮窗）
// 真实职责：小号透明悬浮窗（窗体名"bianhao"=编号）。Load 时定位到屏幕右上角
// （Left=屏宽-350, Top=100），用 65 号黑体大字显示计算机名后 6 位（机房机器编号标识）；
// 鼠标悬停显示"由机房管理助手自动生成"，移开清除。由 Form1 的 Timer_main 在
// HKCU\Software\jfglzs\bianhao=on 时显示。
// 说明：ConfuserEx 混淆反编译产物，标识符已按 bianhao.cs.md 映射表反混淆重命名。
// ============================================================================
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using gypZRX1vuWc3aeye0R.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

// Token: 0x02000008 RID: 8
[DesignerGenerated]
public partial class bianhao : Form
{
	// Token: 0x0600002D RID: 45 RVA: 0x000029D4 File Offset: 0x00000BD4
/// <summary>构造：订阅 Load（定位与取编号），随后初始化组件。</summary>
	public bianhao()
	{
		base.Load += this.定位与取编号;
		this.初始化组件();
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00002A40 File Offset: 0x00000C40
/// <summary>
/// 初始化组件：创建编号 Label（Label1，65 号黑体橙色，默认 "stu30"）与提示 Label（Label2），
/// 窗体无边框、TransparencyKey 透明背景、StartPosition=Manual。
/// </summary>
	private void 初始化组件()
	{
		this.Label1 = new Label();
		this.Label2 = new Label();
		this.SuspendLayout();
		this.Label1.AutoSize = true;
		this.Label1.Font = new Font("黑体", 65.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.Label1.ForeColor = Color.FromArgb(255, 128, 0);
		Control label = this.Label1;
		Point location = new Point(2, 2);
		label.Location = location;
		this.Label1.Name = "Label1";
		Control label2 = this.Label1;
		Size size = new Size(257, 87);
		label2.Size = size;
		this.Label1.TabIndex = 1;
		this.Label1.Text = "stu30";
		this.Label2.AutoSize = true;
		this.Label2.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 134);
		Control label3 = this.Label2;
		location = new Point(27, 9);
		label3.Location = location;
		this.Label2.Name = "Label2";
		Control label4 = this.Label2;
		size = new Size(0, 16);
		label4.Size = size;
		this.Label2.TabIndex = 2;
		SizeF autoScaleDimensions = new SizeF(6f, 12f);
		this.AutoScaleDimensions = autoScaleDimensions;
		this.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = Color.FromArgb(192, 64, 0);
		size = new Size(321, 97);
		this.ClientSize = size;
		this.Controls.Add(this.Label2);
		this.Controls.Add(this.Label1);
		this.FormBorderStyle = FormBorderStyle.None;
		this.Name = "bianhao";
		this.ShowInTaskbar = false;
		this.StartPosition = FormStartPosition.Manual;
		this.Text = "bianhao";
		this.TransparencyKey = Color.FromArgb(192, 64, 0);
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000030 RID: 48 RVA: 0x00002C38 File Offset: 0x00000E38
	// (set) Token: 0x06000031 RID: 49 RVA: 0x00002C4C File Offset: 0x00000E4C
/// <summary>编号 Label（Label1）；MouseHover → 悬停显示提示；MouseLeave → 移开清除提示。</summary>
	internal virtual Label Label1
	{
		get
		{
			return this.编号Label;
		}
		set
		{
			EventHandler value2 = new EventHandler(this.移开清除提示);
			EventHandler value3 = new EventHandler(this.悬停显示提示);
			if (this.编号Label != null)
			{
				this.编号Label.MouseLeave -= value2;
				this.编号Label.MouseHover -= value3;
			}
			this.编号Label = value;
			if (this.编号Label != null)
			{
				this.编号Label.MouseLeave += value2;
				this.编号Label.MouseHover += value3;
			}
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000032 RID: 50 RVA: 0x00002CBC File Offset: 0x00000EBC
	// (set) Token: 0x06000033 RID: 51 RVA: 0x000021DC File Offset: 0x000003DC
/// <summary>提示 Label（Label2）：悬停时显示"由机房管理助手自动生成"。</summary>
	internal virtual Label Label2
	{
		get
		{
			return this.提示Label;
		}
		set
		{
			this.提示Label = value;
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002CD0 File Offset: 0x00000ED0
/// <summary>
/// Load 定位与取编号：Left = 屏幕宽 - 350、Top = 100；
/// Label1.Text = Strings.Right(Account.MyComputer.Name, 6)（计算机名末 6 字符）。
/// </summary>
	private void 定位与取编号(object sender, EventArgs e)
	{
		checked
		{
			short num = (short)Screen.PrimaryScreen.Bounds.Width;
			this.Left = (int)(num - 350);
			this.Top = 100;
			string name = Account.MyComputer.Name;
			this.Label1.Text = Strings.Right(name, 6);
		}
	}

	// Token: 0x06000035 RID: 53 RVA: 0x000021E5 File Offset: 0x000003E5
/// <summary>悬停显示提示：Label2.Text = "由机房管理助手自动生成"。</summary>
	private void 悬停显示提示(object sender, EventArgs e)
	{
		this.Label2.Text = "由机房管理助手自动生成";
	}

	// Token: 0x06000036 RID: 54 RVA: 0x000021F7 File Offset: 0x000003F7
/// <summary>移开清除提示：Label2.Text = ""。</summary>
	private void 移开清除提示(object sender, EventArgs e)
	{
		this.Label2.Text = "";
	}

	// Token: 0x0400000E RID: 14
	[AccessedThroughProperty("Label1")]
/// <summary>控件字段：Label1（编号 Label）后备字段。</summary>
	private Label 编号Label;

	// Token: 0x0400000F RID: 15
	[AccessedThroughProperty("Label2")]
/// <summary>控件字段：Label2（提示 Label）后备字段。</summary>
	private Label 提示Label;
}
