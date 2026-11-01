using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace OZxTKXuvsl9D34LWhP.Internal
{
	/// <summary>
	/// VB 的 <c>My.Application</c>（类名 Val 是混淆名，原类型名应为 <c>MyApplication</c>）。
	/// <b>它同时是本进程的入口点</b>：csproj 的 <c>&lt;StartupObject&gt;</c> 指向
	/// <c>OZxTKXuvsl9D34LWhP.Internal.Val</c>，即本类的 <see cref="CallVal"/>（带 [STAThread]）。
	///
	/// 行为：
	/// <list type="bullet">
	/// <item>构造函数设 <c>IsSingleInstance=false</c>（可以多开）、<c>EnableVisualStyles=true</c>、
	///   <c>SaveMySettingsOnExit=true</c>、<c>ShutdownStyle = AfterMainFormCloses</c>；</item>
	/// <item><see cref="OnCreateMainForm"/> 把主窗体设成 <c>Queue.MyForms.Form1</c>——
	///   也就是说启动的就是那个隐藏运行的管控窗体；</item>
	/// <item><see cref="CallVal"/> 先设文本渲染默认值，再 <c>Queue.MyApplication.Run(args)</c>。</item>
	/// </list>
	/// 类名、入口方法名都**不能改**：csproj 的 <c>StartupObject</c> 直接写死了
	/// <c>OZxTKXuvsl9D34LWhP.Internal.Val</c>，改名就要动工程文件（本任务禁止）。
	/// </summary>
	// Token: 0x02000002 RID: 2
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class Val : WindowsFormsApplicationBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000021B4 File Offset: 0x000003B4
		/// <summary>
		/// 进程入口（原名 CallVal，csproj 的 StartupObject 指向本类）：
		/// 设置兼容文本渲染，然后 <c>Queue.MyApplication.Run(args)</c> 进入消息循环。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[STAThread]
		internal static void CallVal(string[] args)
		{
			Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			Queue.MyApplication.Run(args);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000026B4
		/// <summary>构造 My.Application：允许多开、开视觉样式、退出时保存设置、主窗体关闭后关程序。</summary>
		public Val()
			: base(AuthenticationMode.Windows)
		{
			base.IsSingleInstance = false;
			base.EnableVisualStyles = true;
			base.SaveMySettingsOnExit = true;
			base.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021CB
		/// <summary>把主窗体指定为 <see cref="Form1"/>（隐藏运行的管控核心）。</summary>
		protected override void OnCreateMainForm()
		{
			base.MainForm = Queue.MyForms.Form1;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021DD
		/// <summary>.NET Reactor 生成的授权标记桩，恒返回 true，无调用者（原名 EnableTag）。</summary>
		internal static bool EnableTag()
		{
			return true;
		}
	}
}
