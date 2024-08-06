using TinyFunctionalLanguage.Errors;
using TinyFunctionalLanguage;
using System.Reflection;

try
{
    var code = Console.In.ReadToEnd();
    var module = Compiler.Compile(code);

    var func = module.GetMethod("main")!;
    object? result = func.Invoke(null, []);

    Dump(result);
}
catch (LanguageException ex)
{
    Console.WriteLine($"{ex.Span}: {ex.Message}");
}

void Dump(object? value, int indent = 1)
{
    if (value is null)
    {
        Console.WriteLine("null");
        return;
    }

    var indentStr = new string(' ', indent * 2);
    Console.WriteLine(value);

    foreach (var field in value!.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
    {
        var subvalue = field.GetValue(value);

        Console.Write(indentStr);
        Console.Write($"{field.Name} = ");

        Dump(subvalue, indent + 1);
    }
}
