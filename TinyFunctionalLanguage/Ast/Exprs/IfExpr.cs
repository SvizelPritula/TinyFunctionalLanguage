using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

record class IfExpr(IExpression Condition, BlockExpr TrueBlock, BlockExpr? FalseBlock, Span Span) : IExpression
{
    // Filled in by the type inference pass
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
