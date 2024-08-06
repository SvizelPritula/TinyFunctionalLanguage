using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

public interface IFunctionLike : IBindable
{
    public IReadOnlyList<IVariableLike> Arguments { get; }
    public IType? ReturnType { get; }
}

public record class Function(List<Argument> Arguments) : IFunctionLike
{
    public IType? ReturnType { get; set; }
    public MethodBuilder? MethodBuilder { get; set; }

    IReadOnlyList<IVariableLike> IFunctionLike.Arguments => Arguments;
}

public record class Struct(List<Field> Fields) : IFunctionLike, IType
{
    public TypeBuilder? TypeBuilder { get; set; }
    public ConstructorInfo? ConstructorInfo { get; set; }

    public Type? ClrType => TypeBuilder;
    public bool IsPrimitive => false;

    IReadOnlyList<IVariableLike> IFunctionLike.Arguments => Fields;
    IType? IFunctionLike.ReturnType => this;
}
