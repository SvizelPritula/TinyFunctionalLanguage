using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public interface IDeclaration
{
    Ident Ident { get; }
    Span Span { get; }
}
