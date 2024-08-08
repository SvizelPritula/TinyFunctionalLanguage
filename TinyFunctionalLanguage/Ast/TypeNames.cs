using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public interface ITypeName
{
    Span Span { get; }

    T Accept<T>(ITypeNameVisitor<T> visitor);
    void Accept(ITypeNameVisitor visitor);
}

public record class IntTypeName(Span Span) : ITypeName
{
    public T Accept<T>(ITypeNameVisitor<T> visitor) => visitor.Visit(this);
    public void Accept(ITypeNameVisitor visitor) => visitor.Visit(this);
}

public record class BoolTypeName(Span Span) : ITypeName
{
    public T Accept<T>(ITypeNameVisitor<T> visitor) => visitor.Visit(this);
    public void Accept(ITypeNameVisitor visitor) => visitor.Visit(this);
}

public record class StringTypeName(Span Span) : ITypeName
{
    public T Accept<T>(ITypeNameVisitor<T> visitor) => visitor.Visit(this);
    public void Accept(ITypeNameVisitor visitor) => visitor.Visit(this);
}

public record class UnitTypeName(Span Span) : ITypeName
{
    public T Accept<T>(ITypeNameVisitor<T> visitor) => visitor.Visit(this);
    public void Accept(ITypeNameVisitor visitor) => visitor.Visit(this);
}

public record class NamedTypeName(Ident Ident, Span Span) : ITypeName
{
    public Struct? Reference { get; set; } = null;

    public T Accept<T>(ITypeNameVisitor<T> visitor) => visitor.Visit(this);
    public void Accept(ITypeNameVisitor visitor) => visitor.Visit(this);
}

public interface ITypeNameVisitor<T>
{
    T Visit(IntTypeName typeName);
    T Visit(BoolTypeName typeName);
    T Visit(StringTypeName typeName);
    T Visit(UnitTypeName typeName);
    T Visit(NamedTypeName typeName);
}

public interface ITypeNameVisitor
{
    void Visit(IntTypeName typeName);
    void Visit(BoolTypeName typeName);
    void Visit(StringTypeName typeName);
    void Visit(UnitTypeName typeName);
    void Visit(NamedTypeName typeName);
}
