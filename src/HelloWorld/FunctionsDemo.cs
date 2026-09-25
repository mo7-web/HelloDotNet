namespace MyDemo.FunctionsAndNullability;

// 引入全球化格式化控制命名空间
using System.Globalization;

// 用于演示 in 只读引用传递的大型结构体
public readonly struct HardwareConfig
{
    public required long BufferSize { get; init; }
    public required double LatencyThresholdMs { get; init; }
}

public static class FunctionsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [2.1 函数表达与现代声明] ===");
        DemonstrateFunctions();

        Console.WriteLine("\n=== [2.2 参数修饰符与性能控制] ===");
        DemonstrateParameters();

        Console.WriteLine("\n=== [2.3 严格空安全与现代操作符] ===");
        DemonstrateNullSafety();
    }

    // ==========================================
    // 2.1 现代函数定义与表达
    // ==========================================

    private static int StandardAdd(int a, int b)
    {
        return a + b;
    }

    private static int Multiply(int a, int b)
    {
        return a * b;
    }

    private static void DemonstrateFunctions()
    {
        Console.WriteLine($"Standard Add: {StandardAdd(10, 20)}");
        Console.WriteLine($"Expression Multiply: {Multiply(5, 6)}");
    }

    // ==========================================
    // 2.2 灵活参数特性
    // ==========================================

    private static void ConnectServer(string host, int port = 8080, bool enableTls = false)
    {
        Console.WriteLine($"Connecting to {host}:{port} (TLS: {enableTls})");
    }

    private static bool TryDivide(int numerator, int denominator, out int result)
    {
        if (denominator == 0)
        {
            result = default;
            return false;
        }

        result = numerator / denominator;
        return true;
    }

    private static void Increment(ref int counter)
    {
        counter++;
    }

    private static void InspectConfig(in HardwareConfig cfg)
    {
        Console.WriteLine($"[Config Readonly Ref] Buffer: {cfg.BufferSize}, Latency: {cfg.LatencyThresholdMs}ms");
    }

    private static void DemonstrateParameters()
    {
        ConnectServer(enableTls: true, host: "127.0.0.1", port: 9443);

        if (TryDivide(100, 5, out int divResult))
        {
            Console.WriteLine($"Division Success: {divResult}");
        }

        int trackingCount = 10;
        Increment(ref trackingCount);
        Console.WriteLine($"Incremented via ref: {trackingCount}");

        var config = new HardwareConfig { BufferSize = 1024 * 1024, LatencyThresholdMs = 0.5 };
        InspectConfig(in config);
    }

    // ==========================================
    // 2.3 严格空安全
    // ==========================================

    private static void DemonstrateNullSafety()
    {
        string? nullableStr = null;

        // 核心修复行：显式传递 CultureInfo.InvariantCulture 消除 CA1305 告警/错误
        int? length = nullableStr?.Length;

        Console.WriteLine($"Nullable length1: {length}");

        string lengthDisplay = length?.ToString(CultureInfo.InvariantCulture) ?? "NULL_RESULT";
        Console.WriteLine($"Nullable Length: {lengthDisplay}");

        string safeValue = nullableStr ?? "Default Value Fallback";
        Console.WriteLine($"Fallback Result: {safeValue}");

        nullableStr ??= "Assigned by Null-Coalescing";
        Console.WriteLine($"After ??= : {nullableStr}");

        string forcedStr = nullableStr!;
        Console.WriteLine($"Forced length: {forcedStr.Length}");
    }
}
