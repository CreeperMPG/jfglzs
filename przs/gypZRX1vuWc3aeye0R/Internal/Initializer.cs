using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

// ============================================================================
// Initializer.cs — 注册表工具库 + SHA256 口令哈希 + 全局配置字段
// 真实职责：反混淆后为注册表读写/工具静态类（ConfuserEx 改名 Initializer），外加全局配置字段
// （程序安装目录、360 下载目录名、锁屏放行标志）。Form1/Form2 大量调用：
// 读注册表值/写注册表值/口令加盐哈希。类内残留一组未使用的 CryptoAPI P/Invoke 与常量（壳/混淆器残留）。
// 说明：类名保持混淆原名，方法/字段已按 Initializer.cs.md 映射表反混淆重命名。
// ============================================================================
namespace gypZRX1vuWc3aeye0R.Internal
{
	// Token: 0x0200000F RID: 15
	[StandardModule]
	internal sealed class Initializer
	{
		// Token: 0x06000091 RID: 145
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
/// <summary>未使用的 CryptoAPI P/Invoke 残留（壳/混淆器注入，无调用点）：CryptAcquireContext。</summary>
		private static extern bool CryptAcquireContext(ref IntPtr intptr_0, string string_0, string string_1, int int_0, int int_1);

		// Token: 0x06000092 RID: 146
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
/// <summary>未使用的 CryptoAPI P/Invoke 残留（无调用点）：CryptCreateHash。</summary>
		private static extern bool CryptCreateHash(IntPtr intptr_0, int int_0, IntPtr intptr_1, int int_1, ref IntPtr intptr_2);

		// Token: 0x06000093 RID: 147
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
/// <summary>未使用的 CryptoAPI P/Invoke 残留（无调用点）：CryptHashData。</summary>
		private static extern bool CryptHashData(IntPtr intptr_0, byte[] byte_0, int int_0, int int_1);

		// Token: 0x06000094 RID: 148
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
/// <summary>未使用的 CryptoAPI P/Invoke 残留（无调用点）：CryptGetHashParam。</summary>
		private static extern bool CryptGetHashParam(IntPtr intptr_0, int int_0, byte[] byte_0, ref int int_1, int int_2);

		// Token: 0x06000095 RID: 149
		[DllImport("advapi32.dll", SetLastError = true)]
/// <summary>未使用的 CryptoAPI P/Invoke 残留（无调用点）：CryptDestroyHash。</summary>
		private static extern bool CryptDestroyHash(IntPtr intptr_0);

		// Token: 0x06000096 RID: 150
		[DllImport("advapi32.dll", SetLastError = true)]
/// <summary>未使用的 CryptoAPI P/Invoke 残留（无调用点）：CryptReleaseContext。</summary>
		private static extern bool CryptReleaseContext(IntPtr intptr_0, int int_0);

		// Token: 0x06000097 RID: 151 RVA: 0x000070EC File Offset: 0x000052EC
/// <summary>
/// 口令加盐哈希：SHA256(UTF8(输入 + "bfdshgs"))，每字节 ToString("x2") 拼接成 64 位十六进制串。
/// Form2 退出密码比对用（密码 + 固定盐 "bfdshgs"）。
/// </summary>
		public static string 口令加盐哈希(string string_0)
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

		// Token: 0x06000098 RID: 152 RVA: 0x00007178 File Offset: 0x00005378
/// <summary>
/// 读注册表值：OpenSubKey(子键)；键存在 → GetValue(值名, 默认值)；不存在 → 返回默认值
/// （VB 错误状态机包裹，异常走 ProjectData.CreateProjectError）。
/// </summary>
		public static object 读注册表值(RegistryKey registryKey_0, string string_0, string string_1, object object_0 = null)
		{
			// [反混淆] 原状态机56行 → 还原为正常控制流
			object result;
			try
			{
				using (RegistryKey subKey = registryKey_0.OpenSubKey(string_0))
				{
					if (subKey != null)
					{
						result = subKey.GetValue(string_1, RuntimeHelpers.GetObjectValue(object_0));
					}
					else
					{
						result = object_0;
					}
				}
			}
			catch (Exception ex)
			{
				// [反混淆] 原 VB 错误状态机：异常统一转抛 ProjectData.CreateProjectError(-2146828237)（endfilter 样板），保持原错误契约
				throw ProjectData.CreateProjectError(-2146828237);
			}
			return result;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00007240 File Offset: 0x00005440
/// <summary>
/// 写注册表值：OpenSubKey(子键, writable:true)；为 null 则 CreateSubKey 后 SetValue；否则直接
/// SetValue(值名, 值, ValueKind)。失败抛"写入注册表值时发生错误: …"。
/// 被 Form1 用于写禁止下载策略、usbstor 禁用、自启动等。
/// </summary>
		public static void 写注册表值(RegistryKey registryKey_0, string string_0, string string_1, object object_0, RegistryValueKind registryValueKind_0 = RegistryValueKind.String)
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

		// Token: 0x0600009A RID: 154 RVA: 0x000072E0 File Offset: 0x000054E0
/// <summary>删除注册表值：DeleteValue（键不存在时忽略）。失败抛"删除注册表值时发生错误: …"。</summary>
		public static void 删除注册表值(RegistryKey registryKey_0, string string_0, string string_1)
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

		// Token: 0x0600009B RID: 155 RVA: 0x00007348 File Offset: 0x00005548
/// <summary>检查注册表键存在：OpenSubKey != null；异常时返回 false。</summary>
		public static bool 检查注册表键存在(RegistryKey registryKey_0, string string_0)
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

		// Token: 0x0600009C RID: 156 RVA: 0x000073A0 File Offset: 0x000055A0
/// <summary>
/// 确保键存在：不存在则 CreateSubKey 并返回 true；失败抛"确保注册表键存在操作失败: …"。
/// </summary>
		public static bool 确保键存在(RegistryKey registryKey_0, string string_0)
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

		// Token: 0x0600009D RID: 157 RVA: 0x00007424 File Offset: 0x00005624
/// <summary>删除注册表键：存在则 DeleteSubKey。失败抛"删除注册表键时发生错误: …"。</summary>
		public static void 删除注册表键(RegistryKey registryKey_0, string string_0)
		{
			try
			{
				if (Initializer.检查注册表键存在(registryKey_0, string_0))
				{
					registryKey_0.DeleteSubKey(string_0);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		// Token: 0x04000049 RID: 73
/// <summary>未使用字段（混淆器/壳残留）。</summary>
		public static object _Filter;

		// Token: 0x0400004A RID: 74
/// <summary>未使用字段（混淆器/壳残留）。</summary>
		public static object system;

		// Token: 0x0400004B RID: 75
/// <summary>程序安装目录：由 Form1 从 HKLM\...\Run\prozs 解析得到（去掉路径末尾 9 字符）。</summary>
		public static string 程序安装目录;

		// Token: 0x0400004C RID: 76
/// <summary>锁屏放行标志：false 时 Form4 可关闭并解除鼠标锁定（默认 false）。</summary>
		public static bool 锁屏放行标志 = false;

		// Token: 0x0400004D RID: 77
/// <summary>360 安全浏览器下载目录名：Form1 全盘查找并监视的目录之一。</summary>
		public static string 360安全浏览器下载目录名 = "360安全浏览器下载";

		// Token: 0x0400004E RID: 78
/// <summary>360 极速浏览器下载目录名：Form1 全盘查找并监视的目录之一。</summary>
		public static string 360极速浏览器下载目录名 = "360极速浏览器下载";

		// Token: 0x0400004F RID: 79
/// <summary>未使用常量残留（CryptoAPI PROV/ALG 标志，壳/混淆器注入）。</summary>
		private const int proxy = 1;

		// Token: 0x04000050 RID: 80
/// <summary>未使用常量残留（0xF0000000，CryptoAPI PROV 标志）。</summary>
		private const int _Algo = -268435456;

		// Token: 0x04000051 RID: 81
/// <summary>未使用常量残留（0x8001，CALG_MD5）。</summary>
		private const int m_Helper = 32771;

		// Token: 0x04000052 RID: 82
/// <summary>未使用常量残留（CryptoAPI 标志）。</summary>
		private const int m_Bridge = 2;
	}
}
