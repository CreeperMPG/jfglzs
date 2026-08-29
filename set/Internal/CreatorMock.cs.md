# Internal/CreatorMock.cs — 混淆器标记 Attribute（ComVisible 元数据特性）

## 文件作用
一个自定义 Attribute 类，`AttributeUsage` 覆盖 Assembly/Class/Method/Field/Event/Property/Interface/Parameter/Delegate 等几乎所有目标且 `AllowMultiple=true`。这类"全能 Attribute"是加壳/混淆工具（IntelliLock/.NET Reactor 运行时或 ConfuserEx 后处理）用来在程序集上打标记的**元数据特征**，本身不含任何可执行逻辑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| CreatorMock : Attribute | 混淆器/壳标记特性（bool×3 + string×1 属性） |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| RemoveMock()/CloneMock(b) | FlagA 读/写 | 字段 _ListenerMock，默认 true |
| RunMock()/InitMock(b) | FlagB 读/写 | 字段 _RefMock，默认 true |
| PostMock()/CountMock(b) | FlagC 读/写 | 字段 m_WriterMock，默认 true |
| AssetMock()/FlushMock(s) | 字符串属性读/写 | 字段 _FacadeMock，默认 "" |
| CloneClass() | 占位 | return false |

## 恶意行为 / 敏感操作
- 无；属性默认值全 true / 空串，仅元数据用途

## IOC / 硬编码
- 无
