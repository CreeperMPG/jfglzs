using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1
{
	// Token: 0x0200000C RID: 12
	[DesignerGenerated]
	// ============================================================
	// Form3.cs — 警告提示窗（非锁定，仅显示违规说明）
	// 居中的置顶警告窗（无边框），三个红色说明标签（shuoming/shuoming2/shuoming3），无按钮、无密码输入。由 Form1 各监控逻辑经 BeginInvoke 或定时器弹出；超阈值后被 Form2/Form4 锁屏取代并 Close()。纯 UI 组件，无恶意逻辑。
	// ============================================================
	public partial class Form3 : Form
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00002271 File Offset: 0x00000471
		/// <summary>
		/// 构造器：调用 InitControlsAndLayout() 初始化控件。
		/// </summary>
		public Form3()
		{
			this.InitControlsAndLayout();
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00008458 File Offset: 0x00006658
		/// <summary>
		/// 初始化控件与布局（原名 RemoveTemplate）：创建 shuoming（15.75pt 红字）、shuoming2（15.75pt 红字）、shuoming3（14.25pt 说明/水印）；BackColor=SystemColors.Highlight（蓝底）；StartPosition=CenterScreen、TopMost=true、无边框。
		/// </summary>
		private void InitControlsAndLayout()
		{
			this.shuoming = new Label();
			this.shuoming2 = new Label();
			this.shuoming3 = new Label();
			this.SuspendLayout();
			this.shuoming.AutoSize = true;
			this.shuoming.Font = new Font("宋体", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming.ForeColor = Color.Red;
			Control shuoming = this.shuoming;
			Point location = new Point(72, 202);
			shuoming.Location = location;
			this.shuoming.Name = "shuoming";
			Control shuoming2 = this.shuoming;
			Size size = new Size(0, 21);
			shuoming2.Size = size;
			this.shuoming.TabIndex = 17;
			this.shuoming2.AutoSize = true;
			this.shuoming2.Font = new Font("宋体", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming2.ForeColor = Color.Red;
			Control shuoming3 = this.shuoming2;
			location = new Point(72, 266);
			shuoming3.Location = location;
			this.shuoming2.Name = "shuoming2";
			Control shuoming4 = this.shuoming2;
			size = new Size(21, 21);
			shuoming4.Size = size;
			this.shuoming2.TabIndex = 18;
			this.shuoming2.Text = " ";
			this.shuoming3.AutoSize = true;
			this.shuoming3.Font = new Font("宋体", 14.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			Control shuoming5 = this.shuoming3;
			location = new Point(76, 337);
			shuoming5.Location = location;
			this.shuoming3.Name = "shuoming3";
			Control shuoming6 = this.shuoming3;
			size = new Size(0, 19);
			shuoming6.Size = size;
			this.shuoming3.TabIndex = 19;
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Highlight;
			size = new Size(1260, 558);
			this.ClientSize = size;
			this.Controls.Add(this.shuoming3);
			this.Controls.Add(this.shuoming2);
			this.Controls.Add(this.shuoming);
			this.Name = "Form3";
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "警告";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000086D4 File Offset: 0x000068D4
		// (set) Token: 0x06000094 RID: 148 RVA: 0x0000227F File Offset: 0x0000047F
		/// <summary>
		/// 警告主文案标签（由 Form1 各监控逻辑动态赋值）。
		/// </summary>
		internal virtual Label shuoming
		{
			get
			{
				return this.m_MessageVisitor;
			}
			set
			{
				this.m_MessageVisitor = value;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000095 RID: 149 RVA: 0x000086E8 File Offset: 0x000068E8
		// (set) Token: 0x06000096 RID: 150 RVA: 0x00002288 File Offset: 0x00000488
		/// <summary>
		/// 警告主文案标签（由 Form1 各监控逻辑动态赋值）。
		/// </summary>
		internal virtual Label shuoming2
		{
			get
			{
				return this._ProductVisitor;
			}
			set
			{
				this._ProductVisitor = value;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000097 RID: 151 RVA: 0x000086FC File Offset: 0x000068FC
		// (set) Token: 0x06000098 RID: 152 RVA: 0x00002291 File Offset: 0x00000491
		/// <summary>
		/// 警告主文案标签（由 Form1 各监控逻辑动态赋值）。
		/// </summary>
		internal virtual Label shuoming3
		{
			get
			{
				return this._ExceptionVisitor;
			}
			set
			{
				this._ExceptionVisitor = value;
			}
		}

		// Token: 0x04000091 RID: 145
		[AccessedThroughProperty("shuoming")]
		/// <summary>
		/// shuoming（警告主文案）后备字段。
		/// </summary>
		private Label m_MessageVisitor;

		// Token: 0x04000092 RID: 146
		[AccessedThroughProperty("shuoming2")]
		/// <summary>
		/// shuoming2（警告副文案）后备字段。
		/// </summary>
		private Label _ProductVisitor;

		// Token: 0x04000093 RID: 147
		[AccessedThroughProperty("shuoming3")]
		/// <summary>
		/// shuoming3（说明/水印）后备字段。
		/// </summary>
		private Label _ExceptionVisitor;
	}
}
