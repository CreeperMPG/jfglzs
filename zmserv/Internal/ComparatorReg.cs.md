# Internal/ComparatorReg.cs — IntelliLock 未注册许可提示（加壳器残留）

## 文件作用
本文件是 .NET Reactor / IntelliLock（eziriz 产品）**未注册版本提示**的残留代码：`smethod_0` 在首次调用时弹出一个 MessageBox，声明程序受"IntelliLock"保护并指向 `www.eziriz.com`。这证明该恶意程序曾用未注册的 .NET Reactor/IntelliLock 加壳。其余方法为混淆填充。**无业务恶意逻辑**。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `ComparatorReg` | IntelliLock 许可提示（一次性） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `m_TestsReg` | 提示已显示标志 | 保证只弹一次 |
| `smethod_0` | 显示未注册提示 | MessageBox / 兜底 JS alert |
| `QueryList(object)` | 兜底提示方法 | 空实现 |
| `RunParser` | 混淆填充 | 恒真 |

## 关键方法与流程

### 未注册提示（原名 `smethod_0`）
- **流程**：1. 若 `m_TestsReg` 为 false → 置 true；2. `MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com")`；3. 若 MessageBox 抛异常 → 调用 `QueryList("<script language='javascript'>alert(...)</script>")`（空实现）。
- **关键调用/API**：`System.Windows.Forms.MessageBox.Show`。
- 意义：仅用于识别加壳器（IOC），不影响行为链。

## 恶意行为 / 敏感操作
- 无（弹窗本身是加壳器水印）。可被攻击者当作"是否已注册"的环境探测点。

## IOC / 硬编码
- 字符串：`This assembly is protected by an unregistered version of "IntelliLock"!`、`www.eziriz.com`
- 兜底载荷：`<script language='javascript'>alert( "This assembly is protected by an unregistered version of IntelliLock!" );</script>`
- 加壳器指纹：IntelliLock / .NET Reactor（eziriz.com）
