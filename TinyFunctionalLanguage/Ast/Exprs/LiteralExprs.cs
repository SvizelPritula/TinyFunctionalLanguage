using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

record class IntLiteralExpr(long Value, Span Span) : IExpression
{
    // Filled in by the type inference pass
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}

record class BoolLiteralExpr(bool Value, Span Span) : IExpression
{
    // Filled in by the type inference pass
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}

record class StringLiteralExpr(string Value, Span Span) : IExpression
{
    // Filled in by the type inference pass
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
