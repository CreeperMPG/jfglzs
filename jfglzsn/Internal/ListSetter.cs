using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace jfglzsn.Internal
{
	// Token: 0x02000015 RID: 21
	/// <summary>
	/// 壳运行时核心（.NET Reactor / IntelliLock 的保护代码），反混淆后可以分成五块：
	/// <list type="number">
	/// <item><b>自实现 MD5</b>：<see cref="自实现MD5"/> 是完整的手写 MD5 主循环（含 F/G/H/I 四个轮函数），
	/// 之所以自带一份，是因为 FIPS 策略下系统的 <c>MD5CryptoServiceProvider</c> 会被禁用
	/// （见 <see cref="计算MD5"/> 的分支）。</item>
	/// <item><b>对称加密</b>：<see cref="创建对称算法"/> 按 FIPS 标志选 AES / Rijndael；
	/// <see cref="AES解密"/>、<see cref="AES加密"/> 分别用硬编码 Key/IV 解密资源、加密配置。</item>
	/// <item><b>字符串解密</b>：<see cref="解密字符串"/> 用 <c>ReadProcessMemory</c> 从**自身模块内存**里
	/// 按偏移读出密文再 AES 解密——这是壳的字符串加密方案；本工程里已无调用点（字符串已被前置阶段解出）。</item>
	/// <item><b>运行时自补丁</b>：<see cref="运行时还原方法体"/> 从内嵌资源取补丁数据，
	/// 解密后用 <c>VirtualProtect</c> + <c>WriteProcessMemory</c> 往自身代码段写回原始内容（反脱壳/反篡改）。</item>
	/// <item><b>一堆死桩</b>：<see cref="空方法桩"/>、6 个恒返回 <c>null</c> 的 <c>byte[]</c> 方法、
	/// 6 个恒返回 <c>{1,2}</c> 的授权数据桩、以及 <see cref="视图规则桩"/>/<see cref="登录规则桩"/>。</item>
	/// </list>
	/// <para>
	/// 关键结论：除 <see cref="计算MD5"/> 外，本类所有保护相关方法在本工程内**都没有调用点**，
	/// 说明脱壳/解密阶段已把调用点替换成明文，这段壳代码只是残骸——但它是理解样本来源的直接证据。
	/// </para>
	/// </summary>
	internal class ListSetter
	{
		// Token: 0x060000D7 RID: 215 RVA: 0x000025B6 File Offset: 0x000007B6
		/// <summary>壳留下的空方法体（dnSpy 原名 <c>method_0</c>），无参数无返回值，工程内无调用。</summary>
		private void 空方法桩()
		{
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00009450 File Offset: 0x00007650
		/// <summary>
		/// 自实现 MD5：与 <c>System.Security.Cryptography.MD5</c> 输出完全一致（16 字节小端摘要）。
		/// <para>
		/// 结构：补位（<c>0x80</c> + 零填充 + 64 位小端比特长度）→ 按 512 位分组 → 64 步四轮运算
		/// （F 16 步 / G 16 步 / H 16 步 / I 16 步）→ 累加回初值 → 输出 A/B/C/D。
		/// </para>
		/// <para>
		/// 两个必须原样保留的怪点：
		/// ① 填字块的内层循环上界写的是 <c>num13 &lt; 61</c>（步长 4），正好 16 次，改成 <c>&lt; 64</c> 就是 17 次，会崩；
		/// ② <c>消息块视图</c> 与 <c>缓冲区</c> 是**同一个数组的两个别名**（VB 里就是同一个对象），
		/// 所以 <c>消息块视图[num5] |= 128</c> 实际改的是缓冲区首字节——不要拆成两个独立数组。
		/// </para>
		/// </summary>
		/// <param name="数据">待摘要的字节数组。</param>
		/// <returns>16 字节 MD5 摘要。</returns>
		internal static byte[] 自实现MD5(byte[] 数据)
		{
			uint[] 消息块 = new uint[16];
			// 补位到 448 bits 需要补的 bit 数
			int 补位比特 = 448 - 数据.Length * 8 % 512;
			uint 补位字节 = (uint)((补位比特 + 512) % 512);
			if (补位字节 == 0U)
			{
				补位字节 = 512U;
			}
			// 总长度 = 原文 + 补位字节 + 8 字节长度字段
			uint 总长度 = (uint)((long)数据.Length + (long)((ulong)(补位字节 / 8U)) + 8L);
			ulong 比特长度 = (ulong)((long)数据.Length * 8L);
			byte[] 缓冲区 = new byte[总长度];
			for (int i = 0; i < 数据.Length; i++)
			{
				缓冲区[i] = 数据[i];
			}
			// 别名：消息块视图 与 缓冲区 指向同一个数组（原版如此）
			byte[] 消息块视图 = 缓冲区;
			int 数据长度 = 数据.Length;
			消息块视图[数据长度] |= 128;
			// 末尾 8 字节写入原始比特长度（小端）
			for (int j = 8; j > 0; j--)
			{
				缓冲区[(int)(checked((IntPtr)(unchecked((ulong)总长度 - (ulong)((long)j)))))] = (byte)((比特长度 >> (8 - j) * 8) & 255UL);
			}
			uint 分组数 = (uint)(缓冲区.Length * 8 / 32);
			// MD5 初始向量 A/B/C/D
			uint 状态A = 1732584193U;
			uint 状态B = 4023233417U;
			uint 状态C = 2562383102U;
			uint 状态D = 271733878U;
			for (uint 组号 = 0U; 组号 < 分组数 / 16U; 组号 += 1U)
			{
				uint 组偏移 = 组号 << 6;
				// 把 16 个字按小端从缓冲区搬进 消息块；上界 61 是原版写法，正好 16 次
				for (uint 字偏移 = 0U; 字偏移 < 61U; 字偏移 += 4U)
				{
					消息块[(int)((UIntPtr)(字偏移 >> 2))] = (uint)(((int)缓冲区[(int)((UIntPtr)(组偏移 + (字偏移 + 3U)))] << 24) | ((int)缓冲区[(int)((UIntPtr)(组偏移 + (字偏移 + 2U)))] << 16) | ((int)缓冲区[(int)((UIntPtr)(组偏移 + (字偏移 + 1U)))] << 8) | (int)缓冲区[(int)((UIntPtr)(组偏移 + 字偏移))]);
				}
				// 保存本轮开始前的状态，供末尾累加
				uint 原始A = 状态A;
				uint 原始B = 状态B;
				uint 原始C = 状态C;
				uint 原始D = 状态D;
				// 第 1 轮：F 函数，位移 7/12/17/22
				自实现MD5_轮函数F(ref 状态A, 状态B, 状态C, 状态D, 0U, 7, 1U, 消息块);
				自实现MD5_轮函数F(ref 状态D, 状态A, 状态B, 状态C, 1U, 12, 2U, 消息块);
				自实现MD5_轮函数F(ref 状态C, 状态D, 状态A, 状态B, 2U, 17, 3U, 消息块);
				自实现MD5_轮函数F(ref 状态B, 状态C, 状态D, 状态A, 3U, 22, 4U, 消息块);
				自实现MD5_轮函数F(ref 状态A, 状态B, 状态C, 状态D, 4U, 7, 5U, 消息块);
				自实现MD5_轮函数F(ref 状态D, 状态A, 状态B, 状态C, 5U, 12, 6U, 消息块);
				自实现MD5_轮函数F(ref 状态C, 状态D, 状态A, 状态B, 6U, 17, 7U, 消息块);
				自实现MD5_轮函数F(ref 状态B, 状态C, 状态D, 状态A, 7U, 22, 8U, 消息块);
				自实现MD5_轮函数F(ref 状态A, 状态B, 状态C, 状态D, 8U, 7, 9U, 消息块);
				自实现MD5_轮函数F(ref 状态D, 状态A, 状态B, 状态C, 9U, 12, 10U, 消息块);
				自实现MD5_轮函数F(ref 状态C, 状态D, 状态A, 状态B, 10U, 17, 11U, 消息块);
				自实现MD5_轮函数F(ref 状态B, 状态C, 状态D, 状态A, 11U, 22, 12U, 消息块);
				自实现MD5_轮函数F(ref 状态A, 状态B, 状态C, 状态D, 12U, 7, 13U, 消息块);
				自实现MD5_轮函数F(ref 状态D, 状态A, 状态B, 状态C, 13U, 12, 14U, 消息块);
				自实现MD5_轮函数F(ref 状态C, 状态D, 状态A, 状态B, 14U, 17, 15U, 消息块);
				自实现MD5_轮函数F(ref 状态B, 状态C, 状态D, 状态A, 15U, 22, 16U, 消息块);
				// 第 2 轮：G 函数，位移 5/9/14/20
				自实现MD5_轮函数G(ref 状态A, 状态B, 状态C, 状态D, 1U, 5, 17U, 消息块);
				自实现MD5_轮函数G(ref 状态D, 状态A, 状态B, 状态C, 6U, 9, 18U, 消息块);
				自实现MD5_轮函数G(ref 状态C, 状态D, 状态A, 状态B, 11U, 14, 19U, 消息块);
				自实现MD5_轮函数G(ref 状态B, 状态C, 状态D, 状态A, 0U, 20, 20U, 消息块);
				自实现MD5_轮函数G(ref 状态A, 状态B, 状态C, 状态D, 5U, 5, 21U, 消息块);
				自实现MD5_轮函数G(ref 状态D, 状态A, 状态B, 状态C, 10U, 9, 22U, 消息块);
				自实现MD5_轮函数G(ref 状态C, 状态D, 状态A, 状态B, 15U, 14, 23U, 消息块);
				自实现MD5_轮函数G(ref 状态B, 状态C, 状态D, 状态A, 4U, 20, 24U, 消息块);
				自实现MD5_轮函数G(ref 状态A, 状态B, 状态C, 状态D, 9U, 5, 25U, 消息块);
				自实现MD5_轮函数G(ref 状态D, 状态A, 状态B, 状态C, 14U, 9, 26U, 消息块);
				自实现MD5_轮函数G(ref 状态C, 状态D, 状态A, 状态B, 3U, 14, 27U, 消息块);
				自实现MD5_轮函数G(ref 状态B, 状态C, 状态D, 状态A, 8U, 20, 28U, 消息块);
				自实现MD5_轮函数G(ref 状态A, 状态B, 状态C, 状态D, 13U, 5, 29U, 消息块);
				自实现MD5_轮函数G(ref 状态D, 状态A, 状态B, 状态C, 2U, 9, 30U, 消息块);
				自实现MD5_轮函数G(ref 状态C, 状态D, 状态A, 状态B, 7U, 14, 31U, 消息块);
				自实现MD5_轮函数G(ref 状态B, 状态C, 状态D, 状态A, 12U, 20, 32U, 消息块);
				// 第 3 轮：H 函数，位移 4/11/16/23
				自实现MD5_轮函数H(ref 状态A, 状态B, 状态C, 状态D, 5U, 4, 33U, 消息块);
				自实现MD5_轮函数H(ref 状态D, 状态A, 状态B, 状态C, 8U, 11, 34U, 消息块);
				自实现MD5_轮函数H(ref 状态C, 状态D, 状态A, 状态B, 11U, 16, 35U, 消息块);
				自实现MD5_轮函数H(ref 状态B, 状态C, 状态D, 状态A, 14U, 23, 36U, 消息块);
				自实现MD5_轮函数H(ref 状态A, 状态B, 状态C, 状态D, 1U, 4, 37U, 消息块);
				自实现MD5_轮函数H(ref 状态D, 状态A, 状态B, 状态C, 4U, 11, 38U, 消息块);
				自实现MD5_轮函数H(ref 状态C, 状态D, 状态A, 状态B, 7U, 16, 39U, 消息块);
				自实现MD5_轮函数H(ref 状态B, 状态C, 状态D, 状态A, 10U, 23, 40U, 消息块);
				自实现MD5_轮函数H(ref 状态A, 状态B, 状态C, 状态D, 13U, 4, 41U, 消息块);
				自实现MD5_轮函数H(ref 状态D, 状态A, 状态B, 状态C, 0U, 11, 42U, 消息块);
				自实现MD5_轮函数H(ref 状态C, 状态D, 状态A, 状态B, 3U, 16, 43U, 消息块);
				自实现MD5_轮函数H(ref 状态B, 状态C, 状态D, 状态A, 6U, 23, 44U, 消息块);
				自实现MD5_轮函数H(ref 状态A, 状态B, 状态C, 状态D, 9U, 4, 45U, 消息块);
				自实现MD5_轮函数H(ref 状态D, 状态A, 状态B, 状态C, 12U, 11, 46U, 消息块);
				自实现MD5_轮函数H(ref 状态C, 状态D, 状态A, 状态B, 15U, 16, 47U, 消息块);
				自实现MD5_轮函数H(ref 状态B, 状态C, 状态D, 状态A, 2U, 23, 48U, 消息块);
				// 第 4 轮：I 函数，位移 6/10/15/21
				自实现MD5_轮函数I(ref 状态A, 状态B, 状态C, 状态D, 0U, 6, 49U, 消息块);
				自实现MD5_轮函数I(ref 状态D, 状态A, 状态B, 状态C, 7U, 10, 50U, 消息块);
				自实现MD5_轮函数I(ref 状态C, 状态D, 状态A, 状态B, 14U, 15, 51U, 消息块);
				自实现MD5_轮函数I(ref 状态B, 状态C, 状态D, 状态A, 5U, 21, 52U, 消息块);
				自实现MD5_轮函数I(ref 状态A, 状态B, 状态C, 状态D, 12U, 6, 53U, 消息块);
				自实现MD5_轮函数I(ref 状态D, 状态A, 状态B, 状态C, 3U, 10, 54U, 消息块);
				自实现MD5_轮函数I(ref 状态C, 状态D, 状态A, 状态B, 10U, 15, 55U, 消息块);
				自实现MD5_轮函数I(ref 状态B, 状态C, 状态D, 状态A, 1U, 21, 56U, 消息块);
				自实现MD5_轮函数I(ref 状态A, 状态B, 状态C, 状态D, 8U, 6, 57U, 消息块);
				自实现MD5_轮函数I(ref 状态D, 状态A, 状态B, 状态C, 15U, 10, 58U, 消息块);
				自实现MD5_轮函数I(ref 状态C, 状态D, 状态A, 状态B, 6U, 15, 59U, 消息块);
				自实现MD5_轮函数I(ref 状态B, 状态C, 状态D, 状态A, 13U, 21, 60U, 消息块);
				自实现MD5_轮函数I(ref 状态A, 状态B, 状态C, 状态D, 4U, 6, 61U, 消息块);
				自实现MD5_轮函数I(ref 状态D, 状态A, 状态B, 状态C, 11U, 10, 62U, 消息块);
				自实现MD5_轮函数I(ref 状态C, 状态D, 状态A, 状态B, 2U, 15, 63U, 消息块);
				自实现MD5_轮函数I(ref 状态B, 状态C, 状态D, 状态A, 9U, 21, 64U, 消息块);
				// 累加回本轮初值（MD5 的分组链接）
				状态A += 原始A;
				状态B += 原始B;
				状态C += 原始C;
				状态D += 原始D;
			}
			// 输出：A/B/C/D 各 4 字节小端拼接 = 16 字节摘要
			byte[] 摘要 = new byte[16];
			Array.Copy(BitConverter.GetBytes(状态A), 0, 摘要, 0, 4);
			Array.Copy(BitConverter.GetBytes(状态B), 0, 摘要, 4, 4);
			Array.Copy(BitConverter.GetBytes(状态C), 0, 摘要, 8, 4);
			Array.Copy(BitConverter.GetBytes(状态D), 0, 摘要, 12, 4);
			return 摘要;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000025B8 File Offset: 0x000007B8
		/// <summary>
		/// MD5 第 1 轮步函数 F：<c>a = b + ROTL(a + (b&amp;c)|(~b&amp;d) + M[k] + T[i], s)</c>。
		/// 参数顺序被刻意排列成「当前字 + 其余三字 + 消息下标 + 位移 + 常量下标」，调用处 16 连带 <c>ref</c> 轮转。
		/// </summary>
		/// <param name="当前字">被更新的状态字（结果写回）。</param>
		/// <param name="字B">状态 B。</param>
		/// <param name="字C">状态 C。</param>
		/// <param name="字D">状态 D。</param>
		/// <param name="消息下标">消息块下标 k。</param>
		/// <param name="位移">循环左移位数 s。</param>
		/// <param name="常量下标">T 表下标 + 1（函数内部再减 1）。</param>
		/// <param name="消息块">16 字的当前分组。</param>
		private static void 自实现MD5_轮函数F(ref uint 当前字, uint 字B, uint 字C, uint 字D, uint 消息下标, ushort 位移, uint 常量下标, uint[] 消息块)
		{
			当前字 = 字B + ListSetter.循环左移(当前字 + ((字B & 字C) | (~字B & 字D)) + 消息块[(int)((UIntPtr)消息下标)] + ListSetter.MD5常量表[(int)((UIntPtr)(常量下标 - 1U))], 位移);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000025E3 File Offset: 0x000007E3
		/// <summary>MD5 第 2 轮步函数 G：<c>a = b + ROTL(a + (b&amp;d)|(c&amp;~d) + M[k] + T[i], s)</c>。</summary>
		/// <param name="当前字">被更新的状态字（结果写回）。</param>
		/// <param name="字B">状态 B。</param>
		/// <param name="字C">状态 C。</param>
		/// <param name="字D">状态 D。</param>
		/// <param name="消息下标">消息块下标 k。</param>
		/// <param name="位移">循环左移位数 s。</param>
		/// <param name="常量下标">T 表下标 + 1。</param>
		/// <param name="消息块">16 字的当前分组。</param>
		private static void 自实现MD5_轮函数G(ref uint 当前字, uint 字B, uint 字C, uint 字D, uint 消息下标, ushort 位移, uint 常量下标, uint[] 消息块)
		{
			当前字 = 字B + ListSetter.循环左移(当前字 + ((字B & 字D) | (字C & ~字D)) + 消息块[(int)((UIntPtr)消息下标)] + ListSetter.MD5常量表[(int)((UIntPtr)(常量下标 - 1U))], 位移);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000260E File Offset: 0x0000080E
		/// <summary>MD5 第 3 轮步函数 H：<c>a = b + ROTL(a + (b^c^d) + M[k] + T[i], s)</c>。</summary>
		/// <param name="当前字">被更新的状态字（结果写回）。</param>
		/// <param name="字B">状态 B。</param>
		/// <param name="字C">状态 C。</param>
		/// <param name="字D">状态 D。</param>
		/// <param name="消息下标">消息块下标 k。</param>
		/// <param name="位移">循环左移位数 s。</param>
		/// <param name="常量下标">T 表下标 + 1。</param>
		/// <param name="消息块">16 字的当前分组。</param>
		private static void 自实现MD5_轮函数H(ref uint 当前字, uint 字B, uint 字C, uint 字D, uint 消息下标, ushort 位移, uint 常量下标, uint[] 消息块)
		{
			当前字 = 字B + ListSetter.循环左移(当前字 + (字B ^ 字C ^ 字D) + 消息块[(int)((UIntPtr)消息下标)] + ListSetter.MD5常量表[(int)((UIntPtr)(常量下标 - 1U))], 位移);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00002636 File Offset: 0x00000836
		/// <summary>MD5 第 4 轮步函数 I：<c>a = b + ROTL(a + (c^(b|~d)) + M[k] + T[i], s)</c>。</summary>
		/// <param name="当前字">被更新的状态字（结果写回）。</param>
		/// <param name="字B">状态 B。</param>
		/// <param name="字C">状态 C。</param>
		/// <param name="字D">状态 D。</param>
		/// <param name="消息下标">消息块下标 k。</param>
		/// <param name="位移">循环左移位数 s。</param>
		/// <param name="常量下标">T 表下标 + 1。</param>
		/// <param name="消息块">16 字的当前分组。</param>
		private static void 自实现MD5_轮函数I(ref uint 当前字, uint 字B, uint 字C, uint 字D, uint 消息下标, ushort 位移, uint 常量下标, uint[] 消息块)
		{
			当前字 = 字B + ListSetter.循环左移(当前字 + (字C ^ (字B | ~字D)) + 消息块[(int)((UIntPtr)消息下标)] + ListSetter.MD5常量表[(int)((UIntPtr)(常量下标 - 1U))], 位移);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000265F File Offset: 0x0000085F
		/// <summary>
		/// 32 位循环左移（MD5 的 ROTL）。VB 的移位对移位数取模，这里靠 <c>(32 - 位移)</c> 的互补移位实现，移位数恒在 1~31。
		/// </summary>
		/// <param name="值">待移位值。</param>
		/// <param name="位移">左移位数。</param>
		/// <returns>循环左移后的值。</returns>
		private static uint 循环左移(uint 值, ushort 位移)
		{
			return (值 >> (int)(32 - 位移)) | (值 << (int)位移);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00002671 File Offset: 0x00000871
		/// <summary>
		/// 查询「系统是否只允许 FIPS 认证的加密算法」，首次调用时探测并缓存。
		/// 结果决定 <see cref="计算MD5"/> 与 <see cref="创建对称算法"/> 走系统实现还是自实现。
		/// </summary>
		/// <returns>FIPS-only 策略开启时为 <c>true</c>。</returns>
		internal static bool 是否仅FIPS算法()
		{
			if (!ListSetter.已探测FIPS标志)
			{
				ListSetter.探测FIPS策略();
				ListSetter.已探测FIPS标志 = true;
			}
			return ListSetter.FIPS仅允许标志;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00009AF4 File Offset: 0x00007CF4
		/// <summary>
		/// 用反射读 <c>CryptoConfig.AllowOnlyFipsAlgorithms</c>（.NET 4 起才有的属性，反射可以避免编译期依赖）。
		/// 任何异常（老框架无此属性 / 反射被限制）都被吞掉，标志保持 <c>false</c>——与原版一致。
		/// </summary>
		internal static void 探测FIPS策略()
		{
			try
			{
				ListSetter.FIPS仅允许标志 = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
				// 原版空 catch：拿不到属性就当非 FIPS 模式
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00009B54 File Offset: 0x00009B54
		/// <summary>
		/// 创建一个对称算法实例：FIPS-only 模式下优先 <see cref="AesCryptoServiceProvider"/>（构造失败退回
		/// <see cref="RijndaelManaged"/>），否则直接用 <see cref="RijndaelManaged"/>。
		/// 调用方自行设置 Key/IV/Mode，本方法不改任何属性。
		/// </summary>
		/// <returns>未初始化的对称算法对象。</returns>
		internal static SymmetricAlgorithm 创建对称算法()
		{
			SymmetricAlgorithm 算法 = null;
			if (ListSetter.是否仅FIPS算法())
			{
				try
				{
					return new AesCryptoServiceProvider();
				}
				catch
				{
					return new RijndaelManaged();
				}
			}
			算法 = new RijndaelManaged();
			return 算法;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000268A File Offset: 0x0000086A
		/// <summary>
		/// 计算 MD5，按 FIPS 策略选实现：非 FIPS 用系统 <c>MD5CryptoServiceProvider</c>，
		/// FIPS 下系统 MD5 被禁用，改用 <see cref="自实现MD5"/>。两者输出一致。
		/// <para>
		/// 调用点：<see cref="PrototypeSetter"/> 用它算内嵌资源的资源名；<see cref="AES加密"/> 用它把口令派生为 IV。
		/// </para>
		/// </summary>
		/// <param name="数据">待摘要的字节数组。</param>
		/// <returns>16 字节 MD5 摘要。</returns>
		internal static byte[] 计算MD5(byte[] 数据)
		{
			if (!ListSetter.是否仅FIPS算法())
			{
				return new MD5CryptoServiceProvider().ComputeHash(数据);
			}
			return ListSetter.自实现MD5(数据);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00009B94 File Offset: 0x00009B94
		/// <summary>
		/// 字符串解密（壳的字符串加密方案）。
		/// <para>
		/// 首次调用（<paramref name="字符串序号"/> 为 <c>-1</c>）时：读取内嵌资源
		/// <c>2163406f-f304-4b64-8350-1fe705a394ba</c>，用「硬编码 32 字节密钥 XOR 硬编码 16 字节 IV」
		/// 得到真正的 AES-256 密钥，CBC 解密出字符串偏移表（<see cref="解密数据表"/>），
		/// 再做一遍自定义的滚动 XOR 混淆还原，最后取本模块 <c>HINSTANCE</c> 作为基址。
		/// </para>
		/// <para>
		/// 之后每次调用：<paramref name="字符串序号"/> 是偏移表里的索引，查缓存；
		/// 未命中则从偏移表取「数据偏移」，用 <c>ReadProcessMemory</c> 从**自身进程内存**
		/// （模块基址 + 偏移）读出 <c>[4 字节长度][密文]</c>，交给 <see cref="AES解密"/> 解出 UTF-16 字符串并缓存。
		/// </para>
		/// <para>
		/// 反混淆说明：原版对 <c>密钥</c>/<c>初始化向量</c> 的每个元素写了 3~7 次（混淆诱饵，后面的覆盖前面的），
		/// 这里只保留**最终生效值**，数组内容与运行时完全一致；其余语句（顺序、流控、内存读取）逐条保留。
		/// 本方法在本工程内无调用点。
		/// </para>
		/// </summary>
		/// <param name="字符串序号">偏移表下标；<c>-1</c> 表示初始化整张表。</param>
		/// <returns>解密出的字符串。</returns>
		static string 解密字符串(int 字符串序号)
		{
			if (ListSetter.解密数据表.Length == 0)
			{
				BinaryReader 读取器 = new BinaryReader(typeof(ListSetter).Assembly.GetManifestResourceStream("2163406f-f304-4b64-8350-1fe705a394ba"));
				读取器.BaseStream.Position = 0L;
				byte[] 密文数据 = 读取器.ReadBytes((int)读取器.BaseStream.Length);
				// 32 字节密钥（原版每个下标被反复赋值，此处仅保留最后一次生效的值）
				byte[] 密钥 = new byte[]
				{
					117, 180, 112, 108, 225, 207, 9, 250, 37, 97, 48, 84, 43, 186, 145, 87, 1, 2, 35, 222, 128, 59, 228, 136, 249, 57, 180, 25, 186, 149, 137, 44
				};
				// 16 字节 IV（同样只保留最终值）
				byte[] 初始化向量 = new byte[]
				{
					63, 156, 138, 34, 156, 109, 15, 17, 21, 132, 121, 206, 47, 105, 113, 129
				};
				// 若程序集带强名称，则用 PublicKeyToken 的 8 个字节替换 IV 的奇数位（本样本未强签名，此分支不生效）
				byte[] publicKeyToken = typeof(ListSetter).Assembly.GetName().GetPublicKeyToken();
				if (publicKeyToken != null && publicKeyToken.Length > 0)
				{
					初始化向量[1] = publicKeyToken[0];
					初始化向量[3] = publicKeyToken[1];
					初始化向量[5] = publicKeyToken[2];
					初始化向量[7] = publicKeyToken[3];
					初始化向量[9] = publicKeyToken[4];
					初始化向量[11] = publicKeyToken[5];
					初始化向量[13] = publicKeyToken[6];
					初始化向量[15] = publicKeyToken[7];
				}
				// 密钥前 16 字节与 IV 逐字节 XOR，得到真正的 AES 密钥
				for (int i = 0; i < 初始化向量.Length; i++)
				{
					密钥[i] ^= 初始化向量[i];
				}
				if (字符串序号 == -1)
				{
					SymmetricAlgorithm 算法 = ListSetter.创建对称算法();
					算法.Mode = CipherMode.CBC;
					ICryptoTransform 解密器 = 算法.CreateDecryptor(密钥, 初始化向量);
					MemoryStream 输出流 = new MemoryStream();
					CryptoStream 解流 = new CryptoStream(输出流, 解密器, CryptoStreamMode.Write);
					解流.Write(密文数据, 0, 密文数据.Length);
					解流.FlushFinalBlock();
					ListSetter.解密数据表 = 输出流.ToArray();
					if (ListSetter.解密数据表.Length > 0)
					{
						ListSetter.字符串缓存表 = new string[ListSetter.解密数据表.Length / 4 + 1];
						ListSetter.缓存槽索引 = new int[ListSetter.解密数据表.Length / 4 + 1];
					}
					输出流.Close();
					解流.Close();
					读取器.Close();
					密文数据 = ListSetter.解密数据表;
				}
				int 余数 = 密文数据.Length % 4;
				int 块数 = 密文数据.Length / 4;
				byte[] 解出 = new byte[密文数据.Length];
				int 密钥块数 = 密钥.Length / 4;
				uint 累计 = 0U;
				if (余数 > 0)
				{
					块数++;
				}
				for (int 块 = 0; 块 < 块数; 块++)
				{
					int 密钥块号 = 块 % 密钥块数;
					int 数据偏移 = 块 * 4;
					uint 密钥偏移 = (uint)(密钥块号 * 4);
					uint 密钥字 = (uint)(((int)密钥[(int)((UIntPtr)(密钥偏移 + 3U))] << 24) | ((int)密钥[(int)((UIntPtr)(密钥偏移 + 2U))] << 16) | ((int)密钥[(int)((UIntPtr)(密钥偏移 + 1U))] << 8) | (int)密钥[(int)((UIntPtr)密钥偏移)]);
					uint 掩码 = 255U;
					int 移位 = 0;
					uint 明文字;
					if (块 == 块数 - 1 && 余数 > 0)
					{
						// 末块不足 4 字节：从尾部倒着拼
						明文字 = 0U;
						for (int k = 0; k < 余数; k++)
						{
							if (k > 0)
							{
								明文字 <<= 8;
							}
							明文字 |= (uint)密文数据[密文数据.Length - (1 + k)];
						}
						累计 += 密钥字;
					}
					else
					{
						密钥偏移 = (uint)数据偏移;
						明文字 = (uint)(((int)密文数据[(int)((UIntPtr)(密钥偏移 + 3U))] << 24) | ((int)密文数据[(int)((UIntPtr)(密钥偏移 + 2U))] << 16) | ((int)密文数据[(int)((UIntPtr)(密钥偏移 + 1U))] << 8) | (int)密文数据[(int)((UIntPtr)密钥偏移)]);
						累计 += 密钥字;
					}
					// 自定义滚动混淆的逆运算（固定常数链，等价于一个 32 位非线性混合器）
					uint 旧累计 = 累计;
					uint 混合 = 累计;
					混合 ^= 混合 >> 25;
					混合 += 1394801171U;
					混合 ^= 混合 << 5;
					混合 += 3457341307U;
					混合 ^= 混合 >> 3;
					混合 += 4040842541U;
					混合 = 2018246656U + 混合;
					累计 = 旧累计 + (uint)混合;
					if (块 == 块数 - 1 && 余数 > 0)
					{
						uint 异或值 = 累计 ^ 明文字;
						for (int l = 0; l < 余数; l++)
						{
							if (l > 0)
							{
								掩码 <<= 8;
								移位 += 8;
							}
							解出[数据偏移 + l] = (byte)((异或值 & 掩码) >> 移位);
						}
					}
					else
					{
						uint 异或值 = 累计 ^ 明文字;
						解出[数据偏移] = (byte)(异或值 & 255U);
						解出[数据偏移 + 1] = (byte)((异或值 & 65280U) >> 8);
						解出[数据偏移 + 2] = (byte)((异或值 & 16711680U) >> 16);
						解出[数据偏移 + 3] = (byte)((异或值 & 4278190080U) >> 24);
					}
				}
				ListSetter.解密数据表 = 解出;
				if (ListSetter.解密数据表.Length > 0)
				{
					ListSetter.字符串缓存表 = new string[ListSetter.解密数据表.Length / 4 + 1];
					ListSetter.缓存槽索引 = new int[ListSetter.解密数据表.Length / 4 + 1];
				}
				// 取本模块加载基址：32 位进程走 int，64 位走 long（两条都给，运行期只用其一）
				Assembly 本程序集 = typeof(ListSetter).Assembly;
				if (IntPtr.Size == 4)
				{
					ListSetter.模块基址32 = Marshal.GetHINSTANCE(本程序集.GetModules()[0]).ToInt32();
				}
				ListSetter.模块基址64 = Marshal.GetHINSTANCE(本程序集.GetModules()[0]).ToInt64();
			}
			// 缓存命中：同一序号第二次解密直接返回上次结果
			int 缓存槽 = 字符串序号 / 4;
			if (ListSetter.缓存槽索引[缓存槽] > 0)
			{
				return ((string[])ListSetter.字符串缓存表)[ListSetter.缓存槽索引[缓存槽]];
			}
			// 偏移表里存的是「字符串记录在模块内的偏移」
			int 记录偏移 = BitConverter.ToInt32(ListSetter.解密数据表, 字符串序号);
			if (ListSetter.本进程句柄 == IntPtr.Zero)
			{
				// 0x10 = PROCESS_VM_READ；bInheritHandle 传 1（原版如此）
				ListSetter.本进程句柄 = ListSetter.OpenProcess(16U, 1, (uint)Process.GetCurrentProcess().Id);
			}
			byte[] 长度缓冲 = new byte[4];
			if (IntPtr.Size == 4)
			{
				ListSetter.ReadProcessMemory(ListSetter.本进程句柄, new IntPtr(ListSetter.模块基址32 + 记录偏移), 长度缓冲, 4U, out ListSetter.读写字数);
			}
			else
			{
				ListSetter.ReadProcessMemory(ListSetter.本进程句柄, new IntPtr(ListSetter.模块基址64 + (long)记录偏移), 长度缓冲, 4U, out ListSetter.读写字数);
			}
			int 密文长度 = BitConverter.ToInt32(长度缓冲, 0);
			长度缓冲 = new byte[密文长度];
			if (IntPtr.Size == 4)
			{
				ListSetter.ReadProcessMemory(ListSetter.本进程句柄, new IntPtr(ListSetter.模块基址32 + 记录偏移 + 4), 长度缓冲, Convert.ToUInt32(密文长度), out ListSetter.读写字数);
			}
			else
			{
				ListSetter.ReadProcessMemory(ListSetter.本进程句柄, new IntPtr(ListSetter.模块基址64 + (long)记录偏移 + 4L), 长度缓冲, Convert.ToUInt32(密文长度), out ListSetter.读写字数);
			}
			byte[] 明文 = ListSetter.AES解密(长度缓冲);
			string 结果字符串 = Encoding.Unicode.GetString(明文, 0, 明文.Length);
			ListSetter.缓存槽索引[缓存槽] = ListSetter.下一个缓存槽;
			((string[])ListSetter.字符串缓存表)[ListSetter.下一个缓存槽] = 结果字符串;
			ListSetter.下一个缓存槽++;
			return 结果字符串;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0000ACEC File Offset: 0x00008EEC
		/// <summary>
		/// 把 Base64 字符串还原成 UTF-16 字符串（不是普通解码：它把 Base64 解出的字节直接按 Unicode 读）。
		/// 工程内无调用点。
		/// </summary>
		/// <param name="base64文本">Base64 文本。</param>
		/// <returns>解码后的字符串。</returns>
		internal static string Base64转字符串(string base64文本)
		{
			byte[] 字节 = Convert.FromBase64String(base64文本);
			return Encoding.Unicode.GetString(字节, 0, 字节.Length);
		}

		// Token: 0x060000E4 RID: 228
		/// <summary>kernel32：修改内存页保护属性（自补丁时临时放开代码段写权限）。</summary>
		/// <param name="lpAddress">目标地址。</param>
		/// <param name="dwSize">长度。</param>
		/// <param name="flNewProtect">新保护属性，<c>4</c> = PAGE_READWRITE。</param>
		/// <param name="lpflOldProtect">输出原保护属性。</param>
		/// <returns>非 0 表示成功。</returns>
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr lpAddress, int dwSize, int flNewProtect, ref int lpflOldProtect);

		// Token: 0x060000E5 RID: 229 RVA: 0x0000AD10 File Offset: 0x00008F10
		/// <summary>
		/// 运行时自补丁（反脱壳/反篡改的核心）。
		/// <para>
		/// 流程：读内嵌资源 <c>c2a928a7-eb8e-4050-b410-49f5dde0edce</c> → 用硬编码 32 字节密钥
		/// （与 16 字节 IV 先整体反转、再用 PublicKeyToken 覆盖奇数位）XOR 出 AES 密钥 →
		/// 同样的滚动 XOR 混淆还原 → 得到一张「补丁表」→ 以本模块 <c>HINSTANCE</c> 为基址，
		/// 逐条 <see cref="VirtualProtect"/> 放开写权限后用 <see cref="WriteProcessMemory"/> /
		/// <c>Marshal.WriteInt32</c> 把原始数据写回自身内存。
		/// </para>
		/// <para>
		/// 也就是说：磁盘上的方法体是被改过的，靠这段代码在运行期改回来；静态反编译看到的差异由此而来。
		/// 由 <see cref="已执行还原标志"/> 保证只跑一次。本工程内无调用点（原本由模块初始化器触发）。
		/// </para>
		/// <para>
		/// 反混淆说明：<c>密钥</c>/<c>初始化向量</c> 的重复赋值只保留最终值；其余异常处理、句柄释放、
		/// 32/64 位分支全部逐条保留（含原版那句无意义的自赋值 <c>累计 = 累计;</c>）。
		/// </para>
		/// </summary>
		[ListSetter.BaseSetter(typeof(ListSetter.BaseSetter.ParamsSetter<object>[]))]
		static void 运行时还原方法体()
		{
			if (!ListSetter.已执行还原标志)
			{
				ListSetter.已执行还原标志 = true;
				BinaryReader 读取器 = new BinaryReader(typeof(ListSetter).Assembly.GetManifestResourceStream("c2a928a7-eb8e-4050-b410-49f5dde0edce"));
				读取器.BaseStream.Position = 0L;
				byte[] 密文数据 = 读取器.ReadBytes((int)读取器.BaseStream.Length);
				// 32 字节密钥（只保留最终生效值）
				byte[] 密钥 = new byte[]
				{
					198, 13, 105, 42, 35, 239, 245, 150, 82, 115, 174, 92, 154, 123, 74, 25, 46, 154, 51, 77, 106, 129, 235, 106, 198, 101, 109, 126, 181, 74, 74, 138
				};
				// 16 字节 IV（只保留最终生效值）
				byte[] 初始化向量 = new byte[]
				{
					68, 84, 96, 184, 123, 242, 197, 131, 0, 78, 33, 213, 211, 12, 105, 111
				};
				// 与 解密字符串 的差别：这里先把 IV 整体反转
				Array.Reverse(初始化向量);
				byte[] publicKeyToken = typeof(ListSetter).Assembly.GetName().GetPublicKeyToken();
				if (publicKeyToken != null && publicKeyToken.Length > 0)
				{
					初始化向量[1] = publicKeyToken[0];
					初始化向量[3] = publicKeyToken[1];
					初始化向量[5] = publicKeyToken[2];
					初始化向量[7] = publicKeyToken[3];
					初始化向量[9] = publicKeyToken[4];
					初始化向量[11] = publicKeyToken[5];
					初始化向量[13] = publicKeyToken[6];
					初始化向量[15] = publicKeyToken[7];
					// 与 解密字符串 的差别：这里用完立即把 token 清零
					Array.Clear(publicKeyToken, 0, publicKeyToken.Length);
				}
				// 密钥前 16 字节与（反转后的）IV 逐字节 XOR
				for (int i = 0; i < 初始化向量.Length; i++)
				{
					密钥[i] ^= 初始化向量[i];
				}
				byte[] 待还原 = 密文数据;
				int 余数 = 待还原.Length % 4;
				int 块数 = 待还原.Length / 4;
				byte[] 解出 = new byte[待还原.Length];
				int 密钥块数 = 密钥.Length / 4;
				uint 累计 = 0U;
				if (余数 > 0)
				{
					块数++;
				}
				for (int 块 = 0; 块 < 块数; 块++)
				{
					int 密钥块号 = 块 % 密钥块数;
					int 数据偏移 = 块 * 4;
					uint 密钥偏移 = (uint)(密钥块号 * 4);
					uint 密钥字 = (uint)(((int)密钥[(int)((UIntPtr)(密钥偏移 + 3U))] << 24) | ((int)密钥[(int)((UIntPtr)(密钥偏移 + 2U))] << 16) | ((int)密钥[(int)((UIntPtr)(密钥偏移 + 1U))] << 8) | (int)密钥[(int)((UIntPtr)密钥偏移)]);
					uint 掩码 = 255U;
					int 移位 = 0;
					uint 明文字;
					if (块 == 块数 - 1 && 余数 > 0)
					{
						累计 += 密钥字;
						明文字 = 0U;
						for (int k = 0; k < 余数; k++)
						{
							if (k > 0)
							{
								明文字 <<= 8;
							}
							明文字 |= (uint)待还原[待还原.Length - (1 + k)];
						}
					}
					else
					{
						密钥偏移 = (uint)数据偏移;
						累计 += 密钥字;
						明文字 = (uint)(((int)待还原[(int)((UIntPtr)(密钥偏移 + 3U))] << 24) | ((int)待还原[(int)((UIntPtr)(密钥偏移 + 2U))] << 16) | ((int)待还原[(int)((UIntPtr)(密钥偏移 + 1U))] << 8) | (int)待还原[(int)((UIntPtr)密钥偏移)]);
					}
					累计 = 累计; // 原版自赋值（混淆残留），无实际操作
					uint 旧累计 = 累计;
					uint 混合 = 累计;
					混合 ^= 混合 >> 25;
					混合 += 1394801171U;
					混合 ^= 混合 << 5;
					混合 += 3457341307U;
					混合 ^= 混合 >> 3;
					混合 += 4040842541U;
					混合 = 2018246656U + 混合;
					累计 = 旧累计 + (uint)混合;
					if (块 == 块数 - 1 && 余数 > 0)
					{
						uint 异或值 = 累计 ^ 明文字;
						for (int l = 0; l < 余数; l++)
						{
							if (l > 0)
							{
								掩码 <<= 8;
								移位 += 8;
							}
							解出[数据偏移 + l] = (byte)((异或值 & 掩码) >> 移位);
						}
					}
					else
					{
						uint 异或值 = 累计 ^ 明文字;
						解出[数据偏移] = (byte)(异或值 & 255U);
						解出[数据偏移 + 1] = (byte)((异或值 & 65280U) >> 8);
						解出[数据偏移 + 2] = (byte)((异或值 & 16711680U) >> 16);
						解出[数据偏移 + 3] = (byte)((异或值 & 4278190080U) >> 24);
					}
				}
				byte[] 补丁数据 = 解出;
				Array.Clear(初始化向量, 0, 初始化向量.Length);
				读取器.Close();
				读取器 = new BinaryReader(new MemoryStream(补丁数据));
				读取器.BaseStream.Position = 0L;
				IntPtr 进程句柄 = IntPtr.Zero;
				Assembly 本程序集 = typeof(ListSetter).Assembly;
				// 0x38 = PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ
				进程句柄 = ListSetter.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				ListSetter.模块基址64 = Marshal.GetHINSTANCE(本程序集.GetModules()[0]).ToInt64();
				IntPtr 写写字数 = IntPtr.Zero;
				int 原保护 = 0;
				int 条目数 = 读取器.ReadInt32();
				读取器.ReadInt32();
				// 第一段：逐条改写 4 字节
				for (int m = 0; m < 条目数; m++)
				{
					IntPtr 目标地址 = new IntPtr(ListSetter.模块基址64 + (long)读取器.ReadInt32());
					ListSetter.VirtualProtect(目标地址, 4, 4, ref 原保护);
					if (IntPtr.Size == 4)
					{
						ListSetter.WriteProcessMemory(进程句柄, 目标地址, BitConverter.GetBytes(读取器.ReadInt32()), 4U, out 写写字数);
					}
					else
					{
						ListSetter.WriteProcessMemory(进程句柄, 目标地址, BitConverter.GetBytes(读取器.ReadInt32()), 4U, out 写写字数);
					}
					ListSetter.VirtualProtect(目标地址, 4, 原保护, ref 原保护);
				}
				try
				{
					// 第二段：整块改写（块内每个 int32 一条）
					while (读取器.BaseStream.Position < 读取器.BaseStream.Length - 1L)
					{
						int 块偏移 = 读取器.ReadInt32();
						IntPtr 块地址 = new IntPtr(ListSetter.模块基址64 + (long)块偏移);
						int 块字数 = 读取器.ReadInt32();
						ListSetter.VirtualProtect(块地址, 块字数 * 4, 4, ref 原保护);
						for (int n = 0; n < 块字数; n++)
						{
							Marshal.WriteInt32(new IntPtr(块地址.ToInt64() + (long)(n * 4)), 读取器.ReadInt32());
						}
						ListSetter.VirtualProtect(块地址, 块字数 * 4, 原保护, ref 原保护);
					}
					ListSetter.CloseHandle(进程句柄);
				}
				catch
				{
					// 原版空 catch：补丁过程出错时静默放弃（已写入的部分不会回滚）
				}
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000BF3C File Offset: 0x0000A13C
		/// <summary>
		/// 取程序集的磁盘路径，三种手段依次尝试：<c>Assembly.Location</c> → <c>CodeBase</c>（去掉 <c>file:///</c> 前缀）
		/// → 反射读 <c>Location</c> 属性（应对被混淆/替换的 Assembly 子类）。都失败返回空串。
		/// </summary>
		/// <param name="程序集">目标程序集。</param>
		/// <returns>存在的文件路径，或空串。</returns>
		internal static object 取程序集路径(Assembly 程序集)
		{
			try
			{
				if (File.Exists(((Assembly)程序集).Location))
				{
					return ((Assembly)程序集).Location;
				}
			}
			catch
			{
				// 原版空 catch：动态程序集没有 Location，抛异常即跳过
			}
			try
			{
				if (File.Exists(((Assembly)程序集).GetName().CodeBase.ToString().Replace("file:///", "")))
				{
					return ((Assembly)程序集).GetName().CodeBase.ToString().Replace("file:///", "");
				}
			}
			catch
			{
				// 原版空 catch
			}
			try
			{
				if (File.Exists(程序集.GetType().GetProperty("Location").GetValue(程序集, new object[0])
					.ToString()))
				{
					return 程序集.GetType().GetProperty("Location").GetValue(程序集, new object[0])
						.ToString();
				}
			}
			catch
			{
				// 原版空 catch
			}
			return "";
		}

		// Token: 0x060000E7 RID: 231
		/// <summary>kernel32：写目标进程内存（自补丁用）。</summary>
		/// <param name="hProcess">目标进程句柄。</param>
		/// <param name="lpBaseAddress">目标地址。</param>
		/// <param name="lpBuffer">源数据。</param>
		/// <param name="nSize">字节数。</param>
		/// <param name="lpNumberOfBytesWritten">输出实际写入字节数。</param>
		/// <returns>非 0 表示成功。</returns>
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] [Out] byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

		// Token: 0x060000E8 RID: 232
		/// <summary>kernel32：读目标进程内存（字符串解密用）。</summary>
		/// <param name="hProcess">目标进程句柄。</param>
		/// <param name="lpBaseAddress">目标地址。</param>
		/// <param name="lpBuffer">接收缓冲。</param>
		/// <param name="nSize">字节数。</param>
		/// <param name="lpNumberOfBytesRead">输出实际读取字节数。</param>
		/// <returns>非 0 表示成功。</returns>
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In] [Out] byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesRead);

		// Token: 0x060000E9 RID: 233
		/// <summary>kernel32：打开进程，拿 VM 读写权限（这里打开的是自身进程）。</summary>
		/// <param name="dwDesiredAccess">访问权限位。</param>
		/// <param name="bInheritHandle">是否可继承句柄。</param>
		/// <param name="dwProcessId">进程 ID。</param>
		/// <returns>进程句柄。</returns>
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

		// Token: 0x060000EA RID: 234
		/// <summary>kernel32：关闭句柄。</summary>
		/// <param name="hObject">句柄。</param>
		/// <returns>非 0 表示成功。</returns>
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr hObject);

		// Token: 0x060000EB RID: 235 RVA: 0x0000C04C File Offset: 0x0000A24C
		/// <summary>
		/// 把整个文件读进内存（<see cref="FileShare.Read"/> 独占读）。工程内无调用点，
		/// 原本用于读取被保护的附属文件/授权文件。
		/// </summary>
		/// <param name="路径">文件路径。</param>
		/// <returns>文件全部字节。</returns>
		private static byte[] 读整个文件(string 路径)
		{
			byte[] 内容;
			using (FileStream 文件 = new FileStream(路径, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				int 已读 = 0;
				long 长度 = 文件.Length;
				int 剩余 = (int)长度;
				内容 = new byte[剩余];
				while (剩余 > 0)
				{
					int 本次 = 文件.Read(内容, 已读, 剩余);
					已读 += 本次;
					剩余 -= 本次;
				}
			}
			return 内容;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000C0B4 File Offset: 0x0000A2B4
		/// <summary>
		/// AES 解密（Key/IV 硬编码在方法内，32 字节 Key = AES-256，IV 16 字节）。
		/// <para>
		/// 用于解开字符串记录密文；<see cref="解密字符串"/> 是它唯一的调用方。
		/// 算法实例由 <see cref="创建对称算法"/> 按 FIPS 策略给出，Mode 保持默认（CBC）。
		/// 注意没有显式 <c>FlushFinalBlock</c>，靠 <c>CryptoStream.Close()</c> 收尾——与原版一致。
		/// </para>
		/// </summary>
		/// <param name="密文">AES 密文。</param>
		/// <returns>明文字节。</returns>
		private static byte[] AES解密(byte[] 密文)
		{
			MemoryStream 输出流 = new MemoryStream();
			SymmetricAlgorithm 算法 = ListSetter.创建对称算法();
			算法.Key = new byte[]
			{
				43, 141, 39, 235, 62, 227, 120, 163, 228, 147,
				184, 192, 30, 19, 73, 36, 5, 188, 199, 56,
				130, 178, 139, 87, 75, 159, 37, 245, 127, 175,
				121, 34
			};
			算法.IV = new byte[]
			{
				84, 233, 235, 135, 93, 125, 119, 114, 87, 33,
				60, 75, 99, 97, 0, 134
			};
			CryptoStream 解流 = new CryptoStream(输出流, 算法.CreateDecryptor(), CryptoStreamMode.Write);
			解流.Write(密文, 0, 密文.Length);
			解流.Close();
			return 输出流.ToArray();
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000C124 File Offset: 0x0000A324
		/// <summary>壳留下的空桩，恒返回 <c>null</c>，工程内无调用。</summary>
		private byte[] DeleteEvent()
		{
			return null;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000C124 File Offset: 0x0000A324
		/// <summary>壳留下的空桩，恒返回 <c>null</c>，工程内无调用。</summary>
		private byte[] LoginEvent()
		{
			return null;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000C124 File Offset: 0x0000A324
		/// <summary>壳留下的空桩，恒返回 <c>null</c>，工程内无调用。</summary>
		private byte[] FindEvent()
		{
			return null;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000C124 File Offset: 0x0000A324
		/// <summary>壳留下的空桩，恒返回 <c>null</c>，工程内无调用。</summary>
		private byte[] StopEvent()
		{
			return null;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000C134 File Offset: 0x0000A334
		/// <summary>壳留下的空桩，恒返回 <c>null</c>，工程内无调用。</summary>
		private byte[] InsertEvent()
		{
			return null;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000C134 File Offset: 0x0000A334
		/// <summary>壳留下的空桩，恒返回 <c>null</c>，工程内无调用。</summary>
		private byte[] PostEvent()
		{
			return null;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000C144 File Offset: 0x0000A344
		/// <summary>
		/// IntelliLock 授权数据桩之一：先放一个 <c>{11111-22222-40001-00001}</c> 形状的许可标识字符串，
		/// 再恒返回 <c>{1,2}</c>（两句返回完全一样，<c>text.Length &gt; 0</c> 恒真，是纯混淆结构）。
		/// 工程内无调用。
		/// </summary>
		internal byte[] InitEvent()
		{
			string 许可标识 = "{11111-22222-40001-00001}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000C184 File Offset: 0x0000A384
		/// <summary>IntelliLock 授权数据桩之二（<c>{11111-22222-40001-00002}</c>），恒返回 <c>{1,2}</c>，无调用。</summary>
		internal byte[] CountEvent()
		{
			string 许可标识 = "{11111-22222-40001-00002}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000C1C4 File Offset: 0x0000A3C4
		/// <summary>IntelliLock 授权数据桩之三（<c>{11111-22222-50001-00001}</c>），恒返回 <c>{1,2}</c>，无调用。</summary>
		internal byte[] CloneEvent()
		{
			string 许可标识 = "{11111-22222-50001-00001}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000C204 File Offset: 0x0000A404
		/// <summary>IntelliLock 授权数据桩之四（<c>{11111-22222-50001-00002}</c>），恒返回 <c>{1,2}</c>，无调用。</summary>
		internal byte[] ExcludeEvent()
		{
			string 许可标识 = "{11111-22222-50001-00002}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000C244 File Offset: 0x0000A444
		/// <summary>IntelliLock 授权数据桩之五（<c>{11111-22222-60001-00001}</c>），恒返回 <c>{1,2}</c>，无调用。</summary>
		internal byte[] StartEvent()
		{
			string 许可标识 = "{11111-22222-60001-00001}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000C284 File Offset: 0x0000A484
		/// <summary>IntelliLock 授权数据桩之六（<c>{11111-22222-60001-00002}</c>），恒返回 <c>{1,2}</c>，无调用。</summary>
		internal byte[] VisitEvent()
		{
			string 许可标识 = "{11111-22222-60001-00002}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000C2C4 File Offset: 0x0000A4C4
		/// <summary>
		/// AES 加密（配置/授权数据的写盘格式），返回 Base64。
		/// <para>
		/// Key 是硬编码的 32 字节（AES-256），IV 则由 <b>MD5(UTF16LE(口令))</b> 现算——
		/// 即「固定密钥 + 口令派生 IV」。明文按 UTF-16LE 编码后加密，Mode 用默认 CBC。
		/// 与 <see cref="解密字符串"/> 的字符串方案不同，这套是给授权/配置用的对称封装。
		/// </para>
		/// <para>
		/// 本工程内无调用点（原本用于生成注册码/加密配置）。
		/// </para>
		/// </summary>
		/// <param name="明文">待加密文本。</param>
		/// <param name="口令">用于派生 IV 的口令。</param>
		/// <returns>Base64 密文。</returns>
		internal static string AES加密(string 明文, string 口令)
		{
			byte[] 明文字节 = Encoding.Unicode.GetBytes(明文);
			byte[] 待加密 = 明文字节;
			byte[] 固定密钥 = new byte[]
			{
				82, 102, 104, 110, 32, 77, 24, 34, 118, 181,
				51, 17, 18, 51, 12, 109, 10, 32, 77, 24,
				34, 158, 161, 41, 97, 28, 118, 181, 5, 25,
				1, 88
			};
			byte[] 初始化向量 = ListSetter.计算MD5(Encoding.Unicode.GetBytes(口令));
			MemoryStream 输出流 = new MemoryStream();
			SymmetricAlgorithm 算法 = ListSetter.创建对称算法();
			算法.Key = 固定密钥;
			算法.IV = 初始化向量;
			CryptoStream 加流 = new CryptoStream(输出流, 算法.CreateEncryptor(), CryptoStreamMode.Write);
			加流.Write(待加密, 0, 待加密.Length);
			加流.Close();
			return Convert.ToBase64String(输出流.ToArray());
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>壳插入的判定桩，恒返回 <c>true</c>，工程内无引用。</summary>
		internal static bool 视图规则桩()
		{
			return true;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000021E0 File Offset: 0x000003E0
		/// <summary>壳插入的判定桩，恒返回 <c>false</c>，工程内无引用。</summary>
		internal static bool 登录规则桩()
		{
			return false;
		}

		// Token: 0x040000B2 RID: 178
		/// <summary>「系统只允许 FIPS 算法」的缓存值，由 <see cref="探测FIPS策略"/> 写入。</summary>
		private static bool FIPS仅允许标志 = false;

		// Token: 0x040000B3 RID: 179
		/// <summary>未被任何方法使用的缓冲（原样保留，壳残留）。</summary>
		private static byte[] m_InstanceSetter = new byte[0];

		// Token: 0x040000B4 RID: 180
		/// <summary>字符串偏移表（解密后的数据），<see cref="解密字符串"/> 的初始化结果。</summary>
		private static byte[] 解密数据表 = new byte[0];

		// Token: 0x040000B5 RID: 181
		/// <summary>自身进程句柄（<c>PROCESS_VM_READ</c>），字符串解密时惰性打开并缓存。</summary>
		private static IntPtr 本进程句柄 = IntPtr.Zero;

		// Token: 0x040000B6 RID: 182
		/// <summary>模块基址（32 位版），取 <c>HINSTANCE</c> 的 <c>ToInt32()</c>。</summary>
		private static int 模块基址32 = 0;

		// Token: 0x040000B7 RID: 183
		/// <summary>已解密字符串的缓存表。</summary>
		private static string[] 字符串缓存表 = new string[0];

		// Token: 0x040000B8 RID: 184
		/// <summary>自补丁是否已执行（幂等标志），见 <see cref="运行时还原方法体"/>。</summary>
		private static bool 已执行还原标志 = false;

		// Token: 0x040000B9 RID: 185
		/// <summary>FIPS 策略是否已探测（幂等标志）。</summary>
		private static bool 已探测FIPS标志 = false;

		// Token: 0x040000BA RID: 186
		/// <summary>模块基址（64 位版），取 <c>HINSTANCE</c> 的 <c>ToInt64()</c>。</summary>
		private static long 模块基址64 = 0L;

		// Token: 0x040000BB RID: 187
		/// <summary>偏移表下标 → 缓存槽号 的映射（0 表示该槽未解密）。</summary>
		private static int[] 缓存槽索引 = new int[0];

		// Token: 0x040000BC RID: 188
		/// <summary><c>ReadProcessMemory</c>/<c>WriteProcessMemory</c> 的输出参数（实际字节数），仅作占位。</summary>
		private static IntPtr 读写字数 = IntPtr.Zero;

		// Token: 0x040000BD RID: 189
		/// <summary>未被任何方法使用的缓冲（原样保留，壳残留）。</summary>
		private static byte[] _ClassSetter = new byte[0];

		// Token: 0x040000BE RID: 190
		/// <summary>下一个可用的缓存槽号，初始 1（0 号槽留空表示「未缓存」）。</summary>
		private static int 下一个缓存槽 = 1;

		// Token: 0x040000BF RID: 191
		/// <summary>MD5 的 64 项常量表 T[i] = floor(2^32 × |sin(i+1)|)，原版以十进制字面量硬编码。</summary>
		private static uint[] MD5常量表 = new uint[]
		{
			3614090360U, 3905402710U, 606105819U, 3250441966U, 4118548399U, 1200080426U, 2821735955U, 4249261313U, 1770035416U, 2336552879U,
			4294925233U, 2304563134U, 1804603682U, 4254626195U, 2792965006U, 1236535329U, 4129170786U, 3225465664U, 643717713U, 3921069994U,
			3593408605U, 38016083U, 3634488961U, 3889429448U, 568446438U, 3275163606U, 4107603335U, 1163531501U, 2850285829U, 4243563512U,
			1735328473U, 2368359562U, 4294588738U, 2272392833U, 1839030562U, 4259657740U, 2763975236U, 1272893353U, 4139469664U, 3200236656U,
			681279174U, 3936430074U, 3572445317U, 76029189U, 3654602809U, 3873151461U, 530742520U, 3299628645U, 4096336452U, 1126891415U,
			2878612391U, 4237533241U, 1700485571U, 2399980690U, 4293915773U, 2240044497U, 1873313359U, 4264355552U, 2734768916U, 1309151649U,
			4149444226U, 3174756917U, 718787259U, 3951481745U
		};

		// Token: 0x040000C0 RID: 192
		/// <summary>未被任何方法使用的缓冲（原样保留，壳残留）。</summary>
		private static byte[] _ClientSetter = new byte[0];

		// Token: 0x02000016 RID: 22
		/// <summary>
		/// 壳自带的标记属性（可标在任意方法/类型上，参数是一个类型数组）。
		/// 本工程里只被 <see cref="运行时还原方法体"/> 自己标了一次，也没有任何代码读取它——
		/// 属于保护工具的元数据残留。
		/// </summary>
		internal class BaseSetter : Attribute
		{
			// Token: 0x060000FD RID: 253 RVA: 0x000026A5 File Offset: 0x000008A5
			/// <summary>构造：接收一个数组类型描述（本工程传入 <c>ParamsSetter&lt;object&gt;[]</c>），不保存也不使用。</summary>
			/// <param name="参数数组类型">参数数组类型描述。</param>
			[ListSetter.BaseSetter(typeof(ListSetter.BaseSetter.ParamsSetter<object>[]))]
			public BaseSetter(object 参数数组类型)
			{
			}

			// Token: 0x02000017 RID: 23
			/// <summary>仅用于让上面的 <c>typeof(...[])</c> 有类型可写的空泛型壳，无任何成员逻辑。</summary>
			internal class ParamsSetter<T>
			{
				// Token: 0x060000FF RID: 255 RVA: 0x000021DD File Offset: 0x000003DD
				/// <summary>壳插入的判定桩，恒返回 <c>true</c>，无引用。</summary>
				internal static bool ResolveRule()
				{
					return true;
				}

				// Token: 0x06000100 RID: 256 RVA: 0x000021E0 File Offset: 0x000003E0
				/// <summary>壳插入的判定桩，恒返回 <c>false</c>，无引用。</summary>
				internal static bool InsertRule()
				{
					return false;
				}
			}
		}

		// Token: 0x02000018 RID: 24
		/// <summary>空的 <c>[Flags]</c> 枚举，没有任何成员，也没有地方使用——壳残留。</summary>
		[Flags]
		private enum WriterSetter
		{

		}
	}
}
