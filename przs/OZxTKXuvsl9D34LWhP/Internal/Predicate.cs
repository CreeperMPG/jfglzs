using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace OZxTKXuvsl9D34LWhP.Internal
{
	/// <summary>
	/// .NET Reactor 加壳器的运行时残留（类名 Predicate 是混淆名，原类型名不可考）。
	///
	/// 真实职责分三块，都与"机房管理助手"的业务无关，但关系到程序能不能跑起来：
	/// <list type="number">
	/// <item>自实现 MD5：<see cref="自实现MD5"/> 与四个轮函数。
	///   当系统配置成"只允许 FIPS 认证算法"（CryptoConfig.AllowOnlyFipsAlgorithms）时，
	///   MD5CryptoServiceProvider 会抛异常，加壳器改用自己手写的 MD5 顶上。</item>
	/// <item>字符串解密：<see cref="解密字符串"/> 从内嵌资源
	///   940fde4f-6937-4325-8f76-5a332189a59c 恢复出字符串表，再按传入下标从
	///   本进程映像里读出一条 Unicode 字符串（每个方法里的 <c>Predicate.RunWatcher(0x……)</c>
	///   调用就是它；NETReactorSlayer 解壳后这些调用大多已被替换成明文字符串，本类随后变成死代码）。</item>
	/// <item>方法体解密：<see cref="解密方法体并回写内存"/> 从内嵌资源
	///   5c5b1961-5059-4bdd-b555-d9e6275d4842 解出 IL 补丁数据，用
	///   VirtualProtect + WriteProcessMemory 直接改写"自己进程"里的方法体——
	///   这是加壳器把加密方法在 JIT 之前还原成真身的手段。</item>
	/// </list>
	/// 说明：本类只在内部调用自己，Form1/Form2 等业务代码完全不引用它。
	/// 但 <see cref="解密方法体并回写内存"/> 的"自我内存补丁"正是全样本所有加密方法能执行的前提，
	/// 分析样本行为时不能忽略这一步。
	/// </summary>
	internal class Predicate
	{
		// Token: 0x060000AD RID: 173 RVA: 0x00002579 File Offset: 0x00000779
		/// <summary>加壳器留下的空方法，无调用者。</summary>
		private void 占位空方法()
		{
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00006EFC File Offset: 0x000050FC
		/// <summary>
		/// 自实现的 MD5（原名 RevertVal），返回 16 字节摘要。
		/// 标准 MD5：先补 0x80 + 0 到 448 mod 512，再补 64 位小端原始位长度，
		/// 然后按 512 位分块做 64 轮（F/G/H/I 四组各 16 轮），最后把 A/B/C/D 小端拼回。
		/// 注意：原导出签名的参数类型是 object（天真的反编译结果），IL 里按 byte[] 用，
		/// 唯一的调用方 <see cref="计算MD5"/> 传的也是 byte[]，这里按真实类型标注，行为不变。
		/// </summary>
		/// <param name="数据">待求摘要的字节数组。</param>
		internal static byte[] 自实现MD5(byte[] 数据)
		{
			uint[] 消息字 = new uint[16];
			// 448 - (len*8 % 512)：还要补多少位才能让"位长度"落在 448
			int 尾部需补位数 = 448 - 数据.Length * 8 % 512;
			uint 填充字节数 = (uint)((尾部需补位数 + 512) % 512);
			if (填充字节数 == 0U)
			{
				填充字节数 = 512U;
			}
			uint 总字节数 = (uint)((long)数据.Length + (long)((ulong)(填充字节数 / 8U)) + 8L);
			ulong 原始位长度 = (ulong)((long)数据.Length * 8L);
			byte[] 填充后数据 = new byte[总字节数];
			for (int i = 0; i < 数据.Length; i++)
			{
				填充后数据[i] = 数据[i];
			}
			int 填充起点 = 数据.Length;
			填充后数据[填充起点] |= 128;
			// 末尾 8 字节写原始位长度（小端）
			for (int j = 8; j > 0; j--)
			{
				填充后数据[(int)(checked((IntPtr)(unchecked((ulong)总字节数 - (ulong)((long)j)))))] = (byte)((原始位长度 >> (8 - j) * 8) & 255UL);
			}
			uint 总字数 = (uint)(填充后数据.Length * 8 / 32);
			uint 甲 = 1732584193U;  // A = 0x67452301
			uint 乙 = 4023233417U;  // B = 0xEFCDAB89
			uint 丙 = 2562383102U;  // C = 0x98BADCFE
			uint 丁 = 271733878U;   // D = 0x10325476
			for (uint 块 = 0U; 块 < 总字数 / 16U; 块 += 1U)
			{
				uint 块偏移 = 块 << 6;
				// 每 4 个字节拼成一个小端 uint；上界写 61 而不是 64，步长 4 → 0,4,…,60 共 16 次，结果一致
				for (uint i = 0U; i < 61U; i += 4U)
				{
					消息字[(int)((UIntPtr)(i >> 2))] = (uint)(((int)填充后数据[(int)((UIntPtr)(块偏移 + (i + 3U)))] << 24) | ((int)填充后数据[(int)((UIntPtr)(块偏移 + (i + 2U)))] << 16) | ((int)填充后数据[(int)((UIntPtr)(块偏移 + (i + 1U)))] << 8) | (int)填充后数据[(int)((UIntPtr)(块偏移 + i))]);
				}
				uint 暂存甲 = 甲;
				uint 暂存乙 = 乙;
				uint 暂存丙 = 丙;
				uint 暂存丁 = 丁;
				// 第 1 组：F 函数，共 16 轮
				MD5轮函数F(ref 甲, 乙, 丙, 丁, 0U, 7, 1U, 消息字);
				MD5轮函数F(ref 丁, 甲, 乙, 丙, 1U, 12, 2U, 消息字);
				MD5轮函数F(ref 丙, 丁, 甲, 乙, 2U, 17, 3U, 消息字);
				MD5轮函数F(ref 乙, 丙, 丁, 甲, 3U, 22, 4U, 消息字);
				MD5轮函数F(ref 甲, 乙, 丙, 丁, 4U, 7, 5U, 消息字);
				MD5轮函数F(ref 丁, 甲, 乙, 丙, 5U, 12, 6U, 消息字);
				MD5轮函数F(ref 丙, 丁, 甲, 乙, 6U, 17, 7U, 消息字);
				MD5轮函数F(ref 乙, 丙, 丁, 甲, 7U, 22, 8U, 消息字);
				MD5轮函数F(ref 甲, 乙, 丙, 丁, 8U, 7, 9U, 消息字);
				MD5轮函数F(ref 丁, 甲, 乙, 丙, 9U, 12, 10U, 消息字);
				MD5轮函数F(ref 丙, 丁, 甲, 乙, 10U, 17, 11U, 消息字);
				MD5轮函数F(ref 乙, 丙, 丁, 甲, 11U, 22, 12U, 消息字);
				MD5轮函数F(ref 甲, 乙, 丙, 丁, 12U, 7, 13U, 消息字);
				MD5轮函数F(ref 丁, 甲, 乙, 丙, 13U, 12, 14U, 消息字);
				MD5轮函数F(ref 丙, 丁, 甲, 乙, 14U, 17, 15U, 消息字);
				MD5轮函数F(ref 乙, 丙, 丁, 甲, 15U, 22, 16U, 消息字);
				// 第 2 组：G 函数，共 16 轮
				MD5轮函数G(ref 甲, 乙, 丙, 丁, 1U, 5, 17U, 消息字);
				MD5轮函数G(ref 丁, 甲, 乙, 丙, 6U, 9, 18U, 消息字);
				MD5轮函数G(ref 丙, 丁, 甲, 乙, 11U, 14, 19U, 消息字);
				MD5轮函数G(ref 乙, 丙, 丁, 甲, 0U, 20, 20U, 消息字);
				MD5轮函数G(ref 甲, 乙, 丙, 丁, 5U, 5, 21U, 消息字);
				MD5轮函数G(ref 丁, 甲, 乙, 丙, 10U, 9, 22U, 消息字);
				MD5轮函数G(ref 丙, 丁, 甲, 乙, 15U, 14, 23U, 消息字);
				MD5轮函数G(ref 乙, 丙, 丁, 甲, 4U, 20, 24U, 消息字);
				MD5轮函数G(ref 甲, 乙, 丙, 丁, 9U, 5, 25U, 消息字);
				MD5轮函数G(ref 丁, 甲, 乙, 丙, 14U, 9, 26U, 消息字);
				MD5轮函数G(ref 丙, 丁, 甲, 乙, 3U, 14, 27U, 消息字);
				MD5轮函数G(ref 乙, 丙, 丁, 甲, 8U, 20, 28U, 消息字);
				MD5轮函数G(ref 甲, 乙, 丙, 丁, 13U, 5, 29U, 消息字);
				MD5轮函数G(ref 丁, 甲, 乙, 丙, 2U, 9, 30U, 消息字);
				MD5轮函数G(ref 丙, 丁, 甲, 乙, 7U, 14, 31U, 消息字);
				MD5轮函数G(ref 乙, 丙, 丁, 甲, 12U, 20, 32U, 消息字);
				// 第 3 组：H 函数，共 16 轮
				MD5轮函数H(ref 甲, 乙, 丙, 丁, 5U, 4, 33U, 消息字);
				MD5轮函数H(ref 丁, 甲, 乙, 丙, 8U, 11, 34U, 消息字);
				MD5轮函数H(ref 丙, 丁, 甲, 乙, 11U, 16, 35U, 消息字);
				MD5轮函数H(ref 乙, 丙, 丁, 甲, 14U, 23, 36U, 消息字);
				MD5轮函数H(ref 甲, 乙, 丙, 丁, 1U, 4, 37U, 消息字);
				MD5轮函数H(ref 丁, 甲, 乙, 丙, 4U, 11, 38U, 消息字);
				MD5轮函数H(ref 丙, 丁, 甲, 乙, 7U, 16, 39U, 消息字);
				MD5轮函数H(ref 乙, 丙, 丁, 甲, 10U, 23, 40U, 消息字);
				MD5轮函数H(ref 甲, 乙, 丙, 丁, 13U, 4, 41U, 消息字);
				MD5轮函数H(ref 丁, 甲, 乙, 丙, 0U, 11, 42U, 消息字);
				MD5轮函数H(ref 丙, 丁, 甲, 乙, 3U, 16, 43U, 消息字);
				MD5轮函数H(ref 乙, 丙, 丁, 甲, 6U, 23, 44U, 消息字);
				MD5轮函数H(ref 甲, 乙, 丙, 丁, 9U, 4, 45U, 消息字);
				MD5轮函数H(ref 丁, 甲, 乙, 丙, 12U, 11, 46U, 消息字);
				MD5轮函数H(ref 丙, 丁, 甲, 乙, 15U, 16, 47U, 消息字);
				MD5轮函数H(ref 乙, 丙, 丁, 甲, 2U, 23, 48U, 消息字);
				// 第 4 组：I 函数，共 16 轮
				MD5轮函数I(ref 甲, 乙, 丙, 丁, 0U, 6, 49U, 消息字);
				MD5轮函数I(ref 丁, 甲, 乙, 丙, 7U, 10, 50U, 消息字);
				MD5轮函数I(ref 丙, 丁, 甲, 乙, 14U, 15, 51U, 消息字);
				MD5轮函数I(ref 乙, 丙, 丁, 甲, 5U, 21, 52U, 消息字);
				MD5轮函数I(ref 甲, 乙, 丙, 丁, 12U, 6, 53U, 消息字);
				MD5轮函数I(ref 丁, 甲, 乙, 丙, 3U, 10, 54U, 消息字);
				MD5轮函数I(ref 丙, 丁, 甲, 乙, 10U, 15, 55U, 消息字);
				MD5轮函数I(ref 乙, 丙, 丁, 甲, 1U, 21, 56U, 消息字);
				MD5轮函数I(ref 甲, 乙, 丙, 丁, 8U, 6, 57U, 消息字);
				MD5轮函数I(ref 丁, 甲, 乙, 丙, 15U, 10, 58U, 消息字);
				MD5轮函数I(ref 丙, 丁, 甲, 乙, 6U, 15, 59U, 消息字);
				MD5轮函数I(ref 乙, 丙, 丁, 甲, 13U, 21, 60U, 消息字);
				MD5轮函数I(ref 甲, 乙, 丙, 丁, 4U, 6, 61U, 消息字);
				MD5轮函数I(ref 丁, 甲, 乙, 丙, 11U, 10, 62U, 消息字);
				MD5轮函数I(ref 丙, 丁, 甲, 乙, 2U, 15, 63U, 消息字);
				MD5轮函数I(ref 乙, 丙, 丁, 甲, 9U, 21, 64U, 消息字);
				甲 += 暂存甲;
				乙 += 暂存乙;
				丙 += 暂存丙;
				丁 += 暂存丁;
			}
			byte[] 摘要 = new byte[16];
			Array.Copy(BitConverter.GetBytes(甲), 0, 摘要, 0, 4);
			Array.Copy(BitConverter.GetBytes(乙), 0, 摘要, 4, 4);
			Array.Copy(BitConverter.GetBytes(丙), 0, 摘要, 8, 4);
			Array.Copy(BitConverter.GetBytes(丁), 0, 摘要, 12, 4);
			return 摘要;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000257B File Offset: 0x0000077B
		/// <summary>MD5 第 1 组轮函数 F(X,Y,Z) = (X &amp; Y) | (~X &amp; Z)，原名 CallWatcher。</summary>
		private static void MD5轮函数F(ref uint 甲, uint 乙, uint 丙, uint 丁, uint 消息索引, ushort 左移位数, uint 常量序号, uint[] 消息字)
		{
			甲 = 乙 + 循环左移(甲 + ((乙 & 丙) | (~乙 & 丁)) + 消息字[(int)((UIntPtr)消息索引)] + MD5常量表[(int)((UIntPtr)(常量序号 - 1U))], 左移位数);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000025A6 File Offset: 0x000007A6
		/// <summary>MD5 第 2 组轮函数 G(X,Y,Z) = (X &amp; Z) | (Y &amp; ~Z)，原名 NewWatcher。</summary>
		private static void MD5轮函数G(ref uint 甲, uint 乙, uint 丙, uint 丁, uint 消息索引, ushort 左移位数, uint 常量序号, uint[] 消息字)
		{
			甲 = 乙 + 循环左移(甲 + ((乙 & 丁) | (丙 & ~丁)) + 消息字[(int)((UIntPtr)消息索引)] + MD5常量表[(int)((UIntPtr)(常量序号 - 1U))], 左移位数);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000025D1 File Offset: 0x000007D1
		/// <summary>MD5 第 3 组轮函数 H(X,Y,Z) = X ^ Y ^ Z，原名 ConcatWatcher。</summary>
		private static void MD5轮函数H(ref uint 甲, uint 乙, uint 丙, uint 丁, uint 消息索引, ushort 左移位数, uint 常量序号, uint[] 消息字)
		{
			甲 = 乙 + 循环左移(甲 + (乙 ^ 丙 ^ 丁) + 消息字[(int)((UIntPtr)消息索引)] + MD5常量表[(int)((UIntPtr)(常量序号 - 1U))], 左移位数);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000025F9 File Offset: 0x000007F9
		/// <summary>MD5 第 4 组轮函数 I(X,Y,Z) = Y ^ (X | ~Z)，原名 PostWatcher。</summary>
		private static void MD5轮函数I(ref uint 甲, uint 乙, uint 丙, uint 丁, uint 消息索引, ushort 左移位数, uint 常量序号, uint[] 消息字)
		{
			甲 = 乙 + 循环左移(甲 + (丙 ^ (乙 | ~丁)) + 消息字[(int)((UIntPtr)消息索引)] + MD5常量表[(int)((UIntPtr)(常量序号 - 1U))], 左移位数);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00002622 File Offset: 0x00000822
		/// <summary>32 位循环左移，原名 ResetWatcher。</summary>
		private static uint 循环左移(uint 值, ushort 位数)
		{
			return (值 >> (int)(32 - 位数)) | (值 << (int)位数);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00002634 File Offset: 0x00000834
		/// <summary>
		/// 查询"系统是否只允许 FIPS 认证算法"（原名 PushWatcher）。
		/// 首次调用时反射读取并缓存 CryptoConfig.AllowOnlyFipsAlgorithms。
		/// </summary>
		internal static bool 是否只允许FIPS算法()
		{
			if (!已读取FIPS配置)
			{
				读取FIPS配置();
				已读取FIPS配置 = true;
			}
			return 仅允许FIPS算法;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000075A0 File Offset: 0x000057A0
		/// <summary>
		/// 反射读取 CryptoConfig.AllowOnlyFipsAlgorithms 并存进字段（原名 OrderWatcher）。
		/// 读不到就吞掉异常，字段保持 false。
		/// </summary>
		internal static void 读取FIPS配置()
		{
			try
			{
				仅允许FIPS算法 = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00007600 File Offset: 0x00007600
		/// <summary>
		/// 创建对称加密算法（原名 PopWatcher）：FIPS 模式下优先 AES CSP，失败退回 Rijndael；
		/// 非 FIPS 模式直接用 RijndaelManaged。
		/// </summary>
		internal static SymmetricAlgorithm 创建对称加密算法()
		{
			SymmetricAlgorithm 算法 = null;
			if (是否只允许FIPS算法())
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

		// Token: 0x060000B7 RID: 183 RVA: 0x0000264D File Offset: 0x0000264D
		/// <summary>
		/// 计算 MD5（原名 CollectWatcher）：非 FIPS 走系统 MD5CryptoServiceProvider，
		/// FIPS 走 <see cref="自实现MD5"/>。
		/// </summary>
		internal static byte[] 计算MD5(byte[] 数据)
		{
			if (!是否只允许FIPS算法())
			{
				return new MD5CryptoServiceProvider().ComputeHash(数据);
			}
			return 自实现MD5(数据);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00007640 File Offset: 0x00007640
		/// <summary>
		/// 解密一条字符串（原名 RunWatcher），加壳器把每条字符串编译成
		/// <c>Predicate.RunWatcher(0x0000XXXX)</c> 这样的调用。
		///
		/// 流程：
		/// <list type="number">
		/// <item>首次进入时读内嵌资源 940fde4f-6937-4325-8f76-5a332189a59c，
		///   用 32 字节密钥 + 16 字节 IV 做 AES-CBC 解密（<paramref name="字符串ID"/> 为 -1 时才做，
		///   即"整表预解密"模式）；</item>
		/// <item>对结果再做一层自制的 xorshift 流解密（块大小 4 字节、密钥循环使用）；</item>
		/// <item>之后按 <paramref name="字符串ID"/> 从解密缓冲里取"偏移 + 长度"，
		///   再用 ReadProcessMemory 从本模块的内存映像里把密文读出来，AES 解密后按 Unicode 解码。</item>
		/// </list>
		/// 结果带缓存（<see cref="缓存索引"/> / <see cref="字符串缓存"/>）。
		/// </summary>
		static string 解密字符串(int 字符串ID)
		{
			if (已解密的字符串缓冲.Length == 0)
			{
				BinaryReader 资源读取器 = new BinaryReader(typeof(Predicate).Assembly.GetManifestResourceStream("940fde4f-6937-4325-8f76-5a332189a59c"));
				资源读取器.BaseStream.Position = 0L;
				byte[] 资源数据 = 资源读取器.ReadBytes((int)资源读取器.BaseStream.Length);
				// 32 字节密钥。原版对同一下标重复赋值了 3~6 次（加壳器的垃圾赋值），
				// 只有最后一次写入有效，这里只保留最终值，字节数/顺序与运行结果完全一致。
				byte[] 密钥 = new byte[32];
				密钥[0] = 251;
				密钥[1] = 0;
				密钥[2] = 52;
				密钥[3] = 224;
				密钥[4] = 103;
				密钥[5] = 167;
				密钥[6] = 113;
				密钥[7] = 64;
				密钥[8] = 228;
				密钥[9] = 80;
				密钥[10] = 29;
				密钥[11] = 73;
				密钥[12] = 140;
				密钥[13] = 142;
				密钥[14] = 208;
				密钥[15] = 248;
				密钥[16] = 172;
				密钥[17] = 35;
				密钥[18] = 83;
				密钥[19] = 93;
				密钥[20] = 229;
				密钥[21] = 145;
				密钥[22] = 56;
				密钥[23] = 233;
				密钥[24] = 180;
				密钥[25] = 59;
				密钥[26] = 222;
				密钥[27] = 185;
				密钥[28] = 126;
				密钥[29] = 46;
				密钥[30] = 174;
				密钥[31] = 139;
				byte[] 初始向量 = new byte[16];
				初始向量[0] = 92;
				初始向量[1] = 18;
				初始向量[2] = 49;
				初始向量[3] = 233;
				初始向量[4] = 234;
				初始向量[5] = 87;
				初始向量[6] = 204;
				初始向量[7] = 72;
				初始向量[8] = 51;
				初始向量[9] = 7;
				初始向量[10] = 29;
				初始向量[11] = 76;
				初始向量[12] = 39;
				初始向量[13] = 77;
				初始向量[14] = 69;
				初始向量[15] = 85;
				byte[] 公钥标记 = typeof(Predicate).Assembly.GetName().GetPublicKeyToken();
				// 有强名称时，用公钥标记的低 8 字节替换 IV 的奇数位（防篡改：换了签名就解不出来）
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
				}
				// 密钥前 16 字节与 IV 异或（array3 只是 array2 的别名，改的就是密钥本身）
				for (int i = 0; i < 初始向量.Length; i++)
				{
					密钥[i] ^= 初始向量[i];
				}
				if (字符串ID == -1)
				{
					SymmetricAlgorithm 算法 = 创建对称加密算法();
					算法.Mode = CipherMode.CBC;
					ICryptoTransform 解密器 = 算法.CreateDecryptor(密钥, 初始向量);
					MemoryStream 输出流 = new MemoryStream();
					CryptoStream 解密流 = new CryptoStream(输出流, 解密器, CryptoStreamMode.Write);
					解密流.Write(资源数据, 0, 资源数据.Length);
					解密流.FlushFinalBlock();
					已解密的字符串缓冲 = 输出流.ToArray();
					if (已解密的字符串缓冲.Length > 0)
					{
						字符串缓存 = new string[已解密的字符串缓冲.Length / 4 + 1];
						缓存索引 = new int[已解密的字符串缓冲.Length / 4 + 1];
					}
					输出流.Close();
					解密流.Close();
					资源读取器.Close();
					资源数据 = 已解密的字符串缓冲;
				}
				// 流解密：每 4 字节一块，密钥表循环复用，块序号累加后 xorshift 混淆
				int 尾块字节数 = 资源数据.Length % 4;
				int 块数 = 资源数据.Length / 4;
				byte[] 流解密结果 = new byte[资源数据.Length];
				int 密钥块数 = 密钥.Length / 4;
				uint 累加值 = 0U;
				if (尾块字节数 > 0)
				{
					块数++;
				}
				for (int 块索引 = 0; 块索引 < 块数; 块索引++)
				{
					int 密钥块索引 = 块索引 % 密钥块数;
					int 块偏移 = 块索引 * 4;
					uint 密钥内偏移 = (uint)(密钥块索引 * 4);
					// 密钥字按小端从 4 个密钥字节拼出
					uint 密钥字 = (uint)(((int)密钥[(int)((UIntPtr)(密钥内偏移 + 3U))] << 24) | ((int)密钥[(int)((UIntPtr)(密钥内偏移 + 2U))] << 16) | ((int)密钥[(int)((UIntPtr)(密钥内偏移 + 1U))] << 8) | (int)密钥[(int)((UIntPtr)密钥内偏移)]);
					uint 掩码 = 255U;
					int 已移位数 = 0;
					uint 密文字;
					if (块索引 == 块数 - 1 && 尾块字节数 > 0)
					{
						// 最后不足 4 字节的尾巴：按剩余字节数从后往前拼
						密文字 = 0U;
						for (int k = 0; k < 尾块字节数; k++)
						{
							if (k > 0)
							{
								密文字 <<= 8;
							}
							密文字 |= (uint)资源数据[资源数据.Length - (1 + k)];
						}
						累加值 += 密钥字;
					}
					else
					{
						密钥内偏移 = (uint)块偏移;
						密文字 = (uint)(((int)资源数据[(int)((UIntPtr)(密钥内偏移 + 3U))] << 24) | ((int)资源数据[(int)((UIntPtr)(密钥内偏移 + 2U))] << 16) | ((int)资源数据[(int)((UIntPtr)(密钥内偏移 + 1U))] << 8) | (int)资源数据[(int)((UIntPtr)密钥内偏移)]);
						累加值 += 密钥字;
					}
					uint 累加前 = 累加值;
					uint 混淆值 = 累加值;
					混淆值 ^= 混淆值 >> 25;
					混淆值 += 1394801171U;
					混淆值 ^= 混淆值 << 5;
					混淆值 += 3457341307U;
					混淆值 ^= 混淆值 >> 3;
					混淆值 += 4040842541U;
					混淆值 = 2018246656U + 混淆值;
					累加值 = 累加前 + (uint)混淆值;
					if (块索引 == 块数 - 1 && 尾块字节数 > 0)
					{
						uint 明文字 = 累加值 ^ 密文字;
						for (int l = 0; l < 尾块字节数; l++)
						{
							if (l > 0)
							{
								掩码 <<= 8;
								已移位数 += 8;
							}
							流解密结果[块偏移 + l] = (byte)((明文字 & 掩码) >> 已移位数);
						}
					}
					else
					{
						uint 明文字 = 累加值 ^ 密文字;
						流解密结果[块偏移] = (byte)(明文字 & 255U);
						流解密结果[块偏移 + 1] = (byte)((明文字 & 65280U) >> 8);
						流解密结果[块偏移 + 2] = (byte)((明文字 & 16711680U) >> 16);
						流解密结果[块偏移 + 3] = (byte)((明文字 & 4278190080U) >> 24);
					}
				}
				已解密的字符串缓冲 = 流解密结果;
				if (已解密的字符串缓冲.Length > 0)
				{
					字符串缓存 = new string[已解密的字符串缓冲.Length / 4 + 1];
					缓存索引 = new int[已解密的字符串缓冲.Length / 4 + 1];
				}
				Assembly 宿主程序集 = typeof(Predicate).Assembly;
				if (IntPtr.Size == 4)
				{
					模块基址32 = Marshal.GetHINSTANCE(宿主程序集.GetModules()[0]).ToInt32();
				}
				模块基址64 = Marshal.GetHINSTANCE(宿主程序集.GetModules()[0]).ToInt64();
			}
			int 缓存槽 = 字符串ID / 4;
			if (缓存索引[缓存槽] > 0)
			{
				return 字符串缓存[缓存索引[缓存槽]];
			}
			// 缓冲里存的是"相对模块基址的偏移"，指向映像里的 密文长度 + 密文
			int 表内偏移 = BitConverter.ToInt32(已解密的字符串缓冲, 字符串ID);
			if (本进程句柄 == IntPtr.Zero)
			{
				本进程句柄 = OpenProcess(16U, 1, (uint)Process.GetCurrentProcess().Id);
			}
			byte[] 长度缓冲 = new byte[4];
			if (IntPtr.Size == 4)
			{
				ReadProcessMemory(本进程句柄, new IntPtr(模块基址32 + 表内偏移), 长度缓冲, 4U, out 读写字节数);
			}
			else
			{
				ReadProcessMemory(本进程句柄, new IntPtr(模块基址64 + (long)表内偏移), 长度缓冲, 4U, out 读写字节数);
			}
			int 密文长度 = BitConverter.ToInt32(长度缓冲, 0);
			长度缓冲 = new byte[密文长度];
			if (IntPtr.Size == 4)
			{
				ReadProcessMemory(本进程句柄, new IntPtr(模块基址32 + 表内偏移 + 4), 长度缓冲, Convert.ToUInt32(密文长度), out 读写字节数);
			}
			else
			{
				ReadProcessMemory(本进程句柄, new IntPtr(模块基址64 + (long)表内偏移 + 4L), 长度缓冲, Convert.ToUInt32(密文长度), out 读写字节数);
			}
			byte[] 明文 = AES解密(长度缓冲);
			string 结果 = Encoding.Unicode.GetString(明文, 0, 明文.Length);
			缓存索引[缓存槽] = 缓存计数;
			字符串缓存[缓存计数] = 结果;
			缓存计数++;
			return 结果;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00008728 File Offset: 0x00006928
		/// <summary>
		/// Base64 → Unicode 字符串（原名 VisitWatcher），加壳器给"整条明文"用的快速解码。
		/// </summary>
		internal static string Base64解码为Unicode字符串(string 编码文本)
		{
			byte[] 字节 = Convert.FromBase64String(编码文本);
			return Encoding.Unicode.GetString(字节, 0, 字节.Length);
		}

		// Token: 0x060000BA RID: 186
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr 地址, int 长度, int 新保护属性, ref int 旧保护属性);

		// Token: 0x060000BB RID: 187 RVA: 0x0000874C File Offset: 0x0000874C
		/// <summary>
		/// 解密方法体并回写内存（原名 ConnectWatcher）—— .NET Reactor 的核心运行时。
		///
		/// 流程：
		/// <list type="number">
		/// <item>读内嵌资源 5c5b1961-5059-4bdd-b555-d9e6275d4842；</item>
		/// <item>用 32 字节密钥 + 16 字节 IV（IV 先整体反转，再用公钥标记替换奇数位，
		///   最后密钥前 16 字节与 IV 异或）做 xorshift 流解密；</item>
		/// <item>把解出来的数据当"IL 补丁脚本"：先写一批 (RVA, 值) 的 4 字节补丁，
		///   再按 (节 RVA, 字数) 成批写入方法体；</item>
		/// <item>写入前用 VirtualProtect 把目标页改成可写（PAGE_READWRITE=4），写完再改回原属性。</item>
		/// </list>
		/// 目标始终是本进程（OpenProcess/GetCurrentProcess），不涉及其它进程；
		/// 但"运行期改写自己的代码段"这一行为本身属于自修改代码，安全软件通常会盯。
		/// </summary>
		[Predicate.Indexer(typeof(Predicate.Indexer.Wrapper<object>[]))]
		static void 解密方法体并回写内存()
		{
			if (!方法体已解密)
			{
				方法体已解密 = true;
				BinaryReader 资源读取器 = new BinaryReader(typeof(Predicate).Assembly.GetManifestResourceStream("5c5b1961-5059-4bdd-b555-d9e6275d4842"));
				资源读取器.BaseStream.Position = 0L;
				byte[] 原始资源 = 资源读取器.ReadBytes((int)资源读取器.BaseStream.Length);
				// 32 字节密钥：原版每个下标被重复赋值 4~7 次，只有最后一次有效；
				// 这里只保留最终值（与运行时结果一致），去掉几百行垃圾赋值。
				byte[] 密钥 = new byte[32];
				密钥[0] = 40;
				密钥[1] = 123;
				密钥[2] = 105;
				密钥[3] = 163;
				密钥[4] = 251;
				密钥[5] = 96;
				密钥[6] = 239;
				密钥[7] = 88;
				密钥[8] = 112;
				密钥[9] = 167;
				密钥[10] = 34;
				密钥[11] = 195;
				密钥[12] = 14;
				密钥[13] = 104;
				密钥[14] = 10;
				密钥[15] = 131;
				密钥[16] = 195;
				密钥[17] = 232;
				密钥[18] = 88;
				密钥[19] = 26;
				密钥[20] = 228;
				密钥[21] = 161;
				密钥[22] = 113;
				密钥[23] = 38;
				密钥[24] = 110;
				密钥[25] = 221;
				密钥[26] = 153;
				密钥[27] = 80;
				密钥[28] = 62;
				密钥[29] = 125;
				密钥[30] = 91;
				密钥[31] = 74;
				byte[] 初始向量 = new byte[16];
				初始向量[0] = 98;
				初始向量[1] = 74;
				初始向量[2] = 12;
				初始向量[3] = 23;
				初始向量[4] = 163;
				初始向量[5] = 246;
				初始向量[6] = 159;
				初始向量[7] = 7;
				初始向量[8] = 177;
				初始向量[9] = 22;
				初始向量[10] = 3;
				初始向量[11] = 222;
				初始向量[12] = 80;
				初始向量[13] = 112;
				初始向量[14] = 95;
				初始向量[15] = 28;
				Array.Reverse(初始向量);
				byte[] 公钥标记 = typeof(Predicate).Assembly.GetName().GetPublicKeyToken();
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
					密钥[i] ^= 初始向量[i];
				}
				byte[] 待解数据 = 原始资源;
				int 尾块字节数 = 待解数据.Length % 4;
				int 块数 = 待解数据.Length / 4;
				byte[] 流解密结果 = new byte[待解数据.Length];
				int 密钥块数 = 密钥.Length / 4;
				uint 累加值 = 0U;
				if (尾块字节数 > 0)
				{
					块数++;
				}
				for (int 块索引 = 0; 块索引 < 块数; 块索引++)
				{
					int 密钥块索引 = 块索引 % 密钥块数;
					int 块偏移 = 块索引 * 4;
					uint 密钥内偏移 = (uint)(密钥块索引 * 4);
					uint 密钥字 = (uint)(((int)密钥[(int)((UIntPtr)(密钥内偏移 + 3U))] << 24) | ((int)密钥[(int)((UIntPtr)(密钥内偏移 + 2U))] << 16) | ((int)密钥[(int)((UIntPtr)(密钥内偏移 + 1U))] << 8) | (int)密钥[(int)((UIntPtr)密钥内偏移)]);
					uint 掩码 = 255U;
					int 已移位数 = 0;
					uint 密文字;
					if (块索引 == 块数 - 1 && 尾块字节数 > 0)
					{
						累加值 += 密钥字;
						密文字 = 0U;
						for (int k = 0; k < 尾块字节数; k++)
						{
							if (k > 0)
							{
								密文字 <<= 8;
							}
							密文字 |= (uint)待解数据[待解数据.Length - (1 + k)];
						}
					}
					else
					{
						密钥内偏移 = (uint)块偏移;
						累加值 += 密钥字;
						密文字 = (uint)(((int)待解数据[(int)((UIntPtr)(密钥内偏移 + 3U))] << 24) | ((int)待解数据[(int)((UIntPtr)(密钥内偏移 + 2U))] << 16) | ((int)待解数据[(int)((UIntPtr)(密钥内偏移 + 1U))] << 8) | (int)待解数据[(int)((UIntPtr)密钥内偏移)]);
					}
					// 这句在原版里是 num4 = num4; —— 自赋值，无作用，保留位置的说明而已
					uint 累加前 = 累加值;
					uint 混淆值 = 累加值;
					混淆值 ^= 混淆值 >> 25;
					混淆值 += 1394801171U;
					混淆值 ^= 混淆值 << 5;
					混淆值 += 3457341307U;
					混淆值 ^= 混淆值 >> 3;
					混淆值 += 4040842541U;
					混淆值 = 2018246656U + 混淆值;
					累加值 = 累加前 + (uint)混淆值;
					if (块索引 == 块数 - 1 && 尾块字节数 > 0)
					{
						uint 明文字 = 累加值 ^ 密文字;
						for (int l = 0; l < 尾块字节数; l++)
						{
							if (l > 0)
							{
								掩码 <<= 8;
								已移位数 += 8;
							}
							流解密结果[块偏移 + l] = (byte)((明文字 & 掩码) >> 已移位数);
						}
					}
					else
					{
						uint 明文字 = 累加值 ^ 密文字;
						流解密结果[块偏移] = (byte)(明文字 & 255U);
						流解密结果[块偏移 + 1] = (byte)((明文字 & 65280U) >> 8);
						流解密结果[块偏移 + 2] = (byte)((明文字 & 16711680U) >> 16);
						流解密结果[块偏移 + 3] = (byte)((明文字 & 4278190080U) >> 24);
					}
				}
				byte[] 解密数据 = 流解密结果;
				Array.Clear(初始向量, 0, 初始向量.Length);
				资源读取器.Close();
				资源读取器 = new BinaryReader(new MemoryStream(解密数据));
				资源读取器.BaseStream.Position = 0L;
				IntPtr 本进程句柄2 = IntPtr.Zero;
				Assembly 宿主程序集 = typeof(Predicate).Assembly;
				本进程句柄2 = OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				模块基址64 = Marshal.GetHINSTANCE(宿主程序集.GetModules()[0]).ToInt64();
				IntPtr 写入字节数 = IntPtr.Zero;
				int 旧保护属性 = 0;
				int 补丁条数 = 资源读取器.ReadInt32();
				资源读取器.ReadInt32();
				// 第一批：逐条 4 字节补丁（方法头/跳转桩）
				for (int m = 0; m < 补丁条数; m++)
				{
					IntPtr 目标地址 = new IntPtr(模块基址64 + (long)资源读取器.ReadInt32());
					VirtualProtect(目标地址, 4, 4, ref 旧保护属性);
					if (IntPtr.Size == 4)
					{
						WriteProcessMemory(本进程句柄2, 目标地址, BitConverter.GetBytes(资源读取器.ReadInt32()), 4U, out 写入字节数);
					}
					else
					{
						WriteProcessMemory(本进程句柄2, 目标地址, BitConverter.GetBytes(资源读取器.ReadInt32()), 4U, out 写入字节数);
					}
					VirtualProtect(目标地址, 4, 旧保护属性, ref 旧保护属性);
				}
				try
				{
					// 第二批：按 (节 RVA, 32 位字数) 成批覆盖方法体
					while (资源读取器.BaseStream.Position < 资源读取器.BaseStream.Length - 1L)
					{
						int 节偏移 = 资源读取器.ReadInt32();
						IntPtr 目标地址2 = new IntPtr(模块基址64 + (long)节偏移);
						int 字数 = 资源读取器.ReadInt32();
						VirtualProtect(目标地址2, 字数 * 4, 4, ref 旧保护属性);
						for (int n = 0; n < 字数; n++)
						{
							Marshal.WriteInt32(new IntPtr(目标地址2.ToInt64() + (long)(n * 4)), 资源读取器.ReadInt32());
						}
						VirtualProtect(目标地址2, 字数 * 4, 旧保护属性, ref 旧保护属性);
					}
					CloseHandle(本进程句柄2);
				}
				catch
				{
				}
			}
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000099D0 File Offset: 0x00007BD0
		/// <summary>
		/// 取程序集在磁盘上的路径（原名 ValidateWatcher）：三种办法依次尝试，
		/// 全部失败返回空字符串。三段 try/catch 各自独立吞异常（原始 VB 就是逐条 On Error Resume Next）。
		/// </summary>
		internal static object 获取程序集磁盘路径(Assembly 程序集)
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

		// Token: 0x060000BD RID: 189
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr 进程句柄, IntPtr 目标地址, [In] [Out] byte[] 缓冲, uint 长度, out IntPtr 写入字节数);

		// Token: 0x060000BE RID: 190
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr 进程句柄, IntPtr 源地址, [In] [Out] byte[] 缓冲, uint 长度, out IntPtr 读取字节数);

		// Token: 0x060000BF RID: 191
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint 需要的权限, int 允许继承句柄, uint 进程ID);

		// Token: 0x060000C0 RID: 192
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr 句柄);

		// Token: 0x060000C1 RID: 193 RVA: 0x00009AE0 File Offset: 0x00009B48
		/// <summary>把整个文件读成字节数组（原名 PrepareWatcher）。本文件内没有调用者。</summary>
		private static byte[] 读取整个文件(string 路径)
		{
			byte[] 文件内容;
			using (FileStream 文件流 = new FileStream(路径, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				int 已读 = 0;
				long 文件长度 = 文件流.Length;
				int 剩余 = (int)文件长度;
				文件内容 = new byte[剩余];
				while (剩余 > 0)
				{
					int 本次读 = 文件流.Read(文件内容, 已读, 剩余);
					已读 += 本次读;
					剩余 -= 本次读;
				}
			}
			return 文件内容;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00009B48 File Offset: 0x00009BB8
		/// <summary>
		/// 固定密钥的 AES 解密（原名 CloneWatcher）：用于解密"从本模块映像里读出来的密文"。
		/// Key/IV 是写死的 32/16 字节常量（与 <see cref="自实现MD5"/> 无关）。
		/// </summary>
		private static byte[] AES解密(byte[] 密文)
		{
			MemoryStream 输出流 = new MemoryStream();
			SymmetricAlgorithm 算法 = 创建对称加密算法();
			算法.Key = new byte[]
			{
				83, 142, 41, 134, 139, 74, 128, 31, 250, 136,
				229, 52, 225, 44, 238, 18, 83, 237, 252, 27,
				221, 108, 195, 112, 153, 13, 149, 168, 35, 93,
				87, 221
			};
			算法.IV = new byte[]
			{
				156, 128, 209, 88, 136, 59, 7, 151, 171, 235,
				18, 103, 137, 122, 62, 81
			};
			CryptoStream 解密流 = new CryptoStream(输出流, 算法.CreateDecryptor(), CryptoStreamMode.Write);
			解密流.Write(密文, 0, 密文.Length);
			解密流.Close();
			return 输出流.ToArray();
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00009BB8 File Offset: 0x00009BB8
		/// <summary>加壳器占位方法，恒返回 null，无调用者。</summary>
		private byte[] MoveWatcher()
		{
			return null;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00009BB8 File Offset: 0x00009BB8
		/// <summary>加壳器占位方法，恒返回 null，无调用者。</summary>
		private byte[] InstantiateWatcher()
		{
			return null;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00009BB8 File Offset: 0x00009BB8
		/// <summary>加壳器占位方法，恒返回 null，无调用者。</summary>
		private byte[] CompareWatcher()
		{
			return null;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00009BB8 File Offset: 0x00009BB8
		/// <summary>加壳器占位方法，恒返回 null，无调用者。</summary>
		private byte[] DestroyWatcher()
		{
			return null;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00009BB8 File Offset: 0x00009BB8
		/// <summary>加壳器占位方法，恒返回 null，无调用者。</summary>
		private byte[] ViewWatcher()
		{
			return null;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00009BB8 File Offset: 0x00009BB8
		/// <summary>加壳器占位方法，恒返回 null，无调用者。</summary>
		private byte[] ExcludeWatcher()
		{
			return null;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00009BC8 File Offset: 0x00009BC8
		/// <summary>加壳器生成的假方法：带着 {11111-22222-…} 字面量、两个分支返回同一个值，无调用者。</summary>
		internal byte[] ReflectWatcher()
		{
			string 假标记 = "{11111-22222-40001-00001}";
			if (假标记.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00009C08 File Offset: 0x00009C08
		/// <summary>加壳器生成的假方法（第二个假标记），无调用者。</summary>
		internal byte[] CustomizeWatcher()
		{
			string 假标记 = "{11111-22222-40001-00002}";
			if (假标记.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00009C48 File Offset: 0x00009C48
		/// <summary>加壳器生成的假方法（第三个假标记），无调用者。</summary>
		internal byte[] InterruptWatcher()
		{
			string 假标记 = "{11111-22222-50001-00001}";
			if (假标记.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00009C88 File Offset: 0x00009C88
		/// <summary>加壳器生成的假方法（第四个假标记），无调用者。</summary>
		internal byte[] RestartWatcher()
		{
			string 假标记 = "{11111-22222-50001-00002}";
			if (假标记.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00009CC8 File Offset: 0x00009CC8
		/// <summary>加壳器生成的假方法（第五个假标记），无调用者。</summary>
		internal byte[] ReadWatcher()
		{
			string 假标记 = "{11111-22222-60001-00001}";
			if (假标记.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00009D08 File Offset: 0x00009D08
		/// <summary>加壳器生成的假方法（第六个假标记），无调用者。</summary>
		internal byte[] CancelWatcher()
		{
			string 假标记 = "{11111-22222-60001-00002}";
			if (假标记.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00009D48 File Offset: 0x00009D48
		/// <summary>
		/// 对称加密成 Base64（原名 ComputeWatcher）：Key 写死 32 字节，
		/// IV = MD5(Unicode(第二参数))，明文 = Unicode(第一参数)。本文件内没有调用者。
		/// </summary>
		internal static string AES加密为Base64(string 明文, string 初始向量种子)
		{
			byte[] 明文字节 = Encoding.Unicode.GetBytes(明文);
			byte[] 明文字节副本 = 明文字节;
			byte[] 密钥 = new byte[]
			{
				82, 102, 104, 110, 32, 77, 24, 34, 118, 181,
				51, 17, 18, 51, 12, 109, 10, 32, 77, 24,
				34, 158, 161, 41, 97, 28, 118, 181, 5, 25,
				1, 88
			};
			byte[] 初始向量 = 计算MD5(Encoding.Unicode.GetBytes(初始向量种子));
			MemoryStream 输出流 = new MemoryStream();
			SymmetricAlgorithm 算法 = 创建对称加密算法();
			算法.Key = 密钥;
			算法.IV = 初始向量;
			CryptoStream 加密流 = new CryptoStream(输出流, 算法.CreateEncryptor(), CryptoStreamMode.Write);
			加密流.Write(明文字节副本, 0, 明文字节副本.Length);
			加密流.Close();
			return Convert.ToBase64String(输出流.ToArray());
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>加壳器生成的授权标记桩函数，恒返回 true，无调用者。</summary>
		internal static bool RevertTag()
		{
			return true;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>加壳器生成的授权标记桩函数，恒返回 false，无调用者。</summary>
		internal static bool SearchParser()
		{
			return false;
		}

		// Token: 0x04000058 RID: 88
		/// <summary>MD5 轮常量表（sin 表 × 2^32 取整，64 项）。原名 _Algo。</summary>
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

		// Token: 0x04000059 RID: 89
		/// <summary>FIPS 配置是否已读取过。原名 m_Listener。</summary>
		private static bool 已读取FIPS配置 = false;

		// Token: 0x0400005A RID: 90
		/// <summary>解密后的字符串表/方法体数据缓冲。原名 m_Database。</summary>
		private static byte[] 已解密的字符串缓冲 = new byte[0];

		// Token: 0x0400005B RID: 91
		/// <summary>"字符串ID / 4" → 缓存槽下标。原名 m_Item。</summary>
		private static int[] 缓存索引 = new int[0];

		// Token: 0x0400005C RID: 92
		/// <summary>本模块基址（64 位视角）。原名 m_Comparator。</summary>
		private static long 模块基址64 = 0L;

		// Token: 0x0400005D RID: 93
		/// <summary>本进程句柄（ReadProcessMemory 用）。原名 m_Thread。</summary>
		private static IntPtr 本进程句柄 = IntPtr.Zero;

		// Token: 0x0400005E RID: 94
		/// <summary>方法体是否已解密并回写。原名 _Field。</summary>
		private static bool 方法体已解密 = false;

		// Token: 0x0400005F RID: 95
		/// <summary>CryptoConfig.AllowOnlyFipsAlgorithms 的取值。原名 context。</summary>
		private static bool 仅允许FIPS算法 = false;

		// Token: 0x04000060 RID: 96
		/// <summary>已解出的字符串（按槽位缓存）。原名 specification。</summary>
		private static string[] 字符串缓存 = new string[0];

		// Token: 0x04000061 RID: 97
		/// <summary>本模块基址（32 位视角）。原名 _Service。</summary>
		private static int 模块基址32 = 0;

		// Token: 0x04000062 RID: 98
		/// <summary>未使用的空白缓冲（加壳器残留）。原名 _Task。</summary>
		private static byte[] _Task = new byte[0];

		// Token: 0x04000063 RID: 99
		/// <summary>字符串缓存写指针，从 1 开始。原名 connection。</summary>
		private static int 缓存计数 = 1;

		// Token: 0x04000064 RID: 100
		/// <summary>未使用的空白缓冲（加壳器残留）。原名 m_Page。</summary>
		private static byte[] m_Page = new byte[0];

		// Token: 0x04000065 RID: 101
		/// <summary>ReadProcessMemory / WriteProcessMemory 的 out 参数占位。原名 m_Dispatcher。</summary>
		private static IntPtr 读写字节数 = IntPtr.Zero;

		// Token: 0x04000066 RID: 102
		/// <summary>未使用的空白缓冲（加壳器残留）。原名 facade。</summary>
		private static byte[] facade = new byte[0];

		// Token: 0x02000016 RID: 22
		/// <summary>加壳器自用的标记特性（原名 Indexer），无实际逻辑，只是让 IL 里带上类型引用。</summary>
		internal class Indexer : Attribute
		{
			// Token: 0x060000D3 RID: 211 RVA: 0x00002668 File Offset: 0x00002668
			[Predicate.Indexer(typeof(Predicate.Indexer.Wrapper<object>[]))]
			public Indexer(object 标记参数)
			{
			}

			// Token: 0x02000017 RID: 23
			/// <summary>加壳器泛型标记类型（原名 Wrapper），只用来构造 typeof(...[]) 参数。</summary>
			internal class Wrapper<T>
			{
				// Token: 0x060000D5 RID: 213 RVA: 0x000021DD File Offset: 0x000003DD
				/// <summary>加壳器授权标记桩函数，恒返回 true，无调用者。</summary>
				internal static bool ViewTag()
				{
					return true;
				}

				// Token: 0x060000D6 RID: 214 RVA: 0x000021E8 File Offset: 0x000003E8
				/// <summary>加壳器授权标记桩函数，恒返回 false，无调用者。</summary>
				internal static bool LoginTag()
				{
					return false;
				}
			}
		}

		// Token: 0x02000018 RID: 24
		/// <summary>空枚举（原名 AuthFlags），加壳器残留，无成员。</summary>
		[Flags]
		private enum AuthFlags
		{

		}
	}
}
