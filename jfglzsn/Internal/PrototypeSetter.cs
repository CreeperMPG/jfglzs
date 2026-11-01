using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace jfglzsn.Internal
{
	// Token: 0x0200001A RID: 26
	/// <summary>
	/// 内嵌程序集加载器（壳的「AssemblyResolve 钩子」）。
	/// <para>
	/// 加壳样本会把依赖程序集加密后塞进自身资源；本类在 <see cref="AppDomain.AssemblyResolve"/> 事件上挂一个回调，
	/// 运行时按请求名算出资源名（<c>"b0494a1f-4bd3-" + Base64(哈希)</c>），从自身清单资源里取出密文、
	/// 在内存里 <see cref="Assembly.Load(byte[])"/>；若内存加载失败（<see cref="FileLoadException"/> /
	/// <see cref="BadImageFormatException"/>），则把解密后的 DLL 落盘到
	/// <c>%TEMP%\b0494a1f-4bd3-&lt;Base64&gt;\&lt;程序集简单名&gt;.dll</c> 再 <see cref="Assembly.LoadFile(string)"/>。
	/// </para>
	/// <para>
	/// 敏感点：这是典型的**磁盘落盘式载荷释放**——会在临时目录创建 UUID 风格子目录并写入 DLL。
	/// 落盘文件名/目录名由 <c>b0494a1f-4bd3-</c> 前缀加哈希 Base64 决定，是本样本的稳定 IOC。
	/// 哈希算法由 <see cref="ListSetter.计算MD5"/> 提供（对 <c>Encoding.Unicode</c> 编码的字节）。
	/// </para>
	/// <para>
	/// 本类没有任何地方被调用（<see cref="注册程序集解析器"/> 在工程内无调用点），
	/// 说明这段加载链在样本中已失效或由壳的自定义入口单独启动。
	/// </para>
	/// </summary>
	internal class PrototypeSetter
	{
		// Token: 0x06000106 RID: 262 RVA: 0x0000C3A0 File Offset: 0x0000A5A0
		/// <summary>
		/// 只挂一次 <see cref="AppDomain.AssemblyResolve"/> 钩子；标志位先置真再挂接，
		/// 与原版一致（挂接抛异常时也不会重试）。
		/// </summary>
		internal static void 注册程序集解析器()
		{
			if (!PrototypeSetter.已注册标志)
			{
				PrototypeSetter.已注册标志 = true;
				AppDomain currentDomain = AppDomain.CurrentDomain;
				currentDomain.AssemblyResolve += PrototypeSetter.解析程序集回调;
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000C3D4 File Offset: 0x0000C3D4
		/// <summary>
		/// 程序集解析回调：先查内存缓存，未命中则按请求名定位内嵌资源、解密并加载，
		/// 加载结果（含 <c>null</c>）不予缓存——只有真正拿到程序集才写入缓存表。
		/// <para>
		/// 锁的覆盖范围与原版一致：缓存查找、解密、落盘、装载全在 <c>lock</c> 内，串行化整个解析过程。
		/// 两处 <c>catch { }</c> 分别吞掉「资源读取/装载失败」和「整体异常」，
		/// 最终返回 <c>null</c> 让运行时继续走默认探测——不要删这两个空 catch。
		/// </para>
		/// </summary>
		/// <param name="发送方">事件源，未使用。</param>
		/// <param name="解析参数">包含请求的程序集全名（<c>Name, Version=...</c>）。</param>
		/// <returns>解密得到的程序集；失败时为 <c>null</c>。</returns>
		private static Assembly 解析程序集回调(object 发送方, ResolveEventArgs 解析参数)
		{
			Assembly 结果;
			lock (PrototypeSetter.已加载程序集缓存)
			{
				string 请求全名 = 解析参数.Name.Trim();
				object 缓存项 = PrototypeSetter.已加载程序集缓存[请求全名];
				if (缓存项 == null)
				{
					try
					{
						// 让 RSA 使用机器级密钥容器（壳生成 / 校验资源时的环境要求）
						RSACryptoServiceProvider.UseMachineKeyStore = true;
						string 程序集简单名 = PrototypeSetter.取程序集简单名(请求全名);
						byte[] 请求名Unicode字节 = Encoding.Unicode.GetBytes(程序集简单名);
						// 资源名 = 固定前缀 + 哈希的 Base64；同一字符串后面还当临时子目录名用
						string 资源名 = "b0494a1f-4bd3-" + Convert.ToBase64String(ListSetter.计算MD5(请求名Unicode字节));
						Stream 资源流 = typeof(PrototypeSetter).Assembly.GetManifestResourceStream(资源名);
						if (资源流 != null)
						{
							try
							{
								BinaryReader 读取器 = new BinaryReader(资源流);
								读取器.BaseStream.Position = 0L;
								byte[] 密文 = new byte[资源流.Length];
								读取器.Read(密文, 0, 密文.Length);
								读取器.Close();
								bool 需要落盘 = false;
								Assembly 已加载程序集 = null;
								try
								{
									// 首选：纯内存装载（不碰磁盘）
									已加载程序集 = Assembly.Load(密文);
								}
								catch (FileLoadException)
								{
									需要落盘 = true;
								}
								catch (BadImageFormatException)
								{
									需要落盘 = true;
								}
								if (需要落盘)
								{
									// 兜底：落到 %TEMP%\<资源名>\<程序集简单名>.dll 再从文件装载
									string 临时目录 = Path.Combine(Path.GetTempPath(), 资源名);
									string 落盘路径 = Path.Combine(临时目录, 程序集简单名 + ".dll");
									if (!File.Exists(落盘路径))
									{
										Directory.CreateDirectory(Path.GetDirectoryName(落盘路径));
										FileStream 写句柄 = new FileStream(落盘路径, FileMode.Create, FileAccess.Write);
										写句柄.Write(密文, 0, 密文.Length);
										写句柄.Close();
									}
									已加载程序集 = Assembly.LoadFile(落盘路径);
									PrototypeSetter.已加载程序集缓存.Add(请求全名, 已加载程序集);
								}
								else
								{
									PrototypeSetter.已加载程序集缓存.Add(请求全名, 已加载程序集);
								}
								return 已加载程序集;
							}
							catch
							{
								// 原版空 catch：资源读取或装载失败时静默放弃
							}
						}
					}
					catch
					{
						// 原版空 catch：解密/命名为空等任何异常都静默放弃
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

		// Token: 0x06000108 RID: 264 RVA: 0x0000C5E8 File Offset: 0x0000A7E8
		/// <summary>
		/// 从程序集全名里截出简单名：取第一个逗号之前的部分（无逗号则原样返回）。
		/// 例：<c>"Foo, Version=1.0.0.0, Culture=neutral"</c> → <c>"Foo"</c>。
		/// </summary>
		/// <param name="程序集全名">程序集全名。</param>
		/// <returns>程序集简单名。</returns>
		private static string 取程序集简单名(string 程序集全名)
		{
			string 名字 = 程序集全名.Trim();
			int 逗号位置 = 名字.IndexOf(',');
			if (逗号位置 >= 0)
			{
				名字 = 名字.Substring(0, 逗号位置);
			}
			return 名字;
		}

		// Token: 0x040000C3 RID: 195
		/// <summary>请求全名 → 已加载程序集 的缓存表（Hashtable 本身非泛型，需注意装箱）。</summary>
		private static Hashtable 已加载程序集缓存 = new Hashtable();

		// Token: 0x040000C4 RID: 196
		/// <summary>是否已挂接 AssemblyResolve 事件。</summary>
		private static bool 已注册标志 = false;
	}
}
