# Form3.cs — "提醒"完成窗体：告知重启后生效并校验设置是否成功

## 文件作用
保存设置流程最后一步显示的提示窗体（`Reader.ReflectProperty.Form3.Show()`）。告诉老师"请在硬盘保护系统中创建进度或者保存系统"、"软件在重启电脑后自动运行"。Load 时校验注册表自启动项是否写入成功，若失败则提示需要管理员权限重跑。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| Form3 | 完成提醒窗 + 权限校验 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| ReadProperty() | BuildUI_构建界面 | 创建 Label1/Label2/Button2("确  定")，TopMost 置顶 |
| ResolveProperty(sender,e) | Button2_Click_退出 | `ProjectData.EndApp()` |
| CollectProperty(sender,e) | Form_Load_校验自启动是否成功 | 读 HKLM RUN 的 prozs 值（32/64 位）判断 |
| invocationMock/queueMock/definitionMock | Button2/Label2/Label1 | 控件后备字段 |

## 关键方法与流程
### Form_Load_校验自启动（CollectProperty）
1. 读 `HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN\prozs`，为空再读 `HKLM\SOFTWARE\WOW6432NODE\...\RUN\prozs`
2. 仍为空 → Label2 变红："因权限不够设置不成功，请打开机房管理助手的安装文件夹（C盘j开头的文件夹），右击文件set.exe，选择 以管理员身份运行，再次设置。"

## 恶意行为 / 敏感操作
- 无直接恶意操作；仅提示 + 校验持久化键（RUN\prozs）是否写入成功

## IOC / 硬编码
- 注册表：`HKLM\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN\prozs`、`HKLM\SOFTWARE\WOW6432NODE\MICROSOFT\WINDOWS\CURRENTVERSION\RUN\prozs`
- 安装目录提示："C盘j开头的文件夹"（jfglzsn 系列安装包）
