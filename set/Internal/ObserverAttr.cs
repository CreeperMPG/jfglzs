using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace set.Internal
{
	/// <summary>
	/// 一个「凑数」的工具类：注册表读写封装 + SHA256 摘要 + WinForms 重启 + 几个 Crypt* P/Invoke 声明。
	///
	/// 对恶意行为最关键的只有两个：
	/// <list type="bullet">
	/// <item><see cref="InstantiateInfo"/>：<c>SHA256(密码 + "bfdshgs")</c> 十六进制串 —— 全样本的密码摘要算法；</item>
	/// <item><see cref="RateInfo"/>：写注册表值的通用封装（DefineInfo 里几十次策略写入全靠它）。</item>
	/// </list>
	/// 另一个重点是静态字段 <see cref="账号进程名"/>（旧名 m_AccountAttr），它由外部程序集写进来，
	/// 被 <c>Form1.CallInfo</c> 当成「要杀掉的账号进程名」使用。
	/// </summary>
	// Token: 0x02000010 RID: 16
	[StandardModule]
	internal sealed class ObserverAttr
	{
		// 以下是 advapi32 的 CryptoAPI 声明，本工程没有任何调用点（加壳工具残留）
		// Token: 0x060000E7 RID: 231
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptAcquireContext(ref IntPtr intptr_0, string string_0, string string_1, int int_0, int int_1);

		// Token: 0x060000E8 RID: 232
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptCreateHash(IntPtr intptr_0, int int_0, IntPtr intptr_1, int int_1, ref IntPtr intptr_2);

		// Token: 0x060000E9 RID: 233
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptHashData(IntPtr intptr_0, byte[] byte_0, int int_0, int int_1);

		// Token: 0x060000EA RID: 234
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptGetHashParam(IntPtr intptr_0, int int_0, byte[] byte_0, ref int int_1, int int_2);

		// Token: 0x060000EB RID: 235
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptDestroyHash(IntPtr intptr_0);

		// Token: 0x060000EC RID: 236
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptReleaseContext(IntPtr intptr_0, int int_0);

		/// <summary>
		/// 密码摘要：<c>hex( SHA256( 原文 + "bfdshgs" ) )</c>。
		/// 加盐字符串 "bfdshgs" 是样本的硬编码盐；结果被写进 <c>HKCU\Software\n</c>，供其它组件比对密码。
		/// </summary>
		/// <param name="原文">明文密码（Trim 过后由调用方传入）。</param>
		// Token: 0x060000ED RID: 237 RVA: 0x0000AFF0 File Offset: 0x000091F0
		public static string InstantiateInfo(string 原文)
		{
			string 摘要;
			using (SHA256 算法 = SHA256.Create())
			{
				byte[] 哈希字节 = 算法.ComputeHash(Encoding.UTF8.GetBytes(原文 + "bfdshgs"));
				StringBuilder 构造 = new StringBuilder();
				foreach (byte 单字节 in 哈希字节)
				{
					构造.Append(单字节.ToString("x2"));
				}
				摘要 = 构造.ToString();
			}
			return 摘要;
		}

		/// <summary>
		/// 启用视觉样式并以 <c>load</c> 窗体起一个独立的 WinForms 消息循环（本工程没有调用点）。
		/// </summary>
		// Token: 0x060000EE RID: 238 RVA: 0x0000B074 File Offset: 0x00009274
		public static void CloneInfo()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			CandidateAttr.窗体集合.load.Show();
			Application.Run();
		}

		/// <summary>
		/// 读注册表值（带默认值）。语义等价于 VB 的：
		/// <c>On Error Resume Next</c> → 打开子键 → 有键就用 <c>GetValue(值名, 默认值)</c>，没键就用默认值。
		/// 注意：出错时返回值是 VB 函数默认值 <c>Nothing</c>（**不是**传进来的 <paramref name="默认值"/>），
		/// 因为赋值语句没执行完；<c>Form1.AwakeInfo</c> 依赖这个行为（读不到就当空串）。
		/// </summary>
		/// <param name="根键">注册表根键（CurrentUser / LocalMachine）。</param>
		/// <param name="子键">子键路径。</param>
		/// <param name="值名">值名。</param>
		/// <param name="默认值">子键或值不存在时使用的默认值。</param>
		// Token: 0x060000EF RID: 239 RVA: 0x0000B0A0 File Offset: 0x000092A0
		public static object SortInfo(RegistryKey 根键, string 子键, string 值名, object 默认值 = null)
		{
			object 结果 = null;
			try
			{
				using (RegistryKey 键 = 根键.OpenSubKey(子键))
				{
					if (键 != null)
					{
						结果 = 键.GetValue(值名, RuntimeHelpers.GetObjectValue(默认值));
					}
					else
					{
						结果 = 默认值;
					}
				}
			}
			catch
			{
				// 原版 On Error Resume Next：读失败就吞掉，结果保持 Nothing
			}
			return 结果;
		}

		/// <summary>
		/// 写注册表值（本样本所有策略写入的唯一入口）：子键存在就直接写，不存在就先建键再写。
		/// 任何异常都被包成 <c>Exception("写入注册表值时发生错误: ...")</c> 抛出 —— 注意它是**抛出**而不是吞掉，
		/// 所以调用方的 try/catch 边界决定策略写入的成败范围。
		/// </summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键">子键路径（不存在会创建）。</param>
		/// <param name="值名">值名。</param>
		/// <param name="值">值（object，VB 弱类型原样传入）。</param>
		/// <param name="类型">值类型，默认 String。</param>
		// Token: 0x060000F0 RID: 240 RVA: 0x0000B168 File Offset: 0x00009368
		public static void RateInfo(RegistryKey 根键, string 子键, string 值名, object 值, RegistryValueKind 类型 = RegistryValueKind.String)
		{
			try
			{
				using (RegistryKey 键 = 根键.OpenSubKey(子键, true))
				{
					if (键 == null)
					{
						using (RegistryKey 新建键 = 根键.CreateSubKey(子键))
						{
							新建键.SetValue(值名, RuntimeHelpers.GetObjectValue(值), 类型);
							// 原版这里 goto 到 using 末尾，相当于「建键写完就结束」，不进下面的分支
						}
					}
					else
					{
						键.SetValue(值名, RuntimeHelpers.GetObjectValue(值), 类型);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("写入注册表值时发生错误: " + ex.Message);
			}
		}

		/// <summary>删除注册表值（不存在的值不报错，<c>throwOnMissingValue: false</c>）。</summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键">子键路径。</param>
		/// <param name="值名">要删除的值名。</param>
		// Token: 0x060000F1 RID: 241 RVA: 0x0000B208 File Offset: 0x00009408
		public static void CountInfo(RegistryKey 根键, string 子键, string 值名)
		{
			try
			{
				using (RegistryKey 键 = 根键.OpenSubKey(子键, true))
				{
					if (键 != null)
					{
						键.DeleteValue(值名, false);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表值时发生错误: " + ex.Message);
			}
		}

		/// <summary>判断注册表子键是否存在（异常一律当不存在）。</summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键">子键路径。</param>
		// Token: 0x060000F2 RID: 242 RVA: 0x0000B274 File Offset: 0x00009474
		public static bool DisableInfo(RegistryKey 根键, string 子键)
		{
			bool 存在;
			try
			{
				using (RegistryKey 键 = 根键.OpenSubKey(子键))
				{
					存在 = 键 != null;
				}
			}
			catch (Exception ex)
			{
				存在 = false;
			}
			return 存在;
		}

		/// <summary>
		/// 确保注册表子键存在：存在就返回 true，不存在就创建后返回 true；出错则抛出包装过的异常。
		/// 注意原版在内层用的是「没有变量的 Using」(<c>Using 根键.CreateSubKey(子键)</c>) + 直接 return，这里照原样保留。
		/// </summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键">子键路径。</param>
		// Token: 0x060000F3 RID: 243 RVA: 0x0000B2C8 File Offset: 0x000094C8
		public static bool ListInfo(RegistryKey 根键, string 子键)
		{
			bool 结果;
			try
			{
				using (RegistryKey 键 = 根键.OpenSubKey(子键))
				{
					if (键 == null)
					{
						using (根键.CreateSubKey(子键))
						{
							return true;
						}
					}
					结果 = true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("确保注册表键存在操作失败: " + ex.Message);
			}
			return 结果;
		}

		/// <summary>删除整个注册表子键（只在它确实存在时删），出错抛出包装异常。</summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键">子键路径。</param>
		// Token: 0x060000F4 RID: 244 RVA: 0x0000B34C File Offset: 0x0000954C
		public static void CalculateInfo(RegistryKey 根键, string 子键)
		{
			try
			{
				if (ObserverAttr.DisableInfo(根键, 子键))
				{
					根键.DeleteSubKey(子键);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		/// <summary>恒返回 false 的死代码桩，无调用点。</summary>
		// Token: 0x060000F5 RID: 245 RVA: 0x000021E8 File Offset: 0x000003E8
		internal static bool 恒假桩()
		{
			return false;
		}

		/// <summary>账号进程名（由外部程序集写入），<c>Form1.CallInfo</c> 拿它当「要杀掉的账号进程名」。</summary>
		// Token: 0x0400006F RID: 111
		public static string 账号进程名;

		/// <summary>访客名占位字段，全工程无引用。</summary>
		// Token: 0x04000070 RID: 112
		public static string 访客名;

		// 下面四个常量全工程无引用（加壳工具残留）
		// Token: 0x04000071 RID: 113
		private const int m_TagAttr = 1;

		// Token: 0x04000072 RID: 114
		private const int _DispatcherAttr = -268435456;

		// Token: 0x04000073 RID: 115
		private const int stubAttr = 32771;

		// Token: 0x04000074 RID: 116
		private const int _ItemAttr = 2;
	}
}
