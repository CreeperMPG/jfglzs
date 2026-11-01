using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace set.Internal
{
	// Token: 0x02000013 RID: 19
	internal class CollectionAttr
	{
		/// <summary>空方法（原 `method_0`），没有方法体也没有调用点。</summary>
		// Token: 0x060000FA RID: 250 RVA: 0x000027FF File Offset: 0x000009FF
		private void 空方法()
		{
		}

		/// <summary>
		/// <b>自实现 MD5 主循环</b>（标准 RFC 1321 流程，只在系统强制 FIPS 算法时才会被用到，见 <see cref="计算摘要"/>）。
		///
		/// 步骤：
		/// 1. 补齐：把输入按 512 位分组，末尾补 0x80 再补零到 448 位，最后 8 字节写原始长度（小端）；
		/// 2. 初始化 A/B/C/D 四个链值为 MD5 标准初值 0x67452301 / 0xEFCDAB89 / 0x98BADCFE / 0x10325476；
		/// 3. 每块做 4 轮 × 16 步，共 64 步：轮函数 F/G/H/I + 常量表 + 循环左移；
		/// 4. 64 步结束后把结果累加回链值；
		/// 5. 最后把 A/B/C/D 按小端拼成 16 字节摘要返回。
		/// </summary>
		/// <param name="输入">待摘要的数据（反编译成 <c>object</c>，实际是 <c>byte[]</c>）。</param>
		// Token: 0x060000FB RID: 251 RVA: 0x0000B444 File Offset: 0x00009644
		internal static byte[] 自实现MD5(object 输入)
		{
			// X：每块拆成的 16 个 32 位字
			uint[] X = new uint[16];
			int 补齐目标位 = 448 - 输入.Length * 8 % 512;
			uint 补齐字节数 = (uint)((补齐目标位 + 512) % 512);
			if (补齐字节数 == 0U)
			{
				补齐字节数 = 512U;
			}
			uint 总字节数 = (uint)((long)输入.Length + (long)((ulong)(补齐字节数 / 8U)) + 8L);
			ulong 原始长度位 = (ulong)((long)输入.Length * 8L);
			byte[] 缓冲 = new byte[总字节数];
			for (int i = 0; i < 输入.Length; i++)
			{
				缓冲[i] = 输入[i];
			}
			byte[] 缓冲别名 = 缓冲;
			int 原长度 = 输入.Length;
			缓冲别名[原长度] |= 128;
			// 末尾 8 字节写入原始长度（小端）
			for (int j = 8; j > 0; j--)
			{
				缓冲[(int)(checked((IntPtr)(unchecked((ulong)总字节数 - (ulong)((long)j)))))] = (byte)((原始长度位 >> (8 - j) * 8) & 255UL);
			}
			uint 块数 = (uint)(缓冲.Length * 8 / 32);
			uint A = 1732584193U;
			uint B = 4023233417U;
			uint C = 2562383102U;
			uint D = 271733878U;
			for (uint 块 = 0U; 块 < 块数 / 16U; 块 += 1U)
			{
				uint 块偏移 = 块 << 6;
				for (uint 偏移 = 0U; 偏移 < 61U; 偏移 += 4U)
				{
					// 4 个字节按小端拼成一个 32 位字
					X[(int)((UIntPtr)(偏移 >> 2))] = (uint)(((int)缓冲[(int)((UIntPtr)(块偏移 + (偏移 + 3U)))] << 24) | ((int)缓冲[(int)((UIntPtr)(块偏移 + (偏移 + 2U)))] << 16) | ((int)缓冲[(int)((UIntPtr)(块偏移 + (偏移 + 1U)))] << 8) | (int)缓冲[(int)((UIntPtr)(块偏移 + 偏移))]);
				}
				uint AA = A;
				uint BB = B;
				uint CC = C;
				uint DD = D;
				// 第 1 轮：轮函数 F，移位 7/12/17/22
				CollectionAttr.MD5轮函数F(ref A, B, C, D, 0U, 7, 1U, X);
				CollectionAttr.MD5轮函数F(ref D, A, B, C, 1U, 12, 2U, X);
				CollectionAttr.MD5轮函数F(ref C, D, A, B, 2U, 17, 3U, X);
				CollectionAttr.MD5轮函数F(ref B, C, D, A, 3U, 22, 4U, X);
				CollectionAttr.MD5轮函数F(ref A, B, C, D, 4U, 7, 5U, X);
				CollectionAttr.MD5轮函数F(ref D, A, B, C, 5U, 12, 6U, X);
				CollectionAttr.MD5轮函数F(ref C, D, A, B, 6U, 17, 7U, X);
				CollectionAttr.MD5轮函数F(ref B, C, D, A, 7U, 22, 8U, X);
				CollectionAttr.MD5轮函数F(ref A, B, C, D, 8U, 7, 9U, X);
				CollectionAttr.MD5轮函数F(ref D, A, B, C, 9U, 12, 10U, X);
				CollectionAttr.MD5轮函数F(ref C, D, A, B, 10U, 17, 11U, X);
				CollectionAttr.MD5轮函数F(ref B, C, D, A, 11U, 22, 12U, X);
				CollectionAttr.MD5轮函数F(ref A, B, C, D, 12U, 7, 13U, X);
				CollectionAttr.MD5轮函数F(ref D, A, B, C, 13U, 12, 14U, X);
				CollectionAttr.MD5轮函数F(ref C, D, A, B, 14U, 17, 15U, X);
				CollectionAttr.MD5轮函数F(ref B, C, D, A, 15U, 22, 16U, X);
				// 第 2 轮：轮函数 G，移位 5/9/14/20
				CollectionAttr.MD5轮函数G(ref A, B, C, D, 1U, 5, 17U, X);
				CollectionAttr.MD5轮函数G(ref D, A, B, C, 6U, 9, 18U, X);
				CollectionAttr.MD5轮函数G(ref C, D, A, B, 11U, 14, 19U, X);
				CollectionAttr.MD5轮函数G(ref B, C, D, A, 0U, 20, 20U, X);
				CollectionAttr.MD5轮函数G(ref A, B, C, D, 5U, 5, 21U, X);
				CollectionAttr.MD5轮函数G(ref D, A, B, C, 10U, 9, 22U, X);
				CollectionAttr.MD5轮函数G(ref C, D, A, B, 15U, 14, 23U, X);
				CollectionAttr.MD5轮函数G(ref B, C, D, A, 4U, 20, 24U, X);
				CollectionAttr.MD5轮函数G(ref A, B, C, D, 9U, 5, 25U, X);
				CollectionAttr.MD5轮函数G(ref D, A, B, C, 14U, 9, 26U, X);
				CollectionAttr.MD5轮函数G(ref C, D, A, B, 3U, 14, 27U, X);
				CollectionAttr.MD5轮函数G(ref B, C, D, A, 8U, 20, 28U, X);
				CollectionAttr.MD5轮函数G(ref A, B, C, D, 13U, 5, 29U, X);
				CollectionAttr.MD5轮函数G(ref D, A, B, C, 2U, 9, 30U, X);
				CollectionAttr.MD5轮函数G(ref C, D, A, B, 7U, 14, 31U, X);
				CollectionAttr.MD5轮函数G(ref B, C, D, A, 12U, 20, 32U, X);
				// 第 3 轮：轮函数 H，移位 4/11/16/23
				CollectionAttr.MD5轮函数H(ref A, B, C, D, 5U, 4, 33U, X);
				CollectionAttr.MD5轮函数H(ref D, A, B, C, 8U, 11, 34U, X);
				CollectionAttr.MD5轮函数H(ref C, D, A, B, 11U, 16, 35U, X);
				CollectionAttr.MD5轮函数H(ref B, C, D, A, 14U, 23, 36U, X);
				CollectionAttr.MD5轮函数H(ref A, B, C, D, 1U, 4, 37U, X);
				CollectionAttr.MD5轮函数H(ref D, A, B, C, 4U, 11, 38U, X);
				CollectionAttr.MD5轮函数H(ref C, D, A, B, 7U, 16, 39U, X);
				CollectionAttr.MD5轮函数H(ref B, C, D, A, 10U, 23, 40U, X);
				CollectionAttr.MD5轮函数H(ref A, B, C, D, 13U, 4, 41U, X);
				CollectionAttr.MD5轮函数H(ref D, A, B, C, 0U, 11, 42U, X);
				CollectionAttr.MD5轮函数H(ref C, D, A, B, 3U, 16, 43U, X);
				CollectionAttr.MD5轮函数H(ref B, C, D, A, 6U, 23, 44U, X);
				CollectionAttr.MD5轮函数H(ref A, B, C, D, 9U, 4, 45U, X);
				CollectionAttr.MD5轮函数H(ref D, A, B, C, 12U, 11, 46U, X);
				CollectionAttr.MD5轮函数H(ref C, D, A, B, 15U, 16, 47U, X);
				CollectionAttr.MD5轮函数H(ref B, C, D, A, 2U, 23, 48U, X);
				// 第 4 轮：轮函数 I，移位 6/10/15/21
				CollectionAttr.MD5轮函数I(ref A, B, C, D, 0U, 6, 49U, X);
				CollectionAttr.MD5轮函数I(ref D, A, B, C, 7U, 10, 50U, X);
				CollectionAttr.MD5轮函数I(ref C, D, A, B, 14U, 15, 51U, X);
				CollectionAttr.MD5轮函数I(ref B, C, D, A, 5U, 21, 52U, X);
				CollectionAttr.MD5轮函数I(ref A, B, C, D, 12U, 6, 53U, X);
				CollectionAttr.MD5轮函数I(ref D, A, B, C, 3U, 10, 54U, X);
				CollectionAttr.MD5轮函数I(ref C, D, A, B, 10U, 15, 55U, X);
				CollectionAttr.MD5轮函数I(ref B, C, D, A, 1U, 21, 56U, X);
				CollectionAttr.MD5轮函数I(ref A, B, C, D, 8U, 6, 57U, X);
				CollectionAttr.MD5轮函数I(ref D, A, B, C, 15U, 10, 58U, X);
				CollectionAttr.MD5轮函数I(ref C, D, A, B, 6U, 15, 59U, X);
				CollectionAttr.MD5轮函数I(ref B, C, D, A, 13U, 21, 60U, X);
				CollectionAttr.MD5轮函数I(ref A, B, C, D, 4U, 6, 61U, X);
				CollectionAttr.MD5轮函数I(ref D, A, B, C, 11U, 10, 62U, X);
				CollectionAttr.MD5轮函数I(ref C, D, A, B, 2U, 15, 63U, X);
				CollectionAttr.MD5轮函数I(ref B, C, D, A, 9U, 21, 64U, X);
				// 累加回链值
				A += AA;
				B += BB;
				C += CC;
				D += DD;
			}
			// 输出：A/B/C/D 各 4 字节小端
			byte[] 摘要 = new byte[16];
			Array.Copy(BitConverter.GetBytes(A), 0, 摘要, 0, 4);
			Array.Copy(BitConverter.GetBytes(B), 0, 摘要, 4, 4);
			Array.Copy(BitConverter.GetBytes(C), 0, 摘要, 8, 4);
			Array.Copy(BitConverter.GetBytes(D), 0, 摘要, 12, 4);
			return 摘要;
		}

		/// <summary>MD5 第 1 轮轮函数 F：<c>a = b + rol(a + ((b &amp; c) | (~b &amp; d)) + X[k] + T[i], s)</c>。</summary>
		// Token: 0x060000FC RID: 252 RVA: 0x00002801 File Offset: 0x00000A01
		private static void MD5轮函数F(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i, object X)
		{
			a = b + CollectionAttr.循环左移(a + ((b & c) | (~b & d)) + X[(int)((UIntPtr)k)] + CollectionAttr.MD5常量表[(int)((UIntPtr)(i - 1U))], s);
		}

		/// <summary>MD5 第 2 轮轮函数 G：<c>a = b + rol(a + ((b &amp; d) | (c &amp; ~d)) + X[k] + T[i], s)</c>。</summary>
		// Token: 0x060000FD RID: 253 RVA: 0x0000282C File Offset: 0x00000A2C
		private static void MD5轮函数G(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i, object X)
		{
			a = b + CollectionAttr.循环左移(a + ((b & d) | (c & ~d)) + X[(int)((UIntPtr)k)] + CollectionAttr.MD5常量表[(int)((UIntPtr)(i - 1U))], s);
		}

		/// <summary>MD5 第 3 轮轮函数 H：<c>a = b + rol(a + (b ^ c ^ d) + X[k] + T[i], s)</c>。</summary>
		// Token: 0x060000FE RID: 254 RVA: 0x00002857 File Offset: 0x00000A57
		private static void MD5轮函数H(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i, object X)
		{
			a = b + CollectionAttr.循环左移(a + (b ^ c ^ d) + X[(int)((UIntPtr)k)] + CollectionAttr.MD5常量表[(int)((UIntPtr)(i - 1U))], s);
		}

		/// <summary>MD5 第 4 轮轮函数 I：<c>a = b + rol(a + (c ^ (b | ~d)) + X[k] + T[i], s)</c>。</summary>
		// Token: 0x060000FF RID: 255 RVA: 0x0000287F File Offset: 0x00000A7F
		private static void MD5轮函数I(ref uint a, uint b, uint c, uint d, uint k, ushort s, uint i, object X)
		{
			a = b + CollectionAttr.循环左移(a + (c ^ (b | ~d)) + X[(int)((UIntPtr)k)] + CollectionAttr.MD5常量表[(int)((UIntPtr)(i - 1U))], s);
		}

		/// <summary>32 位循环左移：右移 (32-s) 位 | 左移 s 位。</summary>
		// Token: 0x06000100 RID: 256 RVA: 0x000028A8 File Offset: 0x00000AA8
		private static uint 循环左移(uint 值, ushort s)
		{
			return (值 >> (int)(32 - s)) | (值 << (int)s);
		}

		/// <summary>系统是否「只允许 FIPS 认证算法」。只探测一次，结果缓存在 <see cref="仅FIPS算法"/>。</summary>
		// Token: 0x06000101 RID: 257 RVA: 0x000028BA File Offset: 0x00000ABA
		internal static bool 是否FIPS模式()
		{
			if (!CollectionAttr.已读取FIPS开关)
			{
				CollectionAttr.读取FIPS开关();
				CollectionAttr.已读取FIPS开关 = true;
			}
			return CollectionAttr.仅FIPS算法;
		}

		/// <summary>反射读 <c>CryptoConfig.AllowOnlyFipsAlgorithms</c>，读不到就当 false。</summary>
		// Token: 0x06000102 RID: 258 RVA: 0x0000BAE8 File Offset: 0x00009CE8
		internal static void 读取FIPS开关()
		{
			try
			{
				CollectionAttr.仅FIPS算法 = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
			}
		}

		/// <summary>按 FIPS 开关挑对称算法：FIPS 优先 AesCryptoServiceProvider（失败退回 RijndaelManaged），否则 RijndaelManaged。</summary>
		// Token: 0x06000103 RID: 259 RVA: 0x0000BB48 File Offset: 0x00009D48
		internal static SymmetricAlgorithm 新建对称算法()
		{
			SymmetricAlgorithm 结果 = null;
			if (CollectionAttr.是否FIPS模式())
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
			symmetricAlgorithm = new RijndaelManaged();
			return 结果;
		}

		/// <summary>
		/// 计算摘要（16 字节 MD5）：FIPS 模式走 <see cref="自实现MD5"/>，否则用系统 <see cref="MD5CryptoServiceProvider"/>。
		/// 这个值就是 <c>CandidateCandidate</c> 拼内嵌程序集资源名时那段 Base64 的原始数据。
		/// </summary>
		// Token: 0x06000104 RID: 260 RVA: 0x000028D3 File Offset: 0x00000AD3
		internal static byte[] 计算摘要(byte[] 数据)
		{
			if (!CollectionAttr.是否FIPS模式())
			{
				return new MD5CryptoServiceProvider().ComputeHash(数据);
			}
			return CollectionAttr.自实现MD5(数据);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000BB88 File Offset: 0x00009D88
		/// <summary>
		/// <b>AES 解密</b>内嵌资源（.NET Reactor 的资源解密器）。
		/// 从清单资源 <c>c4a6c669-082b-46ec-be30-7d414450bf0b</c> 里读出加密数据，
		/// 用 <see cref="新建对称算法"/> 建出的 AES/Rijndael 对象按 <paramref name="int_0"/> 指定的一段做解密并转成字符串。
		/// 本工程内没有调用点 —— 由 Reactor 生成的初始化代码在运行期调用。
		/// </summary>
		/// <param name="int_0">要取第几段（内部的偏移/索引）。</param>
		static string ExcludeAttr(int int_0)
		{
			if (CollectionAttr._BroadcasterAttr.Length == 0)
			{
				BinaryReader binaryReader = new BinaryReader(typeof(CollectionAttr).Assembly.GetManifestResourceStream("c4a6c669-082b-46ec-be30-7d414450bf0b"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 166;
				array2[0] = 160;
				array2[0] = 120;
				array2[0] = 138;
				array2[0] = 166;
				array2[1] = 132;
				array2[1] = 106;
				array2[1] = 211;
				array2[1] = 92;
				array2[1] = 109;
				array2[1] = 37;
				array2[2] = 189;
				array2[2] = 112;
				array2[2] = 83;
				array2[2] = 162;
				array2[2] = 176;
				array2[3] = 54;
				array2[3] = 92;
				array2[3] = 185;
				array2[4] = 164;
				array2[4] = 82;
				array2[4] = 230;
				array2[4] = 139;
				array2[4] = 44;
				array2[5] = 112;
				array2[5] = 169;
				array2[5] = 26;
				array2[6] = 175;
				array2[6] = 165;
				array2[6] = 130;
				array2[6] = 117;
				array2[6] = 118;
				array2[6] = 245;
				array2[7] = 121;
				array2[7] = 87;
				array2[7] = 90;
				array2[7] = 172;
				array2[7] = 155;
				array2[7] = 169;
				array2[8] = 97;
				array2[8] = 134;
				array2[8] = 30;
				array2[9] = 127;
				array2[9] = 188;
				array2[9] = 46;
				array2[10] = 78;
				array2[10] = 151;
				array2[10] = 140;
				array2[10] = 121;
				array2[10] = 168;
				array2[10] = 112;
				array2[11] = 47;
				array2[11] = 83;
				array2[11] = 201;
				array2[11] = 138;
				array2[11] = 100;
				array2[11] = 243;
				array2[12] = 111;
				array2[12] = 155;
				array2[12] = 97;
				array2[12] = 97;
				array2[13] = 124;
				array2[13] = 164;
				array2[13] = 68;
				array2[13] = 149;
				array2[13] = 165;
				array2[13] = 177;
				array2[14] = 166;
				array2[14] = 31;
				array2[14] = 163;
				array2[14] = 175;
				array2[15] = 224;
				array2[15] = 118;
				array2[15] = 40;
				array2[15] = 73;
				array2[15] = 156;
				array2[16] = 156;
				array2[16] = 140;
				array2[16] = 215;
				array2[16] = 152;
				array2[16] = 85;
				array2[16] = 119;
				array2[17] = 87;
				array2[17] = 45;
				array2[17] = 154;
				array2[17] = 160;
				array2[17] = 89;
				array2[17] = 143;
				array2[18] = 80;
				array2[18] = 57;
				array2[18] = 206;
				array2[18] = 135;
				array2[19] = 100;
				array2[19] = 101;
				array2[19] = 106;
				array2[19] = 92;
				array2[19] = 36;
				array2[20] = 162;
				array2[20] = 150;
				array2[20] = 102;
				array2[20] = 224;
				array2[21] = 119;
				array2[21] = 108;
				array2[21] = 107;
				array2[21] = 138;
				array2[21] = 152;
				array2[22] = 95;
				array2[22] = 132;
				array2[22] = 93;
				array2[22] = 86;
				array2[22] = 128;
				array2[23] = 120;
				array2[23] = 67;
				array2[23] = 28;
				array2[24] = 166;
				array2[24] = 159;
				array2[24] = 131;
				array2[24] = 34;
				array2[25] = 190;
				array2[25] = 160;
				array2[25] = 149;
				array2[25] = 119;
				array2[25] = 139;
				array2[25] = 78;
				array2[26] = 132;
				array2[26] = 132;
				array2[26] = 95;
				array2[26] = 49;
				array2[26] = 129;
				array2[27] = 132;
				array2[27] = 127;
				array2[27] = 153;
				array2[28] = 153;
				array2[28] = 98;
				array2[28] = 170;
				array2[28] = 135;
				array2[28] = 92;
				array2[28] = 97;
				array2[29] = 90;
				array2[29] = 116;
				array2[29] = 244;
				array2[30] = 120;
				array2[30] = 102;
				array2[30] = 107;
				array2[30] = 170;
				array2[31] = 88;
				array2[31] = 167;
				array2[31] = 83;
				array2[31] = 43;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 177;
				array4[0] = 50;
				array4[0] = 85;
				array4[0] = 78;
				array4[0] = 48;
				array4[1] = 57;
				array4[1] = 118;
				array4[1] = 165;
				array4[1] = 74;
				array4[2] = 180;
				array4[2] = 143;
				array4[2] = 90;
				array4[2] = 130;
				array4[3] = 158;
				array4[3] = 203;
				array4[3] = 54;
				array4[3] = 190;
				array4[3] = 140;
				array4[3] = 188;
				array4[4] = 59;
				array4[4] = 110;
				array4[4] = 58;
				array4[4] = 149;
				array4[5] = 119;
				array4[5] = 144;
				array4[5] = 177;
				array4[5] = 124;
				array4[5] = 134;
				array4[5] = 109;
				array4[6] = 156;
				array4[6] = 156;
				array4[6] = 130;
				array4[6] = 191;
				array4[6] = 81;
				array4[6] = 223;
				array4[7] = 46;
				array4[7] = 151;
				array4[7] = 97;
				array4[7] = 92;
				array4[7] = 154;
				array4[8] = 166;
				array4[8] = 77;
				array4[8] = 160;
				array4[9] = 156;
				array4[9] = 170;
				array4[9] = 132;
				array4[9] = 120;
				array4[9] = 107;
				array4[9] = 236;
				array4[10] = 145;
				array4[10] = 204;
				array4[10] = 207;
				array4[10] = 159;
				array4[11] = 141;
				array4[11] = 125;
				array4[11] = 143;
				array4[11] = 154;
				array4[12] = 96;
				array4[12] = 69;
				array4[12] = 153;
				array4[12] = 158;
				array4[12] = 169;
				array4[12] = 90;
				array4[13] = 165;
				array4[13] = 163;
				array4[13] = 125;
				array4[13] = 174;
				array4[13] = 28;
				array4[14] = 128;
				array4[14] = 134;
				array4[14] = 140;
				array4[14] = 223;
				array4[15] = 120;
				array4[15] = 142;
				array4[15] = 248;
				byte[] array5 = array4;
				byte[] publicKeyToken = typeof(CollectionAttr).Assembly.GetName().GetPublicKeyToken();
				if (publicKeyToken != null && publicKeyToken.Length > 0)
				{
					array5[1] = publicKeyToken[0];
					array5[3] = publicKeyToken[1];
					array5[5] = publicKeyToken[2];
					array5[7] = publicKeyToken[3];
					array5[9] = publicKeyToken[4];
					array5[11] = publicKeyToken[5];
					array5[13] = publicKeyToken[6];
					array5[15] = publicKeyToken[7];
				}
				for (int i = 0; i < array5.Length; i++)
				{
					array3[i] ^= array5[i];
				}
				if (int_0 == -1)
				{
					SymmetricAlgorithm symmetricAlgorithm = CollectionAttr.新建对称算法();
					symmetricAlgorithm.Mode = CipherMode.CBC;
					ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor(array3, array5);
					MemoryStream memoryStream = new MemoryStream();
					CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
					cryptoStream.Write(array, 0, array.Length);
					cryptoStream.FlushFinalBlock();
					CollectionAttr._BroadcasterAttr = memoryStream.ToArray();
					if (CollectionAttr._BroadcasterAttr.Length > 0)
					{
						CollectionAttr.m_MerchantAttr = new string[CollectionAttr._BroadcasterAttr.Length / 4 + 1];
						CollectionAttr.callbackAttr = new int[CollectionAttr._BroadcasterAttr.Length / 4 + 1];
					}
					memoryStream.Close();
					cryptoStream.Close();
					binaryReader.Close();
					array = CollectionAttr._BroadcasterAttr;
				}
				int num = array.Length % 4;
				int num2 = array.Length / 4;
				byte[] array6 = new byte[array.Length];
				int num3 = array3.Length / 4;
				uint num4 = 0U;
				if (num > 0)
				{
					num2++;
				}
				for (int j = 0; j < num2; j++)
				{
					int num5 = j % num3;
					int num6 = j * 4;
					uint num7 = (uint)(num5 * 4);
					uint num8 = (uint)(((int)array3[(int)((UIntPtr)(num7 + 3U))] << 24) | ((int)array3[(int)((UIntPtr)(num7 + 2U))] << 16) | ((int)array3[(int)((UIntPtr)(num7 + 1U))] << 8) | (int)array3[(int)((UIntPtr)num7)]);
					uint num9 = 255U;
					int num10 = 0;
					uint num11;
					if (j == num2 - 1 && num > 0)
					{
						num11 = 0U;
						for (int k = 0; k < num; k++)
						{
							if (k > 0)
							{
								num11 <<= 8;
							}
							num11 |= (uint)array[array.Length - (1 + k)];
						}
						num4 += num8;
					}
					else
					{
						num7 = (uint)num6;
						num11 = (uint)(((int)array[(int)((UIntPtr)(num7 + 3U))] << 24) | ((int)array[(int)((UIntPtr)(num7 + 2U))] << 16) | ((int)array[(int)((UIntPtr)(num7 + 1U))] << 8) | (int)array[(int)((UIntPtr)num7)]);
						num4 += num8;
					}
					uint num12 = num4;
					uint num13 = num4;
					num13 ^= num13 >> 25;
					num13 += 1394801171U;
					num13 ^= num13 << 5;
					num13 += 3457341307U;
					num13 ^= num13 >> 3;
					num13 += 4040842541U;
					num13 = 2018246656U + num13;
					num4 = num12 + (uint)num13;
					if (j == num2 - 1 && num > 0)
					{
						uint num14 = num4 ^ num11;
						for (int l = 0; l < num; l++)
						{
							if (l > 0)
							{
								num9 <<= 8;
								num10 += 8;
							}
							array6[num6 + l] = (byte)((num14 & num9) >> num10);
						}
					}
					else
					{
						uint num15 = num4 ^ num11;
						array6[num6] = (byte)(num15 & 255U);
						array6[num6 + 1] = (byte)((num15 & 65280U) >> 8);
						array6[num6 + 2] = (byte)((num15 & 16711680U) >> 16);
						array6[num6 + 3] = (byte)((num15 & 4278190080U) >> 24);
					}
				}
				CollectionAttr._BroadcasterAttr = array6;
				if (CollectionAttr._BroadcasterAttr.Length > 0)
				{
					CollectionAttr.m_MerchantAttr = new string[CollectionAttr._BroadcasterAttr.Length / 4 + 1];
					CollectionAttr.callbackAttr = new int[CollectionAttr._BroadcasterAttr.Length / 4 + 1];
				}
				Assembly assembly = typeof(CollectionAttr).Assembly;
				if (IntPtr.Size == 4)
				{
					CollectionAttr._WrapperAttr = Marshal.GetHINSTANCE(assembly.GetModules()[0]).ToInt32();
				}
				CollectionAttr.m_CustomerAttr = Marshal.GetHINSTANCE(assembly.GetModules()[0]).ToInt64();
			}
			int num16 = int_0 / 4;
			if (CollectionAttr.callbackAttr[num16] > 0)
			{
				return ((string[])CollectionAttr.m_MerchantAttr)[CollectionAttr.callbackAttr[num16]];
			}
			int num17 = BitConverter.ToInt32(CollectionAttr._BroadcasterAttr, int_0);
			if (CollectionAttr._InitializerAttr == IntPtr.Zero)
			{
				CollectionAttr._InitializerAttr = CollectionAttr.OpenProcess(16U, 1, (uint)Process.GetCurrentProcess().Id);
			}
			byte[] array7 = new byte[4];
			if (IntPtr.Size == 4)
			{
				CollectionAttr.ReadProcessMemory(CollectionAttr._InitializerAttr, new IntPtr(CollectionAttr._WrapperAttr + num17), array7, 4U, out CollectionAttr._SingletonAttr);
			}
			else
			{
				CollectionAttr.ReadProcessMemory(CollectionAttr._InitializerAttr, new IntPtr(CollectionAttr.m_CustomerAttr + (long)num17), array7, 4U, out CollectionAttr._SingletonAttr);
			}
			int num18 = BitConverter.ToInt32(array7, 0);
			array7 = new byte[num18];
			if (IntPtr.Size == 4)
			{
				CollectionAttr.ReadProcessMemory(CollectionAttr._InitializerAttr, new IntPtr(CollectionAttr._WrapperAttr + num17 + 4), array7, Convert.ToUInt32(num18), out CollectionAttr._SingletonAttr);
			}
			else
			{
				CollectionAttr.ReadProcessMemory(CollectionAttr._InitializerAttr, new IntPtr(CollectionAttr.m_CustomerAttr + (long)num17 + 4L), array7, Convert.ToUInt32(num18), out CollectionAttr._SingletonAttr);
			}
			byte[] array8 = CollectionAttr.UpdateAttr(array7);
			string @string = Encoding.Unicode.GetString(array8, 0, array8.Length);
			CollectionAttr.callbackAttr[num16] = CollectionAttr.invocationAttr;
			((string[])CollectionAttr.m_MerchantAttr)[CollectionAttr.invocationAttr] = @string;
			CollectionAttr.invocationAttr++;
			return @string;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000CD24 File Offset: 0x0000AF24
		internal static string GetAttr(string string_0)
		{
			byte[] array = Convert.FromBase64String(string_0);
			return Encoding.Unicode.GetString(array, 0, array.Length);
		}

		// Token: 0x06000107 RID: 263
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr intptr_0, int int_0, int int_1, ref int int_2);

		// Token: 0x06000108 RID: 264 RVA: 0x0000CD48 File Offset: 0x0000AF48
		[CollectionAttr.PublisherAttr(typeof(CollectionAttr.PublisherAttr.ListAttr<object>[]))]
		/// <summary>
		/// Reactor 的「在线程里改写 JIT 后代码」段：配合 <c>VirtualProtect</c> / <c>WriteProcessMemory</c> 一类 API
		/// 对已加载的托管方法做内存补丁（本工程内无调用点，全是壳的运行时残留）。
		/// </summary>
		static void PublishAttr()
		{
			if (!CollectionAttr.proccesorAttr)
			{
				CollectionAttr.proccesorAttr = true;
				BinaryReader binaryReader = new BinaryReader(typeof(CollectionAttr).Assembly.GetManifestResourceStream("a5bad3b5-e40f-4fda-bdcf-d44ab2b77bc9"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 118;
				array2[0] = 136;
				array2[0] = 160;
				array2[0] = 155;
				array2[0] = 118;
				array2[1] = 198;
				array2[1] = 100;
				array2[1] = 135;
				array2[1] = 198;
				array2[2] = 201;
				array2[2] = 142;
				array2[2] = 94;
				array2[2] = 88;
				array2[2] = 89;
				array2[2] = 201;
				array2[3] = 10;
				array2[3] = 92;
				array2[3] = 116;
				array2[3] = 122;
				array2[3] = 26;
				array2[3] = 137;
				array2[3] = 10;
				array2[4] = 137;
				array2[4] = 154;
				array2[4] = 87;
				array2[4] = 137;
				array2[5] = 35;
				array2[5] = 157;
				array2[5] = 156;
				array2[5] = 122;
				array2[5] = 35;
				array2[6] = 204;
				array2[6] = 132;
				array2[6] = 119;
				array2[6] = 167;
				array2[6] = 204;
				array2[7] = 209;
				array2[7] = 142;
				array2[7] = 120;
				array2[7] = 103;
				array2[7] = 120;
				array2[7] = 75;
				array2[7] = 209;
				array2[8] = 167;
				array2[8] = 103;
				array2[8] = 160;
				array2[8] = 102;
				array2[8] = 167;
				array2[9] = 188;
				array2[9] = 174;
				array2[9] = 104;
				array2[9] = 95;
				array2[9] = 188;
				array2[10] = 49;
				array2[10] = 101;
				array2[10] = 133;
				array2[10] = 27;
				array2[10] = 148;
				array2[10] = 49;
				array2[11] = 137;
				array2[11] = 43;
				array2[11] = 109;
				array2[11] = 90;
				array2[11] = 90;
				array2[11] = 16;
				array2[11] = 137;
				array2[12] = 181;
				array2[12] = 165;
				array2[12] = 159;
				array2[12] = 168;
				array2[12] = 181;
				array2[13] = 40;
				array2[13] = 190;
				array2[13] = 168;
				array2[13] = 89;
				array2[13] = 84;
				array2[13] = 40;
				array2[14] = 63;
				array2[14] = 134;
				array2[14] = 156;
				array2[14] = 149;
				array2[14] = 63;
				array2[15] = 251;
				array2[15] = 169;
				array2[15] = 148;
				array2[15] = 152;
				array2[15] = 130;
				array2[15] = 251;
				array2[16] = 110;
				array2[16] = 130;
				array2[16] = 144;
				array2[16] = 117;
				array2[16] = 118;
				array2[16] = 169;
				array2[16] = 110;
				array2[17] = 7;
				array2[17] = 100;
				array2[17] = 168;
				array2[17] = 177;
				array2[17] = 17;
				array2[17] = 135;
				array2[17] = 7;
				array2[18] = 48;
				array2[18] = 192;
				array2[18] = 146;
				array2[18] = 124;
				array2[18] = 48;
				array2[19] = 67;
				array2[19] = 163;
				array2[19] = 131;
				array2[19] = 155;
				array2[19] = 179;
				array2[19] = 114;
				array2[19] = 67;
				array2[20] = 147;
				array2[20] = 139;
				array2[20] = 134;
				array2[20] = 148;
				array2[20] = 143;
				array2[20] = 116;
				array2[20] = 147;
				array2[21] = 95;
				array2[21] = 121;
				array2[21] = 119;
				array2[21] = 83;
				array2[21] = 95;
				array2[22] = 112;
				array2[22] = 158;
				array2[22] = 56;
				array2[22] = 172;
				array2[22] = 90;
				array2[22] = 112;
				array2[23] = 213;
				array2[23] = 167;
				array2[23] = 160;
				array2[23] = 166;
				array2[23] = 127;
				array2[23] = 28;
				array2[23] = 213;
				array2[24] = 56;
				array2[24] = 92;
				array2[24] = 148;
				array2[24] = 100;
				array2[24] = 85;
				array2[24] = 56;
				array2[25] = 251;
				array2[25] = 80;
				array2[25] = 125;
				array2[25] = 103;
				array2[25] = 251;
				array2[26] = 78;
				array2[26] = 61;
				array2[26] = 134;
				array2[26] = 100;
				array2[26] = 88;
				array2[26] = 78;
				array2[27] = 119;
				array2[27] = 150;
				array2[27] = 158;
				array2[27] = 103;
				array2[27] = 86;
				array2[27] = 148;
				array2[27] = 119;
				array2[28] = 103;
				array2[28] = 128;
				array2[28] = 116;
				array2[28] = 93;
				array2[28] = 86;
				array2[28] = 103;
				array2[29] = 95;
				array2[29] = 124;
				array2[29] = 135;
				array2[29] = 198;
				array2[29] = 160;
				array2[29] = 95;
				array2[30] = 45;
				array2[30] = 110;
				array2[30] = 169;
				array2[30] = 45;
				array2[31] = 44;
				array2[31] = 114;
				array2[31] = 134;
				array2[31] = 97;
				array2[31] = 87;
				array2[31] = 114;
				array2[31] = 44;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 136;
				array4[0] = 164;
				array4[0] = 198;
				array4[1] = 87;
				array4[1] = 100;
				array4[1] = 178;
				array4[2] = 108;
				array4[2] = 97;
				array4[2] = 73;
				array4[3] = 72;
				array4[3] = 89;
				array4[3] = 131;
				array4[4] = 147;
				array4[4] = 70;
				array4[4] = 121;
				array4[4] = 148;
				array4[5] = 26;
				array4[5] = 154;
				array4[5] = 152;
				array4[5] = 74;
				array4[6] = 87;
				array4[6] = 124;
				array4[6] = 160;
				array4[6] = 156;
				array4[6] = 128;
				array4[6] = 89;
				array4[7] = 46;
				array4[7] = 94;
				array4[7] = 228;
				array4[8] = 167;
				array4[8] = 140;
				array4[8] = 142;
				array4[8] = 159;
				array4[9] = 101;
				array4[9] = 134;
				array4[9] = 247;
				array4[10] = 116;
				array4[10] = 114;
				array4[10] = 194;
				array4[10] = 146;
				array4[10] = 97;
				array4[10] = 108;
				array4[11] = 151;
				array4[11] = 69;
				array4[11] = 156;
				array4[11] = 65;
				array4[11] = 91;
				array4[11] = 155;
				array4[12] = 159;
				array4[12] = 137;
				array4[12] = 75;
				array4[12] = 113;
				array4[12] = 92;
				array4[12] = 9;
				array4[13] = 90;
				array4[13] = 16;
				array4[13] = 75;
				array4[13] = 94;
				array4[13] = 52;
				array4[13] = 180;
				array4[14] = 190;
				array4[14] = 198;
				array4[14] = 179;
				array4[15] = 84;
				array4[15] = 86;
				array4[15] = 124;
				array4[15] = 140;
				byte[] array5 = array4;
				Array.Reverse(array5);
				byte[] publicKeyToken = typeof(CollectionAttr).Assembly.GetName().GetPublicKeyToken();
				if (publicKeyToken != null && publicKeyToken.Length > 0)
				{
					array5[1] = publicKeyToken[0];
					array5[3] = publicKeyToken[1];
					array5[5] = publicKeyToken[2];
					array5[7] = publicKeyToken[3];
					array5[9] = publicKeyToken[4];
					array5[11] = publicKeyToken[5];
					array5[13] = publicKeyToken[6];
					array5[15] = publicKeyToken[7];
					Array.Clear(publicKeyToken, 0, publicKeyToken.Length);
				}
				for (int i = 0; i < array5.Length; i++)
				{
					array3[i] ^= array5[i];
				}
				byte[] array6 = array;
				int num = array6.Length % 4;
				int num2 = array6.Length / 4;
				byte[] array7 = new byte[array6.Length];
				int num3 = array3.Length / 4;
				uint num4 = 0U;
				if (num > 0)
				{
					num2++;
				}
				for (int j = 0; j < num2; j++)
				{
					int num5 = j % num3;
					int num6 = j * 4;
					uint num7 = (uint)(num5 * 4);
					uint num8 = (uint)(((int)array3[(int)((UIntPtr)(num7 + 3U))] << 24) | ((int)array3[(int)((UIntPtr)(num7 + 2U))] << 16) | ((int)array3[(int)((UIntPtr)(num7 + 1U))] << 8) | (int)array3[(int)((UIntPtr)num7)]);
					uint num9 = 255U;
					int num10 = 0;
					uint num11;
					if (j == num2 - 1 && num > 0)
					{
						num4 += num8;
						num11 = 0U;
						for (int k = 0; k < num; k++)
						{
							if (k > 0)
							{
								num11 <<= 8;
							}
							num11 |= (uint)array6[array6.Length - (1 + k)];
						}
					}
					else
					{
						num7 = (uint)num6;
						num4 += num8;
						num11 = (uint)(((int)array6[(int)((UIntPtr)(num7 + 3U))] << 24) | ((int)array6[(int)((UIntPtr)(num7 + 2U))] << 16) | ((int)array6[(int)((UIntPtr)(num7 + 1U))] << 8) | (int)array6[(int)((UIntPtr)num7)]);
					}
					num4 = num4;
					uint num12 = num4;
					uint num13 = num4;
					num13 ^= num13 >> 25;
					num13 += 1394801171U;
					num13 ^= num13 << 5;
					num13 += 3457341307U;
					num13 ^= num13 >> 3;
					num13 += 4040842541U;
					num13 = 2018246656U + num13;
					num4 = num12 + (uint)num13;
					if (j == num2 - 1 && num > 0)
					{
						uint num14 = num4 ^ num11;
						for (int l = 0; l < num; l++)
						{
							if (l > 0)
							{
								num9 <<= 8;
								num10 += 8;
							}
							array7[num6 + l] = (byte)((num14 & num9) >> num10);
						}
					}
					else
					{
						uint num15 = num4 ^ num11;
						array7[num6] = (byte)(num15 & 255U);
						array7[num6 + 1] = (byte)((num15 & 65280U) >> 8);
						array7[num6 + 2] = (byte)((num15 & 16711680U) >> 16);
						array7[num6 + 3] = (byte)((num15 & 4278190080U) >> 24);
					}
				}
				byte[] array8 = array7;
				Array.Clear(array5, 0, array5.Length);
				binaryReader.Close();
				binaryReader = new BinaryReader(new MemoryStream(array8));
				binaryReader.BaseStream.Position = 0L;
				IntPtr intPtr = IntPtr.Zero;
				Assembly assembly = typeof(CollectionAttr).Assembly;
				intPtr = CollectionAttr.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				CollectionAttr.m_CustomerAttr = Marshal.GetHINSTANCE(assembly.GetModules()[0]).ToInt64();
				IntPtr zero = IntPtr.Zero;
				int num16 = 0;
				int num17 = binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				for (int m = 0; m < num17; m++)
				{
					IntPtr intPtr2 = new IntPtr(CollectionAttr.m_CustomerAttr + (long)binaryReader.ReadInt32());
					CollectionAttr.VirtualProtect(intPtr2, 4, 4, ref num16);
					if (IntPtr.Size == 4)
					{
						CollectionAttr.WriteProcessMemory(intPtr, intPtr2, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					else
					{
						CollectionAttr.WriteProcessMemory(intPtr, intPtr2, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					CollectionAttr.VirtualProtect(intPtr2, 4, num16, ref num16);
				}
				try
				{
					while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1L)
					{
						int num18 = binaryReader.ReadInt32();
						IntPtr intPtr3 = new IntPtr(CollectionAttr.m_CustomerAttr + (long)num18);
						int num19 = binaryReader.ReadInt32();
						CollectionAttr.VirtualProtect(intPtr3, num19 * 4, 4, ref num16);
						for (int n = 0; n < num19; n++)
						{
							Marshal.WriteInt32(new IntPtr(intPtr3.ToInt64() + (long)(n * 4)), binaryReader.ReadInt32());
						}
						CollectionAttr.VirtualProtect(intPtr3, num19 * 4, num16, ref num16);
					}
					CollectionAttr.CloseHandle(intPtr);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000DFEC File Offset: 0x0000C1EC
		/// <summary>从已加载程序集里解析出 Reactor 需要的宿主对象（本工程内无调用点）。</summary>
		internal static object ResolveAttr(Assembly assembly_0)
		{
			try
			{
				if (File.Exists(((Assembly)assembly_0).Location))
				{
					return ((Assembly)assembly_0).Location;
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(((Assembly)assembly_0).GetName().CodeBase.ToString().Replace("file:///", "")))
				{
					return ((Assembly)assembly_0).GetName().CodeBase.ToString().Replace("file:///", "");
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(assembly_0.GetType().GetProperty("Location").GetValue(assembly_0, new object[0])
					.ToString()))
				{
					return assembly_0.GetType().GetProperty("Location").GetValue(assembly_0, new object[0])
						.ToString();
				}
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x0600010A RID: 266
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x0600010B RID: 267
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x0600010C RID: 268
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint uint_0, int int_0, uint uint_1);

		// Token: 0x0600010D RID: 269
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr intptr_0);

		// Token: 0x0600010E RID: 270 RVA: 0x0000E0FC File Offset: 0x0000C2FC
		/// <summary>把字符串按 Reactor 自己的编码规则还原成字节（<paramref name="string_0"/> 一般是 Base64 或十六进制串）。</summary>
		private static byte[] DefineAttr(string string_0)
		{
			byte[] array;
			using (FileStream fileStream = new FileStream(string_0, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				int num = 0;
				long length = fileStream.Length;
				int i = (int)length;
				array = new byte[i];
				while (i > 0)
				{
					int num2 = fileStream.Read(array, num, i);
					num += num2;
					i -= num2;
				}
			}
			return array;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000E164 File Offset: 0x0000C364
		/// <summary>对字节数组做一次整体变换（Reactor 的资源解码链的一环）。</summary>
		private static byte[] UpdateAttr(byte[] byte_0)
		{
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = CollectionAttr.新建对称算法();
			symmetricAlgorithm.Key = new byte[]
			{
				34, 194, 192, 73, 86, 105, 183, 83, 68, 36,
				144, 203, 120, 63, 25, 150, 69, 113, 205, 194,
				224, 119, 79, 93, 16, 201, 30, 80, 67, 83,
				187, 130
			};
			symmetricAlgorithm.IV = new byte[]
			{
				158, 251, 202, 131, 70, 130, 16, 115, 129, 178,
				98, 226, 28, 90, 145, 209
			};
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(byte_0, 0, byte_0.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000E1D4 File Offset: 0x0000C3D4
		// 以下 12 个 <c>byte[] XxxAttr()</c> 都是 Reactor 留下的常量/资源分片读取器，
		// 名字已被打乱且互不调用，本工程内没有任何调用点；方法体保持原样未改。
		private byte[] CollectAttr()
		{
			return null;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000E1D4 File Offset: 0x0000C3D4
		private byte[] SelectAttr()
		{
			return null;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000E1E4 File Offset: 0x0000C3E4
		private byte[] CallAttr()
		{
			return null;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000E1E4 File Offset: 0x0000C3E4
		private byte[] VisitAttr()
		{
			return null;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000E1D4 File Offset: 0x0000C3D4
		private byte[] ForgotAttr()
		{
			return null;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000E1D4 File Offset: 0x0000C3D4
		private byte[] ResetAttr()
		{
			return null;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000E1F4 File Offset: 0x0000C3F4
		internal byte[] FlushAttr()
		{
			string text = "{11111-22222-40001-00001}";
			if (text.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000E234 File Offset: 0x0000C434
		internal byte[] InsertAttr()
		{
			string text = "{11111-22222-40001-00002}";
			if (text.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000E274 File Offset: 0x0000C474
		internal byte[] DeleteAttr()
		{
			string text = "{11111-22222-50001-00001}";
			if (text.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000E2B4 File Offset: 0x0000C4B4
		internal byte[] RegisterAttr()
		{
			string text = "{11111-22222-50001-00002}";
			if (text.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000E2F4 File Offset: 0x0000C4F4
		internal byte[] CheckAttr()
		{
			string text = "{11111-22222-60001-00001}";
			if (text.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000E334 File Offset: 0x0000C534
		internal byte[] SearchAttr()
		{
			string text = "{11111-22222-60001-00002}";
			if (text.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000E374 File Offset: 0x0000C574
		/// <summary>
		/// 读/写一个「键值」配置：用 <see cref="计算摘要"/> 把参数算成资源名，再从内嵌资源里解密取值。
		/// （<paramref name="string_0"/> 选段、<paramref name="string_1"/> 是键名。）
		/// </summary>
		internal static string QueryAttr(string string_0, string string_1)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(string_0);
			byte[] array = bytes;
			byte[] array2 = new byte[]
			{
				82, 102, 104, 110, 32, 77, 24, 34, 118, 181,
				51, 17, 18, 51, 12, 109, 10, 32, 77, 24,
				34, 158, 161, 41, 97, 28, 118, 181, 5, 25,
				1, 88
			};
			byte[] array3 = CollectionAttr.计算摘要(Encoding.Unicode.GetBytes(string_1));
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = CollectionAttr.新建对称算法();
			symmetricAlgorithm.Key = array2;
			symmetricAlgorithm.IV = array3;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.Close();
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000021E5 File Offset: 0x000003E5
		internal static bool PublishFactory()
		{
			return true;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000021E8 File Offset: 0x000003E8
		internal static bool ViewFactory()
		{
			return false;
		}

		// Token: 0x04000075 RID: 117
		private static bool 已读取FIPS开关 = false;

		// Token: 0x04000076 RID: 118
		private static bool 仅FIPS算法 = false;

		// Token: 0x04000077 RID: 119
		private static byte[] filterAttr = new byte[0];

		// Token: 0x04000078 RID: 120
		private static int _WrapperAttr = 0;

		// Token: 0x04000079 RID: 121
		private static int invocationAttr = 1;

		// Token: 0x0400007A RID: 122
		private static IntPtr _SingletonAttr = IntPtr.Zero;

		// Token: 0x0400007B RID: 123
		private static int[] callbackAttr = new int[0];

		// Token: 0x0400007C RID: 124
		private static IntPtr _InitializerAttr = IntPtr.Zero;

		// Token: 0x0400007D RID: 125
		private static string[] m_MerchantAttr = new string[0];

		// Token: 0x0400007E RID: 126
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

		// Token: 0x0400007F RID: 127
		private static bool proccesorAttr = false;

		// Token: 0x04000080 RID: 128
		private static byte[] workerAttr = new byte[0];

		// Token: 0x04000081 RID: 129
		private static byte[] m_ExceptionAttr = new byte[0];

		// Token: 0x04000082 RID: 130
		private static byte[] _BroadcasterAttr = new byte[0];

		// Token: 0x04000083 RID: 131
		private static long m_CustomerAttr = 0L;

		// Token: 0x02000014 RID: 20
		internal class PublisherAttr : Attribute
		{
			// Token: 0x06000120 RID: 288 RVA: 0x000028EE File Offset: 0x00000AEE
			[CollectionAttr.PublisherAttr(typeof(CollectionAttr.PublisherAttr.ListAttr<object>[]))]
			public PublisherAttr(object object_0)
			{
			}

			// Token: 0x02000015 RID: 21
			internal class ListAttr<T>
			{
				// Token: 0x06000122 RID: 290 RVA: 0x000021E5 File Offset: 0x000003E5
				internal static bool PopFactory()
				{
					return true;
				}

				// Token: 0x06000123 RID: 291 RVA: 0x000021E8 File Offset: 0x000003E8
				internal static bool ResolveFactory()
				{
					return false;
				}
			}
		}

		// Token: 0x02000016 RID: 22
		[Flags]
		private enum Bitness
		{

		}
	}
}
