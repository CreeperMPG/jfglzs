using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using WindowsApplication1.Internal;

namespace WindowsApplication1
{
	/// <summary>
	/// 退出程序窗体：本程序唯一受密码保护的出口，也是整套管控的总开关。
	/// 名字叫「退出」，实际做的是「临时解除管控」——密码校验通过后停掉监控进程与服务、
	/// 把 USB 存储与浏览器下载策略改回允许、杀掉浏览器、重启 explorer。
	/// </summary>
	/// <remarks>
	/// 敏感行为：单轮 SHA256(密码 + 固定盐 "bfdshgs") 校验（盐硬编码，可离线爆破）；
	/// 按精确名与「名字形状」模糊查杀进程；Interaction.Shell("net stop zmserv") 停服务（隐藏窗口、同步等待）；
	/// 改写 HKLM/HKCU 下 USB、命令提示符、Edge/Chrome/Firefox/IE 共 16 处策略；杀 explorer 后重启以立即生效。
	/// </remarks>
	// Token: 0x0200000C RID: 12
	[DesignerGenerated]
	public class Form3 : Form
	{
		// Token: 0x06000057 RID: 87 RVA: 0x0000242F File Offset: 0x0000062F
		/// <summary>
		/// 构造函数：直接调用初始化控件（VB 的 InitializeComponent）。
		/// </summary>
		public Form3()
		{
			this.初始化控件();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003410 File Offset: 0x00001610
		/// <summary>
		/// 设计器生成的释放逻辑：disposing 为真时先释放控件容器，再调用基类 Dispose。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.控件容器 != null)
				{
					this.控件容器.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003450 File Offset: 0x00001650
		/// <summary>
		/// VB 的 InitializeComponent：密码提示标签、密码框、退出按钮、说明文字（顺序为原版顺序）。
		/// 窗口标题「退出程序」、ClientSize 417×181，密码框 PasswordChar 为 '*'。
		/// </summary>
		private void 初始化控件()
		{
			this.Label1 = new Label();
			this.Label3 = new Label();
			this.TextBox1 = new TextBox();
			this.Button1 = new Button();
			this.Label2 = new Label();
			base.SuspendLayout();
			this.Label1.AutoSize = true;
			this.Label1.ForeColor = Color.Red;
			this.Label1.Location = new Point(56, 112);
			this.Label1.Name = "Label1";
			this.Label1.Size = new Size(0, 12);
			this.Label1.TabIndex = 19;
			this.Label3.AutoSize = true;
			this.Label3.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label3.ForeColor = Color.DarkRed;
			this.Label3.Location = new Point(37, 46);
			this.Label3.Name = "Label3";
			this.Label3.Size = new Size(119, 14);
			this.Label3.TabIndex = 18;
			this.Label3.Text = "输入小助手密码：";
			this.TextBox1.Location = new Point(172, 42);
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.PasswordChar = '*';
			this.TextBox1.Size = new Size(104, 21);
			this.TextBox1.TabIndex = 17;
			this.Button1.Location = new Point(294, 39);
			this.Button1.Name = "Button1";
			this.Button1.Size = new Size(96, 27);
			this.Button1.TabIndex = 16;
			this.Button1.Text = "退出(需10秒)";
			this.Button1.UseVisualStyleBackColor = true;
			this.Label2.AutoSize = true;
			this.Label2.Location = new Point(38, 87);
			this.Label2.Name = "Label2";
			this.Label2.Size = new Size(287, 12);
			this.Label2.TabIndex = 21;
			this.Label2.Text = "注：用密码退出后可使用U盘、下载文件，停止监控。";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(417, 181);
			base.Controls.Add(this.Label2);
			base.Controls.Add(this.Label1);
			base.Controls.Add(this.Label3);
			base.Controls.Add(this.TextBox1);
			base.Controls.Add(this.Button1);
			base.Name = "Form3";
			this.Text = "退出程序";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600005A RID: 90 RVA: 0x0000243D File Offset: 0x0000063D
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00002445 File Offset: 0x00000645
		/// <summary>
		/// 提示标签（Label1）：密码过短或密码错误时显示红色提示。
		/// </summary>
		internal virtual Label Label1 { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600005C RID: 92 RVA: 0x0000244E File Offset: 0x0000064E
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00002456 File Offset: 0x00000656
		/// <summary>
		/// 密码标题标签（Label3）：固定文本「输入小助手密码：」，深红色 10.5pt。
		/// </summary>
		internal virtual Label Label3 { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600005E RID: 94 RVA: 0x0000245F File Offset: 0x0000065F
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00002467 File Offset: 0x00000667
		/// <summary>
		/// 密码输入框（TextBox1）：PasswordChar 为 '*'，校验失败或过短时被清空。
		/// </summary>
		internal virtual TextBox TextBox1 { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00002470 File Offset: 0x00000670
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00003754 File Offset: 0x00001954
		/// <summary>
		/// 退出按钮（Button1）。setter 在换实例时同步退订/订阅 Click（指向 退出按钮_Click）。
		/// </summary>
		internal virtual Button Button1
		{
			[CompilerGenerated]
			get
			{
				return this.退出按钮;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.退出按钮_Click);
				Button button = this.退出按钮;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this.退出按钮 = value;
				button = this.退出按钮;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00002478 File Offset: 0x00000678
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00002480 File Offset: 0x00000680
		/// <summary>
		/// 说明标签（Label2）：「注：用密码退出后可使用U盘、下载文件，停止监控。」——直接说明本窗体的真实用途。
		/// </summary>
		internal virtual Label Label2 { get; set; }

		// Token: 0x06000064 RID: 100 RVA: 0x00003798 File Offset: 0x00001998
		/// <summary>
		/// 「退出(需10秒)」按钮：先查密码长度，再比对密码哈希，通过后依次执行解除管控的动作。
		/// 顺序：停监控与服务 → 恢复浏览器策略 → 放开 USB 存储（4 个 ControlSet）→ 重启 explorer。
		/// </summary>
		/// <remarks>
		/// 原版整段是 VB 的 <c>On Error Resume Next</c>：<c>ClearProjectError()</c> 开场、<c>num2 = 2..23</c>
		/// 是每条语句的状态号、末尾 <c>catch when (endfilter(...))</c> 按状态号分发。
		/// 三处跳转的含义：IL_0046 = 密码够长、跳过提示进入校验；IL_0182 = 跳进「密码不正确」分支；
		/// IL_01A8 = 统一出口（两种情况都从这里结束）。
		/// 这里还原成「提前 return + if」并把每个有副作用/可能抛异常的语句各自包 try/catch，
		/// 与「某条失败就丢掉、继续下一条」的 VB 语义一致；纯局部变量赋值不会抛异常，没有包。
		/// 密码错误与密码过短的处理相同：写提示 + 清空输入框后结束。
		/// </remarks>
		private void 退出按钮_Click(object sender, EventArgs e)
		{
			if (Strings.Len(this.TextBox1.Text) < 6)
			{
				try
				{
					this.Label1.Text = "密码应该是6位以上的字母或数字，请重新输入！";
				}
				catch
				{
				}
				try
				{
					this.TextBox1.Text = "";
				}
				catch
				{
				}
				return;
			}
			string 输入哈希 = "";
			try
			{
				输入哈希 = SetterMapper.CalcComposer(Strings.Trim(this.TextBox1.Text));
			}
			catch
			{
			}
			string 已存哈希 = "";
			try
			{
				已存哈希 = Conversions.ToString(SetterMapper.DisableComposer(Registry.CurrentUser, "Software", "n", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(输入哈希, 已存哈希, false) != 0)
			{
				try
				{
					this.Label1.Text = "密码不正确，请重新输入！";
				}
				catch
				{
				}
				try
				{
					this.TextBox1.Text = "";
				}
				catch
				{
				}
				return;
			}
			try
			{
				this.Button1.Enabled = false;
			}
			catch
			{
			}
			try
			{
				this.Label1.Text = "";
			}
			catch
			{
			}
			try
			{
				this.执行退出流程();
			}
			catch
			{
			}
			try
			{
				this.恢复浏览器下载策略();
			}
			catch
			{
			}
			try
			{
				SetterMapper.PrintComposer(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
			}
			catch
			{
			}
			try
			{
				SetterMapper.PrintComposer(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
			}
			catch
			{
			}
			try
			{
				SetterMapper.PrintComposer(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
			}
			catch
			{
			}
			try
			{
				SetterMapper.PrintComposer(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
			}
			catch
			{
			}
			try
			{
				foreach (Process process in Process.GetProcessesByName("explorer"))
				{
					try
					{
						process.Kill();
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			try
			{
				Process.Start("explorer.exe");
			}
			catch
			{
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003A24 File Offset: 0x00001C24
		/// <summary>
		/// 按进程名（不是文件名）判断进程是否存在。
		/// </summary>
		/// <param name="进程名">不带 .exe 后缀的进程名</param>
		/// <returns>存在至少一个同名进程返回 true</returns>
		private bool 进程是否在运行(string 进程名)
		{
			return Process.GetProcessesByName(进程名).Length > 0;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003A44 File Offset: 0x00001C44
		/// <summary>
		/// 停监控端的主流程：先循环杀掉助手/监控进程，再按「名字形状」模糊查杀，
		/// 然后放开命令提示符、两次 net stop zmserv，最后判定是否停干净——成功则关窗体，失败则提示并恢复按钮。
		/// </summary>
		/// <remarks>
		/// 原版整段处于 checked 上下文，且是 VB 的 <c>On Error Resume Next</c>（状态号 num2 = 2..53 +
		/// 末尾按状态号分发的 catch）；<c>num8</c> 是外层状态机变量、<c>num</c> 是错误分发标志，都是编译器噪声。
		/// 这里还原为 if/while + 「每条有副作用的语句各自 try/catch」：取进程列表、Kill、写注册表、
		/// 执行命令、弹框、关闭窗体各自吞异常，一条失败不影响后续步骤。纯局部变量赋值不会抛异常，未包。
		/// 两个 do-while 是重试循环：先取列表再 Kill 第 0 个，列表为空时 [0] 抛下标越界被单独吞掉，
		/// 循环条件「两个列表都不为空」为假才退出——即一直重试到两个进程都不在为止。
		/// 数组初值给空数组只是为满足 C# 的肯定赋值：VB 里初值是 Nothing，取列表失败时同样表现为「空」并按此退出循环。
		/// </remarks>
		private void 执行退出流程()
		{
			checked
			{
				try
				{
					this.Button1.Enabled = false;
				}
				catch
				{
				}
				string 服务名 = "zmserv";
				string 助手进程名 = Conversions.ToString(SetterMapper._ValMapper);
				string 监控进程名 = "jfglzsn";
				Process[] 助手进程列表 = new Process[0];
				Process[] 监控进程列表 = new Process[0];
				do
				{
					try
					{
						助手进程列表 = Process.GetProcessesByName(助手进程名);
					}
					catch
					{
					}
					try
					{
						助手进程列表[0].Kill();
					}
					catch
					{
					}
					try
					{
						监控进程列表 = Process.GetProcessesByName(监控进程名);
					}
					catch
					{
					}
					try
					{
						监控进程列表[0].Kill();
					}
					catch
					{
					}
				}
				while (!((助手进程列表.Length == 0) & (监控进程列表.Length == 0)));
				int 字符下界 = 10;
				字符下界 = 字符下界 * 10 + 2;
				Process[] 全部进程 = new Process[0];
				try
				{
					全部进程 = Process.GetProcesses();
				}
				catch
				{
				}
				foreach (Process process in 全部进程)
				{
					try
					{
						int 进程名长度 = Strings.Len(process.ProcessName);
						if (进程名长度 >= 4)
						{
							int 循环上界 = 进程名长度;
							int 扫描位置 = 1;
							while (扫描位置 <= 循环上界)
							{
								int 字符编码 = 0;
								try
								{
									字符编码 = Strings.Asc(Strings.Mid(process.ProcessName, 扫描位置, 1));
								}
								catch
								{
								}
								if (字符编码 >= 字符下界 & 字符编码 <= 字符下界 + 9)
								{
									扫描位置++;
									continue;
								}
								break;
							}
							if (扫描位置 == 进程名长度 + 1)
							{
								try
								{
									process.Kill();
								}
								catch
								{
								}
							}
						}
					}
					catch
					{
					}
				}
				try
				{
					SetterMapper.PrintComposer(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				}
				catch
				{
				}
				try
				{
					Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
				}
				catch
				{
				}
				try
				{
					Thread.Sleep(3000);
				}
				catch
				{
				}
				Process[] 服务进程列表 = new Process[0];
				try
				{
					服务进程列表 = Process.GetProcessesByName(服务名);
				}
				catch
				{
				}
				try
				{
					服务进程列表[0].Kill();
				}
				catch
				{
				}
				do
				{
					try
					{
						助手进程列表 = Process.GetProcessesByName(助手进程名);
					}
					catch
					{
					}
					try
					{
						助手进程列表[0].Kill();
					}
					catch
					{
					}
					try
					{
						监控进程列表 = Process.GetProcessesByName(监控进程名);
					}
					catch
					{
					}
					try
					{
						监控进程列表[0].Kill();
					}
					catch
					{
					}
				}
				while (!((助手进程列表.Length == 0) & (监控进程列表.Length == 0)));
				try
				{
					Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
				}
				catch
				{
				}
				try
				{
					Thread.Sleep(2000);
				}
				catch
				{
				}
				if (!this.进程是否在运行("jfglzsn") & !this.进程是否在运行(Conversions.ToString(SetterMapper._ValMapper)))
				{
					try
					{
						Interaction.MsgBox("退出程序成功！", MsgBoxStyle.OkOnly, null);
					}
					catch
					{
					}
					try
					{
						base.Close();
					}
					catch
					{
					}
				}
				else
				{
					try
					{
						Interaction.MsgBox("退出程序不成功！请重新执行退出程序命令或重启电脑", MsgBoxStyle.OkOnly, null);
					}
					catch
					{
					}
					try
					{
						this.Button1.Enabled = true;
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003DF8 File Offset: 0x00001FF8
		/// <summary>
		/// 恢复浏览器下载策略：把 Edge / Chrome / Firefox / IE 的下载、另存为、开发者工具共 12 条策略
		/// 全部改回「允许」，然后依次杀掉 chrome / msedge / firefox / iexplorer 进程使策略立即生效。
		/// </summary>
		/// <remarks>
		/// 12 条写入全部是 RegistryValueKind.DWord，顺序与根键（HKCU / HKLM）为原版顺序，不可调换。
		/// 本方法原版没有 try/catch：任一次写入失败都会把异常抛给调用方（在 退出按钮_Click 里被吞掉），这里保持一致。
		/// 最后一个进程名原文就是 "iexplorer"（少一个 e），杀不到 IE，属原版拼写，不要「修正」。
		/// </remarks>
		private void 恢复浏览器下载策略()
		{
			SetterMapper.PrintComposer(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 0, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 0, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 0, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 0, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 1, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 0, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 1, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 0, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 0, RegistryValueKind.DWord);
			SetterMapper.PrintComposer(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord);
			Process[] chrome进程 = Process.GetProcessesByName("chrome");
			checked
			{
				for (int i = 0; i < chrome进程.Length; i++)
				{
					chrome进程[i].Kill();
				}
				Process[] msedge进程 = Process.GetProcessesByName("msedge");
				for (int j = 0; j < msedge进程.Length; j++)
				{
					msedge进程[j].Kill();
				}
				Process[] firefox进程 = Process.GetProcessesByName("firefox");
				for (int k = 0; k < firefox进程.Length; k++)
				{
					firefox进程[k].Kill();
				}
				Process[] ie进程 = Process.GetProcessesByName("iexplorer");
				for (int l = 0; l < ie进程.Length; l++)
				{
					ie进程[l].Kill();
				}
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003FE0 File Offset: 0x000021E0
		/// <summary>
		/// 计算本机编号：取环境变量 COMPUTERNAME 的末字符 ASCII 码；取到空串时返回 88。
		/// </summary>
		/// <remarks>
		/// 环境变量取不到（返回 null）会直接空引用，原版同样如此，未做保护。
		/// 本程序集内没有调用点。
		/// </remarks>
		public int pc_num()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("COMPUTERNAME");
			int 机器编号;
			if (environmentVariable.Length <= 0)
			{
				机器编号 = 88;
			}
			else
			{
				机器编号 = Strings.Asc(environmentVariable[checked(environmentVariable.Length - 1)]);
			}
			return 机器编号;
		}

		// Token: 0x0400001C RID: 28
		/// <summary>控件容器：承载窗体组件的 IContainer，Dispose 时一并释放。</summary>
		private IContainer 控件容器;

		// Token: 0x0400001D RID: 29
		/// <summary>提示标签（Label1）的后备字段。</summary>
		[AccessedThroughProperty("Label1")]
		[CompilerGenerated]
		private Label 提示标签;

		// Token: 0x0400001E RID: 30
		/// <summary>密码标题标签（Label3）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label3")]
		private Label 密码标题标签;

		// Token: 0x0400001F RID: 31
		/// <summary>密码输入框（TextBox1）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("TextBox1")]
		private TextBox 密码输入框;

		// Token: 0x04000020 RID: 32
		/// <summary>退出按钮（Button1）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Button1")]
		private Button 退出按钮;

		// Token: 0x04000021 RID: 33
		/// <summary>说明标签（Label2）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label2")]
		private Label 说明标签;
	}
}
