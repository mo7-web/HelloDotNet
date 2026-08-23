/* 
Console.WriteLine("The current time is " + DateTime.Now);

string name = "墨七";

Console.WriteLine("Hello, " + name + "! Welcome to the program.");

var name2 = "墨七2";

name2 = "3";

Console.WriteLine("Hello2, " + name2 + "! Welcome to the program.");

 */


namespace YourNamespace
{
  class Program
  {
    static void Main()
    {
      Console.WriteLine("Hello, World! YourNamespace2");

      var ppppp = new YourNamespace3.Program();
      ppppp.MyFunc();


      YourNamespace2.Program.MyFunc();


      YourNamespace4.Program.MyFunc();
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


