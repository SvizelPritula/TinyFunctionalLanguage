using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class UnaryOpExpr(UnaryOperator Operator, IExpression Value, Span Span) : IExpression
{
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}

public enum UnaryOperator
{
    Minus,
    Not,
}
