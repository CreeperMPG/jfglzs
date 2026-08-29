using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000002 RID: 2
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	// ============================================================
	// Internal/TemplateConfig.cs — VB 应用程序启动模板（程序入口，Main 函数所在）
	// VB.NET WinForms 项目程序入口（WindowsFormsApplicationBase 子类）：UpdateTemplate（=Main）设置兼容文本渲染后调用 TokenizerConfig.MyApplication.Run(args) 启动；OnCreateMainForm 把主窗体指定为 Form1（恶意主控窗体）。OrderParser 是壳/混淆器加入的恒 true 桩。
	// ============================================================
	internal class TemplateConfig : WindowsFormsApplicationBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002508 File Offset: 0x00000708
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[STAThread]
		/// <summary>
		/// Main 入口（原名 UpdateTemplate）：[STAThread]；Application.SetCompatibleTextRenderingDefault(...)（try/finally 空壳）后 TokenizerConfig.MyApplication.Run(args) 进入 VB 消息循环。
		/// </summary>
		internal static void UpdateTemplate(string[] args)
		{
			try
			{
				Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			}
			finally
			{
			}
			TokenizerConfig.MyApplication.Run(args);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002540 File Offset: 0x00000740
		/// <summary>
		/// 应用配置：IsSingleInstance=false、EnableVisualStyles=true、SaveMySettingsOnExit=true、ShutdownStyle=AfterMainFormCloses。
		/// </summary>
		public TemplateConfig() : base(AuthenticationMode.Windows)
		{
			this.IsSingleInstance = false;
			this.EnableVisualStyles = true;
			this.SaveMySettingsOnExit = true;
			this.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021B4 File Offset: 0x000003B4
		/// <summary>
		/// 指定主窗体：MainForm = TokenizerConfig.MyForms.Form1 —— 程序启动即进入恶意主控窗体 Form1。
		/// </summary>
		protected override void OnCreateMainForm()
		{
			this.MainForm = TokenizerConfig.MyForms.Form1;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>
		/// 壳桩：恒返回 true。
		/// </summary>
		internal static bool OrderParser()
		{
			return true;
		}
	}
}
