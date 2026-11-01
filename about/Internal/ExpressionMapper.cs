using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApplication1.Internal
{
	/// <summary>
	/// VB.NET 的 `My` 命名空间投影：把 `My.Computer` / `My.Application` / `My.User` /
	/// `My.Forms` / `My.WebServices` 五个静态入口按线程缓存各自的实例。
	/// 三个内部类分别负责窗体缓存（<see cref="窗体集合缓存"/>）、
	/// Web 服务缓存（<see cref="Web服务集合缓存"/>）和「懒加载单例」本身（<see cref="线程级单例{T}"/>）。
	/// 业务代码实际只用到 `My.Forms.Form1`（主窗体）与 `My.Application`（退出事件源）。
	/// </summary>
	// Token: 0x02000004 RID: 4
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[StandardModule]
	[HideModuleName]
	internal sealed class ExpressionMapper
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000021EB File Offset: 0x000003EB
		/// <summary>`My.Computer`。</summary>
		[HelpKeyword("My.Computer")]
		internal static MapperMapper SetComposer
		{
			get
			{
				return ExpressionMapper.计算机访问器.获取实例();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021F7 File Offset: 0x000003F7
		/// <summary>
		/// `My.Application`。除了给程序入口用 `Run()`，业务代码还通过它订阅 `Shutdown`
		/// （见 MySettings 的退出自动保存）。
		/// </summary>
		[HelpKeyword("My.Application")]
		internal static ComposerMapper CancelComposer
		{
			get
			{
				return ExpressionMapper.应用程序访问器.获取实例();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002203 File Offset: 0x00000403
		/// <summary>`My.User`（当前 Windows 身份）。本工程未使用。</summary>
		[HelpKeyword("My.User")]
		internal static User RateComposer
		{
			get
			{
				return ExpressionMapper.用户访问器.获取实例();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000220F File Offset: 0x0000040F
		/// <summary>
		/// `My.Forms`。主窗体就是从这里取的：`My.Forms.Form1` 首次访问时 new 出 Form1 并缓存，
		/// 之后返回同一实例（`IsSingleInstance = false` 只影响进程级多开，不影响这里的实例复用）。
		/// </summary>
		[HelpKeyword("My.Forms")]
		internal static ExpressionMapper.窗体集合缓存 VisitComposer
		{
			get
			{
				return ExpressionMapper.窗体访问器.获取实例();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000221B File Offset: 0x0000041B
		/// <summary>`My.WebServices`。本工程没有 Web 服务引用，该类型是空的。</summary>
		[HelpKeyword("My.WebServices")]
		internal static ExpressionMapper.Web服务集合缓存 CheckComposer
		{
			get
			{
				return ExpressionMapper.Web服务访问器.获取实例();
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>恒返回 false 的桩方法，无调用点。</summary>
		internal static bool 返回假占位()
		{
			return false;
		}

		// Token: 0x04000001 RID: 1
		/// <summary>`My.Computer` 的线程级缓存。</summary>
		private static readonly ExpressionMapper.线程级单例<MapperMapper> 计算机访问器 = new ExpressionMapper.线程级单例<MapperMapper>();

		// Token: 0x04000002 RID: 2
		/// <summary>`My.Application` 的线程级缓存。</summary>
		private static readonly ExpressionMapper.线程级单例<ComposerMapper> 应用程序访问器 = new ExpressionMapper.线程级单例<ComposerMapper>();

		// Token: 0x04000003 RID: 3
		/// <summary>`My.User` 的线程级缓存。</summary>
		private static readonly ExpressionMapper.线程级单例<User> 用户访问器 = new ExpressionMapper.线程级单例<User>();

		// Token: 0x04000004 RID: 4
		/// <summary>`My.Forms` 的线程级缓存（没有 readonly，模板原样）。</summary>
		private static ExpressionMapper.线程级单例<ExpressionMapper.窗体集合缓存> 窗体访问器 = new ExpressionMapper.线程级单例<ExpressionMapper.窗体集合缓存>();

		// Token: 0x04000005 RID: 5
		/// <summary>`My.WebServices` 的线程级缓存。</summary>
		private static readonly ExpressionMapper.线程级单例<ExpressionMapper.Web服务集合缓存> Web服务访问器 = new ExpressionMapper.线程级单例<ExpressionMapper.Web服务集合缓存>();

		// Token: 0x02000005 RID: 5
		/// <summary>
		/// `My.Forms` 的实现：按类型缓存窗体实例，并负责窗体的创建/销毁。
		/// 创建时用 `[ThreadStatic]` 的 Hashtable 记录「正在创建中的类型」，
		/// 用于检测构造函数里的递归创建（会抛 WinForms_RecursiveFormCreate）。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		internal sealed class 窗体集合缓存
		{
			// Token: 0x0600000F RID: 15 RVA: 0x00002644 File Offset: 0x00000844
			/// <summary>
			/// 取窗体实例：已存在且未释放就复用；否则标记「创建中」再 new。
			/// `TargetInvocationException`（构造函数内部抛异常）会被包成 InvalidOperationException 并带上内层消息。
			/// 无论成功失败都在 finally 里清掉「创建中」标记。
			/// </summary>
			private static T 获取或创建窗体<T>(T 窗体实例) where T : Form, new()
			{
				T 结果;
				if (窗体实例 != null && !窗体实例.IsDisposed)
				{
					结果 = 窗体实例;
				}
				else
				{
					if (ExpressionMapper.窗体集合缓存.正在创建的窗体 != null)
					{
						if (ExpressionMapper.窗体集合缓存.正在创建的窗体.ContainsKey(typeof(T)))
						{
							throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
						}
					}
					else
					{
						ExpressionMapper.窗体集合缓存.正在创建的窗体 = new Hashtable();
					}
					ExpressionMapper.窗体集合缓存.正在创建的窗体.Add(typeof(T), null);
					try
					{
						结果 = new T();
					}
					catch (TargetInvocationException ex) when (ex.InnerException != null)
					{
						throw new InvalidOperationException(Utils.GetResourceString("WinForms_SeeInnerException", new string[] { ex.InnerException.Message }), ex.InnerException);
					}
					finally
					{
						ExpressionMapper.窗体集合缓存.正在创建的窗体.Remove(typeof(T));
					}
				}
				return 结果;
			}

			// Token: 0x06000010 RID: 16 RVA: 0x00002227 File Offset: 0x00000427
			/// <summary>销毁窗体：Dispose 之后把引用置为 default（即 null），下次访问会重建。</summary>
			private void 销毁窗体<T>(ref T 窗体) where T : Form
			{
				窗体.Dispose();
				窗体 = default(T);
			}

			// Token: 0x06000011 RID: 17 RVA: 0x0000223C File Offset: 0x0000043C
			/// <summary>空构造函数（`My.Forms` 实例本身不需要状态）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public 窗体集合缓存()
			{
			}

			// Token: 0x06000012 RID: 18 RVA: 0x00002244 File Offset: 0x00000444
			/// <summary>
			/// 模板产物：`My.Forms` 的相等性判断。注意它调的是 `base.Equals` 且先过
			/// `RuntimeHelpers.GetObjectValue`，等价于引用比较，不是按内容比较。
			/// </summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000013 RID: 19 RVA: 0x00002252 File Offset: 0x00000452
			/// <summary>模板产物：直接转发到 `base.GetHashCode()`。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000014 RID: 20 RVA: 0x0000225A File Offset: 0x0000045A
			/// <summary>模板产物：返回本类型的 Type。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type 获取类型()
			{
				return typeof(ExpressionMapper.窗体集合缓存);
			}

			// Token: 0x06000015 RID: 21 RVA: 0x00002266 File Offset: 0x00000466
			/// <summary>模板产物：转发到 `base.ToString()`。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x06000016 RID: 22 RVA: 0x0000226E File Offset: 0x0000046E
			// (set) Token: 0x06000018 RID: 24 RVA: 0x00002748 File: 0x00000948
			/// <summary>
			/// `My.Forms.Form1`：主窗体入口。取的时候走「缓存或新建」，
			/// 赋值只允许赋 null（赋非 null 直接抛 ArgumentException），这是 VB 模板的固定行为。
			/// </summary>
			public Form1 Form1
			{
				get
				{
					this.缓存的Form1 = ExpressionMapper.窗体集合缓存.获取或创建窗体<Form1>(this.缓存的Form1);
					return this.缓存的Form1;
				}
				set
				{
					if (value != this.缓存的Form1)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.销毁窗体<Form1>(ref this.缓存的Form1);
					}
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000017 RID: 23 RVA: 0x00002287 File: 0x00000487
			// (set) Token: 0x06000019 RID: 25 RVA: 0x00002778 File: 0x00000978
			/// <summary>
			/// `My.Forms.Form3`：退出密码窗体。只有用户在「退出程序」菜单里通过了注册表里的密码判断，
			/// Form1 才会去 `Show()` 它。同样只允许赋 null。
			/// </summary>
			public Form3 Form3
			{
				get
				{
					this.缓存的Form3 = ExpressionMapper.窗体集合缓存.获取或创建窗体<Form3>(this.缓存的Form3);
					return this.缓存的Form3;
				}
				set
				{
					if (value != this.缓存的Form3)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.销毁窗体<Form3>(ref this.缓存的Form3);
					}
				}
			}

			// Token: 0x04000006 RID: 6
			/// <summary>正在创建中的窗体类型集合（线程静态），用于检测递归创建。</summary>
			[ThreadStatic]
			private static Hashtable 正在创建的窗体;

			// Token: 0x04000007 RID: 7
			/// <summary>Form1 实例缓存。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form1 缓存的Form1;

			// Token: 0x04000008 RID: 8
			/// <summary>Form3 实例缓存。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form3 缓存的Form3;
		}

		// Token: 0x02000006 RID: 6
		/// <summary>
		/// `My.WebServices` 的实现。本工程没有引用任何 Web 服务，因此除了模板自带的
		/// 相等性/哈希/ToString 转发和一个「取或建」泛型方法外没有实际成员。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		internal sealed class Web服务集合缓存
		{
			// Token: 0x0600001A RID: 26 RVA: 0x00002244 File Offset: 0x00000444
			/// <summary>模板产物：`base.Equals` + `RuntimeHelpers.GetObjectValue`，等价引用比较。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600001B RID: 27 RVA: 0x00002252 File Offset: 0x00000452
			/// <summary>模板产物：直接转发到 `base.GetHashCode()`。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x0600001C RID: 28 RVA: 0x000022A0 File Offset: 0x000004A0
			/// <summary>模板产物：返回本类型的 Type。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type 获取类型()
			{
				return typeof(ExpressionMapper.Web服务集合缓存);
			}

			// Token: 0x0600001D RID: 29 RVA: 0x00002266 File Offset: 0x00000466
			/// <summary>模板产物：转发到 `base.ToString()`。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x0600001E RID: 30 RVA: 0x000027A8 File Offset: 0x000009A8
			/// <summary>取或建：实例为空时 new 一个，否则原样返回。</summary>
			private static T 获取或创建服务<T>(T 服务实例) where T : new()
			{
				T 结果;
				if (服务实例 == null)
				{
					结果 = new T();
				}
				else
				{
					结果 = 服务实例;
				}
				return 结果;
			}

			// Token: 0x0600001F RID: 31 RVA: 0x000022AC File Offset: 0x000004AC
			/// <summary>销毁：把引用置为 default（即 null）。</summary>
			private void 清空服务实例<T>(ref T 服务实例)
			{
				服务实例 = default(T);
			}

			// Token: 0x06000020 RID: 32 RVA: 0x0000223C File Offset: 0x0000043C
			/// <summary>空构造函数。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Web服务集合缓存()
			{
			}

			// Token: 0x06000021 RID: 33 RVA: 0x000021DD File Offset: 0x000003DD
			/// <summary>恒返回 true 的桩方法，无调用点。</summary>
			internal static bool 返回真占位()
			{
				return true;
			}

			// Token: 0x06000022 RID: 34 RVA: 0x000021E8 File Offset: 0x000003E8
			/// <summary>恒返回 false 的桩方法，无调用点。</summary>
			internal static bool 返回假占位()
			{
				return false;
			}
		}

		// Token: 0x02000007 RID: 7
		/// <summary>
		/// `My` 各路入口用的懒加载单例：每个线程（字段带 `[ThreadStatic]`）第一次访问时
		/// new 一个 T，之后复用。注意**没有锁**，同一线程内并发访问同一入口理论上可能建两个实例。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ComVisible(false)]
		internal sealed class 线程级单例<T> where T : new()
		{
			// Token: 0x06000023 RID: 35 RVA: 0x000022B5 File Offset: 0x000004B5
			/// <summary>取当前线程的实例：为空则 new，否则直接返回。</summary>
			internal T 获取实例()
			{
				if (ExpressionMapper.线程级单例<T>.实例 == null)
				{
					ExpressionMapper.线程级单例<T>.实例 = new T();
				}
				return ExpressionMapper.线程级单例<T>.实例;
			}

			// Token: 0x06000024 RID: 36 RVA: 0x0000223C File Offset: 0x0000043C
			/// <summary>空构造函数。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public 线程级单例()
			{
			}

			// Token: 0x06000025 RID: 37 RVA: 0x000021DD File Offset: 0x000003DD
			/// <summary>恒返回 true 的桩方法，无调用点。</summary>
			internal static bool 返回真占位()
			{
				return true;
			}

			// Token: 0x06000026 RID: 38 RVA: 0x000021E8 File Offset: 0x000003E8
			/// <summary>恒返回 false 的桩方法，无调用点。</summary>
			internal static bool 返回假占位()
			{
				return false;
			}

			// Token: 0x04000009 RID: 9
			/// <summary>当前线程持有的实例。</summary>
			[CompilerGenerated]
			[ThreadStatic]
			private static T 实例;
		}
	}
}
