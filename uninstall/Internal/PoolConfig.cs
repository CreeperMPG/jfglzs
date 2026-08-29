using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000017 RID: 23
	// <summary>
	/// .NET Reactor "Pack/Embed Assemblies"（嵌入程序集）功能的运行时：
	/// 注册 AppDomain.AssemblyResolve 回调，当程序集加载不到时，从自身嵌入资源中
	/// 按名称哈希取出字节流加载（优先内存加载，失败则释放到 %TEMP% 再 LoadFile）。
	/// 属于加壳器运行时，无业务恶意逻辑。
	/// </summary>
	internal class PoolConfig
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00007CD8 File Offset: 0x00005ED8
		/// <summary>
		/// 注册 AssemblyResolve 钩子（原混淆名 smethod_0）：幂等——
		/// 仅当 _hookRegistered 为 false 时挂接一次 OnAssemblyResolve。
		/// </summary>
		internal static void RegisterAssemblyResolveHandler()
		{
			if (!PoolConfig._hookRegistered)
			{
				PoolConfig._hookRegistered = true;
				AppDomain.CurrentDomain.AssemblyResolve += PoolConfig.OnAssemblyResolve;
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00007D08 File Offset: 0x00005F08
		/// <summary>
		/// AssemblyResolve 回调（原混淆名 InsertConfig）：
		/// 1) 请求名去掉强名称部分得到短名 text2；
		/// 2) 资源名 = "b0494a1f-4bd3-" + Base64(MD5(UTF16(text2)))，从自身嵌入资源取流；
		/// 3) 优先 Assembly.Load(字节) 内存加载；失败（FileLoad/BadImageFormat）则写入
		///    %TEMP%\<资源名>\<短名>.dll 后 Assembly.LoadFile；
		/// 4) 结果缓存进 Hashtable _assemblyCache，避免重复解密。
		/// </summary>
		private static Assembly OnAssemblyResolve(object object_0, ResolveEventArgs resolveEventArgs_0)
		{
			Hashtable composerConfig = PoolConfig._assemblyCache;
			Assembly result;
			lock (composerConfig)
			{
				string text = resolveEventArgs_0.Name.Trim();
				object obj = PoolConfig._assemblyCache[text];
				if (obj == null)
				{
					try
					{
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string text2 = PoolConfig.GetAssemblyShortName(text);
						byte[] bytes = Encoding.Unicode.GetBytes(text2);
						string text3 = "b0494a1f-4bd3-" + Convert.ToBase64String(OrderConfig.ComputeMd5Hash(bytes));
						Stream manifestResourceStream = typeof(PoolConfig).Assembly.GetManifestResourceStream(text3);
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
									PoolConfig._assemblyCache.Add(text, assembly);
								}
								else
								{
									PoolConfig._assemblyCache.Add(text, assembly);
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

		// Token: 0x0600009B RID: 155 RVA: 0x00007F04 File Offset: 0x00006104
		/// <summary>
		/// 截取程序集短名（原混淆名 PrepareConfig）：取请求名中逗号之前的部分
		/// （去掉强名称/版本等元数据）。
		/// </summary>
		private static string GetAssemblyShortName(string string_0)
		{
			string text = string_0.Trim();
			int num = text.IndexOf(',');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		// Token: 0x04000042 RID: 66
		/// <summary>已加载程序集缓存（Hashtable，按程序集全名），避免重复解密加载。</summary>
		private static Hashtable _assemblyCache = new Hashtable();

		// Token: 0x04000043 RID: 67
		/// <summary>AssemblyResolve 钩子是否已注册的标志（幂等保护）。</summary>
		private static bool _hookRegistered = false;
	}
}
