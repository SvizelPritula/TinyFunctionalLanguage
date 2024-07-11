using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Errors;

public class LanguageException(string message, Span span) : Exception(message)
{
    public Span Span { get; } = span;
}
