using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.CodeGen;

partial class CodeGenVisitor : IExprVisitor
{
    public void Visit(BinaryOpExpr expr)
    {
        switch ((expr.Operator, expr.Left.Type, expr.Right.Type))
        {
            case (BinaryOperator.Equal, var type, var otherType) when otherType == type:
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                CodeGen.EmitEqualityCheck(generator, type!);
                break;

            case (BinaryOperator.NotEqual, var type, var otherType) when otherType == type:
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                CodeGen.EmitEqualityCheck(generator, type!, true);
                break;

            case (BinaryOperator.Less, IntType, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Clt);
                break;

            case (BinaryOperator.Greater, IntType, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Cgt);
                break;

            case (BinaryOperator.LessEqual, IntType, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Cgt);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
                break;

            case (BinaryOperator.GreaterEqual, IntType, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Clt);
                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Ceq);
                break;

            case (BinaryOperator.Plus, IntType, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Add_Ovf);
                break;

            case (BinaryOperator.Minus, IntType, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Sub_Ovf);
                break;

            case (BinaryOperator.Star, IntType, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Mul_Ovf);
                break;

            case (BinaryOperator.Slash, IntType, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Div);
                break;

            case (BinaryOperator.Percent, IntType, IntType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Rem);
                break;

            case (BinaryOperator.Or, BoolType, BoolType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.Or);
                break;

            case (BinaryOperator.And, BoolType, BoolType):
                expr.Left.Accept(this);
                expr.Right.Accept(this);
                generator.Emit(OpCodes.And);
                break;

            case (BinaryOperator.Plus, StringType or IntType, StringType or IntType):
                expr.Left.Accept(this);
                ConvertToString(expr.Left.Type!);
                expr.Right.Accept(this);
                ConvertToString(expr.Right.Type!);

                generator.Emit(OpCodes.Call, stringConcat);
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

    static readonly MethodInfo stringConcat = typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static, [typeof(string), typeof(string)])!;
    static readonly MethodInfo intToString = IntType.Instance.ClrType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance, [])!;
}
