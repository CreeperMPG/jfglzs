# RegProduct.cs — AssemblyResolve 处理器（从嵌入资源按需解包/落盘加载依赖 DLL）

## 文件作用
注册 `AppDomain.CurrentDomain.AssemblyResolve` 处理器（`smethod_0`，一次性），当程序集加载失败（被加壳合并进主程序的依赖 DLL 被引用）时，按"程序集名 → MD5 → 资源名"规则从主程序嵌入资源中取回程序集字节，优先内存 `Assembly.Load`，失败（FileLoadException/BadImageFormatException）则**解压到 `%TEMP%` 并 `Assembly.LoadFile`**。这是 .NET Reactor 的"资源合并/动态依赖"机制，静态分析时需要留意：大量真实业务代码藏在这些按需加载的嵌入 DLL 里，本文件是它们的"装载器"。方法名被混淆为 `*Product`，并带 `*Listener` 桩。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| RegProduct | 程序集按需解析/解包加载器 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `smethod_0` | 注册 AssemblyResolve 处理器 | 一次性注册 |
| `QueryProduct` | AssemblyResolve 事件处理 | 从嵌入资源加载依赖程序集 |
| `SetProduct` | 提取纯程序集名 | 去掉 `, Culture=...` 等后缀 |
| `m_SpecificationProduct` | 已解析程序集缓存 | Hashtable（线程锁保护） |
| `exceptionProduct` | 是否已注册标志 | — |
| `DestroyListener` | 混淆桩 | 恒 false |

## 关键方法与流程

### AssemblyResolve 处理器（QueryProduct）
- **流程**：
  1. 缓存命中则直接返回；
  2. `text2 = 纯程序集名`；资源名 = `"b0494a1f-4bd3-" + Base64(MD5(UTF16(text2)))`；
  3. `GetManifestResourceStream(资源名)` 读取字节；
  4. 尝试 `Assembly.Load(字节)`；
  5. 若抛 `FileLoadException` / `BadImageFormatException` → 写盘到 `%TEMP%\<资源名>\<程序集名>.dll`（目录不存在则创建）→ `Assembly.LoadFile(路径)`；
  6. 成功则加入缓存并返回；失败返回 null（交给 CLR 默认处理）。
- **关键调用/API**：`AppDomain.AssemblyResolve`、`Assembly.GetManifestResourceStream`、`Assembly.Load/LoadFile`、`InvocationProduct.CountReponse`（MD5）。

## 恶意行为 / 敏感操作
- 将加密/合并的依赖程序集**解包落盘到临时目录**执行（`%TEMP%` 释放文件，属于壳行为，可被检测为"释放并加载"）。
- 隐藏依赖 DLL：静态分析主程序 .cs 只能看到装载逻辑，看不到依赖代码本体。

## IOC / 硬编码
- 嵌入资源命名规则：`b0494a1f-4bd3-` + `Base64(MD5(UTF16(程序集名)))`
- 落盘路径：`%TEMP%\<资源名>\<程序集名>.dll`
- API：`RSACryptoServiceProvider.UseMachineKeyStore = true`（设置机器级密钥存储，壳残留）
