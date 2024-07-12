using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public interface IDeclaration
{
    string Name { get; }
    Span Span { get; }
}
