using System.Reflection.Emit;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

public class Variable : IBindable
{
    public IType? Type { get; set; }

    public LocalBuilder? Local { get; set; }
}
