namespace MiniOrderEngine;

using System;

// 1. 普通单选枚举：显式指定底层物理存储为 byte（仅占 1 字节）
// 对标 Go: type OrderSide byte; const (Buy OrderSide = iota; Sell)
public enum OrderSide : byte
{
    Buy,
    Sell
}

// 2. 位掩码多选枚举：每个选项占一个独立的 Bit
// 对标 Go: 1 << 0, 1 << 1
[Flags]
public enum OrderOptions : byte
{
    None = 0,
    MakerOnly = 1 << 0, // 0001: 只做挂单
    ReduceOnly = 1 << 1, // 0010: 只做平仓
    IsPostOnly = 1 << 2  // 0100: 失败立即撤销
}

// 核心类型 A：值类型 struct（零堆分配，栈上传值语义）
public struct MarketTick
{
    public long Timestamp { get; set; }
    public decimal Price { get; set; }
    public double UnsafePrice { get; set; }

    // 显式传统构造函数，成员访问一律携带 this.
    public MarketTick(long ts, decimal p, double up)
    {
        this.Timestamp = ts;
        this.Price = p;
        this.UnsafePrice = up;
    }
}

// 核心类型 B：引用类型 class（数据存放在托管堆，变量存的是指针）
// 对标 Go: *Account 语义
public class Account
{
    public string AccountId { get; set; }
    public decimal Balance { get; set; }

    public Account(string accountId, decimal initialBalance)
    {
        this.AccountId = accountId;
        this.Balance = initialBalance;
    }
}

// 辅助工具类：演示 using 确定性释放资源（实现 IDisposable 接口）
// 对标 Go: defer cleanup()
public class AuditLogScope : IDisposable
{
    private readonly string scopeName;

    public AuditLogScope(string scopeName)
    {
        this.scopeName = scopeName;
        Console.WriteLine($"[审计开始] 进入操作域: {this.scopeName}");
    }

    public void Dispose()
    {
        Console.WriteLine($"[审计结束] 安全退出操作域: {this.scopeName}");
        GC.SuppressFinalize(this);
    }
}
