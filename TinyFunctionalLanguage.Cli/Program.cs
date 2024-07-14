using System.Text.Json;
using System.Text.Json.Serialization;

using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Errors;
using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Types;
using TinyFunctionalLanguage.CodeGen;

try
{
    var code = Console.In.ReadToEnd();
    var ast = Parser.Parse(new Tokenizer(code));
    BindingPass.Run(ast);
    TypeInferencePass.Run(ast);

    Action func = CodeGen.Compile(ast);
    func();
}
catch (LanguageException ex)
{
    Console.WriteLine($"{ex.Span}: {ex.Message}");
}

#pragma warning disable CS8321 // Local function is declared but never used
static void DumpAst(Program ast)
{
    var options = new JsonSerializerOptions();
    options.Converters.Add(new InterfaceConverter());
    options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.Converters.Add(new TypeConverter());
    options.Converters.Add(new SpanConverter());
    options.WriteIndented = true;

    Console.WriteLine(JsonSerializer.Serialize(ast, options));
}
#pragma warning restore CS8321 // Local function is declared but never used

class InterfaceConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsInterface;

    public override JsonConverter CreateConverter(
        Type type,
        JsonSerializerOptions options)
    {
        return new Converter();
    }

    record struct Instance(string Type, object Value);

    class Converter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, new Instance(value.GetType().Name, value), options);
    }
}

class TypeConverter : JsonConverter<Type>
{
    public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options) => writer.WriteStringValue(value.Name);
}

class SpanConverter : JsonConverter<Span>
{
    public override Span Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, Span value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}
