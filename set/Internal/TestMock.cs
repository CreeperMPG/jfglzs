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
		/// 加壳运行时（.NET Reactor / Eziriz IntelliLock 家族，混杂 ConfuserEx 风格重命名）：
		/// 1) DecryptStringTable(offset)：解密嵌入资源 dfa6a901-... 字符串表，壳把程序字符串字面量替换为表偏移；
		/// 2) ModuleInit_ApplyMemoryPatch：解密资源 e6ab89dd-... 后用 OpenProcess+VirtualProtect+WriteProcessMemory 改写自身模块内存（脱壳/反篡改补丁）；
		/// 3) 密码学原语：手写 MD5（ComputeMd5_Manual + 四个轮函数 + _md5Constants 常量表）、FIPS 探测、AES/Rijndael 选择；
		/// 4) EncryptFixedKey_ToBase64/RijndaelDecrypt_FixedKey：固定密钥加解密（疑似注册码/载荷解密路径）。
		/// </summary>
	// Token: 0x02000013 RID: 19
	internal class TestMock
	{
		/// <summary>
		/// 静态构造：初始化全部静态占位字段（布尔/字节数组/IntPtr/标量）为默认值。
		/// </summary>
		// Token: 0x060000FA RID: 250 RVA: 0x0000BDBC File Offset: 0x00009FBC
		static TestMock()
		{
			TestMock._fipsProbeDone = false;
			TestMock._fipsEnabled = false;
			TestMock._placeholderByteArray_1 = new byte[0];
			TestMock._placeholderByteArray_3 = new byte[0];
			TestMock._decryptedStringTable = new byte[0];
			TestMock._placeholderByteArray_2 = new byte[0];
			TestMock._placeholderIntPtr_1 = IntPtr.Zero;
			TestMock._placeholderIntPtr_2 = IntPtr.Zero;
			TestMock._placeholderStringArray = new string[0];
			TestMock._placeholderIntArray = new int[0];
			TestMock._placeholderScalarInt_1 = 1;
			TestMock._moduleBaseAddress = 0L;
			TestMock._placeholderScalarInt_0 = 0;
			TestMock._moduleInitDone = false;
		}

		/// <summary>
		/// 空占位方法（混淆残留）：方法体为空。
		/// </summary>
		// Token: 0x060000FB RID: 251 RVA: 0x00002479 File Offset: 0x00000679
		private void EmptyPlaceholder_0()
		{
		}

		/// <summary>
		/// 手写 MD5 实现（标准 MD5 主循环）：初值 1732584193/4023233417/2562383102/271733878，64 轮
		/// （Md5Round_F/G/H/I 分别对应 F/G/H/I 轮函数，配合 _md5Constants 64 个标准常量与 RotateLeft 循环左移），输出 16 字节摘要。
		/// </summary>
		// Token: 0x060000FC RID: 252 RVA: 0x0000BE64 File Offset: 0x0000A064
		internal static byte[] ComputeMd5_Manual(object data)
		{
			uint[] array = new uint[16];
			uint num = (uint)((448 - data.Length * 8 % 512 + 512) % 512);
			if (num == 0U)
			{
				num = 512U;
			}
			uint num2 = (uint)((long)data.Length + (long)((ulong)(num / 8U)) + 8L);
			ulong num3 = (ulong)((long)data.Length * 8L);
			byte[] array2 = new byte[num2];
			for (int i = 0; i < data.Length; i++)
			{
				array2[i] = data[i];
			}
			byte[] array3 = array2;
			int num4 = data.Length;
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
				TestMock.Md5Round_F(ref num6, num7, num8, num9, 0U, 7, 1U, array);
				TestMock.Md5Round_F(ref num9, num6, num7, num8, 1U, 12, 2U, array);
				TestMock.Md5Round_F(ref num8, num9, num6, num7, 2U, 17, 3U, array);
				TestMock.Md5Round_F(ref num7, num8, num9, num6, 3U, 22, 4U, array);
				TestMock.Md5Round_F(ref num6, num7, num8, num9, 4U, 7, 5U, array);
				TestMock.Md5Round_F(ref num9, num6, num7, num8, 5U, 12, 6U, array);
				TestMock.Md5Round_F(ref num8, num9, num6, num7, 6U, 17, 7U, array);
				TestMock.Md5Round_F(ref num7, num8, num9, num6, 7U, 22, 8U, array);
				TestMock.Md5Round_F(ref num6, num7, num8, num9, 8U, 7, 9U, array);
				TestMock.Md5Round_F(ref num9, num6, num7, num8, 9U, 12, 10U, array);
				TestMock.Md5Round_F(ref num8, num9, num6, num7, 10U, 17, 11U, array);
				TestMock.Md5Round_F(ref num7, num8, num9, num6, 11U, 22, 12U, array);
				TestMock.Md5Round_F(ref num6, num7, num8, num9, 12U, 7, 13U, array);
				TestMock.Md5Round_F(ref num9, num6, num7, num8, 13U, 12, 14U, array);
				TestMock.Md5Round_F(ref num8, num9, num6, num7, 14U, 17, 15U, array);
				TestMock.Md5Round_F(ref num7, num8, num9, num6, 15U, 22, 16U, array);
				TestMock.Md5Round_G(ref num6, num7, num8, num9, 1U, 5, 17U, array);
				TestMock.Md5Round_G(ref num9, num6, num7, num8, 6U, 9, 18U, array);
				TestMock.Md5Round_G(ref num8, num9, num6, num7, 11U, 14, 19U, array);
				TestMock.Md5Round_G(ref num7, num8, num9, num6, 0U, 20, 20U, array);
				TestMock.Md5Round_G(ref num6, num7, num8, num9, 5U, 5, 21U, array);
				TestMock.Md5Round_G(ref num9, num6, num7, num8, 10U, 9, 22U, array);
				TestMock.Md5Round_G(ref num8, num9, num6, num7, 15U, 14, 23U, array);
				TestMock.Md5Round_G(ref num7, num8, num9, num6, 4U, 20, 24U, array);
				TestMock.Md5Round_G(ref num6, num7, num8, num9, 9U, 5, 25U, array);
				TestMock.Md5Round_G(ref num9, num6, num7, num8, 14U, 9, 26U, array);
				TestMock.Md5Round_G(ref num8, num9, num6, num7, 3U, 14, 27U, array);
				TestMock.Md5Round_G(ref num7, num8, num9, num6, 8U, 20, 28U, array);
				TestMock.Md5Round_G(ref num6, num7, num8, num9, 13U, 5, 29U, array);
				TestMock.Md5Round_G(ref num9, num6, num7, num8, 2U, 9, 30U, array);
				TestMock.Md5Round_G(ref num8, num9, num6, num7, 7U, 14, 31U, array);
				TestMock.Md5Round_G(ref num7, num8, num9, num6, 12U, 20, 32U, array);
				TestMock.Md5Round_H(ref num6, num7, num8, num9, 5U, 4, 33U, array);
				TestMock.Md5Round_H(ref num9, num6, num7, num8, 8U, 11, 34U, array);
				TestMock.Md5Round_H(ref num8, num9, num6, num7, 11U, 16, 35U, array);
				TestMock.Md5Round_H(ref num7, num8, num9, num6, 14U, 23, 36U, array);
				TestMock.Md5Round_H(ref num6, num7, num8, num9, 1U, 4, 37U, array);
				TestMock.Md5Round_H(ref num9, num6, num7, num8, 4U, 11, 38U, array);
				TestMock.Md5Round_H(ref num8, num9, num6, num7, 7U, 16, 39U, array);
				TestMock.Md5Round_H(ref num7, num8, num9, num6, 10U, 23, 40U, array);
				TestMock.Md5Round_H(ref num6, num7, num8, num9, 13U, 4, 41U, array);
				TestMock.Md5Round_H(ref num9, num6, num7, num8, 0U, 11, 42U, array);
				TestMock.Md5Round_H(ref num8, num9, num6, num7, 3U, 16, 43U, array);
				TestMock.Md5Round_H(ref num7, num8, num9, num6, 6U, 23, 44U, array);
				TestMock.Md5Round_H(ref num6, num7, num8, num9, 9U, 4, 45U, array);
				TestMock.Md5Round_H(ref num9, num6, num7, num8, 12U, 11, 46U, array);
				TestMock.Md5Round_H(ref num8, num9, num6, num7, 15U, 16, 47U, array);
				TestMock.Md5Round_H(ref num7, num8, num9, num6, 2U, 23, 48U, array);
				TestMock.Md5Round_I(ref num6, num7, num8, num9, 0U, 6, 49U, array);
				TestMock.Md5Round_I(ref num9, num6, num7, num8, 7U, 10, 50U, array);
				TestMock.Md5Round_I(ref num8, num9, num6, num7, 14U, 15, 51U, array);
				TestMock.Md5Round_I(ref num7, num8, num9, num6, 5U, 21, 52U, array);
				TestMock.Md5Round_I(ref num6, num7, num8, num9, 12U, 6, 53U, array);
				TestMock.Md5Round_I(ref num9, num6, num7, num8, 3U, 10, 54U, array);
				TestMock.Md5Round_I(ref num8, num9, num6, num7, 10U, 15, 55U, array);
				TestMock.Md5Round_I(ref num7, num8, num9, num6, 1U, 21, 56U, array);
				TestMock.Md5Round_I(ref num6, num7, num8, num9, 8U, 6, 57U, array);
				TestMock.Md5Round_I(ref num9, num6, num7, num8, 15U, 10, 58U, array);
				TestMock.Md5Round_I(ref num8, num9, num6, num7, 6U, 15, 59U, array);
				TestMock.Md5Round_I(ref num7, num8, num9, num6, 13U, 21, 60U, array);
				TestMock.Md5Round_I(ref num6, num7, num8, num9, 4U, 6, 61U, array);
				TestMock.Md5Round_I(ref num9, num6, num7, num8, 11U, 10, 62U, array);
				TestMock.Md5Round_I(ref num8, num9, num6, num7, 2U, 15, 63U, array);
				TestMock.Md5Round_I(ref num7, num8, num9, num6, 9U, 21, 64U, array);
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

		/// <summary>
		/// MD5 轮函数 F：state0 = state1 + RotateLeft(state0 + ((state1 & state2) | (~state1 & state3)) + words[wordIndex] + _md5Constants[constIndex-1], shift)。
		/// </summary>
		// Token: 0x060000FD RID: 253 RVA: 0x0000247B File Offset: 0x0000067B
		private static void Md5Round_F(ref uint state0, uint state1, uint state2, uint state3, uint wordIndex, ushort shift, uint constIndex, object words)
		{
			state0 = state1 + TestMock.RotateLeft(state0 + ((state1 & state2) | (~state1 & state3)) + words[(int)wordIndex] + TestMock._md5Constants[(int)(constIndex - 1U)], shift);
		}

		/// <summary>
		/// MD5 轮函数 G：state0 = state1 + RotateLeft(state0 + ((state1 & state3) | (state2 & ~state3)) + words[wordIndex] + _md5Constants[constIndex-1], shift)。
		/// </summary>
		// Token: 0x060000FE RID: 254 RVA: 0x000024A4 File Offset: 0x000006A4
		private static void Md5Round_G(ref uint state0, uint state1, uint state2, uint state3, uint wordIndex, ushort shift, uint constIndex, object words)
		{
			state0 = state1 + TestMock.RotateLeft(state0 + ((state1 & state3) | (state2 & ~state3)) + words[(int)wordIndex] + TestMock._md5Constants[(int)(constIndex - 1U)], shift);
		}

		/// <summary>
		/// MD5 轮函数 H：state0 = state1 + RotateLeft(state0 + (state1 ^ state2 ^ state3) + words[wordIndex] + _md5Constants[constIndex-1], shift)。
		/// </summary>
		// Token: 0x060000FF RID: 255 RVA: 0x000024CD File Offset: 0x000006CD
		private static void Md5Round_H(ref uint state0, uint state1, uint state2, uint state3, uint wordIndex, ushort shift, uint constIndex, object words)
		{
			state0 = state1 + TestMock.RotateLeft(state0 + (state1 ^ state2 ^ state3) + words[(int)wordIndex] + TestMock._md5Constants[(int)(constIndex - 1U)], shift);
		}

		/// <summary>
		/// MD5 轮函数 I：state0 = state1 + RotateLeft(state0 + (state2 ^ (state1 | ~state3)) + words[wordIndex] + _md5Constants[constIndex-1], shift)。
		/// </summary>
		// Token: 0x06000100 RID: 256 RVA: 0x000024F3 File Offset: 0x000006F3
		private static void Md5Round_I(ref uint state0, uint state1, uint state2, uint state3, uint wordIndex, ushort shift, uint constIndex, object words)
		{
			state0 = state1 + TestMock.RotateLeft(state0 + (state2 ^ (state1 | ~state3)) + words[(int)wordIndex] + TestMock._md5Constants[(int)(constIndex - 1U)], shift);
		}

		/// <summary>
		/// 32 位循环左移：value >> (32-shift) | value << shift。
		/// </summary>
		// Token: 0x06000101 RID: 257 RVA: 0x0000251A File Offset: 0x0000071A
		private static uint RotateLeft(uint value, ushort shift)
		{
			return value >> (int)(32 - shift) | value << (int)shift;
		}

		/// <summary>
		/// 是否强制 FIPS 模式：首次调用先 ProbeFipsPolicy()，返回 _fipsEnabled。
		/// </summary>
		// Token: 0x06000102 RID: 258 RVA: 0x0000252C File Offset: 0x0000072C
		internal static bool IsFipsEnabled()
		{
			if (!TestMock._fipsProbeDone)
			{
				TestMock.ProbeFipsPolicy();
				TestMock._fipsProbeDone = true;
			}
			return TestMock._fipsEnabled;
		}

		/// <summary>
		/// 反射探测 FIPS 策略：CryptoConfig.get_AllowOnlyFipsAlgorithms，失败再试 Utils.get_FipsAlgorithmPolicy==1。
		/// </summary>
		// Token: 0x06000103 RID: 259 RVA: 0x0000C4C4 File Offset: 0x0000A6C4
		internal static void ProbeFipsPolicy()
		{
			try
			{
				TestMock._fipsEnabled = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
				try
				{
					if ((int)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.Utils", false).GetMethod("get_FipsAlgorithmPolicy", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[0]) == 1)
					{
						TestMock._fipsEnabled = true;
					}
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// 取对称算法：FIPS 下用 System.Core 3.5 的 AesCryptoServiceProvider（反射），否则 RijndaelManaged。
		/// </summary>
		// Token: 0x06000104 RID: 260 RVA: 0x0000C574 File Offset: 0x0000A774
		internal static SymmetricAlgorithm GetSymmetricAlgorithm()
		{
			SymmetricAlgorithm result = null;
			if (TestMock.IsFipsEnabled())
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

		/// <summary>
		/// MD5 摘要：非 FIPS 用 MD5CryptoServiceProvider，FIPS 用手写 ComputeMd5_Manual。
		/// </summary>
		// Token: 0x06000105 RID: 261 RVA: 0x00002545 File Offset: 0x00000745
		internal static byte[] ComputeMd5(byte[] byte_0)
		{
			if (!TestMock.IsFipsEnabled())
			{
				return new MD5CryptoServiceProvider().ComputeHash(byte_0);
			}
			return TestMock.ComputeMd5_Manual(byte_0);
		}

		/// <summary>
		/// 字符串解密（壳的字符串表读取）：惰性加载资源 dfa6a901-446d-4781-bab5-dec8b23ded66；
		/// 密钥/IV 构造后与程序集 PublicKeyToken 逐字节 XOR 混合；offset==-1 时先做 Rijndael CBC 解密；
		/// 再做 32 位字流变换解密（x^=x<<5; x+=3753412436; x^=x<<6; x+=678874538; x^=x>>13; x+=179542347; x=522623986-x 密钥流）缓存为 _decryptedStringTable；
		/// 最后 BitConverter.ToInt32 取长度，Encoding.Unicode.GetString 返回明文。壳把所有字符串字面量替换为表偏移调用，调用点即被加密的字符串。
		/// </summary>
		// Token: 0x06000106 RID: 262 RVA: 0x0000C5CC File Offset: 0x0000A7CC
		static string DecryptStringTable(int offset)
		{
			if (TestMock._decryptedStringTable.Length == 0)
			{
				BinaryReader binaryReader = new BinaryReader(typeof(TestMock).Assembly.GetManifestResourceStream("dfa6a901-446d-4781-bab5-dec8b23ded66"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 127;
				array2[0] = 150;
				array2[0] = 141;
				array2[1] = 150;
				array2[1] = 77;
				array2[1] = 56;
				array2[1] = 120;
				array2[1] = 55;
				array2[2] = 144;
				array2[2] = 160;
				array2[2] = 90;
				array2[2] = 43;
				array2[3] = 124;
				array2[3] = 119;
				array2[3] = 242;
				array2[4] = 159;
				array2[4] = 140;
				array2[4] = 148;
				array2[4] = 143;
				array2[5] = 111;
				array2[5] = 100;
				array2[5] = 128;
				array2[5] = 216;
				array2[5] = 146;
				array2[5] = 24;
				array2[6] = 130;
				array2[6] = 82;
				array2[6] = 204;
				array2[7] = 198;
				array2[7] = 25;
				array2[7] = 143;
				array2[7] = 185;
				array2[8] = 118;
				array2[8] = 65;
				array2[8] = 110;
				array2[8] = 70;
				array2[8] = 67;
				array2[9] = 45;
				array2[9] = 47;
				array2[9] = 100;
				array2[9] = 135;
				array2[9] = 30;
				array2[10] = 142;
				array2[10] = 164;
				array2[10] = 102;
				array2[10] = 154;
				array2[10] = 55;
				array2[11] = 151;
				array2[11] = 122;
				array2[11] = 74;
				array2[12] = 112;
				array2[12] = 137;
				array2[12] = 99;
				array2[12] = 9;
				array2[13] = 115;
				array2[13] = 120;
				array2[13] = 131;
				array2[13] = 136;
				array2[13] = 119;
				array2[13] = 135;
				array2[14] = 153;
				array2[14] = 164;
				array2[14] = 229;
				array2[14] = 212;
				array2[15] = 99;
				array2[15] = 79;
				array2[15] = 118;
				array2[15] = 112;
				array2[15] = 67;
				array2[16] = 109;
				array2[16] = 111;
				array2[16] = 35;
				array2[17] = 138;
				array2[17] = 96;
				array2[17] = 181;
				array2[17] = 131;
				array2[17] = 206;
				array2[18] = 132;
				array2[18] = 113;
				array2[18] = 237;
				array2[19] = 149;
				array2[19] = 164;
				array2[19] = 156;
				array2[19] = 110;
				array2[19] = 91;
				array2[20] = 128;
				array2[20] = 147;
				array2[20] = 115;
				array2[20] = 94;
				array2[20] = 39;
				array2[21] = 165;
				array2[21] = 126;
				array2[21] = 162;
				array2[21] = 207;
				array2[22] = 133;
				array2[22] = 147;
				array2[22] = 81;
				array2[22] = 85;
				array2[22] = 209;
				array2[22] = 19;
				array2[23] = 141;
				array2[23] = 158;
				array2[23] = 88;
				array2[23] = 102;
				array2[23] = 167;
				array2[24] = 141;
				array2[24] = 145;
				array2[24] = 139;
				array2[24] = 114;
				array2[24] = 148;
				array2[24] = 187;
				array2[25] = 182;
				array2[25] = 182;
				array2[25] = 88;
				array2[25] = 250;
				array2[26] = 118;
				array2[26] = 169;
				array2[26] = 124;
				array2[26] = 152;
				array2[26] = 91;
				array2[27] = 119;
				array2[27] = 98;
				array2[27] = 129;
				array2[27] = 98;
				array2[27] = 239;
				array2[28] = 120;
				array2[28] = 229;
				array2[28] = 120;
				array2[28] = 179;
				array2[28] = 100;
				array2[29] = 146;
				array2[29] = 110;
				array2[29] = 192;
				array2[29] = 131;
				array2[29] = 123;
				array2[29] = 78;
				array2[30] = 96;
				array2[30] = 142;
				array2[30] = 208;
				array2[30] = 126;
				array2[30] = 119;
				array2[30] = 104;
				array2[31] = 112;
				array2[31] = 165;
				array2[31] = 214;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 68;
				array4[0] = 87;
				array4[0] = 124;
				array4[0] = 71;
				array4[0] = 116;
				array4[0] = 9;
				array4[1] = 120;
				array4[1] = 176;
				array4[1] = 160;
				array4[1] = 118;
				array4[1] = 85;
				array4[1] = 148;
				array4[2] = 148;
				array4[2] = 68;
				array4[2] = 91;
				array4[3] = 93;
				array4[3] = 106;
				array4[3] = 156;
				array4[3] = 99;
				array4[3] = 184;
				array4[4] = 100;
				array4[4] = 128;
				array4[4] = 216;
				array4[4] = 51;
				array4[5] = 182;
				array4[5] = 110;
				array4[5] = 92;
				array4[5] = 163;
				array4[5] = 106;
				array4[5] = 232;
				array4[6] = 143;
				array4[6] = 74;
				array4[6] = 96;
				array4[6] = 103;
				array4[6] = 245;
				array4[7] = 56;
				array4[7] = 55;
				array4[7] = 108;
				array4[7] = 149;
				array4[8] = 93;
				array4[8] = 116;
				array4[8] = 198;
				array4[8] = 116;
				array4[8] = 69;
				array4[8] = 59;
				array4[9] = 96;
				array4[9] = 105;
				array4[9] = 151;
				array4[9] = 113;
				array4[9] = 137;
				array4[9] = 186;
				array4[10] = 96;
				array4[10] = 94;
				array4[10] = 168;
				array4[10] = 76;
				array4[11] = 85;
				array4[11] = 146;
				array4[11] = 146;
				array4[11] = 168;
				array4[11] = 219;
				array4[12] = 138;
				array4[12] = 162;
				array4[12] = 140;
				array4[12] = 124;
				array4[12] = 98;
				array4[12] = 149;
				array4[13] = 89;
				array4[13] = 102;
				array4[13] = 151;
				array4[13] = 242;
				array4[14] = 149;
				array4[14] = 110;
				array4[14] = 48;
				array4[15] = 124;
				array4[15] = 138;
				array4[15] = 94;
				byte[] array5 = array4;
				byte[] publicKeyToken = typeof(TestMock).Assembly.GetName().GetPublicKeyToken();
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
				if (offset == -1)
				{
					SymmetricAlgorithm symmetricAlgorithm = TestMock.GetSymmetricAlgorithm();
					symmetricAlgorithm.Mode = CipherMode.CBC;
					ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor(array3, array5);
					MemoryStream memoryStream = new MemoryStream();
					CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
					cryptoStream.Write(array, 0, array.Length);
					cryptoStream.FlushFinalBlock();
					TestMock._decryptedStringTable = memoryStream.ToArray();
					memoryStream.Close();
					cryptoStream.Close();
					binaryReader.Close();
					array = TestMock._decryptedStringTable;
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
				TestMock._decryptedStringTable = array6;
			}
			int num17 = BitConverter.ToInt32(TestMock._decryptedStringTable, offset);
			try
			{
				byte[] array7 = new byte[num17];
				Array.Copy(TestMock._decryptedStringTable, offset + 4, array7, 0, num17);
				return Encoding.Unicode.GetString(array7, 0, array7.Length);
			}
			catch
			{
			}
			return "";
		}

		/// <summary>
		/// Base64 → UTF-16 解码（另一条字符串解密路径）。
		/// </summary>
		// Token: 0x06000107 RID: 263 RVA: 0x0000D548 File Offset: 0x0000B748
		internal static string Base64ToUnicode(string base64Text)
		{
			byte[] array = Convert.FromBase64String(base64Text);
			return Encoding.Unicode.GetString(array, 0, array.Length);
		}

		/// <summary>
		/// P/Invoke(kernel32)：修改内存页保护属性（内存补丁用）。
		/// </summary>
		// Token: 0x06000108 RID: 264
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr intptr_0, int int_0, int int_1, ref int int_2);

		/// <summary>
		/// 模块初始化器（壳核心，.cctor 替代，标 [TestMock.MethodMock] 特性，_moduleInitDone 保证只跑一次）：
		/// 1) 读嵌入资源 e6ab89dd-cbca-49e1-92f5-a278f1600946；构造 32 字节密钥 + 16 字节 IV（Array.Reverse），与 PublicKeyToken XOR 混合；
		/// 2) 数据段 32 位字流变换解密（常量 3753412436/678874538/179542347/522623986）；
		/// 3) OpenProcess(0x38=PROCESS_ALL_ACCESS, 当前进程)；_moduleBaseAddress = GetHINSTANCE(模块0)；
		/// 4) 按补丁清单 VirtualProtect(4, PAGE_READWRITE) → WriteProcessMemory 写 4 字节 int → 恢复保护（RVA 常量补丁）；
		/// 5) 剩余区段 VirtualProtect → Marshal.WriteInt32 逐字写入 → 恢复 → CloseHandle。
		/// 即运行时把加密补丁数据写回自身 .text/.rdata 实现脱壳/反调试补丁（自修改代码特征）。
		/// </summary>
		// Token: 0x06000109 RID: 265 RVA: 0x0000D56C File Offset: 0x0000B76C
		[TestMock.MethodMock(typeof(TestMock.MethodMock.InitializerMock<object>[]))]
		static void ModuleInit_ApplyMemoryPatch()
		{
			if (!TestMock._moduleInitDone)
			{
				TestMock._moduleInitDone = true;
				BinaryReader binaryReader = new BinaryReader(typeof(TestMock).Assembly.GetManifestResourceStream("e6ab89dd-cbca-49e1-92f5-a278f1600946"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 32;
				array2[0] = 115;
				array2[0] = 155;
				array2[0] = 122;
				array2[0] = 102;
				array2[0] = 32;
				array2[1] = 47;
				array2[1] = 104;
				array2[1] = 81;
				array2[1] = 65;
				array2[1] = 132;
				array2[1] = 109;
				array2[1] = 47;
				array2[2] = 104;
				array2[2] = 121;
				array2[2] = 101;
				array2[2] = 74;
				array2[2] = 129;
				array2[2] = 103;
				array2[2] = 104;
				array2[3] = 248;
				array2[3] = 126;
				array2[3] = 110;
				array2[3] = 210;
				array2[3] = 93;
				array2[3] = 248;
				array2[4] = 125;
				array2[4] = 170;
				array2[4] = 139;
				array2[4] = 98;
				array2[4] = 126;
				array2[4] = 125;
				array2[5] = 113;
				array2[5] = 117;
				array2[5] = 57;
				array2[5] = 113;
				array2[6] = 242;
				array2[6] = 112;
				array2[6] = 241;
				array2[6] = 148;
				array2[6] = 148;
				array2[6] = 150;
				array2[6] = 242;
				array2[7] = 230;
				array2[7] = 137;
				array2[7] = 114;
				array2[7] = 104;
				array2[7] = 230;
				array2[8] = 224;
				array2[8] = 106;
				array2[8] = 94;
				array2[8] = 116;
				array2[8] = 102;
				array2[8] = 120;
				array2[8] = 224;
				array2[9] = 17;
				array2[9] = 100;
				array2[9] = 155;
				array2[9] = 98;
				array2[9] = 114;
				array2[9] = 17;
				array2[10] = 39;
				array2[10] = 164;
				array2[10] = 129;
				array2[10] = 195;
				array2[10] = 120;
				array2[10] = 198;
				array2[10] = 39;
				array2[11] = 114;
				array2[11] = 71;
				array2[11] = 100;
				array2[11] = 132;
				array2[11] = 232;
				array2[11] = 114;
				array2[12] = 241;
				array2[12] = 90;
				array2[12] = 87;
				array2[12] = 19;
				array2[12] = 162;
				array2[12] = 54;
				array2[12] = 241;
				array2[13] = 55;
				array2[13] = 108;
				array2[13] = 48;
				array2[13] = 194;
				array2[13] = 219;
				array2[13] = 144;
				array2[13] = 55;
				array2[14] = 152;
				array2[14] = 146;
				array2[14] = 91;
				array2[14] = 96;
				array2[14] = 152;
				array2[15] = 9;
				array2[15] = 152;
				array2[15] = 129;
				array2[15] = 9;
				array2[16] = 224;
				array2[16] = 166;
				array2[16] = 114;
				array2[16] = 120;
				array2[16] = 170;
				array2[16] = 224;
				array2[17] = 140;
				array2[17] = 149;
				array2[17] = 131;
				array2[17] = 91;
				array2[17] = 136;
				array2[17] = 151;
				array2[17] = 140;
				array2[18] = 163;
				array2[18] = 67;
				array2[18] = 127;
				array2[18] = 163;
				array2[19] = 125;
				array2[19] = 117;
				array2[19] = 189;
				array2[19] = 129;
				array2[19] = 125;
				array2[20] = 250;
				array2[20] = 83;
				array2[20] = 120;
				array2[20] = 114;
				array2[20] = 250;
				array2[21] = 213;
				array2[21] = 106;
				array2[21] = 153;
				array2[21] = 114;
				array2[21] = 121;
				array2[21] = 213;
				array2[22] = 218;
				array2[22] = 136;
				array2[22] = 125;
				array2[22] = 218;
				array2[23] = 46;
				array2[23] = 223;
				array2[23] = 98;
				array2[23] = 168;
				array2[23] = 88;
				array2[23] = 85;
				array2[23] = 46;
				array2[24] = 79;
				array2[24] = 160;
				array2[24] = 172;
				array2[24] = 111;
				array2[24] = 79;
				array2[25] = 16;
				array2[25] = 96;
				array2[25] = 94;
				array2[25] = 133;
				array2[25] = 16;
				array2[26] = 212;
				array2[26] = 119;
				array2[26] = 177;
				array2[26] = 96;
				array2[26] = 137;
				array2[26] = 104;
				array2[26] = 212;
				array2[27] = 222;
				array2[27] = 142;
				array2[27] = 166;
				array2[27] = 222;
				array2[28] = 201;
				array2[28] = 88;
				array2[28] = 140;
				array2[28] = 15;
				array2[28] = 201;
				array2[29] = 155;
				array2[29] = 87;
				array2[29] = 101;
				array2[29] = 137;
				array2[29] = 154;
				array2[29] = 155;
				array2[30] = 13;
				array2[30] = 134;
				array2[30] = 112;
				array2[30] = 13;
				array2[31] = 134;
				array2[31] = 112;
				array2[31] = 138;
				array2[31] = 20;
				array2[31] = 108;
				array2[31] = 105;
				array2[31] = 134;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 115;
				array4[0] = 155;
				array4[0] = 0;
				array4[1] = 123;
				array4[1] = 104;
				array4[1] = 81;
				array4[1] = 15;
				array4[2] = 118;
				array4[2] = 86;
				array4[2] = 199;
				array4[3] = 35;
				array4[3] = 146;
				array4[3] = 90;
				array4[4] = 133;
				array4[4] = 106;
				array4[4] = 115;
				array4[4] = 134;
				array4[4] = 135;
				array4[4] = 243;
				array4[5] = 114;
				array4[5] = 195;
				array4[5] = 219;
				array4[5] = 70;
				array4[5] = 132;
				array4[6] = 90;
				array4[6] = 92;
				array4[6] = 92;
				array4[6] = 177;
				array4[7] = 81;
				array4[7] = 112;
				array4[7] = 167;
				array4[7] = 148;
				array4[7] = 148;
				array4[7] = 18;
				array4[8] = 158;
				array4[8] = 206;
				array4[8] = 161;
				array4[8] = 137;
				array4[8] = 108;
				array4[8] = 138;
				array4[9] = 128;
				array4[9] = 37;
				array4[9] = 92;
				array4[10] = 156;
				array4[10] = 100;
				array4[10] = 174;
				array4[11] = 165;
				array4[11] = 141;
				array4[11] = 173;
				array4[11] = 63;
				array4[11] = 94;
				array4[11] = 228;
				array4[12] = 142;
				array4[12] = 120;
				array4[12] = 198;
				array4[12] = 156;
				array4[12] = 99;
				array4[12] = 85;
				array4[13] = 169;
				array4[13] = 207;
				array4[13] = 90;
				array4[13] = 108;
				array4[13] = 241;
				array4[14] = 21;
				array4[14] = 91;
				array4[14] = 3;
				array4[15] = 120;
				array4[15] = 141;
				array4[15] = 227;
				byte[] array5 = array4;
				Array.Reverse(array5);
				byte[] publicKeyToken = typeof(TestMock).Assembly.GetName().GetPublicKeyToken();
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
				Assembly assembly = typeof(TestMock).Assembly;
				intptr_ = TestMock.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				TestMock._moduleBaseAddress = Marshal.GetHINSTANCE(assembly.GetModules()[0]).ToInt64();
				IntPtr zero = IntPtr.Zero;
				int int_ = 0;
				int num16 = binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				for (int m = 0; m < num16; m++)
				{
					IntPtr intPtr = new IntPtr(TestMock._moduleBaseAddress + (long)binaryReader.ReadInt32());
					TestMock.VirtualProtect(intPtr, 4, 4, ref int_);
					if (IntPtr.Size == 4)
					{
						TestMock.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					else
					{
						TestMock.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					TestMock.VirtualProtect(intPtr, 4, int_, ref int_);
				}
				try
				{
					while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1L)
					{
						int num17 = binaryReader.ReadInt32();
						IntPtr intptr_2 = new IntPtr(TestMock._moduleBaseAddress + (long)num17);
						int num18 = binaryReader.ReadInt32();
						TestMock.VirtualProtect(intptr_2, num18 * 4, 4, ref int_);
						for (int n = 0; n < num18; n++)
						{
							Marshal.WriteInt32(new IntPtr(intptr_2.ToInt64() + (long)(n * 4)), binaryReader.ReadInt32());
						}
						TestMock.VirtualProtect(intptr_2, num18 * 4, int_, ref int_);
					}
					TestMock.CloseHandle(intptr_);
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// 取程序集路径：依次尝试 Location、CodeBase（去 file:/// 前缀）、反射 Location 属性；全部失败返回空串。
		/// </summary>
		// Token: 0x0600010A RID: 266 RVA: 0x0000E7F0 File Offset: 0x0000C9F0
		internal static object GetAssemblyPath(Assembly assembly)
		{
			try
			{
				if (File.Exists(((Assembly)assembly).Location))
				{
					return ((Assembly)assembly).Location;
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(((Assembly)assembly).GetName().CodeBase.ToString().Replace("file:///", "")))
				{
					return ((Assembly)assembly).GetName().CodeBase.ToString().Replace("file:///", "");
				}
			}
			catch
			{
			}
			try
			{
				if (File.Exists(assembly.GetType().GetProperty("Location").GetValue(assembly, new object[0]).ToString()))
				{
					return assembly.GetType().GetProperty("Location").GetValue(assembly, new object[0]).ToString();
				}
			}
			catch
			{
			}
			return "";
		}

		/// <summary>
		/// P/Invoke(kernel32)：写自身进程内存（内存补丁用）。
		/// </summary>
		// Token: 0x0600010B RID: 267
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		/// <summary>
		/// P/Invoke(kernel32)：读进程内存（声明，本文件未实际调用）。
		/// </summary>
		// Token: 0x0600010C RID: 268
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		/// <summary>
		/// P/Invoke(kernel32)：打开进程句柄（0x38=ALL_ACCESS 打开自身）。
		/// </summary>
		// Token: 0x0600010D RID: 269
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint uint_0, int int_0, uint uint_1);

		/// <summary>
		/// P/Invoke(kernel32)：关闭句柄。
		/// </summary>
		// Token: 0x0600010E RID: 270
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr intptr_0);

		/// <summary>
		/// FileStream 全量读文件字节（循环 Read 直到读完）。
		/// </summary>
		// Token: 0x0600010F RID: 271 RVA: 0x0000E900 File Offset: 0x0000CB00
		private static byte[] ReadFileBytes(string path)
		{
			byte[] array;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
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

		/// <summary>
		/// Rijndael 固定密钥解密：Key={FB 9A 69 4B FE C2 CD C4 EB C8 5A 6A B6 85 98 BB 8C E1 4C 6F D1 17 51 43 36 1D 46 57 76 2B CF E5}，IV={43 BD 3C 69 16 EA F3 BA DB 42 C0 71 12 E2 22 8F}。
		/// </summary>
		// Token: 0x06000110 RID: 272 RVA: 0x0000E960 File Offset: 0x0000CB60
		private static byte[] RijndaelDecrypt_FixedKey(byte[] encryptedData)
		{
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = TestMock.GetSymmetricAlgorithm();
			symmetricAlgorithm.Key = new byte[]
			{
				251,
				154,
				105,
				75,
				254,
				194,
				205,
				196,
				235,
				200,
				90,
				106,
				182,
				133,
				152,
				187,
				140,
				225,
				76,
				111,
				209,
				23,
				81,
				67,
				54,
				29,
				70,
				87,
				118,
				43,
				207,
				229
			};
			symmetricAlgorithm.IV = new byte[]
			{
				67,
				189,
				60,
				105,
				22,
				234,
				243,
				186,
				219,
				66,
				192,
				113,
				18,
				226,
				34,
				143
			};
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(encryptedData, 0, encryptedData.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		/// <summary>
		/// 占位方法（混淆残留）：返回 null，无实际逻辑。
		/// </summary>
		// Token: 0x06000111 RID: 273 RVA: 0x0000E9CC File Offset: 0x0000CBCC
		private byte[] NullPlaceholder_1()
		{
			return null;
		}

		/// <summary>
		/// 占位方法（混淆残留）：返回 null，无实际逻辑。
		/// </summary>
		// Token: 0x06000112 RID: 274 RVA: 0x0000E9CC File Offset: 0x0000CBCC
		private byte[] NullPlaceholder_2()
		{
			return null;
		}

		/// <summary>
		/// 占位方法（混淆残留）：返回 null，无实际逻辑。
		/// </summary>
		// Token: 0x06000113 RID: 275 RVA: 0x0000E9CC File Offset: 0x0000CBCC
		private byte[] NullPlaceholder_3()
		{
			return null;
		}

		/// <summary>
		/// 占位方法（混淆残留）：返回 null，无实际逻辑。
		/// </summary>
		// Token: 0x06000114 RID: 276 RVA: 0x0000E9CC File Offset: 0x0000CBCC
		private byte[] NullPlaceholder_4()
		{
			return null;
		}

		/// <summary>
		/// 占位方法（混淆残留）：返回 null，无实际逻辑。
		/// </summary>
		// Token: 0x06000115 RID: 277 RVA: 0x0000E9DC File Offset: 0x0000CBDC
		private byte[] NullPlaceholder_5()
		{
			return null;
		}

		/// <summary>
		/// 占位方法（混淆残留）：返回 null，无实际逻辑。
		/// </summary>
		// Token: 0x06000116 RID: 278 RVA: 0x0000E9DC File Offset: 0x0000CBDC
		private byte[] NullPlaceholder_6()
		{
			return null;
		}

		/// <summary>
		/// 占位方法：含 IntelliLock 特征 GUID "{11111-22222-40001-00001}"，返回 {1,2}，无实际逻辑。
		/// </summary>
		// Token: 0x06000117 RID: 279 RVA: 0x00002560 File Offset: 0x00000760
		internal byte[] Placeholder_LicenseGuid_1()
		{
			int length = "{11111-22222-40001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		/// <summary>
		/// 占位方法：含 IntelliLock 特征 GUID "{11111-22222-40001-00002}"，返回 {1,2}，无实际逻辑。
		/// </summary>
		// Token: 0x06000118 RID: 280 RVA: 0x0000257B File Offset: 0x0000077B
		internal byte[] Placeholder_LicenseGuid_2()
		{
			int length = "{11111-22222-40001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		/// <summary>
		/// 占位方法：含 IntelliLock 特征 GUID "{11111-22222-50001-00001}"，返回 {1,2}，无实际逻辑。
		/// </summary>
		// Token: 0x06000119 RID: 281 RVA: 0x00002596 File Offset: 0x00000796
		internal byte[] Placeholder_LicenseGuid_3()
		{
			int length = "{11111-22222-50001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		/// <summary>
		/// 占位方法：含 IntelliLock 特征 GUID "{11111-22222-50001-00002}"，返回 {1,2}，无实际逻辑。
		/// </summary>
		// Token: 0x0600011A RID: 282 RVA: 0x000025B1 File Offset: 0x000007B1
		internal byte[] Placeholder_LicenseGuid_4()
		{
			int length = "{11111-22222-50001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		/// <summary>
		/// 占位方法：含 IntelliLock 特征 GUID "{11111-22222-60001-00001}"，返回 {1,2}，无实际逻辑。
		/// </summary>
		// Token: 0x0600011B RID: 283 RVA: 0x000025CC File Offset: 0x000007CC
		internal byte[] Placeholder_LicenseGuid_5()
		{
			int length = "{11111-22222-60001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		/// <summary>
		/// 占位方法：含 IntelliLock 特征 GUID "{11111-22222-60001-00002}"，返回 {1,2}，无实际逻辑。
		/// </summary>
		// Token: 0x0600011C RID: 284 RVA: 0x000025E7 File Offset: 0x000007E7
		internal byte[] Placeholder_LicenseGuid_6()
		{
			int length = "{11111-22222-60001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		/// <summary>
		/// 固定密钥加密（疑似壳注册码/激活校验）：Key={82,102,104,110,32,77,24,34,118,181,51,17,18,51,12,109,10,32,77,24,34,158,161,41,97,28,118,181,5,25,1,88}，
		/// IV = ComputeMd5(UTF16(ivSeed))；对 UTF16(plainText) 加密后输出 Base64。
		/// </summary>
		// Token: 0x0600011D RID: 285 RVA: 0x0000E9EC File Offset: 0x0000CBEC
		internal static string EncryptFixedKey_ToBase64(string plainText, string ivSeed)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(plainText);
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
			byte[] iv = TestMock.ComputeMd5(Encoding.Unicode.GetBytes(ivSeed));
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = TestMock.GetSymmetricAlgorithm();
			symmetricAlgorithm.Key = key;
			symmetricAlgorithm.IV = iv;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.Close();
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 true，无实际逻辑。
		/// </summary>
		// Token: 0x0600011F RID: 287 RVA: 0x000021D1 File Offset: 0x000003D1
		internal static bool Placeholder_ReturnTrue()
		{
			return true;
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 false，无实际逻辑。
		/// </summary>
		// Token: 0x06000120 RID: 288 RVA: 0x000021C6 File Offset: 0x000003C6
		internal static bool Placeholder_ReturnFalse()
		{
			return false;
		}

		/// <summary>
		/// FIPS 探测是否已完成标志。
		/// </summary>
		// Token: 0x04000075 RID: 117
		private static bool _fipsProbeDone;

		/// <summary>
		/// FIPS 探测结果（是否强制 FIPS）。
		/// </summary>
		// Token: 0x04000076 RID: 118
		private static bool _fipsEnabled;

		/// <summary>
		/// 标量占位字段（混淆残留，初始化为 1）。
		/// </summary>
		// Token: 0x04000077 RID: 119
		private static int _placeholderScalarInt_1;

		/// <summary>
		/// 标量占位字段（混淆残留，初始化为 0）。
		/// </summary>
		// Token: 0x04000078 RID: 120
		private static int _placeholderScalarInt_0;

		/// <summary>
		/// 模块初始化器执行标志（保证 ModuleInit_ApplyMemoryPatch 只跑一次）。
		/// </summary>
		// Token: 0x04000079 RID: 121
		private static bool _moduleInitDone;

		/// <summary>
		/// IntPtr 占位字段（混淆残留，初始化为 Zero）。
		/// </summary>
		// Token: 0x0400007A RID: 122
		private static IntPtr _placeholderIntPtr_2;

		/// <summary>
		/// 空字节数组占位字段（混淆残留）。
		/// </summary>
		// Token: 0x0400007B RID: 123
		private static byte[] _placeholderByteArray_1;

		/// <summary>
		/// 空 int 数组占位字段（混淆残留）。
		/// </summary>
		// Token: 0x0400007C RID: 124
		private static int[] _placeholderIntArray;

		/// <summary>
		/// MD5 标准 64 常量表（3614090360=0x67452301 系，标准 MD5 K 表）。
		/// </summary>
		// Token: 0x0400007D RID: 125
		private static uint[] _md5Constants = new uint[]
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

		/// <summary>
		/// IntPtr 占位字段（混淆残留，初始化为 Zero）。
		/// </summary>
		// Token: 0x0400007E RID: 126
		private static IntPtr _placeholderIntPtr_1;

		/// <summary>
		/// 解密后的字符串表缓存（DecryptStringTable 惰性解密一次）。
		/// </summary>
		// Token: 0x0400007F RID: 127
		private static byte[] _decryptedStringTable;

		/// <summary>
		/// 空字节数组占位字段（混淆残留）。
		/// </summary>
		// Token: 0x04000080 RID: 128
		private static byte[] _placeholderByteArray_2;

		/// <summary>
		/// 空 string 数组占位字段（混淆残留）。
		/// </summary>
		// Token: 0x04000081 RID: 129
		private static string[] _placeholderStringArray;

		/// <summary>
		/// 空字节数组占位字段（混淆残留）。
		/// </summary>
		// Token: 0x04000082 RID: 130
		private static byte[] _placeholderByteArray_3;

		/// <summary>
		/// 模块基址（HINSTANCE，ModuleInit_ApplyMemoryPatch 中 GetHINSTANCE 所得，补丁目标 = 基址 + RVA）。
		/// </summary>
		// Token: 0x04000083 RID: 131
		private static long _moduleBaseAddress;

		/// <summary>
		/// 壳生成特性类：标记模块初始化器（MethodMock(typeof(InitializerMock<object>[]))），构造空实现。
		/// </summary>
		// Token: 0x02000014 RID: 20
		internal class MethodMock : Attribute
		{
		/// <summary>
		/// 特性构造（空实现，仅作标记）。
		/// </summary>
			// Token: 0x06000121 RID: 289 RVA: 0x00002602 File Offset: 0x00000802
			[TestMock.MethodMock(typeof(TestMock.MethodMock.InitializerMock<object>[]))]
			public MethodMock(object object_0)
			{
			}

		/// <summary>
		/// MethodMock 的嵌套空泛型占位类。
		/// </summary>
			// Token: 0x02000015 RID: 21
			internal class InitializerMock<T>
			{
		/// <summary>
		/// 占位方法（混淆残留）：恒返回 true，无实际逻辑。
		/// </summary>
				// Token: 0x06000123 RID: 291 RVA: 0x000021D1 File Offset: 0x000003D1
				internal static bool Placeholder_ReturnTrue()
				{
					return true;
				}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 false，无实际逻辑。
		/// </summary>
				// Token: 0x06000124 RID: 292 RVA: 0x000021C6 File Offset: 0x000003C6
				internal static bool Placeholder_ReturnFalse()
				{
					return false;
				}
			}
		}

		/// <summary>
		/// 空枚举占位（[Flags]，无成员，壳残留）。
		/// </summary>
		// Token: 0x02000016 RID: 22
		[Flags]
		private enum AuthorMessageKind
		{

		}
	}
}
