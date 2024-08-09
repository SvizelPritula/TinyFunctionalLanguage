using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

record class CallExpr(IExpression Function, List<IExpression> Arguments, Span Span) : IExpression
{
    public IType? Type { get; set; } = null;

    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
