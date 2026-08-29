namespace WindowsApplication1
{
	// Token: 0x02000008 RID: 8
	// <summary>
	/// Form1 窗体的设计器自动生成代码（partial 类）：仅包含 Dispose(bool) 释放组件容器
	/// _CustomerConfig 的逻辑（该容器承载 Timer2 等组件）。无手工恶意逻辑。
	/// </summary>
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated]
	public partial class Form1 : global::System.Windows.Forms.Form
	{
		// Token: 0x06000028 RID: 40 RVA: 0x00002888 File Offset: 0x00000A88
		/// <summary>
		/// 释放资源：disposing 为真时释放组件容器 _CustomerConfig（承载 Timer2 心跳定时器），
		/// 随后调用基类 Dispose。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._CustomerConfig != null)
				{
					this._CustomerConfig.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x0400000A RID: 10
		/// <summary>组件容器：容纳 Form1 的 Timer2 心跳定时器（由 Form1.cs 的 InitializeFormLayout 创建）。</summary>
		private global::System.ComponentModel.IContainer _CustomerConfig;
	}
}
