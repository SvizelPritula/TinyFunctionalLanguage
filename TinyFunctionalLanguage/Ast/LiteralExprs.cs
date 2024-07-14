using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

public record class IntLiteralExpr(long Value, Span Span) : IExpression
{
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}

public record class BoolLiteralExpr(bool Value, Span Span) : IExpression
{
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
