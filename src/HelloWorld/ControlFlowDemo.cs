namespace MyDemo.ControlFlow;

using System;

public static class ControlFlowDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [1.4 控制流手感] ===");
        ControlFlowDemo.DemonstrateBranchingAndLoops();

        Console.WriteLine("\n=== [1.5 异常与过滤器手感] ===");
        ControlFlowDemo.DemonstrateExceptionFilters();
    }

    private static void DemonstrateBranchingAndLoops()
    {
        int score = 85;

        // 1. 消除嵌套三元运算符：调用平铺展开的显式纯函数
        string rank = ControlFlowDemo.CalculateRank(score);
        Console.WriteLine($"Score {score} Rank: {rank}");

        // 2. 现代 foreach 集合遍历
        // 显式声明 int num，拒绝 var 盲盒
        int[] numbers = [10, 20, 30, 40, 50];
        int sum = 0;
        foreach (int num in numbers)
        {
            Console.WriteLine($"foreach: {num}");

            if (num == 30)
            {
                continue;
            }

            if (num > 40)
            {
                break;
            }

            sum += num;
        }
        Console.WriteLine($"Sum calculated via foreach: {sum}");

        // 3. 经典 while 循环
        int step = 3;
        while (step > 0)
        {
            step--;
            Console.WriteLine($"while: {step}");
        }

        // 4. do-while 循环 (保证至少执行一次)
        int counter = 0;
        do
        {
            counter++;
            Console.WriteLine($"do-while: {counter}");
        } while (counter < 1);
    }

    // 结构彻底平铺：使用卫语句提前 return 消除缩进，单行断点完全透明
    private static string CalculateRank(int score)
    {
        if (score >= 90)
        {
            return "A";
        }

        if (score >= 80)
        {
            return "B";
        }

        return "C";
    }

    private static void DemonstrateExceptionFilters()
    {
        // 场景 1：C# 核心独有特性 —— catch when 过滤器
        // 底层机理：利用 OS 结构化异常处理（SEH）的两阶段遍历（Two-Pass Exception Handling）。
        // 只有 when 返回 true 才会展开（Unwind）调用栈；若为 false，异常继续向外寻找捕获者，当前栈帧完好保留。
        string[] testPayloads = ["valid", "timeout_error", "fatal_corrupted_payload"];

        // 显式声明 string payload
        foreach (string payload in testPayloads)
        {
            try
            {
                Console.WriteLine($"当前处理: {payload}");
                ControlFlowDemo.ProcessPayload(payload);
            }
            // 显式指定 StringComparison.Ordinal 杜绝区域性字符表开销
            catch (InvalidOperationException ex) when (ex.Message.Contains("timeout", StringComparison.Ordinal))
            {
                Console.WriteLine($"[可重试错误拦截] 检测到超时: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[非预期严重故障] {ex.GetType().Name}: {ex.Message}");
            }
        }

        // 场景 2：Native AOT 高性能标准模式：TryXxx（消除异常惩罚）
        // 机制：通过寄存器传递布尔成功状态与数值，完全不分配 Exception 堆对象，不生成调用栈回溯。
        string inputStr = "999";
        if (int.TryParse(inputStr, out int parsedVal))
        {
            Console.WriteLine($"[推荐的高性能模式] 显式转换成功，无异常抛出开销: {parsedVal}");
        }
        else
        {
            Console.WriteLine("[转换失败]");
        }
    }

    private static void ProcessPayload(string payload)
    {
        if (payload == "timeout_error")
        {
            throw new InvalidOperationException("Network timeout occurred during handshake.");
        }

        if (payload == "fatal_corrupted_payload")
        {
            throw new FormatException("Memory block contains unreadable bytes.");
        }
    }
}
