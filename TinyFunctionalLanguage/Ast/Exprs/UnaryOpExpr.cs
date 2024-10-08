using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

record class UnaryOpExpr(UnaryOperator Operator, IExpression Value, Span Span) : IExpression
{
    // Filled in by the type inference pass
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}

enum UnaryOperator
{
    Minus,
    Not,
}
