using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace WindowsApplication1.Internal
{
	/// <summary>
	/// 注册表与密码哈希工具集（本程序集最核心的「持久化/校密码」支撑类）。
	/// 两类真实用途：
	/// 1. 密码哈希：<see cref="计算密码哈希"/> = `SHA256(UTF8(密码 + "bfdshgs"))` 的小写十六进制，
	///    结果被存在 `HKCU\Software\n` 这个值里，作为「退出程序」的密码凭据；
	/// 2. 注册表读写删：全部围绕 `Microsoft.Win32.Registry`，被 Form1/Form3 用来
	///    写浏览器下载策略、USB 存储策略、DisableCMD 等策略键。
	/// 文件顶部那 6 个 CryptoAPI P/Invoke 与 4 个常量**没有任何调用点**，是作者留下的死代码
	/// （真正算哈希走的是托管 SHA256），保留只为对齐元数据。
	/// </summary>
	// Token: 0x0200000D RID: 13
	[StandardModule]
	internal sealed class SetterMapper
	{
		// Token: 0x06000069 RID: 105
		/// <summary>取得 CSP 句柄。未使用。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptAcquireContext(ref IntPtr 提供程序句柄, string 容器名, string 提供程序名, int 提供程序类型, int 标志);

		// Token: 0x0600006A RID: 106
		/// <summary>创建哈希对象。未使用。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptCreateHash(IntPtr 提供程序句柄, int 算法标识, IntPtr 密钥句柄, int 标志, ref IntPtr 哈希句柄);

		// Token: 0x0600006B RID: 107
		/// <summary>把数据喂给哈希对象。未使用。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptHashData(IntPtr 哈希句柄, byte[] 数据, int 数据长度, int 标志);

		// Token: 0x0600006C RID: 108
		/// <summary>取出哈希值。未使用。</summary>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptGetHashParam(IntPtr 哈希句柄, int 参数标识, byte[] 数据, ref int 数据长度, int 标志);

		// Token: 0x0600006D RID: 109
		/// <summary>销毁哈希对象。未使用。</summary>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptDestroyHash(IntPtr 哈希句柄);

		// Token: 0x0600006E RID: 110
		/// <summary>释放 CSP 句柄。未使用。</summary>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptReleaseContext(IntPtr 提供程序句柄, int 标志);

		// Token: 0x0600006F RID: 111 RVA: 0x0000401C File Offset: 0x0000221C
		/// <summary>
		/// 计算小程序的退出密码哈希：`SHA256(UTF8(密码 + "bfdshgs"))`，逐字节拼成小写十六进制串。
		/// 注意那个固定盐 `bfdshgs` 是硬编码在代码里的，因此密码可以被离线爆破。
		/// Form3 里把用户输入用同一函数算一遍，再和注册表里的值做字符串比较。
		/// </summary>
		public static string 计算密码哈希(string 密码)
		{
			string 结果;
			using (SHA256 sha = SHA256.Create())
			{
				byte[] 摘要 = sha.ComputeHash(Encoding.UTF8.GetBytes(密码 + "bfdshgs"));
				StringBuilder 十六进制 = new StringBuilder();
				foreach (byte 字节 in 摘要)
				{
					十六进制.Append(字节.ToString("x2"));
				}
				结果 = 十六进制.ToString();
			}
			return 结果;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000040A0 File Offset: 0x000022A0
		/// <summary>
		/// 读取注册表值（读 `HKCU\Software\n` 这种「键=值」的取数）。
		/// 语义是 VB 的 `On Error Resume Next`：**任何异常都被吞掉，函数返回 null**，
		/// 调用方用「返回空串 ⇒ 还没设过密码」来判断，所以这里绝不能把异常抛出去。
		/// 原版整个 Using 块只对应一条状态（num2 = 2），出错即跳到函数末尾返回，
		/// 因此这里用「整段一个 try/catch」——与逐条忽略错误在可观测行为上完全一致
		/// （块内没有其他有副作用的语句）。
		/// </summary>
		public static object 读取注册表值(RegistryKey 注册表根, string 子键路径, string 值名, object 默认值 = null)
		{
			object 结果 = null;
			try
			{
				using (RegistryKey 注册表键 = 注册表根.OpenSubKey(子键路径))
				{
					if (注册表键 != null)
					{
						结果 = 注册表键.GetValue(值名, RuntimeHelpers.GetObjectValue(默认值));
					}
					else
					{
						结果 = 默认值;
					}
				}
			}
			catch
			{
			}
			return 结果;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004168 File Offset: 0x00002368
		/// <summary>
		/// 写入注册表值（可指定值类型，默认 REG_SZ）。键不存在时**先 CreateSubKey 再写**，
		/// 存在时直接写；两种分支互斥。任何失败都被包成带中文前缀的 Exception 抛出，
		/// 这句「写入注册表值时发生错误: 」是作者自己写的提示文本，不是系统消息。
		/// 原版用 `goto IL_003F` 跳过「已存在键再写一次」的分支，这里还原成 if/else。
		/// </summary>
		public static void 写入注册表值(RegistryKey 注册表根, string 子键路径, string 值名, object 值, RegistryValueKind 值类型 = RegistryValueKind.String)
		{
			try
			{
				using (RegistryKey 注册表键 = 注册表根.OpenSubKey(子键路径, true))
				{
					if (注册表键 == null)
					{
						using (RegistryKey 新建键 = 注册表根.CreateSubKey(子键路径))
						{
							新建键.SetValue(值名, RuntimeHelpers.GetObjectValue(值), 值类型);
						}
					}
					else
					{
						注册表键.SetValue(值名, RuntimeHelpers.GetObjectValue(值), 值类型);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("写入注册表值时发生错误: " + ex.Message);
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004208 File Offset: 0x00002408
		/// <summary>
		/// 删除注册表值。键不存在时直接跳过（不报错）；`DeleteValue(值名, false)` 的第二个参数
		/// 表示「值不存在时不要抛异常」。失败包成中文前缀的 Exception 抛出。
		/// </summary>
		public static void 删除注册表值(RegistryKey 注册表根, string 子键路径, string 值名)
		{
			try
			{
				using (RegistryKey 注册表键 = 注册表根.OpenSubKey(子键路径, true))
				{
					if (注册表键 != null)
					{
						注册表键.DeleteValue(值名, false);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表值时发生错误: " + ex.Message);
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004274 File Offset: 0x00002474
		/// <summary>
		/// 判断注册表键是否存在（只读探测）。打开失败一律当作「不存在」。
		/// 返回值的语义是 `key != null`。
		/// </summary>
		public static bool 注册表键是否存在(RegistryKey 注册表根, string 子键路径)
		{
			bool 结果;
			try
			{
				using (RegistryKey 注册表键 = 注册表根.OpenSubKey(子键路径))
				{
					结果 = 注册表键 != null;
				}
			}
			catch (Exception ex)
			{
				结果 = false;
			}
			return 结果;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000042C8 File Offset: 0x000024C8
		/// <summary>
		/// 确保注册表键存在：不存在则创建（创建后立即 Dispose），存在则什么也不做。
		/// 失败包成中文前缀的 Exception 抛出。
		/// </summary>
		public static bool 确保注册表键存在(RegistryKey 注册表根, string 子键路径)
		{
			bool 结果;
			try
			{
				using (RegistryKey 注册表键 = 注册表根.OpenSubKey(子键路径))
				{
					if (注册表键 == null)
					{
						using (注册表根.CreateSubKey(子键路径))
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

		// Token: 0x06000075 RID: 117 RVA: 0x0000434C File Offset: 0x0000254C
		/// <summary>
		/// 删除整个注册表子键（递归删除）。先探测存在性再删，避免对不存在的键抛异常；
		/// 失败包成中文前缀的 Exception 抛出。
		/// </summary>
		public static void 删除注册表键(RegistryKey 注册表根, string 子键路径)
		{
			try
			{
				if (SetterMapper.注册表键是否存在(注册表根, 子键路径))
				{
					注册表根.DeleteSubKey(子键路径);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>恒返回 true 的桩方法，无调用点。</summary>
		internal static bool 返回真占位()
		{
			return true;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>恒返回 false 的桩方法，无调用点。</summary>
		internal static bool 返回假占位()
		{
			return false;
		}

		// Token: 0x04000022 RID: 34
		/// <summary>
		/// 小助手（监控端）的**当前进程名**。由 Form1 的 Load 处理器按日期算出来，
		/// Form3 的退出流程用它去枚举并结束对应进程。
		/// </summary>
		public static string 小助手进程名;

		// Token: 0x04000023 RID: 35
		/// <summary>小助手可执行文件名（<see cref="小助手进程名"/> + ".exe"），本工程内未被读取。</summary>
		public static string 小助手可执行文件名;

		// Token: 0x04000024 RID: 36
		/// <summary>CryptoAPI 常量 PROV_RSA_FULL，未使用。</summary>
		private const int PROV_RSA_FULL = 1;

		// Token: 0x04000025 RID: 37
		/// <summary>CryptoAPI 常量 CRYPT_VERIFYCONTEXT，未使用。</summary>
		private const int CRYPT_VERIFYCONTEXT = -268435456;

		// Token: 0x04000026 RID: 38
		/// <summary>CryptoAPI 常量 CALG_SHA1，未使用。</summary>
		private const int CALG_SHA1 = 32771;

		// Token: 0x04000027 RID: 39
		/// <summary>CryptoAPI 常量 HP_HASHVAL，未使用。</summary>
		private const int HP_HASHVAL = 2;
	}
}
