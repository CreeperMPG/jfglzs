using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace WindowsApplication1.Internal
{
	// Token: 0x02000017 RID: 23
	/// <summary>
	/// .NET Reactor 的「内嵌依赖程序集」加载器：把依赖 DLL 作为资源嵌进主程序，
	/// 运行时挂到 AppDomain.AssemblyResolve 上，需要时才从资源里取出来 Assembly.Load。
	///
	/// 资源名不是写死的：程序集短名 → UTF-16 字节 → 计算哈希 → Base64 → 拼成
	/// "b0494a1f-4bd3-{Base64}"，所以谁能反算出这个哈希，谁就能定位内嵌的 DLL。
	///
	/// 安全相关的副作用：
	/// 1) 把 RSA 密钥容器指向「机器级」存储（RSACryptoServiceProvider.UseMachineKeyStore = true）；
	/// 2) 内存加载失败时会把 DLL 明文落到 %TEMP%\{资源名}\{短名}.dll 再从磁盘 LoadFile，
	///    也就是会在磁盘上留下解包后的二进制。
	///
	/// 原混淆名：AttributeConfig
	/// </summary>
	internal class 内嵌程序集解析器
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00007BF4 File Offset: 0x00005DF4
		/// <summary>
		/// 幂等注册：只把本类的 程序集解析回调 挂到当前 AppDomain 的 AssemblyResolve 上，重复调用无副作用。
		/// 原混淆名：smethod_0
		/// </summary>
		internal static void 注册程序集解析回调()
		{
			if (!内嵌程序集解析器.已注册回调)
			{
				内嵌程序集解析器.已注册回调 = true;
				AppDomain 当前域 = AppDomain.CurrentDomain;
				当前域.AssemblyResolve += 内嵌程序集解析器.程序集解析回调;
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00007C28 File Offset: 0x00006000
		/// <summary>
		/// 程序集解析回调：按需从内嵌资源里把目标 DLL 加载出来，成功则返回，失败返回 null
		/// （返回 null 表示「本解析器不认识这个程序集」，交给 CLR 继续按默认规则找）。
		/// 命中缓存时直接返回，不重复解密。整个过程在 程序集缓存 上加了锁。
		/// 原混淆名：PrepareConfig
		/// </summary>
		private static Assembly 程序集解析回调(object 发送者, ResolveEventArgs 解析参数)
		{
			Assembly 结果;
			lock (内嵌程序集解析器.程序集缓存)
			{
				string 程序集全名 = 解析参数.Name.Trim();
				object 缓存命中 = 内嵌程序集解析器.程序集缓存[程序集全名];
				if (缓存命中 == null)
				{
					try
					{
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string 程序集短名 = 内嵌程序集解析器.取程序集短名(程序集全名);
						byte[] 短名字节 = Encoding.Unicode.GetBytes(程序集短名);
						string 资源名 = "b0494a1f-4bd3-" + Convert.ToBase64String(IndexerConfig.计算哈希(短名字节));
						Stream 资源流 = typeof(内嵌程序集解析器).Assembly.GetManifestResourceStream(资源名);
						if (资源流 != null)
						{
							try
							{
								BinaryReader 读取器 = new BinaryReader(资源流);
								读取器.BaseStream.Position = 0L;
								byte[] 程序集字节 = new byte[资源流.Length];
								读取器.Read(程序集字节, 0, 程序集字节.Length);
								读取器.Close();
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
									string 临时文件 = Path.Combine(临时目录, 程序集短名 + ".dll");
									if (!File.Exists(临时文件))
									{
										Directory.CreateDirectory(Path.GetDirectoryName(临时文件));
										FileStream 写出流 = new FileStream(临时文件, FileMode.Create, FileAccess.Write);
										写出流.Write(程序集字节, 0, 程序集字节.Length);
										写出流.Close();
									}
									已加载程序集 = Assembly.LoadFile(临时文件);
									内嵌程序集解析器.程序集缓存.Add(程序集全名, 已加载程序集);
								}
								else
								{
									内嵌程序集解析器.程序集缓存.Add(程序集全名, 已加载程序集);
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
					结果 = null;
				}
				else
				{
					结果 = (Assembly)缓存命中;
				}
			}
			return 结果;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00007E3C File Offset: 0x0000603C
		/// <summary>
		/// 从程序集全名里取短名：去掉逗号后面的 Version/Culture/PublicKeyToken。
		/// 注意用的是 IndexOf(',') + Substring(0, idx)，没有逗号就原样返回（已 Trim）。
		/// 原混淆名：ConnectConfig
		/// </summary>
		private static string 取程序集短名(string 程序集全名)
		{
			string 文本 = 程序集全名.Trim();
			int 逗号位置 = 文本.IndexOf(',');
			if (逗号位置 >= 0)
			{
				文本 = 文本.Substring(0, 逗号位置);
			}
			return 文本;
		}

		// Token: 0x04000042 RID: 66
		/// <summary>已加载的内嵌程序集缓存：程序集全名 → Assembly。</summary>
		private static Hashtable 程序集缓存 = new Hashtable();

		// Token: 0x04000043 RID: 67
		/// <summary>是否已挂过 AssemblyResolve（保证只挂一次）。</summary>
		private static bool 已注册回调 = false;
	}
}
