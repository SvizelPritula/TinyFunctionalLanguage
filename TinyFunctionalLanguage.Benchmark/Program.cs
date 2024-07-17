using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace TinyFunctionalLanguage.Benchmark;

public class TflVsCs
{
    private const int N = 80;
    private const string CODE = """
    func main(n: int): int {
        fib(0, 1, n)
    }

    func fib(a: int, b: int, n: int): int {
        if n <= 0 {
            a
        } else {
            fib(b, a + b, n - 1)
        }
    }
    """;

    private readonly Func<long, long> compiled;
    private readonly Func<long, long> reference;

    public TflVsCs()
    {
        Module module = Compiler.Compile(CODE);
        compiled = module.GetMethod("main")!.CreateDelegate<Func<long, long>>();

        reference = Fibonacci;
    }

    [Benchmark]
    public long Tfl() => compiled(N);

    [Benchmark(Baseline = true)]
    public long Cs() => reference(N);

    static long Fibonacci(long n)
    {
        return FibonacciInternal(0, 1, n);
    }

    static long FibonacciInternal(long a, long b, long n)
    {
        if (n <= 0)
            return a;
        else
            return FibonacciInternal(b, a + b, n - 1);
    }
}

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<TflVsCs>();
    }
}
