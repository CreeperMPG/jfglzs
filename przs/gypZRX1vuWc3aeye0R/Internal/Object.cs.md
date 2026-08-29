# Object.cs — IntelliLock 保护壳的试用期弹窗残留

## 文件作用
反混淆后真实身份：**.NET Reactor / IntelliLock 加壳器注入的"未注册试用版"提示代码**。`smethod_0`
在首次调用时弹窗 `"This assembly is protected by an unregistered version of \"IntelliLock\"!"`
（标题 `www.eziriz.com`，eziriz 即 IntelliLock/.NET Reactor 厂商）；若 MessageBox 失败则改为
执行一段 JS `alert`（`ManageSchema` 空实现接收，属于壳的浏览器弹窗降级路径）。`_Stub` 保证只弹一次。
这是加壳器水印/试用提示，与本木马业务无关，但**证实程序经 .NET Reactor 加壳**（与 ILLicenseModule 呼应）。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Object | IntelliLock 未注册试用提示（壳注入代码） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `smethod_0` | `弹试用提示` | 首次调用弹 MessageBox，失败则走 JS alert 路径 |
| `ManageSchema` | `JS alert 降级接收` | 空方法（壳的浏览器弹窗回调） |
| `_Stub` | `只弹一次标志` | 静态 bool |

## 关键方法与流程
### 弹试用提示（smethod_0）
- **流程**：1. `_Stub` 为 false 则置 true；2. `MessageBox.Show("This assembly is protected by an
  unregistered version of \"IntelliLock\"!", "www.eziriz.com")`；3. 抛异常时调用
  `ManageSchema("<script language='javascript'>alert(...)</script>")`（空实现）。

## 恶意行为 / 敏感操作
- 无恶意行为；属加壳器（.NET Reactor/IntelliLock，eziriz.com）的未注册水印弹窗。

## IOC / 硬编码
- 域名：`www.eziriz.com`（IntelliLock/.NET Reactor 厂商）
- 水印文本：`This assembly is protected by an unregistered version of "IntelliLock"!`
