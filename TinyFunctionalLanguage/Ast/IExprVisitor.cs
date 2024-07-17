namespace TinyFunctionalLanguage.Ast;

public interface IExprVisitor
{
    void Visit(IntLiteralExpr expr);
    void Visit(BoolLiteralExpr expr);
    void Visit(BinaryOpExpr expr);
    void Visit(UnaryOpExpr expr);
    void Visit(BlockExpr expr);
    void Visit(IfExpr expr);
    void Visit(IdentExpr expr);
    void Visit(LetExpr expr);
    void Visit(CallExpr expr);
    void Visit(AssignmentExpr expr);
}
