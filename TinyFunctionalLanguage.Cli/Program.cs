using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Errors;
using System.Text.Json;
using System.Text.Json.Serialization;

try
{
    var code = Console.In.ReadToEnd();
    var ast = Parser.Parse(new Tokenizer(code));

    var options = new JsonSerializerOptions();
    options.Converters.Add(new InterfaceConverter());
    options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.WriteIndented = true;

    Console.WriteLine(JsonSerializer.Serialize(ast, options));
}
catch (LanguageException ex)
{
    Console.WriteLine($"{ex.Span.From.Row}:{ex.Span.From.Col} - {ex.Span.To.Row}:{ex.Span.To.Col}: {ex.Message}");
}

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

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, new Instance(value.GetType().ToString(), value), options);
    }
}
