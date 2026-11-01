## 文件作用

IntelliLock 的**内嵌程序集加载器**。它把「真正的功能 DLL」藏在当前程序集的嵌入资源里，
当 CLR 找不到某个被引用的程序集时，用一条**由程序集名算出来的资源名**去资源里取字节并动态加载。

对分析的意义：`zmserv.exe` 表面上只有一个服务类，但运行时可能再拉起一个**不在文件系统里**的 DLL。
要弄清「机房管理助手」的完整能力，必须把这条资源链（资源名 → 字节 → DLL）单独 dump 出来分析。

注意：本文件里的 `注册程序集解析()` 在工程内**没有调用点**——调用它的模块初始化器在脱壳时被摘掉了，
所以静态看它是一段「挂着但没被点着的引信」；一旦壳完整，它就是加载隐藏 DLL 的入口。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `内嵌程序集加载器`（原 `ParamStrategy`） | 挂钩 `AppDomain.AssemblyResolve`；按「`b0494a1f-4bd3-` + Base64(MD5(Unicode(简单名)))」找嵌入资源并加载 |
| `注册程序集解析()`（原 `smethod_0`） | 幂等挂事件（`已注册` 标志保证一次） |
| `解析程序集加载(sender, e)`（原 `CountParams`） | 真正的加载实现：查缓存 → 算资源名 → `Assembly.Load` → 失败落盘 `%TEMP%` 后 `LoadFile` |
| `取简单名(string)`（原 `IncludeParams`） | 从 `"Name, Version=…, Culture=…, PublicKeyToken=…"` 里截出 `Name` |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `ParamStrategy` | `内嵌程序集加载器` | 类名本身就是随机词，按用途重命名 |
| `smethod_0()` | `注册程序集解析()` | `smethod` 是 dnSpy 对静态方法的占位前缀，无语义 |
| `CountParams` | `解析程序集加载` | `AssemblyResolve` 回调 |
| `IncludeParams` | `取简单名` | 截取程序集简单名 |
| `productStrategy`（Hashtable） | `已加载缓存` | key = 请求的程序集全名 |
| `_WriterStrategy`（bool） | `已注册` | 保证事件只挂一次 |
| `object_0` / `resolveEventArgs_0` | `发送者` / `解析请求` | 事件参数 |
| `text` / `text2` / `text3` | `请求全名` / `简单名` / `资源名` | 逐层推导 |
| `obj` | `缓存项` | `Hashtable` 取出的 `object` |
| `assembly2` / `assembly` | `结果程序集` / `已加载程序集` | 返回值和临时变量 |
| `flag2` | `需要落盘加载` | 两个 `catch`（`FileLoadException`/`BadImageFormatException`）置位 |
| `array` | `程序集字节` | 从资源读出的 DLL 字节 |
| `text4` / `text5` | `临时目录` / `临时文件路径` | `%TEMP%\{资源名}\{简单名}.dll` |
| `binaryReader` | `资源读取器` | 资源流包装 |
| `RateDefinition()` | 保持原名 | 壳注入空桩，`return false` |

## 关键方法与流程

### `注册程序集解析()`
1. `if (!已注册)` → `已注册 = true`；
2. `AppDomain 当前域 = AppDomain.CurrentDomain;`
3. `当前域.AssemblyResolve += 解析程序集加载;`

### `解析程序集加载(object 发送者, ResolveEventArgs 解析请求) -> Assembly`
1. `lock (已加载缓存)`（老式 `Hashtable` 锁，不是 `ConcurrentDictionary`）；
2. `请求全名 = 解析请求.Name.Trim()`；查缓存，命中 → 直接返回强转后的 `Assembly`；
3. 未命中：
   - `RSACryptoServiceProvider.UseMachineKeyStore = true;`（壳为了能用机器密钥库，与业务无关）；
   - `简单名 = 取简单名(请求全名)`；
   - `资源名 = "b0494a1f-4bd3-" + Convert.ToBase64String(计算MD5(Encoding.Unicode.GetBytes(简单名)))`
     （**注意前缀被截断了**，只有 `b0494a1f-4bd3-`，不是完整 GUID）；
   - `资源流 = Assembly.GetManifestResourceStream(资源名)`；为空则跳到失败返回 `null`；
4. 有资源流：
   - `BinaryReader` 定位 0，读满整段字节（`资源流.Length`）；
   - `已加载程序集 = Assembly.Load(程序集字节)`；
   - 若抛 `FileLoadException` / `BadImageFormatException` → `需要落盘加载 = true`；
   - 需要落盘时：`临时目录 = Path.Combine(Path.GetTempPath(), 资源名)`，
     `临时文件路径 = Path.Combine(临时目录, 简单名 + ".dll")`，
     文件不存在则 `Directory.CreateDirectory` + `FileStream(Create/Write)` 写入，然后 `Assembly.LoadFile(临时文件路径)`；
   - 无论走哪条分支，最后 `已加载缓存.Add(请求全名, 已加载程序集)` 并返回；
5. 三层 `try/catch`（资源处理 / 外层 / 内层）全部为原件，**范围一律未改**：任一层出错后 `结果程序集 = null`。

### `取简单名(string 程序集全名)`
- `名称 = 程序集全名.Trim()` → 找第一个 `','` → 有则 `Substring(0, 逗号位置)`；返回。

## 恶意行为 / 敏感操作

1. **隐藏依赖**：真正的功能 DLL 不进文件系统、不出现在 `Assembly` 引用列表的普通路径上，
   而是以「名字被 MD5 散列过的嵌入资源」形式藏在宿主程序里——静态扫描看不到它。
2. **运行时动态加载代码**：`Assembly.Load(byte[])` 直接把内存字节变成可执行程序集；
   失败时退回 `Assembly.LoadFile`，并**写文件到 `%TEMP%`**（可被落盘行为监控抓到）。
3. **全局加密开关**：`RSACryptoServiceProvider.UseMachineKeyStore = true` 是壳为了避免用户配置文件的密钥容器问题，
   顺带也是「按机器绑定」的常见铺垫。
4. **锁粒度粗**：`lock (已加载缓存)` 包住整个加载过程（含文件 IO），并发加载会被串行化——原版如此，不是 bug。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）

| 类型 | 值 |
| --- | --- |
| 资源名前缀 | `b0494a1f-4bd3-` |
| 资源名算法 | `前缀 + Base64(MD5(UTF-16LE(程序集简单名)))` |
| 落盘目录 | `%TEMP%\{资源名}\`（`Path.GetTempPath()`） |
| 落盘文件名 | `{程序集简单名}.dll` |
| 打开的文件模式 | `FileMode.Create`、`FileAccess.Write` |
| 全局开关 | `RSACryptoServiceProvider.UseMachineKeyStore = true` |
| 捕获的异常 | `FileLoadException`、`BadImageFormatException` |
| 引用 | `IntelliLock运行时.计算MD5`（见 `AdvisorStrategy.cs.md`） |
| 注册表 / URL | 无 |
