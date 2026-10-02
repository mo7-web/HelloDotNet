namespace MyDemo.FunctionsAndNullability;

using System;
using System.Globalization;

// 用于演示 in 只读引用传递的大型结构体
public readonly struct HardwareConfig
{
    public long BufferSize { get; init; }
    public double LatencyThresholdMs { get; init; }

    // 禁用主构造函数与黑盒初始化，显式声明传统构造函数并绑定 this
    public HardwareConfig(long bufferSize, double latencyThresholdMs)
    {
        this.BufferSize = bufferSize;
        this.LatencyThresholdMs = latencyThresholdMs;
    }
}

public static class FunctionsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [2.1 函数表达与现代声明] ===");
        FunctionsDemo.DemonstrateFunctions();

        Console.WriteLine("\n=== [2.2 参数修饰符与性能控制] ===");
        FunctionsDemo.DemonstrateParameters();

        Console.WriteLine("\n=== [2.3 严格空安全与现代操作符] ===");
        FunctionsDemo.DemonstrateNullSafety();
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
        // 静态方法严格携带类名
        int sum = FunctionsDemo.StandardAdd(10, 20);
        Console.WriteLine($"Standard Add: {sum}");

        int product = FunctionsDemo.Multiply(5, 6);
        Console.WriteLine($"Expression Multiply: {product}");
    }

    // ==========================================
    // 2.2 灵活参数特性（in / ref / out）
    // ==========================================

    private static void ConnectServer(string host, int port = 8080, bool enableTls = false)
    {
        Console.WriteLine($"Connecting to {host}:{port} (TLS: {enableTls})");
    }

    // out 参数：调用方提供未初始化槽位，函数内部必须负责赋值返回
    private static bool TryDivide(int numerator, int denominator, out int result)
    {
        if (denominator == 0)
        {
            result = 0; // 显式赋零值，杜绝裸 default 隐式推导
            return false;
        }

        result = numerator / denominator;
        return true;
    }

    // ref 参数：传递变量的直接栈地址，支持函数内原地修改
    private static void Increment(ref int counter)
    {
        counter++;
    }

    // in 参数：只读引用传递（只传 8 字节指针，杜绝大结构体值拷贝，同时由编译器锁死不可修改）
    private static void InspectConfig(in HardwareConfig cfg)
    {
        Console.WriteLine($"[Config Readonly Ref] Buffer: {cfg.BufferSize}, Latency: {cfg.LatencyThresholdMs}ms");
    }

    private static void DemonstrateParameters()
    {
        // 具名实参调用
        FunctionsDemo.ConnectServer(enableTls: true, host: "127.0.0.1", port: 9443);

        if (FunctionsDemo.TryDivide(100, 5, out int divResult))
        {
            Console.WriteLine($"Division Success: {divResult}");
        }

        int trackingCount = 10;
        FunctionsDemo.Increment(ref trackingCount);
        Console.WriteLine($"Incremented via ref: {trackingCount}");

        // 显式实例化结构体
        HardwareConfig config = new HardwareConfig(bufferSize: 1024 * 1024, latencyThresholdMs: 0.5);
        FunctionsDemo.InspectConfig(in config);
    }

    // ==========================================
    // 2.3 严格空安全（消除行内 ?? 与 ??= 语法糖，平铺断点展开）
    // ==========================================

    private static void DemonstrateNullSafety()
    {
        string? nullableStr = null;

        // 1. 显式判空展开，消除 ?. 级联语法糖
        int? length = null;
        if (nullableStr != null)
        {
            length = nullableStr.Length;
        }
        Console.WriteLine($"Nullable length1: {length}");

        // 2. 消除行内 ?? 压缩操作符，对标 Go 显式处理 nil
        string lengthDisplay = "NULL_RESULT";
        if (length != null)
        {
            lengthDisplay = length.Value.ToString(CultureInfo.InvariantCulture);
        }
        Console.WriteLine($"Nullable Length: {lengthDisplay}");

        // 3. 消除裸 ?? 回退逻辑
        string safeValue = "Default Value Fallback";
        if (nullableStr != null)
        {
            safeValue = nullableStr;
        }
        Console.WriteLine($"Fallback Result: {safeValue}");

        // 4. 消除 ??= 压缩赋值语法糖
        if (nullableStr == null)
        {
            nullableStr = "Assigned by Null-Coalescing";
        }
        Console.WriteLine($"After Assignment: {nullableStr}");

        // 5. 显式空断言（仅在绝对能保证非空的场景下使用）
        string forcedStr = nullableStr!;
        Console.WriteLine($"Forced length: {forcedStr.Length}");
    }
}
