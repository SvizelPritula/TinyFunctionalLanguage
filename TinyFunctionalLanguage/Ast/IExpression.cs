using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public interface IExpression
{
    Span Span { get; }

    void Accept(IExprVisitor visitor);
}
