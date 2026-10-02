namespace MemoryPointer;

using System;
using System.Globalization;

// 纯静态方法容器，显式声明为 static class，杜绝实例化
public static class BadParse
{
    // 每一帧都会产生巨额 GC 垃圾的传统写法（反面教材演示）
    public static void Parse(string rawData)
    {
        // 灾难：Split 在堆上分配 string[] 数组，并为子字符串分配 3 个全新 string 对象
        string[] parts = rawData.Split(',');

        string symbol = parts[0];
        decimal price = decimal.Parse(parts[1], CultureInfo.InvariantCulture);
        int volume = int.Parse(parts[2], CultureInfo.InvariantCulture);
    }
}

// 展开为主构造函数剥离形态：传统构造函数 + 显式 this 赋值
public readonly record struct ParsedTick
{
    public decimal Price { get; init; }
    public int Volume { get; init; }

    public ParsedTick(decimal price, int volume)
    {
        this.Price = price;
        this.Volume = volume;
    }
}

public static class FastParser
{
    // 核心入参：ReadOnlySpan<char>（只读内存切片，不拷贝字符串，零堆分配）
    public static bool TryParseTick(ReadOnlySpan<char> rawSpan, out ParsedTick result)
    {
        // 显式零值，拒绝裸 default 推导
        result = default(ParsedTick);

        // 1. 寻找第一个逗号
        int firstComma = rawSpan.IndexOf(',');
        if (firstComma == -1)
        {
            return false;
        }

        // 纯指针切片，不产生任何 string 分配
        ReadOnlySpan<char> symbolSpan = rawSpan.Slice(0, firstComma);

        // 2. 截取剩余部分并寻找第二个逗号
        ReadOnlySpan<char> remaining = rawSpan.Slice(firstComma + 1);
        int secondComma = remaining.IndexOf(',');
        if (secondComma == -1)
        {
            return false;
        }

        ReadOnlySpan<char> priceSpan = remaining.Slice(0, secondComma);
        ReadOnlySpan<char> volumeSpan = remaining.Slice(secondComma + 1);

        // 3. 在原始物理切片上直接解析数值（显式传递 InvariantCulture 消除国际化歧义）
        if (!decimal.TryParse(priceSpan, CultureInfo.InvariantCulture, out decimal price))
        {
            return false;
        }

        if (!int.TryParse(volumeSpan, CultureInfo.InvariantCulture, out int volume))
        {
            return false;
        }

        result = new ParsedTick(price, volume);
        return true;
    }
}
