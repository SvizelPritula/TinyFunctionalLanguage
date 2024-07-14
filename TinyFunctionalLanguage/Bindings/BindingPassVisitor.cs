using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Errors;

namespace TinyFunctionalLanguage.Bindings;

class BindingPassVisitor(ScopedMap<string, IBindable> scope) : IExprVisitor, IDeclVisitor
{
    public void Visit(IntLiteralExpr expr) { }

    public void Visit(BoolLiteralExpr expr) { }

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
        if (scope.TryGet(expr.Name, out var reference))
            expr.Reference = reference;
        else
            throw new LanguageException($"There is no variable named {expr.Name} in the current scope.", expr.Span);
    }

    public void Visit(LetExpr expr)
    {
        expr.Value.Accept(this);

        Variable variable = new();
        scope.Insert(expr.Name.Name, variable);
        expr.Name.Reference = variable;
    }

    public void Visit(FunctionDecl decl)
    {
        decl.Block.Accept(this);
    }
}
