using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1
{
	// Token: 0x0200000F RID: 15
	/// <summary>
	/// 「警告」窗体：一整块蓝底红字的提示牌（标题「警告」、1260×558、居中、置顶、无按钮、无输入框）。
	/// <para>
	/// 由主窗体 Form1 在检测到违规时创建：先往三个 <see cref="Label"/> 里写文案，再 <c>Show()</c>，
	/// 展示完随即 <c>Close()</c>。它自身**不做任何拦截**——杀进程、改注册表、锁屏全在 Form1 里，
	/// 这里只负责「吓唬学生」，是三级惩罚链里最温和的第一级。
	/// </para>
	/// <para>
	/// 三个标签的分工：<see cref="shuoming"/> 写违规原因，<see cref="shuoming2"/> 写处置要求，
	/// <see cref="shuoming3"/> 写作者联系方式（固定串 + 被判违规的进程名 + 固定串）。
	/// </para>
	/// </summary>
	[DesignerGenerated]
	public class Form3 : Form
	{
		// Token: 0x0600009B RID: 155 RVA: 0x000024EF File Offset: 0x000006EF
		/// <summary>构造函数：只做 VB 设计器初始化（建三个标签、设窗体外观），没有任何业务逻辑。</summary>
		public Form3()
		{
			this.初始化组件();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00007E54 File Offset: 0x00006054
		/// <summary>
		/// 释放设计器组件容器（本窗体实际为 <c>null</c>），随后在 <c>finally</c> 里调用基类释放——
		/// <c>try</c>/<c>finally</c> 保证基类释放一定执行。
		/// </summary>
		/// <param name="disposing">为 <c>true</c> 时同时释放托管资源。</param>
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

		// Token: 0x0600009D RID: 157 RVA: 0x00007E94 File Offset: 0x00006094
		/// <summary>
		/// VB 设计器的 <c>InitializeComponent</c>（方法名被 .NET Reactor 洗成 <c>CloneCallback</c>）：
		/// 建三个红字标签并设置窗体外观。
		/// <para>
		/// 外观就是「惩罚界面」本体：<see cref="SystemColors.Highlight"/> 蓝底、<c>TopMost</c> 置顶、
		/// <c>CenterScreen</c> 居中、1260×558——足以盖住学生的操作区，但没有遮罩全屏，也不拦输入。
		/// </para>
		/// <para>
		/// 字体参数里最后一个 <c>134</c> 是 <c>GdiCharSet</c>（简体中文代码页 936 对应的 GB2312 字符集），
		/// 全工程统一。控件名 <c>shuoming*</c> 是设计器里的拼音名，Form1 会按名字跨窗体引用，不能改。
		/// </para>
		/// </summary>
		private void 初始化组件()
		{
			this.shuoming = new Label();
			this.shuoming2 = new Label();
			this.shuoming3 = new Label();
			base.SuspendLayout();
			this.shuoming.AutoSize = true;
			this.shuoming.Font = new Font("宋体", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming.ForeColor = Color.Red;
			this.shuoming.Location = new Point(72, 202);
			this.shuoming.Name = "shuoming";
			this.shuoming.Size = new Size(0, 21);
			this.shuoming.TabIndex = 17;
			this.shuoming2.AutoSize = true;
			this.shuoming2.Font = new Font("宋体", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming2.ForeColor = Color.Red;
			this.shuoming2.Location = new Point(72, 266);
			this.shuoming2.Name = "shuoming2";
			this.shuoming2.Size = new Size(21, 21);
			this.shuoming2.TabIndex = 18;
			this.shuoming2.Text = " ";
			this.shuoming3.AutoSize = true;
			this.shuoming3.Font = new Font("宋体", 14.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming3.Location = new Point(76, 337);
			this.shuoming3.Name = "shuoming3";
			this.shuoming3.Size = new Size(0, 19);
			this.shuoming3.TabIndex = 19;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Highlight;
			base.ClientSize = new Size(1260, 558);
			base.Controls.Add(this.shuoming3);
			base.Controls.Add(this.shuoming2);
			base.Controls.Add(this.shuoming);
			base.Name = "Form3";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "警告";
			base.TopMost = true;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600009E RID: 158 RVA: 0x000024FD File Offset: 0x000006FD
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00002505 File Offset: 0x00000705
		/// <summary>违规原因（红色、15.75pt 宋体，绝对定位 72,202），由 Form1 按违规类型写入。</summary>
		internal virtual Label shuoming { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000250E File Offset: 0x0000070E
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x00002516 File Offset: 0x00000716
		/// <summary>处置要求（红色、15.75pt 宋体，绝对定位 72,266），例如「请马上恢复网络，否则会全屏锁定！」。</summary>
		internal virtual Label shuoming2 { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x0000251F File Offset: 0x0000071F
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x00002527 File Offset: 0x00000727
		/// <summary>作者联系方式/附加说明（14.25pt 宋体，默认前景色），内容是混淆串 + 进程名 + 混淆串。</summary>
		internal virtual Label shuoming3 { get; set; }

		// Token: 0x060000A4 RID: 164 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>
		/// 壳注入的判定桩：恒返回 <c>true</c>，工程内没有任何引用（.NET Reactor 完整性检查的残留）。
		/// </summary>
		internal static bool GetRule()
		{
			return true;
		}

		// Token: 0x04000095 RID: 149
		/// <summary>
		/// 设计器组件容器。本窗体的控件都是直接 <c>new</c> 的，从不给这个字段赋值，
		/// 因此它实际恒为 <c>null</c>，<see cref="Dispose(bool)"/> 只是按惯例判空。
		/// </summary>
		private IContainer 组件容器;

		// Token: 0x04000096 RID: 150
		/// <summary><see cref="shuoming"/> 的后备字段（属性访问器直接读写它，<c>[AccessedThroughProperty]</c> 绑定）。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("shuoming")]
		private Label m_ValEvent;

		// Token: 0x04000097 RID: 151
		/// <summary><see cref="shuoming2"/> 的后备字段。</summary>
		[AccessedThroughProperty("shuoming2")]
		[CompilerGenerated]
		private Label singletonEvent;

		// Token: 0x04000098 RID: 152
		/// <summary><see cref="shuoming3"/> 的后备字段。</summary>
		[AccessedThroughProperty("shuoming3")]
		[CompilerGenerated]
		private Label m_StructEvent;
	}
}
