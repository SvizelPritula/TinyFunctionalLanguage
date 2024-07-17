using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace TinyFunctionalLanguage.Benchmark;

public class TflVsCs
{
    private const int N = 80;
    private const string CODE = """
    func main(n: int): int {
        let a = 0;
        let b = 1;

        while n > 0 {
            let sum = a + b;

            a = b;
            b = sum;

            n = n - 1;
        };

        a
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
        long a = 0;
        long b = 1;

        while (n > 0)
        {
            long sum = a + b;

            a = b;
            b = sum;

            n--;
        };

        return a;
    }
}

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<TflVsCs>();
    }
}
