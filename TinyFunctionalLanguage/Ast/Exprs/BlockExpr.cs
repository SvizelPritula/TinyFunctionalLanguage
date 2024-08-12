using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

record class BlockExpr(List<IExpression> Statements, IExpression? Trailing, Span Span, bool ContainsSyntaxErrors = false) : IExpression
{
    // Filled in by the type inference pass
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
