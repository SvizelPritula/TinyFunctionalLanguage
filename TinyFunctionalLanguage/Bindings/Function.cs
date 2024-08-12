using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

interface IFunctionLike : IBindable
{
    public IReadOnlyList<IVariableLike> Arguments { get; }
    public IType? ReturnType { get; }
}

record class Function(List<Argument> Arguments) : IFunctionLike
{
    // Filled in by the type inference pass
    public IType? ReturnType { get; set; }
    // Filled in by the codegen pass
    public MethodBuilder? MethodBuilder { get; set; }

    IReadOnlyList<IVariableLike> IFunctionLike.Arguments => Arguments;
}

record class Struct(List<Field> Fields, string Name) : IFunctionLike, IType
{
    // Filled in by the codegen pass
    public TypeBuilder? TypeBuilder { get; set; }
    // Filled in by the codegen pass
    public ConstructorInfo? ConstructorInfo { get; set; }
    // Filled in by the codegen pass
    public MethodBuilder? EqualOp { get; set; }
    // Filled in by the codegen pass
    public MethodBuilder? NotEqualOp { get; set; }

    public Type? ClrType => TypeBuilder;
    public bool IsPrimitive => false;

    IReadOnlyList<IVariableLike> IFunctionLike.Arguments => Fields;
    IType? IFunctionLike.ReturnType => this;

    public override string ToString() => Name;
}
