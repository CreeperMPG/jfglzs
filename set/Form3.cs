using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using WindowsApplication1.Internal;

namespace WindowsApplication1
{
		/// <summary>
		/// "提醒"完成窗体：保存设置流程最后一步显示，告知重启后生效，并校验自启动项是否写入成功（失败则提示需要管理员权限重跑）。
		/// </summary>
	// Token: 0x0200000B RID: 11
	[DesignerGenerated]
	public partial class Form3 : Form
	{
		/// <summary>
		/// 构造：挂 Load→Form_Load_CheckAutostartWritten，再 BuildReminderFormUi() 构建界面。
		/// </summary>
		// Token: 0x060000C2 RID: 194 RVA: 0x0000AB5C File Offset: 0x00008D5C
		public Form3()
		{
			base.Load += this.Form_Load_CheckAutostartWritten;
			this.BuildReminderFormUi();
		}

		/// <summary>
		/// 创建 Label1（"注：软件在重启电脑后自动运行。"）、Label2、Button2（"确  定"）；TopMost 置顶。
		/// </summary>
		// Token: 0x060000C4 RID: 196 RVA: 0x0000ABC8 File Offset: 0x00008DC8
		private void BuildReminderFormUi()
		{
			this.Button2 = new Button();
			this.Label2 = new Label();
			this.Label1 = new Label();
			this.SuspendLayout();
			Control button = this.Button2;
			Point location = new Point(348, 161);
			button.Location = location;
			this.Button2.Name = "Button2";
			Control button2 = this.Button2;
			Size size = new Size(78, 27);
			button2.Size = size;
			this.Button2.TabIndex = 1;
			this.Button2.Text = "确  定";
			this.Button2.UseVisualStyleBackColor = true;
			this.Label2.ForeColor = SystemColors.ControlText;
			Control label = this.Label2;
			location = new Point(20, 20);
			label.Location = location;
			this.Label2.Name = "Label2";
			Control label2 = this.Label2;
			size = new Size(406, 59);
			label2.Size = size;
			this.Label2.TabIndex = 3;
			this.Label2.Text = "请在硬盘保护系统中创建进度或者保存系统（此步可省略）。";
			Control label3 = this.Label1;
			location = new Point(20, 92);
			label3.Location = location;
			this.Label1.Name = "Label1";
			Control label4 = this.Label1;
			size = new Size(406, 55);
			label4.Size = size;
			this.Label1.TabIndex = 4;
			this.Label1.Text = "注：软件在重启电脑后自动运行。";
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			size = new Size(457, 215);
			this.ClientSize = size;
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Button2);
			this.Name = "Form3";
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "提醒";
			this.TopMost = true;
			this.ResumeLayout(false);
		}

		/// <summary>
		/// 控件属性 Button2（"确  定"）；setter 挂钩 Click→Button2_Click_Exit。
		/// </summary>
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x0000ADC8 File Offset: 0x00008FC8
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x0000ADDC File Offset: 0x00008FDC
		internal virtual Button Button2
		{
			get
			{
				return this.m_Button2;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button2_Click_Exit);
				if (this.m_Button2 != null)
				{
					this.m_Button2.Click -= value2;
				}
				this.m_Button2 = value;
				if (this.m_Button2 != null)
				{
					this.m_Button2.Click += value2;
				}
			}
		}

		/// <summary>
		/// 控件属性 Label2（提示文本，权限不足时变红），后备字段 m_Label2。
		/// </summary>
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x0000AE28 File Offset: 0x00009028
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x000023FE File Offset: 0x000005FE
		internal virtual Label Label2
		{
			get
			{
				return this.m_Label2;
			}
			set
			{
				this.m_Label2 = value;
			}
		}

		/// <summary>
		/// 控件属性 Label1（"注：软件在重启电脑后自动运行。"），后备字段 m_Label1。
		/// </summary>
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x0000AE3C File Offset: 0x0000903C
		// (set) Token: 0x060000CA RID: 202 RVA: 0x00002407 File Offset: 0x00000607
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
		/// 点击"确  定"：ProjectData.EndApp() 直接结束进程。
		/// </summary>
		// Token: 0x060000CB RID: 203 RVA: 0x000023DD File Offset: 0x000005DD
		private void Button2_Click_Exit(object sender, EventArgs e)
		{
			ProjectData.EndApp();
		}

		/// <summary>
		/// Load 事件（状态机还原）：读 HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN\prozs，为空再读 WOW6432NODE 分支；
		/// 仍为空 → Label2 变红："因权限不够设置不成功，请打开机房管理助手的安装文件夹（C盘j开头的文件夹），右击文件set.exe，选择 以管理员身份运行，再次设置。"
		/// </summary>
		// Token: 0x060000CC RID: 204 RVA: 0x0000AE50 File Offset: 0x00009050
		// [反混淆] 原状态机约20行（IL_01~IL_112 + @switch/endfilter 调度）→ 还原为正常 if 控制流
		private void Form_Load_CheckAutostartWritten(object sender, EventArgs e)
		{
			try
			{
				// 1) 读 32 位注册表路径 HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN\prozs
				object registryValue = RuntimeHelpers.GetObjectValue(ExceptionMock.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
				// 2) 为空再读 64 位（WOW6432NODE）分支
				if (Operators.ConditionalCompareObjectEqual(registryValue, "", false))
				{
					registryValue = RuntimeHelpers.GetObjectValue(ExceptionMock.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
				}
				// 3) 两个分支都为空 → 自启动项未写入成功，提示权限不足
				if (Operators.ConditionalCompareObjectEqual(registryValue, "", false))
				{
					this.Label2.ForeColor = Color.Red;
					this.Label2.Text = "因权限不够设置不成功，请打开机房管理助手的安装文件夹（C盘j开头的文件夹），右击文件set.exe，选择 以管理员身份运行，再次设置。";
				}
			}
			catch (Exception ex)
			{
				// 原混淆样板：VB On Error 错误处理状态机（endfilter/ProjectData）已还原，异常按原语义忽略
			}
		}

		/// <summary>
		/// 控件 Button2 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000061 RID: 97
		[AccessedThroughProperty("Button2")]
		private Button m_Button2;

		/// <summary>
		/// 控件 Label2 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000062 RID: 98
		[AccessedThroughProperty("Label2")]
		private Label m_Label2;

		/// <summary>
		/// 控件 Label1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000063 RID: 99
		[AccessedThroughProperty("Label1")]
		private Label m_Label1;
	}
}
