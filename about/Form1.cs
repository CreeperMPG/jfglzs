using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using WindowsApplication1.Internal;

namespace WindowsApplication1
{
	/// <summary>
	/// 主窗体「学生机房管理助手v13.4」：程序的前台控制面板。
	/// 只做四件事——Load 时算出监控端进程名、每 500ms 轮询监控端是否在跑、
	/// 「启动程序」拉起监控端并重启资源管理器、「退出程序」按有无密码分流到 Form3 或 set.exe。
	/// </summary>
	/// <remarks>
	/// 敏感行为（本文件内）：
	/// 1) 枚举并逐个 Kill 名为 explorer 的进程，再重启 explorer.exe（强制刷新桌面）；
	/// 2) 从自身目录拉起 set.exe / przs.exe / jfglzsn.exe、更新器.exe（死代码）；
	/// 3) 读 HKCU\Software 值名 n（退出密码的 SHA256）决定走「免密」还是「要密码」分支；
	/// 4) Load 时以「月×日」为种子生成监控端进程名，使进程名每天变化以规避按名查杀。
	/// 本文件只读注册表，不写入。
	/// </remarks>
	// Token: 0x0200000B RID: 11
	[DesignerGenerated]
	public class Form1 : Form
	{
		// Token: 0x06000033 RID: 51 RVA: 0x00002838 File Offset: 0x00000A38
		/// <summary>
		/// 构造函数：先挂 Load 处理器，再调用初始化控件（顺序为原版顺序，不可交换）。
		/// </summary>
		public Form1()
		{
			base.Load += this.窗体加载_生成助手进程名;
			this.初始化控件();
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002864 File Offset: 0x00000A64
		/// <summary>
		/// 「退出程序」菜单：读 HKCU\Software 值名 n。为空（从未设过密码）时拉起同目录 set.exe 后返回；
		/// 否则弹出 Form3 要求输入密码——也就是说点「退出程序」并不会真的结束本程序。
		/// </summary>
		private void 退出程序_Click(object sender, EventArgs e)
		{
			if (Operators.CompareString(Conversions.ToString(SetterMapper.DisableComposer(Registry.CurrentUser, "Software", "n", "")), "", false) == 0)
			{
				Process.Start(Application.StartupPath + "\\set.exe");
				return;
			}
			ExpressionMapper.VisitComposer.Form3.Show();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000235F File Offset: 0x0000055F
		/// <summary>
		/// 「设  置」菜单：带命令行参数 1 拉起同目录 set.exe（进入设置/改密码界面）。
		/// </summary>
		private void 设置_Click(object sender, EventArgs e)
		{
			Process.Start(Application.StartupPath + "\\set.exe", Conversions.ToString(1));
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000237C File Offset: 0x0000057C
		/// <summary>
		/// 「访问官网」菜单：用默认浏览器打开官网首页。
		/// </summary>
		private void 访问官网_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.jfglzs.com/");
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000028C4 File Offset: 0x00000AC4
		/// <summary>
		/// 按进程名（不是文件名）判断进程是否存在。
		/// </summary>
		/// <param name="进程名">不带 .exe 后缀的进程名</param>
		/// <returns>存在至少一个同名进程返回 true</returns>
		private bool 进程是否在运行(string 进程名)
		{
			return Process.GetProcessesByName(进程名).Length > 0;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000028E4 File Offset: 0x00000AE4
		/// <summary>
		/// 定时器 Tick（500ms）：查监控端进程 jfglzsn 是否在运行，刷新状态标签的文本。
		/// </summary>
		private void 定时检查小助手状态(object sender, EventArgs e)
		{
			if (!this.进程是否在运行("jfglzsn"))
			{
				this.Label2.Text = "关 闭";
				return;
			}
			this.Label2.Text = "正在运行中。。。";
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002920 File Offset: 0x00000B20
		/// <summary>
		/// 「启动程序」菜单：按有无密码决定启动谁——无密码只拉起 set.exe；有密码则先拉起 przs.exe 与监控端
		/// jfglzsn.exe。之后无条件结束全部 explorer 进程并重新启动 explorer.exe（桌面会闪断一次）。
		/// </summary>
		/// <remarks>
		/// 原版是 VB 的 <c>On Error Resume Next</c>：<c>ProjectData.ClearProjectError()</c> 开场，<c>num2 = 2..11</c>
		/// 是编译器给每条语句编的状态号，方法末尾的 <c>catch when (endfilter(...))</c> 按状态号分发回原位置。
		/// 三处跳转的原始含义：IL_0052 = 跳过 set.exe 分支进入「有密码」分支、IL_0080 = 两分支汇合到 explorer 段、
		/// IL_0142 = 正常结束。这里还原成 if/else + 每条语句各自 try/catch——
		/// 与「逐条语句出错即丢弃、继续下一条」的 VB 语义一致；一条 Process.Start 失败不影响后续步骤。
		/// Kill 的 try 必须写在 foreach 内层：若套在外层，一次 Kill 失败会连整个枚举一起中断，与原版逐次忽略不符。
		/// </remarks>
		private void 启动小助手_Click(object sender, EventArgs e)
		{
			string 密码哈希 = "";
			try
			{
				密码哈希 = Conversions.ToString(SetterMapper.DisableComposer(Registry.CurrentUser, "Software", "n", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(密码哈希, "", false) == 0)
			{
				try
				{
					Process.Start(Application.StartupPath + "\\set.exe");
				}
				catch
				{
				}
			}
			else
			{
				try
				{
					Process.Start(Application.StartupPath + "\\przs.exe");
				}
				catch
				{
				}
				try
				{
					Process.Start(Application.StartupPath + "\\jfglzsn.exe");
				}
				catch
				{
				}
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

		// Token: 0x0600003A RID: 58 RVA: 0x00002389 File Offset: 0x00000589
		/// <summary>
		/// 「&gt;&gt;&gt;访问官网」链接：打开官网（此处写法不带结尾斜杠，与原版一致）。
		/// </summary>
		private void 官网链接_Click(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.jfglzs.com");
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002396 File Offset: 0x00000596
		/// <summary>
		/// MenuStrip.ItemClicked 的空处理器：设计器默认挂上，方法体什么都没有。
		/// </summary>
		private void 主菜单项点击_空实现(object sender, ToolStripItemClickedEventArgs e)
		{
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002398 File Offset: 0x00000598
		/// <summary>
		/// 拉起同目录的 更新器.exe。<b>未绑定任何事件</b>（无引用），属于原版残留的死代码。
		/// </summary>
		private void 启动更新器_未绑定(object sender, EventArgs e)
		{
			Process.Start(Application.StartupPath + "\\更新器.exe");
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000023AF File Offset: 0x000005AF
		/// <summary>
		/// 「查看帮助」链接：打开官网帮助页 c1.html（纯浏览器跳转，程序自身不联网）。
		/// </summary>
		private void 查看帮助_Click(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.jfglzs.com/c1.html");
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002A94 File Offset: 0x00000C94
		/// <summary>
		/// 窗体 Load 处理器：用「月×日」算出监控端的进程名与可执行文件名，写进 SetterMapper 的静态字段
		/// （SetterMapper._ValMapper = 进程名，SetterMapper.m_WatcherMapper = 进程名 + ".exe"）。
		/// </summary>
		/// <remarks>
		/// 分两段，第二段覆盖第一段：
		/// 1) 月日乘积 = 月 × 日；取 %7 / %9 / %5 / %3 四个余数，按「月日乘积 % 2」选两套排列，
		///    用 Strings.Chr(97/98/101/99 + 余数) 拼出 4 字母名，先写一次静态字段；
		/// 2) 以「月日乘积」为种子 VBMath.Randomize，取 Rnd()，round(Rnd()*100000*3+1) 得随机数，
		///    再循环 5 次取 Chr(随机数 % 10 + 105)——即 i..r 区间内 5 个字符，覆盖第 1 段写下的名字。
		/// 第 1 段的 4 字母名会被立刻覆盖，属原版残留逻辑，但两次赋值都是真实副作用，故完整保留。
		/// 种子就是「月×日」，因此进程名每天固定、完全可预测；目的只是让进程名不再写死，规避按名字查杀。
		/// 整段处于 checked 上下文（VB 默认 checked，溢出会抛异常），外层 checked 保留。
		/// </remarks>
		private void 窗体加载_生成助手进程名(object sender, EventArgs e)
		{
			checked
			{
				int 月日乘积 = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
				int 余数7 = 月日乘积 % 7;
				int 余数9 = 月日乘积 % 9;
				int 余数5 = 月日乘积 % 5;
				int 余数3 = 月日乘积 % 3;
				string 日期名;
				if (月日乘积 % 2 == 0)
				{
					日期名 = Conversions.ToString(Strings.Chr(97 + 余数7)) + Conversions.ToString(Strings.Chr(98 + 余数9)) + Conversions.ToString(Strings.Chr(101 + 余数5)) + Conversions.ToString(Strings.Chr(99 + 余数3));
				}
				else
				{
					日期名 = Conversions.ToString(Strings.Chr(97 + 余数9)) + Conversions.ToString(Strings.Chr(98 + 余数7)) + Conversions.ToString(Strings.Chr(101 + 余数3)) + Conversions.ToString(Strings.Chr(99 + 余数5));
				}
				SetterMapper._ValMapper = 日期名;
				SetterMapper.m_WatcherMapper = 日期名 + ".exe";
				VBMath.Randomize((double)月日乘积);
				long 随机数 = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
				string 数字名 = "";
				int 位数 = 1;
				do
				{
					数字名 = Conversions.ToString(Strings.Chr((int)(随机数 % 10L + 105L))) + 数字名;
					随机数 /= 10L;
					位数++;
				}
				while (位数 <= 5);
				SetterMapper._ValMapper = 数字名;
				SetterMapper.m_WatcherMapper = 数字名 + ".exe";
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002BFC File Offset: 0x00000DFC
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

		// Token: 0x06000040 RID: 64 RVA: 0x00002C3C File Offset: 0x00000E3C
		/// <summary>
		/// VB 的 InitializeComponent：逐个创建菜单、标签、链接、定时器并赋属性值（顺序为原版顺序）。
		/// 窗口标题「学生机房管理助手v13.4」、ClientSize 682×486、居中启动，背景图与图标取自 Form1.resources。
		/// </summary>
		private void 初始化控件()
		{
			this.控件容器 = new Container();
			ComponentResourceManager 资源管理器 = new ComponentResourceManager(typeof(Form1));
			this.MenuStrip1 = new MenuStrip();
			this.系统功能FToolStripMenuItem = new ToolStripMenuItem();
			this.设置ToolStripMenuItem = new ToolStripMenuItem();
			this.退出程序ToolStripMenuItem = new ToolStripMenuItem();
			this.启动小助手ToolStripMenuItem = new ToolStripMenuItem();
			this.访问官网ToolStripMenuItem = new ToolStripMenuItem();
			this.Timer1 = new Timer(this.控件容器);
			this.Label2 = new Label();
			this.Label1 = new Label();
			this.LinkLabel1 = new LinkLabel();
			this.LinkLabel2 = new LinkLabel();
			this.MenuStrip1.SuspendLayout();
			base.SuspendLayout();
			this.MenuStrip1.Items.AddRange(new ToolStripItem[] { this.系统功能FToolStripMenuItem });
			this.MenuStrip1.Location = new Point(0, 0);
			this.MenuStrip1.Name = "MenuStrip1";
			this.MenuStrip1.Size = new Size(682, 25);
			this.MenuStrip1.TabIndex = 0;
			this.MenuStrip1.Text = "MenuStrip1";
			this.系统功能FToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { this.设置ToolStripMenuItem, this.退出程序ToolStripMenuItem, this.启动小助手ToolStripMenuItem, this.访问官网ToolStripMenuItem });
			this.系统功能FToolStripMenuItem.Name = "系统功能FToolStripMenuItem";
			this.系统功能FToolStripMenuItem.Size = new Size(98, 21);
			this.系统功能FToolStripMenuItem.Text = "系统功能（F）";
			this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
			this.设置ToolStripMenuItem.Size = new Size(124, 22);
			this.设置ToolStripMenuItem.Text = "设  置";
			this.退出程序ToolStripMenuItem.Name = "退出程序ToolStripMenuItem";
			this.退出程序ToolStripMenuItem.Size = new Size(124, 22);
			this.退出程序ToolStripMenuItem.Text = "退出程序";
			this.启动小助手ToolStripMenuItem.Name = "启动小助手ToolStripMenuItem";
			this.启动小助手ToolStripMenuItem.Size = new Size(124, 22);
			this.启动小助手ToolStripMenuItem.Text = "启动程序";
			this.访问官网ToolStripMenuItem.Name = "访问官网ToolStripMenuItem";
			this.访问官网ToolStripMenuItem.Size = new Size(124, 22);
			this.访问官网ToolStripMenuItem.Text = "访问官网";
			this.Timer1.Enabled = true;
			this.Timer1.Interval = 500;
			this.Label2.AutoSize = true;
			this.Label2.BackColor = Color.Transparent;
			this.Label2.ForeColor = Color.Red;
			this.Label2.Location = new Point(157, 413);
			this.Label2.Name = "Label2";
			this.Label2.Size = new Size(41, 12);
			this.Label2.TabIndex = 4;
			this.Label2.Text = "Label2";
			this.Label1.AutoSize = true;
			this.Label1.BackColor = Color.Transparent;
			this.Label1.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label1.ForeColor = Color.Blue;
			this.Label1.Location = new Point(39, 412);
			this.Label1.Name = "Label1";
			this.Label1.Size = new Size(125, 12);
			this.Label1.TabIndex = 3;
			this.Label1.Text = "当前小助手工作状态：";
			this.LinkLabel1.AutoSize = true;
			this.LinkLabel1.BackColor = Color.Transparent;
			this.LinkLabel1.Location = new Point(538, 309);
			this.LinkLabel1.Name = "LinkLabel1";
			this.LinkLabel1.Size = new Size(71, 12);
			this.LinkLabel1.TabIndex = 5;
			this.LinkLabel1.TabStop = true;
			this.LinkLabel1.Text = ">>>访问官网";
			this.LinkLabel2.AutoSize = true;
			this.LinkLabel2.BackColor = Color.Transparent;
			this.LinkLabel2.Location = new Point(268, 412);
			this.LinkLabel2.Name = "LinkLabel2";
			this.LinkLabel2.Size = new Size(53, 12);
			this.LinkLabel2.TabIndex = 6;
			this.LinkLabel2.TabStop = true;
			this.LinkLabel2.Text = "查看帮助";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackgroundImage = (Image)资源管理器.GetObject("$this.BackgroundImage");
			base.ClientSize = new Size(682, 486);
			base.Controls.Add(this.LinkLabel2);
			base.Controls.Add(this.LinkLabel1);
			base.Controls.Add(this.Label2);
			base.Controls.Add(this.Label1);
			base.Controls.Add(this.MenuStrip1);
			base.Icon = (Icon)资源管理器.GetObject("$this.Icon");
			base.MainMenuStrip = this.MenuStrip1;
			base.Name = "Form1";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "学生机房管理助手v13.4";
			this.MenuStrip1.ResumeLayout(false);
			this.MenuStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000023BC File Offset: 0x000005BC
		// (set) Token: 0x06000042 RID: 66 RVA: 0x000031F0 File Offset: 0x000013F0
		/// <summary>
		/// 菜单栏（MenuStrip1）。setter 的写法是「先退订旧实例的 ItemClicked，再订阅新实例」。
		/// </summary>
		internal virtual MenuStrip MenuStrip1
		{
			[CompilerGenerated]
			get
			{
				return this.菜单栏;
			}
			[CompilerGenerated]
			set
			{
				ToolStripItemClickedEventHandler toolStripItemClickedEventHandler = new ToolStripItemClickedEventHandler(this.主菜单项点击_空实现);
				MenuStrip menuStrip = this.菜单栏;
				if (menuStrip != null)
				{
					menuStrip.ItemClicked -= toolStripItemClickedEventHandler;
				}
				this.菜单栏 = value;
				menuStrip = this.菜单栏;
				if (menuStrip != null)
				{
					menuStrip.ItemClicked += toolStripItemClickedEventHandler;
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000023C4 File Offset: 0x000005C4
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000023CC File Offset: 0x000005CC
		/// <summary>
		/// 「系统功能（F）」菜单项。自动属性，实际没有任何读写点。
		/// </summary>
		internal virtual ToolStripMenuItem 系统功能FToolStripMenuItem { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000023D5 File Offset: 0x000005D5
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00003234 File Offset: 0x00001434
		/// <summary>
		/// 「设  置」菜单项。setter 在换实例时同步退订/订阅 Click（指向 设置_Click）。
		/// </summary>
		internal virtual ToolStripMenuItem 设置ToolStripMenuItem
		{
			[CompilerGenerated]
			get
			{
				return this.设置菜单项;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.设置_Click);
				ToolStripMenuItem toolStripMenuItem = this.设置菜单项;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click -= eventHandler;
				}
				this.设置菜单项 = value;
				toolStripMenuItem = this.设置菜单项;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click += eventHandler;
				}
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000023DD File Offset: 0x000005DD
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00003278 File Offset: 0x00001478
		/// <summary>
		/// 「退出程序」菜单项。setter 在换实例时同步退订/订阅 Click（指向 退出程序_Click）。
		/// </summary>
		internal virtual ToolStripMenuItem 退出程序ToolStripMenuItem
		{
			[CompilerGenerated]
			get
			{
				return this.退出程序菜单项;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.退出程序_Click);
				ToolStripMenuItem toolStripMenuItem = this.退出程序菜单项;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click -= eventHandler;
				}
				this.退出程序菜单项 = value;
				toolStripMenuItem = this.退出程序菜单项;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click += eventHandler;
				}
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000049 RID: 73 RVA: 0x000023E5 File Offset: 0x000005E5
		// (set) Token: 0x0600004A RID: 74 RVA: 0x000032BC File Offset: 0x000014BC
		/// <summary>
		/// 「访问官网」菜单项。setter 在换实例时同步退订/订阅 Click（指向 访问官网_Click）。
		/// </summary>
		internal virtual ToolStripMenuItem 访问官网ToolStripMenuItem
		{
			[CompilerGenerated]
			get
			{
				return this.访问官网菜单项;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.访问官网_Click);
				ToolStripMenuItem toolStripMenuItem = this.访问官网菜单项;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click -= eventHandler;
				}
				this.访问官网菜单项 = value;
				toolStripMenuItem = this.访问官网菜单项;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click += eventHandler;
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000023ED File Offset: 0x000005ED
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00003300 File Offset: 0x00001500
		/// <summary>
		/// 状态轮询定时器（Timer1）。setter 在换实例时同步退订/订阅 Tick（指向 定时检查小助手状态）。
		/// </summary>
		internal virtual Timer Timer1
		{
			[CompilerGenerated]
			get
			{
				return this.定时器;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.定时检查小助手状态);
				Timer timer = this.定时器;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this.定时器 = value;
				timer = this.定时器;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000023F5 File Offset: 0x000005F5
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000023FD File Offset: 0x000005FD
		/// <summary>
		/// 状态标签（Label2）：显示「关 闭」或「正在运行中。。。」，红色文字。
		/// </summary>
		internal virtual Label Label2 { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002406 File Offset: 0x00000606
		// (set) Token: 0x06000050 RID: 80 RVA: 0x0000240E File Offset: 0x0000060E
		/// <summary>
		/// 标题标签（Label1）：固定文本「当前小助手工作状态：」，蓝色宋体 9pt。
		/// </summary>
		internal virtual Label Label1 { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002417 File Offset: 0x00000617
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00003344 File Offset: 0x00001544
		/// <summary>
		/// 官网链接（LinkLabel1）。setter 在换实例时同步退订/订阅 LinkClicked（指向 官网链接_Click）。
		/// </summary>
		internal virtual LinkLabel LinkLabel1
		{
			[CompilerGenerated]
			get
			{
				return this.官网链接;
			}
			[CompilerGenerated]
			set
			{
				LinkLabelLinkClickedEventHandler linkLabelLinkClickedEventHandler = new LinkLabelLinkClickedEventHandler(this.官网链接_Click);
				LinkLabel linkLabel = this.官网链接;
				if (linkLabel != null)
				{
					linkLabel.LinkClicked -= linkLabelLinkClickedEventHandler;
				}
				this.官网链接 = value;
				linkLabel = this.官网链接;
				if (linkLabel != null)
				{
					linkLabel.LinkClicked += linkLabelLinkClickedEventHandler;
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000053 RID: 83 RVA: 0x0000241F File Offset: 0x0000061F
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00003388 File Offset: 0x00001588
		/// <summary>
		/// 帮助链接（LinkLabel2）。setter 在换实例时同步退订/订阅 LinkClicked（指向 查看帮助_Click）。
		/// </summary>
		internal virtual LinkLabel LinkLabel2
		{
			[CompilerGenerated]
			get
			{
				return this.帮助链接;
			}
			[CompilerGenerated]
			set
			{
				LinkLabelLinkClickedEventHandler linkLabelLinkClickedEventHandler = new LinkLabelLinkClickedEventHandler(this.查看帮助_Click);
				LinkLabel linkLabel = this.帮助链接;
				if (linkLabel != null)
				{
					linkLabel.LinkClicked -= linkLabelLinkClickedEventHandler;
				}
				this.帮助链接 = value;
				linkLabel = this.帮助链接;
				if (linkLabel != null)
				{
					linkLabel.LinkClicked += linkLabelLinkClickedEventHandler;
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002427 File Offset: 0x00000627
		// (set) Token: 0x06000056 RID: 86 RVA: 0x000033CC File Offset: 0x000015CC
		/// <summary>
		/// 「启动程序」菜单项。setter 在换实例时同步退订/订阅 Click（指向 启动小助手_Click）。
		/// </summary>
		internal virtual ToolStripMenuItem 启动小助手ToolStripMenuItem
		{
			[CompilerGenerated]
			get
			{
				return this.启动小助手菜单项;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.启动小助手_Click);
				ToolStripMenuItem toolStripMenuItem = this.启动小助手菜单项;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click -= eventHandler;
				}
				this.启动小助手菜单项 = value;
				toolStripMenuItem = this.启动小助手菜单项;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click += eventHandler;
				}
			}
		}

		// Token: 0x0400000F RID: 15
		/// <summary>未使用字段：long 残留，全文无赋值点也无读取点。</summary>
		private long 未使用字段;

		// Token: 0x04000010 RID: 16
		/// <summary>控件容器：承载 Timer 等组件的 IContainer，Dispose 时一并释放。</summary>
		private IContainer 控件容器;

		// Token: 0x04000011 RID: 17
		/// <summary>菜单栏（MenuStrip1）的后备字段。</summary>
		[AccessedThroughProperty("MenuStrip1")]
		[CompilerGenerated]
		private MenuStrip 菜单栏;

		// Token: 0x04000012 RID: 18
		/// <summary>「系统功能（F）」菜单项的后备字段；属性是自动属性，此字段实际未被读写。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("系统功能FToolStripMenuItem")]
		private ToolStripMenuItem 系统功能菜单项;

		// Token: 0x04000013 RID: 19
		/// <summary>「设  置」菜单项（设置ToolStripMenuItem）的后备字段。</summary>
		[AccessedThroughProperty("设置ToolStripMenuItem")]
		[CompilerGenerated]
		private ToolStripMenuItem 设置菜单项;

		// Token: 0x04000014 RID: 20
		/// <summary>「退出程序」菜单项（退出程序ToolStripMenuItem）的后备字段。</summary>
		[AccessedThroughProperty("退出程序ToolStripMenuItem")]
		[CompilerGenerated]
		private ToolStripMenuItem 退出程序菜单项;

		// Token: 0x04000015 RID: 21
		/// <summary>「访问官网」菜单项（访问官网ToolStripMenuItem）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("访问官网ToolStripMenuItem")]
		private ToolStripMenuItem 访问官网菜单项;

		// Token: 0x04000016 RID: 22
		/// <summary>状态轮询定时器（Timer1）的后备字段，Interval = 500ms。</summary>
		[AccessedThroughProperty("Timer1")]
		[CompilerGenerated]
		private Timer 定时器;

		// Token: 0x04000017 RID: 23
		/// <summary>状态标签（Label2）的后备字段。</summary>
		[AccessedThroughProperty("Label2")]
		[CompilerGenerated]
		private Label 状态标签;

		// Token: 0x04000018 RID: 24
		/// <summary>标题标签（Label1）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Label1")]
		private Label 标题标签;

		// Token: 0x04000019 RID: 25
		/// <summary>官网链接（LinkLabel1）的后备字段。</summary>
		[AccessedThroughProperty("LinkLabel1")]
		[CompilerGenerated]
		private LinkLabel 官网链接;

		// Token: 0x0400001A RID: 26
		/// <summary>帮助链接（LinkLabel2）的后备字段。</summary>
		[AccessedThroughProperty("LinkLabel2")]
		[CompilerGenerated]
		private LinkLabel 帮助链接;

		// Token: 0x0400001B RID: 27
		/// <summary>「启动程序」菜单项（启动小助手ToolStripMenuItem）的后备字段。</summary>
		[AccessedThroughProperty("启动小助手ToolStripMenuItem")]
		[CompilerGenerated]
		private ToolStripMenuItem 启动小助手菜单项;
	}
}
