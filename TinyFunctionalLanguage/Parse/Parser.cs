using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Errors;

namespace TinyFunctionalLanguage.Parse;

public class Parser
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
        IdentExpr name = ParseIdent();

        List<ArgumentDecl> arguments = ParseParenList(() =>
        {
            IdentExpr name = ParseIdent();
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

    BlockExpr ParseBlock()
    {
        Point start = tokenizer.NextTokenStart;

        Expect(TokenType.LeftBrace);

        List<IExpression> statements = [];
        IExpression? trailing = null;

        bool done = false;
        while (!done)
        {
            switch (tokenizer.Peek().Type)
            {
                case TokenType.Semi:
                    tokenizer.Next();

                    if (trailing is not null)
                        statements.Add(trailing);

                    trailing = null;

                    break;

                case TokenType.RightBrace:
                    tokenizer.Next();

                    done = true;
                    break;

                default:
                    if (trailing is not null)
                    {
                        Point lastEnd = tokenizer.LastTokenEnd;
                        throw new LanguageException("Expected a semicolon", new Span(lastEnd, lastEnd));
                    }

                    trailing = ParseStatement();
                    break;
            }
        }

        Point end = tokenizer.LastTokenEnd;
        return new BlockExpr(statements, trailing, new(start, end));
    }

    IExpression ParseStatement()
    {
        switch (tokenizer.Peek().Type)
        {
            case TokenType.Let:
                return ParseLet();
            default:
                return ParseExpression();
        }
    }

    LetExpr ParseLet()
    {
        Point start = tokenizer.NextTokenStart;

        Expect(TokenType.Let);
        IdentExpr name = ParseIdent();
        Expect(TokenType.Equal);
        IExpression value = ParseExpression();

        Point end = tokenizer.LastTokenEnd;
        return new LetExpr(name, value, new(start, end));
    }

    IExpression ParseExpression() => ParseOrExpression();

    IExpression ParseOrExpression() => ParseBinaryOperatorChain(
        ParseAndExpression,
        t => t switch
        {
            TokenType.Or => BinaryOperator.Or,
            _ => null
        }
    );

    IExpression ParseAndExpression() => ParseBinaryOperatorChain(
        ParseEqualityExpression,
        t => t switch
        {
            TokenType.And => BinaryOperator.And,
            _ => null
        }
    );

    IExpression ParseEqualityExpression() => ParseBinaryOperatorChain(
        ParseComparisonExpression,
        t => t switch
        {
            TokenType.DoubleEqual => BinaryOperator.Equal,
            TokenType.NotEqual => BinaryOperator.NotEqual,
            _ => null
        }
    );

    IExpression ParseComparisonExpression() => ParseBinaryOperatorChain(
        ParseAdditionExpression,
        t => t switch
        {
            TokenType.Less => BinaryOperator.Less,
            TokenType.Greater => BinaryOperator.Greater,
            TokenType.LessEqual => BinaryOperator.LessEqual,
            TokenType.GreaterEqual => BinaryOperator.GreaterEqual,
            _ => null
        }
    );

    IExpression ParseAdditionExpression() => ParseBinaryOperatorChain(
        ParseMultiplicationExpression,
        t => t switch
        {
            TokenType.Plus => BinaryOperator.Plus,
            TokenType.Minus => BinaryOperator.Minus,
            _ => null
        }
    );

    IExpression ParseMultiplicationExpression() => ParseBinaryOperatorChain(
        ParseUnaryExpression,
        t => t switch
        {
            TokenType.Star => BinaryOperator.Star,
            TokenType.Slash => BinaryOperator.Slash,
            TokenType.Percent => BinaryOperator.Percent,
            _ => null
        }
    );

    IExpression ParseBinaryOperatorChain(Func<IExpression> parseOperand, Func<TokenType, BinaryOperator?> tokenToOperator)
    {
        Point start = tokenizer.NextTokenStart;
        IExpression left = parseOperand();

        while (true)
        {
            BinaryOperator? maybeOperator = tokenToOperator(tokenizer.Peek().Type);

            if (maybeOperator is not BinaryOperator @operator)
                break;

            tokenizer.Next();

            IExpression right = parseOperand();
            Point end = tokenizer.LastTokenEnd;

            left = new BinaryOpExpr(@operator, left, right, new(start, end));
        }

        return left;
    }

    IExpression ParseUnaryExpression()
    {
        Point start = tokenizer.NextTokenStart;

        UnaryOperator? maybeOperator = tokenizer.Peek().Type switch
        {
            TokenType.Minus => UnaryOperator.Minus,
            TokenType.Not => UnaryOperator.Not,
            _ => null,
        };

        if (maybeOperator is UnaryOperator @operator)
        {
            tokenizer.Next();

            IExpression inner = ParseUnaryExpression();
            Point end = tokenizer.LastTokenEnd;

            return new UnaryOpExpr(@operator, inner, new(start, end));
        }
        else
        {
            return ParseCallExpression();
        }
    }

    IExpression ParseCallExpression()
    {
        Point start = tokenizer.NextTokenStart;
        IExpression inner = ParsePrimaryExpression();

        while (tokenizer.Peek().Type == TokenType.LeftParen)
        {
            List<IExpression> args = ParseParenList(ParseExpression);
            Point end = tokenizer.LastTokenEnd;

            inner = new CallExpr(inner, args, new(start, end));
        }

        return inner;
    }

    IExpression ParsePrimaryExpression()
    {
        Point start = tokenizer.NextTokenStart;
        Point end;

        Token token = tokenizer.Peek();

        switch (token.Type)
        {
            case TokenType.IntLiteral:
                tokenizer.Next();
                end = tokenizer.LastTokenEnd;

                return new IntLiteralExpr((long)token.Content!, new(start, end));

            case TokenType.True or TokenType.False:
                tokenizer.Next();
                end = tokenizer.LastTokenEnd;

                return new BoolLiteralExpr(token.Type == TokenType.True, new(start, end));

            case TokenType.Ident:
                return ParseIdent();

            case TokenType.LeftParen:
                return ParseParenExpression();

            case TokenType.LeftBrace:
                return ParseBlock();

            case TokenType.If:
                return ParseIfExpression();

            default:
                tokenizer.Next();
                end = tokenizer.LastTokenEnd;

                throw new LanguageException($"Unexpected {token.Type} token", new(start, end));
        }
    }

    IExpression ParseParenExpression()
    {
        Expect(TokenType.LeftParen);
        IExpression inner = ParseExpression();
        Expect(TokenType.RightParen);

        return inner;
    }

    IfExpr ParseIfExpression()
    {
        Point start = tokenizer.NextTokenStart;

        Expect(TokenType.If);

        IExpression condition = ParseExpression();
        BlockExpr trueBlock = ParseBlock();
        BlockExpr? falseBlock = null;

        if (tokenizer.Peek().Type == TokenType.Else)
        {
            tokenizer.Next();
            falseBlock = ParseBlock();
        }

        Point end = tokenizer.LastTokenEnd;
        return new IfExpr(condition, trueBlock, falseBlock, new(start, end));
    }

    IdentExpr ParseIdent()
    {
        Point start = tokenizer.NextTokenStart;
        Token nameToken = tokenizer.Next();
        Point end = tokenizer.LastTokenEnd;

        if (nameToken.Type != TokenType.Ident)
            throw new LanguageException($"Expected an identifier, got a {nameToken.Type} token", new(start, end));

        return new IdentExpr((string)nameToken.Content!, new(start, end));
    }

    ITypeName ParseTypeName()
    {
        Point start = tokenizer.NextTokenStart;
        Token nameToken = tokenizer.Next();
        Point end = tokenizer.LastTokenEnd;
        Span span = new(start, end);

        return nameToken.Type switch
        {
            TokenType.Int => new IntTypeName(span),
            TokenType.Bool => new BoolTypeName(span),
            TokenType.Unit => new UnitTypeName(span),
            _ => throw new LanguageException($"Expected an type, got a {nameToken.Type} token", new(start, end)),
        };
    }

    List<T> ParseParenList<T>(Func<T> parseElement)
    {
        Expect(TokenType.LeftParen);
        List<T> elements = [];

        while (true)
        {
            if (tokenizer.Peek().Type == TokenType.RightParen)
            {
                tokenizer.Next();
                break;
            }

            elements.Add(parseElement());

            Point start = tokenizer.NextTokenStart;
            Token token = tokenizer.Next();
            Point end = tokenizer.LastTokenEnd;

            if (token.Type == TokenType.RightParen)
                break;
            else if (token.Type == TokenType.Comma)
                continue;
            else
                throw new LanguageException($"Expected a comma or right paren", new(start, end));
        }

        return elements;
    }

    void Expect(TokenType expected)
    {
        TokenType got = tokenizer.Peek().Type;

        if (got != expected)
        {
            Point point = tokenizer.LastTokenEnd;
            throw new LanguageException($"Expected {expected}", new(point, point));
        }

        tokenizer.Next();
    }
}
