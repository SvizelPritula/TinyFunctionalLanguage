using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage;

public class LanguageError(string message, Span span) : Exception(message)
{
    public Span Span => span;
}

public class LanguageException(IReadOnlyList<LanguageError> errors)
    : Exception(string.Join(", ", errors.Select(e => e.Message)))
{
    public IReadOnlyList<LanguageError> Errors => errors;
}

public class ErrorSet
{
    List<LanguageError> errors = [];

    public void Add(string message, Span span) => errors.Add(new LanguageError(message, span));
    public void Add(LanguageError error) => errors.Add(error);

    public bool HasErrors => errors.Count > 0;
    public LanguageException Exception() => new(errors);
}
