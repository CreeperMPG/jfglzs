using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace set.Internal
{
	/// <summary>
	/// 程序入口。本类是 VB.NET 的「Windows 应用程序框架」宿主（<see cref="WindowsFormsApplicationBase"/> 子类），
	/// 由编译器模板生成，<see cref="主入口"/> 就是 <c>Main</c>。
	/// 启动流程只有两步：先固定 WinForms 文本渲染方式，再把控制权交给 VB 应用框架的 Run()；
	/// 真正的首个窗体（load = 载入/卡密验证窗口）由 <see cref="OnCreateMainForm"/> 指定。
	/// </summary>
	// Token: 0x02000002 RID: 2
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class InfoAttr : WindowsFormsApplicationBase
	{
		/// <summary>
		/// 程序 Main。设为单实例=否、启用视觉样式、退出时保存 My.Settings，主窗体关闭即退出。
		/// </summary>
		/// <param name="args">命令行参数，原样交给 VB 应用框架（本样本未使用）。</param>
		// Token: 0x06000001 RID: 1 RVA: 0x000021B4 File Offset: 0x000003B4
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[STAThread]
		internal static void 主入口(string[] args)
		{
			// 按 VB 的 UseCompatibleTextRendering（false）固定控件文本渲染方式
			Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			// 交给 VB 应用框架启动（内部会触发 OnCreateMainForm 建主窗体）
			CandidateAttr.应用程序.Run(args);
		}

		/// <summary>初始化 VB 应用框架的启动选项（单实例、视觉样式、退出保存设置、关闭模式）。</summary>
		// Token: 0x06000002 RID: 2 RVA: 0x0000294C File Offset: 0x00000B4C
		public InfoAttr()
			: base(AuthenticationMode.Windows)
		{
			base.IsSingleInstance = false;
			base.EnableVisualStyles = true;
			base.SaveMySettingsOnExit = true;
			base.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		/// <summary>指定启动窗体：<c>load</c>（首次运行要输卡密的载入窗口）。</summary>
		// Token: 0x06000003 RID: 3 RVA: 0x000021CB File Offset: 0x000003CB
		protected override void OnCreateMainForm()
		{
			base.MainForm = CandidateAttr.窗体集合.load;
		}
	}
}
