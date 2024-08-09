using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Types;

static class TypeInferencePass
{
    public static void Run(Program program)
    {
        foreach (StructDecl @struct in program.Structs)
            SetTypesForStruct(@struct);

        foreach (FunctionDecl function in program.Functions)
            SetTypesForFunction(function);

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

    static void SetTypesForStruct(StructDecl decl)
    {
        var @struct = decl.Reference!;

        foreach (var (field, fieldDecl) in @struct.Fields.Zip(decl.Fields))
            field.Type = GetTypeFromTypeName(fieldDecl.Type);
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

    internal static IType GetTypeFromTypeName(ITypeName name)
    {
        return name.Accept(new TypeResolverVisitor());
    }

    class TypeResolverVisitor : ITypeNameVisitor<IType>
    {
        public IType Visit(IntTypeName typeName) => IntType.Instance;
        public IType Visit(BoolTypeName typeName) => BoolType.Instance;
        public IType Visit(StringTypeName typeName) => StringType.Instance;
        public IType Visit(UnitTypeName typeName) => UnitType.Instance;
        public IType Visit(NamedTypeName typeName) => typeName.Reference!;
    }

    internal static bool IsValidLeftHandSide(IExpression expr)
    {
        return expr is IdentExpr || (expr is MemberExpr { Value: var value } && IsValidLeftHandSide(value));
    }
}
