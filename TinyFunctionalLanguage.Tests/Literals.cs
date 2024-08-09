namespace TinyFunctionalLanguage.Tests;

public class Literals
{
    [Theory]
    [InlineData(42)]
    [InlineData(-42)]
    [InlineData(0)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue + 1)]
    public void IntLiteral_Valid(long value)
    {
        var module = Compiler.Compile($"func main(): int {{ {value} }}");
        var method = module.GetMethod("main")!.CreateDelegate<Func<long>>();
        Assert.Equal(value, method());
    }

    [Fact]
    public void IntLiteral_LeadingZeros()
    {
        var module = Compiler.Compile("func main(): int { 012 }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<long>>();
        Assert.Equal(12, method());
    }

    [Fact]
    public void IntLiteral_Underscores()
    {
        var module = Compiler.Compile("func main(): int { 1_000_000 }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<long>>();
        Assert.Equal(1000000, method());
    }

    [Fact]
    public void IntLiteral_Overflow()
    {
        Assert.Throws<LanguageException>(() =>
        {
            Compiler.Compile($"func main(): int {{ {((Int128)long.MaxValue) + 1} }}");
        });
    }

    [Fact]
    public void IntLiteral_Underflow()
    {
        Assert.Throws<LanguageException>(() =>
        {
            Compiler.Compile($"func main(): int {{ {(Int128)long.MinValue} }}");
        });
    }

    [Fact]
    public void IntLiteral_BadChars()
    {
        Assert.Throws<LanguageException>(() =>
        {
            Compiler.Compile("func main(): int { 123a }");
        });
    }
}