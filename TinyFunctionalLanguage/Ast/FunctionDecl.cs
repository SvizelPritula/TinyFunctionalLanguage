using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

public record class FunctionDecl(IdentExpr Name, List<ArgumentDecl> Arguments, ITypeName ReturnType, BlockExpr Block, Span Span) : IDeclaration
{
    public IType? Type { get; set; } = null;
    public void Accept(IDeclVisitor visitor) => visitor.Visit(this);
}

public record struct ArgumentDecl(IdentExpr Name, ITypeName Type);
