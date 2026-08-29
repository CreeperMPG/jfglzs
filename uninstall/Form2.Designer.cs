namespace WindowsApplication1
{
	// Token: 0x02000009 RID: 9
	// <summary>
	/// Form2 窗体的设计器自动生成代码（partial 类）：仅包含 Dispose(bool) 释放组件容器
	/// _InfoConfig 的逻辑（该容器承载 Timer_frm2 等组件）。无手工恶意逻辑。
	/// </summary>
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated]
	public partial class Form2 : global::System.Windows.Forms.Form
	{
		// Token: 0x0600002F RID: 47 RVA: 0x00002DB4 File Offset: 0x00000FB4
		/// <summary>
		/// 释放资源：disposing 为真时释放组件容器 _InfoConfig（承载 Timer_frm2 等组件），
		/// 随后调用基类 Dispose。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._InfoConfig != null)
				{
					this._InfoConfig.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x0400000C RID: 12
		/// <summary>组件容器：容纳 Form2 的 Timer_frm2 定时器（由 Form2.cs 的 InitializeFormLayout 创建）。</summary>
		private global::System.ComponentModel.IContainer _InfoConfig;
	}
}
