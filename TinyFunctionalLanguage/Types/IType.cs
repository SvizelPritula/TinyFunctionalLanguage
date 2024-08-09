namespace TinyFunctionalLanguage.Types;

interface IType
{
    public Type? ClrType { get; }
    public bool IsPrimitive { get; }
}
