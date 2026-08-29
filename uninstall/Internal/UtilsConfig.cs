using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000002 RID: 2
	// <summary>
	/// VB.NET WindowsFormsApplicationBase 子类，即 My.Application。
	/// Main(args)（原 EnableUtils）是程序集入口引导：设置兼容文本渲染后调用
	/// VisitorConfig.Application.Run(args) 启动 WinForms 消息循环；
	/// OnCreateMainForm 把主窗体设为 Form1（卸载入口窗体）。
	/// 启动顺序：Main → Run → OnCreateMainForm → Form1 创建 → Form1_Load → 静默卸载或弹 Form2。
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	internal class UtilsConfig : WindowsFormsApplicationBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002488 File Offset: 0x00000688
		/// <summary>
		/// 程序入口 Main（原混淆名 EnableUtils）：SetCompatibleTextRenderingDefault 后
		/// 调用 VisitorConfig.Application.Run(args) 启动消息循环，主窗体由 OnCreateMainForm 指定。
		/// </summary>
		[STAThread]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static void Main(string[] args)
		{
			try
			{
				Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			}
			finally
			{
			}
			VisitorConfig.Application.Run(args);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000024C0 File Offset: 0x000006C0
		/// <summary>
		/// 构造函数：IsSingleInstance=false、EnableVisualStyles=true、
		/// SaveMySettingsOnExit=true、ShutdownStyle=AfterMainFormCloses（模板生成骨架）。
		/// </summary>
		public UtilsConfig() : base(AuthenticationMode.Windows)
		{
			this.IsSingleInstance = false;
			this.EnableVisualStyles = true;
			this.SaveMySettingsOnExit = true;
			this.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021B4 File Offset: 0x000003B4
		/// <summary>
		/// 指定主窗体：MainForm = VisitorConfig.Forms.Form1（卸载入口窗体）——
		/// 本文件唯一的关键业务绑定。
		/// </summary>
		protected override void OnCreateMainForm()
		{
			this.MainForm = VisitorConfig.Forms.Form1;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>恒 false 混淆桩，无实际逻辑。</summary>
		internal static bool SetRegistry()
		{
			return false;
		}
	}
}
