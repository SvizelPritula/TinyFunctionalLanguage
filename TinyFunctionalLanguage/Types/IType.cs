namespace TinyFunctionalLanguage.Types;

public interface IType
{
    public Type? ClrType { get; }
    public bool IsPrimitive { get; }
}
