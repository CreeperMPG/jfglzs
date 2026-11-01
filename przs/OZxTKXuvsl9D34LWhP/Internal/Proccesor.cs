using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace OZxTKXuvsl9D34LWhP.Internal
{
	/// <summary>
	/// 注册表 + 哈希工具类（类名 Proccesor 是混淆名，拼写错误也保留原样）。
	///
	/// 真实职责：
	/// <list type="bullet">
	/// <item>一组带统一异常包装的注册表读写/删操作（读值、写值、删值、判存在、确保存在、删键）；</item>
	/// <item>一个"SHA256(明文 + 固定盐 bfdshgs) → 小写十六进制"的摘要函数，用于生成机器码/授权串；</item>
	/// <item>保存几个跨窗体共享的全局状态：程序所在目录、两个"360 浏览器下载"目录名、锁屏开关。</item>
	/// </list>
	/// 注意：本类顶部的 6 个 advapi32 声明（CryptAcquireContext 那一套）**没有被任何代码调用**，
	/// 是原作者遗留（可能早期做过 MD5 校验）或加壳器残留，见 md 里的说明。
	/// </summary>
	[StandardModule]
	internal sealed class Proccesor
	{
		// Token: 0x0600009C RID: 156
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptAcquireContext(ref IntPtr 加密服务提供者句柄, string 容器名, string 提供者名, int 提供者类型, int 标志);

		// Token: 0x0600009D RID: 157
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptCreateHash(IntPtr 加密服务提供者句柄, int 算法ID, IntPtr 密钥句柄, int 标志, ref IntPtr 哈希句柄);

		// Token: 0x0600009E RID: 158
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptHashData(IntPtr 哈希句柄, byte[] 数据, int 数据长度, int 标志);

		// Token: 0x0600009F RID: 159
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CryptGetHashParam(IntPtr 哈希句柄, int 参数类型, byte[] 输出缓冲, ref int 缓冲长度, int 标志);

		// Token: 0x060000A0 RID: 160
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptDestroyHash(IntPtr 哈希句柄);

		// Token: 0x060000A1 RID: 161
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool CryptReleaseContext(IntPtr 加密服务提供者句柄, int 标志);

		// Token: 0x060000A2 RID: 162 RVA: 0x00006AD4 File Offset: 0x00004CD4
		/// <summary>
		/// 计算"机器码/授权码"用的摘要（原名 UpdateVal）：
		/// SHA256(UTF-8(明文 + "bfdshgs")) 转成小写十六进制字符串。
		/// 盐固定为 <c>bfdshgs</c>。
		/// </summary>
		/// <param name="原文">参与摘要的原文（调用方传的通常是硬盘序列号或网卡信息）。</param>
		public static string 计算SHA256加盐(string 原文)
		{
			string 十六进制;
			using (SHA256 sha = SHA256.Create())
			{
				byte[] 摘要 = sha.ComputeHash(Encoding.UTF8.GetBytes(原文 + "bfdshgs"));
				StringBuilder 拼接器 = new StringBuilder();
				foreach (byte 单字节 in 摘要)
				{
					拼接器.Append(单字节.ToString("x2"));
				}
				十六进制 = 拼接器.ToString();
			}
			return 十六进制;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00006B58 File Offset: 0x00004CD4
		/// <summary>
		/// 读注册表值（原名 InsertVal），读不到或出错返回默认值（默认值本身可为 null）。
		/// 原版是 VB 的 <c>On Error Resume Next</c>：反编译出来的
		/// <c>ProjectData.ClearProjectError()</c>、<c>num = 1</c>、<c>num2 = 2/3</c>、
		/// <c>catch when (endfilter(...))</c>、<c>num4 = num2; switch(...)</c> 恢复表、
		/// <c>throw ProjectData.CreateProjectError(-2146828237)</c>（"无法恢复"）全是编译器生成的错误恢复机制，
		/// 作用就是**出错丢掉、继续下一条**。
		/// 这里把 <c>值</c> 先置 null（VB 里 Object 局部变量的初值就是 Nothing），
		/// 出错时 catch 留空直接返回它——与原版（出错的语句之后已没有别的赋值）结果一致。
		/// </summary>
		/// <param name="根键">HKEY_LOCAL_MACHINE / HKEY_CURRENT_USER 等。</param>
		/// <param name="子键路径">子键相对路径。</param>
		/// <param name="值名">值名称。</param>
		/// <param name="默认值">值不存在时返回的内容。</param>
		public static object 读取注册表值(RegistryKey 根键, string 子键路径, string 值名, object 默认值 = null)
		{
			object 值 = null;
			try
			{
				using (RegistryKey 子键 = 根键.OpenSubKey(子键路径))
				{
					if (子键 != null)
					{
						值 = 子键.GetValue(值名, RuntimeHelpers.GetObjectValue(默认值));
					}
					else
					{
						值 = 默认值;
					}
				}
			}
			catch
			{
				// 原版 On Error Resume Next：这里吞掉异常，继续往下返回 值
			}
			return 值;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006C20 File Offset: 0x00004CD4
		/// <summary>
		/// 写注册表值（原名 WriteVal）：子键不存在就先建，再 SetValue；失败抛
		/// <c>写入注册表值时发生错误: …</c>。
		/// 原版的 <c>goto IL_003F</c> 是 VB 给"using 块内提前跳出"生成的跳转，
		/// 语义就是"新建子键的分支写完直接结束，不再走下面那行"→ 还原成 if/else。
		/// </summary>
		/// <param name="根键">根键。</param>
		/// <param name="子键路径">子键相对路径。</param>
		/// <param name="值名">值名称。</param>
		/// <param name="值">要写入的数据。</param>
		/// <param name="值类型">默认 <see cref="F:Microsoft.Win32.RegistryValueKind.String"/>。</param>
		public static void 写入注册表值(RegistryKey 根键, string 子键路径, string 值名, object 值, RegistryValueKind 值类型 = RegistryValueKind.String)
		{
			try
			{
				using (RegistryKey 子键 = 根键.OpenSubKey(子键路径, true))
				{
					if (子键 == null)
					{
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

		// Token: 0x060000A5 RID: 165 RVA: 0x00006CC0 File Offset: 0x00004EC0
		/// <summary>
		/// 删除注册表值（原名 ListVal），值不存在时**不**报错（<c>throwOnMissingValue: false</c>）。
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

		// Token: 0x060000A6 RID: 166 RVA: 0x00006D2C File Offset: 0x00004D2C
		/// <summary>判断注册表子键是否存在（原名 PatchVal），出错一律当"不存在"。</summary>
		public static bool 注册表键是否存在(RegistryKey 根键, string 子键路径)
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

		// Token: 0x060000A7 RID: 167 RVA: 0x00006D80 File Offset: 0x00004D80
		/// <summary>
		/// 确保注册表子键存在（原名 StopVal）：不存在就 CreateSubKey，存在也返回 true。
		/// 注意原版写法把 CreateSubKey 的结果直接放进 <c>using</c> 里、立刻释放（建完就用完），
		/// 这里保持同样的结构，不要"顺手"改成持有句柄。
		/// </summary>
		public static bool 确保注册表键存在(RegistryKey 根键, string 子键路径)
		{
			bool 成功;
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
					成功 = true;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("确保注册表键存在操作失败: " + ex.Message);
			}
			return 成功;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006E04 File Offset: 0x00004CD4
		/// <summary>
		/// 删除整个注册表子键（原名 ChangeVal）：先 <see cref="注册表键是否存在"/>，
		/// 存在才 <c>DeleteSubKey</c>（不带递归，子键非空会抛异常 → 被包装后抛出）。
		/// </summary>
		public static void 删除注册表键(RegistryKey 根键, string 子键路径)
		{
			try
			{
				if (Proccesor.注册表键是否存在(根键, 子键路径))
				{
					根键.DeleteSubKey(子键路径);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("删除注册表键时发生错误: " + ex.Message);
			}
		}

		// Token: 0x0400004E RID: 78
		/// <summary>全工程没有任何读写的遗留字段（原名 _Map）。</summary>
		public static object _Map;

		// Token: 0x0400004F RID: 79
		/// <summary>全工程没有任何读写的遗留字段（原名 m_Setter）。</summary>
		public static object m_Setter;

		// Token: 0x04000050 RID: 80
		/// <summary>
		/// 本程序所在目录（原名 m_Filter）。Form1 里用"自身路径去掉末尾 9 个字符"算出来，
		/// 之后被用来拼 <c>jfglzsn.exe</c>、<c>about.exe</c>、<c>yz.exe</c> 的完整路径并启动。
		/// </summary>
		public static string 程序所在目录;

		// Token: 0x04000051 RID: 81
		/// <summary>锁屏时是否禁止关闭 Form4（原名 _Class）。true = 锁屏中，用户关不掉。</summary>
		public static bool 锁屏中禁止关闭 = false;

		// Token: 0x04000052 RID: 82
		/// <summary>要在 D/E/F/C 盘根目录创建的目录名之一（原名 _Object），初值"360安全浏览器下载"。</summary>
		public static string 安全浏览器目录名 = "360安全浏览器下载";

		// Token: 0x04000053 RID: 83
		/// <summary>要在 D/E/F/C 盘根目录创建的目录名之二（原名 m_Iterator），初值"360极速浏览器下载"。</summary>
		public static string 极速浏览器目录名 = "360极速浏览器下载";

		// Token: 0x04000054 RID: 84
		/// <summary>PROV_RSA_FULL，给未使用的 CryptAcquireContext 声明配套（原名 _Visitor）。</summary>
		private const int _Visitor = 1;

		// Token: 0x04000055 RID: 85
		/// <summary>CRYPT_VERIFYCONTEXT（0xF0000000），同上（原名 _Parser）。</summary>
		private const int _Parser = -268435456;

		// Token: 0x04000056 RID: 86
		/// <summary>CALG_MD5（0x8003），同上（原名 property）。</summary>
		private const int property = 32771;

		// Token: 0x04000057 RID: 87
		/// <summary>HP_HASHVAL，给未使用的 CryptGetHashParam 声明配套（原名 configuration）。</summary>
		private const int configuration = 2;
	}
}
