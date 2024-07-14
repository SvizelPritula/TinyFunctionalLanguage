using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class IfExpr(IExpression Condition, BlockExpr TrueBlock, BlockExpr? FalseBlock, Span Span) : IExpression
{
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
