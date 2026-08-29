using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace WindowsApplication1.Internal
{
	// Token: 0x0200000F RID: 15
	[StandardModule]
	// ============================================================
	// Internal/ModelVisitor.cs — 注册表操作封装 + 密码哈希 + 全局配置变量
	// 静态工具类：封装注册表读写/删除/键存在性检查（VB My.Computer.Registry 的混淆化包装）；SHA256 加盐密码哈希 HashPasswordSha256（盐 "bfdshgs"）；全局变量：随机进程名（randomProcessName/randomProcessNameExe，由 Form1 每日生成）、水印名（watermarkClassroomTerminator/watermarkYiYuYanProgram）。文件头声明一组 advapi32 CryptoAPI P/Invoke（声明未调用）。
	// ============================================================
	internal sealed class ModelVisitor
	{
		// Token: 0x060000BA RID: 186
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		/// <summary>
		/// P/Invoke：advapi32.CryptAcquireContext —— CryptoAPI 上下文（声明未调用，壳/混淆残留）。
		/// </summary>
		private static extern bool CryptAcquireContext(ref IntPtr intptr_0, string string_0, string string_1, int int_0, int int_1);

		// Token: 0x060000BB RID: 187
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		/// <summary>
		/// P/Invoke：advapi32.CryptCreateHash —— 创建哈希对象（声明未调用，残留）。
		/// </summary>
		private static extern bool CryptCreateHash(IntPtr intptr_0, int int_0, IntPtr intptr_1, int int_1, ref IntPtr intptr_2);

		// Token: 0x060000BC RID: 188
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		/// <summary>
		/// P/Invoke：advapi32.CryptHashData —— 哈希数据（声明未调用，残留）。
		/// </summary>
		private static extern bool CryptHashData(IntPtr intptr_0, byte[] byte_0, int int_0, int int_1);

		// Token: 0x060000BD RID: 189
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		/// <summary>
		/// P/Invoke：advapi32.CryptGetHashParam —— 取哈希结果（声明未调用，残留）。
		/// </summary>
		private static extern bool CryptGetHashParam(IntPtr intptr_0, int int_0, byte[] byte_0, ref int int_1, int int_2);

		// Token: 0x060000BE RID: 190
		[DllImport("advapi32.dll", SetLastError = true)]
		/// <summary>
		/// P/Invoke：advapi32.CryptDestroyHash —— 销毁哈希对象（声明未调用，残留）。
		/// </summary>
		private static extern bool CryptDestroyHash(IntPtr intptr_0);

		// Token: 0x060000BF RID: 191
		[DllImport("advapi32.dll", SetLastError = true)]
		/// <summary>
		/// P/Invoke：advapi32.CryptReleaseContext —— 释放上下文（声明未调用，残留）。
		/// </summary>
		private static extern bool CryptReleaseContext(IntPtr intptr_0, int int_0);

		// Token: 0x060000C0 RID: 192 RVA: 0x00009898 File Offset: 0x00007A98
		/// <summary>
		/// SHA256 密码哈希（原名 FillConfig）：SHA256.ComputeHash(UTF8(明文 + "bfdshgs")) → 逐字节 x2 十六进制小写拼接。
		/// 用途：Form2/Form4 密码校验（与 HKCU\Software\n、HKCU\Software\jfglzs\n2 比对）。
		/// </summary>
		public static string HashPasswordSha256(string string_0)
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

		// Token: 0x060000C1 RID: 193 RVA: 0x00009924 File Offset: 0x00007B24
		/// <summary>
		/// 读注册表值（原名 DeleteConfig）：OpenSubKey(路径) 非空→GetValue(值名, 默认)；为空→返回默认值。
		/// </summary>
		// [反混淆] 原状态机 ~60行 → 还原为 using + if/else
		public static object ReadRegistryValue(RegistryKey registryKey_0, string string_0, string string_1, object object_0 = null)
		{
			try
			{
				using (RegistryKey registryKey = registryKey_0.OpenSubKey(string_0))
				{
					if (registryKey != null)
					{
						return registryKey.GetValue(string_1, RuntimeHelpers.GetObjectValue(object_0));
					}
					return object_0;
				}
			}
			catch (Exception ex) { /* 原混淆样板：VB On Error 错误处理，原实现重抛 ProjectData 错误，此处忽略 */ }
			return null;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000099EC File Offset: 0x00007BEC
		/// <summary>
		/// 写注册表值（原名 AwakeConfig）：OpenSubKey(路径, true) 为空→CreateSubKey 后 SetValue；否则直接 SetValue。异常统一抛"写入注册表值时发生错误: ..."。
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

		// Token: 0x060000C3 RID: 195 RVA: 0x00009A8C File Offset: 0x00007C8C
		/// <summary>
		/// 删除注册表值（原名 CancelConfig）：DeleteValue(值名, throwOnMissing=false)。异常抛中文错误。
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

		// Token: 0x060000C4 RID: 196 RVA: 0x00009AF4 File Offset: 0x00007CF4
		/// <summary>
		/// 检查注册表键是否存在（原名 PrintConfig）：OpenSubKey != null。
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

		// Token: 0x060000C5 RID: 197 RVA: 0x00009B4C File Offset: 0x00007D4C
		/// <summary>
		/// 确保注册表键存在（原名 QueryConfig）：不存在则 CreateSubKey，返回 true。
		/// </summary>
		public static bool EnsureRegistryKey(RegistryKey registryKey_0, string string_0)
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

		// Token: 0x060000C6 RID: 198 RVA: 0x00009BD0 File Offset: 0x00007DD0
		/// <summary>
		/// 删除注册表键（原名 ForgotConfig）：存在才 DeleteSubKey。异常抛"删除注册表键时发生错误"。
		/// </summary>
		public static void DeleteRegistryKey(RegistryKey registryKey_0, string string_0)
		{
			try
			{
				if (ModelVisitor.RegistryKeyExists(registryKey_0, string_0))
				{
					registryKey_0.DeleteSubKey(string_0);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		// Token: 0x040000A4 RID: 164
		/// <summary>
		/// 随机进程名（无扩展名，原名 _TestsVisitor）：由 Form1.Form1_Load 每日生成（5 位，字符 i~r），用于释放/击杀同伴进程。
		/// </summary>
		public static string randomProcessName;

		// Token: 0x040000A5 RID: 165
		/// <summary>
		/// 随机进程名（带 .exe，原名 m_ConsumerVisitor）= randomProcessName + ".exe"。
		/// </summary>
		public static string randomProcessNameExe;

		// Token: 0x040000A6 RID: 166
		/// <summary>
		/// 锁屏窗体可否关闭标志（原名 _RegVisitor）：Form4.FormClosing 读取（false 可关，true 禁止），可能由同伴/服务端切换。
		/// </summary>
		public static bool lockFormCanClose = false;

		// Token: 0x040000A7 RID: 167
		/// <summary>
		/// 常量 1（Crypt API 相关常量残留，未使用）。
		/// </summary>
		private const int roleVisitor = 1;

		// Token: 0x040000A8 RID: 168
		/// <summary>
		/// 常量 -268435456（Crypt API 相关常量残留，未使用）。
		/// </summary>
		private const int _FacadeVisitor = -268435456;

		// Token: 0x040000A9 RID: 169
		/// <summary>
		/// 常量 32771（Crypt API 相关常量残留，未使用）。
		/// </summary>
		private const int m_RefVisitor = 32771;

		// Token: 0x040000AA RID: 170
		/// <summary>
		/// 常量 2（Crypt API 相关常量残留，未使用）。
		/// </summary>
		private const int annotationVisitor = 2;

		// Token: 0x040000AB RID: 171
		/// <summary>
		/// 水印字符串"教室终结者"（原名 _WatcherVisitor）：用于产品黑名单。
		/// </summary>
		public static string watermarkClassroomTerminator = "教室终结者";

		// Token: 0x040000AC RID: 172
		/// <summary>
		/// 水印字符串"易语言程序"（原名 _RepositoryVisitor）：用于产品黑名单。
		/// </summary>
		public static string watermarkYiYuYanProgram = "易语言程序";
	}
}
