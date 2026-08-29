namespace WindowsApplication1
{
	// Token: 0x0200000A RID: 10
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated]
	// ============================================================
	// Form2 设计器分部类（自动生成）：重写 Dispose(bool) 释放组件容器 componentsContainer（原名 m_ParamsVisitor，Form2 控件与 Timer_frm2 的宿主容器）。
	// ============================================================
	public partial class Form2 : global::System.Windows.Forms.Form
	{
		// Token: 0x06000071 RID: 113 RVA: 0x000073A0 File Offset: 0x000055A0
		/// <summary>
		/// 释放资源：disposing 时释放组件容器 componentsContainer，再调用基类。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.componentsContainer != null)
				{
					this.componentsContainer.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x04000080 RID: 128
		private global::System.ComponentModel.IContainer componentsContainer;
	}
}
