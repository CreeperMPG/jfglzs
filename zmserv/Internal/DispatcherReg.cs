using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace zmserv.Internal
{
	// Token: 0x02000012 RID: 18
	/// <summary>
	/// DispatcherReg —— 嵌入程序集解析器（.NET Reactor 内嵌依赖加载钩子）。
	/// 启动时向 AppDomain.AssemblyResolve 注册钩子；CLR 需要加载本机不存在的程序集时，
	/// 按 "b0494a1f-4bd3-"+MD5(UTF-16(程序集名)) 的 Base64 资源名从嵌入资源取 DLL 字节并 Assembly.Load；
	/// 内存加载失败则释放到 %TEMP% 后 LoadFile。业务恶意逻辑在被动态解出的嵌入资源里，本文件无业务代码。
	/// </summary>
	internal class DispatcherReg
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00005520 File Offset: 0x00003720
		/// <summary>
		/// 注册 AssemblyResolve 解析钩子（一次性）：hookRegistered 为 false 时挂接 OnAssemblyResolve，防止重复注册。
		/// </summary>
		internal static void RegisterResolveHook()
		{
			if (!DispatcherReg.hookRegistered)
			{
				DispatcherReg.hookRegistered = true;
				AppDomain.CurrentDomain.AssemblyResolve += DispatcherReg.OnAssemblyResolve;
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005550 File Offset: 0x00003750
		/// <summary>
		/// 程序集解析回调：① 缓存 loadedAssemblyCache 命中 → 直接返回；② 未命中 → RSACryptoServiceProvider.UseMachineKeyStore=true，
		/// StripAssemblyStrongName 取简单名，资源名 = "b0494a1f-4bd3-" + Base64(MD5(UTF-16(简单名)))；③ 读嵌入资源字节 → Assembly.Load(bytes)；
		/// ④ FileLoadException/BadImageFormatException 时写文件 %TEMP%\<资源名>\<名>.dll（不存在才写）→ Assembly.LoadFile；
		/// ⑤ 成功/失败路径均写缓存；全程 try/catch 吞异常，失败返回 null。
		/// </summary>
		private static Assembly OnAssemblyResolve(object object_0, ResolveEventArgs resolveEventArgs_0)
		{
			Hashtable listenerReg = DispatcherReg.loadedAssemblyCache;
			Assembly result;
			lock (listenerReg)
			{
				string text = resolveEventArgs_0.Name.Trim();
				object obj = DispatcherReg.loadedAssemblyCache[text];
				if (obj == null)
				{
					try
					{
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string text2 = DispatcherReg.StripAssemblyStrongName(text);
						byte[] bytes = Encoding.Unicode.GetBytes(text2);
						string text3 = "b0494a1f-4bd3-" + Convert.ToBase64String(CustomerReg.ComputeMd5Hash(bytes));
						Stream manifestResourceStream = typeof(DispatcherReg).Assembly.GetManifestResourceStream(text3);
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
									DispatcherReg.loadedAssemblyCache.Add(text, assembly);
								}
								else
								{
									DispatcherReg.loadedAssemblyCache.Add(text, assembly);
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

		// Token: 0x06000065 RID: 101 RVA: 0x0000574C File Offset: 0x0000394C
		/// <summary>剥离程序集强名称：取逗号前的简单名（Trim 后截断到第一个 ','）。</summary>
		private static string StripAssemblyStrongName(string string_0)
		{
			string text = string_0.Trim();
			int num = text.IndexOf(',');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>混淆填充：恒真，无业务意义（无调用方）。</summary>
		internal static bool ConfuserFillerTrue()
		{
			return true;
		}

		// Token: 0x04000027 RID: 39
		/// <summary>已加载程序集缓存：程序集名→Assembly（Hashtable，解析回调加锁使用）。</summary>
		private static Hashtable loadedAssemblyCache = new Hashtable();

		// Token: 0x04000028 RID: 40
		/// <summary>钩子已注册标志：防止 AssemblyResolve 重复注册。</summary>
		private static bool hookRegistered = false;
	}
}
