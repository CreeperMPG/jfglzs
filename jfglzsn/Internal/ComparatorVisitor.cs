using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000015 RID: 21
	// ============================================================
	// Internal/ComparatorVisitor.cs — .NET Reactor 壳运行时（字符串解密、资源解密、进程内存补丁、MD5/AES 工具）
	// 职责：1) DecryptStringTable 解密嵌入资源字符串表；2) ApplyShellMemoryPatch 内存补丁自身映像（OpenProcess+WriteProcessMemory 还原被 Reactor 改写的常量/方法体）；3) MD5（含 FIPS 自实现）与 AES/Rijndael 工具（AesDecrypt/AesEncryptWithPassword）；4) 大量壳授权桩（LicenseStub1~6/NullStub1~6）。类内多数字段为壳/混淆器保留的伪状态。
	// ============================================================
	internal class ComparatorVisitor
	{
		// Token: 0x060000D4 RID: 212 RVA: 0x00009CF0 File Offset: 0x00007EF0
		/// <summary>
		/// 静态构造器：初始化壳运行时全部伪状态字段（字节数组/句柄/标志），多数仅壳/混淆器保留。
		/// </summary>
		static ComparatorVisitor()
		{
			ComparatorVisitor.fipsCheckedFlag = false;
			ComparatorVisitor.isFipsModeCached = false;
			ComparatorVisitor.m_CallbackVisitor = new byte[0];
			ComparatorVisitor.singletonVisitor = new byte[0];
			ComparatorVisitor.decryptedStringTable = new byte[0];
			ComparatorVisitor._ServerVisitor = new byte[0];
			ComparatorVisitor.stubVisitor = IntPtr.Zero;
			ComparatorVisitor.serviceVisitor = IntPtr.Zero;
			ComparatorVisitor.m_ThreadVisitor = new string[0];
			ComparatorVisitor.m_InfoVisitor = new int[0];
			ComparatorVisitor.m_CollectionVisitor = 1;
			ComparatorVisitor.moduleBaseAddress = 0L;
			ComparatorVisitor.m_RecordVisitor = 0;
			ComparatorVisitor.patchAppliedFlag = false;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000231E File Offset: 0x0000051E
		/// <summary>
		/// 空方法（混淆残留，无逻辑）。
		/// </summary>
		private void method_0()
		{
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00009D98 File Offset: 0x00007F98
		/// <summary>
		/// 自实现 MD5（原名 RevertConfig）：FIPS 模式（系统禁止 MD5CryptoServiceProvider）下手工计算 MD5。
		/// 流程：1. 按 448−len×8 mod 512 计算填充，末尾补 0x80 与 64 位长度；2. 初始化 A=0x67452301/B=0xEFCDAB89/C=0x98BADCFE/D=0x10325476；3. 每 64 字节一块加载 16 个 uint，执行 64 轮 Md5RoundF/G/H/I（左移 7/12/17/22、5/9/14/20、4/11/16/23、6/10/15/21），每轮加 md5KConstants（MD5 K 常量表）；4. 累加回 A/B/C/D；5. 小端输出 16 字节。
		/// </summary>
		internal static byte[] Md5FipsSelfImplemented(object object_0)
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
				ComparatorVisitor.Md5RoundF(ref num6, num7, num8, num9, 0U, 7, 1U, array);
				ComparatorVisitor.Md5RoundF(ref num9, num6, num7, num8, 1U, 12, 2U, array);
				ComparatorVisitor.Md5RoundF(ref num8, num9, num6, num7, 2U, 17, 3U, array);
				ComparatorVisitor.Md5RoundF(ref num7, num8, num9, num6, 3U, 22, 4U, array);
				ComparatorVisitor.Md5RoundF(ref num6, num7, num8, num9, 4U, 7, 5U, array);
				ComparatorVisitor.Md5RoundF(ref num9, num6, num7, num8, 5U, 12, 6U, array);
				ComparatorVisitor.Md5RoundF(ref num8, num9, num6, num7, 6U, 17, 7U, array);
				ComparatorVisitor.Md5RoundF(ref num7, num8, num9, num6, 7U, 22, 8U, array);
				ComparatorVisitor.Md5RoundF(ref num6, num7, num8, num9, 8U, 7, 9U, array);
				ComparatorVisitor.Md5RoundF(ref num9, num6, num7, num8, 9U, 12, 10U, array);
				ComparatorVisitor.Md5RoundF(ref num8, num9, num6, num7, 10U, 17, 11U, array);
				ComparatorVisitor.Md5RoundF(ref num7, num8, num9, num6, 11U, 22, 12U, array);
				ComparatorVisitor.Md5RoundF(ref num6, num7, num8, num9, 12U, 7, 13U, array);
				ComparatorVisitor.Md5RoundF(ref num9, num6, num7, num8, 13U, 12, 14U, array);
				ComparatorVisitor.Md5RoundF(ref num8, num9, num6, num7, 14U, 17, 15U, array);
				ComparatorVisitor.Md5RoundF(ref num7, num8, num9, num6, 15U, 22, 16U, array);
				ComparatorVisitor.Md5RoundG(ref num6, num7, num8, num9, 1U, 5, 17U, array);
				ComparatorVisitor.Md5RoundG(ref num9, num6, num7, num8, 6U, 9, 18U, array);
				ComparatorVisitor.Md5RoundG(ref num8, num9, num6, num7, 11U, 14, 19U, array);
				ComparatorVisitor.Md5RoundG(ref num7, num8, num9, num6, 0U, 20, 20U, array);
				ComparatorVisitor.Md5RoundG(ref num6, num7, num8, num9, 5U, 5, 21U, array);
				ComparatorVisitor.Md5RoundG(ref num9, num6, num7, num8, 10U, 9, 22U, array);
				ComparatorVisitor.Md5RoundG(ref num8, num9, num6, num7, 15U, 14, 23U, array);
				ComparatorVisitor.Md5RoundG(ref num7, num8, num9, num6, 4U, 20, 24U, array);
				ComparatorVisitor.Md5RoundG(ref num6, num7, num8, num9, 9U, 5, 25U, array);
				ComparatorVisitor.Md5RoundG(ref num9, num6, num7, num8, 14U, 9, 26U, array);
				ComparatorVisitor.Md5RoundG(ref num8, num9, num6, num7, 3U, 14, 27U, array);
				ComparatorVisitor.Md5RoundG(ref num7, num8, num9, num6, 8U, 20, 28U, array);
				ComparatorVisitor.Md5RoundG(ref num6, num7, num8, num9, 13U, 5, 29U, array);
				ComparatorVisitor.Md5RoundG(ref num9, num6, num7, num8, 2U, 9, 30U, array);
				ComparatorVisitor.Md5RoundG(ref num8, num9, num6, num7, 7U, 14, 31U, array);
				ComparatorVisitor.Md5RoundG(ref num7, num8, num9, num6, 12U, 20, 32U, array);
				ComparatorVisitor.Md5RoundH(ref num6, num7, num8, num9, 5U, 4, 33U, array);
				ComparatorVisitor.Md5RoundH(ref num9, num6, num7, num8, 8U, 11, 34U, array);
				ComparatorVisitor.Md5RoundH(ref num8, num9, num6, num7, 11U, 16, 35U, array);
				ComparatorVisitor.Md5RoundH(ref num7, num8, num9, num6, 14U, 23, 36U, array);
				ComparatorVisitor.Md5RoundH(ref num6, num7, num8, num9, 1U, 4, 37U, array);
				ComparatorVisitor.Md5RoundH(ref num9, num6, num7, num8, 4U, 11, 38U, array);
				ComparatorVisitor.Md5RoundH(ref num8, num9, num6, num7, 7U, 16, 39U, array);
				ComparatorVisitor.Md5RoundH(ref num7, num8, num9, num6, 10U, 23, 40U, array);
				ComparatorVisitor.Md5RoundH(ref num6, num7, num8, num9, 13U, 4, 41U, array);
				ComparatorVisitor.Md5RoundH(ref num9, num6, num7, num8, 0U, 11, 42U, array);
				ComparatorVisitor.Md5RoundH(ref num8, num9, num6, num7, 3U, 16, 43U, array);
				ComparatorVisitor.Md5RoundH(ref num7, num8, num9, num6, 6U, 23, 44U, array);
				ComparatorVisitor.Md5RoundH(ref num6, num7, num8, num9, 9U, 4, 45U, array);
				ComparatorVisitor.Md5RoundH(ref num9, num6, num7, num8, 12U, 11, 46U, array);
				ComparatorVisitor.Md5RoundH(ref num8, num9, num6, num7, 15U, 16, 47U, array);
				ComparatorVisitor.Md5RoundH(ref num7, num8, num9, num6, 2U, 23, 48U, array);
				ComparatorVisitor.Md5RoundI(ref num6, num7, num8, num9, 0U, 6, 49U, array);
				ComparatorVisitor.Md5RoundI(ref num9, num6, num7, num8, 7U, 10, 50U, array);
				ComparatorVisitor.Md5RoundI(ref num8, num9, num6, num7, 14U, 15, 51U, array);
				ComparatorVisitor.Md5RoundI(ref num7, num8, num9, num6, 5U, 21, 52U, array);
				ComparatorVisitor.Md5RoundI(ref num6, num7, num8, num9, 12U, 6, 53U, array);
				ComparatorVisitor.Md5RoundI(ref num9, num6, num7, num8, 3U, 10, 54U, array);
				ComparatorVisitor.Md5RoundI(ref num8, num9, num6, num7, 10U, 15, 55U, array);
				ComparatorVisitor.Md5RoundI(ref num7, num8, num9, num6, 1U, 21, 56U, array);
				ComparatorVisitor.Md5RoundI(ref num6, num7, num8, num9, 8U, 6, 57U, array);
				ComparatorVisitor.Md5RoundI(ref num9, num6, num7, num8, 15U, 10, 58U, array);
				ComparatorVisitor.Md5RoundI(ref num8, num9, num6, num7, 6U, 15, 59U, array);
				ComparatorVisitor.Md5RoundI(ref num7, num8, num9, num6, 13U, 21, 60U, array);
				ComparatorVisitor.Md5RoundI(ref num6, num7, num8, num9, 4U, 6, 61U, array);
				ComparatorVisitor.Md5RoundI(ref num9, num6, num7, num8, 11U, 10, 62U, array);
				ComparatorVisitor.Md5RoundI(ref num8, num9, num6, num7, 2U, 15, 63U, array);
				ComparatorVisitor.Md5RoundI(ref num7, num8, num9, num6, 9U, 21, 64U, array);
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

		// Token: 0x060000D7 RID: 215 RVA: 0x00002320 File Offset: 0x00000520
		/// <summary>
		/// MD5 F 轮函数（原名 UpdateVisitor）。
		/// </summary>
		private static void Md5RoundF(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + ComparatorVisitor.RotateLeft(uint_0 + ((uint_1 & uint_2) | (~uint_1 & uint_3)) + object_0[(int)uint_4] + ComparatorVisitor.md5KConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00002349 File Offset: 0x00000549
		/// <summary>
		/// MD5 G 轮函数（原名 PostVisitor）。
		/// </summary>
		private static void Md5RoundG(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + ComparatorVisitor.RotateLeft(uint_0 + ((uint_1 & uint_3) | (uint_2 & ~uint_3)) + object_0[(int)uint_4] + ComparatorVisitor.md5KConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00002372 File Offset: 0x00000572
		/// <summary>
		/// MD5 H 轮函数（原名 PublishVisitor）。
		/// </summary>
		private static void Md5RoundH(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + ComparatorVisitor.RotateLeft(uint_0 + (uint_1 ^ uint_2 ^ uint_3) + object_0[(int)uint_4] + ComparatorVisitor.md5KConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00002398 File Offset: 0x00000598
		/// <summary>
		/// MD5 I 轮函数（原名 SortVisitor）。
		/// </summary>
		private static void Md5RoundI(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + ComparatorVisitor.RotateLeft(uint_0 + (uint_2 ^ (uint_1 | ~uint_3)) + object_0[(int)uint_4] + ComparatorVisitor.md5KConstants[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000023BF File Offset: 0x000005BF
		/// <summary>
		/// 循环左移（原名 PopVisitor）：x >> (32-s) | x << s。
		/// </summary>
		private static uint RotateLeft(uint uint_0, ushort ushort_0)
		{
			return uint_0 >> (int)(32 - ushort_0) | uint_0 << (int)ushort_0;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000023D1 File Offset: 0x000005D1
		/// <summary>
		/// 是否处于 FIPS 模式（原名 CallVisitor）：惰性检测，结果缓存到 isFipsModeCached。
		/// </summary>
		internal static bool IsFipsMode()
		{
			if (!ComparatorVisitor.fipsCheckedFlag)
			{
				ComparatorVisitor.DetectFipsPolicy();
				ComparatorVisitor.fipsCheckedFlag = true;
			}
			return ComparatorVisitor.isFipsModeCached;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000A3F8 File Offset: 0x000085F8
		/// <summary>
		/// 检测系统 FIPS 策略（原名 ManageVisitor）：反射读 CryptoConfig.AllowOnlyFipsAlgorithms；失败反射非公开 Utils.FipsAlgorithmPolicy==1。
		/// </summary>
		internal static void DetectFipsPolicy()
		{
			try
			{
				ComparatorVisitor.isFipsModeCached = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
				try
				{
					if ((int)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.Utils", false).GetMethod("get_FipsAlgorithmPolicy", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[0]) == 1)
					{
						ComparatorVisitor.isFipsModeCached = true;
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000A4A8 File Offset: 0x000086A8
		/// <summary>
		/// 获取对称算法实例（原名 FindVisitor）：FIPS → System.Core 3.5 的 AesCryptoServiceProvider（失败回退 RijndaelManaged）；非 FIPS → new RijndaelManaged()。
		/// </summary>
		internal static SymmetricAlgorithm GetSymmetricAlgorithm()
		{
			SymmetricAlgorithm result = null;
			if (ComparatorVisitor.IsFipsMode())
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

		// Token: 0x060000DF RID: 223 RVA: 0x000023EA File Offset: 0x000005EA
		/// <summary>
		/// MD5 哈希（原名 CloneVisitor）：非 FIPS → MD5CryptoServiceProvider.ComputeHash；FIPS → Md5FipsSelfImplemented。
		/// 使用方：GetterVisitor 哈希程序集名生成资源名；AesEncryptWithPassword 用口令 MD5 作 IV。
		/// </summary>
		internal static byte[] Md5Hash(byte[] byte_0)
		{
			if (!ComparatorVisitor.IsFipsMode())
			{
				return new MD5CryptoServiceProvider().ComputeHash(byte_0);
			}
			return ComparatorVisitor.Md5FipsSelfImplemented(byte_0);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000A500 File Offset: 0x00008700
		/// <summary>
		/// 解密字符串表（原名 AssetVisitor，懒加载）：首次解密嵌入资源 6693dd3c-5579-48c1-9ace-94458d4f769b 得到整块解密字节流；调用方传 int 偏移，读 4 字节长度 + Unicode 字符串返回。
		/// 流程：1. 读资源字节；2. 构造 32 字节密钥（初始字节经与程序集 PublicKeyToken 逐字节异或修正，源码里 array2[i]=X 重复赋值是反编译器伪影）与 16 字节 IV（混入 PublicKeyToken 奇偶位）；3. GetSymmetricAlgorithm() CBC 解密；4. 对解密数据做 32 位分组异或流变换（线性同余式 x^=x<<5; x+=3753412436; ...）还原字符串表；5. 之后每次调用按偏移取长度复制 UTF-16 字符串。
		/// 说明：这是被混淆代码中大量"明文常量"的真正来源。
		/// </summary>
		static string DecryptStringTable(int int_0)
		{
			if (ComparatorVisitor.decryptedStringTable.Length == 0)
			{
				BinaryReader binaryReader = new BinaryReader(typeof(ComparatorVisitor).Assembly.GetManifestResourceStream("6693dd3c-5579-48c1-9ace-94458d4f769b"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 132;
				array2[0] = 157;
				array2[0] = 112;
				array2[0] = 106;
				array2[0] = 98;
				array2[0] = 167;
				array2[1] = 131;
				array2[1] = 112;
				array2[1] = 168;
				array2[1] = 141;
				array2[1] = 171;
				array2[2] = 42;
				array2[2] = 138;
				array2[2] = 132;
				array2[3] = 84;
				array2[3] = 167;
				array2[3] = 182;
				array2[4] = 203;
				array2[4] = 131;
				array2[4] = 94;
				array2[5] = 139;
				array2[5] = 31;
				array2[5] = 248;
				array2[6] = 176;
				array2[6] = 100;
				array2[6] = 199;
				array2[7] = 86;
				array2[7] = 73;
				array2[7] = 192;
				array2[8] = 156;
				array2[8] = 91;
				array2[8] = 93;
				array2[8] = 230;
				array2[9] = 157;
				array2[9] = 167;
				array2[9] = 144;
				array2[9] = 52;
				array2[9] = 69;
				array2[10] = 140;
				array2[10] = 94;
				array2[10] = 152;
				array2[10] = 104;
				array2[10] = 159;
				array2[10] = 243;
				array2[11] = 17;
				array2[11] = 93;
				array2[11] = 90;
				array2[11] = 123;
				array2[11] = 164;
				array2[11] = 129;
				array2[12] = 95;
				array2[12] = 68;
				array2[12] = 86;
				array2[12] = 102;
				array2[12] = 156;
				array2[12] = 166;
				array2[13] = 149;
				array2[13] = 136;
				array2[13] = 108;
				array2[13] = 156;
				array2[13] = 107;
				array2[14] = 111;
				array2[14] = 76;
				array2[14] = 162;
				array2[14] = 124;
				array2[14] = 28;
				array2[15] = 167;
				array2[15] = 158;
				array2[15] = 112;
				array2[15] = 124;
				array2[15] = 186;
				array2[16] = 108;
				array2[16] = 46;
				array2[16] = 139;
				array2[16] = 84;
				array2[16] = 18;
				array2[17] = 120;
				array2[17] = 118;
				array2[17] = 174;
				array2[17] = 210;
				array2[18] = 94;
				array2[18] = 140;
				array2[18] = 118;
				array2[18] = 164;
				array2[19] = 114;
				array2[19] = 238;
				array2[19] = 183;
				array2[20] = 163;
				array2[20] = 118;
				array2[20] = 148;
				array2[20] = 154;
				array2[20] = 152;
				array2[20] = 240;
				array2[21] = 104;
				array2[21] = 93;
				array2[21] = 86;
				array2[21] = 35;
				array2[22] = 30;
				array2[22] = 112;
				array2[22] = 158;
				array2[22] = 128;
				array2[22] = 126;
				array2[22] = 185;
				array2[23] = 211;
				array2[23] = 143;
				array2[23] = 53;
				array2[24] = 155;
				array2[24] = 124;
				array2[24] = 10;
				array2[25] = 156;
				array2[25] = 127;
				array2[25] = 121;
				array2[26] = 167;
				array2[26] = 34;
				array2[26] = 122;
				array2[26] = 82;
				array2[27] = 223;
				array2[27] = 169;
				array2[27] = 135;
				array2[28] = 112;
				array2[28] = 130;
				array2[28] = 146;
				array2[28] = 99;
				array2[28] = 132;
				array2[28] = 205;
				array2[29] = 100;
				array2[29] = 162;
				array2[29] = 95;
				array2[29] = 103;
				array2[30] = 108;
				array2[30] = 81;
				array2[30] = 83;
				array2[30] = 118;
				array2[30] = 36;
				array2[30] = 94;
				array2[31] = 37;
				array2[31] = 62;
				array2[31] = 109;
				array2[31] = 26;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 71;
				array4[0] = 91;
				array4[0] = 96;
				array4[0] = 61;
				array4[0] = 159;
				array4[0] = 87;
				array4[1] = 133;
				array4[1] = 246;
				array4[1] = 109;
				array4[1] = 138;
				array4[1] = 105;
				array4[1] = 185;
				array4[2] = 129;
				array4[2] = 84;
				array4[2] = 167;
				array4[2] = 148;
				array4[2] = 124;
				array4[3] = 133;
				array4[3] = 94;
				array4[3] = 175;
				array4[3] = 31;
				array4[3] = 107;
				array4[3] = 241;
				array4[4] = 239;
				array4[4] = 120;
				array4[4] = 245;
				array4[5] = 128;
				array4[5] = 107;
				array4[5] = 201;
				array4[5] = 91;
				array4[5] = 9;
				array4[6] = 138;
				array4[6] = 192;
				array4[6] = 86;
				array4[6] = 87;
				array4[6] = 244;
				array4[7] = 111;
				array4[7] = 124;
				array4[7] = 245;
				array4[8] = 104;
				array4[8] = 134;
				array4[8] = 28;
				array4[8] = 66;
				array4[9] = 132;
				array4[9] = 115;
				array4[9] = 123;
				array4[9] = 75;
				array4[10] = 129;
				array4[10] = 103;
				array4[10] = 147;
				array4[10] = 104;
				array4[10] = 62;
				array4[10] = 46;
				array4[11] = 90;
				array4[11] = 103;
				array4[11] = 79;
				array4[11] = 42;
				array4[12] = 123;
				array4[12] = 156;
				array4[12] = 31;
				array4[13] = 112;
				array4[13] = 162;
				array4[13] = 242;
				array4[14] = 208;
				array4[14] = 158;
				array4[14] = 112;
				array4[14] = 127;
				array4[14] = 119;
				array4[14] = 119;
				array4[15] = 94;
				array4[15] = 110;
				array4[15] = 148;
				array4[15] = 84;
				byte[] array5 = array4;
				byte[] publicKeyToken = typeof(ComparatorVisitor).Assembly.GetName().GetPublicKeyToken();
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
					SymmetricAlgorithm symmetricAlgorithm = ComparatorVisitor.GetSymmetricAlgorithm();
					symmetricAlgorithm.Mode = CipherMode.CBC;
					ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor(array3, array5);
					MemoryStream memoryStream = new MemoryStream();
					CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
					cryptoStream.Write(array, 0, array.Length);
					cryptoStream.FlushFinalBlock();
					ComparatorVisitor.decryptedStringTable = memoryStream.ToArray();
					memoryStream.Close();
					cryptoStream.Close();
					binaryReader.Close();
					array = ComparatorVisitor.decryptedStringTable;
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
				ComparatorVisitor.decryptedStringTable = array6;
			}
			int num17 = BitConverter.ToInt32(ComparatorVisitor.decryptedStringTable, int_0);
			try
			{
				byte[] array7 = new byte[num17];
				Array.Copy(ComparatorVisitor.decryptedStringTable, int_0 + 4, array7, 0, num17);
				return Encoding.Unicode.GetString(array7, 0, array7.Length);
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000B3F8 File Offset: 0x000095F8
		/// <summary>
		/// Base64 → Unicode 字符串（原名 AddVisitor）：简单解码工具。
		/// </summary>
		internal static string Base64ToUnicodeString(string string_0)
		{
			byte[] array = Convert.FromBase64String(string_0);
			return Encoding.Unicode.GetString(array, 0, array.Length);
		}

		// Token: 0x060000E2 RID: 226
		[DllImport("kernel32.dll")]
		/// <summary>
		/// P/Invoke：kernel32.VirtualProtect —— 修改内存页保护属性（壳内存补丁用）。
		/// </summary>
		private static extern int VirtualProtect(IntPtr intptr_0, int int_0, int int_1, ref int int_2);

		// Token: 0x060000E3 RID: 227 RVA: 0x0000B41C File Offset: 0x0000961C
		[ComparatorVisitor.ProducerVisitor(typeof(ComparatorVisitor.ProducerVisitor.ComposerVisitor<object>[]))]
		/// <summary>
		/// 壳内存补丁（原名 ChangeVisitor，[ProducerVisitor] 标记，静态构造触发）：解密补丁资源 febbdcee-0330-4e15-8e08-e279bd6f0d1e，写入当前进程自己的模块映像，还原 Reactor 加壳时改写的 4 字节常量与方法体数据。
		/// 流程：1. patchAppliedFlag 防重入；2. 读资源→构造密钥/IV（PublicKeyToken 异或修正，IV 先 Array.Reverse）→32 位分组异或流还原补丁字节；3. OpenProcess(56,1,当前PID) 打开自己；4. moduleBaseAddress = Marshal.GetHINSTANCE(模块0)；5. 读 int32 条目数，逐条：偏移=基址+文件偏移→VirtualProtect(PAGE_READWRITE)→WriteProcessMemory 写 4 字节→还原保护；6. 流式条目同样处理；7. CloseHandle。
		/// IOC：嵌入资源名 febbdcee-0330-4e15-8e08-e279bd6f0d1e。
		/// </summary>
		static void ApplyShellMemoryPatch()
		{
			if (!ComparatorVisitor.patchAppliedFlag)
			{
				ComparatorVisitor.patchAppliedFlag = true;
				BinaryReader binaryReader = new BinaryReader(typeof(ComparatorVisitor).Assembly.GetManifestResourceStream("febbdcee-0330-4e15-8e08-e279bd6f0d1e"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 183;
				array2[0] = 98;
				array2[0] = 109;
				array2[0] = 180;
				array2[0] = 183;
				array2[1] = 120;
				array2[1] = 188;
				array2[1] = 231;
				array2[1] = 114;
				array2[1] = 184;
				array2[1] = 120;
				array2[2] = 85;
				array2[2] = 84;
				array2[2] = 158;
				array2[2] = 98;
				array2[2] = 113;
				array2[2] = 152;
				array2[2] = 85;
				array2[3] = 184;
				array2[3] = 105;
				array2[3] = 45;
				array2[3] = 184;
				array2[4] = 149;
				array2[4] = 115;
				array2[4] = 148;
				array2[4] = 61;
				array2[4] = 149;
				array2[5] = 197;
				array2[5] = 162;
				array2[5] = 151;
				array2[5] = 171;
				array2[5] = 144;
				array2[5] = 128;
				array2[5] = 197;
				array2[6] = 57;
				array2[6] = 123;
				array2[6] = 78;
				array2[6] = 57;
				array2[7] = 143;
				array2[7] = 112;
				array2[7] = 90;
				array2[7] = 112;
				array2[7] = 148;
				array2[7] = 143;
				array2[8] = 193;
				array2[8] = 245;
				array2[8] = 109;
				array2[8] = 149;
				array2[8] = 214;
				array2[8] = 193;
				array2[9] = 81;
				array2[9] = 155;
				array2[9] = 132;
				array2[9] = 112;
				array2[9] = 81;
				array2[10] = 80;
				array2[10] = 103;
				array2[10] = 109;
				array2[10] = 56;
				array2[10] = 89;
				array2[10] = 80;
				array2[11] = 140;
				array2[11] = 173;
				array2[11] = 162;
				array2[11] = 140;
				array2[12] = 180;
				array2[12] = 142;
				array2[12] = 128;
				array2[12] = 54;
				array2[12] = 129;
				array2[12] = 207;
				array2[12] = 180;
				array2[13] = 145;
				array2[13] = 113;
				array2[13] = 162;
				array2[13] = 93;
				array2[13] = 145;
				array2[14] = 49;
				array2[14] = 86;
				array2[14] = 157;
				array2[14] = 49;
				array2[15] = 14;
				array2[15] = 105;
				array2[15] = 101;
				array2[15] = 14;
				array2[16] = 197;
				array2[16] = 166;
				array2[16] = 110;
				array2[16] = 156;
				array2[16] = 187;
				array2[16] = 150;
				array2[16] = 197;
				array2[17] = 111;
				array2[17] = 124;
				array2[17] = 9;
				array2[17] = 111;
				array2[18] = 253;
				array2[18] = 86;
				array2[18] = 121;
				array2[18] = 94;
				array2[18] = 119;
				array2[18] = 253;
				array2[19] = 101;
				array2[19] = 118;
				array2[19] = 38;
				array2[19] = 94;
				array2[19] = 101;
				array2[20] = 240;
				array2[20] = 98;
				array2[20] = 90;
				array2[20] = 108;
				array2[20] = 240;
				array2[21] = 147;
				array2[21] = 223;
				array2[21] = 134;
				array2[21] = 123;
				array2[21] = 114;
				array2[21] = 132;
				array2[21] = 147;
				array2[22] = 44;
				array2[22] = 131;
				array2[22] = 108;
				array2[22] = 182;
				array2[22] = 44;
				array2[23] = 215;
				array2[23] = 130;
				array2[23] = 44;
				array2[23] = 190;
				array2[23] = 141;
				array2[23] = 95;
				array2[23] = 215;
				array2[24] = 22;
				array2[24] = 104;
				array2[24] = 129;
				array2[24] = 22;
				array2[25] = 182;
				array2[25] = 113;
				array2[25] = 149;
				array2[25] = 182;
				array2[26] = 39;
				array2[26] = 41;
				array2[26] = 64;
				array2[26] = 211;
				array2[26] = 39;
				array2[27] = 50;
				array2[27] = 146;
				array2[27] = 111;
				array2[27] = 122;
				array2[27] = 159;
				array2[27] = 79;
				array2[27] = 50;
				array2[28] = 161;
				array2[28] = 112;
				array2[28] = 80;
				array2[28] = 197;
				array2[28] = 142;
				array2[28] = 133;
				array2[28] = 161;
				array2[29] = 189;
				array2[29] = 79;
				array2[29] = 21;
				array2[29] = 189;
				array2[30] = 119;
				array2[30] = 152;
				array2[30] = 107;
				array2[30] = 98;
				array2[30] = 110;
				array2[30] = 119;
				array2[31] = 87;
				array2[31] = 135;
				array2[31] = 90;
				array2[31] = 93;
				array2[31] = 87;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 98;
				array4[0] = 109;
				array4[0] = 67;
				array4[1] = 150;
				array4[1] = 186;
				array4[1] = 81;
				array4[1] = 105;
				array4[2] = 92;
				array4[2] = 114;
				array4[2] = 127;
				array4[2] = 28;
				array4[2] = 191;
				array4[2] = 143;
				array4[3] = 113;
				array4[3] = 152;
				array4[3] = 92;
				array4[4] = 55;
				array4[4] = 136;
				array4[4] = 100;
				array4[4] = 166;
				array4[4] = 111;
				array4[4] = 21;
				array4[5] = 112;
				array4[5] = 60;
				array4[5] = 0;
				array4[6] = 168;
				array4[6] = 108;
				array4[6] = 90;
				array4[6] = 108;
				array4[7] = 146;
				array4[7] = 165;
				array4[7] = 86;
				array4[7] = 94;
				array4[8] = 78;
				array4[8] = 148;
				array4[8] = 104;
				array4[8] = 112;
				array4[8] = 74;
				array4[9] = 63;
				array4[9] = 120;
				array4[9] = 145;
				array4[10] = 175;
				array4[10] = 202;
				array4[10] = 110;
				array4[10] = 149;
				array4[11] = 140;
				array4[11] = 146;
				array4[11] = 46;
				array4[12] = 110;
				array4[12] = 98;
				array4[12] = 126;
				array4[12] = 104;
				array4[12] = 156;
				array4[13] = 102;
				array4[13] = 162;
				array4[13] = 33;
				array4[14] = 92;
				array4[14] = 179;
				array4[14] = 173;
				array4[14] = 85;
				array4[15] = 150;
				array4[15] = 205;
				array4[15] = 169;
				array4[15] = 87;
				byte[] array5 = array4;
				Array.Reverse(array5);
				byte[] publicKeyToken = typeof(ComparatorVisitor).Assembly.GetName().GetPublicKeyToken();
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
				Assembly assembly = typeof(ComparatorVisitor).Assembly;
				intptr_ = ComparatorVisitor.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				ComparatorVisitor.moduleBaseAddress = Marshal.GetHINSTANCE(assembly.GetModules()[0]).ToInt64();
				IntPtr zero = IntPtr.Zero;
				int int_ = 0;
				int num16 = binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				for (int m = 0; m < num16; m++)
				{
					IntPtr intPtr = new IntPtr(ComparatorVisitor.moduleBaseAddress + (long)binaryReader.ReadInt32());
					ComparatorVisitor.VirtualProtect(intPtr, 4, 4, ref int_);
					if (IntPtr.Size == 4)
					{
						ComparatorVisitor.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					else
					{
						ComparatorVisitor.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					ComparatorVisitor.VirtualProtect(intPtr, 4, int_, ref int_);
				}
				try
				{
					while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1L)
					{
						int num17 = binaryReader.ReadInt32();
						IntPtr intptr_2 = new IntPtr(ComparatorVisitor.moduleBaseAddress + (long)num17);
						int num18 = binaryReader.ReadInt32();
						ComparatorVisitor.VirtualProtect(intptr_2, num18 * 4, 4, ref int_);
						for (int n = 0; n < num18; n++)
						{
							Marshal.WriteInt32(new IntPtr(intptr_2.ToInt64() + (long)(n * 4)), binaryReader.ReadInt32());
						}
						ComparatorVisitor.VirtualProtect(intptr_2, num18 * 4, int_, ref int_);
					}
					ComparatorVisitor.CloseHandle(intptr_);
				}
				catch
				{
				}
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000C5C0 File Offset: 0x0000A7C0
		/// <summary>
		/// 获取程序集路径（原名 CountVisitor）：依次尝试 Assembly.Location、CodeBase（去 file:/// 前缀）、反射 Location 属性；都失败返回空串。
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

		// Token: 0x060000E5 RID: 229
		[DllImport("kernel32.dll")]
		/// <summary>
		/// P/Invoke：kernel32.WriteProcessMemory —— 写进程内存（壳内存补丁用）。
		/// </summary>
		private static extern int WriteProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x060000E6 RID: 230
		[DllImport("kernel32.dll")]
		/// <summary>
		/// P/Invoke：kernel32.ReadProcessMemory —— 读进程内存（声明未调用，残留）。
		/// </summary>
		private static extern int ReadProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x060000E7 RID: 231
		[DllImport("kernel32.dll")]
		/// <summary>
		/// P/Invoke：kernel32.OpenProcess —— 打开进程句柄（壳内存补丁用，56=PROCESS_ALL_ACCESS）。
		/// </summary>
		private static extern IntPtr OpenProcess(uint uint_0, int int_0, uint uint_1);

		// Token: 0x060000E8 RID: 232
		[DllImport("kernel32.dll")]
		/// <summary>
		/// P/Invoke：kernel32.CloseHandle —— 关闭句柄。
		/// </summary>
		private static extern int CloseHandle(IntPtr intptr_0);

		// Token: 0x060000E9 RID: 233 RVA: 0x0000C6D0 File Offset: 0x0000A8D0
		/// <summary>
		/// 读取文件全部字节（原名 RateVisitor）：FileStream(Open, Read, FileShare.Read) 循环读满。
		/// </summary>
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

		// Token: 0x060000EA RID: 234 RVA: 0x0000C730 File Offset: 0x0000A930
		/// <summary>
		/// AES 解密（原名 ViewVisitor）：固定 32 字节 Key（14 EF 46 3A ...）与固定 16 字节 IV（9B BD 27 94 ...），GetSymmetricAlgorithm().CreateDecryptor 解密。用途推测：解密壳内嵌数据/网络载荷（本文件内未见调用者）。
		/// </summary>
		private static byte[] AesDecrypt(byte[] byte_0)
		{
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = ComparatorVisitor.GetSymmetricAlgorithm();
			symmetricAlgorithm.Key = new byte[]
			{
				20,
				239,
				70,
				58,
				112,
				44,
				59,
				144,
				67,
				41,
				8,
				35,
				120,
				166,
				217,
				39,
				173,
				246,
				124,
				112,
				134,
				223,
				195,
				105,
				222,
				143,
				78,
				75,
				211,
				0,
				88,
				226
			};
			symmetricAlgorithm.IV = new byte[]
			{
				155,
				189,
				39,
				148,
				240,
				95,
				62,
				72,
				23,
				109,
				178,
				41,
				235,
				195,
				61,
				176
			};
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(byte_0, 0, byte_0.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000C79C File Offset: 0x0000A99C
		/// <summary>
		/// 空桩（原名 DestroyVisitor）：返回 null（壳/混淆残留）。
		/// </summary>
		private byte[] NullStub1()
		{
			return null;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000C79C File Offset: 0x0000A99C
		/// <summary>
		/// 空桩（原名 PatchVisitor）：返回 null。
		/// </summary>
		private byte[] NullStub2()
		{
			return null;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000C79C File Offset: 0x0000A99C
		/// <summary>
		/// 空桩（原名 ExcludeVisitor）：返回 null。
		/// </summary>
		private byte[] NullStub3()
		{
			return null;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000C79C File Offset: 0x0000A99C
		/// <summary>
		/// 空桩（原名 SetupVisitor）：返回 null。
		/// </summary>
		private byte[] NullStub4()
		{
			return null;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000C7AC File Offset: 0x0000A9AC
		/// <summary>
		/// 空桩（原名 InstantiateVisitor）：返回 null。
		/// </summary>
		private byte[] NullStub5()
		{
			return null;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000C7AC File Offset: 0x0000A9AC
		/// <summary>
		/// 空桩（原名 RestartVisitor）：返回 null。
		/// </summary>
		private byte[] NullStub6()
		{
			return null;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00002405 File Offset: 0x00000605
		/// <summary>
		/// 授权桩（原名 PushVisitor）：取假 GUID "{11111-22222-40001-00001}" 长度后丢弃，返回 {1,2}。
		/// </summary>
		internal byte[] LicenseStub1()
		{
			int length = "{11111-22222-40001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00002420 File Offset: 0x00000620
		/// <summary>
		/// 授权桩（原名 VerifyVisitor）：GUID "{11111-22222-40001-00002}"，返回 {1,2}。
		/// </summary>
		internal byte[] LicenseStub2()
		{
			int length = "{11111-22222-40001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000243B File Offset: 0x0000063B
		/// <summary>
		/// 授权桩（原名 ValidateVisitor）：GUID "{11111-22222-50001-00001}"，返回 {1,2}。
		/// </summary>
		internal byte[] LicenseStub3()
		{
			int length = "{11111-22222-50001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00002456 File Offset: 0x00000656
		/// <summary>
		/// 授权桩（原名 EnableVisitor）：GUID "{11111-22222-50001-00002}"，返回 {1,2}。
		/// </summary>
		internal byte[] LicenseStub4()
		{
			int length = "{11111-22222-50001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00002471 File Offset: 0x00000671
		/// <summary>
		/// 授权桩（原名 CheckVisitor）：GUID "{11111-22222-60001-00001}"，返回 {1,2}。
		/// </summary>
		internal byte[] LicenseStub5()
		{
			int length = "{11111-22222-60001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000248C File Offset: 0x0000068C
		/// <summary>
		/// 授权桩（原名 CalculateVisitor）：GUID "{11111-22222-60001-00002}"，返回 {1,2}。
		/// </summary>
		internal byte[] LicenseStub6()
		{
			int length = "{11111-22222-60001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000C7BC File Offset: 0x0000A9BC
		/// <summary>
		/// AES 加密（原名 DefineVisitor(明文, 口令)）：Encoding.Unicode.GetBytes(明文)；固定 32 字节 Key（52 66 68 6E ...）；IV = Md5Hash(UnicodeBytes(口令))（口令的 MD5）；CreateEncryptor → 输出 Base64。用途推测：口令派生加密（网络通信/配置加密工具）。
		/// </summary>
		internal static string AesEncryptWithPassword(string string_0, string string_1)
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
			byte[] iv = ComparatorVisitor.Md5Hash(Encoding.Unicode.GetBytes(string_1));
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = ComparatorVisitor.GetSymmetricAlgorithm();
			symmetricAlgorithm.Key = key;
			symmetricAlgorithm.IV = iv;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.Close();
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>
		/// 壳桩：恒返回 true（授权检查路径，无实际逻辑）。
		/// </summary>
		internal static bool InitMethod()
		{
			return true;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>
		/// 壳桩：恒返回 false。
		/// </summary>
		internal static bool CompareMethod()
		{
			return false;
		}

		// Token: 0x040000B2 RID: 178
		/// <summary>
		/// 字节数组伪状态（壳/混淆器保留，未使用）。
		/// </summary>
		private static byte[] m_CallbackVisitor;

		// Token: 0x040000B3 RID: 179
		/// <summary>
		/// 字节数组伪状态（未使用）。
		/// </summary>
		private static byte[] singletonVisitor;

		// Token: 0x040000B4 RID: 180
		/// <summary>
		/// 字节数组伪状态（未使用）。
		/// </summary>
		private static byte[] _ServerVisitor;

		// Token: 0x040000B5 RID: 181
		/// <summary>
		/// IntPtr 伪状态（未使用）。
		/// </summary>
		private static IntPtr serviceVisitor;

		// Token: 0x040000B6 RID: 182
		/// <summary>
		/// 当前进程模块基址（原名 _QueueVisitor）：Marshal.GetHINSTANCE(模块0)，壳内存补丁的偏移基准。
		/// </summary>
		private static long moduleBaseAddress;

		// Token: 0x040000B7 RID: 183
		/// <summary>
		/// 整型伪状态（未使用）。
		/// </summary>
		private static int m_RecordVisitor;

		// Token: 0x040000B8 RID: 184
		/// <summary>
		/// 内存补丁已应用标志（原名 m_PublisherVisitor）：防重入。
		/// </summary>
		private static bool patchAppliedFlag;

		// Token: 0x040000B9 RID: 185
		/// <summary>
		/// IntPtr 伪状态（未使用）。
		/// </summary>
		private static IntPtr stubVisitor;

		// Token: 0x040000BA RID: 186
		/// <summary>
		/// FIPS 是否已检测标志（原名 _AdapterVisitor）。
		/// </summary>
		private static bool fipsCheckedFlag;

		// Token: 0x040000BB RID: 187
		/// <summary>
		/// string[] 伪状态（未使用）。
		/// </summary>
		private static string[] m_ThreadVisitor;

		// Token: 0x040000BC RID: 188
		/// <summary>
		/// FIPS 模式检测结果缓存（原名 m_MockVisitor）。
		/// </summary>
		private static bool isFipsModeCached;

		// Token: 0x040000BD RID: 189
		/// <summary>
		/// MD5 K 常量表（原名 factoryVisitor）：64 个 uint 标准 MD5 正弦常量（0xD76AA478 开头，0xA4BEEA44 结尾）。
		/// </summary>
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

		// Token: 0x040000BE RID: 190
		/// <summary>
		/// int[] 伪状态（未使用）。
		/// </summary>
		private static int[] m_InfoVisitor;

		// Token: 0x040000BF RID: 191
		/// <summary>
		/// 整型伪状态（初始化=1，未使用）。
		/// </summary>
		private static int m_CollectionVisitor;

		// Token: 0x040000C0 RID: 192
		/// <summary>
		/// 解密后的字符串表（原名 _StructVisitor）：DecryptStringTable 懒加载缓存，按偏移读取长度+UTF-16 字符串。
		/// </summary>
		private static byte[] decryptedStringTable;

		// Token: 0x02000016 RID: 22
		internal class ProducerVisitor : Attribute
		{
			// Token: 0x060000FB RID: 251 RVA: 0x000024A7 File Offset: 0x000006A7
			[ComparatorVisitor.ProducerVisitor(typeof(ComparatorVisitor.ProducerVisitor.ComposerVisitor<object>[]))]
		/// <summary>
		/// 壳标记属性构造器（空实现）。
		/// </summary>
			public ProducerVisitor(object object_0)
			{
			}

			// Token: 0x02000017 RID: 23
			internal class ComposerVisitor<T>
			{
				// Token: 0x060000FD RID: 253 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>
		/// 壳桩：恒 true。
		/// </summary>
				internal static bool AwakeMethod()
				{
					return true;
				}

				// Token: 0x060000FE RID: 254 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>
		/// 壳桩：恒 false。
		/// </summary>
				internal static bool RunMethod()
				{
					return false;
				}
			}
		}

		// Token: 0x02000018 RID: 24
		[Flags]
		/// <summary>
		/// 空枚举（混淆残留，无成员）。
		/// </summary>
		private enum LineBreakBoundaryConditions
		{

		}
	}
}
