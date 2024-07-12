namespace TinyFunctionalLanguage.Ast;

public record class FunctionDecl(string Name, List<ArgumentDecl> Arguments, ITypeName ReturnType, BlockExpr Block);

public record struct ArgumentDecl(string Name, ITypeName Type);
