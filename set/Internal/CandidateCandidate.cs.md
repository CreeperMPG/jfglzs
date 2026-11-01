# Internal\CandidateCandidate.cs —— 文件级分析

## 文件作用
**.NET Reactor 的内嵌程序集按需加载器**（`AppDomain.AssemblyResolve` 钩子）。这是解壳后必须保留的关键运行期组件：主程序里藏着一批被加密/合并的依赖程序集（清单资源形式），CLR 找不到某个依赖时由本类按算法算出资源名、解密读出、内存加载；内存加载失败则落盘到 `%TEMP%` 再 `LoadFile`。**资源名的算法依赖同工程的 `Internal\CollectionAttr.cs`（自定义摘要函数 `SetAttr`）。**

## 类与职责（表格：类/模块 | 真实职责）

| 类/模块 | 真实职责 |
| --- | --- |
| `CandidateCandidate` | 内嵌程序集解析器（Reactor 运行时加载核心） |
| `注册程序集解析器()`（旧名 `smethod_0`） | 一次性挂载 `AppDomain.CurrentDomain.AssemblyResolve` 事件 |
| `解析内嵌程序集(object, ResolveEventArgs)`（旧名 `StopAttr`） | 按需求名算出资源名 → 读资源 → `Assembly.Load(byte[])`；失败落盘 `%TEMP%` 再加载；全程异常吞掉 |
| `取程序集简单名(string)`（旧名 `ReadAttr`） | 从「SimpleName, Version=...」里截出逗号前的简单名 |
| `已加载程序集缓存`（旧名 `m_ContextCandidate`） | `Hashtable`：程序集全名 → `Assembly`；非线程安全，靠 lock 保护 |
| `已注册`（旧名 `m_QueueCandidate`） | 防止重复订阅事件 |

## 反混淆命名映射（表格：混淆名 | 反混淆名 | 说明）

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `smethod_0()` | `注册程序集解析器()` | dnSpy 无名占位，按行为重命名 |
| `StopAttr(object, ResolveEventArgs)` | `解析内嵌程序集(object, ResolveEventArgs)` | AssemblyResolve 处理器 |
| `ReadAttr(string)` | `取程序集简单名(string)` | 截取逗号前的程序集简单名 |
| `m_ContextCandidate` | `已加载程序集缓存` | `Hashtable` 缓存 |
| `m_QueueCandidate` | `已注册` | 事件已订阅标志 |
| `object_0`（参数） | `发送者` | 事件源 AppDomain |
| `resolveEventArgs_0`（参数） | `解析事件参数` | 含 `Name` 完整程序集名 |
| `string_0`（参数） | `程序集全名` | 输入的程序集名 |
| 局部 `text` / `text2` | `程序集全名` / `程序集简单名` | 一步步裁剪后的名字 |
| 局部 `text3` / `text4` / `text5` | `资源名` / `临时目录` / `临时文件` | 资源名与落盘路径 |
| 局部 `obj` / `assembly2` / `assembly` | `缓存项` / `结果` / `程序集` | 缓存命中与加载结果 |
| 局部 `bytes` / `array` / `flag2` | `名称字节` / `资源字节` / `需落盘` | |
| 局部 `fileStream` / `binaryReader` / `manifestResourceStream` | `文件流` / `读取器` / `资源流` | |
| `CandidateCandidate` | 未改名 | 类名保留原样 |

## 关键方法与流程（按方法分段，写清步骤）

### `注册程序集解析器()`
1. `if (!已注册)` —— 只挂一次。
2. `已注册 = true`。
3. `AppDomain.CurrentDomain.AssemblyResolve += 解析内嵌程序集`。

### `解析内嵌程序集(发送者, 解析事件参数)`
1. `lock (已加载程序集缓存)` —— 锁的是 Hashtable 对象本身。
2. `程序集全名 = 解析事件参数.Name.Trim()`，查缓存 `缓存项 = 缓存[程序集全名]`。
3. 缓存命中：直接返回 `(Assembly)缓存项`。
4. 未命中：
   1. `RSACryptoServiceProvider.UseMachineKeyStore = true`（Reactor 既有行为，会用到机器密钥容器）。
   2. `程序集简单名 = 取程序集简单名(程序集全名)`。
   3. `名称字节 = Encoding.Unicode.GetBytes(程序集简单名)` —— **UTF-16LE 编码**，不是 UTF-8。
   4. `资源名 = "b0494a1f-4bd3-" + Convert.ToBase64String(CollectionAttr.SetAttr(名称字节))`
      —— 固定前缀 + Base64(自定义摘要)。
   5. `资源流 = 本程序集.GetManifestResourceStream(资源名)`；为 null 就跳过（最后返回 null）。
   6. 用 `BinaryReader` 从 `Position = 0` 读出全部字节 → `资源字节`。
   7. `try { 程序集 = Assembly.Load(资源字节); }` —— 优先内存加载（不落盘）。
   8. `catch (FileLoadException) / catch (BadImageFormatException) → 需落盘 = true`。
   9. 需落盘时：
      1. `临时目录 = Path.Combine(Path.GetTempPath(), 资源名)`
      2. `临时文件 = Path.Combine(临时目录, 简单名 + ".dll")`
      3. 文件不存在就建目录 + 写文件（`FileMode.Create`）
      4. `程序集 = Assembly.LoadFile(临时文件)`，加入缓存
   10. 不需要落盘：直接加入缓存。
   11. `return 程序集;`
5. 任何一步抛异常：内层 `catch {}` 吞掉（读资源/加载阶段），外层 `catch {}` 再吞一次（算摘要/找资源阶段），最终 `结果 = null` 返回。

### `取程序集简单名(程序集全名)`
1. `文本 = 程序集全名.Trim()`
2. `逗号位置 = 文本.IndexOf(',')`；`>= 0` 则 `文本 = 文本.Substring(0, 逗号位置)`
3. 返回 `文本`

## 恶意行为 / 敏感操作
- 这是**加壳保护 / 反分析**组件，不是直接破坏行为，但敏感点明确：
  - **动态程序集加载**（`Assembly.Load(byte[])` / `Assembly.LoadFile`）：典型的内存落地/免杀手法，AV 难以静态看到真实依赖。
  - **写临时目录**：`%TEMP%\<资源名>\<程序集简单名>.dll`，会在磁盘上留下 DLL 直落物。
  - **`RSACryptoServiceProvider.UseMachineKeyStore = true`**：触碰机器级密钥容器。
  - **静默失败**：三层 `catch {}` 把错误全部吞掉，分析时容易出现「什么都没发生」的假象。
- 关键点：`CollectionAttr.SetAttr`（自定义摘要）是资源名的钥匙 —— 想枚举内嵌的加密程序集，必须复现这个算法。

## IOC / 硬编码（路径、注册表键、URL、进程名、字符串常量）
- 资源名前缀：`b0494a1f-4bd3-`（拼上 GUID 风格的其他段，看起来像伪装成 GUID 的资源名）
- 落盘路径模板：`%TEMP%\<资源名>\<程序集简单名>.dll`（`Path.GetTempPath()` + `Path.Combine`）
- 资源命名算法：`"b0494a1f-4bd3-" + Convert.ToBase64String(CollectionAttr.SetAttr(Encoding.Unicode.GetBytes(简单名)))`
- 进程/注册表：无
- 关键 API：`AppDomain.AssemblyResolve`、`Assembly.Load`、`Assembly.LoadFile`、`RSACryptoServiceProvider.UseMachineKeyStore`

## 反混淆处理记录
- goto：0 → 0
- 删除噪声：无 `IL_xxxx:` / `num2 = N` / `ClearProjectError()`
- 保留全部 6 条 `// Token:` 注释
- **语义保护**：三个 `catch { }`（外层、内层、以及 `FileLoadException`/`BadImageFormatException` 分支）全部保留；`lock` 范围未变；`try/finally` 结构未变；字符串常量（含 `"b0494a1f-4bd3-"`、`".dll"`、`","`）逐字符保留
