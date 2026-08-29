# Internal/HelperMock.cs — IntelliLock 未注册版提示弹窗

## 文件作用
加壳工具（Eziriz IntelliLock）**试用版提醒**存根：`smethod_0()` 首次调用时弹 MessageBox "This assembly is protected by an unregistered version of IntelliLock!"（标题 www.eziriz.com），弹窗失败则回退执行一段 HTML script 字符串（`ValidateMock`，实际为空实现）。这是壳特征而非样本自身逻辑；样本正式代码里未见对其调用，属于壳残留。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| HelperMock | 壳试用提示（一次性） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| smethod_0() | ShowTrialNotice_弹未注册提示 | 标志位保护只弹一次 |
| ValidateMock(obj) | 空回退实现 | 参数是 <script>alert(...)</script> 字符串，函数体为空 |
| SortClass() / PrintClass() | 占位 | true / false |
| _ParamsMock | 是否已提示标志 | bool |

## 恶意行为 / 敏感操作
- 无恶意行为；确认样本被 **IntelliLock（Eziriz）** 加壳的直接证据

## IOC / 硬编码
- 弹窗文本：`This assembly is protected by an unregistered version of "IntelliLock"!`
- 弹窗标题：`www.eziriz.com`
