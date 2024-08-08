using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Bindings;

class BindingPassVisitor(ScopedMap<string, IBindable> scope) : IExprVisitor, ITypeNameVisitor
{
    public void Visit(IntLiteralExpr expr) { }

    public void Visit(BoolLiteralExpr expr) { }

    public void Visit(StringLiteralExpr expr) { }

    public void Visit(BinaryOpExpr expr)
    {
        expr.Left.Accept(this);
        expr.Right.Accept(this);
    }

    public void Visit(UnaryOpExpr expr) => expr.Value.Accept(this);

    public void Visit(BlockExpr expr)
    {
        scope.Push();

        foreach (IExpression subExpr in expr.Statements)
            subExpr.Accept(this);
        expr.Trailing?.Accept(this);

        scope.Pop();
    }

    public void Visit(IfExpr expr)
    {
        expr.Condition.Accept(this);
        expr.TrueBlock.Accept(this);
        expr.FalseBlock?.Accept(this);
    }

    public void Visit(IdentExpr expr)
    {
        if (scope.TryGet(expr.Ident.Name, out var reference))
            expr.Reference = reference;
        else
            throw new LanguageException($"There is no variable of function named {expr.Ident.Name} in the current scope.", expr.Span);
    }

    public void Visit(LetExpr expr)
    {
        expr.Value.Accept(this);

        Variable variable = new(expr.Ident.Name);
        scope.Insert(expr.Ident.Name, variable);
        expr.Reference = variable;
    }

    public void Visit(CallExpr expr)
    {
        expr.Function.Accept(this);
        foreach (var arg in expr.Arguments)
            arg.Accept(this);
    }

    public void Visit(AssignmentExpr expr)
    {
        expr.Left.Accept(this);
        expr.Right.Accept(this);
    }

    public void Visit(WhileExpr expr)
    {
        expr.Condition.Accept(this);
        expr.Body.Accept(this);
    }

    public void Visit(MemberExpr expr)
    {
        expr.Value.Accept(this);
    }

    public void Visit(NullExpr expr)
    {
        expr.TypeName.Accept(this);
    }

    public void Visit(IntTypeName typeName) { }
    public void Visit(BoolTypeName typeName) { }
    public void Visit(StringTypeName typeName) { }
    public void Visit(UnitTypeName typeName) { }

    public void Visit(NamedTypeName typeName)
    {
        if (scope.TryGet(typeName.Ident.Name, out var reference))
            if (reference is Struct @struct)
                typeName.Reference = @struct;
            else
                throw new LanguageException($"{typeName.Ident.Name} is not a type.", typeName.Span);
        else
        {
            throw new LanguageException($"There is no type named {typeName.Ident.Name} in the current scope.", typeName.Span);
        }
    }
}
