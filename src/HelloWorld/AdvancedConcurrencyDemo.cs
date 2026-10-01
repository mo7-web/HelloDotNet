namespace MyDemo.Advanced;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

// ==========================================
// 领域契约定义：显式 sealed 便于 AOT 虚方法去虚化
// ==========================================
public abstract record PaymentOrder;

public sealed record CashOrder(decimal Amount) : PaymentOrder;

public sealed record CreditCardOrder(decimal Amount, string CardNumber, int RiskScore) : PaymentOrder;

public sealed record CryptoOrder(decimal UsdtAmount, string Chain) : PaymentOrder;

public static class AdvancedConcurrencyDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== [5.1 扁平类型守卫与模式匹配] ===");
        DemonstratePatternMatching();

        Console.WriteLine("\n=== [5.2 异步并发与协同取消] ===");
        await DemonstrateAsyncAndCancellationAsync();
    }

    // ==========================================
    // 5.1 模式匹配：平铺展开，消除 switch 表达式
    // ==========================================
    private static void DemonstratePatternMatching()
    {
        // 允许使用 C# 现代集合表达式 [ ... ]
        PaymentOrder[] testOrders =
        [
            new CashOrder(150.0m),
            new CreditCardOrder(999.0m, "4111-XXXX-XXXX-1111", 85),
            new CreditCardOrder(50.0m, "4111-XXXX-XXXX-2222", 10),
            new CryptoOrder(3000.0m, "Ethereum")
        ];

        foreach (var order in testOrders)
        {
            string auditResult = EvaluateOrder(order);
            Console.WriteLine(auditResult);
        }

        int temperature = 25;
        string weatherAlert = GetWeatherAlert(temperature);
        Console.WriteLine($"Weather Status: {weatherAlert}");
    }

    // Go 风格卫语句：消除嵌套，每一步都可精准下断点调试
    private static string EvaluateOrder(PaymentOrder order)
    {
        // 场景 A：现金单类型守卫与金额校验
        if (order is CashOrder cash)
        {
            if (cash.Amount > 100.0m)
            {
                return $"[Cash Verified] High-value cash: {cash.Amount.ToString(CultureInfo.InvariantCulture)}";
            }

            return $"[Cash Accepted] Standard cash: {cash.Amount.ToString(CultureInfo.InvariantCulture)}";
        }

        // 场景 B：信用卡类型守卫与风控校验
        if (order is CreditCardOrder card)
        {
            if (card.RiskScore > 80)
            {
                return $"[Card Blocked] Fraud risk detected on card: {card.CardNumber}";
            }

            return $"[Card Accepted] Normal transaction: {card.Amount.ToString(CultureInfo.InvariantCulture)}";
        }

        // 场景 C：加密货币主网白名单校验
        if (order is CryptoOrder crypto)
        {
            var isSupportedChain = crypto.Chain == "Ethereum" || crypto.Chain == "Solana";
            if (isSupportedChain)
            {
                return $"[Crypto Pass] Mainstream chain supported: {crypto.Chain}, Amount: {crypto.UsdtAmount.ToString(CultureInfo.InvariantCulture)}";
            }

            return $"[Crypto Rejected] Unsupported chain: {crypto.Chain}";
        }

        // 兜底返回（对标 TS never 或 Go default）
        return "[Order Rejected] Unsupported payment structure";
    }

    // 扁平化条件分支：杜绝多层三元与 switch 表达式
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
    // 5.2 异步编程 (Task 与 CancellationToken 显式流)
    // ==========================================
    private static async Task DemonstrateAsyncAndCancellationAsync()
    {
        int cachedVal = await FetchDataWithCacheAsync(cacheHit: true);
        Console.WriteLine($"Cache Result: {cachedVal.ToString(CultureInfo.InvariantCulture)}");

        // 强制使用显式大括号 using 块，明确资源存续边界
        using (var cts = new CancellationTokenSource(delay: TimeSpan.FromMilliseconds(500)))
        {
            try
            {
                Console.WriteLine("[Async Task] Starting heavy network stream simulation...");

                // 将 cts.Token 显式透传，对应 Go 的 ctx
                await ProcessNetworkStreamAsync(cts.Token);

                Console.WriteLine("[Async Task] Completed successfully.");
            }
            catch (OperationCanceledException)
            {
                // 捕获取消信号，对应 Go 的 select { case <-ctx.Done(): }
                Console.WriteLine("[Async Cancelled] Worker gracefully stopped via CancellationToken timeout!");
            }
        }

        // 静态本地函数：杜绝闭包捕获，保证 0 堆分配
        static int ComputeFastChecksum(ReadOnlySpan<byte> bytes)
        {
            var checksum = 0;
            foreach (var b in bytes)
            {
                checksum ^= b;
            }

            return checksum;
        }

        var hash = ComputeFastChecksum("PAYLOAD"u8);
        Console.WriteLine($"Local Static Function Checksum: {hash.ToString(CultureInfo.InvariantCulture)}");
    }

    // 异步全透明：统一返回 Task，不搞过度优化的复杂状态机包装
    private static async Task<int> FetchDataWithCacheAsync(bool cacheHit)
    {
        if (cacheHit)
        {
            return 42;
        }

        await Task.Delay(100);
        return 100;
    }

    // 标准 CancellationToken 协作式取消工作流
    private static async Task ProcessNetworkStreamAsync(CancellationToken ct)
    {
        for (var i = 1; i <= 10; i++)
        {
            // 显式检查取消标记
            ct.ThrowIfCancellationRequested();

            Console.WriteLine($"Processing chunk {i.ToString(CultureInfo.InvariantCulture)}/10...");

            await Task.Delay(100, ct);
        }
    }
}
