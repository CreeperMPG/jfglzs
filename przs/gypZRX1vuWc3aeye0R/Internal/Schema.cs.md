# Schema.cs — My.Computer 对象（VB.NET 计算机信息访问）

## 文件作用
反混淆后真实身份：**`My.Computer`**——继承 `Microsoft.VisualBasic.Devices.Computer` 的空子类，
由 `MyTemplate` 生成器产出，ConfuserEx 改名为 `Schema`。程序通过 `Account.StopBroadcaster`
（返回本类实例）访问计算机信息，实际业务使用点：bianhao.cs 的
`Strings.Right(Account.StopBroadcaster.Name, 6)` 取计算机名末 6 位。本身无任何逻辑；
`LogoutThread`/`CallThread` 为混淆器填充方法。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Schema | My.Computer（继承 VB `Computer` 类，提供 Name/文件系统/注册表等属性） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `Account.StopBroadcaster` | My.Computer 单例 | Account.cs 中返回 `Template<Schema>` 单例 |
| `LogoutThread`（恒 true）/`CallThread`（恒 false） | 混淆器填充方法 | 无意义 |

## 关键方法与流程
- 无自定义方法；所有能力继承自 `Microsoft.VisualBasic.Devices.Computer`。

## 恶意行为 / 敏感操作
- 无。被用于读取计算机名（bianhao 编号显示）。

## IOC / 硬编码
- 无。
