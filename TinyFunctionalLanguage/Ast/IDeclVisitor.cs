namespace TinyFunctionalLanguage.Ast;

public interface IDeclVisitor
{
    void Visit(FunctionDecl decl);
}
