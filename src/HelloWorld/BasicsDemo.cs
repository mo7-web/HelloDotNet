namespace MyDemo.Basics;

// 纯静态类：禁止实例化，对标 Go 的独立 package 文件
public static class BasicsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [1.1 基元数值与高精度体系] ===");
        DemonstrateNumericPrimitives();

        Console.WriteLine("\n=== [1.2 变量声明、集合表达式与零值体系] ===");
        DemonstrateVariablesAndDefaults();

        Console.WriteLine("\n=== [1.3 现代字符串与 UTF-8 内存切片] ===");
        DemonstrateStringsAndMemory();
    }

    // ==========================================
    // 1.1 基元数值类型与字面量后缀
    // ==========================================
    private static void DemonstrateNumericPrimitives()
    {
        int population = 67_000_000;
        long distance = 384_400_000L;
        short temperature = -40;
        byte red = 255;
        double pi = 3.141592653589793;
        float gravity = 9.81f;
        decimal price = 19.99m;

        // 原始多行插值字符串：以最后一行 """ 的缩进为基准，前导空白在编译期自动裁切
        // 映射：比 Go 的 `...` 强大（支持直接插值），比 TS 的 `...` 干净（免 strip-indent）
        Console.WriteLine(
            $"""
        population  {population}
        distance    {distance}
        temperature {temperature}
        red         {red}
        pi          {pi}
        gravity     {gravity}
        price       {price}
        """
        );
    }

    // ==========================================
    // 1.2 变量推导、集合表达式与零值机制
    // ==========================================
    private static void DemonstrateVariablesAndDefaults()
    {
        // 1. 显式类型 vs 局部推导
        double inferredDouble = 1.0;          // [Go: v := 1.0] [TS: let v = 1.0] 优先使用具体类型明确语义

        // 现代集合表达式（编译期优化，对标 Go 的切片字面量 []int{1, 2}）
        int[] numbers = [1, 2];

        Console.WriteLine($"[推导与集合] Double: {inferredDouble}, Array Length: {numbers.Length}");

        // 2. 确定性常量（编译期直接内联至机器码）
        const double Pi = 3.1415926;          // [Go: const Pi = 3.1415926]
        Console.WriteLine($"[编译期常量] Pi: {Pi}");

        // 3. 零值体系 (default)
        int defaultInt = default;             // 零值: 0 [Go: var a int]
        bool defaultBool = default;           // 零值: false [Go: var b bool]
        decimal defaultMoney = default;       // 零值: 0.0m
        string? defaultRef = default;         // 零值: null [Go: nil / TS: null]

        // 消除行内 ?? 压缩语法，显式断点判定（对标 Go 显式 nil 校验）
        string displayRef = "NULL";
        if (defaultRef != null)
        {
            displayRef = defaultRef;
        }

        Console.WriteLine($"[零值体系] Int: {defaultInt}, Bool: {defaultBool}, Decimal: {defaultMoney}, Ref: {displayRef}");
    }

    // ==========================================
    // 1.3 现代字符串与 Native AOT 特性
    // ==========================================
    private static void DemonstrateStringsAndMemory()
    {
        string userName = "Alice";
        decimal balance = 199.99m;

        // 1. 现代字符串插值（Native AOT 编译期通过 DefaultInterpolatedStringHandler 实现零分配构建）
        string welcomeMsg = $"Hello, {userName}! Balance: {balance}";
        Console.WriteLine($"[字符串插值] {welcomeMsg}");

        // 2. 原始字符串字面量 (Raw String Literals) - C# 11+
        // 映射：对标 Go 的反引号多行文本 `{"key": "value"}`，内容中无需转义双引号
        string jsonPayload = """
        {
            "userId": 1001,
            "name": "Alice",
            "roles": ["admin", "developer"],
            "isActive": true
        }
        """;
        Console.WriteLine($"[原始多行 JSON]\n{jsonPayload}");

        // 3. UTF-8 字节字面量（Native AOT 高性能关键基石）
        // 映射：对标 Go 的 []byte("PING")。C# 在编译期将其直接固化至只读数据段，完全不经过 UTF-16 转码，零堆分配
        ReadOnlySpan<byte> utf8Ping = "PING"u8;
        Console.WriteLine($"[只读 UTF-8 内存切片] Length: {utf8Ping.Length} bytes, First Byte: {utf8Ping[0]}");

        // 4. 原生路径字符串（消除反斜杠转义）
        string filePath = @"C:\Users\docs\file.txt";
        Console.WriteLine($"[原样路径] {filePath}");
    }
}
