using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace WindowsApplication1.Internal
{
		/// <summary>
		/// 嵌入程序集加载器（AssemblyResolve 处理器）：加壳运行时把依赖 DLL 加密/改名后塞进主程序集，
		/// 程序集解析失败时按程序集名生成资源名，从嵌入资源读出字节 Assembly.Load（或落盘 %TEMP% 后 LoadFile）。
		/// 资源名 = "b0494a1f-4bd3-" + Base64(MD5(程序集名))，脱壳时可按 MD5 预计算枚举。
		/// </summary>
	// Token: 0x02000018 RID: 24
	internal class ConfigurationMock
	{
		/// <summary>
		/// 一次性注册 AppDomain.CurrentDomain.AssemblyResolve → OnAssemblyResolve_LoadEmbeddedAssembly（_handlerRegistered 防重复）。
		/// </summary>
		// Token: 0x0600012A RID: 298 RVA: 0x0000EAB8 File Offset: 0x0000CCB8
		internal static void EnsureAssemblyResolveHandler()
		{
			if (!ConfigurationMock._handlerRegistered)
			{
				ConfigurationMock._handlerRegistered = true;
				AppDomain.CurrentDomain.AssemblyResolve += ConfigurationMock.OnAssemblyResolve_LoadEmbeddedAssembly;
			}
		}

		/// <summary>
		/// 解析程序集：lock 缓存表；命中直接返回；未命中 → UseMachineKeyStore=true、StripAssemblyName 取纯名、
		/// 资源名 = "b0494a1f-4bd3-" + Base64(TestMock.ComputeMd5(UTF16LE(纯名)))，GetManifestResourceStream 读字节；
		/// Assembly.Load 抛 FileLoad/BadImageFormat → 落盘 %TEMP%\<资源名>\<纯名>.dll 再 Assembly.LoadFile；成功入缓存；全程吞异常返回 null。
		/// </summary>
		// Token: 0x0600012B RID: 299 RVA: 0x0000EAE8 File Offset: 0x0000CCE8
		private static Assembly OnAssemblyResolve_LoadEmbeddedAssembly(object sender, ResolveEventArgs args)
		{
			Hashtable codeMock = ConfigurationMock._loadedAssemblyCache;
			Assembly result;
			lock (codeMock)
			{
				string text = args.Name.Trim();
				object obj = ConfigurationMock._loadedAssemblyCache[text];
				if (obj == null)
				{
					try
					{
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string mock = ConfigurationMock.StripAssemblyName(text);
						byte[] bytes = Encoding.Unicode.GetBytes(mock);
						string text2 = "b0494a1f-4bd3-" + Convert.ToBase64String(TestMock.ComputeMd5(bytes));
						Stream manifestResourceStream = typeof(ConfigurationMock).Assembly.GetManifestResourceStream(text2);
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
									string path = Path.Combine(Path.Combine(Path.GetTempPath(), text2), mock + ".dll");
									if (!File.Exists(path))
									{
										Directory.CreateDirectory(Path.GetDirectoryName(path));
										FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
										fileStream.Write(array, 0, array.Length);
										fileStream.Close();
									}
									assembly = Assembly.LoadFile(path);
									ConfigurationMock._loadedAssemblyCache.Add(text, assembly);
								}
								else
								{
									ConfigurationMock._loadedAssemblyCache.Add(text, assembly);
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

		/// <summary>
		/// 去掉程序集全名 ", Version=..." 等后缀，返回纯程序集名。
		/// </summary>
		// Token: 0x0600012C RID: 300 RVA: 0x0000ECE4 File Offset: 0x0000CEE4
		private static string StripAssemblyName(string assemblyFullName)
		{
			string text = assemblyFullName.Trim();
			int num = text.IndexOf(',');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		/// <summary>
		/// 已加载程序集缓存：Hashtable<程序集名, Assembly>。
		/// </summary>
		// Token: 0x04000086 RID: 134
		private static Hashtable _loadedAssemblyCache = new Hashtable();

		/// <summary>
		/// 是否已注册 AssemblyResolve 处理器标志。
		/// </summary>
		// Token: 0x04000087 RID: 135
		private static bool _handlerRegistered = false;
	}
}
