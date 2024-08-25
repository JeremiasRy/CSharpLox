namespace CSharpLox.Src;
public class Environment
{
    readonly Environment? _enclosing;
    readonly Dictionary<string, object?> _values = [];
    public void Define(Token name, object? value)
    {
        _values.Add(name.Lexeme, value);
    }
    public void Assign(Token name, object value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }
        if (_enclosing != null)
        {
            _enclosing.Assign(name, value);
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
        if (_enclosing != null)
        {
            return _enclosing.Get(name);
        }
        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }
    public Environment()
    {
        _enclosing = null;
    }
    public Environment(Environment enclosing)
    {
        _enclosing = enclosing;
    }

}