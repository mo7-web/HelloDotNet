// static void MyFuncMo7()
// {
//     Console.WriteLine("Hello1111, MyFuncMo7");
// }

// MyFuncMo7(); // 如果存在顶层语句，则顶层语句中会被当作入口

namespace NameSpace1
{
  class Program
  {
    static async Task Main()
    {
      static void MyFuncMo7()
      {
        Console.WriteLine("Hello,NameSpace1, MyFuncMo7");
      }

      Console.WriteLine("Hello, World!");

      await Task.Delay(1000);

      YourNamespace2.Program.MyFunc(); // static 方法可以直接调用

      await Task.Delay(1000);

      var ppppp = new YourNamespace3.Program(); // 非 static 方法需要 new 一下
      ppppp.MyFunc();

      await Task.Delay(1000);

      YourNamespace4.Program.MyFunc(); //只要存在于同一个项目中，就可以直接调用

      await Task.Delay(1000);

      MyFuncMo7();

      await Task.Delay(1000);

      Console.WriteLine("按任意键退出...");
      Console.ReadKey();
    }
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

// namespace Foo
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             //...
//             Console.WriteLine("Foo, Program,Main");  // 取消注释，则会优先执行
//         }
//     }
// }
