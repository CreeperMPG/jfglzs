using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace WindowsApplication1.Internal
{
	/// <summary>
	/// VB.NET 的应用程序模型（`My.Application`）实现，同时是本程序集的**启动对象**
	/// （工程文件的 StartupObject 指向这里）。它把 WinForms 的启动参数定死：
	/// 非单实例、启用视觉样式、退出时保存设置、主窗体关闭即退出，
	/// 并把主窗体指向 <see cref="ExpressionMapper.VisitComposer"/> 里的 Form1。
	/// </summary>
	// Token: 0x02000002 RID: 2
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class ComposerMapper : WindowsFormsApplicationBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000021B4 File Offset: 0x000003B4
		/// <summary>
		/// 程序入口。先按 VB 的约定设置文本渲染兼容模式，再把命令行参数交给
		/// <see cref="ExpressionMapper.CancelComposer"/> 的 Run() 进入消息循环。
		/// [STAThread] 必须保留：WinForms 需要单线程单元。
		/// </summary>
		[STAThread]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static void 程序入口(string[] args)
		{
			Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			ExpressionMapper.CancelComposer.Run(args);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000025D4 File Offset: 0x000007D4
		/// <summary>
		/// 应用程序模型配置：Windows 身份验证模式；禁用单实例（允许同时开多个）；
		/// 启用视觉样式；退出时保存 My.Settings；主窗体全部关闭后退出整个程序。
		/// 这些值直接决定程序的存活行为，不是可选项。
		/// </summary>
		public ComposerMapper()
			: base(AuthenticationMode.Windows)
		{
			base.IsSingleInstance = false;
			base.EnableVisualStyles = true;
			base.SaveMySettingsOnExit = true;
			base.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021CB File Offset: 0x000003CB
		/// <summary>
		/// 指定主窗体：取 `My.Forms.Form1` 的单例（首次访问时构造）。窗体由
		/// <see cref="ExpressionMapper.ClientMapper"/> 缓存，关掉再开还是同一个实例。
		/// </summary>
		protected override void OnCreateMainForm()
		{
			base.MainForm = ExpressionMapper.VisitComposer.Form1;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>恒返回 true 的桩方法，无调用点。</summary>
		internal static bool 返回真占位()
		{
			return true;
		}
	}
}
