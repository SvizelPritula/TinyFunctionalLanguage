using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

public record class IdentExpr(string Name, Span Span) : IExpression
{
    public IType? Type { get; set; } = null;
    public IBindable? Reference { get; set; } = null;

    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
