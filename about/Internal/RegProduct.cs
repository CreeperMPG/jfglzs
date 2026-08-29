using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace about.Internal
{
	// Token: 0x02000015 RID: 21
	/// <summary>
	/// AssemblyResolve 处理器（原名 RegProduct）：从嵌入资源按需解包/落盘加载依赖 DLL（.NET Reactor 资源合并机制）。
	/// 大量真实业务代码藏在这些按需加载的嵌入 DLL 里，本文件是它们的「装载器」。
	/// </summary>
	internal class RegProduct
	{
		// Token: 0x060000AB RID: 171 RVA: 0x0000739C File Offset: 0x0000559C
		/// <summary>
		/// 注册 AssemblyResolve 处理器（原名 smethod_0）：一次性注册（isHandlerRegistered 防重复）。
		/// </summary>
		internal static void RegisterAssemblyResolveHandler()
		{
			if (!RegProduct.isHandlerRegistered)
			{
				RegProduct.isHandlerRegistered = true;
				AppDomain.CurrentDomain.AssemblyResolve += RegProduct.OnAssemblyResolve;
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000073CC File Offset: 0x000055CC
		/// <summary>
		/// AssemblyResolve 事件处理（原名 QueryProduct），从嵌入资源加载依赖程序集：
		/// 1. 缓存命中直接返回；
		/// 2. 资源名 = "b0494a1f-4bd3-" + Base64(MD5(UTF16(纯程序集名)))；
		/// 3. GetManifestResourceStream 读取字节，优先 Assembly.Load(内存)；
		/// 4. 抛 FileLoadException / BadImageFormatException 时落盘到 %TEMP%\&lt;资源名&gt;\&lt;程序集名&gt;.dll 再 Assembly.LoadFile；
		/// 5. 成功加入缓存；失败返回 null 交给 CLR 默认处理。
		/// </summary>
		private static Assembly OnAssemblyResolve(object object_0, ResolveEventArgs resolveEventArgs_0)
		{
			Hashtable specificationProduct = RegProduct.resolvedAssemblyCache;
			Assembly result;
			lock (specificationProduct)
			{
				string text = resolveEventArgs_0.Name.Trim();
				object obj = RegProduct.resolvedAssemblyCache[text];
				if (obj == null)
				{
					try
					{
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string text2 = RegProduct.ExtractSimpleAssemblyName(text);
						byte[] bytes = Encoding.Unicode.GetBytes(text2);
						string text3 = "b0494a1f-4bd3-" + Convert.ToBase64String(InvocationProduct.ComputeMd5(bytes));
						Stream manifestResourceStream = typeof(RegProduct).Assembly.GetManifestResourceStream(text3);
						if (manifestResourceStream != null)
						{
							try
							{
								BinaryReader binaryReader = new BinaryReader(manifestResourceStream);
								binaryReader.BaseStream.Position = 0L;
								byte[] array = new byte[manifestResourceStream.Length];
								binaryReader.Read(array, 0, array.Length);
								binaryReader.Close();
								bool flag = false;
								Assembly assembly = null;
								try
								{
									assembly = Assembly.Load(array);
								}
								catch (FileLoadException)
								{
									flag = true;
								}
								catch (BadImageFormatException)
								{
									flag = true;
								}
								if (flag)
								{
									string path = Path.Combine(Path.Combine(Path.GetTempPath(), text3), text2 + ".dll");
									if (!File.Exists(path))
									{
										Directory.CreateDirectory(Path.GetDirectoryName(path));
										FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
										fileStream.Write(array, 0, array.Length);
										fileStream.Close();
									}
									assembly = Assembly.LoadFile(path);
									RegProduct.resolvedAssemblyCache.Add(text, assembly);
								}
								else
								{
									RegProduct.resolvedAssemblyCache.Add(text, assembly);
								}
								return assembly;
							}
							catch
							{
							}
						}
					}
					catch
					{
					}
					result = null;
				}
				else
				{
					result = (Assembly)obj;
				}
			}
			return result;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000075C8 File Offset: 0x000057C8
		/// <summary>提取纯程序集名（原名 SetProduct）：去掉 ", Culture=..." 等后缀。</summary>
		private static string ExtractSimpleAssemblyName(string string_0)
		{
			string text = string_0.Trim();
			int num = text.IndexOf(',');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000021C9 File Offset: 0x000003C9
		/// <summary>混淆填充桩（原名 DestroyListener），恒 false，无实义。</summary>
		internal static bool DestroyListener()
		{
			return false;
		}

		// Token: 0x04000039 RID: 57
		/// <summary>已解析程序集缓存 Hashtable（原名 m_SpecificationProduct），线程锁保护。</summary>
		private static Hashtable resolvedAssemblyCache = new Hashtable();

		// Token: 0x0400003A RID: 58
		/// <summary>是否已注册 AssemblyResolve 处理器标志（原名 exceptionProduct）。</summary>
		private static bool isHandlerRegistered = false;
	}
}
