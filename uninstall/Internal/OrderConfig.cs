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
	// <summary>
	/// .NET Reactor 加壳运行时支撑代码（加壳器残留，不参与卸载业务逻辑），承载：
	/// 1) 反篡改（Anti-Tamper）：ApplyAntiTamperPatch 用 OpenProcess + VirtualProtect +
	///    WriteProcessMemory 在内存中修补/改写当前进程的方法体；
	/// 2) 加密字符串解密：DecryptEmbeddedString 解密嵌入资源中的混淆字符串
	///    （本程序集内"随机进程名"等字符串即由此还原）；
	/// 3) MD5/AES 工具：FIPS 模式自适应（ComputeMd5Managed 自带 MD5 实现）、
	///    AesEncryptString 字符串加密、AesDecryptHardcodedKey 解密。
	/// </summary>
	internal class OrderConfig
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00005014 File Offset: 0x00003214
		/// <summary>静态构造函数：初始化各缓存/标志字段（数组、句柄、计数等）。</summary>
		static OrderConfig()
		{
			OrderConfig._ObserverConfig = new byte[0];
			OrderConfig.m_GlobalConfig = new byte[0];
			OrderConfig._decryptedStringCache = new byte[0];
			OrderConfig.parserConfig = new byte[0];
			OrderConfig._ValueConfig = IntPtr.Zero;
			OrderConfig._TokenConfig = IntPtr.Zero;
			OrderConfig._RegistryConfig = new string[0];
			OrderConfig.queueConfig = new int[0];
			OrderConfig.modelConfig = 1;
			OrderConfig._moduleBaseAddress = 0L;
			OrderConfig.m_PolicyConfig = 0;
			OrderConfig._antiTamperApplied = false;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000229D File Offset: 0x0000049D
		/// <summary>空方法（混淆噪声），无实际逻辑。</summary>
		private void method_0()
		{
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000050BC File Offset: 0x000032BC
		/// <summary>
		/// 自实现标准 MD5 压缩函数（原混淆名 SelectUtils）：
		/// 消息填充（补 0x80 + 64 位长度）、64 轮 F/G/H/I 变换（使用 md5SineConstants
		/// 正弦常数表）、小端输出 16 字节摘要。首常数 3614090360（0xD76AA478）确认标准 MD5。
		/// </summary>
		internal static byte[] ComputeMd5Managed(object object_0)
		{
			uint[] array = new uint[16];
			uint num = (uint)((448 - object_0.Length * 8 % 512 + 512) % 512);
			if (num == 0U)
			{
				num = 512U;
			}
			uint num2 = (uint)((long)object_0.Length + (long)((ulong)(num / 8U)) + 8L);
			ulong num3 = (ulong)((long)object_0.Length * 8L);
			byte[] array2 = new byte[num2];
			for (int i = 0; i < object_0.Length; i++)
			{
				array2[i] = object_0[i];
			}
			byte[] array3 = array2;
			int num4 = object_0.Length;
			array3[num4] |= 128;
			for (int j = 8; j > 0; j--)
			{
				array2[(int)(checked((IntPtr)(unchecked((ulong)num2 - (ulong)((long)j)))))] = (byte)(num3 >> (8 - j) * 8 & 255UL);
			}
			uint num5 = (uint)(array2.Length * 8 / 32);
			uint num6 = 1732584193U;
			uint num7 = 4023233417U;
			uint num8 = 2562383102U;
			uint num9 = 271733878U;
			for (uint num10 = 0U; num10 < num5 / 16U; num10 += 1U)
			{
				uint num11 = num10 << 6;
				for (uint num12 = 0U; num12 < 61U; num12 += 4U)
				{
					array[(int)(num12 >> 2)] = (uint)((int)array2[(int)(num11 + (num12 + 3U))] << 24 | (int)array2[(int)(num11 + (num12 + 2U))] << 16 | (int)array2[(int)(num11 + (num12 + 1U))] << 8 | (int)array2[(int)(num11 + num12)]);
				}
				uint num13 = num6;
				uint num14 = num7;
				uint num15 = num8;
				uint num16 = num9;
				OrderConfig.Md5RoundF(ref num6, num7, num8, num9, 0U, 7, 1U, array);
				OrderConfig.Md5RoundF(ref num9, num6, num7, num8, 1U, 12, 2U, array);
				OrderConfig.Md5RoundF(ref num8, num9, num6, num7, 2U, 17, 3U, array);
				OrderConfig.Md5RoundF(ref num7, num8, num9, num6, 3U, 22, 4U, array);
				OrderConfig.Md5RoundF(ref num6, num7, num8, num9, 4U, 7, 5U, array);
				OrderConfig.Md5RoundF(ref num9, num6, num7, num8, 5U, 12, 6U, array);
				OrderConfig.Md5RoundF(ref num8, num9, num6, num7, 6U, 17, 7U, array);
				OrderConfig.Md5RoundF(ref num7, num8, num9, num6, 7U, 22, 8U, array);
				OrderConfig.Md5RoundF(ref num6, num7, num8, num9, 8U, 7, 9U, array);
				OrderConfig.Md5RoundF(ref num9, num6, num7, num8, 9U, 12, 10U, array);
				OrderConfig.Md5RoundF(ref num8, num9, num6, num7, 10U, 17, 11U, array);
				OrderConfig.Md5RoundF(ref num7, num8, num9, num6, 11U, 22, 12U, array);
				OrderConfig.Md5RoundF(ref num6, num7, num8, num9, 12U, 7, 13U, array);
				OrderConfig.Md5RoundF(ref num9, num6, num7, num8, 13U, 12, 14U, array);
				OrderConfig.Md5RoundF(ref num8, num9, num6, num7, 14U, 17, 15U, array);
				OrderConfig.Md5RoundF(ref num7, num8, num9, num6, 15U, 22, 16U, array);
				OrderConfig.Md5RoundG(ref num6, num7, num8, num9, 1U, 5, 17U, array);
				OrderConfig.Md5RoundG(ref num9, num6, num7, num8, 6U, 9, 18U, array);
				OrderConfig.Md5RoundG(ref num8, num9, num6, num7, 11U, 14, 19U, array);
				OrderConfig.Md5RoundG(ref num7, num8, num9, num6, 0U, 20, 20U, array);
				OrderConfig.Md5RoundG(ref num6, num7, num8, num9, 5U, 5, 21U, array);
				OrderConfig.Md5RoundG(ref num9, num6, num7, num8, 10U, 9, 22U, array);
				OrderConfig.Md5RoundG(ref num8, num9, num6, num7, 15U, 14, 23U, array);
				OrderConfig.Md5RoundG(ref num7, num8, num9, num6, 4U, 20, 24U, array);
				OrderConfig.Md5RoundG(ref num6, num7, num8, num9, 9U, 5, 25U, array);
				OrderConfig.Md5RoundG(ref num9, num6, num7, num8, 14U, 9, 26U, array);
				OrderConfig.Md5RoundG(ref num8, num9, num6, num7, 3U, 14, 27U, array);
				OrderConfig.Md5RoundG(ref num7, num8, num9, num6, 8U, 20, 28U, array);
				OrderConfig.Md5RoundG(ref num6, num7, num8, num9, 13U, 5, 29U, array);
				OrderConfig.Md5RoundG(ref num9, num6, num7, num8, 2U, 9, 30U, array);
				OrderConfig.Md5RoundG(ref num8, num9, num6, num7, 7U, 14, 31U, array);
				OrderConfig.Md5RoundG(ref num7, num8, num9, num6, 12U, 20, 32U, array);
				OrderConfig.Md5RoundH(ref num6, num7, num8, num9, 5U, 4, 33U, array);
				OrderConfig.Md5RoundH(ref num9, num6, num7, num8, 8U, 11, 34U, array);
				OrderConfig.Md5RoundH(ref num8, num9, num6, num7, 11U, 16, 35U, array);
				OrderConfig.Md5RoundH(ref num7, num8, num9, num6, 14U, 23, 36U, array);
				OrderConfig.Md5RoundH(ref num6, num7, num8, num9, 1U, 4, 37U, array);
				OrderConfig.Md5RoundH(ref num9, num6, num7, num8, 4U, 11, 38U, array);
				OrderConfig.Md5RoundH(ref num8, num9, num6, num7, 7U, 16, 39U, array);
				OrderConfig.Md5RoundH(ref num7, num8, num9, num6, 10U, 23, 40U, array);
				OrderConfig.Md5RoundH(ref num6, num7, num8, num9, 13U, 4, 41U, array);
				OrderConfig.Md5RoundH(ref num9, num6, num7, num8, 0U, 11, 42U, array);
				OrderConfig.Md5RoundH(ref num8, num9, num6, num7, 3U, 16, 43U, array);
				OrderConfig.Md5RoundH(ref num7, num8, num9, num6, 6U, 23, 44U, array);
				OrderConfig.Md5RoundH(ref num6, num7, num8, num9, 9U, 4, 45U, array);
				OrderConfig.Md5RoundH(ref num9, num6, num7, num8, 12U, 11, 46U, array);
				OrderConfig.Md5RoundH(ref num8, num9, num6, num7, 15U, 16, 47U, array);
				OrderConfig.Md5RoundH(ref num7, num8, num9, num6, 2U, 23, 48U, array);
				OrderConfig.Md5RoundI(ref num6, num7, num8, num9, 0U, 6, 49U, array);
				OrderConfig.Md5RoundI(ref num9, num6, num7, num8, 7U, 10, 50U, array);
				OrderConfig.Md5RoundI(ref num8, num9, num6, num7, 14U, 15, 51U, array);
				OrderConfig.Md5RoundI(ref num7, num8, num9, num6, 5U, 21, 52U, array);
				OrderConfig.Md5RoundI(ref num6, num7, num8, num9, 12U, 6, 53U, array);
				OrderConfig.Md5RoundI(ref num9, num6, num7, num8, 3U, 10, 54U, array);
				OrderConfig.Md5RoundI(ref num8, num9, num6, num7, 10U, 15, 55U, array);
				OrderConfig.Md5RoundI(ref num7, num8, num9, num6, 1U, 21, 56U, array);
				OrderConfig.Md5RoundI(ref num6, num7, num8, num9, 8U, 6, 57U, array);
				OrderConfig.Md5RoundI(ref num9, num6, num7, num8, 15U, 10, 58U, array);
				OrderConfig.Md5RoundI(ref num8, num9, num6, num7, 6U, 15, 59U, array);
				OrderConfig.Md5RoundI(ref num7, num8, num9, num6, 13U, 21, 60U, array);
				OrderConfig.Md5RoundI(ref num6, num7, num8, num9, 4U, 6, 61U, array);
				OrderConfig.Md5RoundI(ref num9, num6, num7, num8, 11U, 10, 62U, array);
				OrderConfig.Md5RoundI(ref num8, num9, num6, num7, 2U, 15, 63U, array);
				OrderConfig.Md5RoundI(ref num7, num8, num9, num6, 9U, 21, 64U, array);
				num6 += num13;
				num7 += num14;
				num8 += num15;
				num9 += num16;
			}
			byte[] array4 = new byte[16];
			Array.Copy(BitConverter.GetBytes(num6), 0, array4, 0, 4);
			Array.Copy(BitConverter.GetBytes(num7), 0, array4, 4, 4);
			Array.Copy(BitConverter.GetBytes(num8), 0, array4, 8, 4);
			Array.Copy(BitConverter.GetBytes(num9), 0, array4, 12, 4);
			return array4;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000229F File Offset: 0x0000049F
		/// <summary>MD5 第一轮 F 变换（原 GetUtils）：F(x,y,z) = (x&y)|(~x&z)。</summary>
		private static void Md5RoundF(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + OrderConfig.Md5LeftRotate(uint_0 + ((uint_1 & uint_2) | (~uint_1 & uint_3)) + object_0[(int)uint_4] + OrderConfig.md5SineConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000022C8 File Offset: 0x000004C8
		/// <summary>MD5 第二轮 G 变换（原 DisableUtils）：G(x,y,z) = (x&z)|(y&~z)。</summary>
		private static void Md5RoundG(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + OrderConfig.Md5LeftRotate(uint_0 + ((uint_1 & uint_3) | (uint_2 & ~uint_3)) + object_0[(int)uint_4] + OrderConfig.md5SineConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000022F1 File Offset: 0x000004F1
		/// <summary>MD5 第三轮 H 变换（原 CalculateUtils）：H(x,y,z) = x^y^z。</summary>
		private static void Md5RoundH(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + OrderConfig.Md5LeftRotate(uint_0 + (uint_1 ^ uint_2 ^ uint_3) + object_0[(int)uint_4] + OrderConfig.md5SineConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00002317 File Offset: 0x00000517
		/// <summary>MD5 第四轮 I 变换（原 ConcatUtils）：I(x,y,z) = y^(x|~z)。</summary>
		private static void Md5RoundI(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + OrderConfig.Md5LeftRotate(uint_0 + (uint_2 ^ (uint_1 | ~uint_3)) + object_0[(int)uint_4] + OrderConfig.md5SineConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000233E File Offset: 0x0000053E
		/// <summary>MD5 循环左移（原 ForgotUtils）：对 uint 循环左移 ushort_0 位。</summary>
		private static uint Md5LeftRotate(uint uint_0, ushort ushort_0)
		{
			return uint_0 >> (int)(32 - ushort_0) | uint_0 << (int)ushort_0;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002350 File Offset: 0x00000550
		/// <summary>
		/// FIPS 是否启用缓存（原 CancelUtils）：首次调用触发 DetectFipsPolicy 探测，
		/// 结果缓存于 _fipsEnabledCache，后续直接返回。
		/// </summary>
		internal static bool IsFipsEnabled()
		{
			if (!OrderConfig._fipsDetectionDone)
			{
				OrderConfig.DetectFipsPolicy();
				OrderConfig._fipsDetectionDone = true;
			}
			return OrderConfig._fipsEnabledCache;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000571C File Offset: 0x0000391C
		/// <summary>
		/// 探测 FIPS 策略（原 FindUtils）：反射读 CryptoConfig.AllowOnlyFipsAlgorithms；
		/// 失败则回退反射读 Utils.FipsAlgorithmPolicy（==1 视为启用）。
		/// </summary>
		internal static void DetectFipsPolicy()
		{
			try
			{
				OrderConfig._fipsEnabledCache = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
				try
				{
					if ((int)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.Utils", false).GetMethod("get_FipsAlgorithmPolicy", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[0]) == 1)
					{
						OrderConfig._fipsEnabledCache = true;
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000057CC File Offset: 0x000039CC
		/// <summary>
		/// 获取对称算法实例（原 AwakeUtils）：FIPS 环境优先用 System.Core 3.5 的
		/// AesCryptoServiceProvider，否则（或创建失败）用 RijndaelManaged。
		/// </summary>
		internal static SymmetricAlgorithm CreateSymmetricAlgorithm()
		{
			SymmetricAlgorithm result = null;
			if (OrderConfig.IsFipsEnabled())
			{
				try
				{
					return (SymmetricAlgorithm)Activator.CreateInstance(Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089").GetType("System.Security.Cryptography.AesCryptoServiceProvider"));
				}
				catch
				{
					return new RijndaelManaged();
				}
			}
			result = new RijndaelManaged();
			return result;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00002369 File Offset: 0x00000569
		/// <summary>
		/// 计算 MD5 摘要（原 ReflectUtils）：非 FIPS 走 MD5CryptoServiceProvider，
		/// FIPS 走自实现 ComputeMd5Managed（被 PoolConfig 用于嵌入资源命名哈希）。
		/// </summary>
		internal static byte[] ComputeMd5Hash(byte[] byte_0)
		{
			if (!OrderConfig.IsFipsEnabled())
			{
				return new MD5CryptoServiceProvider().ComputeHash(byte_0);
			}
			return OrderConfig.ComputeMd5Managed(byte_0);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005824 File Offset: 0x00003A24
		/// <summary>
		/// 解密嵌入字符串资源（原 ResetUtils）：读取资源 c0682d03-f5b2-467a-9e64-46ffd27adaa2，
		/// 用内嵌密钥（异或公钥令牌）AES-CBC 解密，再做一轮自定义 32 位滚动 XOR 流解密，
		/// 结果缓存于 _decryptedStringCache；按 int_0 偏移读 4 字节长度 + Unicode 字符串返回。
		/// 这是 Reactor "字符串加密"的还原器，供被混淆字符串动态解码。
		/// </summary>
		static string DecryptEmbeddedString(int int_0)
		{
			if (OrderConfig._decryptedStringCache.Length == 0)
			{
				BinaryReader binaryReader = new BinaryReader(typeof(OrderConfig).Assembly.GetManifestResourceStream("c0682d03-f5b2-467a-9e64-46ffd27adaa2"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 103;
				array2[0] = 110;
				array2[0] = 13;
				array2[1] = 98;
				array2[1] = 111;
				array2[1] = 47;
				array2[2] = 153;
				array2[2] = 146;
				array2[2] = 101;
				array2[2] = 93;
				array2[2] = 146;
				array2[2] = 184;
				array2[3] = 109;
				array2[3] = 150;
				array2[3] = 187;
				array2[3] = 97;
				array2[3] = 125;
				array2[3] = 72;
				array2[4] = 224;
				array2[4] = 144;
				array2[4] = 215;
				array2[4] = 17;
				array2[4] = 156;
				array2[4] = 168;
				array2[5] = 105;
				array2[5] = 125;
				array2[5] = 50;
				array2[5] = 172;
				array2[5] = 107;
				array2[5] = 207;
				array2[6] = 88;
				array2[6] = 56;
				array2[6] = 50;
				array2[7] = 111;
				array2[7] = 85;
				array2[7] = 71;
				array2[7] = 158;
				array2[7] = 18;
				array2[8] = 128;
				array2[8] = 165;
				array2[8] = 54;
				array2[8] = 209;
				array2[9] = 92;
				array2[9] = 73;
				array2[9] = 110;
				array2[9] = 144;
				array2[10] = 59;
				array2[10] = 90;
				array2[10] = 120;
				array2[10] = 155;
				array2[11] = 136;
				array2[11] = 166;
				array2[11] = 132;
				array2[11] = 164;
				array2[11] = 149;
				array2[12] = 146;
				array2[12] = 97;
				array2[12] = 137;
				array2[12] = 227;
				array2[13] = 206;
				array2[13] = 107;
				array2[13] = 108;
				array2[13] = 53;
				array2[14] = 122;
				array2[14] = 121;
				array2[14] = 127;
				array2[14] = 157;
				array2[14] = 102;
				array2[14] = 80;
				array2[15] = 104;
				array2[15] = 122;
				array2[15] = 136;
				array2[15] = 109;
				array2[15] = 1;
				array2[16] = 112;
				array2[16] = 86;
				array2[16] = 166;
				array2[16] = 127;
				array2[16] = 67;
				array2[16] = 194;
				array2[17] = 142;
				array2[17] = 109;
				array2[17] = 89;
				array2[17] = 132;
				array2[17] = 170;
				array2[17] = 161;
				array2[18] = 92;
				array2[18] = 185;
				array2[18] = 138;
				array2[18] = 94;
				array2[18] = 95;
				array2[18] = 142;
				array2[19] = 73;
				array2[19] = 144;
				array2[19] = 88;
				array2[19] = 81;
				array2[19] = 134;
				array2[19] = 131;
				array2[20] = 151;
				array2[20] = 174;
				array2[20] = 74;
				array2[21] = 112;
				array2[21] = 128;
				array2[21] = 131;
				array2[21] = 140;
				array2[21] = 173;
				array2[21] = 213;
				array2[22] = 107;
				array2[22] = 43;
				array2[22] = 43;
				array2[22] = 155;
				array2[22] = 47;
				array2[23] = 154;
				array2[23] = 214;
				array2[23] = 154;
				array2[23] = 190;
				array2[24] = 153;
				array2[24] = 179;
				array2[24] = 154;
				array2[24] = 149;
				array2[24] = 69;
				array2[25] = 104;
				array2[25] = 127;
				array2[25] = 104;
				array2[25] = 242;
				array2[26] = 120;
				array2[26] = 164;
				array2[26] = 29;
				array2[26] = 150;
				array2[26] = 5;
				array2[27] = 96;
				array2[27] = 43;
				array2[27] = 164;
				array2[27] = 143;
				array2[27] = 37;
				array2[28] = 58;
				array2[28] = 115;
				array2[28] = 204;
				array2[28] = 26;
				array2[28] = 120;
				array2[28] = 89;
				array2[29] = 96;
				array2[29] = 227;
				array2[29] = 158;
				array2[29] = 104;
				array2[29] = 134;
				array2[29] = 35;
				array2[30] = 162;
				array2[30] = 163;
				array2[30] = 63;
				array2[30] = 206;
				array2[31] = 69;
				array2[31] = 122;
				array2[31] = 5;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 103;
				array4[0] = 55;
				array4[0] = 100;
				array4[0] = 34;
				array4[0] = 122;
				array4[0] = 63;
				array4[1] = 153;
				array4[1] = 123;
				array4[1] = 110;
				array4[1] = 110;
				array4[1] = 108;
				array4[1] = 74;
				array4[2] = 104;
				array4[2] = 199;
				array4[2] = 17;
				array4[2] = 134;
				array4[3] = 148;
				array4[3] = 95;
				array4[3] = 160;
				array4[3] = 147;
				array4[4] = 169;
				array4[4] = 122;
				array4[4] = 102;
				array4[4] = 150;
				array4[4] = 110;
				array4[5] = 105;
				array4[5] = 167;
				array4[5] = 145;
				array4[5] = 119;
				array4[5] = 146;
				array4[5] = 45;
				array4[6] = 158;
				array4[6] = 76;
				array4[6] = 146;
				array4[6] = 176;
				array4[7] = 84;
				array4[7] = 112;
				array4[7] = 116;
				array4[7] = 110;
				array4[7] = 172;
				array4[7] = 252;
				array4[8] = 84;
				array4[8] = 92;
				array4[8] = 125;
				array4[8] = 123;
				array4[9] = 163;
				array4[9] = 101;
				array4[9] = 66;
				array4[9] = 166;
				array4[9] = 98;
				array4[9] = 170;
				array4[10] = 142;
				array4[10] = 136;
				array4[10] = 213;
				array4[10] = 119;
				array4[10] = 106;
				array4[10] = 141;
				array4[11] = 144;
				array4[11] = 98;
				array4[11] = 123;
				array4[11] = 163;
				array4[11] = 250;
				array4[12] = 122;
				array4[12] = 108;
				array4[12] = 120;
				array4[12] = 164;
				array4[12] = 216;
				array4[13] = 152;
				array4[13] = 102;
				array4[13] = 110;
				array4[13] = 104;
				array4[13] = 10;
				array4[14] = 99;
				array4[14] = 117;
				array4[14] = 107;
				array4[14] = 170;
				array4[15] = 44;
				array4[15] = 159;
				array4[15] = 90;
				array4[15] = 111;
				array4[15] = 8;
				byte[] array5 = array4;
				byte[] publicKeyToken = typeof(OrderConfig).Assembly.GetName().GetPublicKeyToken();
				if (publicKeyToken != null && publicKeyToken.Length != 0)
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
					SymmetricAlgorithm symmetricAlgorithm = OrderConfig.CreateSymmetricAlgorithm();
					symmetricAlgorithm.Mode = CipherMode.CBC;
					ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor(array3, array5);
					MemoryStream memoryStream = new MemoryStream();
					CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
					cryptoStream.Write(array, 0, array.Length);
					cryptoStream.FlushFinalBlock();
					OrderConfig._decryptedStringCache = memoryStream.ToArray();
					memoryStream.Close();
					cryptoStream.Close();
					binaryReader.Close();
					array = OrderConfig._decryptedStringCache;
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
					uint num8 = (uint)((int)array3[(int)(num7 + 3U)] << 24 | (int)array3[(int)(num7 + 2U)] << 16 | (int)array3[(int)(num7 + 1U)] << 8 | (int)array3[(int)num7]);
					uint num9 = 255U;
					int num10 = 0;
					uint num11;
					if (j == num2 - 1 && num > 0)
					{
						num11 = 0U;
						num4 += num8;
						for (int k = 0; k < num; k++)
						{
							if (k > 0)
							{
								num11 <<= 8;
							}
							num11 |= (uint)array[array.Length - (1 + k)];
						}
					}
					else
					{
						num4 += num8;
						num7 = (uint)num6;
						num11 = (uint)((int)array[(int)(num7 + 3U)] << 24 | (int)array[(int)(num7 + 2U)] << 16 | (int)array[(int)(num7 + 1U)] << 8 | (int)array[(int)num7]);
					}
					uint num12 = num4;
					num4 += 1U;
					uint num13 = num12;
					uint num14 = num12;
					num14 ^= num14 << 5;
					num14 += 3753412436U;
					num14 ^= num14 << 6;
					num14 += 678874538U;
					num14 ^= num14 >> 13;
					num14 += 179542347U;
					num14 = 522623986U - num14;
					num12 = num13 + (uint)num14;
					num4 = num12;
					if (j == num2 - 1 && num > 0)
					{
						uint num15 = num4 ^ num11;
						for (int l = 0; l < num; l++)
						{
							if (l > 0)
							{
								num9 <<= 8;
								num10 += 8;
							}
							array6[num6 + l] = (byte)((num15 & num9) >> num10);
						}
					}
					else
					{
						uint num16 = num4 ^ num11;
						array6[num6] = (byte)(num16 & 255U);
						array6[num6 + 1] = (byte)((num16 & 65280U) >> 8);
						array6[num6 + 2] = (byte)((num16 & 16711680U) >> 16);
						array6[num6 + 3] = (byte)((num16 & 4278190080U) >> 24);
					}
				}
				OrderConfig._decryptedStringCache = array6;
			}
			int num17 = BitConverter.ToInt32(OrderConfig._decryptedStringCache, int_0);
			try
			{
				byte[] array7 = new byte[num17];
				Array.Copy(OrderConfig._decryptedStringCache, int_0 + 4, array7, 0, num17);
				return Encoding.Unicode.GetString(array7, 0, array7.Length);
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000683C File Offset: 0x00004A3C
		/// <summary>Base64 → Unicode 字符串（原 PostUtils）：解码 Base64 后按 UTF-16 还原。</summary>
		internal static string Base64ToUnicodeString(string string_0)
		{
			byte[] array = Convert.FromBase64String(string_0);
			return Encoding.Unicode.GetString(array, 0, array.Length);
		}

		// Token: 0x06000077 RID: 119
		/// <summary>kernel32 VirtualProtect P/Invoke（反篡改补丁改页权限用）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr intptr_0, int int_0, int int_1, ref int int_2);

		// Token: 0x06000078 RID: 120 RVA: 0x00006860 File Offset: 0x00004A60
		/// <summary>
		/// 反篡改补丁（原 AddUtils，.NET Reactor Anti-Tamper；由模块初始化器调用）：
		/// 1) 读嵌入资源 0d7bf7c5-ac22-45a5-a386-00782fb0dad7；
		/// 2) 用 32 字节密钥流与 16 字节 IV（与程序集公钥令牌异或、IV 反转）解密，
		///    再做一轮自定义 32 位滚动 XOR 流解密得到补丁数据；
		/// 3) OpenProcess(56=PROCESS_ALL_ACCESS, 1, 当前PID) 打开自身进程；
		/// 4) 按补丁数据中的 RVA 偏移：VirtualProtect 改页权限 →
		///    WriteProcessMemory / Marshal.WriteInt32 写入 → 恢复权限
		///    （典型 Reactor 内存补丁：IAT 修正/校验和自校验）；
		/// 5) CloseHandle，异常被吞掉。_moduleBaseAddress 为模块基址（HINSTANCE）。
		/// </summary>
		[OrderConfig.PageConfig(typeof(OrderConfig.PageConfig.RepositoryConfig<object>[]))]
		static void ApplyAntiTamperPatch()
		{
			if (!OrderConfig._antiTamperApplied)
			{
				OrderConfig._antiTamperApplied = true;
				BinaryReader binaryReader = new BinaryReader(typeof(OrderConfig).Assembly.GetManifestResourceStream("0d7bf7c5-ac22-45a5-a386-00782fb0dad7"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 198;
				array2[0] = 149;
				array2[0] = 115;
				array2[0] = 146;
				array2[0] = 198;
				array2[1] = 15;
				array2[1] = 96;
				array2[1] = 148;
				array2[1] = 201;
				array2[1] = 16;
				array2[1] = 15;
				array2[2] = 31;
				array2[2] = 110;
				array2[2] = 114;
				array2[2] = 31;
				array2[3] = 79;
				array2[3] = 118;
				array2[3] = 183;
				array2[3] = 123;
				array2[3] = 204;
				array2[3] = 79;
				array2[4] = 99;
				array2[4] = 99;
				array2[4] = 138;
				array2[4] = 211;
				array2[4] = 128;
				array2[4] = 99;
				array2[5] = 49;
				array2[5] = 154;
				array2[5] = 144;
				array2[5] = 143;
				array2[5] = 49;
				array2[6] = 81;
				array2[6] = 208;
				array2[6] = 147;
				array2[6] = 81;
				array2[7] = 20;
				array2[7] = 114;
				array2[7] = 75;
				array2[7] = 20;
				array2[8] = 240;
				array2[8] = 103;
				array2[8] = 191;
				array2[8] = 198;
				array2[8] = 240;
				array2[9] = 11;
				array2[9] = 72;
				array2[9] = 144;
				array2[9] = 11;
				array2[10] = 197;
				array2[10] = 162;
				array2[10] = 225;
				array2[10] = 197;
				array2[11] = 193;
				array2[11] = 124;
				array2[11] = 129;
				array2[11] = 193;
				array2[12] = 57;
				array2[12] = 128;
				array2[12] = 141;
				array2[12] = 89;
				array2[12] = 97;
				array2[12] = 134;
				array2[12] = 57;
				array2[13] = 156;
				array2[13] = 127;
				array2[13] = 100;
				array2[13] = 153;
				array2[13] = 156;
				array2[14] = 157;
				array2[14] = 51;
				array2[14] = 183;
				array2[14] = 100;
				array2[14] = 112;
				array2[14] = 128;
				array2[14] = 157;
				array2[15] = 14;
				array2[15] = 158;
				array2[15] = 126;
				array2[15] = 182;
				array2[15] = 88;
				array2[15] = 14;
				array2[16] = 137;
				array2[16] = 100;
				array2[16] = 156;
				array2[16] = 137;
				array2[17] = 226;
				array2[17] = 163;
				array2[17] = 64;
				array2[17] = 146;
				array2[17] = 226;
				array2[18] = 4;
				array2[18] = 201;
				array2[18] = 145;
				array2[18] = 129;
				array2[18] = 4;
				array2[19] = 110;
				array2[19] = 179;
				array2[19] = 94;
				array2[19] = 64;
				array2[19] = 110;
				array2[20] = 194;
				array2[20] = 125;
				array2[20] = 134;
				array2[20] = 118;
				array2[20] = 164;
				array2[20] = 194;
				array2[21] = 125;
				array2[21] = 150;
				array2[21] = 140;
				array2[21] = 148;
				array2[21] = 125;
				array2[22] = 132;
				array2[22] = 112;
				array2[22] = 95;
				array2[22] = 137;
				array2[22] = 150;
				array2[22] = 132;
				array2[23] = 90;
				array2[23] = 95;
				array2[23] = 129;
				array2[23] = 163;
				array2[23] = 158;
				array2[23] = 90;
				array2[24] = 170;
				array2[24] = 70;
				array2[24] = 116;
				array2[24] = 170;
				array2[25] = 132;
				array2[25] = 170;
				array2[25] = 56;
				array2[25] = 86;
				array2[25] = 150;
				array2[25] = 121;
				array2[25] = 132;
				array2[26] = 245;
				array2[26] = 144;
				array2[26] = 150;
				array2[26] = 245;
				array2[27] = 64;
				array2[27] = 114;
				array2[27] = 160;
				array2[27] = 99;
				array2[27] = 156;
				array2[27] = 64;
				array2[28] = 116;
				array2[28] = 129;
				array2[28] = 162;
				array2[28] = 116;
				array2[29] = 17;
				array2[29] = 111;
				array2[29] = 98;
				array2[29] = 161;
				array2[29] = 17;
				array2[30] = 160;
				array2[30] = 165;
				array2[30] = 172;
				array2[30] = 16;
				array2[30] = 160;
				array2[31] = 32;
				array2[31] = 132;
				array2[31] = 121;
				array2[31] = 32;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 149;
				array4[0] = 115;
				array4[0] = 192;
				array4[1] = 146;
				array4[1] = 135;
				array4[1] = 143;
				array4[2] = 201;
				array4[2] = 95;
				array4[2] = 213;
				array4[2] = 119;
				array4[3] = 141;
				array4[3] = 81;
				array4[3] = 183;
				array4[3] = 250;
				array4[4] = 152;
				array4[4] = 109;
				array4[4] = 74;
				array4[5] = 112;
				array4[5] = 138;
				array4[5] = 158;
				array4[5] = 146;
				array4[5] = 132;
				array4[5] = 137;
				array4[6] = 124;
				array4[6] = 96;
				array4[6] = 204;
				array4[7] = 102;
				array4[7] = 149;
				array4[7] = 126;
				array4[7] = 127;
				array4[7] = 148;
				array4[8] = 91;
				array4[8] = 191;
				array4[8] = 167;
				array4[8] = 228;
				array4[9] = 112;
				array4[9] = 35;
				array4[9] = 145;
				array4[9] = 167;
				array4[10] = 64;
				array4[10] = 228;
				array4[10] = 107;
				array4[10] = 58;
				array4[10] = 78;
				array4[11] = 228;
				array4[11] = 123;
				array4[11] = 209;
				array4[11] = 100;
				array4[11] = 237;
				array4[12] = 153;
				array4[12] = 127;
				array4[12] = 111;
				array4[12] = 144;
				array4[12] = 51;
				array4[12] = 208;
				array4[13] = 145;
				array4[13] = 100;
				array4[13] = 121;
				array4[13] = 87;
				array4[13] = 184;
				array4[14] = 108;
				array4[14] = 151;
				array4[14] = 69;
				array4[14] = 148;
				array4[14] = 182;
				array4[15] = 167;
				array4[15] = 38;
				array4[15] = 166;
				array4[15] = 156;
				array4[15] = 113;
				array4[15] = 185;
				byte[] array5 = array4;
				Array.Reverse(array5);
				byte[] publicKeyToken = typeof(OrderConfig).Assembly.GetName().GetPublicKeyToken();
				if (publicKeyToken != null && publicKeyToken.Length != 0)
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
					uint num8 = (uint)((int)array3[(int)(num7 + 3U)] << 24 | (int)array3[(int)(num7 + 2U)] << 16 | (int)array3[(int)(num7 + 1U)] << 8 | (int)array3[(int)num7]);
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
						num11 = (uint)((int)array6[(int)(num7 + 3U)] << 24 | (int)array6[(int)(num7 + 2U)] << 16 | (int)array6[(int)(num7 + 1U)] << 8 | (int)array6[(int)num7]);
					}
					num4 = num4;
					uint num12 = num4;
					uint num13 = num4;
					num13 ^= num13 << 5;
					num13 += 3753412436U;
					num13 ^= num13 << 6;
					num13 += 678874538U;
					num13 ^= num13 >> 13;
					num13 += 179542347U;
					num13 = 522623986U - num13;
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
				byte[] buffer = array7;
				Array.Clear(array5, 0, array5.Length);
				binaryReader.Close();
				binaryReader = new BinaryReader(new MemoryStream(buffer));
				binaryReader.BaseStream.Position = 0L;
				IntPtr intptr_ = IntPtr.Zero;
				Assembly assembly = typeof(OrderConfig).Assembly;
				intptr_ = OrderConfig.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				OrderConfig._moduleBaseAddress = Marshal.GetHINSTANCE(assembly.GetModules()[0]).ToInt64();
				IntPtr zero = IntPtr.Zero;
				int int_ = 0;
				int num16 = binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				for (int m = 0; m < num16; m++)
				{
					IntPtr intPtr = new IntPtr(OrderConfig._moduleBaseAddress + (long)binaryReader.ReadInt32());
					OrderConfig.VirtualProtect(intPtr, 4, 4, ref int_);
					if (IntPtr.Size == 4)
					{
						OrderConfig.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					else
					{
						OrderConfig.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					OrderConfig.VirtualProtect(intPtr, 4, int_, ref int_);
				}
				try
				{
					while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1L)
					{
						int num17 = binaryReader.ReadInt32();
						IntPtr intptr_2 = new IntPtr(OrderConfig._moduleBaseAddress + (long)num17);
						int num18 = binaryReader.ReadInt32();
						OrderConfig.VirtualProtect(intptr_2, num18 * 4, 4, ref int_);
						for (int n = 0; n < num18; n++)
						{
							Marshal.WriteInt32(new IntPtr(intptr_2.ToInt64() + (long)(n * 4)), binaryReader.ReadInt32());
						}
						OrderConfig.VirtualProtect(intptr_2, num18 * 4, int_, ref int_);
					}
					OrderConfig.CloseHandle(intptr_);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00007A20 File Offset: 0x00005C20
		/// <summary>
		/// 获取程序集路径（原 FillUtils）：Location / CodeBase（去 file:/// 前缀）/ 反射
		/// Location 属性多级回退解析，全部失败返回空字符串。
		/// </summary>
		internal static object GetAssemblyPath(Assembly assembly_0)
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
				if (File.Exists(assembly_0.GetType().GetProperty("Location").GetValue(assembly_0, new object[0]).ToString()))
				{
					return assembly_0.GetType().GetProperty("Location").GetValue(assembly_0, new object[0]).ToString();
				}
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x0600007A RID: 122
		/// <summary>kernel32 WriteProcessMemory P/Invoke（反篡改补丁写入内存用）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x0600007B RID: 123
		/// <summary>kernel32 ReadProcessMemory P/Invoke（声明但反篡改流程未实际调用）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x0600007C RID: 124
		/// <summary>kernel32 OpenProcess P/Invoke（反篡改补丁打开自身进程用）。</summary>
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint uint_0, int int_0, uint uint_1);

		// Token: 0x0600007D RID: 125
		/// <summary>kernel32 CloseHandle P/Invoke（反篡改补丁关闭句柄用）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr intptr_0);

		// Token: 0x0600007E RID: 126 RVA: 0x00007B30 File Offset: 0x00005D30
		/// <summary>读文件全部字节（原 RevertUtils）：FileStream 循环读取直至读完。</summary>
		private static byte[] ReadAllBytesFromFile(string string_0)
		{
			byte[] array;
			using (FileStream fileStream = new FileStream(string_0, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				int num = 0;
				int i = (int)fileStream.Length;
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

		// Token: 0x0600007F RID: 127 RVA: 0x00007B90 File Offset: 0x00005D90
		/// <summary>
		/// AES 解密（原 EnableConfig，硬编码密钥）：Key={146,223,56,9,...}（32 字节）、
		/// IV={202,43,33,137,...}（16 字节），AES-CBC 解密返回明文。
		/// </summary>
		private static byte[] AesDecryptHardcodedKey(byte[] byte_0)
		{
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = OrderConfig.CreateSymmetricAlgorithm();
			symmetricAlgorithm.Key = new byte[]
			{
				146,
				223,
				56,
				9,
				185,
				188,
				22,
				1,
				133,
				71,
				61,
				23,
				227,
				32,
				176,
				37,
				97,
				86,
				197,
				158,
				196,
				105,
				25,
				48,
				125,
				132,
				48,
				87,
				172,
				233,
				196,
				118
			};
			symmetricAlgorithm.IV = new byte[]
			{
				202,
				43,
				33,
				137,
				214,
				188,
				131,
				55,
				105,
				168,
				114,
				229,
				84,
				3,
				147,
				39
			};
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(byte_0, 0, byte_0.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00007BFC File Offset: 0x00005DFC
		/// <summary>假方法桩（原 DestroyConfig）：恒返回 null，混淆噪声。</summary>
		private byte[] StubReturnNull1()
		{
			return null;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00007BFC File Offset: 0x00005DFC
		/// <summary>假方法桩（原 StopConfig）：恒返回 null，混淆噪声。</summary>
		private byte[] StubReturnNull2()
		{
			return null;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00007BFC File Offset: 0x00005DFC
		/// <summary>假方法桩（原 SortConfig）：恒返回 null，混淆噪声。</summary>
		private byte[] StubReturnNull3()
		{
			return null;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00007BFC File Offset: 0x00005DFC
		/// <summary>假方法桩（原 CollectConfig）：恒返回 null，混淆噪声。</summary>
		private byte[] StubReturnNull4()
		{
			return null;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00007BFC File Offset: 0x00005DFC
		/// <summary>假方法桩（原 FlushConfig）：恒返回 null，混淆噪声。</summary>
		private byte[] StubReturnNull5()
		{
			return null;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00007BFC File Offset: 0x00005DFC
		/// <summary>假方法桩（原 LogoutConfig）：恒返回 null，混淆噪声。</summary>
		private byte[] StubReturnNull6()
		{
			return null;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00002384 File Offset: 0x00000584
		/// <summary>假方法桩（原 PopConfig）：恒返回 {1,2}，混淆噪声（假 GUID 串无意义）。</summary>
		internal byte[] StubReturnBytePair1()
		{
			int length = "{11111-22222-40001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000239F File Offset: 0x0000059F
		/// <summary>假方法桩（原 ListConfig）：恒返回 {1,2}，混淆噪声。</summary>
		internal byte[] StubReturnBytePair2()
		{
			int length = "{11111-22222-40001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000023BA File Offset: 0x000005BA
		/// <summary>假方法桩（原 VerifyConfig）：恒返回 {1,2}，混淆噪声。</summary>
		internal byte[] StubReturnBytePair3()
		{
			int length = "{11111-22222-50001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000023D5 File Offset: 0x000005D5
		/// <summary>假方法桩（原 MapConfig）：恒返回 {1,2}，混淆噪声。</summary>
		internal byte[] StubReturnBytePair4()
		{
			int length = "{11111-22222-50001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000023F0 File Offset: 0x000005F0
		/// <summary>假方法桩（原 CallConfig）：恒返回 {1,2}，混淆噪声。</summary>
		internal byte[] StubReturnBytePair5()
		{
			int length = "{11111-22222-60001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000240B File Offset: 0x0000060B
		/// <summary>假方法桩（原 IncludeConfig）：恒返回 {1,2}，混淆噪声。</summary>
		internal byte[] StubReturnBytePair6()
		{
			int length = "{11111-22222-60001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00007C0C File Offset: 0x00005E0C
		/// <summary>
		/// AES 加密字符串（原 PublishConfig）：Key={82,102,104,110,32,77,24,34,...}（32 字节），
		/// IV = MD5(UTF16(salt))，AES-CBC 加密后输出 Base64——用于运行时生成加密串。
		/// </summary>
		internal static string AesEncryptString(string string_0, string string_1)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(string_0);
			byte[] key = new byte[]
			{
				82,
				102,
				104,
				110,
				32,
				77,
				24,
				34,
				118,
				181,
				51,
				17,
				18,
				51,
				12,
				109,
				10,
				32,
				77,
				24,
				34,
				158,
				161,
				41,
				97,
				28,
				118,
				181,
				5,
				25,
				1,
				88
			};
			byte[] iv = OrderConfig.ComputeMd5Hash(Encoding.Unicode.GetBytes(string_1));
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = OrderConfig.CreateSymmetricAlgorithm();
			symmetricAlgorithm.Key = key;
			symmetricAlgorithm.IV = iv;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.Close();
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>恒 true 混淆桩，无实际逻辑。</summary>
		internal static bool ConcatRegistry()
		{
			return true;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>恒 false 混淆桩，无实际逻辑。</summary>
		internal static bool UpdateRegistry()
		{
			return false;
		}

		// Token: 0x04000031 RID: 49
		/// <summary>MD5 正弦常数表（64 项，首值 3614090360 = 0xD76AA478，确认标准 MD5）。</summary>
		private static uint[] md5SineConstants = new uint[]
		{
			3614090360U,
			3905402710U,
			606105819U,
			3250441966U,
			4118548399U,
			1200080426U,
			2821735955U,
			4249261313U,
			1770035416U,
			2336552879U,
			4294925233U,
			2304563134U,
			1804603682U,
			4254626195U,
			2792965006U,
			1236535329U,
			4129170786U,
			3225465664U,
			643717713U,
			3921069994U,
			3593408605U,
			38016083U,
			3634488961U,
			3889429448U,
			568446438U,
			3275163606U,
			4107603335U,
			1163531501U,
			2850285829U,
			4243563512U,
			1735328473U,
			2368359562U,
			4294588738U,
			2272392833U,
			1839030562U,
			4259657740U,
			2763975236U,
			1272893353U,
			4139469664U,
			3200236656U,
			681279174U,
			3936430074U,
			3572445317U,
			76029189U,
			3654602809U,
			3873151461U,
			530742520U,
			3299628645U,
			4096336452U,
			1126891415U,
			2878612391U,
			4237533241U,
			1700485571U,
			2399980690U,
			4293915773U,
			2240044497U,
			1873313359U,
			4264355552U,
			2734768916U,
			1309151649U,
			4149444226U,
			3174756917U,
			718787259U,
			3951481745U
		};

		// Token: 0x04000032 RID: 50
		/// <summary>未使用的占位字段（静态构造初始化，无业务引用）。</summary>
		private static byte[] parserConfig;

		// Token: 0x04000033 RID: 51
		/// <summary>未使用的占位字段（静态构造初始化，无业务引用）。</summary>
		private static string[] _RegistryConfig;

		// Token: 0x04000034 RID: 52
		/// <summary>未使用的占位字段（静态构造初始化，无业务引用）。</summary>
		private static int[] queueConfig;

		// Token: 0x04000035 RID: 53
		/// <summary>模块基址（HINSTANCE，原 _CreatorConfig）：反篡改补丁按 RVA 计算写入地址用。</summary>
		private static long _moduleBaseAddress;

		// Token: 0x04000036 RID: 54
		/// <summary>未使用的占位字段（静态构造初始化，无业务引用）。</summary>
		private static IntPtr _ValueConfig;

		// Token: 0x04000037 RID: 55
		/// <summary>未使用的占位字段（静态构造初始化，无业务引用）。</summary>
		private static int m_PolicyConfig;

		// Token: 0x04000038 RID: 56
		/// <summary>解密后的嵌入字符串资源缓存（原 _CollectionConfig）：DecryptEmbeddedString 结果。</summary>
		private static byte[] _decryptedStringCache;

		// Token: 0x04000039 RID: 57
		/// <summary>未使用的占位字段（静态构造初始化，无业务引用）。</summary>
		private static byte[] m_GlobalConfig;

		// Token: 0x0400003A RID: 58
		/// <summary>反篡改补丁是否已执行标志（原 m_ObjectConfig）：ApplyAntiTamperPatch 幂等保护。</summary>
		private static bool _antiTamperApplied;

		// Token: 0x0400003B RID: 59
		/// <summary>未使用的占位字段（静态构造初始化，无业务引用）。</summary>
		private static byte[] _ObserverConfig;

		// Token: 0x0400003C RID: 60
		/// <summary>FIPS 策略是否已探测标志（原 _DefinitionConfig）。</summary>
		private static bool _fipsDetectionDone = false;

		// Token: 0x0400003D RID: 61
		/// <summary>FIPS 是否启用缓存（原 m_AttributeConfig）：IsFipsEnabled 返回该值。</summary>
		private static bool _fipsEnabledCache = false;

		// Token: 0x0400003E RID: 62
		/// <summary>未使用的占位字段（静态构造初始化，无业务引用）。</summary>
		private static int modelConfig;

		// Token: 0x0400003F RID: 63
		/// <summary>未使用的占位字段（静态构造初始化，无业务引用）。</summary>
		private static IntPtr _TokenConfig;

		// Token: 0x02000013 RID: 19
		/// <summary>
		/// Reactor 标记特性（标识被保护成员），嵌套的 RepositoryConfig&lt;T&gt; 为空壳桩类。
		/// </summary>
		internal class PageConfig : Attribute
		{
			// Token: 0x06000090 RID: 144 RVA: 0x00002426 File Offset: 0x00000626
			/// <summary>特性构造函数：空实现。</summary>
			[OrderConfig.PageConfig(typeof(OrderConfig.PageConfig.RepositoryConfig<object>[]))]
			public PageConfig(object object_0)
			{
			}

			// Token: 0x02000014 RID: 20
			/// <summary>空壳桩类（无字段/逻辑），加壳器标记用。</summary>
			internal class RepositoryConfig<T>
			{
				// Token: 0x06000092 RID: 146 RVA: 0x000021D1 File Offset: 0x000003D1
				/// <summary>恒 true 混淆桩，无实际逻辑。</summary>
				internal static bool SelectRegistry()
				{
					return true;
				}

				// Token: 0x06000093 RID: 147 RVA: 0x000021C6 File Offset: 0x000003C6
				/// <summary>恒 false 混淆桩，无实际逻辑。</summary>
				internal static bool SearchRegistry()
				{
					return false;
				}
			}
		}

		// Token: 0x02000015 RID: 21
		/// <summary>空壳桩枚举（无成员），混淆噪声。</summary>
		[Flags]
		private enum ImporterConfig
		{

		}
	}
}
