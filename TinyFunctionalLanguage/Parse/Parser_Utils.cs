using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Errors;

namespace TinyFunctionalLanguage.Parse;

public partial class Parser
{
    Ident ParseIdent()
    {
        Point start = tokenizer.NextTokenStart;
        Token nameToken = tokenizer.Next();
        Point end = tokenizer.LastTokenEnd;

        if (nameToken.Type != TokenType.Ident)
            throw new LanguageException($"Expected an identifier, got a {nameToken.Type} token", new(start, end));

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
