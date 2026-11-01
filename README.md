# jfglzs13.4

机房管理助手 13.4 版本源代码 C# 反编译版本，不能直接运行。

样本来源：官方安装包 `setup for win10.exe`（Advanced Installer 11.0 build 55831 引导程序）。
用 `Setup.exe /extract` 取出 `学生机房管理助手.msi` 与 `disk1.cab`，再用 `expand` 解出 6 个 .NET 可执行组件。

分析方式：**纯静态分析**。样本未运行、未安装。流程为
`NETReactorSlayer 解壳 → de4dot --dont-rename → dnSpy 导出 C# 工程 → 控制流还原 / 符号重命名`。

---

## 一、信息概览

| 项 | 值 |
|----|----|
| **类型** | VB.NET WinForms 恶意软件全家桶（"机房管理助手"） |
| **伪装** | 自称"学生机房管理助手"，官网 www.jfglzs.com，有 QQ/客服联系方式 |
| **目标环境** | 学校机房 |
| **技术栈** | .NET Framework 4.5 + VB.NET，.NET Reactor 加壳 + 控制流扁平化 |
| **组成** | 6 个可执行组件 + 11 个"浏览器"外壳 + 6 张壁纸 + 配置/音频 |
| **核心目的** | 批量管控学生机（禁 USB / 禁游戏 / 禁下载 / 锁屏 / 强制关机），并防止被清除 |

---

## 二、组件清单

| 组件 | 角色 | 界面 | 一句话行为 |
|------|------|------|-----------|
| **set.exe** | 管理端设置 | ✅ 「设置」窗体 + 密码闸门 | 密码进入后批量施加限制策略，装服务、写自启动 |
| **przs.exe** | 主控驻留 | ✅ 托盘 + 编号悬浮窗 | 自我迁居到 `Program Files (x86)`、双锁屏、文件监控 |
| **jfglzsn.exe** | 核心调度 | ✅ 两级锁屏窗 + 警告窗 | 14 个定时器 + WMI 进程监控，执行全部策略与反制 |
| **zmserv.exe** | 看门狗服务 | ❌ Windows 服务 | 监控主程序，被杀 → 强制关机 |
| **about.exe** | 退出解锁 | ✅ 退出密码窗 | 输密码 → 杀进程/服务、恢复策略 |
| **uninstall.exe** | 受控卸载 | ✅ 全屏强制窗 | 输密码 → 23 步完整卸载 |

---

## 三、生命周期与攻击链

```
安装阶段                 驻留阶段                      对抗阶段
set(密码门) ──▶ 批量写策略   przs 自我迁居+双锁屏          杀 jfglzsn ──▶ zmserv 强制关机
     │                    jfglzsn 14 定时器              删 zmserv ──▶ jfglzsn 重建
     ├──▶ jfglzsn 隐藏运行  WMI 进程创建回调              杀软/违规进程 ──▶ KILL + 删文件
     │         │              │                            用户 ──▶ 锁屏 + 禁 USB + 假浏览器
     │         └──▶ sc create zmserv
     │                        │
     └──▶ 自启动：服务 + Run 键(prozs / jfglzsn) + 随机名进程
```

### 1. 安装
- `set.exe` 的 `load` 窗体做密码闸门 → 主设置界面 → 管理员勾选策略 → **保存即批量施加**：
  禁用 USB/CMD/注册表编辑器、改 hosts、替换浏览器快捷方式为"假浏览器"、关防火墙、杀 explorer 重启外壳。
- `sc create zmserv` 安装看门狗服务；`C:\windows\jf\` 为家族数据目录。

### 2. 驻留与持久化（三重保险）

| 机制 | 位置 | 说明 |
|------|------|------|
| 服务 | `zmserv` | 监控主程序，被杀即强制关机 |
| 注册表 Run 键 | `HKLM\...\Run`（`prozs`、`jfglzsn`，含 WOW6432Node） | 开机自启 |
| 自我迁居 | `C:\Program Files (x86)\<随机4字>\<随机5字>.exe` | przs 复制自身并加 Hidden+System，启动副本后自杀 |
| 随机进程名 | 以「月×日」为种子生成 4~5 字符名（`i`~`r` 区间） | 每天固定、可提前算出 |

### 3. 恶意行为（jfglzsn 的 14 个定时器 + WMI 回调）

| 行为 | 机制 |
|------|------|
| 禁用 U 盘 | `usbstor\Start=4`（含 ControlSet001~003），改回 3 就强制改回 |
| 关闭防火墙 | `netsh advfirewall` + 改 `EnableFirewall` |
| 改 hosts（封网） | 从云端拉黑名单 `http://<zhuye>/hmd_net.txt`，写 hosts、加黑洞路由、flushdns、备份 `hosts2` |
| 电子教室守护 | 进程缺失 → 从 `.dat` 恢复并拉起；连续 5 次缺失 → **强制关机** |
| 新进程拦截 | WMI `__InstanceCreationEvent` 回调，四张黑名单匹配 → Kill + 删文件 + 弹窗 |
| 杀软对抗 | 360 / 火绒 等进程黑名单 + 极域服务（`tdfilefilter` / `tdnetfilter`）监控 |
| 锁屏 | Form2 / Form4 两级密码锁，`ClipCursor` 锁鼠标 + `Beep` |
| 强制关机 | `RtlAdjustPrivilege(19)` + `NtShutdownSystem(2)` + `shutdown /s /f /t 0` |
| 下载拦截 | Chrome/Edge/Firefox 组策略 + IE Zones + `NoBrowserSaveAs` |
| 假浏览器 | `zy\` 下 11 个浏览器外壳（360/2345/QQ/Chrome/Edge/Firefox…）替换桌面与任务栏快捷方式 |
| 网卡监控 | 有线 IP 变化 / 断网检测 + 分级警告 |
| 端口探测 | `TcpClient` 探测目标可达性 |

### 4. 退出 / 卸载（受控通道）
- **about（退出）**：密码验证 → 杀监控进程/服务、恢复 USB/CMD/浏览器策略、重启 explorer。
  ⚠️ 密码摘要存 `HKCU\Software` 值 `n`；**该值不存在时点「退出」不会退出，而是直接启动 `set.exe`**。
- **uninstall（卸载）**：全屏强制窗 + 密码 → 23 步完整卸载（杀进程 → 停删服务 → 删注册表 →
  **清 21 条 IFEO 劫持** → 恢复 20+ 条策略 → 用 `hosts-bak` 还原 hosts → 删文件 → 恢复壁纸）。
- **关键时序**：必须先 `net stop zmserv` 再杀 `jfglzsn`，否则触发强制关机。

---

## 四、关键 IOC（入侵指标）

### 文件 / 路径
- `C:\windows\jf\`（家族数据目录：`jfglzsn.dat` / `przs.dat` / `zmserv.dat`）
- `C:\Program Files (x86)\<随机4字>\<随机5字>.exe`（przs 迁居副本，Hidden+System）
- `%SystemRoot%\drivers\etc\hosts`，备份 `hosts2` / `hosts-bak`
- 安装目录：`set.exe`、`jfglzsn.exe`、`przs.exe`、`zmserv.exe`、`about.exe`、`uninstall.exe`、
  `regini.exe`、`regqx.ini`、`regqxoff.ini`、`yl.wav`、`huifu.wav`、`hmd_local.txt`、`hosts`、`zy\`

### 注册表
- `HKCU\Software` → `n`（退出密码 `SHA256(明文 + "bfdshgs")`，十六进制小写）
- `HKCU\Software\jfglzs` → `n2`（锁屏备用口令）、`dzjs`、`usb_jianche`、`shadu_jianche`、`cmd_jianche`、
  `games_jianche`、`bianhao`、`xiazai`、`gaowei`、`win_jian`、`zhuye`、`zcb`
- `HKLM\...\Run` → `prozs`、`jfglzsn`（含 `WOW6432Node`）
- `HKLM\SYSTEM\{CurrentControlSet,ControlSet001~003}\Services\usbstor` → `Start`（3/4）
- `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\` → **21 条 IFEO 劫持**
  （`taskkill.exe` / `ntsd.exe` / `sethc.exe` / `Magnify.exe` / 扫雷纸牌麻将等游戏）
- 浏览器策略：`HKLM\SOFTWARE\Policies\Google\Chrome`、`…\Microsoft\Edge`、`…\Mozilla\Firefox`；
  `HKCU\…\Internet Settings\Zones\3`（`1803`、`2200`）；`HKCU\Software\Policies\Microsoft\Internet Explorer\Restrictions`

### 服务 / 进程
- 服务 `zmserv`
- 进程：`jfglzsn`、`przs`、随机名进程（月×日种子，`i`~`r` 区间 4~5 字符）
- 对抗目标：`studentMain`、`ClassMangerApp`、`REDAgent`（极域/电子教室）、`360sd`、`360Tray`、`HipsTray`、`QQPCTray`

### 网络
- 域名：`www.jfglzs.com`、`www.zmyxl2008.com`
- 黑名单：`http://<zhuye>/hmd_net.txt`（`zhuye` 从注册表读，可被服务端改）
- 官网/客服：`https://www.jfglzs.com/c16.html`、`http://www.jfglzs.com/software/bangzhu.htm`

### 密码 / 算法
- 退出与锁屏密码：`SHA256(明文 + "bfdshgs")` → 十六进制小写（**单轮、固定盐，可离线爆破**）
- 随机名种子：`月 × 日`
- 壳：自实现 MD5 + AES-256-CBC 字符串池解密 / IL 补丁（ConfuserEx / Reactor 特征常量
  `1394801171`、`3457341307`、`4040842541`）

---

## 五、反混淆技术总结

| 混淆手段 | 还原方法 |
|---------|---------|
| .NET Reactor 加壳（方法体加密） | NETReactorSlayer 6.4.0 解壳（本例 6 个组件各解出 153~366 个方法） |
| 控制流扁平化（`IL_xxxx` 标签 + `num2` 状态机 + `switch` 分发） | 按基本块重排还原成 `if/else` / `while` / `switch`，**goto 全部清零** |
| VB `On Error Resume Next` 残留（`ClearProjectError` + `endfilter`） | 还原为逐条 `try/catch`（保持"每条语句独立忽略错误"的语义） |
| 混淆器随机化命名（`*Composer` / `*Mapper` / `*Strategy` / `object_0`） | 逐方法读代码按语义重命名，并写进各文件的命名映射表 |
| 字符串常量加密（AES + 自定义流密码） | 静态识别密钥/IV 与流密码常量，在文档中记录，不还原成明文 |
| 垃圾代码（同一变量反复赋值、诱饵方法返回 `{1,2}`） | 识别后删除 / 注释说明，保留真实语义 |

---

## 六、应急处置建议（基于静态分析）

1. **第一优先级**：`net stop zmserv` + `sc delete zmserv`（否则杀主程序会触发强制关机）
2. 结束进程：`jfglzsn`、`przs`、随机名进程（`i`~`r` 区间 4~5 字符）、`about`、`set`、`uninstall`
3. 删除 Run 键 `prozs` / `jfglzsn`（含 WOW6432Node）
4. **清 IFEO**：删除 `Image File Execution Options` 下 21 个劫持项
5. 恢复策略：`usbstor\Start=3`、`DisableCMD=0`、`DisableRegistryTools=0`、防火墙、安全模式、
   浏览器下载/开发者工具策略、IPv6
6. 还原 hosts（用 `hosts-bak` 或删除劫持行），恢复浏览器快捷方式
7. 删除 `C:\windows\jf\`、`C:\Program Files (x86)\<随机4字>\`
8. 全程断网操作（防止从云端重新下发策略/黑名单）

> ⚠️ 以上基于静态分析推断，未做动态验证；实际处置请先在隔离环境确认。

---

## 七、分析产物索引

| 产物 | 位置 |
|------|------|
| 反混淆源代码 + 解决方案 | `jfglzs.slnx`（6 个工程） |
| 文件级分析（102 份） | `<项目>\**\*.cs.md`（与 `.cs` 一一对应） |
| 行为级分析（6 份） | `<项目>\行为分析.md` |
| 原始样本 | `samples\*.exe` |
| 解壳产物（NETReactorSlayer） | `samples\*_Slayed.exe` |
| de4dot 收尾产物 | `unpacked\*.exe` |
| 反编译流程与判定表 | `AGENTS.md` |

### 各工程规模

| 工程 | .cs | .cs.md | 代码规模 |
|------|-----|--------|---------|
| about | 17 | 17 | 壳运行时 + 退出窗体 + 状态轮询 |
| jfglzsn | 18 | 18 | **核心调度**（14 定时器 + WMI 回调整套策略） |
| przs | 19 | 19 | 驻留 + 自我迁居 + 双锁屏 + 文件监控 |
| set | 17 | 17 | 策略设置 + 密码闸门 |
| uninstall | 16 | 16 | 23 步卸载 + IFEO 清理 |
| zmserv | 15 | 15 | 看门狗服务 |
