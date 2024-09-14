namespace CSharpLox.Src;

public class LoxClass : ILoxCallable
{
    readonly public string Name;
    readonly Dictionary<string, LoxFunction> _methods = [];
    readonly LoxClass? _superclass;

    public int Arity()
    {
        LoxFunction? initializer = (LoxFunction?)FindMethod("init");
        if (initializer != null)
        {
            return initializer.Arity();
        }

        return 0;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        LoxInstance instance = new(this);
        object? initializer = FindMethod("init");
        if (initializer is LoxFunction init)
        {
            ((LoxFunction)init.Bind(instance)).Call(interpreter, arguments);
        }
        return instance;
    }

    public override string ToString()
    {
        return Name;
    }
    public LoxClass(string name)
    {
        Name = name;
    }
    public LoxClass(string name, Dictionary<string, LoxFunction> methods, LoxClass? superclass = null)
    {
        Name = name;
        _methods = methods;
        _superclass = superclass;
    }

    internal object? FindMethod(string lexeme)
    {
        if (_methods.TryGetValue(lexeme, out var method))
        {
            return method;
        }
        if (_superclass is not null)
        {
            return _superclass.FindMethod(lexeme);
        }
        return null;
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

        LoxFunction? method = (LoxFunction?)_klass.FindMethod(name.Lexeme);
        if (method is not null)
        {
            return method.Bind(this);
        }
        throw new RuntimeError(name, "Undefined property '" + name.Lexeme + "'.");
    }

    internal void Set(Token name, object value)
    {
        _fields.Add(name.Lexeme, value);
    }
}