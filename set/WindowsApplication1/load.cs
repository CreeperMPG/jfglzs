using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using set.Internal;

namespace WindowsApplication1
{
	/// <summary>
	/// 启动窗体 load（程序真正的主窗体，由 InfoAttr.OnCreateMainForm 指定）：密码闸门 + 随机「账号进程名」生成器。
	/// 密码正确（或从未设过密码）时才隐藏自己并打开 Form1。
	/// </summary>
	/// <remarks>
	/// 访问控制：整个设置界面被 6 位以上密码挡住，摘要算法为 SHA256(明文 + 固定盐 "bfdshgs")，存 HKCU\Software 值名 n。
	/// 首次运行放行：DestroyInfo 发现没设过密码就直接进 Form1，说明这个密码是管理员自己设的，不是授权校验。
	/// 随机进程名：LogoutInfo 生成 'i'~'r' 区间的 5 字符名字写进全局静态字段，供 Form1.CallInfo 当作要杀掉的账号进程名。
	/// 本文件只读注册表，不写文件、不写注册表。
	/// </remarks>
	// Token: 0x0200000F RID: 15
	[DesignerGenerated]
	public class load : Form
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x0000A730 File Offset: 0x00008930
		/// <summary>
		/// 构造函数：先挂 Shown / Load 两个处理器，再调用设计器布局方法（顺序为原版顺序）。
		/// </summary>
		public load()
		{
			base.Shown += this.DestroyInfo;
			base.Load += this.LogoutInfo;
			this.ReadInfo();
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000A770 File Offset: 0x00008970
		/// <summary>
		/// 设计器生成的释放逻辑：disposing 为真时先释放设计器容器 _PrototypeAttr，再调用基类 Dispose。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._PrototypeAttr != null)
				{
					this._PrototypeAttr.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000A7B0 File Offset: 0x000089B0
		/// <summary>
		/// VB 的 InitializeComponent：大号空白标签、红字提示标签、密码标题、密码框、「确 定」按钮（顺序为原版顺序）。
		/// 窗体 534×262、居中、标题「load」。
		/// </summary>
		private void ReadInfo()
		{
			this.Label_pd = new Label();
			this.Label1 = new Label();
			this.Label3 = new Label();
			this.TextBox1 = new TextBox();
			this.Button1 = new Button();
			base.SuspendLayout();
			this.Label_pd.AutoSize = true;
			this.Label_pd.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label_pd.Location = new Point(356, 70);
			this.Label_pd.Name = "Label_pd";
			this.Label_pd.Size = new Size(69, 35);
			this.Label_pd.TabIndex = 25;
			this.Label_pd.Text = "   ";
			this.Label1.AutoSize = true;
			this.Label1.ForeColor = Color.Red;
			this.Label1.Location = new Point(52, 136);
			this.Label1.Name = "Label1";
			this.Label1.Size = new Size(0, 12);
			this.Label1.TabIndex = 24;
			this.Label3.AutoSize = true;
			this.Label3.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label3.ForeColor = Color.DarkRed;
			this.Label3.Location = new Point(46, 82);
			this.Label3.Name = "Label3";
			this.Label3.Size = new Size(119, 14);
			this.Label3.TabIndex = 23;
			this.Label3.Text = "输入小助手密码：";
			this.TextBox1.Location = new Point(163, 78);
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.PasswordChar = '*';
			this.TextBox1.Size = new Size(122, 21);
			this.TextBox1.TabIndex = 22;
			this.Button1.Location = new Point(296, 75);
			this.Button1.Name = "Button1";
			this.Button1.Size = new Size(54, 27);
			this.Button1.TabIndex = 21;
			this.Button1.Text = "确 定";
			this.Button1.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(534, 262);
			base.Controls.Add(this.Label_pd);
			base.Controls.Add(this.Label1);
			base.Controls.Add(this.Label3);
			base.Controls.Add(this.TextBox1);
			base.Controls.Add(this.Button1);
			base.Name = "load";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "load";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x000027BC File Offset: 0x000009BC
		// (set) Token: 0x060000DA RID: 218 RVA: 0x0000AAE0 File Offset: 0x00008CE0
		/// <summary>
		/// 大号空白标签（Label_pd）：显示三个空格，双击无任何效果（ReflectInfo 是空方法）。
		/// </summary>
		internal virtual Label Label_pd
		{
			[CompilerGenerated]
			get
			{
				return this.m_PageAttr;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.ReflectInfo);
				Label label = this.m_PageAttr;
				if (label != null)
				{
					label.DoubleClick -= eventHandler;
				}
				this.m_PageAttr = value;
				label = this.m_PageAttr;
				if (label != null)
				{
					label.DoubleClick += eventHandler;
				}
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000DB RID: 219 RVA: 0x000027C4 File Offset: 0x000009C4
		// (set) Token: 0x060000DC RID: 220 RVA: 0x000027CC File Offset: 0x000009CC
		/// <summary>
		/// 红字提示标签（Label1）：显示密码过短 / 密码不正确两种提示。
		/// </summary>
		internal virtual Label Label1 { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000DD RID: 221 RVA: 0x000027D5 File Offset: 0x000009D5
		// (set) Token: 0x060000DE RID: 222 RVA: 0x000027DD File Offset: 0x000009DD
		/// <summary>
		/// 密码标题标签（Label3）：固定文本「输入小助手密码：」。
		/// </summary>
		internal virtual Label Label3 { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000DF RID: 223 RVA: 0x000027E6 File Offset: 0x000009E6
		// (set) Token: 0x060000E0 RID: 224 RVA: 0x000027EE File Offset: 0x000009EE
		/// <summary>
		/// 密码输入框（TextBox1）：PasswordChar 为 '*'，校验失败或过短时被清空。
		/// </summary>
		internal virtual TextBox TextBox1 { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x000027F7 File Offset: 0x000009F7
		// (set) Token: 0x060000E2 RID: 226 RVA: 0x0000AB24 File Offset: 0x00008D24
		/// <summary>
		/// 「确 定」按钮（Button1）。setter 在换实例时同步退订/订阅 Click（指向 PopInfo）。
		/// </summary>
		internal virtual Button Button1
		{
			[CompilerGenerated]
			get
			{
				return this.watcherAttr;
			}
			[CompilerGenerated]
			set
			{
				EventHandler eventHandler = new EventHandler(this.PopInfo);
				Button button = this.watcherAttr;
				if (button != null)
				{
					button.Click -= eventHandler;
				}
				this.watcherAttr = value;
				button = this.watcherAttr;
				if (button != null)
				{
					button.Click += eventHandler;
				}
			}
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0000AB68 File Offset: 0x00008D68
		/// <summary>
		/// 「确 定」按钮：校验密码摘要——输入摘要 = SHA256(输入 + "bfdshgs") 与 HKCU\Software 值名 n 比对，
		/// 通过则清提示、禁用按钮、隐藏本窗体并显示 Form1；不通过则提示「密码不正确」并清空输入。
		/// 输入不足 6 位时直接提示「密码应该是6位以上的字母或数字」并清空，不做比对。
		/// </summary>
		/// <remarks>
		/// 原版是 VB 的 <c>On Error Resume Next</c>：<c>ClearProjectError()</c> 开场、<c>num2 = 2..15</c> 是状态号、
		/// 末尾 <c>catch when (endfilter(...))</c> 按状态号分发——每条语句失败都只是跳过该条。
		/// 三处跳转的含义：IL_0046 = 输入够长、跳过提示进入校验；IL_00CC = 跳进「密码不正确」分支；
		/// IL_00F2 = 统一出口。这里还原成「提前 return + if」，每个有副作用的语句各自包 try/catch。
		/// 摘要比对用 Operators.CompareString（VB 字符串比较），不能换成 == 。
		/// </remarks>
		private void PopInfo(object sender, EventArgs e)
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
			string 输入摘要 = "";
			try
			{
				输入摘要 = ObserverAttr.InstantiateInfo(Strings.Trim(this.TextBox1.Text));
			}
			catch
			{
			}
			string 已存摘要 = "";
			try
			{
				已存摘要 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software", "n", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(输入摘要, 已存摘要, false) != 0)
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
				this.Label1.Text = "";
			}
			catch
			{
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
				base.Hide();
			}
			catch
			{
			}
			try
			{
				CandidateAttr.ComputeInfo.Form1.Show();
			}
			catch
			{
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000AD20 File Offset: 0x00008F20
		/// <summary>
		/// Label_pd 的双击事件：空方法体（原版只有 On Error Resume Next 骨架，没有任何语句），双击没有任何效果。
		/// </summary>
		private void ReflectInfo(object sender, EventArgs e)
		{
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000ADA8 File Offset: 0x00008DA8
		/// <summary>
		/// Form_Shown：读 HKCU\Software 值名 n，为空（首次运行、还没设过密码）就直接 Hide() 并显示 Form1 放行；
		/// 已设过密码则停在本窗体等输入。
		/// </summary>
		/// <remarks>
		/// 原版是 VB 的 <c>On Error Resume Next</c>（状态号 num2 = 2..5 + 按状态号分发的 catch）。
		/// 跳转 IL_0052 = 统一出口：注册表里有值就什么都不做。读取与两处动作各自包 try/catch，保持逐条忽略错误。
		/// </remarks>
		private void DestroyInfo(object sender, EventArgs e)
		{
			string 已存摘要 = "";
			try
			{
				已存摘要 = Conversions.ToString(ObserverAttr.SortInfo(Registry.CurrentUser, "Software", "n", ""));
			}
			catch
			{
			}
			if (Operators.CompareString(已存摘要, "", false) == 0)
			{
				try
				{
					base.Hide();
				}
				catch
				{
				}
				try
				{
					CandidateAttr.ComputeInfo.Form1.Show();
				}
				catch
				{
				}
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000AE88 File Offset: 0x00009088
		/// <summary>
		/// Form_Load：按「月×日」生成监控端的「账号进程名 / 访客名」，写进 ObserverAttr 的静态字段
		/// （m_AccountAttr = 进程名，m_VisitorAttr = 进程名 + ".exe"）。
		/// </summary>
		/// <remarks>
		/// 分两段，第二段覆盖第一段：
		/// 1) 日期乘积 = 月 × 日；取 %7 / %9 / %5 / %3 四个余数，按「日期乘积 % 2」选两套排列，
		///    用 Strings.Chr(97/98/101/99 + 余数) 拼出 4 个小写字母，先写一次静态字段；
		/// 2) 以「日期乘积」为种子 VBMath.Randomize，取 Math.Round(Rnd()*100000*3+1) 得种子，
		///    再循环 5 次取 Chr(种子 % 10 + 105)——即 i..r 区间 5 个字符，覆盖第 1 段写下的名字。
		/// 第 1 段的 4 字母名会被立刻覆盖，属原版残留逻辑，但两次赋值都是真实副作用，故完整保留。
		/// 种子就是「月×日」，所以进程名每天固定、完全可预测，目的只是不再写死名字以规避按名查杀。
		/// 整段处于 checked 上下文（VB 默认 checked，溢出会抛异常），外层 checked 保留。
		/// </remarks>
		private void LogoutInfo(object sender, EventArgs e)
		{
			checked
			{
				int 日期乘积 = DateAndTime.Month(DateAndTime.Now) * DateAndTime.Day(DateAndTime.Now);
				int 余7 = 日期乘积 % 7;
				int 余9 = 日期乘积 % 9;
				int 余5 = 日期乘积 % 5;
				int 余3 = 日期乘积 % 3;
				string 四字母名;
				if (日期乘积 % 2 == 0)
				{
					四字母名 = Conversions.ToString(Strings.Chr(97 + 余7)) + Conversions.ToString(Strings.Chr(98 + 余9)) + Conversions.ToString(Strings.Chr(101 + 余5)) + Conversions.ToString(Strings.Chr(99 + 余3));
				}
				else
				{
					四字母名 = Conversions.ToString(Strings.Chr(97 + 余9)) + Conversions.ToString(Strings.Chr(98 + 余7)) + Conversions.ToString(Strings.Chr(101 + 余3)) + Conversions.ToString(Strings.Chr(99 + 余5));
				}
				ObserverAttr.m_AccountAttr = 四字母名;
				ObserverAttr.m_VisitorAttr = 四字母名 + ".exe";
				VBMath.Randomize((double)日期乘积);
				long 种子 = (long)Math.Round(unchecked((double)VBMath.Rnd() * 100000.0 * 3.0 + 1.0));
				string 五位名字 = "";
				int 计数 = 1;
				do
				{
					五位名字 = Conversions.ToString(Strings.Chr((int)(种子 % 10L + 105L))) + 五位名字;
					种子 /= 10L;
					计数++;
				}
				while (计数 <= 5);
				ObserverAttr.m_AccountAttr = 五位名字;
				ObserverAttr.m_VisitorAttr = 五位名字 + ".exe";
			}
		}

		// Token: 0x04000069 RID: 105
		/// <summary>设计器容器（就是 components），Dispose 时一并释放。</summary>
		private IContainer _PrototypeAttr;

		// Token: 0x0400006A RID: 106
		/// <summary>大号空白标签（Label_pd）的后备字段。</summary>
		[AccessedThroughProperty("Label_pd")]
		[CompilerGenerated]
		private Label m_PageAttr;

		// Token: 0x0400006B RID: 107
		/// <summary>红字提示标签（Label1）的后备字段。</summary>
		[AccessedThroughProperty("Label1")]
		[CompilerGenerated]
		private Label valAttr;

		// Token: 0x0400006C RID: 108
		/// <summary>密码标题标签（Label3）的后备字段。</summary>
		[AccessedThroughProperty("Label3")]
		[CompilerGenerated]
		private Label _ProcessAttr;

		// Token: 0x0400006D RID: 109
		/// <summary>密码输入框（TextBox1）的后备字段。</summary>
		[CompilerGenerated]
		[AccessedThroughProperty("TextBox1")]
		private TextBox requestAttr;

		// Token: 0x0400006E RID: 110
		/// <summary>「确 定」按钮（Button1）的后备字段。</summary>
		[AccessedThroughProperty("Button1")]
		[CompilerGenerated]
		private Button watcherAttr;
	}
}
