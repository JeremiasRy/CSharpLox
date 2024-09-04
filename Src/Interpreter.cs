using System.Diagnostics;

namespace CSharpLox.Src;
public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<ThankYou>
{
    bool _repl = false;
    Environment _environment;
    public Environment globals;
    readonly Dictionary<Expr, int> _locals = [];
    public Interpreter()
    {
        globals = new Environment();
        globals.Define("clock", new Clock());
        _environment = globals;
    }
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
                    return (double)left - (double)right;
                }
            case TokenType.SLASH:
                {
                    CheckNumberOperands(expr.Op, left, right);
                    if (right is double r && r == 0)
                    {
                        throw new RuntimeError(expr.Op, "Attempted to divide by zero.");
                    }
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
                    if (left is string v && right is not string)
                    {
                        return v + right.ToString();
                    }
                    if (left is not string && right is string rv)
                    {
                        return left.ToString() + rv;
                    }
                    throw new RuntimeError(expr.Op, "Can't perform operation");
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
                {
                    return IsEqual(left, right);
                }

            case TokenType.BANG_EQUAL:
                {
                    return !IsEqual(left, right);
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

                    return -(double)right;
                }
            case TokenType.BANG:
                {
                    return !IsTruthy(right);
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

    public void ExecuteBlockStatement(List<Stmt> statements, Environment environment)
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

        _environment.Define(stmt.Name.Lexeme, value);
        if (_repl)
        {
            Console.WriteLine(value);
        }
        return ThankYou.Bye;
    }

    public object? VisitVariableExpr(Variable expr)
    {
        return LookupVariable(expr.Name, expr);
    }

    private object? LookupVariable(Token name, Variable expr)
    {
        if (_locals.TryGetValue(expr, out int distance))
        {
            return _environment.GetAt(distance, name.Lexeme);
        }
        return _environment.Get(name);
    }

    public object? VisitAssignExpr(Assign expr)
    {
        object? value = Evaluate(expr.Value);
        if (_locals.TryGetValue(expr, out int distance))
        {
            _environment.AssignAt(distance, expr.Name, value);
        }
        else
        {
            _environment.Assign(expr.Name, value);
        }

        if (_repl)
        {
            Console.WriteLine(value);
        }
        return value;
    }

    public ThankYou? VisitBlockStmt(Block stmt)
    {
        ExecuteBlockStatement(stmt.Statements, new Environment(_environment));
        return ThankYou.Bye;
    }

    public ThankYou? VisitIfStmt(If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch != null)
        {
            Execute(stmt.ElseBranch);
        }
        return ThankYou.Bye;
    }

    public object? VisitLogicalExpr(Logical expr)
    {
        object left = Evaluate(expr.Left);
        if (expr.Op.Type == TokenType.OR)
        {
            if (IsTruthy(left))
            {
                return left;
            }
        }
        else
        {
            if (!IsTruthy(left))
            {
                return left;
            }
        }
        return Evaluate(expr.Right);
    }

    public ThankYou? VisitWhileStmt(While stmt)
    {
        object condition = Evaluate(stmt.Condition);
        while (IsTruthy(condition))
        {
            try
            {
                Execute(stmt.Body);
            }
            catch (BreakException)
            {
                break;
            }
            condition = Evaluate(stmt.Condition);
        }
        return ThankYou.Bye;
    }

    public ThankYou? VisitBreakStmt(Break stmt)
    {
        throw new BreakException();
    }

    public ThankYou? VisitContinueStmt(Continue stmt)
    {
        // throw new ContinueException();
        Console.WriteLine("Hi it's the intepreter, we have a continue statement lying around");
        return ThankYou.Bye;
    }

    public object? VisitCallExpr(Call expr)
    {
        object callee = Evaluate(expr.Callee);
        List<object> arguments = new(expr.Arguments.Count);
        foreach (Expr arg in expr.Arguments)
        {
            arguments.Add(Evaluate(arg));
        }
        if (callee is not ILoxCallable)
        {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes");
        }

        ILoxCallable function = (ILoxCallable)callee;
        if (arguments.Count != function.Arity())
        {
            throw new RuntimeError(expr.Paren, $"Expected ${function.Arity()} arguments but got ${arguments.Count}");
        }
        return function.Call(this, arguments);
    }

    public ThankYou? VisitFunctionStmtStmt(FunctionStmt stmt)
    {
        LoxFunction function = new(stmt);
        _environment.Define(stmt.Name.Lexeme, function);
        return ThankYou.Bye;
    }
    public ThankYou? VisitReturnStmtStmt(ReturnStmt stmt)
    {
        object? value = null;
        if (stmt.Value != null)
        {
            value = Evaluate(stmt.Value);
        }
        throw new Return(value);
    }

    public void Resolve(Expr expr, int depth)
    {
        _locals.Add(expr, depth);
    }
}