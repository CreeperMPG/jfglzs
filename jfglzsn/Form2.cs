using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using WindowsApplication1.Internal;

namespace WindowsApplication1
{
	// Token: 0x0200000A RID: 10
	[DesignerGenerated]
	// ============================================================
	// Form2.cs — 全屏锁定密码窗（一级锁屏，解锁即卸载清理）
	// 全屏无边框、置顶（TopMost）；输入正确密码（HKCU\Software\n 的 SHA256 哈希比对）后执行 UninstallAndExit 卸载流程：击杀 zmserv 与随机名同伴进程、枚举击杀"f~o 字符集"命名进程、停服务、释放鼠标、EndApp。FormClosing 永远 e.Cancel=true 禁止关闭；Timer_frm2 每秒 ClipCursor 锁鼠标（顶部 100px 区域）并周期性 Beep 报警。
	// ============================================================
	public partial class Form2 : Form
	{
		// Token: 0x06000070 RID: 112 RVA: 0x00007360 File Offset: 0x00005560
		/// <summary>
		/// 构造器：挂接 FormClosing(Form2_FormClosing) 与 Load(Form2_Load) 事件；调用 InitControlsAndLayout() 初始化控件。
		/// </summary>
		public Form2()
		{
			base.FormClosing += this.Form2_FormClosing;
			base.Load += this.Form2_Load;
			this.InitControlsAndLayout();
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000073E0 File Offset: 0x000055E0
		/// <summary>
		/// 初始化控件与布局（原名 QueryTemplate，Designer 代码被搬进逻辑文件）：创建 Button2/Label2/Label1/TextBox1/Button1/shuoming/Timer_frm2/shuoming2/Label_pd/shuoming3；全屏无边框、蓝色背景(BackColor=Highlight)、TopMost、最大化、不显示在任务栏；Timer_frm2 间隔 1000ms。
		/// </summary>
		private void InitControlsAndLayout()
		{
			this.componentsContainer = new Container();
			this.Button2 = new Button();
			this.Label2 = new Label();
			this.Label1 = new Label();
			this.TextBox1 = new TextBox();
			this.Button1 = new Button();
			this.shuoming = new Label();
			this.Timer_frm2 = new System.Windows.Forms.Timer(this.componentsContainer);
			this.shuoming2 = new Label();
			this.Label_pd = new Label();
			this.shuoming3 = new Label();
			this.SuspendLayout();
			Control button = this.Button2;
			Point location = new Point(389, 17);
			button.Location = location;
			this.Button2.Name = "Button2";
			Control button2 = this.Button2;
			Size size = new Size(82, 28);
			button2.Size = size;
			this.Button2.TabIndex = 15;
			this.Button2.Text = "重启电脑";
			this.Button2.UseVisualStyleBackColor = true;
			this.Label2.AutoSize = true;
			this.Label2.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label2.ForeColor = Color.DarkRed;
			Control label = this.Label2;
			location = new Point(34, 65);
			label.Location = location;
			this.Label2.Name = "Label2";
			Control label2 = this.Label2;
			size = new Size(0, 14);
			label2.Size = size;
			this.Label2.TabIndex = 14;
			this.Label1.AutoSize = true;
			this.Label1.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label1.ForeColor = Color.White;
			Control label3 = this.Label1;
			location = new Point(61, 23);
			label3.Location = location;
			this.Label1.Name = "Label1";
			Control label4 = this.Label1;
			size = new Size(119, 14);
			label4.Size = size;
			this.Label1.TabIndex = 13;
			this.Label1.Text = "退出小助手密码：";
			Control textBox = this.TextBox1;
			location = new Point(196, 19);
			textBox.Location = location;
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.PasswordChar = '*';
			Control textBox2 = this.TextBox1;
			size = new Size(104, 21);
			textBox2.Size = size;
			this.TextBox1.TabIndex = 12;
			Control button3 = this.Button1;
			location = new Point(311, 16);
			button3.Location = location;
			this.Button1.Name = "Button1";
			Control button4 = this.Button1;
			size = new Size(54, 27);
			button4.Size = size;
			this.Button1.TabIndex = 11;
			this.Button1.Text = "确 定";
			this.Button1.UseVisualStyleBackColor = true;
			this.shuoming.AutoSize = true;
			this.shuoming.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming.ForeColor = Color.Red;
			Control shuoming = this.shuoming;
			location = new Point(152, 299);
			shuoming.Location = location;
			this.shuoming.Name = "shuoming";
			Control shuoming2 = this.shuoming;
			size = new Size(1136, 35);
			shuoming2.Size = size;
			this.shuoming.TabIndex = 16;
			this.shuoming.Text = "你运行的软件危害电子教室运行！如有误判，请老师拍照与作者QQ联系。";
			this.Timer_frm2.Interval = 1000;
			this.shuoming2.AutoSize = true;
			this.shuoming2.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming2.ForeColor = Color.Red;
			Control shuoming3 = this.shuoming2;
			location = new Point(152, 372);
			shuoming3.Location = location;
			this.shuoming2.Name = "shuoming2";
			Control shuoming4 = this.shuoming2;
			size = new Size(33, 35);
			shuoming4.Size = size;
			this.shuoming2.TabIndex = 17;
			this.shuoming2.Text = " ";
			this.Label_pd.AutoSize = true;
			this.Label_pd.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			Control label_pd = this.Label_pd;
			location = new Point(1, 2);
			label_pd.Location = location;
			this.Label_pd.Name = "Label_pd";
			Control label_pd2 = this.Label_pd;
			size = new Size(51, 35);
			label_pd2.Size = size;
			this.Label_pd.TabIndex = 18;
			this.Label_pd.Text = "  ";
			this.shuoming3.AutoSize = true;
			this.shuoming3.Font = new Font("宋体", 14.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming3.ForeColor = SystemColors.ActiveCaptionText;
			Control shuoming5 = this.shuoming3;
			location = new Point(669, 125);
			shuoming5.Location = location;
			this.shuoming3.Name = "shuoming3";
			Control shuoming6 = this.shuoming3;
			size = new Size(409, 19);
			shuoming6.Size = size;
			this.shuoming3.TabIndex = 19;
			this.shuoming3.Text = "                                        ";
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Highlight;
			size = new Size(1518, 1092);
			this.ClientSize = size;
			this.Controls.Add(this.shuoming3);
			this.Controls.Add(this.Label_pd);
			this.Controls.Add(this.shuoming2);
			this.Controls.Add(this.shuoming);
			this.Controls.Add(this.Button2);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.TextBox1);
			this.Controls.Add(this.Button1);
			this.FormBorderStyle = FormBorderStyle.None;
			this.Name = "Form2";
			this.ShowInTaskbar = false;
			this.Text = "Form2";
			this.TopMost = true;
			this.WindowState = FormWindowState.Maximized;
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00007A50 File Offset: 0x00005C50
		// (set) Token: 0x06000074 RID: 116 RVA: 0x00007A64 File Offset: 0x00005C64
		/// <summary>
		/// 按钮"重启电脑"（Click 处理器：Button2_Click_RestartComputer）。
		/// </summary>
		internal virtual Button Button2
		{
			get
			{
				return this.m_SystemVisitor;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button2_Click_RestartComputer);
				if (this.m_SystemVisitor != null)
				{
					this.m_SystemVisitor.Click -= value2;
				}
				this.m_SystemVisitor = value;
				if (this.m_SystemVisitor != null)
				{
					this.m_SystemVisitor.Click += value2;
				}
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00007AB0 File Offset: 0x00005CB0
		// (set) Token: 0x06000076 RID: 118 RVA: 0x00002232 File Offset: 0x00000432
		/// <summary>
		/// 密码提示标签（错误信息显示位）。
		/// </summary>
		internal virtual Label Label2
		{
			get
			{
				return this.instanceVisitor;
			}
			set
			{
				this.instanceVisitor = value;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00007AC4 File Offset: 0x00005CC4
		// (set) Token: 0x06000078 RID: 120 RVA: 0x0000223B File Offset: 0x0000043B
		/// <summary>
		/// 标签"退出小助手密码："。
		/// </summary>
		internal virtual Label Label1
		{
			get
			{
				return this.m_SetterVisitor;
			}
			set
			{
				this.m_SetterVisitor = value;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00007AD8 File Offset: 0x00005CD8
		// (set) Token: 0x0600007A RID: 122 RVA: 0x00002244 File Offset: 0x00000444
		/// <summary>
		/// 密码输入框（PasswordChar='*'）。
		/// </summary>
		internal virtual TextBox TextBox1
		{
			get
			{
				return this.prototypeVisitor;
			}
			set
			{
				this.prototypeVisitor = value;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00007AEC File Offset: 0x00005CEC
		// (set) Token: 0x0600007C RID: 124 RVA: 0x00007B00 File Offset: 0x00005D00
		/// <summary>
		/// 按钮"确 定"（Click 处理器：Button1_Click_VerifyPassword）。
		/// </summary>
		internal virtual Button Button1
		{
			get
			{
				return this.orderVisitor;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button1_Click_VerifyPassword);
				if (this.orderVisitor != null)
				{
					this.orderVisitor.Click -= value2;
				}
				this.orderVisitor = value;
				if (this.orderVisitor != null)
				{
					this.orderVisitor.Click += value2;
				}
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00007B4C File Offset: 0x00005D4C
		// (set) Token: 0x0600007E RID: 126 RVA: 0x0000224D File Offset: 0x0000044D
		/// <summary>
		/// 主警告文案标签（大字红色，由 Form1 各监控逻辑动态赋值）。
		/// </summary>
		internal virtual Label shuoming
		{
			get
			{
				return this._GlobalVisitor;
			}
			set
			{
				this._GlobalVisitor = value;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00007B60 File Offset: 0x00005D60
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00007B74 File Offset: 0x00005D74
		/// <summary>
		/// 锁鼠标定时器 Timer_frm2（1000ms）：Tick 处理器为 TimerFrm2_Tick_LockMouseBeep。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_frm2
		{
			get
			{
				return this.tagVisitor;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerFrm2_Tick_LockMouseBeep);
				if (this.tagVisitor != null)
				{
					this.tagVisitor.Tick -= value2;
				}
				this.tagVisitor = value;
				if (this.tagVisitor != null)
				{
					this.tagVisitor.Tick += value2;
				}
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00007BC0 File Offset: 0x00005DC0
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00002256 File Offset: 0x00000456
		/// <summary>
		/// 主警告文案标签（大字红色，由 Form1 各监控逻辑动态赋值）。
		/// </summary>
		internal virtual Label shuoming2
		{
			get
			{
				return this.m_ClassVisitor;
			}
			set
			{
				this.m_ClassVisitor = value;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00007BD4 File Offset: 0x00005DD4
		// (set) Token: 0x06000084 RID: 132 RVA: 0x00007BE8 File Offset: 0x00005DE8
		/// <summary>
		/// 隐藏标签（DoubleClick 处理器：LabelPd_DoubleClick_Noop，空壳）。
		/// </summary>
		internal virtual Label Label_pd
		{
			get
			{
				return this._ListenerVisitor;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.LabelPd_DoubleClick_Noop);
				if (this._ListenerVisitor != null)
				{
					this._ListenerVisitor.DoubleClick -= value2;
				}
				this._ListenerVisitor = value;
				if (this._ListenerVisitor != null)
				{
					this._ListenerVisitor.DoubleClick += value2;
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00007C34 File Offset: 0x00005E34
		// (set) Token: 0x06000086 RID: 134 RVA: 0x0000225F File Offset: 0x0000045F
		/// <summary>
		/// 主警告文案标签（大字红色，由 Form1 各监控逻辑动态赋值）。
		/// </summary>
		internal virtual Label shuoming3
		{
			get
			{
				return this.poolVisitor;
			}
			set
			{
				this.poolVisitor = value;
			}
		}

		// Token: 0x06000087 RID: 135
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：user32.ClipCursor —— 把鼠标限制在指定矩形（锁鼠标）。
		/// </summary>
		public static extern int ClipCursor(Form2.RECT lpRect);

		// Token: 0x06000088 RID: 136
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：kernel32.Beep —— 蜂鸣报警（2000Hz/500ms）。
		/// </summary>
		private static extern long Beep(long long_0, long long_1);

		// Token: 0x06000089 RID: 137 RVA: 0x00007C48 File Offset: 0x00005E48
		/// <summary>
		/// 确定按钮·校验密码（原名 CalcTemplate）。
		/// 流程：1. 密码长度 <6 → Label2 提示"密码应该是6位以上的字母或数字，请重新输入！"并清空；2. ModelVisitor.HashPasswordSha256(Trim(输入))（=SHA256(输入+"bfdshgs")）与 HKCU\Software\n 比较；3. 相等→禁用按钮并调用 UninstallAndExit()（卸载退出）；不等→提示"密码不正确"。
		/// </summary>
		// [反混淆] 原状态机 ~85行 → 还原为 if/else 正常控制流
		private void Button1_Click_VerifyPassword(object sender, EventArgs e)
		{
			try
			{
				// 密码长度 <6 → 提示并清空输入框
				if (Strings.Len(this.TextBox1.Text) < 6)
				{
					this.Label2.Text = "密码应该是6位以上的字母或数字，请重新输入！";
					this.TextBox1.Text = "";
					return;
				}
				// 输入密码 SHA256（带盐 bfdshgs）与注册表 HKCU\Software\n 比对
				string inputHash = ModelVisitor.HashPasswordSha256(Strings.Trim(this.TextBox1.Text));
				string registryHash = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
				if (Operators.CompareString(inputHash, registryHash, false) == 0)
				{
					// 密码正确 → 禁用按钮并执行卸载退出流程
					this.Button1.Enabled = false;
					this.UninstallAndExit();
					return;
				}
				// 密码错误 → 提示并清空
				this.Label2.Text = "密码不正确，请重新输入！";
				this.TextBox1.Text = "";
			}
			catch (Exception ex) { /* 原混淆样板：VB On Error 错误处理，忽略 */ }
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00007E00 File Offset: 0x00006000
		/// <summary>
		/// Timer_frm2.Tick·锁鼠标+蜂鸣（原名 MapTemplate）：TopMost=true；ClipCursor 到 (0,0)-(800,100)（鼠标被限制在屏幕顶部窄条）；beepTickCounter≥5 后 Beep(2000,500) 持续报警。
		/// </summary>
		// [反混淆] 原状态机 ~70行 → 还原为顺序执行 + if
		private void TimerFrm2_Tick_LockMouseBeep(object sender, EventArgs e)
		{
			try
			{
				this.TopMost = true;
				// 鼠标限制在屏幕顶部 100px 窄条 (0,0)-(800,100)
				Form2.RECT cursorClipRect;
				cursorClipRect.Left = 0;
				cursorClipRect.Top = 0;
				cursorClipRect.Right = 800;
				cursorClipRect.Bottom = 100;
				Form2.ClipCursor(cursorClipRect);
				checked
				{
					this.beepTickCounter++;
				}
				// 计数 ≥5 后开始持续蜂鸣报警
				if (this.beepTickCounter >= 5)
				{
					Form2.Beep(2000L, 500L);
				}
			}
			catch (Exception ex) { /* 原混淆样板：VB On Error 错误处理，忽略 */ }
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00007F2C File Offset: 0x0000612C
		/// <summary>
		/// 双击标签空操作（原名 SelectTemplate）：空壳方法（作者预留后门/彩蛋位），仅 VB 异常处理状态机。
		/// </summary>
		// [反混淆] 原状态机 ~35行 → 还原为空方法（无实际逻辑）
		private void LabelPd_DoubleClick_Noop(object sender, EventArgs e)
		{
			// 空壳：作者预留后门/彩蛋位，无实际操作
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00007FBC File Offset: 0x000061BC
		/// <summary>
		/// 卸载清理并退出（原名 PrepareTemplate）：1. 反复击杀随机名同伴进程（ModelVisitor.randomProcessName）；2. 枚举全部进程，进程名长度≥4 且每个字符 Asc 码在 102~111（'f'~'o'）→Kill（清扫同类随机命名进程）；3. DisableCMD=0；4. net stop zmserv；5. Sleep 3000；6. 击杀 zmserv 进程；7. ClipCursor 全屏；8. ProjectData.EndApp() 结束本进程。
		/// </summary>
		// [反混淆] 原状态机 ~150行 → 还原为 do-while + for 正常控制流
		private void UninstallAndExit()
		{
			try
			{
				// 1. 反复击杀随机名同伴进程（存在就一直杀第一个）
				string randomProcessName = Conversions.ToString(ModelVisitor.randomProcessName);
				Process[] processesByName;
				do
				{
					processesByName = Process.GetProcessesByName(randomProcessName);
					processesByName[0].Kill();
				}
				while (processesByName.Length != 0);
				// 2. 清扫"同类随机命名"进程：进程名长度≥4 且每个字符 Asc 码在 102~111（'f'~'o'）→ Kill
				int minAscCode = 102; // 原混淆为 num3 = 10*10+2
				Process[] allProcesses = Process.GetProcesses();
				for (int i = 0; i < allProcesses.Length; i++)
				{
					Process process = allProcesses[i];
					int nameLength = Strings.Len(process.ProcessName);
					if (nameLength >= 4)
					{
						bool allCharsInRange = true;
						for (int j = 1; j <= nameLength; j++)
						{
							int charAsc = Strings.Asc(Strings.Mid(process.ProcessName, j, 1));
							if (charAsc < minAscCode || charAsc > minAscCode + 9)
							{
								allCharsInRange = false;
								break;
							}
						}
						if (allCharsInRange)
						{
							process.Kill();
						}
					}
				}
				// 3. 解除 CMD 禁用 → 4. 停止 zmserv 服务 → 5. 等待 3 秒 → 6. 击杀 zmserv 进程
				ModelVisitor.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
				Thread.Sleep(3000);
				Process[] zmservProcesses = Process.GetProcessesByName("zmserv");
				zmservProcesses[0].Kill();
				// 7. 全屏锁定鼠标 → 8. 结束本进程
				Form2.RECT fullScreenRect;
				fullScreenRect.Left = 0;
				fullScreenRect.Top = 0;
				fullScreenRect.Right = Screen.PrimaryScreen.Bounds.Width;
				fullScreenRect.Bottom = Screen.PrimaryScreen.Bounds.Height;
				Form2.ClipCursor(fullScreenRect);
				ProjectData.EndApp();
			}
			catch (Exception ex) { /* 原混淆样板：VB On Error 错误处理，忽略 */ }
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00002268 File Offset: 0x00000468
		/// <summary>
		/// 禁止关闭（原名 CustomizeTemplate）：e.Cancel = true —— 用户无法用任何方式关闭锁屏窗（Alt+F4 等无效）。
		/// </summary>
		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000082D8 File Offset: 0x000064D8
		/// <summary>
		/// Load（原名 ForgotTemplate）：Timer_frm2.Enabled = true 启动锁鼠标定时器。
		/// </summary>
		// [反混淆] 原状态机 ~35行 → 还原为单语句
		private void Form2_Load(object sender, EventArgs e)
		{
			try
			{
				this.Timer_frm2.Enabled = true;
			}
			catch (Exception ex) { /* 原混淆样板：VB On Error 错误处理，忽略 */ }
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00008374 File Offset: 0x00006574
		/// <summary>
		/// 重启电脑（原名 OrderTemplate）：Interaction.Shell("shutdown /r /f /t 0", MinimizedFocus) 立即强制重启。
		/// </summary>
		// [反混淆] 原状态机 ~35行 → 还原为单语句
		private void Button2_Click_RestartComputer(object sender, EventArgs e)
		{
			try
			{
				Interaction.Shell("shutdown /r /f /t 0", AppWinStyle.MinimizedFocus, false, -1);
			}
			catch (Exception ex) { /* 原混淆样板：VB On Error 错误处理，忽略 */ }
		}

		// Token: 0x04000081 RID: 129
		[AccessedThroughProperty("Button2")]
		/// <summary>
		/// Button2（重启电脑）后备字段。
		/// </summary>
		private Button m_SystemVisitor;

		// Token: 0x04000082 RID: 130
		[AccessedThroughProperty("Label2")]
		/// <summary>
		/// Label2（密码提示）后备字段。
		/// </summary>
		private Label instanceVisitor;

		// Token: 0x04000083 RID: 131
		[AccessedThroughProperty("Label1")]
		/// <summary>
		/// Label1（"退出小助手密码："）后备字段。
		/// </summary>
		private Label m_SetterVisitor;

		// Token: 0x04000084 RID: 132
		[AccessedThroughProperty("TextBox1")]
		/// <summary>
		/// TextBox1（密码输入框）后备字段。
		/// </summary>
		private TextBox prototypeVisitor;

		// Token: 0x04000085 RID: 133
		[AccessedThroughProperty("Button1")]
		/// <summary>
		/// Button1（确定）后备字段。
		/// </summary>
		private Button orderVisitor;

		// Token: 0x04000086 RID: 134
		[AccessedThroughProperty("shuoming")]
		/// <summary>
		/// shuoming（主警告文案）后备字段。
		/// </summary>
		private Label _GlobalVisitor;

		// Token: 0x04000087 RID: 135
		[AccessedThroughProperty("Timer_frm2")]
		/// <summary>
		/// Timer_frm2（锁鼠标定时器）后备字段。
		/// </summary>
		private System.Windows.Forms.Timer tagVisitor;

		// Token: 0x04000088 RID: 136
		[AccessedThroughProperty("shuoming2")]
		/// <summary>
		/// shuoming2（副警告文案）后备字段。
		/// </summary>
		private Label m_ClassVisitor;

		// Token: 0x04000089 RID: 137
		[AccessedThroughProperty("Label_pd")]
		/// <summary>
		/// Label_pd（隐藏标签）后备字段。
		/// </summary>
		private Label _ListenerVisitor;

		// Token: 0x0400008A RID: 138
		[AccessedThroughProperty("shuoming3")]
		/// <summary>
		/// shuoming3（水印标签）后备字段。
		/// </summary>
		private Label poolVisitor;

		// Token: 0x0400008B RID: 139
		/// <summary>
		/// 蜂鸣计数（原名 tokenVisitor）：Timer_frm2 每 Tick +1，≥5 开始 Beep。
		/// </summary>
		private int beepTickCounter;

		// Token: 0x0200000B RID: 11
		/// <summary>
		/// ClipCursor 用矩形结构体（Left/Top/Right/Bottom）。
		/// </summary>
		public struct RECT
		{
			// Token: 0x0400008C RID: 140
			public int Left;

			// Token: 0x0400008D RID: 141
			public int Top;

			// Token: 0x0400008E RID: 142
			public int Right;

			// Token: 0x0400008F RID: 143
			public int Bottom;
		}
	}
}
