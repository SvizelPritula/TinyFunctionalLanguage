using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

public interface IVariableLike : IBindable
{
    string Name { get; }
    IType? Type { get; }
}

public record class Variable(string Name) : IVariableLike
{
    public IType? Type { get; set; }
    public LocalBuilder? Local { get; set; }
}

public record class Argument(string Name) : IVariableLike
{
    public IType? Type { get; set; }
    public short? Index { get; set; }
}

public record class Field(string Name) : IVariableLike
{
    public IType? Type { get; set; }
    public FieldInfo? FieldInfo { get; set; }
}
