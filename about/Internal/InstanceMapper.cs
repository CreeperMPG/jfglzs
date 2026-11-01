using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace WindowsApplication1.Internal
{
	/// <summary>
	/// 混淆器（.NET Reactor）的运行时支撑类。反混淆后可以确认它承担三类互不相干的职责：
	/// 1. 密码学实现：<see cref="自实现MD5"/>（含四个轮函数与常量表）、以及「FIPS 策略探测 + 对称算法选择」；
	/// 2. 字符串解密器 <see cref="解密字符串"/>：从内嵌资源解出加密字符串池，再按索引从**本进程内存**里取单条明文；
	/// 3. 内存补丁器 <see cref="解密并修补内存数据"/>：解密另一份内嵌数据后，用
	///    VirtualProtect/WriteProcessMemory 往自身模块的指定 RVA 回写 int32，属壳的抗篡改/代码还原逻辑。
	/// 其余大量方法恒返回 null / 固定值，是混淆器插入的垃圾方法。整个类型对业务逻辑没有直接贡献，
	/// 但它是「字符串为什么在源码里看不见」的答案。
	/// </summary>
	// Token: 0x02000010 RID: 16
	internal class InstanceMapper
	{
		// Token: 0x0600007C RID: 124 RVA: 0x00002396 File Offset: 0x00000596
		/// <summary>空方法占位：方法体为空，属混淆器/壳插入的噪声。</summary>
		private void 空方法占位()
		{
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004444 File Offset: 0x00002644
		/// <summary>
		/// 自实现 MD5：标准 64 轮、标准 IV（0x67452301/0xEFCDAB89/0x98BADCFE/0x10325476）、
		/// 标准常量表 <see cref="MD5常量表"/>，返回 16 字节摘要。
		/// 这里不用 BCL 的 MD5，是为了在「仅允许 FIPS 算法」的机器上也能算（见 <see cref="MD5摘要"/> 的分流）。
		/// 注意：元数据里参数类型就是 `object`，方法体直接对它取长度并按下标访问（IL 层是 `ldlen`），
		/// 属不可验证代码；唯一调用者传的确实是 byte[]，这里保持原签名不动。
		/// </summary>
		internal static byte[] 自实现MD5(object 输入数据)
		{
			uint[] 消息字 = new uint[16];
			int 补位基准 = 448 - 输入数据.Length * 8 % 512;
			uint 补位字节数 = (uint)((补位基准 + 512) % 512);
			if (补位字节数 == 0U)
			{
				补位字节数 = 512U;
			}
			uint 总长度 = (uint)((long)输入数据.Length + (long)((ulong)(补位字节数 / 8U)) + 8L);
			ulong 位长度 = (ulong)((long)输入数据.Length * 8L);
			byte[] 补位缓冲 = new byte[总长度];
			for (int i = 0; i < 输入数据.Length; i++)
			{
				补位缓冲[i] = 输入数据[i];
			}
			byte[] 补位缓冲别名 = 补位缓冲;
			int 输入长度 = 输入数据.Length;
			补位缓冲别名[输入长度] |= 128;
			for (int j = 8; j > 0; j--)
			{
				补位缓冲[(int)(checked((IntPtr)(unchecked((ulong)总长度 - (ulong)((long)j)))))] = (byte)((位长度 >> (8 - j) * 8) & 255UL);
			}
			uint 分组数 = (uint)(补位缓冲.Length * 8 / 32);
			uint a = 1732584193U;
			uint b = 4023233417U;
			uint c = 2562383102U;
			uint d = 271733878U;
			for (uint 分组 = 0U; 分组 < 分组数 / 16U; 分组 += 1U)
			{
				uint 块偏移 = 分组 << 6;
				// 上界写的是 61 而不是 64，配合步长 4 正好覆盖下标 0..60 共 16 个字（原版写法，未改）
				for (uint 字 = 0U; 字 < 61U; 字 += 4U)
				{
					消息字[(int)((UIntPtr)(字 >> 2))] = (uint)(((int)补位缓冲[(int)((UIntPtr)(块偏移 + (字 + 3U)))] << 24) | ((int)补位缓冲[(int)((UIntPtr)(块偏移 + (字 + 2U)))] << 16) | ((int)补位缓冲[(int)((UIntPtr)(块偏移 + (字 + 1U)))] << 8) | (int)补位缓冲[(int)((UIntPtr)(块偏移 + 字))]);
				}
				uint 旧a = a;
				uint 旧b = b;
				uint 旧c = c;
				uint 旧d = d;
				InstanceMapper.MD5轮函数F(ref a, b, c, d, 0U, 7, 1U, 消息字);
				InstanceMapper.MD5轮函数F(ref d, a, b, c, 1U, 12, 2U, 消息字);
				InstanceMapper.MD5轮函数F(ref c, d, a, b, 2U, 17, 3U, 消息字);
				InstanceMapper.MD5轮函数F(ref b, c, d, a, 3U, 22, 4U, 消息字);
				InstanceMapper.MD5轮函数F(ref a, b, c, d, 4U, 7, 5U, 消息字);
				InstanceMapper.MD5轮函数F(ref d, a, b, c, 5U, 12, 6U, 消息字);
				InstanceMapper.MD5轮函数F(ref c, d, a, b, 6U, 17, 7U, 消息字);
				InstanceMapper.MD5轮函数F(ref b, c, d, a, 7U, 22, 8U, 消息字);
				InstanceMapper.MD5轮函数F(ref a, b, c, d, 8U, 7, 9U, 消息字);
				InstanceMapper.MD5轮函数F(ref d, a, b, c, 9U, 12, 10U, 消息字);
				InstanceMapper.MD5轮函数F(ref c, d, a, b, 10U, 17, 11U, 消息字);
				InstanceMapper.MD5轮函数F(ref b, c, d, a, 11U, 22, 12U, 消息字);
				InstanceMapper.MD5轮函数F(ref a, b, c, d, 12U, 7, 13U, 消息字);
				InstanceMapper.MD5轮函数F(ref d, a, b, c, 13U, 12, 14U, 消息字);
				InstanceMapper.MD5轮函数F(ref c, d, a, b, 14U, 17, 15U, 消息字);
				InstanceMapper.MD5轮函数F(ref b, c, d, a, 15U, 22, 16U, 消息字);
				InstanceMapper.MD5轮函数G(ref a, b, c, d, 1U, 5, 17U, 消息字);
				InstanceMapper.MD5轮函数G(ref d, a, b, c, 6U, 9, 18U, 消息字);
				InstanceMapper.MD5轮函数G(ref c, d, a, b, 11U, 14, 19U, 消息字);
				InstanceMapper.MD5轮函数G(ref b, c, d, a, 0U, 20, 20U, 消息字);
				InstanceMapper.MD5轮函数G(ref a, b, c, d, 5U, 5, 21U, 消息字);
				InstanceMapper.MD5轮函数G(ref d, a, b, c, 10U, 9, 22U, 消息字);
				InstanceMapper.MD5轮函数G(ref c, d, a, b, 15U, 14, 23U, 消息字);
				InstanceMapper.MD5轮函数G(ref b, c, d, a, 4U, 20, 24U, 消息字);
				InstanceMapper.MD5轮函数G(ref a, b, c, d, 9U, 5, 25U, 消息字);
				InstanceMapper.MD5轮函数G(ref d, a, b, c, 14U, 9, 26U, 消息字);
				InstanceMapper.MD5轮函数G(ref c, d, a, b, 3U, 14, 27U, 消息字);
				InstanceMapper.MD5轮函数G(ref b, c, d, a, 8U, 20, 28U, 消息字);
				InstanceMapper.MD5轮函数G(ref a, b, c, d, 13U, 5, 29U, 消息字);
				InstanceMapper.MD5轮函数G(ref d, a, b, c, 2U, 9, 30U, 消息字);
				InstanceMapper.MD5轮函数G(ref c, d, a, b, 7U, 14, 31U, 消息字);
				InstanceMapper.MD5轮函数G(ref b, c, d, a, 12U, 20, 32U, 消息字);
				InstanceMapper.MD5轮函数H(ref a, b, c, d, 5U, 4, 33U, 消息字);
				InstanceMapper.MD5轮函数H(ref d, a, b, c, 8U, 11, 34U, 消息字);
				InstanceMapper.MD5轮函数H(ref c, d, a, b, 11U, 16, 35U, 消息字);
				InstanceMapper.MD5轮函数H(ref b, c, d, a, 14U, 23, 36U, 消息字);
				InstanceMapper.MD5轮函数H(ref a, b, c, d, 1U, 4, 37U, 消息字);
				InstanceMapper.MD5轮函数H(ref d, a, b, c, 4U, 11, 38U, 消息字);
				InstanceMapper.MD5轮函数H(ref c, d, a, b, 7U, 16, 39U, 消息字);
				InstanceMapper.MD5轮函数H(ref b, c, d, a, 10U, 23, 40U, 消息字);
				InstanceMapper.MD5轮函数H(ref a, b, c, d, 13U, 4, 41U, 消息字);
				InstanceMapper.MD5轮函数H(ref d, a, b, c, 0U, 11, 42U, 消息字);
				InstanceMapper.MD5轮函数H(ref c, d, a, b, 3U, 16, 43U, 消息字);
				InstanceMapper.MD5轮函数H(ref b, c, d, a, 6U, 23, 44U, 消息字);
				InstanceMapper.MD5轮函数H(ref a, b, c, d, 9U, 4, 45U, 消息字);
				InstanceMapper.MD5轮函数H(ref d, a, b, c, 12U, 11, 46U, 消息字);
				InstanceMapper.MD5轮函数H(ref c, d, a, b, 15U, 16, 47U, 消息字);
				InstanceMapper.MD5轮函数H(ref b, c, d, a, 2U, 23, 48U, 消息字);
				InstanceMapper.MD5轮函数I(ref a, b, c, d, 0U, 6, 49U, 消息字);
				InstanceMapper.MD5轮函数I(ref d, a, b, c, 7U, 10, 50U, 消息字);
				InstanceMapper.MD5轮函数I(ref c, d, a, b, 14U, 15, 51U, 消息字);
				InstanceMapper.MD5轮函数I(ref b, c, d, a, 5U, 21, 52U, 消息字);
				InstanceMapper.MD5轮函数I(ref a, b, c, d, 12U, 6, 53U, 消息字);
				InstanceMapper.MD5轮函数I(ref d, a, b, c, 3U, 10, 54U, 消息字);
				InstanceMapper.MD5轮函数I(ref c, d, a, b, 10U, 15, 55U, 消息字);
				InstanceMapper.MD5轮函数I(ref b, c, d, a, 1U, 21, 56U, 消息字);
				InstanceMapper.MD5轮函数I(ref a, b, c, d, 8U, 6, 57U, 消息字);
				InstanceMapper.MD5轮函数I(ref d, a, b, c, 15U, 10, 58U, 消息字);
				InstanceMapper.MD5轮函数I(ref c, d, a, b, 6U, 15, 59U, 消息字);
				InstanceMapper.MD5轮函数I(ref b, c, d, a, 13U, 21, 60U, 消息字);
				InstanceMapper.MD5轮函数I(ref a, b, c, d, 4U, 6, 61U, 消息字);
				InstanceMapper.MD5轮函数I(ref d, a, b, c, 11U, 10, 62U, 消息字);
				InstanceMapper.MD5轮函数I(ref c, d, a, b, 2U, 15, 63U, 消息字);
				InstanceMapper.MD5轮函数I(ref b, c, d, a, 9U, 21, 64U, 消息字);
				a += 旧a;
				b += 旧b;
				c += 旧c;
				d += 旧d;
			}
			byte[] 摘要 = new byte[16];
			Array.Copy(BitConverter.GetBytes(a), 0, 摘要, 0, 4);
			Array.Copy(BitConverter.GetBytes(b), 0, 摘要, 4, 4);
			Array.Copy(BitConverter.GetBytes(c), 0, 摘要, 8, 4);
			Array.Copy(BitConverter.GetBytes(d), 0, 摘要, 12, 4);
			return 摘要;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00002489 File Offset: 0x00000689
		/// <summary>
		/// MD5 第 1 轮（F 函数）：`a = b + rol(a + ((b &amp; c) | (~b &amp; d)) + X[k] + T[i])`。
		/// ref 参数是被更新的状态字，其余三个是当前 a/b/c/d 的只读副本。
		/// </summary>
		private static void MD5轮函数F(ref uint 状态, uint b, uint c, uint d, uint 消息字索引, ushort 位移位数, uint 常量索引, object 消息字)
		{
			状态 = b + InstanceMapper.循环左移(状态 + ((b & c) | (~b & d)) + 消息字[(int)((UIntPtr)消息字索引)] + InstanceMapper.MD5常量表[(int)((UIntPtr)(常量索引 - 1U))], 位移位数);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000024B4 File Offset: 0x000006B4
		/// <summary>MD5 第 2 轮（G 函数）：`a = b + rol(a + ((b &amp; d) | (c &amp; ~d)) + X[k] + T[i])`。</summary>
		private static void MD5轮函数G(ref uint 状态, uint b, uint c, uint d, uint 消息字索引, ushort 位移位数, uint 常量索引, object 消息字)
		{
			状态 = b + InstanceMapper.循环左移(状态 + ((b & d) | (c & ~d)) + 消息字[(int)((UIntPtr)消息字索引)] + InstanceMapper.MD5常量表[(int)((UIntPtr)(常量索引 - 1U))], 位移位数);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000024DF File Offset: 0x000006DF
		/// <summary>MD5 第 3 轮（H 函数）：`a = b + rol(a + (b ^ c ^ d) + X[k] + T[i])`。</summary>
		private static void MD5轮函数H(ref uint 状态, uint b, uint c, uint d, uint 消息字索引, ushort 位移位数, uint 常量索引, object 消息字)
		{
			状态 = b + InstanceMapper.循环左移(状态 + (b ^ c ^ d) + 消息字[(int)((UIntPtr)消息字索引)] + InstanceMapper.MD5常量表[(int)((UIntPtr)(常量索引 - 1U))], 位移位数);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002507 File Offset: 0x00000707
		/// <summary>MD5 第 4 轮（I 函数）：`a = b + rol(a + (c ^ (b | ~d)) + X[k] + T[i])`。</summary>
		private static void MD5轮函数I(ref uint 状态, uint b, uint c, uint d, uint 消息字索引, ushort 位移位数, uint 常量索引, object 消息字)
		{
			状态 = b + InstanceMapper.循环左移(状态 + (c ^ (b | ~d)) + 消息字[(int)((UIntPtr)消息字索引)] + InstanceMapper.MD5常量表[(int)((UIntPtr)(常量索引 - 1U))], 位移位数);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00002530 File Offset: 0x00000730
		/// <summary>32 位循环左移：按 `位移位数` 把高低两半拼起来（等价于 ROL）。</summary>
		private static uint 循环左移(uint 值, ushort 位移位数)
		{
			return (值 >> (int)(32 - 位移位数)) | (值 << (int)位移位数);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00002542 File Offset: 0x00000742
		/// <summary>
		/// 返回「本机是否仅允许 FIPS 算法」。首次调用时先探测一次（结果缓存到
		/// <see cref="仅允许FIPS算法"/>），之后的调用直接读缓存。
		/// </summary>
		internal static bool 是否仅允许FIPS算法()
		{
			if (!InstanceMapper.已读取FIPS策略)
			{
				InstanceMapper.读取FIPS策略();
				InstanceMapper.已读取FIPS策略 = true;
			}
			return InstanceMapper.仅允许FIPS算法;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00004AE8 File Offset: 0x00002CE8
		/// <summary>
		/// 用反射读 `CryptoConfig.AllowOnlyFipsAlgorithms`（静态只读属性），结果存进
		/// <see cref="仅允许FIPS算法"/>。`.NET 4.x` 上该属性存在；老框架/裁剪环境找不到类型或方法时
		/// 会抛异常，这里整段吞掉，缓存值保持 false。
		/// </summary>
		internal static void 读取FIPS策略()
		{
			try
			{
				InstanceMapper.仅允许FIPS算法 = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00004B48 File Offset: 0x00004B48
		/// <summary>
		/// 按 FIPS 策略挑一个对称算法实例：允许非 FIPS 时用 RijndaelManaged；
		/// 仅允许 FIPS 时优先 AesCryptoServiceProvider（FIPS 认证实现），取不到再退回 RijndaelManaged。
		/// 两条路都用 CBC（调用方各自设置 Mode）。
		/// </summary>
		internal static SymmetricAlgorithm 创建对称算法()
		{
			SymmetricAlgorithm 算法 = null;
			if (InstanceMapper.是否仅允许FIPS算法())
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

		// Token: 0x06000086 RID: 134 RVA: 0x0000255B File Offset: 0x0000075B
		/// <summary>
		/// 计算 MD5 摘要（16 字节），按 FIPS 策略分流：
		/// 允许非 FIPS → 用 BCL 的 MD5CryptoServiceProvider；仅允许 FIPS → 用
		/// <see cref="自实现MD5"/>（因为 FIPS 模式下 BCL 的 MD5CryptoServiceProvider 会被拒）。
		/// 调用方有两个：内嵌程序集资源名的派生（IssuerMapper）与 AES 加密的 IV 派生（AES加密字符串）。
		/// </summary>
		internal static byte[] MD5摘要(byte[] 数据)
		{
			if (!InstanceMapper.是否仅允许FIPS算法())
			{
				return new MD5CryptoServiceProvider().ComputeHash(数据);
			}
			return InstanceMapper.自实现MD5(数据);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004B88 File Offset: 0x00004801
		/// <summary>
		/// 字符串解密器（.NET Reactor 的字符串池机制）。分两段：
		/// 第一段只跑一次——读内嵌资源 "dac028a9-ca53-4586-8b57-452994e78968"，
		/// 用硬编码的 32 字节「密钥材料」与 16 字节「初始向量」材料（其中奇数号字节会被本程序集
		/// 公钥标记覆盖）XOR 出 AES 密钥，CBC 解出整个字符串池；再对整块做一次自研的 32 位
		/// 流式混洗（累加密钥字 → 乘加扰动 → 与数据字 XOR）。
		/// 第二段按索引取单条：用 <see cref="字符串池数据"/> 里记录的相对偏移，通过
		/// ReadProcessMemory 从**本进程**的模块基址处读「长度前缀 + AES 密文」，
		/// 再用 <see cref="AES解密字符串数据"/> 解成 Unicode 明文并缓存。
		/// 参数 -1 是特殊值：只做第一段（初始化整池），不取单条。
		/// 注意：本解壳产物里该资源已被剥离（现存资源只有 Form1 的图标与背景图），
		/// 因此这个方法在本工程中是死代码；在原样本里它由壳的 stub 调用。
		/// </summary>
		private static string 解密字符串(int 字符串索引)
		{
			if (InstanceMapper.字符串池数据.Length == 0)
			{
				BinaryReader 资源读取器 = new BinaryReader(typeof(InstanceMapper).Assembly.GetManifestResourceStream("dac028a9-ca53-4586-8b57-452994e78968"));
				资源读取器.BaseStream.Position = 0L;
				// 同一缓冲先装资源密文，解密后改指向字符串池（原版复用同一个局部变量）
				byte[] 数据缓冲 = 资源读取器.ReadBytes((int)资源读取器.BaseStream.Length);
				// 原版对同一个下标连续赋值多次，只有最后一次生效；此处按最终值展开成字面量
				byte[] 密钥材料 = new byte[]
				{
					154, 156, 119, 238, 57, 212, 204, 19, 147, 91, 189, 59, 95, 19, 170, 32,
					224, 119, 51, 38, 166, 92, 157, 93, 201, 39, 61, 159, 201, 58, 115, 78
				};
				byte[] 密钥缓冲 = 密钥材料;
				// 同上：反复赋值后只剩最后一组有效
				byte[] 初始向量 = new byte[]
				{
					103, 113, 20, 234, 46, 90, 80, 229, 122, 120, 33, 152, 0, 0, 139, 105
				};
				byte[] 向量缓冲 = 初始向量;
				byte[] 公钥标记 = typeof(InstanceMapper).Assembly.GetName().GetPublicKeyToken();
				if (公钥标记 != null && 公钥标记.Length > 0)
				{
					向量缓冲[1] = 公钥标记[0];
					向量缓冲[3] = 公钥标记[1];
					向量缓冲[5] = 公钥标记[2];
					向量缓冲[7] = 公钥标记[3];
					向量缓冲[9] = 公钥标记[4];
					向量缓冲[11] = 公钥标记[5];
					向量缓冲[13] = 公钥标记[6];
					向量缓冲[15] = 公钥标记[7];
				}
				for (int i = 0; i < 向量缓冲.Length; i++)
				{
					密钥缓冲[i] ^= 向量缓冲[i];
				}
				if (字符串索引 == -1)
				{
					SymmetricAlgorithm 算法 = InstanceMapper.创建对称算法();
					算法.Mode = CipherMode.CBC;
					ICryptoTransform 解密器 = 算法.CreateDecryptor(密钥缓冲, 向量缓冲);
					MemoryStream 内存流 = new MemoryStream();
					CryptoStream 解密流 = new CryptoStream(内存流, 解密器, CryptoStreamMode.Write);
					解密流.Write(数据缓冲, 0, 数据缓冲.Length);
					解密流.FlushFinalBlock();
					InstanceMapper.字符串池数据 = 内存流.ToArray();
					if (InstanceMapper.字符串池数据.Length > 0)
					{
						InstanceMapper.字符串缓存 = new string[InstanceMapper.字符串池数据.Length / 4 + 1];
						InstanceMapper.缓存槽索引 = new int[InstanceMapper.字符串池数据.Length / 4 + 1];
					}
					内存流.Close();
					解密流.Close();
					资源读取器.Close();
					数据缓冲 = InstanceMapper.字符串池数据;
				}
				int 余数 = 数据缓冲.Length % 4;
				int 处理字数 = 数据缓冲.Length / 4;
				byte[] 明文缓冲 = new byte[数据缓冲.Length];
				int 密钥字数 = 密钥缓冲.Length / 4;
				uint 累加器 = 0U;
				if (余数 > 0)
				{
					处理字数++;
				}
				for (int 字序号 = 0; 字序号 < 处理字数; 字序号++)
				{
					int 密钥字序号 = 字序号 % 密钥字数;
					int 输出偏移 = 字序号 * 4;
					uint 字节偏移 = (uint)(密钥字序号 * 4);
					uint 密钥字 = (uint)(((int)密钥缓冲[(int)((UIntPtr)(字节偏移 + 3U))] << 24) | ((int)密钥缓冲[(int)((UIntPtr)(字节偏移 + 2U))] << 16) | ((int)密钥缓冲[(int)((UIntPtr)(字节偏移 + 1U))] << 8) | (int)密钥缓冲[(int)((UIntPtr)字节偏移)]);
					uint 输出掩码 = 255U;
					int 输出移位 = 0;
					uint 数据字;
					if (字序号 == 处理字数 - 1 && 余数 > 0)
					{
						数据字 = 0U;
						for (int k = 0; k < 余数; k++)
						{
							if (k > 0)
							{
								数据字 <<= 8;
							}
							数据字 |= (uint)数据缓冲[数据缓冲.Length - (1 + k)];
						}
						累加器 += 密钥字;
					}
					else
					{
						字节偏移 = (uint)输出偏移;
						数据字 = (uint)(((int)数据缓冲[(int)((UIntPtr)(字节偏移 + 3U))] << 24) | ((int)数据缓冲[(int)((UIntPtr)(字节偏移 + 2U))] << 16) | ((int)数据缓冲[(int)((UIntPtr)(字节偏移 + 1U))] << 8) | (int)数据缓冲[(int)((UIntPtr)字节偏移)]);
						累加器 += 密钥字;
					}
					uint 种子 = 累加器;
					uint 混合值 = 累加器;
					混合值 ^= 混合值 >> 25;
					混合值 += 1394801171U;
					混合值 ^= 混合值 << 5;
					混合值 += 3457341307U;
					混合值 ^= 混合值 >> 3;
					混合值 += 4040842541U;
					混合值 = 2018246656U + 混合值;
					累加器 = 种子 + (uint)混合值;
					if (字序号 == 处理字数 - 1 && 余数 > 0)
					{
						uint 输出值 = 累加器 ^ 数据字;
						for (int l = 0; l < 余数; l++)
						{
							if (l > 0)
							{
								输出掩码 <<= 8;
								输出移位 += 8;
							}
							明文缓冲[输出偏移 + l] = (byte)((输出值 & 输出掩码) >> 输出移位);
						}
					}
					else
					{
						uint 输出值 = 累加器 ^ 数据字;
						明文缓冲[输出偏移] = (byte)(输出值 & 255U);
						明文缓冲[输出偏移 + 1] = (byte)((输出值 & 65280U) >> 8);
						明文缓冲[输出偏移 + 2] = (byte)((输出值 & 16711680U) >> 16);
						明文缓冲[输出偏移 + 3] = (byte)((输出值 & 4278190080U) >> 24);
					}
				}
				InstanceMapper.字符串池数据 = 明文缓冲;
				if (InstanceMapper.字符串池数据.Length > 0)
				{
					InstanceMapper.字符串缓存 = new string[InstanceMapper.字符串池数据.Length / 4 + 1];
					InstanceMapper.缓存槽索引 = new int[InstanceMapper.字符串池数据.Length / 4 + 1];
				}
				Assembly 程序集 = typeof(InstanceMapper).Assembly;
				if (IntPtr.Size == 4)
				{
					InstanceMapper.模块基址32 = Marshal.GetHINSTANCE(程序集.GetModules()[0]).ToInt32();
				}
				InstanceMapper.模块基址64 = Marshal.GetHINSTANCE(程序集.GetModules()[0]).ToInt64();
			}
			int 槽位 = 字符串索引 / 4;
			if (InstanceMapper.缓存槽索引[槽位] > 0)
			{
				return ((string[])InstanceMapper.字符串缓存)[InstanceMapper.缓存槽索引[槽位]];
			}
			int 相对偏移 = BitConverter.ToInt32(InstanceMapper.字符串池数据, 字符串索引);
			if (InstanceMapper.当前进程句柄 == IntPtr.Zero)
			{
				InstanceMapper.当前进程句柄 = InstanceMapper.OpenProcess(16U, 1, (uint)Process.GetCurrentProcess().Id);
			}
			byte[] 临时缓冲 = new byte[4];
			if (IntPtr.Size == 4)
			{
				InstanceMapper.ReadProcessMemory(InstanceMapper.当前进程句柄, new IntPtr(InstanceMapper.模块基址32 + 相对偏移), 临时缓冲, 4U, out InstanceMapper.读取字节数);
			}
			else
			{
				InstanceMapper.ReadProcessMemory(InstanceMapper.当前进程句柄, new IntPtr(InstanceMapper.模块基址64 + (long)相对偏移), 临时缓冲, 4U, out InstanceMapper.读取字节数);
			}
			int 密文长度 = BitConverter.ToInt32(临时缓冲, 0);
			临时缓冲 = new byte[密文长度];
			if (IntPtr.Size == 4)
			{
				InstanceMapper.ReadProcessMemory(InstanceMapper.当前进程句柄, new IntPtr(InstanceMapper.模块基址32 + 相对偏移 + 4), 临时缓冲, Convert.ToUInt32(密文长度), out InstanceMapper.读取字节数);
			}
			else
			{
				InstanceMapper.ReadProcessMemory(InstanceMapper.当前进程句柄, new IntPtr(InstanceMapper.模块基址64 + (long)相对偏移 + 4L), 临时缓冲, Convert.ToUInt32(密文长度), out InstanceMapper.读取字节数);
			}
			byte[] 明文 = InstanceMapper.AES解密字符串数据(临时缓冲);
			string 结果 = Encoding.Unicode.GetString(明文, 0, 明文.Length);
			InstanceMapper.缓存槽索引[槽位] = InstanceMapper.下一个缓存槽;
			((string[])InstanceMapper.字符串缓存)[InstanceMapper.下一个缓存槽] = 结果;
			InstanceMapper.下一个缓存槽++;
			return 结果;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00005D18 File Offset: 0x000003F8
		/// <summary>
		/// Base64 → Unicode 字符串。注意它是「把 Base64 解成字节后整段按 UTF-16 解」，不校验长度奇偶，
		/// 也不去 BOM。本工程内无调用点（原样本里是字符串池之外的一条备用解码路径）。
		/// </summary>
		internal static string Base64转Unicode(string 密文)
		{
			byte[] 字节 = Convert.FromBase64String(密文);
			return Encoding.Unicode.GetString(字节, 0, 字节.Length);
		}

		// Token: 0x06000089 RID: 137
		/// <summary>改内存页保护（PAGE_READWRITE = 4）。被 <see cref="解密并修补内存数据"/> 使用。</summary>
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr 地址, int 大小, int 新保护, ref int 旧保护);

		// Token: 0x0600008A RID: 138 RVA: 0x00005D3C File Offset: 0x0000035C
		/// <summary>
		/// 内存补丁器（壳的抗篡改/代码还原逻辑）。首次调用时：
		/// 读内嵌资源 "c7c9a35c-0300-44ea-976c-1797ff0710b0"，用一套「先反转向量、再与公钥标记
		/// 交错 XOR」的密钥材料解出数据，做一遍与字符串池相同的 32 位混洗，然后拿结果当指令表：
		/// 前段是「RVA + 4 字节」的定长补丁，后段是「RVA + 字数 + 若干 int32」的变长补丁，
		/// 每一处都先 VirtualProtect 改可写、WriteProcessMemory 写入、再恢复原保护。
		/// 目标是**本进程自己**的模块内存（OpenProcess 取的就是当前进程）。
		/// 注意：与 <see cref="解密字符串"/> 一样，依赖的内嵌资源在解壳产物里已不存在，
		/// 所以本方法在本工程中是死代码。
		/// </summary>
		private static void 解密并修补内存数据()
		{
			if (!InstanceMapper.已解密资源数据)
			{
				InstanceMapper.已解密资源数据 = true;
				BinaryReader 资源读取器 = new BinaryReader(typeof(InstanceMapper).Assembly.GetManifestResourceStream("c7c9a35c-0300-44ea-976c-1797ff0710b0"));
				资源读取器.BaseStream.Position = 0L;
				byte[] 密文资源 = 资源读取器.ReadBytes((int)资源读取器.BaseStream.Length);
				// 原版对同一下标连续赋值多次，只有最后一次生效；此处按最终值展开成字面量
				byte[] 密钥材料 = new byte[]
				{
					16, 138, 229, 88, 173, 121, 66, 133, 3, 200, 212, 243, 108, 136, 5, 6,
					105, 64, 79, 178, 156, 100, 65, 27, 106, 52, 111, 204, 159, 111, 213, 110
				};
				// 同上：反复赋值后只剩最后一组有效
				byte[] 初始向量 = new byte[]
				{
					175, 162, 0, 246, 134, 40, 164, 103, 123, 0, 236, 201, 206, 62, 84, 207
				};
				byte[] 向量缓冲 = 初始向量;
				Array.Reverse(向量缓冲);
				byte[] 公钥标记 = typeof(InstanceMapper).Assembly.GetName().GetPublicKeyToken();
				if (公钥标记 != null && 公钥标记.Length > 0)
				{
					向量缓冲[1] = 公钥标记[0];
					向量缓冲[3] = 公钥标记[1];
					向量缓冲[5] = 公钥标记[2];
					向量缓冲[7] = 公钥标记[3];
					向量缓冲[9] = 公钥标记[4];
					向量缓冲[11] = 公钥标记[5];
					向量缓冲[13] = 公钥标记[6];
					向量缓冲[15] = 公钥标记[7];
					Array.Clear(公钥标记, 0, 公钥标记.Length);
				}
				for (int i = 0; i < 向量缓冲.Length; i++)
				{
					密钥材料[i] ^= 向量缓冲[i];
				}
				byte[] 数据缓冲 = 密文资源;
				int 余数 = 数据缓冲.Length % 4;
				int 处理字数 = 数据缓冲.Length / 4;
				byte[] 明文缓冲 = new byte[数据缓冲.Length];
				int 密钥字数 = 密钥材料.Length / 4;
				uint 累加器 = 0U;
				if (余数 > 0)
				{
					处理字数++;
				}
				for (int 字序号 = 0; 字序号 < 处理字数; 字序号++)
				{
					int 密钥字序号 = 字序号 % 密钥字数;
					int 输出偏移 = 字序号 * 4;
					uint 字节偏移 = (uint)(密钥字序号 * 4);
					uint 密钥字 = (uint)(((int)密钥材料[(int)((UIntPtr)(字节偏移 + 3U))] << 24) | ((int)密钥材料[(int)((UIntPtr)(字节偏移 + 2U))] << 16) | ((int)密钥材料[(int)((UIntPtr)(字节偏移 + 1U))] << 8) | (int)密钥材料[(int)((UIntPtr)字节偏移)]);
					uint 输出掩码 = 255U;
					int 输出移位 = 0;
					uint 数据字;
					if (字序号 == 处理字数 - 1 && 余数 > 0)
					{
						累加器 += 密钥字;
						数据字 = 0U;
						for (int k = 0; k < 余数; k++)
						{
							if (k > 0)
							{
								数据字 <<= 8;
							}
							数据字 |= (uint)数据缓冲[数据缓冲.Length - (1 + k)];
						}
					}
					else
					{
						字节偏移 = (uint)输出偏移;
						累加器 += 密钥字;
						数据字 = (uint)(((int)数据缓冲[(int)((UIntPtr)(字节偏移 + 3U))] << 24) | ((int)数据缓冲[(int)((UIntPtr)(字节偏移 + 2U))] << 16) | ((int)数据缓冲[(int)((UIntPtr)(字节偏移 + 1U))] << 8) | (int)数据缓冲[(int)((UIntPtr)字节偏移)]);
					}
					// 原版此处有一条 `num4 = num4;` 自赋值（无副作用），已按噪声删除
					uint 种子 = 累加器;
					uint 混合值 = 累加器;
					混合值 ^= 混合值 >> 25;
					混合值 += 1394801171U;
					混合值 ^= 混合值 << 5;
					混合值 += 3457341307U;
					混合值 ^= 混合值 >> 3;
					混合值 += 4040842541U;
					混合值 = 2018246656U + 混合值;
					累加器 = 种子 + (uint)混合值;
					if (字序号 == 处理字数 - 1 && 余数 > 0)
					{
						uint 输出值 = 累加器 ^ 数据字;
						for (int l = 0; l < 余数; l++)
						{
							if (l > 0)
							{
								输出掩码 <<= 8;
								输出移位 += 8;
							}
							明文缓冲[输出偏移 + l] = (byte)((输出值 & 输出掩码) >> 输出移位);
						}
					}
					else
					{
						uint 输出值 = 累加器 ^ 数据字;
						明文缓冲[输出偏移] = (byte)(输出值 & 255U);
						明文缓冲[输出偏移 + 1] = (byte)((输出值 & 65280U) >> 8);
						明文缓冲[输出偏移 + 2] = (byte)((输出值 & 16711680U) >> 16);
						明文缓冲[输出偏移 + 3] = (byte)((输出值 & 4278190080U) >> 24);
					}
				}
				byte[] 结果数据 = 明文缓冲;
				Array.Clear(向量缓冲, 0, 向量缓冲.Length);
				资源读取器.Close();
				资源读取器 = new BinaryReader(new MemoryStream(结果数据));
				资源读取器.BaseStream.Position = 0L;
				IntPtr 补丁进程句柄 = IntPtr.Zero;
				Assembly 程序集 = typeof(InstanceMapper).Assembly;
				补丁进程句柄 = InstanceMapper.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				InstanceMapper.模块基址64 = Marshal.GetHINSTANCE(程序集.GetModules()[0]).ToInt64();
				IntPtr 写入字节数 = IntPtr.Zero;
				int 旧保护 = 0;
				int 补丁项数 = 资源读取器.ReadInt32();
				资源读取器.ReadInt32();
				for (int m = 0; m < 补丁项数; m++)
				{
					IntPtr 目标地址 = new IntPtr(InstanceMapper.模块基址64 + (long)资源读取器.ReadInt32());
					InstanceMapper.VirtualProtect(目标地址, 4, 4, ref 旧保护);
					// 原版 x86 / x64 两个分支代码完全相同（冗余），此处保留等价写法
					if (IntPtr.Size == 4)
					{
						InstanceMapper.WriteProcessMemory(补丁进程句柄, 目标地址, BitConverter.GetBytes(资源读取器.ReadInt32()), 4U, out 写入字节数);
					}
					else
					{
						InstanceMapper.WriteProcessMemory(补丁进程句柄, 目标地址, BitConverter.GetBytes(资源读取器.ReadInt32()), 4U, out 写入字节数);
					}
					InstanceMapper.VirtualProtect(目标地址, 4, 旧保护, ref 旧保护);
				}
				try
				{
					while (资源读取器.BaseStream.Position < 资源读取器.BaseStream.Length - 1L)
					{
						int 段偏移 = 资源读取器.ReadInt32();
						IntPtr 段地址 = new IntPtr(InstanceMapper.模块基址64 + (long)段偏移);
						int 段字数 = 资源读取器.ReadInt32();
						InstanceMapper.VirtualProtect(段地址, 段字数 * 4, 4, ref 旧保护);
						for (int n = 0; n < 段字数; n++)
						{
							Marshal.WriteInt32(new IntPtr(段地址.ToInt64() + (long)(n * 4)), 资源读取器.ReadInt32());
						}
						InstanceMapper.VirtualProtect(段地址, 段字数 * 4, 旧保护, ref 旧保护);
					}
					InstanceMapper.CloseHandle(补丁进程句柄);
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00006FB4 File Offset: 0x000051B4
		/// <summary>
		/// 尽力取程序集的磁盘路径，三种方法依次尝试：`Assembly.Location`、
		/// `GetName().CodeBase` 去掉 `file:///` 前缀、以及反射读 `Location` 属性。
		/// 每条都套在独立的 try/catch 里（原版逐条忽略错误），全部失败返回空串。
		/// 注意这里没有把 `file:///` 做 URL 反转义（`%20` 之类），与原版一致。
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

		// Token: 0x0600008C RID: 140
		/// <summary>写目标进程内存（P/Invoke），用于 <see cref="解密并修补内存数据"/> 的补丁写入。</summary>
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr 进程句柄, IntPtr 地址, [In] [Out] byte[] 缓冲, uint 大小, out IntPtr 已写入字节数);

		// Token: 0x0600008D RID: 141
		/// <summary>读目标进程内存（P/Invoke），用于 <see cref="解密字符串"/> 从自身模块读密文。</summary>
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr 进程句柄, IntPtr 地址, [In] [Out] byte[] 缓冲, uint 大小, out IntPtr 已读取字节数);

		// Token: 0x0600008E RID: 142
		/// <summary>打开进程句柄（P/Invoke）。本类里两次都是打开**当前进程**自己。</summary>
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint 权限, int 继承句柄, uint 进程ID);

		// Token: 0x0600008F RID: 143
		/// <summary>关闭句柄（P/Invoke）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr 句柄);

		// Token: 0x06000090 RID: 144 RVA: 0x000070C4 File Offset: 0x000052C4
		/// <summary>
		/// 把一个文件整块读进 byte[]。按 `fileStream.Length` 一次性分配，然后循环 Read 直到读满；
		/// 若文件被并发截断导致提前读到 0，这里的 while 会一直空转到 Read 返回 0 后
		/// `i -= 0` 不变——原版就有这个隐患，未做「美化」修正。本工程内无调用点。
		/// </summary>
		private static byte[] 读取整个文件(string 路径)
		{
			byte[] 结果;
			using (FileStream 文件流 = new FileStream(路径, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				int 已读 = 0;
				long 文件长度 = 文件流.Length;
				int 剩余 = (int)文件长度;
				结果 = new byte[剩余];
				while (剩余 > 0)
				{
					int 本次读取 = 文件流.Read(结果, 已读, 剩余);
					已读 += 本次读取;
					剩余 -= 本次读取;
				}
			}
			return 结果;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000712C File Offset: 0x0000052C
		/// <summary>
		/// AES-CBC 解密（固定 key 32 字节 / IV 16 字节，硬编码在方法里），用于解出
		/// <see cref="解密字符串"/> 从内存读到的那段密文。key 与 IV 都是常量，
		/// 等于把密钥写死在代码里——拿到样本就能解出全部被加密的字符串。
		/// </summary>
		private static byte[] AES解密字符串数据(byte[] 密文)
		{
			MemoryStream 内存流 = new MemoryStream();
			SymmetricAlgorithm 算法 = InstanceMapper.创建对称算法();
			算法.Key = new byte[]
			{
				131, 59, 128, 221, 191, 132, 36, 220, 123, 225,
				14, 253, 25, 179, 108, 140, 17, 4, 125, 17,
				193, 199, 53, 34, 245, 77, 13, 21, 233, 241,
				143, 121
			};
			算法.IV = new byte[]
			{
				59, 187, 243, 217, 9, 186, 19, 194, 82, 62,
				165, 9, 19, 216, 237, 157
			};
			CryptoStream 解密流 = new CryptoStream(内存流, 算法.CreateDecryptor(), CryptoStreamMode.Write);
			解密流.Write(密文, 0, 密文.Length);
			解密流.Close();
			return 内存流.ToArray();
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000719C File Offset: 0x0000059C
		/// <summary>垃圾方法：恒返回 null，无调用点。</summary>
		private byte[] 空实现占位1()
		{
			return null;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000719C File Offset: 0x0000059C
		/// <summary>垃圾方法：恒返回 null，无调用点。</summary>
		private byte[] 空实现占位2()
		{
			return null;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000719C File Offset: 0x0000059C
		/// <summary>垃圾方法：恒返回 null，无调用点。</summary>
		private byte[] 空实现占位3()
		{
			return null;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000719C File Offset: 0x0000059C
		/// <summary>垃圾方法：恒返回 null，无调用点。</summary>
		private byte[] 空实现占位4()
		{
			return null;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000071AC File Offset: 0x000005AC
		/// <summary>垃圾方法：恒返回 null，无调用点。</summary>
		private byte[] 空实现占位5()
		{
			return null;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000071AC File Offset: 0x000005AC
		/// <summary>垃圾方法：恒返回 null，无调用点。</summary>
		private byte[] 空实现占位6()
		{
			return null;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000071BC File Offset: 0x000005BC
		/// <summary>
		/// 混淆器插入的占位方法：硬编码 GUID 串 `{11111-22222-40001-00001}`，
		/// 判断其长度后**两个分支返回同一个值** `new byte[] { 1, 2 }`。
		/// 这类方法在 .NET Reactor 产物里成组出现，用于占位/迷惑，无实际行为。
		/// </summary>
		internal byte[] 占位方法1()
		{
			string 标记串 = "{11111-22222-40001-00001}";
			if (标记串.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000071FC File Offset: 0x000005FC
		/// <summary>占位方法：硬编码 `{11111-22222-40001-00002}`，两分支返回同值。</summary>
		internal byte[] 占位方法2()
		{
			string 标记串 = "{11111-22222-40001-00002}";
			if (标记串.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000723C File Offset: 0x0000063C
		/// <summary>占位方法：硬编码 `{11111-22222-50001-00001}`，两分支返回同值。</summary>
		internal byte[] 占位方法3()
		{
			string 标记串 = "{11111-22222-50001-00001}";
			if (标记串.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000727C File Offset: 0x0000067C
		/// <summary>占位方法：硬编码 `{11111-22222-50001-00002}`，两分支返回同值。</summary>
		internal byte[] 占位方法4()
		{
			string 标记串 = "{11111-22222-50001-00002}";
			if (标记串.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000072BC File Offset: 0x000006BC
		/// <summary>占位方法：硬编码 `{11111-22222-60001-00001}`，两分支返回同值。</summary>
		internal byte[] 占位方法5()
		{
			string 标记串 = "{11111-22222-60001-00001}";
			if (标记串.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000072FC File Offset: 0x000006FC
		/// <summary>占位方法：硬编码 `{11111-22222-60001-00002}`，两分支返回同值。</summary>
		internal byte[] 占位方法6()
		{
			string 标记串 = "{11111-22222-60001-00002}";
			if (标记串.Length > 0)
			{
				return new byte[] { 1, 2 };
			}
			return new byte[] { 1, 2 };
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000733C File Offset: 0x0000073C
		/// <summary>
		/// AES-CBC **加密**（并把结果转 Base64）。key 是 32 字节硬编码常量，IV 由
		/// <see cref="MD5摘要"/> 对第二个参数取摘要得到（16 字节，正好当 IV）。
		/// 与 <see cref="AES解密字符串数据"/> 共用同一把 key，所以两者可以互相换算。
		/// 本工程内无调用点；原样本里用于把配置/凭据加密后塞进注册表或内存。
		/// </summary>
		internal static string AES加密字符串(string 明文, string 密钥种子)
		{
			byte[] 明文字节 = Encoding.Unicode.GetBytes(明文);
			byte[] 明文字节别名 = 明文字节;
			byte[] 密钥 = new byte[]
			{
				82, 102, 104, 110, 32, 77, 24, 34, 118, 181,
				51, 17, 18, 51, 12, 109, 10, 32, 77, 24,
				34, 158, 161, 41, 97, 28, 118, 181, 5, 25,
				1, 88
			};
			byte[] 初始向量 = InstanceMapper.MD5摘要(Encoding.Unicode.GetBytes(密钥种子));
			MemoryStream 内存流 = new MemoryStream();
			SymmetricAlgorithm 算法 = InstanceMapper.创建对称算法();
			算法.Key = 密钥;
			算法.IV = 初始向量;
			CryptoStream 加密流 = new CryptoStream(内存流, 算法.CreateEncryptor(), CryptoStreamMode.Write);
			加密流.Write(明文字节别名, 0, 明文字节别名.Length);
			加密流.Close();
			return Convert.ToBase64String(内存流.ToArray());
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>恒返回 true 的桩方法，无调用点。</summary>
		internal static bool 返回真占位()
		{
			return true;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>恒返回 false 的桩方法，无调用点。</summary>
		internal static bool 返回假占位()
		{
			return false;
		}

		// Token: 0x04000028 RID: 40
		/// <summary>MD5 的常量表 T[1..64]（标准值，此处按 0 基存放，索引时用「常量索引 - 1」）。</summary>
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

		// Token: 0x04000029 RID: 41
		/// <summary>未被读取的字节缓冲（未知用途残留）。</summary>
		private static byte[] 未使用缓冲1 = new byte[0];

		// Token: 0x0400002A RID: 42
		/// <summary>字符串池数据（相对偏移表 + 密文），由 <see cref="解密字符串"/> 填充。</summary>
		private static byte[] 字符串池数据 = new byte[0];

		// Token: 0x0400002B RID: 43
		/// <summary>当前进程句柄（ReadProcessMemory 用），首次使用时懒打开。</summary>
		private static IntPtr 当前进程句柄 = IntPtr.Zero;

		// Token: 0x0400002C RID: 44
		/// <summary>资源数据是否已经解密过（<see cref="解密并修补内存数据"/> 的一次性开关）。</summary>
		private static bool 已解密资源数据 = false;

		// Token: 0x0400002D RID: 45
		/// <summary>已解密字符串的缓存数组。</summary>
		private static string[] 字符串缓存 = new string[0];

		// Token: 0x0400002E RID: 46
		/// <summary>未被读取的字节缓冲（未知用途残留）。</summary>
		private static byte[] 未使用缓冲2 = new byte[0];

		// Token: 0x0400002F RID: 47
		/// <summary>32 位进程下的模块基址（Marshal.GetHINSTANCE）。</summary>
		private static int 模块基址32 = 0;

		// Token: 0x04000030 RID: 48
		/// <summary>FIPS 策略是否已经读过了。</summary>
		private static bool 已读取FIPS策略 = false;

		// Token: 0x04000031 RID: 49
		/// <summary>本机是否仅允许 FIPS 算法（读自 CryptoConfig.AllowOnlyFipsAlgorithms）。</summary>
		private static bool 仅允许FIPS算法 = false;

		// Token: 0x04000032 RID: 50
		/// <summary>下一个可用的字符串缓存槽位（从 1 开始，0 表示「没缓存」）。</summary>
		private static int 下一个缓存槽 = 1;

		// Token: 0x04000033 RID: 51
		/// <summary>未被读取的字节缓冲（未知用途残留）。</summary>
		private static byte[] 未使用缓冲3 = new byte[0];

		// Token: 0x04000034 RID: 52
		/// <summary>ReadProcessMemory 的「实际读到的字节数」出参存放处（原版直接复用静态字段）。</summary>
		private static IntPtr 读取字节数 = IntPtr.Zero;

		// Token: 0x04000035 RID: 53
		/// <summary>64 位进程下的模块基址（Marshal.GetHINSTANCE）。</summary>
		private static long 模块基址64 = 0L;

		// Token: 0x04000036 RID: 54
		/// <summary>字符串索引 → 缓存槽位的映射表。</summary>
		private static int[] 缓存槽索引 = new int[0];

		// Token: 0x02000011 RID: 17
		/// <summary>
		/// 混淆器自带的占位 Attribute：只用于给方法贴标记（本类里贴给了
		/// <see cref="解密并修补内存数据"/>），构造函数忽略入参，不做任何事。
		/// </summary>
		internal class GetterMapper : Attribute
		{
			// Token: 0x060000A2 RID: 162 RVA: 0x00002576 File Offset: 0x00000776
			/// <summary>空构造函数：入参被完全忽略。</summary>
			[InstanceMapper.GetterMapper(typeof(InstanceMapper.GetterMapper.ObserverMapper<object>[]))]
			public GetterMapper(object 构造参数)
			{
			}

			// Token: 0x02000012 RID: 18
			/// <summary>占位内部类，只有两个恒值桩方法。</summary>
			internal class ObserverMapper<T>
			{
				// Token: 0x060000A4 RID: 164 RVA: 0x000021DD File Offset: 0x000003DD
				/// <summary>恒返回 true 的桩方法，无调用点。</summary>
				internal static bool 返回真占位()
				{
					return true;
				}

				// Token: 0x060000A5 RID: 165 RVA: 0x000021E8 File Offset: 0x000003E8
				/// <summary>恒返回 false 的桩方法，无调用点。</summary>
				internal static bool 返回假占位()
				{
					return false;
				}
			}
		}

		// Token: 0x02000013 RID: 19
		/// <summary>空枚举（没有任何成员），混淆器/壳的残留，无引用。</summary>
		[Flags]
		private enum FSCOMPRESULT
		{
		}
	}
}
