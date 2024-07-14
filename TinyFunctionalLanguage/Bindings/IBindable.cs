using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

public interface IBindable
{
    IType? Type { get; }
}
