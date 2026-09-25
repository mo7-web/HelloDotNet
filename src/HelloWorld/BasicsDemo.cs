namespace MyDemo.Basics;

public class BasicsDemo
{

    public static void Run()
    {
        // ==========================================
        // 1.1 基础数值类型与字面量后缀
        // ==========================================

        Console.WriteLine("=== [1.1~1.3 基元与字符串演练] ===");

        NumberDemo();
        StringDemo();

        // 整型家族
        int standardInt = 42;                 // [Go: int32] / [JS: number] 默认32位有符号整型
        long largeInt = 9_000_000_000L;       // [Go: int64] / [JS: bigint] 64位整型，后缀 L，下划线为数字分隔符
        uint unsignedInt = 100U;              // [Go: uint32] 无符号整型，后缀 U
        ulong unsignedLarge = 200UL;          // [Go: uint64] 无符号64位，后缀 UL

        Console.WriteLine($"[整型演示] Int: {standardInt}, Long: {largeInt}, UInt: {unsignedInt}, ULong: {unsignedLarge}");

        // 浮点与高精度（重点：涉及金额绝不用 double/float）
        double defaultDouble = 3.1415926535;  // [Go: float64] / [JS: number] 默认浮点类型（IEEE 754 双精度）
        float singleFloat = 3.14f;            // [Go: float32] 单精度浮点，强制后缀 f
        decimal exactMoney = 199.99m;         // [Go: shopspring/decimal] 128位定点数，绝无浮点精度丢失，强制后缀 m

        Console.WriteLine($"[浮点演示] Double: {defaultDouble}, Float: {singleFloat}, Decimal: {exactMoney}");

        // 布尔与字符
        bool isEnabled = true;                // [Go: bool] / [JS: boolean]
        char letter = 'A';                    // [Go: rune] / [JS: string] 16位 Unicode 字符，单引号包裹

        Console.WriteLine($"[布尔与字符演示] Bool: {isEnabled}, Char: {letter}");

        // ==========================================
        // 1.2 变量声明、类型推导与零值体系
        // ==========================================

        // 静态推导 (var)
        var inferredDouble = 1.0;             // [Go: v := 1.0] [TS: let v = 1.0] 编译期推导为 double
        var inferredList = new int[] { 1, 2 }; // 编译期推导为 int[]，推导后不可赋给其他类型

        Console.WriteLine($"[类型推导演示] Inferred Double: {inferredDouble}, Inferred List Length: {inferredList.Length}");

        // 零值机制 (default)
        int defaultInt = default;             // [Go: var a int 产生的 0] 零值: 0
        bool defaultBool = default;           // [Go: var b bool 产生的 false] 零值: false
        string? defaultRef = default;         // [Go: nil] / [JS: null] 引用类型的零值: null

        Console.WriteLine($"[零值演示] Int default: {defaultInt}, Bool default: {defaultBool}, String default: {defaultRef ?? "NULL"}");

        decimal defaultMoney = default;       // 零值: 0.0m

        Console.WriteLine($"[零值演示] Decimal default: {defaultMoney}");

        // 常量定义
        const double Pi = 3.1415926;          // [Go: const Pi = 3.1415926] 编译期常量，直接内联到机器码中

        Console.WriteLine($"[常量演示] Pi: {Pi}");

        // ==========================================
        // 1.3 现代字符串核心操作与 Native AOT 特性
        // ==========================================

        string userName = "Alice";

        // 1. 传统字符串插值
        // [JS: `Hello, ${userName}!`] / [Go: fmt.Sprintf("Hello, %s!", userName)]
        string welcomeMsg = $"Hello, {userName}! Balance: {exactMoney:C2}"; // :C2 为货币格式化

        // 2. 原始字符串字面量 (Raw String Literals) - C# 11+
        // [Go: 反引号 `{"key": "value"}`] / [JS: 模版字符串无转义版]
        // 规则：至少 3 个双引号开头和结尾。内容中的双引号无需转义，并且自动移除以末行 """ 对齐的前导空格。
        string jsonPayload = """
    {
        "userId": 1001,
        "name": "Alice",
        "roles": ["admin", "developer"],
        "isActive": true
    }
    """;

        // 3. UTF-8 字节字面量 (Native AOT 高性能关键)
        // [Go: []byte("PING")] - C# 直接在编译期生成只读内存切片，无堆内存分配、无 UTF-16 转码开销
        ReadOnlySpan<byte> utf8Ping = "PING"u8;

        // ==========================================
        // 控制台输出验证
        // ==========================================
        Console.WriteLine($"[原始多行 JSON]\n{jsonPayload}");
        Console.WriteLine($"[UTF-8 字节切片长度] Length: {utf8Ping.Length}");
    }

    private static void NumberDemo()
    {
        int population = 67_000_000;
        long distance = 384_400_000L;
        short temperature = -40;
        byte red = 255;

        double pi = 3.141592653589793;
        float gravity = 9.81f;
        decimal price = 19.99m;
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

    private static void StringDemo()
    {
        char newline = '\n';
        char unicode = '\u0041'; // 'A'

        var greeting = "Hello, World!";
        var dec = 42;

        string message = $"Found {dec} items"; // interpolated string
        string path = @"C:\Users\docs\file.txt"; // verbatim string
        string json = """
    { "name": "Alice", "age": 30 }
    """; // raw string literal
        string raw = $"""
    Found {dec} items in "{greeting}"
    """; // raw + interpolated

        Console.WriteLine(
          $"""
    newline  {newline}
    unicode  {unicode}
    greeting {greeting}
    dec      {dec}
    message  {message}
    path     {path}
    json     {json}
    raw      {raw}
    """
        );
    }

}
