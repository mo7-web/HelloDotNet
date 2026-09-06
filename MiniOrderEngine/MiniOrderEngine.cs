namespace MiniOrderEngine;

// // ===================
// // 1. 类型定义区域 （感受 struct、class、enum 的定义差异）
// // ===================

// // 枚举1 ： 普通单选枚举（指定物理底层为 byte，仅占1字节）
// // 对应 Go：type OrderSide byte；const （Buy OrderSide = iota；Sell）

public enum OrderSide : byte
{
  Buy,
  Sell,
}

// 枚举2：单位标志多选枚举 （[Flags]，每个选项占一个独立的Bit）
// 对应 Go： 1<<0,1<<1

[Flags]
public enum OrderFlags : byte
{
  None = 0,
  MakerOnly = 1 << 0, // 0001: 只做挂单
  ReduceOnly = 1 << 1, // 0010: 只做平仓
  IsPostOnly = 1 << 2, // 0100: 失败立即撤销
}

// 核心类型 A: 值类型 struct （零堆分配，栈上数据摘块）
// 对应Go 的普通 strict

public struct MarketTick
{
  public long Timestamp;
  public decimal Price; // 使用 高精度十进制数
  public double UnsafePrice; // 故意引入 IEEE 浮点数用于对比

  public MarketTick(long ts, decimal p, double up)
  {
    Timestamp = ts;
    Price = p;
    UnsafePrice = up;
  }
}

// 核心类型 B：引用类型 class（数据强制存放在托管堆，变量存的是指针）
// 类似于 Go 中的 *Account 语义
public class Account
{
  public string AccountId; // 非空保证
  public decimal Balance; // 账户余额

  public Account(string accountId, decimal initialBalance)
  {
    AccountId = accountId;
    Balance = initialBalance;
  }
}

// 辅助工具类：用于演示 using 自动释放资源（实现 IDisposable 接口）
// 对应 Go 的 defer file.Close() 语义，但在 C# 里作用域更加精准
public class AuditLogScope : IDisposable
{
  private readonly string _scopeName;

  public AuditLogScope(string scopeName)
  {
    _scopeName = scopeName;
    Console.WriteLine($"[审计开始] 进入操作域: {_scopeName}");
  }

  // 离开 using 代码块时，CLR/操作系统强制自动触发此函数
  public void Dispose()
  {
    Console.WriteLine($"[审计结束] 安全退出操作域: {_scopeName}");
  }
}
