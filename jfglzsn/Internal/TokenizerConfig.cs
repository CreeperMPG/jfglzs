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
	[HideModuleName]
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	[StandardModule]
	// ============================================================
	// Internal/TokenizerConfig.cs — VB My.* 命名空间基础设施（My.Forms/My.Application 等）
	// VB.NET 编译器生成的 My 项目基础设施，混淆器把成员改名为 *Config/*Template 假名。真实内容：My.Computer（MyComputer→DispatcherConfig）、My.Application（MyApplication→TemplateConfig）、My.User、My.Forms（MyForms→DicConfig：Form1~Form4 懒加载实例管理）、My.WebServices。MyForms.GetOrCreateFormInstance 用线程静态 Hashtable 防递归创建表单。全文件无恶意逻辑，但 MyForms.Form1~Form4 是主控代码拿到锁屏窗体实例的枢纽。
	// ============================================================
	internal sealed class TokenizerConfig
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000025B0 File Offset: 0x000007B0
		[HelpKeyword("My.Computer")]
		/// <summary>
		/// My.Computer（原名 PublishTemplate）：返回 DispatcherConfig 线程静态单例。
		/// </summary>
		internal static DispatcherConfig MyComputer
		{
			get
			{
				return TokenizerConfig._MethodConfig.GetThreadStaticSingleton();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000025C8 File Offset: 0x000007C8
		[HelpKeyword("My.Application")]
		/// <summary>
		/// My.Application（原名 PopTemplate）：返回 TemplateConfig 单例（应用启动器）。
		/// </summary>
		internal static TemplateConfig MyApplication
		{
			get
			{
				return TokenizerConfig.parameterConfig.GetThreadStaticSingleton();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000025E0 File Offset: 0x000007E0
		[HelpKeyword("My.User")]
		/// <summary>
		/// My.User（原名 CallTemplate）：返回 User 单例。
		/// </summary>
		internal static User MyUser
		{
			get
			{
				return TokenizerConfig._PredicateConfig.GetThreadStaticSingleton();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000025F8 File Offset: 0x000007F8
		[HelpKeyword("My.Forms")]
		/// <summary>
		/// My.Forms（原名 FindTemplate）：返回 DicConfig —— Form1~Form4 实例访问入口（主控代码取锁屏窗的枢纽）。
		/// </summary>
		internal static TokenizerConfig.DicConfig MyForms
		{
			get
			{
				return TokenizerConfig.m_StrategyConfig.GetThreadStaticSingleton();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002610 File Offset: 0x00000810
		[HelpKeyword("My.WebServices")]
		/// <summary>
		/// My.WebServices（原名 ConcatTemplate）：返回 ProccesorConfig（空壳集合）。
		/// </summary>
		internal static TokenizerConfig.ProccesorConfig MyWebServices
		{
			get
			{
				return TokenizerConfig._ExpressionConfig.GetThreadStaticSingleton();
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>
		/// 壳桩：恒 true（混淆残留）。
		/// </summary>
		internal static bool PushParser()
		{
			return true;
		}

		// Token: 0x04000001 RID: 1
		/// <summary>
		/// My.Computer 单例容器（AccountConfig<DispatcherConfig>）。
		/// </summary>
		private static readonly TokenizerConfig.AccountConfig<DispatcherConfig> _MethodConfig = new TokenizerConfig.AccountConfig<DispatcherConfig>();

		// Token: 0x04000002 RID: 2
		/// <summary>
		/// My.Application 单例容器（AccountConfig<TemplateConfig>）。
		/// </summary>
		private static readonly TokenizerConfig.AccountConfig<TemplateConfig> parameterConfig = new TokenizerConfig.AccountConfig<TemplateConfig>();

		// Token: 0x04000003 RID: 3
		/// <summary>
		/// My.User 单例容器（AccountConfig<User>）。
		/// </summary>
		private static readonly TokenizerConfig.AccountConfig<User> _PredicateConfig = new TokenizerConfig.AccountConfig<User>();

		// Token: 0x04000004 RID: 4
		/// <summary>
		/// My.Forms 单例容器（AccountConfig<DicConfig>）。
		/// </summary>
		private static TokenizerConfig.AccountConfig<TokenizerConfig.DicConfig> m_StrategyConfig = new TokenizerConfig.AccountConfig<TokenizerConfig.DicConfig>();

		// Token: 0x04000005 RID: 5
		/// <summary>
		/// My.WebServices 单例容器（AccountConfig<ProccesorConfig>）。
		/// </summary>
		private static readonly TokenizerConfig.AccountConfig<TokenizerConfig.ProccesorConfig> _ExpressionConfig = new TokenizerConfig.AccountConfig<TokenizerConfig.ProccesorConfig>();

		// Token: 0x02000005 RID: 5
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		internal sealed class DicConfig
		{
			// Token: 0x17000006 RID: 6
			// (get) Token: 0x0600000F RID: 15 RVA: 0x00002628 File Offset: 0x00000828
			// (set) Token: 0x06000013 RID: 19 RVA: 0x000026B8 File Offset: 0x000008B8
		/// <summary>
		/// My.Forms.Form1：懒加载创建/复用 Form1 实例（恶意主控窗体）。
		/// </summary>
			public Form1 Form1
			{
				get
				{
					this.m_MerchantConfig = TokenizerConfig.DicConfig.GetOrCreateFormInstance<Form1>(this.m_MerchantConfig);
					return this.m_MerchantConfig;
				}
				set
				{
					if (value == this.m_MerchantConfig)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.DisposeFormInstance<Form1>(ref this.m_MerchantConfig);
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000010 RID: 16 RVA: 0x0000264C File Offset: 0x0000084C
			// (set) Token: 0x06000014 RID: 20 RVA: 0x000026EC File Offset: 0x000008EC
		/// <summary>
		/// My.Forms.Form2：懒加载创建/复用 Form2（一级锁屏窗）。
		/// </summary>
			public Form2 Form2
			{
				get
				{
					this.m_SerializerConfig = TokenizerConfig.DicConfig.GetOrCreateFormInstance<Form2>(this.m_SerializerConfig);
					return this.m_SerializerConfig;
				}
				set
				{
					if (value == this.m_SerializerConfig)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.DisposeFormInstance<Form2>(ref this.m_SerializerConfig);
				}
			}

			// Token: 0x17000008 RID: 8
			// (get) Token: 0x06000011 RID: 17 RVA: 0x00002670 File Offset: 0x00000870
			// (set) Token: 0x06000015 RID: 21 RVA: 0x00002720 File Offset: 0x00000920
		/// <summary>
		/// My.Forms.Form3：懒加载创建/复用 Form3（警告提示窗）。
		/// </summary>
			public Form3 Form3
			{
				get
				{
					this._ProxyConfig = TokenizerConfig.DicConfig.GetOrCreateFormInstance<Form3>(this._ProxyConfig);
					return this._ProxyConfig;
				}
				set
				{
					if (value == this._ProxyConfig)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.DisposeFormInstance<Form3>(ref this._ProxyConfig);
				}
			}

			// Token: 0x17000009 RID: 9
			// (get) Token: 0x06000012 RID: 18 RVA: 0x00002694 File Offset: 0x00000894
			// (set) Token: 0x06000016 RID: 22 RVA: 0x00002754 File Offset: 0x00000954
		/// <summary>
		/// My.Forms.Form4：懒加载创建/复用 Form4（二级锁屏窗）。
		/// </summary>
			public Form4 Form4
			{
				get
				{
					this.initializerConfig = TokenizerConfig.DicConfig.GetOrCreateFormInstance<Form4>(this.initializerConfig);
					return this.initializerConfig;
				}
				set
				{
					if (value == this.initializerConfig)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.DisposeFormInstance<Form4>(ref this.initializerConfig);
				}
			}

			// Token: 0x06000017 RID: 23 RVA: 0x00002788 File Offset: 0x00000988
		/// <summary>
		/// 创建/复用表单实例（原名 GetTemplate<T>）：缓存非空且未 Dispose→直接返回；否则查 m_FilterConfig 递归保护（创建期间同类型再次访问抛 WinForms_RecursiveFormCreate）；Activator.CreateInstance<T>() 创建后缓存；TargetInvocationException 时包装内部异常抛出。
		/// </summary>
			private static T GetOrCreateFormInstance<T>(T Instance) where T : Form, new()
			{
				if (Instance != null && !Instance.IsDisposed)
				{
					return Instance;
				}
				if (TokenizerConfig.DicConfig.m_FilterConfig != null)
				{
					if (TokenizerConfig.DicConfig.m_FilterConfig.ContainsKey(typeof(T)))
					{
						throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
					}
				}
				else
				{
					TokenizerConfig.DicConfig.m_FilterConfig = new Hashtable();
				}
				TokenizerConfig.DicConfig.m_FilterConfig.Add(typeof(T), null);
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
					TokenizerConfig.DicConfig.m_FilterConfig.Remove(typeof(T));
				}
				return result;
			}

			// Token: 0x06000018 RID: 24 RVA: 0x00002890 File Offset: 0x00000A90
		/// <summary>
		/// 释放表单实例（原名 AssetTemplate<T>）：Dispose 并置空。
		/// </summary>
			private void DisposeFormInstance<T>(ref T gparam_0) where T : Form
			{
				gparam_0.Dispose();
				gparam_0 = default(T);
			}

			// Token: 0x06000019 RID: 25 RVA: 0x000021D4 File Offset: 0x000003D4
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 构造器（空）。
		/// </summary>
			public DicConfig()
			{
			}

			// Token: 0x0600001A RID: 26 RVA: 0x000028B8 File Offset: 0x00000AB8
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 基类转发（生成代码）。
		/// </summary>
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600001B RID: 27 RVA: 0x000028D4 File Offset: 0x00000AD4
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 基类转发（生成代码）。
		/// </summary>
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x0600001C RID: 28 RVA: 0x000028E8 File Offset: 0x00000AE8
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 返回 DicConfig 类型（生成代码桩）。
		/// </summary>
			internal Type TestTemplate()
			{
				return typeof(TokenizerConfig.DicConfig);
			}

			// Token: 0x0600001D RID: 29 RVA: 0x00002900 File Offset: 0x00000B00
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 基类转发（生成代码）。
		/// </summary>
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x04000006 RID: 6
		/// <summary>
		/// Form1 实例缓存字段。
		/// </summary>
			public Form1 m_MerchantConfig;

			// Token: 0x04000007 RID: 7
		/// <summary>
		/// Form2 实例缓存字段。
		/// </summary>
			public Form2 m_SerializerConfig;

			// Token: 0x04000008 RID: 8
		/// <summary>
		/// Form3 实例缓存字段。
		/// </summary>
			public Form3 _ProxyConfig;

			// Token: 0x04000009 RID: 9
		/// <summary>
		/// Form4 实例缓存字段。
		/// </summary>
			public Form4 initializerConfig;

			// Token: 0x0400000A RID: 10
			[ThreadStatic]
		/// <summary>
		/// 递归创建防护表（ThreadStatic Hashtable）。
		/// </summary>
			private static Hashtable m_FilterConfig;
		}

		// Token: 0x02000006 RID: 6
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		internal sealed class ProccesorConfig
		{
			// Token: 0x0600001E RID: 30 RVA: 0x000028B8 File Offset: 0x00000AB8
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 基类转发（生成代码）。
		/// </summary>
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600001F RID: 31 RVA: 0x000028D4 File Offset: 0x00000AD4
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 基类转发（生成代码）。
		/// </summary>
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000020 RID: 32 RVA: 0x00002914 File Offset: 0x00000B14
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 返回 ProccesorConfig 类型（生成代码桩）。
		/// </summary>
			internal Type RunTemplate()
			{
				return typeof(TokenizerConfig.ProccesorConfig);
			}

			// Token: 0x06000021 RID: 33 RVA: 0x00002900 File Offset: 0x00000B00
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 基类转发（生成代码）。
		/// </summary>
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x06000022 RID: 34 RVA: 0x0000292C File Offset: 0x00000B2C
		/// <summary>
		/// 创建/复用 WebService 实例（原名 RateTemplate<T>，空壳）。
		/// </summary>
			private static T GetOrCreateWebServiceInstance<T>(T instance) where T : new()
			{
				if (instance == null)
				{
					return Activator.CreateInstance<T>();
				}
				return instance;
			}

			// Token: 0x06000023 RID: 35 RVA: 0x00002948 File Offset: 0x00000B48
		/// <summary>
		/// 释放 WebService 实例（原名 ViewTemplate<T>）：置空。
		/// </summary>
			private void DisposeWebServiceInstance<T>(ref T gparam_0)
			{
				gparam_0 = default(T);
			}

			// Token: 0x06000024 RID: 36 RVA: 0x000021D4 File Offset: 0x000003D4
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 构造器（空）。
		/// </summary>
			public ProccesorConfig()
			{
			}

			// Token: 0x06000025 RID: 37 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>
		/// 壳桩：恒 true。
		/// </summary>
			internal static bool ManageParser()
			{
				return true;
			}

			// Token: 0x06000026 RID: 38 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>
		/// 壳桩：恒 false。
		/// </summary>
			internal static bool MoveParser()
			{
				return false;
			}
		}

		// Token: 0x02000007 RID: 7
		[ComVisible(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class AccountConfig<T> where T : new()
		{
			// Token: 0x06000027 RID: 39 RVA: 0x00002964 File Offset: 0x00000B64
		/// <summary>
		/// 获取线程静态单例（原名 DestroyTemplate）：m_CodeConfig 为空→Activator.CreateInstance<T>()；[ThreadStatic] 保证每线程一份。
		/// </summary>
			internal T GetThreadStaticSingleton()
			{
				if (TokenizerConfig.AccountConfig<T>.m_CodeConfig == null)
				{
					TokenizerConfig.AccountConfig<T>.m_CodeConfig = Activator.CreateInstance<T>();
				}
				return TokenizerConfig.AccountConfig<T>.m_CodeConfig;
			}

			// Token: 0x06000028 RID: 40 RVA: 0x000021D4 File Offset: 0x000003D4
			[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// 构造器（空）。
		/// </summary>
			public AccountConfig()
			{
			}

			// Token: 0x06000029 RID: 41 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>
		/// 壳桩：恒 true。
		/// </summary>
			internal static bool ConcatParser()
			{
				return true;
			}

			// Token: 0x0600002A RID: 42 RVA: 0x000021D1 File Offset: 0x000003D1
		/// <summary>
		/// 壳桩：恒 false。
		/// </summary>
			internal static bool UpdateParser()
			{
				return false;
			}

			// Token: 0x0400000B RID: 11
			[ThreadStatic]
			[CompilerGenerated]
		/// <summary>
		/// 线程静态单例缓存（AccountConfig<T>）。
		/// </summary>
			private static T m_CodeConfig;
		}
	}
}
