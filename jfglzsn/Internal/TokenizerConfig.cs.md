# Internal/TokenizerConfig.cs — VB My.* 命名空间基础设施（My.Forms/My.Application 等）

## 文件作用
VB.NET 编译器生成的 `My` 项目基础设施（`[HideModuleName]`、`[GeneratedCode("MyTemplate","8.0.0.0")]`），混淆器把成员改名为 `*Config`/`*Template` 假名。真实内容：`My.Computer`（DispatcherConfig）、`My.Application`（TemplateConfig）、`My.User`、`My.Forms`（DicConfig：Form1~Form4 的懒加载实例管理）、`My.WebServices`（ProccesorConfig）。`DicConfig.GetTemplate` 用线程静态 Hashtable 防递归创建表单；`AccountConfig<T>.DestroyTemplate` 是线程静态单例。全文件无恶意逻辑，但 `My.Forms.Form1` 的访问路径是主控代码拿到 Form1~Form4 实例的枢纽。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `TokenizerConfig` | My 命名空间入口（各属性惰性单例） |
| `TokenizerConfig.DicConfig` | My.Forms 表单集合（Form1~Form4 实例管理） |
| `TokenizerConfig.ProccesorConfig` | My.WebServices 集合（空壳） |
| `TokenizerConfig.AccountConfig<T>` | 线程静态单例容器 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `PublishTemplate` | My.Computer | 返回 DispatcherConfig 单例 |
| `PopTemplate` | My.Application | 返回 TemplateConfig 单例（应用启动器） |
| `CallTemplate` | My.User | 返回 User 单例 |
| `FindTemplate` | My.Forms | 返回 DicConfig（Form1~4 访问入口） |
| `ConcatTemplate` | My.WebServices | 返回 ProccesorConfig |
| `DestroyTemplate` | 获取线程静态单例 | 空则 Activator 创建 |
| `GetTemplate<T>` | 创建/复用表单实例 | 防递归创建（WinForms_RecursiveFormCreate 保护） |
| `AssetTemplate<T>` | 释放表单实例 | Dispose 并置空 |
| `RateTemplate<T>` | 创建/复用 WebService 实例 | 空壳 |
| `ViewTemplate<T>` | 释放 WebService 实例 | 置空 |
| `PushParser/ConcatParser/ManageParser/UpdateParser/MoveParser` 等 | 壳桩 | 恒 true/false |
| `m_MerchantConfig/m_SerializerConfig/_ProxyConfig/initializerConfig` | Form1~Form4 实例字段 | 表单缓存 |
| `m_FilterConfig` | 递归创建防护表 | ThreadStatic Hashtable |

## 关键方法与流程

### My.Forms 表单访问（原名 `FindTemplate.Form1~Form4`）
- **流程**：get 属性 → `GetTemplate<T>(缓存字段)`：缓存非空且未 Dispose→直接返回；否则查 `m_FilterConfig`（递归保护：创建期间同类型再次访问抛 `WinForms_RecursiveFormCreate`）；`Activator.CreateInstance<T>()` 创建后缓存；TargetInvocationException 时包装内部异常抛出。
- **关键调用/API**：`Activator.CreateInstance`、`IsDisposed`。

### 线程静态单例（原名 `AccountConfig<T>.DestroyTemplate`）
- **流程**：`m_CodeConfig` 为空→`Activator.CreateInstance<T>()`；返回。`[ThreadStatic]` 保证每线程一份。
- **关键调用/API**：`[ThreadStatic]`、`Activator`。

## 恶意行为 / 敏感操作
- 无；纯基础设施。但注意：`My.Forms` 暴露的 Form1~Form4 是主控（恶意）代码与锁屏窗体交互的通道。

## IOC / 硬编码
- 无网络/注册表 IOC。异常资源名：`WinForms_RecursiveFormCreate`、`WinForms_SeeInnerException`。
