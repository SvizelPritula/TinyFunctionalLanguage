using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Bindings;

namespace TinyFunctionalLanguage.Types;

class TypeInferencePassVisitor : IExprVisitor
{
    public void Visit(IntLiteralExpr expr) => expr.Type = IntType.Instance;

    public void Visit(BoolLiteralExpr expr) => expr.Type = BoolType.Instance;

    public void Visit(StringLiteralExpr expr) => expr.Type = StringType.Instance;

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

        if (expr.Condition.Type != BoolType.Instance)
            throw new LanguageException($"A condition must evaluate to a boolean, is of type {expr.Condition.Type}", expr.Condition.Span);

        IType type = expr.TrueBlock.Type!;

        if (expr.FalseBlock is BlockExpr falseBlock)
        {
            if (falseBlock.Type != type)
                throw new LanguageException(
                    $"Branches of if blocks have different types, {type} and {falseBlock.Type}",
                    expr.Span
                );
        }
        else
        {
            if (type != UnitType.Instance)
                throw new LanguageException(
                    $"The body of an if expression without an else block must return the unit type, is of type {expr.TrueBlock.Type}",
                    expr.Span
                );
        }

        expr.Type = type;
    }

    public void Visit(IdentExpr expr)
    {
        if (expr.Reference is IVariableLike variable)
            expr.Type = variable.Type;
        else
            throw new LanguageException($"{expr.Ident.Name} doesn't refer to a variable", expr.Span);
    }

    public void Visit(LetExpr expr)
    {
        expr.Value.Accept(this);

        expr.Reference!.Type = expr.Value.Type;
        expr.Type = UnitType.Instance;
    }

    public void Visit(CallExpr expr)
    {
        if (expr.Function is not IdentExpr funcIdent)
            throw new LanguageException("It's only possible to call named functions", expr.Function.Span);

        if (funcIdent.Reference is not IFunctionLike func)
            throw new LanguageException($"{funcIdent.Ident.Name} doesn't refer to a function", expr.Span);

        foreach (var argExpr in expr.Arguments)
            argExpr.Accept(this);

        if (func.Arguments.Count != expr.Arguments.Count)
            throw new LanguageException(
                $"The function takes {func.Arguments.Count} arguments but is called with {expr.Arguments.Count}",
                expr.Span
            );

        foreach (var (arg, argExpr) in func.Arguments.Zip(expr.Arguments))
        {
            if (arg.Type != argExpr.Type)
                throw new LanguageException(
                    $"The argument {arg.Name} has type {arg.Type}, but a value of type {argExpr.Type} is given",
                    argExpr.Span
                );
        }

        expr.Type = func.ReturnType;
    }

    public void Visit(AssignmentExpr expr)
    {
        expr.Left.Accept(this);
        expr.Right.Accept(this);

        if (!TypeInferencePass.IsValidLeftHandSide(expr.Left))
            throw new LanguageException($"Left hand side of assignment must be a variable or field.", expr.Left.Span);

        IType left = expr.Left.Type!;
        IType right = expr.Right.Type!;

        bool possible = (expr.Operator, left, right) switch
        {
            (AssignmentOperator.Set, _, _) => left == right,
            (AssignmentOperator.Plus, IntType, IntType) => true,
            (AssignmentOperator.Minus, IntType, IntType) => true,
            (AssignmentOperator.Star, IntType, IntType) => true,
            (AssignmentOperator.Slash, IntType, IntType) => true,
            (AssignmentOperator.Percent, IntType, IntType) => true,
            (AssignmentOperator.And, BoolType, BoolType) => true,
            (AssignmentOperator.Or, BoolType, BoolType) => true,
            (AssignmentOperator.Plus, StringType, StringType or IntType) => true,
            _ => false
        };

        if (!possible)
            throw new LanguageException($"A value of type {expr.Right.Type} cannot be assigned with {expr.Operator} to type {expr.Left.Type}", expr.Span);

        expr.Type = UnitType.Instance;
    }

    public void Visit(WhileExpr expr)
    {
        expr.Condition.Accept(this);
        expr.Body.Accept(this);

        if (expr.Condition.Type != BoolType.Instance)
            throw new LanguageException($"A condition must evaluate to a boolean, is of type {expr.Condition.Type}", expr.Condition.Span);

        if (expr.Body.Type != UnitType.Instance)
            throw new LanguageException(
                $"The body of a while statement without an else must return the unit type, is of type {expr.Body.Type}",
                expr.Span
            );

        expr.Type = UnitType.Instance;
    }

    public void Visit(MemberExpr expr)
    {
        expr.Value.Accept(this);

        var type = expr.Value.Type!;

        if (type is Struct @struct)
        {
            if (@struct.Fields.Find(f => f.Name == expr.Member.Name) is Field field)
            {
                expr.Reference = field;
                expr.Type = field.Type;
                return;
            }
        }

        throw new LanguageException($"The type {type} doesn't have a member called {expr.Member.Name}", expr.Span);
    }

    public void Visit(NullExpr expr)
    {
        expr.Type = TypeInferencePass.GetTypeFromTypeName(expr.TypeName);

        if (expr.Type is not Struct)
            throw new LanguageException($"Only a struct can be null", expr.TypeName.Span);
    }

    static IType GetBinaryOpResultType(BinaryOpExpr expr)
    {
        BinaryOperator @operator = expr.Operator;
        IType left = expr.Left.Type!;
        IType right = expr.Right.Type!;

        switch (@operator)
        {
            case BinaryOperator.Equal or BinaryOperator.NotEqual:
                if (left == right)
                    return BoolType.Instance;
                break;

            case BinaryOperator.Less or BinaryOperator.Greater
                or BinaryOperator.LessEqual or BinaryOperator.GreaterEqual:
                if (left is IntType && right is IntType)
                    return BoolType.Instance;
                break;

            case BinaryOperator.Plus or BinaryOperator.Minus or BinaryOperator.Star
                or BinaryOperator.Slash or BinaryOperator.Percent:
                if (left is IntType && right is IntType)
                    return IntType.Instance;
                if (left is StringType or IntType && right is StringType or IntType)
                    return StringType.Instance;
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
}
