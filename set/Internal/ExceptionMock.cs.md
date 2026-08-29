# Internal/ExceptionMock.cs — 注册表读写封装 + SHA256 密码哈希 + 程序入口点

## 文件作用
这是本样本的"工具/入口"模块，三个职责：
1. 一组基于 `Microsoft.Win32.RegistryKey` 的注册表增删改查封装（读/写/删值、建/删键、查键存在）；
2. `InsertProperty`：SHA256(输入+"bfdshgs") 十六进制小写输出，用于密码哈希（写入 `HKCU\Software\n` 并在 load 窗体比对）；
3. `InvokeProperty`：程序入口之一——启用视觉样式后显示 load 窗体并进入消息循环（`Application.Run()`），**说明本文件即恶意程序主入口点所在**（背景确认：InvokeProperty→Application.Run 显示 load 窗体）。
另有 CryptoAPI（advapi32）P/Invoke 声明（CryptAcquireContext/CryptCreateHash/CryptHashData/CryptGetHashParam/CryptDestroyHash/CryptReleaseContext）——声明了 SHA 哈希相关的原生 API 但**本文件内未实际调用**，疑为壳/后续载荷预留。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| ExceptionMock（StandardModule，VB 模块） | 注册表工具 + 哈希 + 入口点 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| InsertProperty(s) | HashPassword_SHA256加盐 | SHA256(s+"bfdshgs") 转小写 hex |
| InvokeProperty() | Main_程序入口 | 显示 load 窗体 + Application.Run() |
| PopProperty(key,sub,name,def) | RegistryGetValue_读注册表值 | OpenSubKey 读值，键不存在返回默认值（含 VB 错误处理样板） |
| TestProperty(key,sub,name,val,kind) | RegistrySetValue_写注册表值 | 键不存在先 CreateSubKey 再 SetValue，异常转中文错误 |
| RegisterProperty(key,sub,name) | RegistryDeleteValue_删注册表值 | DeleteValue，异常转中文错误 |
| EnableProperty(key,sub) | RegistryKeyExists_键是否存在 | OpenSubKey != null |
| StartProperty(key,sub) | RegistryEnsureKey_确保键存在 | 不存在则 CreateSubKey |
| DisableProperty(key,sub) | RegistryDeleteKey_删注册表键 | DeleteSubKey |
| _DecoratorMock / _CandidateMock | 随机进程名 / 随机进程名.exe | 由 load.PrepareProperty 赋值，PushProperty 用于杀同名进程 |
| _TokenMock=1 / utilsMock / statusMock / m_TestsMock=2 | VB 内部错误常量 | ProjectData 错误处理用（-268435456=0xF0000000，32771=0x8003） |

## 关键方法与流程

### HashPassword_SHA256加盐（InsertProperty）
1. `SHA256.ComputeHash(UTF8(输入 + "bfdshgs"))`
2. 逐字节 `ToString("x2")` 拼接 → 64 位小写 hex
3. 调用方：Form1.ConcatProperty（写 `HKCU\Software\n`）、load.UpdateProperty（比对）

### Main_程序入口（InvokeProperty）
1. `Application.EnableVisualStyles()` / `SetCompatibleTextRenderingDefault(false)`
2. `Reader.ReflectProperty.load.Show()`（经 VB My.Forms 代理创建 load 窗体实例）
3. `Application.Run()`（无参重载，消息循环直至所有窗体关闭）

### RegistryGetValue_读（PopProperty）
- 反编译残留扁平化状态机（num2/N/switch + catch filter `obj is Exception & num != 0 & num4 == 0`），还原后即：`OpenSubKey(子键)` → 非空则 `GetValue(值名, 默认)`，否则返回默认值。功能与直接写法等价，属 VB 编译器对 On Error 结构的还原产物。

### RegistrySetValue_写（TestProperty）
- 可写打开子键；不存在则创建；`SetValue(名, 值, 类型)`；异常抛"写入注册表值时发生错误: ..."
- 全样本几乎所有恶意注册表写入都走这里（见 Form1.cs.md 的 IOC 清单）

## 恶意行为 / 敏感操作
- 入口点：InvokeProperty 被壳/启动链调用即弹出密码门与设置界面
- 密码哈希盐 "bfdshgs" 硬编码（凭此可离线爆破密码）
- 全套注册表读写原语被恶意配置逻辑复用
- CryptoAPI 声明（SHA 原生接口）为潜在加解密后门

## IOC / 硬编码
- 哈希盐：`bfdshgs`
- 加密 API：advapi32.dll CryptAcquireContext / CryptCreateHash / CryptHashData / CryptGetHashParam / CryptDestroyHash / CryptReleaseContext（声明未用）
- 密码哈希存放：`HKCU\Software\n`
