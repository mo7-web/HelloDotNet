# Mvvm 入门

学习 `CommunityToolkit.Mvvm` 不需要背诵厚重的手册，本质上只有**两个最核心的心智模型**：

1. **数据响应化**：你在私有字段打上 `[ObservableProperty]`，编译期源生成器就自动帮你生成公开属性和广播事件（等价于 TS 的 `ref` / `reactive` 或 MobX）。
2. **动作封装化**：你在方法上打上 `[RelayCommand]`，编译期源生成器就自动包装出一个可供 UI 绑定的命令对象（等价于前端带防重状态的 `onClick` 处理函数）。

只要掌握这套**“打标即展开（Source Generator）”**机制，你就已经掌握了该库 80% 的日常开发。

---

## 心智模型桥接（TS / Go 视角）

```
[你写的源码]                  [源生成器编译时做的事]              [前端 TS / 后端 Go 等价物]
-----------------------------------------------------------------------------------------
[ObservableProperty]     →   自动生成带通知的 Getter/Setter  →   Vue 的 ref(val) 或 MobX @observable
private int _count;          public int Count { get; set; }

[RelayCommand]           →   自动生成包装类 ICommand         →   带有 loading 锁的防重触发函数
public Task RunAsync()       public IRelayCommand RunCommand     (如 runAsync + isRunning 标志位)
```

---

## 第一步：在项目中引入依赖

在你的项目根目录执行 CLI 命令，或者直接在 `.csproj` 中加入：

```bash
dotnet add package CommunityToolkit.Mvvm
```

---

## 第二步：掌握两个最基础的注解规则

### 规则 1：类必须声明为 `partial`

因为 Roslyn 源生成器是在编译期把你写的类进行代码“拼接”扩充，所以当前类必须标记为分部类：

```csharp
// 正确：允许源生成器拼接
public partial class UserViewModel : ObservableObject { }

// 错误：编译直接报错！源生成器无法向密封类注入代码
public class UserViewModel : ObservableObject { }
```

### 规则 2：命名约定触发源生成器

- **属性**：声明 `_fooBar`（私有下划线小驼峰），生成器自动产出 `FooBar`（公开大驼峰）。
- **命令**：声明 `SaveAsync` 或 `Save`，生成器自动产出 `SaveCommand`（自动剥离 Async 并追加 Command 后缀）。

---

## 第三步：端到端入门实战（计数器 Demo）

我们做一个具备“加 1”和“异步重置”功能的完整页面。

### 1. 编写业务逻辑：`CounterViewModel.cs`

严格按团队规范编写（平铺直叙、显示类型、传统构造函数、`this.` 标明归属）：

```csharp
namespace App.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

// 1. 继承 ObservableObject
// 2. 必须声明 partial
public partial class CounterViewModel : ObservableObject
{
    // 自动生成: public int Count { get; set; }
    [ObservableProperty]
    private int _count;

    // 自动生成: public string StatusMessage { get; set; }
    [ObservableProperty]
    private string _statusMessage;

    // 显式传统构造函数，拒绝主构造函数
    public CounterViewModel()
    {
        this._count = 0;
        this._statusMessage = "点击按钮开始计数";
    }

    // 自动生成: public IRelayCommand IncrementCommand { get; }
    [RelayCommand]
    public void Increment()
    {
        this.Count = this.Count + 1;
        this.StatusMessage = string.Concat("当前计数值已更新为: ", this.Count.ToString());
    }

    // 自动生成: public IAsyncRelayCommand ResetAsyncCommand { get; }
    // 源生成器自带 ResetAsyncCommand.IsRunning 状态，可直接在 UI 绑定防重复点击
    [RelayCommand]
    public async Task ResetAsync()
    {
        this.StatusMessage = "正在重置中，请稍候...";

        // 模拟 I/O 等待
        await Task.Delay(1000);

        this.Count = 0;
        this.StatusMessage = "已成功重置为 0！";
    }
}
```

---

### 2. 编写界面绑定：`CounterView.axaml`

在 Avalonia 12 中使用时，**为了 Native AOT 绝对安全，必须配置强类型编译期绑定**：

```xml
<UserControl
  xmlns="https://github.com/avaloniaui"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:vm="using:App.ViewModels"
  x:Class="App.Views.CounterView"
  x:DataType="vm:CounterViewModel"
  x:CompileBindings="True">

    <StackPanel Spacing="12" Margin="30" HorizontalAlignment="Center" VerticalAlignment="Center">
        <!-- 绑定自动生成的 Count 属性 -->
        <TextBlock
      Text="{Binding Count}"
      FontSize="36"
      FontWeight="Bold"
      HorizontalAlignment="Center" />

        <!-- 绑定自动生成的 StatusMessage 属性 -->
        <TextBlock Text="{Binding StatusMessage}" Foreground="Gray" HorizontalAlignment="Center" />

        <!-- 绑定自动生成的 IncrementCommand 命令 -->
        <Button
      Content="数字 + 1"
      Command="{Binding IncrementCommand}"
      Width="160"
      HorizontalContentAlignment="Center" />

        <!-- 绑定自动生成的 ResetAsyncCommand 命令 -->
        <!-- 注意：ResetAsyncCommand 自带 IsRunning 属性，直接用 ! 运算符控制按钮可用状态 -->
        <Button
      Content="重置数据 (异步)"
      Command="{Binding ResetAsyncCommand}"
      IsEnabled="{Binding !ResetAsyncCommand.IsRunning}"
      Width="160"
      HorizontalContentAlignment="Center" />
    </StackPanel>
</UserControl>
```

---

### 3. 将 View 与 ViewModel 组装（`CounterView.axaml.cs`）

用最平铺的 Code-Behind 将两者直接绑定，归属一目了然：

```csharp
namespace App.Views;

using App.ViewModels;
using Avalonia.Controls;

public partial class CounterView : UserControl
{
    private readonly CounterViewModel _viewModel;

    public CounterView()
    {
        this.InitializeComponent();

        // 显式实例化 ViewModel 并挂载到界面的 DataContext 上
        this._viewModel = new CounterViewModel();
        this.DataContext = this._viewModel;
    }
}
```

---

## AOT 编译避坑三铁律

1. **绝对不要丢掉 `x:CompileBindings="True"`**：
   如果不写，Avalonia 会退回到传统运行时反射寻找属性。在 Native AOT 编译裁剪时，属性会被当成死代码剪除，导致运行时点击无反应或直接闪退。
2. **永远使用生成的公开大驼峰属性**：
   在 ViewModel 内部改值时，必须写 `this.Count = 10;`，**绝对不能**在方法里写 `this._count = 10;`。写下划线私有字段会绕过生成器注入的 `PropertyChanged` 广播逻辑，UI 将收不到刷新信号。
3. **方法命令后缀约定**：
   方法名如果带 `Async`，比如 `SaveAsync()`，生成器产出的命令默认叫 `SaveCommand`（它会自动帮你去掉 Async 后缀）；如果不带，比如 `Save()`，产出的命令也叫 `SaveCommand`。绑定时敲错名字，编译期会直接报错报警。
