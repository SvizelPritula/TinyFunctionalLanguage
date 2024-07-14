using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

public class Variable : IBindable
{
    public IType? Type { get; set; }
}
