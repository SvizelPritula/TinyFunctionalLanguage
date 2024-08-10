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
    public IType? ReturnType { get; set; }
    public MethodBuilder? MethodBuilder { get; set; }

    IReadOnlyList<IVariableLike> IFunctionLike.Arguments => Arguments;
}

record class Struct(List<Field> Fields, string Name) : IFunctionLike, IType
{
    public TypeBuilder? TypeBuilder { get; set; }
    public ConstructorInfo? ConstructorInfo { get; set; }
    public MethodBuilder? EqualOp { get; set; }
    public MethodBuilder? NotEqualOp { get; set; }

    public Type? ClrType => TypeBuilder;
    public bool IsPrimitive => false;

    IReadOnlyList<IVariableLike> IFunctionLike.Arguments => Fields;
    IType? IFunctionLike.ReturnType => this;

    public override string ToString() => Name;
}
