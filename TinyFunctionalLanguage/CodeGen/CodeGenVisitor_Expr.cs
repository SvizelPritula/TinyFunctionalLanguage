using System.Reflection.Emit;
using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.CodeGen;

partial class CodeGenVisitor : IExprVisitor
{
    public void Visit(BinaryOpExpr expr)
    {
        switch ((expr.Operator, expr.Left.Type))
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
}
