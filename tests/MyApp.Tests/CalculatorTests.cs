namespace MyApp.Tests;

[TestClass]
public sealed class CalculatorTests
{
    private readonly Calculator _calculator = new();

    [TestMethod]
    public void Add_PositiveNumbers_ReturnsCorrectSum()
    {
        // Act
        var result = _calculator.Add(10, 20);

        // Assert
        Assert.AreEqual(30, result);
    }

    [TestMethod]
    public void Divide_ByZero_ThrowsException()
    {
        // Assert
        Assert.ThrowsException<DivideByZeroException>(() => _calculator.Divide(10, 0));
    }
}
