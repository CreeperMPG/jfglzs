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
	/// IntelliLock 的「内嵌程序集」加载器。
	/// 真实职责（很重要）：挂钩 AppDomain.AssemblyResolve，当 CLR 找不到某个被引用程序集时，
	/// 用「"b0494a1f-4bd3-" + Base64(MD5(程序集简单名))」拼出嵌入资源名，从中读出 DLL 字节，
	/// 先 Assembly.Load(字节) 直接加载到内存；如果被文件加载策略挡住（FileLoadException /
	/// BadImageFormatException），就把字节落到 %TEMP%\{资源名}\{简单名}.dll 再 Assembly.LoadFile。
	/// 也就是说：这个服务程序把真正的功能 DLL 藏在自身资源里，.NET 侧看不到普通文件依赖，
	/// 加壳器的"资源 + 动态加载"是它隐蔽性的一部分。缓存用 Hashtable，key 是带版本的全名。
	/// 注意：RSACryptoServiceProvider.UseMachineKeyStore = true 是加壳器为了能直接用
	/// 机器密钥库做 RSA 而设置的全局开关，与业务无关。
	/// </summary>
	internal class 内嵌程序集加载器
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00005850 File Offset: 0x00003A50
		/// <summary>
		/// 幂等地挂上 AssemblyResolve 事件（_WriterStrategy 标志保证只挂一次）。
		/// 本工程内没有调用点：调用它的入口（模块初始化器）在脱壳时已经被摘掉。
		/// </summary>
		internal static void 注册程序集解析()
		{
			if (!内嵌程序集加载器.已注册)
			{
				内嵌程序集加载器.已注册 = true;
				AppDomain 当前域 = AppDomain.CurrentDomain;
				当前域.AssemblyResolve += 内嵌程序集加载器.解析程序集加载;
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005884
		/// <summary>
		/// AssemblyResolve 回调：按程序集全名找对应的内嵌 DLL 并加载，找不到就返回 null
		/// （让 CLR 继续按自己的规则报错）。全过程持 Hashtable 锁，避免并发重复加载。
		/// 步骤：
		/// 1) 取请求全名（Trim）→ 查缓存，命中直接返回；
		/// 2) 未命中：打开机器密钥库开关 → 截出简单名 → 算资源名
		///    "b0494a1f-4bd3-" + Base64(MD5(Unicode(简单名)))；
		/// 3) 取该嵌入资源流，读出全部字节；
		/// 4) Assembly.Load(字节)；若抛 FileLoadException / BadImageFormatException，
		///    则把字节写到 %TEMP%\{资源名}\{简单名}.dll 再 Assembly.LoadFile；
		/// 5) 两种方式都把结果塞进缓存。
		/// 这里三层 try/catch 都是原件（壳的容错），全部保留，范围一律不动。
		/// </summary>
		private static Assembly 解析程序集加载(object 发送者, ResolveEventArgs 解析请求)
		{
			Assembly 结果程序集;
			lock (内嵌程序集加载器.已加载缓存)
			{
				string 请求全名 = 解析请求.Name.Trim();
				object 缓存项 = 内嵌程序集加载器.已加载缓存[请求全名];
				if (缓存项 == null)
				{
					try
					{
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string 简单名 = 内嵌程序集加载器.取简单名(请求全名);
						byte[] 名称字节 = Encoding.Unicode.GetBytes(简单名);
						string 资源名 = "b0494a1f-4bd3-" + Convert.ToBase64String(IntelliLock运行时.计算MD5(名称字节));
						Stream 资源流 = typeof(内嵌程序集加载器).Assembly.GetManifestResourceStream(资源名);
						if (资源流 != null)
						{
							try
							{
								BinaryReader 资源读取器 = new BinaryReader(资源流);
								资源读取器.BaseStream.Position = 0L;
								byte[] 程序集字节 = new byte[资源流.Length];
								资源读取器.Read(程序集字节, 0, 程序集字节.Length);
								资源读取器.Close();
								bool 需要落盘加载 = false;
								Assembly 已加载程序集 = null;
								try
								{
									已加载程序集 = Assembly.Load(程序集字节);
								}
								catch (FileLoadException)
								{
									需要落盘加载 = true;
								}
								catch (BadImageFormatException)
								{
									需要落盘加载 = true;
								}
								if (需要落盘加载)
								{
									string 临时目录 = Path.Combine(Path.GetTempPath(), 资源名);
									string 临时文件路径 = Path.Combine(临时目录, 简单名 + ".dll");
									if (!File.Exists(临时文件路径))
									{
										Directory.CreateDirectory(Path.GetDirectoryName(临时文件路径));
										FileStream 输出流 = new FileStream(临时文件路径, FileMode.Create, FileAccess.Write);
										输出流.Write(程序集字节, 0, 程序集字节.Length);
										输出流.Close();
									}
									已加载程序集 = Assembly.LoadFile(临时文件路径);
									内嵌程序集加载器.已加载缓存.Add(请求全名, 已加载程序集);
								}
								else
								{
									内嵌程序集加载器.已加载缓存.Add(请求全名, 已加载程序集);
								}
								return 已加载程序集;
							}
							catch
							{
							}
						}
					}
					catch
					{
					}
					结果程序集 = null;
				}
				else
				{
					结果程序集 = (Assembly)缓存项;
				}
			}
			return 结果程序集;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005A98 File Offset: 0x00003C98
		/// <summary>
		/// 从程序集全名里截出简单名：取第一个逗号之前的部分（没有逗号就整串返回）。
		/// 原名 IncludeParams。
		/// </summary>
		private static string 取简单名(string 程序集全名)
		{
			string 名称 = 程序集全名.Trim();
			int 逗号位置 = 名称.IndexOf(',');
			if (逗号位置 >= 0)
			{
				名称 = 名称.Substring(0, 逗号位置);
			}
			return 名称;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
		internal static bool RateDefinition()
		{
			return false;
		}

		// Token: 0x04000027 RID: 39
		/// <summary>已加载程序集缓存：key = 请求全名（带版本/公钥标记）。原名 productStrategy。</summary>
		private static Hashtable 已加载缓存 = new Hashtable();

		// Token: 0x04000028 RID: 40
		/// <summary>是否已经挂过 AssemblyResolve。原名 _WriterStrategy。</summary>
		private static bool 已注册 = false;
	}
}
