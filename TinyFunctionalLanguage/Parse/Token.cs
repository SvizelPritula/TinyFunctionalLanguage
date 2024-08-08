namespace TinyFunctionalLanguage.Parse;

public record struct Token(TokenType Type, object? Content = null);

public enum TokenType
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
    Not,
    Or,
    And,

    Eof,
}
