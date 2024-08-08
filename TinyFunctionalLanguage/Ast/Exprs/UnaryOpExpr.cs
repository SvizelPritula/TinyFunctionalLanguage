using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

public record class UnaryOpExpr(UnaryOperator Operator, IExpression Value, Span Span) : IExpression
{
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}

public enum UnaryOperator
{
    Minus,
    Not,
}
