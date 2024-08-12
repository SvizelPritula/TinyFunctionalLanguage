using TinyFunctionalLanguage.Bindings;

namespace TinyFunctionalLanguage.Ast;

record class FunctionDecl(Ident Ident, List<ArgumentDecl> Arguments, ITypeName ReturnType, BlockExpr Block, Span Span) : IDeclaration
{
    // Filled in by the binding pass
    public Function? Reference { get; set; }
}

record class ArgumentDecl(Ident Ident, ITypeName Type)
{
    // Filled in by the binding pass
    public Argument? Reference { get; set; }
}
