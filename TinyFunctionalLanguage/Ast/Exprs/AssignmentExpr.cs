using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

record class AssignmentExpr(AssignmentOperator Operator, IExpression Left, IExpression Right, Span Span) : IExpression
{
    public IType? Type { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}

enum AssignmentOperator
{
    Set,

    Plus,
    Minus,
    Star,
    Slash,
    Percent,

    Or,
    And,
}
