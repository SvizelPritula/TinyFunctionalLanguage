using TinyFunctionalLanguage.Binding;
using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class IdentExpr(string Name, Span Span) : IExpression
{
    public IBindable? Reference { get; set; } = null;

    public void Accept(IExprVisitor visitor) => visitor.Visit(this);
}
