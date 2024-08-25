namespace CSharpLox.Src;
public class Parser(List<Token> tokens)
{
    private class ParseError : Exception { }
    readonly List<Token> _tokens = tokens;
    int _current;
    // ### Statements
    public List<Stmt> Parse()
    {
        List<Stmt> statements = [];
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }
    private Stmt Declaration()
    {
        try
        {
            if (Match(TokenType.VAR))
            {
                return VarDeclaration();
            }
            return Statement();
        }
        catch
        {
            Synchronize();
            return null;
        }
    }

    private Var VarDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

        Expr? initializer = null;
        if (Match(TokenType.EQUAL))
        {
            initializer = Comma();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
        return new Var(name, initializer);
    }

    private Stmt Statement()
    {
        if (Match(TokenType.PRINT))
        {
            return PrintStatement();
        }
        if (Match(TokenType.LEFT_BRACE))
        {
            return new Block(BlockScope());
        }

        return ExpressionStatement();
    }

    List<Stmt> BlockScope()
    {
        List<Stmt> statements = [];

        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }
        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return statements;
    }

    private ExprStmt ExpressionStatement()
    {
        Expr expr = Comma();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new ExprStmt(expr);
    }

    private Print PrintStatement()
    {
        Expr expr = Comma();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Print(expr);
    }

    // ### Expressions
    Expr Comma()
    {
        Expr expr = Expression();
        while (Match(TokenType.COMMA))
        {
            Token op = Previous();
            Expr right = Expression();
            expr = new Binary(expr, op, right);
        }
        return expr;
    }

    Expr Expression()
    {
        return Assignment();
    }

    private Expr Assignment()
    {
        Expr expr = Ternary();

        if (Match(TokenType.EQUAL))
        {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr is Variable varExpr)
            {
                Token name = varExpr.Name;
                return new Assign(name, value);
            }
            Error(equals, "Invalid assignment target.");
        }
        return expr;
    }

    Expr Ternary()
    {
        Expr expr = Equality();
        if (Match(TokenType.QUESTION_MARK))
        {
            Expr exprIfTrue = Expression();
            Consume(TokenType.COLON, "Expect ':' after expression ternary operator");
            Expr exprIfFalse = Expression();
            expr = new Ternary(expr, exprIfTrue, exprIfFalse);
        }
        return expr;
    }

    Expr Equality()
    {
        Expr expr = Comparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token op = Previous();
            Expr right = Comparison();
            expr = new Binary(expr, op, right);
        }
        return expr;
    }
    Expr Comparison()
    {
        Expr expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = Previous();
            Expr right = Term();
            expr = new Binary(expr, op, right);
        }
        return expr;
    }

    Expr Term()
    {
        Expr expr = Factor();
        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = Previous();
            Expr right = Factor();
            expr = new Binary(expr, op, right);
        }
        return expr;
    }

    Expr Factor()
    {
        Expr expr = GetUnary();
        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            Token op = Previous();
            Expr right = GetUnary();
            expr = new Binary(expr, op, right);
        }
        return expr;
    }

    Expr GetUnary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            Token op = Previous();
            Expr right = GetUnary();
            return new Unary(op, right);
        }
        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE))
        {
            return new Literal(false);
        }
        if (Match(TokenType.TRUE))
        {
            return new Literal(true);
        }
        if (Match(TokenType.NIL))
        {
            return new Literal(null);
        }
        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Literal(Previous().Literal);
        }
        if (Match(TokenType.IDENTIFIER))
        {
            return new Variable(Previous());
        }
        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
            return new Grouping(expr);
        }
        throw Error(Peek(), "Expect expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }
        throw Error(Peek(), message);
    }

    void Synchronize()
    {
        Advance();
        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON)
            {
                return;
            }

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FOR:
                case TokenType.FUN:
                case TokenType.IF:
                case TokenType.PRINT:
                case TokenType.RETURN:
                case TokenType.VAR:
                case TokenType.WHILE:
                    return;
            }

            Advance();
        }
    }
    static ParseError Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParseError();
    }

    bool Check(TokenType token)
    {
        if (IsAtEnd())
        {
            return false;
        }

        return Peek().Type == token;
    }

    bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }
        return Previous();
    }

    Token Peek()
    {
        return _tokens.ElementAt(_current);
    }

    Token Previous()
    {
        return _tokens.ElementAt(_current - 1);
    }

    bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }
}

