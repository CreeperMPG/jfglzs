using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace WindowsApplication1.Internal
{
		/// <summary>
		/// 注册表工具 + SHA256 密码哈希 + 程序入口模块（StandardModule）。
		/// 职责：1) 基于 Microsoft.Win32.RegistryKey 的注册表增删改查封装；
		/// 2) ComputeSaltedSha256：SHA256(输入+"bfdshgs") 十六进制小写，用于密码哈希（HKCU\Software\n）；
		/// 3) ProgramEntry_ShowMainForm：程序入口之一（显示 load 密码窗 + Application.Run）。
		/// 另有 advapi32 Crypt* P/Invoke 声明（CryptoAPI），本文件内未实际调用，疑为壳/后续载荷预留。
		/// </summary>
	// Token: 0x0200000D RID: 13
	[StandardModule]
	internal sealed class ExceptionMock
	{
		/// <summary>
		/// CryptoAPI：获取 CSP 密钥容器句柄（声明但未在本文件调用）。
		/// </summary>
		// Token: 0x060000DE RID: 222
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptAcquireContext(ref IntPtr phProv, string pszContainer, string pszProvider, int dwProvType, int dwFlags);

		/// <summary>
		/// CryptoAPI：创建哈希对象（声明但未在本文件调用）。
		/// </summary>
		// Token: 0x060000DF RID: 223
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptCreateHash(IntPtr hProv, int algId, IntPtr hKey, int dwFlags, ref IntPtr phHash);

		/// <summary>
		/// CryptoAPI：向哈希对象喂入数据（声明但未在本文件调用）。
		/// </summary>
		// Token: 0x060000E0 RID: 224
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptHashData(IntPtr hHash, byte[] pbData, int cbData, int dwFlags);

		/// <summary>
		/// CryptoAPI：取哈希结果参数（声明但未在本文件调用）。
		/// </summary>
		// Token: 0x060000E1 RID: 225
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptGetHashParam(IntPtr hHash, int dwParam, byte[] pbData, ref int pdwDataLen, int dwFlags);

		/// <summary>
		/// CryptoAPI：销毁哈希对象（声明但未在本文件调用）。
		/// </summary>
		// Token: 0x060000E2 RID: 226
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptDestroyHash(IntPtr hHash);

		/// <summary>
		/// CryptoAPI：释放密钥容器上下文（声明但未在本文件调用）。
		/// </summary>
		// Token: 0x060000E3 RID: 227
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptReleaseContext(IntPtr hProv, int dwFlags);

		/// <summary>
		/// 计算密码的加盐 SHA256 哈希：SHA256(UTF8(plainText + "bfdshgs"))，逐字节 ToString("x2") 输出 64 位小写 hex。
		/// 盐 "bfdshgs" 硬编码（IOC），可据此离线爆破密码。
		/// 调用方：SaveSettings_ApplyAllRestrictions 写 HKCU\Software\n；load.Button1_Click_VerifyPassword 比对密码。
		/// </summary>
		// Token: 0x060000E4 RID: 228 RVA: 0x0000B938 File Offset: 0x00009B38
		public static string ComputeSaltedSha256(string plainText)
		{
			string result;
			using (SHA256 sha = SHA256.Create())
			{
				byte[] array = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText + "bfdshgs"));
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte b in array)
				{
					stringBuilder.Append(b.ToString("x2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		/// <summary>
		/// 程序入口之一：启用视觉样式 → 经 Reader.MyForms.load 创建并显示密码门窗体 → Application.Run() 进入消息循环。
		/// 与 Descriptor.Main_ProgramEntry（VB 应用框架入口）互为补充，壳运行时（TestMock.ModuleInit_ApplyMemoryPatch）可能在其中做切换/补丁。
		/// </summary>
		// Token: 0x060000E5 RID: 229 RVA: 0x0000B9C4 File Offset: 0x00009BC4
		public static void ProgramEntry_ShowMainForm()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Reader.MyForms.load.Show();
			Application.Run();
		}

		/// <summary>
		/// 读注册表值：OpenSubKey(subKey) 非空则 GetValue(valueName, defaultValue)，否则返回 defaultValue。
		/// 原 VB On Error 扁平化状态机（num2/num/num4 + @switch + catch when endfilter）已还原为普通 try-catch，真实逻辑如上，与控制流等价。
		/// </summary>
		// Token: 0x060000E6 RID: 230 RVA: 0x0000B9F0 File Offset: 0x00009BF0
		// [反混淆] 原状态机约13行（IL_01~IL_96 + @switch/endfilter 调度）→ 还原为普通 try-catch + using
		public static object ReadRegistryValue(RegistryKey rootKey, string subKey, string valueName, object defaultValue = null)
		{
			object result = null;
			try
			{
				using (RegistryKey registryKey = rootKey.OpenSubKey(subKey))
				{
					if (registryKey != null)
					{
						result = registryKey.GetValue(valueName, RuntimeHelpers.GetObjectValue(defaultValue));
					}
					else
					{
						result = defaultValue;
					}
				}
			}
			catch (Exception ex)
			{
				// 原混淆样板：VB On Error 状态机（endfilter/ProjectData）已还原，异常时按原语义跳过（result 保持 null）
			}
			return result;
		}

		/// <summary>
		/// 写注册表值：以可写方式打开子键，不存在则 CreateSubKey 后 SetValue；异常抛 "写入注册表值时发生错误: ..."。
		/// 全样本几乎所有恶意注册表写入（usbstor/RUN/策略键等）都经由此方法。
		/// </summary>
		// Token: 0x060000E7 RID: 231 RVA: 0x0000BAB8 File Offset: 0x00009CB8
		public static void WriteRegistryValue(RegistryKey rootKey, string subKey, string valueName, object value, RegistryValueKind valueKind = RegistryValueKind.String)
		{
			try
			{
				using (RegistryKey registryKey = rootKey.OpenSubKey(subKey, true))
				{
					if (registryKey == null)
					{
						using (RegistryKey registryKey2 = rootKey.CreateSubKey(subKey))
						{
							registryKey2.SetValue(valueName, RuntimeHelpers.GetObjectValue(value), valueKind);
							return;
						}
					}
					registryKey.SetValue(valueName, RuntimeHelpers.GetObjectValue(value), valueKind);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("写入注册表值时发生错误: " + ex.Message);
			}
		}

		/// <summary>
		/// 删除注册表值：子键存在则 DeleteValue；异常转中文错误。
		/// </summary>
		// Token: 0x060000E8 RID: 232 RVA: 0x0000BB58 File Offset: 0x00009D58
		public static void DeleteRegistryValue(RegistryKey rootKey, string subKey, string valueName)
		{
			try
			{
				using (RegistryKey registryKey = rootKey.OpenSubKey(subKey, true))
				{
					if (registryKey != null)
					{
						registryKey.DeleteValue(valueName, false);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表值时发生错误: " + ex.Message);
			}
		}

		/// <summary>
		/// 判断注册表键是否存在：OpenSubKey != null；异常视为不存在。
		/// </summary>
		// Token: 0x060000E9 RID: 233 RVA: 0x0000BBC0 File Offset: 0x00009DC0
		public static bool RegistryKeyExists(RegistryKey rootKey, string subKey)
		{
			bool result;
			try
			{
				using (RegistryKey registryKey = rootKey.OpenSubKey(subKey))
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

		/// <summary>
		/// 确保注册表键存在：不存在则 CreateSubKey；异常转中文错误。
		/// </summary>
		// Token: 0x060000EA RID: 234 RVA: 0x0000BC18 File Offset: 0x00009E18
		public static bool EnsureRegistryKeyExists(RegistryKey rootKey, string subKey)
		{
			bool result;
			try
			{
				using (RegistryKey registryKey = rootKey.OpenSubKey(subKey))
				{
					if (registryKey == null)
					{
						using (rootKey.CreateSubKey(subKey))
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

		/// <summary>
		/// 删除注册表键：先 RegistryKeyExists 判断，存在则 DeleteSubKey；异常转中文错误。
		/// </summary>
		// Token: 0x060000EB RID: 235 RVA: 0x0000BC9C File Offset: 0x00009E9C
		public static void DeleteRegistryKey(RegistryKey rootKey, string subKey)
		{
			try
			{
				if (ExceptionMock.RegistryKeyExists(rootKey, subKey))
				{
					rootKey.DeleteSubKey(subKey);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		/// <summary>
		/// 随机进程名（无扩展名）：由 load.Form_Load_GenerateRandomProcessName 依日期生成；
		/// Form1.PreSaveCleanup_KillOldProcesses 用它强杀"上一个/同族"随机名进程（防多实例互杀）。
		/// </summary>
		// Token: 0x0400006A RID: 106
		public static string RandomProcessName;

		/// <summary>
		/// 随机进程名 + ".exe"（与 RandomProcessName 同步由 load 生成，实际杀进程用）。
		/// </summary>
		// Token: 0x0400006B RID: 107
		public static string RandomProcessNameWithExe;

		/// <summary>
		/// VB 编译器错误处理样板常量（混淆残留，代码未直接使用）。
		/// </summary>
		// Token: 0x0400006C RID: 108
		private const int VbErrorCode_1 = 1;

		/// <summary>
		/// VB ProjectData 错误处理常量 0xF0000000（混淆残留，代码未直接使用）。
		/// </summary>
		// Token: 0x0400006D RID: 109
		private const int VbErrorCode_Neg268435456 = -268435456;

		/// <summary>
		/// VB 错误处理常量 0x8003（混淆残留，代码未直接使用）。
		/// </summary>
		// Token: 0x0400006E RID: 110
		private const int VbErrorCode_32771 = 32771;

		/// <summary>
		/// VB 编译器错误处理样板常量（混淆残留，代码未直接使用）。
		/// </summary>
		// Token: 0x0400006F RID: 111
		private const int VbErrorCode_2 = 2;
	}
}
