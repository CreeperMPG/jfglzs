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

// ============================================================================
// Account.cs — VB.NET "My" 根对象宿主（My.Computer/My.Application/My.Forms 等单例容器）
// 真实职责：反混淆后为 VB.NET 编译器 My 项目根对象（MyTemplate 生成），ConfuserEx 将类名改为 Account、
// 属性改为 MyComputer/MyApplication 等假名。提供窗体单例（My.Forms → MyForms，全程序窗体工厂）与
// 系统对象访问点：Form1/Form2/Form3/Form4/bianhao 全部通过 Account.MyForms.XXX 访问窗体实例。
// 说明：类名保持混淆原名（规则：只重命名成员），方法/字段已按 Account.cs.md 映射表反混淆重命名。
// ============================================================================
namespace gypZRX1vuWc3aeye0R.Internal
{
	// Token: 0x02000004 RID: 4
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	[StandardModule]
	[HideModuleName]
	internal sealed class Account
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000025A0 File Offset: 0x000007A0
/// <summary>
/// My.Computer：返回 Schema 单例（继承 Microsoft.VisualBasic.Devices.Computer）；
/// bianhao 用它取计算机名（Account.MyComputer.Name）。
/// </summary>
		[HelpKeyword("My.Computer")]
		internal static Schema MyComputer
		{
			get
			{
				return Account.computer单例.获取单例();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000025B8 File Offset: 0x000007B8
/// <summary>
/// My.Application：返回 Broadcaster 单例（WindowsFormsApplicationBase）；
/// MySettings 通过它订阅 Shutdown 事件以自动保存设置。
/// </summary>
		[HelpKeyword("My.Application")]
		internal static Broadcaster MyApplication
		{
			get
			{
				return Account.application单例.获取单例();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000025D0 File Offset: 0x000007D0
/// <summary>My.User：返回 VB 用户对象单例（本项目未使用）。</summary>
		[HelpKeyword("My.User")]
		internal static User MyUser
		{
			get
			{
				return Account.user单例.获取单例();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000025E8 File Offset: 0x000007E8
/// <summary>
/// My.Forms：返回 Importer（窗体工厂），全程序 Form1-4/bianhao 懒加载单例的访问入口。
/// </summary>
		[HelpKeyword("My.Forms")]
		internal static Account.Importer MyForms
		{
			get
			{
				return Account.forms单例.获取单例();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002600 File Offset: 0x00000800
/// <summary>My.WebServices：返回 Container（WebService 占位容器，本项目为空壳）。</summary>
		[HelpKeyword("My.WebServices")]
		internal static Account.Container MyWebServices
		{
			get
			{
				return Account.webServices单例.获取单例();
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>混淆器填充方法（恒返回 true，无意义）。</summary>
		internal static bool ReflectThread()
		{
			return true;
		}

		// Token: 0x04000001 RID: 1
/// <summary>My 单例字段：My.Computer（Schema 模板单例）。</summary>
		private static readonly Account.Template<Schema> computer单例 = new Account.Template<Schema>();

		// Token: 0x04000002 RID: 2
/// <summary>My 单例字段：My.Application（Broadcaster 模板单例）。</summary>
		private static readonly Account.Template<Broadcaster> application单例 = new Account.Template<Broadcaster>();

		// Token: 0x04000003 RID: 3
/// <summary>My 单例字段：My.User（User 模板单例）。</summary>
		private static readonly Account.Template<User> user单例 = new Account.Template<User>();

		// Token: 0x04000004 RID: 4
/// <summary>My 单例字段：My.Forms（Importer 模板单例，全程序窗体工厂）。</summary>
		private static Account.Template<Account.Importer> forms单例 = new Account.Template<Account.Importer>();

		// Token: 0x04000005 RID: 5
/// <summary>My 单例字段：My.WebServices（Container 模板单例）。</summary>
		private static readonly Account.Template<Account.Container> webServices单例 = new Account.Template<Account.Container>();

		// Token: 0x02000005 RID: 5
/// <summary>Importer 公开构造函数（仅供 MyTemplate 内部调用）。</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>
/// My.Forms：Form1/Form2/Form3/Form4/bianhao 的懒加载单例集合（全程序窗体访问入口）。
/// </summary>
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		internal sealed class Importer
		{
			// Token: 0x17000006 RID: 6
			// (get) Token: 0x0600000F RID: 15 RVA: 0x00002618 File Offset: 0x00000818
			// (set) Token: 0x06000014 RID: 20 RVA: 0x000026CC File Offset: 0x000008CC
/// <summary>bianhao 窗体单例（懒加载）。</summary>
			public bianhao bianhao
			{
				get
				{
					this._Property = Account.Importer.窗体懒加载<bianhao>(this._Property);
					return this._Property;
				}
				set
				{
					if (value == this._Property)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.释放窗体<bianhao>(ref this._Property);
				}
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000010 RID: 16 RVA: 0x0000263C File Offset: 0x0000083C
			// (set) Token: 0x06000015 RID: 21 RVA: 0x00002700 File Offset: 0x00000900
/// <summary>Form1 窗体单例（懒加载，主控窗体）。</summary>
			public Form1 Form1
			{
				get
				{
					this.list = Account.Importer.窗体懒加载<Form1>(this.list);
					return this.list;
				}
				set
				{
					if (value == this.list)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.释放窗体<Form1>(ref this.list);
				}
			}

			// Token: 0x17000008 RID: 8
			// (get) Token: 0x06000011 RID: 17 RVA: 0x00002660 File Offset: 0x00000860
			// (set) Token: 0x06000016 RID: 22 RVA: 0x00002734 File Offset: 0x00000934
/// <summary>Form2 窗体单例（懒加载，警告/锁屏窗）。</summary>
			public Form2 Form2
			{
				get
				{
					this.m_Specification = Account.Importer.窗体懒加载<Form2>(this.m_Specification);
					return this.m_Specification;
				}
				set
				{
					if (value == this.m_Specification)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.释放窗体<Form2>(ref this.m_Specification);
				}
			}

			// Token: 0x17000009 RID: 9
			// (get) Token: 0x06000012 RID: 18 RVA: 0x00002684 File Offset: 0x00000884
			// (set) Token: 0x06000017 RID: 23 RVA: 0x00002768 File Offset: 0x00000968
/// <summary>Form3 窗体单例（懒加载，提醒小窗）。</summary>
			public Form3 Form3
			{
				get
				{
					this.dispatcher = Account.Importer.窗体懒加载<Form3>(this.dispatcher);
					return this.dispatcher;
				}
				set
				{
					if (value == this.dispatcher)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.释放窗体<Form3>(ref this.dispatcher);
				}
			}

			// Token: 0x1700000A RID: 10
			// (get) Token: 0x06000013 RID: 19 RVA: 0x000026A8 File Offset: 0x000008A8
			// (set) Token: 0x06000018 RID: 24 RVA: 0x0000279C File Offset: 0x0000099C
/// <summary>Form4 窗体单例（懒加载，最高级别锁屏窗）。</summary>
			public Form4 Form4
			{
				get
				{
					this.attribute = Account.Importer.窗体懒加载<Form4>(this.attribute);
					return this.attribute;
				}
				set
				{
					if (value == this.attribute)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.释放窗体<Form4>(ref this.attribute);
				}
			}

			// Token: 0x06000019 RID: 25 RVA: 0x000027D0 File Offset: 0x000009D0
/// <summary>
/// 窗体懒加载：1. 实例存在且未 IsDisposed → 直接返回；2. 用线程静态 Hashtable policy 防止递归创建
/// （同一类型重复进入抛 WinForms_RecursiveFormCreate）；3. Activator.CreateInstance<T>() 创建新窗体；
/// 4. 捕获 TargetInvocationException 包装为 InvalidOperationException；5. finally 移除递归锁。
/// </summary>
			private static T 窗体懒加载<T>(T Instance) where T : Form, new()
			{
				if (Instance != null && !Instance.IsDisposed)
				{
					return Instance;
				}
				if (Account.Importer.policy != null)
				{
					if (Account.Importer.policy.ContainsKey(typeof(T)))
					{
						throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
					}
				}
				else
				{
					Account.Importer.policy = new Hashtable();
				}
				Account.Importer.policy.Add(typeof(T), null);
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
					Account.Importer.policy.Remove(typeof(T));
				}
				return result;
			}

			// Token: 0x0600001A RID: 26 RVA: 0x000028D8 File Offset: 0x00000AD8
/// <summary>释放窗体：Dispose 并置空（My.Forms 仅允许置空，不允许换新实例）。</summary>
			private void 释放窗体<T>(ref T gparam_0) where T : Form
			{
				gparam_0.Dispose();
				gparam_0 = default(T);
			}

			// Token: 0x0600001B RID: 27 RVA: 0x000021D4 File Offset: 0x000003D4
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Importer()
			{
			}

			// Token: 0x0600001C RID: 28 RVA: 0x00002900 File Offset: 0x00000B00
			[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>标准 Object.Equals 重写（MyTemplate 生成）。</summary>
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600001D RID: 29 RVA: 0x0000291C File Offset: 0x00000B1C
			[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>标准 Object.GetHashCode 重写（MyTemplate 生成）。</summary>
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x0600001E RID: 30 RVA: 0x00002930 File Offset: 0x00000B30
			[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>MyTemplate 生成：返回 Importer 类型（元数据辅助，无业务逻辑）。</summary>
			internal Type UpdateBroadcaster()
			{
				return typeof(Account.Importer);
			}

			// Token: 0x0600001F RID: 31 RVA: 0x00002948 File Offset: 0x00000B48
			[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>标准 Object.ToString 重写（MyTemplate 生成）。</summary>
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x04000006 RID: 6
/// <summary>窗体实例字段：bianhao 单例缓存。</summary>
			public bianhao _Property;

			// Token: 0x04000007 RID: 7
/// <summary>窗体实例字段：Form1 单例缓存。</summary>
			public Form1 list;

			// Token: 0x04000008 RID: 8
/// <summary>窗体实例字段：Form2 单例缓存。</summary>
			public Form2 m_Specification;

			// Token: 0x04000009 RID: 9
/// <summary>窗体实例字段：Form3 单例缓存。</summary>
			public Form3 dispatcher;

			// Token: 0x0400000A RID: 10
/// <summary>窗体实例字段：Form4 单例缓存。</summary>
			public Form4 attribute;

			// Token: 0x0400000B RID: 11
/// <summary>线程静态递归创建保护表（同一类型重复创建时抛异常）。</summary>
			[ThreadStatic]
			private static Hashtable policy;
		}

		// Token: 0x02000006 RID: 6
/// <summary>
/// My.WebServices：WebService 占位容器（本项目为空壳，无实际服务）。
/// </summary>
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class Container
		{
			// Token: 0x06000020 RID: 32 RVA: 0x00002900 File Offset: 0x00000B00
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x06000021 RID: 33 RVA: 0x0000291C File Offset: 0x00000B1C
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000022 RID: 34 RVA: 0x0000295C File Offset: 0x00000B5C
			[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>MyTemplate 生成：返回 Container 类型（元数据辅助，无业务逻辑）。</summary>
			internal Type CompareBroadcaster()
			{
				return typeof(Account.Container);
			}

			// Token: 0x06000023 RID: 35 RVA: 0x00002948 File Offset: 0x00000B48
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x06000024 RID: 36 RVA: 0x00002974 File Offset: 0x00000B74
/// <summary>MyTemplate 生成：实例为空时 Activator 创建（WebServices 占位逻辑）。</summary>
			private static T DeleteBroadcaster<T>(T instance) where T : new()
			{
				if (instance == null)
				{
					return Activator.CreateInstance<T>();
				}
				return instance;
			}

			// Token: 0x06000025 RID: 37 RVA: 0x00002990 File Offset: 0x00000B90
/// <summary>MyTemplate 生成：置空占位（WebServices 释放逻辑）。</summary>
			private void PublishBroadcaster<T>(ref T gparam_0)
			{
				gparam_0 = default(T);
			}

			// Token: 0x06000026 RID: 38 RVA: 0x000021D4 File Offset: 0x000003D4
			[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>Container 构造函数（占位）。</summary>
			public Container()
			{
			}

			// Token: 0x06000027 RID: 39 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>混淆器填充方法（恒返回 true，无意义）。</summary>
			internal static bool VisitThread()
			{
				return true;
			}

			// Token: 0x06000028 RID: 40 RVA: 0x000021C6 File Offset: 0x000003C6
/// <summary>混淆器填充方法（恒返回 false，无意义）。</summary>
			internal static bool RemoveThread()
			{
				return false;
			}
		}

		// Token: 0x02000007 RID: 7
/// <summary>
/// 泛型单例模板：线程局部静态 _Repository 缓存，Activator.CreateInstance 惰性创建单例。
/// </summary>
		[ComVisible(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class Template<T> where T : new()
		{
			// Token: 0x06000029 RID: 41 RVA: 0x000029AC File Offset: 0x00000BAC
/// <summary>
/// 获取单例：_Repository 为 [ThreadStatic] 静态字段，为 null 时 Activator.CreateInstance<T>() 创建并缓存。
/// </summary>
			internal T 获取单例()
			{
				if (Account.Template<T>._Repository == null)
				{
					Account.Template<T>._Repository = Activator.CreateInstance<T>();
				}
				return Account.Template<T>._Repository;
			}

			// Token: 0x0600002A RID: 42 RVA: 0x000021D4 File Offset: 0x000003D4
			[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>Template 构造函数（占位）。</summary>
			public Template()
			{
			}

			// Token: 0x0600002B RID: 43 RVA: 0x000021D1 File Offset: 0x000003D1
/// <summary>混淆器填充方法（恒返回 true，无意义）。</summary>
			internal static bool NewThread()
			{
				return true;
			}

			// Token: 0x0600002C RID: 44 RVA: 0x000021C6 File Offset: 0x000003C6
/// <summary>混淆器填充方法（恒返回 false，无意义）。</summary>
			internal static bool WriteThread()
			{
				return false;
			}

			// Token: 0x0400000C RID: 12
			[ThreadStatic]
			[CompilerGenerated]
/// <summary>[ThreadStatic] 单例缓存字段（每线程独立）。</summary>
			private static T _Repository;
		}
	}
}
