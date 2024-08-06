using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class StructDecl(Ident Ident, List<FieldDecl> Fields, Span Span) : IDeclaration
{
    public Struct? Reference { get; set; }
}

public record class FieldDecl(Ident Ident, ITypeName Type)
{
    public Field? Reference { get; set; }
}
