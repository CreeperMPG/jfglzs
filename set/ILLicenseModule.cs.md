# ILLicenseModule.cs — 壳授权模块占位（IntelliLock 特征）

## 文件作用
加壳工具生成的**授权/许可模块存根**（类名含 "License"，与 HelperMock 的 IntelliLock 提示、TestMock 的 `{11111-22222-...}` 特征 GUID 同属 Eziriz IntelliLock 壳）。仅含两个返回常量的占位方法，无实际逻辑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| ILLicenseModule | 壳授权占位 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| EnableClass() | 占位 | return true |
| StopClass() | 占位 | return false |

## 恶意行为 / 敏感操作
- 无；再次佐证加壳器为 IntelliLock/.NET Reactor 家族

## IOC / 硬编码
- 无
