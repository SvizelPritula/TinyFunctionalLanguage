using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class IdentExpr(string Name, Span Span) : IExpression
{
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
