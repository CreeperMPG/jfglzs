# Internal/ConfigurationMock.cs — 嵌入程序集加载器（AssemblyResolve 处理器）

## 文件作用
加壳运行时（.NET Reactor/IntelliLock 家族）的**嵌入资源程序集加载器**：注册 `AppDomain.CurrentDomain.AssemblyResolve` 事件，当程序集解析失败时，按程序集名生成资源名，从自身嵌入资源中读出字节，直接 `Assembly.Load` 或落盘到 %TEMP% 后 `Assembly.LoadFile`。用于把依赖 DLL 加密/改名后塞进主程序集，是脱壳时必须绕过的逻辑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| ConfigurationMock | 嵌入程序集解析器（惰性加载 + 缓存） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| smethod_0() | EnsureHandler_注册解析事件 | 一次性挂 AssemblyResolve |
| ConcatMock(sender,args) | OnAssemblyResolve_解析程序集 | 核心逻辑 |
| GetMock(name) | StripAssemblyName_取纯名 | 去掉 ", Version=..." 等后缀 |
| _CodeMock | 已加载程序集缓存 | Hashtable<程序集名, Assembly> |
| _IndexerMock | 是否已注册标志 | bool |

## 关键方法与流程
### OnAssemblyResolve_解析程序集（ConcatMock）
1. lock 缓存表；`name = args.Name.Trim()`，命中缓存直接返回
2. 未命中：
   - `RSACryptoServiceProvider.UseMachineKeyStore = true`（密钥容器初始化）
   - `mock = GetMock(name)`（纯程序集名）
   - `text2 = "b0494a1f-4bd3-" + Convert.ToBase64String(TestMock.SetMock(UTF16LE(mock)))` —— **资源名 = 固定 GUID 前缀 + Base64(MD5(程序集名))**
   - `GetManifestResourceStream(text2)` 读资源 → 读全部字节
   - `Assembly.Load(array)`；若抛 FileLoadException/BadImageFormatException（如磁盘文件需要/映像加载失败）：
     - `path = %TEMP%\<text2>\<mock>.dll`，不存在则建目录写文件
     - `Assembly.LoadFile(path)`
   - 成功则入缓存并返回
3. 全程 try/catch 吞异常，失败返回 null（解析失败由 CLR 决定）

## 恶意行为 / 敏感操作
- 隐藏依赖：DLL 以加密资源形式内嵌，运行时才解密加载（`Assembly.Load` / 落盘 %TEMP%）
- %TEMP% 落盘路径模式：`%TEMP%\<b0494a1f-4bd3-<Base64MD5>>\<程序集名>.dll`
- 与 TestMock.SetMock（MD5）联动，资源名可由 MD5 预计算，便于脱壳枚举

## IOC / 硬编码
- 资源名前缀：`b0494a1f-4bd3-`
- 落盘目录：`Path.GetTempPath()` 下
- 相关 API：AssemblyResolve、GetManifestResourceStream、Assembly.Load/LoadFile、RSACryptoServiceProvider.UseMachineKeyStore
