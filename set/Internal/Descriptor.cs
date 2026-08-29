using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace WindowsApplication1.Internal
{
		/// <summary>
		/// VB 应用框架 My.Application（WindowsFormsApplicationBase 子类）：程序正式入口。
		/// Main_ProgramEntry 带 [STAThread] 等价于 Main；OnCreateMainForm 把主窗体设为 Reader.MyForms.load（密码门窗体）。
		/// </summary>
	// Token: 0x02000002 RID: 2
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class Descriptor : WindowsFormsApplicationBase
	{
		/// <summary>
		/// 程序入口（Main）：Application.SetCompatibleTextRenderingDefault 后 Reader.MyApplication.Run(args) 进入 VB 应用框架消息循环；
		/// 框架回调 OnCreateMainForm → 主窗体 = load（密码验证 → Form1 设置界面）。
		/// </summary>
		// Token: 0x06000001 RID: 1 RVA: 0x00002664 File Offset: 0x00000864
		[STAThread]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static void Main_ProgramEntry(string[] args)
		{
			try
			{
				Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			}
			finally
			{
			}
			Reader.MyApplication.Run(args);
		}

		/// <summary>
		/// 初始化框架：IsSingleInstance=false、EnableVisualStyles=true、SaveMySettingsOnExit=true、ShutdownStyle=AfterMainFormCloses。
		/// </summary>
		// Token: 0x06000002 RID: 2 RVA: 0x0000269C File Offset: 0x0000089C
		public Descriptor() : base(AuthenticationMode.Windows)
		{
			this.IsSingleInstance = false;
			this.EnableVisualStyles = true;
			this.SaveMySettingsOnExit = true;
			this.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		/// <summary>
		/// 指定主窗体：MainForm = Reader.MyForms.load（入口链中枢）。
		/// </summary>
		// Token: 0x06000003 RID: 3 RVA: 0x000021B4 File Offset: 0x000003B4
		protected override void OnCreateMainForm()
		{
			this.MainForm = Reader.MyForms.load;
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 false，无实际逻辑。
		/// </summary>
		// Token: 0x06000004 RID: 4 RVA: 0x000021C6 File Offset: 0x000003C6
		internal static bool Placeholder_ReturnFalse()
		{
			return false;
		}
	}
}
