using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Types;

public static class TypeInferencePass
{
    public static void Run(Program program)
    {
        TypeInferencePassVisitor visitor = new();

        foreach (var decl in program.Declarations)
            decl.Accept(visitor);
    }
}
