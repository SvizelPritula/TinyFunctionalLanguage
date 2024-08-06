using System.Reflection.Emit;
using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.CodeGen;

partial class CodeGenVisitor(ILGenerator generator) : IExprVisitor
{
    public void Visit(IntLiteralExpr expr)
    {
        generator.Emit(OpCodes.Ldc_I8, expr.Value);
    }

    public void Visit(BoolLiteralExpr expr)
    {
        generator.Emit(expr.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
    }

    public void Visit(BlockExpr expr)
    {
        foreach (IExpression statement in expr.Statements)
        {
            statement.Accept(this);
            generator.Emit(OpCodes.Pop);
        }

        if (expr.Trailing is IExpression trailing)
            trailing.Accept(this);
        else
            MakeUnit();
    }

    public void Visit(IfExpr expr)
    {
        Label endLabel = generator.DefineLabel();
        Label elseLabel = generator.DefineLabel();

        expr.Condition.Accept(this);
        generator.Emit(OpCodes.Brfalse, elseLabel);

        expr.TrueBlock.Accept(this);
        generator.Emit(OpCodes.Br, endLabel);

        generator.MarkLabel(elseLabel);
        if (expr.FalseBlock != null)
            expr.FalseBlock!.Accept(this);
        else
            MakeUnit();

        generator.MarkLabel(endLabel);
    }

    public void Visit(IdentExpr expr)
    {
        if (expr.Reference is Variable variable)
            generator.Emit(OpCodes.Ldloc, variable.Local!);
        else if (expr.Reference is Argument argument)
            generator.Emit(OpCodes.Ldarg, (short)argument.Index!);
        else
            throw new InvalidOperationException("Unknown binding type");
    }

    public void Visit(LetExpr expr)
    {
        var variable = expr.Reference!;
        var local = generator.DeclareLocal(variable.Type!.ClrType!);
        variable.Local = local;

        expr.Value.Accept(this);
        generator.Emit(OpCodes.Stloc, local);

        MakeUnit();
    }

    public void Visit(CallExpr expr)
    {
        foreach (var arg in expr.Arguments)
            arg.Accept(this);

        if (expr.Function is IdentExpr { Reference: Function func })
            generator.Emit(OpCodes.Call, func.MethodBuilder!);
        else if (expr.Function is IdentExpr { Reference: Struct @struct })
            generator.Emit(OpCodes.Newobj, @struct.ConstructorInfo!);
        else
            throw new InvalidOperationException("Only bound functions can be called");
    }

    public void Visit(AssignmentExpr expr)
    {
        if (expr.Left is not IdentExpr { Reference: IVariableLike var })
            throw new InvalidOperationException("Only variables can be assigned to");

        expr.Right.Accept(this);

        if (var is Variable variable)
            generator.Emit(OpCodes.Stloc, variable.Local!);
        else if (var is Argument argument)
            generator.Emit(OpCodes.Starg, (short)argument.Index!);
        else
            throw new InvalidOperationException("Unknown binding type");

        MakeUnit();
    }

    public void Visit(WhileExpr expr)
    {
        Label conditionLabel = generator.DefineLabel();
        Label endLabel = generator.DefineLabel();

        generator.MarkLabel(conditionLabel);
        expr.Condition.Accept(this);
        generator.Emit(OpCodes.Brfalse, endLabel);

        expr.Body.Accept(this);
        generator.Emit(OpCodes.Pop);
        generator.Emit(OpCodes.Br, conditionLabel);

        generator.MarkLabel(endLabel);
        MakeUnit();
    }

    public void Visit(MemberExpr expr)
    {
        expr.Value.Accept(this);
        generator.Emit(OpCodes.Ldfld, expr.Reference!.FieldInfo!);
    }

    void MakeUnit()
    {
        var local = generator.DeclareLocal(UnitType.Instance.ClrType);
        generator.Emit(OpCodes.Ldloca, local);
        generator.Emit(OpCodes.Initobj, UnitType.Instance.ClrType);
        generator.Emit(OpCodes.Ldloc, local);
    }
}
