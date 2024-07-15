using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public interface ITypeName
{
    Span Span { get; }

    T Accept<T>(ITypeNameVisitor<T> visitor);
}

public record class IntTypeName(Span Span) : ITypeName { public T Accept<T>(ITypeNameVisitor<T> visitor) => visitor.Visit(this); }
public record class BoolTypeName(Span Span) : ITypeName { public T Accept<T>(ITypeNameVisitor<T> visitor) => visitor.Visit(this); }
public record class UnitTypeName(Span Span) : ITypeName { public T Accept<T>(ITypeNameVisitor<T> visitor) => visitor.Visit(this); }

public interface ITypeNameVisitor<T>
{
    T Visit(IntTypeName typeName);
    T Visit(BoolTypeName typeName);
    T Visit(UnitTypeName typeName);
}
