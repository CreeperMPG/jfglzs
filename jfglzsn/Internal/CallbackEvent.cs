using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace jfglzsn.Internal
{
	// Token: 0x02000002 RID: 2
	/// <summary>
	/// 程序启动器，等价于 VB.NET 模板里的「My.Application」。
	/// <para>
	/// <see cref="GetCallback"/> 是整个程序的入口（<c>[STAThread]</c>）：先设置 WinForms 文本渲染兼容模式，
	/// 再通过 <see cref="SetterEvent.CollectCallback"/>.<c>Run</c> 启动 WinForms 应用框架。
	/// 构造器里关掉了单实例限制（<c>IsSingleInstance = false</c>，允许同时跑多份）、
	/// 打开视觉样式、开启退出时保存用户设置，并要求主窗体关闭后即退出。
	/// </para>
	/// <para>
	/// 启动链：<see cref="GetCallback"/> → <see cref="SetterEvent.CollectCallback"/>.Run →
	/// <see cref="OnCreateMainForm"/> → 主窗体 Form1。
	/// </para>
	/// </summary>
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class CallbackEvent : WindowsFormsApplicationBase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000021B4 File Offset: 0x000003B4
		/// <summary>
		/// 进程入口点。<c>SetCompatibleTextRenderingDefault</c> 必须在创建任何窗体之前调用，
		/// 随后把命令行参数原样交给应用框架。
		/// </summary>
		/// <param name="args">命令行参数，由运行时传入，透传给 <c>Run</c>。</param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[STAThread]
		internal static void GetCallback(string[] args)
		{
			Application.SetCompatibleTextRenderingDefault(WindowsFormsApplicationBase.UseCompatibleTextRendering);
			SetterEvent.CollectCallback.Run(args);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002704 File Offset: 0x00000904
		/// <summary>
		/// 应用框架初始化：Windows 身份验证模式、不限制单实例、启用视觉样式、
		/// 退出时保存 My.Settings、主窗体关闭即退出。
		/// </summary>
		public CallbackEvent()
			: base(AuthenticationMode.Windows)
		{
			base.IsSingleInstance = false;
			base.EnableVisualStyles = true;
			base.SaveMySettingsOnExit = true;
			base.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000021CB File Offset: 0x000003CB
		/// <summary>
		/// 指定主窗体：由 <see cref="SetterEvent.ComputeCallback"/>（My.Forms）懒加载出 Form1 的实例。
		/// </summary>
		protected override void OnCreateMainForm()
		{
			base.MainForm = SetterEvent.ComputeCallback.Form1;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>混淆器/模板插入的判定桩，恒返回 <c>true</c>，工程内无引用。</summary>
		internal static bool CountDefinition()
		{
			return true;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000021E0 File Offset: 0x000003E0
		/// <summary>混淆器/模板插入的判定桩，恒返回 <c>false</c>，工程内无引用。</summary>
		internal static bool SelectDefinition()
		{
			return false;
		}
	}
}
