// ============================================================================
// Form1.Designer.cs — Form1 设计器分部（自动生成）
// 真实职责：VS 窗体设计器自动生成的分部文件，仅包含 Dispose(bool) 释放逻辑（释放混淆后名为 queue 的
// IContainer 容器，实际承载托盘图标与 7 个定时器）与容器字段声明。无手工业务逻辑；真正的控件初始化
// 在 Form1.cs 的 初始化组件（原 ValidateBroadcaster）中（反编译合并了 InitializeComponent）。
// 说明：自动生成文件，按规则仅加注释不重命名。
// ============================================================================
﻿// Token: 0x02000009 RID: 9
[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated]
public partial class Form1 : global::System.Windows.Forms.Form
{
	// Token: 0x06000038 RID: 56 RVA: 0x00002D94 File Offset: 0x00000F94
/// <summary>释放资源：disposing 时释放 queue 容器（承载托盘图标与全部定时器）。</summary>
	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing && this.queue != null)
			{
				this.queue.Dispose();
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	// Token: 0x04000010 RID: 16
/// <summary>组件容器字段：承载 NotifyIcon1 与 7 个 Timer（混淆原名 queue 保留）。</summary>
	private global::System.ComponentModel.IContainer queue;
}
