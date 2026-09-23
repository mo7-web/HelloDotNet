namespace MyApp.Tests;


/*
dotnet clean tests/MyApp.Tests

执行测试
dotnet test

运行测试函数
dotnet run --project tests/MyApp.Tests

*/

[TestClass]
public sealed class CalculatorTests
{

    [TestMethod]
    public void HelloTest()
    {
        Console.WriteLine("测试,HelloTest");
        // Assert.Fail("主动触发失败以观察输出");
    }

    // dotnet test --filter Name=HelloTest

    [TestMethod]
    public void HelloTest2()
    {

        Console.WriteLine("测试,HelloTest2");

    }
}


