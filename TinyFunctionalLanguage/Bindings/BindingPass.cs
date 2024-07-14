using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Bindings;

public static class BindingPass
{
    public static void Run(Program program)
    {
        ScopedMap<string, IBindable> scope = new();
        BindingPassVisitor visitor = new(scope);

        foreach (IDeclaration decl in program.Declarations)
            decl.Accept(visitor);
    }
}
