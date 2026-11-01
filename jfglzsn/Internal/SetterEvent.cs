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

namespace jfglzsn.Internal
{
	// Token: 0x02000004 RID: 4
	/// <summary>
	/// VB.NET 模板的「My」命名空间聚合类（等价于 <c>MyProject</c>）：
	/// 对外暴露 My.Computer / My.Application / My.User / My.Forms / My.WebServices 五个入口。
	/// <para>
	/// 这一层没有业务逻辑，全部是模板样板：每个入口都由 <see cref="ResolverSetter{T}"/>
	/// 懒加载并缓存单例。<c>My.Forms</c> 那一份特殊，走 <see cref="PolicySetter"/> 的
	/// 「窗体按需创建 + 递归创建检测 + 支持置空释放」逻辑。
	/// </para>
	/// <para>
	/// 另外还夹着 8 个恒返回 <c>true</c>/<c>false</c> 的判定桩（<c>xxxDefinition</c>/<c>xxxRule</c>），
	/// 分散在三个嵌套类型里，工程内无任何引用——壳留下的噪声。
	/// </para>
	/// </summary>
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[HideModuleName]
	[StandardModule]
	internal sealed class SetterEvent
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021EB File Offset: 0x000003EB
		/// <summary>My.Computer：返回 <see cref="EventEvent"/> 单例（内含未使用的判定桩）。</summary>
		[HelpKeyword("My.Computer")]
		internal static EventEvent CalculateCallback
		{
			get
			{
				return SetterEvent._FactoryEvent.CallEvent();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000021F7 File Offset: 0x000003F7
		/// <summary>
		/// My.Application：返回 <see cref="CallbackEvent"/> 单例。注意它同时是启动框架本体，
		/// 直接访问这个属性就会创建应用对象。
		/// </summary>
		[HelpKeyword("My.Application")]
		internal static CallbackEvent CollectCallback
		{
			get
			{
				return SetterEvent._HelperEvent.CallEvent();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002203 File Offset: 0x00000403
		/// <summary>My.User：返回当前 Windows 用户对象单例。</summary>
		[HelpKeyword("My.User")]
		internal static User ConnectCallback
		{
			get
			{
				return SetterEvent.mockEvent.CallEvent();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000220F File Offset: 0x0000040F
		/// <summary>My.Forms：返回全部窗体的按需创建容器（Form1~Form4 都从这里取单例）。</summary>
		[HelpKeyword("My.Forms")]
		internal static SetterEvent.PolicySetter ComputeCallback
		{
			get
			{
				return SetterEvent.m_ComparatorEvent.CallEvent();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x0000221B File Offset: 0x0000041B
		/// <summary>My.WebServices：返回 Web 服务容器（本工程未声明任何 Web 服务，是个空壳）。</summary>
		[HelpKeyword("My.WebServices")]
		internal static SetterEvent.CustomerSetter AssetCallback
		{
			get
			{
				return SetterEvent.m_ErrorEvent.CallEvent();
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021DD File Offset: 0x000003DD
		/// <summary>模板/壳插入的判定桩，恒返回 <c>true</c>，工程内无引用。</summary>
		internal static bool RegisterDefinition()
		{
			return true;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000021E0 File Offset: 0x000003E0
		/// <summary>模板/壳插入的判定桩，恒返回 <c>false</c>，工程内无引用。</summary>
		internal static bool ExcludeDefinition()
		{
			return false;
		}

		// Token: 0x04000001 RID: 1
		/// <summary>My.Computer 的懒加载容器。</summary>
		private static readonly SetterEvent.ResolverSetter<EventEvent> _FactoryEvent = new SetterEvent.ResolverSetter<EventEvent>();

		// Token: 0x04000002 RID: 2
		/// <summary>My.Application 的懒加载容器。</summary>
		private static readonly SetterEvent.ResolverSetter<CallbackEvent> _HelperEvent = new SetterEvent.ResolverSetter<CallbackEvent>();

		// Token: 0x04000003 RID: 3
		/// <summary>My.User 的懒加载容器。</summary>
		private static readonly SetterEvent.ResolverSetter<User> mockEvent = new SetterEvent.ResolverSetter<User>();

		// Token: 0x04000004 RID: 4
		/// <summary>My.Forms 的懒加载容器。注意这个字段**不是** readonly，与原版一致。</summary>
		private static SetterEvent.ResolverSetter<SetterEvent.PolicySetter> m_ComparatorEvent = new SetterEvent.ResolverSetter<SetterEvent.PolicySetter>();

		// Token: 0x04000005 RID: 5
		/// <summary>My.WebServices 的懒加载容器。</summary>
		private static readonly SetterEvent.ResolverSetter<SetterEvent.CustomerSetter> m_ErrorEvent = new SetterEvent.ResolverSetter<SetterEvent.CustomerSetter>();

		// Token: 0x02000005 RID: 5
		/// <summary>
		/// My.Forms 容器：按需创建 Form1~Form4 并缓存，属性 setter 只接受 <c>null</c>（表示释放并置空）。
		/// 与 VB 模板生成的 <c>MyForms</c> 完全一致。
		/// </summary>
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class PolicySetter
		{
			// Token: 0x06000011 RID: 17 RVA: 0x00002774 File Offset: 0x00000974
			/// <summary>
			/// 取窗体实例：已存在且未释放则复用；否则新建。
			/// <para>
			/// 新建期间会把类型登记进 <see cref="reponseSetter"/>（<c>[ThreadStatic]</c>，按线程隔离），
			/// 若同一线程在构造过程中又请求同类窗体，判定为递归创建并抛
			/// <c>WinForms_RecursiveFormCreate</c>；构造抛出的
			/// <see cref="TargetInvocationException"/> 会被拆包重抛内层异常；无论成败都在 finally 里销号。
			/// </para>
			/// </summary>
			/// <param name="实例">当前缓存的实例，可能为 <c>null</c> 或已释放。</param>
			/// <returns>可用的窗体实例。</returns>
			private static T CalculateEvent<T>(T 实例) where T : Form, new()
			{
				T 结果实例;
				if (实例 != null && !实例.IsDisposed)
				{
					结果实例 = 实例;
				}
				else
				{
					if (SetterEvent.PolicySetter.reponseSetter != null)
					{
						if (SetterEvent.PolicySetter.reponseSetter.ContainsKey(typeof(T)))
						{
							throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
						}
					}
					else
					{
						SetterEvent.PolicySetter.reponseSetter = new Hashtable();
					}
					SetterEvent.PolicySetter.reponseSetter.Add(typeof(T), null);
					try
					{
						结果实例 = new T();
					}
					catch (TargetInvocationException ex) when (ex.InnerException != null)
					{
						throw new InvalidOperationException(Utils.GetResourceString("WinForms_SeeInnerException", new string[] { ex.InnerException.Message }), ex.InnerException);
					}
					finally
					{
						SetterEvent.PolicySetter.reponseSetter.Remove(typeof(T));
					}
				}
				return 结果实例;
			}

			// Token: 0x06000012 RID: 18 RVA: 0x00002227 File Offset: 0x00000427
			/// <summary>释放窗体并把缓存字段置空（My.Forms 属性 setter 传 <c>null</c> 时走这里）。</summary>
			/// <param name="引用">窗体字段的引用。</param>
			private void RegisterEvent<T>(ref T 引用) where T : Form
			{
				引用.Dispose();
				引用 = default(T);
			}

			// Token: 0x06000013 RID: 19 RVA: 0x0000223C File Offset: 0x0000043C
			/// <summary>模板默认构造。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public PolicySetter()
			{
			}

			// Token: 0x06000014 RID: 20 RVA: 0x00002244 File Offset: 0x00000444
			/// <summary>模板样板：直接转发给基类比较。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000015 RID: 21 RVA: 0x00002252 File Offset: 0x00000452
			/// <summary>模板样板：直接转发给基类。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000016 RID: 22 RVA: 0x0000225A File Offset: 0x0000045A
			/// <summary>模板样板：返回本容器类型。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type CollectEvent()
			{
				return typeof(SetterEvent.PolicySetter);
			}

			// Token: 0x06000017 RID: 23 RVA: 0x00002266 File Offset: 0x00000466
			/// <summary>模板样板：转发基类 <c>ToString</c>。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x17000006 RID: 6
			// (get) Token: 0x06000018 RID: 24 RVA: 0x0000226E File Offset: 0x00000A6E
			// (set) Token: 0x0600001C RID: 28 RVA: 0x00002878 File Offset: 0x00000A78
			/// <summary>主窗体单例访问器：取值时按需创建，赋 <c>null</c> 时释放。</summary>
			public Form1 Form1
			{
				get
				{
					this.m_OrderSetter = SetterEvent.PolicySetter.CalculateEvent<Form1>(this.m_OrderSetter);
					return this.m_OrderSetter;
				}
				set
				{
					if (value != this.m_OrderSetter)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.RegisterEvent<Form1>(ref this.m_OrderSetter);
					}
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000019 RID: 25 RVA: 0x00002287 File Offset: 0x00000A87
			// (set) Token: 0x0600001D RID: 29 RVA: 0x000028A8 File Offset: 0x00000AA8
			/// <summary>窗体 Form2 单例访问器：取值时按需创建，赋 <c>null</c> 时释放。</summary>
			public Form2 Form2
			{
				get
				{
					this._IteratorSetter = SetterEvent.PolicySetter.CalculateEvent<Form2>(this._IteratorSetter);
					return this._IteratorSetter;
				}
				set
				{
					if (value != this._IteratorSetter)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.RegisterEvent<Form2>(ref this._IteratorSetter);
					}
				}
			}

			// Token: 0x17000008 RID: 8
			// (get) Token: 0x0600001A RID: 26 RVA: 0x000022A0 File Offset: 0x00000AA0
			// (set) Token: 0x0600001E RID: 30 RVA: 0x000028D8 File Offset: 0x00000AD8
			/// <summary>窗体 Form3 单例访问器：取值时按需创建，赋 <c>null</c> 时释放。</summary>
			public Form3 Form3
			{
				get
				{
					this.parameterSetter = SetterEvent.PolicySetter.CalculateEvent<Form3>(this.parameterSetter);
					return this.parameterSetter;
				}
				set
				{
					if (value != this.parameterSetter)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.RegisterEvent<Form3>(ref this.parameterSetter);
					}
				}
			}

			// Token: 0x17000009 RID: 9
			// (get) Token: 0x0600001B RID: 27 RVA: 0x000022B9 File Offset: 0x00000AB9
			// (set) Token: 0x0600001F RID: 31 RVA: 0x0000290C File Offset: 0x00000B0C
			/// <summary>窗体 Form4（全屏锁屏窗体）单例访问器：取值时按需创建，赋 <c>null</c> 时释放。</summary>
			public Form4 Form4
			{
				get
				{
					this.m_SpecificationSetter = SetterEvent.PolicySetter.CalculateEvent<Form4>(this.m_SpecificationSetter);
					return this.m_SpecificationSetter;
				}
				set
				{
					if (value != this.m_SpecificationSetter)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.RegisterEvent<Form4>(ref this.m_SpecificationSetter);
					}
				}
			}

			// Token: 0x04000006 RID: 6
			/// <summary>「正在构造中」的类型登记表，<c>[ThreadStatic]</c>，用于检测递归创建窗体。</summary>
			[ThreadStatic]
			private static Hashtable reponseSetter;

			// Token: 0x04000007 RID: 7
			/// <summary>Form1 的缓存字段（模板规定为 public 字段，勿改成私有属性）。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form1 m_OrderSetter;

			// Token: 0x04000008 RID: 8
			/// <summary>Form2 的缓存字段。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form2 _IteratorSetter;

			// Token: 0x04000009 RID: 9
			/// <summary>Form3 的缓存字段。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form3 parameterSetter;

			// Token: 0x0400000A RID: 10
			/// <summary>Form4 的缓存字段。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form4 m_SpecificationSetter;
		}

		// Token: 0x02000006 RID: 6
		/// <summary>
		/// My.WebServices 容器：模板生成的空容器，本工程未声明任何 Web 服务。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		internal sealed class CustomerSetter
		{
			// Token: 0x06000020 RID: 32 RVA: 0x00002244 File Offset: 0x00000444
			/// <summary>模板样板：转发基类比较。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000021 RID: 33 RVA: 0x00002252 File Offset: 0x00000452
			/// <summary>模板样板：转发基类。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000022 RID: 34 RVA: 0x000022D2 File Offset: 0x000004D2
			/// <summary>模板样板：返回本容器类型。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type QueryEvent()
			{
				return typeof(SetterEvent.CustomerSetter);
			}

			// Token: 0x06000023 RID: 35 RVA: 0x00002266 File Offset: 0x00000466
			/// <summary>模板样板：转发基类 <c>ToString</c>。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x06000024 RID: 36 RVA: 0x0000293C File Offset: 0x00000B3C
			/// <summary>模板样板：没有实例就新建一个（Web 服务代理的「取实例」逻辑）。</summary>
			/// <param name="实例">已缓存的实例，可为 <c>null</c>。</param>
			private static T FillEvent<T>(T 实例) where T : new()
			{
				T 结果实例;
				if (实例 == null)
				{
					结果实例 = new T();
				}
				else
				{
					结果实例 = 实例;
				}
				return 结果实例;
			}

			// Token: 0x06000025 RID: 37 RVA: 0x000022DE File Offset: 0x000004DE
			/// <summary>模板样板：把缓存的引用置空（不 Dispose，Web 代理无此需要）。</summary>
			/// <param name="引用">实例字段的引用。</param>
			private void AddEvent<T>(ref T 引用)
			{
				引用 = default(T);
			}

			// Token: 0x06000026 RID: 38 RVA: 0x0000223C File Offset: 0x0000043C
			/// <summary>模板默认构造。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public CustomerSetter()
			{
			}

			// Token: 0x06000027 RID: 39 RVA: 0x000021DD File Offset: 0x000003DD
			/// <summary>模板/壳插入的判定桩，恒返回 <c>true</c>，工程内无引用。</summary>
			internal static bool SetRule()
			{
				return true;
			}

			// Token: 0x06000028 RID: 40 RVA: 0x000021E0 File Offset: 0x000003E0
			/// <summary>模板/壳插入的判定桩，恒返回 <c>false</c>，工程内无引用。</summary>
			internal static bool IncludeRule()
			{
				return false;
			}
		}

		// Token: 0x02000007 RID: 7
		/// <summary>
		/// 通用懒加载单例容器：泛型静态字段 <c>[ThreadStatic]</c>，**每个泛型实例化、每个线程各一份**。
		/// My.Computer / My.Application / My.User / My.Forms / My.WebServices 都靠它延迟构造。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ComVisible(false)]
		internal sealed class ResolverSetter<T> where T : new()
		{
			// Token: 0x06000029 RID: 41 RVA: 0x000022E7 File Offset: 0x000004E7
			/// <summary>取单例，首次调用时构造并缓存。</summary>
			/// <returns><typeparamref name="T"/> 的单例。</returns>
			internal T CallEvent()
			{
				if (SetterEvent.ResolverSetter<T>.m_ConnectionSetter == null)
				{
					SetterEvent.ResolverSetter<T>.m_ConnectionSetter = new T();
				}
				return SetterEvent.ResolverSetter<T>.m_ConnectionSetter;
			}

			// Token: 0x0600002A RID: 42 RVA: 0x0000223C File Offset: 0x0000043C
			/// <summary>模板默认构造。</summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public ResolverSetter()
			{
			}

			// Token: 0x0600002B RID: 43 RVA: 0x000021DD File Offset: 0x000003DD
			/// <summary>模板/壳插入的判定桩，恒返回 <c>true</c>，工程内无引用。</summary>
			internal static bool MapRule()
			{
				return true;
			}

			// Token: 0x0600002C RID: 44 RVA: 0x000021E0 File Offset: 0x000003E0
			/// <summary>模板/壳插入的判定桩，恒返回 <c>false</c>，工程内无引用。</summary>
			internal static bool PatchRule()
			{
				return false;
			}

			// Token: 0x0400000B RID: 11
			/// <summary>单例缓存字段（<c>[ThreadStatic]</c> + <c>[CompilerGenerated]</c>，模板生成）。</summary>
			[CompilerGenerated]
			[ThreadStatic]
			private static T m_ConnectionSetter;
		}
	}
}
