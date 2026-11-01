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

namespace set.Internal
{
	/// <summary>
	/// VB.NET「My」命名空间的自动生成聚合入口（原模板就是 MyProject 模块）。
	/// 这里把五组 My.* 成员转成静态属性：My.Computer / My.Application / My.User / My.Forms / My.WebServices。
	/// 每个属性背后都是一个 <see cref="ManagerAttr{T}"/> 惰性单例持有器，第一次访问才 new。
	/// 除 <see cref="应用程序"/>（被 Main 与 My.Settings 使用）外，其余在本样本中基本没有实际用途。
	/// </summary>
	// Token: 0x02000004 RID: 4
	[GeneratedCode("MyTemplate", "11.0.0.0")]
	[HideModuleName]
	[StandardModule]
	internal sealed class CandidateAttr
	{
		/// <summary>My.Computer：键盘/鼠标/剪贴板/文件系统等系统访问入口。</summary>
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000021EB File Offset: 0x000003EB
		[HelpKeyword("My.Computer")]
		internal static AttrAttr 计算机
		{
			get
			{
				return CandidateAttr.计算机持有器.取单例();
			}
		}

		/// <summary>My.Application：当前应用框架实例（启动/关闭/设置保存都挂在它上面）。</summary>
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000021F7 File Offset: 0x000003F7
		[HelpKeyword("My.Application")]
		internal static InfoAttr 应用程序
		{
			get
			{
				return CandidateAttr.应用程序持有器.取单例();
			}
		}

		/// <summary>My.User：当前登录用户信息。</summary>
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002203 File Offset: 0x00000403
		[HelpKeyword("My.User")]
		internal static User 当前用户
		{
			get
			{
				return CandidateAttr.用户持有器.取单例();
			}
		}

		/// <summary>My.Forms：本工程三个窗体的惰性单例集合（Form1 / Form3 / load）。</summary>
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000220F File Offset: 0x0000040F
		[HelpKeyword("My.Forms")]
		internal static CandidateAttr.ServerAttr 窗体集合
		{
			get
			{
				return CandidateAttr.窗体集合持有器.取单例();
			}
		}

		/// <summary>My.WebServices：工程内没有引用任何 Web 服务，这里永远是空集合。</summary>
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000221B File Offset: 0x0000041B
		[HelpKeyword("My.WebServices")]
		internal static CandidateAttr.MessageAttr Web服务
		{
			get
			{
				return CandidateAttr.Web服务持有器.取单例();
			}
		}

		/// <summary>恒返回 false 的死代码桩，无调用点。</summary>
		// Token: 0x0600000D RID: 13 RVA: 0x000021E8 File Offset: 0x000003E8
		internal static bool 恒假桩()
		{
			return false;
		}

		// Token: 0x04000001 RID: 1
		private static readonly CandidateAttr.ManagerAttr<AttrAttr> 计算机持有器 = new CandidateAttr.ManagerAttr<AttrAttr>();

		// Token: 0x04000002 RID: 2
		private static readonly CandidateAttr.ManagerAttr<InfoAttr> 应用程序持有器 = new CandidateAttr.ManagerAttr<InfoAttr>();

		// Token: 0x04000003 RID: 3
		private static readonly CandidateAttr.ManagerAttr<User> 用户持有器 = new CandidateAttr.ManagerAttr<User>();

		// Token: 0x04000004 RID: 4
		private static CandidateAttr.ManagerAttr<CandidateAttr.ServerAttr> 窗体集合持有器 = new CandidateAttr.ManagerAttr<CandidateAttr.ServerAttr>();

		// Token: 0x04000005 RID: 5
		private static readonly CandidateAttr.ManagerAttr<CandidateAttr.MessageAttr> Web服务持有器 = new CandidateAttr.ManagerAttr<CandidateAttr.MessageAttr>();

		/// <summary>
		/// My.Forms 的实现体：窗体级单例池 + 递归创建保护。
		/// 特性里的 <c>"Create__Instance__" / "Dispose__Instance__"</c> 是 VB 编译器约定的模板方法名，
		/// 对应的就是本类里已改名的 <see cref="创建窗体单例{T}"/> / <see cref="释放窗体单例{T}"/>。
		/// </summary>
		// Token: 0x02000005 RID: 5
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class ServerAttr
		{
			/// <summary>
			/// 取窗体单例：实例存在且未释放就复用；否则 new 一个。
			/// 防递归：创建期间把类型记进 ThreadStatic 的 <see cref="创建中窗体表"/>，
			/// 若同一线程上同一类型被要求第二次创建，就抛 InvalidOperationException（WinForms_RecursiveFormCreate）。
			/// 构造函数内的异常被包成 InvalidOperationException（WinForms_SeeInnerException）后重抛。
			/// </summary>
			/// <param name="Instance">已有实例，可能为 null 或已 Dispose。</param>
			// Token: 0x0600000E RID: 14 RVA: 0x000029BC File Offset: 0x00000BBC
			private static T 创建窗体单例<T>(T Instance) where T : Form, new()
			{
				T 结果;
				if (Instance != null && !Instance.IsDisposed)
				{
					结果 = Instance;
				}
				else
				{
					if (CandidateAttr.ServerAttr.创建中窗体表 != null)
					{
						if (CandidateAttr.ServerAttr.创建中窗体表.ContainsKey(typeof(T)))
						{
							throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
						}
					}
					else
					{
						CandidateAttr.ServerAttr.创建中窗体表 = new Hashtable();
					}
					CandidateAttr.ServerAttr.创建中窗体表.Add(typeof(T), null);
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
						CandidateAttr.ServerAttr.创建中窗体表.Remove(typeof(T));
					}
				}
				return 结果;
			}

			/// <summary>释放窗体单例并置空引用（VB 的 Dispose__Instance__）。</summary>
			/// <param name="实例">要释放的窗体引用。</param>
			// Token: 0x0600000F RID: 15 RVA: 0x00002227 File Offset: 0x00000427
			private void 释放窗体单例<T>(ref T 实例) where T : Form
			{
				实例.Dispose();
				实例 = default(T);
			}

			// Token: 0x06000010 RID: 16 RVA: 0x0000223C File Offset: 0x0000043C
			[EditorBrowsable(EditorBrowsableState.Never)]
			public ServerAttr()
			{
			}

			// Token: 0x06000011 RID: 17 RVA: 0x00002244 File Offset: 0x00000444
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000012 RID: 18 RVA: 0x00002252 File Offset: 0x00000452
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			/// <summary>返回本集合类型（VB 模板生成，供设计器识别）。</summary>
			// Token: 0x06000013 RID: 19 RVA: 0x0000225A File Offset: 0x0000045A
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type 取类型()
			{
				return typeof(CandidateAttr.ServerAttr);
			}

			// Token: 0x06000014 RID: 20 RVA: 0x00002266 File Offset: 0x00000466
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			/// <summary>My.Forms.Form1：主设置窗体（机房管理助手设置界面）的惰性单例。</summary>
			// Token: 0x17000006 RID: 6
			// (get) Token: 0x06000015 RID: 21 RVA: 0x0000226E File Offset: 0x0000046E
			// (set) Token: 0x06000018 RID: 24 RVA: 0x00002AC0 File Offset: 0x00000CC0
			public Form1 Form1
			{
				get
				{
					this.form1实例 = CandidateAttr.ServerAttr.创建窗体单例<Form1>(this.form1实例);
					return this.form1实例;
				}
				set
				{
					if (value != this.form1实例)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体单例<Form1>(ref this.form1实例);
					}
				}
			}

			/// <summary>My.Forms.Form3：版权/关于窗体的惰性单例。</summary>
			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000016 RID: 22 RVA: 0x00002287 File Offset: 0x00000487
			// (set) Token: 0x06000019 RID: 25 RVA: 0x00002AF0 File Offset: 0x00000CF0
			public Form3 Form3
			{
				get
				{
					this.form3实例 = CandidateAttr.ServerAttr.创建窗体单例<Form3>(this.form3实例);
					return this.form3实例;
				}
				set
				{
					if (value != this.form3实例)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体单例<Form3>(ref this.form3实例);
					}
				}
			}

			/// <summary>My.Forms.load：启动时的载入/卡密验证窗体的惰性单例。</summary>
			// Token: 0x17000008 RID: 8
			// (get) Token: 0x06000017 RID: 23 RVA: 0x000022A0 File Offset: 0x00000D20
			// (set) Token: 0x0600001A RID: 26 RVA: 0x00002B20 File Offset: 0x00000D20
			public load load
			{
				get
				{
					this.load实例 = CandidateAttr.ServerAttr.创建窗体单例<load>(this.load实例);
					return this.load实例;
				}
				set
				{
					if (value != this.load实例)
					{
						if (value != null)
						{
							throw new ArgumentException("Property can only be set to Nothing");
						}
						this.释放窗体单例<load>(ref this.load实例);
					}
				}
			}

			/// <summary>正在创建中的窗体类型表（线程静态），用于检测构造函数的递归调用。</summary>
			// Token: 0x04000006 RID: 6
			[ThreadStatic]
			private static Hashtable 创建中窗体表;

			// Token: 0x04000007 RID: 7
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form1 form1实例;

			// Token: 0x04000008 RID: 8
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Form3 form3实例;

			// Token: 0x04000009 RID: 9
			[EditorBrowsable(EditorBrowsableState.Never)]
			public load load实例;
		}

		/// <summary>
		/// My.WebServices 的实现体。本工程没有引用任何 Web 服务，所以里面只有模板生成的
		/// Equals/GetHashCode/ToString 和两个私有辅助方法（创建/清空实例），没有实际成员。
		/// </summary>
		// Token: 0x02000006 RID: 6
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		internal sealed class MessageAttr
		{
			// Token: 0x0600001B RID: 27 RVA: 0x00002244 File Offset: 0x00000444
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600001C RID: 28 RVA: 0x00002252 File Offset: 0x00000452
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			/// <summary>返回本集合类型（VB 模板生成）。</summary>
			// Token: 0x0600001D RID: 29 RVA: 0x000022B9 File Offset: 0x000004B9
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type 取类型()
			{
				return typeof(CandidateAttr.MessageAttr);
			}

			// Token: 0x0600001E RID: 30 RVA: 0x00002266 File Offset: 0x00000466
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			/// <summary>null 就新建一个实例，否则复用（VB 的 Create__Instance__）。</summary>
			/// <param name="instance">已有实例或 null。</param>
			// Token: 0x0600001F RID: 31 RVA: 0x00002B54 File Offset: 0x00000D54
			private static T 取非空实例<T>(T instance) where T : new()
			{
				T 结果;
				if (instance == null)
				{
					结果 = new T();
				}
				else
				{
					结果 = instance;
				}
				return 结果;
			}

			/// <summary>把引用置空（VB 的 Dispose__Instance__，Web 服务没有可释放资源）。</summary>
			/// <param name="实例">要置空的引用。</param>
			// Token: 0x06000020 RID: 32 RVA: 0x000022C5 File Offset: 0x000004C5
			private void 清空引用<T>(ref T 实例)
			{
				实例 = default(T);
			}

			// Token: 0x06000021 RID: 33 RVA: 0x0000223C File Offset: 0x0000043C
			[EditorBrowsable(EditorBrowsableState.Never)]
			public MessageAttr()
			{
			}

			/// <summary>恒返回 true 的死代码桩，无调用点。</summary>
			// Token: 0x06000022 RID: 34 RVA: 0x000021E5 File Offset: 0x000003E5
			internal static bool 恒真桩()
			{
				return true;
			}

			/// <summary>恒返回 false 的死代码桩，无调用点。</summary>
			// Token: 0x06000023 RID: 35 RVA: 0x000021E8 File Offset: 0x000003E8
			internal static bool 恒假桩()
			{
				return false;
			}
		}

		/// <summary>
		/// My.* 成员共用的惰性单例持有器。<see cref="取单例"/> 第一次调用时 new 一个 T，
		/// 之后一直返回同一个实例（没有加锁，VB 模板原样如此）。
		/// </summary>
		/// <typeparam name="T">被持有的类型，需有无参构造函数。</typeparam>
		// Token: 0x02000007 RID: 7
		[EditorBrowsable(EditorBrowsableState.Never)]
		[ComVisible(false)]
		internal sealed class ManagerAttr<T> where T : new()
		{
			/// <summary>取单例：字段为空则创建，否则复用。</summary>
			// Token: 0x06000024 RID: 36 RVA: 0x000022CE File Offset: 0x000004CE
			internal T 取单例()
			{
				if (CandidateAttr.ManagerAttr<T>.单例 == null)
				{
					CandidateAttr.ManagerAttr<T>.单例 = new T();
				}
				return CandidateAttr.ManagerAttr<T>.单例;
			}

			// Token: 0x06000025 RID: 37 RVA: 0x0000223C File Offset: 0x0000043C
			[EditorBrowsable(EditorBrowsableState.Never)]
			public ManagerAttr()
			{
			}

			/// <summary>恒返回 true 的死代码桩，无调用点。</summary>
			// Token: 0x06000026 RID: 38 RVA: 0x000021E5 File Offset: 0x000003E5
			internal static bool 恒真桩()
			{
				return true;
			}

			/// <summary>恒返回 false 的死代码桩，无调用点。</summary>
			// Token: 0x06000027 RID: 39 RVA: 0x000021E8 File Offset: 0x000003E8
			internal static bool 恒假桩()
			{
				return false;
			}

			// Token: 0x0400000A RID: 10
			[ThreadStatic]
			[CompilerGenerated]
			private static T 单例;
		}
	}
}
