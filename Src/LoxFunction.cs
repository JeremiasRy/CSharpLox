
namespace CSharpLox.Src;

public class LoxFunction(FunctionStmt declaration, Environment closure) : ILoxCallable
{
    readonly FunctionStmt _declaration = declaration;
    readonly Environment _closure = closure;
    public int Arity()
    {
        return _declaration.Prms.Count;
    }

    public object? Call(Interpreter interpreter, List<object> arguments)
    {
        var environment = new Environment(_closure);
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