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
	// Token: 0x02000004 RID: 4
	/// <summary>
	/// VB.NET 自动生成的 My 命名空间根模块（真实身份已确认：五个属性上的
	/// [HelpKeyword] 分别写着 My.Computer / My.Application / My.User / My.Forms / My.WebServices）。
	/// 它本身不做事，只负责把各个 My.* 单例按需创建出来：
	/// Computer=MyComputer、Application=Importer、User=当前用户、Forms=窗体组、WebServices=Web 服务组。
	/// 原混淆名：Test
	/// </summary>
	[StandardModule]
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[HideModuleName]
	internal sealed class MyProject
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000021EB File Offset: 0x000003EB
		/// <summary>My.Computer：计算机/注册表/文件系统入口（本程序用它取 Registry）。</summary>
		[HelpKeyword("My.Computer")]
		internal static MyComputer Computer
		{
			get
			{
				return MyProject._计算机容器.取本线程实例();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021F7 File Offset: 0x000003F7
		/// <summary>My.Application：应用生命周期对象（卸载器用它取 SaveMySettingsOnExit、挂 Shutdown 事件）。</summary>
		[HelpKeyword("My.Application")]
		internal static Importer Application
		{
			get
			{
				return MyProject._应用程序容器.取本线程实例();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002203 File Offset: 0x00000403
		/// <summary>My.User：当前 Windows 用户身份包装。</summary>
		[HelpKeyword("My.User")]
		internal static User User
		{
			get
			{
				return MyProject._用户容器.取本线程实例();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000220F File Offset: 0x0000040F
		/// <summary>My.Forms：窗体单例组（Form1/Form2 都从这里取，保证同一个实例）。</summary>
		[HelpKeyword("My.Forms")]
		internal static MyProject.MyForms Forms
		{
			get
			{
				return MyProject._窗体容器.取本线程实例();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000221B File Offset: 0x0000041B
		/// <summary>My.WebServices：Web 服务代理组（本工程没有 Web 服务，始终为空壳）。</summary>
		[HelpKeyword("My.WebServices")]
		internal static MyProject.MyWebServices WebServices
		{
			get
			{
				return MyProject._Web服务容器.取本线程实例();
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021E8 File Offset: 0x000003E8
		/// <summary>混淆器注入的占位方法：恒返回 false，无调用者。原混淆名：CompareParser</summary>
		internal static bool 混淆器占位方法_恒假()
		{
			return false;
		}

		// Token: 0x04000001 RID: 1
		/// <summary>My.Computer 的线程内单例容器。</summary>
		private static readonly MyProject.MyGroupCollection<MyComputer> _计算机容器 = new MyProject.MyGroupCollection<MyComputer>();

		// Token: 0x04000002 RID: 2
		/// <summary>My.Application 的线程内单例容器。</summary>
		private static readonly MyProject.MyGroupCollection<Importer> _应用程序容器 = new MyProject.MyGroupCollection<Importer>();

		// Token: 0x04000003 RID: 3
		/// <summary>My.User 的线程内单例容器。</summary>
		private static readonly MyProject.MyGroupCollection<User> _用户容器 = new MyProject.MyGroupCollection<User>();

		// Token: 0x04000004 RID: 4
		/// <summary>My.Forms 的线程内单例容器（注意：这个字段没有 readonly）。</summary>
		private static MyProject.MyGroupCollection<MyProject.MyForms> _窗体容器 = new MyProject.MyGroupCollection<MyProject.MyForms>();

		// Token: 0x04000005 RID: 5
		/// <summary>My.WebServices 的线程内单例容器。</summary>
		private static readonly MyProject.MyGroupCollection<MyProject.MyWebServices> _Web服务容器 = new MyProject.MyGroupCollection<MyProject.MyWebServices>();

		// Token: 0x02000005 RID: 5
		/// <summary>
		/// My.Forms 的真实实现：每个窗体的「单例 + 释放后重建」逻辑都在这里。
		/// 生成器用 [MyGroupCollection] 标出三个魔术方法名（Create__Instance__ / Dispose__Instance__），
		/// 反编译后就是本类的 获取或创建窗体实例 / 释放窗体。
		/// 原混淆名：Test.GetterConfig
		/// </summary>
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class MyForms
		{
			// Token: 0x0600000F RID: 15 RVA: 0x000025E0 File Offset: 0x000007E0
			/// <summary>
			/// 取窗体单例：已有且未释放就直接返回；否则 new 一个。
			/// 用 [ThreadStatic] 的 consumerConfig 记录「正在构造中的类型」，
			/// 用来检测构造函数里又去取自己导致的无限递归（抛 WinForms_RecursiveFormCreate）。
			/// 构造抛出的 TargetInvocationException 会被拆包成 InvalidOperationException。
			/// 原混淆名：PrintReader
			/// </summary>
			private static T 获取或创建窗体实例<T>(T 现有实例) where T : Form, new()
			{
				T 结果;
				if (现有实例 != null && !现有实例.IsDisposed)
				{
					结果 = 现有实例;
				}
				else
				{
					if (MyProject.MyForms.正在创建的窗体类型 != null)
					{
						if (MyProject.MyForms.正在创建的窗体类型.ContainsKey(typeof(T)))
						{
							throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
						}
					}
					else
					{
						MyProject.MyForms.正在创建的窗体类型 = new Hashtable();
					}
					MyProject.MyForms.正在创建的窗体类型.Add(typeof(T), null);
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
						MyProject.MyForms.正在创建的窗体类型.Remove(typeof(T));
					}
				}
				return 结果;
			}

			// Token: 0x06000010 RID: 16 RVA: 0x00002227 File Offset: 0x00000427
			/// <summary>
			/// 释放窗体并把字段清成 Nothing（My.Forms.Xxx = Nothing 时走这里）。
			/// 原混淆名：FlushReader
			/// </summary>
			private void 释放窗体<T>(ref T 窗体实例) where T : Form
			{
				窗体实例.Dispose();
				窗体实例 = default(T);
			}

			// Token: 0x06000011 RID: 17 RVA: 0x0000223C File Offset: 0x0000043C
			/// <summary>生成器要求的空构造函数。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MyForms()
			{
			}

			// Token: 0x06000012 RID: 18 RVA: 0x00002244 File Offset: 0x00000444
			/// <summary>生成器要求的样板 Equals。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000013 RID: 19 RVA: 0x00002252 File Offset: 0x00000452
			/// <summary>生成器要求的样板 GetHashCode。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000014 RID: 20 RVA: 0x0000225A File Offset: 0x0000045A
			/// <summary>生成器要求的样板 GetType 替代（返回本类 Type）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type 取类型()
			{
				return typeof(MyProject.MyForms);
			}

			// Token: 0x06000015 RID: 21 RVA: 0x00002266 File Offset: 0x00000466
			/// <summary>生成器要求的样板 ToString。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x06000016 RID: 22 RVA: 0x0000226E File Offset: 0x000008E4
			// (set) Token: 0x06000018 RID: 24 RVA: 0x000026E4 File Offset: 0x000008E4
			/// <summary>
			/// My.Forms.Form1 —— 卸载器那个隐藏窗口。get 走单例创建；set 只允许设为 Nothing（触发释放）。
			/// </summary>
			public Form1 Form1
			{
				get
				{
					this.m_Form1实例 = MyProject.MyForms.获取或创建窗体实例<Form1>(this.m_Form1实例);
					return this.m_Form1实例;
				}
				set
				{
					if (value != this.m_Form1实例)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体<Form1>(ref this.m_Form1实例);
					}
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000017 RID: 23 RVA: 0x00002287 File Offset: 0x00000487
			// (set) Token: 0x06000019 RID: 25 RVA: 0x00002718 File Offset: 0x00000918
			/// <summary>
			/// My.Forms.Form2 —— 密码输入 / 卸载确认窗口。语义同 Form1：只有 get 建实例，set 只允许 Nothing。
			/// </summary>
			public Form2 Form2
			{
				get
				{
					this.m_Form2实例 = MyProject.MyForms.获取或创建窗体实例<Form2>(this.m_Form2实例);
					return this.m_Form2实例;
				}
				set
				{
					if (value != this.m_Form2实例)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体<Form2>(ref this.m_Form2实例);
					}
				}
			}

			// Token: 0x04000006 RID: 6
			/// <summary>正在构造中的窗体类型集合（线程内），用于检测递归创建。</summary>
			[ThreadStatic]
			private static Hashtable 正在创建的窗体类型;

			// Token: 0x04000007 RID: 7
			/// <summary>Form1 的单例缓存（公开字段，生成的代码就这么暴露）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form1 m_Form1实例;

			// Token: 0x04000008 RID: 8
			/// <summary>Form2 的单例缓存（公开字段，生成的代码就这么暴露）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form2 m_Form2实例;
		}

		// Token: 0x02000006 RID: 6
		/// <summary>
		/// My.WebServices 的真实实现：Web 服务代理的单例/释放逻辑。
		/// 本工程没有声明任何 Web 服务，所以这个类只有样板方法。
		/// 原混淆名：Test.ParserConfig
		/// </summary>
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class MyWebServices
		{
			// Token: 0x0600001A RID: 26 RVA: 0x00002244 File Offset: 0x00000444
			/// <summary>生成器要求的样板 Equals。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600001B RID: 27 RVA: 0x00002252 File Offset: 0x00000452
			/// <summary>生成器要求的样板 GetHashCode。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x0600001C RID: 28 RVA: 0x000022A0 File Offset: 0x000004A0
			/// <summary>生成器要求的样板：返回本类 Type。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type 取类型()
			{
				return typeof(MyProject.MyWebServices);
			}

			// Token: 0x0600001D RID: 29 RVA: 0x00002266 File Offset: 0x00000466
			/// <summary>生成器要求的样板 ToString。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x0600001E RID: 30 RVA: 0x00002748 File Offset: 0x00000948
			/// <summary>
			/// 取 Web 服务代理单例：null 就 new 一个，否则原样返回。
			/// 原混淆名：CalcReader
			/// </summary>
			private static T 取或创建实例<T>(T 实例) where T : new()
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

			// Token: 0x0600001F RID: 31 RVA: 0x000022AC File Offset: 0x000004AC
			/// <summary>把字段清成 default（My.WebServices.Xxx = Nothing 时走这里）。原混淆名：ReflectReader</summary>
			private void 清空实例<T>(ref T 实例)
			{
				实例 = default(T);
			}

			// Token: 0x06000020 RID: 32 RVA: 0x0000223C File Offset: 0x0000043C
			/// <summary>生成器要求的空构造函数。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MyWebServices()
			{
			}

			// Token: 0x06000021 RID: 33 RVA: 0x000021DD File Offset: 0x000003DD
			/// <summary>混淆器注入的占位方法：恒返回 true，无调用者。原混淆名：InsertParser</summary>
			internal static bool 混淆器占位方法_恒真()
			{
				return true;
			}

			// Token: 0x06000022 RID: 34 RVA: 0x000021E8 File Offset: 0x000003E8
			/// <summary>混淆器注入的占位方法：恒返回 false，无调用者。原混淆名：VerifyParser</summary>
			internal static bool 混淆器占位方法_恒假()
			{
				return false;
			}
		}

		// Token: 0x02000007 RID: 7
		/// <summary>
		/// My.* 单例容器的泛型实现：每个线程各持一份实例（[ThreadStatic]），
		/// 第一次取时 new，之后一直复用——这就是 My.Computer 之类永远返回同一个对象的原因。
		/// 原混淆名：Test.ParamsConfig&lt;T&gt;
		/// </summary>
		[ComVisible(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class MyGroupCollection<T> where T : new()
		{
			// Token: 0x06000023 RID: 35 RVA: 0x000022B5 File Offset: 0x000004B5
			/// <summary>取本线程的实例，没有就创建（懒汉式，无锁）。原混淆名：取本线程实例</summary>
			internal T 取本线程实例()
			{
				if (MyProject.MyGroupCollection<T>._实例缓存 == null)
				{
					MyProject.MyGroupCollection<T>._实例缓存 = new T();
				}
				return MyProject.MyGroupCollection<T>._实例缓存;
			}

			// Token: 0x06000024 RID: 36 RVA: 0x0000223C File Offset: 0x0000043C
			/// <summary>生成器要求的空构造函数。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MyGroupCollection()
			{
			}

			// Token: 0x06000025 RID: 37 RVA: 0x000021DD File Offset: 0x000003DD
			/// <summary>混淆器注入的占位方法：恒返回 true，无调用者。原混淆名：DisableParser</summary>
			internal static bool 混淆器占位方法_恒真()
			{
				return true;
			}

			// Token: 0x06000026 RID: 38 RVA: 0x000021E8 File Offset: 0x000003E8
			/// <summary>混淆器注入的占位方法：恒返回 false，无调用者。原混淆名：ForgotParser</summary>
			internal static bool 混淆器占位方法_恒假()
			{
				return false;
			}

			// Token: 0x04000009 RID: 9
			/// <summary>本线程的单例缓存。</summary>
			[CompilerGenerated]
			[ThreadStatic]
			private static T _实例缓存;
		}
	}
}
