namespace CSharpLox.Src;
public interface IVisitor<R>
{
  R VisitBinaryExpr(Binary expr);
  R VisitGroupingExpr(Grouping expr);
  R VisitLiteralExpr(Literal expr);
  R VisitUnaryExpr(Unary expr);
}
public abstract class Expr 
{
  public abstract R Accept<R>(IVisitor<R> visitor);
}

public class Binary(Expr left, Token op, Expr right) : Expr
{
  public readonly Expr Left = left;
  public readonly Token Op = op;
  public readonly Expr Right = right;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitBinaryExpr(this);
  }
}
public class Grouping(Expr expr) : Expr
{
  public readonly Expr Expr = expr;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitGroupingExpr(this);
  }
}
public class Literal(object val) : Expr
{
  public readonly object Val = val;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitLiteralExpr(this);
  }
}
public class Unary(Token op, Expr right) : Expr
{
  public readonly Token Op = op;
  public readonly Expr Right = right;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitUnaryExpr(this);
  }
}
