using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace about.Internal
{
	// Token: 0x02000002 RID: 2
	/// <summary>
	/// VB.NET 生成的 My.Application（WindowsFormsApplicationBase 子类，原名 ReponseProduct）：程序真正的入口。
	/// Main 设置兼容文本渲染后调用 WrapperProduct.MyApplication.Run(args) 启动消息循环；
	/// OnCreateMainForm 指定主窗体为 WrapperProduct.MyForms.Form1（其 Load 事件随即生成随机监控进程名）。
	/// 应用配置：非单实例、启用视觉样式、退出时保存设置、主窗体关闭即退出。
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	internal class ReponseProduct : WindowsFormsApplicationBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000024BC File Offset: 0x000006BC
		/// <summary>
		/// Main 入口（原名 CallReponse）：STAThread；设置兼容文本渲染（UseCompatibleTextRendering）后启动应用消息循环。
		/// csproj StartupObject = about.Internal.ReponseProduct。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[STAThread]
		internal static void Main(string[] args)
		{
			try
			{
				Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			}
			finally
			{
			}
			WrapperProduct.MyApplication.Run(args);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000024F4 File Offset: 0x000006F4
		/// <summary>构造函数：配置应用运行模式（非单实例、视觉样式、退出保存设置、主窗体关闭即退出）。</summary>
		public ReponseProduct() : base(AuthenticationMode.Windows)
		{
			this.IsSingleInstance = false;
			this.EnableVisualStyles = true;
			this.SaveMySettingsOnExit = true;
			this.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021B4 File Offset: 0x000003B4
		/// <summary>指定主窗体：MainForm = WrapperProduct.MyForms.Form1（单例）。</summary>
		protected override void OnCreateMainForm()
		{
			this.MainForm = WrapperProduct.MyForms.Form1;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>混淆填充桩（原名 CreateListener），恒 true，无实义。</summary>
		internal static bool CreateListener()
		{
			return true;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000021C9 File Offset: 0x000003C9
		/// <summary>混淆填充桩（原名 SetupListener），恒 false，无实义。</summary>
		internal static bool SetupListener()
		{
			return false;
		}
	}
}
