using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

public record class IfExpr(IExpression Condition, BlockExpr TrueBlock, BlockExpr? FalseBlock, Span Span) : IExpression
{
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
