# Internal/GetterVisitor.cs — 依赖程序集嵌入资源解密加载器（AssemblyResolve 钩子）

## 文件作用
注册 `AppDomain.CurrentDomain.AssemblyResolve` 事件：当 CLR 需要加载缺失的依赖程序集时，按**程序集名的 MD5 哈希**定位同程序集内的**嵌入资源**（资源名形如 `b0494a1f-4bd3-<base64(MD5(程序集名))>`），读出字节后直接 `Assembly.Load`；若因文件加载异常（FileLoadException/BadImageFormatException）则落盘到 `%TEMP%\<资源名>\<程序集名>.dll` 再用 `Assembly.LoadFile`。这是 .NET Reactor 的"嵌入式依赖程序集"机制，用于隐藏真实程序集、对抗静态分析。已加载结果缓存在 `Hashtable _EventVisitor`。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `GetterVisitor` | 程序集解析器（嵌入资源解密加载） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `smethod_0` | 注册 AssemblyResolve | 一次性注册（`m_UtilsVisitor` 防重入） |
| `ReflectVisitor` | AssemblyResolve 处理器 | 解密并加载嵌入程序集 |
| `FlushVisitor` | 取程序集简单名 | 去掉 `, Version=..., Culture=..., PublicKeyToken=...` |
| `_EventVisitor` | 已加载程序集缓存 | Hashtable：简单名 → Assembly |
| `m_UtilsVisitor` | 是否已注册标志 | 防重复挂接 |

## 关键方法与流程

### 注册 AssemblyResolve（原名 `smethod_0`）
- **流程**：`AppDomain.CurrentDomain.AssemblyResolve += ReflectVisitor`（只挂一次）。

### AssemblyResolve 处理器（原名 `ReflectVisitor`）
- **流程**：
  1. 锁 `_EventVisitor`；取请求名（Trim）；缓存命中→直接返回缓存的 Assembly。
  2. 未命中：`RSACryptoServiceProvider.UseMachineKeyStore = true`（兼容旧系统）；取简单名 `FlushVisitor`。
  3. 计算资源名：`text3 = "b0494a1f-4bd3-" + Convert.ToBase64String(ComparatorVisitor.CloneVisitor(Encoding.Unicode.GetBytes(简单名)))`——即 **MD5(简单名) 的 Base64**。
  4. `GetManifestResourceStream(text3)` 取嵌入资源；BinaryReader 读全部字节。
  5. 先试 `Assembly.Load(bytes)`；捕获 `FileLoadException`/`BadImageFormatException` 后：`Path.Combine(Path.GetTempPath()+text3, 简单名+".dll")` 落盘（目录不存在则创建），再 `Assembly.LoadFile(path)`。
  6. 成功→加入缓存并返回；任一步失败→返回 null（CLR 继续走默认解析）。
- **关键调用/API**：`AppDomain.AssemblyResolve`、`GetManifestResourceStream`、`Assembly.Load/LoadFile`、`MD5`（经 `ComparatorVisitor.CloneVisitor`）、`%TEMP%` 落盘。

## 恶意行为 / 敏感操作
- **程序集隐藏**：依赖程序集以密文资源嵌入主程序，运行期才解密加载（反静态分析/反提取）。
- **临时文件释放**：加载失败时向 `%TEMP%` 释放真实 DLL（可能触发杀软文件监控，故优先内存加载）。

## IOC / 硬编码
- **资源名模式**：`b0494a1f-4bd3-<Base64(MD5(程序集名))>`。
- **落盘路径**：`%TEMP%\<资源名>\<程序集名>.dll`。
