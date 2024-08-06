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
            else if (decl is StructDecl structDecl)
            {
                List<Field> fields = [];

                foreach (FieldDecl fieldDecl in structDecl.Fields)
                {
                    Field field = new(fieldDecl.Ident.Name);
                    fieldDecl.Reference = field;
                    fields.Add(field);
                }

                Struct @struct = new(fields);

                structDecl.Reference = @struct;
                scope.Insert(structDecl.Ident.Name, @struct);
            }
        }

        BindingPassVisitor visitor = new(scope);

        foreach (FunctionDecl funcDecl in program.Functions)
        {
            foreach (var argDecl in funcDecl.Arguments)
                argDecl.Type.Accept(visitor);
            funcDecl.ReturnType.Accept(visitor);

            Function func = funcDecl.Reference!;
            scope.Push();

            foreach (var (arg, argDecl) in func.Arguments.Zip(funcDecl.Arguments))
                scope.Insert(argDecl.Ident.Name, arg);

            funcDecl.Block.Accept(visitor);

            scope.Pop();
        }

        foreach (StructDecl structDecl in program.Structs)
        {
            foreach (var fieldDecl in structDecl.Fields)
                fieldDecl.Type.Accept(visitor);
        }
    }
}
