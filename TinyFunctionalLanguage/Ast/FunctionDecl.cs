using TinyFunctionalLanguage.Parse;

namespace TinyFunctionalLanguage.Ast;

public record class FunctionDecl(string Name, List<ArgumentDecl> Arguments, ITypeName ReturnType, BlockExpr Block, Span Span) : IDeclaration;

public record struct ArgumentDecl(string Name, ITypeName Type);
