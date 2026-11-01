using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace set.Internal
{
	/// <summary>
	/// .NET Reactor 的「内嵌程序集按需加载器」（AssemblyResolve 钩子），是解壳后必须保留的关键运行期组件。
	///
	/// 工作原理：程序集被 Reactor 合并/加密后并以清单资源（manifest resource）的形式藏在主程序里，
	/// 资源名 = <c>"b0494a1f-4bd3-" + Base64( CollectionAttr.SetAttr( Unicode(程序集简单名) ) )</c>。
	/// 当 CLR 需要某个依赖程序集、而磁盘上找不到时，本类的 <see cref="解析内嵌程序集"/> 会被调用，
	/// 它用同样的算法算出资源名、读出资源字节、<see cref="Assembly.Load(byte[])"/> 直接加载；
	/// 若内存加载失败（FileLoadException / BadImageFormatException），就把字节写到
	/// <c>%TEMP%\&lt;资源名&gt;\&lt;程序集简单名&gt;.dll</c> 再从磁盘 LoadFile。
	/// </summary>
	// Token: 0x02000018 RID: 24
	internal class CandidateCandidate
	{
		/// <summary>
		/// 挂载 AssemblyResolve 钩子（只挂一次，用 <see cref="已注册"/> 防重入）。
		/// 由 Reactor 生成的初始化代码在启动早期调用。
		/// </summary>
		// Token: 0x06000129 RID: 297 RVA: 0x0000E450 File Offset: 0x0000C650
		internal static void 注册程序集解析器()
		{
			if (!CandidateCandidate.已注册)
			{
				CandidateCandidate.已注册 = true;
				AppDomain currentDomain = AppDomain.CurrentDomain;
				currentDomain.AssemblyResolve += CandidateCandidate.解析内嵌程序集;
			}
		}

		/// <summary>
		/// AssemblyResolve 事件处理：按需求名算出内嵌资源名 -> 读出字节 -> 加载程序集。
		/// 全过程失败时吞掉异常并返回 null（CLR 随后报常规的「找不到程序集」）。
		/// </summary>
		/// <param name="发送者">事件源（AppDomain），未使用。</param>
		/// <param name="解析事件参数">包含 <c>Name</c>（带版本/公钥标记的完整程序集名）。</param>
		// Token: 0x0600012A RID: 298 RVA: 0x0000E484 File Offset: 0x0000C684
		private static Assembly 解析内嵌程序集(object 发送者, ResolveEventArgs 解析事件参数)
		{
			Assembly 结果;
			// 缓存表不是线程安全的 Hashtable，这里用对象自身做锁
			lock (CandidateCandidate.已加载程序集缓存)
			{
				string 程序集全名 = 解析事件参数.Name.Trim();
				object 缓存项 = CandidateCandidate.已加载程序集缓存[程序集全名];
				if (缓存项 == null)
				{
					try
					{
						// 让 RSA 使用机器级密钥容器（Reactor 的既有行为）
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						// 只取简单名：逗号后的版本/区域/公钥一律丢掉
						string 程序集简单名 = CandidateCandidate.取程序集简单名(程序集全名);
						byte[] 名称字节 = Encoding.Unicode.GetBytes(程序集简单名);
						// 资源名 = 固定前缀 + Base64( 自定义摘要(UTF-16 编码的简单名) )
						string 资源名 = "b0494a1f-4bd3-" + Convert.ToBase64String(CollectionAttr.计算摘要(名称字节));
						Stream 资源流 = typeof(CandidateCandidate).Assembly.GetManifestResourceStream(资源名);
						if (资源流 != null)
						{
							try
							{
								BinaryReader 读取器 = new BinaryReader(资源流);
								读取器.BaseStream.Position = 0L;
								byte[] 资源字节 = new byte[资源流.Length];
								读取器.Read(资源字节, 0, 资源字节.Length);
								读取器.Close();
								bool 需落盘 = false;
								Assembly 程序集 = null;
								try
								{
									// 优先内存加载，不落盘
									程序集 = Assembly.Load(资源字节);
								}
								catch (FileLoadException)
								{
									需落盘 = true;
								}
								catch (BadImageFormatException)
								{
									需落盘 = true;
								}
								if (需落盘)
								{
									// 内存加载失败：写到 %TEMP%\<资源名>\<简单名>.dll 再从磁盘加载
									string 临时目录 = Path.Combine(Path.GetTempPath(), 资源名);
									string 临时文件 = Path.Combine(临时目录, 程序集简单名 + ".dll");
									if (!File.Exists(临时文件))
									{
										Directory.CreateDirectory(Path.GetDirectoryName(临时文件));
										FileStream 文件流 = new FileStream(临时文件, FileMode.Create, FileAccess.Write);
										文件流.Write(资源字节, 0, 资源字节.Length);
										文件流.Close();
									}
									程序集 = Assembly.LoadFile(临时文件);
									CandidateCandidate.已加载程序集缓存.Add(程序集全名, 程序集);
								}
								else
								{
									CandidateCandidate.已加载程序集缓存.Add(程序集全名, 程序集);
								}
								return 程序集;
							}
							catch
							{
								// 读资源/加载过程中的任何异常都被吞掉（原版即如此）
							}
						}
					}
					catch
					{
						// 算摘要、找资源阶段的任何异常同样被吞掉
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

		/// <summary>
		/// 从「SimpleName, Version=..., Culture=..., PublicKeyToken=...」里截出逗号前的简单名。
		/// 没有逗号时原样返回（已 Trim）。
		/// </summary>
		/// <param name="程序集全名">AssemblyResolve 给出的程序集名。</param>
		// Token: 0x0600012B RID: 299 RVA: 0x0000E698 File Offset: 0x0000C898
		private static string 取程序集简单名(string 程序集全名)
		{
			string 文本 = 程序集全名.Trim();
			int 逗号位置 = 文本.IndexOf(',');
			if (逗号位置 >= 0)
			{
				文本 = 文本.Substring(0, 逗号位置);
			}
			return 文本;
		}

		/// <summary>已解析程序集的缓存：程序集全名 -> Assembly。静态、无锁，靠 <see cref="解析内嵌程序集"/> 里的 lock 保护。</summary>
		// Token: 0x04000086 RID: 134
		private static Hashtable 已加载程序集缓存 = new Hashtable();

		/// <summary>AssemblyResolve 钩子是否已注册（防止重复订阅）。</summary>
		// Token: 0x04000087 RID: 135
		private static bool 已注册 = false;
	}
}
