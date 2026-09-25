void HelloWorld()
{
    var now = DateTime.Now;
    Console.WriteLine($"The current time is {now:yyyy-MM-dd HH:mm:ss}");
    string name = "墨七";
    Console.WriteLine($"Hello,{name}!");
    var name2 = "墨七2";
    name2 = name2 + "3";
    Console.WriteLine("Hello," + name2 + "! ");

    BasicsDemo.Program.NumberDemo();
    BasicsDemo.Program.StringDemo();
    // 1.1 ~ 1.3 基础
    BasicsDemo.Program.Run();

}

HelloWorld();

