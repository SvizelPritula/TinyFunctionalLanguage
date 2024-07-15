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

    internal static IType GetTypeFromTypeName(ITypeName name)
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
