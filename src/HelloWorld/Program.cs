using HelloWorld.Basics;
using HelloWorld.ControlFlow;
using MyDemo.FunctionsAndNullability;

void HelloWorld()
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
    FunctionsDemo.Run();

    Console.WriteLine(">>> 全流程执行完毕 <<<");
}

HelloWorld();

