class Program
{
    static async Task Main()
    {
        static void MyFuncMo7()
        {
            Console.WriteLine("Hello, MyFuncMo7");
        }

        Console.WriteLine("Hello, World!");

        await Task.Delay(1000);

        YourNamespace2.Program.MyFunc();

        await Task.Delay(1000);

        var ppppp = new YourNamespace3.Program();
        ppppp.MyFunc();

        await Task.Delay(1000);

        YourNamespace4.Program.MyFunc();

        await Task.Delay(1000);

        MyFuncMo7();

        await Task.Delay(1000);

        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
    }
}

namespace YourNamespace2
{
    class Program
    {
        public static void MyFunc()
        {
            Console.WriteLine("Hello, YourNamespace2");
        }
    }
}

namespace YourNamespace3
{
    class Program
    {
        public void MyFunc()
        {
            Console.WriteLine("Hello, YourNamespace3");
        }
    }
}

namespace Foo
{
    class Program
    {
        static void Main(string[] args)
        {
            //...
        }
    }
}
