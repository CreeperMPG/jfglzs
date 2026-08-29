# ClassConfig.cs — 自定义特性类（加壳器标记用）

## 文件作用
一个自定义 `Attribute` 类，含 3 个布尔属性（默认 true）与 1 个字符串属性（默认 ""）。此类是加壳/混淆工具（.NET Reactor 或 IntelliLock）用来给程序集成员打标记的特性（例如标记保护范围/排除项），运行时无任何行为。`RegisterRegistry()` 为恒 true 桩。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| ClassConfig | 混淆器标记特性 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `PushConfig/InitConfig` | 属性 A get/set | bool，默认 true |
| `ResolveConfig/WriteConfig` | 属性 B get/set | bool，默认 true |
| `RunConfig/DefineConfig` | 属性 C get/set | bool，默认 true |
| `NewConfig/CustomizeConfig` | 属性 D get/set | string，默认 "" |

## 说明
- 无恶意行为，无 IOC；属性值含义仅对加壳器有意义，反编译产物中未见消费方（属运行时反射标记）。
