using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

record class StructDecl(Ident Ident, List<FieldDecl> Fields, Span Span) : IDeclaration
{
    // Filled in by the binding pass
    public Struct? Reference { get; set; }
}

record class FieldDecl(Ident Ident, ITypeName Type)
{
    // Filled in by the binding pass
    public Field? Reference { get; set; }
}
