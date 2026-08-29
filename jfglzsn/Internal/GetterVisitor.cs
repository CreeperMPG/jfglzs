using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace WindowsApplication1.Internal
{
	// Token: 0x0200001A RID: 26
	// ============================================================
	// Internal/GetterVisitor.cs — 依赖程序集嵌入资源解密加载器（AssemblyResolve 钩子）
	// 当 CLR 需要加载缺失的依赖程序集时，按程序集名 MD5 哈希定位嵌入资源（资源名形如 b0494a1f-4bd3-<base64(MD5(程序集名))>），读出字节直接 Assembly.Load；FileLoadException/BadImageFormatException 时落盘 %TEMP%\<资源名>\<程序集名>.dll 再 LoadFile。已加载结果缓存于 loadedAssembliesCache。.NET Reactor 嵌入式依赖程序集机制（隐藏真实程序集、对抗静态分析）。
	// ============================================================
	internal class GetterVisitor
	{
		// Token: 0x06000103 RID: 259 RVA: 0x0000C888 File Offset: 0x0000AA88
		/// <summary>
		/// 注册 AssemblyResolve 钩子（原名 smethod_0）：AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler，用 isRegisteredFlag 保证只挂一次。
		/// </summary>
		internal static void RegisterAssemblyResolve()
		{
			if (!GetterVisitor.isRegisteredFlag)
			{
				GetterVisitor.isRegisteredFlag = true;
				AppDomain.CurrentDomain.AssemblyResolve += GetterVisitor.AssemblyResolveHandler;
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000C8B8 File Offset: 0x0000AAB8
		/// <summary>
		/// AssemblyResolve 处理器（原名 ReflectVisitor）：1. 锁 loadedAssembliesCache，缓存命中直接返回；2. 未命中：UseMachineKeyStore=true，取简单名 GetSimpleAssemblyName；3. 资源名 = "b0494a1f-4bd3-" + Base64(Md5Hash(Unicode(简单名)))；4. GetManifestResourceStream 取嵌入资源；5. 先 Assembly.Load(bytes)，捕获 FileLoadException/BadImageFormatException 后落盘 %TEMP%\<资源名>\<简单名>.dll 再 LoadFile；6. 成功加入缓存返回，失败返回 null。
		/// 说明：.NET Reactor 嵌入式依赖程序集机制（隐藏真实程序集、对抗静态分析）。
		/// </summary>
		private static Assembly AssemblyResolveHandler(object object_0, ResolveEventArgs resolveEventArgs_0)
		{
			Hashtable eventVisitor = GetterVisitor.loadedAssembliesCache;
			Assembly result;
			lock (eventVisitor)
			{
				string text = resolveEventArgs_0.Name.Trim();
				object obj = GetterVisitor.loadedAssembliesCache[text];
				if (obj == null)
				{
					try
					{
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string text2 = GetterVisitor.GetSimpleAssemblyName(text);
						byte[] bytes = Encoding.Unicode.GetBytes(text2);
						string text3 = "b0494a1f-4bd3-" + Convert.ToBase64String(ComparatorVisitor.Md5Hash(bytes));
						Stream manifestResourceStream = typeof(GetterVisitor).Assembly.GetManifestResourceStream(text3);
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
									GetterVisitor.loadedAssembliesCache.Add(text, assembly);
								}
								else
								{
									GetterVisitor.loadedAssembliesCache.Add(text, assembly);
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

		// Token: 0x06000105 RID: 261 RVA: 0x0000CAB4 File Offset: 0x0000ACB4
		/// <summary>
		/// 取程序集简单名（原名 FlushVisitor）：去掉 ", Version=..., Culture=..., PublicKeyToken=..." 后缀。
		/// </summary>
		private static string GetSimpleAssemblyName(string string_0)
		{
			string text = string_0.Trim();
			int num = text.IndexOf(',');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		// Token: 0x040000C3 RID: 195
		/// <summary>
		/// 已加载程序集缓存（原名 _EventVisitor）：Hashtable：程序集名 → Assembly。
		/// </summary>
		private static Hashtable loadedAssembliesCache = new Hashtable();

		// Token: 0x040000C4 RID: 196
		/// <summary>
		/// 是否已注册标志（原名 m_UtilsVisitor）：防重复挂接。
		/// </summary>
		private static bool isRegisteredFlag = false;
	}
}
