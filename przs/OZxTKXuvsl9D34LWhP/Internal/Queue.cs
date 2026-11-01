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

namespace OZxTKXuvsl9D34LWhP.Internal
{
	/// <summary>
	/// VB 的 <c>My</c> 命名空间聚合器（类名 Queue 是混淆名，原类型名应为 <c>MyProject</c>）。
	/// 它本身没有业务逻辑，只负责把"全局单例"挂出来：
	/// <list type="bullet">
	/// <item><see cref="MyComputer"/> = <c>My.Computer</c>（类型 <see cref="Watcher"/>）</item>
	/// <item><see cref="MyApplication"/> = <c>My.Application</c>（类型 <see cref="Val"/>，也是进程入口）</item>
	/// <item><see cref="MyUser"/> = <c>My.User</c></item>
	/// <item><see cref="MyForms"/> = <c>My.Forms</c>（<see cref="FormCollection"/>，本项目里被用得最多：
	///   Form1 通过 <c>Queue.MyForms.Form2</c> 之类的写法操作其它窗体）</item>
	/// <item><see cref="MyWebServices"/> = <c>My.WebServices</c>（本项目没用）</item>
	/// </list>
	/// 内部的 <see cref="LazySingleton{T}"/> 是 VB 生成的延迟单例容器；<see cref="FormCollection"/>
	/// 是 VB 的"窗体组"实现：第一次访问某窗体时创建，赋 null 时释放。
	/// 各 <c>XxxTag()</c> 方法是 .NET Reactor 塞进来的授权标记桩，恒返回 true/false，无调用者。
	/// </summary>
	// Token: 0x02000004 RID: 4
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[HideModuleName]
	[StandardModule]
	internal sealed class Queue
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000021EB File Offset: 0x000003EB
		/// <summary><c>My.Computer</c> 单例（原名 ConcatVal）。</summary>
		[HelpKeyword("My.Computer")]
		internal static Watcher MyComputer
		{
			get
			{
				return Queue.计算机单例.ResolveVal();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021F7 File Offset: 0x000003F7
		/// <summary><c>My.Application</c> 单例（原名 ResetVal）。</summary>
		[HelpKeyword("My.Application")]
		internal static Val MyApplication
		{
			get
			{
				return Queue.应用程序单例.ResolveVal();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002203 File Offset: 0x00000403
		/// <summary><c>My.User</c> 单例（原名 OrderVal）。</summary>
		[HelpKeyword("My.User")]
		internal static User MyUser
		{
			get
			{
				return Queue.用户单例.ResolveVal();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000220F File Offset: 0x0000040F
		/// <summary><c>My.Forms</c> 单例（原名 CollectVal）——本项目里窗体之间互相调用全靠它。</summary>
		[HelpKeyword("My.Forms")]
		internal static Queue.FormCollection MyForms
		{
			get
			{
				return Queue.窗体集合单例.ResolveVal();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000221B File Offset: 0x0000041B
		/// <summary><c>My.WebServices</c> 单例（原名 IncludeVal），本项目未使用。</summary>
		[HelpKeyword("My.WebServices")]
		internal static Queue.WebServiceCollection MyWebServices
		{
			get
			{
				return Queue.Web服务单例.ResolveVal();
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>.NET Reactor 生成的授权标记桩，恒返回 true，无调用者。</summary>
		internal static bool PushTag()
		{
			return true;
		}

		// Token: 0x04000001 RID: 1
		private static readonly Queue.LazySingleton<Watcher> 计算机单例 = new Queue.LazySingleton<Watcher>();

		// Token: 0x04000002 RID: 2
		private static readonly Queue.LazySingleton<Val> 应用程序单例 = new Queue.LazySingleton<Val>();

		// Token: 0x04000003 RID: 3
		private static readonly Queue.LazySingleton<User> 用户单例 = new Queue.LazySingleton<User>();

		// Token: 0x04000004 RID: 4
		private static Queue.LazySingleton<Queue.FormCollection> 窗体集合单例 = new Queue.LazySingleton<Queue.FormCollection>();

		// Token: 0x04000005 RID: 5
		private static readonly Queue.LazySingleton<Queue.WebServiceCollection> Web服务单例 = new Queue.LazySingleton<Queue.WebServiceCollection>();

		// Token: 0x02000005 RID: 5
		/// <summary>
		/// VB 的 <c>My.Forms</c> 窗体组（原名 Adapter）：每个属性第一次被读时才创建窗体，
		/// 赋 null 表示释放；用 [ThreadStatic] 的 <see cref="正在创建的窗体表"/> 检测"窗体 A 的构造函数
		/// 里又去访问窗体 A"的递归创建，命中时抛 InvalidOperationException。
		/// </summary>
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class FormCollection
		{
			// Token: 0x0600000F RID: 15 RVA: 0x00002724
			/// <summary>
			/// 取已有窗体，没有就新建（原名 AwakeVal）。构造函数抛异常时包装成
			/// InvalidOperationException 并带上内部异常；无论成功失败都从"创建中"表里移除类型。
			/// </summary>
			private static T 获取或创建窗体<T>(T 现有实例) where T : Form, new()
			{
				T 结果;
				if (现有实例 != null && !现有实例.IsDisposed)
				{
					结果 = 现有实例;
				}
				else
				{
					if (Queue.FormCollection.正在创建的窗体表 != null)
					{
						if (Queue.FormCollection.正在创建的窗体表.ContainsKey(typeof(T)))
						{
							throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
						}
					}
					else
					{
						Queue.FormCollection.正在创建的窗体表 = new Hashtable();
					}
					Queue.FormCollection.正在创建的窗体表.Add(typeof(T), null);
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
						Queue.FormCollection.正在创建的窗体表.Remove(typeof(T));
					}
				}
				return 结果;
			}

			// Token: 0x06000010 RID: 16 RVA: 0x00002227
			/// <summary>释放窗体并清空引用（原名 LoginVal）。</summary>
			private void 释放窗体<T>(ref T 窗体引用) where T : Form
			{
				窗体引用.Dispose();
				窗体引用 = default(T);
			}

			// Token: 0x06000011 RID: 17 RVA: 0x0000223C
			[EditorBrowsable(EditorBrowsableState.Never)]
			public FormCollection()
			{
			}

			// Token: 0x06000012 RID: 18 RVA: 0x00002244
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000013 RID: 19 RVA: 0x00002252
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000014 RID: 20 RVA: 0x0000225A
			/// <summary>返回本类型（原名 PrintVal），VB 生成的样板。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type PrintVal()
			{
				return typeof(Queue.FormCollection);
			}

			// Token: 0x06000015 RID: 21 RVA: 0x00002266
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x06000016 RID: 22 RVA: 0x0000226E
			// (set) Token: 0x0600001B RID: 27 RVA: 0x00002828
			/// <summary>右下角"编号"小窗（类型 <see cref="bianhao"/>），属性名与类型名相同是 VB 的习惯。</summary>
			public bianhao bianhao
			{
				get
				{
					this.编号窗体缓存 = Queue.FormCollection.获取或创建窗体<bianhao>(this.编号窗体缓存);
					return this.编号窗体缓存;
				}
				set
				{
					if (value != this.编号窗体缓存)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体<bianhao>(ref this.编号窗体缓存);
					}
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000017 RID: 23 RVA: 0x00002287
			// (set) Token: 0x0600001C RID: 28 RVA: 0x0000285C
			/// <summary>主窗体（隐藏运行的管控核心）。</summary>
			public Form1 Form1
			{
				get
				{
					this.主窗体缓存 = Queue.FormCollection.获取或创建窗体<Form1>(this.主窗体缓存);
					return this.主窗体缓存;
				}
				set
				{
					if (value != this.主窗体缓存)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体<Form1>(ref this.主窗体缓存);
					}
				}
			}

			// Token: 0x17000008 RID: 8
			// (get) Token: 0x06000018 RID: 24 RVA: 0x000022A0
			// (set) Token: 0x0600001D RID: 29 RVA: 0x00002890
			/// <summary>全屏警告/锁屏窗（要密码才能退出的那个）。</summary>
			public Form2 Form2
			{
				get
				{
					this.警告窗体缓存 = Queue.FormCollection.获取或创建窗体<Form2>(this.警告窗体缓存);
					return this.警告窗体缓存;
				}
				set
				{
					if (value != this.警告窗体缓存)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体<Form2>(ref this.警告窗体缓存);
					}
				}
			}

			// Token: 0x17000009 RID: 9
			// (get) Token: 0x06000019 RID: 25 RVA: 0x000022B9
			// (set) Token: 0x0600001E RID: 30 RVA: 0x000028C0
			/// <summary>"提醒"三行红字窗。</summary>
			public Form3 Form3
			{
				get
				{
					this.提醒窗体缓存 = Queue.FormCollection.获取或创建窗体<Form3>(this.提醒窗体缓存);
					return this.提醒窗体缓存;
				}
				set
				{
					if (value != this.提醒窗体缓存)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体<Form3>(ref this.提醒窗体缓存);
					}
				}
			}

			// Token: 0x1700000A RID: 10
			// (get) Token: 0x0600001A RID: 26 RVA: 0x000022D2
			// (set) Token: 0x0600001F RID: 31 RVA: 0x000028F0
			/// <summary>全屏锁屏窗（剪鼠标 + 蜂鸣）。</summary>
			public Form4 Form4
			{
				get
				{
					this.锁屏窗体缓存 = Queue.FormCollection.获取或创建窗体<Form4>(this.锁屏窗体缓存);
					return this.锁屏窗体缓存;
				}
				set
				{
					if (value != this.锁屏窗体缓存)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体<Form4>(ref this.锁屏窗体缓存);
					}
				}
			}

			// Token: 0x04000006 RID: 6
			/// <summary>正在创建中的窗体类型表，用来检测递归创建（原名 m_Producer）。</summary>
			[ThreadStatic]
			private static Hashtable 正在创建的窗体表;

			// Token: 0x04000007 RID: 7
			[EditorBrowsable(EditorBrowsableState.Never)]
			public bianhao 编号窗体缓存;

			// Token: 0x04000008 RID: 8
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form1 主窗体缓存;

			// Token: 0x04000009 RID: 9
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form2 警告窗体缓存;

			// Token: 0x0400000A RID: 10
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form3 提醒窗体缓存;

			// Token: 0x0400000B RID: 11
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form4 锁屏窗体缓存;
		}

		// Token: 0x02000006 RID: 6
		/// <summary>VB 的 <c>My.WebServices</c> 组（原名 Order），本项目没有任何 Web 服务，纯样板。</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		internal sealed class WebServiceCollection
		{
			// Token: 0x06000020 RID: 32
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000021 RID: 33
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000022 RID: 34
			/// <summary>返回本类型（原名 FillVal），VB 生成的样板。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type FillVal()
			{
				return typeof(Queue.WebServiceCollection);
			}

			// Token: 0x06000023 RID: 35
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x06000024 RID: 36
			/// <summary>没有实例就新建（原名 DefineVal）。</summary>
			private static T 取实例或新建<T>(T 实例) where T : new()
			{
				T 结果;
				if (实例 == null)
				{
					结果 = new T();
				}
				else
				{
					结果 = 实例;
				}
				return 结果;
			}

			// Token: 0x06000025 RID: 37
			/// <summary>清空引用（原名 RemoveVal）。</summary>
			private void RemoveVal<T>(ref T 引用)
			{
				引用 = default(T);
			}

			// Token: 0x06000026 RID: 38
			[EditorBrowsable(EditorBrowsableState.Never)]
			public WebServiceCollection()
			{
			}

			// Token: 0x06000027 RID: 39
			/// <summary>.NET Reactor 生成的授权标记桩，恒返回 true，无调用者。</summary>
			internal static bool RateTag()
			{
				return true;
			}

			// Token: 0x06000028 RID: 40
			/// <summary>.NET Reactor 生成的授权标记桩，恒返回 false，无调用者。</summary>
			internal static bool FillTag()
			{
				return false;
			}
		}

		// Token: 0x02000007 RID: 7
		/// <summary>泛型延迟单例容器（原名 Parameter&lt;T&gt;）：第一次取时才 new，之后复用。</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ComVisible(false)]
		internal sealed class LazySingleton<T> where T : new()
		{
			// Token: 0x06000029 RID: 41 RVA: 0x00002300
			/// <summary>取单例，为空就新建（原名 ResolveVal）。</summary>
			internal T ResolveVal()
			{
				if (Queue.LazySingleton<T>.单例实例 == null)
				{
					Queue.LazySingleton<T>.单例实例 = new T();
				}
				return Queue.LazySingleton<T>.单例实例;
			}

			// Token: 0x0600002A RID: 42 RVA: 0x0000223C
			[EditorBrowsable(EditorBrowsableState.Never)]
			public LazySingleton()
			{
			}

			// Token: 0x0600002B RID: 43
			/// <summary>.NET Reactor 生成的授权标记桩，恒返回 true，无调用者。</summary>
			internal static bool CloneTag()
			{
				return true;
			}

			// Token: 0x0600002C RID: 44
			/// <summary>.NET Reactor 生成的授权标记桩，恒返回 false，无调用者。</summary>
			internal static bool SortTag()
			{
				return false;
			}

			// Token: 0x0400000C RID: 12
			[ThreadStatic]
			[CompilerGenerated]
			private static T 单例实例;
		}
	}
}
