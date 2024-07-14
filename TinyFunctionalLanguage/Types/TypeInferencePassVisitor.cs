using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Errors;

namespace TinyFunctionalLanguage.Types;

class TypeInferencePassVisitor : IExprVisitor, IDeclVisitor
{
    public void Visit(IntLiteralExpr expr) => expr.Type = IntType.Instance;

    public void Visit(BoolLiteralExpr expr) => expr.Type = BoolType.Instance;

    public void Visit(BinaryOpExpr expr)
    {
        expr.Left.Accept(this);
        expr.Right.Accept(this);
        expr.Type = GetBinaryOpResultType(expr);
    }

    public void Visit(UnaryOpExpr expr)
    {
        expr.Value.Accept(this);
        expr.Type = GetUnaryOpResultType(expr);
    }

    public void Visit(BlockExpr expr)
    {
        foreach (IExpression subExpr in expr.Statements)
            subExpr.Accept(this);
        expr.Trailing?.Accept(this);

        expr.Type = expr.Trailing?.Type ?? UnitType.Instance;
    }

    public void Visit(IfExpr expr)
    {
        expr.Condition.Accept(this);
        expr.TrueBlock.Accept(this);
        expr.FalseBlock?.Accept(this);

        IType type = expr.TrueBlock.Type!;

        if (expr.FalseBlock is BlockExpr falseBlock)
        {
            if (falseBlock.Type != type)
                throw new LanguageException("Branches of if blocks have different types", expr.Span);
        }
        else
        {
            if (type != UnitType.Instance)
                throw new LanguageException("The body of an if block without an else must return the unit type", expr.Span);
        }

        expr.Type = type;
    }


    public void Visit(IdentExpr expr) => expr.Type = expr.Reference!.Type;

    public void Visit(LetExpr expr)
    {
        expr.Name.Accept(this);
        expr.Value.Accept(this);

        ((Variable)expr.Name.Reference!).Type = expr.Value.Type;
        expr.Type = UnitType.Instance;
    }

    static IType GetBinaryOpResultType(BinaryOpExpr expr)
    {
        BinaryOperator @operator = expr.Operator;
        IType left = expr.Left.Type!;
        IType right = expr.Right.Type!;

        switch (@operator)
        {
            case BinaryOperator.Equal or BinaryOperator.NotEqual:
                if (left == right && left.IsPrimitive)
                    return left;
                break;

            case BinaryOperator.Plus or BinaryOperator.Minus or BinaryOperator.Star
                or BinaryOperator.Slash or BinaryOperator.Percent
                or BinaryOperator.Less or BinaryOperator.Greater
                or BinaryOperator.LessEqual or BinaryOperator.GreaterEqual:
                if (left is IntType && right is IntType)
                    return IntType.Instance;
                break;

            case BinaryOperator.Or or BinaryOperator.And:
                if (left is BoolType && right is BoolType)
                    return BoolType.Instance;
                break;
        }

        throw new LanguageException($"The {@operator} operator is not defined for types {left} and {right}", expr.Span);
    }

    static IType GetUnaryOpResultType(UnaryOpExpr expr)
    {
        UnaryOperator @operator = expr.Operator;
        IType type = expr.Value.Type!;

        switch (@operator)
        {
            case UnaryOperator.Minus:
                if (type is IntType)
                    return IntType.Instance;
                break;

            case UnaryOperator.Not:
                if (type is BoolType)
                    return BoolType.Instance;
                break;
        }

        throw new LanguageException($"The {@operator} operator is not defined for type {type}", expr.Span);
    }

    public void Visit(FunctionDecl decl)
    {
        decl.Block.Accept(this);
    }
}
