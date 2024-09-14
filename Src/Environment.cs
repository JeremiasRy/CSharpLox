namespace CSharpLox.Src;
public class Environment
{
    public readonly Environment? Enclosing;
    readonly Dictionary<string, object?> _values = [];
    public void Define(string name, object? value)
    {
        if (_values.TryGetValue(name, out object? initial) && initial == null)
        {
            _values[name] = value;
            return;
        }
        else if (initial != null)
        {
            Console.WriteLine("Some custom error on to not re-define a variable");
        }
        _values.Add(name, value);
    }
    public void Assign(Token name, object value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }
        if (Enclosing != null)
        {
            Enclosing.Assign(name, value);
            return;
        }
        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }
    public object? Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out object? value))
        {
            return value;
        }
        if (Enclosing != null)
        {
            return Enclosing.Get(name);
        }
        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    internal object? GetAt(int distance, string lexeme)
    {
        return Ancestor(distance)._values[lexeme];
    }

    private Environment? Ancestor(int distance)
    {
        var environment = this;
        for (int i = 0; i < distance; i++)
        {
            environment = environment.Enclosing;
        }
        return environment;
    }

    internal void AssignAt(int distance, Token name, object value)
    {
        Ancestor(distance)._values.Add(name.Lexeme, value);
    }

    public Environment()
    {
        Enclosing = null;
    }
    public Environment(Environment enclosing)
    {
        Enclosing = enclosing;
    }

}