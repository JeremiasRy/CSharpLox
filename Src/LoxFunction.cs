
namespace CSharpLox.Src;

public class LoxFunction(FunctionStmt declaration, Environment closure, bool isInitializer) : ILoxCallable
{
    readonly FunctionStmt _declaration = declaration;
    readonly Environment _closure = closure;
    readonly bool _isInitializer = isInitializer;
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
            if (_isInitializer)
            {
                return _closure.GetAt(0, "this");
            }
            return returnValue.Value;
        }
        if (_isInitializer)
        {
            return _closure.GetAt(0, "this");
        }
        return null;
    }
    public override string ToString()
    {
        return $"<fn {_declaration.Name.Lexeme}>";
    }

    internal object Bind(LoxInstance loxInstance)
    {
        Environment environment = new(_closure);
        environment.Define("this", loxInstance);
        return new LoxFunction(_declaration, environment, _isInitializer);
    }
}