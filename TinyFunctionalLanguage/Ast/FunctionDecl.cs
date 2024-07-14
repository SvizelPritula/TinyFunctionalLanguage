using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class FunctionDecl(IdentExpr Name, List<ArgumentDecl> Arguments, ITypeName ReturnType, BlockExpr Block, Span Span) : IDeclaration;

public record struct ArgumentDecl(IdentExpr Name, ITypeName Type);
