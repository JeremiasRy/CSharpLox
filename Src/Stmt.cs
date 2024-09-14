namespace CSharpLox.Src;
public abstract class Stmt 
{
  public interface IVisitor<R>
  {
    abstract R? VisitBlockStmt(Block stmt);
    abstract R? VisitClassStmt(Class stmt);
    abstract R? VisitExprStmtStmt(ExprStmt stmt);
    abstract R? VisitFunctionStmtStmt(FunctionStmt stmt);
    abstract R? VisitIfStmt(If stmt);
    abstract R? VisitPrintStmt(Print stmt);
    abstract R? VisitWhileStmt(While stmt);
    abstract R? VisitVarStmt(Var stmt);
    abstract R? VisitBreakStmt(Break stmt);
    abstract R? VisitContinueStmt(Continue stmt);
    abstract R? VisitReturnStmtStmt(ReturnStmt stmt);
  }
  public abstract R Accept<R>(IVisitor<R> visitor);
}

public class Block(List<Stmt> statements) : Stmt
{
  public readonly List<Stmt> Statements = statements;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitBlockStmt(this);
  }
}
public class Class(Token name, Variable superclass, List<FunctionStmt> methods) : Stmt
{
  public readonly Token Name = name;
  public readonly Variable Superclass = superclass;
  public readonly List<FunctionStmt> Methods = methods;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitClassStmt(this);
  }
}
public class ExprStmt(Expr expression) : Stmt
{
  public readonly Expr Expression = expression;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitExprStmtStmt(this);
  }
}
public class FunctionStmt(Token name, List<Token> prms, List<Stmt> body) : Stmt
{
  public readonly Token Name = name;
  public readonly List<Token> Prms = prms;
  public readonly List<Stmt> Body = body;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitFunctionStmtStmt(this);
  }
}
public class If(Expr condition, Stmt thenBranch, Stmt? elseBranch) : Stmt
{
  public readonly Expr Condition = condition;
  public readonly Stmt ThenBranch = thenBranch;
  public readonly Stmt? ElseBranch = elseBranch;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitIfStmt(this);
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
public class While(Expr condition, Stmt body) : Stmt
{
  public readonly Expr Condition = condition;
  public readonly Stmt Body = body;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitWhileStmt(this);
  }
}
public class Var(Token name, Expr? initializer) : Stmt
{
  public readonly Token Name = name;
  public readonly Expr? Initializer = initializer;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitVarStmt(this);
  }
}
public class Break(Token name) : Stmt
{
  public readonly Token Name = name;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitBreakStmt(this);
  }
}
public class Continue(Token name) : Stmt
{
  public readonly Token Name = name;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitContinueStmt(this);
  }
}
public class ReturnStmt(Token keyword, Expr? value) : Stmt
{
  public readonly Token Keyword = keyword;
  public readonly Expr? Value = value;

  public override R Accept<R>(IVisitor<R> Visitor)
  {
    return Visitor.VisitReturnStmtStmt(this);
  }
}
