using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

record class MemberExpr(IExpression Value, Ident Member, Span Span) : IExpression
{
    public IType? Type { get; set; } = null;
    public Field? Reference { get; set; } = null;
    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
