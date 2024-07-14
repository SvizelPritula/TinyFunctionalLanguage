using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public interface IDeclaration
{
    IdentExpr Name { get; }
    Span Span { get; }
}
