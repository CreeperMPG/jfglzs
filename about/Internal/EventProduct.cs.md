# EventProduct.cs — IntelliLock（Eziriz）未注册版本提示模块

## 文件作用
. NET Reactor/IntelliLock 保护壳的"未注册授权"提示模块：程序首次加载时弹窗/弹 JS 脚本，警告"此程序集受未注册版 IntelliLock 保护"。属于壳的广告/授权水印，无业务逻辑。方法名被混淆。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| EventProduct | IntelliLock 未授权水印提示 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `smethod_0` | 显示未注册提示（一次性） | `pageProduct` 标志防止重复弹窗 |
| `PublishProduct(object)` | 备用脚本提示通道 | 空实现（Web 场景用） |
| `pageProduct` | 是否已提示标志 | — |
| `StopListener` | 混淆桩 | 恒 false |

## 关键方法与流程
- `smethod_0`：若未提示过 → 置标志 → try `MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com")`，catch 后调用 `PublishProduct`（注入 `<script>alert(...)</script>`，Web 宿主场景）。

## 恶意行为 / 敏感操作
- 无业务恶意行为；提示文本确认了加壳工具为 **Eziriz IntelliLock / .NET Reactor 未注册版**（分析情报价值）。

## IOC / 硬编码
- 字符串：`This assembly is protected by an unregistered version of "IntelliLock"!`
- 标题/域名：`www.eziriz.com`
- 脚本：`<script language='javascript'>alert( "This assembly is protected by an unregistered version of IntelliLock!" );</script>`
