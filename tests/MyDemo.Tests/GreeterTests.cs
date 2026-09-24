using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDemo.Core;

namespace MyDemo.Tests;

/*

# 1. 日常快速运行测试（JIT 模式，毫秒级反馈）
dotnet test

# 2. 原生 AOT 模式全链路真机测试（模拟无 JIT 裁剪环境）
dotnet test tests/MyDemo.Tests/MyDemo.Tests.csproj -p:AotTest=true

*/

[TestClass]
public sealed class GreeterTests
{
    [TestMethod]
    public void SayHelloWithValidNameShouldReturnExpectedMessage()
    {
        // 1. Arrange（准备测试上下文）
        string name = "墨七";

        // 2. Act（调用被测逻辑）
        string actualMessage = Greeter.SayHello(name);

        // 3. Assert（断言预期结果）
        Assert.IsNotNull(actualMessage);
        Assert.IsTrue(actualMessage.Contains("墨七"), "返回值中必须包含传入的姓名");
        Assert.AreEqual("[Core 类库响应] 你好，墨七！来自 .NET 10 + C# 14 原生 AOT 类库。", actualMessage);
    }
}
