using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace about.Internal
{
	// Token: 0x02000010 RID: 16
	/// <summary>
	/// .NET Reactor（Eziriz）加壳运行时（原名 InvocationProduct）——壳而非业务逻辑，但极具 IOC 价值：
	/// 1. ApplySelfMemoryPatch：读取嵌入资源 002a5299-... 解密后经 OpenProcess/WriteProcessMemory/VirtualProtect 对自身模块内存打补丁（反篡改/运行时解包）；
	/// 2. GetEncryptedString：读取嵌入资源 5182af00-...（AES-CBC 解密 + 自定义搅乱变换）还原被加密的字符串常量表；
	/// 3. ComputeMd5Manual + Md5RoundF/G/H/I + RotateLeft + md5ConstantsK：手工 MD5（FIPS 模式下替代 MD5CryptoServiceProvider）；
	/// 4. 其余为 AES 加解密/文件读取/IntelliLock 许可证桩等辅助。
	/// 所有方法名被混淆为 *Reponse，并塞入大量 *Listener 恒值桩。
	/// </summary>
	internal class InvocationProduct
	{
		// Token: 0x0600007C RID: 124 RVA: 0x000047A4 File Offset: 0x000029A4
		/// <summary>静态构造函数：初始化全部壳运行时状态字段（FIPS 标志、缓存、模块基址等）。</summary>
		static InvocationProduct()
		{
			InvocationProduct.fipsCheckDone = false;
			InvocationProduct.fipsEnabled = false;
			InvocationProduct.m_ObserverProduct = new byte[0];
			InvocationProduct.m_CallbackProduct = new byte[0];
			InvocationProduct.decryptedStringTable = new byte[0];
			InvocationProduct.importerProduct = new byte[0];
			InvocationProduct.roleProduct = IntPtr.Zero;
			InvocationProduct.m_MerchantProduct = IntPtr.Zero;
			InvocationProduct._GlobalProduct = new string[0];
			InvocationProduct._ConfigProduct = new int[0];
			InvocationProduct.m_ManagerProduct = 1;
			InvocationProduct.moduleBaseAddress = 0L;
			InvocationProduct.algoProduct = 0;
			InvocationProduct.selfPatchApplied = false;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000222E File Offset: 0x0000042E
		/// <summary>空方法（method_0，混淆残留），无逻辑。</summary>
		private void method_0()
		{
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000484C File Offset: 0x00002A4C
		/// <summary>
		/// 手工实现的标准 MD5（原名 InitReponse）：填充（0x80 + 64 位长度）、16 字分块、
		/// 4 轮 64 步压缩（K 常量表 md5ConstantsK，轮函数 Md5RoundF/G/H/I、RotateLeft 循环左移），小端输出 16 字节摘要。
		/// 用于 FIPS 受限环境下替代 MD5CryptoServiceProvider。
		/// </summary>
		internal static byte[] ComputeMd5Manual(object object_0)
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
				InvocationProduct.Md5RoundF(ref num6, num7, num8, num9, 0U, 7, 1U, array);
				InvocationProduct.Md5RoundF(ref num9, num6, num7, num8, 1U, 12, 2U, array);
				InvocationProduct.Md5RoundF(ref num8, num9, num6, num7, 2U, 17, 3U, array);
				InvocationProduct.Md5RoundF(ref num7, num8, num9, num6, 3U, 22, 4U, array);
				InvocationProduct.Md5RoundF(ref num6, num7, num8, num9, 4U, 7, 5U, array);
				InvocationProduct.Md5RoundF(ref num9, num6, num7, num8, 5U, 12, 6U, array);
				InvocationProduct.Md5RoundF(ref num8, num9, num6, num7, 6U, 17, 7U, array);
				InvocationProduct.Md5RoundF(ref num7, num8, num9, num6, 7U, 22, 8U, array);
				InvocationProduct.Md5RoundF(ref num6, num7, num8, num9, 8U, 7, 9U, array);
				InvocationProduct.Md5RoundF(ref num9, num6, num7, num8, 9U, 12, 10U, array);
				InvocationProduct.Md5RoundF(ref num8, num9, num6, num7, 10U, 17, 11U, array);
				InvocationProduct.Md5RoundF(ref num7, num8, num9, num6, 11U, 22, 12U, array);
				InvocationProduct.Md5RoundF(ref num6, num7, num8, num9, 12U, 7, 13U, array);
				InvocationProduct.Md5RoundF(ref num9, num6, num7, num8, 13U, 12, 14U, array);
				InvocationProduct.Md5RoundF(ref num8, num9, num6, num7, 14U, 17, 15U, array);
				InvocationProduct.Md5RoundF(ref num7, num8, num9, num6, 15U, 22, 16U, array);
				InvocationProduct.Md5RoundG(ref num6, num7, num8, num9, 1U, 5, 17U, array);
				InvocationProduct.Md5RoundG(ref num9, num6, num7, num8, 6U, 9, 18U, array);
				InvocationProduct.Md5RoundG(ref num8, num9, num6, num7, 11U, 14, 19U, array);
				InvocationProduct.Md5RoundG(ref num7, num8, num9, num6, 0U, 20, 20U, array);
				InvocationProduct.Md5RoundG(ref num6, num7, num8, num9, 5U, 5, 21U, array);
				InvocationProduct.Md5RoundG(ref num9, num6, num7, num8, 10U, 9, 22U, array);
				InvocationProduct.Md5RoundG(ref num8, num9, num6, num7, 15U, 14, 23U, array);
				InvocationProduct.Md5RoundG(ref num7, num8, num9, num6, 4U, 20, 24U, array);
				InvocationProduct.Md5RoundG(ref num6, num7, num8, num9, 9U, 5, 25U, array);
				InvocationProduct.Md5RoundG(ref num9, num6, num7, num8, 14U, 9, 26U, array);
				InvocationProduct.Md5RoundG(ref num8, num9, num6, num7, 3U, 14, 27U, array);
				InvocationProduct.Md5RoundG(ref num7, num8, num9, num6, 8U, 20, 28U, array);
				InvocationProduct.Md5RoundG(ref num6, num7, num8, num9, 13U, 5, 29U, array);
				InvocationProduct.Md5RoundG(ref num9, num6, num7, num8, 2U, 9, 30U, array);
				InvocationProduct.Md5RoundG(ref num8, num9, num6, num7, 7U, 14, 31U, array);
				InvocationProduct.Md5RoundG(ref num7, num8, num9, num6, 12U, 20, 32U, array);
				InvocationProduct.Md5RoundH(ref num6, num7, num8, num9, 5U, 4, 33U, array);
				InvocationProduct.Md5RoundH(ref num9, num6, num7, num8, 8U, 11, 34U, array);
				InvocationProduct.Md5RoundH(ref num8, num9, num6, num7, 11U, 16, 35U, array);
				InvocationProduct.Md5RoundH(ref num7, num8, num9, num6, 14U, 23, 36U, array);
				InvocationProduct.Md5RoundH(ref num6, num7, num8, num9, 1U, 4, 37U, array);
				InvocationProduct.Md5RoundH(ref num9, num6, num7, num8, 4U, 11, 38U, array);
				InvocationProduct.Md5RoundH(ref num8, num9, num6, num7, 7U, 16, 39U, array);
				InvocationProduct.Md5RoundH(ref num7, num8, num9, num6, 10U, 23, 40U, array);
				InvocationProduct.Md5RoundH(ref num6, num7, num8, num9, 13U, 4, 41U, array);
				InvocationProduct.Md5RoundH(ref num9, num6, num7, num8, 0U, 11, 42U, array);
				InvocationProduct.Md5RoundH(ref num8, num9, num6, num7, 3U, 16, 43U, array);
				InvocationProduct.Md5RoundH(ref num7, num8, num9, num6, 6U, 23, 44U, array);
				InvocationProduct.Md5RoundH(ref num6, num7, num8, num9, 9U, 4, 45U, array);
				InvocationProduct.Md5RoundH(ref num9, num6, num7, num8, 12U, 11, 46U, array);
				InvocationProduct.Md5RoundH(ref num8, num9, num6, num7, 15U, 16, 47U, array);
				InvocationProduct.Md5RoundH(ref num7, num8, num9, num6, 2U, 23, 48U, array);
				InvocationProduct.Md5RoundI(ref num6, num7, num8, num9, 0U, 6, 49U, array);
				InvocationProduct.Md5RoundI(ref num9, num6, num7, num8, 7U, 10, 50U, array);
				InvocationProduct.Md5RoundI(ref num8, num9, num6, num7, 14U, 15, 51U, array);
				InvocationProduct.Md5RoundI(ref num7, num8, num9, num6, 5U, 21, 52U, array);
				InvocationProduct.Md5RoundI(ref num6, num7, num8, num9, 12U, 6, 53U, array);
				InvocationProduct.Md5RoundI(ref num9, num6, num7, num8, 3U, 10, 54U, array);
				InvocationProduct.Md5RoundI(ref num8, num9, num6, num7, 10U, 15, 55U, array);
				InvocationProduct.Md5RoundI(ref num7, num8, num9, num6, 1U, 21, 56U, array);
				InvocationProduct.Md5RoundI(ref num6, num7, num8, num9, 8U, 6, 57U, array);
				InvocationProduct.Md5RoundI(ref num9, num6, num7, num8, 15U, 10, 58U, array);
				InvocationProduct.Md5RoundI(ref num8, num9, num6, num7, 6U, 15, 59U, array);
				InvocationProduct.Md5RoundI(ref num7, num8, num9, num6, 13U, 21, 60U, array);
				InvocationProduct.Md5RoundI(ref num6, num7, num8, num9, 4U, 6, 61U, array);
				InvocationProduct.Md5RoundI(ref num9, num6, num7, num8, 11U, 10, 62U, array);
				InvocationProduct.Md5RoundI(ref num8, num9, num6, num7, 2U, 15, 63U, array);
				InvocationProduct.Md5RoundI(ref num7, num8, num9, num6, 9U, 21, 64U, array);
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

		// Token: 0x0600007F RID: 127 RVA: 0x000022D4 File Offset: 0x000004D4
		/// <summary>MD5 第 1 轮压缩函数 F（原名 PushReponse）：(b&amp;c)|(~b&amp;d)。</summary>
		private static void Md5RoundF(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + InvocationProduct.RotateLeft(uint_0 + ((uint_1 & uint_2) | (~uint_1 & uint_3)) + object_0[(int)uint_4] + InvocationProduct.md5ConstantsK[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000022FD File Offset: 0x000004FD
		/// <summary>MD5 第 2 轮压缩函数 G（原名 InsertReponse）：(b&amp;d)|(c&amp;~d)。</summary>
		private static void Md5RoundG(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + InvocationProduct.RotateLeft(uint_0 + ((uint_1 & uint_3) | (uint_2 & ~uint_3)) + object_0[(int)uint_4] + InvocationProduct.md5ConstantsK[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002326 File Offset: 0x00000526
		/// <summary>MD5 第 3 轮压缩函数 H（原名 WriteReponse）：b^c^d。</summary>
		private static void Md5RoundH(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + InvocationProduct.RotateLeft(uint_0 + (uint_1 ^ uint_2 ^ uint_3) + object_0[(int)uint_4] + InvocationProduct.md5ConstantsK[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000234C File Offset: 0x0000054C
		/// <summary>MD5 第 4 轮压缩函数 I（原名 DisableReponse）：c^(b|~d)。</summary>
		private static void Md5RoundI(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + InvocationProduct.RotateLeft(uint_0 + (uint_2 ^ (uint_1 | ~uint_3)) + object_0[(int)uint_4] + InvocationProduct.md5ConstantsK[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00002373 File Offset: 0x00000573
		/// <summary>32 位循环左移 rotl（原名 ReadReponse）。</summary>
		private static uint RotateLeft(uint uint_0, ushort ushort_0)
		{
			return uint_0 >> (int)(32 - ushort_0) | uint_0 << (int)ushort_0;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00002385 File Offset: 0x00000585
		/// <summary>检测 FIPS 策略（原名 FillReponse）：一次性检测（fipsCheckDone 防重复），返回 fipsEnabled。</summary>
		internal static bool IsFipsMode()
		{
			if (!InvocationProduct.fipsCheckDone)
			{
				InvocationProduct.DetectFipsPolicy();
				InvocationProduct.fipsCheckDone = true;
			}
			return InvocationProduct.fipsEnabled;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00004EAC File Offset: 0x000030AC
		/// <summary>
		/// 反射读取 FIPS 策略（原名 PrepareReponse）：优先 CryptoConfig.AllowOnlyFipsAlgorithms，
		/// 失败则读内部 Utils.FipsAlgorithmPolicy == 1。
		/// </summary>
		internal static void DetectFipsPolicy()
		{
			try
			{
				InvocationProduct.fipsEnabled = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
				try
				{
					if ((int)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.Utils", false).GetMethod("get_FipsAlgorithmPolicy", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[0]) == 1)
					{
						InvocationProduct.fipsEnabled = true;
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004F5C File Offset: 0x0000315C
		/// <summary>创建 AES/Rijndael 算法实例（原名 LogoutReponse）：FIPS 模式下用 AesCryptoServiceProvider，否则 RijndaelManaged。</summary>
		internal static SymmetricAlgorithm CreateSymmetricAlgorithm()
		{
			SymmetricAlgorithm result = null;
			if (InvocationProduct.IsFipsMode())
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

		// Token: 0x06000087 RID: 135 RVA: 0x0000239E File Offset: 0x0000059E
		/// <summary>MD5 哈希（原名 CountReponse，FIPS 兼容）：非 FIPS 直接 MD5CryptoServiceProvider，FIPS 走手工实现。被 RegProduct 用于派生嵌入资源名。</summary>
		internal static byte[] ComputeMd5(byte[] byte_0)
		{
			if (!InvocationProduct.IsFipsMode())
			{
				return new MD5CryptoServiceProvider().ComputeHash(byte_0);
			}
			return InvocationProduct.ComputeMd5Manual(byte_0);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004FB4 File Offset: 0x000031B4
		/// <summary>
		/// 解密加密字符串表并取第 i 条字符串（原名 LoginReponse）：
		/// 1. 读嵌入资源 5182af00-bdc6-4082-952b-72890357d973；
		/// 2. 构造 32 字节密钥（硬编码字节经公钥 Token 异或，16 字节 IV = 公钥 Token）；
		/// 3. AES-CBC（CreateSymmetricAlgorithm）解密 → 逐 4 字节「搅乱」变换（常量 3753412436/678874538/179542347/522623986）；
		/// 4. 结果缓存到 decryptedStringTable；按偏移读 length 前缀的 UTF-16 字符串返回。
		/// 语义：Reactor 字符串加密还原——被加密的字符串常量（如注册表路径等）经此表在运行时恢复。
		/// </summary>
		static string GetEncryptedString(int int_0)
		{
			if (InvocationProduct.decryptedStringTable.Length == 0)
			{
				BinaryReader binaryReader = new BinaryReader(typeof(InvocationProduct).Assembly.GetManifestResourceStream("5182af00-bdc6-4082-952b-72890357d973"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 114;
				array2[0] = 163;
				array2[0] = 40;
				array2[0] = 184;
				array2[1] = 204;
				array2[1] = 146;
				array2[1] = 126;
				array2[2] = 86;
				array2[2] = 130;
				array2[2] = 249;
				array2[3] = 72;
				array2[3] = 206;
				array2[3] = 101;
				array2[3] = 97;
				array2[3] = 182;
				array2[4] = 137;
				array2[4] = 94;
				array2[4] = 88;
				array2[4] = 101;
				array2[4] = 218;
				array2[5] = 130;
				array2[5] = 84;
				array2[5] = 160;
				array2[5] = 167;
				array2[5] = 93;
				array2[5] = 154;
				array2[6] = 140;
				array2[6] = 92;
				array2[6] = 122;
				array2[6] = 229;
				array2[7] = 165;
				array2[7] = 58;
				array2[7] = 155;
				array2[7] = 116;
				array2[7] = 148;
				array2[7] = 63;
				array2[8] = 177;
				array2[8] = 142;
				array2[8] = 129;
				array2[8] = 154;
				array2[8] = 96;
				array2[9] = 134;
				array2[9] = 77;
				array2[9] = 129;
				array2[9] = 200;
				array2[10] = 154;
				array2[10] = 162;
				array2[10] = 110;
				array2[10] = 163;
				array2[11] = 101;
				array2[11] = 69;
				array2[11] = 217;
				array2[11] = 119;
				array2[11] = 171;
				array2[12] = 177;
				array2[12] = 94;
				array2[12] = 26;
				array2[13] = 87;
				array2[13] = 141;
				array2[13] = 139;
				array2[13] = 63;
				array2[14] = 128;
				array2[14] = 153;
				array2[14] = 110;
				array2[14] = 48;
				array2[14] = 139;
				array2[14] = 122;
				array2[15] = 131;
				array2[15] = 199;
				array2[15] = 161;
				array2[15] = 161;
				array2[16] = 154;
				array2[16] = 114;
				array2[16] = 87;
				array2[16] = 187;
				array2[17] = 131;
				array2[17] = 43;
				array2[17] = 114;
				array2[17] = 160;
				array2[18] = 130;
				array2[18] = 105;
				array2[18] = 63;
				array2[19] = 144;
				array2[19] = 98;
				array2[19] = 100;
				array2[20] = 116;
				array2[20] = 162;
				array2[20] = 58;
				array2[20] = 123;
				array2[20] = 191;
				array2[21] = 109;
				array2[21] = 152;
				array2[21] = 90;
				array2[21] = 113;
				array2[22] = 204;
				array2[22] = 167;
				array2[22] = 119;
				array2[22] = 124;
				array2[23] = 138;
				array2[23] = 86;
				array2[23] = 244;
				array2[24] = 75;
				array2[24] = 110;
				array2[24] = 122;
				array2[24] = 120;
				array2[24] = 173;
				array2[24] = 111;
				array2[25] = 110;
				array2[25] = 150;
				array2[25] = 231;
				array2[26] = 147;
				array2[26] = 154;
				array2[26] = 118;
				array2[26] = 115;
				array2[26] = 166;
				array2[26] = 211;
				array2[27] = 169;
				array2[27] = 169;
				array2[27] = 72;
				array2[27] = 168;
				array2[27] = 117;
				array2[28] = 8;
				array2[28] = 92;
				array2[28] = 79;
				array2[29] = 54;
				array2[29] = 170;
				array2[29] = 109;
				array2[29] = 13;
				array2[30] = 112;
				array2[30] = 136;
				array2[30] = 191;
				array2[30] = 185;
				array2[31] = 122;
				array2[31] = 129;
				array2[31] = 87;
				array2[31] = 126;
				array2[31] = 98;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 114;
				array4[0] = 138;
				array4[0] = 168;
				array4[1] = 40;
				array4[1] = 85;
				array4[1] = 146;
				array4[1] = 146;
				array4[1] = 163;
				array4[2] = 101;
				array4[2] = 198;
				array4[2] = 74;
				array4[3] = 100;
				array4[3] = 114;
				array4[3] = 126;
				array4[3] = 159;
				array4[3] = 198;
				array4[4] = 55;
				array4[4] = 110;
				array4[4] = 98;
				array4[4] = 140;
				array4[4] = 125;
				array4[5] = 115;
				array4[5] = 169;
				array4[5] = 70;
				array4[5] = 251;
				array4[6] = 87;
				array4[6] = 155;
				array4[6] = 167;
				array4[6] = 162;
				array4[7] = 153;
				array4[7] = 111;
				array4[7] = 152;
				array4[8] = 65;
				array4[8] = 165;
				array4[8] = 208;
				array4[9] = 94;
				array4[9] = 217;
				array4[9] = 138;
				array4[9] = 26;
				array4[10] = 150;
				array4[10] = 100;
				array4[10] = 145;
				array4[10] = 102;
				array4[10] = 17;
				array4[11] = 93;
				array4[11] = 161;
				array4[11] = 149;
				array4[11] = 127;
				array4[12] = 129;
				array4[12] = 77;
				array4[12] = 152;
				array4[12] = 162;
				array4[12] = 110;
				array4[12] = 215;
				array4[13] = 89;
				array4[13] = 69;
				array4[13] = 227;
				array4[14] = 166;
				array4[14] = 162;
				array4[14] = 206;
				array4[14] = 169;
				array4[15] = 179;
				array4[15] = 89;
				array4[15] = 116;
				array4[15] = 187;
				byte[] array5 = array4;
				byte[] publicKeyToken = typeof(InvocationProduct).Assembly.GetName().GetPublicKeyToken();
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
					SymmetricAlgorithm symmetricAlgorithm = InvocationProduct.CreateSymmetricAlgorithm();
					symmetricAlgorithm.Mode = CipherMode.CBC;
					ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor(array3, array5);
					MemoryStream memoryStream = new MemoryStream();
					CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
					cryptoStream.Write(array, 0, array.Length);
					cryptoStream.FlushFinalBlock();
					InvocationProduct.decryptedStringTable = memoryStream.ToArray();
					memoryStream.Close();
					cryptoStream.Close();
					binaryReader.Close();
					array = InvocationProduct.decryptedStringTable;
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
				InvocationProduct.decryptedStringTable = array6;
			}
			int num17 = BitConverter.ToInt32(InvocationProduct.decryptedStringTable, int_0);
			try
			{
				byte[] array7 = new byte[num17];
				Array.Copy(InvocationProduct.decryptedStringTable, int_0 + 4, array7, 0, num17);
				return Encoding.Unicode.GetString(array7, 0, array7.Length);
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005E4C File Offset: 0x0000404C
		/// <summary>Base64 → UTF-16 解码（原名 ConnectReponse）：解密后的常量字符串还原工具。</summary>
		internal static string Base64ToUtf16String(string string_0)
		{
			byte[] array = Convert.FromBase64String(string_0);
			return Encoding.Unicode.GetString(array, 0, array.Length);
		}

		// Token: 0x0600008A RID: 138
		/// <summary>kernel32 VirtualProtect（内存补丁用）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int VirtualProtect(IntPtr intptr_0, int int_0, int int_1, ref int int_2);

		// Token: 0x0600008B RID: 139 RVA: 0x00005E70 File Offset: 0x00004070
		/// <summary>
		/// 内存反篡改/解包补丁（原名 ManageReponse），一次性（selfPatchApplied 保护）：
		/// 1. 读嵌入资源 002a5299-65c7-44dc-af9b-810aaba51bee；
		/// 2. 硬编码 32 字节密钥（与程序集公钥 Token 异或）做自定义逐 4 字节「搅乱」解密；
		/// 3. OpenProcess(PROCESS_ALL_ACCESS=56) 打开自身进程；moduleBaseAddress = Marshal.GetHINSTANCE(模块[0])；
		/// 4. 读取补丁表：先按 (RVA, value) 对 4 字节位置 VirtualProtect(可写) → WriteProcessMemory → VirtualProtect(还原)；
		///    再按 (RVA, 长度, 数据) 批量写入；最后 CloseHandle。
		/// 语义：运行时解包/防篡改——被加密的代码/常量在运行时原地还原到内存镜像。
		/// </summary>
		[InvocationProduct.StrategyProduct(typeof(InvocationProduct.StrategyProduct.ProducerProduct<object>[]))]
		static void ApplySelfMemoryPatch()
		{
			if (!InvocationProduct.selfPatchApplied)
			{
				InvocationProduct.selfPatchApplied = true;
				BinaryReader binaryReader = new BinaryReader(typeof(InvocationProduct).Assembly.GetManifestResourceStream("002a5299-65c7-44dc-af9b-810aaba51bee"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 63;
				array2[0] = 114;
				array2[0] = 158;
				array2[0] = 63;
				array2[1] = 234;
				array2[1] = 168;
				array2[1] = 169;
				array2[1] = 86;
				array2[1] = 234;
				array2[2] = 60;
				array2[2] = 147;
				array2[2] = 175;
				array2[2] = 100;
				array2[2] = 72;
				array2[2] = 60;
				array2[3] = 76;
				array2[3] = 98;
				array2[3] = 107;
				array2[3] = 87;
				array2[3] = 134;
				array2[3] = 87;
				array2[3] = 76;
				array2[4] = 239;
				array2[4] = 12;
				array2[4] = 137;
				array2[4] = 239;
				array2[5] = 239;
				array2[5] = 207;
				array2[5] = 139;
				array2[5] = 145;
				array2[5] = 90;
				array2[5] = 28;
				array2[5] = 239;
				array2[6] = 177;
				array2[6] = 164;
				array2[6] = 105;
				array2[6] = 177;
				array2[7] = 182;
				array2[7] = 85;
				array2[7] = 88;
				array2[7] = 182;
				array2[8] = 115;
				array2[8] = 137;
				array2[8] = 90;
				array2[8] = 128;
				array2[8] = 116;
				array2[8] = 115;
				array2[9] = 59;
				array2[9] = 220;
				array2[9] = 128;
				array2[9] = 97;
				array2[9] = 59;
				array2[10] = 178;
				array2[10] = 126;
				array2[10] = 168;
				array2[10] = 161;
				array2[10] = 117;
				array2[10] = 178;
				array2[11] = 34;
				array2[11] = 151;
				array2[11] = 92;
				array2[11] = 118;
				array2[11] = 136;
				array2[11] = 64;
				array2[11] = 34;
				array2[12] = 241;
				array2[12] = 129;
				array2[12] = 158;
				array2[12] = 241;
				array2[13] = 210;
				array2[13] = 152;
				array2[13] = 105;
				array2[13] = 29;
				array2[13] = 210;
				array2[14] = 156;
				array2[14] = 155;
				array2[14] = 180;
				array2[14] = 156;
				array2[15] = 16;
				array2[15] = 108;
				array2[15] = 48;
				array2[15] = 126;
				array2[15] = 69;
				array2[15] = 73;
				array2[15] = 16;
				array2[16] = 15;
				array2[16] = 110;
				array2[16] = 157;
				array2[16] = 92;
				array2[16] = 165;
				array2[16] = 159;
				array2[16] = 15;
				array2[17] = 201;
				array2[17] = 174;
				array2[17] = 110;
				array2[17] = 144;
				array2[17] = 120;
				array2[17] = 201;
				array2[18] = 62;
				array2[18] = 106;
				array2[18] = 103;
				array2[18] = 141;
				array2[18] = 61;
				array2[18] = 62;
				array2[19] = 166;
				array2[19] = 173;
				array2[19] = 231;
				array2[19] = 166;
				array2[20] = 74;
				array2[20] = 169;
				array2[20] = 90;
				array2[20] = 127;
				array2[20] = 149;
				array2[20] = 74;
				array2[21] = 244;
				array2[21] = 143;
				array2[21] = 102;
				array2[21] = 244;
				array2[22] = 152;
				array2[22] = 94;
				array2[22] = 141;
				array2[22] = 126;
				array2[22] = 152;
				array2[23] = 0;
				array2[23] = 130;
				array2[23] = 44;
				array2[23] = 0;
				array2[24] = 220;
				array2[24] = 131;
				array2[24] = 155;
				array2[24] = 220;
				array2[25] = 105;
				array2[25] = 132;
				array2[25] = 91;
				array2[25] = 124;
				array2[25] = 124;
				array2[25] = 105;
				array2[26] = 93;
				array2[26] = 95;
				array2[26] = 148;
				array2[26] = 173;
				array2[26] = 137;
				array2[26] = 93;
				array2[27] = 60;
				array2[27] = 56;
				array2[27] = 151;
				array2[27] = 134;
				array2[27] = 173;
				array2[27] = 60;
				array2[28] = 219;
				array2[28] = 173;
				array2[28] = 101;
				array2[28] = 219;
				array2[29] = 103;
				array2[29] = 102;
				array2[29] = 134;
				array2[29] = 139;
				array2[29] = 41;
				array2[29] = 165;
				array2[29] = 103;
				array2[30] = 167;
				array2[30] = 122;
				array2[30] = 50;
				array2[30] = 126;
				array2[30] = 147;
				array2[30] = 54;
				array2[30] = 167;
				array2[31] = 174;
				array2[31] = 123;
				array2[31] = 215;
				array2[31] = 164;
				array2[31] = 118;
				array2[31] = 134;
				array2[31] = 174;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 114;
				array4[0] = 158;
				array4[0] = 179;
				array4[0] = 189;
				array4[0] = 121;
				array4[0] = 152;
				array4[1] = 117;
				array4[1] = 130;
				array4[1] = 100;
				array4[1] = 129;
				array4[1] = 110;
				array4[1] = 179;
				array4[2] = 92;
				array4[2] = 124;
				array4[2] = 232;
				array4[3] = 43;
				array4[3] = 136;
				array4[3] = 144;
				array4[4] = 141;
				array4[4] = 125;
				array4[4] = 166;
				array4[4] = 141;
				array4[4] = 163;
				array4[4] = 112;
				array4[5] = 89;
				array4[5] = 99;
				array4[5] = 181;
				array4[5] = 201;
				array4[6] = 24;
				array4[6] = 106;
				array4[6] = 162;
				array4[6] = 136;
				array4[6] = 100;
				array4[7] = 130;
				array4[7] = 168;
				array4[7] = 66;
				array4[8] = 78;
				array4[8] = 29;
				array4[8] = 111;
				array4[8] = 123;
				array4[8] = 46;
				array4[9] = 156;
				array4[9] = 108;
				array4[9] = 128;
				array4[10] = 145;
				array4[10] = 192;
				array4[10] = 108;
				array4[10] = 78;
				array4[10] = 159;
				array4[10] = 120;
				array4[11] = 100;
				array4[11] = 128;
				array4[11] = 140;
				array4[11] = 115;
				array4[11] = 107;
				array4[12] = 114;
				array4[12] = 36;
				array4[12] = 154;
				array4[12] = 88;
				array4[12] = 128;
				array4[12] = 7;
				array4[13] = 119;
				array4[13] = 94;
				array4[13] = 166;
				array4[13] = 68;
				array4[14] = 86;
				array4[14] = 140;
				array4[14] = 119;
				array4[14] = 170;
				array4[14] = 115;
				array4[14] = 199;
				array4[15] = 137;
				array4[15] = 216;
				array4[15] = 124;
				array4[15] = 144;
				array4[15] = 52;
				byte[] array5 = array4;
				Array.Reverse(array5);
				byte[] publicKeyToken = typeof(InvocationProduct).Assembly.GetName().GetPublicKeyToken();
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
				Assembly assembly = typeof(InvocationProduct).Assembly;
				intptr_ = InvocationProduct.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				InvocationProduct.moduleBaseAddress = Marshal.GetHINSTANCE(assembly.GetModules()[0]).ToInt64();
				IntPtr zero = IntPtr.Zero;
				int int_ = 0;
				int num16 = binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				for (int m = 0; m < num16; m++)
				{
					IntPtr intPtr = new IntPtr(InvocationProduct.moduleBaseAddress + (long)binaryReader.ReadInt32());
					InvocationProduct.VirtualProtect(intPtr, 4, 4, ref int_);
					if (IntPtr.Size == 4)
					{
						InvocationProduct.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					else
					{
						InvocationProduct.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					InvocationProduct.VirtualProtect(intPtr, 4, int_, ref int_);
				}
				try
				{
					while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1L)
					{
						int num17 = binaryReader.ReadInt32();
						IntPtr intptr_2 = new IntPtr(InvocationProduct.moduleBaseAddress + (long)num17);
						int num18 = binaryReader.ReadInt32();
						InvocationProduct.VirtualProtect(intptr_2, num18 * 4, 4, ref int_);
						for (int n = 0; n < num18; n++)
						{
							Marshal.WriteInt32(new IntPtr(intptr_2.ToInt64() + (long)(n * 4)), binaryReader.ReadInt32());
						}
						InvocationProduct.VirtualProtect(intptr_2, num18 * 4, int_, ref int_);
					}
					InvocationProduct.CloseHandle(intptr_);
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000070E4 File Offset: 0x000052E4
		/// <summary>获取程序集文件路径（原名 IncludeReponse）：多重 fallback（Location → CodeBase 去 file:/// 前缀 → 反射 Location），失败返回空串。</summary>
		internal static object GetAssemblyLocation(Assembly assembly_0)
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

		// Token: 0x0600008D RID: 141
		/// <summary>kernel32 WriteProcessMemory（内存补丁用）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int WriteProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x0600008E RID: 142
		/// <summary>kernel32 ReadProcessMemory（声明未使用，壳残留）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int ReadProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x0600008F RID: 143
		/// <summary>kernel32 OpenProcess（内存补丁用，打开自身进程）。</summary>
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint uint_0, int int_0, uint uint_1);

		// Token: 0x06000090 RID: 144
		/// <summary>kernel32 CloseHandle（内存补丁用）。</summary>
		[DllImport("kernel32.dll")]
		private static extern int CloseHandle(IntPtr intptr_0);

		// Token: 0x06000091 RID: 145 RVA: 0x000071F4 File Offset: 0x000053F4
		/// <summary>读文件为字节数组（原名 TestProduct）。</summary>
		private static byte[] ReadFileToBytes(string string_0)
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

		// Token: 0x06000092 RID: 146 RVA: 0x00007254 File Offset: 0x00005454
		/// <summary>
		/// AES-CBC 解密（原名 RunProduct）：硬编码 32 字节 Key 与 16 字节 IV（见 md 的 IOC 表）。
		/// </summary>
		private static byte[] AesCbcDecrypt(byte[] byte_0)
		{
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = InvocationProduct.CreateSymmetricAlgorithm();
			symmetricAlgorithm.Key = new byte[]
			{
				14,
				38,
				36,
				62,
				87,
				26,
				83,
				240,
				34,
				71,
				119,
				86,
				144,
				152,
				228,
				31,
				39,
				44,
				203,
				233,
				62,
				158,
				3,
				34,
				14,
				33,
				15,
				81,
				229,
				193,
				213,
				127
			};
			symmetricAlgorithm.IV = new byte[]
			{
				214,
				212,
				210,
				126,
				136,
				162,
				151,
				142,
				170,
				36,
				154,
				73,
				218,
				168,
				41,
				78
			};
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(byte_0, 0, byte_0.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000072C0 File Offset: 0x000054C0
		/// <summary>空桩（原名 PopProduct）：恒 return null，无实义。</summary>
		private byte[] NullStub1()
		{
			return null;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000072C0 File Offset: 0x000054C0
		/// <summary>空桩（原名 ResetProduct）：恒 return null，无实义。</summary>
		private byte[] NullStub2()
		{
			return null;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000072C0 File Offset: 0x000054C0
		/// <summary>空桩（原名 CollectProduct）：恒 return null，无实义。</summary>
		private byte[] NullStub3()
		{
			return null;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000072C0 File Offset: 0x000054C0
		/// <summary>空桩（原名 PatchProduct）：恒 return null，无实义。</summary>
		private byte[] NullStub4()
		{
			return null;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000072C0 File Offset: 0x000054C0
		/// <summary>空桩（原名 CustomizeProduct）：恒 return null，无实义。</summary>
		private byte[] NullStub5()
		{
			return null;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000072C0 File Offset: 0x000054C0
		/// <summary>空桩（原名 SearchProduct）：恒 return null，无实义。</summary>
		private byte[] NullStub6()
		{
			return null;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000023B9 File Offset: 0x000005B9
		/// <summary>IntelliLock 许可证桩（原名 VisitProduct）：返回 {1,2}，含 {11111-22222-40001-00001} 占位 GUID。</summary>
		internal byte[] IntelliLockStub1()
		{
			int length = "{11111-22222-40001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000023D4 File Offset: 0x000005D4
		/// <summary>IntelliLock 许可证桩（原名 RegisterProduct）：返回 {1,2}，占位 GUID {11111-22222-40001-00002}。</summary>
		internal byte[] IntelliLockStub2()
		{
			int length = "{11111-22222-40001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000023EF File Offset: 0x000005EF
		/// <summary>IntelliLock 许可证桩（原名 MoveProduct）：返回 {1,2}，占位 GUID {11111-22222-50001-00001}。</summary>
		internal byte[] IntelliLockStub3()
		{
			int length = "{11111-22222-50001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000240A File Offset: 0x0000060A
		/// <summary>IntelliLock 许可证桩（原名 InvokeProduct）：返回 {1,2}，占位 GUID {11111-22222-50001-00002}。</summary>
		internal byte[] IntelliLockStub4()
		{
			int length = "{11111-22222-50001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00002425 File Offset: 0x00000625
		/// <summary>IntelliLock 许可证桩（原名 EnableProduct）：返回 {1,2}，占位 GUID {11111-22222-60001-00001}。</summary>
		internal byte[] IntelliLockStub5()
		{
			int length = "{11111-22222-60001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00002440 File Offset: 0x00000640
		/// <summary>IntelliLock 许可证桩（原名 ConcatProduct）：返回 {1,2}，占位 GUID {11111-22222-60001-00002}。</summary>
		internal byte[] IntelliLockStub6()
		{
			int length = "{11111-22222-60001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000072D0 File Offset: 0x000054D0
		/// <summary>
		/// AES 加密→Base64（原名 DefineProduct）：硬编码 Key（见 md IOC 表），IV = MD5(UTF16(s2))，CBC 加密后 Base64。
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
			byte[] iv = InvocationProduct.ComputeMd5(Encoding.Unicode.GetBytes(string_1));
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = InvocationProduct.CreateSymmetricAlgorithm();
			symmetricAlgorithm.Key = key;
			symmetricAlgorithm.IV = iv;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.Close();
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>混淆填充桩（原名 InvokeListener），恒 true，无实义。</summary>
		internal static bool InvokeListener()
		{
			return true;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000021C9 File Offset: 0x000003C9
		/// <summary>混淆填充桩（原名 PushListener），恒 false，无实义。</summary>
		internal static bool PushListener()
		{
			return false;
		}

		// Token: 0x04000028 RID: 40
		/// <summary>FIPS 检测是否已完成标志（原名 _ResolverProduct）。</summary>
		private static bool fipsCheckDone;

		// Token: 0x04000029 RID: 41
		/// <summary>壳运行时状态字段（原名 m_ObserverProduct），静态初始化置空，未在业务中引用。</summary>
		private static byte[] m_ObserverProduct;

		// Token: 0x0400002A RID: 42
		/// <summary>壳运行时状态字段（原名 m_CallbackProduct），未引用。</summary>
		private static byte[] m_CallbackProduct;

		// Token: 0x0400002B RID: 43
		/// <summary>壳运行时状态字段（原名 roleProduct，IntPtr），未引用。</summary>
		private static IntPtr roleProduct;

		// Token: 0x0400002C RID: 44
		/// <summary>壳运行时状态字段（原名 _GlobalProduct，string[]），未引用。</summary>
		private static string[] _GlobalProduct;

		// Token: 0x0400002D RID: 45
		/// <summary>壳运行时状态字段（原名 m_ManagerProduct，int，静态初始化=1），未引用。</summary>
		private static int m_ManagerProduct;

		// Token: 0x0400002E RID: 46
		/// <summary>解密后的字符串表缓存（原名 infoProduct），GetEncryptedString 首次解密后写入。</summary>
		private static byte[] decryptedStringTable;

		// Token: 0x0400002F RID: 47
		/// <summary>壳运行时状态字段（原名 m_MerchantProduct，IntPtr），未引用。</summary>
		private static IntPtr m_MerchantProduct;

		// Token: 0x04000030 RID: 48
		/// <summary>壳运行时状态字段（原名 _ConfigProduct，int[]），未引用。</summary>
		private static int[] _ConfigProduct;

		// Token: 0x04000031 RID: 49
		/// <summary>壳运行时状态字段（原名 importerProduct，byte[]），未引用。</summary>
		private static byte[] importerProduct;

		// Token: 0x04000032 RID: 50
		/// <summary>壳运行时状态字段（原名 algoProduct，int，静态初始化=0），未引用。</summary>
		private static int algoProduct;

		// Token: 0x04000033 RID: 51
		/// <summary>自身模块基址（HINSTANCE，原名 _WatcherProduct），ApplySelfMemoryPatch 写入，补丁 RVA 基于此计算。</summary>
		private static long moduleBaseAddress;

		// Token: 0x04000034 RID: 52
		/// <summary>MD5 K 常量表（64 项，原名 databaseProduct），标准 MD5 常量。</summary>
		private static uint[] md5ConstantsK = new uint[]
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

		// Token: 0x04000035 RID: 53
		/// <summary>ApplySelfMemoryPatch 一次性标志（原名 schemaProduct）。</summary>
		private static bool selfPatchApplied;

		// Token: 0x04000036 RID: 54
		/// <summary>FIPS 策略检测结果（原名 _IdentifierProduct），由 DetectFipsPolicy 写入。</summary>
		private static bool fipsEnabled;

		// Token: 0x02000011 RID: 17
		/// <summary>混淆器添加的自定义特性（原名 StrategyProduct），无实义，仅作元数据干扰。</summary>
		internal class StrategyProduct : Attribute
		{
			// Token: 0x060000A3 RID: 163 RVA: 0x0000245B File Offset: 0x0000065B
			[InvocationProduct.StrategyProduct(typeof(InvocationProduct.StrategyProduct.ProducerProduct<object>[]))]
			public StrategyProduct(object object_0)
			{
			}

			// Token: 0x02000012 RID: 18
			/// <summary>空泛型占位类（原名 ProducerProduct&lt;T&gt;），混淆残留。</summary>
			internal class ProducerProduct<T>
			{
				// Token: 0x060000A5 RID: 165 RVA: 0x000021C6 File Offset: 0x000003C6
				/// <summary>混淆填充桩（原名 ConnectListener），恒 true，无实义。</summary>
				internal static bool ConnectListener()
				{
					return true;
				}

				// Token: 0x060000A6 RID: 166 RVA: 0x000021C9 File Offset: 0x000003C9
				/// <summary>混淆填充桩（原名 PostListener），恒 false，无实义。</summary>
				internal static bool PostListener()
				{
					return false;
				}
			}
		}

		// Token: 0x02000013 RID: 19
		/// <summary>空 Flags 枚举（原名 ContainerProduct），混淆残留，无成员。</summary>
		[Flags]
		private enum ContainerProduct
		{

		}
	}
}
