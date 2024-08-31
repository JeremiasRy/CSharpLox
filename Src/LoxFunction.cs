
namespace CSharpLox.Src;

public class LoxFunction(FunctionStmt declaration) : ILoxCallable
{
    readonly FunctionStmt _declaration = declaration;
    public int Arity()
    {
        return _declaration.Prms.Count();
    }

    public object? Call(Interpreter interpreter, List<object> arguments)
    {
        var environment = new Environment(interpreter.globals);
        for (int i = 0; i < arguments.Count; i++)
        {
            environment.Define(_declaration.Prms.ElementAt(i).Lexeme, arguments.ElementAt(i));
        }
        try
        {
            interpreter.ExecuteBlockStatement(_declaration.Body, environment);
        }
        catch (Return returnValue)
        {
            return returnValue.Value;
        }
        return null;
    }
    public override string ToString()
    {
        return $"<fn {_declaration.Name.Lexeme}>";
    }
}