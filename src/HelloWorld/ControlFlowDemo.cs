namespace HelloWorld.ControlFlow;

public static class ControlFlowDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [1.4 控制流手感] ===");
        DemonstrateBranchingAndLoops();

        Console.WriteLine("\n=== [1.5 异常与过滤器手感] ===");
        DemonstrateExceptionFilters();
    }

    private static void DemonstrateBranchingAndLoops()
    {
        int score = 85;

        // 1. 条件分支与三元运算符 [Go 没有三元运算符，JS 完全一致]
        string rank = score >= 90 ? "A" : (score >= 80 ? "B" : "C");
        Console.WriteLine($"Score {score} Rank: {rank}");

        // 2. 现代 foreach 集合遍历
        // [Go: for _, item := range list] / [JS: for (const item of list)]
        // 在 Native AOT 下，编译器对原生数组的 foreach 会直接降级为无边界检查的高速指针/索引迭代（零分配）
        int[] numbers = [10, 20, 30, 40, 50]; // C# 12+ 集合表达式
        int sum = 0;
        foreach (var num in numbers)
        {
            Console.WriteLine($"foreach: {num}");

            if (num == 30)
            {
                continue; // 跳过当前循环 [Go/JS: continue]
            }

            if (num > 40)
            {
                break;     // 打断循环 [Go/JS: break]
            }

            sum += num;
        }
        Console.WriteLine($"Sum calculated via foreach: {sum}");

        // 3. 经典 while 循环 [JS: while / Go: for condition {}]
        int step = 3;
        while (step > 0)
        {
            step--;
            Console.WriteLine($"while: {step}");
        }

        // 4. do-while 循环 (保证至少执行一次) [JS: do-while / Go 无直接语法]
        int counter = 0;
        do
        {
            counter++;
            Console.WriteLine($"do-while: {counter}");
        } while (counter < 1);
    }

    private static void DemonstrateExceptionFilters()
    {
        // 场景 1：C# 核心独有特性 —— catch when 过滤器
        // 机制：只在 when 满足时才进入 catch 块；不满足时不展开调用栈，保留第一现场。
        string[] testPayloads = ["valid", "timeout_error", "fatal_corrupted_payload"];

        foreach (var payload in testPayloads)
        {
            try
            {
                Console.WriteLine($"当前处理: {payload}");
                ProcessPayload(payload);
            }
            // 条件捕获：仅捕获 InvalidOperationException 且 Message 包含 "timeout" 的异常
            catch (InvalidOperationException ex) when (ex.Message.Contains("timeout"))
            {
                // [JS 需要在 catch 内部做 if 判断再 throw，那会破坏原生栈帧]
                Console.WriteLine($"[可重试错误拦截] 检测到超时: {ex.Message}");
            }
            // 通用捕获基类 Exception [类似 Go 的 recover()]
            catch (Exception ex)
            {
                Console.WriteLine($"[非预期严重故障] {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                // 必定执行清理 [Go: defer 的后半部分效应 / JS: finally]
                // 常见用于无 GC 管辖资源的强制释放
            }
        }

        // 场景 2：Native AOT 高性能标准模式：TryXxx（消除异常惩罚）
        // [类比 Go: val, ok := parse("123")]
        string inputStr = "999";
        // 尝试把字符串转换为 32 位整型。如果成功返回 true 并将解析后的数值塞入 parsedVal；如果失败返回 false，不抛出任何异常。
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

        // 模拟正常业务处理
    }
}
