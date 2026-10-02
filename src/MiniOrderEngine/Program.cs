namespace MiniOrderEngine;

using System;
using System.Globalization;
using MemoryPointer;

internal static class Program
{
    public static void Main()
    {
        Console.WriteLine("=== C# 核心特性与底层心智实战练习 ===\n");

        // --------------------------------------------------------------------
        // 实验 1：double vs decimal 精度实验
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 1：浮点数 vs 十进制精度 ---");

        double unsafeSum = 0.0;
        for (int i = 0; i < 10; i++)
        {
            unsafeSum += 0.1;
        }

        decimal safeSum = 0.0m;
        for (int i = 0; i < 10; i++)
        {
            safeSum += 0.1m;
        }

        Console.WriteLine($"10 次 0.1 (double)  累加结果: {unsafeSum} (是否等于 1.0? {unsafeSum == 1.0})");
        Console.WriteLine($"10 次 0.1 (decimal) 累加结果: {safeSum} (是否等于 1.0? {safeSum == 1.0m})\n");

        // --------------------------------------------------------------------
        // 实验 2：struct（值类型） vs class（引用类型）内存行为
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 2：struct vs class 内存行为 ---");

        MarketTick tickA = new MarketTick(1001, 65000.5m, 65000.5);
        MarketTick tickB = tickA; // 栈内存完整值拷贝
        tickB.Price = 70000.0m;

        Console.WriteLine($"修改副本后：tickA 的价格是 {tickA.Price} (符合值拷贝)");
        Console.WriteLine($"修改副本后：tickB 的价格是 {tickB.Price}");

        Account accA = new Account("ACC-001", 1000m);
        Account accB = accA; // 纯指针拷贝，指向堆上同一块内存
        accB.Balance = 500m;

        Console.WriteLine($"修改指针后：accA 的余额被连带修改成了 {accA.Balance} (共享堆内存)\n");

        // --------------------------------------------------------------------
        // 实验 3：[Flags] 位掩码与标准常量 switch
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 3：位掩码枚举与标准分支 ---");

        OrderOptions flags = OrderOptions.MakerOnly | OrderOptions.IsPostOnly;
        // 位运算检查（对标 Go: flags & OrderOptions.MakerOnly != 0）
        bool isMaker = flags.HasFlag(OrderOptions.MakerOnly);
        Console.WriteLine($"当前订单属性是否为 MakerOnly? {isMaker}");

        // 规范重构：彻底消除 switch 表达式，使用标准常量 switch 语句
        OrderSide side = OrderSide.Buy;
        string sideDesc = Program.GetOrderSideDescription(side);
        Console.WriteLine($"订单操作方向描述: {sideDesc}\n");

        // --------------------------------------------------------------------
        // 实验 4：out 参数与 TryParse（零分配防御性解析）
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 4：防御性解析（对标 Go: val, ok 模式）---");

        string rawInputPrice = "65000.25";
        string invalidInput = "abc_price";

        if (decimal.TryParse(rawInputPrice, CultureInfo.InvariantCulture, out decimal parsedPrice))
        {
            Console.WriteLine($"字符串解析成功: {parsedPrice}");
        }

        if (!decimal.TryParse(invalidInput, CultureInfo.InvariantCulture, out decimal failedPrice))
        {
            Console.WriteLine("安全捕获非法输入，避免了 throw 异常带来的栈展开巨额开销！\n");
        }

        // --------------------------------------------------------------------
        // 实验 5：using 确定性资源释放
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 5：using 资源生命周期监控 ---");
        // 静态方法严格携带 Program. 宿主前缀
        Program.ExecuteMockTransaction(accA, 100m);

        // --------------------------------------------------------------------
        // 实验 6：串联 FastParser 零内存分配切片解析实战
        // --------------------------------------------------------------------
        Console.WriteLine("\n--- 实验 6：ReadOnlySpan<char> 零分配内存切片解析 ---");
        string telemetryPayload = "BTCUSDT,65432.10,1500";
        // AsSpan() 直接提取栈切片，不产生堆拷贝
        if (FastParser.TryParseTick(telemetryPayload.AsSpan(), out ParsedTick parsedTick))
        {
            Console.WriteLine($"[Span 解析成功] Price: {parsedTick.Price}, Volume: {parsedTick.Volume}");
        }

        Console.WriteLine("\n全部手动练习演示执行完毕。");
    }

    // 独立出标准常量 switch 辅助方法，分支可单行命中断点，逻辑透明
    private static string GetOrderSideDescription(OrderSide side)
    {
        switch (side)
        {
            case OrderSide.Buy:
                {
                    return "做多/买入入场";
                }
            case OrderSide.Sell:
                {
                    return "做空/卖出离场";
                }
            default:
                {
                    return "未知方向";
                }
        }
    }

    private static void ExecuteMockTransaction(Account acc, decimal amount)
    {
        // 显式声明 AuditLogScope 类型，拒绝盲盒 var
        using (AuditLogScope audit = new AuditLogScope("扣减保证金事务"))
        {
            Console.WriteLine($"正在为账户 {acc.AccountId} 扣除金额 {amount}...");
            acc.Balance -= amount;
            Console.WriteLine($"扣除成功，当前实时余额: {acc.Balance}");
        }

        Console.WriteLine("事务块外部：后续逻辑安全继续。");
    }
}
