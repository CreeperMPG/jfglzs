# jfglzs13.1

机房管理助手 13.1 版本源代码 C# 反编译版本，不能直接运行，使用 DeepSeek V4 Flash 进行反混淆。https://www.jfglzs.com/

共消耗 **104,487,092 Tokens**（¥19.22）

以下内容为 AI 生成：

> 样本：`~\source\repos\VIRUS\jfglzs13\`（脱壳后 `*_Slayed.exe`）
> 分析产物：`decompiled\` 目录（120 份文件级分析 `.cs.md` + 6 份行为分析 + 反混淆还原代码）
> 分析方式：静态分析（dnSpy 反编译 + 函数级反混淆 + 控制流还原），未运行样本

---

## 一、信息概览

| 项 | 值 |
|----|----|
| **类型** | VB.NET WinForms 恶意软件全家桶（"机房管理助手"） |
| **伪装** | 自称"机房管理小助手"，官网 www.jfglzs.com，有 QQ/客服联系方式 |
| **目标环境** | 学校机房 / 网吧学生机 |
| **技术栈** | .NET Framework + VB.NET，ConfuserEx 混淆 + .NET Reactor 加壳 |
| **组成** | 6 个可执行组件 + 配置/音频/自解压文件 |
| **核心目的** | 批量管控学生机（禁 USB/禁游戏/锁屏/强制关机），防止被清除 |

### 组件清单

| 组件 | 角色 | 界面 | 一句话行为 |
|------|------|------|-----------|
| **set.exe** | 管理端设置 | ✅「设 置」窗体 | 密码门进入，保存即批量施加限制策略 |
| **przs.exe** | 主控驻留 | ✅托盘+编号悬浮窗 | 隐藏驻留、全盘文件监控、自复制持久化 |
| **jfglzsn.exe** | 核心调度 | ✅两级锁屏窗 | 14 定时器执行全部策略 + 反杀软 + 自愈 |
| **zmserv.exe** | 看门狗服务 | ❌ Windows 服务 | 监控主程序，被杀→强制关机 |
| **about.exe** | 退出解锁 | ✅退出密码窗 | 输密码→杀进程/服务、恢复策略 |
| **uninstall.exe** | 受控卸载 | ✅全屏强制窗 | 输密码→20 步完整卸载 |

---

## 二、生命周期与攻击链

```
安装阶段           驻留阶段              对抗阶段
set(密码门)──▶ 策略批量写入      przs 文件监控自愈     杀 jfglzsn ──▶ zmserv 强制关机
     │                │              jfglzsn 14定时器     删 zmserv ──▶ jfglzsn 重建
     └──▶ jfglzsn 隐藏运行          hosts/注册表自愈       杀软 ──▶ 进程黑名单击杀
          │                            │                    用户 ──▶ 锁屏+禁USB+假浏览器
          └──▶ sc create zmserv        │
                    │                   ▼
                    └──▶ 开机自启：服务 + Run键(prozs) + 随机名进程
```

### 1. 安装
- `set` 的 load 密码门 → 主设置界面 → 管理员勾选策略 → **保存即批量施加**：
  禁用 USB/杀软/CMD/注册表编辑器、篡改 hosts、替换浏览器快捷方式为假浏览器、关闭防火墙与安全模式、禁用 IPv6、杀 explorer 重启外壳
- `sc create zmserv` 安装看门狗服务；`C:\windows\jf\` 为家族目录

### 2. 驻留与持久化（三重保险）
| 机制 | 位置 | 说明 |
|------|------|------|
| 服务 | zmserv（Service1，auto） | 监控 + 强制关机 |
| 注册表 Run 键 | `HKLM\...\Run`（prozs） | przs 开机自启 |
| 自复制 | `C:\Program Files (x86)\<随机名>\` | przs 副本 |
| 随机进程名 | 日期种子（月×日）生成 4~5 字符 | 每日更换，躲藏+识别 |

### 3. 恶意行为（14 定时器调度，jfglzsn）
| 行为 | 机制 |
|------|------|
| 禁用 U 盘 | usbstor Start=4 |
| 关闭防火墙 | netsh advfirewall |
| 改 hosts | 从服务器下载黑名单 `http://www.zmyxl2008.com/hmd_net.txt`，备份 hosts2 自愈 |
| 电子教室守护 | 进程缺失→从 .exe2 恢复并拉起；连续5次未找到→强制关机 |
| 杀软对抗 | 360 进程黑名单（360safe/360taskmgr 等）+ 极域服务（tdfilefilter/tdnetfilter）监控 |
| 锁屏 | Form2 一级 / Form4 二级密码锁，ClipCursor 锁鼠标 + Beep |
| 强制关机 | RtlAdjustPrivilege(19) + NtShutdownSystem(2) + shutdown /s /f /t 0 |
| 文件监控 | przs FileSystemWatcher 删"违规"文件（杀软安装包等） |
| 假浏览器 | 替换桌面快捷方式→受控浏览器（流量劫持链） |
| 端口探测 | TcpClient 探测目标可达性 |

### 4. 退出/卸载（受控通道）
- **about（退出）**：密码验证 → 杀监控进程/服务 → 恢复 USB/浏览器/CMD → 重启 explorer
- **uninstall（卸载）**：全屏强制窗 + 密码 → 20 步卸载（杀进程→停删服务→删注册表→恢复策略→删文件→恢复壁纸）
- **关键时序**：必须先 `net stop zmserv` 再杀 jfglzsn，否则触发强制关机

---

## 三、关键 IOC（入侵指标）

### 文件/路径
- `C:\windows\jf\`（家族安装目录）
- `C:\Program Files (x86)\<随机名>\`（przs 副本）
- `hosts` 备份：`C:\Windows\System32\drivers\etc\hosts2`
- 进程副本：`<随机名>.exe2`
- 样本目录：`jfglzs13`（about/jfglzsn/przs/set/uninstall/zmserv/regini_）

### 注册表
- `HKCU\Software\jfglzs`（zhuye/dzjs/usb_jianche/bianhao/n2…）
- `HKCU\Software\n`（主密码 SHA256）
- `HKLM\...\Run`（prozs）

### 服务/进程
- 服务 `Service1`（zmserv，binPath 指向 C:\windows\jf）
- 进程：jfglzsn、随机名进程（日期种子）、"f~o 字符集"命名进程
- 对抗目标：360safe/360taskmgr/360netman/360zippp、tdfilefilter/tdnetfilter

### 密码/算法
- SHA256(明文 + `"bfdshgs"`) → 十六进制小写
- 随机名种子：`月 × 日` 取模（97+…/105+… 字符算法）

### 网络
- 服务器：`HKCU\Software\jfglzs\zhuye`
- 黑名单 URL：`http://www.zmyxl2008.com/hmd_net.txt`
- 官网：www.jfglzs.com

### 其他文件
- regqx.ini / regqxoff.ini（注册表策略开关配置）
- hmd_local.txt、huifu.wav、yl.wav、wallpaper/、zy/
- regini_.exe（微软官方工具，用于修改注册表 ACL）

---

## 四、反混淆技术总结

| 混淆手段 | 还原方法 |
|---------|---------|
| ConfuserEx 语义假名（*Product/*Visitor/*Config/*Mock/*Reg） | 逐方法读代码还原语义，重命名为真实名 |
| 随机 GUID 命名空间（gypZRX1vuWc3aeye0R） | 按类职责分析（通信框架） |
| 控制流扁平化（IL_xx 标签 + num2 状态机 + @switch） | 基本块重排 → if/else/switch/循环/try-catch（2450+ 行还原） |
| VB On Error 残留（endfilter/CreateProjectError） | 还原为普通 try-catch |
| .NET Reactor 加壳 | 脱壳（_Slayed）后 dnSpy 反编译 |

---

## 五、应急处置建议（基于静态分析）

1. **第一优先级**：`net stop zmserv` + `sc delete zmserv`（否则杀主程序会触发强制关机）
2. 结束进程：jfglzsn、przs、about、set、uninstall、随机名进程
3. 删除 Run 键 prozs、`HKCU\Software\jfglzs`、`HKCU\Software\n`
4. 恢复 hosts（删 hosts2 备份）、恢复浏览器快捷方式、启用 USB/防火墙/安全模式
5. 删除 `C:\windows\jf\`、`Program Files (x86)\<随机名>\`
6. 全程断网操作（防止从 zmyxl2008.com 服务器重新下发）

> ⚠️ 以上基于静态分析推断，未做动态验证；实际处置请先在隔离环境确认。

---

## 六、分析产物索引

| 产物 | 位置 |
|------|------|
| 反混淆还原代码 + 解决方案 | `decompiled\jfglzs.sln` |
| 文件级分析（120 份） | `decompiled\<项目>\*.cs.md` |
| 行为级分析（6 份） | `decompiled\<项目>\行为分析.md` |
| 控制流还原规范 | `decompiled\_CTRLFLOW_GUIDE.md` |
| 分析规范 | `decompiled\_ANALYSIS_GUIDE.md` |
