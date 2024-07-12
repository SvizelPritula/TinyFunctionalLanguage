namespace TinyFunctionalLanguage.Ast;

public interface IExprVisitor {
    public void Visit(IntLiteralExpr expr);
    public void Visit(BoolLiteralExpr expr);
    public void Visit(BinaryOpExpr expr);
    public void Visit(UnaryOpExpr expr);
    public void Visit(BlockExpr expr);
}
