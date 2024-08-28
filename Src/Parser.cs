namespace CSharpLox.Src;
public class Parser(List<Token> tokens)
{
    private class ParseError : Exception { }
    readonly List<Token> _tokens = tokens;
    int _current;
    bool _loop = false;
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
        if (Match(TokenType.FUN))
        {
            return Function("function");
        }
        if (Match(TokenType.BREAK))
        {
            Token token = Previous();
            Consume(TokenType.SEMICOLON, "Expect ';' after statement");
            if (!_loop)
            {
                Error(token, "Can't use 'break' outside of loop");
            }
            return new Break(token);
        }
        if (Match(TokenType.CONTINUE))
        {
            Token token = Previous();
            Consume(TokenType.SEMICOLON, "Expect ';' after statement");
            if (!_loop)
            {
                Error(token, "Can't use 'continue' outside of a loop");
            }
            return new Continue(token);
        }
        if (Match(TokenType.PRINT))
        {
            return PrintStatement();
        }
        if (Match(TokenType.FOR))
        {
            return ForStatement();
        }
        if (Match(TokenType.WHILE))
        {
            return WhileStatement();
        }
        if (Match(TokenType.LEFT_BRACE))
        {
            return new Block(BlockScope());
        }
        if (Match(TokenType.IF))
        {
            return IfStatement();
        }

        return ExpressionStatement();
    }

    private FunctionStmt Function(string kind)
    {
        Token name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name");
        Consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name");
        List<Token> parameters = [];
        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 parameters.");
                }
                parameters.Add(Consume(TokenType.IDENTIFIER, "Expectt parameter name"));

            } while (Match(TokenType.COMMA));
        }
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");
        Consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body");
        List<Stmt> body = BlockScope();
        return new FunctionStmt(name, parameters, body);
    }

    private While WhileStatement()
    {
        _loop = true;
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after while condition");

        Stmt body = Statement();
        _loop = false;
        return new While(condition, body);
    }
    private Stmt ForStatement()
    {
        _loop = true;
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'");
        Stmt? initializer;
        if (Match(TokenType.SEMICOLON))
        {
            initializer = null;
        }
        else if (Match(TokenType.VAR))
        {
            initializer = VarDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }

        Expr? condition = null;
        if (!Check(TokenType.SEMICOLON))
        {
            condition = Expression();
        }
        Consume(TokenType.SEMICOLON, "Expect ';' after loop condition");
        Expr? increment = null;
        if (!Check(TokenType.RIGHT_PAREN))
        {
            increment = Expression();
        }
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses");

        Stmt body = Statement();

        if (increment != null)
        {
            body = new Block([body, new ExprStmt(increment)]);
        }

        condition ??= new Literal(true);

        body = new While(condition, body);

        if (initializer != null)
        {
            body = new Block([initializer, body]);
        }
        _loop = false;
        return body;
    }

    private If IfStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'");
        Expr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition");

        Stmt thenBranch = Statement();
        Stmt? elseBranch = null;
        if (Match(TokenType.ELSE))
        {
            elseBranch = Statement();
        }
        return new If(condition, thenBranch, elseBranch);
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
        Expr expr = Or();

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

    Expr Or()
    {
        Expr expr = And();
        if (Match(TokenType.OR))
        {
            Token op = Previous();
            Expr right = And();
            expr = new Logical(expr, op, right);
        }
        return expr;
    }

    Expr And()
    {
        Expr expr = Ternary();
        if (Match(TokenType.AND))
        {
            Token op = Previous();
            Expr right = Ternary();
            expr = new Logical(expr, op, right);
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
        return Call();
    }

    Expr Call()
    {
        Expr expr = Primary();

        while (true)
        {
            if (Match(TokenType.LEFT_PAREN))
            {
                expr = FinishCall(expr);
            }
            else
            {
                break;
            }
        }
        return expr;
    }

    private Call FinishCall(Expr callee)
    {
        List<Expr> arguments = [];
        if (!Check(TokenType.RIGHT_PAREN))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 arguments");
                }
                arguments.Add(Expression());
            } while (Match(TokenType.COMMA));
        }
        Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");
        return new Call(callee, paren, arguments);
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

