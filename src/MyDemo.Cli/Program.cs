using System;
// 导入 MyDemo.Core 类库中的命名空间
using MyDemo.Core;

using MyDemo.Cli.MyMvvm;

// 调用类库提供的方法
string message = Greeter.SayHello("开发者");
Console.WriteLine(message);


Demo demo = new Demo("Alice", 30);
demo.DisplayInfo();
