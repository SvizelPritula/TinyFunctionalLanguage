using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Ast;

interface IExpression
{
    Span Span { get; }
    // Filled in by the type inference pass
    IType? Type { get; }

    void Accept(IExprVisitor visitor);
}
