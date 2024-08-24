namespace CSharpLox.Src;
public class Interpreter : IVisitor<object>
{
    public void Interpret(Expr expression)
    {
        try
        {
            object value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
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
                    return (double)left - (double)right;
                }
            case TokenType.SLASH:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left / (double)right;
                }
            case TokenType.STAR:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left * (double)right;
                }
            case TokenType.PLUS:
                {
                    if (left is double ld && right is double rd)
                    {
                        return ld + rd;
                    }
                    if (left is string ls && right is string rs)
                    {
                        return ls + rs;
                    }
                    throw new RuntimeError(expr.Op, "Opreands must be two numbers or two strings");
                }
            case TokenType.GREATER:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left > (double)right;
                }
            case TokenType.GREATER_EQUAL:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left >= (double)right;
                }
            case TokenType.LESS:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left < (double)right;
                }
            case TokenType.LESS_EQUAL:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left <= (double)right;
                }
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
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
        throw new NotImplementedException();
    }
    public object? VisitUnaryExpr(Unary expr)
    {
        object right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.MINUS:
                {
                    CheckNumberOperand(expr.Op, right);
                    return -(double)right;
                }
            case TokenType.BANG:
                return !IsTruthy(right);
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
}