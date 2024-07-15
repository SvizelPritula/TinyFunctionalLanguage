using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Types;

public static class TypeInferencePass
{
    public static void Run(Program program)
    {
        TypeInferencePassVisitor visitor = new();

        foreach (FunctionDecl function in program.Functions)
            visitor.Visit(function);
    }
}
