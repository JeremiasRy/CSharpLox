namespace CSharpLox.Src;

public class Return(object? value) : Exception
{
    public object? Value = value;
}
