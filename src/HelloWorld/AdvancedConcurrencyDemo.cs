namespace MyDemo.Advanced;

// ==========================================
// 1. 数据契约定义（接口契约 + 具名扁平结构体）
// 映射：对标 Go 的 interface 定义与 struct 实体
// ==========================================

public interface IPaymentOrder
{
    decimal Amount { get; }
}

public readonly record struct CashOrder(decimal Amount) : IPaymentOrder;

public readonly record struct CreditCardOrder(decimal Amount, string CardNumber, int RiskScore) : IPaymentOrder;

public readonly record struct CryptoOrder(decimal Amount, string Chain) : IPaymentOrder;

// ==========================================
// 2. 业务演练主类
// ==========================================

public static class AdvancedConcurrencyDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== [5.1 现代模式匹配与卫语句 (Guard Clauses)] ===");
        DemonstratePatternMatching();

        Console.WriteLine("\n=== [5.2 异步全透明、协同取消与确定性释放] ===");
        await DemonstrateAsyncAndCancellationAsync();
    }

    // ==========================================
    // 5.1 模式匹配扁平化演练
    // ==========================================
    private static void DemonstratePatternMatching()
    {
        // 使用现代集合表达式声明数组契约
        IPaymentOrder[] testOrders =
        [
            new CashOrder(150.0m),
            new CreditCardOrder(999.0m, "4111-XXXX-XXXX-1111", RiskScore: 85),
            new CreditCardOrder(50.0m, "4111-XXXX-XXXX-2222", RiskScore: 10),
            new CryptoOrder(3000.0m, "Ethereum")
        ];

        foreach (IPaymentOrder order in testOrders)
        {
            string auditResult = EvaluateOrder(order);
            Console.WriteLine(auditResult);
        }

        // 扁平状态获取演示
        int temperature = 25;
        string weatherAlert = GetWeatherAlert(temperature);
        Console.WriteLine($"Weather Status: {weatherAlert}");
    }

    // 严禁 switch 表达式，重构为 Go 风格的扁平卫语句
    // 断点可单行命中，分支逻辑透明展开
    private static string EvaluateOrder(IPaymentOrder order)
    {
        // 场景 A：现金单且金额 > 100
        if (order is CashOrder cash && cash.Amount > 100.0m)
        {
            return $"[Cash Verified] High-value cash: {cash.Amount}";
        }

        // 场景 B：信用卡风控拦截（嵌套模式属性检查）
        if (order is CreditCardOrder highRiskCard && highRiskCard.RiskScore > 80)
        {
            return $"[Card Blocked] Fraud risk detected on card: {highRiskCard.CardNumber}";
        }

        // 场景 C：信用卡普通放行
        if (order is CreditCardOrder normalCard)
        {
            return $"[Card Accepted] Normal transaction: {normalCard.Amount}";
        }

        // 场景 D：指定加密链放行（显式逻辑判断，对标 TS/Go 展开）
        if (order is CryptoOrder crypto)
        {
            if (crypto.Chain == "Ethereum" || crypto.Chain == "Solana")
            {
                return $"[Crypto Pass] Mainstream chain supported: {crypto.Chain}, Amount: {crypto.Amount}";
            }
        }

        // 默认兜底回退（对标 Go switch default 或末尾防御返回）
        return "[Order Rejected] Unsupported payment structure";
    }

    // 简单状态判断消除三元与 switch 表达式，清晰展开
    private static string GetWeatherAlert(int temperature)
    {
        if (temperature < 0)
        {
            return "Freezing Alert";
        }

        if (temperature <= 30)
        {
            return "Comfortable Condition";
        }

        return "Heatstroke Warning";
    }

    // ==========================================
    // 5.2 异步编程全透明演示
    // ==========================================
    private static async Task DemonstrateAsyncAndCancellationAsync()
    {
        // 1. 异步数据获取（统一 Task，全程显式 await）
        int cachedVal = await FetchDataWithCacheAsync(cacheHit: false);
        Console.WriteLine($"Async Fetch Result: {cachedVal}");

        // 2. 协同取消机制实战（对标 Go 的 context.WithTimeout）
        // 强制使用带大括号的显式 using 块，杜绝 using var 造成的延迟析构
        using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500)))
        {
            try
            {
                Console.WriteLine("[Async Task] Starting heavy network stream simulation...");

                // 级联透传 CancellationToken（对标 Go 透传 ctx）
                await ProcessNetworkStreamAsync(cts.Token);

                Console.WriteLine("[Async Task] Completed successfully.");
            }
            catch (OperationCanceledException)
            {
                // 拦截超时取消信号（对标 Go select case <-ctx.Done():）
                Console.WriteLine("[Async Cancelled] Worker gracefully stopped via CancellationToken timeout!");
            }
        }

        // 3. 静态本地逻辑提炼为显式静态方法，避免闭包与委托隐式分配
        int hash = ComputeFastChecksum("PAYLOAD"u8);
        Console.WriteLine($"Static Function Checksum: {hash}");
    }

    // 异步全透明：统一返回 Task<T> 并显式 await，杜绝 ValueTask 复杂陷阱
    private static async Task<int> FetchDataWithCacheAsync(bool cacheHit)
    {
        if (cacheHit)
        {
            return 42;
        }

        // 模拟真实 I/O 慢速路径，全链路透明传递
        await Task.Delay(1000);
        return 100;
    }

    // 接受 CancellationToken 级联透传的异步工作负载
    // 映射：Go 的 func ProcessNetworkStream(ctx context.Context) error
    private static async Task ProcessNetworkStreamAsync(CancellationToken cancellationToken)
    {
        for (int i = 1; i <= 10; i++)
        {
            // 核心检测点：抛出 OperationCanceledException
            // 映射：Go 的 if err := ctx.Err(); err != nil { return err }
            cancellationToken.ThrowIfCancellationRequested();

            Console.WriteLine($"Processing chunk {i}/10...");

            // Task.Delay 接收取消令牌，收到取消信号时立刻中断唤醒
            await Task.Delay(100, cancellationToken);
        }
    }

    // 高吞吐基础算法：静态方法杜绝闭包分配，ReadOnlySpan 零内存拷贝
    private static int ComputeFastChecksum(ReadOnlySpan<byte> bytes)
    {
        int checksum = 0;
        foreach (byte b in bytes)
        {
            checksum ^= b;
        }

        return checksum;
    }
}
