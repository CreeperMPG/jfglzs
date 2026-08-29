// ============================================================================
// Form3.cs — 违规提醒小窗（三行红色说明标签）
// 真实职责：无边框提示窗，TopMost 置顶，标题"提醒"。不拦截鼠标、不锁屏，仅用三个
// shuoming/shuoming2/shuoming3 红色 Label 展示违规说明（如"本电脑禁止下载安装软件"
// "现在助手强制恢复""刚刚删除文件：…"）。由 Form1 各定时器/监视器事件驱动显示与关闭。
// 说明：ConfuserEx 混淆反编译产物，标识符已按 Form3.cs.md 映射表反混淆重命名。
// ============================================================================
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

// Token: 0x0200000C RID: 12
[DesignerGenerated]
public partial class Form3 : Form
{
	// Token: 0x06000079 RID: 121 RVA: 0x0000228A File Offset: 0x0000048A
/// <summary>构造：初始化组件（3 个说明 Label）。</summary>
	public Form3()
	{
		this.初始化组件();
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00006A18 File Offset: 0x00004C18
/// <summary>
/// 初始化组件：创建 shuoming2（15.75pt 红色）、shuoming（15.75pt 红色）、shuoming3（默认字号）三个 Label，
/// 窗体 TopMost=true、StartPosition=CenterScreen、Text="提醒"、蓝色背景。
/// </summary>
	private void 初始化组件()
	{
		this.shuoming2 = new Label();
		this.shuoming = new Label();
		this.shuoming3 = new Label();
		this.SuspendLayout();
		this.shuoming2.AutoSize = true;
		this.shuoming2.Font = new Font("宋体", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming2.ForeColor = Color.Red;
		Control shuoming = this.shuoming2;
		Point location = new Point(54, 193);
		shuoming.Location = location;
		this.shuoming2.Name = "shuoming2";
		Control shuoming2 = this.shuoming2;
		Size size = new Size(21, 21);
		shuoming2.Size = size;
		this.shuoming2.TabIndex = 20;
		this.shuoming2.Text = " ";
		this.shuoming.AutoSize = true;
		this.shuoming.Font = new Font("宋体", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 134);
		this.shuoming.ForeColor = Color.Red;
		Control shuoming3 = this.shuoming;
		location = new Point(54, 129);
		shuoming3.Location = location;
		this.shuoming.Name = "shuoming";
		Control shuoming4 = this.shuoming;
		size = new Size(21, 21);
		shuoming4.Size = size;
		this.shuoming.TabIndex = 19;
		this.shuoming.Text = " ";
		this.shuoming3.AutoSize = true;
		Control shuoming5 = this.shuoming3;
		location = new Point(56, 279);
		shuoming5.Location = location;
		this.shuoming3.Name = "shuoming3";
		Control shuoming6 = this.shuoming3;
		size = new Size(0, 12);
		shuoming6.Size = size;
		this.shuoming3.TabIndex = 21;
		SizeF autoScaleDimensions = new SizeF(6f, 12f);
		this.AutoScaleDimensions = autoScaleDimensions;
		this.AutoScaleMode = AutoScaleMode.Font;
		this.BackColor = SystemColors.Highlight;
		size = new Size(1248, 559);
		this.ClientSize = size;
		this.Controls.Add(this.shuoming3);
		this.Controls.Add(this.shuoming2);
		this.Controls.Add(this.shuoming);
		this.Name = "Form3";
		this.StartPosition = FormStartPosition.CenterScreen;
		this.Text = "提醒";
		this.TopMost = true;
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x0600007C RID: 124 RVA: 0x00006C84 File Offset: 0x00004E84
	// (set) Token: 0x0600007D RID: 125 RVA: 0x00002298 File Offset: 0x00000498
/// <summary>次级说明 Label（处理结果/建议）。</summary>
/// <summary>主说明 Label（违规行为描述，26 号红色字）。</summary>
	internal virtual Label shuoming2
	{
		get
		{
			return this._Parameter;
		}
		set
		{
			this._Parameter = value;
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x0600007E RID: 126 RVA: 0x00006C98 File Offset: 0x00004E98
	// (set) Token: 0x0600007F RID: 127 RVA: 0x000022A1 File Offset: 0x000004A1
	internal virtual Label shuoming
	{
		get
		{
			return this.visitor;
		}
		set
		{
			this.visitor = value;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000080 RID: 128 RVA: 0x00006CAC File Offset: 0x00004EAC
	// (set) Token: 0x06000081 RID: 129 RVA: 0x000022AA File Offset: 0x000004AA
/// <summary>详情 Label（被删除文件路径等）。</summary>
	internal virtual Label shuoming3
	{
		get
		{
			return this._Getter;
		}
		set
		{
			this._Getter = value;
		}
	}

	// Token: 0x0400003D RID: 61
	[AccessedThroughProperty("shuoming2")]
/// <summary>控件字段：shuoming2 后备字段。</summary>
	private Label _Parameter;

	// Token: 0x0400003E RID: 62
	[AccessedThroughProperty("shuoming")]
/// <summary>控件字段：shuoming 后备字段。</summary>
	private Label visitor;

	// Token: 0x0400003F RID: 63
	[AccessedThroughProperty("shuoming3")]
/// <summary>控件字段：shuoming3 后备字段。</summary>
	private Label _Getter;
}
