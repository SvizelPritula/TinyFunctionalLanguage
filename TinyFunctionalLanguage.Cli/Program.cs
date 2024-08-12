using TinyFunctionalLanguage;
using System.Reflection;

var cliArgs = CliArgs.ParseOrExit(args);
var code = File.ReadAllText(cliArgs.Program);

Module module;

try
{
    module = Compiler.Compile(code);
}
catch (LanguageException ex)
{
    foreach (var error in ex.Errors)
        Console.Error.WriteLine($"{error.Span}: {error.Message}");

    return 1;
}

var method = module.GetMethod("main");

if (method is null)
{
    Console.Error.WriteLine("There is no method called main.");
    return 1;
}

var parsedParams = PrepareFuncArgsOrExit(method, cliArgs.Args);
var result = method.Invoke(null, parsedParams);
DumpObject(result);

return 0;

static void DumpObject(object? value, int indent = 1)
{
    if (value is null)
    {
        Console.WriteLine("null");
        return;
    }
    else if (value is ValueTuple)
    {
        Console.WriteLine("unit");
        return;
    }

    var indentStr = new string(' ', indent * 2);
    Console.WriteLine(value);

    foreach (var field in value!.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
    {
        var subvalue = field.GetValue(value);

        Console.Write(indentStr);
        Console.Write($"{field.Name} = ");

        DumpObject(subvalue, indent + 1);
    }
}

static object?[] PrepareFuncArgsOrExit(MethodInfo method, string?[] args)
{
    var infos = method.GetParameters();

    if (args.Length != infos.Length)
    {
        Console.Error.WriteLine($"The main function takes {infos.Length} arguments, but {args.Length} were supplied.");
        Environment.Exit(1);
    }

    return infos.Zip(args)
        .Select(p =>
        {
            var (info, arg) = p;

            try
            {
                return Convert.ChangeType(p.Second, p.First.ParameterType);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to parse parameter {info.Name}: {ex.Message}");
                Environment.Exit(1);

                return null; // Unreachable
            }
        })
        .ToArray();
}

record struct CliArgs(string Program, string[] Args)
{
    public static CliArgs ParseOrExit(string[] args)
    {
        if (args.Length < 1)
        {
            var rawArgs = Environment.GetCommandLineArgs();
            var programName = rawArgs.Length >= 1 ? rawArgs[0] : "tfl";

            Console.Error.WriteLine($"Usage: {programName} FILE [ARGS]...");
            Environment.Exit(2);
        }

        return new(args[0], args[1..]);
    }
}
