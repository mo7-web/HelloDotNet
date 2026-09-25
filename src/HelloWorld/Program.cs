namespace HelloWorld;

using MyDemo.Basics;
using MyDemo.ControlFlow;
using MyDemo.FunctionsAndNullability;
using MyDemo.Collections;
using MyDemo.Modeling;
using MyDemo.Advanced;
using MyDemo.Threading;
public class Program
{
    static async Task Main(string[] args)
    {
        var now = DateTime.Now;
        Console.WriteLine($"The current time is {now:yyyy-MM-dd HH:mm:ss}");
        string name = "墨七";
        Console.WriteLine($"Hello,{name}!");
        var name2 = "墨七2";
        name2 = name2 + "3";
        Console.WriteLine("Hello," + name2 + "! ");

        // 第一阶段 1.1 ~ 1.3 基础
        // BasicsDemo.Run();

        // 第一阶段 1.4 ~ 1.5
        // ControlFlowDemo.Run();

        // 第二阶段  2.1~2.3
        // FunctionsDemo.Run();

        // 第三阶段：数据容器与切片
        // CollectionsDemo.Run();

        // 第四阶段：现代面向对象与数据建模
        // ModelingDemo.Run();

        // 第五阶段：现代模式匹配、ValueTask 与协同并发取消 (异步执行)
        // await AdvancedConcurrencyDemo.RunAsync();

        // 第六阶段：多线程底层本质、Channel 传值与硬件级同步
        await ThreadConcurrencyDemo.RunAsync();

        Console.WriteLine(">>> 全流程执行完毕 <<<");

        await Task.Delay(5000);

    }
}
