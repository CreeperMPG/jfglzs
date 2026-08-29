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
		/// VB.NET "My" 命名空间模拟（My.Computer/My.Application/My.User/My.Forms/My.WebServices）门面，
		/// 程序代码通过 Reader.MyComputer.Registry、Reader.MyApplication、Reader.MyForms.Form1/Form3/load 访问系统对象与窗体。
		/// 入口链关键节点：Reader.MyForms.load 即入口窗体（ExceptionMock.ProgramEntry_ShowMainForm 与 Descriptor.OnCreateMainForm 都用它）。
		/// </summary>
	// Token: 0x02000004 RID: 4
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	[HideModuleName]
	[StandardModule]
	internal sealed class Reader
	{
		/// <summary>
		/// My.Computer → Watcher 实例（线程静态单例）。
		/// </summary>
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000009 RID: 9 RVA: 0x0000270C File Offset: 0x0000090C
		[HelpKeyword("My.Computer")]
		internal static Watcher MyComputer
		{
			get
			{
				return Reader._myComputerSingleton.GetSingleton();
			}
		}

		/// <summary>
		/// My.Application → Descriptor 实例（VB 应用框架）。
		/// </summary>
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002724 File Offset: 0x00000924
		[HelpKeyword("My.Application")]
		internal static Descriptor MyApplication
		{
			get
			{
				return Reader._myApplicationSingleton.GetSingleton();
			}
		}

		/// <summary>
		/// My.User → User 实例。
		/// </summary>
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000273C File Offset: 0x0000093C
		[HelpKeyword("My.User")]
		internal static User MyUser
		{
			get
			{
				return Reader._myUserSingleton.GetSingleton();
			}
		}

		/// <summary>
		/// My.Forms → Model 集合（Form1/Form3/load 窗体单例缓存）。
		/// </summary>
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002754 File Offset: 0x00000954
		[HelpKeyword("My.Forms")]
		internal static Reader.Model MyForms
		{
			get
			{
				return Reader._myFormsSingleton.GetSingleton();
			}
		}

		/// <summary>
		/// My.WebServices → Map 集合（空占位）。
		/// </summary>
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000276C File Offset: 0x0000096C
		[HelpKeyword("My.WebServices")]
		internal static Reader.Map MyWebServices
		{
			get
			{
				return Reader._myWebServicesSingleton.GetSingleton();
			}
		}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 false，无实际逻辑。
		/// </summary>
		// Token: 0x0600000E RID: 14 RVA: 0x000021C6 File Offset: 0x000003C6
		internal static bool Placeholder_ReturnFalse()
		{
			return false;
		}

		/// <summary>
		/// My.Computer 单例容器（Watcher）。
		/// </summary>
		// Token: 0x04000001 RID: 1
		private static readonly Reader.Configuration<Watcher> _myComputerSingleton = new Reader.Configuration<Watcher>();

		/// <summary>
		/// My.Application 单例容器（Descriptor）。
		/// </summary>
		// Token: 0x04000002 RID: 2
		private static readonly Reader.Configuration<Descriptor> _myApplicationSingleton = new Reader.Configuration<Descriptor>();

		/// <summary>
		/// My.User 单例容器（User）。
		/// </summary>
		// Token: 0x04000003 RID: 3
		private static readonly Reader.Configuration<User> _myUserSingleton = new Reader.Configuration<User>();

		/// <summary>
		/// My.Forms 单例容器（Model）。
		/// </summary>
		// Token: 0x04000004 RID: 4
		private static Reader.Configuration<Reader.Model> _myFormsSingleton = new Reader.Configuration<Reader.Model>();

		/// <summary>
		/// My.WebServices 单例容器（Map）。
		/// </summary>
		// Token: 0x04000005 RID: 5
		private static readonly Reader.Configuration<Reader.Map> _myWebServicesSingleton = new Reader.Configuration<Reader.Map>();

		/// <summary>
		/// My.Forms 集合：Form1/Form3/load 三个窗体属性的惰性单例容器（含防递归创建检测）。
		/// </summary>
		// Token: 0x02000005 RID: 5
		[MyGroupCollection("System.Windows.Forms.Form", "Create__Instance__", "Dispose__Instance__", "My.MyProject.Forms")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class Model
		{
		/// <summary>
		/// My.Forms.Form1：getter 经 GetOrCreateForm 惰性创建/返回缓存实例；setter 只允许赋 null（Dispose 语义）。
		/// </summary>
			// Token: 0x17000006 RID: 6
			// (get) Token: 0x0600000F RID: 15 RVA: 0x00002784 File Offset: 0x00000984
			// (set) Token: 0x06000012 RID: 18 RVA: 0x000027F0 File Offset: 0x000009F0
			public Form1 Form1
			{
				get
				{
					this.m_form1 = Reader.Model.GetOrCreateForm<Form1>(this.m_form1);
					return this.m_form1;
				}
				set
				{
					if (value == this.m_form1)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.DisposeForm<Form1>(ref this.m_form1);
				}
			}

		/// <summary>
		/// My.Forms.Form3：同上（完成提醒窗体）。
		/// </summary>
			// Token: 0x17000007 RID: 7
			// (get) Token: 0x06000010 RID: 16 RVA: 0x000027A8 File Offset: 0x000009A8
			// (set) Token: 0x06000013 RID: 19 RVA: 0x00002824 File Offset: 0x00000A24
			public Form3 Form3
			{
				get
				{
					this.m_form3 = Reader.Model.GetOrCreateForm<Form3>(this.m_form3);
					return this.m_form3;
				}
				set
				{
					if (value == this.m_form3)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.DisposeForm<Form3>(ref this.m_form3);
				}
			}

		/// <summary>
		/// My.Forms.load：同上（密码门窗体，入口窗体）。
		/// </summary>
			// Token: 0x17000008 RID: 8
			// (get) Token: 0x06000011 RID: 17 RVA: 0x000027CC File Offset: 0x000009CC
			// (set) Token: 0x06000014 RID: 20 RVA: 0x00002858 File Offset: 0x00000A58
			public load load
			{
				get
				{
					this.m_loadForm = Reader.Model.GetOrCreateForm<load>(this.m_loadForm);
					return this.m_loadForm;
				}
				set
				{
					if (value == this.m_loadForm)
					{
						return;
					}
					if (value != null)
					{
						throw new ArgumentException("Property can only be set to Nothing");
					}
					this.DisposeForm<load>(ref this.m_loadForm);
				}
			}

		/// <summary>
		/// 窗体惰性创建：实例存在且未 Dispose 直接返回；否则检查 _recursionGuard 防递归（抛 WinForms_RecursiveFormCreate），
		/// Activator.CreateInstance<T>() 创建；TargetInvocationException 时抛 WinForms_SeeInnerException。
		/// </summary>
			// Token: 0x06000015 RID: 21 RVA: 0x0000288C File Offset: 0x00000A8C
			private static T GetOrCreateForm<T>(T formInstance) where T : Form, new()
			{
				if (formInstance != null && !formInstance.IsDisposed)
				{
					return formInstance;
				}
				if (Reader.Model._recursionGuard != null)
				{
					if (Reader.Model._recursionGuard.ContainsKey(typeof(T)))
					{
						throw new InvalidOperationException(Utils.GetResourceString("WinForms_RecursiveFormCreate", new string[0]));
					}
				}
				else
				{
					Reader.Model._recursionGuard = new Hashtable();
				}
				Reader.Model._recursionGuard.Add(typeof(T), null);
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
					Reader.Model._recursionGuard.Remove(typeof(T));
				}
				return result;
			}

		/// <summary>
		/// 释放窗体：Dispose 后置 null。
		/// </summary>
			// Token: 0x06000016 RID: 22 RVA: 0x00002994 File Offset: 0x00000B94
			private void DisposeForm<T>(ref T formInstance) where T : Form
			{
				formInstance.Dispose();
				formInstance = default(T);
			}

		/// <summary>
		/// Model 构造（框架占位，空）。
		/// </summary>
			// Token: 0x06000017 RID: 23 RVA: 0x000021D4 File Offset: 0x000003D4
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Model()
			{
			}

		/// <summary>
		/// 框架样板 override（Equals 转发 base）。
		/// </summary>
			// Token: 0x06000018 RID: 24 RVA: 0x000029BC File Offset: 0x00000BBC
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

		/// <summary>
		/// 框架样板 override（GetHashCode 转发 base）。
		/// </summary>
			// Token: 0x06000019 RID: 25 RVA: 0x000029D8 File Offset: 0x00000BD8
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

		/// <summary>
		/// 占位方法：返回自身类型，无实际逻辑。
		/// </summary>
			// Token: 0x0600001A RID: 26 RVA: 0x000029EC File Offset: 0x00000BEC
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type Placeholder_ReturnType()
			{
				return typeof(Reader.Model);
			}

		/// <summary>
		/// 框架样板 override（ToString 转发 base）。
		/// </summary>
			// Token: 0x0600001B RID: 27 RVA: 0x00002A04 File Offset: 0x00000C04
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

		/// <summary>
		/// Form1 窗体单例缓存字段。
		/// </summary>
			// Token: 0x04000006 RID: 6
			public Form1 m_form1;

		/// <summary>
		/// Form3 窗体单例缓存字段。
		/// </summary>
			// Token: 0x04000007 RID: 7
			public Form3 m_form3;

		/// <summary>
		/// load 窗体单例缓存字段。
		/// </summary>
			// Token: 0x04000008 RID: 8
			public load m_loadForm;

		/// <summary>
		/// 递归创建检测表（ThreadStatic）。
		/// </summary>
			// Token: 0x04000009 RID: 9
			[ThreadStatic]
			private static Hashtable _recursionGuard;
		}

		/// <summary>
		/// My.WebServices 集合占位（SoapHttpClientProtocol 集合，空实现）。
		/// </summary>
		// Token: 0x02000006 RID: 6
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class Map
		{
		/// <summary>
		/// 框架样板 override（Equals 转发 base）。
		/// </summary>
			// Token: 0x0600001C RID: 28 RVA: 0x000029BC File Offset: 0x00000BBC
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

		/// <summary>
		/// 框架样板 override（GetHashCode 转发 base）。
		/// </summary>
			// Token: 0x0600001D RID: 29 RVA: 0x000029D8 File Offset: 0x00000BD8
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

		/// <summary>
		/// 占位方法：返回自身类型，无实际逻辑。
		/// </summary>
			// Token: 0x0600001E RID: 30 RVA: 0x00002A18 File Offset: 0x00000C18
			[EditorBrowsable(EditorBrowsableState.Never)]
			internal Type Placeholder_ReturnType()
			{
				return typeof(Reader.Map);
			}

		/// <summary>
		/// 框架样板 override（ToString 转发 base）。
		/// </summary>
			// Token: 0x0600001F RID: 31 RVA: 0x00002A04 File Offset: 0x00000C04
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

		/// <summary>
		/// WebService 惰性创建：instance 为 null 时 Activator.CreateInstance<T>()。
		/// </summary>
			// Token: 0x06000020 RID: 32 RVA: 0x00002A30 File Offset: 0x00000C30
			private static T GetOrCreateWebService<T>(T instance) where T : new()
			{
				if (instance == null)
				{
					return Activator.CreateInstance<T>();
				}
				return instance;
			}

		/// <summary>
		/// 重置 WebService 槽位为 default(T)。
		/// </summary>
			// Token: 0x06000021 RID: 33 RVA: 0x00002A4C File Offset: 0x00000C4C
			private void ResetWebServiceSlot<T>(ref T webService)
			{
				webService = default(T);
			}

		/// <summary>
		/// Map 构造（框架占位，空）。
		/// </summary>
			// Token: 0x06000022 RID: 34 RVA: 0x000021D4 File Offset: 0x000003D4
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Map()
			{
			}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 true，无实际逻辑。
		/// </summary>
			// Token: 0x06000023 RID: 35 RVA: 0x000021D1 File Offset: 0x000003D1
			internal static bool Placeholder_ReturnTrue()
			{
				return true;
			}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 false，无实际逻辑。
		/// </summary>
			// Token: 0x06000024 RID: 36 RVA: 0x000021C6 File Offset: 0x000003C6
			internal static bool Placeholder_ReturnFalse()
			{
				return false;
			}
		}

		/// <summary>
		/// 线程静态单例容器：惰性 Activator.CreateInstance 缓存单例（_listener）。
		/// </summary>
		// Token: 0x02000007 RID: 7
		[ComVisible(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal sealed class Configuration<T> where T : new()
		{
		/// <summary>
		/// 取单例：线程静态 _listener 为 null 时创建并缓存。
		/// </summary>
			// Token: 0x06000025 RID: 37 RVA: 0x00002A68 File Offset: 0x00000C68
			internal T GetSingleton()
			{
				if (Reader.Configuration<T>._Listener == null)
				{
					Reader.Configuration<T>._Listener = Activator.CreateInstance<T>();
				}
				return Reader.Configuration<T>._Listener;
			}

		/// <summary>
		/// Configuration 构造（框架占位，空）。
		/// </summary>
			// Token: 0x06000026 RID: 38 RVA: 0x000021D4 File Offset: 0x000003D4
			[EditorBrowsable(EditorBrowsableState.Never)]
			public Configuration()
			{
			}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 true，无实际逻辑。
		/// </summary>
			// Token: 0x06000027 RID: 39 RVA: 0x000021D1 File Offset: 0x000003D1
			internal static bool Placeholder_ReturnTrue()
			{
				return true;
			}

		/// <summary>
		/// 占位方法（混淆残留）：恒返回 false，无实际逻辑。
		/// </summary>
			// Token: 0x06000028 RID: 40 RVA: 0x000021C6 File Offset: 0x000003C6
			internal static bool Placeholder_ReturnFalse()
			{
				return false;
			}

		/// <summary>
		/// Configuration<T> 的线程静态单例缓存字段。
		/// </summary>
			// Token: 0x0400000A RID: 10
			[ThreadStatic]
			[CompilerGenerated]
			private static T _Listener;
		}
	}
}
