using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public interface ITypeName
{
    Span Span { get; }
}

public record class IntTypeName(Span Span) : ITypeName;
public record class BoolTypeName(Span Span) : ITypeName;
public record class UnitTypeName(Span Span) : ITypeName;
