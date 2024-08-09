using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

record class StructDecl(Ident Ident, List<FieldDecl> Fields, Span Span) : IDeclaration
{
    public Struct? Reference { get; set; }
}

record class FieldDecl(Ident Ident, ITypeName Type)
{
    public Field? Reference { get; set; }
}
