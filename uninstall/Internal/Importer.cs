using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000002 RID: 2
	/// <summary>
	/// VB.NET 自动生成的 My.Application（WindowsFormsApplicationBase 派生，真实身份已确认：
	/// [HelpKeyword("My.Application")] 指向它，同时 csproj 的 StartupObject 也是这个类）。
	/// 职责：设置单实例/视觉样式/退出保存策略，把主窗体定为 Form1。
	///
	/// 注意：类名 Importer 被 csproj 的 &lt;StartupObject&gt; 引用，不能改，否则工程入口丢失；
	/// 原混淆名：Importer
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	internal class Importer : WindowsFormsApplicationBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000021B4 File Offset: 0x000003B4
		/// <summary>
		/// 程序入口（等价于 VB 自动生成的 Sub Main）：
		/// 先固定「兼容文本渲染」开关，再把命令行原样交给 My.Application 的 Run。
		/// 原混淆名：VerifyReader
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[STAThread]
		internal static void Main(string[] 命令行参数)
		{
			Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			MyProject.Application.Run(命令行参数);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002570 File Offset: 0x00000770
		/// <summary>
		/// 四个应用级开关全部在构造函数里硬编码（对应工程属性页的设置）：
		/// 非单实例、启用视觉样式、退出时保存 My.Settings、主窗体关闭即退出。
		/// </summary>
		public Importer()
			: base(AuthenticationMode.Windows)
		{
			base.IsSingleInstance = false;
			base.EnableVisualStyles = true;
			base.SaveMySettingsOnExit = true;
			base.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021CB File Offset: 0x000003CB
		/// <summary>
		/// 指定主窗体：通过 My.Forms.Form1 拿单例，所以启动时显示的一定是 Form1（隐藏的卸载器窗口）。
		/// </summary>
		protected override void OnCreateMainForm()
		{
			base.MainForm = MyProject.Forms.Form1;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>混淆器注入的占位方法：恒返回 true，无调用者。原混淆名：CallParser</summary>
		internal static bool 混淆器占位方法_恒真()
		{
			return true;
		}
	}
}
