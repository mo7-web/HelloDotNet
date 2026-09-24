namespace MyDemo.Core;

public static class Greeter
{
    // 提供一个简单的静态方法供外部调用
    public static string SayHello(string name)
    {
        return $"[Core 类库响应] 你好，{name}！来自 .NET 10 + C# 14 原生 AOT 类库。";
    }
}
