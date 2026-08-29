using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using about.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace WindowsApplication1
{
	// Token: 0x02000008 RID: 8
	[DesignerGenerated]
	public partial class Form1 : Form
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002890 File Offset: 0x00000A90
		/// <summary>
		/// 主窗体构造函数。挂接 Load 事件（窗体加载时生成随机监控进程名）并执行控件初始化。
		/// </summary>
		public Form1()
		{
			base.Load += this.GenerateRandomMonitorProcessName;
			this.InitializeComponent();
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000028FC File Offset: 0x00000AFC
		/// <summary>
		/// 手工版 Designer 控件初始化（VB.NET DesignerGenerated 产物，原名 StartReponse）。
		/// 创建 MenuStrip（系统功能F）、四个子菜单、500ms 定时器 Timer1、状态标签与两个超链接，
		/// 并设置标题「学生机房管理助手v13.1」。控件事件通过各属性 setter 挂接。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Form1));
			this.MenuStrip1 = new MenuStrip();
			this.系统功能FToolStripMenuItem = new ToolStripMenuItem();
			this.设置ToolStripMenuItem = new ToolStripMenuItem();
			this.退出程序ToolStripMenuItem = new ToolStripMenuItem();
			this.启动小助手ToolStripMenuItem = new ToolStripMenuItem();
			this.访问官网ToolStripMenuItem = new ToolStripMenuItem();
			this.Timer1 = new Timer(this.components);
			this.Label2 = new Label();
			this.Label1 = new Label();
			this.LinkLabel1 = new LinkLabel();
			this.LinkLabel2 = new LinkLabel();
			this.MenuStrip1.SuspendLayout();
			this.SuspendLayout();
			this.MenuStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.系统功能FToolStripMenuItem
			});
			Control menuStrip = this.MenuStrip1;
			Point location = new Point(0, 0);
			menuStrip.Location = location;
			this.MenuStrip1.Name = "MenuStrip1";
			Control menuStrip2 = this.MenuStrip1;
			Size size = new Size(682, 25);
			menuStrip2.Size = size;
			this.MenuStrip1.TabIndex = 0;
			this.MenuStrip1.Text = "MenuStrip1";
			this.系统功能FToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.设置ToolStripMenuItem,
				this.退出程序ToolStripMenuItem,
				this.启动小助手ToolStripMenuItem,
				this.访问官网ToolStripMenuItem
			});
			this.系统功能FToolStripMenuItem.Name = "系统功能FToolStripMenuItem";
			ToolStripItem 系统功能FToolStripMenuItem = this.系统功能FToolStripMenuItem;
			size = new Size(98, 21);
			系统功能FToolStripMenuItem.Size = size;
			this.系统功能FToolStripMenuItem.Text = "系统功能（F）";
			this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
			ToolStripItem 设置ToolStripMenuItem = this.设置ToolStripMenuItem;
			size = new Size(124, 22);
			设置ToolStripMenuItem.Size = size;
			this.设置ToolStripMenuItem.Text = "设  置";
			this.退出程序ToolStripMenuItem.Name = "退出程序ToolStripMenuItem";
			ToolStripItem 退出程序ToolStripMenuItem = this.退出程序ToolStripMenuItem;
			size = new Size(124, 22);
			退出程序ToolStripMenuItem.Size = size;
			this.退出程序ToolStripMenuItem.Text = "退出程序";
			this.启动小助手ToolStripMenuItem.Name = "启动小助手ToolStripMenuItem";
			ToolStripItem 启动小助手ToolStripMenuItem = this.启动小助手ToolStripMenuItem;
			size = new Size(124, 22);
			启动小助手ToolStripMenuItem.Size = size;
			this.启动小助手ToolStripMenuItem.Text = "启动程序";
			this.访问官网ToolStripMenuItem.Name = "访问官网ToolStripMenuItem";
			ToolStripItem 访问官网ToolStripMenuItem = this.访问官网ToolStripMenuItem;
			size = new Size(124, 22);
			访问官网ToolStripMenuItem.Size = size;
			this.访问官网ToolStripMenuItem.Text = "访问官网";
			this.Timer1.Enabled = true;
			this.Timer1.Interval = 500;
			this.Label2.AutoSize = true;
			this.Label2.BackColor = Color.Transparent;
			this.Label2.ForeColor = Color.Red;
			Control label = this.Label2;
			location = new Point(157, 413);
			label.Location = location;
			this.Label2.Name = "Label2";
			Control label2 = this.Label2;
			size = new Size(41, 12);
			label2.Size = size;
			this.Label2.TabIndex = 4;
			this.Label2.Text = "Label2";
			this.Label1.AutoSize = true;
			this.Label1.BackColor = Color.Transparent;
			this.Label1.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label1.ForeColor = Color.Blue;
			Control label3 = this.Label1;
			location = new Point(39, 412);
			label3.Location = location;
			this.Label1.Name = "Label1";
			Control label4 = this.Label1;
			size = new Size(125, 12);
			label4.Size = size;
			this.Label1.TabIndex = 3;
			this.Label1.Text = "当前小助手工作状态：";
			this.LinkLabel1.AutoSize = true;
			this.LinkLabel1.BackColor = Color.Transparent;
			Control linkLabel = this.LinkLabel1;
			location = new Point(538, 309);
			linkLabel.Location = location;
			this.LinkLabel1.Name = "LinkLabel1";
			Control linkLabel2 = this.LinkLabel1;
			size = new Size(71, 12);
			linkLabel2.Size = size;
			this.LinkLabel1.TabIndex = 5;
			this.LinkLabel1.TabStop = true;
			this.LinkLabel1.Text = ">>>访问官网";
			this.LinkLabel2.AutoSize = true;
			this.LinkLabel2.BackColor = Color.Transparent;
			Control linkLabel3 = this.LinkLabel2;
			location = new Point(268, 412);
			linkLabel3.Location = location;
			this.LinkLabel2.Name = "LinkLabel2";
			Control linkLabel4 = this.LinkLabel2;
			size = new Size(53, 12);
			linkLabel4.Size = size;
			this.LinkLabel2.TabIndex = 6;
			this.LinkLabel2.TabStop = true;
			this.LinkLabel2.Text = "查看帮助";
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			this.BackgroundImage = (Image)componentResourceManager.GetObject("$this.BackgroundImage");
			size = new Size(682, 486);
			this.ClientSize = size;
			this.Controls.Add(this.LinkLabel2);
			this.Controls.Add(this.LinkLabel1);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.MenuStrip1);
			this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			this.MainMenuStrip = this.MenuStrip1;
			this.Name = "Form1";
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "学生机房管理助手v13.1";
			this.MenuStrip1.ResumeLayout(false);
			this.MenuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002EE8 File Offset: 0x000010E8
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002EFC File Offset: 0x000010FC
		/// <summary>
		/// MenuStrip1 主菜单（内含「系统功能（F）」下拉）。setter 挂接 ItemClicked → OnMenuStripItemClicked。
		/// 后备字段 _menuStrip1。
		/// </summary>
		internal virtual MenuStrip MenuStrip1
		{
			get
			{
				return this._menuStrip1;
			}
			set
			{
				ToolStripItemClickedEventHandler value2 = new ToolStripItemClickedEventHandler(this.OnMenuStripItemClicked);
				if (this._menuStrip1 != null)
				{
					this._menuStrip1.ItemClicked -= value2;
				}
				this._menuStrip1 = value;
				if (this._menuStrip1 != null)
				{
					this._menuStrip1.ItemClicked += value2;
				}
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002F48 File Offset: 0x00001148
		// (set) Token: 0x06000030 RID: 48 RVA: 0x000021DC File Offset: 0x000003DC
		/// <summary>
		/// 「系统功能（F）」顶级菜单项。后备字段 _systemFunctionMenuItem。
		/// </summary>
		internal virtual ToolStripMenuItem 系统功能FToolStripMenuItem
		{
			get
			{
				return this._systemFunctionMenuItem;
			}
			set
			{
				this._systemFunctionMenuItem = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002F5C File Offset: 0x0000115C
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002F70 File Offset: 0x00001170
		/// <summary>
		/// 「设 置」菜单项。setter 挂接 Click → OnSettingsMenuClick（启动 set.exe 1）。
		/// 后备字段 _settingsMenuItem。
		/// </summary>
		internal virtual ToolStripMenuItem 设置ToolStripMenuItem
		{
			get
			{
				return this._settingsMenuItem;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.OnSettingsMenuClick);
				if (this._settingsMenuItem != null)
				{
					this._settingsMenuItem.Click -= value2;
				}
				this._settingsMenuItem = value;
				if (this._settingsMenuItem != null)
				{
					this._settingsMenuItem.Click += value2;
				}
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002FBC File Offset: 0x000011BC
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002FD0 File Offset: 0x000011D0
		/// <summary>
		/// 「退出程序」菜单项。setter 挂接 Click → OnExitProgramMenuClick（注册表密码判断）。
		/// 后备字段 _exitProgramMenuItem。
		/// </summary>
		internal virtual ToolStripMenuItem 退出程序ToolStripMenuItem
		{
			get
			{
				return this._exitProgramMenuItem;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.OnExitProgramMenuClick);
				if (this._exitProgramMenuItem != null)
				{
					this._exitProgramMenuItem.Click -= value2;
				}
				this._exitProgramMenuItem = value;
				if (this._exitProgramMenuItem != null)
				{
					this._exitProgramMenuItem.Click += value2;
				}
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000035 RID: 53 RVA: 0x0000301C File Offset: 0x0000121C
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00003030 File Offset: 0x00001230
		/// <summary>
		/// 「访问官网」菜单项。setter 挂接 Click → OnVisitWebsiteMenuClick（打开官网）。
		/// 后备字段 _visitWebsiteMenuItem。
		/// </summary>
		internal virtual ToolStripMenuItem 访问官网ToolStripMenuItem
		{
			get
			{
				return this._visitWebsiteMenuItem;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.OnVisitWebsiteMenuClick);
				if (this._visitWebsiteMenuItem != null)
				{
					this._visitWebsiteMenuItem.Click -= value2;
				}
				this._visitWebsiteMenuItem = value;
				if (this._visitWebsiteMenuItem != null)
				{
					this._visitWebsiteMenuItem.Click += value2;
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000037 RID: 55 RVA: 0x0000307C File Offset: 0x0000127C
		// (set) Token: 0x06000038 RID: 56 RVA: 0x00003090 File Offset: 0x00001290
		/// <summary>
		/// Timer1 状态检测定时器（500ms）。setter 挂接 Tick → OnStatusTimerTick（更新监控状态标签）。
		/// 后备字段 _timer1。
		/// </summary>
		internal virtual Timer Timer1
		{
			get
			{
				return this._timer1;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.OnStatusTimerTick);
				if (this._timer1 != null)
				{
					this._timer1.Tick -= value2;
				}
				this._timer1 = value;
				if (this._timer1 != null)
				{
					this._timer1.Tick += value2;
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000030DC File Offset: 0x000012DC
		// (set) Token: 0x0600003A RID: 58 RVA: 0x000021E5 File Offset: 0x000003E5
		/// <summary>
		/// Label2 状态标签（显示「正在运行中。。。」/「关 闭」）。后备字段 _label2。
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

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600003B RID: 59 RVA: 0x000030F0 File Offset: 0x000012F0
		// (set) Token: 0x0600003C RID: 60 RVA: 0x000021EE File Offset: 0x000003EE
		/// <summary>
		/// Label1 说明标签（「当前小助手工作状态：」）。后备字段 _label1。
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00003104 File Offset: 0x00001304
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00003118 File Offset: 0x00001318
		/// <summary>
		/// LinkLabel1「>>>访问官网」超链接。setter 挂接 LinkClicked → OnVisitWebsiteLinkClick。
		/// 后备字段 _linkLabel1。
		/// </summary>
		internal virtual LinkLabel LinkLabel1
		{
			get
			{
				return this._linkLabel1;
			}
			set
			{
				LinkLabelLinkClickedEventHandler value2 = new LinkLabelLinkClickedEventHandler(this.OnVisitWebsiteLinkClick);
				if (this._linkLabel1 != null)
				{
					this._linkLabel1.LinkClicked -= value2;
				}
				this._linkLabel1 = value;
				if (this._linkLabel1 != null)
				{
					this._linkLabel1.LinkClicked += value2;
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00003164 File Offset: 0x00001364
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00003178 File Offset: 0x00001378
		/// <summary>
		/// LinkLabel2「查看帮助」超链接。setter 挂接 LinkClicked → OnHelpLinkClick（打开帮助页）。
		/// 后备字段 _linkLabel2。
		/// </summary>
		internal virtual LinkLabel LinkLabel2
		{
			get
			{
				return this._linkLabel2;
			}
			set
			{
				LinkLabelLinkClickedEventHandler value2 = new LinkLabelLinkClickedEventHandler(this.OnHelpLinkClick);
				if (this._linkLabel2 != null)
				{
					this._linkLabel2.LinkClicked -= value2;
				}
				this._linkLabel2 = value;
				if (this._linkLabel2 != null)
				{
					this._linkLabel2.LinkClicked += value2;
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000031C4 File Offset: 0x000013C4
		// (set) Token: 0x06000042 RID: 66 RVA: 0x000031D8 File Offset: 0x000013D8
		/// <summary>
		/// 「启动程序」菜单项。setter 挂接 Click → OnStartProgramMenuClick（启动监控进程并重启 explorer）。
		/// 后备字段 _startAssistantMenuItem。
		/// </summary>
		internal virtual ToolStripMenuItem 启动小助手ToolStripMenuItem
		{
			get
			{
				return this._startAssistantMenuItem;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.OnStartProgramMenuClick);
				if (this._startAssistantMenuItem != null)
				{
					this._startAssistantMenuItem.Click -= value2;
				}
				this._startAssistantMenuItem = value;
				if (this._startAssistantMenuItem != null)
				{
					this._startAssistantMenuItem.Click += value2;
				}
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003224 File Offset: 0x00001424
		/// <summary>
		/// 菜单「退出程序」点击处理（原名 SelectReponse）。
		/// 读取注册表 HKCU\Software\n 中的密码哈希：为空则启动 set.exe（说明尚未完成配置），
		/// 否则显示需密码的退出窗体 Form3（WrapperProduct.MyForms.Form3 懒加载单例）。
		/// </summary>
		private void OnExitProgramMenuClick(object sender, EventArgs e)
		{
			string left = Conversions.ToString(PropertyProduct.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
			if (Operators.CompareString(left, "", false) == 0)
			{
				Process.Start(Application.StartupPath + "\\set.exe");
			}
			else
			{
				WrapperProduct.MyForms.Form3.Show();
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000021F7 File Offset: 0x000003F7
		/// <summary>
		/// 菜单「设置」点击处理（原名 FlushReponse）：启动 StartupPath\set.exe 并传参数 "1"。
		/// </summary>
		private void OnSettingsMenuClick(object sender, EventArgs e)
		{
			Process.Start(Application.StartupPath + "\\set.exe", Conversions.ToString(1));
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002214 File Offset: 0x00000414
		/// <summary>
		/// 菜单「访问官网」点击处理（原名 FindReponse）：用默认浏览器打开 http://www.jfglzs.com/。
		/// </summary>
		private void OnVisitWebsiteMenuClick(object sender, EventArgs e)
		{
			Process.Start("http://www.jfglzs.com/");
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003288 File Offset: 0x00001488
		/// <summary>
		/// 检测指定名称的进程是否在运行（原名 ViewReponse）：Process.GetProcessesByName 数量大于 0。
		/// </summary>
		private bool IsProcessRunning(string string_0)
		{
			return Process.GetProcessesByName(string_0).Length > 0;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000032A8 File Offset: 0x000014A8
		/// <summary>
		/// Timer1 定时器 Tick（500ms，原名 RemoveReponse）：实时检测监控进程 jfglzsn 是否在运行，
		/// 更新 Label2 状态（「正在运行中。。。」/「关 闭」）。
		/// </summary>
		private void OnStatusTimerTick(object sender, EventArgs e)
		{
			if (this.IsProcessRunning("jfglzsn"))
			{
				this.Label2.Text = "正在运行中。。。";
			}
			else
			{
				this.Label2.Text = "关 闭";
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000032E4 File Offset: 0x000014E4
		/// <summary>
		/// 菜单「启动程序」点击处理（原名 InstantiateReponse）。
		/// 真实逻辑：
		/// 1. 读注册表 HKCU\Software\n（密码哈希）：为空则启动 set.exe（首次部署），否则依次启动 przs.exe 与 jfglzsn.exe（监控主程序）；
		/// 2. 遍历所有 explorer 进程并 Kill()；
		/// 3. 重新启动 explorer.exe（让桌面在监控策略下重建）。
		/// [反混淆] 原 IL 状态机（num2 = 2→3→4/6→7→8→10→11→13，约100行）已还原为正常 if/foreach 控制流。
		/// </summary>
		private void OnStartProgramMenuClick(object sender, EventArgs e)
		{
			// [反混淆] 原状态机约100行 → 还原为正常控制流
			try
			{
				// 1. 读注册表 HKCU\Software\n（密码哈希）：为空 → 首次部署，启动 set.exe
				string registryPassword = Conversions.ToString(PropertyProduct.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
				if (Operators.CompareString(registryPassword, "", false) == 0)
				{
					Process.Start(Application.StartupPath + "\\set.exe");
				}
				else
				{
					// 2. 已配置 → 依次启动监控主程序 przs.exe 与 jfglzsn.exe
					Process.Start(Application.StartupPath + "\\przs.exe");
					Process.Start(Application.StartupPath + "\\jfglzsn.exe");
				}
				// 3. 遍历所有 explorer 进程并 Kill（强刷桌面以应用监控策略）
				Process[] explorerProcesses = Process.GetProcessesByName("explorer");
				foreach (Process explorerProcess in explorerProcesses)
				{
					explorerProcess.Kill();
				}
				// 4. 重新启动 explorer.exe
				Process.Start("explorer.exe");
			}
			catch (Exception ex)
			{
				// [反混淆] 原混淆样板：VB On Error 错误处理（原结构实为重抛 ProjectData.CreateProjectError(-2146828237)），
				// 对实际流程无影响，还原为吞掉异常。
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002221 File Offset: 0x00000421
		/// <summary>
		/// LinkLabel1「>>>访问官网」点击（原名 CheckReponse）：打开 http://www.jfglzs.com。
		/// </summary>
		private void OnVisitWebsiteLinkClick(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.jfglzs.com");
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000222E File Offset: 0x0000042E
		/// <summary>
		/// MenuStrip ItemClicked 事件（原名 ListReponse）：空实现占位。
		/// </summary>
		private void OnMenuStripItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002230 File Offset: 0x00000430
		/// <summary>
		/// 启动「更新器.exe」（原名 CompareReponse）：未被 Designer 挂接的遗留方法，暗示程序可自我更新。
		/// </summary>
		private void LaunchUpdater(object sender, EventArgs e)
		{
			Process.Start(Application.StartupPath + "\\更新器.exe");
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002247 File Offset: 0x00000447
		/// <summary>
		/// LinkLabel2「查看帮助」点击（原名 DestroyReponse）：打开帮助页 http://www.jfglzs.com/c1.html。
		/// </summary>
		private void OnHelpLinkClick(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.jfglzs.com/c1.html");
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003478 File Offset: 0x00001678
		/// <summary>
		/// 窗体加载时生成随机监控进程名（原名 RateReponse，挂在 base.Load 上）。
		/// 流程：
		/// 1. num = 月×日；取 num%7/%9/%5/%3 得 4 个偏移，奇偶两种顺序用 chr(97/98/101/99+偏移) 拼 4 字符名；
		/// 2. 先写 PropertyProduct.MonitorProcessName / MonitorProcessNameWithExe；
		/// 3. 再以 num 为种子 Randomize，生成 1~300000 随机数，逐位转 chr(105+digit) 拼 5 字符名，再次覆盖（最终生效）；
		/// 效果：监控进程名每天/每次启动不同，对抗按固定进程名查杀。
		/// </summary>
		private void GenerateRandomMonitorProcessName(object sender, EventArgs e)
		{
			checked
			{
				int num = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
				int num2 = num % 7;
				int num3 = num % 9;
				int num4 = num % 5;
				int num5 = num % 3;
				string text;
				if (num % 2 == 0)
				{
					text = Conversions.ToString(Strings.Chr(97 + num2)) + Conversions.ToString(Strings.Chr(98 + num3)) + Conversions.ToString(Strings.Chr(101 + num4)) + Conversions.ToString(Strings.Chr(99 + num5));
				}
				else
				{
					text = Conversions.ToString(Strings.Chr(97 + num3)) + Conversions.ToString(Strings.Chr(98 + num2)) + Conversions.ToString(Strings.Chr(101 + num5)) + Conversions.ToString(Strings.Chr(99 + num4));
				}
				PropertyProduct.MonitorProcessName = text;
				PropertyProduct.MonitorProcessNameWithExe = text + ".exe";
				VBMath.Randomize((double)num);
				long num6 = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
				string text2 = "";
				int num7 = 1;
				do
				{
					long num8 = num6 % 10L + 105L;
					text2 = Conversions.ToString(Strings.Chr((int)num8)) + text2;
					num6 /= 10L;
					num7++;
				}
				while (num7 <= 5);
				PropertyProduct.MonitorProcessName = text2;
				PropertyProduct.MonitorProcessNameWithExe = text2 + ".exe";
			}
		}

		// Token: 0x0400000B RID: 11
		/// <summary>MenuStrip1 后备字段（原名 _ExpressionProduct）。</summary>
		[AccessedThroughProperty("MenuStrip1")]
		private MenuStrip _menuStrip1;

		// Token: 0x0400000C RID: 12
		/// <summary>「系统功能（F）」菜单项后备字段（原名 orderProduct）。</summary>
		[AccessedThroughProperty("系统功能FToolStripMenuItem")]
		private ToolStripMenuItem _systemFunctionMenuItem;

		// Token: 0x0400000D RID: 13
		/// <summary>「设 置」菜单项后备字段（原名 _CodeProduct）。</summary>
		[AccessedThroughProperty("设置ToolStripMenuItem")]
		private ToolStripMenuItem _settingsMenuItem;

		// Token: 0x0400000E RID: 14
		/// <summary>「退出程序」菜单项后备字段（原名 _ServerProduct）。</summary>
		[AccessedThroughProperty("退出程序ToolStripMenuItem")]
		private ToolStripMenuItem _exitProgramMenuItem;

		// Token: 0x0400000F RID: 15
		/// <summary>「访问官网」菜单项后备字段（原名 m_PoolProduct）。</summary>
		[AccessedThroughProperty("访问官网ToolStripMenuItem")]
		private ToolStripMenuItem _visitWebsiteMenuItem;

		// Token: 0x04000010 RID: 16
		/// <summary>Timer1（500ms 状态检测）后备字段（原名 m_BridgeProduct）。</summary>
		[AccessedThroughProperty("Timer1")]
		private Timer _timer1;

		// Token: 0x04000011 RID: 17
		/// <summary>Label2（监控状态标签）后备字段（原名 mapProduct）。</summary>
		[AccessedThroughProperty("Label2")]
		private Label _label2;

		// Token: 0x04000012 RID: 18
		/// <summary>Label1（说明标签）后备字段（原名 m_DecoratorProduct）。</summary>
		[AccessedThroughProperty("Label1")]
		private Label _label1;

		// Token: 0x04000013 RID: 19
		/// <summary>LinkLabel1（访问官网超链接）后备字段（原名 _FacadeProduct）。</summary>
		[AccessedThroughProperty("LinkLabel1")]
		private LinkLabel _linkLabel1;

		// Token: 0x04000014 RID: 20
		/// <summary>LinkLabel2（查看帮助超链接）后备字段（原名 tokenizerProduct）。</summary>
		[AccessedThroughProperty("LinkLabel2")]
		private LinkLabel _linkLabel2;

		// Token: 0x04000015 RID: 21
		/// <summary>「启动程序」菜单项后备字段（原名 parameterProduct）。</summary>
		[AccessedThroughProperty("启动小助手ToolStripMenuItem")]
		private ToolStripMenuItem _startAssistantMenuItem;

		// Token: 0x04000016 RID: 22
		/// <summary>未使用的 long 型字段（混淆残留，原名 comparatorProduct），无任何引用。</summary>
		private long comparatorProduct;
	}
}
