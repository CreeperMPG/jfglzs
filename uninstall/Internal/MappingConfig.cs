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
	// Token: 0x0200000F RID: 15
	/// <summary>
	/// 「机房管理助手」的卸载/回滚核心（VB 模块，全静态方法）。
	/// 它负责把管理端在本机留下的东西全部拆掉：结束进程、删文件、恢复被改写的注册表策略、
	/// 还原 hosts、删服务、删 IFEO 劫持项、导入 huifu.reg、重启 explorer。
	///
	/// 也就是说：本模块的每一行都对应管理端当初做过的某一次「加固」，这里逐条反向执行。
	/// 原混淆名：MappingConfig
	/// </summary>
	[StandardModule]
	internal sealed class 卸载核心
	{
		// Token: 0x06000059 RID: 89 RVA: 0x0000385C File Offset: 0x00001A5C
		/// <summary>
		/// 卸载主流程（Form2 密码校验通过后调用；如果本机根本没设密码，Form1 直接调它）。
		/// 顺序：
		/// 1) 读 HKLM Run 下的 "prozs" 取安装目录，循环结束正在运行的管理进程与 jfglzsn；
		/// 2) 删缓存目录 C:\windows\jf（jfglzsn.dat/przs.dat/zmserv.dat）与安装目录里的可执行文件；
		/// 3) 逐条恢复被改过的系统策略（命令提示符、注册表工具、运行、USB、锁屏、浏览器下载/开发者工具等）；
		/// 4) 用 hosts-bak 覆盖 hosts，还原被劫持的域名；
		/// 5) 停/删服务 zmserv、删 IFEO 劫持项、导入 huifu.reg；
		/// 6) 再杀一轮进程/删一轮文件，最后重启 explorer 让所有改动生效。
		///
		/// 注意：原版整体处于 On Error Resume Next 之下——每条语句出错都被吞掉、继续下一条。
		/// 所以下面几乎每条语句都单独包了 try/catch；把它们删掉会让卸载中途中断，属于改变行为。
		/// 纯算术/局部赋值（不可能抛异常）没有包，见行内说明。
		/// 原混淆名：PopReader
		/// </summary>
		public static void 执行卸载()
		{
			Process[] 进程组 = null;
			Process[] 守护进程组 = null;
			// 1) 找安装目录：HKLM 的 Run 键 "prozs"（64 位），取不到再读 WOW6432NODE
			object 运行项 = RuntimeHelpers.GetObjectValue(卸载核心.读取注册表值(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
			if (Operators.ConditionalCompareObjectEqual(运行项, "", false))
			{
				运行项 = RuntimeHelpers.GetObjectValue(卸载核心.读取注册表值(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs", ""));
			}
			// 去掉末尾 "\prozs.exe"（9 个字符）= 安装目录。Run 键不存在时 Mid 长度变负数会抛异常，
			// 原版由 On Error 吞掉、安装目录保持 Nothing。
			string 安装目录 = null;
			try { 安装目录 = Strings.Mid(Conversions.ToString(运行项), 1, checked(Strings.Len(RuntimeHelpers.GetObjectValue(运行项)) - 9)); } catch { }
			string 服务名 = "zmserv";
			string 管理进程名 = Conversions.ToString(卸载核心.进程名);
			string 守护进程名 = "jfglzsn";
			// 循环杀进程：两个进程都杀不到（GetProcessesByName 返回空数组 → Kill 越界抛异常）才退出
			do
			{
				try { 进程组 = Process.GetProcessesByName(管理进程名); } catch { }
				try { 进程组[0].Kill(); } catch { }
				try { 守护进程组 = Process.GetProcessesByName(守护进程名); } catch { }
				try { 守护进程组[0].Kill(); } catch { }
			}
			while (!((进程组.Length == 0) & (守护进程组.Length == 0)));
			// 2) 删缓存目录 C:\windows\jf：先去掉只读属性，再删文件、删目录
			//（原版逐条忽略错误，所以每条语句各自 try/catch）
			try { File.SetAttributes(卸载核心.缓存目录 + "\\jfglzsn.dat", FileAttributes.Normal); } catch { }
			try { File.SetAttributes(卸载核心.缓存目录 + "\\przs.dat", FileAttributes.Normal); } catch { }
			try { File.SetAttributes(卸载核心.缓存目录 + "\\zmserv.dat", FileAttributes.Normal); } catch { }
			try { File.SetAttributes(卸载核心.缓存目录, FileAttributes.Normal); } catch { }
			try { File.Delete(卸载核心.缓存目录 + "\\jfglzsn.dat"); } catch { }
			try { File.Delete(卸载核心.缓存目录 + "\\przs.dat"); } catch { }
			try { File.Delete(卸载核心.缓存目录 + "\\zmserv.dat"); } catch { }
			try { Directory.Delete(卸载核心.缓存目录); } catch { }
			// 3) 删安装目录里的三个 exe 和提示音
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\jfglzsn.exe")), FileAttributes.Normal); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\zmserv.exe")), FileAttributes.Normal); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\przs.exe")), FileAttributes.Normal); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\yl.wav")), FileAttributes.Normal); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\jfglzsn.exe"))); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\zmserv.exe"))); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\przs.exe"))); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\yl.wav"))); } catch { }
			// 4) 恢复被改写的系统策略：命令提示符 / 注册表编辑器 / 运行
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableRegistryTools", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoRun", 0, RegistryValueKind.DWord); } catch { }
			// 5) HKCU\Software\jfglzs 的 "dzjs" 存着管理端安装目录（末尾带 16 位校验串，要去掉）
			string 注册表安装路径 = null;
			try { 注册表安装路径 = Conversions.ToString(卸载核心.读取注册表值(Registry.CurrentUser, "Software\\jfglzs", "dzjs", "")); } catch { }
			string 注册表路径副本 = 注册表安装路径; // 原码多了一个传手变量，没有实际作用，保留以对齐语句序列
			string 特殊设置目录 = null;
			try { 特殊设置目录 = Strings.Mid(注册表路径副本, 1, checked(Strings.Len(注册表路径副本) - 16)); } catch { }
			try { File.SetAttributes(特殊设置目录 + "\\SpecialSet.exe", FileAttributes.Normal); } catch { }
			// 6) 停服务 zmserv（net stop，等待返回），等 3 秒，再把 zmserv 进程杀掉
			try { Interaction.Shell("net stop zmserv", AppWinStyle.Hide, true, -1); } catch { }
			try { Thread.Sleep(3000); } catch { }
			Process[] 服务进程组 = null;
			try { 服务进程组 = Process.GetProcessesByName(服务名); } catch { }
			try { 服务进程组[0].Kill(); } catch { }
			// 7) 删掉 Run 键里的自启动项（prozs / jfglzsn，32 位与 64 位视图各一份）
			RegistryKey 注册表根 = MyProject.Computer.Registry.LocalMachine;
			try { 卸载核心.删除注册表值(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs"); } catch { }
			try { 卸载核心.删除注册表值(Registry.LocalMachine, "SOFTWARE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn"); } catch { }
			try { 卸载核心.删除注册表值(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "prozs"); } catch { }
			try { 卸载核心.删除注册表值(Registry.LocalMachine, "SOFTWARE\\WOW6432NODE\\MICROSOFT\\WINDOWS\\CURRENTVERSION\\RUN", "jfglzsn"); } catch { }
			// 8) 打开启动菜单（管理端当初把它关掉了）；恢复 USB 存储驱动为可用（Start=3）
			try { Interaction.Shell("cmd /c bcdedit /set {bootmgr} displaybootmenu yes", AppWinStyle.MinimizedFocus, false, -1); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SYSTEM\\ControlSet001\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SYSTEM\\ControlSet002\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SYSTEM\\ControlSet003\\Services\\usbstor", "Start", 3, RegistryValueKind.DWord); } catch { }
			// 9) 允许锁屏；删掉 HKCU\Software 下的密码/时间限制标记（n / dtmm / czmm）
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableLockWorkstation", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.删除注册表值(Registry.CurrentUser, "software", "n"); } catch { }
			try { 卸载核心.删除注册表值(Registry.CurrentUser, "software", "dtmm"); } catch { }
			try { 卸载核心.删除注册表值(Registry.CurrentUser, "software", "czmm"); } catch { }
			// 10) 整个删掉 HKCU\Software\jfglzs（管理端的配置根）；先确认存在再删
			注册表根 = MyProject.Computer.Registry.CurrentUser;
			RegistryKey 待删键 = null;
			try { 待删键 = 注册表根.OpenSubKey("Software\\jfglzs"); } catch { }
			if (待删键 != null)
			{
				try { 注册表根.DeleteSubKey("Software\\jfglzs"); } catch { }
			}
			注册表根 = MyProject.Computer.Registry.LocalMachine;
			// 11) 删 IFEO（Image File Execution Options）劫持项：管理端用「调试器替换」把
			//     任务杀手/任务管理器/粘滞键/放大镜/扫雷纸牌等工具与游戏全部封掉。
			//     这些键大多不存在（DeleteSubKey 会抛异常），原版逐条忽略错误，
			//     所以这里必须逐条 try/catch——合成一个大 try 会在第一条缺失处停住，后面的键就漏删了。
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\taskkill.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\ntsd.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\sidebar.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Chess.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\FreeCell.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Hearts.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Minesweeper.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\PurblePlace.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Mahjong.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\SpiderSolitaire.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\bckgzm.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\chkrzm.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\shvlzm.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Solitaire.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\winmine"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\PurblePlace.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\Magnify.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\sethc.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\tasklist.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\taskkill.exe"); } catch { }
			try { 注册表根.DeleteSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\route.exe"); } catch { }
			// 12) 删 Chrome / Edge 的策略键（管理端用组策略关掉下载/开发者工具等）
			注册表根 = MyProject.Computer.Registry.LocalMachine;
			try { 待删键 = 注册表根.OpenSubKey("Software\\Policies\\Google\\Chrome"); } catch { }
			if (待删键 != null)
			{
				try { 注册表根.DeleteSubKey("Software\\Policies\\Google\\Chrome"); } catch { }
			}
			try { 待删键 = 注册表根.OpenSubKey("Software\\Policies\\Microsoft\\Edge"); } catch { }
			if (待删键 != null)
			{
				try { 注册表根.DeleteSubKey("Software\\Policies\\Microsoft\\Edge"); } catch { }
			}
			// 13) hosts 还原：hosts-bak（管理端备份的原文件）覆盖回 hosts
			try { File.SetAttributes(卸载核心.hosts文件路径, FileAttributes.Normal); } catch { }
			try { File.SetAttributes(卸载核心.hosts备份文件路径, FileAttributes.Normal); } catch { }
			try { File.SetAttributes("C:\\Windows\\System32\\drivers\\etc", FileAttributes.Normal); } catch { }
			try { File.Copy(卸载核心.hosts备份文件路径, 卸载核心.hosts文件路径, true); } catch { }
			// 14) 恢复资源管理器 / IE / Edge / Chrome / Firefox 的策略开关
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoFolderOptions", 0, RegistryValueKind.DWord); } catch { }
			// 让「显示隐藏文件」重新生效（CheckedValue = 1）
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\Folder\\Hidden\\SHOWALL", "CheckedValue", 1, RegistryValueKind.DWord); } catch { }
			// 解除 IE 对下载/文件上传的限制（Zones\3 = Internet 区域）
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "1803", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", "2200", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Internet Explorer\\Restrictions", "NoBrowserSaveAs", 0, RegistryValueKind.DWord); } catch { }
			// Edge：允许下载、允许另存为、允许开发者工具
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DownloadRestrictions", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "SaveAs", 1, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Edge", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord); } catch { }
			// Chrome：同上
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DownloadRestrictions", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "SaveAs", 1, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Google\\Chrome", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord); } catch { }
			// Firefox：允许下载、允许 about:downloads、允许开发者工具
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DisableDownloads", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "BlockAboutDownloads", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SOFTWARE\\Policies\\Mozilla\\Firefox", "DeveloperToolsAvailability", 0, RegistryValueKind.DWord); } catch { }
			// 15) 删服务 zmserv（sc delete，等待返回）
			try { Interaction.Shell("sc delete zmserv", AppWinStyle.Hide, true, -1); } catch { }
			// 16) huifu.wav 其实是注册表脚本（改扩展名躲检测），复制成 huifu.reg 后用 regedit /s 静默导入
			try { File.Copy(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\huifu.wav")), Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\huifu.reg")), true); } catch { }
			try { Interaction.Shell(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("regedit.exe /s ", 安装目录), "\\huifu.reg")), AppWinStyle.Hide, true, -1); } catch { }
			try { Thread.Sleep(2000); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\huifu.reg")), FileAttributes.Normal); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\huifu.reg"))); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\yl.reg")), FileAttributes.Normal); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\yl.reg"))); } catch { }
			// 17) 恢复托盘右键菜单与任务视图按钮
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoTrayContextMenu", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowTaskViewButton", 1, RegistryValueKind.DWord); } catch { }
			// 18) 恢复关机/挂起行为：删 AutoEndTasks，超时值改回 3000 / 10000 毫秒
			try { 卸载核心.删除注册表值(Registry.CurrentUser, "Control Panel\\Desktop", "AutoEndTasks"); } catch { }
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Control Panel\\Desktop", "HungAppTimeout", 3000, RegistryValueKind.String); } catch { }
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "Control Panel\\Desktop", "WaitToKillAppTimeout", 10000, RegistryValueKind.String); } catch { }
			try { 卸载核心.删除注册表值(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Serialize", "StartupDelayInMSec"); } catch { }
			try { File.SetAttributes("C:\\Program Files\\Microsoft Games", FileAttributes.Normal); } catch { }
			// 19) 恢复 IPv6 组件与任务栏提示条
			try { 卸载核心.写入注册表值(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\Tcpip6\\Parameters", "DisabledComponents", 0, RegistryValueKind.DWord); } catch { }
			try { 卸载核心.写入注册表值(Registry.CurrentUser, "SOFTWARE\\Microsoft\\TabletTip\\1.7", "TipbandDesiredVisibility", 1, RegistryValueKind.DWord); } catch { }
			// 20) 再杀一轮进程（regedit 导入后守护进程可能又被拉起来）
			do
			{
				try { 进程组 = Process.GetProcessesByName(管理进程名); } catch { }
				try { 进程组[0].Kill(); } catch { }
				try { 守护进程组 = Process.GetProcessesByName(守护进程名); } catch { }
				try { 守护进程组[0].Kill(); } catch { }
			}
			while (!((进程组.Length == 0) & (守护进程组.Length == 0)));
			// 21) 再清一遍缓存目录与安装目录（第一轮删不掉的，进程被杀了之后这次能删掉）
			try { File.SetAttributes(卸载核心.缓存目录 + "\\jfglzsn.dat", FileAttributes.Normal); } catch { }
			try { File.SetAttributes(卸载核心.缓存目录 + "\\przs.dat", FileAttributes.Normal); } catch { }
			try { File.SetAttributes(卸载核心.缓存目录 + "\\zmserv.dat", FileAttributes.Normal); } catch { }
			try { File.SetAttributes(卸载核心.缓存目录, FileAttributes.Normal); } catch { }
			try { File.Delete(卸载核心.缓存目录 + "\\jfglzsn.dat"); } catch { }
			try { File.Delete(卸载核心.缓存目录 + "\\przs.dat"); } catch { }
			try { File.Delete(卸载核心.缓存目录 + "\\zmserv.dat"); } catch { }
			try { Directory.Delete(卸载核心.缓存目录); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\jfglzsn.exe")), FileAttributes.Normal); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\zmserv.exe")), FileAttributes.Normal); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\przs.exe")), FileAttributes.Normal); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\yl.wav")), FileAttributes.Normal); } catch { }
			try { File.SetAttributes(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\huifu.wav")), FileAttributes.Normal); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\jfglzsn.exe"))); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\zmserv.exe"))); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\przs.exe"))); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\yl.wav"))); } catch { }
			try { File.Delete(Conversions.ToString(Operators.ConcatenateObject(安装目录, "\\huifu.wav"))); } catch { }
			// 22) 恢复 C:\Windows\Microsoft.NET 的属性，等 2 秒
			try { File.SetAttributes("C:\\Windows\\Microsoft.NET", FileAttributes.Normal); } catch { }
			try { Thread.Sleep(2000); } catch { }
			// 23) 杀掉所有 explorer.exe 再重启，让资源管理器/策略改动立即生效
			//     （第一条语句取进程列表，出错的概率极低但原版同样吞错）
			Process[] 浏览器进程组 = null;
			try { 浏览器进程组 = Process.GetProcessesByName("explorer"); } catch { }
			foreach (Process 资源管理器进程 in 浏览器进程组)
			{
				try { 资源管理器进程.Kill(); } catch { }
			}
			try { Process.Start("explorer.exe"); } catch { }
		}

		// Token: 0x0600005A RID: 90
		/// <summary>
		/// advapi32 的 CryptoAPI 六个函数：本工程里一个都没调用，是保护器/旧版本遗留的声明
		/// （CryptAcquireContext/CreateHash/HashData/GetHashParam/DestroyHash/ReleaseContext 是一套完整哈希流程）。
		/// 保留声明以对齐原始类型表；参数名按 CryptoAPI 语义还原。
		/// </summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptAcquireContext(ref IntPtr 提供程序句柄, string 容器名, string 提供程序名, int 提供程序类型, int 标志);

		// Token: 0x0600005B RID: 91
		/// <summary>advapi32!CryptCreateHash：创建哈希对象（未被调用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptCreateHash(IntPtr 提供程序句柄, int 算法ID, IntPtr 密钥句柄, int 标志, ref IntPtr 哈希句柄);

		// Token: 0x0600005C RID: 92
		/// <summary>advapi32!CryptHashData：喂数据给哈希对象（未被调用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptHashData(IntPtr 哈希句柄, byte[] 数据, int 数据长度, int 标志);

		// Token: 0x0600005D RID: 93
		/// <summary>advapi32!CryptGetHashParam：取哈希结果（未被调用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptGetHashParam(IntPtr 哈希句柄, int 参数类型, byte[] 输出缓冲, ref int 缓冲长度, int 标志);

		// Token: 0x0600005E RID: 94
		/// <summary>advapi32!CryptDestroyHash：销毁哈希对象（未被调用）。</summary>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptDestroyHash(IntPtr 哈希句柄);

		// Token: 0x0600005F RID: 95
		/// <summary>advapi32!CryptReleaseContext：释放加密提供程序（未被调用）。</summary>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptReleaseContext(IntPtr 提供程序句柄, int 标志);

		// Token: 0x06000060 RID: 96 RVA: 0x000048BC File Offset: 0x00002ABC
		/// <summary>
		/// 计算卸载密码的哈希：SHA256( UTF8(输入 + "bfdshgs") ) 的十六进制小写串。
		/// 加盐串 "bfdshgs" 是硬编码的固定盐；Form2 拿用户输入算一遍，与
		/// HKCU\Software 下 "n" 里保存的值比较——相等才允许卸载。
		/// 原混淆名：CustomizeReader
		/// </summary>
		public static string 计算密码哈希(string 明文密码)
		{
			string 哈希十六进制;
			using (SHA256 SHA256实例 = SHA256.Create())
			{
				byte[] 哈希字节 = SHA256实例.ComputeHash(Encoding.UTF8.GetBytes(明文密码 + "bfdshgs"));
				StringBuilder 构造器 = new StringBuilder();
				foreach (byte 字节 in 哈希字节)
				{
					构造器.Append(字节.ToString("x2"));
				}
				哈希十六进制 = 构造器.ToString();
			}
			return 哈希十六进制;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004940 File Offset: 0x00002B40
		/// <summary>
		/// 读一个注册表值（默认值可省）。原版是 On Error Resume Next：
		/// 打开子键/取值任一步抛异常都被吞掉，函数返回 null（局部变量没被赋值）。
		/// 键不存在时返回调用方给的默认值（注意与「出错返回 null」不同）。
		/// 原混淆名：SortReader
		/// </summary>
		public static object 读取注册表值(RegistryKey 根键, string 子键路径, string 值名, object 默认值 = null)
		{
			object 结果 = null;
			try
			{
				using (RegistryKey 子键 = 根键.OpenSubKey(子键路径))
				{
					if (子键 != null)
					{
						结果 = 子键.GetValue(值名, RuntimeHelpers.GetObjectValue(默认值));
					}
					else
					{
						结果 = 默认值;
					}
				}
			}
			catch
			{
				// On Error Resume Next：出错时不赋值，结果保持 null
			}
			return 结果;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004A08 File Offset: 0x00002C08
		/// <summary>
		/// 写一个注册表值（子键不存在就先建）。失败时把异常包成中文消息再抛——
		/// 注意它并不吞异常，调用方（执行卸载）靠外层 try/catch 才继续。
		/// 原码在「键不存在」分支建完键写值后直接跳出 using，不会重复写一次，这里用 if/else 等价表达。
		/// 原混淆名：ResetReader
		/// </summary>
		public static void 写入注册表值(RegistryKey 根键, string 子键路径, string 值名, object 值, RegistryValueKind 值类型 = RegistryValueKind.String)
		{
			try
			{
				using (RegistryKey 子键 = 根键.OpenSubKey(子键路径, true))
				{
					if (子键 == null)
					{
						using (RegistryKey 新建键 = 根键.CreateSubKey(子键路径))
						{
							新建键.SetValue(值名, RuntimeHelpers.GetObjectValue(值), 值类型);
						}
					}
					else
					{
						子键.SetValue(值名, RuntimeHelpers.GetObjectValue(值), 值类型);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("写入注册表值时发生错误: " + ex.Message);
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004AA8 File Offset: 0x00002CA8
		/// <summary>
		/// 删一个注册表值：子键不存在就什么都不做；值不存在也不报错（DeleteValue 的第二个参数为 false）。
		/// 失败时包装成中文异常抛出。原混淆名：DisableReader
		/// </summary>
		public static void 删除注册表值(RegistryKey 根键, string 子键路径, string 值名)
		{
			try
			{
				using (RegistryKey 子键 = 根键.OpenSubKey(子键路径, true))
				{
					if (子键 != null)
					{
						子键.DeleteValue(值名, false);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表值时发生错误: " + ex.Message);
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004B14 File Offset: 0x00002D14
		/// <summary>判断注册表子键是否存在；任何异常都当作「不存在」（返回 false）。原混淆名：UpdateReader</summary>
		public static bool 注册表键存在(RegistryKey 根键, string 子键路径)
		{
			bool 存在;
			try
			{
				using (RegistryKey 子键 = 根键.OpenSubKey(子键路径))
				{
					存在 = 子键 != null;
				}
			}
			catch (Exception)
			{
				存在 = false;
			}
			return 存在;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004B68 File Offset: 0x00002D68
		/// <summary>
		/// 确保注册表子键存在：不存在就创建。成功返回 true；出错时包装成中文异常抛出。
		/// 全工程没有调用点（保护器遗留的工具方法）。
		/// 原混淆名：RemoveReader
		/// </summary>
		public static bool 确保注册表键存在(RegistryKey 根键, string 子键路径)
		{
			try
			{
				using (RegistryKey 子键 = 根键.OpenSubKey(子键路径))
				{
					if (子键 == null)
					{
						using (RegistryKey 新建键 = 根键.CreateSubKey(子键路径))
						{
							return true;
						}
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("确保注册表键存在操作失败: " + ex.Message);
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004BEC File Offset: 0x00002DEC
		/// <summary>子键存在就删掉（不存在就什么都不做），失败包装成中文异常。原混淆名：CollectReader</summary>
		public static void 删除注册表键(RegistryKey 根键, string 子键路径)
		{
			try
			{
				if (卸载核心.注册表键存在(根键, 子键路径))
				{
					根键.DeleteSubKey(子键路径);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		// Token: 0x04000027 RID: 39
		/// <summary>系统 hosts 文件路径（管理端劫持域名的目标）。</summary>
		private static string hosts文件路径 = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts";

		// Token: 0x04000028 RID: 40
		/// <summary>hosts 的备份：管理端改 hosts 之前留的原件，卸载时用它覆盖回去。</summary>
		private static string hosts备份文件路径 = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts-bak";

		// Token: 0x04000029 RID: 41
		/// <summary>管理端临时缓存目录 C:\windows\jf（存放 jfglzsn.dat / przs.dat / zmserv.dat）。</summary>
		private static string 缓存目录 = "C:\\windows\\jf";

		// Token: 0x0400002A RID: 42
		/// <summary>
		/// 管理端主进程名：Form1 在运行时用 日期(月×日) 做种子生成随机数，再转成 5 个 i..r 的字母写进这里，
		/// 卸载时用它去找进程——所以进程名每个日期都不一样，静态写死杀不掉。
		/// </summary>
		public static string 进程名;

		// Token: 0x0400002B RID: 43
		/// <summary>进程名 + ".exe"（Form1 里赋值；本工程没有读取点）。</summary>
		public static string 进程文件名;

		// Token: 0x0400002C RID: 44
		/// <summary>候选安装目录 "C:\Program Files (x86)\" + 进程名第 2~5 位（Form1 里赋值；本工程没有读取点）。</summary>
		public static string 候选安装目录;

		// Token: 0x0400002D RID: 45
		/// <summary>未使用的常量（保护器遗留，全工程无引用）。</summary>
		private const int 未使用常量1 = 1;

		// Token: 0x0400002E RID: 46
		/// <summary>未使用的常量（-268435456 = 0xF0000000，保护器遗留）。</summary>
		private const int 未使用常量2 = -268435456;

		// Token: 0x0400002F RID: 47
		/// <summary>未使用的常量（32771 = 0x8003，保护器遗留）。</summary>
		private const int 未使用常量3 = 32771;

		// Token: 0x04000030 RID: 48
		/// <summary>未使用的常量（保护器遗留）。</summary>
		private const int 未使用常量4 = 2;
	}
}
