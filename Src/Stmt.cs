namespace CSharpLox.Src;
public abstract class Stmt
{
  public interface IVisitor<R>
  {
    abstract R? VisitExprStmtStmt(ExprStmt stmt);
    abstract R? VisitPrintStmt(Print stmt);
  }
  public abstract R Accept<R>(IVisitor<R> visitor);
}

public class ExprStmt(Expr expression) : Stmt
{
  public readonly Expr Expression = expression;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitExprStmtStmt(this);
  }
}
public class Print(Expr expression) : Stmt
{
  public readonly Expr Expression = expression;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitPrintStmt(this);
  }
}
