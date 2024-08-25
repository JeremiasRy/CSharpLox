using System.Text;
namespace CSharpLox.Src;

public class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }
    public string VisitBinaryExpr(Binary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
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

    public string VisitTernaryExpr(Ternary expr)
    {
        return Parenthesize("ternary", expr.Condition, expr.IfTrue, expr.IfFalse);
    }

    public string VisitUnaryExpr(Unary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Right);
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