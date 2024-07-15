namespace TinyFunctionalLanguage.Ast;

public record class Program(List<IDeclaration> Declarations)
{
    public IEnumerable<FunctionDecl> Functions => Declarations.OfType<FunctionDecl>();
}
