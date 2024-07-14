using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class LetExpr(IdentExpr Name, IExpression Value, Span Span) : IExpression
{
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
