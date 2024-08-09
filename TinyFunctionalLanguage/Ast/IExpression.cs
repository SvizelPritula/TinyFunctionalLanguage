using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

interface IExpression
{
    Span Span { get; }
    IType? Type { get; }

    void Accept(IExprVisitor visitor);
}
