namespace MyDemo.Advanced;

// ==========================================
// 1. 数据契约定义（接口契约 + 具名扁平结构体）
// 对标：Go 的 interface 与 struct 具名属性 + 构造函数
// ==========================================

public interface IPaymentOrder
{
    decimal Amount { get; }
}

public readonly record struct CashOrder : IPaymentOrder
{
    public decimal Amount { get; init; }

    // 禁用主构造函数，显式声明传统构造函数并绑定 this
    public CashOrder(decimal amount)
    {
        this.Amount = amount;
    }
}

public readonly record struct CreditCardOrder : IPaymentOrder
{
    public decimal Amount { get; init; }
    public string CardNumber { get; init; }
    public int RiskScore { get; init; }

    public CreditCardOrder(decimal amount, string cardNumber, int riskScore)
    {
        this.Amount = amount;
        this.CardNumber = cardNumber;
        this.RiskScore = riskScore;
    }
}

public readonly record struct CryptoOrder : IPaymentOrder
{
    public decimal Amount { get; init; }
    public string Chain { get; init; }

    public CryptoOrder(decimal amount, string chain)
    {
        this.Amount = amount;
        this.Chain = chain;
    }
}

// ==========================================
// 2. 业务演练主类
// ==========================================


/*

实例成员（非 static）	必须依赖 new 分配在堆上的具体对象	必须显式写 this.	Go 的 this.Method() (Receiver)
静态成员（static）	纯符号地址，与具体对象无关，全局唯一	必须显式写 ClassName.	Go 的 pkg.Func() (包级函数)

 */

public static class AdvancedConcurrencyDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== [5.1 显式分支与卫语句 (Guard Clauses)] ===");
        // 静态方法一律带类名，明确调用来源
        AdvancedConcurrencyDemo.DemonstratePatternMatching();

        Console.WriteLine("\n=== [5.2 异步全透明、协同取消与确定性释放] ===");
        await AdvancedConcurrencyDemo.DemonstrateAsyncAndCancellationAsync();
    }

    // ==========================================
    // 5.1 分支扁平化演练
    // ==========================================
    private static void DemonstratePatternMatching()
    {
        // 显式集合声明（规范允许集合表达式）
        IPaymentOrder[] testOrders =
        [
            new CashOrder(150.0m),
            new CreditCardOrder(999.0m, "4111-XXXX-XXXX-1111", 85),
            new CreditCardOrder(50.0m, "4111-XXXX-XXXX-2222", 10),
            new CryptoOrder(3000.0m, "Ethereum")
        ];

        foreach (IPaymentOrder order in testOrders)
        {
            string auditResult = AdvancedConcurrencyDemo.EvaluateOrder(order);
            Console.WriteLine(auditResult);
        }

        int temperature = 25;
        string weatherAlert = AdvancedConcurrencyDemo.GetWeatherAlert(temperature);
        Console.WriteLine($"Weather Status: {weatherAlert}");
    }

    // 结构彻底平铺：单层职责、单行断点，对标 Go 的类型断言 (v, ok := i.(Type))
    private static string EvaluateOrder(IPaymentOrder order)
    {
        // 场景 A：现金单校验
        if (order is CashOrder)
        {
            CashOrder cash = (CashOrder)order;
            if (cash.Amount > 100.0m)
            {
                return $"[Cash Verified] High-value cash: {cash.Amount}";
            }
        }

        // 场景 B & C：信用卡类型校验
        if (order is CreditCardOrder)
        {
            CreditCardOrder card = (CreditCardOrder)order;
            if (card.RiskScore > 80)
            {
                return $"[Card Blocked] Fraud risk detected on card: {card.CardNumber}";
            }

            return $"[Card Accepted] Normal transaction: {card.Amount}";
        }

        // 场景 D：加密货币链校验
        if (order is CryptoOrder)
        {
            CryptoOrder crypto = (CryptoOrder)order;
            if (crypto.Chain == "Ethereum" || crypto.Chain == "Solana")
            {
                return $"[Crypto Pass] Mainstream chain supported: {crypto.Chain}, Amount: {crypto.Amount}";
            }
        }

        // 防御性兜底
        return "[Order Rejected] Unsupported payment structure";
    }

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
    // 5.2 异步全透明演练
    // ==========================================
    private static async Task DemonstrateAsyncAndCancellationAsync()
    {
        int cachedVal = await AdvancedConcurrencyDemo.FetchDataWithCacheAsync(false);
        Console.WriteLine($"Async Fetch Result: {cachedVal}");

        // 显式 using 作用域块，严格保证非托管资源在离开大括号时同步释放
        using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500)))
        {
            try
            {
                Console.WriteLine("[Async Task] Starting heavy network stream simulation...");

                // 级联透传 CancellationToken
                await AdvancedConcurrencyDemo.ProcessNetworkStreamAsync(cts.Token);

                Console.WriteLine("[Async Task] Completed successfully.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[Async Cancelled] Worker gracefully stopped via CancellationToken timeout!");
            }
        }

        int hash = AdvancedConcurrencyDemo.ComputeFastChecksum("PAYLOAD"u8);
        Console.WriteLine($"Static Function Checksum: {hash}");
    }

    private static async Task<int> FetchDataWithCacheAsync(bool cacheHit)
    {
        if (cacheHit)
        {
            return 42;
        }

        await Task.Delay(1000);
        return 100;
    }

    private static async Task ProcessNetworkStreamAsync(CancellationToken cancellationToken)
    {
        for (int i = 1; i <= 10; i++)
        {
            // 显式检测中断信号（对标 Go: if err := ctx.Err(); err != nil）
            cancellationToken.ThrowIfCancellationRequested();

            Console.WriteLine($"Processing chunk {i}/10...");

            await Task.Delay(100, cancellationToken);
        }
    }

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
