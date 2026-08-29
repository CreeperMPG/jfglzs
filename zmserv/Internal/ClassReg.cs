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
	[HideModuleName]
	[StandardModule]
	[GeneratedCode("MyTemplate", "8.0.0.0")]
	/// <summary>
	/// ClassReg —— VB.NET "My.*" 门面类（MyTemplate 自动生成，混淆器只改了名字）。
	/// 通过 ErrorReg<T>（线程静态惰性单例容器）暴露 My.Computer（RegReg）、My.Application（ListReg）、
	/// My.User（User）、My.WebServices（TemplateReg）四个访问器。无任何恶意逻辑，属 VB 脚手架代码。
	/// </summary>
	internal sealed class ClassReg
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002434 File Offset: 0x00000634
		[HelpKeyword("My.Computer")]
		/// <summary>My.Computer 访问器：返回 computerHolder 惰性单例（RegReg）。</summary>
		internal static RegReg MyComputer
		{
			get
			{
				return ClassReg.computerHolder.GetOrCreateInstance();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x0000244C File Offset: 0x0000064C
		[HelpKeyword("My.Application")]
		/// <summary>My.Application 访问器：返回 applicationHolder 惰性单例（ListReg）。</summary>
		internal static ListReg MyApplication
		{
			get
			{
				return ClassReg.applicationHolder.GetOrCreateInstance();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002464 File Offset: 0x00000664
		[HelpKeyword("My.User")]
		/// <summary>My.User 访问器：返回 userHolder 惰性单例（User）。</summary>
		internal static User MyUser
		{
			get
			{
				return ClassReg.userHolder.GetOrCreateInstance();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000247C File Offset: 0x0000067C
		[HelpKeyword("My.WebServices")]
		/// <summary>My.WebServices 访问器：返回 webServicesHolder 惰性单例（TemplateReg）。</summary>
		internal static ClassReg.TemplateReg MyWebServices
		{
			get
			{
				return ClassReg.webServicesHolder.GetOrCreateInstance();
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000021BC File Offset: 0x000003BC
		/// <summary>混淆填充：恒真，无业务意义（ClassReg 作用域）。</summary>
		internal static bool ConfuserFillerTrue()
		{
			return true;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021BF File Offset: 0x000003BF
		/// <summary>混淆填充：恒假，无业务意义（ClassReg 作用域）。</summary>
		internal static bool ConfuserFillerFalse()
		{
			return false;
		}

		// Token: 0x04000001 RID: 1
		/// <summary>My.Computer 单例持有器（ErrorReg<RegReg> 实例）。</summary>
		private static readonly ClassReg.ErrorReg<RegReg> computerHolder = new ClassReg.ErrorReg<RegReg>();

		// Token: 0x04000002 RID: 2
		/// <summary>My.Application 单例持有器（ErrorReg<ListReg> 实例）。</summary>
		private static readonly ClassReg.ErrorReg<ListReg> applicationHolder = new ClassReg.ErrorReg<ListReg>();

		// Token: 0x04000003 RID: 3
		/// <summary>My.User 单例持有器（ErrorReg<User> 实例）。</summary>
		private static readonly ClassReg.ErrorReg<User> userHolder = new ClassReg.ErrorReg<User>();

		// Token: 0x04000004 RID: 4
		/// <summary>My.WebServices 单例持有器（ErrorReg<TemplateReg> 实例）。</summary>
		private static readonly ClassReg.ErrorReg<ClassReg.TemplateReg> webServicesHolder = new ClassReg.ErrorReg<ClassReg.TemplateReg>();

		// Token: 0x02000005 RID: 5
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MyGroupCollection("System.Web.Services.Protocols.SoapHttpClientProtocol", "Create__Instance__", "Dispose__Instance__", "")]
		/// <summary>
		/// TemplateReg —— My.WebServices 集合类（SoapHttpClientProtocol 客户端集合，生成器产物，无手工逻辑）。
		/// </summary>
		internal sealed class TemplateReg
		{
			// Token: 0x0600000E RID: 14 RVA: 0x00002494 File Offset: 0x00000694
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override bool Equals(object obj)
			{
				return base.Equals(RuntimeHelpers.GetObjectValue(obj));
			}

			// Token: 0x0600000F RID: 15 RVA: 0x000024B0 File Offset: 0x000006B0
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			// Token: 0x06000010 RID: 16 RVA: 0x000024C4 File Offset: 0x000006C4
			[EditorBrowsable(EditorBrowsableState.Never)]
			/// <summary>返回自身类型（生成器代码）。</summary>
			internal Type GetMyType()
			{
				return typeof(ClassReg.TemplateReg);
			}

			// Token: 0x06000011 RID: 17 RVA: 0x000024DC File Offset: 0x000006DC
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override string ToString()
			{
				return base.ToString();
			}

			// Token: 0x06000012 RID: 18 RVA: 0x000024F0 File Offset: 0x000006F0
			/// <summary>空则新建：instance 为 null 时 Activator.CreateInstance<T>()，否则原样返回（生成器代码）。</summary>
			private static T GetOrCreate<T>(T instance) where T : new()
			{
				if (instance == null)
				{
					return Activator.CreateInstance<T>();
				}
				return instance;
			}

			// Token: 0x06000013 RID: 19 RVA: 0x0000250C File Offset: 0x0000070C
			/// <summary>置默认值：gparam_0 = default(T)（生成器代码）。</summary>
			private void ResetToDefault<T>(ref T gparam_0)
			{
				gparam_0 = default(T);
			}

			// Token: 0x06000014 RID: 20 RVA: 0x000021CA File Offset: 0x000003CA
			[EditorBrowsable(EditorBrowsableState.Never)]
			public TemplateReg()
			{
			}

			// Token: 0x06000015 RID: 21 RVA: 0x000021BC File Offset: 0x000003BC
			/// <summary>混淆填充：恒真，无业务意义（TemplateReg 作用域）。</summary>
			internal static bool ConfuserFillerTrue()
			{
				return true;
			}

			// Token: 0x06000016 RID: 22 RVA: 0x000021BF File Offset: 0x000003BF
			/// <summary>混淆填充：恒假，无业务意义（TemplateReg 作用域）。</summary>
			internal static bool ConfuserFillerFalse()
			{
				return false;
			}
		}

		// Token: 0x02000006 RID: 6
		[ComVisible(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		/// <summary>
		/// ErrorReg<T> —— 线程静态惰性单例容器（get-or-create）：GetOrCreateInstance 懒创建并缓存于 threadInstance。
		/// </summary>
		internal sealed class ErrorReg<T> where T : new()
		{
			// Token: 0x06000017 RID: 23 RVA: 0x00002528 File Offset: 0x00000728
			/// <summary>取单例：threadInstance（ThreadStatic）为 null 时 Activator.CreateInstance<T>() 并缓存后返回。</summary>
			internal T GetOrCreateInstance()
			{
				if (ClassReg.ErrorReg<T>.threadInstance == null)
				{
					ClassReg.ErrorReg<T>.threadInstance = Activator.CreateInstance<T>();
				}
				return ClassReg.ErrorReg<T>.threadInstance;
			}

			// Token: 0x06000018 RID: 24 RVA: 0x000021CA File Offset: 0x000003CA
			[EditorBrowsable(EditorBrowsableState.Never)]
			public ErrorReg()
			{
			}

			// Token: 0x06000019 RID: 25 RVA: 0x000021BC File Offset: 0x000003BC
			/// <summary>混淆填充：恒真，无业务意义（ErrorReg 作用域）。</summary>
			internal static bool ConfuserFillerTrue()
			{
				return true;
			}

			// Token: 0x0600001A RID: 26 RVA: 0x000021BF File Offset: 0x000003BF
			/// <summary>混淆填充：恒假，无业务意义（ErrorReg 作用域）。</summary>
			internal static bool ConfuserFillerFalse()
			{
				return false;
			}

			// Token: 0x04000005 RID: 5
			[ThreadStatic]
			[CompilerGenerated]
			/// <summary>线程静态单例缓存（ThreadStatic）。</summary>
			private static T threadInstance;
		}
	}
}
