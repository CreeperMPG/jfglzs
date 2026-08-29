namespace WindowsApplication1
{
	// Token: 0x02000008 RID: 8
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated]
	// ============================================================
	// Form1 设计器分部类（自动生成）：重写 Dispose(bool) 释放组件容器 componentsContainer（原名 m_RulesConfig，在 Form1.InitTimersAndForms 中被用作全部 14 个定时器的宿主 Container）。
	// ============================================================
	public partial class Form1 : global::System.Windows.Forms.Form
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00002AF4 File Offset: 0x00000CF4
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

		// Token: 0x0400000C RID: 12
		private global::System.ComponentModel.IContainer componentsContainer;
	}
}
