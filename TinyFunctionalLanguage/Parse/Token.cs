namespace TinyFunctionalLanguage.Parse;

public record struct Token(TokenType Type, object? LiteralValue = null);

public enum TokenType
{
    Ident,
    IntLiteral,

    If,
    While,
    Func,
    Struct,
    Let,
    Int,
    Bool,
    True,
    False,

    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,

    Comma,
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
    Not,
    Or,
    And,

    Eof,
}
