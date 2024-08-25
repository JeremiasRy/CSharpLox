namespace CSharpLox.Src;
public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<ThankYou>
{
    bool _repl = false;
    Environment _environment = new();
    public void SetToReplSession() => _repl = true;
    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (Stmt stmt in statements)
            {
                Execute(stmt);
            }
        }
        catch (RuntimeError e)
        {
            Lox.RuntimeError(e);
        }
    }
    public object? VisitBinaryExpr(Binary expr)
    {
        object left = Evaluate(expr.Left);
        object right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.MINUS:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    double result = (double)left - (double)right;
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }
            case TokenType.SLASH:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    if (right is double r && r == 0)
                    {
                        throw new RuntimeError(expr.Op, "Attempted to divide by zero.");
                    }
                    double result = (double)left / (double)right;
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }
            case TokenType.STAR:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    double result = (double)left * (double)right;
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }
            case TokenType.PLUS:
                {
                    if (left is double ld && right is double rd)
                    {
                        double result = ld + rd;
                        if (_repl)
                        {
                            Console.WriteLine(result);
                        }
                        return result;
                    }
                    if (left is string ls && right is string rs)
                    {
                        string result = ls + rs;
                        if (_repl)
                        {
                            Console.WriteLine(result);
                        }
                        return result;
                    }
                    if (left is string v && right is not string)
                    {
                        string result = v + right.ToString();
                        if (_repl)
                        {
                            Console.WriteLine(result);
                        }
                        return result;
                    }
                    if (left is not string && right is string rv)
                    {
                        string result = left.ToString() + rv;
                        if (_repl)
                        {
                            Console.WriteLine(result);
                        }
                        return result;
                    }
                    throw new RuntimeError(expr.Op, "Can't perform operation");
                }
            case TokenType.GREATER:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    bool result = (double)left > (double)right;
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }
            case TokenType.GREATER_EQUAL:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    bool result = (double)left >= (double)right;
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }
            case TokenType.LESS:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    bool result = (double)left < (double)right;
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }
            case TokenType.LESS_EQUAL:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    bool result = (double)left <= (double)right;
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }
            case TokenType.EQUAL_EQUAL:
                {
                    bool result = IsEqual(left, right);
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return !IsEqual(left, right);
                }

            case TokenType.BANG_EQUAL:
                {
                    bool result = !IsEqual(left, right);
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }
        };
        return null;
    }
    public object VisitGroupingExpr(Grouping expr)
    {
        return Evaluate(expr.Expr);
    }
    public object? VisitLiteralExpr(Literal expr)
    {
        return expr.Val;
    }
    public object VisitTernaryExpr(Ternary expr)
    {
        object obj = expr.Condition.Accept(this);

        if (IsTruthy(obj))
        {
            return expr.IfTrue.Accept(this);
        }
        else
        {
            return expr.IfFalse.Accept(this);
        }
    }
    public object? VisitUnaryExpr(Unary expr)
    {
        object right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.MINUS:
                {
                    CheckNumberOperand(expr.Op, right);

                    double result = -(double)right;
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }
            case TokenType.BANG:
                {
                    bool result = !IsTruthy(right);
                    if (_repl)
                    {
                        Console.WriteLine(result);
                    }
                    return result;
                }

        }
        return null;
    }
    static private bool IsTruthy(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is bool v)
        {
            return v;
        }
        return true;
    }
    private object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }
    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }
    private void ExecuteBlockStatement(List<Stmt> statements, Environment environment)
    {
        Environment prev = _environment;
        try
        {
            _environment = environment;
            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = prev;
        }
    }

    private static bool IsEqual(object a, object b)
    {
        if (a == null && b == null)
        {
            return true;
        }
        if (a == null)
        {
            return false;
        }

        return a.Equals(b);
    }
    private static string Stringify(object obj)
    {
        if (obj == null)
        {
            return "nil";
        }
        if (obj is double num)
        {
            var text = num.ToString();
            if (text.EndsWith(".0"))
            {
                text = text[0..(text.Length - 2)];
            }
            return text;
        }
        return obj.ToString() ?? "nil";
    }
    private static void CheckNumberOperand(Token op, object operand)
    {
        if (operand is double)
        {
            return;
        }
        throw new RuntimeError(op, "Operand must be a number");
    }
    private static void CheckNumberOperands(Token op, object a, object b)
    {
        if (a is double && b is double)
        {
            return;
        }
        throw new RuntimeError(op, "Operands must be numbers.");
    }
    public ThankYou? VisitExprStmtStmt(ExprStmt stmt)
    {
        object result = Evaluate(stmt.Expression);
        if (_repl)
        {
            Console.WriteLine(Stringify(result));
        }
        return ThankYou.Bye;
    }
    public ThankYou? VisitPrintStmt(Print stmt)
    {
        object value = Evaluate(stmt.Expression);
        Console.WriteLine(Stringify(value));
        return ThankYou.Bye;
    }

    public ThankYou? VisitVarStmt(Var stmt)
    {
        object? value = null;
        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }

        _environment.Define(stmt.Name, value);
        if (_repl)
        {
            Console.WriteLine(value);
        }
        return ThankYou.Bye;
    }

    public object? VisitVariableExpr(Variable expr)
    {
        return _environment.Get(expr.Name);
    }

    public object? VisitAssignExpr(Assign expr)
    {
        object? value = Evaluate(expr.Value);
        _environment.Define(expr.Name, value);
        if (_repl)
        {
            Console.WriteLine(value);
        }
        return ThankYou.Bye;
    }

    public ThankYou? VisitBlockStmt(Block stmt)
    {
        ExecuteBlockStatement(stmt.Statements, new Environment(_environment));
        return ThankYou.Bye;
    }
}