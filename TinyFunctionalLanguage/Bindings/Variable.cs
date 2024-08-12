using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.Bindings;

interface IVariableLike : IBindable
{
    string Name { get; }
    IType? Type { get; }
}

record class Variable(string Name) : IVariableLike
{
    // Filled in by the type inference pass
    public IType? Type { get; set; }
    // Filled in by the codegen pass
    public LocalBuilder? Local { get; set; }
}

record class Argument(string Name) : IVariableLike
{
    // Filled in by the type inference pass
    public IType? Type { get; set; }
    // Filled in by the codegen pass
    public short? Index { get; set; }
}

record class Field(string Name) : IVariableLike
{
    // Filled in by the type inference pass
    public IType? Type { get; set; }
    // Filled in by the codegen pass
    public FieldInfo? FieldInfo { get; set; }
}
