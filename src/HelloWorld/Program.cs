namespace HelloWorld;

using System;
using System.Threading.Tasks;
using MyDemo.Advanced;
using MyDemo.Basics;
using MyDemo.Collections;
using MyDemo.ControlFlow;
using MyDemo.FunctionsAndNullability;
using MyDemo.Modeling;
using MyDemo.Threading;

// 入口类显式标记为 internal static，杜绝任何外部 new 实例化
internal static class Program
{
    public static async Task Main(string[] args)
    {
        // 显式类型声明，杜绝 blind var
        DateTime now = DateTime.Now;
        Console.WriteLine($"The current time is {now:yyyy-MM-dd HH:mm:ss}");

        string name = "墨七";
        Console.WriteLine($"Hello, {name}!");

        string name2 = "墨七2";
        name2 = $"{name2}3";
        // 统一插值字符串直写缓冲区，消灭 '+' 字符串拼接的中间临时垃圾
        Console.WriteLine($"Hello, {name2}! ");

        // 第一阶段 1.1 ~ 1.3 基础
        // BasicsDemo.Run();

        // 第一阶段 1.4 ~ 1.5 控制流与异常
        // ControlFlowDemo.Run();

        // 第二阶段 2.1 ~ 2.3 函数与空安全
        // FunctionsDemo.Run();

        // 第三阶段 3.1 ~ 3.3 集合容器与切片
        // CollectionsDemo.Run();

        // 第四阶段 4.1 ~ 4.4 现代面向对象与数据建模
        // ModelingDemo.Run();

        // 第五阶段 5.1 ~ 5.2 显式分支与异步取消机制
        // await AdvancedConcurrencyDemo.RunAsync();

        // 第六阶段 多线程底层本质、Channel 传值与硬件级同步
        await ThreadConcurrencyDemo.RunAsync();

        Console.WriteLine(">>> 全流程执行完毕 <<<");

        await Task.Delay(5000);
    }
}
