# Internal/AttributeConfig.cs 分析

## 文件作用

.NET Reactor 的**内嵌依赖程序集加载器**：依赖 DLL 被作为资源嵌进主程序，运行时挂到
`AppDomain.AssemblyResolve` 上，需要时才从资源里取出来 `Assembly.Load`。

资源名不是写死的常量：

```
资源名 = "b0494a1f-4bd3-" + Base64( 计算哈希( UTF16( 程序集短名 ) ) )
```

也就是说，只要知道「程序集短名」就能算出资源名（`计算哈希` 见 `Internal/IndexerConfig.cs`）。

安全相关的两个副作用：

1. 把 RSA 密钥容器指向**机器级**存储：`RSACryptoServiceProvider.UseMachineKeyStore = true`；
2. 内存加载失败（`FileLoadException` / `BadImageFormatException`）时，会把 DLL **明文写到磁盘**
   `%TEMP%\{资源名}\{短名}.dll`，再从磁盘 `Assembly.LoadFile` —— 磁盘上会留下解包后的二进制。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `内嵌程序集解析器`（原 `AttributeConfig`） | 内嵌程序集加载器（AssemblyResolve 回调 + 缓存） |
| `注册程序集解析回调()`（原 `smethod_0`） | 幂等注册：只把回调挂一次到 `AppDomain.CurrentDomain.AssemblyResolve` |
| `程序集解析回调(object, ResolveEventArgs)`（原 `PrepareConfig`） | 核心：查缓存 → 算资源名 → 读资源 → 内存 `Assembly.Load` 或落盘 `LoadFile` |
| `取程序集短名(string)`（原 `ConnectConfig`） | `IndexOf(',')` 截断，去掉 `Version/Culture/PublicKeyToken` |
| `程序集缓存`（原 `m_VisitorConfig`） | `Hashtable`：程序集全名 → `Assembly` |
| `已注册回调`（原 `_PredicateConfig`） | 只挂一次的标志 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `AttributeConfig` | `内嵌程序集解析器` | 类里全是 AssemblyResolve + 资源加载，与「特性」无关 |
| `smethod_0` | `注册程序集解析回调` |  |
| `PrepareConfig(object, ResolveEventArgs)` | `程序集解析回调(object 发送者, ResolveEventArgs 解析参数)` |  |
| `ConnectConfig(string)` | `取程序集短名(string 程序集全名)` |  |
| `m_VisitorConfig` | `程序集缓存` |  |
| `_PredicateConfig` | `已注册回调` |  |
| 局部 `text` | `程序集全名` |  |
| 局部 `obj` | `缓存命中` |  |
| 局部 `text2` | `程序集短名` |  |
| 局部 `bytes` | `短名字节` |  |
| 局部 `text3` | `资源名` |  |
| 局部 `manifestResourceStream` | `资源流` |  |
| 局部 `binaryReader` | `读取器` |  |
| 局部 `array` | `程序集字节` |  |
| 局部 `flag2` | `需要落盘加载` |  |
| 局部 `assembly` | `已加载程序集` |  |
| 局部 `text4` / `text5` | `临时目录` / `临时文件` |  |
| 局部 `fileStream` | `写出流` |  |
| 参数 `object_0` / `resolveEventArgs_0` / `string_0` | `发送者` / `解析参数` / `程序集全名` |  |

## 关键方法与流程

```
lock (程序集缓存)
{
    程序集全名 = 解析参数.Name.Trim();
    缓存命中 = 程序集缓存[程序集全名];
    if (缓存命中 == null)
    {
        try
        {
            RSACryptoServiceProvider.UseMachineKeyStore = true;
            程序集短名 = 取程序集短名(程序集全名);
            资源名 = "b0494a1f-4bd3-" + Base64( 计算哈希(UTF16(程序集短名)) );
            资源流 = 本程序集.GetManifestResourceStream(资源名);
            if (资源流 != null)
            {
                try
                {
                    读全部字节;
                    try { 已加载程序集 = Assembly.Load(程序集字节); }
                    catch (FileLoadException)       { 需要落盘加载 = true; }
                    catch (BadImageFormatException) { 需要落盘加载 = true; }
                    if (需要落盘加载)
                    {
                        临时目录 = Path.Combine(Path.GetTempPath(), 资源名);
                        临时文件 = Path.Combine(临时目录, 程序集短名 + ".dll");
                        if (!File.Exists(临时文件)) { CreateDirectory; 写文件; }
                        已加载程序集 = Assembly.LoadFile(临时文件);
                    }
                    程序集缓存.Add(程序集全名, 已加载程序集);
                    return 已加载程序集;
                }
                catch { }
            }
        }
        catch { }
        结果 = null;
    }
    else 结果 = (Assembly)缓存命中;
}
return 结果;
```

要点：

- **两层 `try/catch { }`** 都是「静默失败」——资源不存在、解密失败、加载失败一律返回 `null`，
  由 CLR 继续按默认规则找程序集；这也是保护器「查不到就让我自己失败」的写法。
- `Add(程序集全名, 已加载程序集)` 在锁内执行，缓存键是**带版本的全名**。
- 落盘分支的目录名就是资源名（GUID 前缀串），文件名是程序集短名。

## 恶意行为 / 敏感操作

| 类型 | 具体行为 | 说明 |
| --- | --- | --- |
| 隐蔽加载 | `Assembly.Load(byte[])` 从内存加载依赖，不经磁盘 | 常规 .NET 资源加载器的做法；也让「文件是否存在」不再是 IOC |
| **落盘明文** | `%TEMP%\{资源名}\{短名}.dll` 写原始 DLL 再 `LoadFile` | 这是最直接的取证点：运行后 TEMP 里会留下解包后的二进制 |
| 全局钩子 | `AppDomain.CurrentDomain.AssemblyResolve += …` | 影响整个进程的程序集解析 |
| 密钥存储 | `RSACryptoServiceProvider.UseMachineKeyStore = true` | 把 RSA 密钥容器指向机器级存储（影响后续所有 RSA 操作） |
| 名称混淆 | 资源名 = 固定前缀 + Base64(MD5(UTF16(短名))) | 资源清单里看不到任何可读 DLL 名 |

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 资源名前缀 | `b0494a1f-4bd3-` | 内嵌程序集资源名的固定前缀 |
| 资源名算法 | `"b0494a1f-4bd3-" + Base64(MD5(UTF16(程序集短名)))` | 由短名反推资源名 |
| 临时目录 | `%TEMP%\{资源名}\` | 落盘分支 |
| 临时文件 | `{程序集短名}.dll` | 落盘分支 |
| 后缀处理 | `Replace("file:///", "")` | 位于 `IndexerConfig.获取程序集路径`，同属保护器工具链 |
| API | `AppDomain.AssemblyResolve`、`Assembly.Load(byte[])`、`Assembly.LoadFile`、`GetManifestResourceStream` | 加载链 |
| 属性 | `RSACryptoServiceProvider.UseMachineKeyStore = true` | 机器级密钥容器 |
| 异常类型 | `FileLoadException`、`BadImageFormatException` | 决定是否落盘 |
| Token | 类型 `0x02000017`；方法 `0x06000099/9A/9B`；字段 `0x04000042/43` | 元数据行号 |
