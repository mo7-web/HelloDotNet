namespace MemoryPointer;

public struct BadParse
{
  // 从网络接收到一段行情文本数据："BTCUSDT,65432.10,1500"（交易对、价格、数量）。
  // 每一帧都会产生巨额 GC 垃圾的传统写法：
  public void Parse(string rawData)
  {
    // 灾难 1：Split 会在堆上分配一个 string[] 数组
    // 灾难 2：Split 还会为切分出来的 3 个子片段在堆上分配 3 个全新的 string 对象！
    string[] parts = rawData.Split(',');

    string symbol = parts[0];
    decimal price = decimal.Parse(parts[1]);
    int volume = int.Parse(parts[2]);
    // 哪怕后续啥都不干，这一次解析就已经在堆上丢弃了 4 个垃圾对象！
  }
}

public readonly record struct ParsedTick(decimal Price, int Volume);

public class FastParser
{
  // 核心入参：ReadOnlySpan<char>（只读内存切片，不拷贝字符串）
  public static bool TryParseTick(ReadOnlySpan<char> rawSpan, out ParsedTick result)
  {
    result = default;

    // 1. 寻找第一个逗号的位置（硬件 SIMD 指令加速寻找）
    int firstComma = rawSpan.IndexOf(',');
    if (firstComma == -1)
    {
      return false;
    }

    // 切片获取 Symbol，纯粹是指针偏移，无任何堆内存分配！
    ReadOnlySpan<char> symbolSpan = rawSpan.Slice(0, firstComma);

    // 2. 截取剩余部分
    ReadOnlySpan<char> remaining = rawSpan.Slice(firstComma + 1);
    int secondComma = remaining.IndexOf(',');
    if (secondComma == -1)
    {
      return false;
    }

    ReadOnlySpan<char> priceSpan = remaining.Slice(0, secondComma);
    ReadOnlySpan<char> volumeSpan = remaining.Slice(secondComma + 1);

    // 3. 直接在 Span 切片上进行数字解析（.NET 核心库对 Span 有全套原生支持）
    // 整个解析过程不需要将 Span 转换回 string，直接在原始字符物理地址上扫描！
    if (!decimal.TryParse(priceSpan, out decimal price))
      return false;

    if (!int.TryParse(volumeSpan, out int volume))
    {
      return false;
    }

    result = new ParsedTick(price, volume);
    return true;
  }
}
