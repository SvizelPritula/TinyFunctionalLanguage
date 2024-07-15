using System.Reflection.Emit;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

public interface IVariableLike : IBindable
{
    IType? Type { get; }
}

public class Variable : IVariableLike
{
    public IType? Type { get; set; }

    public LocalBuilder? Local { get; set; }
}

public class Argument : IVariableLike
{
    public IType? Type { get; set; }
}
