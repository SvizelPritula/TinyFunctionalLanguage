using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class IntLiteralExpr(long Value, Span Span) : IExpression
{
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}

public record class BoolLiteralExpr(bool Value, Span Span) : IExpression
{
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
