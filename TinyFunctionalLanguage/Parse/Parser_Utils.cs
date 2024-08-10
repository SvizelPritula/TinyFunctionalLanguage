using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Parse;

partial class Parser
{
    Ident ParseIdent()
    {
        Point start = tokenizer.NextTokenStart;
        Token nameToken = tokenizer.Next();
        Point end = tokenizer.LastTokenEnd;

        if (nameToken.Type != TokenType.Ident)
            throw new LanguageError($"Expected an identifier, got a {nameToken.Type.Name()} token.", new(start, end));

        return new((string)nameToken.Content!, new(start, end));
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
            TokenType.String => new StringTypeName(span),
            TokenType.Ident => new NamedTypeName(new((string)nameToken.Content!, span), span),
            _ => throw new LanguageError($"Expected a type, got a {nameToken.Type.Name()} token.", new(start, end)),
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
                throw new LanguageError($"Expected a ',' or ')' token, got a {token.Type.Name()} token.", new(start, end));
        }

        return elements;
    }

    void Expect(TokenType expected)
    {
        TokenType got = tokenizer.Peek().Type;

        if (got != expected)
        {
            Point point = tokenizer.LastTokenEnd;
            throw new LanguageError($"Expected a {expected.Name()} token, got a {got.Name()} token.", new(point, point));
        }

        tokenizer.Next();
    }

    void SkipUntil(Func<TokenType, bool> end)
    {
        while (!end(tokenizer.Peek().Type))
            SkipTokenOrGroup();
    }

    void SkipTokenOrGroup()
    {
        switch (tokenizer.Next().Type)
        {
            case TokenType.LeftBrace:
                SkipUntil(t => t == TokenType.RightBrace);
                break;
            case TokenType.LeftParen:
                SkipUntil(t => t == TokenType.RightParen);
                break;
        }
    }
}
