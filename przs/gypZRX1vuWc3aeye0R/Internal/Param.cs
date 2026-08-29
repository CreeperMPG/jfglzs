using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// ============================================================================
// Param.cs — 壳/混淆器核心：字符串解密 + 反篡改内存补丁 + 加解密原语
// 真实职责：反混淆后为 ConfuserEx（含 .NET Reactor 混合）加壳/反篡改运行时模块。承担四类工作：
// 1. MD5 自实现（自实现MD5 + MD5轮函数F/G/H/I + 循环左移 + MD5常数表）——FIPS 模式下替代系统 MD5；
// 2. 嵌入资源解密（取解密字符串常量）：AES-CBC（密钥=程序集公钥标记派生）解密嵌入资源
//    f2c33f7a-…（字符串常量池），再经自研 32 位流式 XOR/累加算法（常数 3753412436/678874538/
//    179542347/522623986，ConfuserEx 常量加密特征）解出密文字符串；
// 3. 反篡改（反篡改内存补丁）：解密嵌入资源 d08e40f8-…，用 VirtualProtect + WriteProcessMemory/
//    Marshal.WriteInt32 把解密后的节数据写回自身映像（anti-tamper 运行时自修改）；
// 4. AES 加解密原语：AES加密转Base64（固定密钥加密）、AES解密（固定密钥/IV 解密）。
// 若干 *Schema() 桩方法仅返回 {1,2} 并引用 {11111-…} 伪 GUID 字符串——混淆器插入的诱饵。
// 说明：类名保持混淆原名，方法/字段已按 Param.cs.md 映射表反混淆重命名。
// ============================================================================
namespace gypZRX1vuWc3aeye0R.Internal
{
	// Token: 0x02000015 RID: 21
	internal class Param
	{
		// Token: 0x060000AC RID: 172 RVA: 0x00007544 File Offset: 0x00005744
/// <summary>
/// 静态构造：初始化全部静态字段（fips 标志、字节数组、句柄、计数器与反篡改已执行标志）。
/// </summary>
		static Param()
		{
			Param.fips检测完成标志 = false;
			Param.fips模式标志 = false;
			Param._Strategy = new byte[0];
			Param.m_Observer = new byte[0];
			Param.解密字符串池 = new byte[0];
			Param._Field = new byte[0];
			Param.mock = IntPtr.Zero;
			Param.task = IntPtr.Zero;
			Param._Comparator = new string[0];
			Param.role = new int[0];
			Param.merchant = 1;
			Param.模块基址 = 0L;
			Param.m_Connection = 0;
			Param.反篡改已执行标志 = false;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00002321 File Offset: 0x00000521
/// <summary>空方法桩（混淆器填充，无逻辑）。</summary>
		private void 空方法桩()
		{
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000075EC File Offset: 0x000057EC
/// <summary>
/// 自实现 MD5：1. 标准 MD5 填充（补 0x80、64 位长度）；2. 初始化 A=0x67452301、B=0xEFCDAB89、
/// C=0x98BADCFE、D=0x10325476；3. 每 64 字节块执行 4 轮×16 步（MD5轮函数F/G/H/I + MD5常数表）；
/// 4. 输出 16 字节摘要。FIPS 合规环境下替代系统 MD5（MD5摘要 二选一）。
/// </summary>
		internal static byte[] 自实现MD5(object object_0)
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
				Param.MD5轮函数F(ref num6, num7, num8, num9, 0U, 7, 1U, array);
				Param.MD5轮函数F(ref num9, num6, num7, num8, 1U, 12, 2U, array);
				Param.MD5轮函数F(ref num8, num9, num6, num7, 2U, 17, 3U, array);
				Param.MD5轮函数F(ref num7, num8, num9, num6, 3U, 22, 4U, array);
				Param.MD5轮函数F(ref num6, num7, num8, num9, 4U, 7, 5U, array);
				Param.MD5轮函数F(ref num9, num6, num7, num8, 5U, 12, 6U, array);
				Param.MD5轮函数F(ref num8, num9, num6, num7, 6U, 17, 7U, array);
				Param.MD5轮函数F(ref num7, num8, num9, num6, 7U, 22, 8U, array);
				Param.MD5轮函数F(ref num6, num7, num8, num9, 8U, 7, 9U, array);
				Param.MD5轮函数F(ref num9, num6, num7, num8, 9U, 12, 10U, array);
				Param.MD5轮函数F(ref num8, num9, num6, num7, 10U, 17, 11U, array);
				Param.MD5轮函数F(ref num7, num8, num9, num6, 11U, 22, 12U, array);
				Param.MD5轮函数F(ref num6, num7, num8, num9, 12U, 7, 13U, array);
				Param.MD5轮函数F(ref num9, num6, num7, num8, 13U, 12, 14U, array);
				Param.MD5轮函数F(ref num8, num9, num6, num7, 14U, 17, 15U, array);
				Param.MD5轮函数F(ref num7, num8, num9, num6, 15U, 22, 16U, array);
				Param.MD5轮函数G(ref num6, num7, num8, num9, 1U, 5, 17U, array);
				Param.MD5轮函数G(ref num9, num6, num7, num8, 6U, 9, 18U, array);
				Param.MD5轮函数G(ref num8, num9, num6, num7, 11U, 14, 19U, array);
				Param.MD5轮函数G(ref num7, num8, num9, num6, 0U, 20, 20U, array);
				Param.MD5轮函数G(ref num6, num7, num8, num9, 5U, 5, 21U, array);
				Param.MD5轮函数G(ref num9, num6, num7, num8, 10U, 9, 22U, array);
				Param.MD5轮函数G(ref num8, num9, num6, num7, 15U, 14, 23U, array);
				Param.MD5轮函数G(ref num7, num8, num9, num6, 4U, 20, 24U, array);
				Param.MD5轮函数G(ref num6, num7, num8, num9, 9U, 5, 25U, array);
				Param.MD5轮函数G(ref num9, num6, num7, num8, 14U, 9, 26U, array);
				Param.MD5轮函数G(ref num8, num9, num6, num7, 3U, 14, 27U, array);
				Param.MD5轮函数G(ref num7, num8, num9, num6, 8U, 20, 28U, array);
				Param.MD5轮函数G(ref num6, num7, num8, num9, 13U, 5, 29U, array);
				Param.MD5轮函数G(ref num9, num6, num7, num8, 2U, 9, 30U, array);
				Param.MD5轮函数G(ref num8, num9, num6, num7, 7U, 14, 31U, array);
				Param.MD5轮函数G(ref num7, num8, num9, num6, 12U, 20, 32U, array);
				Param.MD5轮函数H(ref num6, num7, num8, num9, 5U, 4, 33U, array);
				Param.MD5轮函数H(ref num9, num6, num7, num8, 8U, 11, 34U, array);
				Param.MD5轮函数H(ref num8, num9, num6, num7, 11U, 16, 35U, array);
				Param.MD5轮函数H(ref num7, num8, num9, num6, 14U, 23, 36U, array);
				Param.MD5轮函数H(ref num6, num7, num8, num9, 1U, 4, 37U, array);
				Param.MD5轮函数H(ref num9, num6, num7, num8, 4U, 11, 38U, array);
				Param.MD5轮函数H(ref num8, num9, num6, num7, 7U, 16, 39U, array);
				Param.MD5轮函数H(ref num7, num8, num9, num6, 10U, 23, 40U, array);
				Param.MD5轮函数H(ref num6, num7, num8, num9, 13U, 4, 41U, array);
				Param.MD5轮函数H(ref num9, num6, num7, num8, 0U, 11, 42U, array);
				Param.MD5轮函数H(ref num8, num9, num6, num7, 3U, 16, 43U, array);
				Param.MD5轮函数H(ref num7, num8, num9, num6, 6U, 23, 44U, array);
				Param.MD5轮函数H(ref num6, num7, num8, num9, 9U, 4, 45U, array);
				Param.MD5轮函数H(ref num9, num6, num7, num8, 12U, 11, 46U, array);
				Param.MD5轮函数H(ref num8, num9, num6, num7, 15U, 16, 47U, array);
				Param.MD5轮函数H(ref num7, num8, num9, num6, 2U, 23, 48U, array);
				Param.MD5轮函数I(ref num6, num7, num8, num9, 0U, 6, 49U, array);
				Param.MD5轮函数I(ref num9, num6, num7, num8, 7U, 10, 50U, array);
				Param.MD5轮函数I(ref num8, num9, num6, num7, 14U, 15, 51U, array);
				Param.MD5轮函数I(ref num7, num8, num9, num6, 5U, 21, 52U, array);
				Param.MD5轮函数I(ref num6, num7, num8, num9, 12U, 6, 53U, array);
				Param.MD5轮函数I(ref num9, num6, num7, num8, 3U, 10, 54U, array);
				Param.MD5轮函数I(ref num8, num9, num6, num7, 10U, 15, 55U, array);
				Param.MD5轮函数I(ref num7, num8, num9, num6, 1U, 21, 56U, array);
				Param.MD5轮函数I(ref num6, num7, num8, num9, 8U, 6, 57U, array);
				Param.MD5轮函数I(ref num9, num6, num7, num8, 15U, 10, 58U, array);
				Param.MD5轮函数I(ref num8, num9, num6, num7, 6U, 15, 59U, array);
				Param.MD5轮函数I(ref num7, num8, num9, num6, 13U, 21, 60U, array);
				Param.MD5轮函数I(ref num6, num7, num8, num9, 4U, 6, 61U, array);
				Param.MD5轮函数I(ref num9, num6, num7, num8, 11U, 10, 62U, array);
				Param.MD5轮函数I(ref num8, num9, num6, num7, 2U, 15, 63U, array);
				Param.MD5轮函数I(ref num7, num8, num9, num6, 9U, 21, 64U, array);
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

		// Token: 0x060000AF RID: 175 RVA: 0x00002323 File Offset: 0x00000523
/// <summary>MD5 第一轮 F 函数：(B AND C) OR (NOT B AND D)。</summary>
		private static void MD5轮函数F(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + Param.循环左移(uint_0 + ((uint_1 & uint_2) | (~uint_1 & uint_3)) + object_0[(int)uint_4] + Param.MD5常数表[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000234C File Offset: 0x0000054C
/// <summary>MD5 第二轮 G 函数：(B AND D) OR (C AND NOT D)。</summary>
		private static void MD5轮函数G(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + Param.循环左移(uint_0 + ((uint_1 & uint_3) | (uint_2 & ~uint_3)) + object_0[(int)uint_4] + Param.MD5常数表[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00002375 File Offset: 0x00000575
/// <summary>MD5 第三轮 H 函数：B XOR C XOR D。</summary>
		private static void MD5轮函数H(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + Param.循环左移(uint_0 + (uint_1 ^ uint_2 ^ uint_3) + object_0[(int)uint_4] + Param.MD5常数表[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000239B File Offset: 0x0000059B
/// <summary>MD5 第四轮 I 函数：C XOR (B OR NOT D)。</summary>
		private static void MD5轮函数I(ref uint uint_0, uint uint_1, uint uint_2, uint uint_3, uint uint_4, ushort ushort_0, uint uint_5, object object_0)
		{
			uint_0 = uint_1 + Param.循环左移(uint_0 + (uint_2 ^ (uint_1 | ~uint_3)) + object_0[(int)uint_4] + Param.MD5常数表[(int)(uint_5 - 1U)], ushort_0);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000023C2 File Offset: 0x000005C2
/// <summary>循环左移：ROL(uint, n) = (x >> (32-n)) | (x << n)。</summary>
		private static uint 循环左移(uint uint_0, ushort ushort_0)
		{
			return uint_0 >> (int)(32 - ushort_0) | uint_0 << (int)ushort_0;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000023D4 File Offset: 0x000005D4
/// <summary>是否 FIPS 模式：检测标志未初始化时先检测FIPS策略，缓存结果后返回 fips模式标志。</summary>
		internal static bool 是否FIPS模式()
		{
			if (!Param.fips检测完成标志)
			{
				Param.检测FIPS策略();
				Param.fips检测完成标志 = true;
			}
			return Param.fips模式标志;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00007C4C File Offset: 0x00005E4C
/// <summary>
/// 检测 FIPS 策略：反射读 CryptoConfig.AllowOnlyFipsAlgorithms；失败则反射读
/// Utils.FipsAlgorithmPolicy（==1 则置 fips模式标志），均失败忽略。
/// </summary>
		internal static void 检测FIPS策略()
		{
			try
			{
				Param.fips模式标志 = (bool)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.CryptoConfig", false).GetMethod("get_AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
			}
			catch
			{
				try
				{
					if ((int)typeof(RijndaelManaged).Assembly.GetType("System.Security.Cryptography.Utils", false).GetMethod("get_FipsAlgorithmPolicy", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[0]) == 1)
					{
						Param.fips模式标志 = true;
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00007CFC File Offset: 0x00005EFC
/// <summary>
/// 创建对称算法：FIPS 下尝试加载 System.Core 3.5 的 AesCryptoServiceProvider（失败退回 RijndaelManaged），
/// 非 FIPS 用 RijndaelManaged。
/// </summary>
		internal static SymmetricAlgorithm 创建对称算法()
		{
			SymmetricAlgorithm result = null;
			if (Param.是否FIPS模式())
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

		// Token: 0x060000B7 RID: 183 RVA: 0x000023ED File Offset: 0x000005ED
/// <summary>MD5 摘要：非 FIPS 用 MD5CryptoServiceProvider，FIPS 用自实现MD5。</summary>
		internal static byte[] MD5摘要(byte[] byte_0)
		{
			if (!Param.是否FIPS模式())
			{
				return new MD5CryptoServiceProvider().ComputeHash(byte_0);
			}
			return Param.自实现MD5(byte_0);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00007D54 File Offset: 0x00005F54
/// <summary>
/// 取解密字符串常量：1. 首次执行读嵌入资源 f2c33f7a-a406-48ab-af63-034b0bca5201 全部字节；
/// 2. 构造 32 字节密钥（硬编码数组与程序集 PublicKeyToken 异或，偶数下标写入 token 字节），16 字节 IV 同理；
/// 3. offset==-1 时用 AES-CBC 解密整段资源存入 解密字符串池；4. 再做自研 32 位流式解密
/// （num4 += keyDword；x ^= x<<5; x+=3753412436; x^=x<<6; x+=678874538; x^=x>>13; x+=179542347; x=522623986-x），
/// 得到明文；5. 按偏移读 int32 长度 + Unicode 文本返回（混淆字符串常量池）。
/// </summary>
		static string 取解密字符串常量(int int_0)
		{
			if (Param.解密字符串池.Length == 0)
			{
				BinaryReader binaryReader = new BinaryReader(typeof(Param).Assembly.GetManifestResourceStream("f2c33f7a-a406-48ab-af63-034b0bca5201"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 130;
				array2[0] = 95;
				array2[0] = 57;
				array2[1] = 146;
				array2[1] = 137;
				array2[1] = 159;
				array2[1] = 132;
				array2[2] = 102;
				array2[2] = 131;
				array2[2] = 168;
				array2[3] = 74;
				array2[3] = 80;
				array2[3] = 100;
				array2[3] = 59;
				array2[4] = 48;
				array2[4] = 109;
				array2[4] = 128;
				array2[4] = 44;
				array2[4] = 5;
				array2[5] = 169;
				array2[5] = 148;
				array2[5] = 133;
				array2[6] = 128;
				array2[6] = 170;
				array2[6] = 37;
				array2[7] = 142;
				array2[7] = 85;
				array2[7] = 84;
				array2[8] = 121;
				array2[8] = 132;
				array2[8] = 166;
				array2[8] = 13;
				array2[9] = 72;
				array2[9] = 94;
				array2[9] = 133;
				array2[9] = 97;
				array2[9] = 63;
				array2[10] = 124;
				array2[10] = 127;
				array2[10] = 94;
				array2[11] = 161;
				array2[11] = 43;
				array2[11] = 247;
				array2[12] = 165;
				array2[12] = 92;
				array2[12] = 134;
				array2[12] = 214;
				array2[13] = 126;
				array2[13] = 176;
				array2[13] = 124;
				array2[14] = 86;
				array2[14] = 179;
				array2[14] = 129;
				array2[14] = 67;
				array2[15] = 69;
				array2[15] = 111;
				array2[15] = 89;
				array2[15] = 74;
				array2[16] = 154;
				array2[16] = 86;
				array2[16] = 106;
				array2[16] = 224;
				array2[16] = 91;
				array2[16] = 7;
				array2[17] = 115;
				array2[17] = 72;
				array2[17] = 89;
				array2[17] = 108;
				array2[17] = 216;
				array2[18] = 158;
				array2[18] = 177;
				array2[18] = 252;
				array2[19] = 176;
				array2[19] = 117;
				array2[19] = 122;
				array2[19] = 92;
				array2[19] = 249;
				array2[20] = 83;
				array2[20] = 144;
				array2[20] = 148;
				array2[20] = 184;
				array2[21] = 169;
				array2[21] = 140;
				array2[21] = 38;
				array2[21] = 142;
				array2[22] = 110;
				array2[22] = 158;
				array2[22] = 86;
				array2[23] = 114;
				array2[23] = 151;
				array2[23] = 147;
				array2[23] = 203;
				array2[24] = 142;
				array2[24] = 168;
				array2[24] = 102;
				array2[24] = 63;
				array2[24] = 97;
				array2[24] = 203;
				array2[25] = 181;
				array2[25] = 106;
				array2[25] = 26;
				array2[25] = 97;
				array2[26] = 235;
				array2[26] = 140;
				array2[26] = 72;
				array2[26] = 25;
				array2[27] = 89;
				array2[27] = 108;
				array2[27] = 125;
				array2[27] = 90;
				array2[27] = 152;
				array2[27] = 239;
				array2[28] = 73;
				array2[28] = 53;
				array2[28] = 39;
				array2[29] = 141;
				array2[29] = 164;
				array2[29] = 112;
				array2[29] = 164;
				array2[29] = 166;
				array2[29] = 218;
				array2[30] = 126;
				array2[30] = 204;
				array2[30] = 120;
				array2[30] = 143;
				array2[31] = 125;
				array2[31] = 120;
				array2[31] = 188;
				array2[31] = 172;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 130;
				array4[0] = 83;
				array4[0] = 146;
				array4[0] = 137;
				array4[0] = 41;
				array4[1] = 144;
				array4[1] = 61;
				array4[1] = 71;
				array4[1] = 74;
				array4[1] = 150;
				array4[2] = 93;
				array4[2] = 140;
				array4[2] = 92;
				array4[2] = 150;
				array4[2] = 116;
				array4[2] = 75;
				array4[3] = 101;
				array4[3] = 32;
				array4[3] = 126;
				array4[3] = 200;
				array4[4] = 174;
				array4[4] = 128;
				array4[4] = 120;
				array4[4] = 192;
				array4[5] = 89;
				array4[5] = 110;
				array4[5] = 166;
				array4[5] = 168;
				array4[6] = 73;
				array4[6] = 135;
				array4[6] = 115;
				array4[6] = 141;
				array4[6] = 148;
				array4[6] = 0;
				array4[7] = 133;
				array4[7] = 97;
				array4[7] = 154;
				array4[7] = 23;
				array4[8] = 156;
				array4[8] = 106;
				array4[8] = 171;
				array4[8] = 51;
				array4[8] = 186;
				array4[8] = 66;
				array4[9] = 146;
				array4[9] = 222;
				array4[9] = 144;
				array4[9] = 176;
				array4[9] = 189;
				array4[10] = 170;
				array4[10] = 151;
				array4[10] = 129;
				array4[10] = 164;
				array4[10] = 196;
				array4[11] = 124;
				array4[11] = 153;
				array4[11] = 166;
				array4[11] = 159;
				array4[12] = 84;
				array4[12] = 86;
				array4[12] = 74;
				array4[12] = 156;
				array4[12] = 181;
				array4[13] = 148;
				array4[13] = 118;
				array4[13] = 178;
				array4[13] = 171;
				array4[13] = 205;
				array4[14] = 163;
				array4[14] = 100;
				array4[14] = 141;
				array4[15] = 75;
				array4[15] = 108;
				array4[15] = 247;
				byte[] array5 = array4;
				byte[] publicKeyToken = typeof(Param).Assembly.GetName().GetPublicKeyToken();
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
					SymmetricAlgorithm symmetricAlgorithm = Param.创建对称算法();
					symmetricAlgorithm.Mode = CipherMode.CBC;
					ICryptoTransform transform = symmetricAlgorithm.CreateDecryptor(array3, array5);
					MemoryStream memoryStream = new MemoryStream();
					CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
					cryptoStream.Write(array, 0, array.Length);
					cryptoStream.FlushFinalBlock();
					Param.解密字符串池 = memoryStream.ToArray();
					memoryStream.Close();
					cryptoStream.Close();
					binaryReader.Close();
					array = Param.解密字符串池;
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
				Param.解密字符串池 = array6;
			}
			int num17 = BitConverter.ToInt32(Param.解密字符串池, int_0);
			try
			{
				byte[] array7 = new byte[num17];
				Array.Copy(Param.解密字符串池, int_0 + 4, array7, 0, num17);
				return Encoding.Unicode.GetString(array7, 0, array7.Length);
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00008BF4 File Offset: 0x00006DF4
/// <summary>Base64→Unicode：Convert.FromBase64String + Encoding.Unicode.GetString。</summary>
		internal static string Base64转Unicode(string string_0)
		{
			byte[] array = Convert.FromBase64String(string_0);
			return Encoding.Unicode.GetString(array, 0, array.Length);
		}

		// Token: 0x060000BA RID: 186
		[DllImport("kernel32.dll")]
/// <summary>Win32 kernel32!VirtualProtect：修改内存页保护（反篡改写回用）。</summary>
		private static extern int VirtualProtect(IntPtr intptr_0, int int_0, int int_1, ref int int_2);

		// Token: 0x060000BB RID: 187 RVA: 0x00008C18 File Offset: 0x00006E18
		[Param.Service(typeof(Param.Service.Worker<object>[]))]
/// <summary>
/// 反篡改内存补丁（anti-tamper，一次性）：1. 反篡改已执行标志 防重入；2. 读嵌入资源
/// d08e40f8-0b4f-412d-8089-995ba478b812；3. 构造 32 字节密钥并与 PublicKeyToken 异或（IV 先 Array.Reverse），
/// 自研流解密出补丁数据；4. OpenProcess(PROCESS_ALL_ACCESS=56, 1, 当前PID)；5. 取自身模块基址
/// 模块基址 = Marshal.GetHINSTANCE(modules[0])；6. 读 count 循环：对 基址+偏移 处 VirtualProtect(
/// PAGE_EXECUTE_READWRITE=4) → WriteProcessMemory 写 4 字节 → 恢复保护；7. 再处理"长度+数据块"
/// （VirtualProtect → Marshal.WriteInt32 逐字写回）；8. CloseHandle。效果：运行时把被加密的 IL/常量区
/// 解密写回自身内存（对抗静态分析与内存 dump）。
/// </summary>
		static void 反篡改内存补丁()
		{
			if (!Param.反篡改已执行标志)
			{
				Param.反篡改已执行标志 = true;
				BinaryReader binaryReader = new BinaryReader(typeof(Param).Assembly.GetManifestResourceStream("d08e40f8-0b4f-412d-8089-995ba478b812"));
				binaryReader.BaseStream.Position = 0L;
				byte[] array = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				byte[] array2 = new byte[32];
				array2[0] = 220;
				array2[0] = 93;
				array2[0] = 158;
				array2[0] = 68;
				array2[0] = 152;
				array2[0] = 137;
				array2[0] = 220;
				array2[1] = 61;
				array2[1] = 97;
				array2[1] = 198;
				array2[1] = 61;
				array2[2] = 81;
				array2[2] = 30;
				array2[2] = 92;
				array2[2] = 81;
				array2[3] = 65;
				array2[3] = 199;
				array2[3] = 163;
				array2[3] = 65;
				array2[4] = 234;
				array2[4] = 88;
				array2[4] = 153;
				array2[4] = 159;
				array2[4] = 106;
				array2[4] = 112;
				array2[4] = 234;
				array2[5] = 10;
				array2[5] = 117;
				array2[5] = 131;
				array2[5] = 93;
				array2[5] = 108;
				array2[5] = 10;
				array2[6] = 49;
				array2[6] = 86;
				array2[6] = 44;
				array2[6] = 165;
				array2[6] = 49;
				array2[7] = 182;
				array2[7] = 196;
				array2[7] = 22;
				array2[7] = 96;
				array2[7] = 182;
				array2[8] = 159;
				array2[8] = 189;
				array2[8] = 154;
				array2[8] = 159;
				array2[9] = 53;
				array2[9] = 103;
				array2[9] = 87;
				array2[9] = 157;
				array2[9] = 130;
				array2[9] = 102;
				array2[9] = 53;
				array2[10] = 100;
				array2[10] = 96;
				array2[10] = 164;
				array2[10] = 127;
				array2[10] = 54;
				array2[10] = 124;
				array2[10] = 100;
				array2[11] = 212;
				array2[11] = 116;
				array2[11] = 153;
				array2[11] = 212;
				array2[12] = 250;
				array2[12] = 148;
				array2[12] = 162;
				array2[12] = 250;
				array2[13] = 45;
				array2[13] = 54;
				array2[13] = 227;
				array2[13] = 117;
				array2[13] = 103;
				array2[13] = 45;
				array2[14] = 99;
				array2[14] = 102;
				array2[14] = 150;
				array2[14] = 119;
				array2[14] = 99;
				array2[15] = 144;
				array2[15] = 156;
				array2[15] = 156;
				array2[15] = 88;
				array2[15] = 98;
				array2[15] = 46;
				array2[15] = 144;
				array2[16] = 80;
				array2[16] = 86;
				array2[16] = 134;
				array2[16] = 91;
				array2[16] = 146;
				array2[16] = 80;
				array2[17] = 213;
				array2[17] = 113;
				array2[17] = 149;
				array2[17] = 145;
				array2[17] = 114;
				array2[17] = 213;
				array2[18] = 123;
				array2[18] = 64;
				array2[18] = 49;
				array2[18] = 147;
				array2[18] = 92;
				array2[18] = 149;
				array2[18] = 123;
				array2[19] = 28;
				array2[19] = 31;
				array2[19] = 164;
				array2[19] = 85;
				array2[19] = 117;
				array2[19] = 92;
				array2[19] = 28;
				array2[20] = 11;
				array2[20] = 142;
				array2[20] = 75;
				array2[20] = 103;
				array2[20] = 127;
				array2[20] = 105;
				array2[20] = 11;
				array2[21] = 173;
				array2[21] = 148;
				array2[21] = 166;
				array2[21] = 104;
				array2[21] = 173;
				array2[22] = 5;
				array2[22] = 127;
				array2[22] = 134;
				array2[22] = 148;
				array2[22] = 159;
				array2[22] = 36;
				array2[22] = 5;
				array2[23] = 211;
				array2[23] = 216;
				array2[23] = 65;
				array2[23] = 84;
				array2[23] = 156;
				array2[23] = 133;
				array2[23] = 211;
				array2[24] = 209;
				array2[24] = 173;
				array2[24] = 99;
				array2[24] = 149;
				array2[24] = 209;
				array2[25] = 39;
				array2[25] = 192;
				array2[25] = 164;
				array2[25] = 90;
				array2[25] = 127;
				array2[25] = 130;
				array2[25] = 39;
				array2[26] = 172;
				array2[26] = 104;
				array2[26] = 161;
				array2[26] = 86;
				array2[26] = 101;
				array2[26] = 132;
				array2[26] = 172;
				array2[27] = 41;
				array2[27] = 150;
				array2[27] = 100;
				array2[27] = 15;
				array2[27] = 119;
				array2[27] = 133;
				array2[27] = 41;
				array2[28] = 132;
				array2[28] = 90;
				array2[28] = 100;
				array2[28] = 110;
				array2[28] = 160;
				array2[28] = 164;
				array2[28] = 132;
				array2[29] = 88;
				array2[29] = 207;
				array2[29] = 130;
				array2[29] = 47;
				array2[29] = 84;
				array2[29] = 169;
				array2[29] = 88;
				array2[30] = 234;
				array2[30] = 142;
				array2[30] = 155;
				array2[30] = 234;
				array2[31] = 246;
				array2[31] = 138;
				array2[31] = 155;
				array2[31] = 231;
				array2[31] = 142;
				array2[31] = 106;
				array2[31] = 246;
				byte[] array3 = array2;
				byte[] array4 = new byte[16];
				array4[0] = 84;
				array4[0] = 227;
				array4[0] = 13;
				array4[1] = 98;
				array4[1] = 142;
				array4[1] = 88;
				array4[1] = 97;
				array4[1] = 82;
				array4[2] = 197;
				array4[2] = 35;
				array4[2] = 7;
				array4[3] = 92;
				array4[3] = 213;
				array4[3] = 125;
				array4[3] = 112;
				array4[3] = 88;
				array4[3] = 29;
				array4[4] = 128;
				array4[4] = 123;
				array4[4] = 138;
				array4[4] = 3;
				array4[5] = 122;
				array4[5] = 149;
				array4[5] = 91;
				array4[5] = 96;
				array4[5] = 119;
				array4[6] = 108;
				array4[6] = 87;
				array4[6] = 8;
				array4[7] = 110;
				array4[7] = 112;
				array4[7] = 135;
				array4[8] = 122;
				array4[8] = 101;
				array4[8] = 192;
				array4[9] = 93;
				array4[9] = 96;
				array4[9] = 128;
				array4[9] = 38;
				array4[10] = 137;
				array4[10] = 103;
				array4[10] = 85;
				array4[11] = 130;
				array4[11] = 156;
				array4[11] = 171;
				array4[11] = 111;
				array4[12] = 162;
				array4[12] = 88;
				array4[12] = 145;
				array4[12] = 91;
				array4[13] = 108;
				array4[13] = 96;
				array4[13] = 117;
				array4[13] = 132;
				array4[13] = 130;
				array4[14] = 134;
				array4[14] = 148;
				array4[14] = 162;
				array4[14] = 72;
				array4[14] = 18;
				array4[15] = 166;
				array4[15] = 112;
				array4[15] = 239;
				byte[] array5 = array4;
				Array.Reverse(array5);
				byte[] publicKeyToken = typeof(Param).Assembly.GetName().GetPublicKeyToken();
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
				Assembly assembly = typeof(Param).Assembly;
				intptr_ = Param.OpenProcess(56U, 1, (uint)Process.GetCurrentProcess().Id);
				Param.模块基址 = Marshal.GetHINSTANCE(assembly.GetModules()[0]).ToInt64();
				IntPtr zero = IntPtr.Zero;
				int int_ = 0;
				int num16 = binaryReader.ReadInt32();
				binaryReader.ReadInt32();
				for (int m = 0; m < num16; m++)
				{
					IntPtr intPtr = new IntPtr(Param.模块基址 + (long)binaryReader.ReadInt32());
					Param.VirtualProtect(intPtr, 4, 4, ref int_);
					if (IntPtr.Size == 4)
					{
						Param.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					else
					{
						Param.WriteProcessMemory(intptr_, intPtr, BitConverter.GetBytes(binaryReader.ReadInt32()), 4U, out zero);
					}
					Param.VirtualProtect(intPtr, 4, int_, ref int_);
				}
				try
				{
					while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length - 1L)
					{
						int num17 = binaryReader.ReadInt32();
						IntPtr intptr_2 = new IntPtr(Param.模块基址 + (long)num17);
						int num18 = binaryReader.ReadInt32();
						Param.VirtualProtect(intptr_2, num18 * 4, 4, ref int_);
						for (int n = 0; n < num18; n++)
						{
							Marshal.WriteInt32(new IntPtr(intptr_2.ToInt64() + (long)(n * 4)), binaryReader.ReadInt32());
						}
						Param.VirtualProtect(intptr_2, num18 * 4, int_, ref int_);
					}
					Param.CloseHandle(intptr_);
				}
				catch
				{
				}
			}
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00009E84 File Offset: 0x00008084
/// <summary>取程序集路径：依次尝试 Location / CodeBase(去 file:/// 前缀) / 反射 Location 属性。</summary>
		internal static object 取程序集路径(Assembly assembly_0)
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

		// Token: 0x060000BD RID: 189
		[DllImport("kernel32.dll")]
/// <summary>Win32 kernel32!WriteProcessMemory：写远程/自身进程内存（反篡改补丁写回）。</summary>
		private static extern int WriteProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x060000BE RID: 190
		[DllImport("kernel32.dll")]
/// <summary>Win32 kernel32!ReadProcessMemory（声明但未调用，残留）。</summary>
		private static extern int ReadProcessMemory(IntPtr intptr_0, IntPtr intptr_1, [In] [Out] byte[] byte_0, uint uint_0, out IntPtr intptr_2);

		// Token: 0x060000BF RID: 191
		[DllImport("kernel32.dll")]
/// <summary>Win32 kernel32!OpenProcess：打开进程句柄（反篡改用）。</summary>
		private static extern IntPtr OpenProcess(uint uint_0, int int_0, uint uint_1);

		// Token: 0x060000C0 RID: 192
		[DllImport("kernel32.dll")]
/// <summary>Win32 kernel32!CloseHandle：关闭句柄（反篡改用）。</summary>
		private static extern int CloseHandle(IntPtr intptr_0);

		// Token: 0x060000C1 RID: 193 RVA: 0x00009F94 File Offset: 0x00008194
/// <summary>读文件全部字节：FileStream 循环读取直至读完。</summary>
		private static byte[] 读文件全部字节(string string_0)
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

		// Token: 0x060000C2 RID: 194 RVA: 0x00009FF4 File Offset: 0x000081F4
/// <summary>
/// AES 解密：固定 32B Key（174,3,233,…）/16B IV（72,84,7,…），CreateDecryptor 解密输入字节。
/// </summary>
		private static byte[] AES解密(byte[] byte_0)
		{
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = Param.创建对称算法();
			symmetricAlgorithm.Key = new byte[]
			{
				174,
				3,
				233,
				198,
				108,
				124,
				130,
				181,
				209,
				138,
				124,
				54,
				1,
				58,
				146,
				83,
				227,
				144,
				35,
				236,
				213,
				169,
				79,
				17,
				160,
				140,
				193,
				1,
				137,
				181,
				233,
				125
			};
			symmetricAlgorithm.IV = new byte[]
			{
				72,
				84,
				7,
				140,
				39,
				237,
				168,
				77,
				36,
				176,
				187,
				177,
				22,
				104,
				217,
				203
			};
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(byte_0, 0, byte_0.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000A060 File Offset: 0x00008260
/// <summary>空桩方法（恒返回 null，混淆器填充）。</summary>
		private byte[] 空桩1()
		{
			return null;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000A060 File Offset: 0x00008260
/// <summary>空桩方法（恒返回 null，混淆器填充）。</summary>
		private byte[] 空桩2()
		{
			return null;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000A060 File Offset: 0x00008260
/// <summary>空桩方法（恒返回 null，混淆器填充）。</summary>
		private byte[] 空桩3()
		{
			return null;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000A060 File Offset: 0x00008260
/// <summary>空桩方法（恒返回 null，混淆器填充）。</summary>
		private byte[] 空桩4()
		{
			return null;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000A070 File Offset: 0x00008270
/// <summary>空桩方法（恒返回 null，混淆器填充）。</summary>
		private byte[] 空桩5()
		{
			return null;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000A070 File Offset: 0x00008270
/// <summary>空桩方法（恒返回 null，混淆器填充）。</summary>
		private byte[] 空桩6()
		{
			return null;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00002408 File Offset: 0x00000608
/// <summary>诱饵方法：返回 {1,2}，引用伪 GUID 串 {11111-22222-40001-00001}（混淆器填充）。</summary>
		internal byte[] 诱饵1()
		{
			int length = "{11111-22222-40001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00002423 File Offset: 0x00000623
/// <summary>诱饵方法：返回 {1,2}，引用伪 GUID 串 {11111-22222-40001-00002}（混淆器填充）。</summary>
		internal byte[] 诱饵2()
		{
			int length = "{11111-22222-40001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000243E File Offset: 0x0000063E
/// <summary>诱饵方法：返回 {1,2}，引用伪 GUID 串 {11111-22222-50001-00001}（混淆器填充）。</summary>
		internal byte[] 诱饵3()
		{
			int length = "{11111-22222-50001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00002459 File Offset: 0x00000659
/// <summary>诱饵方法：返回 {1,2}，引用伪 GUID 串 {11111-22222-50001-00002}（混淆器填充）。</summary>
		internal byte[] 诱饵4()
		{
			int length = "{11111-22222-50001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00002474 File Offset: 0x00000674
/// <summary>诱饵方法：返回 {1,2}，引用伪 GUID 串 {11111-22222-60001-00001}（混淆器填充）。</summary>
		internal byte[] 诱饵5()
		{
			int length = "{11111-22222-60001-00001}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000248F File Offset: 0x0000068F
/// <summary>诱饵方法：返回 {1,2}，引用伪 GUID 串 {11111-22222-60001-00002}（混淆器填充）。</summary>
		internal byte[] 诱饵6()
		{
			int length = "{11111-22222-60001-00002}".Length;
			return new byte[]
			{
				1,
				2
			};
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000A080 File Offset: 0x00008280
/// <summary>
/// AES 加密→Base64：Key=32 字节固定数组；IV = MD5摘要(MD5(UTF8(string_1)))；CreateEncryptor 加密
/// UTF8(string_0) 后 Base64 输出（字符串加密工具，供壳解密字符串用）。
/// </summary>
		internal static string AES加密转Base64(string string_0, string string_1)
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
			byte[] iv = Param.MD5摘要(Encoding.Unicode.GetBytes(string_1));
			MemoryStream memoryStream = new MemoryStream();
			SymmetricAlgorithm symmetricAlgorithm = Param.创建对称算法();
			symmetricAlgorithm.Key = key;
			symmetricAlgorithm.IV = iv;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.Close();
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>混淆器填充方法（恒返回 true，无意义）。</summary>
		internal static bool RevertThread()
		{
			return true;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x000021C6 File Offset: 0x000003C6
/// <summary>混淆器填充方法（恒返回 false，无意义）。</summary>
		internal static bool ForgotClass()
		{
			return false;
		}

		// Token: 0x04000058 RID: 88
/// <summary>fips 模式标志：检测FIPS策略 反射设置，是否FIPS模式 返回。</summary>
		private static bool fips模式标志;

		// Token: 0x04000059 RID: 89
/// <summary>未使用字段（混淆器填充）。</summary>
		private static string[] _Comparator;

		// Token: 0x0400005A RID: 90
/// <summary>未使用字段（混淆器填充，静态构造置 1）。</summary>
		private static int merchant;

		// Token: 0x0400005B RID: 91
/// <summary>模块基址：Marshal.GetHINSTANCE(模块0) 所得自身映像基址（反篡改偏移基准）。</summary>
		private static long 模块基址;

		// Token: 0x0400005C RID: 92
/// <summary>反篡改已执行标志：反篡改内存补丁 一次性防重入。</summary>
		private static bool 反篡改已执行标志;

		// Token: 0x0400005D RID: 93
/// <summary>未使用字段（混淆器填充）。</summary>
		private static byte[] m_Observer;

		// Token: 0x0400005E RID: 94
/// <summary>未使用字段（混淆器填充）。</summary>
		private static int[] role;

		// Token: 0x0400005F RID: 95
/// <summary>未使用字段（混淆器填充）。</summary>
		private static IntPtr task;

		// Token: 0x04000060 RID: 96
/// <summary>未使用字段（混淆器填充）。</summary>
		private static byte[] _Field;

		// Token: 0x04000061 RID: 97
/// <summary>未使用字段（混淆器填充）。</summary>
		private static byte[] _Strategy;

		// Token: 0x04000062 RID: 98
/// <summary>fips 检测完成标志：是否FIPS模式 首次调用后置位。</summary>
		private static bool fips检测完成标志;

		// Token: 0x04000063 RID: 99
/// <summary>未使用字段（混淆器填充）。</summary>
		private static IntPtr mock;

		// Token: 0x04000064 RID: 100
/// <summary>解密后的字符串池：取解密字符串常量 缓存（首次解密整段资源）。</summary>
		private static byte[] 解密字符串池;

		// Token: 0x04000065 RID: 101
/// <summary>MD5 64 轮常数表（3614090360 起始的标准 MD5 常量，自实现MD5 用）。</summary>
		private static uint[] MD5常数表 = new uint[]
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

		// Token: 0x04000066 RID: 102
/// <summary>未使用字段（混淆器填充）。</summary>
		private static int m_Connection;

		// Token: 0x02000016 RID: 22
/// <summary>混淆器辅助 Attribute（标记方法，无运行时逻辑）。</summary>
		internal class Service : Attribute
		{
			// Token: 0x060000D3 RID: 211 RVA: 0x000024AA File Offset: 0x000006AA
			[Param.Service(typeof(Param.Service.Worker<object>[]))]
/// <summary>Service 构造函数（空实现，混淆器填充）。</summary>
			public Service(object object_0)
			{
			}

			// Token: 0x02000017 RID: 23
/// <summary>填充泛型（恒 true/false 方法）。</summary>
			internal class Worker<T>
			{
				// Token: 0x060000D5 RID: 213 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>混淆器填充方法（恒返回 true，无意义）。</summary>
				internal static bool CompareThread()
				{
					return true;
				}

				// Token: 0x060000D6 RID: 214 RVA: 0x000021C6 File Offset: 0x000003C6
/// <summary>混淆器填充方法（恒返回 false，无意义）。</summary>
				internal static bool SortThread()
				{
					return false;
				}
			}
		}

		// Token: 0x02000018 RID: 24
/// <summary>空 Flags 枚举（填充物，无成员）。</summary>
		[Flags]
		private enum AuditSettings
		{

		}
	}
}
