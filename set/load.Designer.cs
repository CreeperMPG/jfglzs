namespace WindowsApplication1
{
		/// <summary>
		/// load 设计器分部类（自动生成）：仅含 Dispose(bool) 与容器字段；控件初始化在 load.cs 的 BuildPasswordFormUi()。
		/// </summary>
	// Token: 0x0200000C RID: 12
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated]
	public partial class load : global::System.Windows.Forms.Form
	{
		/// <summary>
		/// 释放资源：disposing 时释放 m_ReponseMock 容器，再 base.Dispose。
		/// </summary>
		// Token: 0x060000CE RID: 206 RVA: 0x0000AFC8 File Offset: 0x000091C8
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.m_ReponseMock != null)
				{
					this.m_ReponseMock.Dispose();
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
		// Token: 0x04000064 RID: 100
		private global::System.ComponentModel.IContainer m_ReponseMock;
	}
}
