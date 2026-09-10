using MiniOrderEngine;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== C# 核心特性与底层心智实战练习 ===\n");

        // --------------------------------------------------------------------
        // 实验 1：double vs decimal 精度灾难实验
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 1：浮点数 vs 十进制精度 ---");

        // 使用 IEEE 754 标准的 double 进行累加（JS 和 Go float64 默认行为）
        double unsafeSum = 0.0;
        for (int i = 0; i < 10; i++)
        {
            unsafeSum += 0.1;
        }

        // 使用 C# 特有的 decimal 累加
        decimal safeSum = 0.0m; // 注意 m 后缀
        for (int i = 0; i < 10; i++)
        {
            safeSum += 0.1m;
        }

        // 观察两者的控制台输出差异
        Console.WriteLine(
          $"10 次 0.1 (double)  累加结果: {unsafeSum} (是否等于 1.0? {unsafeSum == 1.0})"
        );
        Console.WriteLine(
          $"10 次 0.1 (decimal) 累加结果: {safeSum} (是否等于 1.0? {safeSum == 1.0m})\n"
        );

        // --------------------------------------------------------------------
        // 实验 2：struct（值类型） vs class（引用类型）物理篡改实验
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 2：struct vs class 内存行为 ---");

        // 测试 struct：赋值意味着整块物理内存的深度拷贝（栈拷贝）
        MarketTick tickA = new MarketTick(1001, 65000.5m, 65000.5);
        MarketTick tickB = tickA; // 发生栈内存复制，tickB 是完全独立的副本
        tickB.Price = 70000.0m; // 修改副本

        Console.WriteLine($"修改副本后：tickA 的价格是 {tickA.Price} (未被修改，符合值拷贝)");
        Console.WriteLine($"修改副本后：tickB 的价格是 {tickB.Price}");

        // 测试 class：赋值只是拷贝堆内存的地址指针
        Account accA = new Account("ACC-001", 1000m);
        Account accB = accA; // 拷贝指针！accA 和 accB 指向堆上的同一块内存
        accB.Balance = 500m; // 篡改数据

        Console.WriteLine($"修改指针后：accA 的余额被连带修改成了 {accA.Balance} (因为共享堆内存)\n");

        // --------------------------------------------------------------------
        // 实验 3：[Flags] 枚举与现代 switch 表达式
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 3：位掩码枚举与模式匹配 ---");

        // 组合权限：同时指定为 MakerOnly 和 IsPostOnly（按位或 |）
        OrderFlags flags = OrderFlags.MakerOnly | OrderFlags.IsPostOnly;

        // 检查是否包含 MakerOnly 权限
        bool isMaker = flags.HasFlag(OrderFlags.MakerOnly);
        Console.WriteLine($"当前订单属性是否为 MakerOnly? {isMaker}");

        // 现代 switch 表达式：根据买卖方向输出描述
        OrderSide side = OrderSide.Buy;
        string sideDesc = side switch
        {
            OrderSide.Buy => "做多/买入入场",
            OrderSide.Sell => "做空/卖出离场",
            _ => "未知方向",
        };
        Console.WriteLine($"订单操作方向描述: {sideDesc}\n");

        // --------------------------------------------------------------------
        // 实验 4：out 参数与 TryParse（零分配防御性解析）
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 4：防御性解析（对标 Go 的 val, ok 模式）---");

        string rawInputPrice = "65000.25";
        string invalidInput = "abc_price";

        // TryParse 内部失败绝不抛异常，而是返回 false，并通过 out 关键字向外填充解析值
        if (decimal.TryParse(rawInputPrice, out decimal parsedPrice))
        {
            Console.WriteLine($"字符串解析成功: {parsedPrice}");
        }

        if (!decimal.TryParse(invalidInput, out decimal failedPrice))
        {
            Console.WriteLine($"安全捕获非法输入，避免了 throw 异常带来的栈展开巨额开销！\n");
        }

        // --------------------------------------------------------------------
        // 实验 5：using 确定性资源释放
        // --------------------------------------------------------------------
        Console.WriteLine("--- 实验 5：using 资源生命周期监控 ---");

        ExecuteMockTransaction(accA, 100m);

        Console.WriteLine("\n全部手动练习演示执行完毕。");
    }

    // 模拟一段交易执行逻辑，利用 using 监控生命周期
    private static void ExecuteMockTransaction(Account acc, decimal amount)
    {
        // using 括号内的变量在执行完当前大括号后，必定自动触发 Dispose()
        // 作用域比 Go 的函数级 defer 更灵活（它可以限制在任意局部代码块内）
        using (var audit = new AuditLogScope("扣减保证金事务"))
        {
            Console.WriteLine($"正在为账户 {acc.AccountId} 扣除金额 {amount}...");
            acc.Balance -= amount;
            Console.WriteLine($"扣除成功，当前实时余额: {acc.Balance}");
        } // 离开这个右大括号的瞬间，立即打印 [审计结束]

        Console.WriteLine("事务块外部：后续逻辑安全继续。");
    }
}
