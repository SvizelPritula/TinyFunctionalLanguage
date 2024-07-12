using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class BinaryOpExpr(BinaryOperator Operator, IExpression Left, IExpression Right, Span Span) : IExpression
{
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}

public enum BinaryOperator
{
    Equal,
    NotEqual,
    Less,
    Greater,
    LessEqual,
    GreaterEqual,

    Plus,
    Minus,
    Star,
    Slash,
    Percent,
    Or,
    And,
}
