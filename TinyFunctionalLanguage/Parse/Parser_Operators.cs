using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.Parse;

public partial class Parser
{
    IExpression ParseExpression() => ParseOrExpression();

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

    IExpression ParseUnaryOperator()
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

            IExpression inner = ParseUnaryOperator();
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
        ParseUnaryOperator,
        t => t switch
        {
            TokenType.Star => BinaryOperator.Star,
            TokenType.Slash => BinaryOperator.Slash,
            TokenType.Percent => BinaryOperator.Percent,
            _ => null
        }
    );
}
