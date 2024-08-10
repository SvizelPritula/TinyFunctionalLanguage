using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Bindings;

static class BindingPass
{
    public static void Run(Program program, ErrorSet errors)
    {
        ScopedMap<string, IBindable> scope = new();

        foreach (IDeclaration decl in program.Declarations)
        {
            if (decl is FunctionDecl functionDecl)
            {
                AttachFunctionObjects(functionDecl);
                if (scope.Insert(functionDecl.Ident.Name, functionDecl.Reference!))
                    errors.Add($"Double definition of name {functionDecl.Ident.Name}.", functionDecl.Ident.Span);
            }
            else if (decl is StructDecl structDecl)
            {
                AttachStructObjects(structDecl);
                if (scope.Insert(structDecl.Ident.Name, structDecl.Reference!))
                    errors.Add($"Double definition of name {structDecl.Ident.Name}.", structDecl.Ident.Span);
            }
        }

        foreach (FunctionDecl funcDecl in program.Functions)
            BindFunction(scope, errors, funcDecl);

        foreach (StructDecl structDecl in program.Structs)
        {
            BindingPassVisitor visitor = new(scope, errors);
            foreach (var fieldDecl in structDecl.Fields)
                fieldDecl.Type.Accept(visitor);
        }
    }

    static void AttachFunctionObjects(FunctionDecl decl)
    {
        List<Argument> arguments = [];

        foreach (ArgumentDecl argDecl in decl.Arguments)
        {
            Argument arg = new(argDecl.Ident.Name);
            argDecl.Reference = arg;
            arguments.Add(arg);
        }

        decl.Reference = new(arguments);
    }

    static void AttachStructObjects(StructDecl decl)
    {
        List<Field> fields = [];

        foreach (FieldDecl fieldDecl in decl.Fields)
        {
            Field field = new(fieldDecl.Ident.Name);
            fieldDecl.Reference = field;
            fields.Add(field);
        }

        decl.Reference = new(fields, decl.Ident.Name);
    }

    static void BindFunction(ScopedMap<string, IBindable> scope, ErrorSet errors, FunctionDecl funcDecl)
    {

        BindingPassVisitor visitor = new(scope, errors);

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
}
