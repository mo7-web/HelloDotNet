namespace MyDemo.Modeling;

using System.Globalization;

// ==========================================
// 4.4 接口与契约定义
// ==========================================
// [Go: type Runner interface { Run() string }] / [TS: interface IRunner { run(): string }]
public interface IRunner
{
    string Run();
}

// ==========================================
// 4.1 类（Class）、主构造函数、required 与 init
// ==========================================
// 主构造函数 (Primary Constructor): endpoint 和 timeoutMs 在整类作用域内有效
// [TS: class NetworkClient(private endpoint: string) { ... }]
public class NetworkClient(string endpoint, int timeoutMs) : IRunner
{
    // 现代属性：只读初始化 init（构建后不可篡改）与 required（实例化时强制必填）
    // 编译器杜绝“未初始化就使用”的经典空指针缺陷
    public required string AuthToken { get; init; }

    // 普通自动读写属性
    public bool EnableCompression { get; set; } = true;

    // 显式实现接口方法
    public string Run() =>
        $"[Client] Connected to {endpoint} (Timeout: {timeoutMs.ToString(CultureInfo.InvariantCulture)}ms, Token: {AuthToken})";
}

// ==========================================
// 4.2 数据实体神器：record (不可变 DTO 的终极形态)
// ==========================================
// 一行代码定义：自动拥有属性、构造器、解构器、ToString 格式化和基于值的相等性比较
// [Go: type UserProfile struct { ... } 但自带深度值比对与 ToString]
public record UserProfile(int Id, string DisplayName, string Role);

// ==========================================
// 4.3 结构体（struct）与内存本质（栈 vs 堆）
// ==========================================
// 推荐最佳实践：永远优先使用 readonly struct，杜绝可变结构体的防御性拷贝陷阱
// [Go: type Point struct { X, Y int } 传值语义]
public readonly struct Point(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;

    // 结构体表达式主体方法
    public Point Translate(int dx, int dy) => new(X + dx, Y + dy);
}

// 可变结构体（反面教材演示：用于验证值传递深拷贝）
public struct MutableCoordinate
{
    public int Latitude { get; set; }
    public int Longitude { get; set; }
}

// ==========================================
// 运行调度与验证
// ==========================================
public static class ModelingDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [4.1 现代 Class 与主构造函数] ===");
        DemonstrateClassAndInit();

        Console.WriteLine("\n=== [4.2 记录类型 Record 与不可变突变 (with)] ===");
        DemonstrateRecords();

        Console.WriteLine("\n=== [4.3 结构体与堆栈内存物理差异] ===");
        DemonstrateStructVsClass();
    }

    private static void DemonstrateClassAndInit()
    {
        // 1. 具体类型实例化与属性校验
        var client = new NetworkClient("https://api.internal:8443", 5000)
        {
            AuthToken = "Bearer sk_sec_9999",
            EnableCompression = false
        };

        // client.AuthToken = "change"; // 编译错误！init 属性仅在构建阶段允许赋值

        IRunner runner = client; // 向上转型为接口契约
        Console.WriteLine(runner.Run());
    }

    private static void DemonstrateRecords()
    {
        var admin = new UserProfile(1001, "Arch-Dev", "SuperAdmin");
        var adminClone = new UserProfile(1001, "Arch-Dev", "SuperAdmin");

        // 1. 自动重写 ToString()：格式化打印极其友好，无需手写
        Console.WriteLine($"Record Print: {admin}");

        // 2. 基于“值相等（Value Equality）”：两个独立堆对象，只要内部数据一致，== 结果就是 true！
        // [TS / JS 的引用比对会是 false；Go 纯 struct 比对是 true]
        bool isValueEqual = (admin == adminClone);
        bool isRefEqual = ReferenceEquals(admin, adminClone);
        Console.WriteLine($"Values Equal: {isValueEqual}, References Equal: {isRefEqual}");

        // 3. 非破坏性突变 (Non-destructive Mutation via 'with')
        // [JS: const editor = { ...admin, Role: "Editor" }]
        // [Go 需要显式拷贝结构体并改属性]
        var editor = admin with { Role = "Editor" };
        Console.WriteLine($"Mutated Copy: {editor}");
        Console.WriteLine($"Original Unchanged: {admin}");
    }

    private static void DemonstrateStructVsClass()
    {
        // 场景 1：验证 struct 默认是“栈上传值拷贝（Pass-by-value / Deep Copy）”
        var originalCoord = new MutableCoordinate { Latitude = 10, Longitude = 20 };
        ModifyStruct(originalCoord); // 发生栈内存浅拷贝，原变量毫发无损
        Console.WriteLine($"Struct Original after Modify: Lat={originalCoord.Latitude.ToString(CultureInfo.InvariantCulture)} (未被外部修改)");

        // 场景 2：只读结构体（零 GC 开销的高性能值对象）
        var p1 = new Point(5, 10);
        var p2 = p1.Translate(1, 2);
        Console.WriteLine($"Point translated: X={p2.X.ToString(CultureInfo.InvariantCulture)}, Y={p2.Y.ToString(CultureInfo.InvariantCulture)}");
    }

    private static void ModifyStruct(MutableCoordinate coord)
    {
        coord.Latitude = 999; // 仅修改了当前函数栈帧里的副本，调用方无感知
    }
}
