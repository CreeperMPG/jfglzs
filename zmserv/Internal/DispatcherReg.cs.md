# Internal/DispatcherReg.cs — 嵌入程序集解析器（AssemblyResolve 钩子，.NET Reactor 内嵌依赖加载）

## 文件作用
本文件实现了一个**嵌入程序集加载器**：程序启动时（`smethod_0`）向 `AppDomain.CurrentDomain.AssemblyResolve` 注册钩子。当 CLR 需要加载某个本机不存在的程序集时，钩子根据程序集名计算哈希，从**程序集嵌入资源**中取出加密/内嵌的 DLL 字节并 `Assembly.Load`；若内存加载失败（如被文件锁定/格式问题），则把 DLL 释放到临时目录 `%TEMP%\<资源名>\<名>.dll` 再 `Assembly.LoadFile`。这是 .NET Reactor 加壳后把业务 DLL 藏进资源的典型加载机制——**业务代码并不在本文件，而在被动态解出的嵌入资源里**。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `DispatcherReg` | AssemblyResolve 钩子 + 嵌入资源程序集加载器 + 缓存 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `smethod_0` | 注册解析钩子 | 一次性挂接 `AppDomain.AssemblyResolve` |
| `GetList` | 程序集解析回调 | 从嵌入资源加载缺失程序集并缓存 |
| `DeleteList` | 剥离程序集强名称 | 取逗号前的简单名 |
| `_ListenerReg` (Hashtable) | 已加载程序集缓存 | 名称→Assembly |
| `_RulesReg` | 钩子已注册标志 | 防重复注册 |

## 关键方法与流程

### 注册钩子（原名 `smethod_0`）
- **流程**：若 `_RulesReg` 为 false → 置 true → `AppDomain.CurrentDomain.AssemblyResolve += GetList`。
- 典型调用时机：程序集静态构造（Reactor 注入的模块初始化代码）。

### 解析回调（原名 `GetList`）
- **流程**：
  1. 加锁缓存表；取 `resolveEventArgs.Name` 并 Trim；
  2. 缓存命中 → 直接返回缓存的 Assembly；
  3. 未命中 → `RSACryptoServiceProvider.UseMachineKeyStore = true`（初始化加密服务）→ `DeleteList` 取简单名；
  4. 计算资源名：`"b0494a1f-4bd3-" + Convert.ToBase64String(CustomerReg.PatchList(Encoding.Unicode.GetBytes(简单名)))`——即 **"b0494a1f-4bd3-" + 程序集名UTF-16的MD5的Base64**；
  5. `GetManifestResourceStream(资源名)` 读取全部字节（内嵌且常被加密的 DLL）；
  6. 尝试 `Assembly.Load(bytes)`；捕获 `FileLoadException`/`BadImageFormatException` 时置 flag；
  7. 失败分支：写文件 `%TEMP%\<资源名>\<名>.dll`（不存在才写）→ `Assembly.LoadFile(path)`；
  8. 成功路径与失败路径都写入 `_ListenerReg` 缓存并返回 Assembly；
  9. 全程 try/catch 吞异常，失败返回 null。
- **关键调用/API**：`AppDomain.AssemblyResolve`、`GetManifestResourceStream`、`Assembly.Load/LoadFile`、`RSACryptoServiceProvider.UseMachineKeyStore`、`CustomerReg.PatchList`（MD5）、`Path.GetTempPath`、`FileStream`。

## 恶意行为 / 敏感操作
- **动态加载隐藏代码**：业务/恶意程序集不落盘（或仅在内存加载失败时落盘到 %TEMP%），静态扫描主 EXE 无法看到完整恶意逻辑。
- **落盘释放**：异常情况下在临时目录生成 `<资源名>\<名>.dll`，可作取证线索。
- 资源命名规则可作为 IOC 检索：`b0494a1f-4bd3-<base64(MD5(UTF16(程序集名)))>`。

## IOC / 硬编码
- 资源名前缀：`b0494a1f-4bd3-` + MD5(UTF-16 程序集名) 的 Base64
- 释放路径：`%TEMP%\<资源名>\<程序集名>.dll`
- 依赖 API：`CustomerReg.PatchList`（MD5）、`System.Security.Cryptography.RSACryptoServiceProvider`
- 加壳器指纹：.NET Reactor 嵌入资源加载器模式
