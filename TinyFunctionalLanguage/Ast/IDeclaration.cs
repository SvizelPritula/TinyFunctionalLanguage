using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

interface IDeclaration
{
    Ident Ident { get; }
    Span Span { get; }
}
