# Internal/MappingConfig.cs 分析

## 文件作用

`uninstall.exe` 的**卸载/回滚核心**。VB 标准模块（`[StandardModule]`，全静态方法），没有界面，
只提供一组「把管理端做过的事反向执行掉」的方法：结束进程、删文件、改注册表、删服务、还原 hosts、
删 IFEO 劫持、导入 huifu.reg、重启资源管理器。

调用关系：

- `Form1.SearchReader`：本机**没有**设置卸载密码时，直接调用 `执行卸载()`；
- `Form2.InsertReader`：有密码时，用户输对密码后调用 `执行卸载()`；
- 本文件里的 `计算密码哈希` 被 `Form2` 用来校验密码；其余注册表小工具只在本文件内部使用。

## 类与职责

| 类/模块 | 真实职责 |
| --- | --- |
| `卸载核心`（原 `MappingConfig`） | 卸载主流程 + 注册表读写小工具 + 密码哈希 + 一组未使用的 CryptoAPI 声明 |
| `执行卸载()`（原 `PopReader`） | 一次完整卸载：杀进程 → 删文件 → 恢复策略 → 还原 hosts → 删服务/IFEO → 导入 huifu.reg → 重启 explorer |
| `计算密码哈希()`（原 `CustomizeReader`） | `SHA256(UTF8(密码 + "bfdshgs"))` 的小写十六进制串 |
| `读取注册表值()`（原 `SortReader`） | 读值，键不存在返回默认值，出错返回 `null`（On Error Resume Next） |
| `写入注册表值()`（原 `ResetReader`） | 写值，子键不存在则创建；失败抛中文异常 |
| `删除注册表值()`（原 `DisableReader`） | 删值，缺失不报错；失败抛中文异常 |
| `注册表键存在()`（原 `UpdateReader`） | 判断子键是否存在，异常一律当 false |
| `确保注册表键存在()`（原 `RemoveReader`） | 不存在就创建；无调用点 |
| `删除注册表键()`（原 `CollectReader`） | 存在就删；无调用点 |
| `CryptAcquireContext` 等 6 个 DllImport | 完整的 CryptoAPI 哈希流程声明，**全工程没有调用点**，保护器遗留 |

## 反混淆命名映射

| 混淆名 | 反混淆名 | 说明 |
| --- | --- | --- |
| `MappingConfig` | `卸载核心` | 类是「卸载」本体，原名与语义无关 |
| `PopReader()` | `执行卸载()` | 主流程 |
| `CustomizeReader(string)` | `计算密码哈希(string 明文密码)` | SHA256 + 固定盐 |
| `SortReader(RegistryKey, string, string, object)` | `读取注册表值(根键, 子键路径, 值名, 默认值)` | 参数 `registryKey_0/string_0/string_1/object_0` 全部还原 |
| `ResetReader(...)` | `写入注册表值(..., 值类型)` | 参数 `registryValueKind_0` → `值类型` |
| `DisableReader(...)` | `删除注册表值(...)` |  |
| `UpdateReader(...)` | `注册表键存在(...)` |  |
| `RemoveReader(...)` | `确保注册表键存在(...)` |  |
| `CollectReader(...)` | `删除注册表键(...)` |  |
| `m_RequestConfig` | `hosts文件路径` | `%SystemRoot%\drivers\etc\hosts` |
| `_ImporterConfig` | `hosts备份文件路径` | `...\etc\hosts-bak` |
| `m_MethodConfig` | `缓存目录` | `C:\windows\jf` |
| `containerConfig` | `进程名` | 管理端主进程名（运行时随机生成 5 个字母） |
| `strategyConfig` | `进程文件名` | 进程名 + `.exe`（本工程不读） |
| `m_ObserverConfig` | `候选安装目录` | `C:\Program Files (x86)\` + 进程名第 2~5 位（本工程不读） |
| `m_TemplateConfig` / `m_ClassConfig` / `contextConfig` / `_ResolverConfig` | `未使用常量1..4` | 私有常量，无引用 |
| 局部 `obj`（Run 键值） | `运行项` |  |
| 局部 `obj2` | `安装目录` | Run 值去掉末尾 9 个字符 |
| 局部 `text` / `text2` / `text3` | `服务名` / `管理进程名` / `守护进程名` | `"zmserv"` / 随机名 / `"jfglzsn"` |
| 局部 `text4` / `text5` / `text6` | `注册表安装路径` / `注册表路径副本` / `特殊设置目录` | `text5` 只是传手变量，原样保留 |
| 局部 `array` / `array2` | `进程组` / `守护进程组` | 两轮循环杀进程复用 |
| 局部 `registryKey` / `registryKey2` | `注册表根` / `待删键` |  |
| `intptr_0` 等 DllImport 参数 | `提供程序句柄` / `哈希句柄` / `数据` / `缓冲长度` … | 按 CryptoAPI 语义 |

## 关键方法与流程

### `执行卸载()` —— 一次完整卸载（约 200 行，逐条容错）

1. **找安装目录**：读 `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN` 的 `prozs`；若为空再读
   `WOW6432NODE` 同名键；取到的值用 `Strings.Mid(值, 1, Len(值) - 9)` 去掉末尾 `\prozs.exe` 得到安装目录。
2. **杀进程**：`do { 杀 管理进程名; 杀 jfglzsn } while (两个进程都杀不到)` ——
   用「Kill 抛异常说明进程已不存在」来判断，直到两轮都拿不到进程才退出。
3. **清缓存目录**：`C:\windows\jf` 下的 `jfglzsn.dat` / `przs.dat` / `zmserv.dat` 先去只读属性再删，最后删目录。
4. **清安装目录**：`jfglzsn.exe` / `zmserv.exe` / `przs.exe` / `yl.wav` 同样先去属性再删。
5. **恢复策略**：`DisableCMD=0`、`DisableRegistryTools=0`、`NoRun=0`（命令提示符、注册表编辑器、运行）。
6. **读 `HKCU\Software\jfglzs` 的 `dzjs`**（末尾 16 位校验串去掉）→ 去 `SpecialSet.exe` 的只读属性。
7. **停服务**：`net stop zmserv`（隐藏窗口、等待返回），`Thread.Sleep(3000)`，再 Kill `zmserv` 进程。
8. **删自启动**：`HKLM\...\RUN` 与 `WOW6432NODE\...\RUN` 下的 `prozs`、`jfglzsn` 四个值。
9. **开机菜单**：`cmd /c bcdedit /set {bootmgr} displaybootmenu yes`（最小化、不等待）。
10. **恢复 USB**：`usbstor` 的 `Start=3`，写 `CurrentControlSet` 与 `ControlSet001/002/003` 四处。
11. **解锁屏**：`DisableLockWorkstation=0`；删 `HKCU\Software` 的 `n` / `dtmm` / `czmm`。
12. **删配置根**：`HKCU\Software\jfglzs`（先 OpenSubKey 判断存在再删）。
13. **删 IFEO 劫持**：21 条 `Image File Execution Options\{进程名}` 子键
    （`taskkill.exe`、`ntsd.exe`、`sidebar.exe`、扫雷/纸牌/麻将等游戏、`Magnify.exe`、`sethc.exe`、
    `tasklist.exe`、`route.exe`；其中 `PurblePlace.exe`、`taskkill.exe` 各出现两次——原码就是删两遍）。
14. **删浏览器策略键**：`Software\Policies\Google\Chrome`、`Software\Policies\Microsoft\Edge`。
15. **还原 hosts**：`hosts` 与 `hosts-bak` 去只读 → `drivers\etc` 目录去只读 → `File.Copy(hosts-bak, hosts, true)`。
16. **恢复浏览器策略**：Explorer `NoFolderOptions=0`、隐藏文件 `CheckedValue=1`、IE `Zones\3` 的
    `1803=0`/`2200=0`、IE `NoBrowserSaveAs=0`、Edge/Chrome `DownloadRestrictions=0`/`SaveAs=1`/
    `DeveloperToolsAvailability=0`、Firefox `DisableDownloads=0`/`BlockAboutDownloads=0`/
    `DeveloperToolsAvailability=0`。
17. **删服务**：`sc delete zmserv`。
18. **导入注册表**：把安装目录里的 `huifu.wav`（其实是 .reg 文本，改扩展名躲检测）复制成 `huifu.reg`，
    用 `regedit.exe /s <安装目录>\huifu.reg` 静默导入；等 2 秒后删 `huifu.reg`、`yl.reg`。
19. **恢复界面策略**：`NoTrayContextMenu=0`、`ShowTaskViewButton=1`。
20. **恢复关机行为**：删 `AutoEndTasks`，`HungAppTimeout` 写字符串 `"3000"`、`WaitToKillAppTimeout` 写 `"10000"`，
    删 `StartupDelayInMSec`，恢复 `C:\Program Files\Microsoft Games` 属性。
21. **恢复网络/提示**：`Tcpip6 DisabledComponents=0`、`TabletTip TipbandDesiredVisibility=1`。
22. **再杀一轮进程 + 再清一轮缓存目录/安装目录**（含 `huifu.wav`）。
23. **重启外壳**：恢复 `C:\Windows\Microsoft.NET` 属性 → `Sleep 2000` → 杀掉所有 `explorer.exe` →
    `Process.Start("explorer.exe")`。

关键点：整段都处在 VB 的 `On Error Resume Next` 之下，**每条语句出错只被吞掉、继续下一条**。
反混淆后每条语句单独 `try { … } catch { }`（IFEO 那 21 条尤其不能合并，否则第一条不存在的键就会中断后面的删除）。

### `计算密码哈希(string)`

`SHA256(UTF8(密码 + "bfdshgs"))` → 逐字节 `ToString("x2")` 拼接。`Form2` 拿用户输入算一遍，
与 `HKCU\Software` 的 `n` 比较，相等才允许卸载。

### 注册表小工具

- `读取注册表值`：`OpenSubKey` → `GetValue(值名, 默认值)`；键不存在返回默认值；**出错返回 `null`**（原版局部变量未赋值）。
- `写入注册表值`：`OpenSubKey(子键, true)`；为 `null` 时 `CreateSubKey` 后写一次，否则直接写一次；失败抛 `"写入注册表值时发生错误: …"`。
- `删除注册表值`：`DeleteValue(值名, false)`（值缺失不报错）；失败抛 `"删除注册表值时发生错误: …"`。
- `注册表键存在` / `确保注册表键存在` / `删除注册表键`：三个小包装，后两个无调用点。

## 恶意行为 / 敏感操作

| 类型 | 具体行为 |
| --- | --- |
| 进程查杀 | 按进程名 `Kill()`：随机 5 字母进程名、`jfglzsn`、`zmserv`、`explorer`（最后重启） |
| 文件删除 | `C:\windows\jf\*.dat` + 目录；安装目录下 3 个 exe、2 个 wav、2 个 reg |
| 文件覆盖 | `hosts-bak` → `hosts`（还原被劫持的域名解析） |
| 注册表写入 | HKLM 的 `Run`/`usbstor`/`Tcpip6`/IFEO/浏览器策略；HKCU 的 Explorer/IE/Desktop/Run 策略 |
| 注册表删除 | 整个 `HKCU\Software\jfglzs`；IFEO 21 条；Chrome/Edge 策略根键 |
| 服务控制 | `net stop zmserv`、`sc delete zmserv` |
| 命令执行 | `Interaction.Shell`：`net stop`、`sc delete`、`bcdedit /set {bootmgr} displaybootmenu yes`、`regedit.exe /s`、`explorer.exe` |
| 权限要求 | 写 HKLM 与服务需要**管理员**；普通用户下多数步骤会失败并被 `catch` 吞掉 |
| 性质说明 | 本文件是「管理端加固动作的逆操作」，但能力本身＝任意杀进程 + 删文件 + 改注册表 + 执行命令；把常量换成别的值就是一个破坏性工具，分析时按「高风险系统操作集合」对待 |

## IOC / 硬编码

| 类型 | 值 | 用途 |
| --- | --- | --- |
| 进程名 | `zmserv`、`jfglzsn`、`{5 个 i..r 字母}`（运行时生成）、`explorer` | 查杀目标 / 重启外壳 |
| 文件路径 | `C:\windows\jf` | 管理端缓存目录 |
| 文件名 | `jfglzsn.dat`、`przs.dat`、`zmserv.dat` | 缓存目录内数据文件 |
| 文件名 | `jfglzsn.exe`、`zmserv.exe`、`przs.exe`、`yl.wav`、`huifu.wav`、`yl.reg`、`huifu.reg`、`SpecialSet.exe` | 安装目录内文件 |
| 路径 | `%SystemRoot%\drivers\etc\hosts`、`…\hosts-bak` | 域名劫持与还原 |
| 路径 | `C:\Windows\System32\drivers\etc`、`C:\Windows\Microsoft.NET`、`C:\Program Files\Microsoft Games` | 只读属性处理 |
| 注册表 | `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN`（`prozs`、`jfglzsn`） | 自启动 |
| 注册表 | `HKLM\SOFTWARE\WOW6432NODE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN`（同名） | 32 位视图自启动 |
| 注册表 | `HKCU\Software\jfglzs`（值 `dzjs`） | 安装目录 + 校验串 |
| 注册表 | `HKCU\Software`：`n`（密码哈希）、`dtmm`、`czmm` | 卸载密码 / 时间限制标记 |
| 注册表 | `HKLM\SYSTEM\{CurrentControlSet,ControlSet001,ControlSet002,ControlSet003}\Services\usbstor` → `Start` | USB 存储开关 |
| 注册表 | `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\…` | IFEO 劫持（21 个进程名见上文） |
| 注册表 | `Software\Policies\Google\Chrome`、`Software\Policies\Microsoft\Edge`、`SOFTWARE\Policies\Mozilla\Firefox` | 浏览器策略 |
| 注册表 | `HKCU\…\Policies\Explorer`、`…\Policies\System`、`…\Policies\Microsoft\Windows\System` | 系统策略 |
| 注册表 | `SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters` → `DisabledComponents` | IPv6 |
| 注册表 | `HKCU\SOFTWARE\Microsoft\TabletTip\1.7` → `TipbandDesiredVisibility` | 触控提示条 |
| 命令 | `net stop zmserv`、`sc delete zmserv`、`cmd /c bcdedit /set {bootmgr} displaybootmenu yes`、`regedit.exe /s …\huifu.reg` | 系统操作 |
| 密码盐 | `bfdshgs` | 卸载密码哈希的固定盐 |
| 异常文案 | `写入注册表值时发生错误: `、`删除注册表值时发生错误: `、`确保注册表键存在操作失败: `、`删除注册表键时发生错误: ` | 中文错误消息（可作为家族指纹） |
