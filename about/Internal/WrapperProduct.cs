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
using WindowsApplication1;

namespace about.Internal
{
	// Token: 0x02000004 RID: 4
	/// <summary>
	/// VB.NET 生成的 My.* 全局访问器（原名 WrapperProduct）：对外暴露 My.Computer（MyComputer→ProductProduct）、
	/// My.Application（MyApplication→ReponseProduct）、My.User（MyUser）、My.Forms（MyForms→FilterProduct，
	/// Form1/Form3 懒加载单例）、My.WebServices（MyWebServices→StatusProduct）。
	/// 业务入口 WrapperProduct.MyForms.Form1/.Form3 被 Form1/Form3 使用。无恶意逻辑，纯框架代码。
	/// </summary>
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	[HideModuleName]
	[StandardModule]
	internal sealed class WrapperProduct
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002564 File Offset: 0x00000764
		/// <summary>My.Computer 访问器（原名 RunReponse）→ ProductProduct 单例。</summary>
		[HelpKeyword("My.Computer")]
		internal static ProductProduct MyComputer
		{
			get
			{
				return WrapperProduct._ModelProduct.GetReponse();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000257C File Offset: 0x0000077C
		/// <summary>My.Application 访问器（原名 ResetReponse）→ ReponseProduct 单例。</summary>
		[HelpKeyword("My.Application")]
		internal static ReponseProduct MyApplication
		{
			get
			{
				return WrapperProduct.m_RepositoryProduct.GetReponse();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002594 File Offset: 0x00000794
		/// <summary>My.User 访问器（原名 PatchReponse）→ User 单例。</summary>
		[HelpKeyword("My.User")]
		internal static User MyUser
		{
			get
			{
				return WrapperProduct._DicProduct.GetReponse();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000025AC File Offset: 0x000007AC
		/// <summary>My.Forms 访问器（原名 SearchReponse）→ FilterProduct（Form1/Form3 单例管理器）。</summary>
		[HelpKeyword("My.Forms")]
		internal static WrapperProduct.FilterProduct MyForms
		{
			get
			{
				return WrapperProduct.m_AdapterProduct.GetReponse();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000025C4 File Offset: 0x000007C4
		/// <summary>My.WebServices 访问器（原名 RegisterReponse）→ StatusProduct（空集合）。</summary>
		[HelpKeyword("My.WebServices")]
		internal static WrapperProduct.StatusProduct MyWebServices
		{
			get
			{
				return WrapperProduct.m_StubProduct.GetReponse();
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021C6 File Offset: 0x000003C6
		/// <summary>混淆填充桩（原名 StartListener），恒 true，无实义。</summary>
		internal static bool StartListener()
		{
			return true;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000021C9 File Offset: 0x000003C9
		/// <summary>混淆填充桩（原名 ReflectListener），恒 false，无实义。</summary>
		internal static bool ReflectListener()
		{
			return false;
		}

		// Token: 0x04000001 RID: 1
		/// <summary>My.Computer 单例持有器（BroadcasterProduct&lt;ProductProduct&gt;）。</summary>
		private static readonly WrapperProduct.BroadcasterProduct<ProductProduct> _ModelProduct = new WrapperProduct.BroadcasterProduct<ProductProduct>();

		// Token: 0x04000002 RID: 2
		/// <summary>My.Application 单例持有器（BroadcasterProduct&lt;ReponseProduct&gt;）。</summary>
		private static readonly WrapperProduct.BroadcasterProduct<ReponseProduct> m_RepositoryProduct = new WrapperProduct.BroadcasterProduct<ReponseProduct>();

		// Token: 0x04000003 RID: 3
		/// <summary>My.User 单例持有器（BroadcasterProduct&lt;User&gt;）。</summary>
		private static readonly WrapperProduct.BroadcasterProduct<User> _DicProduct = new WrapperProduct.BroadcasterProduct<User>();

		// Token: 0x04000004 RID: 4
		/// <summary>My.Forms 单例持有器（BroadcasterProduct&lt;FilterProduct&gt;）。</summary>
		private static WrapperProduct.BroadcasterProduct<WrapperProduct.FilterProduct> m_AdapterProduct = new WrapperProduct.BroadcasterProduct<WrapperProduct.FilterProduct>();

		// Token: 0x04000005 RID: 5
		/// <summary>My.WebServices 单例持有器（BroadcasterProduct&lt;StatusProduct&gt;）。</summary>
		private static readonly WrapperProduct.BroadcasterProduct<WrapperProduct.StatusProduct> m_StubProduct = new WrapperProduct.BroadcasterProduct<WrapperProduct.StatusProduct>();

		// Token: 0x02000005 RID: 5
		/// <summary>
		/// My.Forms 实现（原名 FilterProduct）：Form1/Form3 懒加载单例管理。
		/// Form1 单例字段 form1Singleton（原名 m_ProxyProduct）、Form3 单例字段 form3Singleton（原名 tagProduct）。
		/// </summary>
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class FilterProduct
		{
			// Token: 0x17000006 RID: 6
			// (get) Token: 0x06000011 RID: 17 RVA: 0x000025DC File Offset: 0x000007DC
			// (set) Token: 0x06000013 RID: 19 RVA: 0x00002624 File Offset: 0x00000824
			/// <summary>Form1 单例访问器：存活则返回，否则经 GetOrCreateFormInstance 创建；置 null 则释放。</summary>
			public Form1 Form1
			{
				get
				{
					this.form1Singleton = WrapperProduct.FilterProduct.GetOrCreateFormInstance<Form1>(this.form1Singleton);
					return this.form1Singleton;
				}
				set
				{
					if (value == this.form1Singleton)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.ReleaseFormInstance<Form1>(ref this.form1Singleton);
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000012 RID: 18 RVA: 0x00002600 File Offset: 0x00000800
			// (set) Token: 0x06000014 RID: 20 RVA: 0x00002658 File Offset: 0x00000858
			/// <summary>Form3 单例访问器：存活则返回，否则经 GetOrCreateFormInstance 创建；置 null 则释放。</summary>
			public Form3 Form3
			{
				get
				{
					this.form3Singleton = WrapperProduct.FilterProduct.GetOrCreateFormInstance<Form3>(this.form3Singleton);
					return this.form3Singleton;
				}
				set
				{
					if (value == this.form3Singleton)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.ReleaseFormInstance<Form3>(ref this.form3Singleton);
				}
			}

			// Token: 0x06000015 RID: 21 RVA: 0x0000268C File Offset: 0x0000088C
			/// <summary>
			/// 获取或创建窗体单例（原名 MoveReponse&lt;T&gt;）：存活且未 Dispose 直接返回；
			/// 否则用 ThreadStatic Hashtable（instancesBeingCreated）标记防递归创建，Activator.CreateInstance 实例化，
			/// 异常时包装 InnerException 抛出（WinForms_SeeInnerException）。
			/// </summary>
			private static T GetOrCreateFormInstance<T>(T Instance) where T : Form, new()
			{
				if (Instance != null && !Instance.IsDisposed)
				{
					return Instance;
				}
				if (WrapperProduct.FilterProduct.instancesBeingCreated != null)
				{
					if (WrapperProduct.FilterProduct.instancesBeingCreated.ContainsKey(typeof(T)))
					{
						throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
					}
				}
				else
				{
					WrapperProduct.FilterProduct.instancesBeingCreated = new Hashtable();
				}
				WrapperProduct.FilterProduct.instancesBeingCreated.Add(typeof(T), null);
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
					WrapperProduct.FilterProduct.instancesBeingCreated.Remove(typeof(T));
				}
				return result;
			}

			// Token: 0x06000016 RID: 22 RVA: 0x00002794 File Offset: 0x00000994
			/// <summary>释放窗体单例（原名 InvokeReponse&lt;T&gt;）：Dispose 并置默认值。</summary>
			private void ReleaseFormInstance<T>(ref T gparam_0) where T : Form
			{
				gparam_0.Dispose();
				gparam_0 = default(T);
			}

			// Token: 0x06000017 RID: 23 RVA: 0x000021D4 File Offset: 0x000003D4
			/// <summary>构造函数（生成代码）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public FilterProduct()
			{
			}

			// Token: 0x06000018 RID: 24 RVA: 0x000027BC File Offset: 0x000009BC
			/// <summary>重写 Equals（生成代码）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000019 RID: 25 RVA: 0x000027D8 File Offset: 0x000009D8
			/// <summary>重写 GetHashCode（生成代码）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x0600001A RID: 26 RVA: 0x000027EC File Offset: 0x000009EC
			/// <summary>返回自身类型（生成代码，原名 EnableReponse，My.Forms 集合机制用）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type EnableReponse()
			{
				return typeof(WrapperProduct.FilterProduct);
			}

			// Token: 0x0600001B RID: 27 RVA: 0x00002804 File Offset: 0x00000A04
			/// <summary>重写 ToString（生成代码）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x0600001C RID: 28 RVA: 0x000021C6 File Offset: 0x000003C6
			/// <summary>混淆填充桩（原名 ValidateListener），恒 true，无实义。</summary>
			internal static bool ValidateListener()
			{
				return true;
			}

			// Token: 0x04000006 RID: 6
			/// <summary>Form1 单例字段（原名 m_ProxyProduct）。</summary>
			public Form1 form1Singleton;

			// Token: 0x04000007 RID: 7
			/// <summary>Form3 单例字段（原名 tagProduct）。</summary>
			public Form3 form3Singleton;

			// Token: 0x04000008 RID: 8
			/// <summary>创建中窗体防递归表（ThreadStatic Hashtable，原名 _InstanceProduct）。</summary>
			[ThreadStatic]
			private static Hashtable instancesBeingCreated;
		}

		// Token: 0x02000006 RID: 6
		/// <summary>
		/// My.WebServices 实现（原名 StatusProduct）：空集合（本程序无 WebService），含 Create/Dispose 机制桩。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		internal sealed class StatusProduct
		{
			// Token: 0x0600001D RID: 29 RVA: 0x000027BC File Offset: 0x000009BC
			/// <summary>重写 Equals（生成代码）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600001E RID: 30 RVA: 0x000027D8 File Offset: 0x000009D8
			/// <summary>重写 GetHashCode（生成代码）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x0600001F RID: 31 RVA: 0x00002818 File Offset: 0x00000A18
			/// <summary>返回自身类型（生成代码，原名 SetReponse，My.WebServices 集合机制用）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type SetReponse()
			{
				return typeof(WrapperProduct.StatusProduct);
			}

			// Token: 0x06000020 RID: 32 RVA: 0x00002804 File Offset: 0x00000A04
			/// <summary>重写 ToString（生成代码）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x06000021 RID: 33 RVA: 0x00002830 File Offset: 0x00000A30
			/// <summary>获取或创建实例（生成代码，原名 StopReponse&lt;T&gt;，对应 Create__Instance__ 机制）。</summary>
			private static T StopReponse<T>(T instance) where T : new()
			{
				if (instance == null)
				{
					return Activator.CreateInstance<T>();
				}
				return instance;
			}

			// Token: 0x06000022 RID: 34 RVA: 0x0000284C File Offset: 0x00000A4C
			/// <summary>置空（生成代码，原名 AddReponse&lt;T&gt;，对应 Dispose__Instance__ 机制）。</summary>
			private void AddReponse<T>(ref T gparam_0)
			{
				gparam_0 = default(T);
			}

			// Token: 0x06000023 RID: 35 RVA: 0x000021D4 File Offset: 0x000003D4
			/// <summary>构造函数（生成代码）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public StatusProduct()
			{
			}

			// Token: 0x06000024 RID: 36 RVA: 0x000021C6 File Offset: 0x000003C6
			/// <summary>混淆填充桩（原名 LoginListener），恒 true，无实义。</summary>
			internal static bool LoginListener()
			{
				return true;
			}

			// Token: 0x06000025 RID: 37 RVA: 0x000021C9 File Offset: 0x000003C9
			/// <summary>混淆填充桩（原名 VisitListener），恒 false，无实义。</summary>
			internal static bool VisitListener()
			{
				return false;
			}
		}

		// Token: 0x02000007 RID: 7
		/// <summary>
		/// 线程静态单例持有器（原名 BroadcasterProduct&lt;T&gt;）：GetReponse 惰性创建并缓存 T 实例，供 My.* 访问器使用。
		/// </summary>
		[ComVisible(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class BroadcasterProduct<T> where T : new()
		{
			// Token: 0x06000026 RID: 38 RVA: 0x00002868 File Offset: 0x00000A68
			/// <summary>获取单例：_ClassProduct 为空则 Activator.CreateInstance 后缓存返回。</summary>
			internal T GetReponse()
			{
				if (WrapperProduct.BroadcasterProduct<T>._ClassProduct == null)
				{
					WrapperProduct.BroadcasterProduct<T>._ClassProduct = Activator.CreateInstance<T>();
				}
				return WrapperProduct.BroadcasterProduct<T>._ClassProduct;
			}

			// Token: 0x06000027 RID: 39 RVA: 0x000021D4 File Offset: 0x000003D4
			/// <summary>构造函数（生成代码）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public BroadcasterProduct()
			{
			}

			// Token: 0x06000028 RID: 40 RVA: 0x000021C6 File Offset: 0x000003C6
			/// <summary>混淆填充桩（原名 RemoveListener），恒 true，无实义。</summary>
			internal static bool RemoveListener()
			{
				return true;
			}

			// Token: 0x06000029 RID: 41 RVA: 0x000021C9 File Offset: 0x000003C9
			/// <summary>混淆填充桩（原名 NewListener），恒 false，无实义。</summary>
			internal static bool NewListener()
			{
				return false;
			}

			// Token: 0x04000009 RID: 9
			/// <summary>单例缓存字段（ThreadStatic，原名 _ClassProduct）。</summary>
			[CompilerGenerated]
			[ThreadStatic]
			private static T _ClassProduct;
		}
	}
}
