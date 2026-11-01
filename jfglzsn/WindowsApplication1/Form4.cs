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
	// Token: 0x02000011 RID: 17
	/// <summary>
	/// 全屏「锁屏」窗体 —— 惩罚链第三级，也是最强硬的一级（与 Form2 同构，语义更狠）。
	/// <para>
	/// 外观与 Form2 完全一致（无边框 + 最大化 + 置顶 + 不进任务栏 + 系统高亮色背景），
	/// 行为上多两处差别：
	/// <list type="bullet">
	/// <item>口令校验接受<strong>两套</strong>：主口令 <c>HKCU\Software\n</c>，备用口令 <c>HKCU\Software\jfglzs\n2</c>
	/// （后者是作者给老师留的锁屏专用应急口令）；</item>
	/// <item>退出流程会<strong>保留</strong> <c>DisableCMD</c> 的原有状态：原本禁用就临时放开、停完服务再禁回去，
	/// 所以退出后学生依旧没有 cmd。</item>
	/// </list>
	/// </para>
	/// <para>
	/// 锁机手段：1000 ms 定时器每秒重新置顶 + <c>ClipCursor</c> 把鼠标关进左上角 800×100；
	/// 「重启电脑」按钮零延迟强制重启；<c>FormClosing</c> 只在 <see cref="ThreadEvent.mapperEvent"/>
	/// 为真时才拒绝关闭（本工程内没有任何地方把它置真，所以这层锁实际可绕过）。
	/// </para>
	/// <para>
	/// 方法名沿用反编译得到的名字（壳随机化），只有设计器的 <c>InitializeComponent</c> 按约定改名为
	/// <see cref="初始化组件"/>；真实语义写在各自的 &lt;summary&gt; 里。
	/// 控件名 <c>shuoming*</c> 是设计器里的拼音名，Form1 会按名字跨窗体引用，不能改。
	/// 另外本窗体的 <c>Text</c> 被复制成 "Form2"（作者从 Form2 拖过来忘了改），可作同源指纹。
	/// </para>
	/// </summary>
	[DesignerGenerated]
	public class Form4 : Form
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x000084A0 File Offset: 0x000066A0
		/// <summary>
		/// 构造函数：挂 <c>FormClosing</c>（按 <see cref="ThreadEvent.mapperEvent"/> 决定放行/锁死）
		/// 与 <c>Load</c>（开锁定定时器），再做设计器初始化。
		/// </summary>
		public Form4()
		{
			base.FormClosing += this.RevertCallback;
			base.Load += this.GetEvent;
			this.初始化组件();
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000084E0 File Offset: 0x000066E0
		/// <summary>
		/// 释放设计器组件容器（连带释放挂在它上面的 <see cref="Timer_frm4"/>），
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

		// Token: 0x060000B5 RID: 181 RVA: 0x00008520 File Offset: 0x00006720
		/// <summary>
		/// VB 设计器的 <c>InitializeComponent</c>（方法名被 .NET Reactor 洗成 <c>ManageCallback</c>）：
		/// 控件集合与 Form2 完全一致（容器 + 9 个控件 + 1000 ms 定时器），只是字号和坐标不同——
		/// <c>shuoming</c> 26.25pt、<c>shuoming3</c> 9pt、ClientSize 1254×729、定时器叫
		/// <see cref="Timer_frm4"/>，说明这个窗体是从 Form2 复制出来的。
		/// <para>
		/// 窗体属性仍是无边框 + 最大化 + 置顶 + 不进任务栏；<c>Text</c> 沿用了 "Form2"（复制残留）。
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
			this.Timer_frm4 = new global::System.Windows.Forms.Timer(this.组件容器);
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
			this.shuoming.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming.ForeColor = Color.Red;
			this.shuoming.Location = new Point(152, 299);
			this.shuoming.Name = "shuoming";
			this.shuoming.Size = new Size(33, 35);
			this.shuoming.TabIndex = 16;
			this.shuoming.Text = " ";
			this.Timer_frm4.Interval = 1000;
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
			this.Label_pd.Location = new Point(1, 3);
			this.Label_pd.Name = "Label_pd";
			this.Label_pd.Size = new Size(51, 35);
			this.Label_pd.TabIndex = 18;
			this.Label_pd.Text = "  ";
			this.shuoming3.AutoSize = true;
			this.shuoming3.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming3.ForeColor = SystemColors.ActiveCaptionText;
			this.shuoming3.Location = new Point(922, 103);
			this.shuoming3.Name = "shuoming3";
			this.shuoming3.Size = new Size(245, 12);
			this.shuoming3.TabIndex = 19;
			this.shuoming3.Text = "                                        ";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Highlight;
			base.ClientSize = new Size(1254, 729);
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
			base.Name = "Form4";
			base.ShowInTaskbar = false;
			this.Text = "Form2";
			base.TopMost = true;
			base.WindowState = FormWindowState.Maximized;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00002530 File Offset: 0x00000730
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00008B50 File Offset: 0x00006D50
		/// <summary>
		/// 「重启电脑」按钮。setter 把 <c>Click</c> 挂到 <see cref="CompareEvent"/>（先摘后挂，
		/// VB 设计器 <c>WithEvents</c> 的标准产物），点击即零延迟强制重启。
		/// </summary>
		internal virtual Button Button2
		{
			[CompilerGenerated]
			get
			{
				return this._FactorySetter;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.CompareEvent);
				Button button = this._FactorySetter;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this._FactorySetter = value;
				button = this._FactorySetter;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00002538 File Offset: 0x00000738
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00002540 File Offset: 0x00000740
		/// <summary>校验结果提示（深红、10.5pt 宋体）：显示「密码应该是6位以上…」或「密码不正确…」。</summary>
		internal virtual Label Label2 { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00002549 File Offset: 0x00000749
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00002551 File Offset: 0x00000751
		/// <summary>静态提示文字「退出小助手密码：」（白色、10.5pt 宋体）。</summary>
		internal virtual Label Label1 { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000BC RID: 188 RVA: 0x0000255A File Offset: 0x0000075A
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00002562 File Offset: 0x00000762
		/// <summary>口令输入框（<c>PasswordChar = '*'</c>，掩码显示），由 <see cref="InvokeCallback"/> 读取。</summary>
		internal virtual TextBox TextBox1 { get; set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000BE RID: 190 RVA: 0x0000256B File Offset: 0x0000076B
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00008B94 File Offset: 0x00006D94
		/// <summary>
		/// 「确 定」按钮。setter 把 <c>Click</c> 挂到 <see cref="InvokeCallback"/>（双口令校验）。
		/// 校验通过后本按钮会被 <c>Enabled = false</c> 禁用。
		/// </summary>
		internal virtual Button Button1
		{
			[CompilerGenerated]
			get
			{
				return this.m_ErrorSetter;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.InvokeCallback);
				Button button = this.m_ErrorSetter;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this.m_ErrorSetter = value;
				button = this.m_ErrorSetter;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00002573 File Offset: 0x00000773
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x0000257B File Offset: 0x0000077B
		/// <summary>违规原因主文案（26.25pt 红字），由 Form1 按违规类型写入；初值为一个空格。</summary>
		internal virtual Label shuoming { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00002584 File Offset: 0x00000784
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x00008BD8 File Offset: 0x00006DD8
		/// <summary>
		/// 锁定定时器（1000 ms）。setter 把 <c>Tick</c> 挂到 <see cref="DefineCallback"/>：
		/// 每秒重新置顶 + 把鼠标关进 800×100，是本窗体「锁屏」的执行器。
		/// </summary>
		internal virtual global::System.Windows.Forms.Timer Timer_frm4
		{
			[CompilerGenerated]
			get
			{
				return this.propertySetter;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.DefineCallback);
				global::System.Windows.Forms.Timer timer = this.propertySetter;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this.propertySetter = value;
				timer = this.propertySetter;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x0000258C File Offset: 0x0000078C
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x00002594 File Offset: 0x00000794
		/// <summary>处置要求文案（26.25pt 红字），由 Form1 写入，例如「请马上恢复，否则会锁屏！」。</summary>
		internal virtual Label shuoming2 { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x0000259D File Offset: 0x0000079D
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x00008C1C File Offset: 0x00006E1C
		/// <summary>
		/// 左上角的小标签（26.25pt）。setter 把 <c>DoubleClick</c> 挂到 <see cref="PushCallback"/>，
		/// 但那个处理器是空实现——双击没有任何效果。
		/// </summary>
		internal virtual Label Label_pd
		{
			[CompilerGenerated]
			get
			{
				return this.m_IdentifierSetter;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.PushCallback);
				Label label = this.m_IdentifierSetter;
				if (label != null)
				{
					label.DoubleClick -= eventHandler;
				}
				this.m_IdentifierSetter = value;
				label = this.m_IdentifierSetter;
				if (label != null)
				{
					label.DoubleClick += eventHandler;
				}
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x000025A5 File Offset: 0x000007A5
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x000025AD File Offset: 0x000007AD
		/// <summary>
		/// 作者联系方式（9pt）：内容是 <c>"gyhrw4yw4y3tq3yszz" + 进程名 + "mmfnfxhghrts"</c> 的混淆串，
		/// 用来标记是哪个进程被判违规，同时充当行为指纹。
		/// </summary>
		internal virtual Label shuoming3 { get; set; }

		// Token: 0x060000CA RID: 202
		/// <summary>
		/// user32!ClipCursor：把鼠标光标限制在给定矩形内；传覆盖整屏的矩形即解除限制。
		/// <para>本窗体每秒用 0,0,800,100 调它（锁鼠标），退出流程与 <see cref="RevertCallback"/> 用整屏调用它（解锁）。</para>
		/// </summary>
		/// <param name="lpRect">目标矩形（屏幕坐标）。</param>
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern int ClipCursor(Form4.RECT lpRect);

		// Token: 0x060000CB RID: 203
		/// <summary>kernel32!Beep：按指定频率发声，计数达到 5 次后每秒响一次（骚扰用）。</summary>
		/// <param name="频率">频率（Hz），调用点固定 2000。</param>
		/// <param name="时长">时长（ms），调用点固定 500。</param>
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern long Beep(long 频率, long 时长);

		// Token: 0x060000CC RID: 204 RVA: 0x00008C60 File Offset: 0x00006E60
		/// <summary>
		/// 「确 定」按钮（<see cref="Button1"/>.Click）：<strong>双口令</strong>校验。
		/// <para>
		/// 规则：长度必须 ≥ 6，且 <c>SHA256(输入 + "bfdshgs")</c> 要等于下面任意一个——
		/// 主口令摘要 <c>HKCU\Software\n</c>，或备用口令摘要 <c>HKCU\Software\jfglzs\n2</c>。
		/// 备用口令只在本窗体校验（Form2 只认 <c>n</c>），是作者留给老师的「锁屏应急口令」。
		/// </para>
		/// <para>
		/// 注意判断式里用的是按位或 <c>|</c> 而不是 <c>||</c>：两个比较<strong>都会求值</strong>，
		/// 这是 VB <c>Or</c> 的原样产物，不要"顺手"改成短路或。
		/// </para>
		/// <para>
		/// 原版整段 <c>On Error Resume Next</c>；这里与 Form2 的处理一致，把互为因果的几步合成一个
		/// <c>try</c>，任何一步抛异常都等于「校验不通过」。
		/// </para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void InvokeCallback(object sender, EventArgs e)
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
				string 主口令摘要 = Conversions.ToString(ThreadEvent.读注册表值(Registry.CurrentUser, "Software", "n", ""));
				string 备用口令摘要 = Conversions.ToString(ThreadEvent.读注册表值(Registry.CurrentUser, "Software\\jfglzs", "n2", ""));
				if (!((Operators.CompareString(输入摘要, 主口令摘要, false) == 0) | (Operators.CompareString(输入摘要, 备用口令摘要, false) == 0)))
				{
					this.Label2.Text = "密码不正确，请重新输入！";
					this.TextBox1.Text = "";
					return;
				}
				this.Button1.Enabled = false;
				this.SortCallback();
			}
			catch
			{
				// 原版 On Error Resume Next：算摘要 / 读注册表失败时静默丢弃错误，界面停在原样
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00008E20 File Offset: 0x00007020
		/// <summary>
		/// 锁定定时器（<see cref="Timer_frm4"/>，1000 ms）：重新置顶 + 把鼠标关进左上角 800×100，
		/// 计数达到 5 之后每次再蜂鸣（2000 Hz / 500 ms）。与 Form2 的 <c>StopCallback</c> 完全一致。
		/// <para>
		/// 原版逐句 <c>On Error Resume Next</c>，这里按语句拆 <c>try</c>/<c>catch</c>：
		/// 置顶失败不能阻断后面的锁鼠标动作。
		/// </para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void DefineCallback(object sender, EventArgs e)
		{
			try
			{
				base.TopMost = true;
			}
			catch
			{
				// 原版 On Error Resume Next：置顶失败就算了
			}
			Form4.RECT rect;
			rect.Left = 0;
			rect.Top = 0;
			rect.Right = 800;
			rect.Bottom = 100;
			try
			{
				// 鼠标锁定：光标只能停留在屏幕左上角 800×100 的窄条里
				Form4.ClipCursor(rect);
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
					this._ProxySetter++;
				}
			}
			catch
			{
				// 原版 On Error Resume Next
			}
			if (this._ProxySetter >= 5)
			{
				try
				{
					Form4.Beep(2000L, 500L);
				}
				catch
				{
					// 原版 On Error Resume Next
				}
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000079CC File Offset: 0x00005BCC
		/// <summary>
		/// <see cref="Label_pd"/> 的双击处理器 —— <strong>空实现</strong>。
		/// <para>原版方法体除 <c>On Error</c> 脚手架外没有任何语句，双击没有效果；保留只为维持事件绑定。</para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void PushCallback(object sender, EventArgs e)
		{
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00008F40 File Offset: 0x00007140
		/// <summary>
		/// 口令正确后的「退出并复盘」流程。与 Form2 的 <c>PostCallback</c> 基本同构，
		/// 关键差别是<strong>会保留 <c>DisableCMD</c> 的原有状态</strong>。
		/// <para>按原版顺序：</para>
		/// <list type="number">
		/// <item>反复杀与自己同名的进程（名字取 <see cref="ThreadEvent.m_AdapterEvent"/>），直到一个都不剩；</item>
		/// <item>遍历全部进程做「模糊查杀」：名字长度 ≥ 4、且每个字符的 ASCII 都在 102~111（'f'~'o'）就 <c>Kill()</c>；</item>
		/// <item>读 <c>DisableCMD</c>：原本是 <c>1</c>（禁用）就先置 0 临时放开 → <c>net stop zmserv</c> → 等 3 秒
		/// → 再置回 1；否则只做 <c>net stop zmserv</c> → 等 3 秒；</item>
		/// <item>按名字杀 <c>zmserv</c> 进程；</item>
		/// <item>把 <c>ClipCursor</c> 重置为整屏（解除鼠标锁定），最后 <c>ProjectData.EndApp()</c> 结束进程。</item>
		/// </list>
		/// <para>
		/// 也就是说退出后学生依旧没有 CMD——这点比 Form2 更狠（Form2 退出时把 CMD 永久放开）。
		/// </para>
		/// <para>
		/// 原版整段 <c>On Error Resume Next</c>，这里按「一步一个 try」还原；第 1 步依赖「下标越界异常
		/// 跳出循环」是原版刻意行为。原版方法体整个处在 <c>checked</c> 上下文，此处唯一算术是
		/// <c>10 * 10 + 2</c>，不可能溢出，故没有再套 <c>checked</c> 块。
		/// </para>
		/// </summary>
		private void SortCallback()
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

			// 3) 读 DisableCMD 原值；读不到 / 不是数字时按 0 处理（转换失败会被原版 On Error 吞掉）
			int 命令提示符状态;
			try
			{
				命令提示符状态 = Conversions.ToInteger(ThreadEvent.读注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", ""));
			}
			catch
			{
				// 原版 On Error Resume Next：变量保持默认值 0
				命令提示符状态 = 0;
			}
			if (命令提示符状态 == 1)
			{
				// 原本禁用：临时放开 → 停服务 → 等 3 秒 → 再禁回去（维持管控）
				try
				{
					ThreadEvent.写注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				}
				catch
				{
					// 原版 On Error Resume Next
				}
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
				try
				{
					ThreadEvent.写注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 1, RegistryValueKind.DWord);
				}
				catch
				{
					// 原版 On Error Resume Next
				}
			}
			else
			{
				// 原本就没禁用（或读不到）：只停服务、等 3 秒，不动注册表
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
			}

			// 4) 按名字杀 zmserv 进程
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
			Form4.RECT 整屏;
			整屏.Left = 0;
			整屏.Top = 0;
			整屏.Right = Screen.PrimaryScreen.Bounds.Width;
			整屏.Bottom = Screen.PrimaryScreen.Bounds.Height;
			try
			{
				Form4.ClipCursor(整屏);
			}
			catch
			{
				// 原版 On Error Resume Next
			}
			ProjectData.EndApp();   // 直接结束进程，不返回调用方
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000092A0 File Offset: 0x000074A0
		/// <summary>
		/// <c>FormClosing</c> 处理器：按 <see cref="ThreadEvent.mapperEvent"/> 决定放行还是锁死。
		/// <list type="bullet">
		/// <item><c>mapperEvent</c> 为假（默认）：允许关闭 <c>e.Cancel = false</c>，并顺手把 <c>ClipCursor</c>
		/// 重置为整屏——避免窗口没了、鼠标还被关在 800×100 的小矩形里；</item>
		/// <item>为真：<c>e.Cancel = true</c>，锁死窗口。</item>
		/// </list>
		/// <para>
		/// 注意：本工程内<strong>没有任何代码把 <c>mapperEvent</c> 置真</strong>，所以这个锁屏窗实际总能被关掉
		/// （要么是去壳时丢了一环，要么原版靠别的手段赋值）。
		/// </para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">关闭事件参数。</param>
		private void RevertCallback(object sender, FormClosingEventArgs e)
		{
			if (!ThreadEvent.mapperEvent)
			{
				e.Cancel = false;
				Form4.RECT rect;
				rect.Left = 0;
				rect.Top = 0;
				rect.Right = Screen.PrimaryScreen.Bounds.Width;
				rect.Bottom = Screen.PrimaryScreen.Bounds.Height;
				Form4.ClipCursor(rect);
				return;
			}
			e.Cancel = true;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000930C File Offset: 0x0000750C
		/// <summary>
		/// <c>Load</c> 处理器：打开锁定定时器 <see cref="Timer_frm4"/>（Interval 在
		/// <see cref="初始化组件"/> 里设为 1000 ms），之后由 <see cref="DefineCallback"/> 每秒锁一次鼠标。
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void GetEvent(object sender, EventArgs e)
		{
			try
			{
				this.Timer_frm4.Enabled = true;
			}
			catch
			{
				// 原版 On Error Resume Next：定时器启动失败也照常显示窗口
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007DB8 File Offset: 0x00005FB8
		/// <summary>
		/// 「重启电脑」按钮（<see cref="Button2"/>.Click）：执行 <c>shutdown /r /f /t 0</c>。
		/// <para>
		/// <c>/r</c> 重启、<c>/f</c> 强制结束应用、<c>/t 0</c> 零延迟；第三个参数 <c>false</c> = 不等待。
		/// <strong>不给用户任何保存或取消的机会</strong>，属于破坏性操作。
		/// </para>
		/// </summary>
		/// <param name="sender">事件源（未使用）。</param>
		/// <param name="e">事件参数（未使用）。</param>
		private void CompareEvent(object sender, EventArgs e)
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

		// Token: 0x040000A2 RID: 162
		/// <summary>设计器组件容器：承载 <see cref="Timer_frm4"/>，<see cref="Dispose(bool)"/> 时一并释放。</summary>
		private IContainer 组件容器;

		// Token: 0x040000A3 RID: 163
		/// <summary><see cref="Button2"/>（重启电脑）的后备字段，属性 setter 在这里挂/摘 <c>Click</c>。</summary>
		[AccessedThroughProperty("Button2")]
		[CompilerGenerated]
		private Button _FactorySetter;

		// Token: 0x040000A4 RID: 164
		/// <summary><see cref="Label2"/>（校验提示）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label2")]
		private Label helperSetter;

		// Token: 0x040000A5 RID: 165
		/// <summary><see cref="Label1"/>（「退出小助手密码：」）的后备字段。</summary>
		[AccessedThroughProperty("Label1")]
		[CompilerGenerated]
		private Label m_MockSetter;

		// Token: 0x040000A6 RID: 166
		/// <summary><see cref="TextBox1"/>（口令输入框）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("TextBox1")]
		private TextBox _ComparatorSetter;

		// Token: 0x040000A7 RID: 167
		/// <summary><see cref="Button1"/>（确定）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Button1")]
		private Button m_ErrorSetter;

		// Token: 0x040000A8 RID: 168
		/// <summary><see cref="shuoming"/> 的后备字段。</summary>
		[AccessedThroughProperty("shuoming")]
		[CompilerGenerated]
		private Label m_ProductSetter;

		// Token: 0x040000A9 RID: 169
		/// <summary><see cref="Timer_frm4"/>（锁定定时器）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Timer_frm4")]
		private global::System.Windows.Forms.Timer propertySetter;

		// Token: 0x040000AA RID: 170
		/// <summary><see cref="shuoming2"/> 的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("shuoming2")]
		private Label _ExceptionSetter;

		// Token: 0x040000AB RID: 171
		/// <summary><see cref="Label_pd"/> 的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label_pd")]
		private Label m_IdentifierSetter;

		// Token: 0x040000AC RID: 172
		/// <summary><see cref="shuoming3"/> 的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("shuoming3")]
		private Label _AnnotationSetter;

		// Token: 0x040000AD RID: 173
		/// <summary>锁定计时：定时器每跳一次 +1（checked 上下文），达到 5 之后每跳一次蜂鸣。</summary>
		private int _ProxySetter;

		// Token: 0x02000012 RID: 18
		/// <summary>Win32 <c>RECT</c>：<see cref="ClipCursor"/> 的目标矩形，单位像素、屏幕坐标。</summary>
		public struct RECT
		{
			// Token: 0x040000AE RID: 174
			/// <summary>左边界。</summary>
			public int Left;

			// Token: 0x040000AF RID: 175
			/// <summary>上边界。</summary>
			public int Top;

			// Token: 0x040000B0 RID: 176
			/// <summary>右边界。</summary>
			public int Right;

			// Token: 0x040000B1 RID: 177
			/// <summary>下边界。</summary>
			public int Bottom;
		}
	}
}
