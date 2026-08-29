namespace WindowsApplication1
{
		/// <summary>
		/// Form1 设计器分部类（自动生成）：仅含 Dispose(bool) 释放容器与字段声明；
		/// 控件初始化被反编译器合并进主文件 Form1.cs 的 BuildUi_CreateAllControls()，本文件无手工逻辑。
		/// </summary>
	// Token: 0x02000008 RID: 8
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated]
	public partial class Form1 : global::System.Windows.Forms.Form
	{
		/// <summary>
		/// 释放资源：disposing 时释放 _Authentication 容器，再 base.Dispose。
		/// </summary>
		// Token: 0x0600002A RID: 42 RVA: 0x00002B30 File Offset: 0x00000D30
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._Authentication != null)
				{
					this._Authentication.Dispose();
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
		// Token: 0x0400000B RID: 11
		private global::System.ComponentModel.IContainer _Authentication;
	}
}
