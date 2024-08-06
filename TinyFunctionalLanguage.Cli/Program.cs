using TinyFunctionalLanguage.Errors;
using TinyFunctionalLanguage;
using System.Reflection;

try
{
    var code = Console.In.ReadToEnd();
    var module = Compiler.Compile(code);

    var func = module.GetMethod("main")!;
    object? result = func.Invoke(null, []);

    Console.WriteLine(result);

    foreach (var field in result!.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        Console.WriteLine($"{field.Name} = {field.GetValue(result)}");
}
catch (LanguageException ex)
{
    Console.WriteLine($"{ex.Span}: {ex.Message}");
}
