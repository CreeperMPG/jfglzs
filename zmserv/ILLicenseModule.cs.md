# ILLicenseModule.cs — IntelliLock 许可模块空壳

## 文件作用
仅含两个混淆填充方法（恒真/恒假）的空类。类名 `ILLicenseModule` 暗示其原为 IntelliLock 许可证模块（IL = IntelliLock），经脱壳后仅剩空壳，无任何逻辑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `ILLicenseModule` | 许可模块残留空壳 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `ResetParser` | 混淆填充 | 恒真 |
| `RestartParser` | 混淆填充 | 恒假 |

## 关键方法与流程
- 无。

## 恶意行为 / 敏感操作
- 无。

## IOC / 硬编码
- 类名含加壳器指纹：IntelliLock（eziriz）。
