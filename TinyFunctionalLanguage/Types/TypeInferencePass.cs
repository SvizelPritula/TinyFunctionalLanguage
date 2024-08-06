using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Errors;

namespace TinyFunctionalLanguage.Types;

public static class TypeInferencePass
{
    public static void Run(Program program)
    {
        foreach (FunctionDecl function in program.Functions)
            SetTypesForFunction(function);

        TypeInferencePassVisitor visitor = new();

        foreach (FunctionDecl function in program.Functions)
            ProcessFunction(function);
    }

    static void SetTypesForFunction(FunctionDecl decl)
    {
        var func = decl.Reference!;
        func.ReturnType = GetTypeFromTypeName(decl.ReturnType);

        foreach (var (arg, argDecl) in func.Arguments.Zip(decl.Arguments))
            arg.Type = GetTypeFromTypeName(argDecl.Type);

    }

    static void ProcessFunction(FunctionDecl decl)
    {
        var func = decl.Reference!;

        decl.Block.Accept(new TypeInferencePassVisitor());

        if (decl.Block.Type != func.ReturnType)
            throw new LanguageException(
                $"The {decl.Ident.Name} function should return {func.ReturnType} but returns {decl.Block.Type}",
                decl.Block.Span
            );
    }

    static IType GetTypeFromTypeName(ITypeName name)
    {
        return name.Accept(new TypeResolverVisitor());
    }

    class TypeResolverVisitor : ITypeNameVisitor<IType>
    {
        public IType Visit(IntTypeName typeName) => IntType.Instance;

        public IType Visit(BoolTypeName typeName) => BoolType.Instance;

        public IType Visit(UnitTypeName typeName) => UnitType.Instance;
    }
}
