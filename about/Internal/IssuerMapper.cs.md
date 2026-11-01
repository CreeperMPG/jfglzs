# Internal/IssuerMapper.cs

## 文件作用

内嵌程序集解析器（.NET Reactor 的 `AssemblyResolve` 钩子）。它的存在说明：样本的依赖 DLL 不是放在磁盘上让 CLR 自己找，而是被打包进主程序集的内嵌资源里、由这段代码在运行期解出来加载。这是加壳程序最典型的分发形态，也是**磁盘落地痕迹**的来源。

解析流程一句话：`AssemblyResolve` 事件 → 程序集全名 → 取简单名 → `MD5(Unicode(简单名))` → 拼资源名 `b0494a1f-4bd3-<base64>` → 读内嵌资源 → 先试内存加载，失败则写 `%TEMP%` 再 `LoadFile`。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `IssuerMapper` | 注册并处理 `AppDomain.AssemblyResolve`；从内嵌资源里加载被隐藏的依赖程序集 |
| `注册程序集解析事件()` | 幂等注册事件（静态布尔去重） |
| `解析内嵌程序集(object, ResolveEventArgs)` | 核心解析逻辑：查缓存 → 算资源名 → 读资源 → 内存加载或落地加载 → 写缓存 |
| `取程序集简单名(string)` | 截取程序集全名里第一个逗号之前的部分 |
| 静态字段 `已加载程序集缓存` | `Hashtable` 缓存，同时被当作 `lock` 的对象 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `smethod_0()` | `注册程序集解析事件()` | 挂 `AssemblyResolve` 事件 |
| `SetupMapper(object, ResolveEventArgs)` | `解析内嵌程序集(...)` | 事件处理器本体 |
| `RunMapper(string)` | `取程序集简单名(string)` | 截逗号前的简单名 |
| `CreateFacade()` | `返回真占位()` | 恒 `return true;` |
| `m_CallbackMapper` | `已加载程序集缓存` | 全名→Assembly 的 Hashtable，兼作锁 |
| `m_AttributeMapper` | `已注册解析事件` | 幂等标志 |
| `object_0` / `resolveEventArgs_0` / `string_0` | `发送者` / `解析参数` / `程序集全名` | 参数语义化 |
| `text` / `text2` / `text3` / `text4` / `text5` | `程序集名` / `简单名` / `资源名` / `临时目录` / `临时文件路径` | 逐级派生的字符串 |
| `array` / `bytes` | `程序集字节` / `名称字节` | 资源内容与哈希输入 |
| `flag2` | `需要写入临时文件` | 内存加载失败标志 |
| `obj` / `assembly2` | `缓存项` / `结果` | 缓存命中值与返回值 |

## 关键方法与流程

1. **`注册程序集解析事件()`**
   - 读 `已注册解析事件`；false 时先置 true，再 `AppDomain.CurrentDomain.AssemblyResolve += 解析内嵌程序集`。
   - 本工程内无调用点；原样本里由壳的初始化代码调用。
2. **`解析内嵌程序集(sender, args)`**
   - 整体 `lock (已加载程序集缓存)`；
   - `程序集名 = args.Name.Trim()`，查缓存；命中直接把 `Assembly` 强转返回；
   - 未命中 → `try`：
     1. `RSACryptoServiceProvider.UseMachineKeyStore = true`（CryptoAPI 全局开关）；
     2. `简单名 = 取程序集简单名(程序集名)`；
     3. `资源名 = "b0494a1f-4bd3-" + Base64(MD5(Unicode(简单名)))`；
     4. `GetManifestResourceStream(资源名)`；为空则整段放弃（结果为 null）；
     5. 内层 `try`：把资源全读进 `byte[]` → `Assembly.Load(bytes)`；
     6. `FileLoadException` / `BadImageFormatException` → `需要写入临时文件 = true`；
     7. 需要落地时：`%TEMP%\b0494a1f-4bd3-<base64>\<简单名>.dll`，不存在才创建目录并写文件，然后 `Assembly.LoadFile(path)`；
     8. 无论内存还是落地，都 `已加载程序集缓存.Add(程序集名, 程序集)`，返回该程序集；
     9. 内层 `catch { }` 吞掉一切异常，落到 `结果 = null`；
   - 外层 `catch { }` 同样吞异常。
3. **`取程序集简单名(全名)`**：`Trim()` → 找 `','` → 有则 `Substring(0, index)`，无则整串。

## 恶意行为 / 敏感操作

- **内嵌 DLL 落地**：失败回退路径会向 `%TEMP%` 写 DLL 文件——静态分析时这是最值得抓的痕迹，可用作主机侧 IOC。
- **隐藏依赖**：依赖 DLL 不在主程序目录，杀软按「目录里的 DLL 集合」扫描会漏。
- `RSACryptoServiceProvider.UseMachineKeyStore = true`：改用机器级密钥容器，属 CryptoAPI 的常规但不必要的设置。
- 无注册表写入、无网络请求、无进程操作。

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 资源名前缀 | `b0494a1f-4bd3-` + Base64(MD5(Unicode(程序集简单名))) |
| 落地目录 | `Path.GetTempPath()` + `b0494a1f-4bd3-<base64>`（通常为 `%TEMP%`） |
| 落地文件名 | `<程序集简单名>.dll` |
| 加载 API | `Assembly.Load(byte[])`、`Assembly.LoadFile(string)` |
| 加密相关 | `RSACryptoServiceProvider.UseMachineKeyStore = true` |
| 事件 | `AppDomain.CurrentDomain.AssemblyResolve` |
| IL 行号 | 类型 `0x02000015`，方法 `0x060000AA` / `0x060000AB` / `0x060000AC` / `0x060000AF` |
