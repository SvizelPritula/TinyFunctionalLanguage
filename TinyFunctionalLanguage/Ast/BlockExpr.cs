using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class BlockExpr(List<IExpression> Statements, IExpression? Trailing, Span Span) : IExpression
{
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
