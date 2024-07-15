using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Bindings;

public static class BindingPass
{
    public static void Run(Program program)
    {
        ScopedMap<string, IBindable> scope = new();
        BindingPassVisitor visitor = new(scope);

        foreach (FunctionDecl function in program.Functions)
            visitor.Visit(function);
    }
}
