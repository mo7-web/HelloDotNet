namespace MyDemo.Cli.MyMvvm;

using System;

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
    public void Increment() // 会在末尾自动拼上 Command，变成 IncrementCommand
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


/*

IRelayCommand：服务于同步操作（对应返回值为 void 的普通方法）。
IAsyncRelayCommand：服务于异步操作（对应返回值为 Task 的异步方法）。
会自动生成，不用管

 */
