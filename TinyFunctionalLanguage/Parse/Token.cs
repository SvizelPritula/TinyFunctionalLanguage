namespace TinyFunctionalLanguage.Parse;

record struct Token(TokenType Type, object? Content = null);

enum TokenType
{
    Ident,
    IntLiteral,
    StringLiteral,

    If,
    Else,
    While,
    Func,
    Struct,
    Let,
    Int,
    Bool,
    Unit,
    String,
    True,
    False,
    Null,

    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,

    Comma,
    Colon,
    Semi,
    Dot,

    Equal,
    DoubleEqual,
    NotEqual,
    Less,
    Greater,
    LessEqual,
    GreaterEqual,

    Plus,
    Minus,
    Star,
    Slash,
    Percent,
    Bang,
    Or,
    And,

    PlusEqual,
    MinusEqual,
    StarEqual,
    SlashEqual,
    PercentEqual,
    OrEqual,
    AndEqual,

    Eof,
    Error
}

static class TokenExt
{
    public static string Name(this TokenType token) => token switch
    {
        TokenType.Ident => "identifier",
        TokenType.IntLiteral => "int literal",
        TokenType.StringLiteral => "string literal",
        TokenType.If => "if",
        TokenType.Else => "else",
        TokenType.While => "while",
        TokenType.Func => "func",
        TokenType.Struct => "struct",
        TokenType.Let => "let",
        TokenType.Int => "int",
        TokenType.Bool => "bool",
        TokenType.Unit => "unit",
        TokenType.String => "string",
        TokenType.True => "true",
        TokenType.False => "false",
        TokenType.Null => "null",
        TokenType.LeftParen => "'('",
        TokenType.RightParen => "')'",
        TokenType.LeftBrace => "'{'",
        TokenType.RightBrace => "'}'",
        TokenType.Comma => "','",
        TokenType.Colon => "':'",
        TokenType.Semi => "';'",
        TokenType.Dot => "'.'",
        TokenType.Equal => "'='",
        TokenType.DoubleEqual => "'=='",
        TokenType.NotEqual => "'!='",
        TokenType.Less => "'<'",
        TokenType.Greater => "'>'",
        TokenType.LessEqual => "'<='",
        TokenType.GreaterEqual => "'>='",
        TokenType.Plus => "'+'",
        TokenType.Minus => "'-'",
        TokenType.Star => "'*'",
        TokenType.Slash => "'/'",
        TokenType.Percent => "'%'",
        TokenType.Bang => "'!'",
        TokenType.Or => "'|'",
        TokenType.And => "'&'",
        TokenType.PlusEqual => "'+='",
        TokenType.MinusEqual => "'-='",
        TokenType.StarEqual => "'*='",
        TokenType.SlashEqual => "'/='",
        TokenType.PercentEqual => "'%='",
        TokenType.OrEqual => "'|='",
        TokenType.AndEqual => "'&='",
        TokenType.Eof => "end of file",
        TokenType.Error => "invalid token",
        _ => throw new InvalidOperationException("An invalid token type doesn't have a name.")
    };
}
