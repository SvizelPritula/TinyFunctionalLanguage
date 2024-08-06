using System.Reflection.Emit;
using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.CodeGen;

class AssignmentCodeGenVisitor(ILGenerator generator, Action modify) : IExprVisitor
{
    public void Visit(IdentExpr expr)
    {
        var target = expr.Reference!;

        if (target is Variable variable)
        {
            generator.Emit(OpCodes.Ldloc, variable.Local!);
            modify();
            generator.Emit(OpCodes.Stloc, variable.Local!);
        }
        else if (target is Argument argument)
        {
            generator.Emit(OpCodes.Ldarg, (short)argument.Index!);
            modify();
            generator.Emit(OpCodes.Starg, (short)argument.Index!);
        }
        else
        {
            throw new InvalidOperationException("Unknown binding type");
        }
    }

    public void Visit(MemberExpr expr)
    {
        expr.Value.Accept(new AssignmentCodeGenVisitor(generator, () =>
        {
            var @struct = (Struct)expr.Value.Type!;

            var local = generator.DeclareLocal(@struct.ClrType!);
            generator.Emit(OpCodes.Stloc, local);

            foreach (var field in @struct.Fields)
            {
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Ldfld, field.FieldInfo!);

                if (field == expr.Reference)
                    modify();
            }

            generator.Emit(OpCodes.Newobj, @struct.ConstructorInfo!);
        }));
    }

    public void Visit(IntLiteralExpr expr) => throw BadExpr();
    public void Visit(BoolLiteralExpr expr) => throw BadExpr();
    public void Visit(BinaryOpExpr expr) => throw BadExpr();
    public void Visit(UnaryOpExpr expr) => throw BadExpr();
    public void Visit(BlockExpr expr) => throw BadExpr();
    public void Visit(IfExpr expr) => throw BadExpr();
    public void Visit(LetExpr expr) => throw BadExpr();
    public void Visit(CallExpr expr) => throw BadExpr();
    public void Visit(AssignmentExpr expr) => throw BadExpr();
    public void Visit(WhileExpr expr) => throw BadExpr();
    public void Visit(NullExpr expr) => throw BadExpr();

    public Exception BadExpr() => new InvalidOperationException("Only variables and fields can be assigned to");
}
