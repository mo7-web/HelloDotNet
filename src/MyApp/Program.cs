namespace MyApp;

public class Program
{
    static void Main(string[] args)
    {
        //...
        Console.WriteLine("这里是，MyApp");
    }

    // 1. 必须声明为 public，允许外部项目引用
    public static string GetGreeting(string name)
    {

        Console.WriteLine("你好，{0}！", name);

        return $"你好，{name}！";
    }
}
