## 文件作用

内嵌程序集加载器（壳的 `AssemblyResolve` 钩子）与 `%TEMP%` 落盘式载荷释放器。

加壳样本会把依赖程序集加密塞进自身资源；本类在 `AppDomain.AssemblyResolve` 上挂回调，运行时按请求名算出资源名（`"b0494a1f-4bd3-" + Base64(MD5(Unicode(简单名)))`），从自身清单资源取密文，优先**纯内存** `Assembly.Load(byte[])`；若内存装载失败（`FileLoadException` / `BadImageFormatException`），则把解密后的 DLL **写到磁盘**再 `Assembly.LoadFile`。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `PrototypeSetter` | 程序集解析器：注册钩子 / 解密内嵌资源 / 内存或落盘装载 / 缓存结果 |
| `注册程序集解析器()`（原 `smethod_0`） | 一次性挂接 `AppDomain.AssemblyResolve` |
| `解析程序集回调()`（原 `DestroyEvent`） | 解析回调本体：算资源名 → 取流 → 解密 → 装载 → 缓存 |
| `取程序集简单名()`（原 `SelectEvent`） | 从程序集全名截第一个逗号前的简单名 |
| `已加载程序集缓存`（原 `_AccountSetter`） | `Hashtable`，请求全名 → 已加载程序集 |
| `已注册标志`（原 `collectionSetter`） | 钩子是否已挂 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `smethod_0()` | `注册程序集解析器()` | dnSpy 占位名（第 0 个静态方法），工程内无调用点，改名安全 |
| `DestroyEvent(object, ResolveEventArgs)` | `解析程序集回调()` | 壳随机名；实为 `AssemblyResolve` 事件处理器 |
| `SelectEvent(string)` | `取程序集简单名()` | 壳随机名；行为是截逗号前的部分 |
| `_AccountSetter`（Hashtable） | `已加载程序集缓存` | 壳随机名；实际是解析结果缓存 |
| `collectionSetter`（bool） | `已注册标志` | 壳随机名；实际是"钩子已挂"标志 |
| `object_0`（`解析程序集回调` 参数） | `发送方` | 事件源，未使用 |
| `resolveEventArgs_0` | `解析参数` | 含请求程序集全名 |
| `string_0`（`取程序集简单名` 参数） | `程序集全名` | |
| `text` / `text2` / `text3` / `text4` / `text5` | `请求全名` / `程序集简单名` / `资源名` / `临时目录` / `落盘路径` | 按用途命名 |
| `obj` / `assembly` / `assembly2` / `array` / `flag2` / `binaryReader` / `manifestResourceStream` | `缓存项` / `已加载程序集` / `结果` / `密文` / `需要落盘` / `读取器` / `资源流` | 按用途命名 |

## 关键方法与流程

### `注册程序集解析器()`

1. 若 `已注册标志` 为真 → 返回。
2. **先**置 `已注册标志 = true`（挂接抛异常也不会重试，原版如此）。
3. `AppDomain.CurrentDomain.AssemblyResolve += 解析程序集回调;`

⚠️ 工程内**没有任何地方调用它**，说明这段加载链在本样本里已失效，或由壳的自定义入口单独触发（入口已被前置处理抹掉）。

### `解析程序集回调(发送方, 解析参数)` — 核心

1. `lock (已加载程序集缓存)` 包住全过程（缓存查找 + 解密 + 落盘 + 装载串行化）。
2. `请求全名 = 解析参数.Name.Trim()`，查缓存：
   - **命中** → `结果 = (Assembly)缓存项`；
   - **未命中** → 进 `try`：
     1. `RSACryptoServiceProvider.UseMachineKeyStore = true;`（让 RSA 走机器级密钥容器）
     2. `程序集简单名 = 取程序集简单名(请求全名)`
     3. `资源名 = "b0494a1f-4bd3-" + Convert.ToBase64String(ListSetter.计算MD5(Encoding.Unicode.GetBytes(程序集简单名)))`
     4. `资源流 = Assembly.GetManifestResourceStream(资源名)`；为空则跳过
     5. 读全部字节到 `密文`（`BinaryReader` + `BaseStream.Position = 0`）
     6. **优先内存装载**：`Assembly.Load(密文)`；捕获 `FileLoadException` / `BadImageFormatException` → `需要落盘 = true`
     7. 若 `需要落盘`：
        - `临时目录 = Path.Combine(Path.GetTempPath(), 资源名)`
        - `落盘路径 = Path.Combine(临时目录, 程序集简单名 + ".dll")`
        - 文件不存在才 `Directory.CreateDirectory` + `FileStream(..., FileMode.Create, FileAccess.Write)` 写入
        - `已加载程序集 = Assembly.LoadFile(落盘路径)`
     8. 把结果写入缓存（两条分支各 `Add` 一次，与原版一致）
     9. `return 已加载程序集`
   - 内层 `catch { }` 吞掉"资源读取/装载失败"，外层 `catch { }` 吞掉"整体异常"，最终 `结果 = null` 让运行时继续默认探测。
3. 返回 `结果`。

### `取程序集简单名(程序集全名)`

`Trim()` 后取第一个 `,` 之前的部分；无逗号原样返回。例：`"Foo, Version=1.0.0.0"` → `"Foo"`。

## 恶意行为 / 敏感操作

| 行为 | 说明 | 风险 |
| --- | --- | --- |
| 内嵌加密程序集自解密 | 依赖 `.NET Reactor`/`IntelliLock` 的资源加密格式 | 高（载荷隐藏手段） |
| **`%TEMP%` 落盘 DLL** | `%TEMP%\b0494a1f-4bd3-<Base64>\<名字>.dll` | 高（磁盘载荷释放，可被 AV 抓） |
| `Assembly.LoadFile` 从临时目录装载 | 绕过常规探测路径 | 高 |
| `UseMachineKeyStore = true` | 触碰机器级 RSA 密钥容器 | 中 |

## IOC / 硬编码

| 类型 | 值 |
| --- | --- |
| 资源名前缀 | `b0494a1f-4bd3-` |
| 资源名算法 | `Base64(MD5(UTF-16LE(程序集简单名)))` 拼接前缀 |
| 临时目录 | `%TEMP%\b0494a1f-4bd3-<Base64>` |
| 落盘文件名 | `<程序集简单名>.dll` |
| API | `Assembly.Load(byte[])`、`Assembly.LoadFile(string)`、`Assembly.GetManifestResourceStream`、`RSACryptoServiceProvider.UseMachineKeyStore` |
| 依赖 | `ListSetter.计算MD5`（FIPS 感知的 MD5 分发） |
