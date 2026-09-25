namespace MyDemo.Advanced;

using System.Globalization;

// 领域模型定义：用于模式匹配演练
public abstract record PaymentOrder;
public record CashOrder(decimal Amount) : PaymentOrder;
public record CreditCardOrder(decimal Amount, string CardNumber, int RiskScore) : PaymentOrder;
public record CryptoOrder(decimal UsdtAmount, string Chain) : PaymentOrder;

public static class AdvancedConcurrencyDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== [5.1 现代模式匹配 Pattern Matching] ===");
        DemonstratePatternMatching();

        Console.WriteLine("\n=== [5.2 异步并发、ValueTask 与协同取消] ===");
        await DemonstrateAsyncAndCancellationAsync();
    }

    // ==========================================
    // 5.1 现代模式匹配 (Pattern Matching)
    // ==========================================
    private static void DemonstratePatternMatching()
    {
        // 1. switch 表达式 + 类型匹配 + 属性解构 + 关系模式 (> / <=)
        // [TS 需要写多层 if (typeof/instanceof) 以及 switch-case；Go 的 switch 只支持基础值比对]
        PaymentOrder[] testOrders =
        [
            new CashOrder(150.0m),
            new CreditCardOrder(999.0m, "4111-XXXX-XXXX-1111", RiskScore: 85),
            new CreditCardOrder(50.0m, "4111-XXXX-XXXX-2222", RiskScore: 10),
            new CryptoOrder(3000.0m, "Ethereum")
        ];

        foreach (var order in testOrders)
        {
            string auditResult = EvaluateOrder(order);
            Console.WriteLine(auditResult);
        }

        // 2. 关系与逻辑组合模式 (and / or / not)
        int temperature = 25;
        string weatherAlert = temperature switch
        {
            < 0 => "Freezing Alert",
            >= 0 and <= 30 => "Comfortable Condition", // [TS: temp >= 0 && temp <= 30]
            _ => "Heatstroke Warning"                  // [TS/Go: default]
        };
        Console.WriteLine($"Weather Status: {weatherAlert}");
    }

    // 纯声明式 switch 表达式：返回值驱动，告别老旧的 break 语句
    private static string EvaluateOrder(PaymentOrder order)
    {
        return order switch
        {
            // 场景 A：现金单，且金额 > 100 (关系模式)
            CashOrder { Amount: > 100m } cash =>
                $"[Cash Verified] High-value cash: {cash.Amount.ToString(CultureInfo.InvariantCulture)}",

            // 场景 B：信用卡，嵌套属性模式匹配：风控分 > 80 直接阻断
            CreditCardOrder { RiskScore: > 80, CardNumber: var card } =>
                $"[Card Blocked] Fraud risk detected on card: {card}",

            // 场景 C：信用卡普通放行
            CreditCardOrder card =>
                $"[Card Accepted] Normal transaction: {card.Amount.ToString(CultureInfo.InvariantCulture)}",

            // 场景 D：加密货币指定链匹配
            CryptoOrder { Chain: "Ethereum" or "Solana" } crypto =>
                $"[Crypto Pass] Mainstream chain supported: {crypto.Chain}, Amount: {crypto.UsdtAmount.ToString(CultureInfo.InvariantCulture)}",

            // 弃元模式（对标 Go 的 _ 与 TS default）
            _ => "[Order Rejected] Unsupported payment structure"
        };
    }

    // ==========================================
    // 5.2 异步编程 (Task / ValueTask / CancellationToken)
    // ==========================================
    private static async Task DemonstrateAsyncAndCancellationAsync()
    {
        // 1. ValueTask 性能演示：如果结果已经同步就绪，绝对零堆分配 (Native AOT 核心基石)
        int cachedVal = await FetchDataWithCacheAsync(cacheHit: false);
        Console.WriteLine($"ValueTask Cache Hit: {cachedVal.ToString(CultureInfo.InvariantCulture)}");

        // 2. 协同取消机制实战 (对标 Go 的 context.WithTimeout)
        // [Go: ctx, cancel := context.WithTimeout(context.Background(), 200*time.Millisecond)]
        using var cts = new CancellationTokenSource(delay: TimeSpan.FromMilliseconds(500));

        try
        {
            Console.WriteLine("[Async Task] Starting heavy network stream simulation...");

            // 将 cts.Token (对标 ctx) 往下层函数透传
            await ProcessNetworkStreamAsync(cts.Token);

            Console.WriteLine("[Async Task] Completed successfully.");
        }
        catch (OperationCanceledException)
        {
            // 优雅拦截协同取消信号 [对标 Go 里的 case <-ctx.Done():]
            Console.WriteLine("[Async Cancelled] Worker gracefully stopped via CancellationToken timeout!");
        }

        // ==========================================
        // 5.3 本地函数 (Local Functions) 实战
        // ==========================================
        // static 本地函数：编译器强制阻断闭包上下文捕获，保证 0 委托分配
        // [TS: const helper = () => { ... } 会产生函数对象分配]
        static int ComputeFastChecksum(ReadOnlySpan<byte> bytes)
        {
            int checksum = 0;
            foreach (var b in bytes)
            {
                checksum ^= b;
            }

            return checksum;
        }

        int hash = ComputeFastChecksum("PAYLOAD"u8);
        Console.WriteLine($"Local Static Function Checksum: {hash.ToString(CultureInfo.InvariantCulture)}");
    }

    // ValueTask<T>：对标高吞吐接口。缓存命中时是无堆分配的栈值；未命中时才退化为异步 Task
    private static ValueTask<int> FetchDataWithCacheAsync(bool cacheHit)
    {
        if (cacheHit)
        {
            // 同步快速路径 (Fast Path)：零分配返回！
            return new ValueTask<int>(42);
        }

        // 慢速路径 (Slow Path)：走真实异步状态机
        return new ValueTask<int>(Task.Run(async () =>
        {
            await Task.Delay(1000);
            return 100;
        }));
    }

    // 接受 CancellationToken 级联透传的异步工作负载
    // [Go: func ProcessNetworkStream(ctx context.Context) error]
    private static async Task ProcessNetworkStreamAsync(CancellationToken ct)
    {
        for (int i = 1; i <= 10; i++)
        {
            // 核心检测点：抛出 OperationCanceledException
            // [Go: if err := ctx.Err(); err != nil { return err }]
            ct.ThrowIfCancellationRequested();

            Console.WriteLine($"Processing chunk {i.ToString(CultureInfo.InvariantCulture)}/10...");

            // Task.Delay 同样原生接收取消信号，超时立刻唤醒并终止
            // [Go: select { case <-time.After(100*time.Millisecond): case <-ctx.Done(): }]
            await Task.Delay(100, ct);
        }
    }
}
