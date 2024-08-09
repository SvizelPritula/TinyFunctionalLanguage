using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

record class LetExpr(Ident Ident, IExpression Value, Span Span) : IExpression
{
    public IType? Type { get; set; } = null;
    public Variable? Reference { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
