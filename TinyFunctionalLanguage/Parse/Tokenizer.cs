using System.Globalization;
using System.Text;

namespace TinyFunctionalLanguage.Parse;

class Tokenizer
{
    readonly SpannedReader reader;
    readonly ErrorSet errors;

    public Point LastTokenEnd { get; private set; }
    public Point NextTokenStart { get; private set; }
    public Point NextTokenEnd => reader.Point;

    Token nextToken;

    public Tokenizer(string sourceCode, ErrorSet errors)
    {
        reader = new(sourceCode);
        this.errors = errors;

        SeekToNextToken();
        nextToken = ParseNextToken();
    }

    public Token Peek() => nextToken;

    public Token Next()
    {
        Token token = nextToken;

        SeekToNextToken();
        nextToken = ParseNextToken();

        return token;
    }

    void SeekToNextToken()
    {
        LastTokenEnd = reader.Point;

        while (true)
        {
            if (IsCharAnd(reader.Peek(), char.IsWhiteSpace))
                reader.Read();
            else if (IsCharAnd(reader.Peek(), c => c == '#'))
                SkipComment();
            else
                break;
        }

        NextTokenStart = reader.Point;
    }

    void SkipComment()
    {
        while (IsCharAnd(reader.Read(), c => c != '\n')) { }
    }

    Token ParseNextToken()
    {
        int firstInt = reader.Peek();

        if (firstInt < 0)
            return new(TokenType.Eof);

        char first = (char)firstInt;

        if (IsIdentStartChar(first))
            return ParseIdentOrKeyword();
        else if (char.IsAsciiDigit(first))
            return ParseIntLiteral();
        else if (first == '"')
            return ParseStringLiteral();
        else
            return ParsePunctuation();
    }

    Token ParseIdentOrKeyword()
    {
        StringBuilder builder = new();

        while (IsCharAnd(reader.Peek(), IsIdentChar))
            builder.Append((char)reader.Read());

        string text = builder.ToString();

        if (keywords.TryGetValue(text, out var tokenType))
            return new(tokenType);

        return new(TokenType.Ident, text);
    }

    Token ParseIntLiteral()
    {
        Int128 value = 0;
        Point start = reader.Point;

        bool hasOverflow = false;
        bool hasBadCharacters = false;

        while (true)
        {
            int c = reader.Peek();

            if (IsCharAnd(c, char.IsAsciiDigit))
            {
                int digit = c - '0';

                value *= 10;
                value += digit;

                if (value > long.MaxValue)
                    hasOverflow = true;
            }
            else if (IsCharAnd(c, char.IsLetterOrDigit))
            {
                hasBadCharacters = true;
            }
            else if (c != '_')
            {
                break;
            }

            reader.Read();
        }

        Point end = reader.Point;

        if (hasBadCharacters)
            errors.Add("Integer literal cannot contain letters.", new(start, end));

        if (hasOverflow)
            errors.Add("Literal too large to fit in 64-bit signed integer.", new(start, end));

        if (hasBadCharacters || hasOverflow)
            value = 0;

        return new(TokenType.IntLiteral, (long)value);
    }

    Token ParseStringLiteral()
    {
        StringBuilder builder = new();

        Point start = reader.Point;
        reader.Read();

        char ReadOrError()
        {
            int c = reader.Read();

            if (c < 0)
            {
                errors.Add("String literal isn't terminated until end of file.", new(start, reader.Point));
                return '"';
            }

            return (char)c;
        }

        while (true)
        {
            Point escapeStart = reader.Point;
            char c = ReadOrError();

            if (c == '\\')
            {
                char escape = ReadOrError();

                if (stringEscapes.TryGetValue(escape, out char result))
                {
                    builder.Append(result);
                }
                else if (escape == 'u')
                {
                    StringBuilder charCode = new();

                    for (int i = 0; i < 4; i++)
                    {
                        if (IsCharAnd(reader.Peek(), char.IsAsciiHexDigit))
                            charCode.Append((char)reader.Read());
                        else
                            errors.Add("A \\u escape sequence must be followed by 4 hexadecimal characters.", new(escapeStart, reader.Point));
                    }

                    int parsed = int.Parse(charCode.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    builder.Append((char)parsed);
                }
                else
                {
                    errors.Add("Invalid escape type.", new(escapeStart, reader.Point));
                }
            }
            else if (c == '"')
            {
                break;
            }
            else
            {
                builder.Append(c);
            }
        }

        return new(TokenType.StringLiteral, builder.ToString());
    }

    Token ParsePunctuation()
    {
        Point start = reader.Point;
        char first = (char)reader.Read();

        int secondInt = reader.Peek();
        if (secondInt >= 0)
        {
            char second = (char)secondInt;

            if (twoCharPuncts.TryGetValue((first, second), out var tokenTypeTwoChar))
            {
                reader.Read();
                return new(tokenTypeTwoChar);
            }
        }

        if (oneCharPuncts.TryGetValue(first, out var tokenTypeOneChar))
            return new(tokenTypeOneChar);

        Point end = reader.Point;

        errors.Add("Unknown character.", new(start, end));
        return new(TokenType.Error);
    }

    static bool IsCharAnd(int c, Func<char, bool> func) => c >= 0 && func((char)c);
    static bool IsIdentStartChar(char c) => char.IsLetter(c) || c == '_';
    static bool IsIdentChar(char c) => char.IsLetterOrDigit(c) || c == '_';

    static readonly Dictionary<string, TokenType> keywords = new()
    {
        ["func"] = TokenType.Func,
        ["struct"] = TokenType.Struct,

        ["if"] = TokenType.If,
        ["else"] = TokenType.Else,
        ["while"] = TokenType.While,
        ["let"] = TokenType.Let,

        ["int"] = TokenType.Int,
        ["bool"] = TokenType.Bool,
        ["unit"] = TokenType.Unit,
        ["string"] = TokenType.String,

        ["true"] = TokenType.True,
        ["false"] = TokenType.False,
        ["null"] = TokenType.Null,
    };

    static readonly Dictionary<char, TokenType> oneCharPuncts = new()
    {
        ['('] = TokenType.LeftParen,
        [')'] = TokenType.RightParen,
        ['{'] = TokenType.LeftBrace,
        ['}'] = TokenType.RightBrace,
        [','] = TokenType.Comma,
        [':'] = TokenType.Colon,
        [';'] = TokenType.Semi,
        ['.'] = TokenType.Dot,

        ['='] = TokenType.Equal,
        ['<'] = TokenType.Less,
        ['>'] = TokenType.Greater,

        ['+'] = TokenType.Plus,
        ['-'] = TokenType.Minus,
        ['*'] = TokenType.Star,
        ['/'] = TokenType.Slash,
        ['%'] = TokenType.Percent,
        ['!'] = TokenType.Bang,
        ['|'] = TokenType.Or,
        ['&'] = TokenType.And,
    };

    static readonly Dictionary<(char, char), TokenType> twoCharPuncts = new()
    {
        [('=', '=')] = TokenType.DoubleEqual,
        [('!', '=')] = TokenType.NotEqual,
        [('<', '=')] = TokenType.LessEqual,
        [('>', '=')] = TokenType.GreaterEqual,

        [('+', '=')] = TokenType.PlusEqual,
        [('-', '=')] = TokenType.MinusEqual,
        [('*', '=')] = TokenType.StarEqual,
        [('/', '=')] = TokenType.SlashEqual,
        [('%', '=')] = TokenType.PercentEqual,
        [('&', '=')] = TokenType.AndEqual,
        [('|', '=')] = TokenType.OrEqual,
    };

    static readonly Dictionary<char, char> stringEscapes = new()
    {
        ['"'] = '"',
        ['\\'] = '\\',
        ['/'] = '/',
        ['b'] = '\b',
        ['f'] = '\f',
        ['n'] = '\n',
        ['r'] = '\r',
        ['t'] = '\t',
    };
}
