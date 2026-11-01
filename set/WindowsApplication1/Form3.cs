using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using set.Internal;

namespace WindowsApplication1
{
	/// <summary>
	/// 「保存完成」提醒窗体 Form3（标题「提醒」，TopMost = true，屏幕居中）。
	/// 由 Form1.PrepareInfo 在设置全部写完并重启 explorer 之后 Show() 弹出。
	/// 同时兼一个权限自检：查 HKLM 的 Run 项里有没有 prozs，没有就把提示改成红字，
	/// 告诉用户要用管理员身份重跑 set.exe。
	/// </summary>
	/// <remarks>
	/// 敏感动作：「确  定」按钮调用 ProjectData.EndApp() 直接掐断进程（不触发正常关闭流程）；
	/// 读 HKLM 的 RUN 自启动项判断自启动是否写成功（本文件只读，写入在 Form1.DefineInfo）。
	/// 提示语「右击文件set.exe，选择 以管理员身份运行」本身是 IOC：这套限制需要管理员权限才能落地。
	/// </remarks>
	// Token: 0x0200000E RID: 14
	[DesignerGenerated]
	public class Form3 : Form
	{
		// Token: 0x060000CB RID: 203 RVA: 0x0000A370 File Offset: 0x00008570
		/// <summary>
		/// 构造函数：先挂 Load 处理器（自启动自检），再调用设计器布局方法（顺序为原版顺序）。
		/// </summary>
		public Form3()
		{
			base.Load += this.StopInfo;
			this.QueryInfo();
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000A39C File Offset: 0x0000859C
		/// <summary>
		/// 设计器生成的释放逻辑：disposing 为真时先释放设计器容器 _ErrorAttr，再调用基类 Dispose。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._ErrorAttr != null)
				{
					this._ErrorAttr.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000A3DC File Offset: 0x000085DC
		/// <summary>
		/// VB 的 InitializeComponent：创建「确  定」按钮与两段提示文字并设置外观（顺序为原版顺序）。
		/// 窗体外框 457×215、居中、置顶、标题「提醒」。
		/// </summary>
		private void QueryInfo()
		{
			this.Button2 = new Button();
			this.Label2 = new Label();
			this.Label1 = new Label();
			base.SuspendLayout();
			this.Button2.Location = new Point(348, 161);
			this.Button2.Name = "Button2";
			this.Button2.Size = new Size(78, 27);
			this.Button2.TabIndex = 1;
			this.Button2.Text = "确  定";
			this.Button2.UseVisualStyleBackColor = true;
			this.Label2.ForeColor = SystemColors.ControlText;
			this.Label2.Location = new Point(20, 20);
			this.Label2.Name = "Label2";
			this.Label2.Size = new Size(406, 59);
			this.Label2.TabIndex = 3;
			this.Label2.Text = "请在硬盘保护系统中创建进度或者保存系统（此步可省略）。";
			this.Label1.Location = new Point(20, 92);
			this.Label1.Name = "Label1";
			this.Label1.Size = new Size(406, 55);
			this.Label1.TabIndex = 4;
			this.Label1.Text = "注：软件在重启电脑后自动运行。";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(457, 215);
			base.Controls.Add(this.Label1);
			base.Controls.Add(this.Label2);
			base.Controls.Add(this.Button2);
			base.Name = "Form3";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "提醒";
			base.TopMost = true;
			base.ResumeLayout(false);
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00002792 File Offset: 0x00000992
		// (set) Token: 0x060000CF RID: 207 RVA: 0x0000A5C4 File Offset: 0x000087C4
		/// <summary>
		/// 「确  定」按钮（Button2）。setter 在换实例时同步退订/订阅 Click（指向 InitInfo）。
		/// </summary>
		internal virtual Button Button2
		{
			[CompilerGenerated]
			get
			{
				return this.creatorAttr;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.InitInfo);
				Button button = this.creatorAttr;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this.creatorAttr = value;
				button = this.creatorAttr;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x0000279A File Offset: 0x0000099A
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x000027A2 File Offset: 0x000009A2
		/// <summary>
		/// 提示标签（Label2）：默认黑字；自启动没写成功时改为红字并换成「权限不够」的说明。
		/// </summary>
		internal virtual Label Label2 { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x000027AB File Offset: 0x000009AB
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x000027B3 File Offset: 0x000009B3
		/// <summary>
		/// 说明标签（Label1）：固定文本「注：软件在重启电脑后自动运行。」
		/// </summary>
		internal virtual Label Label1 { get; set; }

		// Token: 0x060000D4 RID: 212 RVA: 0x00002378 File Offset: 0x00000578
		/// <summary>
		/// 「确  定」按钮：调用 ProjectData.EndApp() —— 相当于 VB 的 End，直接掐断进程，
		/// 不走正常的窗体关闭流程（不触发 FormClosing / Dispose）。
		/// </summary>
		private void InitInfo(object sender, EventArgs e)
		{
			ProjectData.EndApp();
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000A608 File Offset: 0x00008808
		/// <summary>
		/// Form_Load 自检：查 HKLM 的 RUN 项里有没有 prozs，判断自启动是否真的写进去了。
		/// 先查 32 位视图的 RUN，取到的值为空再查 WOW6432NODE 那份（64 位系统上 32 位程序的自启动位置）；
		/// 两处都空就把提示改成红字，提示用户用管理员身份重跑。
		/// </summary>
		/// <remarks>
		/// 原版是 VB 的 <c>On Error Resume Next</c>：<c>ClearProjectError()</c> 开场、<c>num2 = 2..7</c> 是状态号、
		/// 末尾 <c>catch when (endfilter(...))</c> 按状态号分发——读注册表抛异常就当没查到，继续往下判断。
		/// 这里把两次读取和两处赋值各自包 try/catch，保持「哪一条失败都不影响后面的判断」。
		/// 两处跳转的含义：IL_005A = 第一个键已取到值、跳过 WOW6432NODE 查询；IL_008E = 统一出口。
		/// <c>Operators.ConditionalCompareObjectEqual</c> 是 VB 的弱类型比较，不能改写成 == ；
		/// 首次读取失败时 自启动值 保持 Nothing，语义与 VB 局部变量一致。
		/// </remarks>
		private void StopInfo(object sender, EventArgs e)
		{
			object 自启动值 = null;
			try
			{
				自启动值 = RuntimeHelpers.GetObjectValue(ObserverAttr.SortInfo(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
			}
			catch
			{
			}
			if (Operators.ConditionalCompareObjectEqual(自启动值, "", false))
			{
				try
				{
					自启动值 = RuntimeHelpers.GetObjectValue(ObserverAttr.SortInfo(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
				}
				catch
				{
				}
			}
			if (Operators.ConditionalCompareObjectEqual(自启动值, "", false))
			{
				try
				{
					this.Label2.ForeColor = Color.Red;
				}
				catch
				{
				}
				try
				{
					this.Label2.Text = "因权限不够设置不成功，请打开机房管理助手的安装文件夹（C盘j开头的文件夹），右击文件set.exe，选择 以管理员身份运行，再次设置。";
				}
				catch
				{
				}
			}
		}

		// Token: 0x04000065 RID: 101
		/// <summary>设计器容器（就是 components），Dispose 时一并释放。</summary>
		private IContainer _ErrorAttr;

		// Token: 0x04000066 RID: 102
		/// <summary>「确  定」按钮（Button2）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Button2")]
		private Button creatorAttr;

		// Token: 0x04000067 RID: 103
		/// <summary>提示标签（Label2）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label2")]
		private Label m_ImporterAttr;

		// Token: 0x04000068 RID: 104
		/// <summary>说明标签（Label1）的后备字段。</summary>
		[AccessedThroughProperty("Label1")]
		[CompilerGenerated]
		private Label m_EventAttr;
	}
}
