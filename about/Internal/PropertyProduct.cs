using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace about.Internal
{
	// Token: 0x0200000A RID: 10
	[StandardModule]
	/// <summary>
	/// 全局工具模块（VB.NET StandardModule，原名 PropertyProduct）：
	/// 封装注册表值的读（ReadRegistryValue）、写（WriteRegistryValue）、删（DeleteRegistryValue / DeleteRegistryKey）、
	/// 键存在性判断（RegistryKeyExists / EnsureRegistryKeyExists），并提供密码哈希 ComputePasswordHash（SHA256+盐 "bfdshgs"）。
	/// 持有全局静态字段 MonitorProcessName / MonitorProcessNameWithExe（随机监控进程名），由 Form1 写入、Form3 按名杀进程，
	/// 是跨窗体共享「监控进程身份」的枢纽。
	/// 文件头部残留一组 advapi32.dll 加密 API 的 DllImport（CryptAcquireContext 等），本文件中未被调用，疑为加壳/混淆器残留。
	/// </summary>
	internal sealed class PropertyProduct
	{
		// Token: 0x06000060 RID: 96
		/// <summary>advapi32 CryptAcquireContext（壳残留，未使用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptAcquireContext(ref IntPtr intptr_0, string string_0, string string_1, int int_0, int int_1);

		// Token: 0x06000061 RID: 97
		/// <summary>advapi32 CryptCreateHash（壳残留，未使用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptCreateHash(IntPtr intptr_0, int int_0, IntPtr intptr_1, int int_1, ref IntPtr intptr_2);

		// Token: 0x06000062 RID: 98
		/// <summary>advapi32 CryptHashData（壳残留，未使用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptHashData(IntPtr intptr_0, byte[] byte_0, int int_0, int int_1);

		// Token: 0x06000063 RID: 99
		/// <summary>advapi32 CryptGetHashParam（壳残留，未使用）。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptGetHashParam(IntPtr intptr_0, int int_0, byte[] byte_0, ref int int_1, int int_2);

		// Token: 0x06000064 RID: 100
		/// <summary>advapi32 CryptDestroyHash（壳残留，未使用）。</summary>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptDestroyHash(IntPtr intptr_0);

		// Token: 0x06000065 RID: 101
		/// <summary>advapi32 CryptReleaseContext（壳残留，未使用）。</summary>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptReleaseContext(IntPtr intptr_0, int int_0);

		// Token: 0x06000066 RID: 102 RVA: 0x0000434C File Offset: 0x0000254C
		/// <summary>
		/// 密码哈希（原名 ExcludeReponse）：SHA256.Create() → ComputeHash(UTF8(输入 + "bfdshgs")) →
		/// 每字节 ToString("x2") 拼接小写十六进制。盐 "bfdshgs" 硬编码，与 Form3.OnExitButtonClick 配合校验密码。
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

		// Token: 0x06000067 RID: 103 RVA: 0x000043D8 File Offset: 0x000025D8
		/// <summary>
		/// 读注册表值（原名 PostReponse）：OpenSubKey(sub) 打开子键，非空则 GetValue(name, defaultValue)，
		/// 子键不存在返回默认值。
		/// [反混淆] 原 IL 状态机（num2 = 2，约60行）已还原为正常 try-catch-return 控制流；
		/// 原异常路径经 VB 错误处理重抛 ProjectData.CreateProjectError(-2146828237)，此处保留。
		/// </summary>
		public static object ReadRegistryValue(RegistryKey registryKey_0, string string_0, string string_1, object object_0 = null)
		{
			// [反混淆] 原状态机约60行 → 还原为正常控制流
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
			catch (Exception ex)
			{
				// [反混淆] 原混淆样板：异常经 VB 错误处理重抛为项目错误（-2146828237）
				throw ProjectData.CreateProjectError(-2146828237);
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000044A0 File Offset: 0x000026A0
		/// <summary>
		/// 写/创建注册表值（原名 ChangeReponse）：OpenSubKey(sub, writable:true)，存在则 SetValue，
		/// 不存在则 CreateSubKey 后 SetValue；异常统一包装抛「写入注册表值时发生错误: …」。
		/// Form1/Form3 全部策略修改（usbstor、DisableCMD、浏览器策略、密码哈希存储）都经由此方法执行。
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

		// Token: 0x06000069 RID: 105 RVA: 0x00004540 File Offset: 0x00002740
		/// <summary>
		/// 删除注册表值（原名 AssetReponse）：子键存在则 DeleteValue(name)；失败抛「删除注册表值时发生错误: …」。
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

		// Token: 0x0600006A RID: 106 RVA: 0x000045A8 File Offset: 0x000027A8
		/// <summary>
		/// 判断子键是否存在（原名 UpdateReponse）：OpenSubKey 结果非空即存在；异常视为 false。
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

		// Token: 0x0600006B RID: 107 RVA: 0x00004600 File Offset: 0x00002800
		/// <summary>
		/// 确保子键存在（原名 NewReponse）：不存在则创建并返回 true；创建失败抛「确保注册表键存在操作失败: …」。
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

		// Token: 0x0600006C RID: 108 RVA: 0x00004684 File Offset: 0x00002884
		/// <summary>
		/// 删除子键（原名 PrintReponse）：存在才删（经 RegistryKeyExists 判断）；失败抛「删除注册表键时发生错误: …」。
		/// </summary>
		public static void DeleteRegistryKey(RegistryKey registryKey_0, string string_0)
		{
			try
			{
				if (PropertyProduct.RegistryKeyExists(registryKey_0, string_0))
				{
					registryKey_0.DeleteSubKey(string_0);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		// Token: 0x0400001D RID: 29
		/// <summary>
		/// 随机监控进程名（无 .exe，原名 _SystemProduct）：由 Form1.GenerateRandomMonitorProcessName 在 Load 时写入，
		/// Form3.KillMonitorProcessesAndServices 读取并按此名杀进程。
		/// </summary>
		public static string MonitorProcessName;

		// Token: 0x0400001E RID: 30
		/// <summary>随机监控进程名 + ".exe"（原名 indexerProduct），与 MonitorProcessName 同步写入。</summary>
		public static string MonitorProcessNameWithExe;

		// Token: 0x0400001F RID: 31
		/// <summary>混淆器添加的无用常量（原名 m_StateProduct），无引用。</summary>
		private const int m_StateProduct = 1;

		// Token: 0x04000020 RID: 32
		/// <summary>混淆器添加的无用常量（原名 methodProduct），无引用。</summary>
		private const int methodProduct = -268435456;

		// Token: 0x04000021 RID: 33
		/// <summary>混淆器添加的无用常量（原名 workerProduct），无引用。</summary>
		private const int workerProduct = 32771;

		// Token: 0x04000022 RID: 34
		/// <summary>混淆器添加的无用常量（原名 prototypeProduct），无引用。</summary>
		private const int prototypeProduct = 2;
	}
}
