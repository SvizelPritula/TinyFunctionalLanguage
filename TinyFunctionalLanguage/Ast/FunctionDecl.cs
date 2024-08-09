using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

record class FunctionDecl(Ident Ident, List<ArgumentDecl> Arguments, ITypeName ReturnType, BlockExpr Block, Span Span) : IDeclaration
{
    public Function? Reference { get; set; }
}

record class ArgumentDecl(Ident Ident, ITypeName Type)
{
    public Argument? Reference { get; set; }
}
