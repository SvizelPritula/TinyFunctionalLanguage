namespace TinyFunctionalLanguage.Types;

class IntType : IType
{
    private IntType() { }
    public static IntType Instance { get; } = new IntType();

    public Type ClrType => typeof(long);
    public bool IsPrimitive => true;
}

class BoolType : IType
{
    private BoolType() { }
    public static BoolType Instance { get; } = new BoolType();

    public Type ClrType => typeof(bool);
    public bool IsPrimitive => true;
}

class StringType : IType
{
    private StringType() { }
    public static StringType Instance { get; } = new StringType();

    public Type ClrType => typeof(string);
    public bool IsPrimitive => true;
}

class UnitType : IType
{
    private UnitType() { }
    public static UnitType Instance { get; } = new UnitType();

    public Type ClrType => typeof(ValueTuple);
    public bool IsPrimitive => true;
}
