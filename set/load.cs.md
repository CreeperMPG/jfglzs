# load.cs — 启动"密码门"窗体：验证后才能进入设置界面

## 文件作用
程序启动后显示的第一个窗体（`Reader.ReflectProperty.load`，由 Descriptor.OnCreateMainForm 与 ExceptionMock.InvokeProperty 共同指向）。功能是**访问控制门**：若系统尚未设置密码（`HKCU\Software\n` 为空）则直接放行进入 Form1；否则要求输入密码，SHA256(输入+"bfdshgs") 与注册表哈希比对，正确才放行。同时负责生成一个"随机进程名"供后续杀进程使用。

## 类与职责
| 类/模块 | 真实职责 |
|---------|---------|
| load | 密码验证窗体 + 随机进程名生成器 |

## 反混淆命名映射
| 混淆名 | 反混淆名 | 说明 |
|--------|---------|------|
| LogoutProperty() | BuildUI_构建界面 | 手工创建控件（Label_pd/Label1/Label3/TextBox1/Button1） |
| UpdateProperty(sender,e) | Button1_Click_校验密码 | 输入≥6位，比对哈希，正确→显示 Form1 |
| CalculateProperty(sender,e) | Label_pd_DoubleClick_空实现 | 仅剩混淆样板，无实际逻辑 |
| ExcludeProperty(sender,e) | Form_Shown_检查是否已设密码 | 未设密码→跳过密码门直接进 Form1 |
| PrepareProperty(sender,e) | Form_Load_生成随机进程名 | 依日期生成 4~5 字符随机名，写 ExceptionMock._DecoratorMock/_CandidateMock |

### 字段
| 混淆字段 | 反混淆名 | 说明 |
|---------|---------|------|
| _InterpreterMock | Label_pd | 大字号标签（双击事件挂 CalculateProperty） |
| _SchemaMock | Label1 | 红色错误提示标签 |
| _ThreadMock | Label3 | "输入小助手密码：" |
| recordMock | TextBox1 | 密码输入框（PasswordChar='*'） |
| parameterMock | Button1 | "确 定"按钮 |

## 关键方法与流程

### Form_Load_生成随机进程名（PrepareProperty）
1. `num = 当前月 × 当前日`，取 num%7、num%9、num%5、num%3
2. 依 num 奇偶用 Chr(97+…)/Chr(98+…)/Chr(101+…)/Chr(99+…) 拼 4 字母串，先赋给 `ExceptionMock._DecoratorMock`
3. `VBMath.Randomize(num)`，取 `Rnd()*300000+1`，逐位 `num%10+105`（→'i'~'r'）反序拼成 5 字符名，**覆盖** `_DecoratorMock`，`_CandidateMock = 名+".exe"`
4. 该随机名在 Form1.PushProperty 中用于杀掉"上一个/同族"随机名进程（防多实例互杀）

### Form_Shown_是否放行（ExcludeProperty）
- 读 `HKCU\Software\n`，为空（从未设置密码）→ Hide() 自己并直接 `Reader.ReflectProperty.Form1.Show()`

### Button1_Click_校验密码（UpdateProperty）
1. 输入长度 <6 → "密码应该是6位以上的字母或数字，请重新输入！"，清空输入框
2. `left = InsertProperty(输入)`（即 SHA256(输入+"bfdshgs")）；`right = HKCU\Software\n`
3. 相等 → 清提示、禁用按钮、Hide()、`Reader.ReflectProperty.Form1.Show()` 进入设置
4. 不等 → "密码不正确，请重新输入！"

## 恶意行为 / 敏感操作
- 密码门（混淆哈希校验）：哈希盐 `bfdshgs` 硬编码；若密码丢失则无法进入设置（除非删除 HKCU\Software\n）
- 日期驱动的随机进程名生成——用于强杀同名进程，属自清理/防多开逻辑

## IOC / 硬编码
- 注册表：`HKCU\Software\n`（SHA256(密码+"bfdshgs") 哈希值）
- 密码规则：≥6 位；错误提示"密码不正确，请重新输入！"
