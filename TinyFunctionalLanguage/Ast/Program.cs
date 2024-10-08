namespace TinyFunctionalLanguage.Ast;

record class Program(List<IDeclaration> Declarations)
{
    public IEnumerable<FunctionDecl> Functions => Declarations.OfType<FunctionDecl>();
    public IEnumerable<StructDecl> Structs => Declarations.OfType<StructDecl>();
}
