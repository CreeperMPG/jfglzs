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
	// Token: 0x02000008 RID: 8
	// <summary>
	/// 卸载器入口窗体（隐藏）。窗体本身不可见（无边框、最小化、不在任务栏显示），
	/// 构造时注册 Load 事件（Form1_Load），加载时生成随机进程名/安装目录名并决定
	/// 静默卸载（无密码）还是弹出 Form2 密码窗体（有密码）。
	/// </summary>
	[DesignerGenerated]
	public partial class Form1 : Form
	{
		// Token: 0x06000027 RID: 39 RVA: 0x0000285C File Offset: 0x00000A5C
		/// <summary>
		/// 构造函数：挂接 Load 事件（Form1_Load）并初始化窗体布局。
		/// 真正的卸载决策逻辑在 Form1_Load 中。
		/// </summary>
		public Form1()
		{
			base.Load += this.Form1_Load;
			this.InitializeFormLayout();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000028C8 File Offset: 0x00000AC8
		/// <summary>
		/// 初始化窗体控件/布局（原混淆名 NewUtils）：
		/// 创建 Container 与 1 秒间隔心跳定时器 Timer2，并把窗体外观设为不可见形式
		/// （无边框 ControlBox=false、ShowInTaskbar=false、最小化、标题"卸载"），
		/// 用于隐蔽执行卸载流程。
		/// </summary>
		private void InitializeFormLayout()
		{
			this._CustomerConfig = new Container();
			this.Timer2 = new System.Windows.Forms.Timer(this._CustomerConfig);
			this.SuspendLayout();
			this.Timer2.Interval = 1000;
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			Size clientSize = new Size(385, 224);
			this.ClientSize = clientSize;
			this.ControlBox = false;
			this.FormBorderStyle = FormBorderStyle.None;
			this.Name = "Form1";
			this.ShowInTaskbar = false;
			this.Text = "卸载";
			this.WindowState = FormWindowState.Minimized;
			this.ResumeLayout(false);
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002978 File Offset: 0x00000B78
		// (set) Token: 0x0600002B RID: 43 RVA: 0x0000298C File Offset: 0x00000B8C
		/// <summary>
		/// 心跳定时器 Timer2（1 秒间隔）：set 时自动挂接/解除 Timer2_Tick 事件，
		/// 用于密码模式下持续确保 Form2 可见。
		/// </summary>
		internal virtual System.Windows.Forms.Timer Timer2
		{
			get
			{
				return this._Timer2Field;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Timer2_Tick);
				if (this._Timer2Field != null)
				{
					this._Timer2Field.Tick -= value2;
				}
				this._Timer2Field = value;
				if (this._Timer2Field != null)
				{
					this._Timer2Field.Tick += value2;
				}
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000029D8 File Offset: 0x00000BD8
		/// <summary>
		/// Form1 加载事件（原混淆名 CustomizeUtils，控制流被扁平化为 IL 标签+switch 状态机，
		/// 以下为还原后的真实逻辑顺序）：
		/// 1) 计算 num3 = 当前月份×当前日期，再取对 7/9/5/3 的余数 num4~num7；
		/// 2) 依据 num3 奇偶用 Chr(97+余数) 拼一个 4 字符 Name（随即被覆盖，属混淆噪声）；
		/// 3) Randomize(num3) 后 Rnd() 生成 1~300000 随机数 num8，循环 5 次取每位数字
		///    转 Chr(数字+105)（'i'~'r'）拼成 5 字符随机名 text；
		/// 4) 写入全局配置：randomProcessName=text、randomProcessExeName=text+".exe"、
		///    installDirTemplate="C:\Program Files (x86)\" + Mid(text,2,4)（随机安装目录）；
		/// 5) 读注册表 HKCU\Software\n：为空（未设密码）→ RegConfig.RunFullUninstall() 静默
		///    执行完整卸载 → Sleep(7000) → ProjectData.EndApp() 结束进程；
		/// 6) 非空（已设密码）→ 显示 VisitorConfig.Forms.Form2 密码窗体，启动 Timer2 心跳。
		/// </summary>
		private void Form1_Load(object sender, EventArgs e)
		{
			// [反混淆] 原状态机（IL_xx 标签 + num/num2 调度 + endfilter 错误重派）→ 还原为顺序 if/for 控制流
			try
			{
				checked
				{
					// 种子：当前月份 × 日期（决定下方 4 个余数组合）
					int seed = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
					int mod7 = seed % 7;
					int mod9 = seed % 9;
					int mod5 = seed % 5;
					int mod3 = seed % 3;
					// 依据奇偶用不同顺序拼 4 字符名（随即被覆盖，属混淆噪声）
					if (seed % 2 != 0)
					{
						this.Name = Conversions.ToString(Strings.Chr(97 + mod9)) + Conversions.ToString(Strings.Chr(98 + mod7)) + Conversions.ToString(Strings.Chr(101 + mod3)) + Conversions.ToString(Strings.Chr(99 + mod5));
					}
					else
					{
						this.Name = Conversions.ToString(Strings.Chr(97 + mod7)) + Conversions.ToString(Strings.Chr(98 + mod9)) + Conversions.ToString(Strings.Chr(101 + mod5)) + Conversions.ToString(Strings.Chr(99 + mod3));
					}
					RegConfig.randomProcessName = this.Name;
					RegConfig.randomProcessExeName = this.Name + ".exe";
					// 随机 5 字符进程名：Rnd()*300000+1 取整后逐位取数字 → Chr(数字+105)（字符集 'i'~'r'）
					VBMath.Randomize((double)seed);
					long randomValue = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
					string randomName = "";
					for (int digitIndex = 1; digitIndex <= 5; digitIndex++)
					{
						long charCode = randomValue % 10L + 105L;
						randomName = Conversions.ToString(Strings.Chr((int)charCode)) + randomName;
						randomValue /= 10L;
					}
					RegConfig.randomProcessName = randomName;
					RegConfig.randomProcessExeName = randomName + ".exe";
					RegConfig.installDirTemplate = "C:\\Program Files (x86)\\" + Strings.Mid(randomName, 2, 4);
					this.Show();
					// 读卸载密码标记 HKCU\Software\n：非空 → 弹 Form2 密码窗体并启动心跳；空 → 静默执行完整卸载
					string passwordMark = Conversions.ToString(RegConfig.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
					if (Operators.CompareString(passwordMark, "", false) != 0)
					{
						VisitorConfig.Forms.Form2.Show();
						this.Timer2.Enabled = true;
					}
					else
					{
						RegConfig.RunFullUninstall();
						Thread.Sleep(7000);
						ProjectData.EndApp();
					}
				}
			}
			catch (Exception ex)
			{
				// 原混淆样板（VB On Error 残留）：忽略错误
			}
		}

// Token: 0x0600002D RID: 45 RVA: 0x000021DC File Offset: 0x000003DC
		/// <summary>
		/// Timer2 心跳（原混淆名 ExcludeUtils）：Form2 被关闭/隐藏时强制重新 Show，
		/// 与 Form2 的 FormClosing 拦截配合构成"无法关闭"的强制卸载界面。
		/// </summary>
		private void Timer2_Tick(object sender, EventArgs e)
		{
			if (!VisitorConfig.Forms.Form2.Visible)
			{
				VisitorConfig.Forms.Form2.Show();
			}
		}

		// Token: 0x0400000B RID: 11
		/// <summary>Timer2 心跳定时器的后端字段（属性 Timer2 的存储）。</summary>
		[AccessedThroughProperty("Timer2")]
		private System.Windows.Forms.Timer _Timer2Field;
	}
}
