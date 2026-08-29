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
	/// CustomerReg —— .NET Reactor 加壳运行时残留（字符串解密 / 反篡改内存补丁 / 许可证 API 桩）。
	/// 真实功能：① 手写 MD5（ComputeMd5/Md5RoundF~I）；② 解密嵌入资源字符串表（DecryptStringTable）；
	/// ③ 反篡改/解壳内存补丁（ApplyAntiTamperPatch，OpenProcess+WriteProcessMemory 改写自身模块内存）；
	/// ④ IntelliLock 许可证 API 桩。非业务逻辑，但残留密钥、资源名、内存补丁模式是重要加壳 IOC。
	/// </summary>
	internal class CustomerReg
	{
		// Token: 0x06000034 RID: 52 RVA: 0x000029A8 File Offset: 0x00000BA8
		/// <summary>
		/// 静态构造函数：初始化全部静态字段（多数为混淆填充/未使用字段）。
		/// </summary>
		static CustomerReg()
		{
			CustomerReg.fipsFlag = false;
			CustomerReg.unusedUtils = new byte[0];
			CustomerReg.unusedParam = new byte[0];
			CustomerReg.decryptedStringTable = new byte[0];
			CustomerReg.unusedContainer = new byte[0];
			CustomerReg.unusedMapper = IntPtr.Zero;
			CustomerReg.unusedRegistry = IntPtr.Zero;
			CustomerReg.unusedSystemArray = new string[0];
			CustomerReg.unusedFieldArray = new int[0];
			CustomerReg.unusedMock = 1;
			CustomerReg.selfModuleBase = 0L;
			CustomerReg.unusedIdentifier = 0;
			CustomerReg.patchApplied = false;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002211 File Offset: 0x00000411
		/// <summary>
		/// 空方法：方法体为空、无调用方，属混淆填充。
		/// </summary>
		private void UnusedEmptyMethod()
		{
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002A50 File Offset: 0x00000C50
		/// <summary>
		/// 手写标准 MD5 哈希：对任意字节串计算 16 字节摘要。
		/// 流程：0x80 补位 + 64 位长度 → 初始化 A/B/C/D 幻数 → 每 64 字节一块执行 4 轮×16 步
		/// （Md5RoundF=轮1、Md5RoundG=轮2、Md5RoundH=轮3、Md5RoundI=轮4，轮常量取自 md5KConstants）→ 小端输出 16 字节。
		/// 意义：加壳器以自实现 MD5 摆脱对系统加密 API 的依赖（也用于生成嵌入资源名哈希）。
		/// </summary>
		internal static byte[] ComputeMd5(object object_0)
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
				CustomerReg.Md5RoundF(ref num6, num7, num8, num9, 0U, 7, 1U, array);
				CustomerReg.Md5RoundF(ref num9, num6, num7, num8, 1U, 12, 2U, array);
				CustomerReg.Md5RoundF(ref num8, num9, num6, num7, 2U, 17, 3U, array);
				CustomerReg.Md5RoundF(ref num7, num8, num9, num6, 3U, 22, 4U, array);
				CustomerReg.Md5RoundF(ref num6, num7, num8, num9, 4U, 7, 5U, array);
				CustomerReg.Md5RoundF(ref num9, num6, num7, num8, 5U, 12, 6U, array);
				CustomerReg.Md5RoundF(ref num8, num9, num6, num7, 6U, 17, 7U, array);
				CustomerReg.Md5RoundF(ref num7, num8, num9, num6, 7U, 22, 8U, array);
				CustomerReg.Md5RoundF(ref num6, num7, num8, num9, 8U, 7, 9U, array);
				CustomerReg.Md5RoundF(ref num9, num6, num7, num8, 9U, 12, 10U, array);
				CustomerReg.Md5RoundF(ref num8, num9, num6, num7, 10U, 17, 11U, array);
				CustomerReg.Md5RoundF(ref num7, num8, num9, num6, 11U, 22, 12U, array);
				CustomerReg.Md5RoundF(ref num6, num7, num8, num9, 12U, 7, 13U, array);
				CustomerReg.Md5RoundF(ref num9, num6, num7, num8, 13U, 12, 14U, array);
				CustomerReg.Md5RoundF(ref num8, num9, num6, num7, 14U, 17, 15U, array);
				CustomerReg.Md5RoundF(ref num7, num8, num9, num6, 15U, 22, 16U, array);
				CustomerReg.Md5RoundG(ref num6, num7, num8, num9, 1U, 5, 17U, array);
				CustomerReg.Md5RoundG(ref num9, num6, num7, num8, 6U, 9, 18U, array);
				CustomerReg.Md5RoundG(ref num8, num9, num6, num7, 11U, 14, 19U, array);
				CustomerReg.Md5RoundG(ref num7, num8, num9, num6, 0U, 20, 20U, array);
				CustomerReg.Md5RoundG(ref num6, num7, num8, num9, 5U, 5, 21U, array);
				CustomerReg.Md5RoundG(ref num9, num6, num7, num8, 10U, 9, 22U, array);
				CustomerReg.Md5RoundG(ref num8, num9, num6, num7, 15U, 14, 23U, array);
				CustomerReg.Md5RoundG(ref num7, num8, num9, num6, 4U, 20, 24U, array);
				CustomerReg.Md5RoundG(ref num6, num7, num8, num9, 9U, 5, 25U, array);
				CustomerReg.Md5RoundG(ref num9, num6, num7, num8, 14U, 9, 26U, array);
				CustomerReg.Md5RoundG(ref num8, num9, num6, num7, 3U, 14, 27U, array);
				CustomerReg.Md5RoundG(ref num7, num8, num9, num6, 8U, 20, 28U, array);
				CustomerReg.Md5RoundG(ref num6, num7, num8, num9, 13U, 5, 29U, array);
				CustomerReg.Md5RoundG(ref num9, num6, num7, num8, 2U, 9, 30U, array);
				CustomerReg.Md5RoundG(ref num8, num9, num6, num7, 7U, 14, 31U, array);
				CustomerReg.Md5RoundG(ref num7, num8, num9, num6, 12U, 20, 32U, array);
				CustomerReg.Md5RoundH(ref num6, num7, num8, num9, 5U, 4, 33U, array);
				CustomerReg.Md5RoundH(ref num9, num6, num7, num8, 8U, 11, 34U, array);
				CustomerReg.Md5RoundH(ref num8, num9, num6, num7, 11U, 16, 35U, array);
				CustomerReg.Md5RoundH(ref num7, num8, num9, num6, 14U, 23, 36U, array);
				CustomerReg.Md5RoundH(ref num6, num7, num8, num9, 1U, 4, 37U, array);
				CustomerReg.Md5RoundH(ref num9, num6, num7, num8, 4U, 11, 38U, array);
				CustomerReg.Md5RoundH(ref num8, num9, num6, num7, 7U, 16, 39U, array);
				CustomerReg.Md5RoundH(ref num7, num8, num9, num6, 10U, 23, 40U, array);
				CustomerReg.Md5RoundH(ref num6, num7, num8, num9, 13U, 4, 41U, array);
				CustomerReg.Md5RoundH(ref num9, num6, num7, num8, 0U, 11, 42U, array);
				CustomerReg.Md5RoundH(ref num8, num9, num6, num7, 3U, 16, 43U, array);
				CustomerReg.Md5RoundH(ref num7, num8, num9, num6, 6U, 23, 44U, array);
				CustomerReg.Md5RoundH(ref num6, num7, num8, num9, 9U, 4, 45U, array);
				CustomerReg.Md5RoundH(ref num9, num6, num7, num8, 12U, 11, 46U, array);
				CustomerReg.Md5RoundH(ref num8, num9, num6, num7, 15U, 16, 47U, array);
				CustomerReg.Md5RoundH(ref num7, num8, num9, num6, 2U, 23, 48U, array);
				CustomerReg.Md5RoundI(ref num6, num7, num8, num9, 0U, 6, 49U, array);
				CustomerReg.Md5RoundI(ref num9, num6, num7, num8, 7U, 10, 50U, array);
				CustomerReg.Md5RoundI(ref num8, num9, num6, num7, 14U, 15, 51U, array);
				CustomerReg.Md5RoundI(ref num7, num8, num9, num6, 5U, 21, 52U, array);
				CustomerReg.Md5RoundI(ref num6, num7, num8, num9, 12U, 6, 53U, array);
				CustomerReg.Md5RoundI(ref num9, num6, num7, num8, 3U, 10, 54U, array);
				CustomerReg.Md5RoundI(ref num8, num9, num6, num7, 10U, 15, 55U, array);
				CustomerReg.Md5RoundI(ref num7, num8, num9, num6, 1U, 21, 56U, array);
				CustomerReg.Md5RoundI(ref num6, num7, num8, num9, 8U, 6, 57U, array);
				CustomerReg.Md5RoundI(ref num9, num6, num7, num8, 15U, 10, 58U, array);
				CustomerReg.Md5RoundI(ref num8, num9, num6, num7, 6U, 15, 59U, array);
				CustomerReg.Md5RoundI(ref num7, num8, num9, num6, 13U, 21, 60U, array);
				CustomerReg.Md5RoundI(ref num6, num7, num8, num9, 4U, 6, 61U, array);
				CustomerReg.Md5RoundI(ref num9, num6, num7, num8, 11U, 10, 62U, array);
				CustomerReg.Md5RoundI(ref num8, num9, num6, num7, 2U, 15, 63U, array);
				CustomerReg.Md5RoundI(ref num7, num8, num9, num6, 9U, 21, 64U, array);
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

		// Token: 0x06000037 RID: 55 RVA: 0x00002213 File Offset: 0x00000413
		/// <summary>MD5 第 1 轮非线性函数 F(b,c,d) = (b&c)|(~b&d)，含对应轮常量。</summary>
		private static void Md5RoundF(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + CustomerReg.RotateLeft(uint_0 + ((uint_1 & uint_2) | (~uint_1 & uint_3)) + object_0[(int)uint_4] + CustomerReg.md5KConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000223C File Offset: 0x0000043C
		/// <summary>MD5 第 2 轮非线性函数 G(b,c,d) = (b&d)|(c&~d)，含对应轮常量。</summary>
		private static void Md5RoundG(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + CustomerReg.RotateLeft(uint_0 + ((uint_1 & uint_3) | (uint_2 & ~uint_3)) + object_0[(int)uint_4] + CustomerReg.md5KConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002265 File Offset: 0x00000465
		/// <summary>MD5 第 3 轮非线性函数 H(b,c,d) = b^c^d，含对应轮常量。</summary>
		private static void Md5RoundH(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + CustomerReg.RotateLeft(uint_0 + (uint_1 ^ uint_2 ^ uint_3) + object_0[(int)uint_4] + CustomerReg.md5KConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000228B File Offset: 0x0000048B
		/// <summary>MD5 第 4 轮非线性函数 I(b,c,d) = c^(b|~d)，含对应轮常量。</summary>
		private static void Md5RoundI(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + CustomerReg.RotateLeft(uint_0 + (uint_2 ^ (uint_1 | ~uint_3)) + object_0[(int)uint_4] + CustomerReg.md5KConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000022B2 File Offset: 0x000004B2
		/// <summary>MD5 循环左移：rotate_left(x, n) = (x >> (32-n)) | (x << n)。</summary>
		private static uint RotateLeft(uint uint_0, ushort ushort_0)
		{
			return uint_0 >> (int)(32 - ushort_0) | uint_0 << (int)ushort_0;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000022C4 File Offset: 0x000004C4
		/// <summary>
		/// FIPS 策略检测（带缓存）：首次调用触发 DetectFipsPolicy，返回系统是否强制 FIPS 算法（fipsFlag）。
		/// </summary>
		internal static bool IsFipsEnforced()
		{
			if (!CustomerReg.fipsChecked)
			{
				CustomerReg.DetectFipsPolicy();
				CustomerReg.fipsChecked = true;
			}
			return CustomerReg.fipsFlag;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000030B0 File Offset: 0x000012B0
		/// <summary>
		/// 反射检测系统 FIPS 策略：优先读取 CryptoConfig.AllowOnlyFipsAlgorithms；失败则回退 Utils.FipsAlgorithmPolicy==1；结果存 fipsFlag。
		/// </summary>
		internal static void DetectFipsPolicy()
		{
			try
			{
				CustomerReg.fipsFlag = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
				try
				{
					if ((int)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.Utils", false).GetMethod("get_FipsAlgorithmPolicy", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[0]) == 1)
					{
						CustomerReg.fipsFlag = true;
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003160 File Offset: 0x00001360
		/// <summary>
		/// 按 FIPS 策略获取对称算法：FIPS 强制时用 AesCryptoServiceProvider（异常回退 RijndaelManaged），否则 RijndaelManaged。
		/// </summary>
		internal static SymmetricAlgorithm GetSymmetricAlgorithm()
		{
			SymmetricAlgorithm result = null;
			if (CustomerReg.IsFipsEnforced())
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

		// Token: 0x0600003F RID: 63 RVA: 0x000022DD File Offset: 0x000004DD
		/// <summary>
		/// MD5 摘要入口：FIPS 环境下走手写 ComputeMd5，否则用 MD5CryptoServiceProvider.ComputeHash。
		/// </summary>
		internal static byte[] ComputeMd5Hash(byte[] byte_0)
		{
			if (!CustomerReg.IsFipsEnforced())
			{
				return new MD5CryptoServiceProvider().ComputeHash(byte_0);
			}
			return CustomerReg.ComputeMd5(byte_0);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000031B8 File Offset: 0x000013B8
		/// <summary>
		/// 字符串表解密：读嵌入资源 c3a777aa-b0d9-45b5-9f38-a4546dce2557 全部字节，构造 32 字节密钥
		/// （硬编码数组逐字节 XOR 程序集公钥令牌）与 16 字节 IV，经 CBC 解密后再跑自定义 xorshift 式密钥流
		/// （魔数 3753412436/678874538/179542347/522623986）得到"长度前缀+UTF-16"字符串表，按下标 int_0 返回字符串。
		/// </summary>
		static string DecryptStringTable(int int_0)
		{
			if (CustomerReg.decryptedStringTable.Length == 0)
			{
				BinaryReader binaryReader = new BinaryReader(typeof(CustomerReg).Assembly.GetManifestResourceStream("c3a777aa-b0d9-45b5-9f38-a4546dce2557"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 109;
				array2[0] = 95;
				array2[0] = 142;
				array2[0] = 172;
				array2[1] = 214;
				array2[1] = 99;
				array2[1] = 248;
				array2[2] = 156;
				array2[2] = 126;
				array2[2] = 89;
				array2[2] = 37;
				array2[3] = 100;
				array2[3] = 170;
				array2[3] = 87;
				array2[3] = 124;
				array2[3] = 123;
				array2[4] = 103;
				array2[4] = 150;
				array2[4] = 118;
				array2[4] = 77;
				array2[4] = 132;
				array2[4] = 10;
				array2[5] = 98;
				array2[5] = 104;
				array2[5] = 179;
				array2[5] = 201;
				array2[6] = 100;
				array2[6] = 113;
				array2[6] = 127;
				array2[6] = 100;
				array2[6] = 241;
				array2[7] = 92;
				array2[7] = 146;
				array2[7] = 122;
				array2[7] = 93;
				array2[7] = 168;
				array2[7] = 77;
				array2[8] = 139;
				array2[8] = 92;
				array2[8] = 134;
				array2[8] = 134;
				array2[8] = 107;
				array2[9] = 109;
				array2[9] = 148;
				array2[9] = 146;
				array2[9] = 109;
				array2[9] = 28;
				array2[10] = 109;
				array2[10] = 174;
				array2[10] = 124;
				array2[10] = 201;
				array2[10] = 169;
				array2[11] = 128;
				array2[11] = 120;
				array2[11] = 138;
				array2[11] = 148;
				array2[11] = 172;
				array2[12] = 178;
				array2[12] = 87;
				array2[12] = 170;
				array2[12] = 64;
				array2[13] = 108;
				array2[13] = 84;
				array2[13] = 184;
				array2[13] = 130;
				array2[13] = 135;
				array2[13] = 129;
				array2[14] = 73;
				array2[14] = 114;
				array2[14] = 207;
				array2[14] = 93;
				array2[14] = 83;
				array2[15] = 148;
				array2[15] = 118;
				array2[15] = 154;
				array2[15] = 185;
				array2[15] = 100;
				array2[15] = 85;
				array2[16] = 144;
				array2[16] = 218;
				array2[16] = 75;
				array2[17] = 96;
				array2[17] = 146;
				array2[17] = 99;
				array2[18] = 87;
				array2[18] = 167;
				array2[18] = 16;
				array2[19] = 30;
				array2[19] = 109;
				array2[19] = 96;
				array2[19] = 21;
				array2[20] = 160;
				array2[20] = 167;
				array2[20] = 134;
				array2[20] = 124;
				array2[20] = 199;
				array2[21] = 64;
				array2[21] = 113;
				array2[21] = 239;
				array2[22] = 138;
				array2[22] = 92;
				array2[22] = 92;
				array2[22] = 142;
				array2[22] = 140;
				array2[22] = 16;
				array2[23] = 103;
				array2[23] = 40;
				array2[23] = 109;
				array2[23] = 0;
				array2[24] = 73;
				array2[24] = 160;
				array2[24] = 171;
				array2[24] = 2;
				array2[25] = 98;
				array2[25] = 153;
				array2[25] = 156;
				array2[25] = 235;
				array2[26] = 159;
				array2[26] = 184;
				array2[26] = 199;
				array2[26] = 165;
				array2[27] = 130;
				array2[27] = 145;
				array2[27] = 65;
				array2[28] = 124;
				array2[28] = 41;
				array2[28] = 105;
				array2[28] = 63;
				array2[28] = 127;
				array2[29] = 84;
				array2[29] = 134;
				array2[29] = 13;
				array2[30] = 104;
				array2[30] = 114;
				array2[30] = 154;
				array2[30] = 148;
				array2[31] = 119;
				array2[31] = 91;
				array2[31] = 147;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 109;
				array4[0] = 95;
				array4[0] = 171;
				array4[1] = 48;
				array4[1] = 214;
				array4[1] = 110;
				array4[2] = 204;
				array4[2] = 154;
				array4[2] = 104;
				array4[2] = 26;
				array4[3] = 76;
				array4[3] = 138;
				array4[3] = 250;
				array4[4] = 124;
				array4[4] = 164;
				array4[4] = 103;
				array4[4] = 150;
				array4[4] = 210;
				array4[5] = 93;
				array4[5] = 83;
				array4[5] = 91;
				array4[5] = 97;
				array4[5] = 94;
				array4[5] = 72;
				array4[6] = 148;
				array4[6] = 100;
				array4[6] = 113;
				array4[6] = 187;
				array4[6] = 173;
				array4[7] = 150;
				array4[7] = 125;
				array4[7] = 146;
				array4[7] = 122;
				array4[7] = 89;
				array4[8] = 84;
				array4[8] = 168;
				array4[8] = 111;
				array4[8] = 189;
				array4[9] = 22;
				array4[9] = 87;
				array4[9] = 32;
				array4[10] = 106;
				array4[10] = 114;
				array4[10] = 137;
				array4[10] = 149;
				array4[10] = 108;
				array4[11] = 109;
				array4[11] = 152;
				array4[11] = 29;
				array4[12] = 114;
				array4[12] = 156;
				array4[12] = 158;
				array4[12] = 131;
				array4[12] = 2;
				array4[13] = 139;
				array4[13] = 138;
				array4[13] = 142;
				array4[13] = 112;
				array4[13] = 131;
				array4[14] = 206;
				array4[14] = 108;
				array4[14] = 66;
				array4[14] = 87;
				array4[14] = 136;
				array4[15] = 123;
				array4[15] = 110;
				array4[15] = 123;
				array4[15] = 36;
				byte[] array5 = array4;
				byte[] publicKeyToken = typeof(CustomerReg).Assembly.GetName().GetPublicKeyToken();
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
					SymmetricAlgorithm symmetricAlgorithm = CustomerReg.GetSymmetricAlgorithm();
					symmetricAlgorithm.Mode = CipherMode.CBC;
					ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor(array3, array5);
					MemoryStream memoryStream = new MemoryStream();
					CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
					cryptoStream.Write(array, 0, array.Length);
					cryptoStream.FlushFinalBlock();
					CustomerReg.decryptedStringTable = memoryStream.ToArray();
					memoryStream.Close();
					cryptoStream.Close();
					binaryReader.Close();
					array = CustomerReg.decryptedStringTable;
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
				CustomerReg.decryptedStringTable = array6;
			}
			int num17 = BitConverter.ToInt32(CustomerReg.decryptedStringTable, int_0);
			try
			{
				byte[] array7 = new byte[num17];
				Array.Copy(CustomerReg.decryptedStringTable, int_0 + 4, array7, 0, num17);
				return Encoding.Unicode.GetString(array7, 0, array7.Length);
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004060 File Offset: 0x00002260
		/// <summary>Base64 → UTF-16 字符串解码助手（简单字符串解密）。</summary>
		internal static string Base64ToUnicodeString(string string_0)
		{
			byte[] array = Convert.FromBase64String(string_0);
			return Encoding.Unicode.GetString(array, 0, array.Length);
		}

		// Token: 0x06000042 RID: 66
		/// <summary>P/Invoke：kernel32.VirtualProtect —— 修改内存页保护属性（补丁用 PAGE_EXECUTE_READWRITE=4）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr intptr_0, int int_0, int int_1, ref int int_2);

		// Token: 0x06000043 RID: 67 RVA: 0x00004084 File Offset: 0x00002284
		[CustomerReg.AnnotationReg(typeof(CustomerReg.AnnotationReg.ComposerReg<object>[]))]
		/// <summary>
		/// 反篡改/解壳内存补丁（加壳残留高危逻辑）：解密资源 b215f184-b268-4760-a367-fb3a9e128901 →
		/// OpenProcess(56=PROCESS_ALL_ACCESS) 打开自身 → 取自身模块基址 selfModuleBase → 按 RVA 列表
		/// VirtualProtect(PAGE_EXECUTE_READWRITE) + WriteProcessMemory/Marshal.WriteInt32 改写自身模块内存 → CloseHandle。
		/// 危害：进程自我修改内存的反调试/反篡改机制，运行期改写自身 IL 或校验和，干扰内存取证与补丁分析。
		/// </summary>
		static void ApplyAntiTamperPatch()
		{
			if (!CustomerReg.patchApplied)
			{
				CustomerReg.patchApplied = true;
				BinaryReader binaryReader = new BinaryReader(typeof(CustomerReg).Assembly.GetManifestResourceStream("b215f184-b268-4760-a367-fb3a9e128901"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 217;
				array2[0] = 119;
				array2[0] = 112;
				array2[0] = 217;
				array2[1] = 36;
				array2[1] = 127;
				array2[1] = 120;
				array2[1] = 96;
				array2[1] = 18;
				array2[1] = 119;
				array2[1] = 36;
				array2[2] = 63;
				array2[2] = 150;
				array2[2] = 82;
				array2[2] = 63;
				array2[3] = 228;
				array2[3] = 167;
				array2[3] = 66;
				array2[3] = 228;
				array2[4] = 115;
				array2[4] = 135;
				array2[4] = 51;
				array2[4] = 115;
				array2[5] = 27;
				array2[5] = 96;
				array2[5] = 158;
				array2[5] = 27;
				array2[6] = 14;
				array2[6] = 83;
				array2[6] = 149;
				array2[6] = 76;
				array2[6] = 130;
				array2[6] = 134;
				array2[6] = 14;
				array2[7] = 222;
				array2[7] = 97;
				array2[7] = 62;
				array2[7] = 212;
				array2[7] = 114;
				array2[7] = 101;
				array2[7] = 222;
				array2[8] = 248;
				array2[8] = 104;
				array2[8] = 159;
				array2[8] = 151;
				array2[8] = 248;
				array2[9] = 249;
				array2[9] = 70;
				array2[9] = 148;
				array2[9] = 249;
				array2[10] = 227;
				array2[10] = 165;
				array2[10] = 182;
				array2[10] = 227;
				array2[11] = 51;
				array2[11] = 128;
				array2[11] = 186;
				array2[11] = 59;
				array2[11] = 156;
				array2[11] = 193;
				array2[11] = 51;
				array2[12] = 122;
				array2[12] = 105;
				array2[12] = 94;
				array2[12] = 122;
				array2[13] = 8;
				array2[13] = 106;
				array2[13] = 96;
				array2[13] = 8;
				array2[14] = 245;
				array2[14] = 97;
				array2[14] = 97;
				array2[14] = 212;
				array2[14] = 167;
				array2[14] = 146;
				array2[14] = 245;
				array2[15] = 91;
				array2[15] = 204;
				array2[15] = 195;
				array2[15] = 103;
				array2[15] = 106;
				array2[15] = 91;
				array2[16] = 111;
				array2[16] = 162;
				array2[16] = 108;
				array2[16] = 173;
				array2[16] = 152;
				array2[16] = 188;
				array2[16] = 111;
				array2[17] = 192;
				array2[17] = 37;
				array2[17] = 131;
				array2[17] = 192;
				array2[18] = 53;
				array2[18] = 92;
				array2[18] = 163;
				array2[18] = 137;
				array2[18] = 56;
				array2[18] = 53;
				array2[19] = 123;
				array2[19] = 96;
				array2[19] = 166;
				array2[19] = 123;
				array2[20] = 90;
				array2[20] = 144;
				array2[20] = 166;
				array2[20] = 104;
				array2[20] = 118;
				array2[20] = 90;
				array2[21] = 114;
				array2[21] = 101;
				array2[21] = 87;
				array2[21] = 60;
				array2[21] = 114;
				array2[22] = 183;
				array2[22] = 31;
				array2[22] = 150;
				array2[22] = 183;
				array2[23] = 202;
				array2[23] = 149;
				array2[23] = 117;
				array2[23] = 100;
				array2[23] = 146;
				array2[23] = 202;
				array2[24] = 157;
				array2[24] = 103;
				array2[24] = 97;
				array2[24] = 128;
				array2[24] = 182;
				array2[24] = 157;
				array2[25] = 229;
				array2[25] = 79;
				array2[25] = 112;
				array2[25] = 120;
				array2[25] = 44;
				array2[25] = 118;
				array2[25] = 229;
				array2[26] = 81;
				array2[26] = 164;
				array2[26] = 138;
				array2[26] = 195;
				array2[26] = 81;
				array2[27] = 226;
				array2[27] = 121;
				array2[27] = 130;
				array2[27] = 226;
				array2[28] = 162;
				array2[28] = 123;
				array2[28] = 127;
				array2[28] = 162;
				array2[29] = 82;
				array2[29] = 98;
				array2[29] = 139;
				array2[29] = 157;
				array2[29] = 150;
				array2[29] = 41;
				array2[29] = 82;
				array2[30] = 148;
				array2[30] = 160;
				array2[30] = 172;
				array2[30] = 118;
				array2[30] = 148;
				array2[31] = 178;
				array2[31] = 121;
				array2[31] = 134;
				array2[31] = 178;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 119;
				array4[0] = 112;
				array4[0] = 158;
				array4[0] = 232;
				array4[1] = 103;
				array4[1] = 158;
				array4[1] = 94;
				array4[1] = 117;
				array4[2] = 104;
				array4[2] = 90;
				array4[2] = 74;
				array4[2] = 108;
				array4[2] = 145;
				array4[2] = 62;
				array4[3] = 106;
				array4[3] = 200;
				array4[3] = 130;
				array4[3] = 108;
				array4[3] = 192;
				array4[4] = 97;
				array4[4] = 112;
				array4[4] = 96;
				array4[4] = 158;
				array4[4] = 21;
				array4[5] = 124;
				array4[5] = 142;
				array4[5] = 137;
				array4[6] = 106;
				array4[6] = 164;
				array4[6] = 132;
				array4[6] = 67;
				array4[6] = 113;
				array4[7] = 161;
				array4[7] = 151;
				array4[7] = 108;
				array4[7] = 125;
				array4[7] = 104;
				array4[7] = 24;
				array4[8] = 120;
				array4[8] = 70;
				array4[8] = 148;
				array4[8] = 150;
				array4[8] = 3;
				array4[9] = 153;
				array4[9] = 126;
				array4[9] = 197;
				array4[9] = 226;
				array4[10] = 186;
				array4[10] = 90;
				array4[10] = 145;
				array4[10] = 228;
				array4[11] = 146;
				array4[11] = 104;
				array4[11] = 92;
				array4[11] = 184;
				array4[11] = 86;
				array4[12] = 186;
				array4[12] = 92;
				array4[12] = 150;
				array4[12] = 137;
				array4[13] = 97;
				array4[13] = 212;
				array4[13] = 167;
				array4[13] = 160;
				array4[14] = 142;
				array4[14] = 204;
				array4[14] = 195;
				array4[14] = 103;
				array4[14] = 195;
				array4[15] = 133;
				array4[15] = 100;
				array4[15] = 102;
				array4[15] = 134;
				array4[15] = 92;
				byte[] array5 = array4;
				Array.Reverse(array5);
				byte[] publicKeyToken = typeof(CustomerReg).Assembly.GetName().GetPublicKeyToken();
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
				Assembly assembly = typeof(CustomerReg).Assembly;
				intptr_ = CustomerReg.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				CustomerReg.selfModuleBase = Marshal.GetHINSTANCE(assembly.GetModules()[0]).ToInt64();
				IntPtr zero = IntPtr.Zero;
				int int_ = 0;
				int num16 = binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				for (int m = 0; m < num16; m++)
				{
					IntPtr intPtr = new IntPtr(CustomerReg.selfModuleBase + (long)binaryReader.ReadInt32());
					CustomerReg.VirtualProtect(intPtr, 4, 4, ref int_);
					if (IntPtr.Size == 4)
					{
						CustomerReg.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					else
					{
						CustomerReg.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					CustomerReg.VirtualProtect(intPtr, 4, int_, ref int_);
				}
				try
				{
					while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1L)
					{
						int num17 = binaryReader.ReadInt32();
						IntPtr intptr_2 = new IntPtr(CustomerReg.selfModuleBase + (long)num17);
						int num18 = binaryReader.ReadInt32();
						CustomerReg.VirtualProtect(intptr_2, num18 * 4, 4, ref int_);
						for (int n = 0; n < num18; n++)
						{
							Marshal.WriteInt32(new IntPtr(intptr_2.ToInt64() + (long)(n * 4)), binaryReader.ReadInt32());
						}
						CustomerReg.VirtualProtect(intptr_2, num18 * 4, int_, ref int_);
					}
					CustomerReg.CloseHandle(intptr_);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00005268 File Offset: 0x00003468
		/// <summary>取程序集文件路径：Location / CodeBase（去 file:/// 前缀）/ 反射 Location 属性多级兜底，全失败返回空串。</summary>
		internal static object GetAssemblyFilePath(Assembly assembly_0)
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

		// Token: 0x06000045 RID: 69
		/// <summary>P/Invoke：kernel32.WriteProcessMemory —— 向指定进程内存写入字节（内存补丁用）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x06000046 RID: 70
		/// <summary>P/Invoke：kernel32.ReadProcessMemory —— 读取指定进程内存（本类未实际调用，声明残留）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x06000047 RID: 71
		/// <summary>P/Invoke：kernel32.OpenProcess —— 打开进程句柄（56=PROCESS_ALL_ACCESS，此处打开自身）。</summary>
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint uint_0, int int_0, uint uint_1);

		// Token: 0x06000048 RID: 72
		/// <summary>P/Invoke：kernel32.CloseHandle —— 关闭句柄。</summary>
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr intptr_0);

		// Token: 0x06000049 RID: 73 RVA: 0x00005378 File Offset: 0x00003578
		/// <summary>读文件全部字节：FileStream 循环读取直至文件尾。</summary>
		private static byte[] ReadAllBytes(string string_0)
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

		// Token: 0x0600004A RID: 74 RVA: 0x000053D8 File Offset: 0x000035D8
		/// <summary>
		/// AES 解密（硬编码密钥/IV）：用于解密许可证/配置等载荷。
		/// 密钥：05 50 DC 8D 55 71 78 92 B0 96 DC 9C D9 43 3C D2 8F C3 28 D0 0D B5 20 E0 5B 77 20 39 CD F0 C2 CE；
		/// IV：07 13 8F A1 51 09 0A 1B F0 49 4F 2B 8B 54 1A 2D。
		/// </summary>
		private static byte[] AesDecrypt(byte[] byte_0)
		{
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = CustomerReg.GetSymmetricAlgorithm();
			symmetricAlgorithm.Key = new byte[]
			{
				5,
				80,
				220,
				141,
				85,
				113,
				120,
				146,
				176,
				150,
				220,
				156,
				217,
				67,
				60,
				210,
				143,
				195,
				40,
				208,
				13,
				181,
				32,
				224,
				91,
				119,
				32,
				57,
				205,
				240,
				194,
				206
			};
			symmetricAlgorithm.IV = new byte[]
			{
				7,
				19,
				143,
				161,
				81,
				9,
				10,
				27,
				240,
				73,
				79,
				43,
				139,
				84,
				26,
				45
			};
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(byte_0, 0, byte_0.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00005444 File Offset: 0x00003644
		/// <summary>空桩 1/6：始终返回 null（壳剥离后的残留方法，无调用方）。</summary>
		private byte[] NullStub1()
		{
			return null;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00005444 File Offset: 0x00003644
		/// <summary>空桩 2/6：始终返回 null（壳剥离后的残留方法，无调用方）。</summary>
		private byte[] NullStub2()
		{
			return null;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00005444 File Offset: 0x00003644
		/// <summary>空桩 3/6：始终返回 null（壳剥离后的残留方法，无调用方）。</summary>
		private byte[] NullStub3()
		{
			return null;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00005444 File Offset: 0x00003644
		/// <summary>空桩 4/6：始终返回 null（壳剥离后的残留方法，无调用方）。</summary>
		private byte[] NullStub4()
		{
			return null;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00005444 File Offset: 0x00003644
		/// <summary>空桩 5/6：始终返回 null（壳剥离后的残留方法，无调用方）。</summary>
		private byte[] NullStub5()
		{
			return null;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00005444 File Offset: 0x00003644
		/// <summary>空桩 6/6：始终返回 null（壳剥离后的残留方法，无调用方）。</summary>
		private byte[] NullStub6()
		{
			return null;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000022F8 File Offset: 0x000004F8
		/// <summary>许可证 API 桩 1/6：返回 {1,2}，内含 IntelliLock 演示许可 GUID "{11111-22222-40001-00001}"。</summary>
		internal byte[] LicenseApiStub1()
		{
			int length = "{11111-22222-40001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002313 File Offset: 0x00000513
		/// <summary>许可证 API 桩 2/6：返回 {1,2}，内含 IntelliLock 演示许可 GUID "{11111-22222-40001-00002}"。</summary>
		internal byte[] LicenseApiStub2()
		{
			int length = "{11111-22222-40001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000232E File Offset: 0x0000052E
		/// <summary>许可证 API 桩 3/6：返回 {1,2}，内含 IntelliLock 演示许可 GUID "{11111-22222-50001-00001}"。</summary>
		internal byte[] LicenseApiStub3()
		{
			int length = "{11111-22222-50001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002349 File Offset: 0x00000549
		/// <summary>许可证 API 桩 4/6：返回 {1,2}，内含 IntelliLock 演示许可 GUID "{11111-22222-50001-00002}"。</summary>
		internal byte[] LicenseApiStub4()
		{
			int length = "{11111-22222-50001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002364 File Offset: 0x00000564
		/// <summary>许可证 API 桩 5/6：返回 {1,2}，内含 IntelliLock 演示许可 GUID "{11111-22222-60001-00001}"。</summary>
		internal byte[] LicenseApiStub5()
		{
			int length = "{11111-22222-60001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000237F File Offset: 0x0000057F
		/// <summary>许可证 API 桩 6/6：返回 {1,2}，内含 IntelliLock 演示许可 GUID "{11111-22222-60001-00002}"。</summary>
		internal byte[] LicenseApiStub6()
		{
			int length = "{11111-22222-60001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00005454 File Offset: 0x00003654
		/// <summary>
		/// AES 加密 → Base64：对 string_0 的 UTF-16 字节加密（密钥硬编码），IV = ComputeMd5Hash(UTF-16(string_1))，疑似许可激活辅助。
		/// </summary>
		internal static string AesEncryptToBase64(string string_0, string string_1)
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
			byte[] iv = CustomerReg.ComputeMd5Hash(Encoding.Unicode.GetBytes(string_1));
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = CustomerReg.GetSymmetricAlgorithm();
			symmetricAlgorithm.Key = key;
			symmetricAlgorithm.IV = iv;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.Close();
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>混淆填充：恒真，无业务意义（ConfuserEx 填充方法，无调用方）。</summary>
		internal static bool ConfuserFillerTrue()
		{
			return true;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>混淆填充：恒假，无业务意义（ConfuserEx 填充方法，无调用方）。</summary>
		internal static bool ConfuserFillerFalse()
		{
			return false;
		}

		// Token: 0x04000016 RID: 22
		/// <summary>MD5 K 常量表：64 个标准 MD5 轮常量（首项 3614090360=0xD76AA478），证实为 MD5。</summary>
		private static uint[] md5KConstants = new uint[]
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

		// Token: 0x04000017 RID: 23
		/// <summary>未使用字段（混淆填充）：字符串数组，仅在静态构造中置空。</summary>
		private static string[] unusedSystemArray;

		// Token: 0x04000018 RID: 24
		/// <summary>未使用字段（混淆填充）：整型数组，仅在静态构造中置空。</summary>
		private static int[] unusedFieldArray;

		// Token: 0x04000019 RID: 25
		/// <summary>自身模块基址：Marshal.GetHINSTANCE(GetModules()[0])，ApplyAntiTamperPatch 补丁计算用。</summary>
		private static long selfModuleBase;

		// Token: 0x0400001A RID: 26
		/// <summary>未使用字段（混淆填充）：整型，仅在静态构造中置 0。</summary>
		private static int unusedIdentifier;

		// Token: 0x0400001B RID: 27
		/// <summary>未使用字段（混淆填充）：字节数组，仅在静态构造中置空。</summary>
		private static byte[] unusedParam;

		// Token: 0x0400001C RID: 28
		/// <summary>FIPS 标志：系统是否强制 FIPS 算法（DetectFipsPolicy 检测结果）。</summary>
		private static bool fipsFlag;

		// Token: 0x0400001D RID: 29
		/// <summary>未使用字段（混淆填充）：整型，仅在静态构造中置 1。</summary>
		private static int unusedMock;

		// Token: 0x0400001E RID: 30
		/// <summary>FIPS 已检测标志：保证 DetectFipsPolicy 只执行一次。</summary>
		private static bool fipsChecked = false;

		// Token: 0x0400001F RID: 31
		/// <summary>未使用字段（混淆填充）：句柄，仅在静态构造中置 IntPtr.Zero。</summary>
		private static IntPtr unusedRegistry;

		// Token: 0x04000020 RID: 32
		/// <summary>未使用字段（混淆填充）：句柄，仅在静态构造中置 IntPtr.Zero。</summary>
		private static IntPtr unusedMapper;

		// Token: 0x04000021 RID: 33
		/// <summary>未使用字段（混淆填充）：字节数组，仅在静态构造中置空。</summary>
		private static byte[] unusedContainer;

		// Token: 0x04000022 RID: 34
		/// <summary>未使用字段（混淆填充）：字节数组，仅在静态构造中置空。</summary>
		private static byte[] unusedUtils;

		// Token: 0x04000023 RID: 35
		/// <summary>解密后字符串表：一次性解密缓存（DecryptStringTable 使用）。</summary>
		private static byte[] decryptedStringTable;

		// Token: 0x04000024 RID: 36
		/// <summary>ApplyAntiTamperPatch 执行标志：防止内存补丁重复执行。</summary>
		private static bool patchApplied;

		// Token: 0x0200000E RID: 14
		/// <summary>保护器注入的标记 Attribute（无逻辑，仅元数据标记，可忽略）。</summary>
		internal class AnnotationReg : Attribute
		{
			// Token: 0x0600005B RID: 91 RVA: 0x0000239A File Offset: 0x0000059A
			[CustomerReg.AnnotationReg(typeof(CustomerReg.AnnotationReg.ComposerReg<object>[]))]
			public AnnotationReg(object object_0)
			{
			}

			// Token: 0x0200000F RID: 15
		/// <summary>空泛型嵌套类（混淆填充，无成员意义）。</summary>
			internal class ComposerReg<T>
			{
				// Token: 0x0600005D RID: 93 RVA: 0x000021BC File Offset: 0x000003BC
				internal static bool ConfuserFillerTrue()
				{
					return true;
				}

				// Token: 0x0600005E RID: 94 RVA: 0x000021BF File Offset: 0x000003BF
				internal static bool ConfuserFillerFalse()
				{
					return false;
				}
			}
		}

		// Token: 0x02000010 RID: 16
		[Flags]
		/// <summary>空枚举（混淆装饰，无成员）。</summary>
		private enum WebBrowserPermissionLevel
		{

		}
	}
}
