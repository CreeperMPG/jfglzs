# Form4.cs — 二级密码锁定窗（高级锁屏，带 DisableCMD 恢复逻辑）

## 文件作用
Form4 与 Form2 几乎相同的**全屏锁定密码窗**（同样无边框、置顶、最大化、ClipCursor 锁鼠标 + Beep），由 `Timer4`（StartTemplate）弹出。区别在于：1) 密码可匹配两个注册表值——主密码 `HKCU\Software\n` 或二级密码 `HKCU\Software\jfglzs\n2`；2) 解锁清理流程 `FindConfig` 额外处理 `DisableCMD` 的开关恢复（临时放开 cmd 以执行 `net stop zmserv`，若之前是禁用状态则结束后恢复禁用）；3) `FormClosing` 行为受 `ModelVisitor._RegVisitor` 控制（false 时可关闭并释放鼠标，true 时禁止关闭）——该标志位可能在同伴程序/服务端控制下切换，使本窗体可被"老师端"关闭。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| `Form4` | 二级锁屏 + 密码校验 + 卸载清理（含 DisableCMD 恢复） |
| `Form4.RECT` | ClipCursor 矩形结构体 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| `StopTemplate` | 初始化控件与布局 | Designer 控件初始化 |
| `RevertTemplate`（Button1.Click） | 确定按钮_校验密码 | 双密码比对 |
| `UpdateConfig`（Timer_frm4.Tick） | 锁鼠标+蜂鸣 | 每秒 ClipCursor 到 (0,0)-(800,100)，第 5 次起 Beep |
| `PostConfig`（Label_pd.DoubleClick） | 双击标签空操作 | 空壳 |
| `FindConfig` | 卸载清理并退出 | 杀进程、停服务、恢复 DisableCMD、EndApp |
| `ConcatConfig`（FormClosing） | 按 _RegVisitor 决定可否关闭 | false→放行并释放鼠标；true→禁止 |
| `AddConfig`（Load） | 启用锁鼠标定时器 | Timer_frm4.Enabled = true |
| `CountConfig`（Button2.Click） | 重启电脑 | `shutdown /r /f /t 0` |

## 关键方法与流程

### 确定按钮_校验密码（原名 `RevertTemplate`）
- **流程**：1. 长度 <6 → 提示重输；2. `FillConfig(Trim(输入))` 与 `HKCU\Software\n` **或** `HKCU\Software\jfglzs\n2` 任一相等→禁用按钮、`FindConfig()`；3. 否则提示"密码不正确"。
- **关键调用/API**：`ModelVisitor.FillConfig`（SHA256(输入+"bfdshgs")）、`ModelVisitor.DeleteConfig`。

### 卸载清理并退出（原名 `FindConfig`）
- **职责**：同 Form2.PrepareTemplate，但增加 DisableCMD 恢复逻辑。
- **流程**：1. 反复击杀随机名进程（`_TestsVisitor`）；2. 枚举并击杀"名字全由 Asc 102~111 字符组成"的进程；3. 读 `DisableCMD`：==1 → 先写 0（放开 cmd）→ `net stop zmserv` → Sleep 3000 → 写回 1（恢复禁用）；!=1 → 直接 `net stop zmserv` + Sleep 3000；4. 击杀 zmserv 进程；5. `ClipCursor` 全屏；6. `EndApp()`。
- **关键调用/API**：`Process.Kill`、`Interaction.Shell("net stop zmserv")`、`ModelVisitor.AwakeConfig`（写 DisableCMD）、`ClipCursor`、`EndApp`。

### 按 _RegVisitor 决定可否关闭（原名 `ConcatConfig`）
- **流程**：`!_RegVisitor` → `e.Cancel=false`（允许关闭）并 `ClipCursor` 全屏（释放鼠标）；否则 `e.Cancel=true`。
- **关键调用/API**：`ModelVisitor._RegVisitor`（全局静态标志，默认 false；真值由外部/同伴设置）。

### 锁鼠标+蜂鸣（原名 `UpdateConfig`）/ 重启电脑（原名 `CountConfig`）
- 与 Form2 相同：每秒 ClipCursor 到 (0,0)-(800,100)，第 5 次起 `Beep(2000,500)`；按钮执行 `shutdown /r /f /t 0`。

## 恶意行为 / 敏感操作
- 全屏锁屏 + 鼠标锁定 + 禁止/条件关闭；强制重启；击杀随机名进程与服务；操作 DisableCMD 注册表策略。

## IOC / 硬编码
- **注册表**：`HKCU\Software\n`（主密码）；`HKCU\Software\jfglzs\n2`（二级密码）；`HKCU\Software\Policies\Microsoft\Windows\System\DisableCMD`（读/写 0/1）。
- **进程/服务**：`zmserv`；随机 5 位名进程。
- **命令**：`net stop zmserv`、`shutdown /r /f /t 0`。
