using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace WindowsApplication1.Internal
{
	/// <summary>
	/// 内嵌程序集加载器（.NET Reactor 的 AssemblyResolve 钩子）。
	/// 程序集启动后注册一次 <see cref="AppDomain.AssemblyResolve"/>；之后每当 CLR 找不到某个依赖，
	/// 它就用「程序集简单名的 MD5 摘要」拼出资源名 <c>b0494a1f-4bd3-&lt;base64&gt;</c>，
	/// 从本程序集的内嵌资源里取出被压缩/加密的依赖 DLL，直接 <see cref="Assembly.Load(byte[])"/> 加载；
	/// 若 Load 失败（FileLoadException / BadImageFormatException），则把 DLL 落到
	/// <c>%TEMP%\b0494a1f-4bd3-&lt;base64&gt;\&lt;简单名&gt;.dll</c> 再用 <see cref="Assembly.LoadFile(string)"/> 加载。
	/// 也就是说：**本程序在运行期会把 DLL 写到用户临时目录**，这是重要的落地痕迹。
	/// </summary>
	// Token: 0x02000015 RID: 21
	internal class IssuerMapper
	{
		// Token: 0x060000AA RID: 170 RVA: 0x00007418 File Offset: 0x00005618
		/// <summary>
		/// 注册一次程序集解析钩子（幂等：用静态布尔做去重，先置标志再挂事件）。
		/// 本工程内没有调用点，原样本里由壳的初始化 stub 调用。
		/// </summary>
		internal static void 注册程序集解析事件()
		{
			if (!IssuerMapper.已注册解析事件)
			{
				IssuerMapper.已注册解析事件 = true;
				AppDomain 当前域 = AppDomain.CurrentDomain;
				当前域.AssemblyResolve += IssuerMapper.解析内嵌程序集;
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000744C File Offset: 0x0000564C
		/// <summary>
		/// AssemblyResolve 处理器：按「程序集全名 → 简单名 → MD5 → 资源名」的顺序找回内嵌依赖。
		/// 整个方法在 <see cref="已加载程序集缓存"/> 上加锁，避免同名程序集被并发加载两次；
		/// 成功解析出的程序集会按原始全名缓存，后续直接命中。
		/// 任何一步失败都只返回 null（外层两处 catch 全吞），不会打断调用方的加载流程。
		/// </summary>
		private static Assembly 解析内嵌程序集(object 发送者, ResolveEventArgs 解析参数)
		{
			Assembly 结果;
			lock (IssuerMapper.已加载程序集缓存)
			{
				string 程序集名 = 解析参数.Name.Trim();
				object 缓存项 = IssuerMapper.已加载程序集缓存[程序集名];
				if (缓存项 == null)
				{
					try
					{
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string 简单名 = IssuerMapper.取程序集简单名(程序集名);
						byte[] 名称字节 = Encoding.Unicode.GetBytes(简单名);
						string 资源名 = "b0494a1f-4bd3-" + Convert.ToBase64String(InstanceMapper.MD5摘要(名称字节));
						Stream 资源流 = typeof(IssuerMapper).Assembly.GetManifestResourceStream(资源名);
						if (资源流 != null)
						{
							try
							{
								BinaryReader 资源读取器 = new BinaryReader(资源流);
								资源读取器.BaseStream.Position = 0L;
								byte[] 程序集字节 = new byte[资源流.Length];
								资源读取器.Read(程序集字节, 0, 程序集字节.Length);
								资源读取器.Close();
								bool 需要写入临时文件 = false;
								Assembly 程序集 = null;
								try
								{
									程序集 = Assembly.Load(程序集字节);
								}
								catch (FileLoadException)
								{
									需要写入临时文件 = true;
								}
								catch (BadImageFormatException)
								{
									需要写入临时文件 = true;
								}
								if (需要写入临时文件)
								{
									string 临时目录 = Path.Combine(Path.GetTempPath(), 资源名);
									string 临时文件路径 = Path.Combine(临时目录, 简单名 + ".dll");
									if (!File.Exists(临时文件路径))
									{
										Directory.CreateDirectory(Path.GetDirectoryName(临时文件路径));
										FileStream 临时文件流 = new FileStream(临时文件路径, FileMode.Create, FileAccess.Write);
										临时文件流.Write(程序集字节, 0, 程序集字节.Length);
										临时文件流.Close();
									}
									程序集 = Assembly.LoadFile(临时文件路径);
									IssuerMapper.已加载程序集缓存.Add(程序集名, 程序集);
								}
								else
								{
									IssuerMapper.已加载程序集缓存.Add(程序集名, 程序集);
								}
								return 程序集;
							}
							catch
							{
							}
						}
					}
					catch
					{
					}
					结果 = null;
				}
				else
				{
					结果 = (Assembly)缓存项;
				}
			}
			return 结果;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00007660 File Offset: 0x00005860
		/// <summary>
		/// 从程序集全名里截出简单名：全名形如 <c>Name, Version=..., Culture=..., PublicKeyToken=...</c>，
		/// 取第一个逗号之前的部分。<see cref="string.Trim"/> 先去掉首尾空白，找不到逗号就整串返回。
		/// </summary>
		private static string 取程序集简单名(string 程序集全名)
		{
			string 程序集名 = 程序集全名.Trim();
			int 逗号位置 = 程序集名.IndexOf(',');
			if (逗号位置 >= 0)
			{
				程序集名 = 程序集名.Substring(0, 逗号位置);
			}
			return 程序集名;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>恒返回 true 的桩方法，无调用点。</summary>
		internal static bool 返回真占位()
		{
			return true;
		}

		// Token: 0x04000039 RID: 57
		/// <summary>已解析程序集的缓存：程序集全名 → Assembly。同时充当 <c>lock</c> 的对象。</summary>
		private static Hashtable 已加载程序集缓存 = new Hashtable();

		// Token: 0x0400003A RID: 58
		/// <summary>是否已注册 AssemblyResolve 事件。</summary>
		private static bool 已注册解析事件 = false;
	}
}
