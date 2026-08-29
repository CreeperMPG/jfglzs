using System;
using System.ComponentModel;
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
		/// 启动"密码门"窗体：验证通过后才能进入 Form1 设置界面。
		/// 若系统未设密码（HKCU\Software\n 为空）直接放行；否则要求输入密码，
		/// SHA256(输入+"bfdshgs") 与注册表哈希比对，正确才放行。同时生成随机进程名供后续杀进程使用。
		/// </summary>
	// Token: 0x0200000C RID: 12
	[DesignerGenerated]
	public partial class load : Form
	{
		/// <summary>
		/// 构造：挂 Shown→Form_Shown_SkipPasswordIfUnset、Load→Form_Load_GenerateRandomProcessName 事件，再 BuildPasswordFormUi() 构建界面。
		/// </summary>
		// Token: 0x060000CD RID: 205 RVA: 0x0000AF88 File Offset: 0x00009188
		public load()
		{
			base.Shown += this.Form_Shown_SkipPasswordIfUnset;
			base.Load += this.Form_Load_GenerateRandomProcessName;
			this.BuildPasswordFormUi();
		}

		/// <summary>
		/// 在代码中手工创建全部控件：Label_pd（大字号标签）、Label1（红色错误提示）、Label3（"输入小助手密码："）、TextBox1（PasswordChar='*'）、Button1（"确 定"）。
		/// </summary>
		// Token: 0x060000CF RID: 207 RVA: 0x0000B008 File Offset: 0x00009208
		private void BuildPasswordFormUi()
		{
			this.Label_pd = new Label();
			this.Label1 = new Label();
			this.Label3 = new Label();
			this.TextBox1 = new TextBox();
			this.Button1 = new Button();
			this.SuspendLayout();
			this.Label_pd.AutoSize = true;
			this.Label_pd.Font = new Font("宋体", 26.25f, FontStyle.Regular, GraphicsUnit.Point, 134);
			Control label_pd = this.Label_pd;
			Point location = new Point(356, 70);
			label_pd.Location = location;
			this.Label_pd.Name = "Label_pd";
			Control label_pd2 = this.Label_pd;
			Size size = new Size(69, 35);
			label_pd2.Size = size;
			this.Label_pd.TabIndex = 25;
			this.Label_pd.Text = "   ";
			this.Label1.AutoSize = true;
			this.Label1.ForeColor = Color.Red;
			Control label = this.Label1;
			location = new Point(52, 136);
			label.Location = location;
			this.Label1.Name = "Label1";
			Control label2 = this.Label1;
			size = new Size(0, 12);
			label2.Size = size;
			this.Label1.TabIndex = 24;
			this.Label3.AutoSize = true;
			this.Label3.Font = new Font("宋体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.Label3.ForeColor = Color.DarkRed;
			Control label3 = this.Label3;
			location = new Point(46, 82);
			label3.Location = location;
			this.Label3.Name = "Label3";
			Control label4 = this.Label3;
			size = new Size(119, 14);
			label4.Size = size;
			this.Label3.TabIndex = 23;
			this.Label3.Text = "输入小助手密码：";
			Control textBox = this.TextBox1;
			location = new Point(163, 78);
			textBox.Location = location;
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.PasswordChar = '*';
			Control textBox2 = this.TextBox1;
			size = new Size(122, 21);
			textBox2.Size = size;
			this.TextBox1.TabIndex = 22;
			Control button = this.Button1;
			location = new Point(296, 75);
			button.Location = location;
			this.Button1.Name = "Button1";
			Control button2 = this.Button1;
			size = new Size(54, 27);
			button2.Size = size;
			this.Button1.TabIndex = 21;
			this.Button1.Text = "确 定";
			this.Button1.UseVisualStyleBackColor = true;
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			size = new Size(534, 262);
			this.ClientSize = size;
			this.Controls.Add(this.Label_pd);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.TextBox1);
			this.Controls.Add(this.Button1);
			this.Name = "load";
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "load";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		/// <summary>
		/// 控件属性 Label_pd：后备字段 m_LabelPd；setter 自动挂钩 DoubleClick→Label_pd_DoubleClick_Empty（空实现）。
		/// </summary>
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x0000B35C File Offset: 0x0000955C
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x0000B370 File Offset: 0x00009570
		internal virtual Label Label_pd
		{
			get
			{
				return this.m_LabelPd;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Label_pd_DoubleClick_Empty);
				if (this.m_LabelPd != null)
				{
					this.m_LabelPd.DoubleClick -= value2;
				}
				this.m_LabelPd = value;
				if (this.m_LabelPd != null)
				{
					this.m_LabelPd.DoubleClick += value2;
				}
			}
		}

		/// <summary>
		/// 控件属性 Label1（红色错误提示标签），后备字段 m_Label1。
		/// </summary>
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x0000B3BC File Offset: 0x000095BC
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x00002410 File Offset: 0x00000610
		internal virtual Label Label1
		{
			get
			{
				return this.m_Label1;
			}
			set
			{
				this.m_Label1 = value;
			}
		}

		/// <summary>
		/// 控件属性 Label3（"输入小助手密码："），后备字段 m_Label3。
		/// </summary>
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x0000B3D0 File Offset: 0x000095D0
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x00002419 File Offset: 0x00000619
		internal virtual Label Label3
		{
			get
			{
				return this.m_Label3;
			}
			set
			{
				this.m_Label3 = value;
			}
		}

		/// <summary>
		/// 控件属性 TextBox1（密码输入框），后备字段 m_TextBox1。
		/// </summary>
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x0000B3E4 File Offset: 0x000095E4
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00002422 File Offset: 0x00000622
		internal virtual TextBox TextBox1
		{
			get
			{
				return this.m_TextBox1;
			}
			set
			{
				this.m_TextBox1 = value;
			}
		}

		/// <summary>
		/// 控件属性 Button1（"确 定"）；setter 自动挂钩 Click→Button1_Click_VerifyPassword。
		/// </summary>
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x0000B3F8 File Offset: 0x000095F8
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x0000B40C File Offset: 0x0000960C
		internal virtual Button Button1
		{
			get
			{
				return this.m_Button1;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button1_Click_VerifyPassword);
				if (this.m_Button1 != null)
				{
					this.m_Button1.Click -= value2;
				}
				this.m_Button1 = value;
				if (this.m_Button1 != null)
				{
					this.m_Button1.Click += value2;
				}
			}
		}

		/// <summary>
		/// 校验密码（扁平化状态机还原后的真实逻辑）：
		/// 1) 输入 <6 位 → 提示"密码应该是6位以上的字母或数字，请重新输入！"并清空输入框；
		/// 2) left = ExceptionMock.ComputeSaltedSha256(Trim(输入))；right = HKCU\Software\n；
		/// 3) 相等 → 清提示、禁用按钮、Hide()、Reader.MyForms.Form1.Show() 进入设置界面；
		/// 4) 不等 → 提示"密码不正确，请重新输入！"。
		/// 原 num2/num 状态机与 catch when endfilter 结构已还原为普通 if/else + try-catch（2026-08 反混淆）。
		/// </summary>
		// Token: 0x060000DA RID: 218 RVA: 0x0000B458 File Offset: 0x00009658
		// [反混淆] 原状态机约50行（IL_01~IL_1BE + @switch/endfilter 调度）→ 还原为 if/else 控制流
		private void Button1_Click_VerifyPassword(object sender, EventArgs e)
		{
			try
			{
				// 1) 输入长度不足 6 位 → 提示并清空输入框
				if (Strings.Len(this.TextBox1.Text) >= 6)
				{
					// 2) left = SHA256(输入+"bfdshgs")；right = HKCU\Software\n
					string left = ExceptionMock.ComputeSaltedSha256(Strings.Trim(this.TextBox1.Text));
					string right = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
					// 3) 相等 → 放行进入 Form1 设置界面
					if (Operators.CompareString(left, right, false) == 0)
					{
						this.Label1.Text = "";
						this.Button1.Enabled = false;
						this.Hide();
						Reader.MyForms.Form1.Show();
					}
					else
					{
						// 4) 不等 → 密码错误提示并清空输入框
						this.Label1.Text = "密码不正确，请重新输入！";
						this.TextBox1.Text = "";
					}
				}
				else
				{
					this.Label1.Text = "密码应该是6位以上的字母或数字，请重新输入！";
					this.TextBox1.Text = "";
				}
			}
			catch (Exception ex)
			{
				// 原混淆样板：VB On Error 错误处理状态机已还原，异常按原语义忽略
			}
		}

		/// <summary>
		/// 空实现（混淆样板残留）：仅剩状态机框架，无实际逻辑。
		/// </summary>
		// Token: 0x060000DB RID: 219 RVA: 0x0000B648 File Offset: 0x00009848
		// [反混淆] 原状态机约25行（IL_01~IL_69）仅为空壳调度（无任何实际操作）→ 还原为空方法
		private void Label_pd_DoubleClick_Empty(object sender, EventArgs e)
		{
			// 空实现（混淆样板残留）：原状态机无任何实际操作，已还原为空方法
		}

		/// <summary>
		/// Shown 事件：读 HKCU\Software\n，为空（从未设置密码）→ Hide() 自己并直接 Reader.MyForms.Form1.Show() 跳过密码门。
		/// </summary>
		// Token: 0x060000DC RID: 220 RVA: 0x0000B6D8 File Offset: 0x000098D8
		// [反混淆] 原状态机约40行（IL_01~IL_C8 + @switch/endfilter 调度）→ 还原为 if 控制流
		private void Form_Shown_SkipPasswordIfUnset(object sender, EventArgs e)
		{
			try
			{
				// 读 HKCU\Software\n 中保存的密码哈希
				string left = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software", "n", ""));
				// 为空（从未设置密码）→ 跳过密码门，直接进入 Form1
				if (Operators.CompareString(left, "", false) == 0)
				{
					this.Hide();
					Reader.MyForms.Form1.Show();
				}
			}
			catch (Exception ex)
			{
				// 原混淆样板：VB On Error 错误处理状态机已还原，异常按原语义忽略
			}
		}

		/// <summary>
		/// Load 事件：依当前日期（月×日）生成随机进程名：
		/// 1) num=月×日，取 num%7/%9/%5/%3，按奇偶用 Chr(97/98/101/99+余数) 拼 4 字母串，先写 ExceptionMock.RandomProcessName；
		/// 2) VBMath.Randomize(num)，Rnd()*300000+1，逐位 num%10+105（→'i'~'r'）反序拼成 5 字符名，覆盖 RandomProcessName，RandomProcessNameWithExe = 名+".exe"；
		/// 3) 该随机名在 Form1.PreSaveCleanup_KillOldProcesses 中用于杀掉同族随机名进程。
		/// </summary>
		// Token: 0x060000DD RID: 221 RVA: 0x0000B7C8 File Offset: 0x000099C8
		private void Form_Load_GenerateRandomProcessName(object sender, EventArgs e)
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
				ExceptionMock.RandomProcessName = text;
				ExceptionMock.RandomProcessNameWithExe = text + ".exe";
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
				ExceptionMock.RandomProcessName = text2;
				ExceptionMock.RandomProcessNameWithExe = text2 + ".exe";
			}
		}

		/// <summary>
		/// 控件 Label_pd 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000065 RID: 101
		[AccessedThroughProperty("Label_pd")]
		private Label m_LabelPd;

		/// <summary>
		/// 控件 Label1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000066 RID: 102
		[AccessedThroughProperty("Label1")]
		private Label m_Label1;

		/// <summary>
		/// 控件 Label3 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000067 RID: 103
		[AccessedThroughProperty("Label3")]
		private Label m_Label3;

		/// <summary>
		/// 控件 TextBox1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000068 RID: 104
		[AccessedThroughProperty("TextBox1")]
		private TextBox m_TextBox1;

		/// <summary>
		/// 控件 Button1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000069 RID: 105
		[AccessedThroughProperty("Button1")]
		private Button m_Button1;
	}
}
