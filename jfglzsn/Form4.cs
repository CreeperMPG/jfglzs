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
	// Token: 0x0200000D RID: 13
	[DesignerGenerated]
	// ============================================================
	// Form4.cs — 二级密码锁定窗（高级锁屏，带 DisableCMD 恢复逻辑）
	// 与 Form2 几乎相同：无边框、置顶、最大化、ClipCursor 锁鼠标+Beep。区别：1) 密码可匹配主密码 HKCU\Software\n 或二级密码 HKCU\Software\jfglzs\n2；2) UninstallAndExit 额外处理 DisableCMD 开关恢复（临时放开 cmd 执行 net stop zmserv，之后恢复）；3) FormClosing 受 ModelVisitor.lockFormCanClose 控制（false 可关并释放鼠标，true 禁止）。
	// ============================================================
	public partial class Form4 : Form
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00008710 File Offset: 0x00006910
		/// <summary>
		/// 构造器：挂接 Load(Form4_Load) 与 FormClosing(Form4_FormClosing) 事件；调用 InitControlsAndLayout() 初始化控件。
		/// </summary>
		public Form4()
		{
			base.Load += this.Form4_Load;
			base.FormClosing += this.Form4_FormClosing;
			this.InitControlsAndLayout();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00008790 File Offset: 0x00006990
		/// <summary>
		/// 初始化控件与布局（原名 StopTemplate）：与 Form2 相同布局（全屏无边框、蓝色背景、TopMost、最大化），Timer_frm4 间隔 1000ms。
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
			this.Timer_frm4 = new System.Windows.Forms.Timer(this.componentsContainer);
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
			size = new Size(33, 35);
			shuoming2.Size = size;
			this.shuoming.TabIndex = 16;
			this.shuoming.Text = " ";
			this.Timer_frm4.Interval = 1000;
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
			location = new Point(1, 3);
			label_pd.Location = location;
			this.Label_pd.Name = "Label_pd";
			Control label_pd2 = this.Label_pd;
			size = new Size(51, 35);
			label_pd2.Size = size;
			this.Label_pd.TabIndex = 18;
			this.Label_pd.Text = "  ";
			this.shuoming3.AutoSize = true;
			this.shuoming3.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.shuoming3.ForeColor = SystemColors.ActiveCaptionText;
			Control shuoming5 = this.shuoming3;
			location = new Point(922, 103);
			shuoming5.Location = location;
			this.shuoming3.Name = "shuoming3";
			Control shuoming6 = this.shuoming3;
			size = new Size(245, 12);
			shuoming6.Size = size;
			this.shuoming3.TabIndex = 19;
			this.shuoming3.Text = "                                        ";
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.Highlight;
			size = new Size(1254, 729);
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
			this.Name = "Form4";
			this.ShowInTaskbar = false;
			this.Text = "Form2";
			this.TopMost = true;
			this.WindowState = FormWindowState.Maximized;
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00008DFC File Offset: 0x00006FFC
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00008E10 File Offset: 0x00007010
		/// <summary>
		/// 按钮"重启电脑"（Click 处理器：Button2_Click_RestartComputer）。
		/// </summary>
		internal virtual Button Button2
		{
			get
			{
				return this._ClientVisitor;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button2_Click_RestartComputer);
				if (this._ClientVisitor != null)
				{
					this._ClientVisitor.Click -= value2;
				}
				this._ClientVisitor = value;
				if (this._ClientVisitor != null)
				{
					this._ClientVisitor.Click += value2;
				}
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00008E5C File Offset: 0x0000705C
		// (set) Token: 0x0600009F RID: 159 RVA: 0x0000229A File Offset: 0x0000049A
		/// <summary>
		/// 密码提示标签。
		/// </summary>
		internal virtual Label Label2
		{
			get
			{
				return this._CandidateVisitor;
			}
			set
			{
				this._CandidateVisitor = value;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00008E70 File Offset: 0x00007070
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x000022A3 File Offset: 0x000004A3
		/// <summary>
		/// 标签"退出小助手密码："。
		/// </summary>
		internal virtual Label Label1
		{
			get
			{
				return this.printerVisitor;
			}
			set
			{
				this.printerVisitor = value;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00008E84 File Offset: 0x00007084
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x000022AC File Offset: 0x000004AC
		/// <summary>
		/// 密码输入框（PasswordChar='*'）。
		/// </summary>
		internal virtual TextBox TextBox1
		{
			get
			{
				return this._WrapperVisitor;
			}
			set
			{
				this._WrapperVisitor = value;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00008E98 File Offset: 0x00007098
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x00008EAC File Offset: 0x000070AC
		/// <summary>
		/// 按钮"确 定"（Click 处理器：Button1_Click_VerifyPassword）。
		/// </summary>
		internal virtual Button Button1
		{
			get
			{
				return this.advisorVisitor;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button1_Click_VerifyPassword);
				if (this.advisorVisitor != null)
				{
					this.advisorVisitor.Click -= value2;
				}
				this.advisorVisitor = value;
				if (this.advisorVisitor != null)
				{
					this.advisorVisitor.Click += value2;
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x00008EF8 File Offset: 0x000070F8
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x000022B5 File Offset: 0x000004B5
		/// <summary>
		/// 主警告文案标签。
		/// </summary>
		internal virtual Label shuoming
		{
			get
			{
				return this._PageVisitor;
			}
			set
			{
				this._PageVisitor = value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00008F0C File Offset: 0x0000710C
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00008F20 File Offset: 0x00007120
		/// <summary>
		/// 锁鼠标定时器 Timer_frm4（1000ms）：Tick 处理器为 TimerFrm4_Tick_LockMouseBeep。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer_frm4
		{
			get
			{
				return this.statusVisitor;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.TimerFrm4_Tick_LockMouseBeep);
				if (this.statusVisitor != null)
				{
					this.statusVisitor.Tick -= value2;
				}
				this.statusVisitor = value;
				if (this.statusVisitor != null)
				{
					this.statusVisitor.Tick += value2;
				}
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00008F6C File Offset: 0x0000716C
		// (set) Token: 0x060000AB RID: 171 RVA: 0x000022BE File Offset: 0x000004BE
		/// <summary>
		/// 主警告文案标签。
		/// </summary>
		internal virtual Label shuoming2
		{
			get
			{
				return this.m_MapperVisitor;
			}
			set
			{
				this.m_MapperVisitor = value;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000AC RID: 172 RVA: 0x00008F80 File Offset: 0x00007180
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00008F94 File Offset: 0x00007194
		/// <summary>
		/// 隐藏标签（DoubleClick 处理器：LabelPd_DoubleClick_Noop，空壳）。
		/// </summary>
		internal virtual Label Label_pd
		{
			get
			{
				return this.interpreterVisitor;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.LabelPd_DoubleClick_Noop);
				if (this.interpreterVisitor != null)
				{
					this.interpreterVisitor.DoubleClick -= value2;
				}
				this.interpreterVisitor = value;
				if (this.interpreterVisitor != null)
				{
					this.interpreterVisitor.DoubleClick += value2;
				}
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00008FE0 File Offset: 0x000071E0
		// (set) Token: 0x060000AF RID: 175 RVA: 0x000022C7 File Offset: 0x000004C7
		/// <summary>
		/// 主警告文案标签。
		/// </summary>
		internal virtual Label shuoming3
		{
			get
			{
				return this._ListVisitor;
			}
			set
			{
				this._ListVisitor = value;
			}
		}

		// Token: 0x060000B0 RID: 176
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：user32.ClipCursor —— 把鼠标限制在指定矩形（锁鼠标）。
		/// </summary>
		public static extern int ClipCursor(Form4.RECT lpRect);

		// Token: 0x060000B1 RID: 177
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		/// <summary>
		/// P/Invoke：kernel32.Beep —— 蜂鸣报警（2000Hz/500ms）。
		/// </summary>
		private static extern long Beep(long long_0, long long_1);

		// Token: 0x060000B2 RID: 178 RVA: 0x00008FF4 File Offset: 0x000071F4
		/// <summary>
		/// 确定按钮·校验密码（原名 RevertTemplate）：双密码比对——HashPasswordSha256(Trim(输入)) 与 HKCU\Software\n **或** HKCU\Software\jfglzs\n2 任一相等→禁用按钮、调用 UninstallAndExit()；否则提示"密码不正确"。
		/// </summary>
		// [反混淆] 原状态机 ~90行 → 还原为 if/else 正常控制流
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
				// 输入密码 SHA256（带盐 bfdshgs）与主密码 HKCU\\Software\\n 或二级密码 HKCU\\Software\\jfglzs\\n2 比对
				string inputHash = ModelVisitor.HashPasswordSha256(Strings.Trim(this.TextBox1.Text));
				string primaryHash = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
				string secondaryHash = Conversions.ToString(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "n2", ""));
				if (Operators.CompareString(inputHash, primaryHash, false) == 0 | Operators.CompareString(inputHash, secondaryHash, false) == 0)
				{
					// 任一密码正确 → 禁用按钮并执行卸载退出流程
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

		// Token: 0x060000B3 RID: 179 RVA: 0x000091EC File Offset: 0x000073EC
		/// <summary>
		/// Timer_frm4.Tick·锁鼠标+蜂鸣（原名 UpdateConfig）：与 Form2 相同——TopMost=true、ClipCursor 到 (0,0)-(800,100)、beepTickCounter≥5 后 Beep(2000,500)。
		/// </summary>
		// [反混淆] 原状态机 ~70行 → 还原为顺序执行 + if
		private void TimerFrm4_Tick_LockMouseBeep(object sender, EventArgs e)
		{
			try
			{
				this.TopMost = true;
				// 鼠标限制在屏幕顶部 100px 窄条 (0,0)-(800,100)
				Form4.RECT cursorClipRect;
				cursorClipRect.Left = 0;
				cursorClipRect.Top = 0;
				cursorClipRect.Right = 800;
				cursorClipRect.Bottom = 100;
				Form4.ClipCursor(cursorClipRect);
				checked
				{
					this.beepTickCounter++;
				}
				// 计数 ≥5 后开始持续蜂鸣报警
				if (this.beepTickCounter >= 5)
				{
					Form4.Beep(2000L, 500L);
				}
			}
			catch (Exception ex) { /* 原混淆样板：VB On Error 错误处理，忽略 */ }
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00007F2C File Offset: 0x0000612C
		/// <summary>
		/// 双击标签空操作（原名 PostConfig）：空壳方法。
		/// </summary>
		// [反混淆] 原状态机 ~35行 → 还原为空方法（无实际逻辑）
		private void LabelPd_DoubleClick_Noop(object sender, EventArgs e)
		{
			// 空壳：无实际操作
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00009318 File Offset: 0x00007518
		/// <summary>
		/// 卸载清理并退出（原名 FindConfig）：同 Form2 流程，但增加 DisableCMD 恢复逻辑——读 DisableCMD：==1 → 先写 0（放开 cmd）→ net stop zmserv → Sleep 3000 → 写回 1（恢复禁用）；!=1 → 直接 net stop zmserv + Sleep 3000；随后击杀 zmserv 进程、ClipCursor 全屏、EndApp()。
		/// </summary>
		// [反混淆] 原状态机 ~185行 → 还原为 do-while + for + if 正常控制流
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
				// 3. DisableCMD 恢复逻辑：若之前禁用 cmd（==1）→ 先写 0 放开 → net stop zmserv → Sleep 3s → 写回 1 恢复禁用；否则直接 net stop + Sleep
				int disableCmdValue = Conversions.ToInteger(ModelVisitor.ReadRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", ""));
				if (disableCmdValue == 1)
				{
					ModelVisitor.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
					Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
					Thread.Sleep(3000);
					ModelVisitor.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 1, RegistryValueKind.DWord);
				}
				else
				{
					Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
					Thread.Sleep(3000);
				}
				// 4. 击杀 zmserv 进程 → 5. 全屏锁定鼠标 → 6. 结束本进程
				Process[] zmservProcesses = Process.GetProcessesByName("zmserv");
				zmservProcesses[0].Kill();
				Form4.RECT fullScreenRect;
				fullScreenRect.Left = 0;
				fullScreenRect.Top = 0;
				fullScreenRect.Right = Screen.PrimaryScreen.Bounds.Width;
				fullScreenRect.Bottom = Screen.PrimaryScreen.Bounds.Height;
				Form4.ClipCursor(fullScreenRect);
				ProjectData.EndApp();
			}
			catch (Exception ex) { /* 原混淆样板：VB On Error 错误处理，忽略 */ }
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000096C4 File Offset: 0x000078C4
		/// <summary>
		/// 按 lockFormCanClose 决定可否关闭（原名 ConcatConfig）：ModelVisitor.lockFormCanClose==false → e.Cancel=false（允许关闭）并 ClipCursor 全屏（释放鼠标）；true → e.Cancel=true 禁止关闭。该标志可能由同伴程序/服务端切换。
		/// </summary>
		private void Form4_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!ModelVisitor.lockFormCanClose)
			{
				e.Cancel = false;
				Form4.RECT lpRect;
				lpRect.Left = 0;
				lpRect.Top = 0;
				lpRect.Right = Screen.PrimaryScreen.Bounds.Width;
				lpRect.Bottom = Screen.PrimaryScreen.Bounds.Height;
				Form4.ClipCursor(lpRect);
			}
			else
			{
				e.Cancel = true;
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00009734 File Offset: 0x00007934
		/// <summary>
		/// Load（原名 AddConfig）：Timer_frm4.Enabled = true 启动锁鼠标定时器。
		/// </summary>
		// [反混淆] 原状态机 ~35行 → 还原为单语句
		private void Form4_Load(object sender, EventArgs e)
		{
			try
			{
				this.Timer_frm4.Enabled = true;
			}
			catch (Exception ex) { /* 原混淆样板：VB On Error 错误处理，忽略 */ }
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000097D0 File Offset: 0x000079D0
		/// <summary>
		/// 重启电脑（原名 CountConfig）：Interaction.Shell("shutdown /r /f /t 0", MinimizedFocus)。
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

		// Token: 0x04000095 RID: 149
		[AccessedThroughProperty("Button2")]
		/// <summary>
		/// Button2（重启电脑）后备字段。
		/// </summary>
		private Button _ClientVisitor;

		// Token: 0x04000096 RID: 150
		[AccessedThroughProperty("Label2")]
		/// <summary>
		/// Label2（密码提示）后备字段。
		/// </summary>
		private Label _CandidateVisitor;

		// Token: 0x04000097 RID: 151
		[AccessedThroughProperty("Label1")]
		/// <summary>
		/// Label1（"退出小助手密码："）后备字段。
		/// </summary>
		private Label printerVisitor;

		// Token: 0x04000098 RID: 152
		[AccessedThroughProperty("TextBox1")]
		/// <summary>
		/// TextBox1（密码输入框）后备字段。
		/// </summary>
		private TextBox _WrapperVisitor;

		// Token: 0x04000099 RID: 153
		[AccessedThroughProperty("Button1")]
		/// <summary>
		/// Button1（确定）后备字段。
		/// </summary>
		private Button advisorVisitor;

		// Token: 0x0400009A RID: 154
		[AccessedThroughProperty("shuoming")]
		/// <summary>
		/// shuoming（主警告文案）后备字段。
		/// </summary>
		private Label _PageVisitor;

		// Token: 0x0400009B RID: 155
		[AccessedThroughProperty("Timer_frm4")]
		/// <summary>
		/// Timer_frm4（锁鼠标定时器）后备字段。
		/// </summary>
		private System.Windows.Forms.Timer statusVisitor;

		// Token: 0x0400009C RID: 156
		[AccessedThroughProperty("shuoming2")]
		/// <summary>
		/// shuoming2（副警告文案）后备字段。
		/// </summary>
		private Label m_MapperVisitor;

		// Token: 0x0400009D RID: 157
		[AccessedThroughProperty("Label_pd")]
		/// <summary>
		/// Label_pd（隐藏标签）后备字段。
		/// </summary>
		private Label interpreterVisitor;

		// Token: 0x0400009E RID: 158
		[AccessedThroughProperty("shuoming3")]
		/// <summary>
		/// shuoming3（水印标签）后备字段。
		/// </summary>
		private Label _ListVisitor;

		// Token: 0x0400009F RID: 159
		/// <summary>
		/// 蜂鸣计数（原名 invocationVisitor）：Timer_frm4 每 Tick +1，≥5 开始 Beep。
		/// </summary>
		private int beepTickCounter;

		// Token: 0x0200000E RID: 14
		/// <summary>
		/// ClipCursor 用矩形结构体（Left/Top/Right/Bottom）。
		/// </summary>
		public struct RECT
		{
			// Token: 0x040000A0 RID: 160
			public int Left;

			// Token: 0x040000A1 RID: 161
			public int Top;

			// Token: 0x040000A2 RID: 162
			public int Right;

			// Token: 0x040000A3 RID: 163
			public int Bottom;
		}
	}
}
