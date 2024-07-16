using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.CodeGen;

class CodeGenVisitor(ILGenerator generator) : IExprVisitor
{
    public void Visit(IntLiteralExpr expr)
    {
        generator.Emit(OpCodes.Ldc_I8, expr.Value);
    }

    public void Visit(BoolLiteralExpr expr)
    {
        generator.Emit(expr.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
    }

    public void Visit(BinaryOpExpr expr)
    {
        switch ((expr.Operator, expr.Type))
        {
            case (BinaryOperator.Equal, IntType or BoolType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Ceq);
                break;

            case (BinaryOperator.NotEqual, IntType or BoolType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Ceq);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
                break;

            case (BinaryOperator.Equal, UnitType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Pop);
                generator.Emit(OpCodes.Pop);
                generator.Emit(OpCodes.Ldc_I4_1);
                break;

            case (BinaryOperator.NotEqual, UnitType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Pop);
                generator.Emit(OpCodes.Pop);
                generator.Emit(OpCodes.Ldc_I4_0);
                break;

            case (BinaryOperator.Less, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Clt);
                break;

            case (BinaryOperator.Greater, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Cgt);
                break;

            case (BinaryOperator.LessEqual, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Cgt);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
                break;

            case (BinaryOperator.GreaterEqual, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Clt);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
                break;

            case (BinaryOperator.Plus, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Add_Ovf);
                break;

            case (BinaryOperator.Minus, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Sub_Ovf);
                break;

            case (BinaryOperator.Star, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Mul_Ovf);
                break;

            case (BinaryOperator.Slash, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Div);
                break;

            case (BinaryOperator.Percent, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Rem);
                break;

            case (BinaryOperator.Or, BoolType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Or);
                break;

            case (BinaryOperator.And, BoolType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.And);
                break;

            default:
                throw new InvalidOperationException("Unexpected operator or type");
        }
    }

    public void Visit(UnaryOpExpr expr)
    {
        switch ((expr.Operator, expr.Type))
        {
            case (UnaryOperator.Minus, IntType):
                generator.Emit(OpCodes.Ldc_I8, (long)0);
                expr.Value.Accept(this);
                generator.Emit(OpCodes.Sub_Ovf);
                break;

            case (UnaryOperator.Not, BoolType):
                expr.Value.Accept(this);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
                break;

            default:
                throw new InvalidOperationException("Unexpected operator or type");
        }
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
        bool hasElse = expr.FalseBlock != null;

        Label endLabel = generator.DefineLabel();
        Label elseLabel = hasElse ? generator.DefineLabel() : endLabel;

        expr.Condition.Accept(this);
        generator.Emit(OpCodes.Brfalse, elseLabel);

        expr.TrueBlock.Accept(this);

        if (hasElse)
        {
            generator.Emit(OpCodes.Br, endLabel);
            generator.MarkLabel(elseLabel);
            expr.FalseBlock!.Accept(this);
        }

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
        var variable = (Variable)expr.Name.Reference!;
        var local = generator.DeclareLocal(variable.Type!.ClrType!);
        variable.Local = local;

        expr.Value.Accept(this);
        generator.Emit(OpCodes.Stloc, local);

        MakeUnit();
    }

    public void Visit(CallExpr expr)
    {
        if (expr.Function is not IdentExpr { Reference: Function func })
            throw new InvalidOperationException("Only bound functions can be called");

        foreach (var arg in expr.Arguments)
            arg.Accept(this);

        generator.Emit(OpCodes.Call, func.Method!);
    }

    public void Generate(FunctionDecl funcDecl)
    {
        Function func = (Function)funcDecl.Name.Reference!;

        for (short i = 0; i < func.Arguments.Count; i++)
            func.Arguments[i].Index = i;

        funcDecl.Block.Accept(this);
        generator.Emit(OpCodes.Ret);
    }

    void MakeUnit()
    {
        var local = generator.DeclareLocal(UnitType.Instance.ClrType);
        generator.Emit(OpCodes.Ldloca, local);
        generator.Emit(OpCodes.Initobj, UnitType.Instance.ClrType);
        generator.Emit(OpCodes.Ldloc, local);
    }
}
