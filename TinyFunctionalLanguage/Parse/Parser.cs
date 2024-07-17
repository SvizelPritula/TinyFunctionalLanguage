using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Parse;

public partial class Parser
{
    public static Program Parse(Tokenizer tokenizer)
    {
        return new Parser(tokenizer).ParseProgram();
    }

    private Parser(Tokenizer tokenizer)
    {
        this.tokenizer = tokenizer;
    }

    readonly Tokenizer tokenizer;

    Program ParseProgram()
    {
        List<IDeclaration> declarations = [];

        while (tokenizer.Peek().Type != TokenType.Eof)
        {
            declarations.Add(ParseFunction());
        }

        return new(declarations);
    }

    FunctionDecl ParseFunction()
    {
        Point start = tokenizer.NextTokenStart;

        Expect(TokenType.Func);
        Ident name = ParseIdent();

        List<ArgumentDecl> arguments = ParseParenList(() =>
        {
            Ident name = ParseIdent();
            Expect(TokenType.Colon);
            ITypeName type = ParseTypeName();

            return new ArgumentDecl(name, type);
        });

        Expect(TokenType.Colon);
        ITypeName returnType = ParseTypeName();

        BlockExpr block = ParseBlock();

        Point end = tokenizer.LastTokenEnd;

        return new FunctionDecl(name, arguments, returnType, block, new(start, end));
    }

    IExpression ParseStatement()
    {
        switch (tokenizer.Peek().Type)
        {
            case TokenType.Let:
                return ParseLet();
            case TokenType.While:
                return ParseWhile();
            default:
                return ParseAssignmentOrExpression();
        }
    }

    LetExpr ParseLet()
    {
        Point start = tokenizer.NextTokenStart;

        Expect(TokenType.Let);
        Ident name = ParseIdent();
        Expect(TokenType.Equal);
        IExpression value = ParseExpression();

        Point end = tokenizer.LastTokenEnd;
        return new LetExpr(name, value, new(start, end));
    }

    IExpression ParseAssignmentOrExpression()
    {
        Point start = tokenizer.NextTokenStart;
        IExpression left = ParseExpression();

        if (tokenizer.Peek().Type != TokenType.Equal)
            return left;

        tokenizer.Next();

        IExpression right = ParseExpression();
        Point end = tokenizer.LastTokenEnd;

        return new AssignmentExpr(left, right, new(start, end));
    }

    WhileExpr ParseWhile()
    {
        Point start = tokenizer.NextTokenStart;

        Expect(TokenType.While);
        IExpression condition = ParseExpression();
        BlockExpr body = ParseBlock();

        Point end = tokenizer.LastTokenEnd;

        return new WhileExpr(condition, body, new(start, end));
    }
}
