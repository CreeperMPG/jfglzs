using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using jfglzsn.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace WindowsApplication1
{
	// Token: 0x0200000D RID: 13
	/// <summary>
	/// 全屏「违规警告 + 退出密码」惩罚窗体（惩罚链第二级）。
	/// <para>
	/// 外观：无边框 + 最大化 + 置顶 + 不进任务栏 + 系统高亮色背景，红色大字写违规原因。
	/// <see cref="TestCallback"/> 无条件取消 <c>FormClosing</c>，所以<strong>学生关不掉它</strong>。
	/// </para>
	/// <para>
	/// 三个动作构成「惩罚」：
	/// <list type="bullet">
	/// <item>定时器每 1000 ms 由 <see cref="StopCallback"/> 重新置顶，并用 <c>ClipCursor</c> 把鼠标关进 800×100 的窄条；</item>
	/// <item>「重启电脑」按钮直接执行 <c>shutdown /r /f /t 0</c>——零延迟强制重启，不给保存机会；</item>
	/// <item>只有输入正确口令（<c>SHA256(口令 + "bfdshgs")</c> 命中注册表 <c>HKCU\Software\n</c>）才走
	/// <see cref="PostCallback"/>：杀进程、恢复 CMD、停服务、解除鼠标锁定、结束进程。</item>
	/// </list>
	/// </para>
	/// <para>
	/// 方法名沿用反编译得到的名字（壳随机化），只有设计器的 <c>InitializeComponent</c> 按约定改名为
	/// <see cref="初始化组件"/>；真实语义写在各自的 &lt;summary&gt; 里。
	/// 控件名 <c>shuoming*</c> 是设计器里的拼音名，Form1 会按名字跨窗体引用，不能改。
	/// </para>
	/// </summary>
	[DesignerGenerated]
	public class Form2 : Form
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00006F5C File Offset: 0x0000515C
		/// <summary>
		/// 构造函数：挂 <c>FormClosing</c>（锁死关闭）与 <c>Load</c>（开锁定定时器），再做设计器初始化。
		/// <para>注意挂事件的顺序在 <see cref="初始化组件"/> 之前，原版如此，不影响结果。</para>
		/// </summary>
		public Form2()
		{
			base.FormClosing += this.TestCallback;
			base.Load += this.InitCallback;
			this.初始化组件();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00006F9C File Offset: 0x0000519C
		/// <summary>
		/// 释放设计器组件容器（连带释放挂在它上面的 <see cref="Timer_frm2"/>），
		/// 随后在 <c>finally</c> 里调用基类释放——<c>try</c>/<c>finally</c> 保证基类释放一定执行。
		/// </summary>
		/// <param name="disposing">为 <c>true</c> 时同时释放托管资源。</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.组件容器 != null)
				{
					this.组件容器.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00006FDC File Offset: 0x000051DC
		/// <summary>
		/// VB 设计器的 <c>InitializeComponent</c>（方法名被 .NET Reactor 洗成 <c>DeleteCallback</c>）：
		/// 建 1 个容器 + 9 个控件 + 1 个 1000 ms 定时器，并设置窗体的「惩罚界面」外观。
		/// <para>
		/// 外观关键词：<c>FormBorderStyle.None</c> + <c>WindowState.Maximized</c> + <c>TopMost</c> +
		/// <c>ShowInTaskbar = false</c>——盖住整块屏幕、无法拖动、任务栏看不到入口。
		/// 定时器 <see cref="Timer_frm2"/> 的 Interval 在这里设成 1000 ms。
		/// </para>
		/// </summary>
		private void 初始化组件()
		{
			this.组件容器 = new Container();
			this.Button2 = new Button();
			this.Label2 = new Label();
			this.Label1 = new Label();
			this.TextBox1 = new TextBox();
			this.Button1 = new Button();
			this.shuoming = new Label();
			this.Timer_frm2 = new global::System.Windows.Forms.Timer(this.组件容器);
			this.shuoming2 = new Label();
			this.Label_pd = new Label();
			this.shuoming3 = new Label();
			base.SuspendLayout();
			this.Button2.Location = new Point(389, 17);
			this.Button2.Name = "Button2";
			this.Button2.Size = new Size(82, 28);
			this.Button2.TabIndex = 15;
			this.Button2.Text = "重启电脑";
			this.Button2.UseVisualStyleBackColor = true;
			this.Label2.AutoSize = true;
			this.Label2.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label2.ForeColor = Color.DarkRed;
			this.Label2.Location = new Point(34, 65);
			this.Label2.Name = "Label2";
			this.Label2.Size = new Size(0, 14);
			this.Label2.TabIndex = 14;
			this.Label1.AutoSize = true;
			this.Label1.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label1.ForeColor = Color.White;
			this.Label1.Location = new Point(61, 23);
			this.Label1.Name = "Label1";
			this.Label1.Size = new Size(119, 14);
			this.Label1.TabIndex = 13;
			this.Label1.Text = "退出小助手密码：";
			this.TextBox1.Location = new Point(196, 19);
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.PasswordChar = '*';
			this.TextBox1.Size = new Size(104, 21);
			this.TextBox1.TabIndex = 12;
			this.Button1.Location = new Point(311, 16);
			this.Button1.Name = "Button1";
			this.Button1.Size = new Size(54, 27);
			this.Button1.TabIndex = 11;
			this.Button1.Text = "确 定";
			this.Button1.UseVisualStyleBackColor = true;
			this.shuoming.AutoSize = true;
			this.shuoming.Font = new Font("宋体", 18f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming.ForeColor = Color.Red;
			this.shuoming.Location = new Point(152, 299);
			this.shuoming.Name = "shuoming";
			this.shuoming.Size = new Size(778, 24);
			this.shuoming.TabIndex = 16;
			this.shuoming.Text = "你运行的软件危害电子教室运行！如有误判，请老师拍照与作者QQ联系。";
			this.Timer_frm2.Interval = 1000;
			this.shuoming2.AutoSize = true;
			this.shuoming2.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming2.ForeColor = Color.Red;
			this.shuoming2.Location = new Point(152, 372);
			this.shuoming2.Name = "shuoming2";
			this.shuoming2.Size = new Size(33, 35);
			this.shuoming2.TabIndex = 17;
			this.shuoming2.Text = " ";
			this.Label_pd.AutoSize = true;
			this.Label_pd.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label_pd.Location = new Point(1, 2);
			this.Label_pd.Name = "Label_pd";
			this.Label_pd.Size = new Size(51, 35);
			this.Label_pd.TabIndex = 18;
			this.Label_pd.Text = "  ";
			this.shuoming3.AutoSize = true;
			this.shuoming3.Font = new Font("宋体", 14.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming3.ForeColor = SystemColors.ActiveCaptionText;
			this.shuoming3.Location = new Point(669, 125);
			this.shuoming3.Name = "shuoming3";
			this.shuoming3.Size = new Size(409, 19);
			this.shuoming3.TabIndex = 19;
			this.shuoming3.Text = "                                        ";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Highlight;
			base.ClientSize = new Size(1518, 1092);
			base.Controls.Add(this.shuoming3);
			base.Controls.Add(this.Label_pd);
			base.Controls.Add(this.shuoming2);
			base.Controls.Add(this.shuoming);
			base.Controls.Add(this.Button2);
			base.Controls.Add(this.Label2);
			base.Controls.Add(this.Label1);
			base.Controls.Add(this.TextBox1);
			base.Controls.Add(this.Button1);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "Form2";
			base.ShowInTaskbar = false;
			this.Text = "Form2";
			base.TopMost = true;
			base.WindowState = FormWindowState.Maximized;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00002460 File Offset: 0x00000660
		// (set) Token: 0x0600007F RID: 127 RVA: 0x00007610 File Offset: 0x00005810
		/// <summary>
		/// 「重启电脑」按钮。setter 在这里把 <c>Click</c> 挂到 <see cref="CountCallback"/>（先摘后挂，
		/// 这是 VB 设计器 <c>WithEvents</c> 的标准产物），点击即零延迟强制重启。
		/// </summary>
		internal virtual Button Button2
		{
			[CompilerGenerated]
			get
			{
				return this.m_StubEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.CountCallback);
				Button button = this.m_StubEvent;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this.m_StubEvent = value;
				button = this.m_StubEvent;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00002468 File Offset: 0x00000668
		// (set) Token: 0x06000081 RID: 129 RVA: 0x00002470 File Offset: 0x00000670
		/// <summary>校验结果提示（深红、10.5pt 宋体）：显示「密码应该是6位以上…」或「密码不正确…」。</summary>
		internal virtual Label Label2 { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00002479 File Offset: 0x00000679
		// (set) Token: 0x06000083 RID: 131 RVA: 0x00002481 File Offset: 0x00000681
		/// <summary>静态提示文字「退出小助手密码：」（白色、10.5pt 宋体）。</summary>
		internal virtual Label Label1 { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000084 RID: 132 RVA: 0x0000248A File Offset: 0x0000068A
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00002492 File Offset: 0x00000692
		/// <summary>口令输入框（<c>PasswordChar = '*'</c>，掩码显示），由 <see cref="FindCallback"/> 读取。</summary>
		internal virtual TextBox TextBox1 { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000086 RID: 134 RVA: 0x0000249B File Offset: 0x0000069B
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00007654 File Offset: 0x00005854
		/// <summary>
		/// 「确 定」按钮。setter 把 <c>Click</c> 挂到 <see cref="FindCallback"/>（口令校验）。
		/// 校验通过后本按钮会被 <c>Enabled = false</c> 禁用，防止重复触发退出流程。
		/// </summary>
		internal virtual Button Button1
		{
			[CompilerGenerated]
			get
			{
				return this.m_QueueEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.FindCallback);
				Button button = this.m_QueueEvent;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this.m_QueueEvent = value;
				button = this.m_QueueEvent;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000088 RID: 136 RVA: 0x000024A3 File Offset: 0x000006A3
		// (set) Token: 0x06000089 RID: 137 RVA: 0x000024AB File Offset: 0x000006AB
		/// <summary>违规原因主文案（18pt 红字），默认是「你运行的软件危害电子教室运行！…」，由 Form1 改写。</summary>
		internal virtual Label shuoming { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600008A RID: 138 RVA: 0x000024B4 File Offset: 0x000006B4
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00007698 File Offset: 0x00005898
		/// <summary>
		/// 锁定定时器（1000 ms）。setter 把 <c>Tick</c> 挂到 <see cref="StopCallback"/>：
		/// 每秒重新置顶 + 把鼠标关进 800×100，是本窗体「锁机」的执行器。
		/// </summary>
		internal virtual global::System.Windows.Forms.Timer Timer_frm2
		{
			[CompilerGenerated]
			get
			{
				return this.readerEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.StopCallback);
				global::System.Windows.Forms.Timer timer = this.readerEvent;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this.readerEvent = value;
				timer = this.readerEvent;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600008C RID: 140 RVA: 0x000024BC File Offset: 0x000006BC
		// (set) Token: 0x0600008D RID: 141 RVA: 0x000024C4 File Offset: 0x000006C4
		/// <summary>处置要求文案（26.25pt 红字），由 Form1 写入，例如「请马上恢复网络，否则会全屏锁定！」。</summary>
		internal virtual Label shuoming2 { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600008E RID: 142 RVA: 0x000024CD File Offset: 0x000006CD
		// (set) Token: 0x0600008F RID: 143 RVA: 0x000076DC File Offset: 0x000058DC
		/// <summary>
		/// 左上角的小标签（26.25pt）。setter 把 <c>DoubleClick</c> 挂到 <see cref="InsertCallback"/>，
		/// 但那个处理器是空实现——双击没有任何效果。
		/// </summary>
		internal virtual Label Label_pd
		{
			[CompilerGenerated]
			get
			{
				return this.listenerEvent;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.InsertCallback);
				Label label = this.listenerEvent;
				if (label != null)
				{
					label.DoubleClick -= eventHandler;
				}
				this.listenerEvent = value;
				label = this.listenerEvent;
				if (label != null)
				{
					label.DoubleClick += eventHandler;
				}
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000090 RID: 144 RVA: 0x000024D5 File Offset: 0x000006D5
		// (set) Token: 0x06000091 RID: 145 RVA: 0x000024DD File Offset: 0x000006DD
		/// <summary>
		/// 作者联系方式（14.25pt）：内容是 <c>"gyhrw4yw4y3tq3yszz" + 进程名 + "mmfnfxhghrts"</c> 的混淆串，
		/// 用来标记是哪个进程被判违规，同时充当行为指纹。
		/// </summary>
		internal virtual Label shuoming3 { get; set; }

		// Token: 0x06000092 RID: 146
		/// <summary>
		/// user32!ClipCursor：把鼠标光标限制在给定矩形内；传一个覆盖整屏的矩形即解除限制。
		/// <para>本窗体每秒用 0,0,800,100 调它（锁鼠标），退出流程里用整屏调用它（解锁）。</para>
		/// </summary>
		/// <param name="lpRect">目标矩形（屏幕坐标）。</param>
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern int ClipCursor(Form2.RECT lpRect);

		// Token: 0x06000093 RID: 147
		/// <summary>kernel32!Beep：按指定频率发声，计数达到 5 次后每秒响一次（骚扰用）。</summary>
		/// <param name="频率">频率（Hz），调用点固定 2000。</param>
		/// <param name="时长">时长（ms），调用点固定 500。</param>
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern long Beep(long 频率, long 时长);

		// Token: 0x06000094 RID: 148 RVA: 0x00007720 File Offset: 0x00005920
		/// <summary>
		/// 「确 定」按钮（<see cref="Button1"/>.Click）：校验退出小助手用的口令。
		/// <para>
		/// 规则：长度必须 ≥ 6，且 <c>SHA256(输入 + "bfdshgs")</c> 的十六进制摘要要等于
		/// 注册表 <c>HKCU\Software</c> 值名 <c>n</c> 里存的那一串（固定盐、单轮 SHA256，
		/// 意味着口令可离线爆破——这是本样本最关键的管控缺陷）。
		/// </para>
		/// <para>
		/// 通过 → 禁用按钮并进入 <see cref="PostCallback"/> 的恢复流程；不通过 → 只改提示文字并清空输入框。
		/// </para>
		/// <para>
		/// 原版整段在 <c>On Error Resume Next</c> 下，逐条语句各自吞异常。这里把「取输入 → 算摘要 →
		/// 读期望值 → 比较 → 分支」合成一个 <c>try</c>：这几步互为因果，任何一步抛异常都等于「校验不通过」，
		/// 与逐句忽略错误的最终结果相同（差别只在极端情况下不显示提示文字）。
		/// </para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void FindCallback(object sender, EventArgs e)
		{
			try
			{
				if (Strings.Len(this.TextBox1.Text) < 6)
				{
					this.Label2.Text = "密码应该是6位以上的字母或数字，请重新输入！";
					this.TextBox1.Text = "";
					return;
				}
				string 输入摘要 = ThreadEvent.计算SHA256摘要(Strings.Trim(this.TextBox1.Text));
				string 期望摘要 = Conversions.ToString(ThreadEvent.读注册表值(Registry.CurrentUser, "Software", "n", ""));
				if (Operators.CompareString(输入摘要, 期望摘要, false) != 0)
				{
					this.Label2.Text = "密码不正确，请重新输入！";
					this.TextBox1.Text = "";
					return;
				}
				this.Button1.Enabled = false;
				this.PostCallback();
			}
			catch
			{
				// 原版 On Error Resume Next：算摘要 / 读注册表失败时静默丢弃错误，界面停在原样
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000078A8 File Offset: 0x00005AA8
		/// <summary>
		/// 锁定定时器（<see cref="Timer_frm2"/>，1000 ms）：重新置顶 + 把鼠标关进左上角 800×100，
		/// 计数达到 5 之后每次再蜂鸣（2000 Hz / 500 ms）。
		/// <para>
		/// 这是「锁机」的执行部分：窗口本身已经 <c>TopMost</c>，这里每秒再重申一次；
		/// <c>ClipCursor</c> 把光标限制在窄条里，学生基本没法操作桌面。
		/// </para>
		/// <para>
		/// 原版逐句 <c>On Error Resume Next</c>，这里按语句拆 <c>try</c>/<c>catch</c>：
		/// 某一步失败（例如系统拒绝了置顶请求）不能阻断后面的锁鼠标动作。
		/// </para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void StopCallback(object sender, EventArgs e)
		{
			try
			{
				base.TopMost = true;
			}
			catch
			{
				// 原版 On Error Resume Next：置顶失败就算了
			}
			Form2.RECT rect;
			rect.Left = 0;
			rect.Top = 0;
			rect.Right = 800;
			rect.Bottom = 100;
			try
			{
				// 鼠标锁定：光标只能停留在屏幕左上角 800×100 的窄条里
				Form2.ClipCursor(rect);
			}
			catch
			{
				// 原版 On Error Resume Next
			}
			try
			{
				// VB 默认 checked（溢出检查），原版的 ++ 同样处在 checked 上下文
				checked
				{
					this.m_MerchantEvent++;
				}
			}
			catch
			{
				// 原版 On Error Resume Next
			}
			if (this.m_MerchantEvent >= 5)
			{
				try
				{
					Form2.Beep(2000L, 500L);
				}
				catch
				{
					// 原版 On Error Resume Next
				}
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000079CC File Offset: 0x00005BCC
		/// <summary>
		/// <see cref="Label_pd"/> 的双击处理器 —— <strong>空实现</strong>。
		/// <para>
		/// 原版方法体除了 <c>On Error</c> 脚手架外没有任何语句（第一条指令就跳到方法末尾），
		/// 双击没有任何效果。保留空方法只为维持事件绑定。
		/// </para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void InsertCallback(object sender, EventArgs e)
		{
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00007A50 File Offset: 0x00005C50
		/// <summary>
		/// 口令正确后的「退出并复盘」流程 —— 本窗体的收尾动作，也是破坏性最强的一段。
		/// <para>按原版顺序：</para>
		/// <list type="number">
		/// <item>反复杀与自己同名的进程（名字取 <see cref="ThreadEvent.m_AdapterEvent"/>），直到一个都不剩；</item>
		/// <item>遍历全部进程做「模糊查杀」：名字长度 ≥ 4、且每个字符的 ASCII 都落在 102~111（'f'~'o'）
		/// 的进程一律 <c>Kill()</c>；</item>
		/// <item>写 <c>HKCU\Software\Policies\Microsoft\Windows\System</c> 的 <c>DisableCMD = 0</c>，放开命令提示符；</item>
		/// <item><c>net stop zmserv</c> 停掉自家服务（隐藏窗口 + 等待），睡 3 秒后按名字杀 <c>zmserv</c> 进程；</item>
		/// <item>把 <c>ClipCursor</c> 重置为整屏（解除鼠标锁定），最后 <c>ProjectData.EndApp()</c> 直接结束进程。</item>
		/// </list>
		/// <para>
		/// 原版整段在 <c>On Error Resume Next</c> 下：任何一步失败都继续下一步，最后照样 <c>EndApp</c>，
		/// 因此这里按「一步一个 try」还原。第 1 步依赖「下标越界异常跳出循环」是原版刻意行为，见行内注释。
		/// 另：原版整个方法体处在 <c>checked</c> 上下文（VB 默认溢出检查），此处唯一的算术是
		/// <c>10 * 10 + 2</c>，不可能溢出，故没有再套一层 <c>checked</c> 块。
		/// </para>
		/// </summary>
		private void PostCallback()
		{
			string 服务名 = "zmserv";
			string 当前进程名 = Conversions.ToString(ThreadEvent.m_AdapterEvent);

			// 1) 点名杀自己：每轮取一遍同名进程、杀 [0]；进程被杀光后 [0] 越界抛异常，
			//    原版靠 On Error 吞掉它，随后 while 读到 Length == 0 自然退出。
			Process[] 同名进程列表 = null;
			do
			{
				try
				{
					同名进程列表 = Process.GetProcessesByName(当前进程名);
				}
				catch
				{
					// 原版 On Error Resume Next
				}
				try
				{
					同名进程列表[0].Kill();
				}
				catch
				{
					// 已经没有同名进程：越界异常被吞，控制流落到 while 判定
				}
			}
			while (同名进程列表.Length != 0);

			// 2) 模糊查杀：字符下界先写 10，再 *10+2 得到 102（'f'），上界为下界 + 9 = 111（'o'）
			int 字符下界 = 10;
			字符下界 = 字符下界 * 10 + 2;
			try
			{
				foreach (Process 进程 in Process.GetProcesses())
				{
					int 名称长度;
					try
					{
						名称长度 = Strings.Len(进程.ProcessName);
					}
					catch
					{
						// 进程在枚举期间退出：原版让长度保持 0，等于跳过本进程
						continue;
					}
					if (名称长度 >= 4)
					{
						int 位置 = 1;
						int 循环上限 = 名称长度;
						while (位置 <= 循环上限)
						{
							int 字符编码 = Strings.Asc(Strings.Mid(进程.ProcessName, 位置, 1));
							if (字符编码 >= 字符下界)
							{
								if (字符编码 <= 字符下界 + 9)
								{
									位置++;
									continue;
								}
							}
							break;
						}
						// 原版循环体里还有一条 `if (位置 == 长度 + 1) { Kill(); }`：循环内位置最大只到长度，
						// 该判定恒假（死代码）；只有循环正常走完、执行到同一段 IL 时才成立。
						// 所以这里把「全部字符都在 'f'~'o' 区间→杀」统一放到循环外面判定。
						if (位置 == 名称长度 + 1)
						{
							try
							{
								进程.Kill();
							}
							catch
							{
								// 权限不足 / 进程已退出：原版吞掉后继续扫下一个
							}
						}
					}
				}
			}
			catch
			{
				// 原版逐条忽略错误；整轮枚举失败时直接进入下一步
			}

			// 3) 放开命令提示符：DisableCMD 置 0（这是"退出小助手"的副作用，不是善意）
			try
			{
				ThreadEvent.写注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
			}
			catch
			{
				// 原版 On Error Resume Next
			}

			// 4) 停自家服务：隐藏窗口执行并等待，睡 3 秒后按名字杀 zmserv
			try
			{
				Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
			}
			catch
			{
				// 原版 On Error Resume Next
			}
			try
			{
				Thread.Sleep(3000);
			}
			catch
			{
				// 原版 On Error Resume Next
			}
			Process[] 服务进程列表 = null;
			try
			{
				服务进程列表 = Process.GetProcessesByName(服务名);
			}
			catch
			{
				// 原版 On Error Resume Next
			}
			try
			{
				服务进程列表[0].Kill();
			}
			catch
			{
				// 服务已经停了 / 杀不掉：原版吞掉
			}

			// 5) 解除鼠标锁定（重置为整屏），随后直接结束进程
			Form2.RECT 整屏;
			整屏.Left = 0;
			整屏.Top = 0;
			整屏.Right = Screen.PrimaryScreen.Bounds.Width;
			整屏.Bottom = Screen.PrimaryScreen.Bounds.Height;
			try
			{
				Form2.ClipCursor(整屏);
			}
			catch
			{
				// 原版 On Error Resume Next
			}
			ProjectData.EndApp();   // 直接结束进程，不返回调用方
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000024E6 File Offset: 0x000006E6
		/// <summary>
		/// <c>FormClosing</c> 处理器：<strong>无条件</strong> <c>e.Cancel = true</c>。
		/// <para>
		/// 关闭按钮、Alt+F4、任务管理器「结束任务」之外的常规关闭路径一律被拒——学生只能输入口令，
		/// 由 <see cref="PostCallback"/> 走到 <c>ProjectData.EndApp()</c> 才真正结束。
		/// </para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">关闭事件参数，<c>Cancel</c> 被强制置真。</param>
		private void TestCallback(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00007D20 File Offset: 0x00005F20
		/// <summary>
		/// <c>Load</c> 处理器：打开锁定定时器 <see cref="Timer_frm2"/>（Interval 在
		/// <see cref="初始化组件"/> 里设为 1000 ms），之后由 <see cref="StopCallback"/> 每秒锁一次鼠标。
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void InitCallback(object sender, EventArgs e)
		{
			try
			{
				this.Timer_frm2.Enabled = true;
			}
			catch
			{
				// 原版 On Error Resume Next：定时器启动失败也照常显示窗口
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00007DB8 File Offset: 0x00005FB8
		/// <summary>
		/// 「重启电脑」按钮（<see cref="Button2"/>.Click）：执行 <c>shutdown /r /f /t 0</c>。
		/// <para>
		/// <c>/r</c> 重启、<c>/f</c> 强制结束应用、<c>/t 0</c> 零延迟；第三个参数 <c>false</c> = 不等待，
		/// 命令发出即返回。<strong>不给用户任何保存或取消的机会</strong>，属于破坏性操作。
		/// </para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void CountCallback(object sender, EventArgs e)
		{
			try
			{
				Interaction.Shell("shutdown /r /f /t 0", AppWinStyle.MinimizedFocus, false, -1);
			}
			catch
			{
				// 原版 On Error Resume Next
			}
		}

		// Token: 0x04000085 RID: 133
		/// <summary>设计器组件容器：承载 <see cref="Timer_frm2"/>，<see cref="Dispose(bool)"/> 时一并释放。</summary>
		private IContainer 组件容器;

		// Token: 0x04000086 RID: 134
		/// <summary><see cref="Button2"/>（重启电脑）的后备字段，属性 setter 在这里挂/摘 <c>Click</c>。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Button2")]
		private Button m_StubEvent;

		// Token: 0x04000087 RID: 135
		/// <summary><see cref="Label2"/>（校验提示）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label2")]
		private Label m_StatusEvent;

		// Token: 0x04000088 RID: 136
		/// <summary><see cref="Label1"/>（「退出小助手密码：」）的后备字段。</summary>
		[AccessedThroughProperty("Label1")]
		[CompilerGenerated]
		private Label _ProducerEvent;

		// Token: 0x04000089 RID: 137
		/// <summary><see cref="TextBox1"/>（口令输入框）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("TextBox1")]
		private TextBox _SystemEvent;

		// Token: 0x0400008A RID: 138
		/// <summary><see cref="Button1"/>（确定）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Button1")]
		private Button m_QueueEvent;

		// Token: 0x0400008B RID: 139
		/// <summary><see cref="shuoming"/> 的后备字段。</summary>
		[AccessedThroughProperty("shuoming")]
		[CompilerGenerated]
		private Label containerEvent;

		// Token: 0x0400008C RID: 140
		/// <summary><see cref="Timer_frm2"/>（锁定定时器）的后备字段。</summary>
		[AccessedThroughProperty("Timer_frm2")]
		[CompilerGenerated]
		private global::System.Windows.Forms.Timer readerEvent;

		// Token: 0x0400008D RID: 141
		/// <summary><see cref="shuoming2"/> 的后备字段。</summary>
		[AccessedThroughProperty("shuoming2")]
		[CompilerGenerated]
		private Label m_DescriptorEvent;

		// Token: 0x0400008E RID: 142
		/// <summary><see cref="Label_pd"/> 的后备字段。</summary>
		[AccessedThroughProperty("Label_pd")]
		[CompilerGenerated]
		private Label listenerEvent;

		// Token: 0x0400008F RID: 143
		/// <summary><see cref="shuoming3"/> 的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("shuoming3")]
		private Label _PublisherEvent;

		// Token: 0x04000090 RID: 144
		/// <summary>锁定计时：定时器每跳一次 +1（checked 上下文），达到 5 之后每跳一次蜂鸣。</summary>
		private int m_MerchantEvent;

		// Token: 0x0200000E RID: 14
		/// <summary>Win32 <c>RECT</c>：<see cref="ClipCursor"/> 的目标矩形，单位像素、屏幕坐标。</summary>
		public struct RECT
		{
			// Token: 0x04000091 RID: 145
			/// <summary>左边界。</summary>
			public int Left;

			// Token: 0x04000092 RID: 146
			/// <summary>上边界。</summary>
			public int Top;

			// Token: 0x04000093 RID: 147
			/// <summary>右边界。</summary>
			public int Right;

			// Token: 0x04000094 RID: 148
			/// <summary>下边界。</summary>
			public int Bottom;
		}
	}
}
