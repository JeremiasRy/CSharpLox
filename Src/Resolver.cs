
using System.Xml;

namespace CSharpLox.Src;
public class Resolver(Interpreter interpreter) : Expr.IVisitor<ThankYou>, Stmt.IVisitor<ThankYou>
{
    readonly Interpreter _interpreter = interpreter;
    readonly Stack<Dictionary<string, bool>> _scopes = [];
    public ThankYou? VisitAssignExpr(Assign expr)
    {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return ThankYou.Bye;
    }

    public ThankYou? VisitBinaryExpr(Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return ThankYou.Bye;
    }

    public ThankYou? VisitBlockStmt(Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return ThankYou.Bye;
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    public void Resolve(List<Stmt> statements)
    {
        foreach (Stmt stmt in statements)
        {
            Resolve(stmt);
        }
    }

    private void Resolve(Stmt stmt)
    {
        stmt.Accept(this);
    }

    private void Resolve(Expr expr)
    {
        expr.Accept(this);
    }

    private void BeginScope()
    {
        _scopes.Push([]);
    }

    public ThankYou? VisitBreakStmt(Break stmt)
    {
        return ThankYou.Bye;
    }

    public ThankYou? VisitCallExpr(Call expr)
    {
        Resolve(expr.Callee);
        foreach (var argument in expr.Arguments)
        {
            Resolve(argument);
        }
        return ThankYou.Bye;
    }

    public ThankYou? VisitContinueStmt(Continue stmt)
    {
        // Here would be the place to error if statement is used outside of loop
        return ThankYou.Bye;
    }

    public ThankYou? VisitExprStmtStmt(ExprStmt stmt)
    {
        Resolve(stmt.Expression);
        return ThankYou.Bye;
    }

    public ThankYou? VisitFunctionStmtStmt(FunctionStmt stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);
        ResolveFunction(stmt);
        return ThankYou.Bye;
    }

    private void ResolveFunction(FunctionStmt stmt)
    {
        BeginScope();
        foreach (var parameters in stmt.Prms)
        {
            Declare(parameters);
            Define(parameters);
        }
        Resolve(stmt.Body);
        EndScope();
    }

    public ThankYou? VisitGroupingExpr(Grouping expr)
    {
        Resolve(expr.Expr);
        return ThankYou.Bye;
    }

    public ThankYou? VisitIfStmt(If stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.ThenBranch);
        if (stmt.ElseBranch != null)
        {
            Resolve(stmt.ElseBranch);
        }
        return ThankYou.Bye;

    }

    public ThankYou? VisitLiteralExpr(Literal expr)
    {
        return ThankYou.Bye;
    }

    public ThankYou? VisitLogicalExpr(Logical expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return ThankYou.Bye;
    }

    public ThankYou? VisitPrintStmt(Print stmt)
    {
        Resolve(stmt.Expression);
        return ThankYou.Bye;
    }

    public ThankYou? VisitReturnStmtStmt(ReturnStmt stmt)
    {
        if (stmt.Value != null)
        {
            Resolve(stmt.Value);
        }
        return ThankYou.Bye;
    }

    public ThankYou? VisitTernaryExpr(Ternary expr)
    {
        Resolve(expr.Condition);
        Resolve(expr.IfFalse);
        Resolve(expr.IfTrue);
        return ThankYou.Bye;
    }

    public ThankYou? VisitUnaryExpr(Unary expr)
    {
        Resolve(expr.Right);
        return ThankYou.Bye;
    }

    public ThankYou? VisitVariableExpr(Variable expr)
    {
        if (_scopes.Count != 0)
        {
            if (_scopes.Peek().TryGetValue(expr.Name.Lexeme, out bool resolved) && !resolved)
            {
                Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
            }
        }
        ResolveLocal(expr, expr.Name);
        return ThankYou.Bye;
    }
    private void ResolveLocal(Expr expr, Token name)
    {
        for (int i = 0; i < _scopes.Count - 1; i++)
        {
            if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
            {
                _interpreter.Resolve(expr, _scopes.Count - 1 - i);
                return;
            }
        }
    }

    public ThankYou? VisitVarStmt(Var stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initializer != null)
        {
            Resolve(stmt.Initializer);
        }
        Define(stmt.Name);
        return ThankYou.Bye;
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0)
        {
            return;
        }
        _scopes.Peek()[name.Lexeme] = true;
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0)
        {
            return;
        }
        var scope = _scopes.Peek();

        if (scope.ContainsKey(name.Lexeme))
        {
            Lox.Error(name, "Already a variable with this name in this scope");
            return;
        }
        scope.Add(name.Lexeme, false);
    }

    public ThankYou? VisitWhileStmt(While stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return ThankYou.Bye;
    }
}