


namespace CSharpLox.Src;

public class LoxClass(string name) : ILoxCallable
{
    readonly public string Name = name;

    public int Arity()
    {
        return 0;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        LoxInstance instance = new(this);
        return instance;
    }

    public override string ToString()
    {
        return Name;
    }
}

public class LoxInstance(LoxClass klass)
{
    readonly LoxClass _klass = klass;
    readonly Dictionary<string, object> _fields = [];
    public override string ToString()
    {
        return $"{_klass.Name} instance";
    }

    internal object? Get(Token name)
    {
        if (_fields.TryGetValue(name.Lexeme, out object obj))
        {
            return obj;
        }
        throw new RuntimeError(name, "Undefined property '" + name.Lexeme + "'.");
    }

    internal void Set(Token name, object value)
    {
        _fields.Add(name.Lexeme, value);
    }
}