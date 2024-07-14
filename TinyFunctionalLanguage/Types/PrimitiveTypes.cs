namespace TinyFunctionalLanguage.Types;

public class IntType : IType
{
    private IntType() { }
    public static IntType Instance { get; } = new IntType();

    public Type ClrType => typeof(long);
    public bool IsPrimitive => true;
}

public class BoolType : IType
{
    private BoolType() { }
    public static BoolType Instance { get; } = new BoolType();

    public Type ClrType => typeof(bool);
    public bool IsPrimitive => true;
}

public class UnitType : IType
{
    private UnitType() { }
    public static UnitType Instance { get; } = new UnitType();

    public Type ClrType => typeof(ValueTuple);
    public bool IsPrimitive => true;
}
