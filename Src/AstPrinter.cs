using System.Text;
namespace CSharpLox.Src;

public class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string? VisitAssignExpr(Assign expr)
    {
        throw new NotImplementedException();
    }

    public string VisitBinaryExpr(Binary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
    }

    public string? VisitCallExpr(Call expr)
    {
        throw new NotImplementedException();
    }

    public string VisitGroupingExpr(Grouping expr)
    {
        return Parenthesize("group", expr.Expr);
    }

    public string VisitLiteralExpr(Literal expr)
    {
        if (expr.Val == null)
        {
            return "nil";
        }
        return expr.Val.ToString() ?? "nil";
    }

    public string? VisitLogicalExpr(Logical expr)
    {
        throw new NotImplementedException();
    }

    public string VisitTernaryExpr(Ternary expr)
    {
        return Parenthesize("ternary", expr.Condition, expr.IfTrue, expr.IfFalse);
    }

    public string VisitUnaryExpr(Unary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    public string? VisitVariableExpr(Variable expr)
    {
        throw new NotImplementedException();
    }

    string Parenthesize(string name, params Expr[] exprs)
    {
        StringBuilder builder = new();

        builder.Append('(').Append(name);
        foreach (Expr expr in exprs)
        {
            builder.Append(' ');
            builder.Append(expr.Accept(this));
        }
        builder.Append(')');
        return builder.ToString();
    }
}