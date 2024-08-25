namespace CSharpLox.Src;
public abstract class Expr
{
  public interface IVisitor<R>
  {
    abstract R? VisitBinaryExpr(Binary expr);
    abstract R? VisitGroupingExpr(Grouping expr);
    abstract R? VisitLiteralExpr(Literal expr);
    abstract R? VisitUnaryExpr(Unary expr);
    abstract R? VisitTernaryExpr(Ternary expr);
  }
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
public class Literal(object? val) : Expr
{
  public readonly object? Val = val;

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
public class Ternary(Expr condition, Expr ifTrue, Expr ifFalse) : Expr
{
  public readonly Expr Condition = condition;
  public readonly Expr IfTrue = ifTrue;
  public readonly Expr IfFalse = ifFalse;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitTernaryExpr(this);
  }
}
