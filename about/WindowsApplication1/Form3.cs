using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using about.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace WindowsApplication1
{
	// Token: 0x02000009 RID: 9
	/// <summary>
	/// 「退出程序」密码验证窗体：输入 6 位以上密码与 HKCU\Software\n 中保存的 SHA256(密码+"bfdshgs") 比对，
	/// 正确后执行整套解锁流程（杀监控进程/服务、恢复 USB/浏览器/CMD 策略、重启 explorer）。
	/// 核心逻辑在 OnExitButtonClick / KillMonitorProcessesAndServices / RestoreBrowserPolicies，
	/// 前两者原带控制流扁平化混淆，已[反混淆]还原为正常控制流。
	/// </summary>
	[DesignerGenerated]
	public partial class Form3 : Form
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00002254 File Offset: 0x00000454
		/// <summary>构造函数：仅执行控件初始化（原名 DeleteReponse）。</summary>
		public Form3()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003628 File Offset: 0x00001828
		/// <summary>
		/// 手工版 Designer 控件初始化（原名 DeleteReponse）：创建提示标签、密码输入框（PasswordChar='*'）、
		/// 「退出(需10秒)」按钮与使用说明标签。控件事件通过属性 setter 挂接。
		/// </summary>
		private void InitializeComponent()
		{
			this.Label1 = new Label();
			this.Label3 = new Label();
			this.TextBox1 = new TextBox();
			this.Button1 = new Button();
			this.Label2 = new Label();
			this.SuspendLayout();
			this.Label1.AutoSize = true;
			this.Label1.ForeColor = Color.Red;
			Control label = this.Label1;
			Point location = new Point(56, 112);
			label.Location = location;
			this.Label1.Name = "Label1";
			Control label2 = this.Label1;
			Size size = new Size(0, 12);
			label2.Size = size;
			this.Label1.TabIndex = 19;
			this.Label3.AutoSize = true;
			this.Label3.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label3.ForeColor = Color.DarkRed;
			Control label3 = this.Label3;
			location = new Point(37, 46);
			label3.Location = location;
			this.Label3.Name = "Label3";
			Control label4 = this.Label3;
			size = new Size(119, 14);
			label4.Size = size;
			this.Label3.TabIndex = 18;
			this.Label3.Text = "输入小助手密码：";
			Control textBox = this.TextBox1;
			location = new Point(172, 42);
			textBox.Location = location;
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.PasswordChar = '*';
			Control textBox2 = this.TextBox1;
			size = new Size(104, 21);
			textBox2.Size = size;
			this.TextBox1.TabIndex = 17;
			Control button = this.Button1;
			location = new Point(294, 39);
			button.Location = location;
			this.Button1.Name = "Button1";
			Control button2 = this.Button1;
			size = new Size(96, 27);
			button2.Size = size;
			this.Button1.TabIndex = 16;
			this.Button1.Text = "退出(需10秒)";
			this.Button1.UseVisualStyleBackColor = true;
			this.Label2.AutoSize = true;
			Control label5 = this.Label2;
			location = new Point(38, 87);
			label5.Location = location;
			this.Label2.Name = "Label2";
			Control label6 = this.Label2;
			size = new Size(287, 12);
			label6.Size = size;
			this.Label2.TabIndex = 21;
			this.Label2.Text = "注：用密码退出后可使用U盘、下载文件，停止监控。";
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			size = new Size(417, 181);
			this.ClientSize = size;
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.TextBox1);
			this.Controls.Add(this.Button1);
			this.Name = "Form3";
			this.Text = "退出程序";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00003950 File Offset: 0x00001B50
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00002262 File Offset: 0x00000462
		/// <summary>
		/// Label1 提示信息标签（红色，原名 fieldProduct），显示密码校验结果/错误提示。后备字段 _label1。
		/// </summary>
		internal virtual Label Label1
		{
			get
			{
				return this._label1;
			}
			set
			{
				this._label1 = value;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00003964 File Offset: 0x00001B64
		// (set) Token: 0x06000054 RID: 84 RVA: 0x0000226B File Offset: 0x0000046B
		/// <summary>
		/// Label3「输入小助手密码：」提示标签（原名 m_ParamsProduct）。后备字段 _label3。
		/// </summary>
		internal virtual Label Label3
		{
			get
			{
				return this._label3;
			}
			set
			{
				this._label3 = value;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00003978 File Offset: 0x00001B78
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00002274 File Offset: 0x00000474
		/// <summary>
		/// TextBox1 密码输入框（PasswordChar='*'，原名 connectionProduct）。后备字段 _textBox1。
		/// </summary>
		internal virtual TextBox TextBox1
		{
			get
			{
				return this._textBox1;
			}
			set
			{
				this._textBox1 = value;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000057 RID: 87 RVA: 0x0000398C File Offset: 0x00001B8C
		// (set) Token: 0x06000058 RID: 88 RVA: 0x000039A0 File Offset: 0x00001BA0
		/// <summary>
		/// Button1「退出(需10秒)」按钮（原名 _ParamProduct）。setter 挂接 Click → OnExitButtonClick。
		/// 后备字段 _button1。
		/// </summary>
		internal virtual Button Button1
		{
			get
			{
				return this._button1;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.OnExitButtonClick);
				if (this._button1 != null)
				{
					this._button1.Click -= value2;
				}
				this._button1 = value;
				if (this._button1 != null)
				{
					this._button1.Click += value2;
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000059 RID: 89 RVA: 0x000039EC File Offset: 0x00001BEC
		// (set) Token: 0x0600005A RID: 90 RVA: 0x0000227D File Offset: 0x0000047D
		/// <summary>
		/// Label2 使用说明标签（「注：用密码退出后可使用U盘、下载文件，停止监控。」，原名 ruleProduct）。
		/// 后备字段 _label2。
		/// </summary>
		internal virtual Label Label2
		{
			get
			{
				return this._label2;
			}
			set
			{
				this._label2 = value;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003A00 File Offset: 0x00001C00
		/// <summary>
		/// 「退出(需10秒)」按钮点击处理（原名 InterruptReponse）——密码校验 + 触发解锁流程。
		/// 真实逻辑：
		/// 1. 密码长度 &lt; 6 → 提示「密码应该是6位以上的字母或数字，请重新输入！」并清空输入框；
		/// 2. left = SHA256(Trim(密码) + "bfdshgs")（PropertyProduct.ComputePasswordHash），与 HKCU\Software\n 比对；
		/// 3. 不一致 → 提示「密码不正确，请重新输入！」；一致 → 禁用按钮、清空提示；
		/// 4. 调用 KillMonitorProcessesAndServices()（杀监控）与 RestoreBrowserPolicies()（恢复浏览器策略）；
		/// 5. 将 usbstor 服务（CurrentControlSet/ControlSet001~003）Start 值设为 3（恢复 USB 可用）；
		/// 6. Kill 所有 explorer 并重新启动 explorer.exe（重建桌面）。
		/// [反混淆] 原 IL 状态机（num2 = 2→3→4 / 7→8→9→10→12→13→14…→21，或错误分支 23→24→25，约142行）
		/// 已还原为正常 if/foreach 控制流。
		/// </summary>
		private void OnExitButtonClick(object sender, EventArgs e)
		{
			// [反混淆] 原状态机约142行 → 还原为正常控制流
			try
			{
				// 1. 密码长度 < 6 → 提示并清空输入框，结束
				if (Strings.Len(this.TextBox1.Text) < 6)
				{
					this.Label1.Text = "密码应该是6位以上的字母或数字，请重新输入！";
					this.TextBox1.Text = "";
					return;
				}
				// 2. 计算 SHA256(Trim(密码)+"bfdshgs") 并与注册表 HKCU\Software\n 保存的哈希比对
				string inputHash = PropertyProduct.ComputePasswordHash(Strings.Trim(this.TextBox1.Text));
				string storedHash = Conversions.ToString(PropertyProduct.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
				// 3. 不一致 → 提示「密码不正确」；一致 → 继续解锁流程
				if (Operators.CompareString(inputHash, storedHash, false) != 0)
				{
					this.Label1.Text = "密码不正确，请重新输入！";
					this.TextBox1.Text = "";
					return;
				}
				// 4. 密码正确 → 禁用按钮、清空提示，执行杀监控 + 恢复浏览器策略
				this.Button1.Enabled = false;
				this.Label1.Text = "";
				this.KillMonitorProcessesAndServices();
				this.RestoreBrowserPolicies();
				// 5. 恢复 usbstor 服务为可用（Start = 3）：CurrentControlSet 与 ControlSet001~003 共四处
				PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
				PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
				PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
				PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
				// 6. Kill 所有 explorer 并重新启动 explorer.exe（重建桌面）
				Process[] explorerProcesses = Process.GetProcessesByName("explorer");
				foreach (Process explorerProcess in explorerProcesses)
				{
					explorerProcess.Kill();
				}
				Process.Start("explorer.exe");
			}
			catch (Exception ex)
			{
				// [反混淆] 原混淆样板：VB On Error 错误处理（原结构实为重抛 ProjectData.CreateProjectError(-2146828237)），
				// 对实际流程无影响，还原为吞掉异常。
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003288 File Offset: 0x00001488
		/// <summary>
		/// 检测指定名称的进程是否在运行（原名 MapReponse）：与 Form1.IsProcessRunning 相同实现。
		/// </summary>
		private bool IsProcessRunning(string string_0)
		{
			return Process.GetProcessesByName(string_0).Length > 0;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00003CD4 File Offset: 0x00001ED4
		/// <summary>
		/// 结束监控进程/服务并恢复被禁用的 CMD（原名 CalcReponse）。
		/// 真实逻辑：
		/// 1. 禁用按钮；目标进程：zmserv（服务）、PropertyProduct.MonitorProcessName（随机名进程）、jfglzsn；
		/// 2. 循环 Kill 随机名进程[0] 与 jfglzsn[0]，直到两者都不存在；
		/// 3. num3 = 10*10+2 = 102：遍历所有进程，进程名长度 ≥4 且每个字符 ASCII ∈ [102,111]（'f'~'o'）的进程一律 Kill
		///    （疑似为全数字/特定命名隐藏进程设计的模糊判定，反编译后阈值失真）；
		/// 4. 写 HKCU\Software\Policies\Microsoft\Windows\System 的 DisableCMD = 0（重新启用命令提示符）；
		/// 5. Interaction.Shell("net stop zmserv", Hide) 停服务 → Sleep(3000) → Kill zmserv[0]；
		/// 6. 再次循环 Kill 随机名进程与 jfglzsn 直到消失；再 net stop zmserv、Sleep(2000)；
		/// 7. 两者都不在运行 → MsgBox「退出程序成功！」→ Close()；否则 MsgBox「退出程序不成功！…」并重新启用按钮。
		/// [反混淆] 原 IL 状态机（num2 = 2→3→4→5→6→8→9…→60，约207行）已还原为正常 do-while/for/if 控制流。
		/// </summary>
		private void KillMonitorProcessesAndServices()
		{
			// [反混淆] 原状态机约207行 → 还原为正常控制流
			try
			{
				// 1. 禁用按钮；定义目标：zmserv（服务名）、随机名监控进程、jfglzsn（固定名监控进程）
				this.Button1.Enabled = false;
				string serviceName = "zmserv";
				string randomProcessName = Conversions.ToString(PropertyProduct.MonitorProcessName);
				string monitorProcessName = "jfglzsn";
				// 2. 循环 Kill 随机名进程[0] 与 jfglzsn[0]，直到两者都不存在
				//    （原代码以数组[0].Kill() 直接调用，进程不存在时抛出的 IndexOutOfRange 被 VB 错误处理吞掉，
				//     等价于「存在才 Kill」，此处用 Length>0 守卫等价还原）
				Process[] randomNameProcesses;
				Process[] monitorProcesses;
				do
				{
					randomNameProcesses = Process.GetProcessesByName(randomProcessName);
					if (randomNameProcesses.Length > 0)
					{
						randomNameProcesses[0].Kill();
					}
					monitorProcesses = Process.GetProcessesByName(monitorProcessName);
					if (monitorProcesses.Length > 0)
					{
						monitorProcesses[0].Kill();
					}
				}
				while (randomNameProcesses.Length > 0 || monitorProcesses.Length > 0);
				// 3. num3 = 10*10+2 = 102：遍历所有进程，进程名长度 ≥4 且每个字符 ASCII ∈ [102,111]（'f'~'o'）→ Kill
				Process[] allProcesses = Process.GetProcesses();
				foreach (Process process in allProcesses)
				{
					int nameLength = Strings.Len(process.ProcessName);
					if (nameLength >= 4)
					{
						// 原状态机：For j = 1 To 长度，任字符 <102 或 >111 即 break；循环完整走完（j == 长度+1）才 Kill
						bool allCharsInRange = true;
						for (int j = 1; j <= nameLength; j++)
						{
							int charCode = Strings.Asc(Strings.Mid(process.ProcessName, j, 1));
							if (charCode < 102 || charCode > 111)
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
				// 4. 恢复 CMD：HKCU\Software\Policies\Microsoft\Windows\System 的 DisableCMD = 0
				PropertyProduct.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				// 5. 停止 zmserv 服务 → 等待3秒 → 结束 zmserv 进程（进程不存在时原代码经错误处理自动跳过，此处守卫等价还原）
				Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
				Thread.Sleep(3000);
				Process[] zmservProcesses = Process.GetProcessesByName(serviceName);
				if (zmservProcesses.Length > 0)
				{
					zmservProcesses[0].Kill();
				}
				// 6. 再次循环 Kill 随机名进程与 jfglzsn 直到消失；再 net stop zmserv、等待2秒
				do
				{
					randomNameProcesses = Process.GetProcessesByName(randomProcessName);
					if (randomNameProcesses.Length > 0)
					{
						randomNameProcesses[0].Kill();
					}
					monitorProcesses = Process.GetProcessesByName(monitorProcessName);
					if (monitorProcesses.Length > 0)
					{
						monitorProcesses[0].Kill();
					}
				}
				while (randomNameProcesses.Length > 0 || monitorProcesses.Length > 0);
				Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
				Thread.Sleep(2000);
				// 7. 两个监控进程都不在运行 → 提示成功并关闭窗体；否则提示失败并重新启用按钮
				//    （原条件 !(!A & !B) 等价于「任一在运行 → 失败分支」；& 无短路，两个调用都会执行，此处拆成两个局部变量等价还原）
				bool jfglzsnRunning = this.IsProcessRunning("jfglzsn");
				bool randomNameRunning = this.IsProcessRunning(randomProcessName);
				if (!jfglzsnRunning && !randomNameRunning)
				{
					Interaction.MsgBox("退出程序成功！", MsgBoxStyle.OkOnly, null);
					this.Close();
				}
				else
				{
					Interaction.MsgBox("退出程序不成功！请重新执行退出程序命令或重启电脑", MsgBoxStyle.OkOnly, null);
					this.Button1.Enabled = true;
				}
			}
			catch (Exception ex)
			{
				// [反混淆] 原混淆样板：VB On Error 错误处理（原结构实为重抛 ProjectData.CreateProjectError(-2146828237)），
				// 对实际流程无影响，还原为吞掉异常。
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000040FC File Offset: 0x000022FC
		/// <summary>
		/// 恢复浏览器下载与开发者工具策略（原名 SortReponse），撤销监控期的封锁并杀掉所有浏览器进程：
		/// 1. IE 安全区（Zones\3）：1803=0、2200=0；IE 策略 NoBrowserSaveAs=0；
		/// 2. Edge/Chrome：DownloadRestrictions=0、SaveAs=1、DeveloperToolsAvailability=0；
		/// 3. Firefox：DisableDownloads=0、BlockAboutDownloads=0、DeveloperToolsAvailability=0；
		/// 4. 依次 Kill 全部 chrome / msedge / firefox / iexplorer 进程。
		/// </summary>
		private void RestoreBrowserPolicies()
		{
			PropertyProduct.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 0, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 0, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 0, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 0, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 1, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 0, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 1, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 0, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 0, RegistryValueKind.DWord);
			PropertyProduct.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord);
			Process[] processesByName = Process.GetProcessesByName("chrome");
			foreach (Process process in processesByName)
			{
				process.Kill();
			}
			Process[] processesByName2 = Process.GetProcessesByName("msedge");
			foreach (Process process2 in processesByName2)
			{
				process2.Kill();
			}
			Process[] processesByName3 = Process.GetProcessesByName("firefox");
			foreach (Process process3 in processesByName3)
			{
				process3.Kill();
			}
			Process[] processesByName4 = Process.GetProcessesByName("iexplorer");
			foreach (Process process4 in processesByName4)
			{
				process4.Kill();
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000430C File Offset: 0x0000250C
		/// <summary>
		/// 计算机器码（原名 pc_num）：读环境变量 COMPUTERNAME，为空返回 88，否则返回主机名末字符的 ASCII 码。
		/// 用于授权/密码派生。
		/// </summary>
		public int GetMachineCode()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("COMPUTERNAME");
			int result;
			if (environmentVariable.Length <= 0)
			{
				result = 88;
			}
			else
			{
				char @string = environmentVariable[checked(environmentVariable.Length - 1)];
				result = Strings.Asc(@string);
			}
			return result;
		}

		// Token: 0x04000018 RID: 24
		/// <summary>Label1 提示信息标签后备字段（原名 fieldProduct）。</summary>
		[AccessedThroughProperty("Label1")]
		private Label _label1;

		// Token: 0x04000019 RID: 25
		/// <summary>Label3「输入小助手密码：」标签后备字段（原名 m_ParamsProduct）。</summary>
		[AccessedThroughProperty("Label3")]
		private Label _label3;

		// Token: 0x0400001A RID: 26
		/// <summary>TextBox1 密码输入框后备字段（原名 connectionProduct）。</summary>
		[AccessedThroughProperty("TextBox1")]
		private TextBox _textBox1;

		// Token: 0x0400001B RID: 27
		/// <summary>Button1「退出(需10秒)」按钮后备字段（原名 _ParamProduct）。</summary>
		[AccessedThroughProperty("Button1")]
		private Button _button1;

		// Token: 0x0400001C RID: 28
		/// <summary>Label2 使用说明标签后备字段（原名 ruleProduct）。</summary>
		[AccessedThroughProperty("Label2")]
		private Label _label2;
	}
}
