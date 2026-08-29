# PoolConfig.cs — 嵌入程序集解析器（.NET Reactor 资源打包）

## 文件作用
.NET Reactor "Pack/Embed Assemblies" 功能的运行时：注册 `AppDomain.AssemblyResolve` 回调，当程序集加载不到时，从自身嵌入资源中按名称哈希取出字节流加载（内存加载，失败则释放到 %TEMP% 再 LoadFile）。属于加壳器运行时，无业务恶意逻辑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| PoolConfig | 嵌入程序集加载器 |
| `m_ComposerConfig` | 已加载程序集缓存（Hashtable，按程序集全名） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `smethod_0` | 注册 AssemblyResolve 钩子 | 幂等（_BridgeConfig 标志） |
| `InsertConfig` | AssemblyResolve 回调：从资源加载程序集 | |
| `PrepareConfig` | 截取程序集短名 | 去掉逗号后部分 |
| `_BridgeConfig` | 是否已挂钩标志 | |

## 关键方法与流程
### AssemblyResolve 回调（原名 InsertConfig）
1. 请求名去掉强名称部分得到短名 `text2`。
2. 资源名 = `"b0494a1f-4bd3-" + Base64(MD5(UTF16(text2)))`，从自身嵌入资源取流。
3. 优先 `Assembly.Load(字节)` 内存加载；失败（FileLoad/BadImageFormat）则写入 `%TEMP%\<资源名>\<短名>.dll` 后 `Assembly.LoadFile`。
4. 结果缓存进 Hashtable，避免重复解密。

## IOC / 硬编码
- 资源名前缀模板：`b0494a1f-4bd3-<Base64(MD5)>`
- 落盘路径：`%TEMP%\<资源名>\<程序集短名>.dll`
