using System.Text.Json;

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
    public void IntLiteral_MultipleUnderscores()
    {
        var module = Compiler.Compile("func main(): int { 1_____2_____3 }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<long>>();
        Assert.Equal(123, method());
    }

    [Fact]
    public void IntLiteral_TrailingUnderscores()
    {
        var module = Compiler.Compile("func main(): int { 1_____ }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<long>>();
        Assert.Equal(1, method());
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

    [Fact]
    public void StringLiteral_Simple()
    {
        var module = Compiler.Compile("func main(): string { \"hello\" }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<string>>();
        Assert.Equal("hello", method());
    }

    [Fact]
    public void StringLiteral_Newline()
    {
        var module = Compiler.Compile("func main(): string { \"\n\" }");
        var method = module.GetMethod("main")!.CreateDelegate<Func<string>>();
        Assert.Equal("\n", method());
    }

    [Theory]
    [InlineData(@"\\")]
    [InlineData(@"\/")]
    [InlineData(@"\""")]
    [InlineData(@"\b")]
    [InlineData(@"\f")]
    [InlineData(@"\n")]
    [InlineData(@"\r")]
    [InlineData(@"\t")]
    [InlineData(@"\u0123")]
    [InlineData(@"\u01aa")]
    [InlineData(@"\\\\\\\\")]
    public void StringLiteral_JsonEscapes(string escaped)
    {
        escaped = $"\"{escaped}\"";
        var expected = JsonSerializer.Deserialize<string>(escaped);

        var module = Compiler.Compile($"func main(): string {{ {escaped} }}");
        var method = module.GetMethod("main")!.CreateDelegate<Func<string>>();

        Assert.Equal(expected, method());
    }

    [Fact]
    public void StringLiteral_EscapedTerminator()
    {
        Assert.Throws<LanguageException>(() =>
        {
            Compiler.Compile(@"func main(): string { ""\"" }");
        });
    }

    [Fact]
    public void StringLiteral_UnknownEscape()
    {
        Assert.Throws<LanguageException>(() =>
        {
            Compiler.Compile(@"func main(): string { ""\a"" }");
        });
    }

    [Fact]
    public void StringLiteral_EscapeBeforeEndOfFile()
    {
        Assert.Throws<LanguageException>(() =>
        {
            Compiler.Compile(@"func main(): string { ""\");
        });
    }

    [Fact]
    public void StringLiteral_UnicodeEscapeBeforeEndOfFile()
    {
        Assert.Throws<LanguageException>(() =>
        {
            Compiler.Compile(@"func main(): string { ""\u1");
        });
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void BoolLiteral_Valid(string name, bool value)
    {
        var module = Compiler.Compile($"func main(): bool {{ {name} }}");
        var method = module.GetMethod("main")!.CreateDelegate<Func<bool>>();
        Assert.Equal(value, method());
    }
}
