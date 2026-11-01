using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace zmserv.Internal
{
	// Token: 0x0200000D RID: 13
	/// <summary>
	/// IntelliLock（EZIRIZ）加壳器的运行时保护代码。整类不是业务逻辑，而是"壳"，
	/// 但它是理解本样本的关键，因为字符串加密、内嵌 DLL、运行时代码还原全在这里。
	/// 真实职责：
	/// 1) 加解密工具箱：BCL 不可用（FIPS 模式）时用自实现 MD5，否则用 MD5CryptoServiceProvider；
	///    对称算法按同样规则选 AesCryptoServiceProvider / RijndaelManaged；
	/// 2) 字符串解密（解密字符串）：从嵌入资源 "9f3465d1-7736-405a-9581-d615ccfda07a"
	///    取密文，AES-CBC 解一层，再用"累加 + 异或 + 移位"的自制流密码解第二层，
	///    得到一张字符串表；之后按偏移读表：先从【本进程模块内存】取长度与密文
	///    （ReadProcessMemory 读自己，用来对抗内存转储），再用固定 Key/IV 的 AES 解一次；
	/// 3) 代码还原（运行时还原方法体）：从资源 "08d03c1e-656e-4b82-b230-c3549f0c2ec9"
	///    取 IL 补丁，解密后通过 VirtualProtect + WriteProcessMemory 把方法体写回本进程，
	///    这是"代码加密"在运行时被还原的地方；
	/// 4) 授权/杂项残留：一堆返回 {1,2} 或 null 的空桩、一个返回本程序集路径的工具方法。
	/// 注意：下面大量 "byte[] xxx = new byte[N] { ... }" 数组，原版对同一个下标连续写了
	/// 4~7 次（加壳器的诱饵写入，只有最后一次生效）。这里按"最终生效值"列出，顺序与下标一致，
	/// 数组内容与原版完全等价。
	/// </summary>
	internal class IntelliLock运行时
	{
		// Token: 0x06000035 RID: 53 RVA: 0x000022EC File Offset: 0x000004EC
		/// <summary>空方法体（一个从未被调用的私有实例方法，壳的残留）。原名 method_0。</summary>
		private void 空方法体()
		{
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000028E4 File Offset: 0x00000AE4
		/// <summary>
		/// 自实现的 MD5（RFC 1321 逐句对应）。只在 FIPS 模式下的 计算MD5 里被调用。
		/// 阶段：补位（0x80 + 补零 + 64 位小端原始比特长度）→ 分块 → 每块 4 轮 64 步 →
		/// 累加回初始 IV（1732584193/4023233417/2562383102/271733878）→ 4 个小端 uint 拼成 16 字节摘要。
		/// 注意循环里 "< 61U; += 4U" 是原版的写法（等于遍历 16 个字），常量不要改。
		/// </summary>
		internal static byte[] 自实现MD5(byte[] 消息)
		{
			uint[] 消息字 = new uint[16];
			int 补位差 = 448 - 消息.Length * 8 % 512;
			uint 补位字节数 = (uint)((补位差 + 512) % 512);
			if (补位字节数 == 0U)
			{
				补位字节数 = 512U;
			}
			uint 总字节数 = (uint)((long)消息.Length + (long)((ulong)(补位字节数 / 8U)) + 8L);
			ulong 原始比特长度 = (ulong)((long)消息.Length * 8L);
			byte[] 填充数据 = new byte[总字节数];
			for (int i = 0; i < 消息.Length; i++)
			{
				填充数据[i] = 消息[i];
			}
			// 原版先取一个别名再写 0x80（别名只是反编译噪声，写的是同一个数组）
			填充数据[消息.Length] |= 128;
			// 末尾 8 字节写原始比特长度：j=8 写最低字节、j=1 写最高字节（小端），写法与原版一致
			for (int j = 8; j > 0; j--)
			{
				填充数据[(int)(总字节数 - (ulong)j)] = (byte)((原始比特长度 >> (8 - j) * 8) & 255UL);
			}
			uint 字数 = (uint)(填充数据.Length * 8 / 32);
			uint 状态A = 1732584193U;
			uint 状态B = 4023233417U;
			uint 状态C = 2562383102U;
			uint 状态D = 271733878U;
			for (uint 块序号 = 0U; 块序号 < 字数 / 16U; 块序号 += 1U)
			{
				uint 块字节偏移 = 块序号 << 6;
				// 以 4 字节为步长把 16 个字按小端拼成 uint（0,4,...,60 共 16 次）
				for (uint 字偏移 = 0U; 字偏移 < 61U; 字偏移 += 4U)
				{
					消息字[(int)(字偏移 >> 2)] = (uint)(((int)填充数据[(int)(块字节偏移 + (字偏移 + 3U))] << 24) | ((int)填充数据[(int)(块字节偏移 + (字偏移 + 2U))] << 16) | ((int)填充数据[(int)(块字节偏移 + (字偏移 + 1U))] << 8) | (int)填充数据[(int)(块字节偏移 + 字偏移)]);
				}
				uint 初值A = 状态A;
				uint 初值B = 状态B;
				uint 初值C = 状态C;
				uint 初值D = 状态D;
				// 第 1 轮：F(b,c,d) = (b & c) | (~b & d)
				IntelliLock运行时.MD5轮函数F(ref 状态A, 状态B, 状态C, 状态D, 0U, 7, 1U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态D, 状态A, 状态B, 状态C, 1U, 12, 2U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态C, 状态D, 状态A, 状态B, 2U, 17, 3U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态B, 状态C, 状态D, 状态A, 3U, 22, 4U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态A, 状态B, 状态C, 状态D, 4U, 7, 5U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态D, 状态A, 状态B, 状态C, 5U, 12, 6U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态C, 状态D, 状态A, 状态B, 6U, 17, 7U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态B, 状态C, 状态D, 状态A, 7U, 22, 8U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态A, 状态B, 状态C, 状态D, 8U, 7, 9U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态D, 状态A, 状态B, 状态C, 9U, 12, 10U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态C, 状态D, 状态A, 状态B, 10U, 17, 11U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态B, 状态C, 状态D, 状态A, 11U, 22, 12U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态A, 状态B, 状态C, 状态D, 12U, 7, 13U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态D, 状态A, 状态B, 状态C, 13U, 12, 14U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态C, 状态D, 状态A, 状态B, 14U, 17, 15U, 消息字);
				IntelliLock运行时.MD5轮函数F(ref 状态B, 状态C, 状态D, 状态A, 15U, 22, 16U, 消息字);
				// 第 2 轮：G(b,c,d) = (b & d) | (c & ~d)
				IntelliLock运行时.MD5轮函数G(ref 状态A, 状态B, 状态C, 状态D, 1U, 5, 17U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态D, 状态A, 状态B, 状态C, 6U, 9, 18U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态C, 状态D, 状态A, 状态B, 11U, 14, 19U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态B, 状态C, 状态D, 状态A, 0U, 20, 20U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态A, 状态B, 状态C, 状态D, 5U, 5, 21U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态D, 状态A, 状态B, 状态C, 10U, 9, 22U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态C, 状态D, 状态A, 状态B, 15U, 14, 23U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态B, 状态C, 状态D, 状态A, 4U, 20, 24U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态A, 状态B, 状态C, 状态D, 9U, 5, 25U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态D, 状态A, 状态B, 状态C, 14U, 9, 26U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态C, 状态D, 状态A, 状态B, 3U, 14, 27U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态B, 状态C, 状态D, 状态A, 8U, 20, 28U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态A, 状态B, 状态C, 状态D, 13U, 5, 29U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态D, 状态A, 状态B, 状态C, 2U, 9, 30U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态C, 状态D, 状态A, 状态B, 7U, 14, 31U, 消息字);
				IntelliLock运行时.MD5轮函数G(ref 状态B, 状态C, 状态D, 状态A, 12U, 20, 32U, 消息字);
				// 第 3 轮：H(b,c,d) = b ^ c ^ d
				IntelliLock运行时.MD5轮函数H(ref 状态A, 状态B, 状态C, 状态D, 5U, 4, 33U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态D, 状态A, 状态B, 状态C, 8U, 11, 34U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态C, 状态D, 状态A, 状态B, 11U, 16, 35U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态B, 状态C, 状态D, 状态A, 14U, 23, 36U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态A, 状态B, 状态C, 状态D, 1U, 4, 37U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态D, 状态A, 状态B, 状态C, 4U, 11, 38U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态C, 状态D, 状态A, 状态B, 7U, 16, 39U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态B, 状态C, 状态D, 状态A, 10U, 23, 40U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态A, 状态B, 状态C, 状态D, 13U, 4, 41U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态D, 状态A, 状态B, 状态C, 0U, 11, 42U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态C, 状态D, 状态A, 状态B, 3U, 16, 43U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态B, 状态C, 状态D, 状态A, 6U, 23, 44U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态A, 状态B, 状态C, 状态D, 9U, 4, 45U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态D, 状态A, 状态B, 状态C, 12U, 11, 46U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态C, 状态D, 状态A, 状态B, 15U, 16, 47U, 消息字);
				IntelliLock运行时.MD5轮函数H(ref 状态B, 状态C, 状态D, 状态A, 2U, 23, 48U, 消息字);
				// 第 4 轮：I(b,c,d) = c ^ (b | ~d)
				IntelliLock运行时.MD5轮函数I(ref 状态A, 状态B, 状态C, 状态D, 0U, 6, 49U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态D, 状态A, 状态B, 状态C, 7U, 10, 50U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态C, 状态D, 状态A, 状态B, 14U, 15, 51U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态B, 状态C, 状态D, 状态A, 5U, 21, 52U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态A, 状态B, 状态C, 状态D, 12U, 6, 53U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态D, 状态A, 状态B, 状态C, 3U, 10, 54U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态C, 状态D, 状态A, 状态B, 10U, 15, 55U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态B, 状态C, 状态D, 状态A, 1U, 21, 56U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态A, 状态B, 状态C, 状态D, 8U, 6, 57U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态D, 状态A, 状态B, 状态C, 15U, 10, 58U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态C, 状态D, 状态A, 状态B, 6U, 15, 59U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态B, 状态C, 状态D, 状态A, 13U, 21, 60U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态A, 状态B, 状态C, 状态D, 4U, 6, 61U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态D, 状态A, 状态B, 状态C, 11U, 10, 62U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态C, 状态D, 状态A, 状态B, 2U, 15, 63U, 消息字);
				IntelliLock运行时.MD5轮函数I(ref 状态B, 状态C, 状态D, 状态A, 9U, 21, 64U, 消息字);
				状态A += 初值A;
				状态B += 初值B;
				状态C += 初值C;
				状态D += 初值D;
			}
			byte[] 摘要 = new byte[16];
			Array.Copy(BitConverter.GetBytes(状态A), 0, 摘要, 0, 4);
			Array.Copy(BitConverter.GetBytes(状态B), 0, 摘要, 4, 4);
			Array.Copy(BitConverter.GetBytes(状态C), 0, 摘要, 8, 4);
			Array.Copy(BitConverter.GetBytes(状态D), 0, 摘要, 12, 4);
			return 摘要;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000022EE File Offset: 0x000004EE
		/// <summary>
		/// MD5 第 1 轮函数 F：(b &amp; c) | (~b &amp; d)。
		/// 参数顺序照原版：先 ref 待更新状态，再 b/c/d、消息字下标、循环左移位数、常量表下标（1 基）。
		/// </summary>
		private static void MD5轮函数F(ref uint 当前值, uint b, uint c, uint d, uint 消息下标, ushort 循环位数, uint 常量下标, uint[] 消息字)
		{
			当前值 = b + IntelliLock运行时.循环左移(当前值 + ((b & c) | (~b & d)) + 消息字[(int)消息下标] + IntelliLock运行时.MD5常量表[(int)(常量下标 - 1U)], 循环位数);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002319 File Offset: 0x00000519
		/// <summary>
		/// MD5 第 2 轮函数 G：(b &amp; d) | (c &amp; ~d)。参数含义同 F。
		/// </summary>
		private static void MD5轮函数G(ref uint 当前值, uint b, uint c, uint d, uint 消息下标, ushort 循环位数, uint 常量下标, uint[] 消息字)
		{
			当前值 = b + IntelliLock运行时.循环左移(当前值 + ((b & d) | (c & ~d)) + 消息字[(int)消息下标] + IntelliLock运行时.MD5常量表[(int)(常量下标 - 1U)], 循环位数);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002344 File Offset: 0x00000544
		/// <summary>
		/// MD5 第 3 轮函数 H：b ^ c ^ d。参数含义同 F。
		/// </summary>
		private static void MD5轮函数H(ref uint 当前值, uint b, uint c, uint d, uint 消息下标, ushort 循环位数, uint 常量下标, uint[] 消息字)
		{
			当前值 = b + IntelliLock运行时.循环左移(当前值 + (b ^ c ^ d) + 消息字[(int)消息下标] + IntelliLock运行时.MD5常量表[(int)(常量下标 - 1U)], 循环位数);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000236C File Offset: 0x0000056C
		/// <summary>
		/// MD5 第 4 轮函数 I：c ^ (b | ~d)。参数含义同 F。
		/// </summary>
		private static void MD5轮函数I(ref uint 当前值, uint b, uint c, uint d, uint 消息下标, ushort 循环位数, uint 常量下标, uint[] 消息字)
		{
			当前值 = b + IntelliLock运行时.循环左移(当前值 + (c ^ (b | ~d)) + 消息字[(int)消息下标] + IntelliLock运行时.MD5常量表[(int)(常量下标 - 1U)], 循环位数);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002395 File Offset: 0x00000595
		/// <summary>
		/// 32 位循环左移（原名 EnableParams）。位数来自 MD5 的步长表，恒在 1..23 之间。
		/// </summary>
		private static uint 循环左移(uint 值, ushort 位数)
		{
			return (值 >> (int)(32 - 位数)) | (值 << (int)位数);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000023A7 File Offset: 0x000005A7
		/// <summary>
		/// 系统是否「只允许 FIPS 认证的算法」（惰性求值一次，结果缓存在 FIPS开关值）。
		/// 注意这个返回值的方向：true 表示系统处于 FIPS 限制模式，
		/// 于是调用方改用 BCL 的 AesCryptoServiceProvider / MD5CryptoServiceProvider；
		/// false 时才用自实现 MD5 和 RijndaelManaged。
		/// </summary>
		internal static bool 是否仅允许FIPS算法()
		{
			if (!IntelliLock运行时.已检查FIPS)
			{
				IntelliLock运行时.读取FIPS限制开关();
				IntelliLock运行时.已检查FIPS = true;
			}
			return IntelliLock运行时.FIPS开关值;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002F88 File Offset: 0x00001188
		/// <summary>
		/// 用反射读 System.Security.Cryptography.CryptoConfig.AllowOnlyFipsAlgorithms
		/// （故意走反射而不是直接引用，避免编译期依赖）。
		/// 反射失败时吞掉异常，FIPS开关值 保持 false——try/catch 是原件，不能删。
		/// </summary>
		internal static void 读取FIPS限制开关()
		{
			try
			{
				IntelliLock运行时.FIPS开关值 = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002FE8 File Offset: 0x000011E8
		/// <summary>
		/// 按当前加密策略创建对称算法：FIPS 模式优先 AesCryptoServiceProvider（建不出来就退回
		/// RijndaelManaged），非 FIPS 模式直接用 RijndaelManaged。
		/// </summary>
		internal static SymmetricAlgorithm 创建对称加密算法()
		{
			SymmetricAlgorithm 算法 = null;
			if (IntelliLock运行时.是否仅允许FIPS算法())
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

		// Token: 0x0600003F RID: 63 RVA: 0x000023C0 File Offset: 0x000004C0
		/// <summary>
		/// 统一的 MD5 入口（原名 RateParams）：FIPS 模式用 BCL 的实现，否则用自己的 自实现MD5。
		/// 内嵌程序集加载器就是用它算资源名的。
		/// </summary>
		internal static byte[] 计算MD5(byte[] 数据)
		{
			if (!IntelliLock运行时.是否仅允许FIPS算法())
			{
				return new MD5CryptoServiceProvider().ComputeHash(数据);
			}
			return IntelliLock运行时.自实现MD5(数据);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003028 File Offset: 0x00001228
		/// <summary>
		/// 按字符串表偏移取一个被加密的字符串（IntelliLock 字符串加密的核心）。
		/// 首次进入（字符串表为空）时先建表：
		/// 1) 读嵌入资源 "9f3465d1-7736-405a-9581-d615ccfda07a" 的全部字节；
		/// 2) 准备 32 字节 AES 密钥素材与 16 字节 IV 素材，把本程序集公钥标记的 8 个字节
		///    写进 IV 的奇数位（[1][3][5]...[15]），再让密钥逐字节异或 IV；
		/// 3) 偏移 == -1 时：AES-CBC 解密资源 → 得到字符串表原始字节（同时分配缓存数组）；
		/// 4) 无论偏移是不是 -1，都再跑一遍自制流密码：按 4 字节一组，用密钥算"密钥字"，
		///    用 累加器 = 累加器 + 密钥字 + 混沌(累加器) 生成掩码，与 4 字节数据字异或后写回；
		///    最后一组不足 4 字节时按从尾往前的字节拼字、再逐字节写回；
		/// 5) 记下模块在内存中的基址（32/64 位各存一份）。
		/// 之后每次调用：先查 偏移到缓存槽，命中直接返回；未命中就
		/// 6) 从字符串表读 4 字节"表内偏移"，用 ReadProcessMemory 从【本进程模块内存】
		///    读长度 + 密文字节（读自己是为了让静态转储拿不到明文）；
		/// 7) AES解密（固定 Key/IV）→ Unicode 字符串 → 存进缓存 → 返回。
		/// </summary>
		static string 解密字符串(int 字符串表偏移)
		{
			if (IntelliLock运行时.字符串表.Length == 0)
			{
				BinaryReader 资源读取器 = new BinaryReader(typeof(IntelliLock运行时).Assembly.GetManifestResourceStream("9f3465d1-7736-405a-9581-d615ccfda07a"));
				资源读取器.BaseStream.Position = 0L;
				byte[] 表字节 = 资源读取器.ReadBytes((int)资源读取器.BaseStream.Length);
				// 32 字节 AES 密钥素材（原版每个下标写了 4~7 次，只保留最终生效值）
				byte[] 密钥素材 = new byte[32]
				{
					187, 155, 14, 206, 64, 218, 4, 187, 188, 111, 114, 135, 161, 59, 18, 64,
					86, 109, 242, 248, 201, 19, 240, 40, 243, 162, 96, 115, 144, 60, 174, 71
				};
				byte[] 密钥 = 密钥素材;
				// 16 字节 IV 素材（同样只保留最终生效值）
				byte[] IV素材 = new byte[16]
				{
					1, 159, 234, 155, 181, 140, 150, 141, 247, 35, 193, 170, 77, 110, 196, 73
				};
				byte[] 初始化向量 = IV素材;
				byte[] 公钥标记 = typeof(IntelliLock运行时).Assembly.GetName().GetPublicKeyToken();
				if (公钥标记 != null && 公钥标记.Length > 0)
				{
					初始化向量[1] = 公钥标记[0];
					初始化向量[3] = 公钥标记[1];
					初始化向量[5] = 公钥标记[2];
					初始化向量[7] = 公钥标记[3];
					初始化向量[9] = 公钥标记[4];
					初始化向量[11] = 公钥标记[5];
					初始化向量[13] = 公钥标记[6];
					初始化向量[15] = 公钥标记[7];
				}
				for (int i = 0; i < 初始化向量.Length; i++)
				{
					密钥[i] ^= 初始化向量[i];
				}
				if (字符串表偏移 == -1)
				{
					SymmetricAlgorithm 算法 = IntelliLock运行时.创建对称加密算法();
					算法.Mode = CipherMode.CBC;
					ICryptoTransform 解密器 = 算法.CreateDecryptor(密钥, 初始化向量);
					MemoryStream 内存流 = new MemoryStream();
					CryptoStream 解密流 = new CryptoStream(内存流, 解密器, CryptoStreamMode.Write);
					解密流.Write(表字节, 0, 表字节.Length);
					解密流.FlushFinalBlock();
					IntelliLock运行时.字符串表 = 内存流.ToArray();
					if (IntelliLock运行时.字符串表.Length > 0)
					{
						IntelliLock运行时.字符串缓存 = new string[IntelliLock运行时.字符串表.Length / 4 + 1];
						IntelliLock运行时.偏移到缓存槽 = new int[IntelliLock运行时.字符串表.Length / 4 + 1];
					}
					内存流.Close();
					解密流.Close();
					资源读取器.Close();
					表字节 = IntelliLock运行时.字符串表;
				}
				int 余数字节 = 表字节.Length % 4;
				int 完整字数 = 表字节.Length / 4;
				byte[] 解密输出 = new byte[表字节.Length];
				int 密钥字数 = 密钥.Length / 4;
				uint 累加器 = 0U;
				if (余数字节 > 0)
				{
					完整字数++;
				}
				for (int 序号 = 0; 序号 < 完整字数; 序号++)
				{
					int 密钥字序号 = 序号 % 密钥字数;
					int 输出偏移 = 序号 * 4;
					uint 临时偏移 = (uint)(密钥字序号 * 4);
					uint 密钥字 = (uint)(((int)密钥[(int)(临时偏移 + 3U)] << 24) | ((int)密钥[(int)(临时偏移 + 2U)] << 16) | ((int)密钥[(int)(临时偏移 + 1U)] << 8) | (int)密钥[(int)临时偏移]);
					uint 尾字节掩码 = 255U;
					int 尾字节移位 = 0;
					uint 数据字;
					if (序号 == 完整字数 - 1 && 余数字节 > 0)
					{
						// 最后一组不足 4 字节：从末尾往前拼，先拼尾字再累加密钥字（顺序同原版）
						数据字 = 0U;
						for (int 尾字节序号 = 0; 尾字节序号 < 余数字节; 尾字节序号++)
						{
							if (尾字节序号 > 0)
							{
								数据字 <<= 8;
							}
							数据字 |= (uint)表字节[表字节.Length - (1 + 尾字节序号)];
						}
						累加器 += 密钥字;
					}
					else
					{
						临时偏移 = (uint)输出偏移;
						数据字 = (uint)(((int)表字节[(int)(临时偏移 + 3U)] << 24) | ((int)表字节[(int)(临时偏移 + 2U)] << 16) | ((int)表字节[(int)(临时偏移 + 1U)] << 8) | (int)表字节[(int)临时偏移]);
						累加器 += 密钥字;
					}
					uint 累加前值 = 累加器;
					uint 混沌值 = 累加器;
					混沌值 ^= 混沌值 >> 25;
					混沌值 += 1394801171U;
					混沌值 ^= 混沌值 << 5;
					混沌值 += 3457341307U;
					混沌值 ^= 混沌值 >> 3;
					混沌值 += 4040842541U;
					混沌值 = 2018246656U + 混沌值;
					累加器 = 累加前值 + (uint)混沌值;
					if (序号 == 完整字数 - 1 && 余数字节 > 0)
					{
						uint 尾字异或结果 = 累加器 ^ 数据字;
						for (int 尾字节写序号 = 0; 尾字节写序号 < 余数字节; 尾字节写序号++)
						{
							if (尾字节写序号 > 0)
							{
								尾字节掩码 <<= 8;
								尾字节移位 += 8;
							}
							解密输出[输出偏移 + 尾字节写序号] = (byte)((尾字异或结果 & 尾字节掩码) >> 尾字节移位);
						}
					}
					else
					{
						uint 字异或结果 = 累加器 ^ 数据字;
						解密输出[输出偏移] = (byte)(字异或结果 & 255U);
						解密输出[输出偏移 + 1] = (byte)((字异或结果 & 65280U) >> 8);
						解密输出[输出偏移 + 2] = (byte)((字异或结果 & 16711680U) >> 16);
						解密输出[输出偏移 + 3] = (byte)((字异或结果 & 4278190080U) >> 24);
					}
				}
				IntelliLock运行时.字符串表 = 解密输出;
				if (IntelliLock运行时.字符串表.Length > 0)
				{
					IntelliLock运行时.字符串缓存 = new string[IntelliLock运行时.字符串表.Length / 4 + 1];
					IntelliLock运行时.偏移到缓存槽 = new int[IntelliLock运行时.字符串表.Length / 4 + 1];
				}
				Assembly 本程序集 = typeof(IntelliLock运行时).Assembly;
				if (IntPtr.Size == 4)
				{
					IntelliLock运行时.模块基址32位 = Marshal.GetHINSTANCE(本程序集.GetModules()[0]).ToInt32();
				}
				IntelliLock运行时.模块基址 = Marshal.GetHINSTANCE(本程序集.GetModules()[0]).ToInt64();
			}
			int 槽位 = 字符串表偏移 / 4;
			if (IntelliLock运行时.偏移到缓存槽[槽位] > 0)
			{
				return IntelliLock运行时.字符串缓存[IntelliLock运行时.偏移到缓存槽[槽位]];
			}
			int 表内偏移 = BitConverter.ToInt32(IntelliLock运行时.字符串表, 字符串表偏移);
			if (IntelliLock运行时.本进程句柄 == IntPtr.Zero)
			{
				IntelliLock运行时.本进程句柄 = IntelliLock运行时.OpenProcess(16U, 1, (uint)Process.GetCurrentProcess().Id);
			}
			byte[] 缓冲区 = new byte[4];
			if (IntPtr.Size == 4)
			{
				IntelliLock运行时.ReadProcessMemory(IntelliLock运行时.本进程句柄, new IntPtr(IntelliLock运行时.模块基址32位 + 表内偏移), 缓冲区, 4U, out IntelliLock运行时.已读字节数);
			}
			else
			{
				IntelliLock运行时.ReadProcessMemory(IntelliLock运行时.本进程句柄, new IntPtr(IntelliLock运行时.模块基址 + (long)表内偏移), 缓冲区, 4U, out IntelliLock运行时.已读字节数);
			}
			int 字符串字节数 = BitConverter.ToInt32(缓冲区, 0);
			缓冲区 = new byte[字符串字节数];
			if (IntPtr.Size == 4)
			{
				IntelliLock运行时.ReadProcessMemory(IntelliLock运行时.本进程句柄, new IntPtr(IntelliLock运行时.模块基址32位 + 表内偏移 + 4), 缓冲区, Convert.ToUInt32(字符串字节数), out IntelliLock运行时.已读字节数);
			}
			else
			{
				IntelliLock运行时.ReadProcessMemory(IntelliLock运行时.本进程句柄, new IntPtr(IntelliLock运行时.模块基址 + (long)表内偏移 + 4L), 缓冲区, Convert.ToUInt32(字符串字节数), out IntelliLock运行时.已读字节数);
			}
			byte[] 明文字节 = IntelliLock运行时.AES解密(缓冲区);
			string 结果 = Encoding.Unicode.GetString(明文字节, 0, 明文字节.Length);
			IntelliLock运行时.偏移到缓存槽[槽位] = IntelliLock运行时.下一个缓存槽位;
			IntelliLock运行时.字符串缓存[IntelliLock运行时.下一个缓存槽位] = 结果;
			IntelliLock运行时.下一个缓存槽位++;
			return 结果;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004184 File Offset: 0x00002384
		/// <summary>
		/// Base64 文本 → Unicode 字符串（原名 CompareParams）。与 AES加密为Base64 互逆，
		/// 走的是"先转字节再按 Unicode 解"，不检查多余字节。
		/// </summary>
		internal static string Base64转Unicode字符串(string 内容)
		{
			byte[] 字节 = Convert.FromBase64String(内容);
			return Encoding.Unicode.GetString(字节, 0, 字节.Length);
		}

		// Token: 0x06000042 RID: 66
		/// <summary>
		/// kernel32!VirtualProtect：改一段内存的保护属性，返回原保护值到 out 参数。
		/// 这里是给"往自己模块里写 IL"临时开写权限用的。
		/// </summary>
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr 地址, int 长度, int 新保护, ref int 旧保护);

		// Token: 0x06000043 RID: 67 RVA: 0x000041A8 File Offset: 0x000023A8
		/// <summary>
		/// 代码还原入口（IntelliLock 的模块初始化器，原属性标记为模块初始化点）。
		/// 真实职责：把程序集里"加密的 IL 补丁"写回本进程内存，让被加密的方法体恢复可执行。
		/// 步骤：
		/// 1) 读嵌入资源 "08d03c1e-656e-4b82-b230-c3549f0c2ec9"；
		/// 2) 准备 32 字节密钥、16 字节 IV 素材 → Array.Reverse(IV) → 用公钥标记覆盖 IV 奇数位
		///    （覆盖后 Array.Clear 掉公钥标记）→ 密钥异或 IV；
		/// 3) 用同一套"累加 + 混沌 + 异或"流密码把资源解成补丁字节流；
		/// 4) 把补丁流当成二进制脚本：先读"条目数 + 一个保留字段"，每条写 4 字节；
		///    再循环读"相对偏移 + 字数"，用 VirtualProtect 开读写 → 逐 4 字节写回
		///    → 再恢复原保护属性（原版 intPtr/write 的 if (IntPtr.Size == 4) 两个分支体
		///    完全相同，是壳的写法，照原样保留）。
		/// 整个过程包在 try/catch 里：写内存失败就放弃（不抛给上层），catch 必须留着。
		/// </summary>
		[IntelliLock运行时.模块初始化标记(typeof(IntelliLock运行时.模块初始化标记.标记占位<object>[]))]
		static void 运行时还原方法体()
		{
			if (!IntelliLock运行时.方法体已还原)
			{
				IntelliLock运行时.方法体已还原 = true;
				BinaryReader 补丁读取器 = new BinaryReader(typeof(IntelliLock运行时).Assembly.GetManifestResourceStream("08d03c1e-656e-4b82-b230-c3549f0c2ec9"));
				补丁读取器.BaseStream.Position = 0L;
				byte[] 补丁流 = 补丁读取器.ReadBytes((int)补丁读取器.BaseStream.Length);
				// 32 字节密钥素材（原版每个下标写了 4~6 次，只保留最终生效值）
				byte[] 密钥素材 = new byte[32]
				{
					94, 92, 40, 45, 5, 179, 190, 225, 55, 128, 208, 17, 133, 84, 149, 116,
					9, 134, 24, 78, 151, 228, 126, 21, 63, 208, 205, 135, 28, 91, 23, 110
				};
				byte[] 密钥 = 密钥素材;
				// 16 字节 IV 素材（同样只保留最终生效值；注意这个方法的 IV 之后要整体 Reverse）
				byte[] IV素材 = new byte[16]
				{
					14, 136, 12, 170, 30, 40, 188, 231, 204, 247, 32, 247, 48, 51, 97, 148
				};
				byte[] 初始化向量 = IV素材;
				Array.Reverse(初始化向量);
				byte[] 公钥标记 = typeof(IntelliLock运行时).Assembly.GetName().GetPublicKeyToken();
				if (公钥标记 != null && 公钥标记.Length > 0)
				{
					初始化向量[1] = 公钥标记[0];
					初始化向量[3] = 公钥标记[1];
					初始化向量[5] = 公钥标记[2];
					初始化向量[7] = 公钥标记[3];
					初始化向量[9] = 公钥标记[4];
					初始化向量[11] = 公钥标记[5];
					初始化向量[13] = 公钥标记[6];
					初始化向量[15] = 公钥标记[7];
					Array.Clear(公钥标记, 0, 公钥标记.Length);
				}
				for (int i = 0; i < 初始化向量.Length; i++)
				{
					密钥[i] ^= 初始化向量[i];
				}
				byte[] 输入字节 = 补丁流;
				int 余数字节 = 输入字节.Length % 4;
				int 完整字数 = 输入字节.Length / 4;
				byte[] 解密输出 = new byte[输入字节.Length];
				int 密钥字数 = 密钥.Length / 4;
				uint 累加器 = 0U;
				if (余数字节 > 0)
				{
					完整字数++;
				}
				for (int 序号 = 0; 序号 < 完整字数; 序号++)
				{
					int 密钥字序号 = 序号 % 密钥字数;
					int 输出偏移 = 序号 * 4;
					uint 临时偏移 = (uint)(密钥字序号 * 4);
					uint 密钥字 = (uint)(((int)密钥[(int)(临时偏移 + 3U)] << 24) | ((int)密钥[(int)(临时偏移 + 2U)] << 16) | ((int)密钥[(int)(临时偏移 + 1U)] << 8) | (int)密钥[(int)临时偏移]);
					uint 尾字节掩码 = 255U;
					int 尾字节移位 = 0;
					uint 数据字;
					if (序号 == 完整字数 - 1 && 余数字节 > 0)
					{
						// 原版在这个分支里是"先累加密钥字，再拼尾字"（与 解密字符串 的顺序相反）
						累加器 += 密钥字;
						数据字 = 0U;
						for (int 尾字节序号 = 0; 尾字节序号 < 余数字节; 尾字节序号++)
						{
							if (尾字节序号 > 0)
							{
								数据字 <<= 8;
							}
							数据字 |= (uint)输入字节[输入字节.Length - (1 + 尾字节序号)];
						}
					}
					else
					{
						临时偏移 = (uint)输出偏移;
						累加器 += 密钥字;
						数据字 = (uint)(((int)输入字节[(int)(临时偏移 + 3U)] << 24) | ((int)输入字节[(int)(临时偏移 + 2U)] << 16) | ((int)输入字节[(int)(临时偏移 + 1U)] << 8) | (int)输入字节[(int)临时偏移]);
					}
					uint 累加前值 = 累加器;
					uint 混沌值 = 累加器;
					混沌值 ^= 混沌值 >> 25;
					混沌值 += 1394801171U;
					混沌值 ^= 混沌值 << 5;
					混沌值 += 3457341307U;
					混沌值 ^= 混沌值 >> 3;
					混沌值 += 4040842541U;
					混沌值 = 2018246656U + 混沌值;
					累加器 = 累加前值 + (uint)混沌值;
					if (序号 == 完整字数 - 1 && 余数字节 > 0)
					{
						uint 尾字异或结果 = 累加器 ^ 数据字;
						for (int 尾字节写序号 = 0; 尾字节写序号 < 余数字节; 尾字节写序号++)
						{
							if (尾字节写序号 > 0)
							{
								尾字节掩码 <<= 8;
								尾字节移位 += 8;
							}
							解密输出[输出偏移 + 尾字节写序号] = (byte)((尾字异或结果 & 尾字节掩码) >> 尾字节移位);
						}
					}
					else
					{
						uint 字异或结果 = 累加器 ^ 数据字;
						解密输出[输出偏移] = (byte)(字异或结果 & 255U);
						解密输出[输出偏移 + 1] = (byte)((字异或结果 & 65280U) >> 8);
						解密输出[输出偏移 + 2] = (byte)((字异或结果 & 16711680U) >> 16);
						解密输出[输出偏移 + 3] = (byte)((字异或结果 & 4278190080U) >> 24);
					}
				}
				byte[] 补丁字节 = 解密输出;
				Array.Clear(初始化向量, 0, 初始化向量.Length);
				补丁读取器.Close();
				补丁读取器 = new BinaryReader(new MemoryStream(补丁字节));
				补丁读取器.BaseStream.Position = 0L;
				IntPtr 已写字节数 = IntPtr.Zero;
				Assembly 本程序集 = typeof(IntelliLock运行时).Assembly;
				IntPtr 本进程句柄 = IntelliLock运行时.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				IntelliLock运行时.模块基址 = Marshal.GetHINSTANCE(本程序集.GetModules()[0]).ToInt64();
				int 旧保护值 = 0;
				int 条目数 = 补丁读取器.ReadInt32();
				补丁读取器.ReadInt32();
				for (int 条目序号 = 0; 条目序号 < 条目数; 条目序号++)
				{
					IntPtr 目标地址 = new IntPtr(IntelliLock运行时.模块基址 + (long)补丁读取器.ReadInt32());
					IntelliLock运行时.VirtualProtect(目标地址, 4, 4, ref 旧保护值);
					if (IntPtr.Size == 4)
					{
						IntelliLock运行时.WriteProcessMemory(本进程句柄, 目标地址, BitConverter.GetBytes(补丁读取器.ReadInt32()), 4U, out 已写字节数);
					}
					else
					{
						IntelliLock运行时.WriteProcessMemory(本进程句柄, 目标地址, BitConverter.GetBytes(补丁读取器.ReadInt32()), 4U, out 已写字节数);
					}
					IntelliLock运行时.VirtualProtect(目标地址, 4, 旧保护值, ref 旧保护值);
				}
				try
				{
					while (补丁读取器.BaseStream.Position < 补丁读取器.BaseStream.Length - 1L)
					{
						int 段相对偏移 = 补丁读取器.ReadInt32();
						IntPtr 段地址 = new IntPtr(IntelliLock运行时.模块基址 + (long)段相对偏移);
						int 段字数 = 补丁读取器.ReadInt32();
						IntelliLock运行时.VirtualProtect(段地址, 段字数 * 4, 4, ref 旧保护值);
						for (int 字序号 = 0; 字序号 < 段字数; 字序号++)
						{
							Marshal.WriteInt32(new IntPtr(段地址.ToInt64() + (long)(字序号 * 4)), 补丁读取器.ReadInt32());
						}
						IntelliLock运行时.VirtualProtect(段地址, 段字数 * 4, 旧保护值, ref 旧保护值);
					}
					IntelliLock运行时.CloseHandle(本进程句柄);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000053FC File Offset: 0x000035FC
		/// <summary>
		/// 取程序集的物理路径（原名 SetParams）：依次尝试 Assembly.Location、
		/// GetName().CodeBase 去掉 file:///、反射取 Location 属性，都拿不到就返回空串。
		/// 三段各自 try/catch 吞异常（原件如此，不能合并成一个 try——第一段失败不能跳过第二段）。
		/// </summary>
		internal static object 获取程序集路径(Assembly 目标程序集)
		{
			try
			{
				if (File.Exists(目标程序集.Location))
				{
					return 目标程序集.Location;
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(目标程序集.GetName().CodeBase.ToString().Replace("file:///", "")))
				{
					return 目标程序集.GetName().CodeBase.ToString().Replace("file:///", "");
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(目标程序集.GetType().GetProperty("Location").GetValue(目标程序集, new object[0])
					.ToString()))
				{
					return 目标程序集.GetType().GetProperty("Location").GetValue(目标程序集, new object[0])
						.ToString();
				}
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x06000045 RID: 69
		/// <summary>kernel32!WriteProcessMemory：往目标进程（这里就是自己）写内存。</summary>
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr 进程句柄, IntPtr 目标地址, [In] [Out] byte[] 缓冲区, uint 字节数, out IntPtr 已写字节数);

		// Token: 0x06000046 RID: 70
		/// <summary>kernel32!ReadProcessMemory：从目标进程（这里就是自己）读内存。</summary>
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr 进程句柄, IntPtr 源地址, [In] [Out] byte[] 缓冲区, uint 字节数, out IntPtr 已读字节数);

		// Token: 0x06000047 RID: 71
		/// <summary>kernel32!OpenProcess：打开进程句柄（16 = VM_READ，56 = VM_READ|VM_WRITE|VM_OPERATION）。</summary>
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint 访问权限, int 继承句柄, uint 进程ID);

		// Token: 0x06000048 RID: 72
		/// <summary>kernel32!CloseHandle：关闭句柄。</summary>
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr 句柄);

		// Token: 0x06000049 RID: 73 RVA: 0x0000550C File Offset: 0x0000370C
		/// <summary>
		/// 分块读满整个文件（原名 InitParams）。用 while 循环处理 FileStream.Read
		/// 可能读不满的情况，读满 length 字节才返回。工程内无调用点。
		/// </summary>
		private static byte[] 读取文件字节(string 文件路径)
		{
			byte[] 数据;
			using (FileStream 文件流 = new FileStream(文件路径, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				int 已读 = 0;
				long 长度 = 文件流.Length;
				int 剩余 = (int)长度;
				数据 = new byte[剩余];
				while (剩余 > 0)
				{
					int 本次读 = 文件流.Read(数据, 已读, 剩余);
					已读 += 本次读;
					剩余 -= 本次读;
				}
			}
			return 数据;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00005574 File Offset: 0x00003774
		/// <summary>
		/// AES 解密（原名 OrderParams）：固定 Key / IV，CBC（算法默认）+ PKCS7，
		/// 把密文写进 CryptoStream 后取内存流结果。Key/IV 是壳里写死的常量，
		/// 任何用这套常量的样本都能被同一份代码解开，属于强 IOC。
		/// </summary>
		private static byte[] AES解密(byte[] 密文)
		{
			MemoryStream 内存流 = new MemoryStream();
			SymmetricAlgorithm 算法 = IntelliLock运行时.创建对称加密算法();
			算法.Key = new byte[]
			{
				72, 203, 11, 197, 94, 74, 26, 212, 240, 241,
				21, 60, 137, 176, 184, 49, 116, 97, 251, 21,
				11, 216, 220, 39, 247, 191, 141, 34, 113, 72,
				117, 53
			};
			算法.IV = new byte[]
			{
				59, 246, 166, 75, 41, 13, 119, 13, 44, 46,
				98, 21, 28, 251, 224, 108
			};
			CryptoStream 解密流 = new CryptoStream(内存流, 算法.CreateDecryptor(), CryptoStreamMode.Write);
			解密流.Write(密文, 0, 密文.Length);
			解密流.Close();
			return 内存流.ToArray();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000055E4 File Offset: 0x000037E4
		/// <summary>空桩：永远返回 null，工程内无调用点（壳的占位方法）。</summary>
		private byte[] CloneParams()
		{
			return null;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000055E4 File Offset: 0x000037E4
		/// <summary>空桩：永远返回 null，工程内无调用点。</summary>
		private byte[] CheckParams()
		{
			return null;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000055E4 File Offset: 0x000037E4
		/// <summary>空桩：永远返回 null，工程内无调用点。</summary>
		private byte[] ValidateParams()
		{
			return null;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000055E4 File Offset: 0x000037E4
		/// <summary>空桩：永远返回 null，工程内无调用点。</summary>
		private byte[] StartParams()
		{
			return null;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000055E4 File Offset: 0x000037E4
		/// <summary>空桩：永远返回 null，工程内无调用点。</summary>
		private byte[] PostParams()
		{
			return null;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000055E4 File Offset: 0x000037E4
		/// <summary>空桩：永远返回 null，工程内无调用点。</summary>
		private byte[] PrintParams()
		{
			return null;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000055F4 File Offset: 0x000037F4
		/// <summary>
		/// 授权校验空桩：拼一个授权编号格式的字符串（{11111-22222-40001-00001}），
		/// if/else 两支返回同样的 {1,2}，等于什么都没校验。无调用点。
		/// 字符串常量保留原样——它们是 IntelliLock 授权编号模板，可用于在别的样本里搜索。
		/// </summary>
		internal byte[] CreateParams()
		{
			string 授权编号 = "{11111-22222-40001-00001}";
			if (授权编号.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00005634
		/// <summary>授权校验空桩（编号模板 {11111-22222-40001-00002}），两支都返回 {1,2}。无调用点。</summary>
		internal byte[] QueryParams()
		{
			string 授权编号 = "{11111-22222-40001-00002}";
			if (授权编号.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00005674
		/// <summary>授权校验空桩（编号模板 {11111-22222-50001-00001}），两支都返回 {1,2}。无调用点。</summary>
		internal byte[] FindParams()
		{
			string 授权编号 = "{11111-22222-50001-00001}";
			if (授权编号.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000056B4
		/// <summary>授权校验空桩（编号模板 {11111-22222-50001-00002}），两支都返回 {1,2}。无调用点。</summary>
		internal byte[] FillParams()
		{
			string 授权编号 = "{11111-22222-50001-00002}";
			if (授权编号.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000056F4
		/// <summary>授权校验空桩（编号模板 {11111-22222-60001-00001}），两支都返回 {1,2}。无调用点。</summary>
		internal byte[] CallParams()
		{
			string 授权编号 = "{11111-22222-60001-00001}";
			if (授权编号.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00005734
		/// <summary>授权校验空桩（编号模板 {11111-22222-60001-00002}），两支都返回 {1,2}。无调用点。</summary>
		internal byte[] PopParams()
		{
			string 授权编号 = "{11111-22222-60001-00002}";
			if (授权编号.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00005774 File Offset: 0x00003974
		/// <summary>
		/// AES 加密后转 Base64（原名 VisitParams）：明文按 Unicode 取字节，
		/// Key 是写死的 32 字节常量，IV = MD5(Unicode(密钥源字符串))。
		/// 与 Base64转Unicode字符串 不是一对（后者不做解密），是壳里另一条加密路径。工程内无调用点。
		/// </summary>
		internal static string AES加密为Base64(string 明文, string 密钥源)
		{
			byte[] 明文字节 = Encoding.Unicode.GetBytes(明文);
			byte[] 数据 = 明文字节;
			byte[] 固定密钥 = new byte[]
			{
				82, 102, 104, 110, 32, 77, 24, 34, 118, 181,
				51, 17, 18, 51, 12, 109, 10, 32, 77, 24,
				34, 158, 161, 41, 97, 28, 118, 181, 5, 25,
				1, 88
			};
			byte[] 初始化向量 = IntelliLock运行时.计算MD5(Encoding.Unicode.GetBytes(密钥源));
			MemoryStream 内存流 = new MemoryStream();
			SymmetricAlgorithm 算法 = IntelliLock运行时.创建对称加密算法();
			算法.Key = 固定密钥;
			算法.IV = 初始化向量;
			CryptoStream 加密流 = new CryptoStream(内存流, 算法.CreateEncryptor(), CryptoStreamMode.Write);
			加密流.Write(数据, 0, 数据.Length);
			加密流.Close();
			return Convert.ToBase64String(内存流.ToArray());
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>壳注入的空桩：固定返回 true，无调用点。</summary>
		internal static bool AwakeDefinition()
		{
			return true;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
		internal static bool CheckDefinition()
		{
			return false;
		}

		// Token: 0x04000016 RID: 22
		/// <summary>
		/// MD5 的 64 个常量 T[i] = floor(2^32 × |sin(i)|)（原名 proxyStrategy）。
		/// 原版就用 uint[] 字面量，值不动。
		/// </summary>
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

		// Token: 0x04000017 RID: 23
		/// <summary>是否已经读过 FIPS 开关。原名 m_IdentifierStrategy。</summary>
		private static bool 已检查FIPS = false;

		// Token: 0x04000018 RID: 24
		/// <summary>垃圾字段：只在声明处赋值，全类没有任何读写（原名 serializerStrategy）。</summary>
		private static byte[] 未使用字段1 = new byte[0];

		// Token: 0x04000019 RID: 25
		/// <summary>ReadProcessMemory 的 out 参数接收器。原名 publisherStrategy。</summary>
		private static IntPtr 已读字节数 = IntPtr.Zero;

		// Token: 0x0400001A RID: 26
		/// <summary>偏移 → 缓存槽 的映射：值 &gt; 0 表示该偏移已有缓存。原名 systemStrategy。</summary>
		private static int[] 偏移到缓存槽 = new int[0];

		// Token: 0x0400001B RID: 27
		/// <summary>方法体是否已还原（保证只还原一次）。原名 m_CallbackStrategy。</summary>
		private static bool 方法体已还原 = false;

		// Token: 0x0400001C RID: 28
		/// <summary>垃圾字段：只在声明处赋值（原名 _DispatcherStrategy）。</summary>
		private static byte[] 未使用字段2 = new byte[0];

		// Token: 0x0400001D RID: 29
		/// <summary>已经解出来的字符串缓存（下标从 1 开始用）。原名 m_InfoStrategy。</summary>
		private static string[] 字符串缓存 = new string[0];

		// Token: 0x0400001E RID: 30
		/// <summary>AllowOnlyFipsAlgorithms 的取值（true = 系统只允许 FIPS 认证算法）。原名 _ConfigStrategy。</summary>
		private static bool FIPS开关值 = false;

		// Token: 0x0400001F RID: 31
		/// <summary>本模块在内存中的基址（64 位）。原名 stateStrategy。</summary>
		private static long 模块基址 = 0L;

		// Token: 0x04000020 RID: 32
		/// <summary>下一个可用的缓存槽位，从 1 开始。原名 _IndexerStrategy。</summary>
		private static int 下一个缓存槽位 = 1;

		// Token: 0x04000021 RID: 33
		/// <summary>本模块在内存中的基址（32 位进程用）。原名 m_CandidateStrategy。</summary>
		private static int 模块基址32位 = 0;

		// Token: 0x04000022 RID: 34
		/// <summary>本进程句柄（OpenProcess 得到，复用不关闭直到需要时才关）。原名 objectStrategy。</summary>
		private static IntPtr 本进程句柄 = IntPtr.Zero;

		// Token: 0x04000023 RID: 35
		/// <summary>解密后的字符串表原始字节。原名 _MerchantStrategy。</summary>
		private static byte[] 字符串表 = new byte[0];

		// Token: 0x04000024 RID: 36
		/// <summary>垃圾字段：只在声明处赋值（原名 _ValStrategy）。</summary>
		private static byte[] 未使用字段3 = new byte[0];

		// Token: 0x0200000E RID: 14
		/// <summary>
		/// 加壳器用来标记「模块初始化点」的特性（原名 ErrorStrategy）。
		/// 自定义构造函数刻意再贴一个自身特性，是壳的写法；本类不在业务里被使用。
		/// </summary>
		internal class 模块初始化标记 : Attribute
		{
			// Token: 0x0600005B RID: 91 RVA: 0x000023DB File Offset: 0x000005DB
			/// <summary>空构造：接收一个 Type[] 占位参数，什么都不做。</summary>
			[IntelliLock运行时.模块初始化标记(typeof(IntelliLock运行时.模块初始化标记.标记占位<object>[]))]
			public 模块初始化标记(object 参数)
			{
			}

			// Token: 0x0200000F RID: 15
			/// <summary>泛型占位类型，只用来让上面的 typeof 表达式成立；两个方法都是空桩。</summary>
			internal class 标记占位<T>
			{
				// Token: 0x0600005D RID: 93 RVA: 0x000021BC File Offset: 0x000003BC
				/// <summary>壳注入的空桩：固定返回 true，无调用点。</summary>
				internal static bool ReflectDefinition()
				{
					return true;
				}

				// Token: 0x0600005E RID: 94 RVA: 0x000021BF File Offset: 0x000003BF
				/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
				internal static bool UpdateDefinition()
				{
					return false;
				}
			}
		}

		// Token: 0x02000010 RID: 16
		/// <summary>空的 [Flags] 枚举（原名 LabelPositions），壳注入的残留，没有任何成员与引用。</summary>
		[Flags]
		private enum LabelPositions
		{

		}
	}
}
