namespace MyDemo.Modeling;

using System;
using System.Globalization;

// ==========================================
// 4.4 接口与契约定义
// 对标 Go: type Runner interface { Run() string }
// ==========================================
public interface IRunner
{
    string Run();
}

// ==========================================
// 4.1 类（Class）：显式字段、传统构造函数与属性
// ==========================================
public class NetworkClient : IRunner
{
    // 显式私有成员，拒绝主构造函数的隐式黑盒捕获
    private readonly string endpoint;
    private readonly int timeoutMs;

    public required string AuthToken { get; init; }
    public bool EnableCompression { get; set; } = true;

    // 传统显式构造函数，参数严格绑定至 this
    public NetworkClient(string endpoint, int timeoutMs)
    {
        this.endpoint = endpoint;
        this.timeoutMs = timeoutMs;
    }

    public string Run()
    {
        // 访问成员强制携带 this.，静态格式化携带类名
        return $"[Client] Connected to {this.endpoint} (Timeout: {this.timeoutMs}ms, Token: {this.AuthToken})";
    }
}

// ==========================================
// 4.2 数据实体：record（展开为主构造函数剥离形态）
// ==========================================
public record UserProfile
{
    public int Id { get; init; }
    public string DisplayName { get; init; }
    public string Role { get; init; }

    // 显式传统构造函数
    public UserProfile(int id, string displayName, string role)
    {
        this.Id = id;
        this.DisplayName = displayName;
        this.Role = role;
    }
}

// ==========================================
// 4.3 结构体（struct）：纯栈上高性能值对象
// ==========================================
public readonly struct Point
{
    public int X { get; init; }
    public int Y { get; init; }

    // 显式传统构造函数
    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    // 拒绝隐式 new()，显式写明返回的具体类型 Point，访问属性带 this.
    public Point Translate(int dx, int dy)
    {
        return new Point(this.X + dx, this.Y + dy);
    }
}

// 可变结构体（反面教材演示：用于验证栈内存深拷贝）
public struct MutableCoordinate
{
    public int Latitude { get; set; }
    public int Longitude { get; set; }

    public MutableCoordinate(int latitude, int longitude)
    {
        this.Latitude = latitude;
        this.Longitude = longitude;
    }
}

// ==========================================
// 运行调度与验证
// ==========================================
public static class ModelingDemo
{
    public static void Run()
    {
        Console.WriteLine("=== [4.1 显式 Class 与构造初始化] ===");
        ModelingDemo.DemonstrateClassAndInit();

        Console.WriteLine("\n=== [4.2 记录类型 Record 与不可变突变 (with)] ===");
        ModelingDemo.DemonstrateRecords();

        Console.WriteLine("\n=== [4.3 结构体与堆栈内存物理差异] ===");
        ModelingDemo.DemonstrateStructVsClass();
    }

    public static void Execute<T>(T runner) where T : IRunner
    {
        string result = runner.Run();
        Console.WriteLine($"执行结果: {result}");
    }

    private static void DemonstrateClassAndInit()
    {
        // 允许 var：右侧显式存在 new NetworkClient
        var client = new NetworkClient("https://api.internal:8443", 5000)
        {
            AuthToken = "Bearer sk_sec_9999",
            EnableCompression = false
        };

        // 静态方法调用必须显式携带类名
        ModelingDemo.Execute(client);

        Console.WriteLine($"[Concrete Direct Call] {client.Run()}");
    }

    private static void DemonstrateRecords()
    {
        var admin = new UserProfile(1001, "Arch-Dev", "SuperAdmin");
        var adminClone = new UserProfile(1001, "Arch-Dev", "SuperAdmin");

        Console.WriteLine($"Record Print: {admin}");

        // 1. 值相等比对（Value Equality）
        bool isValueEqual = (admin == adminClone);
        // 静态方法严格携带宿主 object
        bool isRefEqual = object.ReferenceEquals(admin, adminClone);
        Console.WriteLine($"Values Equal: {isValueEqual}, References Equal: {isRefEqual}");

        // 2. 非破坏性突变 (with 表达式)：左侧禁止盲盒 var，必须显式声明类型
        UserProfile editor = admin with { Role = "Editor" };
        Console.WriteLine($"Mutated Copy: {editor}");
        Console.WriteLine($"Original Unchanged: {admin}");
    }

    private static void DemonstrateStructVsClass()
    {
        // 场景 1：栈内存深拷贝验证
        var originalCoord = new MutableCoordinate(10, 20);
        ModelingDemo.ModifyStruct(originalCoord);
        Console.WriteLine($"Struct Original after Modify: Lat={originalCoord.Latitude.ToString(CultureInfo.InvariantCulture)} (未被外部修改)");

        // 场景 2：只读结构体（零 GC 开销）
        var p1 = new Point(5, 10);
        // 方法返回类型不明显，左侧禁止盲盒 var，显式声明 Point
        Point p2 = p1.Translate(1, 2);
        Console.WriteLine($"Point translated: X={p2.X.ToString(CultureInfo.InvariantCulture)}, Y={p2.Y.ToString(CultureInfo.InvariantCulture)}");
    }

    private static void ModifyStruct(MutableCoordinate coord)
    {
        // 仅修改了当前函数栈帧里的副本（8 字节值拷贝），调用方无感知
        coord.Latitude = 999;
    }
}
