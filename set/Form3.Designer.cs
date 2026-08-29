namespace WindowsApplication1
{
		/// <summary>
		/// Form3 设计器分部类（自动生成）：仅含 Dispose(bool) 与容器字段；控件初始化在 Form3.cs 的 BuildReminderFormUi()。
		/// </summary>
	// Token: 0x0200000B RID: 11
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated]
	public partial class Form3 : global::System.Windows.Forms.Form
	{
		/// <summary>
		/// 释放资源：disposing 时释放 filterMock 容器，再 base.Dispose。
		/// </summary>
		// Token: 0x060000C3 RID: 195 RVA: 0x0000AB88 File Offset: 0x00008D88
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.filterMock != null)
				{
					this.filterMock.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		/// 组件容器字段（Designer 自动生成，控件归属管理用）。
		/// </summary>
		// Token: 0x04000060 RID: 96
		private global::System.ComponentModel.IContainer filterMock;
	}
}
