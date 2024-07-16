using System.Reflection.Emit;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

public interface IFunctionLike : IBindable
{
    public List<Argument> Arguments { get; }
    public IType? ReturnType { get; set; }
}

public record class Function(List<Argument> Arguments) : IFunctionLike
{
    public IType? ReturnType { get; set; }
    public MethodBuilder? Method { get; set; }
}
