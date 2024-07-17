using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Bindings;

public static class BindingPass
{
    public static void Run(Program program)
    {
        ScopedMap<string, IBindable> scope = new();

        foreach (IDeclaration decl in program.Declarations)
        {
            if (decl is FunctionDecl functionDecl)
            {
                List<Argument> arguments = [];

                foreach (ArgumentDecl argDecl in functionDecl.Arguments)
                {
                    Argument arg = new(argDecl.Ident.Name);
                    argDecl.Reference = arg;
                    arguments.Add(arg);
                }

                Function function = new(arguments);

                functionDecl.Reference = function;
                scope.Insert(functionDecl.Ident.Name, function);
            }
        }

        BindingPassVisitor visitor = new(scope);

        foreach (FunctionDecl function in program.Functions)
            visitor.Process(function);
    }
}
