namespace CSharpLox.Src;
public class RuntimeError(Token token, string message) : Exception(message)
{
    public Token Token = token;
}