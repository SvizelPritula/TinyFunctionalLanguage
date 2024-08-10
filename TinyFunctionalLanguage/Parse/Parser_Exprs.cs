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
                end = tokenizer.NextTokenEnd;

                throw new LanguageError($"Expected an expression, got a {token.Type.Name()} token.", new(start, end));
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

        bool containsSyntaxErrors = false;

        while (true)
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

                    goto done;

                default:
                    if (trailing is not null)
                    {
                        Point lastEnd = tokenizer.LastTokenEnd;
                        errors.Add("Expected a semicolon.", new Span(lastEnd, lastEnd));
                    }

                    try
                    {
                        trailing = ParseStatement();
                    }
                    catch (LanguageError error)
                    {
                        errors.Add(error);
                        containsSyntaxErrors = true;
                        SkipUntil(t => t == TokenType.Semi || t == TokenType.RightBrace);
                    }
                    break;
            }
        }

    done:
        Point end = tokenizer.LastTokenEnd;
        return new BlockExpr(statements, trailing, new(start, end), containsSyntaxErrors);
    }
}
