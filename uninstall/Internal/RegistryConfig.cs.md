# Internal/RegistryConfig.cs 分析

## 文件作用

保护器（IntelliLock）留下的「未注册版本」提醒类。**全工程没有任何调用点**，
逻辑本身只是弹一个 MessageBox；这是加壳工具试用版的广告/水印代码，不是「机房管理助手」的业务逻辑。

它同时也是一条重要线索：说明样本用**未注册的 IntelliLock** 加壳（与 `AssemblyInfo` 的
`SuppressIldasm`、`ILLicenseModule` 类互相印证）。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `未注册许可证提示`（原 `RegistryConfig`） | IntelliLock 未注册提示（未被调用） |
| `显示未注册提示()`（原 `smethod_0`） | 静态标志只弹一次；`MessageBox.Show` 失败时退化成一段 JS `alert` |
| `显示网页提示(object)`（原 `RateConfig`） | **空实现**（方法体为空） |
| `混淆器占位方法_恒假()`（原 `ViewParser`） | `return false` |
| `已提示过`（原 `_CustomerConfig`） | 只弹一次的标志 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `RegistryConfig` | `未注册许可证提示` | 类里全是 IntelliLock 提示文案，与「注册表」无关 |
| `smethod_0` | `显示未注册提示` |  |
| `RateConfig(object)` | `显示网页提示(object 提示脚本)` | 空方法 |
| `ViewParser` | `混淆器占位方法_恒假` | 占位 |
| `_CustomerConfig` | `已提示过` | 静态标志 |
| 参数 `object_0` | `提示脚本` |  |

## 关键方法与流程

```
if (!已提示过)
{
    已提示过 = true;
    try   { MessageBox.Show("This assembly is protected by an unregistered version of \"IntelliLock\"!", "www.eziriz.com"); }
    catch { 显示网页提示("<script language='javascript'>alert( … );</script>"); }
}
```

`MessageBox.Show` 在无交互桌面（服务/会话 0）会抛异常，于是走 `catch` 把一段 JS 交给
`显示网页提示` —— 而后者是空方法，等于什么都不做。也就是说：**在正常桌面环境下它只会弹一次广告框**。

之所以要 `try/catch`：这是保护器自检代码的通用写法，检测到「未授权/试用」时想尽办法提示。

## 恶意行为 / 敏感操作

| 类型 | 说明 |
| --- | --- |
| 广告/水印 | 弹 IntelliLock 未注册提示框（标题 `www.eziriz.com`） |
| 反分析线索 | 未注册加壳器 = 打包者用的是破解/试用版 IntelliLock，可作为家族/打包者关联特征 |
| 无实际危害 | 没有文件、注册表、网络操作；且无调用点，运行时不会执行 |

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 提示文案 | `This assembly is protected by an unregistered version of "IntelliLock"!` | MessageBox 正文 |
| 标题 | `www.eziriz.com` | MessageBox 标题（Eziriz 是 IntelliLock 厂商） |
| 脚本 | `<script language='javascript'>alert( "This assembly is protected by an unregistered version of IntelliLock!" );</script>` | 退化分支的 JS |
| Token | 类型 `0x02000016`；方法 `0x06000095/96/98`；字段 `0x04000041` | 元数据行号 |
