using System.Text.Json;

using TinyFunctionalLanguage.Errors;
using TinyFunctionalLanguage;

try
{
    var code = Console.In.ReadToEnd();
    var module = Compiler.Compile(code);

    var func = module.GetMethod("main")!;
    object? result = func.Invoke(null, []);

    foreach (var field in result!.GetType().GetFields())
        Console.WriteLine($"{field.Name} = {field.GetValue(result)}");
}
catch (LanguageException ex)
{
    Console.WriteLine($"{ex.Span}: {ex.Message}");
}
