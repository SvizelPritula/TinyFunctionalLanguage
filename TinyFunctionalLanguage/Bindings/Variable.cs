using System.Reflection.Emit;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

public interface IVariableLike : IBindable
{
    IType? Type { get; }
}

public record class Variable : IVariableLike
{
    public IType? Type { get; set; }
    public LocalBuilder? Local { get; set; }
}

public record class Argument(string Name) : IVariableLike
{
    public IType? Type { get; set; }
    public short? Index { get; set; }
}
