using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;

namespace zmserv.Internal
{
	// Token: 0x02000004 RID: 4
	/// <summary>
	/// VB 的 My 命名空间根模块（原编译产物里叫 MyProject）。
	/// 真实职责：把 My.Computer / My.Application / My.User / My.WebServices 四个入口
	/// 转发到四个线程安全的对象提供器（ThreadSafeObjectProvider）。没有业务逻辑。
	/// 这四个属性在本工程里都没有任何调用点。
	/// </summary>
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[StandardModule]
	[HideModuleName]
	internal sealed class MyProject
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000021CA File Offset: 0x000003CA
		/// <summary>My.Computer：取本线程的 MyComputer 实例。</summary>
		[HelpKeyword("My.Computer")]
		internal static MyComputer Computer
		{
			get
			{
				return MyProject.computerProvider.GetInstance();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000021D6 File Offset: 0x000003D6
		/// <summary>My.Application：取本线程的 MyApplication 实例。</summary>
		[HelpKeyword("My.Application")]
		internal static MyApplication Application
		{
			get
			{
				return MyProject.applicationProvider.GetInstance();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021E2 File Offset: 0x000003E2
		/// <summary>My.User：取本线程的 User 实例（VB 的当前用户抽象）。</summary>
		[HelpKeyword("My.User")]
		internal static User User
		{
			get
			{
				return MyProject.userProvider.GetInstance();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000021EE File Offset: 0x000003EE
		/// <summary>My.WebServices：取本线程的 MyWebServices 集合。</summary>
		[HelpKeyword("My.WebServices")]
		internal static MyWebServices WebServices
		{
			get
			{
				return MyProject.webServicesProvider.GetInstance();
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>壳注入的空桩：固定返回 true，无调用点。</summary>
		internal static bool VisitDefinition()
		{
			return true;
		}

		// Token: 0x04000001 RID: 1
		/// <summary>My.Computer 的对象提供器。</summary>
		private static readonly MyProject.ThreadSafeObjectProvider<MyComputer> computerProvider = new MyProject.ThreadSafeObjectProvider<MyComputer>();

		// Token: 0x04000002 RID: 2
		/// <summary>My.Application 的对象提供器。</summary>
		private static readonly MyProject.ThreadSafeObjectProvider<MyApplication> applicationProvider = new MyProject.ThreadSafeObjectProvider<MyApplication>();

		// Token: 0x04000003 RID: 3
		/// <summary>My.User 的对象提供器。</summary>
		private static readonly MyProject.ThreadSafeObjectProvider<User> userProvider = new MyProject.ThreadSafeObjectProvider<User>();

		// Token: 0x04000004 RID: 4
		/// <summary>My.WebServices 的对象提供器。</summary>
		private static readonly MyProject.ThreadSafeObjectProvider<MyProject.MyWebServices> webServicesProvider = new MyProject.ThreadSafeObjectProvider<MyProject.MyWebServices>();

		// Token: 0x02000005 RID: 5
		/// <summary>
		/// My.WebServices 集合（[MyGroupCollection] 生成）。本工程没有任何 Web 服务引用，
		/// 所以里面除模板成员外空无一物；下面两个静态的 Definition 方法同样是壳注入空桩。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		internal sealed class MyWebServices
		{
			// Token: 0x0600000D RID: 13 RVA: 0x000021FA File Offset: 0x000003FA
			/// <summary>模板生成的 Equals 转发（用 VB 的 RuntimeHelpers.GetObjectValue 取值，语义与 C# 不同）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600000E RID: 14 RVA: 0x00002208 File Offset: 0x00000408
			/// <summary>模板生成的 GetHashCode 转发。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x0600000F RID: 15 RVA: 0x00002210 File Offset: 0x00000410
			/// <summary>模板生成的 GetType 遮蔽（VB 里是 Shadows；这里改名以免与 Object.GetType 混淆）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type GetInstanceType()
			{
				return typeof(MyProject.MyWebServices);
			}

			// Token: 0x06000010 RID: 16 RVA: 0x0000221C File Offset: 0x0000041C
			/// <summary>模板生成的 ToString 转发。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x06000011 RID: 17 RVA: 0x00002474 File Offset: 0x00000674
			/// <summary>模板生成的实例工厂：null 就新建，否则原样返回。</summary>
			private static T CreateInstance<T>(T instance) where T : new()
			{
				T result;
				if (instance == null)
				{
					result = new T();
				}
				else
				{
					result = instance;
				}
				return result;
			}

			// Token: 0x06000012 RID: 18 RVA: 0x00002224 File Offset: 0x00000424
			/// <summary>模板生成的实例释放：直接把引用置为默认值。</summary>
			private void DisposeInstance<T>(ref T instance)
			{
				instance = default(T);
			}

			// Token: 0x06000013 RID: 19 RVA: 0x0000222D File Offset: 0x0000042D
			/// <summary>空构造（模板生成）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MyWebServices()
			{
			}

			// Token: 0x06000014 RID: 20 RVA: 0x000021BC File Offset: 0x000003BC
			/// <summary>壳注入的空桩：固定返回 true，无调用点。</summary>
			internal static bool PrepareDefinition()
			{
				return true;
			}

			// Token: 0x06000015 RID: 21 RVA: 0x000021BF File Offset: 0x000003BF
			/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
			internal static bool CompareDefinition()
			{
				return false;
			}
		}

		// Token: 0x02000006 RID: 6
		/// <summary>
		/// 线程安全的延迟实例化提供器（原编译产物里的 ThreadSafeObjectProvider&lt;T&gt;）：
		/// 每个线程各自缓存一个 T 实例，只有第一次取用时才 new。
		/// </summary>
		[ComVisible(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class ThreadSafeObjectProvider<T> where T : new()
		{
			// Token: 0x06000016 RID: 22 RVA: 0x00002235 File Offset: 0x00000435
			/// <summary>取本线程的实例，不存在则创建。</summary>
			internal T GetInstance()
			{
				if (MyProject.ThreadSafeObjectProvider<T>.threadLocalInstance == null)
				{
					MyProject.ThreadSafeObjectProvider<T>.threadLocalInstance = new T();
				}
				return MyProject.ThreadSafeObjectProvider<T>.threadLocalInstance;
			}

			// Token: 0x06000017 RID: 23 RVA: 0x0000222D File Offset: 0x0000042D
			/// <summary>空构造（模板生成）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public ThreadSafeObjectProvider()
			{
			}

			// Token: 0x06000018 RID: 24 RVA: 0x000021BC File Offset: 0x000003BC
			/// <summary>壳注入的空桩：固定返回 true，无调用点。</summary>
			internal static bool InvokeDefinition()
			{
				return true;
			}

			// Token: 0x06000019 RID: 25 RVA: 0x000021BF File Offset: 0x000003BF
			/// <summary>壳注入的空桩：固定返回 false，无调用点。</summary>
			internal static bool ManageDefinition()
			{
				return false;
			}

			// Token: 0x04000005 RID: 5
			/// <summary>[ThreadStatic] 的线程本地缓存字段。</summary>
			[CompilerGenerated]
			[ThreadStatic]
			private static T threadLocalInstance;
		}
	}
}
