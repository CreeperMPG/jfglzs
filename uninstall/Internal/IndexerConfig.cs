using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000012 RID: 18
	/// <summary>
	/// .NET Reactor 的运行时辅助类（加壳后保护器把自己的运行时塞进了这个类）。
	/// 它承担四件事，全部与「保护」有关，与机房管理助手本身的业务无关：
	/// 1) 自实现 MD5（自实现MD5 / MD5轮函数F..I / 循环左移），在系统禁止 FIPS 之外的算法时顶替
	///    MD5CryptoServiceProvider，用来计算内嵌资源名和 AES 的 IV；
	/// 2) 字符串解密（解密字符串）：从内嵌资源里解出加密的字符串表，再按偏移逐条 AES 解密；
	/// 3) 方法体还原（还原方法体）：把加密的方法体解密后直接写回本进程内存——即自修改代码；
	/// 4) AES 加解密工具（AES解密 / AES加密）与一堆空实现/占位方法。
	///
	/// 结论：看到本类的调用，基本等于看到「保护器在自检/解密」，不是程序功能。
	/// 原混淆名：IndexerConfig
	/// </summary>
	internal class IndexerConfig
	{
		// Token: 0x0600006B RID: 107 RVA: 0x00002422 File Offset: 0x00000622
		/// <summary>空方法（方法体只有 ret）。保护器留下的占位。原混淆名：method_0</summary>
		private void 空方法()
		{
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004CE4 File Offset: 0x00002EE4
		/// <summary>
		/// 自实现 MD5（标准 RFC 1321，无任何改动）：
		/// 先按 512 位分块补位（补 0x80、补零、末尾 8 字节写比特长度的小端值），
		/// 再用四个状态字 A/B/C/D（初值 0x67452301 / 0xefcdab89 / 0x98badcfe / 0x10325476）
		/// 跑 4 轮 × 16 步，每轮用对应的轮函数 F/G/H/I 和 MD5常量表[64]，
		/// 最后把 A/B/C/D 以小端顺序拼成 16 字节摘要返回。
		///
		/// 保护器为什么要自己写一份 MD5：系统若只允许 FIPS 认证算法则标准 MD5 实现会拒绝工作，
		/// 自实现版本不受此限，保证资源名/IV 永远算得出来（见 计算哈希）。
		///
		/// 原混淆名：DefineReader（参数在 IL 里声明为 object，实际只接受 byte[]）
		/// </summary>
		internal static byte[] 自实现MD5(byte[] 待哈希数据)
		{
			uint[] 消息字 = new uint[16];
			int 补位基准 = 448 - 待哈希数据.Length * 8 % 512;
			uint 补位位数 = (uint)((补位基准 + 512) % 512);
			if (补位位数 == 0U)
			{
				补位位数 = 512U;
			}
			uint 总长度 = (uint)((long)待哈希数据.Length + (long)((ulong)(补位位数 / 8U)) + 8L);
			ulong 比特长度 = (ulong)((long)待哈希数据.Length * 8L);
			byte[] 填充缓冲 = new byte[总长度];
			for (int i = 0; i < 待哈希数据.Length; i++)
			{
				填充缓冲[i] = 待哈希数据[i];
			}
			byte[] 填充缓冲别名 = 填充缓冲;
			int 末字节位置 = 待哈希数据.Length;
			填充缓冲别名[末字节位置] |= 128;
			for (int j = 8; j > 0; j--)
			{
				填充缓冲[(int)(checked((IntPtr)(unchecked((ulong)总长度 - (ulong)((long)j)))))] = (byte)((比特长度 >> (8 - j) * 8) & 255UL);
			}
			uint 三十二位字数 = (uint)(填充缓冲.Length * 8 / 32);
			uint A = 1732584193U;
			uint B = 4023233417U;
			uint C = 2562383102U;
			uint D = 271733878U;
			for (uint 块号 = 0U; 块号 < 三十二位字数 / 16U; 块号 += 1U)
			{
				uint 块偏移 = 块号 << 6;
				for (uint 字偏移 = 0U; 字偏移 < 61U; 字偏移 += 4U)
				{
					消息字[(int)((UIntPtr)(字偏移 >> 2))] = (uint)(((int)填充缓冲[(int)((UIntPtr)(块偏移 + (字偏移 + 3U)))] << 24) | ((int)填充缓冲[(int)((UIntPtr)(块偏移 + (字偏移 + 2U)))] << 16) | ((int)填充缓冲[(int)((UIntPtr)(块偏移 + (字偏移 + 1U)))] << 8) | (int)填充缓冲[(int)((UIntPtr)(块偏移 + 字偏移))]);
				}
				uint 旧A = A;
				uint 旧B = B;
				uint 旧C = C;
				uint 旧D = D;
				// 第 1 轮：F(b,c,d) = (b &amp; c) | (~b &amp; d)，循环移位 7/12/17/22，消息字索引依次 0..15
				MD5轮函数F(ref A, B, C, D, 0U, 7, 1U, 消息字);
				MD5轮函数F(ref D, A, B, C, 1U, 12, 2U, 消息字);
				MD5轮函数F(ref C, D, A, B, 2U, 17, 3U, 消息字);
				MD5轮函数F(ref B, C, D, A, 3U, 22, 4U, 消息字);
				MD5轮函数F(ref A, B, C, D, 4U, 7, 5U, 消息字);
				MD5轮函数F(ref D, A, B, C, 5U, 12, 6U, 消息字);
				MD5轮函数F(ref C, D, A, B, 6U, 17, 7U, 消息字);
				MD5轮函数F(ref B, C, D, A, 7U, 22, 8U, 消息字);
				MD5轮函数F(ref A, B, C, D, 8U, 7, 9U, 消息字);
				MD5轮函数F(ref D, A, B, C, 9U, 12, 10U, 消息字);
				MD5轮函数F(ref C, D, A, B, 10U, 17, 11U, 消息字);
				MD5轮函数F(ref B, C, D, A, 11U, 22, 12U, 消息字);
				MD5轮函数F(ref A, B, C, D, 12U, 7, 13U, 消息字);
				MD5轮函数F(ref D, A, B, C, 13U, 12, 14U, 消息字);
				MD5轮函数F(ref C, D, A, B, 14U, 17, 15U, 消息字);
				MD5轮函数F(ref B, C, D, A, 15U, 22, 16U, 消息字);
				// 第 2 轮：G(b,c,d) = (b &amp; d) | (c &amp; ~d)，循环移位 5/9/14/20，消息字索引依次 1,6,11,0,5,10,15,4,9,14,3,8,13,2,7,12
				MD5轮函数G(ref A, B, C, D, 1U, 5, 17U, 消息字);
				MD5轮函数G(ref D, A, B, C, 6U, 9, 18U, 消息字);
				MD5轮函数G(ref C, D, A, B, 11U, 14, 19U, 消息字);
				MD5轮函数G(ref B, C, D, A, 0U, 20, 20U, 消息字);
				MD5轮函数G(ref A, B, C, D, 5U, 5, 21U, 消息字);
				MD5轮函数G(ref D, A, B, C, 10U, 9, 22U, 消息字);
				MD5轮函数G(ref C, D, A, B, 15U, 14, 23U, 消息字);
				MD5轮函数G(ref B, C, D, A, 4U, 20, 24U, 消息字);
				MD5轮函数G(ref A, B, C, D, 9U, 5, 25U, 消息字);
				MD5轮函数G(ref D, A, B, C, 14U, 9, 26U, 消息字);
				MD5轮函数G(ref C, D, A, B, 3U, 14, 27U, 消息字);
				MD5轮函数G(ref B, C, D, A, 8U, 20, 28U, 消息字);
				MD5轮函数G(ref A, B, C, D, 13U, 5, 29U, 消息字);
				MD5轮函数G(ref D, A, B, C, 2U, 9, 30U, 消息字);
				MD5轮函数G(ref C, D, A, B, 7U, 14, 31U, 消息字);
				MD5轮函数G(ref B, C, D, A, 12U, 20, 32U, 消息字);
				// 第 3 轮：H(b,c,d) = b ^ c ^ d，循环移位 4/11/16/23，消息字索引依次 5,8,11,14,1,4,7,10,13,0,3,6,9,12,15,2
				MD5轮函数H(ref A, B, C, D, 5U, 4, 33U, 消息字);
				MD5轮函数H(ref D, A, B, C, 8U, 11, 34U, 消息字);
				MD5轮函数H(ref C, D, A, B, 11U, 16, 35U, 消息字);
				MD5轮函数H(ref B, C, D, A, 14U, 23, 36U, 消息字);
				MD5轮函数H(ref A, B, C, D, 1U, 4, 37U, 消息字);
				MD5轮函数H(ref D, A, B, C, 4U, 11, 38U, 消息字);
				MD5轮函数H(ref C, D, A, B, 7U, 16, 39U, 消息字);
				MD5轮函数H(ref B, C, D, A, 10U, 23, 40U, 消息字);
				MD5轮函数H(ref A, B, C, D, 13U, 4, 41U, 消息字);
				MD5轮函数H(ref D, A, B, C, 0U, 11, 42U, 消息字);
				MD5轮函数H(ref C, D, A, B, 3U, 16, 43U, 消息字);
				MD5轮函数H(ref B, C, D, A, 6U, 23, 44U, 消息字);
				MD5轮函数H(ref A, B, C, D, 9U, 4, 45U, 消息字);
				MD5轮函数H(ref D, A, B, C, 12U, 11, 46U, 消息字);
				MD5轮函数H(ref C, D, A, B, 15U, 16, 47U, 消息字);
				MD5轮函数H(ref B, C, D, A, 2U, 23, 48U, 消息字);
				// 第 4 轮：I(b,c,d) = c ^ (b | ~d)，循环移位 6/10/15/21，消息字索引依次 0,7,14,5,12,3,10,1,8,15,6,13,4,11,2,9
				MD5轮函数I(ref A, B, C, D, 0U, 6, 49U, 消息字);
				MD5轮函数I(ref D, A, B, C, 7U, 10, 50U, 消息字);
				MD5轮函数I(ref C, D, A, B, 14U, 15, 51U, 消息字);
				MD5轮函数I(ref B, C, D, A, 5U, 21, 52U, 消息字);
				MD5轮函数I(ref A, B, C, D, 12U, 6, 53U, 消息字);
				MD5轮函数I(ref D, A, B, C, 3U, 10, 54U, 消息字);
				MD5轮函数I(ref C, D, A, B, 10U, 15, 55U, 消息字);
				MD5轮函数I(ref B, C, D, A, 1U, 21, 56U, 消息字);
				MD5轮函数I(ref A, B, C, D, 8U, 6, 57U, 消息字);
				MD5轮函数I(ref D, A, B, C, 15U, 10, 58U, 消息字);
				MD5轮函数I(ref C, D, A, B, 6U, 15, 59U, 消息字);
				MD5轮函数I(ref B, C, D, A, 13U, 21, 60U, 消息字);
				MD5轮函数I(ref A, B, C, D, 4U, 6, 61U, 消息字);
				MD5轮函数I(ref D, A, B, C, 11U, 10, 62U, 消息字);
				MD5轮函数I(ref C, D, A, B, 2U, 15, 63U, 消息字);
				MD5轮函数I(ref B, C, D, A, 9U, 21, 64U, 消息字);
				A += 旧A;
				B += 旧B;
				C += 旧C;
				D += 旧D;
			}
			byte[] 摘要 = new byte[16];
			Array.Copy(BitConverter.GetBytes(A), 0, 摘要, 0, 4);
			Array.Copy(BitConverter.GetBytes(B), 0, 摘要, 4, 4);
			Array.Copy(BitConverter.GetBytes(C), 0, 摘要, 8, 4);
			Array.Copy(BitConverter.GetBytes(D), 0, 摘要, 12, 4);
			return 摘要;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002424 File Offset: 0x00000624
		/// <summary>
		/// MD5 第 1 轮轮函数 F：
		/// 目标字 = b + 循环左移(目标字 + ((b &amp; c) | (~b &amp; d)) + 消息字[索引] + MD5常量表[常量索引-1], 循环位数)。
		/// 四个形参在调用点会轮换（第二个参数不一定是 b），按调用顺序理解即可。
		/// 原混淆名：RegisterReader
		/// </summary>
		private static void MD5轮函数F(ref uint 目标字, uint b, uint c, uint d, uint 消息字索引, ushort 循环位数, uint 常量索引, uint[] 消息字表)
		{
			目标字 = b + IndexerConfig.循环左移(目标字 + ((b & c) | (~b & d)) + 消息字表[(int)((UIntPtr)消息字索引)] + IndexerConfig.MD5常量表[(int)((UIntPtr)(常量索引 - 1U))], 循环位数);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000244F File Offset: 0x0000064F
		/// <summary>
		/// MD5 第 2 轮轮函数 G：目标字 = b + 循环左移(目标字 + ((b &amp; d) | (c &amp; ~d)) + 消息字[索引] + 常量, 位数)。
		/// 原混淆名：AwakeReader
		/// </summary>
		private static void MD5轮函数G(ref uint 目标字, uint b, uint c, uint d, uint 消息字索引, ushort 循环位数, uint 常量索引, uint[] 消息字表)
		{
			目标字 = b + IndexerConfig.循环左移(目标字 + ((b & d) | (c & ~d)) + 消息字表[(int)((UIntPtr)消息字索引)] + IndexerConfig.MD5常量表[(int)((UIntPtr)(常量索引 - 1U))], 循环位数);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000247A File Offset: 0x0000067A
		/// <summary>
		/// MD5 第 3 轮轮函数 H：目标字 = b + 循环左移(目标字 + (b ^ c ^ d) + 消息字[索引] + 常量, 位数)。
		/// 原混淆名：MoveReader
		/// </summary>
		private static void MD5轮函数H(ref uint 目标字, uint b, uint c, uint d, uint 消息字索引, ushort 循环位数, uint 常量索引, uint[] 消息字表)
		{
			目标字 = b + IndexerConfig.循环左移(目标字 + (b ^ c ^ d) + 消息字表[(int)((UIntPtr)消息字索引)] + IndexerConfig.MD5常量表[(int)((UIntPtr)(常量索引 - 1U))], 循环位数);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000024A2 File Offset: 0x000006A2
		/// <summary>
		/// MD5 第 4 轮轮函数 I：目标字 = b + 循环左移(目标字 + (c ^ (b | ~d)) + 消息字[索引] + 常量, 位数)。
		/// 原混淆名：RunReader
		/// </summary>
		private static void MD5轮函数I(ref uint 目标字, uint b, uint c, uint d, uint 消息字索引, ushort 循环位数, uint 常量索引, uint[] 消息字表)
		{
			目标字 = b + IndexerConfig.循环左移(目标字 + (c ^ (b | ~d)) + 消息字表[(int)((UIntPtr)消息字索引)] + IndexerConfig.MD5常量表[(int)((UIntPtr)(常量索引 - 1U))], 循环位数);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000024CB File Offset: 0x000006CB
		/// <summary>
		/// 32 位循环左移：把低位移到高位补齐，等价于 (v &lt;&lt; n) | (v &gt;&gt; (32 - n))。
		/// 原混淆名：GetReader
		/// </summary>
		private static uint 循环左移(uint 值, ushort 位数)
		{
			return (值 >> (int)(32 - 位数)) | (值 << (int)位数);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000024DD File Offset: 0x000006DD
		/// <summary>
		/// 系统是否「只允许 FIPS 认证算法」。首次调用时反射探测一次并缓存（探测失败则保持 false）。
		/// 原混淆名：SetupReader
		/// </summary>
		internal static bool 是否允许FIPS算法()
		{
			if (!IndexerConfig.已探测FIPS)
			{
				IndexerConfig.探测FIPS开关();
				IndexerConfig.已探测FIPS = true;
			}
			return IndexerConfig.允许FIPS算法;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00005388 File Offset: 0x00003588
		/// <summary>
		/// 反射读 System.Security.Cryptography.CryptoConfig.AllowOnlyFipsAlgorithms 的值。
		/// 用反射而不是直接引用，是因为该类在 .NET 2.0/CF 等环境下可能不存在；读失败就吞掉异常（保持 false）。
		/// 原混淆名：DestroyReader
		/// </summary>
		internal static void 探测FIPS开关()
		{
			try
			{
				IndexerConfig.允许FIPS算法 = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000053E8 File Offset: 0x000035E8
		/// <summary>
		/// 造一个对称算法实例：FIPS 模式下优先 AesCryptoServiceProvider（FIPS 认证实现），
		/// 拿不到就退回 RijndaelManaged；非 FIPS 模式直接用 RijndaelManaged。
		/// 原混淆名：QueryReader
		/// </summary>
		internal static SymmetricAlgorithm 创建AES实现()
		{
			SymmetricAlgorithm 结果 = null;
			if (IndexerConfig.是否允许FIPS算法())
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
			结果 = new RijndaelManaged();
			return 结果;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000024F6 File Offset: 0x000006F6
		/// <summary>
		/// 统一的「哈希」入口：非 FIPS 模式用系统 MD5CryptoServiceProvider（快），
		/// FIPS 模式改用自实现MD5（标准实现会被系统拒绝）。
		/// 调用点既拿它算内嵌资源名，也拿它算 AES 的 IV，所以两边必须得到同一个结果。
		/// 原混淆名：AssetReader
		/// </summary>
		internal static byte[] 计算哈希(byte[] 待哈希数据)
		{
			if (!IndexerConfig.是否允许FIPS算法())
			{
				return new MD5CryptoServiceProvider().ComputeHash(待哈希数据);
			}
			return IndexerConfig.自实现MD5(待哈希数据);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00005428 File Offset: 0x00003628
		/// <summary>
		/// .NET Reactor 的字符串解密入口（源码里被加密的字符串常量最终都会变成对它的一次调用）。
		/// 首次调用时把整张字符串表解开并缓存，之后按偏移直接取。
		///
		/// 首次解密流程：
		/// 1) 读内嵌资源 "21413984-2025-4873-8c6f-e4b197ae8963"（加密的字符串表）；
		/// 2) 用下面的 array2（32 字节密钥）/ array4（16 字节 IV），必要时再叠加程序集公钥标记，XOR 出真 Key/IV；
		/// 3) 对资源做 AES-CBC 解密，得到「偏移表」；随后又对偏移表做一遍 XOR + 变种 ARX 流密码解密；
		/// 4) 每条字符串：从偏移表取 RVA，用 ReadProcessMemory 从本模块内存镜像里读 4 字节长度 + 密文，
		///    再 AES 解密成 UTF-16 字符串；
		/// 5) 结果按偏移缓存到 字符串缓存 / 字符串缓存索引表，避免重复解密。
		///
		/// 特征：密文是从「自己的进程内存镜像」里读的，不是从文件读——说明密文就地布置在模块里，
		/// 也使静态 dump 更难直接拿到明文。
		/// 原混淆名：RestartReader
		/// </summary>
		static string 解密字符串(int 字符串偏移)
		{
			if (IndexerConfig.解密后数据.Length == 0)
			{
				BinaryReader 资源读取器 = new BinaryReader(typeof(IndexerConfig).Assembly.GetManifestResourceStream("21413984-2025-4873-8c6f-e4b197ae8963"));
				资源读取器.BaseStream.Position = 0L;
				byte[] 资源字节 = 资源读取器.ReadBytes((int)资源读取器.BaseStream.Length);
				// array2 是 AES-256 密钥的原始字节。混淆器对同一个下标连续写入多次（垃圾常量写入），
				// 所以下面每一段 array2[N] = ... 只有最后一次赋值真正生效。
				byte[] array2 = new byte[32];
				array2[0] = 177;
				array2[0] = 136;
				array2[0] = 80;
				array2[0] = 127;
				array2[0] = 225;
				array2[0] = 54;
				array2[1] = 28;
				array2[1] = 112;
				array2[1] = 39;
				array2[1] = 141;
				array2[1] = 196;
				array2[1] = 42;
				array2[2] = 189;
				array2[2] = 105;
				array2[2] = 138;
				array2[3] = 168;
				array2[3] = 112;
				array2[3] = 214;
				array2[4] = 36;
				array2[4] = 211;
				array2[4] = 81;
				array2[4] = 88;
				array2[4] = 197;
				array2[5] = 111;
				array2[5] = 88;
				array2[5] = 77;
				array2[5] = 133;
				array2[5] = 130;
				array2[5] = 175;
				array2[6] = 172;
				array2[6] = 193;
				array2[6] = 126;
				array2[6] = 206;
				array2[7] = 203;
				array2[7] = 167;
				array2[7] = 196;
				array2[8] = 162;
				array2[8] = 148;
				array2[8] = 154;
				array2[8] = 198;
				array2[8] = 169;
				array2[8] = 33;
				array2[9] = 89;
				array2[9] = 182;
				array2[9] = 97;
				array2[9] = 106;
				array2[9] = 15;
				array2[10] = 209;
				array2[10] = 98;
				array2[10] = 97;
				array2[10] = 107;
				array2[10] = 166;
				array2[10] = 101;
				array2[11] = 134;
				array2[11] = 95;
				array2[11] = 132;
				array2[11] = 223;
				array2[12] = 95;
				array2[12] = 100;
				array2[12] = 131;
				array2[13] = 175;
				array2[13] = 225;
				array2[13] = 140;
				array2[13] = 106;
				array2[13] = 133;
				array2[14] = 92;
				array2[14] = 74;
				array2[14] = 27;
				array2[15] = 175;
				array2[15] = 209;
				array2[15] = 89;
				array2[15] = 98;
				array2[15] = 254;
				array2[16] = 155;
				array2[16] = 113;
				array2[16] = 116;
				array2[16] = 96;
				array2[16] = 172;
				array2[16] = 225;
				array2[17] = 175;
				array2[17] = 134;
				array2[17] = 30;
				array2[18] = 156;
				array2[18] = 127;
				array2[18] = 161;
				array2[18] = 143;
				array2[18] = 240;
				array2[19] = 122;
				array2[19] = 112;
				array2[19] = 148;
				array2[19] = 138;
				array2[19] = 80;
				array2[20] = 130;
				array2[20] = 39;
				array2[20] = 134;
				array2[20] = 66;
				array2[20] = 187;
				array2[21] = 162;
				array2[21] = 146;
				array2[21] = 135;
				array2[22] = 223;
				array2[22] = 180;
				array2[22] = 24;
				array2[22] = 110;
				array2[22] = 196;
				array2[23] = 118;
				array2[23] = 124;
				array2[23] = 136;
				array2[23] = 99;
				array2[23] = 134;
				array2[23] = 121;
				array2[24] = 100;
				array2[24] = 158;
				array2[24] = 17;
				array2[25] = 113;
				array2[25] = 38;
				array2[25] = 222;
				array2[25] = 153;
				array2[25] = 101;
				array2[26] = 60;
				array2[26] = 121;
				array2[26] = 187;
				array2[27] = 120;
				array2[27] = 143;
				array2[27] = 98;
				array2[27] = 101;
				array2[27] = 84;
				array2[27] = 181;
				array2[28] = 170;
				array2[28] = 161;
				array2[28] = 88;
				array2[28] = 94;
				array2[29] = 108;
				array2[29] = 223;
				array2[29] = 133;
				array2[29] = 182;
				array2[30] = 197;
				array2[30] = 175;
				array2[30] = 164;
				array2[30] = 160;
				array2[30] = 206;
				array2[30] = 108;
				array2[31] = 198;
				array2[31] = 116;
				array2[31] = 65;
				// array2 的生效值（每下标最后一次赋值），即 AES-256 密钥：
				//   54, 42, 138, 214, 197, 175, 206, 196, 33, 15, 101, 223, 131, 133, 27, 254,
				//   225, 30, 240, 80, 187, 135, 196, 121, 17, 101, 187, 181, 94, 182, 108, 65
				byte[] AES密钥 = array2;
				// array4 是 16 字节 IV 的原始字节，同样每个下标被反复写入；生效值：
				//   185, 172, 25, 163, 0, 15, 90, 109, 199, 31, 118, 120, 140, 167, 161, 14
				byte[] array4 = new byte[16];
				array4[0] = 122;
				array4[0] = 158;
				array4[0] = 185;
				array4[1] = 80;
				array4[1] = 105;
				array4[1] = 102;
				array4[1] = 159;
				array4[1] = 164;
				array4[1] = 172;
				array4[2] = 112;
				array4[2] = 39;
				array4[2] = 155;
				array4[2] = 166;
				array4[2] = 102;
				array4[2] = 25;
				array4[3] = 142;
				array4[3] = 90;
				array4[3] = 163;
				array4[4] = 160;
				array4[4] = 112;
				array4[4] = 185;
				array4[4] = 0;
				array4[5] = 211;
				array4[5] = 81;
				array4[5] = 88;
				array4[5] = 153;
				array4[5] = 15;
				array4[6] = 64;
				array4[6] = 77;
				array4[6] = 133;
				array4[6] = 130;
				array4[6] = 90;
				array4[7] = 172;
				array4[7] = 160;
				array4[7] = 146;
				array4[7] = 109;
				array4[8] = 71;
				array4[8] = 148;
				array4[8] = 158;
				array4[8] = 90;
				array4[8] = 173;
				array4[8] = 199;
				array4[9] = 119;
				array4[9] = 154;
				array4[9] = 31;
				array4[10] = 141;
				array4[10] = 87;
				array4[10] = 118;
				array4[11] = 131;
				array4[11] = 106;
				array4[11] = 198;
				array4[11] = 120;
				array4[12] = 184;
				array4[12] = 99;
				array4[12] = 166;
				array4[12] = 140;
				array4[13] = 134;
				array4[13] = 43;
				array4[13] = 130;
				array4[13] = 95;
				array4[13] = 100;
				array4[13] = 167;
				array4[14] = 143;
				array4[14] = 138;
				array4[14] = 139;
				array4[14] = 158;
				array4[14] = 161;
				array4[15] = 47;
				array4[15] = 74;
				array4[15] = 170;
				array4[15] = 136;
				array4[15] = 145;
				array4[15] = 14;
				byte[] AES初始向量 = array4;
				byte[] 公钥标记 = typeof(IndexerConfig).Assembly.GetName().GetPublicKeyToken();
				if (公钥标记 != null && 公钥标记.Length > 0)
				{
					// 强名称程序集：把公钥标记的 8 个字节隔位塞进 IV（跳过 8 个位置），
					// 这样同一个加壳器产出的不同程序集 IV 都不同。
					AES初始向量[1] = 公钥标记[0];
					AES初始向量[3] = 公钥标记[1];
					AES初始向量[5] = 公钥标记[2];
					AES初始向量[7] = 公钥标记[3];
					AES初始向量[9] = 公钥标记[4];
					AES初始向量[11] = 公钥标记[5];
					AES初始向量[13] = 公钥标记[6];
					AES初始向量[15] = 公钥标记[7];
				}
				for (int i = 0; i < AES初始向量.Length; i++)
				{
					AES密钥[i] ^= AES初始向量[i];
				}
				if (字符串偏移 == -1)
				{
					// 哨兵值 -1：表示「现在解密整张表」，结果写回 解密后数据 并建缓存数组。
					SymmetricAlgorithm AES算法 = IndexerConfig.创建AES实现();
					AES算法.Mode = CipherMode.CBC;
					ICryptoTransform 解密器 = AES算法.CreateDecryptor(AES密钥, AES初始向量);
					MemoryStream 内存流 = new MemoryStream();
					CryptoStream 解密流 = new CryptoStream(内存流, 解密器, CryptoStreamMode.Write);
					解密流.Write(资源字节, 0, 资源字节.Length);
					解密流.FlushFinalBlock();
					IndexerConfig.解密后数据 = 内存流.ToArray();
					if (IndexerConfig.解密后数据.Length > 0)
					{
						IndexerConfig.字符串缓存 = new string[IndexerConfig.解密后数据.Length / 4 + 1];
						IndexerConfig.字符串缓存索引表 = new int[IndexerConfig.解密后数据.Length / 4 + 1];
					}
					内存流.Close();
					解密流.Close();
					资源读取器.Close();
					资源字节 = IndexerConfig.解密后数据;
				}
				// 下面把「AES 解出来的数据」再解一层：按 4 字节一组做 XOR + 变种 ARX（加/异或/移位）流密码。
				int 余数字节 = 资源字节.Length % 4;
				int 块数 = 资源字节.Length / 4;
				byte[] 偏移表 = new byte[资源字节.Length];
				int 密钥字数 = AES密钥.Length / 4;
				uint 状态 = 0U;
				if (余数字节 > 0)
				{
					块数++;
				}
				for (int j = 0; j < 块数; j++)
				{
					int 密钥字序号 = j % 密钥字数;
					int 块内偏移 = j * 4;
					uint 临时偏移 = (uint)(密钥字序号 * 4);
					uint 密钥字 = (uint)(((int)AES密钥[(int)((UIntPtr)(临时偏移 + 3U))] << 24) | ((int)AES密钥[(int)((UIntPtr)(临时偏移 + 2U))] << 16) | ((int)AES密钥[(int)((UIntPtr)(临时偏移 + 1U))] << 8) | (int)AES密钥[(int)((UIntPtr)临时偏移)]);
					uint 掩码 = 255U;
					int 移位量 = 0;
					uint 明文块;
					if (j == 块数 - 1 && 余数字节 > 0)
					{
						明文块 = 0U;
						for (int k = 0; k < 余数字节; k++)
						{
							if (k > 0)
							{
								明文块 <<= 8;
							}
							明文块 |= (uint)资源字节[资源字节.Length - (1 + k)];
						}
						状态 += 密钥字;
					}
					else
					{
						临时偏移 = (uint)块内偏移;
						明文块 = (uint)(((int)资源字节[(int)((UIntPtr)(临时偏移 + 3U))] << 24) | ((int)资源字节[(int)((UIntPtr)(临时偏移 + 2U))] << 16) | ((int)资源字节[(int)((UIntPtr)(临时偏移 + 1U))] << 8) | (int)资源字节[(int)((UIntPtr)临时偏移)]);
						状态 += 密钥字;
					}
					uint 旧状态 = 状态;
					uint 混合值 = 状态;
					混合值 ^= 混合值 >> 25;
					混合值 += 1394801171U;
					混合值 ^= 混合值 << 5;
					混合值 += 3457341307U;
					混合值 ^= 混合值 >> 3;
					混合值 += 4040842541U;
					混合值 = 2018246656U + 混合值;
					状态 = 旧状态 + (uint)混合值;
					if (j == 块数 - 1 && 余数字节 > 0)
					{
						uint 异或结果 = 状态 ^ 明文块;
						for (int l = 0; l < 余数字节; l++)
						{
							if (l > 0)
							{
								掩码 <<= 8;
								移位量 += 8;
							}
							偏移表[块内偏移 + l] = (byte)((异或结果 & 掩码) >> 移位量);
						}
					}
					else
					{
						uint 异或结果 = 状态 ^ 明文块;
						偏移表[块内偏移] = (byte)(异或结果 & 255U);
						偏移表[块内偏移 + 1] = (byte)((异或结果 & 65280U) >> 8);
						偏移表[块内偏移 + 2] = (byte)((异或结果 & 16711680U) >> 16);
						偏移表[块内偏移 + 3] = (byte)((异或结果 & 4278190080U) >> 24);
					}
				}
				IndexerConfig.解密后数据 = 偏移表;
				if (IndexerConfig.解密后数据.Length > 0)
				{
					IndexerConfig.字符串缓存 = new string[IndexerConfig.解密后数据.Length / 4 + 1];
					IndexerConfig.字符串缓存索引表 = new int[IndexerConfig.解密后数据.Length / 4 + 1];
				}
				Assembly 本程序集 = typeof(IndexerConfig).Assembly;
				if (IntPtr.Size == 4)
				{
					IndexerConfig.模块基址32 = Marshal.GetHINSTANCE(本程序集.GetModules()[0]).ToInt32();
				}
				IndexerConfig.模块基址64 = Marshal.GetHINSTANCE(本程序集.GetModules()[0]).ToInt64();
			}
			// —— 以下每次调用都会走：从偏移表拿 RVA，再从本进程内存镜像里读密文并 AES 解密 ——
			int 缓存槽 = 字符串偏移 / 4;
			if (IndexerConfig.字符串缓存索引表[缓存槽] > 0)
			{
				return ((string[])IndexerConfig.字符串缓存)[IndexerConfig.字符串缓存索引表[缓存槽]];
			}
			int 字符串RVA = BitConverter.ToInt32(IndexerConfig.解密后数据, 字符串偏移);
			if (IndexerConfig.进程句柄 == IntPtr.Zero)
			{
				// OpenProcess(PROCESS_VM_READ=0x10, FALSE, 自己) —— 读自己的内存镜像
				IndexerConfig.进程句柄 = IndexerConfig.OpenProcess(16U, 1, (uint)Process.GetCurrentProcess().Id);
			}
			byte[] 读缓冲 = new byte[4];
			if (IntPtr.Size == 4)
			{
				IndexerConfig.ReadProcessMemory(IndexerConfig.进程句柄, new IntPtr(IndexerConfig.模块基址32 + 字符串RVA), 读缓冲, 4U, out IndexerConfig.读取字节数);
			}
			else
			{
				IndexerConfig.ReadProcessMemory(IndexerConfig.进程句柄, new IntPtr(IndexerConfig.模块基址64 + (long)字符串RVA), 读缓冲, 4U, out IndexerConfig.读取字节数);
			}
			int 密文长度 = BitConverter.ToInt32(读缓冲, 0);
			读缓冲 = new byte[密文长度];
			if (IntPtr.Size == 4)
			{
				IndexerConfig.ReadProcessMemory(IndexerConfig.进程句柄, new IntPtr(IndexerConfig.模块基址32 + 字符串RVA + 4), 读缓冲, Convert.ToUInt32(密文长度), out IndexerConfig.读取字节数);
			}
			else
			{
				IndexerConfig.ReadProcessMemory(IndexerConfig.进程句柄, new IntPtr(IndexerConfig.模块基址64 + (long)字符串RVA + 4L), 读缓冲, Convert.ToUInt32(密文长度), out IndexerConfig.读取字节数);
			}
			byte[] 明文 = IndexerConfig.AES解密(读缓冲);
			string 结果字符串 = Encoding.Unicode.GetString(明文, 0, 明文.Length);
			IndexerConfig.字符串缓存索引表[缓存槽] = IndexerConfig.字符串缓存游标;
			((string[])IndexerConfig.字符串缓存)[IndexerConfig.字符串缓存游标] = 结果字符串;
			IndexerConfig.字符串缓存游标++;
			return 结果字符串;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00006578 File Offset: 0x00004778
		/// <summary>
		/// 最简单的「字符串」解码：Base64 → UTF-16 字节 → 字符串。
		/// 有些明文字符串常量被保护器改成了 Base64，走到这里就还原。
		/// 原混淆名：CancelReader
		/// </summary>
		internal static string Base64解码字符串(string 待解码)
		{
			byte[] 字节 = Convert.FromBase64String(待解码);
			return Encoding.Unicode.GetString(字节, 0, 字节.Length);
		}

		// Token: 0x06000078 RID: 120
		/// <summary>
		/// kernel32!VirtualProtect：改内存页的保护属性（这里用它把代码页改成可写，改完再改回去）。
		/// 原混淆名：无（DllImport 的方法名没被改，只把参数名还原了）
		/// </summary>
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr 地址, int 长度, int 新保护属性, ref int 旧保护属性);

		// Token: 0x06000079 RID: 121 RVA: 0x0000659C File Offset: 0x0000479C
		/// <summary>
		/// .NET Reactor 的方法体还原（anti-tamper 的另一半）：把内嵌资源
		/// "47bd6f50-e78c-4bfb-9a1d-e06490ced281" 解密成一张「补丁表」，然后用
		/// VirtualProtect + WriteProcessMemory / Marshal.WriteInt32 直接把数据写回本进程的模块镜像。
		/// 也就是说：方法体在磁盘上是加密的，运行时由这段代码原地解密覆盖，静态反编译看不到真实实现。
		///
		/// 补丁表格式（解密后）：
		/// 1) 前 4 字节 = 补丁条数 N，随后 4 字节忽略；
		/// 2) 重复 N 次：{ 4 字节 RVA, 4 字节 值 }，把 RVA 处的 4 字节改写成给定值；
		/// 3) 之后每块：{ 4 字节 RVA, 4 字节 字数 }，后面跟「字数」个 4 字节值，连续写入。
		///
		/// 这些写操作改的是本进程的可执行内存——典型的自修改代码/内存补丁，杀软通常重点关注。
		/// 原混淆名：ManageReader
		/// </summary>
		[IndexerConfig.WatcherConfig(typeof(IndexerConfig.WatcherConfig.CreatorConfig<object>[]))]
		static void 还原方法体()
		{
			if (!IndexerConfig.已还原方法体)
			{
				IndexerConfig.已还原方法体 = true;
				BinaryReader 资源读取器 = new BinaryReader(typeof(IndexerConfig).Assembly.GetManifestResourceStream("47bd6f50-e78c-4bfb-9a1d-e06490ced281"));
				资源读取器.BaseStream.Position = 0L;
				byte[] 资源字节 = 资源读取器.ReadBytes((int)资源读取器.BaseStream.Length);
				// array2：AES-256 密钥原始字节（同样每个下标被反复写入，只有最后一次生效）
				byte[] array2 = new byte[32];
				array2[0] = 21;
				array2[0] = 114;
				array2[0] = 148;
				array2[0] = 12;
				array2[0] = 161;
				array2[0] = 21;
				array2[1] = 235;
				array2[1] = 86;
				array2[1] = 118;
				array2[1] = 235;
				array2[2] = 68;
				array2[2] = 51;
				array2[2] = 100;
				array2[2] = 153;
				array2[2] = 68;
				array2[3] = 119;
				array2[3] = 221;
				array2[3] = 100;
				array2[3] = 119;
				array2[4] = 61;
				array2[4] = 19;
				array2[4] = 108;
				array2[4] = 125;
				array2[4] = 110;
				array2[4] = 61;
				array2[5] = 41;
				array2[5] = 206;
				array2[5] = 131;
				array2[5] = 214;
				array2[5] = 41;
				array2[6] = 47;
				array2[6] = 53;
				array2[6] = 219;
				array2[6] = 83;
				array2[6] = 85;
				array2[6] = 144;
				array2[6] = 47;
				array2[7] = 14;
				array2[7] = 164;
				array2[7] = 139;
				array2[7] = 32;
				array2[7] = 145;
				array2[7] = 109;
				array2[7] = 14;
				array2[8] = 211;
				array2[8] = 194;
				array2[8] = 140;
				array2[8] = 90;
				array2[8] = 211;
				array2[9] = 150;
				array2[9] = 151;
				array2[9] = 144;
				array2[9] = 150;
				array2[10] = 154;
				array2[10] = 103;
				array2[10] = 105;
				array2[10] = 146;
				array2[10] = 169;
				array2[10] = 154;
				array2[11] = 31;
				array2[11] = 104;
				array2[11] = 66;
				array2[11] = 155;
				array2[11] = 94;
				array2[11] = 31;
				array2[12] = 231;
				array2[12] = 165;
				array2[12] = 234;
				array2[12] = 231;
				array2[13] = 112;
				array2[13] = 188;
				array2[13] = 162;
				array2[13] = 153;
				array2[13] = 135;
				array2[13] = 119;
				array2[13] = 112;
				array2[14] = 30;
				array2[14] = 110;
				array2[14] = 160;
				array2[14] = 30;
				array2[15] = 78;
				array2[15] = 69;
				array2[15] = 88;
				array2[15] = 78;
				array2[16] = 90;
				array2[16] = 140;
				array2[16] = 90;
				array2[16] = 91;
				array2[16] = 146;
				array2[16] = 56;
				array2[16] = 90;
				array2[17] = 80;
				array2[17] = 96;
				array2[17] = 124;
				array2[17] = 104;
				array2[17] = 104;
				array2[17] = 109;
				array2[17] = 80;
				array2[18] = 24;
				array2[18] = 140;
				array2[18] = 147;
				array2[18] = 102;
				array2[18] = 24;
				array2[19] = 105;
				array2[19] = 168;
				array2[19] = 108;
				array2[19] = 100;
				array2[19] = 105;
				array2[20] = 200;
				array2[20] = 88;
				array2[20] = 133;
				array2[20] = 200;
				array2[21] = 39;
				array2[21] = 213;
				array2[21] = 137;
				array2[21] = 24;
				array2[21] = 117;
				array2[21] = 132;
				array2[21] = 39;
				array2[22] = 82;
				array2[22] = 158;
				array2[22] = 155;
				array2[22] = 133;
				array2[22] = 82;
				array2[23] = 96;
				array2[23] = 134;
				array2[23] = 100;
				array2[23] = 97;
				array2[23] = 35;
				array2[23] = 154;
				array2[23] = 96;
				array2[24] = 74;
				array2[24] = 92;
				array2[24] = 130;
				array2[24] = 155;
				array2[24] = 40;
				array2[24] = 135;
				array2[24] = 74;
				array2[25] = 120;
				array2[25] = 108;
				array2[25] = 122;
				array2[25] = 120;
				array2[26] = 4;
				array2[26] = 130;
				array2[26] = 158;
				array2[26] = 4;
				array2[27] = 43;
				array2[27] = 100;
				array2[27] = 109;
				array2[27] = 159;
				array2[27] = 86;
				array2[27] = 107;
				array2[27] = 43;
				array2[28] = 53;
				array2[28] = 38;
				array2[28] = 197;
				array2[28] = 164;
				array2[28] = 53;
				array2[29] = 186;
				array2[29] = 110;
				array2[29] = 105;
				array2[29] = 186;
				array2[30] = 20;
				array2[30] = 129;
				array2[30] = 200;
				array2[30] = 117;
				array2[30] = 20;
				array2[31] = 148;
				array2[31] = 106;
				array2[31] = 131;
				array2[31] = 156;
				array2[31] = 148;
				// array2 的生效值（每下标最后一次赋值），即补丁表的 AES-256 密钥：
				//   21, 235, 68, 119, 61, 41, 47, 14, 211, 150, 154, 31, 231, 112, 30, 78,
				//   90, 80, 24, 105, 200, 39, 82, 96, 74, 120, 4, 43, 53, 186, 20, 148
				byte[] 密钥流表 = array2;
				// array4 是 IV 原始字节；生效值（写入时会先被 Array.Reverse 反转）：
				//   238, 55, 125, 6, 25, 23, 5, 143, 28, 164, 147, 193, 24, 110, 247, 220
				byte[] array4 = new byte[16];
				array4[0] = 149;
				array4[0] = 144;
				array4[0] = 238;
				array4[1] = 85;
				array4[1] = 161;
				array4[1] = 112;
				array4[1] = 86;
				array4[1] = 55;
				array4[2] = 149;
				array4[2] = 51;
				array4[2] = 61;
				array4[2] = 110;
				array4[2] = 87;
				array4[2] = 125;
				array4[3] = 129;
				array4[3] = 96;
				array4[3] = 40;
				array4[3] = 114;
				array4[3] = 6;
				array4[4] = 125;
				array4[4] = 110;
				array4[4] = 25;
				array4[5] = 119;
				array4[5] = 147;
				array4[5] = 136;
				array4[5] = 23;
				array4[6] = 214;
				array4[6] = 131;
				array4[6] = 118;
				array4[6] = 5;
				array4[7] = 83;
				array4[7] = 85;
				array4[7] = 100;
				array4[7] = 143;
				array4[8] = 139;
				array4[8] = 149;
				array4[8] = 105;
				array4[8] = 123;
				array4[8] = 28;
				array4[9] = 109;
				array4[9] = 86;
				array4[9] = 194;
				array4[9] = 164;
				array4[10] = 113;
				array4[10] = 77;
				array4[10] = 148;
				array4[10] = 128;
				array4[10] = 147;
				array4[11] = 62;
				array4[11] = 112;
				array4[11] = 65;
				array4[11] = 116;
				array4[11] = 127;
				array4[11] = 193;
				array4[12] = 66;
				array4[12] = 103;
				array4[12] = 125;
				array4[12] = 24;
				array4[13] = 167;
				array4[13] = 164;
				array4[13] = 110;
				array4[14] = 188;
				array4[14] = 114;
				array4[14] = 96;
				array4[14] = 184;
				array4[14] = 137;
				array4[14] = 247;
				array4[15] = 161;
				array4[15] = 110;
				array4[15] = 116;
				array4[15] = 220;
				byte[] 初始向量 = array4;
				Array.Reverse(初始向量);
				byte[] 公钥标记 = typeof(IndexerConfig).Assembly.GetName().GetPublicKeyToken();
				if (公钥标记 != null && 公钥标记.Length > 0)
				{
					初始向量[1] = 公钥标记[0];
					初始向量[3] = 公钥标记[1];
					初始向量[5] = 公钥标记[2];
					初始向量[7] = 公钥标记[3];
					初始向量[9] = 公钥标记[4];
					初始向量[11] = 公钥标记[5];
					初始向量[13] = 公钥标记[6];
					初始向量[15] = 公钥标记[7];
					Array.Clear(公钥标记, 0, 公钥标记.Length);
				}
				for (int i = 0; i < 初始向量.Length; i++)
				{
					密钥流表[i] ^= 初始向量[i];
				}
				// 下面把资源数据解一层：按 4 字节一组做 XOR + 变种 ARX 流密码（与 解密字符串 里的第二层同款）。
				byte[] 补丁数据 = 资源字节;
				int 余数字节 = 补丁数据.Length % 4;
				int 块数 = 补丁数据.Length / 4;
				byte[] 解密缓冲 = new byte[补丁数据.Length];
				int 密钥字数 = 密钥流表.Length / 4;
				uint 状态 = 0U;
				if (余数字节 > 0)
				{
					块数++;
				}
				for (int j = 0; j < 块数; j++)
				{
					int 密钥字序号 = j % 密钥字数;
					int 块内偏移 = j * 4;
					uint 临时偏移 = (uint)(密钥字序号 * 4);
					uint 密钥字 = (uint)(((int)密钥流表[(int)((UIntPtr)(临时偏移 + 3U))] << 24) | ((int)密钥流表[(int)((UIntPtr)(临时偏移 + 2U))] << 16) | ((int)密钥流表[(int)((UIntPtr)(临时偏移 + 1U))] << 8) | (int)密钥流表[(int)((UIntPtr)临时偏移)]);
					uint 掩码 = 255U;
					int 移位量 = 0;
					uint 明文块;
					if (j == 块数 - 1 && 余数字节 > 0)
					{
						状态 += 密钥字;
						明文块 = 0U;
						for (int k = 0; k < 余数字节; k++)
						{
							if (k > 0)
							{
								明文块 <<= 8;
							}
							明文块 |= (uint)补丁数据[补丁数据.Length - (1 + k)];
						}
					}
					else
					{
						临时偏移 = (uint)块内偏移;
						状态 += 密钥字;
						明文块 = (uint)(((int)补丁数据[(int)((UIntPtr)(临时偏移 + 3U))] << 24) | ((int)补丁数据[(int)((UIntPtr)(临时偏移 + 2U))] << 16) | ((int)补丁数据[(int)((UIntPtr)(临时偏移 + 1U))] << 8) | (int)补丁数据[(int)((UIntPtr)临时偏移)]);
					}
					状态 = 状态; // 原样保留的自赋值（VB 整数运算产生的中间赋值），无实际作用
					uint 旧状态 = 状态;
					uint 混合值 = 状态;
					混合值 ^= 混合值 >> 25;
					混合值 += 1394801171U;
					混合值 ^= 混合值 << 5;
					混合值 += 3457341307U;
					混合值 ^= 混合值 >> 3;
					混合值 += 4040842541U;
					混合值 = 2018246656U + 混合值;
					状态 = 旧状态 + (uint)混合值;
					if (j == 块数 - 1 && 余数字节 > 0)
					{
						uint 异或结果 = 状态 ^ 明文块;
						for (int l = 0; l < 余数字节; l++)
						{
							if (l > 0)
							{
								掩码 <<= 8;
								移位量 += 8;
							}
							解密缓冲[块内偏移 + l] = (byte)((异或结果 & 掩码) >> 移位量);
						}
					}
					else
					{
						uint 异或结果 = 状态 ^ 明文块;
						解密缓冲[块内偏移] = (byte)(异或结果 & 255U);
						解密缓冲[块内偏移 + 1] = (byte)((异或结果 & 65280U) >> 8);
						解密缓冲[块内偏移 + 2] = (byte)((异或结果 & 16711680U) >> 16);
						解密缓冲[块内偏移 + 3] = (byte)((异或结果 & 4278190080U) >> 24);
					}
				}
				byte[] 补丁表 = 解密缓冲;
				Array.Clear(初始向量, 0, 初始向量.Length);
				资源读取器.Close();
				资源读取器 = new BinaryReader(new MemoryStream(补丁表));
				资源读取器.BaseStream.Position = 0L;
				IntPtr 进程句柄 = IntPtr.Zero;
				Assembly 本程序集 = typeof(IndexerConfig).Assembly;
				// 0x38 = PROCESS_VM_OPERATION(0x08) | PROCESS_VM_READ(0x10) | PROCESS_VM_WRITE(0x20)：留着自己写自己的内存
				进程句柄 = IndexerConfig.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				IndexerConfig.模块基址64 = Marshal.GetHINSTANCE(本程序集.GetModules()[0]).ToInt64();
				IntPtr 写入字节数 = IntPtr.Zero;
				int 旧保护属性 = 0;
				int 补丁条数 = 资源读取器.ReadInt32();
				资源读取器.ReadInt32();
				for (int m = 0; m < 补丁条数; m++)
				{
					// 逐条 4 字节补丁：{ RVA, 值 }
					IntPtr 目标地址 = new IntPtr(IndexerConfig.模块基址64 + (long)资源读取器.ReadInt32());
					// 4 = PAGE_READWRITE
					IndexerConfig.VirtualProtect(目标地址, 4, 4, ref 旧保护属性);
					if (IntPtr.Size == 4)
					{
						IndexerConfig.WriteProcessMemory(进程句柄, 目标地址, BitConverter.GetBytes(资源读取器.ReadInt32()), 4U, out 写入字节数);
					}
					else
					{
						IndexerConfig.WriteProcessMemory(进程句柄, 目标地址, BitConverter.GetBytes(资源读取器.ReadInt32()), 4U, out 写入字节数);
					}
					// 注意：恢复保护属性时把「旧值」同时当作新值传进去，这是原码写法（会覆盖旧值），照原样保留
					IndexerConfig.VirtualProtect(目标地址, 4, 旧保护属性, ref 旧保护属性);
				}
				try
				{
					while (资源读取器.BaseStream.Position < 资源读取器.BaseStream.Length - 1L)
					{
						// 后续是成块的 DWORD 写入：{ RVA, 字数 } + 数据
						int 补丁RVA = 资源读取器.ReadInt32();
						IntPtr 块地址 = new IntPtr(IndexerConfig.模块基址64 + (long)补丁RVA);
						int 写入字数 = 资源读取器.ReadInt32();
						IndexerConfig.VirtualProtect(块地址, 写入字数 * 4, 4, ref 旧保护属性);
						for (int n = 0; n < 写入字数; n++)
						{
							Marshal.WriteInt32(new IntPtr(块地址.ToInt64() + (long)(n * 4)), 资源读取器.ReadInt32());
						}
						IndexerConfig.VirtualProtect(块地址, 写入字数 * 4, 旧保护属性, ref 旧保护属性);
					}
					IndexerConfig.CloseHandle(进程句柄);
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00007790 File Offset: 0x00005990
		/// <summary>
		/// 取程序集在磁盘上的路径，三种办法依次尝试（反射加载的程序集没有 Location，CodeBase 也常常拿不到）：
		/// 1) Assembly.Location；2) AssemblyName.CodeBase（去掉 file:/// 前缀）；3) 反射取 Location 属性。
		/// 都失败就返回空串。三个分支各自吞异常。
		/// 原混淆名：ForgotReader
		/// </summary>
		internal static object 获取程序集路径(Assembly 程序集)
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
			}
			return "";
		}

		// Token: 0x0600007B RID: 123
		/// <summary>kernel32!WriteProcessMemory：向指定进程的内存写数据（这里是自己）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr 进程句柄, IntPtr 目标地址, [In] [Out] byte[] 缓冲区, uint 长度, out IntPtr 写入字节数);

		// Token: 0x0600007C RID: 124
		/// <summary>kernel32!ReadProcessMemory：从指定进程的内存读数据（字符串解密用它读自己的模块镜像）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr 进程句柄, IntPtr 源地址, [In] [Out] byte[] 缓冲区, uint 长度, out IntPtr 读取字节数);

		// Token: 0x0600007D RID: 125
		/// <summary>kernel32!OpenProcess：取进程句柄（本程序只用来操作自己）。</summary>
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint 访问权限, int 继承句柄, uint 进程ID);

		// Token: 0x0600007E RID: 126
		/// <summary>kernel32!CloseHandle：关闭句柄。</summary>
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr 句柄);

		// Token: 0x0600007F RID: 127 RVA: 0x000078A0 File Offset: 0x00005AA0
		/// <summary>
		/// 把一个文件整体读成字节数组（循环读直到读满，因为 FileStream.Read 不保证一次读完）。
		/// 全工程没有调用点，是保护器留下的工具方法。
		/// 原混淆名：RevertReader
		/// </summary>
		private static byte[] 读取整个文件(string 文件路径)
		{
			byte[] 内容;
			using (FileStream 文件流 = new FileStream(文件路径, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				int 已读 = 0;
				long 文件长度 = 文件流.Length;
				int 剩余 = (int)文件长度;
				内容 = new byte[剩余];
				while (剩余 > 0)
				{
					int 本次读 = 文件流.Read(内容, 已读, 剩余);
					已读 += 本次读;
					剩余 -= 本次读;
				}
			}
			return 内容;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00007908 File Offset: 0x00005B08
		/// <summary>
		/// 用「固定 Key + 固定 IV」做 AES-CBC 解密（无填充校验，结果直接返回）。
		/// 字符串解密流程的最后一步用的就是它：读出来的密文丢进来，出去就是 UTF-16 明文。
		/// 原混淆名：VerifyConfig
		/// </summary>
		private static byte[] AES解密(byte[] 密文)
		{
			MemoryStream 内存流 = new MemoryStream();
			SymmetricAlgorithm AES算法 = IndexerConfig.创建AES实现();
			// 硬编码的 32 字节 Key / 16 字节 IV（.NET Reactor 字符串加密的固定密钥，见同名 .md 的 IOC 段）
			AES算法.Key = new byte[]
			{
				248, 124, 241, 203, 138, 171, 153, 86, 79, 123,
				202, 239, 236, 16, 191, 67, 80, 235, 38, 120,
				232, 218, 19, 17, 82, 207, 204, 82, 116, 20,
				145, 96
			};
			AES算法.IV = new byte[]
			{
				97, 22, 156, 34, 144, 187, 12, 232, 197, 73,
				101, 49, 195, 148, 20, 151
			};
			CryptoStream 解密流 = new CryptoStream(内存流, AES算法.CreateDecryptor(), CryptoStreamMode.Write);
			解密流.Write(密文, 0, 密文.Length);
			解密流.Close();
			return 内存流.ToArray();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00007978 File Offset: 0x00005B78
		/// <summary>空实现，恒返回 null，无调用者；保护器给别的保护特性（授权/加密模块）预留的占位。原混淆名：IncludeConfig</summary>
		private byte[] 空实现_返回null_1()
		{
			return null;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00007978 File Offset: 0x00005B78
		/// <summary>空实现，恒返回 null，无调用者。原混淆名：SetConfig</summary>
		private byte[] 空实现_返回null_2()
		{
			return null;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00007988 File Offset: 0x00005B88
		/// <summary>空实现，恒返回 null，无调用者。原混淆名：EnableConfig</summary>
		private byte[] 空实现_返回null_3()
		{
			return null;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00007988 File Offset: 0x00005B88
		/// <summary>空实现，恒返回 null，无调用者。原混淆名：InterruptConfig</summary>
		private byte[] 空实现_返回null_4()
		{
			return null;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00007978 File Offset: 0x00005B78
		/// <summary>空实现，恒返回 null，无调用者。原混淆名：PushConfig</summary>
		private byte[] 空实现_返回null_5()
		{
			return null;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00007978 File Offset: 0x00005B78
		/// <summary>空实现，恒返回 null，无调用者。原混淆名：FindConfig</summary>
		private byte[] 空实现_返回null_6()
		{
			return null;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00007998 File Offset: 0x00005B98
		/// <summary>
		/// 混淆桩：局部变量是一个 GUID 样式的许可标识串，但两个分支返回的常量完全一样（{1, 2}），
		/// 所以无论走哪条路结果都相同——纯噪声，用来让反编译结果看起来像「在检查许可证」。
		/// 无调用者。原混淆名：ComputeConfig
		/// </summary>
		internal byte[] 混淆桩_返回1和2_1()
		{
			string 许可标识 = "{11111-22222-40001-00001}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000079D8 File Offset: 0x00005BD8
		/// <summary>混淆桩，两分支返回相同常量。许可标识串："{11111-22222-40001-00002}"。原混淆名：CallConfig</summary>
		internal byte[] 混淆桩_返回1和2_2()
		{
			string 许可标识 = "{11111-22222-40001-00002}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00007A18 File Offset: 0x00005C18
		/// <summary>混淆桩，两分支返回相同常量。许可标识串："{11111-22222-50001-00001}"。原混淆名：CalculateConfig</summary>
		internal byte[] 混淆桩_返回1和2_3()
		{
			string 许可标识 = "{11111-22222-50001-00001}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00007A58 File Offset: 0x00005C58
		/// <summary>混淆桩，两分支返回相同常量。许可标识串："{11111-22222-50001-00002}"。原混淆名：PostConfig</summary>
		internal byte[] 混淆桩_返回1和2_4()
		{
			string 许可标识 = "{11111-22222-50001-00002}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00007A98 File Offset: 0x00005C98
		/// <summary>混淆桩，两分支返回相同常量。许可标识串："{11111-22222-60001-00001}"。原混淆名：SearchConfig</summary>
		internal byte[] 混淆桩_返回1和2_5()
		{
			string 许可标识 = "{11111-22222-60001-00001}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00007AD8 File Offset: 0x00005CD8
		/// <summary>混淆桩，两分支返回相同常量。许可标识串："{11111-22222-60001-00002}"。原混淆名：PublishConfig</summary>
		internal byte[] 混淆桩_返回1和2_6()
		{
			string 许可标识 = "{11111-22222-60001-00002}";
			if (许可标识.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00007B18 File Offset: 0x00005D18
		/// <summary>
		/// AES-CBC 加密（AES解密 的逆操作）：明文按 UTF-16 编码，
		/// Key 用下面硬编码的 32 字节常量，IV = 计算哈希(UTF-16(第二个参数))，输出 Base64。
		/// 全工程没有调用点，应当是保护器加密配置/许可数据用的工具方法。
		/// 原混淆名：FillConfig
		/// </summary>
		internal static string AES加密(string 明文, string 初始向量种子)
		{
			byte[] UTF16字节 = Encoding.Unicode.GetBytes(明文);
			byte[] 明文数组 = UTF16字节;
			// 硬编码的 32 字节 Key（与 AES解密 里那一组不是同一套，见同名 .md 的 IOC 段）
			byte[] 固定密钥 = new byte[]
			{
				82, 102, 104, 110, 32, 77, 24, 34, 118, 181,
				51, 17, 18, 51, 12, 109, 10, 32, 77, 24,
				34, 158, 161, 41, 97, 28, 118, 181, 5, 25,
				1, 88
			};
			byte[] 初始向量 = IndexerConfig.计算哈希(Encoding.Unicode.GetBytes(初始向量种子));
			MemoryStream 内存流 = new MemoryStream();
			SymmetricAlgorithm AES算法 = IndexerConfig.创建AES实现();
			AES算法.Key = 固定密钥;
			AES算法.IV = 初始向量;
			CryptoStream 加密流 = new CryptoStream(内存流, AES算法.CreateEncryptor(), CryptoStreamMode.Write);
			加密流.Write(明文数组, 0, 明文数组.Length);
			加密流.Close();
			return Convert.ToBase64String(内存流.ToArray());
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>混淆器注入的占位方法：恒返回 true，无调用者。原混淆名：PostParser</summary>
		internal static bool 混淆器占位方法_恒真()
		{
			return true;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>混淆器注入的占位方法：恒返回 false，无调用者。原混淆名：SetupParser</summary>
		internal static bool 混淆器占位方法_恒假()
		{
			return false;
		}

		// Token: 0x04000031 RID: 49
		/// <summary>字符串表 / 补丁表解密后的字节（首次解密时填充）。</summary>
		private static byte[] 解密后数据 = new byte[0];

		// Token: 0x04000032 RID: 50
		/// <summary>偏移 → 字符串缓存下标的映射（0 表示这个偏移还没解过）。</summary>
		private static int[] 字符串缓存索引表 = new int[0];

		// Token: 0x04000033 RID: 51
		/// <summary>本模块在内存里的基址（64 位形式，来自 Marshal.GetHINSTANCE）。</summary>
		private static long 模块基址64 = 0L;

		// Token: 0x04000034 RID: 52
		/// <summary>是否已探测过 FIPS 开关（只探测一次）。</summary>
		private static bool 已探测FIPS = false;

		// Token: 0x04000035 RID: 53
		/// <summary>MD5 的 64 个常量 K[i] = floor(2^32 × |sin(i+1)|)，标准 RFC 1321 表，一个字节都没改。</summary>
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

		// Token: 0x04000036 RID: 54
		/// <summary>读自己内存用的进程句柄（首次调用 解密字符串 时打开，之后复用）。</summary>
		private static IntPtr 进程句柄 = IntPtr.Zero;

		// Token: 0x04000037 RID: 55
		/// <summary>是否已执行过 还原方法体（补丁只打一次）。</summary>
		private static bool 已还原方法体 = false;

		// Token: 0x04000038 RID: 56
		/// <summary>本模块在内存里的基址（32 位形式，只有 IntPtr.Size == 4 时才填）。</summary>
		private static int 模块基址32 = 0;

		// Token: 0x04000039 RID: 57
		/// <summary>已解密字符串的缓存（按 字符串缓存游标 顺序存放）。</summary>
		private static string[] 字符串缓存 = new string[0];

		// Token: 0x0400003A RID: 58
		/// <summary>ReadProcessMemory 的出参：实际读到的字节数（本程序不检查它）。</summary>
		private static IntPtr 读取字节数 = IntPtr.Zero;

		// Token: 0x0400003B RID: 59
		/// <summary>未被任何代码使用的残留字段。</summary>
		private static byte[] 未使用字段_1 = new byte[0];

		// Token: 0x0400003C RID: 60
		/// <summary>系统是否只允许 FIPS 算法（反射探测结果）。</summary>
		private static bool 允许FIPS算法 = false;

		// Token: 0x0400003D RID: 61
		/// <summary>已解密字符串的个数（下一个缓存下标）。</summary>
		private static int 字符串缓存游标 = 1;

		// Token: 0x0400003E RID: 62
		/// <summary>未被任何代码使用的残留字段。</summary>
		private static byte[] 未使用字段_2 = new byte[0];

		// Token: 0x0400003F RID: 63
		/// <summary>未被任何代码使用的残留字段。</summary>
		private static byte[] 未使用字段_3 = new byte[0];

		// Token: 0x02000013 RID: 19
		/// <summary>
		/// 混淆用的空特性：构造函数收一个 object 参数但什么都不做，唯一的用途是挂在
		/// 还原方法体 上面，把 typeof(CreatorConfig&lt;object&gt;[]) 塞进元数据——
		/// 目的是让「方法体还原」这个方法的签名看起来引用了一个泛型类型（增加反射扫描噪声）。
		/// 原混淆名：WatcherConfig
		/// </summary>
		internal class WatcherConfig : Attribute
		{
			// Token: 0x06000091 RID: 145 RVA: 0x00002511 File Offset: 0x00000711
			/// <summary>空构造函数：只接收一个参数，不做任何事。原混淆名：WatcherConfig(object)</summary>
			[IndexerConfig.WatcherConfig(typeof(IndexerConfig.WatcherConfig.CreatorConfig<object>[]))]
			public WatcherConfig(object 占位参数)
			{
			}

			// Token: 0x02000014 RID: 20
			/// <summary>只用来被 typeof 引用的空泛型类，本身没有成员逻辑。原混淆名：CreatorConfig&lt;T&gt;</summary>
			internal class CreatorConfig<T>
			{
				// Token: 0x06000093 RID: 147 RVA: 0x000021DD File Offset: 0x000003DD
				/// <summary>混淆器注入的占位方法：恒返回 true，无调用者。原混淆名：CancelParser</summary>
				internal static bool 混淆器占位方法_恒真()
				{
					return true;
				}

				// Token: 0x06000094 RID: 148 RVA: 0x000021E8 File Offset: 0x000003E8
				/// <summary>混淆器注入的占位方法：恒返回 false，无调用者。原混淆名：CreateParser</summary>
				internal static bool 混淆器占位方法_恒假()
				{
					return false;
				}
			}
		}

		// Token: 0x02000015 RID: 21
		/// <summary>
		/// 空枚举（连一个成员都没有），带 [Flags]。纯混淆噪声，用来占一个类型槽。
		/// 原混淆名：VirtualTreeDisplayDataChanges
		/// </summary>
		[Flags]
		private enum VirtualTreeDisplayDataChanges
		{

		}
	}
}
