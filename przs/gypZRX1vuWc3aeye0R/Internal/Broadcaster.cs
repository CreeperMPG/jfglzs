using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

// ============================================================================
// Broadcaster.cs — VB.NET 应用程序入口（My.Application / WindowsFormsApplicationBase）
// 真实职责：反混淆后为 VB.NET 的 My.Application 对象（MyTemplate 生成，ConfuserEx 改名 Broadcaster）。
// 定义程序启动方式：Main 入口（程序入口Main）→ 设置兼容渲染 → 运行 My.Application；
// OnCreateMainForm 把主窗体指定为 Form1（恶意主控窗体）。IsSingleInstance=false、
// EnableVisualStyles=true、SaveMySettingsOnExit=true、ShutdownMode=AfterMainFormCloses。
// 说明：类名保持混淆原名，方法已按 Broadcaster.cs.md 映射表反混淆重命名。
// ============================================================================
namespace gypZRX1vuWc3aeye0R.Internal
{
	// Token: 0x02000002 RID: 2
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class Broadcaster : WindowsFormsApplicationBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000024F8 File Offset: 0x000006F8
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[STAThread]
/// <summary>
/// 程序入口 Main（[STAThread]）：1. Application.SetCompatibleTextRenderingDefault（try/finally 空壳）；
/// 2. Account.MyApplication.Run(args) 启动 WindowsFormsApplicationBase 消息循环。
/// </summary>
		internal static void 程序入口Main(string[] args)
		{
			try
			{
				Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			}
			finally
			{
			}
			Account.MyApplication.Run(args);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002530 File Offset: 0x00000730
/// <summary>
/// 构造函数：IsSingleInstance=false、EnableVisualStyles=true、SaveMySettingsOnExit=true、
/// ShutdownMode=AfterMainFormCloses。
/// </summary>
		public Broadcaster() : base(AuthenticationMode.Windows)
		{
			this.IsSingleInstance = false;
			this.EnableVisualStyles = true;
			this.SaveMySettingsOnExit = true;
			this.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021B4 File Offset: 0x000003B4
/// <summary>
/// 指定主窗体：MainForm = Account.MyForms.Form1——程序一启动即实例化 Form1
/// （触发隐藏窗口、自我复制、注册表持久化、全盘文件监视等恶意初始化）。
/// </summary>
		protected override void OnCreateMainForm()
		{
			this.MainForm = Account.MyForms.Form1;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021C6 File Offset: 0x000003C6
/// <summary>混淆器填充方法（恒返回 false，无意义）。</summary>
		internal static bool SetThread()
		{
			return false;
		}
	}
}
