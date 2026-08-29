using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace WindowsApplication1.Internal
{
	// Token: 0x0200000C RID: 12
	// <summary>
	/// 卸载器核心引擎：RunFullUninstall()（原 InvokeUtils）是超长的顺序卸载流程——
	/// 两轮"杀进程 + 删文件"、停止并删除 zmserv 服务、删除开机自启项与大量注册表项、
	/// 恢复被恶意软件篡改的系统策略（禁用 CMD/注册表编辑器/浏览器限制/IFEO 劫持等）、
	/// 恢复 hosts 文件、导入隐藏的 .reg 恢复数据、重启 explorer 等；
	/// 另提供注册表读写删除的工具函数与密码哈希函数 ComputePasswordHash（原 VisitUtils）。
	/// P/Invoke Crypt* 为 CryptoAPI（MD5）声明，加壳运行时残留，本文件未实际调用。
	/// </summary>
	[StandardModule]
	internal sealed class RegConfig
	{
		// Token: 0x0600004F RID: 79 RVA: 0x00003A9C File Offset: 0x00001C9C
		/// <summary>
		/// 执行完整卸载（原混淆名 InvokeUtils；被 Form1/Form2 调用）。
		/// 控制流被扁平化为 IL 标签 + switch 状态机，以下为还原后的真实执行顺序：
		/// 1) 定位安装目录：读 HKLM\...\Run\prozs（为空则读 WOW6432Node 分支），取值去掉末尾
		///    9 字符（".exe"+5 字符随机文件名）得安装目录 obj；
		/// 2) 第一轮杀进程：循环 Kill 随机 5 字符名与 jfglzsn 的进程直到两者都为空；
		/// 3) 删隐藏目录 C:\windows\jf\ 下 jfglzsn.dat/przs.dat/zmserv.dat（先清只读属性）再删目录；
		/// 4) 删安装目录文件 jfglzsn.exe/zmserv.exe/przs.exe/yl.wav（先清属性）；
		/// 5) 恢复系统策略：DisableCMD=0、DisableRegistryTools=0、NoRun=0；
		/// 6) 读 HKCU\Software\jfglzs\dzjs 取前 Len-16 字符路径，该路径下 SpecialSet.exe 属性清 Normal；
		/// 7) Shell("net stop zmserv", Hide, 等待) → Sleep(3000) → Kill zmserv 进程；
		/// 8) 删 Run 自启值 prozs/jfglzsn（含 WOW6432Node 分支）；
		/// 9) Shell("cmd /c bcdedit /set {bootmgr} displaybootmenu yes") 恢复开机启动菜单；
		/// 10) 恢复 USB 存储：usbstor 服务 Start=3（CurrentControlSet/ControlSet001/002/003）；
		/// 11) DisableLockWorkstation=0；删 HKCU\software 下 n/dtmm/czmm；删 HKCU\Software\jfglzs 键；
		/// 12) 删 20+ 个 IFEO 调试器劫持键（taskkill/ntsd/sidebar/sethc/tasklist/route/Magnify/
		///     winmine/Chess/FreeCell/Hearts/Minesweeper/PurblePlace/Mahjong/SpiderSolitaire/
		///     Solitaire/bckgzm/chkrzm/shvlzm 等，taskkill/PurblePlace 出现两次）；
		/// 13) 删 Chrome/Edge 浏览器策略键；
		/// 14) 恢复 hosts：File.Copy(hosts-bak → hosts, true)；
		/// 15) 继续恢复策略：NoFolderOptions=0、SHOWALL CheckedValue=1、IE Zones\3 1803/2200=0、
		///     NoBrowserSaveAs=0、Edge/Chrome/Firefox 下载与开发者工具策略；
		/// 16) Shell("sc delete zmserv") 删服务；
		/// 17) 伪装成音频的 huifu.wav 复制为 huifu.reg → Shell("regedit.exe /s <dir>\huifu.reg",
		///     Hide, 等待) 静默导入 → Sleep(2000) → 删 huifu.reg 与 yl.reg（清属性后删）；
		/// 18) 桌面/性能恢复：NoTrayContextMenu=0、ShowTaskViewButton=1、删 AutoEndTasks、
		///     HungAppTimeout=3000、WaitToKillAppTimeout=10000、删 StartupDelayInMSec、
		///     Microsoft Games 清属性、Tcpip6 DisabledComponents=0、TabletTip TipbandDesiredVisibility=1；
		/// 19) 第二轮杀进程 + 删文件（重复 2~4，另删 huifu.wav）；
		/// 20) C:\Windows\Microsoft.NET 清属性 → Sleep(3000) → Kill 全部 explorer.exe →
		///     Process.Start("explorer.exe") 重启资源管理器（强制刷新桌面）。
		/// </summary>
		public static void RunFullUninstall()
		{
			// [反混淆] 原状态机（IL_xx 标签 + num/num2 调度 + endfilter 错误重派）→ 还原为顺序控制流（保留 20 步流程与调用顺序）
			try
			{
				checked
				{
					// 1) 定位安装目录：读 HKLM\...\Run\prozs，为空则读 WOW6432Node 分支；取值去末尾 9 字符（".exe"+5 随机字符）得安装目录
					object runValue = RuntimeHelpers.GetObjectValue(RegConfig.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
					if (Operators.ConditionalCompareObjectEqual(runValue, "", false))
					{
						runValue = RuntimeHelpers.GetObjectValue(RegConfig.ReadRegistryValue(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
					}
					object installDirObj = Strings.Mid(Conversions.ToString(runValue), 1, Strings.Len(RuntimeHelpers.GetObjectValue(runValue)) - 9);
					string installPath = Conversions.ToString(installDirObj);
					// 2) 第一轮杀进程：随机 5 字符名进程 + jfglzsn 进程，直到两者都为空（原状态机 do-while 循环）
					string zmservProcessName = "zmserv";
					string victimProcessName = Conversions.ToString(RegConfig.randomProcessName);
					string jfglzsnProcessName = "jfglzsn";
					Process[] randomNameProcs;
					Process[] jfglzsnProcs;
					do
					{
						randomNameProcs = Process.GetProcessesByName(victimProcessName);
						randomNameProcs[0].Kill();
						jfglzsnProcs = Process.GetProcessesByName(jfglzsnProcessName);
						jfglzsnProcs[0].Kill();
					}
					while (!(randomNameProcs.Length == 0 & jfglzsnProcs.Length == 0));
					// 3) 删隐藏目录 C:\windows\jf\ 下三个 .dat（先清只读属性）再删目录
					File.SetAttributes(RegConfig._hiddenDataDir + "\\jfglzsn.dat", FileAttributes.Normal);
					File.SetAttributes(RegConfig._hiddenDataDir + "\\przs.dat", FileAttributes.Normal);
					File.SetAttributes(RegConfig._hiddenDataDir + "\\zmserv.dat", FileAttributes.Normal);
					File.SetAttributes(RegConfig._hiddenDataDir, FileAttributes.Normal);
					File.Delete(RegConfig._hiddenDataDir + "\\jfglzsn.dat");
					File.Delete(RegConfig._hiddenDataDir + "\\przs.dat");
					File.Delete(RegConfig._hiddenDataDir + "\\zmserv.dat");
					Directory.Delete(RegConfig._hiddenDataDir);
					// 4) 删安装目录文件 jfglzsn.exe/zmserv.exe/przs.exe/yl.wav（先清属性）
					File.SetAttributes(installPath + "\\jfglzsn.exe", FileAttributes.Normal);
					File.SetAttributes(installPath + "\\zmserv.exe", FileAttributes.Normal);
					File.SetAttributes(installPath + "\\przs.exe", FileAttributes.Normal);
					File.SetAttributes(installPath + "\\yl.wav", FileAttributes.Normal);
					File.Delete(installPath + "\\jfglzsn.exe");
					File.Delete(installPath + "\\zmserv.exe");
					File.Delete(installPath + "\\przs.exe");
					File.Delete(installPath + "\\yl.wav");
					// 5) 恢复系统策略：解除禁用 CMD / 注册表编辑器 / 运行框
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableRegistryTools", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoRun", 0, RegistryValueKind.DWord);
					// 6) 读 HKCU\Software\jfglzs\dzjs 取路径（前 Len-16 字符），该路径下 SpecialSet.exe 属性清 Normal
					string dzjsPathValue = Conversions.ToString(RegConfig.ReadRegistryValue(Registry.CurrentUser, "Software\\jfglzs", "dzjs", ""));
					string specialSetDir = Strings.Mid(dzjsPathValue, 1, Strings.Len(dzjsPathValue) - 16);
					File.SetAttributes(specialSetDir + "\\SpecialSet.exe", FileAttributes.Normal);
					// 7) 停止 zmserv 服务 → 等待 3 秒 → 杀死 zmserv 进程
					Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1);
					Thread.Sleep(3000);
					Process[] zmservProcs = Process.GetProcessesByName(zmservProcessName);
					zmservProcs[0].Kill();
					// 8) 删 Run 自启值 prozs/jfglzsn（含 WOW6432Node 分支）
					RegConfig.DeleteRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs");
					RegConfig.DeleteRegistryValue(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn");
					RegConfig.DeleteRegistryValue(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs");
					RegConfig.DeleteRegistryValue(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn");
					// 9) 恢复开机启动菜单（bcdedit）
					Interaction.Shell("cmd /c bcdedit /set {bootmgr} displaybootmenu yes", AppWinStyle.MinimizedFocus, false, -1);
					// 10) 恢复 USB 存储：usbstor 服务 Start=3（CurrentControlSet + ControlSet001/002/003）
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord);
					// 11) 恢复锁屏；删卸载密码/标记值 n、dtmm、czmm
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableLockWorkstation", 0, RegistryValueKind.DWord);
					RegConfig.DeleteRegistryValue(Registry.CurrentUser, "software", "n");
					RegConfig.DeleteRegistryValue(Registry.CurrentUser, "software", "dtmm");
					RegConfig.DeleteRegistryValue(Registry.CurrentUser, "software", "czmm");
					// 12) 删 HKCU\Software\jfglzs 键（若存在）
					//     注：原状态机经 VisitorConfig.Computer.Registry（My.Computer）取根键，等价于直接使用 Registry.CurrentUser
					using (RegistryKey jfglzsKey = Registry.CurrentUser.OpenSubKey("Software\\jfglzs"))
					{
						if (jfglzsKey != null)
						{
							Registry.CurrentUser.DeleteSubKey("Software\\jfglzs");
						}
					}
					// 13) 删 20+ 个 IFEO 调试器劫持键（taskkill/PurblePlace 各出现两次，与原状态机一致）
					RegistryKey localMachineRoot = Registry.LocalMachine;
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\taskkill.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\ntsd.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\sidebar.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Chess.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\FreeCell.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Hearts.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Minesweeper.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\PurblePlace.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Mahjong.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\SpiderSolitaire.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\bckgzm.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\chkrzm.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\shvlzm.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Solitaire.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\winmine");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\PurblePlace.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Magnify.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\sethc.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\tasklist.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\taskkill.exe");
					localMachineRoot.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\route.exe");
					// 14) 删 Chrome/Edge 浏览器组策略键（若存在）
					using (RegistryKey chromePolicyKey = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Google\\Chrome"))
					{
						if (chromePolicyKey != null)
						{
							Registry.LocalMachine.DeleteSubKey("Software\\Policies\\Google\\Chrome");
						}
					}
					using (RegistryKey edgePolicyKey = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Edge"))
					{
						if (edgePolicyKey != null)
						{
							Registry.LocalMachine.DeleteSubKey("Software\\Policies\\Microsoft\\Edge");
						}
					}
					// 15) 恢复 hosts 文件（备份 hosts-bak 覆盖，先清属性）
					File.SetAttributes(RegConfig._hostsFilePath, FileAttributes.Normal);
					File.SetAttributes(RegConfig._hostsBackupPath, FileAttributes.Normal);
					File.SetAttributes("C:\\Windows\\System32\\drivers\\etc", FileAttributes.Normal);
					File.Copy(RegConfig._hostsBackupPath, RegConfig._hostsFilePath, true);
					// 16) 继续恢复策略：文件夹选项 / 显示隐藏文件 / IE 区域 / 浏览器下载与开发者工具限制
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoFolderOptions", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\Folder\\Hidden\\SHOWALL", "CheckedValue", 1, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 1, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 1, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord);
					// 17) 删除 zmserv 服务
					Interaction.Shell("sc delete zmserv", AppWinStyle.Hide, true, -1);
					// 18) 伪装成音频的 huifu.wav → huifu.reg，regedit 静默导入恢复数据 → 清理 huifu.reg/yl.reg
					File.Copy(installPath + "\\huifu.wav", installPath + "\\huifu.reg", true);
					Interaction.Shell("regedit.exe /s " + installPath + "\\huifu.reg", AppWinStyle.Hide, true, -1);
					Thread.Sleep(2000);
					File.SetAttributes(installPath + "\\huifu.reg", FileAttributes.Normal);
					File.Delete(installPath + "\\huifu.reg");
					File.SetAttributes(installPath + "\\yl.reg", FileAttributes.Normal);
					File.Delete(installPath + "\\yl.reg");
					// 19) 桌面/性能恢复：任务栏/右键菜单/超时设置/IPv6/输入面板
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoTrayContextMenu", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowTaskViewButton", 1, RegistryValueKind.DWord);
					RegConfig.DeleteRegistryValue(Registry.CurrentUser, "Control Panel\\Desktop", "AutoEndTasks");
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Control Panel\\Desktop", "HungAppTimeout", 3000, RegistryValueKind.String);
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "Control Panel\\Desktop", "WaitToKillAppTimeout", 10000, RegistryValueKind.String);
					RegConfig.DeleteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Serialize", "StartupDelayInMSec");
					File.SetAttributes("C:\\Program Files\\Microsoft Games", FileAttributes.Normal);
					RegConfig.WriteRegistryValue(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\Tcpip6\\Parameters", "DisabledComponents", 0, RegistryValueKind.DWord);
					RegConfig.WriteRegistryValue(Registry.CurrentUser, "SOFTWARE\\Microsoft\\TabletTip\\1.7", "TipbandDesiredVisibility", 1, RegistryValueKind.DWord);
					// 20) 第二轮杀进程 + 删文件（重复步骤 2~4，另删 huifu.wav）
					do
					{
						randomNameProcs = Process.GetProcessesByName(victimProcessName);
						randomNameProcs[0].Kill();
						jfglzsnProcs = Process.GetProcessesByName(jfglzsnProcessName);
						jfglzsnProcs[0].Kill();
					}
					while (!(randomNameProcs.Length == 0 & jfglzsnProcs.Length == 0));
					File.SetAttributes(RegConfig._hiddenDataDir + "\\jfglzsn.dat", FileAttributes.Normal);
					File.SetAttributes(RegConfig._hiddenDataDir + "\\przs.dat", FileAttributes.Normal);
					File.SetAttributes(RegConfig._hiddenDataDir + "\\zmserv.dat", FileAttributes.Normal);
					File.SetAttributes(RegConfig._hiddenDataDir, FileAttributes.Normal);
					File.Delete(RegConfig._hiddenDataDir + "\\jfglzsn.dat");
					File.Delete(RegConfig._hiddenDataDir + "\\przs.dat");
					File.Delete(RegConfig._hiddenDataDir + "\\zmserv.dat");
					Directory.Delete(RegConfig._hiddenDataDir);
					File.SetAttributes(installPath + "\\jfglzsn.exe", FileAttributes.Normal);
					File.SetAttributes(installPath + "\\zmserv.exe", FileAttributes.Normal);
					File.SetAttributes(installPath + "\\przs.exe", FileAttributes.Normal);
					File.SetAttributes(installPath + "\\yl.wav", FileAttributes.Normal);
					File.SetAttributes(installPath + "\\huifu.wav", FileAttributes.Normal);
					File.Delete(installPath + "\\jfglzsn.exe");
					File.Delete(installPath + "\\zmserv.exe");
					File.Delete(installPath + "\\przs.exe");
					File.Delete(installPath + "\\yl.wav");
					File.Delete(installPath + "\\huifu.wav");
					// 21) 清 C:\Windows\Microsoft.NET 属性 → 等待 3 秒 → 杀全部 explorer → 重启资源管理器（强制刷新桌面）
					File.SetAttributes("C:\\Windows\\Microsoft.NET", FileAttributes.Normal);
					Thread.Sleep(3000);
					Process[] explorerProcs = Process.GetProcessesByName("explorer");
					for (int i = 0; i < explorerProcs.Length; i++)
					{
						explorerProcs[i].Kill();
					}
					Process.Start("explorer.exe");
				}
			}
			catch (Exception ex)
			{
				// 原混淆样板（VB On Error 残留）：忽略错误
			}
		}

// Token: 0x06000050 RID: 80
		/// <summary>CryptoAPI CryptAcquireContext P/Invoke 声明（加壳运行时残留，本文件未实际调用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptAcquireContext(ref IntPtr intptr_0, string string_0, string string_1, int int_0, int int_1);

		// Token: 0x06000051 RID: 81
		/// <summary>CryptoAPI CryptCreateHash P/Invoke 声明（加壳运行时残留，本文件未实际调用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptCreateHash(IntPtr intptr_0, int int_0, IntPtr intptr_1, int int_1, ref IntPtr intptr_2);

		// Token: 0x06000052 RID: 82
		/// <summary>CryptoAPI CryptHashData P/Invoke 声明（加壳运行时残留，本文件未实际调用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptHashData(IntPtr intptr_0, byte[] byte_0, int int_0, int int_1);

		// Token: 0x06000053 RID: 83
		/// <summary>CryptoAPI CryptGetHashParam P/Invoke 声明（加壳运行时残留，本文件未实际调用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptGetHashParam(IntPtr intptr_0, int int_0, byte[] byte_0, ref int int_1, int int_2);

		// Token: 0x06000054 RID: 84
		/// <summary>CryptoAPI CryptDestroyHash P/Invoke 声明（加壳运行时残留，本文件未实际调用）。</summary>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptDestroyHash(IntPtr intptr_0);

		// Token: 0x06000055 RID: 85
		/// <summary>CryptoAPI CryptReleaseContext P/Invoke 声明（加壳运行时残留，本文件未实际调用）。</summary>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptReleaseContext(IntPtr intptr_0, int int_0);

		// Token: 0x06000056 RID: 86 RVA: 0x00004BBC File Offset: 0x00002DBC
		/// <summary>
		/// 计算卸载密码哈希（原混淆名 VisitUtils）：
		/// SHA256(UTF8(输入 + "bfdshgs")) → 逐字节转十六进制小写字符串，
		/// 与 HKCU\Software\n 中存值比对（Form2 密码校验用）。盐值固定为 "bfdshgs"。
		/// </summary>
		public static string ComputePasswordHash(string string_0)
		{
			string result;
			using (SHA256 sha = SHA256.Create())
			{
				byte[] array = sha.ComputeHash(Encoding.UTF8.GetBytes(string_0 + "bfdshgs"));
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte b in array)
				{
					stringBuilder.Append(b.ToString("x2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004C48 File Offset: 0x00002E48
		/// <summary>
		/// 读取注册表值（原混淆名 RemoveUtils）：只读打开子键读取值；
		/// 键或值不存在时返回默认值 defaultValue（默认 null）。
		/// </summary>
		public static object ReadRegistryValue(RegistryKey rootKey, string subKeyPath, string valueName, object defaultValue = null)
		{
			// [反混淆] 原状态机（IL_xx 标签 + num/num2 调度 + endfilter 错误重派）→ 还原为顺序 try-catch
			object result = defaultValue;
			try
			{
				using (RegistryKey registryKey = rootKey.OpenSubKey(subKeyPath))
				{
					if (registryKey != null)
					{
						result = registryKey.GetValue(valueName, RuntimeHelpers.GetObjectValue(defaultValue));
					}
				}
			}
			catch (Exception ex)
			{
				// 原混淆样板（VB On Error 残留）：忽略错误，返回默认值
			}
			return result;
		}

// Token: 0x06000058 RID: 88 RVA: 0x00004D10 File Offset: 0x00002F10
		/// <summary>
		/// 写入注册表值（原混淆名 ViewUtils）：可写打开子键写值；
		/// 键不存在则先 CreateSubKey；失败抛"写入注册表值时发生错误"。
		/// </summary>
		public static void WriteRegistryValue(RegistryKey registryKey_0, string string_0, string string_1, object object_0, RegistryValueKind registryValueKind_0 = RegistryValueKind.String)
		{
			try
			{
				using (RegistryKey registryKey = registryKey_0.OpenSubKey(string_0, true))
				{
					if (registryKey == null)
					{
						using (RegistryKey registryKey2 = registryKey_0.CreateSubKey(string_0))
						{
							registryKey2.SetValue(string_1, RuntimeHelpers.GetObjectValue(object_0), registryValueKind_0);
							return;
						}
					}
					registryKey.SetValue(string_1, RuntimeHelpers.GetObjectValue(object_0), registryValueKind_0);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("写入注册表值时发生错误: " + ex.Message);
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004DB0 File Offset: 0x00002FB0
		/// <summary>
		/// 删除注册表值（原混淆名 ChangeUtils）：可写打开子键删除指定值；
		/// 子键不存在则跳过；失败抛"删除注册表值时发生错误"。
		/// </summary>
		public static void DeleteRegistryValue(RegistryKey registryKey_0, string string_0, string string_1)
		{
			try
			{
				using (RegistryKey registryKey = registryKey_0.OpenSubKey(string_0, true))
				{
					if (registryKey != null)
					{
						registryKey.DeleteValue(string_1, false);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表值时发生错误: " + ex.Message);
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004E18 File Offset: 0x00003018
		/// <summary>
		/// 判断注册表键是否存在（原混淆名 RegisterUtils）：只读打开子键，非 null 即存在；
		/// 异常时返回 false。
		/// </summary>
		public static bool RegistryKeyExists(RegistryKey registryKey_0, string string_0)
		{
			bool result;
			try
			{
				using (RegistryKey registryKey = registryKey_0.OpenSubKey(string_0))
				{
					result = (registryKey != null);
				}
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004E70 File Offset: 0x00003070
		/// <summary>
		/// 确保注册表键存在（原混淆名 CalcUtils）：不存在则创建，恒返回 true；
		/// 失败抛"确保注册表键存在操作失败"。
		/// </summary>
		public static bool EnsureRegistryKeyExists(RegistryKey registryKey_0, string string_0)
		{
			bool result;
			try
			{
				using (RegistryKey registryKey = registryKey_0.OpenSubKey(string_0))
				{
					if (registryKey == null)
					{
						using (registryKey_0.CreateSubKey(string_0))
						{
							return true;
						}
					}
					result = true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("确保注册表键存在操作失败: " + ex.Message);
			}
			return result;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004EF4 File Offset: 0x000030F4
		/// <summary>
		/// 删除注册表键（若存在）（原混淆名 PatchUtils）：先 RegistryKeyExists 判断，
		/// 存在则 DeleteSubKey 整体删除；失败抛"删除注册表键时发生错误"。
		/// </summary>
		public static void DeleteRegistryKeyIfExists(RegistryKey registryKey_0, string string_0)
		{
			try
			{
				if (RegConfig.RegistryKeyExists(registryKey_0, string_0))
				{
					registryKey_0.DeleteSubKey(string_0);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		// Token: 0x04000022 RID: 34
		/// <summary>hosts 文件路径（原 _ProducerConfig）：%System%\drivers\etc\hosts。</summary>
		private static string _hostsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts";

		// Token: 0x04000023 RID: 35
		/// <summary>hosts 备份路径（原 issuerConfig）：%System%\drivers\etc\hosts-bak，卸载时覆盖回 hosts。</summary>
		private static string _hostsBackupPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts-bak";

		// Token: 0x04000024 RID: 36
		/// <summary>隐藏数据目录（原 m_HelperConfig）：C:\windows\jf（存放 jfglzsn.dat/przs.dat/zmserv.dat）。</summary>
		private static string _hiddenDataDir = "C:\\windows\\jf";

		// Token: 0x04000025 RID: 37
		/// <summary>随机进程名（原 m_ConnectionConfig，5 字符，字符集 'i'~'r'）：由 Form1_Load 生成，
		/// 卸载时按此名 Kill 进程并匹配可执行文件。</summary>
		public static string randomProcessName;

		// Token: 0x04000026 RID: 38
		/// <summary>随机进程名 + ".exe"（原 propertyConfig）：即随机可执行文件名。</summary>
		public static string randomProcessExeName;

		// Token: 0x04000027 RID: 39
		/// <summary>安装目录模板（原 m_PrototypeConfig）："C:\Program Files (x86)\<4 随机字符>"。</summary>
		public static string installDirTemplate;

		// Token: 0x04000028 RID: 40
		/// <summary>未使用的 CryptoAPI 常量（加壳运行时残留）。</summary>
		private const int m_DecoratorConfig = 1;

		// Token: 0x04000029 RID: 41
		/// <summary>未使用的 CryptoAPI 常量（加壳运行时残留）。</summary>
		private const int m_TagConfig = -268435456;

		// Token: 0x0400002A RID: 42
		/// <summary>未使用的 CryptoAPI 常量（加壳运行时残留）。</summary>
		private const int m_ComparatorConfig = 32771;

		// Token: 0x0400002B RID: 43
		/// <summary>未使用的 CryptoAPI 常量（加壳运行时残留）。</summary>
		private const int m_SetterConfig = 2;
	}
}
