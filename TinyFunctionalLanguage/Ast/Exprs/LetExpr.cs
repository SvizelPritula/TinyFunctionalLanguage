using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

record class LetExpr(Ident Ident, IExpression Value, Span Span) : IExpression
{
    // Filled in by the type inference pass
    public IType? Type { get; set; } = null;
    // Filled in by the binding pass
    public Variable? Reference { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
