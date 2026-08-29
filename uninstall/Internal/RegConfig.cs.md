# RegConfig.cs — 卸载器核心引擎：杀进程、删服务、清注册表、删文件、恢复系统策略

## 文件作用
本文件是整个"卸载器"组件的核心：`InvokeUtils()` 是一个超长的顺序卸载流程，包含两轮"杀进程 + 删文件"、停止并删除 `zmserv` 服务、删除开机自启项与大量注册表项、恢复被恶意软件篡改的系统策略（禁用 CMD/注册表编辑器/浏览器限制/IFEO 劫持等）、恢复 hosts 文件、导入隐藏的 .reg 恢复数据、重启 explorer 等。另提供注册表读写删除的工具函数与密码哈希函数 `VisitUtils`。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| RegConfig | 卸载执行器 + 注册表工具集 + 密码哈希 |
| P/Invoke Crypt* | CryptoAPI（MD5）声明，加壳运行时残留，本文件未实际调用 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `InvokeUtils` | 执行完整卸载 | 主流程（被 Form1/Form2 调用） |
| `VisitUtils` | 计算卸载密码哈希 | SHA256(密码+"bfdshgs") 十六进制 |
| `RemoveUtils` | 读取注册表值 | 键不存在返回默认值 |
| `ViewUtils` | 写入注册表值 | 键不存在则创建 |
| `ChangeUtils` | 删除注册表值 | |
| `RegisterUtils` | 判断注册表键是否存在 | |
| `CalcUtils` | 确保注册表键存在 | 不存在则创建，恒 true |
| `PatchUtils` | 删除注册表键（若存在） | |
| `m_HelperConfig` | 隐藏数据目录 | `C:\windows\jf` |
| `m_ConnectionConfig` | 随机进程名（5 字符） | Form1 生成 |
| `propertyConfig` | 随机进程名 + ".exe" | |
| `m_PrototypeConfig` | 安装目录模板 | `C:\Program Files (x86)\<4字符>` |
| `_ProducerConfig` | hosts 文件路径 | `%System%\drivers\etc\hosts` |
| `issuerConfig` | hosts 备份路径 | `%System%\drivers\etc\hosts-bak` |

## 关键方法与流程

### 执行完整卸载（原名 InvokeUtils）
- **职责**：清除恶意软件家族的进程、服务、自启动、文件与对系统的篡改，并恢复系统策略。
- **流程**（还原扁平化状态机，按真实执行顺序）：
  1. **定位安装目录**：读 `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\prozs`；为空则读 `HKLM\SOFTWARE\WOW6432Node\...\Run\prozs`。取值的路径去掉末尾 9 字符（= ".exe"+5 字符随机文件名），得到安装目录 `obj`。
  2. **第一轮杀进程**：循环 `Kill` 进程名 = 随机 5 字符名 与 `jfglzsn` 的进程，直到两者都为空。
  3. **删除隐藏目录** `C:\windows\jf\` 下 `jfglzsn.dat`、`przs.dat`、`zmserv.dat`（先清只读属性），再删目录本身。
  4. **删除安装目录文件**：`jfglzsn.exe`、`zmserv.exe`、`przs.exe`、`yl.wav`（先清属性）。
  5. **恢复系统策略（写回 0/1）**：
     - `HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD = 0`（解除禁用 CMD）
     - `HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\System\DisableRegistryTools = 0`（解除禁用注册表编辑器）
     - `HKCU\...\Policies\Explorer\NoRun = 0`（恢复运行框）
  6. 读 `HKCU\Software\jfglzs\dzjs`，取前 Len-16 字符得到路径，将该路径下 `SpecialSet.exe` 属性清为 Normal（解除隐藏/只读，供恢复工具使用）。
  7. **停止并杀死服务进程**：`Shell("net stop zmserv", Hide, wait)` → `Sleep(3000)` → Kill `zmserv` 进程。
  8. **删除开机自启值**：`HKLM\...\Run` 的 `prozs`、`jfglzsn`；`HKLM\SOFTWARE\WOW6432Node\...\Run` 的 `prozs`、`jfglzsn`。
  9. `Shell("cmd /c bcdedit /set {bootmgr} displaybootmenu yes")`（恢复开机启动菜单，便于修复系统）。
  10. **恢复 USB 存储**：`HKLM\SYSTEM\CurrentControlSet\Services\usbstor\Start = 3`，以及 ControlSet001/002/003 同样处理（恶意软件曾禁用 U 盘）。
  11. `HKCU\...\Policies\System\DisableLockWorkstation = 0`（恢复锁屏）。
  12. **删除密码/标记值**：`HKCU\software` 下 `n`（卸载密码）、`dtmm`、`czmm`。
  13. 删除 `HKCU\Software\jfglzs` 整个键（若存在）。
  14. **删除 IFEO 调试器劫持键**（恶意软件曾劫持以下工具/游戏使它们无法运行或用作诱饵）：`HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\` 下的 `taskkill.exe`、`ntsd.exe`、`sidebar.exe`、`Chess.exe`、`FreeCell.exe`、`Hearts.exe`、`Minesweeper.exe`、`PurblePlace.exe`、`Mahjong.exe`、`SpiderSolitaire.exe`、`bckgzm.exe`、`chkrzm.exe`、`shvlzm.exe`、`Solitaire.exe`、`winmine`、`Magnify.exe`、`sethc.exe`、`tasklist.exe`、`route.exe`（taskkill/PurblePlace 出现两次）。
  15. **删除浏览器组策略**：`HKLM\Software\Policies\Google\Chrome`、`HKLM\Software\Policies\Microsoft\Edge`（若存在）。
  16. **恢复 hosts 文件**：`File.Copy(hosts-bak → hosts, true)`（用备份覆盖被篡改的 hosts）。
  17. **继续恢复策略**：`NoFolderOptions=0`（恢复文件夹选项）、`SHOWALL\CheckedValue=1`（显示隐藏文件）、IE `Zones\3` 的 `1803=0`、`2200=0`（解除 IE 下载/表单限制）、`NoBrowserSaveAs=0`；Edge：`DownloadRestrictions=0`、`SaveAs=1`、`DeveloperToolsAvailability=0`；Chrome 同 Edge；Firefox：`DisableDownloads=0`、`BlockAboutDownloads=0`、`DeveloperToolsAvailability=0`。
  18. **删除服务**：`Shell("sc delete zmserv", Hide, wait)`。
  19. **导入隐藏恢复注册表文件**：把安装目录下伪装成音频的 `huifu.wav` 复制为 `huifu.reg` → `Shell("regedit.exe /s <dir>\huifu.reg", Hide, wait)` 静默导入 → `Sleep(2000)` → 删除 `huifu.reg` 与 `yl.reg`（清属性后删）。**说明：huifu.reg 内容不可见，可能承载剩余的键值恢复/文件删除（如 SpecialSet.exe 相关）。**
  20. **桌面/性能恢复**：`NoTrayContextMenu=0`、`ShowTaskViewButton=1`、删除 `Control Panel\Desktop\AutoEndTasks`、`HungAppTimeout=3000`（字符串）、`WaitToKillAppTimeout=10000`（字符串）、删除 `...\Explorer\Serialize\StartupDelayInMSec`；`C:\Program Files\Microsoft Games` 清属性；`Tcpip6\Parameters\DisabledComponents=0`（恢复 IPv6）；`TabletTip\1.7\TipbandDesiredVisibility=1`。
  21. **第二轮杀进程 + 删文件**：重复步骤 2~4（随机名进程与 `jfglzsn` 进程再次 Kill；`C:\windows\jf\*.dat` 与目录再删；安装目录下 `jfglzsn.exe`、`zmserv.exe`、`przs.exe`、`yl.wav`、`huifu.wav` 删除）。
  22. `C:\Windows\Microsoft.NET` 清属性（恶意软件曾设隐藏/系统属性）。
  23. `Sleep(3000)` → **Kill 全部 `explorer.exe` 进程 → 重新 `Process.Start("explorer.exe")`**（强制刷新桌面与资源管理器设置）。
  24. 结束。
- **关键调用/API**：`Process.GetProcessesByName().Kill()`、`Interaction.Shell`、`File.Delete/SetAttributes`、`Directory.Delete`、`RegistryKey.DeleteSubKey/DeleteValue/SetValue`、`regedit.exe /s`、`sc delete`、`net stop`、`bcdedit`、`shutdown`。

### 计算卸载密码哈希（原名 VisitUtils）
- **流程**：`SHA256(UTF8(输入 + "bfdshgs"))` → 逐字节转十六进制小写字符串。与 `HKCU\Software\n` 存值比对。

### 注册表工具函数
- `RemoveUtils(key, subkey, name, default)`：只读打开子键读取值，键或值不存在返回默认值。
- `ViewUtils(key, subkey, name, value, kind)`：可写打开子键写值；键不存在先 `CreateSubKey`；失败抛"写入注册表值时发生错误"。
- `ChangeUtils(key, subkey, name)`：删除指定值；失败抛"删除注册表值时发生错误"。
- `RegisterUtils`：子键是否存在。
- `CalcUtils`：确保子键存在（创建），恒 true。
- `PatchUtils`：子键存在则整体删除。

## 恶意行为 / 敏感操作
- **杀进程**：随机名进程、`jfglzsn`、`zmserv`、全部 `explorer.exe`。
- **删服务**：`net stop zmserv` + `sc delete zmserv`（服务名 `zmserv`）。
- **删注册表**：Run 自启动（prozs/jfglzsn，含 WOW6432Node）、`HKCU\Software\jfglzs`、密码标记 `n/dtmm/czmm`、20+ 个 IFEO 劫持键、Chrome/Edge/Firefox/IE 策略键。
- **删文件**：`C:\windows\jf\*.dat` + 目录；安装目录下 `jfglzsn.exe/zmserv.exe/przs.exe/yl.wav/huifu.wav/huifu.reg/yl.reg`。
- **系统级操作**：恢复 hosts（hosts-bak 覆盖 hosts）、`bcdedit` 开启启动菜单、重启资源管理器。
- **隐蔽技巧**：恢复数据以 `.wav` 扩展名伪装存放（`huifu.wav`），卸载时改名 `.reg` 静默导入。

## IOC / 硬编码
- 服务：`zmserv`
- 进程：`jfglzsn`、`zmserv`、随机 5 字符名（字符集 'i'~'r'）、`explorer`
- 文件：`C:\windows\jf\jfglzsn.dat`、`C:\windows\jf\przs.dat`、`C:\windows\jf\zmserv.dat`、`C:\windows\jf\`；安装目录下 `jfglzsn.exe`、`zmserv.exe`、`przs.exe`、`yl.wav`、`huifu.wav`、`SpecialSet.exe`；`%System%\drivers\etc\hosts` / `hosts-bak`
- 注册表：
  - `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\prozs`、`\jfglzsn`（及 WOW6432Node）
  - `HKCU\Software\jfglzs`（含 `dzjs` 值）
  - `HKCU\Software\n`（卸载密码哈希）、`dtmm`、`czmm`
  - IFEO 劫持：`HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\{taskkill,ntsd,sidebar,sethc,tasklist,route,Magnify,winmine,Chess,FreeCell,Hearts,Minesweeper,PurblePlace,Mahjong,SpiderSolitaire,Solitaire,bckgzm,chkrzm,shvlzm}.exe`
  - 策略：DisableCMD、DisableRegistryTools、NoRun、NoFolderOptions、DisableLockWorkstation、usbstor Start、IE Zones\3 1803/2200、Edge/Chrome/Firefox 下载与开发者工具策略
- 命令：`net stop zmserv`、`sc delete zmserv`、`regedit.exe /s <dir>\huifu.reg`、`cmd /c bcdedit /set {bootmgr} displaybootmenu yes`
- 盐值：密码哈希盐 `"bfdshgs"`
