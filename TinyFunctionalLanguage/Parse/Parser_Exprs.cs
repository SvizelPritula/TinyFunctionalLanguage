using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Parse;

partial class Parser
{
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

            case TokenType.StringLiteral:
                tokenizer.Next();
                end = tokenizer.LastTokenEnd;

                return new StringLiteralExpr((string)token.Content!, new(start, end));

            case TokenType.Ident:
                Ident ident = ParseIdent();
                return new IdentExpr(ident, ident.Span);

            case TokenType.LeftParen:
                return ParseParenExpression();

            case TokenType.LeftBrace:
                return ParseBlock();

            case TokenType.If:
                return ParseIfExpression();

            case TokenType.Null:
                return ParseNullExpression();

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

    NullExpr ParseNullExpression()
    {
        Point start = tokenizer.NextTokenStart;

        Expect(TokenType.Null);
        ITypeName type = ParseTypeName();

        Point end = tokenizer.LastTokenEnd;
        return new NullExpr(type, new(start, end));
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
}
