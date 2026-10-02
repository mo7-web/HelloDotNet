namespace MyDemo.Basics;

// 纯静态类：禁止实例化，对标 Go 的独立 package 文件
public static class BasicsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [1.1 基元数值与高精度体系] ===");
        // 静态方法严格携带类名前缀
        BasicsDemo.DemonstrateNumericPrimitives();

        Console.WriteLine("\n=== [1.2 变量声明、集合表达式与零值体系] ===");
        BasicsDemo.DemonstrateVariablesAndDefaults();

        Console.WriteLine("\n=== [1.3 现代字符串与 UTF-8 内存切片] ===");
        BasicsDemo.DemonstrateStringsAndMemory();
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
        // 映射：对标 Go 的 `...`，原生支持安全插值，杜绝转义混乱
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
        // 1. 显式类型声明（左侧严禁盲盒推导）
        double inferredDouble = 1.0;

        // 现代集合表达式（规范允许，编译期直接布局为连续数组）
        int[] numbers = [1, 2];

        Console.WriteLine($"[推导与集合] Double: {inferredDouble}, Array Length: {numbers.Length}");

        // 2. 确定性常量（编译期直接内联至机器码常量池）
        const double Pi = 3.1415926;
        Console.WriteLine($"[编译期常量] Pi: {Pi}");

        // 3. 显式零值体系 default(T)，拒绝裸 default 隐式推导
        // 映射：对标 Go 的 var a int (0) / var b bool (false)
        int defaultInt = default(int);             // 0
        bool defaultBool = default(bool);           // false
        decimal defaultMoney = default(decimal);     // 0.0m
        string? defaultRef = default(string);       // null

        // 卫语句与平铺判空，消除行内 ?? 压缩语法糖
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

        // 1. 现代字符串插值
        string welcomeMsg = $"Hello, {userName}! Balance: {balance}";
        Console.WriteLine($"[字符串插值] {welcomeMsg}");

        // 2. 原始多行字符串字面量 (Raw String Literals)
        string jsonPayload = """
        {
            "userId": 1001,
            "name": "Alice",
            "roles": ["admin", "developer"],
            "isActive": true
        }
        """;
        Console.WriteLine($"[原始多行 JSON]\n{jsonPayload}");

        // 3. UTF-8 字节切片字面量（Native AOT 核心零分配特性）
        // 映射：直接固化在编译产物的只读常量区（.rdata/.rodata），与 Go 的 []byte("PING") 在数据段原理一致
        ReadOnlySpan<byte> utf8Ping = "PING"u8;
        Console.WriteLine($"[只读 UTF-8 内存切片] Length: {utf8Ping.Length} bytes, First Byte: {utf8Ping[0]}");

        // 4. 原生路径字符串（逐字文本，禁用转义解释）
        string filePath = @"C:\Users\docs\file.txt";
        Console.WriteLine($"[原样路径] {filePath}");
    }
}
