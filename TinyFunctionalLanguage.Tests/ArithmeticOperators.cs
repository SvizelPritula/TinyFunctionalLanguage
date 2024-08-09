using System.Text.Json;

namespace TinyFunctionalLanguage.Tests;

public class ArithmeticOperators
{
    [Theory]
    [InlineData(1, 2)]
    [InlineData(-1, -2)]
    public void Addition_Valid(long a, long b)
    {
        var module = Compiler.Compile($"func add(): int {{ {a} + {b} }}");
        var method = module.GetMethod("add")!.CreateDelegate<Func<long>>();
        Assert.Equal(a + b, method());
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(-1, -2)]
    public void Subtraction_Valid(long a, long b)
    {
        var module = Compiler.Compile($"func sub(): int {{ {a} - {b} }}");
        var method = module.GetMethod("sub")!.CreateDelegate<Func<long>>();
        Assert.Equal(a - b, method());
    }

    [Theory]
    [InlineData(2, 4)]
    [InlineData(2, -4)]
    public void Multiplication_Valid(long a, long b)
    {
        var module = Compiler.Compile($"func mul(): int {{ {a} * {b} }}");
        var method = module.GetMethod("mul")!.CreateDelegate<Func<long>>();
        Assert.Equal(a * b, method());
    }

    [Theory]
    [InlineData(9, 5)]
    [InlineData(10, 5)]
    [InlineData(11, 5)]
    [InlineData(6, -5)]
    public void Division_Valid(long a, long b)
    {
        var module = Compiler.Compile($"func div(): int {{ {a} / {b} }}");
        var method = module.GetMethod("div")!.CreateDelegate<Func<long>>();
        Assert.Equal(a / b, method());
    }

    [Theory]
    [InlineData(9, 5)]
    [InlineData(10, 5)]
    [InlineData(11, 5)]
    [InlineData(-6, 5)]
    public void Remainder_Valid(long a, long b)
    {
        var module = Compiler.Compile($"func div(): int {{ {a} % {b} }}");
        var method = module.GetMethod("div")!.CreateDelegate<Func<long>>();
        Assert.Equal(a % b, method());
    }

    [Theory]
    [InlineData(10)]
    [InlineData(-10)]
    [InlineData(long.MaxValue)]
    public void Negate_Valid(long n)
    {
        var module = Compiler.Compile($"func neg(): int {{ -{n} }}");
        var method = module.GetMethod("neg")!.CreateDelegate<Func<long>>();
        Assert.Equal(-n, method());
    }

    [Fact]
    public void Addition_Overflow()
    {
        var module = Compiler.Compile($"func add(): int {{ {long.MaxValue} + 1 }}");
        var method = module.GetMethod("add")!.CreateDelegate<Func<long>>();
        Assert.Throws<OverflowException>(() => method());
    }

    [Fact]
    public void Subtraction_Overflow()
    {
        var module = Compiler.Compile($"func sub(): int {{ {long.MinValue + 1} - 2 }}");
        var method = module.GetMethod("sub")!.CreateDelegate<Func<long>>();
        Assert.Throws<OverflowException>(() => method());
    }

    [Fact]
    public void Multiplication_Overflow()
    {
        var module = Compiler.Compile($"func mul(): int {{ {1L << 32} * {1L << 32} }}");
        var method = module.GetMethod("mul")!.CreateDelegate<Func<long>>();
        Assert.Throws<OverflowException>(() => method());
    }

    [Fact]
    public void Division_Overflow()
    {
        var module = Compiler.Compile($"func div(): int {{ ({long.MinValue + 1} - 1) / -1 }}");
        var method = module.GetMethod("div")!.CreateDelegate<Func<long>>();
        Assert.Throws<OverflowException>(() => method());
    }

    [Fact]
    public void Negate_Overflow()
    {
        var module = Compiler.Compile($"func neg(): int {{ -({long.MinValue + 1} - 1) }}");
        var method = module.GetMethod("neg")!.CreateDelegate<Func<long>>();
        Assert.Throws<OverflowException>(() => method());
    }

    [Fact]
    public void Division_ByZero()
    {
        var module = Compiler.Compile("func div(): int { 10 / 0 }");
        var method = module.GetMethod("div")!.CreateDelegate<Func<long>>();
        Assert.Throws<DivideByZeroException>(() => method());
    }

    [Fact]
    public void Remainder_ByZero()
    {
        var module = Compiler.Compile("func rem(): int { 10 % 0 }");
        var method = module.GetMethod("rem")!.CreateDelegate<Func<long>>();
        Assert.Throws<DivideByZeroException>(() => method());
    }

    [Fact]
    public void Subtraction_AssociatesLeftToRight()
    {
        var module = Compiler.Compile("func main(): int { 10 - 1 - 2 }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<long>>();
        Assert.Equal(7, method());
    }

    [Fact]
    public void Division_AssociatesLeftToRight()
    {
        var module = Compiler.Compile("func main(): int { 50 / 5 / 2 }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<long>>();
        Assert.Equal(5, method());
    }

    [Fact]
    public void Precedence_MultiplicationBeforeAddition_Left()
    {
        var module = Compiler.Compile("func main(): int { 10 * 10 + 1 }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<long>>();
        Assert.Equal(101, method());
    }

    [Fact]
    public void Precedence_MultiplicationBeforeAddition_Right()
    {
        var module = Compiler.Compile("func main(): int { 1 + 10 * 10 }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<long>>();
        Assert.Equal(101, method());
    }

    [Theory]
    [InlineData('-')]
    [InlineData('*')]
    [InlineData('/')]
    [InlineData('%')]
    public void Binary_DontWorkOnStrings(char op)
    {
        Assert.Throws<LanguageException>(() => Compiler.Compile($"func main(): string {{ \"\" {op} \"\" }}"));
    }

    [Theory]
    [InlineData('+')]
    [InlineData('-')]
    [InlineData('*')]
    [InlineData('/')]
    [InlineData('%')]
    public void Binary_DontWorkOnBools(char op)
    {
        Assert.Throws<LanguageException>(() => Compiler.Compile($"func main(): bool {{ true {op} true }}"));
    }

    [Fact]
    public void Negate_DoesntWorkOnStrings()
    {
        Assert.Throws<LanguageException>(() => Compiler.Compile($"func main(): string {{ - \"\" }}"));
    }

    [Fact]
    public void Negate_DoesntWorkOnBools()
    {
        Assert.Throws<LanguageException>(() => Compiler.Compile($"func main(): bool {{ - true }}"));
    }
}
