using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Errors;

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
        bool endOfFile = false;

        while (!endOfFile)
        {
            switch (tokenizer.Peek().Type)
            {
                case TokenType.Eof:
                    endOfFile = true;
                    break;

                case TokenType.Func:
                    declarations.Add(ParseFunction());
                    break;

                case TokenType.Struct:
                    declarations.Add(ParseStruct());
                    break;

                default:
                    throw new LanguageException("Expected a func of struct declaration", new(tokenizer.NextTokenStart));
            }
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

    StructDecl ParseStruct()
    {
        Point start = tokenizer.NextTokenStart;

        Expect(TokenType.Struct);
        Ident name = ParseIdent();

        List<FieldDecl> fields = [];
        Expect(TokenType.LeftBrace);

        while (tokenizer.Peek().Type != TokenType.RightBrace)
        {
            Ident fieldName = ParseIdent();
            Expect(TokenType.Colon);
            ITypeName fieldType = ParseTypeName();
            Expect(TokenType.Semi);

            fields.Add(new FieldDecl(fieldName, fieldType));
        }

        Expect(TokenType.RightBrace);
        Point end = tokenizer.LastTokenEnd;

        return new StructDecl(name, fields, new(start, end));
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
