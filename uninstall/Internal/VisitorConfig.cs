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
	// <summary>
	/// VB.NET 自动生成的 My 命名空间访问器（MyTemplate 生成）：把 My.Computer（ConfigConfig）、
	/// My.Application（UtilsConfig）、My.User、My.Forms（MapperConfig）、My.WebServices
	/// （ProcessConfig）以单例形式暴露。MapperConfig 是 Form1/Form2 的懒加载单例容器，
	/// 业务代码通过 VisitorConfig.Forms.Form2 / VisitorConfig.Forms.Form1 访问窗体实例。
	/// 全部为 VS 模板自动生成代码，无恶意行为、无 IOC。
	/// </summary>
	[HideModuleName]
	[StandardModule]
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	internal sealed class VisitorConfig
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002530 File Offset: 0x00000730
		/// <summary>My.Computer（原混淆名 StopUtils）：返回 ConfigConfig（Computer 子类）。</summary>
		[HelpKeyword("My.Computer")]
		internal static ConfigConfig Computer
		{
			get
			{
				return VisitorConfig._computerSingleton.GetInstance();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002548 File Offset: 0x00000748
		/// <summary>My.Application（原混淆名 CollectUtils）：返回 UtilsConfig（应用启动器）。</summary>
		[HelpKeyword("My.Application")]
		internal static UtilsConfig Application
		{
			get
			{
				return VisitorConfig._applicationSingleton.GetInstance();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002560 File Offset: 0x00000760
		/// <summary>My.User（原混淆名 LogoutUtils）：返回 Microsoft.VisualBasic.ApplicationServices.User。</summary>
		[HelpKeyword("My.User")]
		internal static User User
		{
			get
			{
				return VisitorConfig._userSingleton.GetInstance();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002578 File Offset: 0x00000778
		/// <summary>My.Forms（原混淆名 ListUtils）：返回 MapperConfig（Form1/Form2 单例容器）。
		/// 业务调用点：Form1/Form2/UtilsConfig 经此访问窗体实例。</summary>
		[HelpKeyword("My.Forms")]
		internal static VisitorConfig.MapperConfig Forms
		{
			get
			{
				return VisitorConfig._formsSingleton.GetInstance();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002590 File Offset: 0x00000790
		/// <summary>My.WebServices（原混淆名 MapUtils）：返回 ProcessConfig（Web 服务桩）。</summary>
		[HelpKeyword("My.WebServices")]
		internal static VisitorConfig.ProcessConfig WebServices
		{
			get
			{
				return VisitorConfig._webServicesSingleton.GetInstance();
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>恒 false 混淆桩，无实际逻辑。</summary>
		internal static bool StartRegistry()
		{
			return false;
		}

		// Token: 0x04000001 RID: 1
		/// <summary>My.Computer 单例持有字段（原 m_ExceptionConfig）。</summary>
		private static readonly VisitorConfig.SingletonConfig<ConfigConfig> _computerSingleton = new VisitorConfig.SingletonConfig<ConfigConfig>();

		// Token: 0x04000002 RID: 2
		/// <summary>My.Application 单例持有字段（原 m_ReponseConfig）。</summary>
		private static readonly VisitorConfig.SingletonConfig<UtilsConfig> _applicationSingleton = new VisitorConfig.SingletonConfig<UtilsConfig>();

		// Token: 0x04000003 RID: 3
		/// <summary>My.User 单例持有字段（原 invocationConfig）。</summary>
		private static readonly VisitorConfig.SingletonConfig<User> _userSingleton = new VisitorConfig.SingletonConfig<User>();

		// Token: 0x04000004 RID: 4
		/// <summary>My.Forms 单例持有字段（原 m_ContainerConfig）。</summary>
		private static VisitorConfig.SingletonConfig<VisitorConfig.MapperConfig> _formsSingleton = new VisitorConfig.SingletonConfig<VisitorConfig.MapperConfig>();

		// Token: 0x04000005 RID: 5
		/// <summary>My.WebServices 单例持有字段（原 m_InterceptorConfig）。</summary>
		private static readonly VisitorConfig.SingletonConfig<VisitorConfig.ProcessConfig> _webServicesSingleton = new VisitorConfig.SingletonConfig<VisitorConfig.ProcessConfig>();

		// Token: 0x02000005 RID: 5
		/// <summary>
		/// My.Forms 窗体单例容器（原 MapperConfig）：Form1/Form2 属性懒加载——
		/// 实例为空或已 Dispose 则 GetOrCreateForm 新建；业务代码经
		/// VisitorConfig.Forms.Form2 / Form1 访问（Form2 被反复 Show 正是经由此容器）。
		/// </summary>
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class MapperConfig
		{
			// Token: 0x17000006 RID: 6
			// (get) Token: 0x0600000F RID: 15 RVA: 0x000025A8 File Offset: 0x000007A8
			// (set) Token: 0x06000011 RID: 17 RVA: 0x000025F0 File Offset: 0x000007F0
			/// <summary>Form1 单例（懒创建，已释放则重建）；set 仅允许置 Nothing（触发释放）。</summary>
			public Form1 Form1
			{
				get
				{
					this._form1 = VisitorConfig.MapperConfig.GetOrCreateForm<Form1>(this._form1);
					return this._form1;
				}
				set
				{
					if (value == this._form1)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.DisposeAndClearForm<Form1>(ref this._form1);
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000010 RID: 16 RVA: 0x000025CC File Offset: 0x000007CC
			// (set) Token: 0x06000012 RID: 18 RVA: 0x00002624 File Offset: 0x00000824
			/// <summary>Form2 单例（懒创建，已释放则重建）；set 仅允许置 Nothing（触发释放）。</summary>
			public Form2 Form2
			{
				get
				{
					this._form2 = VisitorConfig.MapperConfig.GetOrCreateForm<Form2>(this._form2);
					return this._form2;
				}
				set
				{
					if (value == this._form2)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.DisposeAndClearForm<Form2>(ref this._form2);
				}
			}

			// Token: 0x06000013 RID: 19 RVA: 0x00002658 File Offset: 0x00000858
			/// <summary>
			/// 窗体单例获取（原 CallUtils&lt;T&gt;）：实例存在且未释放则复用，否则
			/// Activator.CreateInstance&lt;T&gt;() 新建；用 _recursionGuard（ThreadStatic Hashtable）
			/// 防递归创建（递归时抛 WinForms_RecursiveFormCreate）。
			/// </summary>
			private static T GetOrCreateForm<T>(T Instance) where T : Form, new()
			{
				if (Instance != null && !Instance.IsDisposed)
				{
					return Instance;
				}
				if (VisitorConfig.MapperConfig._recursionGuard != null)
				{
					if (VisitorConfig.MapperConfig._recursionGuard.ContainsKey(typeof(T)))
					{
						throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
					}
				}
				else
				{
					VisitorConfig.MapperConfig._recursionGuard = new Hashtable();
				}
				VisitorConfig.MapperConfig._recursionGuard.Add(typeof(T), null);
				T result;
				try
				{
					result = Activator.CreateInstance<T>();
				}
				catch (TargetInvocationException ex) when (ex.InnerException != null)
				{
					string resourceString = Utils.GetResourceString("WinForms_SeeInnerException", new string[]
					{
						ex.InnerException.Message
					});
					throw new InvalidOperationException(resourceString, ex.InnerException);
				}
				finally
				{
					VisitorConfig.MapperConfig._recursionGuard.Remove(typeof(T));
				}
				return result;
			}

			// Token: 0x06000014 RID: 20 RVA: 0x00002760 File Offset: 0x00000960
			/// <summary>释放并置空窗体（原 IncludeUtils&lt;T&gt;）：Dispose 后置 default(T)。</summary>
			private void DisposeAndClearForm<T>(ref T gparam_0) where T : Form
			{
				gparam_0.Dispose();
				gparam_0 = default(T);
			}

			// Token: 0x06000015 RID: 21 RVA: 0x000021D4 File Offset: 0x000003D4
			/// <summary>空构造函数（模板生成）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MapperConfig()
			{
			}

			// Token: 0x06000016 RID: 22 RVA: 0x00002788 File Offset: 0x00000988
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000017 RID: 23 RVA: 0x000027A4 File Offset: 0x000009A4
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000018 RID: 24 RVA: 0x000027B8 File Offset: 0x000009B8
			/// <summary>返回自身类型（模板生成，My.Forms 基础设施用）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type PublishUtils()
			{
				return typeof(VisitorConfig.MapperConfig);
			}

			// Token: 0x06000019 RID: 25 RVA: 0x000027D0 File Offset: 0x000009D0
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x04000006 RID: 6
			/// <summary>Form1 单例持有字段（原 m_ConfigurationConfig）。</summary>
			public Form1 _form1;

			// Token: 0x04000007 RID: 7
			/// <summary>Form2 单例持有字段（原 factoryConfig）。</summary>
			public Form2 _form2;

			// Token: 0x04000008 RID: 8
			/// <summary>递归创建防护（ThreadStatic Hashtable，原 m_InterpreterConfig）。</summary>
			[ThreadStatic]
			private static Hashtable _recursionGuard;
		}

		// Token: 0x02000006 RID: 6
		/// <summary>
		/// My.WebServices 容器（原 ProcessConfig）：Web 服务桩类，仅模板方法，
		/// 本组件未定义任何 Web 服务，无业务逻辑。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		internal sealed class ProcessConfig
		{
			// Token: 0x0600001A RID: 26 RVA: 0x00002788 File Offset: 0x00000988
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600001B RID: 27 RVA: 0x000027A4 File Offset: 0x000009A4
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x0600001C RID: 28 RVA: 0x000027E4 File Offset: 0x000009E4
			/// <summary>返回自身类型（模板生成，My.WebServices 基础设施用）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type InitUtils()
			{
				return typeof(VisitorConfig.ProcessConfig);
			}

			// Token: 0x0600001D RID: 29 RVA: 0x000027D0 File Offset: 0x000009D0
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x0600001E RID: 30 RVA: 0x000027FC File Offset: 0x000009FC
			/// <summary>懒创建单例（模板生成）：实例为 null 时 Activator 创建，否则复用。</summary>
			private static T MoveUtils<T>(T instance) where T : new()
			{
				if (instance == null)
				{
					return Activator.CreateInstance<T>();
				}
				return instance;
			}

			// Token: 0x0600001F RID: 31 RVA: 0x00002818 File Offset: 0x00000A18
			/// <summary>置空引用（模板生成）。</summary>
			private void ResolveUtils<T>(ref T gparam_0)
			{
				gparam_0 = default(T);
			}

			// Token: 0x06000020 RID: 32 RVA: 0x000021D4 File Offset: 0x000003D4
			/// <summary>空构造函数（模板生成）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public ProcessConfig()
			{
			}

			// Token: 0x06000021 RID: 33 RVA: 0x000021D1 File Offset: 0x000003D1
			/// <summary>恒 true 混淆桩，无实际逻辑。</summary>
			internal static bool CheckRegistry()
			{
				return true;
			}

				// Token: 0x06000022 RID: 34 RVA: 0x000021C6 File Offset: 0x000003C6
			/// <summary>恒 false 混淆桩，无实际逻辑。</summary>
			internal static bool LoginRegistry()
			{
				return false;
			}
		}

		// Token: 0x02000007 RID: 7
		/// <summary>
		/// 线程级懒加载单例持有器（原 SingletonConfig&lt;T&gt;）：GetInstance（原 WriteUtils）
		/// 首次调用时 Activator 创建并缓存于 _instance（ThreadStatic），之后直接复用。
		/// </summary>
		[ComVisible(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class SingletonConfig<T> where T : new()
		{
			// Token: 0x06000023 RID: 35 RVA: 0x00002834 File Offset: 0x00000A34
			/// <summary>取单例实例（原 WriteUtils）：懒创建并缓存。</summary>
			internal T GetInstance()
			{
				if (VisitorConfig.SingletonConfig<T>._instance == null)
				{
					VisitorConfig.SingletonConfig<T>._instance = Activator.CreateInstance<T>();
				}
				return VisitorConfig.SingletonConfig<T>._instance;
			}

			// Token: 0x06000024 RID: 36 RVA: 0x000021D4 File Offset: 0x000003D4
			/// <summary>空构造函数（模板生成）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public SingletonConfig()
			{
			}

			// Token: 0x06000025 RID: 37 RVA: 0x000021D1 File Offset: 0x000003D1
			/// <summary>恒 true 混淆桩，无实际逻辑。</summary>
			internal static bool VisitRegistry()
			{
				return true;
			}

			// Token: 0x06000026 RID: 38 RVA: 0x000021C6 File Offset: 0x000003C6
			/// <summary>恒 false 混淆桩，无实际逻辑。</summary>
			internal static bool RemoveRegistry()
			{
				return false;
			}

			// Token: 0x04000009 RID: 9
			/// <summary>线程级单例缓存字段（原 _GetterConfig）。</summary>
			[ThreadStatic]
			[CompilerGenerated]
			private static T _instance;
		}
	}
}
